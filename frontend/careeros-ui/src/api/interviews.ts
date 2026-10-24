import client from './client'

export const InterviewType = {
  Recruiter: 0,
  Technical: 1,
  Behavioral: 2,
  SystemDesign: 3,
  Manager: 4,
  Final: 5,
  TakeHomeReview: 6,
  Other: 7,
} as const
export type InterviewType = typeof InterviewType[keyof typeof InterviewType]

export const InterviewFormat = {
  VideoCall: 0,
  Phone: 1,
  Onsite: 2,
  Asynchronous: 3,
} as const
export type InterviewFormat = typeof InterviewFormat[keyof typeof InterviewFormat]

export const InterviewStatus = {
  Scheduled: 0,
  Completed: 1,
  Cancelled: 2,
  Rescheduled: 3,
  NoShow: 4,
} as const
export type InterviewStatus = typeof InterviewStatus[keyof typeof InterviewStatus]

export const InterviewOutcome = {
  Pending: 0,
  Passed: 1,
  Failed: 2,
  Neutral: 3,
  StrongHire: 4,
  Hire: 5,
  NoHire: 6,
} as const
export type InterviewOutcome = typeof InterviewOutcome[keyof typeof InterviewOutcome]

export interface InterviewInterviewer {
  contactId: string
  contactName: string
  role?: string
  contactEmail?: string
  contactLinkedIn?: string
}

export interface Interview {
  id: string
  title: string
  interviewType: InterviewType
  typeName: string
  format: InterviewFormat
  formatName: string
  status: InterviewStatus
  statusName: string
  outcome: InterviewOutcome
  outcomeName: string
  scheduledAt: string
  durationMinutes: number
  endAt: string
  meetingLink?: string
  location?: string
  interviewerNames?: string
  jobApplicationId?: string
  jobId?: string
  jobTitle?: string
  companyId?: string
  companyName?: string
  preparationNotes?: string
  questionsAsked?: string
  feedbackNotes?: string
  rating?: number
  interviewers: InterviewInterviewer[]
  createdAt: string
  updatedAt?: string
}

export interface InterviewStats {
  totalInterviews: number
  upcomingCount: number
  completedCount: number
  technicalCount: number
  systemDesignCount: number
  behavioralCount: number
  passedCount: number
  passRate: number
}

export interface CreateInterviewRequest {
  title: string
  interviewType: InterviewType
  format: InterviewFormat
  scheduledAt: string
  durationMinutes: number
  jobApplicationId?: string
  jobId?: string
  companyId?: string
  meetingLink?: string
  location?: string
  interviewerNames?: string
  preparationNotes?: string
  interviewerContactIds?: string[]
}

export interface UpdateInterviewRequest extends CreateInterviewRequest {
  id: string
  status: InterviewStatus
  outcome: InterviewOutcome
  questionsAsked?: string
  feedbackNotes?: string
  rating?: number
}

export interface UpdateInterviewStatusRequest {
  status: InterviewStatus
  outcome: InterviewOutcome
  feedbackNotes?: string
  rating?: number
}

export interface InterviewFilterParams {
  applicationId?: string
  companyId?: string
  jobId?: string
  type?: InterviewType
  status?: InterviewStatus
  fromDate?: string
  toDate?: string
  searchTerm?: string
}

export const interviewsApi = {
  getAll: (params?: InterviewFilterParams) =>
    client.get<Interview[]>('/interviews', { params }).then((r) => r.data),

  getStats: () =>
    client.get<InterviewStats>('/interviews/stats').then((r) => r.data),

  getUpcoming: (count = 10) =>
    client.get<Interview[]>('/interviews/upcoming', { params: { count } }).then((r) => r.data),

  getById: (id: string) =>
    client.get<Interview>(`/interviews/${id}`).then((r) => r.data),

  create: (data: CreateInterviewRequest) =>
    client.post<Interview>('/interviews', data).then((r) => r.data),

  update: (id: string, data: UpdateInterviewRequest) =>
    client.put<Interview>(`/interviews/${id}`, data).then((r) => r.data),

  updateStatus: (id: string, data: UpdateInterviewStatusRequest) =>
    client.patch<Interview>(`/interviews/${id}/status`, data).then((r) => r.data),

  delete: (id: string) =>
    client.delete(`/interviews/${id}`).then((r) => r.data),
}

