<template>
  <div class="users">

    <!-- Header -->
    <div class="page-header">
      <h2 class="page-title">Gestión de usuarios</h2>
      <button class="btn-primary" @click="openCreate">
        + Nuevo usuario
      </button>
    </div>

    <!-- KPIs -->
    <div class="kpi-grid">
      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">TOTAL USUARIOS</span>
          <div class="kpi-icon blue">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
              <circle cx="12" cy="7" r="4"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">{{ users.length }}</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">ACTIVOS</span>
          <div class="kpi-icon green">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="20 6 9 17 4 12"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">{{ activeCount }}</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">INACTIVOS</span>
          <div class="kpi-icon red">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <circle cx="12" cy="12" r="10"/>
              <line x1="4.93" y1="4.93" x2="19.07" y2="19.07"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">{{ inactiveCount }}</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">ADMINISTRADORES</span>
          <div class="kpi-icon purple">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/>
              <path d="M7 11V7a5 5 0 0 1 10 0v4"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">{{ adminCount }}</div>
      </div>
    </div>

    <!-- Filtros -->
    <div class="filters">
      <div class="search-box">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
          viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <circle cx="11" cy="11" r="8"/>
          <line x1="21" y1="21" x2="16.65" y2="16.65"/>
        </svg>
        <input v-model="search" type="text"
          placeholder="Buscar..." class="search-input" />
      </div>
      <select v-model="roleFilter" class="filter-select">
        <option value="">Todos los roles</option>
        <option value="Administrador">Administrador</option>
        <option value="Supervisor">Supervisor</option>
        <option value="Operador">Operador</option>
      </select>
      <select v-model="statusFilter" class="filter-select">
        <option value="">Todos los estados</option>
        <option value="Activo">Activo</option>
        <option value="Inactivo">Inactivo</option>
      </select>
    </div>

    <p v-if="loadError" class="error-banner">{{ loadError }}</p>

    <!-- Tabla -->
    <div class="table-wrapper">
      <table class="table">
        <thead>
          <tr>
            <th>USUARIO</th>
            <th>CORREO ELECTRÓNICO</th>
            <th>ROL</th>
            <th>ESTADO</th>
            <th>ACCIONES</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="u in filteredUsers" :key="u.id">
            <td>
              <div class="user-cell">
                <div class="user-avatar">
                  {{ initialsOf(u) }}
                </div>
                <p class="user-name">{{ u.firstName }} {{ u.lastName }}</p>
              </div>
            </td>
            <td class="td-gray">{{ u.email }}</td>
            <td>
              <span class="role-badge" :class="u.role.toLowerCase()">
                {{ u.role }}
              </span>
            </td>
            <td>
              <span class="badge" :class="u.isActive ? 'activo' : 'inactivo'">
                • {{ u.isActive ? 'Activo' : 'Inactivo' }}
              </span>
            </td>
            <td>
              <div class="actions">
                <button class="action-btn" title="Editar" @click="openEdit(u)">
                  <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13"
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
                    <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
                  </svg>
                </button>
                <button class="action-btn" title="Enviar enlace para restablecer contraseña"
                  @click="handleSendReset(u)">
                  <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13"
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/>
                    <path d="M7 11V7a5 5 0 0 1 10 0v4"/>
                  </svg>
                </button>
                <button class="action-btn" :title="u.isActive ? 'Desactivar' : 'Activar'"
                  @click="handleToggleActive(u)">
                  <svg v-if="u.isActive" xmlns="http://www.w3.org/2000/svg" width="13" height="13"
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <circle cx="12" cy="12" r="10"/><line x1="4.93" y1="4.93" x2="19.07" y2="19.07"/>
                  </svg>
                  <svg v-else xmlns="http://www.w3.org/2000/svg" width="13" height="13"
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <polyline points="20 6 9 17 4 12"/>
                  </svg>
                </button>
                <button class="action-btn danger" title="Eliminar" @click="confirmDelete(u)">
                  <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13"
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <polyline points="3 6 5 6 21 6"/>
                    <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/>
                  </svg>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- crear/editar usuario -->
    <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
      <div class="modal">
        <div class="modal-header">
          <h3 class="modal-title">
            {{ editingUser ? 'Editar usuario' : 'Nuevo usuario' }}
          </h3>
          <button class="modal-close" @click="closeModal">✕</button>
        </div>

        <div class="modal-body">
          <div class="form-grid">

            <div class="form-group">
              <label class="form-label">Nombre</label>
              <input v-model="form.firstName" type="text"
                class="form-input" placeholder="ej: Michael" />
            </div>

            <div class="form-group">
              <label class="form-label">Apellido</label>
              <input v-model="form.lastName" type="text"
                class="form-input" placeholder="ej: Admin" />
            </div>

            <div class="form-group">
              <label class="form-label">Rol</label>
              <select v-model="form.role" class="form-input" :disabled="!!editingUser">
                <option value="">Seleccionar rol</option>
                <option value="Administrador">Administrador</option>
                <option value="Supervisor">Supervisor</option>
                <option value="Operador">Operador</option>
              </select>
            </div>

            <div class="form-group">
              <label class="form-label">Correo electrónico</label>
              <input v-model="form.email" type="email"
                class="form-input" placeholder="ej: usuario@empresa.com" />
            </div>

            <div v-if="!editingUser" class="form-group full-width">
              <label class="form-label">Contraseña temporal</label>
              <input v-model="form.password" type="password"
                class="form-input" placeholder="Mín. 6 caracteres" />
            </div>

          </div>

          <p v-if="modalError" class="error-msg">{{ modalError }}</p>
        </div>

        <div class="modal-footer">
          <button class="btn-cancel" @click="closeModal">Cancelar</button>
          <button class="btn-submit" @click="handleSave" :disabled="loading">
            {{ loading ? 'Guardando...' : editingUser ? 'Guardar cambios' : 'Crear usuario' }}
          </button>
        </div>
      </div>
    </div>

    <ModalConfirm
      v-model="showDeleteConfirm"
      title="Eliminar usuario"
      :message="`¿Seguro que deseas eliminar a ${deleteTarget?.firstName} ${deleteTarget?.lastName}? Esta acción no se puede deshacer.`"
      confirm-text="Eliminar"
      variant="danger"
      :is-loading="deleting"
      @confirm="handleDelete"
    />

  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import UsersService from '@/services/users.service.js'
