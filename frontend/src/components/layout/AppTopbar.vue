<template>
  <header class="topbar">

    <div class="topbar-left">
      <h1 class="page-title">{{ title }}</h1>
      <span class="page-date">{{ fechaHoy }}</span>
    </div>

    <div class="topbar-right">

      <button class="icon-btn" @click="toggleNotifications">
        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20"
          viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"/>
          <path d="M13.73 21a2 2 0 0 1-3.46 0"/>
        </svg>
        <span v-if="unreadCount > 0" class="notif-dot">{{ unreadCount }}</span>
      </button>

      <div class="user-avatar" @click="$router.push('/profile')" style="cursor:pointer">{{ userInitials }}</div>

    </div>

    <!-- Panel de notificaciones -->
    <div v-if="showNotifications" class="notif-overlay" @click="showNotifications = false" />
    <div class="notif-panel" :class="{ open: showNotifications }">

      <div class="notif-header">
        <div class="notif-header-left">
          <span class="notif-title">Notificaciones</span>
          <span v-if="unreadCount > 0" class="notif-count">{{ unreadCount }} nuevas</span>
        </div>
        <div class="notif-header-right">
          <button class="notif-mark-read">Marcar leídas</button>
          <button class="notif-close" @click="showNotifications = false">✕</button>
        </div>
      </div>

      <div class="notif-list">
        <!-- Nuevas -->
        <div
          v-for="notif in newNotifications"
          :key="notif.id"
          class="notif-item unread"
          :class="notif.color"
        >
          <div class="notif-icon" :style="{ background: notif.iconBg }">
            <span>{{ notif.icon }}</span>
          </div>
          <div class="notif-content">
            <p class="notif-name">{{ notif.title }}</p>
            <p class="notif-desc">{{ notif.description }}</p>
            <p class="notif-time">{{ notif.time }}</p>
          </div>
          <span class="notif-dot-color" :style="{ background: notif.dotColor }"></span>
        </div>

        <!-- Anteriores -->
        <div class="notif-section-title">ANTERIORES</div>
        <div
          v-for="notif in oldNotifications"
          :key="notif.id"
          class="notif-item"
        >
          <div class="notif-icon gray">
            <span>{{ notif.icon }}</span>
          </div>
          <div class="notif-content">
            <p class="notif-name">{{ notif.title }}</p>
            <p class="notif-desc">{{ notif.description }}</p>
            <p class="notif-time">{{ notif.time }}</p>
          </div>
        </div>
      </div>

      <div class="notif-footer">
        <router-link to="/notifications" @click="showNotifications = false">
          Ver todas las notificaciones →
        </router-link>
      </div>

    </div>

  </header>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useAuthStore } from '@/stores/auth.store'
import { useRoute } from 'vue-router'
import { format } from 'date-fns'
import { es } from 'date-fns/locale'

const auth = useAuthStore()
const route = useRoute()
const showNotifications = ref(false)

const toggleNotifications = () => {
  showNotifications.value = !showNotifications.value
}

const title = computed(() => {
  const titles = {
    '/dashboard':    'Dashboard',
    '/vehicles':     'Vehículos',
    '/drivers':      'Conductores',
    '/requests':     'Solicitudes de transporte',
    '/assignments':  'Asignaciones',
    '/calendar':     'Agenda',
    '/maintenance':  'Mantenimiento',
    '/fuel':         'Combustible',
    '/trips':        'Historial de viajes',
    '/reports':      'Reportes',
    '/notifications':'Notificaciones',
    '/profile':      'Mi perfil',
    '/admin/users':  'Usuarios',
    '/admin/roles':  'Roles',
  }
  return titles[route.path] || 'TransFleet'
})

const fechaHoy = computed(() =>
  format(new Date(), "EEEE d 'de' MMMM, yyyy", { locale: es })
)

const userInitials = computed(() => {
  const name = auth.user?.name || 'U'
  return name.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase()
})

// Notificaciones mock
const unreadCount = ref(4)

const newNotifications = ref([
  { id: 1, title: 'Mantenimiento urgente', description: 'Honda CRV DEF-456 vence en 4 días', time: 'Hace 5 min', icon: '⚠️', iconBg: '#fff7ed', dotColor: '#ef4444' },
  { id: 2, title: 'Nueva solicitud pendiente', description: 'Área de Ventas — 29 mayo 2026', time: 'Hace 23 min', icon: '📋', iconBg: '#f5f3ff', dotColor: '#8b5cf6' },
  { id: 3, title: 'Licencia próxima a vencer', description: 'Miguel Fernández — 12 días', time: 'Hace 1 hora', icon: '📅', iconBg: '#fffbeb', dotColor: '#f59e0b' },
  { id: 4, title: 'Solicitud #003 aprobada', description: 'Carlos Supervisor aprobó tu viaje', time: 'Hace 2 horas', icon: '✅', iconBg: '#f0fdf4', dotColor: '#22c55e' },
])

