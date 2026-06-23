<template>
  <aside class="sidebar" :class="{ collapsed: isCollapsed }">

    <!-- Logo -->
    <div class="sidebar-logo">
      <img v-if="!isCollapsed" src="/logo2.png" alt="TransFleet" class="logo-full" />
      <img v-else src="/logo2.png" alt="TransFleet" class="logo-mini" />
    </div>

    <!-- Botón para colapsar -->
    <button class="collapse-btn" @click="isCollapsed = !isCollapsed">
      <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
        viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <path v-if="!isCollapsed" d="M15 18l-6-6 6-6"/>
        <path v-else d="M9 18l6-6-6-6"/>
      </svg>
    </button>

    <!-- Navegación -->
    <nav class="sidebar-nav">

      <div class="nav-section">
        <span class="nav-section-title" v-if="!isCollapsed">PRINCIPAL</span>
        <router-link to="/dashboard" class="nav-item">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/>
            <rect x="14" y="14" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/>
          </svg>
          <span v-if="!isCollapsed">Dashboard</span>
        </router-link>

        <router-link to="/vehicles" class="nav-item">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="1" y="3" width="15" height="13" rx="2"/>
            <path d="M16 8h4l3 3v5h-7V8z"/>
            <circle cx="5.5" cy="18.5" r="2.5"/><circle cx="18.5" cy="18.5" r="2.5"/>
          </svg>
          <span v-if="!isCollapsed">Vehículos</span>
        </router-link>

        <router-link to="/drivers" class="nav-item">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
            <circle cx="12" cy="7" r="4"/>
          </svg>
          <span v-if="!isCollapsed">Conductores</span>
        </router-link>

        <router-link to="/requests" class="nav-item">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
            <polyline points="14 2 14 8 20 8"/>
          </svg>
          <span v-if="!isCollapsed">Solicitudes</span>
          <span v-if="!isCollapsed && pendingCount > 0" class="nav-badge">{{ pendingCount }}</span>
        </router-link>
      </div>

      <div class="nav-section">
        <span class="nav-section-title" v-if="!isCollapsed">OPERACIONES</span>

        <router-link to="/schedules" class="nav-item">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
            <line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/>
            <line x1="3" y1="10" x2="21" y2="10"/>
          </svg>
          <span v-if="!isCollapsed">Agenda</span>
        </router-link>

        <router-link to="/trips" class="nav-item">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/>
          </svg>
          <span v-if="!isCollapsed">Historial de viajes</span>
        </router-link>

        <router-link to="/fuel" class="nav-item">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M3 22V8a2 2 0 0 1 2-2h6a2 2 0 0 1 2 2v14"/>
            <path d="M3 22h10M13 8h2a2 2 0 0 1 2 2v3a2 2 0 0 0 2 2h0a2 2 0 0 0 2-2V9"/>
            <line x1="6" y1="2" x2="10" y2="2"/><line x1="8" y1="2" x2="8" y2="6"/>
          </svg>
          <span v-if="!isCollapsed">Combustible</span>
        </router-link>

        <router-link v-if="can('view', 'maintenance')" to="/maintenance" class="nav-item">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/>
          </svg>
          <span v-if="!isCollapsed">Mantenimiento</span>
        </router-link>

        <router-link v-if="can('view', 'reports')" to="/reports" class="nav-item">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="18" y1="20" x2="18" y2="10"/>
            <line x1="12" y1="20" x2="12" y2="4"/>
            <line x1="6" y1="20" x2="6" y2="14"/>
          </svg>
          <span v-if="!isCollapsed">Reportes</span>
        </router-link>
      </div>

      <div class="nav-section" v-if="can('view', 'users')">
        <span class="nav-section-title" v-if="!isCollapsed">ADMINISTRACIÓN</span>
        <router-link to="/users" class="nav-item">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
            <circle cx="9" cy="7" r="4"/>
            <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
            <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
          </svg>
          <span v-if="!isCollapsed">Usuarios</span>
        </router-link>

        <router-link to="/roles" class="nav-item">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/>
            <path d="M7 11V7a5 5 0 0 1 10 0v4"/>
          </svg>
          <span v-if="!isCollapsed">Roles y permisos</span>
        </router-link>
      </div>

    </nav>

    <!-- Usuario en la parte inferior -->
    <div class="sidebar-user">
      <div class="sidebar-user-info" @click="$router.push('/profile')">
        <div class="user-avatar">{{ userInitials }}</div>
        <div class="user-info" v-if="!isCollapsed">
          <span class="user-name">{{ userName }}</span>
          <span class="user-role">{{ userRole }}</span>
        </div>
      </div>
      <button v-if="!isCollapsed" class="logout-btn" title="Cerrar sesión" @click="handleLogout">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
          viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/>
          <polyline points="16 17 21 12 16 7"/>
          <line x1="21" y1="12" x2="9" y2="12"/>
        </svg>
      </button>
    </div>

  </aside>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth.store'
