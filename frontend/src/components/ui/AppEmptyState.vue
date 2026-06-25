<script setup>
defineProps({
  variant: { type: String, default: 'blue' }, // blue | green
  title: { type: String, default: '' },
  message: { type: String, default: '' },
})
</script>

<template>
  <div class="empty-state">
    <div class="empty-icon" :class="`empty-icon--${variant}`">
      <slot name="icon" />
    </div>
    <p class="empty-title">{{ title }}</p>
    <p v-if="message" class="empty-sub">{{ message }}</p>
    <slot name="action" />
  </div>
</template>
