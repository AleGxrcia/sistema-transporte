<template>
  <div class="users">

    <!-- Header -->
    <div class="page-header">
      <h2 class="page-title">Gestión de usuarios</h2>
      <button class="btn-primary" @click="showModal = true">
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
        <div class="kpi-value">12</div>
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
        <div class="kpi-value">10</div>
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
        <div class="kpi-value">2</div>
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
        <div class="kpi-value">2</div>
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

    <!-- Tabla -->
    <div class="table-wrapper">
      <table class="table">
        <thead>
          <tr>
            <th>USUARIO</th>
            <th>CORREO ELECTRÓNICO</th>
            <th>ROL</th>
            <th>ÚLTIMO ACCESO</th>
            <th>ESTADO</th>
            <th>ACCIONES</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in filteredUsers" :key="user.id">
            <td>
              <div class="user-cell">
                <div class="user-avatar" :style="{ background: user.avatarColor }">
                  {{ user.initials }}
                </div>
                <div>
                  <p class="user-name">{{ user.name }}</p>
                  <p class="user-role-label">{{ user.position }}</p>
                </div>
              </div>
            </td>
            <td class="td-gray">{{ user.email }}</td>
            <td>
              <span class="role-badge" :class="user.role.toLowerCase()">
                {{ user.role }}
              </span>
            </td>
            <td class="td-gray">{{ user.lastAccess }}</td>
            <td>
              <span class="badge" :class="user.status === 'Activo' ? 'activo' : 'inactivo'">
                • {{ user.status }}
              </span>
            </td>
            <td>
              <div class="actions">
                <button class="action-btn" title="Editar" @click="openEdit(user)">
                  <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13"
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
                    <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
                  </svg>
                </button>
                <button class="action-btn" title="Cambiar contraseña">
                  <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13"
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/>
                    <path d="M7 11V7a5 5 0 0 1 10 0v4"/>
                  </svg>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Paginación -->
    <div class="pagination">
      <span class="pagination-info">
        Mostrando 1-{{ filteredUsers.length }} de {{ users.length }} usuarios
      </span>
      <div class="pagination-btns">
        <button class="page-btn" disabled>← Anterior</button>
        <button class="page-btn">Siguiente →</button>
      </div>
    </div>

    <!-- editar usuario -->
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
              <select v-model="form.role" class="form-input">
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

            <div class="form-group">
              <label class="form-label">Teléfono</label>
              <input v-model="form.phone" type="text"
                class="form-input" placeholder="ej: 809-555-0001" />
            </div>

            <div class="form-group">
              <label class="form-label">Departamento</label>
              <input v-model="form.department" type="text"
                class="form-input" placeholder="ej: Tecnología" />
            </div>

            <div class="form-group full-width">
              <label class="form-label">Cargo</label>
              <input v-model="form.position" type="text"
                class="form-input" placeholder="ej: Administrador del Sistema" />
            </div>

          </div>
        </div>

        <div class="modal-footer">
          <button class="btn-cancel" @click="closeModal">Cancelar</button>
          <button class="btn-submit" @click="handleSave" :disabled="loading">
            {{ loading ? 'Guardando...' : editingUser ? 'Guardar cambios' : 'Crear usuario' }}
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'

const search = ref('')
const roleFilter = ref('')
const statusFilter = ref('')
const showModal = ref(false)
const editingUser = ref(null)
const loading = ref(false)

const form = reactive({
  firstName: '', lastName: '', role: '',
  email: '', phone: '', department: '', position: ''
})

const users = ref([
  { id: 1, name: 'Michael Admin',    initials: 'MA', avatarColor: '#3b82f6', position: 'Administrador del sistema', email: 'michael@empresa.com', role: 'Administrador', lastAccess: 'Hoy, 09:42',      status: 'Activo'   },
  { id: 2, name: 'Laura Administradora', initials: 'LA', avatarColor: '#22c55e', position: 'Administradora',        email: 'laura@empresa.com',   role: 'Administrador', lastAccess: 'Ayer, 17:15',      status: 'Activo'   },
  { id: 3, name: 'Carlos Supervisor', initials: 'CS', avatarColor: '#06b6d4', position: 'Supervisor',               email: 'carlos@empresa.com',  role: 'Supervisor',    lastAccess: 'Hoy, 08:30',       status: 'Activo'   },
  { id: 4, name: 'Pedro Operador',    initials: 'PO', avatarColor: '#f59e0b', position: 'Operador',                 email: 'pedro@empresa.com',   role: 'Operador',      lastAccess: '26/05/26, 14:00',  status: 'Activo'   },
  { id: 5, name: 'Roberto Inactivo',  initials: 'RI', avatarColor: '#9ca3af', position: 'Operador',                 email: 'roberto@empresa.com', role: 'Operador',      lastAccess: '10/03/26, 11:22',  status: 'Inactivo' },
])

