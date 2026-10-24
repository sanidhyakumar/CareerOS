import client from './client'

export interface Tag {
  id: string
  name: string
  colorHex?: string
  createdAt: string
}

export const tagsApi = {
  getAll: () => client.get<Tag[]>('/tags').then((r) => r.data),
  create: (name: string, colorHex?: string) => client.post<Tag>('/tags', { name, colorHex }).then((r) => r.data),
  delete: (id: string) => client.delete(`/tags/${id}`),
}

