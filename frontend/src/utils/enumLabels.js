export const VEHICLE_TYPES = [
  { value: 0, name: 'Sedan', label: 'Sedán' },
  { value: 1, name: 'Suv', label: 'SUV' },
  { value: 2, name: 'Van', label: 'Van' },
  { value: 3, name: 'Bus', label: 'Bus' },
  { value: 4, name: 'Pickup', label: 'Pickup' },
]

export const MAINTENANCE_TYPES = [
  { value: 0, name: 'Preventive', label: 'Preventivo' },
  { value: 1, name: 'Corrective', label: 'Correctivo' },
  { value: 2, name: 'Predictive', label: 'Predictivo' },
]

export const LICENSE_CATEGORIES = [
  { value: 0, name: 'One', label: 'Categoría 1' },
  { value: 1, name: 'Two', label: 'Categoría 2' },
  { value: 2, name: 'Three', label: 'Categoría 3' },
  { value: 3, name: 'Four', label: 'Categoría 4' },
  { value: 4, name: 'Five', label: 'Categoría 5' },
]

export const VEHICLE_STATUSES = [
  { name: 'Available', label: 'Disponible' },
  { name: 'OnTrip', label: 'En viaje' },
  { name: 'InMaintenance', label: 'Mantenimiento' },
  { name: 'Inactive', label: 'Inactivo' },
]

export const DRIVER_STATUSES = [
  { name: 'Available', label: 'Disponible' },
  { name: 'OnTrip', label: 'En viaje' },
  { name: 'Suspended', label: 'Suspendido' },
  { name: 'Inactive', label: 'Inactivo' },
]

export const REQUEST_STATUSES = [
  { name: 'Pending', label: 'Pendiente' },
  { name: 'Approved', label: 'Aprobada' },
  { name: 'Rejected', label: 'Rechazada' },
  { name: 'Assigned', label: 'Asignada' },
  { name: 'InProgress', label: 'En curso' },
  { name: 'Completed', label: 'Completada' },
  { name: 'Cancelled', label: 'Cancelada' },
]

export const ASSIGNMENT_STATUSES = [
  { name: 'Scheduled', label: 'Programada' },
  { name: 'InProgress', label: 'En curso' },
  { name: 'Completed', label: 'Completada' },
  { name: 'Cancelled', label: 'Cancelada' },
]

function labelByName(list, name) {
  return list.find((i) => i.name === name)?.label ?? name
}

function valueByName(list, name) {
  return list.find((i) => i.name === name)?.value ?? null
}

function nameByValue(list, value) {
  return list.find((i) => i.value === value)?.name ?? null
}

export const getVehicleTypeLabel = (name) => labelByName(VEHICLE_TYPES, name)
export const getMaintenanceTypeLabel = (name) => labelByName(MAINTENANCE_TYPES, name)
export const getLicenseCategoryLabel = (name) => labelByName(LICENSE_CATEGORIES, name)
export const getVehicleStatusLabel = (name) => labelByName(VEHICLE_STATUSES, name)
export const getDriverStatusLabel = (name) => labelByName(DRIVER_STATUSES, name)
export const getRequestStatusLabel = (name) => labelByName(REQUEST_STATUSES, name)
export const getAssignmentStatusLabel = (name) => labelByName(ASSIGNMENT_STATUSES, name)

export const vehicleTypeValueByName = (name) => valueByName(VEHICLE_TYPES, name)
export const maintenanceTypeValueByName = (name) => valueByName(MAINTENANCE_TYPES, name)
export const licenseCategoryValueByName = (name) => valueByName(LICENSE_CATEGORIES, name)

export const vehicleTypeNameByValue = (value) => nameByValue(VEHICLE_TYPES, value)
export const licenseCategoryNameByValue = (value) => nameByValue(LICENSE_CATEGORIES, value)