import AuthService from '@/services/auth.service.js'
import ModalConfirm from '@/components/modals/ModalConfirm.vue'

const search = ref('')
const roleFilter = ref('')
const statusFilter = ref('')
const showModal = ref(false)
const editingUser = ref(null)
const loading = ref(false)
const modalError = ref('')
const loadError = ref('')

const users = ref([])

const form = reactive({
  firstName: '', lastName: '', role: '', email: '', password: '',
})

const showDeleteConfirm = ref(false)
const deleteTarget = ref(null)
const deleting = ref(false)

const activeCount = computed(() => users.value.filter(u => u.isActive).length)
const inactiveCount = computed(() => users.value.filter(u => !u.isActive).length)
const adminCount = computed(() => users.value.filter(u => u.role === 'Administrador').length)

const filteredUsers = computed(() => {
  let result = users.value
  if (search.value) {
    const q = search.value.toLowerCase()
    result = result.filter(u =>
      `${u.firstName} ${u.lastName}`.toLowerCase().includes(q) ||
      u.email.toLowerCase().includes(q)
    )
  }
  if (roleFilter.value) result = result.filter(u => u.role === roleFilter.value)
  if (statusFilter.value) {
    result = result.filter(u => (statusFilter.value === 'Activo') === u.isActive)
  }
  return result
})

function initialsOf(u) {
  return [u.firstName?.[0], u.lastName?.[0]].filter(Boolean).join('').toUpperCase()
}

