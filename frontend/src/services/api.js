import axios from 'axios';

const ACCESS_TOKEN_KEY = 'tf_access_token'
const REFRESH_TOKEN_KEY = 'tf_refresh_token'

const apiClient = axios.create({
    baseURL : import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api',
    headers: {
        'Content-Type': 'application/json'
    },
    timeout: 15000,
})

apiClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem(ACCESS_TOKEN_KEY)
        if (token) {
            config.headers.Authorization = `Bearer ${token}`
        }
        return config
    },
    (error) => {
        return Promise.reject(error)
    }
)

let isRefreshing = false
let failedQueue = []

function processQueue(error, token = null) {
  failedQueue.forEach(({ resolve, reject }) => {
    if (error) reject(error)
    else resolve(token)
  })
  failedQueue = []
}

apiClient.interceptors.response.use(
    (response) => {
        return response
    },
    async (error) => {
        const originalRequest = error.config;

        if (error.response && error.response.status === 401 && !originalRequest._retry) {
            if (originalRequest.url?.includes('/auth/refresh')) {
                localStorage.removeItem(ACCESS_TOKEN_KEY);
                localStorage.removeItem(REFRESH_TOKEN_KEY);
                window.location.href = '/login';
                return Promise.reject(error);
            }

            if (isRefreshing) {
                return new Promise((resolve, reject) => {
                    failedQueue.push({ resolve, reject });
                }).then((token) => {
                    originalRequest.headers.Authorization = `Bearer ${token}`;
                    return apiClient(originalRequest);
                })
            }

            originalRequest._retry = true;
            isRefreshing = true;

            try {
                const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);

                const { data } = await axios.post(
                    `${apiClient.defaults.baseURL}/auth/refresh`,
                    { refreshToken }
                );

                localStorage.setItem(ACCESS_TOKEN_KEY, data.jwtToken);
                localStorage.setItem(REFRESH_TOKEN_KEY, data.refreshToken);

                apiClient.defaults.headers.common.Authorization = `Bearer ${data.jwtToken}`;
                processQueue(null, data.jwtToken);

                originalRequest.headers.Authorization = `Bearer ${data.jwtToken}`;
                return apiClient(originalRequest);
            } catch (refreshError) {
                processQueue(refreshError, null);
                localStorage.removeItem(ACCESS_TOKEN_KEY);
                localStorage.removeItem(REFRESH_TOKEN_KEY);
                window.location.href = '/login';
                return Promise.reject(refreshError)
            } finally {
                isRefreshing = false;
            }
        }
        return Promise.reject(error);
    }
)

export default apiClient;