import { useAuth } from '@/composables/useAuth'
import { useRequestsStore } from '@/stores/requests.store'

const auth = useAuthStore()
const { logout, can } = useAuth()
const isCollapsed = ref(false)

function handleLogout() {
  logout()
}

// Badge de solicitudes pendientes
const requestsStore = useRequestsStore()
onMounted(() => requestsStore.fetchPending())
const pendingCount = computed(() => requestsStore.pendingCount)

const userName = computed(() => auth.user?.fullName || 'Usuario')
const userRole = computed(() => auth.currentRole || '')
const userInitials = computed(() => {
  const name = auth.user?.fullName || 'U'
  return name.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase()
})
</script>

<style scoped>
.sidebar {
  width: 220px;
  min-width: 220px;
  background: #0f172a;
  display: flex;
  flex-direction: column;
  height: 100vh;
  position: sticky;
  top: 0;
  transition: width 0.25s ease, min-width 0.25s ease;
  overflow: hidden;
}

.sidebar.collapsed {
  width: 64px;
  min-width: 64px;
}

/* Logo */
.sidebar-logo {
  padding: 1.25rem 1rem 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 72px;
}

.logo-full {
  height: 48px;
  width: auto;
  object-fit: contain;
  mix-blend-mode: lighten;
}

.logo-mini {
  height: 32px;
  width: 32px;
  object-fit: contain;
  mix-blend-mode: lighten;
}

/* Botón colapsar */
.collapse-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 24px;
  height: 24px;
  background: #1e293b;
  border: 1px solid #334155;
  border-radius: 50%;
  color: #94a3b8;
  cursor: pointer;
  position: absolute;
  right: -12px;
  top: 72px;
  z-index: 10;
  transition: background 0.2s;
}

.collapse-btn:hover { background: #334155; }

/* Navegación */
.sidebar-nav {
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden;
  padding: 0.5rem 0;
}

.sidebar-nav::-webkit-scrollbar { width: 4px; }
.sidebar-nav::-webkit-scrollbar-thumb { background: #334155; border-radius: 4px; }

.nav-section {
  padding: 0.75rem 0 0.25rem;
}

.nav-section-title {
  display: block;
  font-size: 0.7rem;
  font-weight: 600;
  color: #475569;
  letter-spacing: 0.08em;
  padding: 0 1rem 0.5rem;
  white-space: nowrap;
}

/* Item de navegación */
.nav-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.6rem 1rem;
  color: #94a3b8;
  text-decoration: none;
  font-size: 0.925rem;
  font-family: 'Inter', sans-serif;
  border-radius: 6px;
  margin: 0 0.5rem;
  transition: background 0.15s, color 0.15s;
  white-space: nowrap;
  position: relative;
}

.nav-item:hover {
  background: #1e293b;
  color: #e2e8f0;
}

.nav-item.router-link-active {
  background: #1e40af;
  color: #fff;
}

.nav-item svg { flex-shrink: 0; }

/* Badge de notificaciones */
.nav-badge {
  margin-left: auto;
  background: #ef4444;
  color: #fff;
  font-size: 0.75rem;
  font-weight: 700;
  padding: 1px 6px;
  border-radius: 999px;
  min-width: 18px;
  text-align: center;
}

/* Vista Usuario inferior */
.sidebar-user {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  border-top: 1px solid #1e293b;
  margin-top: auto;
}

.sidebar-user-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  flex: 1;
  min-width: 0;
  padding: 0.25rem;
  border-radius: 6px;
  cursor: pointer;
  transition: background 0.2s;
}

.sidebar-user-info:hover {
  background: #1e293b;
}

.logout-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  min-width: 32px;
  border-radius: 6px;
  border: none;
  background: transparent;
  color: #94a3b8;
  cursor: pointer;
  transition: background 0.2s, color 0.2s;
}

.logout-btn:hover {
  background: #1e293b;
  color: #ef4444;
}

.user-avatar {
  width: 36px;
  height: 36px;
  min-width: 36px;
  background: #2563eb;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.8rem;
  font-weight: 700;
  color: #fff;
}

.user-info {
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.user-name {
  font-size: 0.85rem;
  font-weight: 600;
  color: #e2e8f0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.user-role {
  font-size: 0.75rem;
  color: #64748b;
}
</style>