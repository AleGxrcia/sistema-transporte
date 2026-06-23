<template>
  <div class="notifications">

    <!-- Header acciones -->
    <div class="page-header">
      <div class="tabs">
        <button
          v-for="tab in tabs"
          :key="tab.key"
          class="tab"
          :class="{ active: activeTab === tab.key }"
          @click="activeTab = tab.key"
        >
          {{ tab.label }}
          <span v-if="tab.count" class="tab-count" :class="{ active: activeTab === tab.key }">
            {{ tab.count }}
          </span>
        </button>
      </div>
      <div class="header-actions">
        <button class="btn-secondary">Marcar todas como leídas</button>
        <button class="btn-danger-outline">Limpiar todo</button>
      </div>
    </div>

    <!-- Contenido: lista + detalle -->
    <div class="notif-layout">

      <!-- Lista izquierda -->
      <div class="notif-list">
        <div
          v-for="notif in filteredNotifications"
          :key="notif.id"
          class="notif-item"
          :class="{ selected: selectedId === notif.id, unread: notif.unread }"
          @click="selectedId = notif.id"
        >
          <div class="notif-icon-wrap" :style="{ background: notif.iconBg }">
            <span>{{ notif.icon }}</span>
          </div>
          <div class="notif-content">
            <p class="notif-title">{{ notif.title }}</p>
            <p class="notif-desc">{{ notif.description }}</p>
            <p class="notif-time">{{ notif.time }}</p>
          </div>
          <span v-if="notif.unread" class="unread-dot" :style="{ background: notif.dotColor }"></span>
        </div>
      </div>

      <!-- Detalle derecha -->
      <div class="notif-detail" v-if="selectedNotif">

        <!-- Header detalle -->
        <div class="detail-header" :class="selectedNotif.urgency">
          <div class="detail-icon-wrap" :style="{ background: selectedNotif.iconBg }">
            <span style="font-size:1.25rem">{{ selectedNotif.icon }}</span>
          </div>
          <div class="detail-header-info">
            <h3 class="detail-title" :class="selectedNotif.urgency">{{ selectedNotif.title }}</h3>
            <p class="detail-meta">{{ selectedNotif.meta }}</p>
          </div>
          <span v-if="selectedNotif.urgencyLabel" class="urgency-badge" :class="selectedNotif.urgency">
            • {{ selectedNotif.urgencyLabel }}
          </span>
        </div>

        <!-- Cuerpo -->
        <div class="detail-body">
          <p class="detail-text" v-html="selectedNotif.body"></p>

          <!-- Info vehículo afectado -->
          <div v-if="selectedNotif.vehicle" class="detail-vehicle">
            <div class="vehicle-title">VEHÍCULO AFECTADO</div>
            <div class="vehicle-grid">
              <div class="vehicle-item">
                <span class="v-label">MATRÍCULA</span>
                <span class="v-value">{{ selectedNotif.vehicle.plate }}</span>
              </div>
              <div class="vehicle-item">
                <span class="v-label">MODELO</span>
                <span class="v-value">{{ selectedNotif.vehicle.model }}</span>
              </div>
              <div class="vehicle-item">
                <span class="v-label">TIPO MANT.</span>
                <span class="v-value orange">{{ selectedNotif.vehicle.maintenanceType }}</span>
              </div>
              <div class="vehicle-item">
                <span class="v-label">FECHA PROG.</span>
                <span class="v-value red">{{ selectedNotif.vehicle.scheduledDate }}</span>
              </div>
              <div class="vehicle-item">
                <span class="v-label">TALLER</span>
                <span class="v-value">{{ selectedNotif.vehicle.shop }}</span>
              </div>
              <div class="vehicle-item">
                <span class="v-label">ESTADO</span>
                <span class="badge en-viaje">• {{ selectedNotif.vehicle.status }}</span>
              </div>
            </div>
          </div>

          <!-- Acciones -->
          <div class="detail-actions">
            <div class="actions-left">
              <button
                v-for="action in selectedNotif.actions"
                :key="action.label"
                class="btn-action"
                :class="action.style"
              >
                {{ action.label }}
              </button>
            </div>
            <button class="btn-discard">Descartar alerta</button>
          </div>
        </div>

      </div>

      <!-- Empty state si no hay selección -->
      <div v-else class="notif-empty">
        <p>Selecciona una notificación para ver el detalle</p>
      </div>

    </div>

  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const activeTab = ref('todas')
