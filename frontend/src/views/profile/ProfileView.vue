<template>
  <div class="profile">

    <!-- Dos columnas -->
    <div class="profile-grid">

      <!-- Panel izquierdo -->
      <div class="left-panel">

        <!-- Avatar y nombre -->
        <div class="avatar-card">
          <div class="avatar-wrapper">
            <div class="avatar">{{ userInitials }}</div>
            <button class="avatar-edit-btn">
              <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12"
                viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
                <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
              </svg>
            </button>
          </div>
          <h3 class="profile-name">{{ user.name }}</h3>
          <span class="role-badge">{{ user.role }}</span>
          <p class="profile-email">{{ user.email }}</p>
        </div>

        <!-- Actividad reciente -->
        <div class="side-card">
          <div class="side-card-title">ACTIVIDAD RECIENTE</div>
          <div class="activity-list">
            <div v-for="item in recentActivity" :key="item.id" class="activity-item">
              <span class="activity-dot" :style="{ background: item.color }"></span>
              <div>
                <p class="activity-text">{{ item.text }}</p>
                <p class="activity-time">{{ item.time }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Sesión activa -->
        <div class="side-card">
          <div class="side-card-title">SESIÓN ACTIVA</div>
          <div class="session-list">
            <div class="session-row">
              <span class="session-label">Dispositivo</span>
              <span class="session-value">Chrome / Windows</span>
            </div>
            <div class="session-row">
              <span class="session-label">IP</span>
              <span class="session-value">192.168.1.45</span>
            </div>
            <div class="session-row">
              <span class="session-label">Inicio</span>
              <span class="session-value">Hoy, 08:12</span>
            </div>
            <div class="session-row">
              <span class="session-label">Token expira</span>
              <span class="session-value green">En 45 min</span>
            </div>
          </div>
        </div>

      </div>

      <!-- Panel derecho -->
      <div class="right-panel">
        
        <!-- Tabs -->
        <div class="tabs-bar">
          <button
            v-for="tab in tabs"
            :key="tab.key"
            class="tab"
            :class="{ active: activeTab === tab.key }"
            @click="activeTab = tab.key"
          >
            {{ tab.label }}
          </button>
        </div>

        <!-- Tab: Datos personales -->
        <div v-if="activeTab === 'personal'" class="tab-card">
          <div class="section-title">INFORMACIÓN PERSONAL</div>
          <div class="form-grid-3">

            <div class="form-group">
              <label class="form-label">Nombre</label>
              <input v-model="form.firstName" type="text"
                class="form-input" placeholder="Nombre" />
            </div>

            <div class="form-group">
              <label class="form-label">Apellido</label>
              <input v-model="form.lastName" type="text"
                class="form-input" placeholder="Apellido" />
            </div>

            <div class="form-group">
              <label class="form-label">Rol</label>
              <input :value="user.role" type="text"
                class="form-input" disabled />
            </div>

          </div>

          <div class="form-group full-width">
            <label class="form-label">Correo electrónico</label>
            <div class="email-field">
              <input v-model="form.email" type="email"
                class="form-input" placeholder="correo@empresa.com" />
              <span class="verified-badge">Verificado ✓</span>
            </div>
          </div>

          <div class="form-grid-3">
            <div class="form-group">
              <label class="form-label">Teléfono</label>
              <input v-model="form.phone" type="text"
                class="form-input" placeholder="809-555-0001" />
            </div>
            <div class="form-group">
              <label class="form-label">Departamento</label>
              <input v-model="form.department" type="text"
                class="form-input" placeholder="Tecnología" />
            </div>
            <div class="form-group">
              <label class="form-label">Cargo</label>
              <input v-model="form.position" type="text"
                class="form-input" placeholder="Administrador del Sistema" />
            </div>
          </div>

          <div class="form-actions">
            <button class="btn-cancel" @click="resetForm">Descartar cambios</button>
            <button class="btn-submit" @click="handleSave" :disabled="loading">
              {{ loading ? 'Guardando...' : 'Guardar cambios' }}
            </button>
          </div>
        </div>

        <!-- Tab: Cambiar contraseña -->
        <div v-if="activeTab === 'password'" class="tab-card">
          <div class="section-title">CAMBIAR CONTRASEÑA</div>

          <div class="form-group">
            <label class="form-label">Contraseña actual</label>
            <PasswordInput v-model="passwordForm.current" />
          </div>

          <div class="form-group">
            <label class="form-label">Nueva contraseña</label>
            <PasswordInput v-model="passwordForm.new"
              placeholder="Mín. 8 caracteres" />
          </div>

          <div class="form-group">
            <label class="form-label">Confirmar nueva contraseña</label>
            <PasswordInput v-model="passwordForm.confirm"
              placeholder="Repite la nueva contraseña" />
          </div>

          <div class="warning-banner">
            ⚠️ Al cambiar tu contraseña,
            <strong>todas las sesiones activas en otros dispositivos serán cerradas</strong>
            por seguridad.
          </div>

          <div class="form-actions">
            <button class="btn-cancel" @click="resetPasswordForm">Cancelar</button>
            <button class="btn-submit" @click="handlePasswordChange" :disabled="loadingPassword">
              {{ loadingPassword ? 'Cambiando...' : 'Cambiar contraseña' }}
            </button>
          </div>
        </div>

        <!-- Tab: Preferencias -->
        <div v-if="activeTab === 'preferences'" class="tab-card">
          <div class="section-title">PREFERENCIAS DEL SISTEMA</div>

          <div class="pref-list">
            <div v-for="pref in preferences" :key="pref.key" class="pref-item">
              <div class="pref-info">
                <p class="pref-name">{{ pref.name }}</p>
                <p class="pref-desc">{{ pref.description }}</p>
              </div>
              <button
                class="toggle"
                :class="{ active: pref.enabled }"
                @click="pref.enabled = !pref.enabled"
              >
                <span class="toggle-thumb"></span>
              </button>
            </div>
          </div>

          <div class="form-actions">
            <button class="btn-submit" @click="handleSavePreferences">
              Guardar preferencias
            </button>
          </div>
        </div>

      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { useAuthStore } from '@/stores/auth.store'
import PasswordInput from '@/components/forms/PasswordInput.vue'

const auth = useAuthStore()
const activeTab = ref('personal')
const loading = ref(false)
const loadingPassword = ref(false)

const tabs = [
  { key: 'personal',    label: 'Datos personales'   },
  { key: 'password',    label: 'Cambiar contraseña'  },
  { key: 'preferences', label: 'Preferencias'         },
]

const user = ref({
  name: auth.user?.name || 'Michael Admin',
  role: auth.role || 'Administrador',
  email: 'michael@empresa.com',
})

const userInitials = computed(() => {
  return user.value.name.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase()
})

const form = reactive({
  firstName:  'Michael',
  lastName:   'Admin',
  email:      'michael@empresa.com',
  phone:      '809-555-0001',
  department: 'Tecnología',
  position:   'Administrador del Sistema',
})

const passwordForm = reactive({
  current: '', new: '', confirm: ''
})

const preferences = ref([
  { key: 'emailNotif',    name: 'Notificaciones por correo',  description: 'Recibe alertas de mantenimiento y solicitudes',    enabled: true  },
  { key: 'maintenance',   name: 'Alertas de mantenimiento',   description: 'Notificaciones 7 días antes del servicio',          enabled: true  },
  { key: 'screenNotif',   name: 'Notificaciones en pantalla', description: 'Toasts al aprobar, rechazar o asignar',             enabled: true  },
  { key: 'licenseExpiry', name: 'Vencimiento de licencias',   description: 'Alertas cuando licencia de conductor vence en 30 días', enabled: false },
])

const recentActivity = ref([
  { id: 1, text: 'Solicitud #001 aprobada',    time: 'Hoy, 09:15',      color: '#22c55e' },
  { id: 2, text: 'Vehículo GHI-789 editado',   time: 'Ayer, 14:30',     color: '#3b82f6' },
  { id: 3, text: 'Mantenimiento registrado',    time: '26/05/26, 11:00', color: '#f59e0b' },
  { id: 4, text: 'Usuario Pedro creado',        time: '25/05/26, 09:15', color: '#8b5cf6' },
])

function resetForm() {
  Object.assign(form, {
    firstName: 'Michael', lastName: 'Admin',
    email: 'michael@empresa.com', phone: '809-555-0001',
    department: 'Tecnología', position: 'Administrador del Sistema',
  })
}

function resetPasswordForm() {
  Object.assign(passwordForm, { current: '', new: '', confirm: '' })
}

async function handleSave() {
  loading.value = true
  try {
    await new Promise(r => setTimeout(r, 800))
  } finally {
    loading.value = false
  }
}

async function handlePasswordChange() {
  loadingPassword.value = true
  try {
    await new Promise(r => setTimeout(r, 800))
    resetPasswordForm()
  } finally {
    loadingPassword.value = false
  }
}

async function handleSavePreferences() {
  await new Promise(r => setTimeout(r, 500))
}
</script>

<style scoped>
.profile {
  font-family: 'Inter', sans-serif;
}

.profile-grid {
  display: grid;
  grid-template-columns: 300px 1fr;
  gap: 1.25rem;
  align-items: flex-start;
}

/* Panel izquierdo */
.left-panel {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.avatar-card {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.avatar-wrapper {
  position: relative;
  margin-bottom: 0.25rem;
}

.avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: #3b82f6;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  font-weight: 700;
  color: #fff;
}

.avatar-edit-btn {
  position: absolute;
  bottom: 0;
  right: 0;
  width: 26px;
  height: 26px;
  border-radius: 50%;
  background: #111827;
  border: 2px solid #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #fff;
  cursor: pointer;
}

.profile-name {
  font-size: 1rem;
  font-weight: 700;
  color: #111827;
}

.role-badge {
  background: #f5f3ff;
  color: #7c3aed;
  font-size: 0.75rem;
  font-weight: 500;
  padding: 3px 12px;
  border-radius: 999px;
}

.profile-email {
  font-size: 0.78rem;
  color: #9ca3af;
}

/* Side cards */
.side-card {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  padding: 1.25rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
  display: flex;
  flex-direction: column;
  gap: 0.875rem;
}

.side-card-title {
  font-size: 0.7rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.08em;
}

/* Activity */
.activity-list { display: flex; flex-direction: column; gap: 0.75rem; }

.activity-item {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
}

.activity-dot {
  width: 8px;
  height: 8px;
  min-width: 8px;
  border-radius: 50%;
  margin-top: 4px;
}

.activity-text {
  font-size: 0.82rem;
  font-weight: 500;
  color: #111827;
}

.activity-time {
  font-size: 0.72rem;
  color: #9ca3af;
  margin-top: 1px;
}

/* Session */
.session-list { display: flex; flex-direction: column; gap: 0.5rem; }

.session-row {
  display: flex;
  justify-content: space-between;
  font-size: 0.82rem;
}

.session-label { color: #9ca3af; }
.session-value { color: #374151; font-weight: 500; }
.session-value.green { color: #16a34a; }

/* Panel derecho */
.right-panel {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

/* Tabs */
.tabs-bar {
  display: flex;
  border-bottom: 1px solid #e5e7eb;
}

.tab {
  padding: 0.75rem 1.25rem;
  border: none;
  background: none;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #6b7280;
  cursor: pointer;
  border-bottom: 2px solid transparent;
  transition: all 0.2s;
  margin-bottom: -1px;
}

.tab:hover { color: #111827; }
.tab.active { color: #2563eb; border-bottom-color: #2563eb; font-weight: 600; }

/* Tab card */
.tab-card {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  padding: 1.5rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.section-title {
  font-size: 0.7rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.08em;
}

/* Formularios */
.form-grid-3 {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  gap: 1rem;
}

.form-group { display: flex; flex-direction: column; gap: 0.3rem; }
.form-group.full-width { grid-column: 1 / -1; }

.form-label {
  font-size: 0.82rem;
  font-weight: 500;
  color: #374151;
}

.form-input {
  padding: 0.6rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #111827;
  outline: none;
  transition: border-color 0.2s;
  background: #fff;
}

.form-input:focus {
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
}

.form-input:disabled {
  background: #f9fafb;
  color: #9ca3af;
  cursor: not-allowed;
}

.email-field {
  position: relative;
  display: flex;
  align-items: center;
}

.email-field .form-input { flex: 1; padding-right: 110px; }

.verified-badge {
  position: absolute;
  right: 12px;
  font-size: 0.75rem;
  font-weight: 500;
  color: #16a34a;
  background: #f0fdf4;
  padding: 2px 8px;
  border-radius: 999px;
  border: 1px solid #bbf7d0;
}

/* Warning banner */
.warning-banner {
  background: #fffbeb;
  border: 1px solid #fde68a;
  border-radius: 8px;
  padding: 0.875rem 1rem;
  font-size: 0.82rem;
  color: #92400e;
  line-height: 1.5;
}

/* Form actions */
.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  margin-top: 0.25rem;
}

.btn-cancel {
  padding: 0.6rem 1.25rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #fff;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
  cursor: pointer;
}

.btn-cancel:hover { background: #f9fafb; }

.btn-submit {
  padding: 0.6rem 1.25rem;
  background: #2563eb;
  color: #fff;
  border: none;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-submit:hover:not(:disabled) { background: #1d4ed8; }
.btn-submit:disabled { opacity: 0.6; cursor: not-allowed; }

/* Preferences toggle */
.pref-list { display: flex; flex-direction: column; gap: 0; }

.pref-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 0;
  border-bottom: 1px solid #f3f4f6;
}

.pref-item:last-child { border-bottom: none; }

.pref-info { flex: 1; }

.pref-name {
  font-size: 0.875rem;
  font-weight: 500;
  color: #111827;
}

.pref-desc {
  font-size: 0.75rem;
  color: #9ca3af;
  margin-top: 2px;
}

/* Toggle switch */
.toggle {
  width: 44px;
  height: 24px;
  border-radius: 999px;
  background: #d1d5db;
  border: none;
  cursor: pointer;
  position: relative;
  transition: background 0.2s;
  flex-shrink: 0;
}

.toggle.active { background: #2563eb; }

.toggle-thumb {
  position: absolute;
  top: 2px;
  left: 2px;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: #fff;
  transition: transform 0.2s;
  box-shadow: 0 1px 3px rgba(0,0,0,0.2);
}

.toggle.active .toggle-thumb { transform: translateX(20px); }
</style>