<template>
  <div class="maintenance">

    <!-- Alerta -->
    <div class="alert-banner">
      <span>⚠️</span>
      <span>
        <strong>3 vehículos requieren mantenimiento</strong>
        próximamente — Toyota Hilux (7 días), Honda CRV (4 días), Hyundai H1 (12 días).
      </span>
    </div>

    <div class="main-grid">

      <!-- Próximos mantenimientos -->
      <div class="card">
        <div class="card-section-title">PRÓXIMOS MANTENIMIENTOS</div>
        <div class="upcoming-list">
          <div v-for="item in upcoming" :key="item.id" class="upcoming-item">
            <div class="upcoming-icon" :class="item.color">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/>
              </svg>
            </div>
            <div class="upcoming-info">
              <p class="upcoming-name">{{ item.vehicle }}</p>
              <p class="upcoming-detail">{{ item.type }} · {{ item.shop }}</p>
            </div>
            <span class="upcoming-days" :class="item.daysColor">
              en {{ item.days }} días
            </span>
          </div>
        </div>
      </div>

      <!-- Formulario registro -->
      <div class="card">
        <div class="card-section-title">REGISTRAR MANTENIMIENTO</div>

        <div class="form-group">
          <label class="form-label">Vehículo</label>
          <select v-model="form.vehicle" class="form-input">
            <option value="">Seleccionar vehículo</option>
            <option value="GHI-789">Toyota Hilux — GHI-789</option>
            <option value="ABC-123">Toyota Hiace — ABC-123</option>
            <option value="DEF-456">Honda CRV — DEF-456</option>
            <option value="JKL-012">Nissan Frontier — JKL-012</option>
            <option value="MNO-345">Hyundai H1 — MNO-345</option>
          </select>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Tipo</label>
            <select v-model="form.type" class="form-input">
              <option value="">Seleccionar</option>
              <option value="Preventivo">Preventivo</option>
              <option value="Correctivo">Correctivo</option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">Fecha de ingreso</label>
            <input v-model="form.date" type="date" class="form-input" />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Costo (RD$)</label>
            <input v-model="form.cost" type="number"
              class="form-input" placeholder="0.00" />
          </div>
          <div class="form-group">
            <label class="form-label">Taller</label>
            <input v-model="form.shop" type="text"
              class="form-input" placeholder="Nombre del taller" />
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">Descripción</label>
          <textarea v-model="form.description" class="form-textarea"
            placeholder="Detalla el trabajo realizado..." />
        </div>

        <div class="form-group">
          <label class="form-label">Fecha próximo mantenimiento</label>
          <input v-model="form.nextService" type="date" class="form-input" />
        </div>

        <div class="form-actions">
          <button class="btn-cancel" @click="resetForm">Cancelar</button>
          <button class="btn-submit" @click="handleSave" :disabled="loading">
            {{ loading ? 'Guardando...' : 'Guardar registro' }}
          </button>
        </div>
      </div>

    </div>

    <!-- Historial tabla -->
    <div class="card">
      <h3 class="section-title">Historial de mantenimiento</h3>
      <div class="table-wrapper">
        <table class="table">
          <thead>
            <tr>
              <th>VEHÍCULO</th>
              <th>TIPO</th>
              <th>DESCRIPCIÓN</th>
              <th>FECHA</th>
              <th>COSTO</th>
              <th>TALLER</th>
              <th>PRÓX. MANT.</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in history" :key="item.id">
              <td class="td-bold">{{ item.vehicle }}</td>
              <td>
                <span class="badge" :class="item.type === 'Preventivo' ? 'preventivo' : 'correctivo'">
                  • {{ item.type }}
                </span>
              </td>
              <td>{{ item.description }}</td>
              <td class="td-gray">{{ item.date }}</td>
              <td class="td-bold">{{ item.cost }}</td>
              <td class="td-gray">{{ item.shop }}</td>
              <td :class="item.urgent ? 'red' : 'td-gray'">{{ item.nextService }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'

const loading = ref(false)

const form = reactive({
  vehicle: 'GHI-789', type: 'Preventivo', date: '',
  cost: '', shop: '', description: '', nextService: ''
})

const upcoming = ref([
  { id: 1, vehicle: 'Honda CRV — DEF-456',   type: 'Preventivo', shop: 'Taller Honda',   days: 4,  color: 'red',    daysColor: 'text-red'    },
  { id: 2, vehicle: 'Toyota Hilux — GHI-789', type: 'Correctivo', shop: 'AutoFix',        days: 7,  color: 'orange', daysColor: 'text-orange' },
  { id: 3, vehicle: 'Hyundai H1 — MNO-345',  type: 'Preventivo', shop: 'Hyundai Motors', days: 12, color: 'gray',   daysColor: 'text-gray'   },
])

const history = ref([
  { id: 1, vehicle: 'GHI-789', type: 'Correctivo', description: 'Cambio de frenos y aceite',  date: '15/05/26', cost: 'RD$ 8,500',  shop: 'AutoFix',       nextService: '04/06/26', urgent: true  },
  { id: 2, vehicle: 'ABC-123', type: 'Preventivo', description: 'Revisión general 40k km',    date: '10/05/26', cost: 'RD$ 5,200',  shop: 'Toyota Service',nextService: '10/08/26', urgent: false },
  { id: 3, vehicle: 'DEF-456', type: 'Correctivo', description: 'Reparación A/C + gomas',     date: '02/05/26', cost: 'RD$ 14,800', shop: 'Taller Honda',  nextService: '01/06/26', urgent: true  },
  { id: 4, vehicle: 'JKL-012', type: 'Preventivo', description: 'Cambio de aceite y filtros', date: '28/04/26', cost: 'RD$ 3,200',  shop: 'Toyota Service',nextService: '28/07/26', urgent: false },
])

function resetForm() {
  Object.assign(form, {
    vehicle: '', type: '', date: '', cost: '', shop: '', description: '', nextService: ''
  })
}

async function handleSave() {
  loading.value = true
  try {
    await new Promise(r => setTimeout(r, 800))
    resetForm()
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.maintenance {
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
  padding: 0.875rem 1rem;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.875rem;
  color: #92400e;
}

/* Grid principal */
.main-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.25rem;
  align-items: stretch;
}

/* Card */
.card {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  padding: 1.25rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
  display: flex;
  flex-direction: column;
  gap: 0.875rem;
}

.card-section-title {
  font-size: 0.7rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.08em;
}

.section-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: #111827;
}

/* Upcoming */
.upcoming-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.upcoming-item {
  display: flex;
  align-items: center;
  gap: 0.875rem;
  padding: 0.75rem;
  border: 1px solid #f3f4f6;
  border-radius: 8px;
}

.upcoming-icon {
  width: 36px;
  height: 36px;
  min-width: 36px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.upcoming-icon.red    { background: #fef2f2; color: #dc2626; }
.upcoming-icon.orange { background: #fff7ed; color: #d97706; }
.upcoming-icon.gray   { background: #f3f4f6; color: #9ca3af; }

.upcoming-info { flex: 1; }

.upcoming-name {
  font-size: 0.875rem;
  font-weight: 600;
  color: #111827;
}

.upcoming-detail {
  font-size: 0.75rem;
  color: #9ca3af;
  margin-top: 2px;
}

.upcoming-days {
  font-size: 0.82rem;
  font-weight: 500;
  white-space: nowrap;
}

.text-red    { color: #dc2626; }
.text-orange { color: #d97706; }
.text-gray   { color: #9ca3af; }

/* Formulario */
.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.75rem;
}

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

.form-textarea {
  padding: 0.6rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #111827;
  outline: none;
  resize: vertical;
  min-height: 90px;
  transition: border-color 0.2s;
}

.form-textarea:focus {
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  margin-top: 0.25rem;
}

.btn-cancel {
  padding: 0.6rem 1rem;
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
  padding: 0.6rem 1rem;
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

/* Tabla */
.table-wrapper {
  overflow: hidden;
  border-radius: 8px;
  border: 1px solid #f3f4f6;
}

.table {
  width: 100%;
  border-collapse: collapse;
}

.table th {
  text-align: left;
  font-size: 0.68rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.05em;
  padding: 0.75rem 1rem;
  border-bottom: 1px solid #f3f4f6;
  background: #fff;
}

.table td {
  padding: 0.875rem 1rem;
  font-size: 0.85rem;
  color: #374151;
  border-bottom: 1px solid #f9fafb;
}

.table tbody tr:last-child td { border-bottom: none; }
.table tbody tr:hover { background: #f9fafb; }

.td-bold { font-weight: 600; color: #111827; }
.td-gray { color: #9ca3af; }
.red     { color: #dc2626; font-weight: 600; }

/* Badges */
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

.badge.preventivo { background: #f0fdf4; color: #16a34a; border-color: #bbf7d0; }
.badge.correctivo { background: #fff7ed; color: #d97706; border-color: #fed7aa; }
</style>
