<template>
  <div class="users">

    <!-- Header -->
    <div class="page-header">
      <h1>Gestión de usuarios</h1>
      <button v-if="!isArchived" class="btn primary" @click="openCreate">
        + Nuevo usuario
      </button>
    </div>

    <!-- Toggle Activos / Archivados -->
    <div class="view-toggle">
      <button :class="{ active: !isArchived }" @click="setView('active')">Activos</button>
      <button :class="{ active: isArchived }" @click="setView('archived')">Archivados</button>
    </div>

    <!-- KPIs -->
    <div v-if="!isArchived" class="kpi-grid" style="grid-template-columns:repeat(4,1fr)">
      <div class="kpi">
        <div class="kpi-label">Total usuarios</div>
        <div class="kpi-val">{{ users.length }}</div>
        <div class="kpi-icon blue"><Users :size="16" /></div>
      </div>
      <div class="kpi">
        <div class="kpi-label">Activos</div>
        <div class="kpi-val">{{ activeCount }}</div>
        <div class="kpi-icon green"><UserCheck :size="16" /></div>
      </div>
      <div class="kpi">
        <div class="kpi-label">Inactivos</div>
        <div class="kpi-val">{{ inactiveCount }}</div>
        <div class="kpi-icon red"><UserX :size="16" /></div>
      </div>
      <div class="kpi">
        <div class="kpi-label">Administradores</div>
        <div class="kpi-val">{{ adminCount }}</div>
        <div class="kpi-icon purple"><ShieldCheck :size="16" /></div>
      </div>
    </div>

    <!-- Filtros -->
    <div class="search-row">
      <input v-model="search" class="search-input" placeholder="Buscar por nombre o correo…" />
      <select v-model="roleFilter">
        <option value="">Todos los roles</option>
        <option value="Administrador">Administrador</option>
        <option value="Supervisor">Supervisor</option>
        <option value="Operador">Operador</option>
      </select>
      <select v-if="!isArchived" v-model="statusFilter">
        <option value="">Todos los estados</option>
        <option value="Activo">Activo</option>
        <option value="Inactivo">Inactivo</option>
      </select>
    </div>

    <p v-if="loadError" class="alert red">{{ loadError }}</p>

    <!-- Tabla -->
    <div class="table-wrap">
      <table>
        <thead>
          <tr>
            <th>Usuario</th>
            <th>Correo electrónico</th>
            <th>Rol</th>
            <th>Estado</th>
            <th>Acciones</th>
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
            <td class="muted">{{ u.email }}</td>
            <td>
              <span class="role-badge" :class="u.role.toLowerCase()">
                {{ u.role }}
              </span>
            </td>
            <td>
              <span v-if="isArchived" class="badge inactivo">• Archivado</span>
              <span v-else class="badge" :class="u.isActive ? 'activo' : 'inactivo'">
                • {{ u.isActive ? 'Activo' : 'Inactivo' }}
              </span>
            </td>
            <td>
              <div class="action-buttons">
                <template v-if="!isArchived">
                  <button class="icon-btn edit" title="Editar" @click="openEdit(u)">
                    <Pencil :size="14" />
                  </button>
                  <button class="icon-btn" title="Enviar enlace para restablecer contraseña"
                    @click="handleSendReset(u)">
                    <KeyRound :size="14" />
                  </button>
                  <button class="icon-btn" :title="u.isActive ? 'Desactivar' : 'Activar'"
                    @click="handleToggleActive(u)">
                    <Ban v-if="u.isActive" :size="14" />
                    <Check v-else :size="14" />
                  </button>
                  <button class="icon-btn danger" title="Eliminar" @click="confirmDelete(u)">
                    <Trash2 :size="14" />
                  </button>
                </template>
                <button v-else class="icon-btn restore" title="Restaurar" @click="restoreUser(u)">
                  <RotateCcw :size="14" />
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
              <label>Nombre</label>
              <input v-model="form.firstName" type="text"
                placeholder="ej: Michael" />
            </div>

            <div class="form-group">
              <label>Apellido</label>
              <input v-model="form.lastName" type="text"
                placeholder="ej: Admin" />
            </div>

            <div class="form-group">
              <label>Rol</label>
              <select v-model="form.role" :disabled="!!editingUser">
                <option value="">Seleccionar rol</option>
                <option value="Administrador">Administrador</option>
                <option value="Supervisor">Supervisor</option>
                <option value="Operador">Operador</option>
              </select>
            </div>

            <div class="form-group">
              <label>Correo electrónico</label>
              <input v-model="form.email" type="email"
                placeholder="ej: usuario@empresa.com" />
            </div>

            <div v-if="!editingUser" class="form-group full-width">
              <label>Contraseña temporal</label>
              <input v-model="form.password" type="password"
                placeholder="Mín. 6 caracteres" />
            </div>

          </div>

          <p v-if="modalError" class="error-msg">{{ modalError }}</p>
        </div>

        <div class="modal-footer">
          <button class="btn" @click="closeModal">Cancelar</button>
          <button class="btn primary" @click="handleSave" :disabled="loading">
            {{ loading ? 'Guardando...' : editingUser ? 'Guardar cambios' : 'Crear usuario' }}
          </button>
        </div>
      </div>
    </div>

    <ModalConfirm
      v-model="showDeleteConfirm"
      title="Eliminar usuario"
      :message="`${deleteTarget?.firstName} ${deleteTarget?.lastName} se archivará: no podrá iniciar sesión y dejará de aparecer en el listado. Su historial de acciones se conserva.`"
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
import { Users, UserCheck, UserX, ShieldCheck, Pencil, KeyRound, Ban, Check, Trash2, RotateCcw } from '@lucide/vue'

