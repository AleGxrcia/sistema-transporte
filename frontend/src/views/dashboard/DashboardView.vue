<template>
  <div class="dashboard">

    <!-- Alerta de mantenimiento -->
    <div class="alert-banner">
      <span class="alert-icon"></span>
      <span>
        <strong>2 alertas de mantenimiento</strong> —
        Toyota Hilux (GHI-789) y Honda CRV (DEF-456) tienen servicio programado en los próximos 7 días.
      </span>
    </div>

    <!-- KPIs fila 1 -->
    <div class="kpi-grid kpi-grid-4">
      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">TOTAL VEHÍCULOS</span>
          <div class="kpi-icon blue">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <rect x="1" y="3" width="15" height="13" rx="2"/>
              <path d="M16 8h4l3 3v5h-7V8z"/>
              <circle cx="5.5" cy="18.5" r="2.5"/>
              <circle cx="18.5" cy="18.5" r="2.5"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">24</div>
        <div class="kpi-sub green">↑ 2 este mes</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">DISPONIBLES</span>
          <div class="kpi-icon green">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="20 6 9 17 4 12"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">16</div>
        <div class="kpi-sub gray">67% de la flota</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">EN MANTENIMIENTO</span>
          <div class="kpi-icon orange">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">4</div>
        <div class="kpi-sub gray">2 programados hoy</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">FUERA DE SERVICIO</span>
          <div class="kpi-icon red">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <circle cx="12" cy="12" r="10"/>
              <line x1="4.93" y1="4.93" x2="19.07" y2="19.07"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">2</div>
        <div class="kpi-sub red">Requieren atención</div>
      </div>
    </div>

    <!-- KPIs fila 2 -->
    <div class="kpi-grid kpi-grid-3">
      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">TOTAL CONDUCTORES</span>
          <div class="kpi-icon purple">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
              <circle cx="12" cy="7" r="4"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">18</div>
        <div class="kpi-sub gray">12 disponibles ahora</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">SOLICITUDES PENDIENTES</span>
          <div class="kpi-icon red">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
              <polyline points="14 2 14 8 20 8"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">5</div>
        <div class="kpi-sub red">Requieren aprobación</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">GALONES — MAYO</span>
          <div class="kpi-icon teal">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="12" y1="5" x2="12" y2="19"/>
              <line x1="5" y1="12" x2="19" y2="12"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">842</div>
        <div class="kpi-sub green">RD$ 84,200 estimado</div>
      </div>
    </div>

    <div class="bottom-grid">

      <!-- Viajes de hoy -->
      <div class="card">
        <div class="card-header">
          <span class="card-title">Viajes de hoy</span>
          <router-link to="/trips" class="card-link">Ver todos →</router-link>
        </div>
        <div class="trip-list">
          <div v-for="trip in tripsToday" :key="trip.id" class="trip-item">
            <div class="trip-left">
              <span class="trip-dot" :style="{ background: trip.color }"></span>
              <div>
                <p class="trip-route">{{ trip.route }}</p>
                <p class="trip-detail">{{ trip.detail }}</p>
                <p v-if="trip.warning" class="trip-warning">⚠️ {{ trip.warning }}</p>
              </div>
            </div>
            <span class="trip-time">{{ trip.time }}</span>
          </div>
        </div>
      </div>

      <!-- Solicitudes pendientes -->
      <div class="card">
        <div class="card-header">
          <span class="card-title">Solicitudes pendientes</span>
          <router-link to="/requests" class="card-link">Gestionar →</router-link>
        </div>
        <div class="request-list">
          <div v-for="req in pendingRequests" :key="req.id" class="request-item">
            <div class="request-left">
              <span class="request-dot" :style="{ background: req.color }"></span>
              <div>
                <p class="request-area">{{ req.area }}</p>
                <p class="request-detail">{{ req.detail }}</p>
              </div>
            </div>
            <div class="request-actions">
              <button class="action-btn approve">✓</button>
              <button class="action-btn reject">✕</button>
            </div>
          </div>
        </div>
      </div>

    </div>

    <!-- Gráfico viajes por mes -->
    <div class="card chart-card">
      <div class="card-header">
        <span class="card-title">Viajes por mes — 2026</span>
      </div>
      <div class="chart-bars">
        <div
          v-for="(mes, i) in chartData"
          :key="i"
          class="chart-col"
        >
          <div class="bar-wrapper">
            <div
              class="bar"
              :style="{
                height: (mes.value / maxValue * 100) + '%',
                background: mes.active ? '#2563eb' : '#bfdbfe'
              }"
            ></div>
          </div>
          <span class="bar-label">{{ mes.label }}</span>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const tripsToday = ref([
  { id: 1, route: 'RRHH → Aeropuerto Las Américas', detail: 'Juan Pérez · Toyota Hiace · 8 pasajeros', time: '07:30', color: '#22c55e' },
  { id: 2, route: 'Finanzas → Banco Central',        detail: 'Carlos López · Nissan Frontier · 4 pasajeros', time: '09:00', color: '#3b82f6' },
  { id: 3, route: 'IT → Zona Franca',                detail: 'Roberto Díaz · Toyota Corolla · 2 pasajeros', time: '10:30', color: '#3b82f6' },
  { id: 4, route: 'Gerencia → DGII',                 detail: '', warning: 'Sin conductor asignado', time: '14:00', color: '#f59e0b' },
])

