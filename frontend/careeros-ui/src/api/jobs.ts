import client from './client'

export const JobStatus = {
  Saved: 0,
  Applied: 1,
  PhoneScreen: 2,
  Interview: 3,
  Offer: 4,
  Rejected: 5,
  Withdrawn: 6,
  Accepted: 7,
} as const
export type JobStatus = typeof JobStatus[keyof typeof JobStatus]

export const JobPriority = {
  Low: 0,
  Medium: 1,
  High: 2,
  Dream: 3,
} as const
export type JobPriority = typeof JobPriority[keyof typeof JobPriority]

export const EmploymentType = {
  FullTime: 0,
  Contract: 1,
  PartTime: 2,
  Internship: 3,
  Freelance: 4,
  ContractToHire: 5,
} as const
export type EmploymentType = typeof EmploymentType[keyof typeof EmploymentType]

export const WorkplaceType = {
  Remote: 0,
  Hybrid: 1,
  Onsite: 2,
} as const
export type WorkplaceType = typeof WorkplaceType[keyof typeof WorkplaceType]

export const SeniorityLevel = {
  Entry: 0,
  Mid: 1,
  Senior: 2,
  Lead: 3,
  Staff: 4,
  Principal: 5,
  Director: 6,
  Executive: 7,
} as const
export type SeniorityLevel = typeof SeniorityLevel[keyof typeof SeniorityLevel]

export interface Job {
  id: string
  title: string
  company: string
  companyId?: string
  companyName?: string
  department?: string
  location?: string
  url?: string
  description?: string
  notes?: string
  status: JobStatus
  statusName: string
  priority: JobPriority
  priorityName: string
  employmentType: EmploymentType
  employmentTypeName: string
  workplaceType: WorkplaceType
  workplaceTypeName: string
  seniority: SeniorityLevel
  seniorityName: string
  minSalary?: number
  maxSalary?: number
  currency: string
  salary?: string
  equity?: string
  deadline?: string
  techStack?: string
  appliedAt?: string
  interviewAt?: string
  createdAt: string
  updatedAt?: string
}

export interface CreateJobRequest {
  title: string
  company: string
  companyId?: string
  department?: string
  location?: string
  url?: string
  description?: string
  notes?: string
  status: JobStatus
  priority: JobPriority
  employmentType: EmploymentType
  workplaceType: WorkplaceType
  seniority: SeniorityLevel
  minSalary?: number
  maxSalary?: number
  currency: string
  salary?: string
  equity?: string
  deadline?: string
  techStack?: string
  appliedAt?: string
  interviewAt?: string
}

export interface UpdateJobRequest extends CreateJobRequest {}

export interface JobStats {
  totalJobs: number
  activePositions: number
  dreamPriorityCount: number
  highPriorityCount: number
  remoteCount: number
  averageMidSalary?: number
}

export interface JobFilterParams {
  searchTerm?: string
  companyId?: string
  status?: JobStatus
  priority?: JobPriority
  employmentType?: EmploymentType
  workplaceType?: WorkplaceType
  seniority?: SeniorityLevel
}

export const jobsApi = {
  getAll: (params?: JobFilterParams) =>
    client.get<Job[]>('/jobs', { params }).then((r) => r.data),

  getStats: () =>
    client.get<JobStats>('/jobs/stats').then((r) => r.data),

  getByCompany: (companyId: string) =>
    client.get<Job[]>(`/jobs/company/${companyId}`).then((r) => r.data),

  getById: (id: string) =>
    client.get<Job>(`/jobs/${id}`).then((r) => r.data),

  create: (data: CreateJobRequest) =>
    client.post<Job>('/jobs', data).then((r) => r.data),

  update: (id: string, data: UpdateJobRequest) =>
    client.put<Job>(`/jobs/${id}`, data).then((r) => r.data),

  delete: (id: string) =>
    client.delete(`/jobs/${id}`),
}
