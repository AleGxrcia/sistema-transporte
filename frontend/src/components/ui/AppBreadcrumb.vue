<script setup>
import { RouterLink } from 'vue-router'
import { ArrowLeft } from '@lucide/vue'

defineProps({
  items: { type: Array, required: true },
})
</script>

<template>
  <nav class="breadcrumb">
    <template v-for="(item, i) in items" :key="i">
      <RouterLink v-if="item.to" :to="item.to" class="breadcrumb__link">
        <ArrowLeft v-if="i === 0" :size="13" />
        {{ item.label }}
      </RouterLink>
      <span v-else class="breadcrumb__current">{{ item.label }}</span>
      <span v-if="i < items.length - 1" class="breadcrumb__sep">/</span>
    </template>
  </nav>
</template>

<style scoped>
.breadcrumb {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  margin-bottom: 16px;
  flex-wrap: wrap;
}
.breadcrumb__link {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  color: var(--blue);
  font-weight: 500;
  text-decoration: none;
}
.breadcrumb__link:hover {
  text-decoration: underline;
}
.breadcrumb__sep {
  color: var(--border-strong);
}
.breadcrumb__current {
  color: var(--text-3);
}
</style>
