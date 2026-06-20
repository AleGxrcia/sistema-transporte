const apliClient = axios.create({
    baseURL : process.env.VITE_API_BASE_URL || 'http://localhost:5000/api',
    headers: {
        'Content-Type': 'application/json'
    },
    timeout: 15000,
})

apliClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('accessToken')
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

apliClient.interceptors.response.use(
    (response) => {
        return response    
    },
    async (error) => {
        const originalRequest = error.config;

        if (error.response && error.response.status === 401 && !originalRequest._retry) {
            if (originalRequest.url?.includes('/auth/refresh')) {
                localStorage.removeItem('accessToken');
                localStorage.removeItem('refreshToken');
                window.location.href = '/login';
                return Promise.reject(error);
            }

            if (isRefreshing) {
                return new Promise((resolve, reject) => {
                    failedQueue.push({ resolve, reject });
                }).then((token) => {
                    originalRequest.headers.Authorization = `Bearer ${token}`;
                    return apliClient(originalRequest);
                })
            }

            originalRequest._retry = true;
            isRefreshing = true;

            try {
                const refreshToken = localStorage.getItem('refreshToken');
                const accessToken = localStorage.getItem('accessToken');

                const { data } = await axios.post(
                    `${apiClient.defaults.baseURL}/auth/refresh`,
                    { accessToken, refreshToken }
                );

                localStorage.setItem('accessToken', data.jwtToken);
                localStorage.setItem('refreshToken', data.refreshToken);

                apiClient.defaults.headers.common.Authorization = `Bearer ${data.jwtToken}`;
                processQueue(null, data.jwtToken);

                originalRequest.headers.Authorization = `Bearer ${data.jwtToken}`;
                return apiClient(originalRequest);
            } catch (refreshError) {
                processQueue(refreshError, null);
                localStorage.removeItem('accessToken');
                localStorage.removeItem('refreshToken');
                window.location.href = '/login';
                return Promise.reject(refreshError)
            } finally {
                isRefreshing = false;
            }
        }
        return Promise.reject(error);
    }
)

export default apliClient;