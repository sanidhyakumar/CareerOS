import client from './client'

export const SponsorshipStatus = {
  Unknown: 0,
  Yes: 1,
  No: 2,
  CaseByCase: 3,
} as const
export type SponsorshipStatus = typeof SponsorshipStatus[keyof typeof SponsorshipStatus]

export const RelocationStatus = {
  Unknown: 0,
  Yes: 1,
  No: 2,
  DomesticOnly: 3,
  International: 4,
} as const
export type RelocationStatus = typeof RelocationStatus[keyof typeof RelocationStatus]

export interface Company {
  id: string
  name: string
  description?: string
  headquarters?: string
  size?: string
  stage?: string
  foundedYear?: number
  logoUrl?: string

  websiteUrl?: string
  careersUrl?: string
  techBlogUrl?: string
  linkedInUrl?: string
  glassdoorUrl?: string

  glassdoorRating?: number
  personalRating: number
  cultureRating: number
  workLifeBalanceRating: number

  sponsorshipStatus: SponsorshipStatus
  sponsorshipStatusName: string
  sponsoredVisas?: string
  sponsorshipNotes?: string
  relocationStatus: RelocationStatus
  relocationStatusName: string
  relocationNotes?: string

  industry?: string
  sector?: string
  tags?: string

  notes?: string
  pros?: string
  cons?: string
  isTarget: boolean

  jobsCount: number
  contactsCount: number

  createdAt: string
  updatedAt?: string
}

export interface CreateCompanyRequest {
  name: string
  description?: string
  headquarters?: string
  size?: string
  stage?: string
  foundedYear?: number
  logoUrl?: string

  websiteUrl?: string
  careersUrl?: string
  techBlogUrl?: string
  linkedInUrl?: string
  glassdoorUrl?: string

  glassdoorRating?: number
  personalRating: number
  cultureRating: number
  workLifeBalanceRating: number

  sponsorshipStatus: SponsorshipStatus
  sponsoredVisas?: string
  sponsorshipNotes?: string
  relocationStatus: RelocationStatus
  relocationNotes?: string

  industry?: string
  sector?: string
  tags?: string

  notes?: string
  pros?: string
  cons?: string
  isTarget: boolean
}

export interface UpdateCompanyRequest extends CreateCompanyRequest {}

export interface CompanyStats {
  totalCompanies: number
  targetCompanies: number
  sponsoringCompanies: number
  relocatingCompanies: number
  topIndustries: string[]
}

export interface CompanyFilterParams {
  searchTerm?: string
  industry?: string
  sponsorship?: SponsorshipStatus
  relocation?: RelocationStatus
  minRating?: number
  isTarget?: boolean
}

export const companiesApi = {
  getAll: (params?: CompanyFilterParams) =>
    client.get<Company[]>('/companies', { params }).then((r) => r.data),

  getStats: () =>
    client.get<CompanyStats>('/companies/stats').then((r) => r.data),

  getById: (id: string) =>
    client.get<Company>(`/companies/${id}`).then((r) => r.data),

  create: (data: CreateCompanyRequest) =>
    client.post<Company>('/companies', data).then((r) => r.data),

  update: (id: string, data: UpdateCompanyRequest) =>
    client.put<Company>(`/companies/${id}`, data).then((r) => r.data),

  delete: (id: string) =>
    client.delete(`/companies/${id}`),
}

