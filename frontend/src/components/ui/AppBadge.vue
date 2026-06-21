<script setup>
import { computed } from 'vue'

const props = defineProps({
  status: { type: String, required: true },
})

const badge = computed(() => {
  const map = {
    Available:       { label: 'Disponible',       cls: 'badge--green'  },
    OnTrip:          { label: 'En viaje',          cls: 'badge--blue'   },
    InMaintenance:   { label: 'Mantenimiento',     cls: 'badge--amber'  },
    Inactive:        { label: 'Inactivo',          cls: 'badge--gray'   },
    Suspended:       { label: 'Suspendido',        cls: 'badge--red'    },

    Pendiente:       { label: 'Pendiente',         cls: 'badge--amber'  },
    Aprobada:        { label: 'Aprobada',          cls: 'badge--green'  },
    Rechazada:       { label: 'Rechazada',         cls: 'badge--red'    },
    Asignada:        { label: 'Asignada',          cls: 'badge--blue'   },
    EnCurso:         { label: 'En curso',          cls: 'badge--blue'   },
    Completada:      { label: 'Completada',        cls: 'badge--green'  },
    Cancelada:       { label: 'Cancelada',         cls: 'badge--gray'   },
  }

  return map[props.status] ?? { label: props.status, cls: 'badge--gray' }
})
</script>

<template>
  <span class="badge" :class="badge.cls">{{ badge.label }}</span>
</template>

<style scoped>
.badge {
  display: inline-flex;
  align-items: center;
  padding: 3px 9px;
  border-radius: 20px;
  font-size: 11px;
  font-weight: 600;
  letter-spacing: 0.02em;
  white-space: nowrap;
  border: 1px solid transparent;
}

.badge--green {
  background: var(--mint-bg);
  color: var(--mint-dark);
  border-color: #6ee7b7;
}
.badge--blue {
  background: var(--blue-light, #dbeafe);
  color: var(--blue, #2563eb);
  border-color: #93c5fd;
}
.badge--amber {
  background: var(--amber-bg);
  color: #b45309;
  border-color: var(--amber-border);
}
.badge--red {
  background: var(--red-bg);
  color: var(--red);
  border-color: var(--red-border);
}
.badge--gray {
  background: var(--bg);
  color: var(--text-3);
  border-color: var(--border);
}
</style>