async function loadUsers() {
  loadError.value = ''
  try {
    users.value = await UsersService.getAll()
  } catch {
    loadError.value = 'No se pudieron cargar los usuarios.'
  }
}

onMounted(loadUsers)

function openCreate() {
  editingUser.value = null
  Object.assign(form, { firstName: '', lastName: '', role: '', email: '', password: '' })
  modalError.value = ''
  showModal.value = true
}

function openEdit(u) {
  editingUser.value = u
  Object.assign(form, {
    firstName: u.firstName, lastName: u.lastName,
    role: u.role, email: u.email, password: '',
  })
  modalError.value = ''
  showModal.value = true
}

function closeModal() {
  showModal.value = false
  editingUser.value = null
}

async function handleSave() {
  loading.value = true
  modalError.value = ''
  try {
    if (editingUser.value) {
      await UsersService.update(editingUser.value.id, {
        firstName: form.firstName, lastName: form.lastName, email: form.email,
      })
    } else {
      await UsersService.create({
        firstName: form.firstName, lastName: form.lastName,
        email: form.email, password: form.password, role: form.role,
      })
    }
    await loadUsers()
    closeModal()
  } catch {
    modalError.value = 'No se pudo guardar el usuario. Verifica los datos e intenta de nuevo.'
  } finally {
    loading.value = false
  }
}

async function handleSendReset(u) {
  try {
    await AuthService.forgotPassword(u.email)
  } catch {
    loadError.value = 'No se pudo enviar el enlace de restablecimiento.'
  }
}

async function handleToggleActive(u) {
  try {
    if (u.isActive) await UsersService.deactivate(u.id)
    else await UsersService.activate(u.id)
    await loadUsers()
  } catch {
    loadError.value = 'No se pudo actualizar el estado del usuario.'
  }
}

function confirmDelete(u) {
  deleteTarget.value = u
  showDeleteConfirm.value = true
}

async function handleDelete() {
  if (!deleteTarget.value) return
  deleting.value = true
  try {
    await UsersService.remove(deleteTarget.value.id)
    await loadUsers()
    showDeleteConfirm.value = false
  } catch {
    loadError.value = 'No se pudo eliminar el usuario.'
  } finally {
    deleting.value = false
  }
}
</script>

<style scoped>
.users {
  display: flex;
  flex-direction: column;
  gap: 0.8rem;
  font-family: 'Inter', sans-serif;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.page-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--text);
}

.btn-primary {
  background: var(--blue);
  color: var(--white);
  border: none;
  border-radius: 8px;
  padding: 0.6rem 1.1rem;
  font-size: 0.875rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-primary:hover { background: var(--blue-hover); }

/* KPIs */
.kpi-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.kpi-card {
  background: var(--white);
  border-radius: 10px;
  padding: 1.1rem;
  border: 1px solid var(--border);
  box-shadow: var(--shadow-xs);
  transition: box-shadow .15s, transform .15s;
}

.kpi-card:hover {
  box-shadow: var(--shadow-sm);
  transform: translateY(-1px);
}

.kpi-top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 0.5rem;
}

.kpi-label {
  font-size: 0.65rem;
  font-weight: 600;
  color: var(--text-3);
  letter-spacing: 0.05em;
}

