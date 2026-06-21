import { ref, readonly } from 'vue'

export function useModal(defaultPayload = null) {
  const isOpen = ref(false)
  const payload = ref(defaultPayload)

  function open(data = null) {
    payload.value = data
    isOpen.value = true
  }

  function close() {
    isOpen.value = false
    setTimeout(() => {
      payload.value = defaultPayload
    }, 200)
  }

  return {
    isOpen: readonly(isOpen),
    payload: readonly(payload),
    open,
    close,
  }
}