const oldNotifications = ref([
  { id: 5, title: 'Viaje #087 finalizado', description: 'RRHH → Aeropuerto completado', time: 'Hoy, 09:45', icon: '📈' },
  { id: 6, title: 'Consumo registrado — ABC-123', description: '45.5 galones cargados', time: 'Ayer, 16:20', icon: '⛽' },
])
</script>

<style scoped>
.topbar {
  height: 60px;
  background: #fff;
  border-bottom: 1px solid #e5e7eb;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 1.5rem;
  position: sticky;
  top: 0;
  z-index: 100;
}

.topbar-left {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.page-title {
  font-size: 1.1rem;
  font-weight: 700;
  color: #111827;
  font-family: 'Inter', sans-serif;
}

.page-date {
  font-size: 0.8rem;
  color: #9ca3af;
  font-family: 'Inter', sans-serif;
  text-transform: capitalize;
}

.topbar-right {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

/* Botón ícono */
.icon-btn {
  position: relative;
  width: 38px;
  height: 38px;
  border-radius: 50%;
  border: none;
  background: #f3f4f6;
  color: #6b7280;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: background 0.2s;
}

.icon-btn:hover { background: #e5e7eb; }

.notif-dot {
  position: absolute;
  top: 4px;
  right: 4px;
  background: #ef4444;
  color: #fff;
  font-size: 0.6rem;
  font-weight: 700;
  width: 16px;
  height: 16px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.user-avatar {
  width: 36px;
  height: 36px;
  background: #2563eb;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: 700;
  color: #fff;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
}

.notif-overlay {
  position: fixed;
  inset: 0;
  z-index: 200;
}

/* Panel de notificaciones */
.notif-panel {
  position: fixed;
  top: 0;
  right: -400px;
  width: 380px;
  height: 100vh;
  background: #fff;
  box-shadow: -4px 0 24px rgba(0,0,0,0.12);
  z-index: 201;
  display: flex;
  flex-direction: column;
  transition: right 0.3s ease;
}

.notif-panel.open { right: 0; }

.notif-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.25rem 1rem;
  border-bottom: 1px solid #f3f4f6;
}

.notif-header-left {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.notif-title {
  font-size: 1rem;
  font-weight: 700;
  color: #111827;
  font-family: 'Inter', sans-serif;
}

.notif-count {
  background: #ef4444;
  color: #fff;
  font-size: 0.7rem;
  font-weight: 700;
  padding: 2px 8px;
  border-radius: 999px;
}

.notif-header-right {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.notif-mark-read {
  font-size: 0.78rem;
  color: #2563eb;
  background: none;
  border: 1px solid #2563eb;
  border-radius: 6px;
  padding: 3px 10px;
  cursor: pointer;
  font-family: 'Inter', sans-serif;
}

.notif-close {
  background: none;
  border: none;
  font-size: 0.9rem;
  color: #9ca3af;
  cursor: pointer;
  padding: 4px 6px;
}

/* Lista */
.notif-list {
  flex: 1;
  overflow-y: auto;
  padding: 0.5rem;
}

.notif-section-title {
  font-size: 0.65rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.08em;
  padding: 0.75rem 0.5rem 0.25rem;
}

.notif-item {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 0.75rem;
  border-radius: 8px;
  margin-bottom: 0.25rem;
  position: relative;
  cursor: pointer;
  transition: background 0.15s;
}

.notif-item:hover { background: #f9fafb; }

.notif-item.unread { background: #f8faff; }

.notif-icon {
  width: 36px;
  height: 36px;
  min-width: 36px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1rem;
}

.notif-icon.gray { background: #f3f4f6; }

.notif-content { flex: 1; }

.notif-name {
  font-size: 0.85rem;
  font-weight: 600;
  color: #111827;
  font-family: 'Inter', sans-serif;
  margin-bottom: 2px;
}

.notif-desc {
  font-size: 0.78rem;
  color: #6b7280;
  font-family: 'Inter', sans-serif;
  margin-bottom: 2px;
}

.notif-time {
  font-size: 0.72rem;
  color: #9ca3af;
  font-family: 'Inter', sans-serif;
}

.notif-dot-color {
  width: 8px;
  height: 8px;
  min-width: 8px;
  border-radius: 50%;
  margin-top: 4px;
}

.notif-footer {
  padding: 1rem;
  border-top: 1px solid #f3f4f6;
  text-align: center;
}

.notif-footer a {
  font-size: 0.875rem;
  color: #374151;
  text-decoration: none;
  font-family: 'Inter', sans-serif;
  font-weight: 500;
}

.notif-footer a:hover { color: #2563eb; }
</style>