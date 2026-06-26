<script setup>
import { computed } from 'vue'

const props = defineProps({
  data: { type: Array, default: () => [] },
  color: { type: String, default: 'var(--blue)' },
  labelWidth: { type: String, default: '84px' },
})

const maxValue = computed(() => Math.max(1, ...props.data.map((d) => d.value || 0)))

const rows = computed(() =>
  props.data.map((d) => ({
    ...d,
    pct: Math.round(((d.value || 0) / maxValue.value) * 100),
  }))
)
</script>

<template>
  <div class="hbar-chart">
    <div v-for="(row, i) in rows" :key="i" class="hbar-row">
      <div class="hbar-label" :style="{ width: labelWidth }" :title="row.label">{{ row.label }}</div>
      <div class="hbar-track">
        <div class="hbar-fill" :style="{ width: `${row.pct}%`, background: color }" />
      </div>
      <div class="hbar-value">{{ row.value }}</div>
    </div>
  </div>
</template>

<style scoped>
.hbar-chart {
  display: flex;
  flex-direction: column;
  gap: 11px;
}
.hbar-row {
  display: flex;
  align-items: center;
  gap: 10px;
}
.hbar-label {
  flex-shrink: 0;
  font-size: 12.5px;
  color: var(--text-2);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.hbar-track {
  flex: 1;
  height: 8px;
  background: var(--bg);
  border-radius: 5px;
  overflow: hidden;
}
.hbar-fill {
  height: 100%;
  border-radius: 5px;
  min-width: 2px;
  transition: width 0.4s ease;
}
.hbar-value {
  width: 22px;
  text-align: right;
  font-size: 12.5px;
  font-weight: 600;
  color: var(--text-2);
}
</style>
