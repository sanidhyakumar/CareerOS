import client from './client'

export interface Resume {
  id: string
  name: string
  content?: string
  version: number
  isDefault: boolean
  tags?: string
  createdAt: string
  updatedAt: string
}

export interface CreateResumeRequest {
  name: string
  content?: string
  isDefault: boolean
  tags?: string
}

export const resumesApi = {
  getAll: () => client.get<Resume[]>('/resumes').then((r) => r.data),
  getById: (id: string) => client.get<Resume>(`/resumes/${id}`).then((r) => r.data),
  create: (data: CreateResumeRequest) => client.post<Resume>('/resumes', data).then((r) => r.data),
  update: (id: string, data: CreateResumeRequest) => client.put<Resume>(`/resumes/${id}`, data).then((r) => r.data),
  delete: (id: string) => client.delete(`/resumes/${id}`),
}

