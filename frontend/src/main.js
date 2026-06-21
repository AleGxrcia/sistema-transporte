import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router/index.js'
import './assets/styles/main.css'

const app = createApp(App)

// Error handling
app.config.errorHandler = (err, instance, info) => {
  console.error('Vue Error:', err)
  console.error('Component:', instance)
  console.error('Info:', info)
}

app.use(createPinia())
app.use(router)
app.mount('#app')

console.log('✅ App mounted successfully')