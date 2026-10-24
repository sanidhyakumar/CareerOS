import client from './client'

export type JobStatus =
  | 'Saved' | 'Applied' | 'PhoneScreen' | 'Interview'
  | 'Offer' | 'Rejected' | 'Withdrawn' | 'Accepted'

export interface Job {
  id: string
  title: string
  company: string
  location?: string
  url?: string
  description?: string
  notes?: string
  status: number
  salary?: string
  appliedAt?: string
  interviewAt?: string
  createdAt: string
  updatedAt: string
}

export interface CreateJobRequest {
  title: string
  company: string
  location?: string
  url?: string
  description?: string
  notes?: string
  status: number
  salary?: string
}

export interface UpdateJobRequest extends CreateJobRequest {
  appliedAt?: string
  interviewAt?: string
}

export const jobsApi = {
  getAll: () => client.get<Job[]>('/jobs').then((r) => r.data),
  getById: (id: string) => client.get<Job>(`/jobs/${id}`).then((r) => r.data),
  create: (data: CreateJobRequest) => client.post<Job>('/jobs', data).then((r) => r.data),
  update: (id: string, data: UpdateJobRequest) => client.put<Job>(`/jobs/${id}`, data).then((r) => r.data),
  delete: (id: string) => client.delete(`/jobs/${id}`),
}

