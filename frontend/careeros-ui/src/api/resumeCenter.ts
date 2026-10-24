import client from './client'

export interface ResumeProfile {
  id: string
  name: string
  targetRole: string
  targetIndustry?: string
  summary?: string
  primarySkills?: string
  secondarySkills?: string
  isDefault: boolean
  isArchived: boolean
  versionCount: number
  submissionCount: number
  createdAt: string
  updatedAt?: string
  versions?: ResumeVersion[]
}

export interface CreateResumeProfileRequest {
  name: string
  targetRole: string
  targetIndustry?: string
  summary?: string
  primarySkills?: string
  secondarySkills?: string
  isDefault: boolean
}

export interface UpdateResumeProfileRequest {
  name: string
  targetRole: string
  targetIndustry?: string
  summary?: string
  primarySkills?: string
  secondarySkills?: string
  isDefault: boolean
  isArchived: boolean
}

export interface ResumeVersion {
  id: string
  resumeProfileId: string
  profileName?: string
  versionName: string
  versionNumber: number
  documentId?: string
  documentFileName?: string
  documentSha256?: string
  fileFormat: string
  targetCompany?: string
  targetJobTitle?: string
  tailoredKeywords?: string
  content?: string
  changeNotes?: string
  isBaseVersion: boolean
  isArchived: boolean
  submissionCount: number
  createdAt: string
  updatedAt?: string
}

export interface CreateResumeVersionRequest {
  resumeProfileId: string
  versionName: string
  documentId?: string
  fileFormat?: string
  targetCompany?: string
  targetJobTitle?: string
  tailoredKeywords?: string
  content?: string
  changeNotes?: string
  isBaseVersion: boolean
}

export interface UpdateResumeVersionRequest {
  versionName: string
  documentId?: string
  fileFormat: string
  targetCompany?: string
  targetJobTitle?: string
  tailoredKeywords?: string
  content?: string
  changeNotes?: string
  isBaseVersion: boolean
  isArchived: boolean
}