const filteredUsers = computed(() => {
  let result = users.value
  if (search.value) {
    const q = search.value.toLowerCase()
    result = result.filter(u =>
      u.name.toLowerCase().includes(q) ||
      u.email.toLowerCase().includes(q)
    )
  }
  if (roleFilter.value)   result = result.filter(u => u.role === roleFilter.value)
  if (statusFilter.value) result = result.filter(u => u.status === statusFilter.value)
  return result
})

function openEdit(user) {
  editingUser.value = user
  const [firstName, ...rest] = user.name.split(' ')
  Object.assign(form, {
    firstName, lastName: rest.join(' '),
    role: user.role, email: user.email,
    phone: '', department: '', position: user.position
  })
  showModal.value = true
}

function closeModal() {
  showModal.value = false
  editingUser.value = null
  Object.assign(form, {
    firstName: '', lastName: '', role: '',
    email: '', phone: '', department: '', position: ''
  })
}

async function handleSave() {
  loading.value = true
  try {
    await new Promise(r => setTimeout(r, 800))
    closeModal()
  } finally {
    loading.value = false
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
  color: #111827;
}

.btn-primary {
  background: #2563eb;
  color: #fff;
  border: none;
  border-radius: 8px;
  padding: 0.6rem 1.1rem;
  font-size: 0.875rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-primary:hover { background: #1d4ed8; }

/* KPIs */
.kpi-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.kpi-card {
  background: #fff;
  border-radius: 10px;
  padding: 1.1rem;
  border: 1px solid #f3f4f6;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
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
  color: #9ca3af;
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

.kpi-icon.blue   { background: #eff6ff; color: #2563eb; }
.kpi-icon.green  { background: #f0fdf4; color: #16a34a; }
.kpi-icon.red    { background: #fef2f2; color: #dc2626; }
.kpi-icon.purple { background: #f5f3ff; color: #7c3aed; }

.kpi-value {
  font-size: 1.75rem;
  font-weight: 700;
  color: #111827;
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
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 0.5rem 0.75rem;
  background: #fff;
  color: #9ca3af;
}

.search-input {
  border: none;
  outline: none;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #111827;
  width: 160px;
}

.filter-select {
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
  background: #fff;
  outline: none;
  min-width: 160px;
}

/* Tabla */
.table-wrapper {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.table { width: 100%; border-collapse: collapse; }

.table th {
  text-align: left;
  font-size: 0.7rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.05em;
  padding: 0.75rem 1rem;
  border-bottom: 1px solid #f3f4f6;
}

.table td {
  padding: 0.875rem 1rem;
  font-size: 0.85rem;
  color: #374151;
  border-bottom: 1px solid #f9fafb;
}

.table tbody tr:last-child td { border-bottom: none; }
.table tbody tr:hover { background: #f9fafb; }

.td-gray { color: #9ca3af; }

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
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: 700;
  color: #fff;
}

.user-name {
  font-size: 0.875rem;
  font-weight: 600;
  color: #111827;
}

.user-role-label {
  font-size: 0.75rem;
  color: #9ca3af;
  margin-top: 1px;
}

/* Role badges */
.role-badge {
  display: inline-flex;
  padding: 3px 10px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 500;
}

.role-badge.administrador { background: #f5f3ff; color: #7c3aed; }
.role-badge.supervisor    { background: #eff6ff; color: #2563eb; }
.role-badge.operador      { background: #fffbeb; color: #d97706; }

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

.badge.activo   { background: #f0fdf4; color: #16a34a; border-color: #bbf7d0; }
.badge.inactivo { background: #f9fafb; color: #9ca3af; border-color: #e5e7eb; }

/* Acciones */
.actions { display: flex; gap: 0.4rem; }

.action-btn {
  width: 28px;
  height: 28px;
  border-radius: 6px;
  border: 1px solid #e5e7eb;
  background: #fff;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #6b7280;
  transition: all 0.15s;
}

.action-btn:hover { background: #eff6ff; border-color: #2563eb; color: #2563eb; }

/* Paginación */
.pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.pagination-info { font-size: 0.8rem; color: #9ca3af; }
.pagination-btns { display: flex; gap: 0.5rem; }

.page-btn {
  padding: 0.4rem 0.875rem;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  background: #fff;
  font-size: 0.8rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
  cursor: pointer;
}

.page-btn:hover:not(:disabled) { background: #f9fafb; }
.page-btn:disabled { color: #d1d5db; cursor: not-allowed; }

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
  background: #fff;
  border-radius: 12px;
  width: 100%;
  max-width: 520px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 20px 60px rgba(0,0,0,0.2);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid #f3f4f6;
}

.modal-title {
  font-size: 1rem;
  font-weight: 700;
  color: #111827;
}

.modal-close {
  background: none;
  border: none;
  font-size: 1rem;
  color: #9ca3af;
  cursor: pointer;
}

.modal-body { padding: 1.5rem; }

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

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 1rem 1.5rem;
  border-top: 1px solid #f3f4f6;
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
</style>