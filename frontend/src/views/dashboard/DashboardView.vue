<script setup>
import { computed } from 'vue'
import { useAuth } from '@/composables/useAuth'
import ManagerDashboard from '@/components/dashboard/ManagerDashboard.vue'
import OperatorDashboard from '@/components/dashboard/OperatorDashboard.vue'

const { currentRole } = useAuth()

// Administrador y Supervisor comparten el panel de gestión.
// El Operador tiene un inicio centrado en crear y seguir sus solicitudes.
const isOperator = computed(() => currentRole.value === 'Operador')
</script>

<template>
  <OperatorDashboard v-if="isOperator" />
  <ManagerDashboard v-else />
</template>
