import client from './client'

export const ApplicationStatus = {
  Draft: 0,
  Applied: 1,
  UnderReview: 2,
  PhoneScreen: 3,
  TechnicalInterview: 4,
  BehavioralInterview: 5,
  FinalRound: 6,
  OfferReceived: 7,
  Accepted: 8,
  Rejected: 9,
  Withdrawn: 10,
} as const
export type ApplicationStatus = typeof ApplicationStatus[keyof typeof ApplicationStatus]

export interface ApplicationTimelineEvent {
  id: string
  jobApplicationId: string
  status: ApplicationStatus
  statusName: string
  eventDate: string
  title: string
  notes?: string
  createdAt: string
}

export interface ApplicationSnapshot {
  id: string
  jobApplicationId: string
  snapshotDate: string
  jobTitle: string
  companyName: string
  department?: string
  location?: string
  workplaceType?: string
  seniority?: string
  salaryRange?: string
  techStack?: string
  resumeProfileName?: string
  resumeVersionName?: string
  resumeVersionNumber?: number
  resumeDocumentSha256?: string
  resumeDocumentFileName?: string
  resumeContentSnapshot?: string
  coverLetterTitle?: string
  coverLetterBodySnapshot?: string
  portfolioName?: string
  portfolioUrlSnapshot?: string
  channel: string
  referralContactName?: string
  submissionNotes?: string
}

export interface JobApplication {
  id: string
  jobId: string
  jobTitle: string
  companyName: string
  companyId?: string
  jobDepartment?: string
  jobLocation?: string
  jobSalary?: string
  techStack?: string
  resumeVersionId?: string
  resumeVersionName?: string
  resumeProfileName?: string
  coverLetterId?: string
  coverLetterTitle?: string
  portfolioVersionId?: string
  portfolioName?: string
  status: ApplicationStatus
  statusName: string
  applicationDate: string
  channel: string
  referralContactId?: string
  referralContactName?: string
  notes?: string
  snapshotId?: string
  hasSnapshot: boolean
  timelineEventsCount: number
  latestEventTitle?: string
  latestEventDate?: string
  timelineEvents: ApplicationTimelineEvent[]
  createdAt: string
  updatedAt?: string
}

export interface ApplicationStats {
  totalApplications: number
  activePipeline: number
  interviewingCount: number
  offersCount: number
  rejectedCount: number
  offerRate: number
}

export interface CreateApplicationRequest {
  jobId: string
  companyId?: string
  resumeVersionId?: string
  coverLetterId?: string
  portfolioVersionId?: string
  status: ApplicationStatus
  applicationDate: string
  channel: string
  referralContactId?: string
  notes?: string
}

export interface UpdateApplicationRequest {
  id: string
  resumeVersionId?: string
  coverLetterId?: string
  portfolioVersionId?: string
  status: ApplicationStatus
  applicationDate: string
  channel: string
  referralContactId?: string
  notes?: string
}

export interface UpdateStatusRequest {
  newStatus: ApplicationStatus
  eventTitle?: string
  notes?: string
}

export interface AddTimelineEventRequest {
  status: ApplicationStatus
  eventDate?: string
  title: string
  notes?: string
}

export interface ApplicationFilterParams {
  jobId?: string
  companyId?: string
  status?: ApplicationStatus
  searchTerm?: string
}

export const applicationsApi = {
  getAll: (params?: ApplicationFilterParams) =>
    client.get<JobApplication[]>('/applications', { params }).then((r) => r.data),

  getById: (id: string) =>
    client.get<JobApplication>(`/applications/${id}`).then((r) => r.data),

  getStats: () =>
    client.get<ApplicationStats>('/applications/stats').then((r) => r.data),

  create: (data: CreateApplicationRequest) =>
    client.post<JobApplication>('/applications', data).then((r) => r.data),

  update: (id: string, data: UpdateApplicationRequest) =>
    client.put<JobApplication>(`/applications/${id}`, data).then((r) => r.data),

  updateStatus: (id: string, data: UpdateStatusRequest) =>
    client.patch<JobApplication>(`/applications/${id}/status`, data).then((r) => r.data),

  getTimeline: (id: string) =>
    client.get<ApplicationTimelineEvent[]>(`/applications/${id}/timeline`).then((r) => r.data),

  addTimelineEvent: (id: string, data: AddTimelineEventRequest) =>
    client.post<ApplicationTimelineEvent>(`/applications/${id}/timeline`, data).then((r) => r.data),

  getSnapshot: (id: string) =>
    client.get<ApplicationSnapshot>(`/applications/${id}/snapshot`).then((r) => r.data),

  delete: (id: string) =>
    client.delete(`/applications/${id}`).then((r) => r.data),
}