.kpi-icon {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.kpi-icon.blue   { background: var(--blue-light); color: var(--blue-hover); }
.kpi-icon.green  { background: var(--mint-bg); color: var(--mint-dark); }
.kpi-icon.red    { background: var(--red-bg); color: var(--red); }
.kpi-icon.purple { background: var(--purple-bg); color: var(--purple); }

.kpi-value {
  font-size: 1.75rem;
  font-weight: 700;
  color: var(--text);
}

/* Filtros */
.filters {
  display: flex;
  gap: 0.75rem;
  align-items: center;
}

.search-box {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 0.5rem 0.75rem;
  background: var(--white);
  color: var(--text-3);
}

.search-input {
  border: none;
  outline: none;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: var(--text);
  width: 160px;
}

.filter-select {
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: var(--text-2);
  background: var(--white);
  outline: none;
  min-width: 160px;
}

.error-banner {
  background: var(--red-bg);
  border: 1px solid var(--red-border);
  color: var(--red);
  border-radius: 8px;
  padding: 0.6rem 0.875rem;
  font-size: 0.82rem;
}

/* Tabla */
.table-wrapper {
  background: var(--white);
  border-radius: 10px;
  border: 1px solid var(--border);
  overflow: hidden;
  box-shadow: var(--shadow-xs);
}

.table { width: 100%; border-collapse: collapse; }

.table th {
  text-align: left;
  font-size: 0.7rem;
  font-weight: 600;
  color: var(--text-3);
  letter-spacing: 0.05em;
  padding: 0.75rem 1rem;
  border-bottom: 1px solid var(--border);
}

.table td {
  padding: 0.875rem 1rem;
  font-size: 0.85rem;
  color: var(--text-2);
  border-bottom: 1px solid var(--surface-hover);
}

.table tbody tr:last-child td { border-bottom: none; }
.table tbody tr:hover { background: var(--surface-hover); }

.td-gray { color: var(--text-3); }

/* User cell */
.user-cell {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.user-avatar {
  width: 36px;
  height: 36px;
  min-width: 36px;
  border-radius: 50%;
  background: var(--blue);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: 700;
  color: var(--white);
}

.user-name {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--text);
}

/* Role badges */
.role-badge {
  display: inline-flex;
  padding: 3px 10px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 500;
}

.role-badge.administrador { background: var(--purple-bg); color: var(--purple); }
.role-badge.supervisor    { background: var(--blue-light); color: var(--blue-hover); }
.role-badge.operador      { background: var(--amber-bg); color: var(--amber-text); }

/* Status badges */
.badge {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 3px 10px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 500;
  border: 1px solid transparent;
}

.badge.activo   { background: var(--mint-bg); color: var(--mint-dark); border-color: var(--mint-border); }
.badge.inactivo { background: var(--surface-hover); color: var(--text-3); border-color: var(--border); }

/* Acciones */
.actions { display: flex; gap: 0.4rem; }

.action-btn {
  width: 28px;
  height: 28px;
  border-radius: 6px;
  border: 1px solid var(--border);
  background: var(--white);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-3);
  transition: all 0.15s;
}

.action-btn:hover { background: var(--blue-light); border-color: var(--blue-hover); color: var(--blue-hover); }
.action-btn.danger:hover { background: var(--red-bg); border-color: var(--red); color: var(--red); }

/* Modal */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 500;
}

.modal {
  background: var(--white);
  border-radius: 12px;
  width: 100%;
  max-width: 520px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: var(--shadow-md);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid var(--border);
}

.modal-title {
  font-size: 1rem;
  font-weight: 700;
  color: var(--text);
}

.modal-close {
  background: none;
  border: none;
  font-size: 1rem;
  color: var(--text-3);
  cursor: pointer;
}

.modal-body { padding: 1.5rem; display: flex; flex-direction: column; gap: 1rem; }

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-group { display: flex; flex-direction: column; gap: 0.3rem; }
.form-group.full-width { grid-column: 1 / -1; }

.form-label {
  font-size: 0.82rem;
  font-weight: 500;
  color: var(--text-2);
}

.form-input {
  padding: 0.6rem 0.75rem;
  border: 1px solid var(--border-strong);
  border-radius: 8px;
  font-size: 0.875rem;
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
  font-size: 0.78rem;
  color: var(--red);
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 1rem 1.5rem;
  border-top: 1px solid var(--border);
}

.btn-cancel {
  padding: 0.6rem 1.25rem;
  border: 1px solid var(--border);
  border-radius: 8px;
  background: var(--white);
  font-size: 0.875rem;
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
  font-size: 0.875rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-submit:hover:not(:disabled) { background: var(--blue-hover); }
.btn-submit:disabled { opacity: 0.6; cursor: not-allowed; }
</style>