const pendingRequests = ref([
  { id: 1, area: 'Área de Ventas',  detail: '29 May · Sto. Domingo Este · 6 pers.', color: '#3b82f6' },
  { id: 2, area: 'Operaciones',     detail: '30 May · La Romana · 3 pers.',         color: '#8b5cf6' },
  { id: 3, area: 'Legal',           detail: '31 May · Santiago · 5 pers.',          color: '#8b5cf6' },
])

const chartData = ref([
  { label: 'Ene', value: 42 },
  { label: 'Feb', value: 38 },
  { label: 'Mar', value: 55 },
  { label: 'Abr', value: 61 },
  { label: 'May', value: 87 },
  { label: 'Jun', value: 30, active: true },
  { label: 'Jul', value: 20 },
  { label: 'Ago', value: 15 },
  { label: 'Sep', value: 10 },
  { label: 'Oct', value: 8 },
  { label: 'Nov', value: 5 },
  { label: 'Dic', value: 3 },
])

const maxValue = computed(() => Math.max(...chartData.value.map(m => m.value)))
</script>

<style scoped>
.dashboard {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  font-family: 'Inter', sans-serif;
}

/* Alerta */
.alert-banner {
  background: #fffbeb;
  border: 1px solid #fde68a;
  border-radius: 8px;
  padding: 0.75rem 1rem;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.875rem;
  color: #92400e;
}

.alert-icon { font-size: 1rem; }

/* KPI Grid */
.kpi-grid {
  display: grid;
  gap: 1rem;
}

.kpi-grid-4 { grid-template-columns: repeat(4, 1fr); }
.kpi-grid-3 { grid-template-columns: repeat(3, 1fr); }

.kpi-card {
  background: #fff;
  border-radius: 10px;
  padding: 1.25rem;
  border: 1px solid #f3f4f6;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.kpi-top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 0.75rem;
}

.kpi-label {
  font-size: 0.7rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.05em;
}

.kpi-icon {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.kpi-icon.blue   { background: #eff6ff; color: #2563eb; }
.kpi-icon.green  { background: #f0fdf4; color: #16a34a; }
.kpi-icon.orange { background: #fff7ed; color: #d97706; }
.kpi-icon.red    { background: #fef2f2; color: #dc2626; }
.kpi-icon.purple { background: #f5f3ff; color: #7c3aed; }
.kpi-icon.teal   { background: #f0fdfa; color: #0d9488; }

.kpi-value {
  font-size: 2rem;
  font-weight: 700;
  color: #111827;
  line-height: 1;
  margin-bottom: 0.4rem;
}

.kpi-sub { font-size: 0.78rem; }
.kpi-sub.green { color: #16a34a; }
.kpi-sub.red   { color: #dc2626; }
.kpi-sub.gray  { color: #6b7280; }

/* Bottom grid */
.bottom-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

/* Card genérica */
.card {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
  padding: 1.25rem;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.card-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: #111827;
}

.card-link {
  font-size: 0.8rem;
  color: #2563eb;
  text-decoration: none;
}

.card-link:hover { text-decoration: underline; }

/* Viajes */
.trip-list { 
  display: flex; 
  flex-direction: column; 
}

.trip-item {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  border-bottom: 1px solid #d1d5db;
}

.trip-item:last-child {
  border-bottom: none;
  padding-bottom: 0;
  margin-bottom: 0;
}

.trip-left {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
}

.trip-dot {
  width: 8px;
  height: 8px;
  min-width: 8px;
  border-radius: 50%;
  align-self: flex-start;
  margin-top: 1.1rem;
}

.trip-route {
  font-size: 0.85rem;
  font-weight: 600;
  color: #111827;
}

.trip-detail {
  font-size: 0.75rem;
  color: #6b7280;
  margin-top: 2px;
}

.trip-warning {
  font-size: 0.75rem;
  color: #d97706;
  margin-top: 2px;
}

.trip-time {
  font-size: 0.82rem;
  color: #6b7280;
  white-space: nowrap;
}

/* Solicitudes */
.request-list { 
  display: flex; 
  flex-direction: column; 
}

.request-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #d1d5db;
}

.request-item:last-child { 
  border-bottom: none;
  padding-bottom: 0;
  margin-bottom: 0; 
}

.request-left {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
}

.request-dot {
  width: 8px;
  height: 8px;
  min-width: 8px;
  border-radius: 50%;
  align-self: flex-start;
  margin-top: 1.1rem;
}

.request-area {
  font-size: 0.85rem;
  font-weight: 600;
  color: #111827;
}

.request-detail {
  font-size: 0.75rem;
  color: #6b7280;
  margin-top: 2px;
}

.request-actions {
  display: flex;
  gap: 0.4rem;
}

.action-btn {
  width: 28px;
  height: 28px;
  border-radius: 6px;
  border: 1px solid #e5e7eb;
  background: #fff;
  cursor: pointer;
  font-size: 0.8rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.15s;
}

.action-btn.approve:hover { background: #f0fdf4; border-color: #16a34a; color: #16a34a; }
.action-btn.reject:hover  { background: #fef2f2; border-color: #dc2626; color: #dc2626; }

/* Gráfico */
.chart-card { margin-top: 0; }

.chart-bars {
  display: flex;
  align-items: flex-end;
  gap: 0.5rem;
  height: 140px;
  padding-top: 1rem;
}

.chart-col {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  height: 100%;
  gap: 0.25rem;
}

.bar-wrapper {
  flex: 1;
  width: 100%;
  display: flex;
  align-items: flex-end;
}

.bar {
  width: 100%;
  border-radius: 4px 4px 0 0;
  transition: height 0.3s ease;
  min-height: 4px;
}

.bar-label {
  font-size: 0.65rem;
  color: #9ca3af;
  text-align: center;
}
</style>