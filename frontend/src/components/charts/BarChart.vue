<script setup>
import { computed } from 'vue'

const props = defineProps({
  data: { type: Array, default: () => [] },
  height: { type: Number, default: 130 },
  color: { type: String, default: 'var(--blue)' },
  highlightMax: { type: Boolean, default: true },
})

const maxValue = computed(() => Math.max(1, ...props.data.map((d) => d.value || 0)))

const bars = computed(() =>
  props.data.map((d) => ({
    ...d,
    pct: Math.round(((d.value || 0) / maxValue.value) * 100),
    isMax: props.highlightMax && (d.value || 0) === maxValue.value && (d.value || 0) > 0,
  }))
)
</script>

<template>
  <div class="bar-chart" :style="{ height: `${height}px` }">
    <div v-for="(bar, i) in bars" :key="i" class="bar-col" :title="`${bar.label}: ${bar.value}`">
      <span class="bar-value" :class="{ strong: bar.isMax }">{{ bar.value }}</span>
      <div
        class="bar"
        :style="{
          height: `${bar.pct}%`,
          background: bar.isMax ? color : 'var(--blue-mid)',
          opacity: bar.isMax ? 1 : 0.65,
        }"
      />
      <span class="bar-label" :class="{ strong: bar.isMax }">{{ bar.label }}</span>
    </div>
  </div>
</template>

<style scoped>
.bar-chart {
  display: flex;
  align-items: flex-end;
  gap: 8px;
}
.bar-col {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: flex-end;
  height: 100%;
  gap: 6px;
  min-width: 0;
}
.bar {
  width: 100%;
  max-width: 26px;
  border-radius: 5px 5px 0 0;
  min-height: 3px;
  transition: height 0.4s ease;
}
.bar-value {
  font-size: 10.5px;
  font-weight: 600;
  color: var(--text-3);
}
.bar-value.strong {
  color: var(--blue);
}
.bar-label {
  font-size: 10px;
  color: var(--text-3);
  font-weight: 500;
}
.bar-label.strong {
  color: var(--text);
  font-weight: 600;
}
</style>
