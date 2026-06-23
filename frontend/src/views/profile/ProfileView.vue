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
          </div>
          <h3 class="profile-name">{{ fullName }}</h3>
          <span class="role-badge">{{ user.role }}</span>
          <p class="profile-email">{{ user.email }}</p>
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
            <input v-model="form.email" type="email"
              class="form-input" placeholder="correo@empresa.com" />
          </div>

          <p v-if="saveError" class="error-msg">{{ saveError }}</p>

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
              placeholder="Mín. 6 caracteres" />
          </div>

          <div class="form-group">
            <label class="form-label">Confirmar nueva contraseña</label>
            <PasswordInput v-model="passwordForm.confirm"
              placeholder="Repite la nueva contraseña" />
            <p v-if="passwordMismatch" class="error-msg">Las contraseñas no coinciden</p>
          </div>

          <p v-if="passwordError" class="error-msg">{{ passwordError }}</p>
          <p v-if="passwordSuccess" class="success-msg">Contraseña actualizada correctamente.</p>

          <div class="form-actions">
            <button class="btn-cancel" @click="resetPasswordForm">Cancelar</button>
            <button class="btn-submit" @click="handlePasswordChange" :disabled="loadingPassword">
              {{ loadingPassword ? 'Cambiando...' : 'Cambiar contraseña' }}
            </button>
          </div>
        </div>

      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth.store'
import UsersService from '@/services/users.service'
import PasswordInput from '@/components/forms/PasswordInput.vue'

const authStore = useAuthStore()
const activeTab = ref('personal')
const loading = ref(false)
const loadingPassword = ref(false)
const saveError = ref('')
const passwordError = ref('')
const passwordSuccess = ref(false)

const tabs = [
  { key: 'personal', label: 'Datos personales'  },
  { key: 'password', label: 'Cambiar contraseña' },
]

const user = ref({ role: authStore.currentRole || '', email: '' })

const fullName = computed(() => [form.firstName, form.lastName].filter(Boolean).join(' '))

const userInitials = computed(() => {
  const initials = [form.firstName?.[0], form.lastName?.[0]].filter(Boolean).join('')
  return (initials || fullName.value.slice(0, 2)).toUpperCase()
})

const form = reactive({
  firstName: '', lastName: '', email: '',
})

const passwordForm = reactive({
  current: '', new: '', confirm: ''
})

const passwordMismatch = computed(() =>
  passwordForm.confirm.length > 0 && passwordForm.new !== passwordForm.confirm
)

onMounted(async () => {
  if (!authStore.user?.id) return
  const data = await UsersService.getById(authStore.user.id)
  user.value = data
  Object.assign(form, {
    firstName: data.firstName,
    lastName: data.lastName,
    email: data.email,
  })
})

function resetForm() {
  Object.assign(form, {
    firstName: user.value.firstName,
    lastName: user.value.lastName,
    email: user.value.email,
  })
  saveError.value = ''
}

function resetPasswordForm() {
  Object.assign(passwordForm, { current: '', new: '', confirm: '' })
  passwordError.value = ''
  passwordSuccess.value = false
}

async function handleSave() {
  loading.value = true
  saveError.value = ''
  try {
    const updated = await UsersService.update(authStore.user.id, {
      firstName: form.firstName,
      lastName: form.lastName,
      email: form.email,
    })
    user.value = updated
    authStore.updateUserInfo(updated)
  } catch {
    saveError.value = 'No se pudieron guardar los cambios. Intenta de nuevo.'
  } finally {
    loading.value = false
  }
}

async function handlePasswordChange() {
  if (passwordMismatch.value) return
  loadingPassword.value = true
  passwordError.value = ''
  passwordSuccess.value = false
  try {
    await UsersService.changePassword(authStore.user.id, {
      currentPassword: passwordForm.current,
      newPassword: passwordForm.new,
    })
    passwordSuccess.value = true
    resetPasswordForm()
  } catch {
    passwordError.value = 'No se pudo cambiar la contraseña. Verifica tu contraseña actual.'
  } finally {
    loadingPassword.value = false
  }
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
  background: var(--white);
  border-radius: 10px;
  border: 1px solid var(--border);
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  box-shadow: var(--shadow-xs);
}

.avatar-wrapper {
  position: relative;
  margin-bottom: 0.25rem;
}

.avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: var(--blue);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--white);
}

.profile-name {
  font-size: 1rem;
  font-weight: 700;
  color: var(--text);
}

.role-badge {
  background: var(--purple-bg);
  color: var(--purple);
  font-size: 0.8rem;
  font-weight: 500;
  padding: 3px 12px;
  border-radius: 999px;
}

.profile-email {
  font-size: 0.83rem;
  color: var(--text-3);
}

/* Panel derecho */
.right-panel {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

/* Tabs */
.tabs-bar {
  display: flex;
  border-bottom: 1px solid var(--border);
}

.tab {
  padding: 0.75rem 1.25rem;
  border: none;
  background: none;
  font-size: 0.925rem;
  font-family: 'Inter', sans-serif;
  color: var(--text-2);
  cursor: pointer;
  border-bottom: 2px solid transparent;
  transition: all 0.2s;
  margin-bottom: -1px;
}

.tab:hover { color: var(--text); }
.tab.active { color: var(--blue-hover); border-bottom-color: var(--blue-hover); font-weight: 600; }

/* Tab card */
.tab-card {
  background: var(--white);
  border-radius: 10px;
  border: 1px solid var(--border);
  padding: 1.5rem;
  box-shadow: var(--shadow-xs);
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.section-title {
  font-size: 0.75rem;
  font-weight: 600;
  color: var(--text-3);
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
  font-size: 0.87rem;
  font-weight: 500;
  color: var(--text-2);
}

.form-input {
  padding: 0.6rem 0.75rem;
  border: 1px solid var(--border-strong);
  border-radius: 8px;
  font-size: 0.925rem;
  font-family: 'Inter', sans-serif;
  color: var(--text);
  outline: none;
  transition: border-color 0.2s;
  background: var(--white);
}

.form-input:focus {
  border-color: var(--blue-hover);
  box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
}

.form-input:disabled {
  background: var(--surface-hover);
  color: var(--text-3);
  cursor: not-allowed;
}

.error-msg {
  font-size: 0.83rem;
  color: var(--red);
}

.success-msg {
  font-size: 0.83rem;
  color: var(--mint-dark);
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
  border: 1px solid var(--border);
  border-radius: 8px;
  background: var(--white);
  font-size: 0.925rem;
  font-family: 'Inter', sans-serif;
  color: var(--text-2);
  cursor: pointer;
}

.btn-cancel:hover { background: var(--surface-hover); }

.btn-submit {
  padding: 0.6rem 1.25rem;
  background: var(--blue);
  color: var(--white);
  border: none;
  border-radius: 8px;
  font-size: 0.925rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-submit:hover:not(:disabled) { background: var(--blue-hover); }
.btn-submit:disabled { opacity: 0.6; cursor: not-allowed; }
</style>