const search = ref('')
const roleFilter = ref('')
const statusFilter = ref('')
const viewMode = ref('active') // 'active' | 'archived'
const isArchived = computed(() => viewMode.value === 'archived')
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
    users.value = await UsersService.getAll(isArchived.value)
  } catch {
    loadError.value = 'No se pudieron cargar los usuarios.'
  }
}

onMounted(loadUsers)

function setView(mode) {
  if (viewMode.value === mode) return
  viewMode.value = mode
  search.value = ''
  roleFilter.value = ''
  statusFilter.value = ''
  loadUsers()
}

async function restoreUser(u) {
  try {
    await UsersService.restore(u.id)
    await loadUsers()
  } catch {
    loadError.value = 'No se pudo restaurar el usuario.'
  }
}

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
  font-family: 'Inter', sans-serif;
}

/* Toggle Activos / Archivados */
.view-toggle {
  display: inline-flex;
  gap: 2px;
  padding: 3px;
  margin-bottom: 14px;
  background: var(--surface-2, #eef1f5);
  border-radius: 8px;
}
.view-toggle button {
  border: none;
  background: transparent;
  padding: 6px 16px;
  font-size: 13px;
  font-weight: 600;
  color: var(--text-2);
  border-radius: 6px;
  cursor: pointer;
  transition: background 0.15s, color 0.15s;
}
.view-toggle button.active {
  background: var(--white);
  color: var(--blue);
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.08);
}
.icon-btn.restore { color: var(--blue); }

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
  font-size: 0.8rem;
  font-weight: 700;
  color: var(--white);
}

.user-name {
  font-size: 0.925rem;
  font-weight: 600;
  color: var(--text);
}

/* Role badges */
.role-badge {
  display: inline-flex;
  padding: 3px 10px;
  border-radius: 999px;
  font-size: 0.8rem;
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
  font-size: 0.8rem;
  font-weight: 500;
  border: 1px solid transparent;
}

.badge.activo   { background: var(--mint-bg); color: var(--mint-dark); border-color: var(--mint-border); }
.badge.inactivo { background: var(--surface-hover); color: var(--text-3); border-color: var(--border); }

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

/* form-grid, form-group e inputs heredan del sistema global (main.css) */
.form-grid { margin-bottom: 0; }

.error-msg {
  font-size: 0.83rem;
  color: var(--red);
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 1rem 1.5rem;
  border-top: 1px solid var(--border);
}

</style>
