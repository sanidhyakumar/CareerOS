import client from './client'

export interface Contact {
  id: string
  name: string
  email?: string
  phone?: string
  company?: string
  jobTitle?: string
  linkedIn?: string
  notes?: string
  createdAt: string
  updatedAt: string
}

export interface CreateContactRequest {
  name: string
  email?: string
  phone?: string
  company?: string
  jobTitle?: string
  linkedIn?: string
  notes?: string
}

export const contactsApi = {
  getAll: () => client.get<Contact[]>('/contacts').then((r) => r.data),
  getById: (id: string) => client.get<Contact>(`/contacts/${id}`).then((r) => r.data),
  create: (data: CreateContactRequest) => client.post<Contact>('/contacts', data).then((r) => r.data),
  update: (id: string, data: CreateContactRequest) => client.put<Contact>(`/contacts/${id}`, data).then((r) => r.data),
  delete: (id: string) => client.delete(`/contacts/${id}`),
}

