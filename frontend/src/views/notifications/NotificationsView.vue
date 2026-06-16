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
  border: 1px solid #e5e7eb;
  border-radius: 999px;
  background: #fff;
  font-size: 0.82rem;
  font-family: 'Inter', sans-serif;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s;
}

.tab.active {
  background: #2563eb;
  color: #fff;
  border-color: #2563eb;
  font-weight: 600;
}

.tab-count {
  background: #f3f4f6;
  color: #6b7280;
  font-size: 0.7rem;
  font-weight: 700;
  padding: 1px 6px;
  border-radius: 999px;
}

.tab-count.active {
  background: rgba(255,255,255,0.25);
  color: #fff;
}

.header-actions { display: flex; gap: 0.5rem; }

.btn-secondary {
  padding: 0.5rem 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #fff;
  font-size: 0.82rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
  cursor: pointer;
}

.btn-secondary:hover { background: #f9fafb; }

.btn-danger-outline {
  padding: 0.5rem 1rem;
  border: 1px solid #fecaca;
  border-radius: 8px;
  background: #fff;
  font-size: 0.82rem;
  font-family: 'Inter', sans-serif;
  color: #dc2626;
  cursor: pointer;
}

.btn-danger-outline:hover { background: #fef2f2; }

/* Layout */
.notif-layout {
  display: grid;
  grid-template-columns: 340px 1fr;
  gap: 0;
  flex: 1;
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

/* Lista */
.notif-list {
  border-right: 1px solid #f3f4f6;
  overflow-y: auto;
}

.notif-item {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 1rem;
  cursor: pointer;
  border-bottom: 1px solid #f9fafb;
  transition: background 0.15s;
  position: relative;
}

.notif-item:hover { background: #f9fafb; }
.notif-item.selected { background: #eff6ff; border-left: 3px solid #2563eb; }
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
  font-size: 0.82rem;
  font-weight: 600;
  color: #111827;
  margin-bottom: 2px;
}

.notif-desc {
  font-size: 0.75rem;
  color: #6b7280;
  margin-bottom: 2px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.notif-time {
  font-size: 0.7rem;
  color: #9ca3af;
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
  border-bottom: 1px solid #f3f4f6;
}

.detail-header.urgent { background: #fef2f2; }
.detail-header.warning { background: #fffbeb; }

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
  color: #111827;
}

.detail-title.urgent  { color: #dc2626; }
.detail-title.warning { color: #d97706; }

.detail-meta {
  font-size: 0.75rem;
  color: #9ca3af;
  margin-top: 2px;
}

.urgency-badge {
  font-size: 0.75rem;
  font-weight: 600;
  padding: 3px 10px;
  border-radius: 999px;
  white-space: nowrap;
}

.urgency-badge.urgent  { background: #fef2f2; color: #dc2626; }
.urgency-badge.warning { background: #fffbeb; color: #d97706; }

/* Body */
.detail-body {
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.detail-text {
  font-size: 0.875rem;
  color: #374151;
  line-height: 1.6;
}

/* Vehicle info */
.detail-vehicle {
  background: #f9fafb;
  border: 1px solid #f3f4f6;
  border-radius: 8px;
  padding: 1rem;
}

.vehicle-title {
  font-size: 0.65rem;
  font-weight: 600;
  color: #9ca3af;
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
  font-size: 0.65rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.05em;
}

.v-value {
  font-size: 0.875rem;
  font-weight: 600;
  color: #111827;
}

.v-value.orange { color: #d97706; }
.v-value.red    { color: #dc2626; }

/* Badge */
.badge {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 3px 10px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 500;
  border: 1px solid transparent;
  width: fit-content;
}

.badge.en-viaje { background: #eff6ff; color: #2563eb; border-color: #bfdbfe; }

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
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  border: 1px solid transparent;
}

.btn-action.primary {
  background: #2563eb;
  color: #fff;
  border-color: #2563eb;
}

.btn-action.primary:hover { background: #1d4ed8; }

.btn-action.secondary {
  background: #fff;
  color: #374151;
  border-color: #e5e7eb;
}

.btn-action.secondary:hover { background: #f9fafb; }

.btn-discard {
  padding: 0.6rem 1.1rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #fff;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #6b7280;
  cursor: pointer;
}

.btn-discard:hover { background: #f9fafb; }

/* Empty */
.notif-empty {
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.875rem;
  color: #9ca3af;
}
</style>