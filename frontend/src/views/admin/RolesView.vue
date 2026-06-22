<template>
  <div class="roles">

    <!-- Alerta -->
    <div class="alert-banner">
      <span>⚠️</span>
      <span>
        <strong>Los cambios en permisos</strong>
        tendrán efecto en el próximo inicio de sesión de los usuarios afectados.
      </span>
    </div>

    <!-- Cards de roles -->
    <div class="roles-grid">
      <div
        v-for="role in roles"
        :key="role.key"
        class="role-card"
        :class="{ active: selectedRole === role.key }"
        @click="selectedRole = role.key"
      >
        <span class="role-emoji">{{ role.emoji }}</span>
        <span class="role-name" :class="{ active: selectedRole === role.key }">
          {{ role.name }}
        </span>
        <span class="role-count">{{ role.count }} usuarios</span>
      </div>
    </div>

    <!-- Tabla de permisos -->
    <div class="permissions-card">
      <div class="permissions-header">
        <div>
          <div class="permissions-title">
            PERMISOS DEL ROL:
            <span :class="selectedRoleData?.color">{{ selectedRoleData?.name.toUpperCase() }}</span>
          </div>
          <div class="permissions-subtitle">{{ selectedRoleData?.description }}</div>
        </div>
        <span v-if="selectedRoleData?.restricted" class="restricted-badge">
          Rol de sistema — edición restringida
        </span>
      </div>

      <div class="table-wrapper">
        <table class="table">
          <thead>
            <tr>
              <th>MÓDULO</th>
              <th>VER</th>
              <th>CREAR</th>
              <th>EDITAR</th>
              <th>ELIMINAR</th>
              <th>APROBAR</th>
              <th>REPORTES</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="module in currentPermissions" :key="module.name">
              <td class="td-module">{{ module.name }}</td>
              <td><PermIcon :value="module.ver"      :editable="!selectedRoleData?.restricted" /></td>
              <td><PermIcon :value="module.crear"    :editable="!selectedRoleData?.restricted" /></td>
              <td><PermIcon :value="module.editar"   :editable="!selectedRoleData?.restricted" /></td>
              <td><PermIcon :value="module.eliminar" :editable="!selectedRoleData?.restricted" /></td>
              <td><PermIcon :value="module.aprobar"  :editable="!selectedRoleData?.restricted" /></td>
              <td><PermIcon :value="module.reportes" :editable="!selectedRoleData?.restricted" /></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, defineComponent, h } from 'vue'

const PermIcon = defineComponent({
  props: {
    value:    { type: Boolean, default: false },
    editable: { type: Boolean, default: false },
  },
  setup(props) {
    return () => props.value
      ? h('div', { class: 'perm-check' },
          h('svg', {
            xmlns: 'http://www.w3.org/2000/svg',
            width: 14, height: 14,
            viewBox: '0 0 24 24',
            fill: 'none',
            stroke: 'white',
            'stroke-width': 2.5,
          }, [
            h('polyline', { points: '20 6 9 17 4 12' })
          ])
        )
      : h('span', { class: 'perm-dash' }, '—')
  }
})

const selectedRole = ref('admin')

const roles = ref([
  { key: 'admin',      name: 'Administrador', emoji: '👑', count: 2, color: 'purple', description: 'Acceso total al sistema',               restricted: true  },
  { key: 'supervisor', name: 'Supervisor',     emoji: '🔍', count: 3, color: 'blue',   description: 'Puede aprobar solicitudes y asignar',   restricted: false },
  { key: 'operator',   name: 'Operador',       emoji: '✏️', count: 7, color: 'gray',   description: 'Puede registrar solicitudes y ver info', restricted: false },
])

const selectedRoleData = computed(() =>
  roles.value.find(r => r.key === selectedRole.value)
)

