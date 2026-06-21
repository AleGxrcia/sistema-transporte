<script setup>
import { computed, onMounted, onUnmounted } from 'vue'

const props = defineProps({
  modelValue:  { type: Boolean, default: false },
  title:       { type: String,  default: '¿Estás seguro?' },
  message:     { type: String,  default: '' },
  confirmText: { type: String,  default: 'Confirmar' },
  cancelText:  { type: String,  default: 'Cancelar' },
  variant:     { type: String,  default: 'danger' },
  isLoading:   { type: Boolean, default: false },
})

const emit = defineEmits(['update:modelValue', 'confirm'])

function close() {
  if (props.isLoading) return
  emit('update:modelValue', false)
}

function onKeydown(e) {
  if (e.key === 'Escape' && props.modelValue) close()
}

onMounted(() => document.addEventListener('keydown', onKeydown))
onUnmounted(() => document.removeEventListener('keydown', onKeydown))

const confirmBtnClass = computed(() => {
  const map = {
    danger:  'btn danger',
    warning: 'btn warning',
    primary: 'btn primary',
  }
  return map[props.variant] ?? 'btn danger'
})
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="modelValue" class="modal-overlay" @click.self="close">
        <div class="modal-card confirm-modal" role="dialog" aria-modal="true">

          <div class="confirm-icon" :class="`icon--${variant}`">
            <svg v-if="variant === 'danger'" width="20" height="20" fill="none" stroke="currentColor" stroke-width="2.2" viewBox="0 0 24 24">
              <polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/>
            </svg>
            <svg v-else-if="variant === 'warning'" width="20" height="20" fill="none" stroke="currentColor" stroke-width="2.2" viewBox="0 0 24 24">
              <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/>
            </svg>
            <svg v-else width="20" height="20" fill="none" stroke="currentColor" stroke-width="2.2" viewBox="0 0 24 24">
              <circle cx="12" cy="12" r="10"/><path d="M9 12l2 2 4-4"/>
            </svg>
          </div>

          <h3 class="confirm-title">{{ title }}</h3>
          <p v-if="message" class="confirm-message">{{ message }}</p>

          <div v-if="$slots.default" class="confirm-slot">
            <slot />
          </div>

          <div class="modal-actions">
            <button class="btn" :disabled="isLoading" @click="close">
              {{ cancelText }}
            </button>
            <button :class="confirmBtnClass" :disabled="isLoading" @click="$emit('confirm')">
              <svg v-if="isLoading" class="spinner" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                <path d="M12 2v4M12 18v4M4.93 4.93l2.83 2.83M16.24 16.24l2.83 2.83M2 12h4M18 12h4M4.93 19.07l2.83-2.83M16.24 7.76l2.83-2.83"/>
              </svg>
              {{ isLoading ? 'Procesando…' : confirmText }}
            </button>
          </div>

        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  padding: 16px;
}

.confirm-modal {
  background: var(--white);
  border-radius: 14px;
  padding: 28px 28px 24px;
  width: 100%;
  max-width: 420px;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  gap: 10px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.18);
}

.confirm-icon {
  width: 52px;
  height: 52px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 4px;
}
.icon--danger  { background: var(--red-bg);   color: var(--red);   border: 1px solid var(--red-border); }
.icon--warning { background: var(--amber-bg); color: var(--amber); border: 1px solid var(--amber-border); }
.icon--primary { background: var(--blue-light, #dbeafe); color: var(--blue, #2563eb); border: 1px solid #93c5fd; }

.confirm-title {
  font-size: 16px;
  font-weight: 700;
  color: var(--text);
  margin: 0;
}

.confirm-message {
  font-size: 13px;
  color: var(--text-2);
  margin: 0;
  line-height: 1.5;
}

.confirm-slot {
  width: 100%;
  text-align: left;
  padding-top: 4px;
}

.modal-actions {
  display: flex;
  gap: 8px;
  justify-content: center;
  width: 100%;
  margin-top: 6px;
}
.modal-actions .btn { flex: 1; justify-content: center; }

.spinner {
  animation: spin 0.8s linear infinite;
  flex-shrink: 0;
}
@keyframes spin { to { transform: rotate(360deg); } }

.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.18s ease, transform 0.18s ease;
}
.modal-enter-from,
.modal-leave-to {
  opacity: 0;
  transform: scale(0.96);
}

.btn.danger  { background: var(--red);   color: #fff; border-color: var(--red);   }
.btn.warning { background: var(--amber); color: #fff; border-color: var(--amber); }
.btn.danger:hover  { opacity: 0.88; }
.btn.warning:hover { opacity: 0.88; }
.btn:disabled { opacity: 0.5; cursor: not-allowed; }
</style>