const selectedId = ref(1)

const tabs = [
  { key: 'todas',   label: 'Todas',   count: 8 },
  { key: 'alertas', label: 'Alertas', count: null },
  { key: 'info',    label: 'Info',    count: null },
  { key: 'sistema', label: 'Sistema', count: null },
]

const notifications = ref([
  {
    id: 1,
    title: 'Mantenimiento urgente',
    description: 'Honda CRV DEF-456 vence en 4 días',
    time: 'Hace 5 minutos',
    icon: '⚠️',
    iconBg: '#fff7ed',
    dotColor: '#ef4444',
    unread: true,
    type: 'alertas',
    urgency: 'urgent',
    urgencyLabel: 'Urgente',
    meta: 'Alerta generada automáticamente por el sistema · Hace 5 minutos',
    body: 'El vehículo <strong>Honda CRV — DEF-456</strong> tiene programado un mantenimiento correctivo en <span style="color:#ef4444"><strong>4 días</strong></span> (01/06/2026). Se recomienda coordinar con el taller <strong>Taller Honda</strong> para confirmar la cita.',
    vehicle: {
      plate: 'DEF-456',
      model: 'Honda CRV 2021',
      maintenanceType: 'Correctivo',
      scheduledDate: '01/06/2026',
      shop: 'Taller Honda',
      status: 'En viaje',
    },
    actions: [
      { label: 'Ver vehículo',          style: 'primary' },
      { label: 'Programar mantenimiento', style: 'secondary' },
    ],
  },
  {
    id: 2,
    title: 'Nueva solicitud pendiente',
    description: 'Área de Ventas — 29 mayo 2026',
    time: 'Hace 23 minutos',
    icon: '📋',
    iconBg: '#f5f3ff',
    dotColor: '#8b5cf6',
    unread: true,
    type: 'info',
    urgency: '',
    urgencyLabel: '',
    meta: 'Solicitud recibida · Hace 23 minutos',
    body: 'El área de <strong>Ventas</strong> ha enviado una nueva solicitud de transporte para el <strong>29 de mayo 2026</strong>. Requiere aprobación de supervisor.',
    vehicle: null,
    actions: [
      { label: 'Ver solicitud', style: 'primary' },
      { label: 'Aprobar',       style: 'secondary' },
    ],
  },
  {
    id: 3,
    title: 'Licencia próxima a vencer',
    description: 'Miguel Fernández — 12 días restantes',
    time: 'Hace 1 hora',
    icon: '📅',
    iconBg: '#fffbeb',
    dotColor: '#f59e0b',
    unread: true,
    type: 'alertas',
    urgency: 'warning',
    urgencyLabel: 'Advertencia',
    meta: 'Alerta de licencia · Hace 1 hora',
    body: 'La licencia del conductor <strong>Miguel Fernández</strong> vence en <strong style="color:#d97706">12 días</strong> (09/06/2026). Se recomienda iniciar el proceso de renovación.',
    vehicle: null,
    actions: [
      { label: 'Ver conductor', style: 'primary' },
    ],
  },
  {
    id: 4,
    title: 'Solicitud aprobada',
    description: 'Tu solicitud #003 fue aprobada',
    time: 'Hace 2 horas',
    icon: '✅',
    iconBg: '#f0fdf4',
    dotColor: '#22c55e',
    unread: true,
    type: 'info',
    urgency: '',
    urgencyLabel: '',
    meta: 'Aprobación de solicitud · Hace 2 horas',
    body: 'La solicitud <strong>#003</strong> del área de <strong>RRHH</strong> ha sido aprobada por el supervisor <strong>Carlos Supervisor</strong>. El vehículo ABC-123 y el conductor Juan Pérez han sido asignados.',
    vehicle: null,
    actions: [
      { label: 'Ver solicitud', style: 'primary' },
    ],
  },
  {
    id: 5,
    title: 'Viaje finalizado',
    description: 'RRHH → Aeropuerto completado',
    time: 'Hoy, 09:45',
    icon: '📈',
    iconBg: '#f3f4f6',
    dotColor: null,
    unread: false,
    type: 'info',
    urgency: '',
    urgencyLabel: '',
    meta: 'Viaje completado · Hoy, 09:45',
    body: 'El viaje del área <strong>RRHH</strong> al <strong>Aeropuerto Las Américas</strong> ha sido completado exitosamente. Conductor: Juan Pérez · Vehículo: ABC-123.',
    vehicle: null,
    actions: [
      { label: 'Ver historial', style: 'secondary' },
    ],
  },
  {
    id: 6,
    title: 'Consumo registrado',
    description: '45.5 gl cargados en ABC-123',
    time: 'Ayer, 16:20',
    icon: '⛽',
    iconBg: '#f3f4f6',
    dotColor: null,
    unread: false,
    type: 'sistema',
    urgency: '',
    urgencyLabel: '',
    meta: 'Registro de combustible · Ayer, 16:20',
    body: 'Se han registrado <strong>45.5 galones</strong> de combustible para el vehículo <strong>ABC-123 — Toyota Hiace</strong>. Costo total: RD$ 4,527.',
    vehicle: null,
    actions: [],
  },
  {
    id: 7,
    title: 'Solicitud rechazada',
    description: 'Marketing #006 — ver motivo',
    time: '26/05/26, 11:00',
    icon: '❌',
    iconBg: '#fef2f2',
    dotColor: null,
    unread: false,
    type: 'info',
    urgency: '',
    urgencyLabel: '',
    meta: 'Solicitud rechazada · 26/05/26, 11:00',
    body: 'La solicitud <strong>#006</strong> del área de <strong>Marketing</strong> fue rechazada. Motivo: fechas en conflicto con otros viajes programados.',
    vehicle: null,
    actions: [
      { label: 'Ver solicitud', style: 'secondary' },
    ],
  },
  {
    id: 8,
    title: 'Usuario creado',
    description: 'Pedro Operador agregado al sistema',
    time: '25/05/26, 09:15',
    icon: '👤',
    iconBg: '#f3f4f6',
    dotColor: null,
    unread: false,
    type: 'sistema',
    urgency: '',
    urgencyLabel: '',
    meta: 'Usuario creado · 25/05/26, 09:15',
    body: 'El usuario <strong>Pedro Operador</strong> ha sido creado exitosamente con rol de <strong>Operador</strong>. Las credenciales han sido enviadas a su correo electrónico.',
    vehicle: null,
    actions: [],
  },
])

