// Mirrors the backend UserRole enum (TransportSystem.Core.Application.Common.Enums.UserRole)
export const ROLE_TO_LABEL = { 0: 'Administrador', 1: 'Supervisor', 2: 'Operador' }
export const LABEL_TO_ROLE = { Administrador: 0, Supervisor: 1, Operador: 2 }

export function roleLabel(role) {
  return ROLE_TO_LABEL[role] ?? 'Operador'
}

export function roleValue(label) {
  return LABEL_TO_ROLE[label]
}
