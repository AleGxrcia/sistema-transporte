import dayjs from 'dayjs'
import 'dayjs/locale/es'

dayjs.locale('es')

export function buildMonthGrid(date) {
  const start = dayjs(date).startOf('month').startOf('week')
  return Array.from({ length: 42 }, (_, i) => start.add(i, 'day'))
}

export function buildWeekDays(date) {
  const start = dayjs(date).startOf('week')
  return Array.from({ length: 7 }, (_, i) => start.add(i, 'day'))
}

export function rangeForView(view, date) {
  const d = dayjs(date)
  if (view === 'month') {
    return { from: d.startOf('month').startOf('week'), to: d.endOf('month').endOf('week') }
  }
  if (view === 'week') {
    return { from: d.startOf('week'), to: d.endOf('week') }
  }
  return { from: d.startOf('day'), to: d.endOf('day') }
}