const filteredNotifications = computed(() => {
  if (activeTab.value === 'todas') return notifications.value
  return notifications.value.filter(n => n.type === activeTab.value)
})

const selectedNotif = computed(() =>
  notifications.value.find(n => n.id === selectedId.value)
)
</script>

<style scoped>
.notifications {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  font-family: 'Inter', sans-serif;
  height: calc(100vh - 120px);
}

/* Header */
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.tabs { display: flex; gap: 0.5rem; }

.tab {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.4rem 1rem;
  border: 1px solid var(--border);
  border-radius: 999px;
  background: var(--white);
  font-size: 0.87rem;
  font-family: 'Inter', sans-serif;
  color: var(--text-2);
  cursor: pointer;
  transition: all 0.2s;
}

.tab.active {
  background: var(--blue-hover);
  color: var(--white);
  border-color: var(--blue-hover);
  font-weight: 600;
}

.tab-count {
  background: var(--bg);
  color: var(--text-2);
  font-size: 0.75rem;
  font-weight: 700;
  padding: 1px 6px;
  border-radius: 999px;
}

.tab-count.active {
  background: rgba(255,255,255,0.25);
  color: var(--white);
}

.header-actions { display: flex; gap: 0.5rem; }

.btn-secondary {
  padding: 0.5rem 1rem;
  border: 1px solid var(--border);
  border-radius: 8px;
  background: var(--white);
  font-size: 0.87rem;
  font-family: 'Inter', sans-serif;
  color: var(--text-2);
  cursor: pointer;
}

.btn-secondary:hover { background: var(--surface-hover); }

.btn-danger-outline {
  padding: 0.5rem 1rem;
  border: 1px solid var(--red-border);
  border-radius: 8px;
  background: var(--white);
  font-size: 0.87rem;
  font-family: 'Inter', sans-serif;
  color: var(--red);
  cursor: pointer;
}

.btn-danger-outline:hover { background: var(--red-bg); }

/* Layout */
.notif-layout {
  display: grid;
  grid-template-columns: 340px 1fr;
  gap: 0;
  flex: 1;
  background: var(--white);
  border-radius: 10px;
  border: 1px solid var(--border);
  overflow: hidden;
  box-shadow: var(--shadow-xs);
}

/* Lista */
.notif-list {
  border-right: 1px solid var(--border);
  overflow-y: auto;
}

