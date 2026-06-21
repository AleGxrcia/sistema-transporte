import dayjs from 'dayjs'
import 'dayjs/locale/es'

dayjs.locale('es')

export function formatDate(date, format = 'DD/MM/YYYY') {
  return dayjs(date).format(format)
}

export function formatDateTime(date) {
  return dayjs(date).format('DD/MM/YYYY HH:mm')
}

export function formatRelativeDate(date) {
  const diff = dayjs().diff(dayjs(date), 'day')
  if (diff === 0) return 'Hoy'
  if (diff === 1) return 'Ayer'
  if (diff < 7) return `Hace ${diff} días`
  return formatDate(date)
}

export function formatCurrency(amount, currency = 'RD$') {
  return `${currency} ${new Intl.NumberFormat('es-DO').format(amount)}`
}

export function formatNumber(num) {
  return new Intl.NumberFormat('es-DO').format(num)
}

export function formatKilometers(km) {
  return `${formatNumber(km)} km`
}

export function getInitials(firstName, lastName) {
  return `${firstName?.[0] || ''}${lastName?.[0] || ''}`.toUpperCase()
}

export function daysUntil(date) {
  return dayjs(date).diff(dayjs(), 'day')
}