export interface CoverLetter {
  id: string
  resumeProfileId?: string
  profileName?: string
  title: string
  targetCompany?: string
  targetRole?: string
  recipientName?: string
  recipientTitle?: string
  body: string
  isTemplate: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateCoverLetterRequest {
  resumeProfileId?: string
  title: string
  targetCompany?: string
  targetRole?: string
  recipientName?: string
  recipientTitle?: string
  body: string
  isTemplate: boolean
}

export interface PortfolioVersion {
  id: string
  name: string
  url?: string
  targetRole?: string
  description?: string
  featuredProjects?: string
  isDefault: boolean
  submissionCount: number
  createdAt: string
  updatedAt?: string
}

export interface CreatePortfolioVersionRequest {
  name: string
  url?: string
  targetRole?: string
  description?: string
  featuredProjects?: string
  isDefault: boolean
}

export interface ResumeSubmission {
  id: string
  resumeProfileId: string
  profileName?: string
  resumeVersionId: string
  versionName?: string
  versionNumber: number
  coverLetterId?: string
  coverLetterTitle?: string
  portfolioVersionId?: string
  portfolioName?: string
  companyName: string
  roleTitle: string
  submissionDate: string
  channel: string
  status: string
  notes?: string
  snapshotId?: string
  createdAt: string
  updatedAt?: string
}

export interface CreateResumeSubmissionRequest {
  resumeProfileId: string
  resumeVersionId: string
  companyName: string
  roleTitle: string
  submissionDate?: string
  channel: string
  status: string
  coverLetterId?: string
  portfolioVersionId?: string
  notes?: string
}

export interface SubmissionSnapshot {
  id: string
  submissionId: string
  snapshotDate: string
  resumeProfileName: string
  resumeVersionName: string
  resumeVersionNumber: number
  resumeDocumentSha256?: string
  resumeDocumentFileName?: string
  resumeContentSnapshot?: string
  coverLetterTitle?: string
  coverLetterBodySnapshot?: string
  portfolioName?: string
  portfolioUrlSnapshot?: string
  metadataJson?: string
  createdAt: string
}

export interface ProfileStrategyMetric {
  profileId: string
  profileName: string
  targetRole: string
  versionsCount: number
  submissionsCount: number
  interviewsCount: number
  offersCount: number
  responseRate: number
}

export interface ChannelMetric {
  channel: string
  submissionsCount: number
  interviewsCount: number
  responseRate: number
}

export interface ResumeAnalytics {
  totalProfiles: number
  totalVersions: number
  totalCoverLetters: number
  totalPortfolios: number
  totalSubmissions: number
  totalInterviews: number
  totalOffers: number
  overallResponseRate: number
  profileMetrics: ProfileStrategyMetric[]
  channelMetrics: ChannelMetric[]
  recentSubmissions: ResumeSubmission[]
}

// API methods
export const resumeProfilesApi = {
  getAll: (includeArchived = false) =>
    client.get<ResumeProfile[]>('/resume-profiles', { params: { includeArchived } }).then((r) => r.data),
  getById: (id: string) => client.get<ResumeProfile>(`/resume-profiles/${id}`).then((r) => r.data),
  create: (data: CreateResumeProfileRequest) => client.post<ResumeProfile>('/resume-profiles', data).then((r) => r.data),
  update: (id: string, data: UpdateResumeProfileRequest) =>
    client.put<ResumeProfile>(`/resume-profiles/${id}`, data).then((r) => r.data),
  delete: (id: string) => client.delete(`/resume-profiles/${id}`),
  setDefault: (id: string) => client.post(`/resume-profiles/${id}/set-default`),
  getVersions: (id: string, includeArchived = false) =>
    client.get<ResumeVersion[]>(`/resume-profiles/${id}/versions`, { params: { includeArchived } }).then((r) => r.data),
  createVersion: (id: string, data: CreateResumeVersionRequest) =>
    client.post<ResumeVersion>(`/resume-profiles/${id}/versions`, data).then((r) => r.data),
}

export const resumeVersionsApi = {
  getById: (id: string) => client.get<ResumeVersion>(`/resume-versions/${id}`).then((r) => r.data),
  update: (id: string, data: UpdateResumeVersionRequest) =>
    client.put<ResumeVersion>(`/resume-versions/${id}`, data).then((r) => r.data),
  delete: (id: string) => client.delete(`/resume-versions/${id}`),
  clone: (id: string, newVersionName?: string, targetCompany?: string) =>
    client
      .post<ResumeVersion>(`/resume-versions/${id}/clone`, null, { params: { newVersionName, targetCompany } })
      .then((r) => r.data),
}

export const coverLettersApi = {
  getAll: (profileId?: string, templatesOnly = false) =>
    client.get<CoverLetter[]>('/cover-letters', { params: { profileId, templatesOnly } }).then((r) => r.data),
  getById: (id: string) => client.get<CoverLetter>(`/cover-letters/${id}`).then((r) => r.data),
  create: (data: CreateCoverLetterRequest) => client.post<CoverLetter>('/cover-letters', data).then((r) => r.data),
  update: (id: string, data: CreateCoverLetterRequest) =>
    client.put<CoverLetter>(`/cover-letters/${id}`, data).then((r) => r.data),
  delete: (id: string) => client.delete(`/cover-letters/${id}`),
}

export const portfolioVersionsApi = {
  getAll: () => client.get<PortfolioVersion[]>('/portfolio-versions').then((r) => r.data),
  getById: (id: string) => client.get<PortfolioVersion>(`/portfolio-versions/${id}`).then((r) => r.data),
  create: (data: CreatePortfolioVersionRequest) =>
    client.post<PortfolioVersion>('/portfolio-versions', data).then((r) => r.data),
  update: (id: string, data: CreatePortfolioVersionRequest) =>
    client.put<PortfolioVersion>(`/portfolio-versions/${id}`, data).then((r) => r.data),
  delete: (id: string) => client.delete(`/portfolio-versions/${id}`),
}

export const resumeSubmissionsApi = {
  getAll: (profileId?: string, versionId?: string) =>
    client.get<ResumeSubmission[]>('/resume-submissions', { params: { profileId, versionId } }).then((r) => r.data),
  getById: (id: string) => client.get<ResumeSubmission>(`/resume-submissions/${id}`).then((r) => r.data),
  create: (data: CreateResumeSubmissionRequest) =>
    client.post<ResumeSubmission>('/resume-submissions', data).then((r) => r.data),
  delete: (id: string) => client.delete(`/resume-submissions/${id}`),
  getSnapshot: (id: string) => client.get<SubmissionSnapshot>(`/resume-submissions/${id}/snapshot`).then((r) => r.data),
}

export const resumeAnalyticsApi = {
  getAnalytics: () => client.get<ResumeAnalytics>('/resume-analytics').then((r) => r.data),
}