.notif-item {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 1rem;
  cursor: pointer;
  border-bottom: 1px solid var(--surface-hover);
  transition: background 0.15s;
  position: relative;
}

.notif-item:hover { background: var(--surface-hover); }
.notif-item.selected { background: var(--blue-light); border-left: 3px solid var(--blue-hover); }
.notif-item.unread { background: #fafbff; }

.notif-icon-wrap {
  width: 36px;
  height: 36px;
  min-width: 36px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1rem;
}

.notif-content { flex: 1; min-width: 0; }

.notif-title {
  font-size: 0.87rem;
  font-weight: 600;
  color: var(--text);
  margin-bottom: 2px;
}

.notif-desc {
  font-size: 0.8rem;
  color: var(--text-2);
  margin-bottom: 2px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.notif-time {
  font-size: 0.75rem;
  color: var(--text-3);
}

.unread-dot {
  width: 8px;
  height: 8px;
  min-width: 8px;
  border-radius: 50%;
  margin-top: 4px;
}

/* Detalle */
.notif-detail {
  display: flex;
  flex-direction: column;
  overflow-y: auto;
}

.detail-header {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid var(--border);
}

.detail-header.urgent { background: var(--red-bg); }
.detail-header.warning { background: var(--amber-bg); }

.detail-icon-wrap {
  width: 44px;
  height: 44px;
  min-width: 44px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.detail-header-info { flex: 1; }

.detail-title {
  font-size: 1rem;
  font-weight: 700;
  color: var(--text);
}

.detail-title.urgent  { color: var(--red); }
.detail-title.warning { color: var(--amber-text); }

.detail-meta {
  font-size: 0.8rem;
  color: var(--text-3);
  margin-top: 2px;
}

.urgency-badge {
  font-size: 0.8rem;
  font-weight: 600;
  padding: 3px 10px;
  border-radius: 999px;
  white-space: nowrap;
}

.urgency-badge.urgent  { background: var(--red-bg); color: var(--red); }
.urgency-badge.warning { background: var(--amber-bg); color: var(--amber-text); }

/* Body */
.detail-body {
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.detail-text {
  font-size: 0.925rem;
  color: var(--text-2);
  line-height: 1.6;
}

/* Vehicle info */
.detail-vehicle {
  background: var(--surface-hover);
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 1rem;
}

.vehicle-title {
  font-size: 0.7rem;
  font-weight: 600;
  color: var(--text-3);
  letter-spacing: 0.08em;
  margin-bottom: 0.875rem;
}

.vehicle-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
}

.vehicle-item { display: flex; flex-direction: column; gap: 2px; }

.v-label {
  font-size: 0.7rem;
  font-weight: 600;
  color: var(--text-3);
  letter-spacing: 0.05em;
}

.v-value {
  font-size: 0.925rem;
  font-weight: 600;
  color: var(--text);
}

.v-value.orange { color: var(--amber-text); }
.v-value.red    { color: var(--red); }

/* Badge */
.badge {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 3px 10px;
  border-radius: 999px;
  font-size: 0.8rem;
  font-weight: 500;
  border: 1px solid transparent;
  width: fit-content;
}

.badge.en-viaje { background: var(--blue-light); color: var(--blue-hover); border-color: var(--blue-mid); }

/* Actions */
.detail-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.actions-left { display: flex; gap: 0.75rem; }

.btn-action {
  padding: 0.6rem 1.1rem;
  border-radius: 8px;
  font-size: 0.925rem;
  font-family: 'Inter', sans-serif;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  border: 1px solid transparent;
}

.btn-action.primary {
  background: var(--blue-hover);
  color: var(--white);
  border-color: var(--blue-hover);
}

.btn-action.primary:hover { background: var(--blue-hover); }

.btn-action.secondary {
  background: var(--white);
  color: var(--text-2);
  border-color: var(--border);
}

.btn-action.secondary:hover { background: var(--surface-hover); }

.btn-discard {
  padding: 0.6rem 1.1rem;
  border: 1px solid var(--border);
  border-radius: 8px;
  background: var(--white);
  font-size: 0.925rem;
  font-family: 'Inter', sans-serif;
  color: var(--text-2);
  cursor: pointer;
}

.btn-discard:hover { background: var(--surface-hover); }

/* Empty */
.notif-empty {
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.925rem;
  color: var(--text-3);
}
</style>