const permissionsMap = {
  admin: [
    { name: 'Dashboard',    ver: true,  crear: false, editar: false, eliminar: false, aprobar: false, reportes: true  },
    { name: 'Vehículos',    ver: true,  crear: true,  editar: true,  eliminar: true,  aprobar: false, reportes: true  },
    { name: 'Conductores',  ver: true,  crear: true,  editar: true,  eliminar: true,  aprobar: false, reportes: true  },
    { name: 'Solicitudes',  ver: true,  crear: true,  editar: true,  eliminar: true,  aprobar: true,  reportes: true  },
    { name: 'Asignaciones', ver: true,  crear: true,  editar: true,  eliminar: true,  aprobar: false, reportes: false },
    { name: 'Mantenimiento',ver: true,  crear: true,  editar: true,  eliminar: true,  aprobar: false, reportes: true  },
    { name: 'Reportes',     ver: true,  crear: false, editar: false, eliminar: false, aprobar: false, reportes: true  },
    { name: 'Usuarios',     ver: true,  crear: true,  editar: true,  eliminar: true,  aprobar: false, reportes: false },
  ],
  supervisor: [
    { name: 'Dashboard',    ver: true,  crear: false, editar: false, eliminar: false, aprobar: false, reportes: true  },
    { name: 'Vehículos',    ver: true,  crear: true,  editar: true,  eliminar: false, aprobar: false, reportes: true  },
    { name: 'Conductores',  ver: true,  crear: true,  editar: true,  eliminar: false, aprobar: false, reportes: true  },
    { name: 'Solicitudes',  ver: true,  crear: true,  editar: true,  eliminar: false, aprobar: true,  reportes: true  },
    { name: 'Asignaciones', ver: true,  crear: true,  editar: true,  eliminar: false, aprobar: false, reportes: false },
    { name: 'Mantenimiento',ver: true,  crear: false, editar: false, eliminar: false, aprobar: false, reportes: true  },
    { name: 'Reportes',     ver: true,  crear: false, editar: false, eliminar: false, aprobar: false, reportes: true  },
    { name: 'Usuarios',     ver: false, crear: false, editar: false, eliminar: false, aprobar: false, reportes: false },
  ],
  operator: [
    { name: 'Dashboard',    ver: true,  crear: false, editar: false, eliminar: false, aprobar: false, reportes: false },
    { name: 'Vehículos',    ver: true,  crear: false, editar: false, eliminar: false, aprobar: false, reportes: false },
    { name: 'Conductores',  ver: false, crear: false, editar: false, eliminar: false, aprobar: false, reportes: false },
    { name: 'Solicitudes',  ver: true,  crear: true,  editar: false, eliminar: false, aprobar: false, reportes: false },
    { name: 'Asignaciones', ver: false, crear: false, editar: false, eliminar: false, aprobar: false, reportes: false },
    { name: 'Mantenimiento',ver: false, crear: false, editar: false, eliminar: false, aprobar: false, reportes: false },
    { name: 'Reportes',     ver: false, crear: false, editar: false, eliminar: false, aprobar: false, reportes: false },
    { name: 'Usuarios',     ver: false, crear: false, editar: false, eliminar: false, aprobar: false, reportes: false },
  ],
}

const currentPermissions = computed(() => permissionsMap[selectedRole.value] || [])
</script>

<style scoped>
.roles {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  font-family: 'Inter', sans-serif;
}

/* Alerta */
.alert-banner {
  background: var(--amber-bg);
  border: 1px solid var(--amber-border);
  border-radius: 8px;
  padding: 0.875rem 1rem;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.875rem;
  color: var(--amber-text);
}

/* Roles grid */
.roles-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
}

.role-card {
  background: var(--white);
  border: 1px solid var(--border);
  border-radius: 10px;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.4rem;
  cursor: pointer;
  transition: all 0.2s;
}

.role-card:hover { border-color: var(--blue-hover); }

.role-card.active {
  border-color: var(--purple);
  background: var(--purple-bg);
  box-shadow: 0 0 0 1px var(--purple);
}

.role-emoji { font-size: 1.75rem; }

.role-name {
  font-size: 1rem;
  font-weight: 600;
  color: var(--text-2);
}

.role-name.active { color: var(--purple); }

.role-count {
  font-size: 0.78rem;
  color: var(--text-3);
}

/* Permisos */
.permissions-card {
  background: var(--white);
  border-radius: 10px;
  border: 1px solid var(--border);
  overflow: hidden;
  box-shadow: var(--shadow-xs);
}

.permissions-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid var(--border);
}

.permissions-title {
  font-size: 0.75rem;
  font-weight: 600;
  color: var(--text-3);
  letter-spacing: 0.08em;
  margin-bottom: 0.25rem;
}

.permissions-title .purple { color: var(--purple); }
.permissions-title .blue   { color: var(--blue-hover); }
.permissions-title .gray   { color: var(--text-2); }

.permissions-subtitle {
  font-size: 0.82rem;
  color: var(--text-2);
}

.restricted-badge {
  background: var(--purple-bg);
  color: var(--purple);
  font-size: 0.75rem;
  font-weight: 500;
  padding: 4px 12px;
  border-radius: 999px;
  border: 1px solid var(--purple-border);
  white-space: nowrap;
}

/* Tabla */
.table-wrapper { overflow: hidden; }

.table { width: 100%; border-collapse: collapse; }

.table th {
  text-align: center;
  font-size: 0.68rem;
  font-weight: 600;
  color: var(--text-3);
  letter-spacing: 0.05em;
  padding: 0.75rem 1rem;
  border-bottom: 1px solid var(--border);
}

.table th:first-child { text-align: left; }

.table td {
  padding: 0.875rem 1rem;
  font-size: 0.875rem;
  color: var(--text-2);
  border-bottom: 1px solid var(--surface-hover);
  text-align: center;
}

.table td:first-child { text-align: left; }
.table tbody tr:last-child td { border-bottom: none; }
.table tbody tr:hover { background: var(--surface-hover); }

.td-module {
  font-weight: 500;
  color: var(--text);
  text-align: left;
}

/* Perm icons */
:deep(.perm-check) {
  width: 22px;
  height: 22px;
  background: var(--mint-dark);
  border-radius: 5px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto;
}

:deep(.perm-dash) {
  color: var(--border-strong);
  font-size: 1rem;
  display: block;
  text-align: center;
}
</style>