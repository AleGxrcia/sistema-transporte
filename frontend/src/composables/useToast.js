import { ref } from 'vue'

const toasts = ref([])
let nextId = 0

function addToast(type, title, message = '', duration = 4000) {
  const id = ++nextId
  toasts.value.push({ id, type, title, message })
  setTimeout(() => removeToast(id), duration)
}

function removeToast(id) {
  const idx = toasts.value.findIndex(t => t.id === id)
  if (idx !== -1) toasts.value.splice(idx, 1)
}

export function useToast() {
  return {
    toasts,
    success: (title, message) => addToast('success', title, message),
    error:   (title, message) => addToast('error',   title, message),
    warning: (title, message) => addToast('warning', title, message),
    info:    (title, message) => addToast('info',    title, message),
    remove:  removeToast,
  }
}