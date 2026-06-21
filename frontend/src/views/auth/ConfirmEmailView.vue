<template>
  <div class="auth-page">
    <div class="auth-card">

      <div class="auth-logo">
        <img src="/logo.png" alt="FleetPro" class="logo-img" />
      </div>

      <h1 class="auth-title">Confirmar cuenta</h1>

      <p v-if="status === 'loading'" class="status-msg">Confirmando tu cuenta...</p>
      <p v-if="status === 'success'" class="success-msg">✅ Tu cuenta ha sido confirmada.</p>
      <p v-if="status === 'error'" class="error-msg">{{ error }}</p>

      <button class="btn-primary" @click="router.push('/login')">
        Ir a iniciar sesión
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import AuthService from '@/services/auth.service.js'

const route = useRoute()
const router = useRouter()
const status = ref('loading')
const error = ref('')

onMounted(async () => {
  const { userId, token } = route.query
  if (!userId || !token) {
    status.value = 'error'
    error.value = 'Enlace de confirmación inválido.'
    return
  }

  try {
    await AuthService.confirmEmail(userId, token)
    status.value = 'success'
  } catch {
    status.value = 'error'
    error.value = 'No se pudo confirmar la cuenta. El enlace puede haber expirado.'
  }
})
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap');

* { font-family: 'Inter', sans-serif; }

.auth-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f3f4f6;
}

.auth-card {
  background: #fff;
  border-radius: 16px;
  padding: 2rem;
  width: 100%;
  max-width: 380px;
  box-shadow: 0 4px 24px rgba(0,0,0,0.08);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.auth-logo { display: flex; justify-content: center; }

.logo-img {
  height: 80px;
  width: auto;
  object-fit: contain;
  mix-blend-mode: multiply;
}

.auth-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #111827;
  text-align: center;
}

.status-msg { font-size: 0.875rem; color: #6b7280; text-align: center; }
.success-msg { font-size: 0.875rem; color: #16a34a; text-align: center; }
.error-msg { font-size: 0.875rem; color: #dc2626; text-align: center; }

.btn-primary {
  width: 70%;
  padding: 0.75rem;
  background: #2563eb;
  color: #fff;
  border: none;
  border-radius: 8px;
  font-size: 0.95rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-primary:hover { background: #1d4ed8; }
</style>
