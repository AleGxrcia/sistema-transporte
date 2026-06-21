import apiClient from '@/services/api.js'
import { roleLabel, roleValue } from '@/utils/roles.js'

function toUser(dto) {
  return {
    id: dto.id,
    email: dto.email,
    firstName: dto.firstName,
    lastName: dto.lastName,
    role: roleLabel(dto.role),
    isActive: dto.isActive,
  }
}

const UsersService = {
  async getAll() {
    const { data } = await apiClient.get('/users')
    return data.map(toUser)
  },

  async getById(id) {
    const { data } = await apiClient.get(`/users/${id}`)
    return toUser(data)
  },

  async create({ firstName, lastName, email, password, role }) {
    const { data } = await apiClient.post('/users', {
      firstName, lastName, email, password, role: roleValue(role),
    })
    return toUser(data)
  },

  async update(id, { firstName, lastName, email }) {
    const { data } = await apiClient.put(`/users/${id}`, { firstName, lastName, email })
    return toUser(data)
  },

  async remove(id) {
    await apiClient.delete(`/users/${id}`)
  },

  async activate(id) {
    await apiClient.patch(`/users/${id}/activate`)
  },

  async deactivate(id) {
    await apiClient.patch(`/users/${id}/deactivate`)
  },

  async changePassword(id, { currentPassword, newPassword }) {
    await apiClient.post(`/users/${id}/change-password`, { currentPassword, newPassword })
  },
}

export default UsersService
