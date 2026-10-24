import axios from 'axios'

export interface CareerProfilePreferenceDto {
  id: string
  careerProfileId: string
  targetRoles: string
  targetLocations: string
  targetSalaryMin?: number
  targetSalaryMax?: number
  currency: string
  openToRelocation: boolean
  remotePreference: string
  notes?: string
}

export interface CareerProfileAddressDto {
  id: string
  careerProfileId: string
  type: string
  street: string
  city: string
  stateOrProvince: string
  postalCode: string
  country: string
  isPrimary: boolean
}

export interface CareerProfileDto {
  id: string
  name: string
  slug: string
  relationship: string
  email: string
  phoneNumber?: string
  avatarUrl?: string
  colorCode: string
  isPrimary: boolean
  isActive: boolean
  headline?: string
  bio?: string
  createdAt: string
  updatedAt: string
  preferences?: CareerProfilePreferenceDto
  addresses: CareerProfileAddressDto[]
  documentCount: number
  applicationCount: number
  interviewCount: number
  contactCount: number
  sponsorshipCount: number
  wikiCount: number
  resumeProfileCount: number
}

export interface CreateCareerProfileDto {
  name: string
  relationship: string
  email: string
  phoneNumber?: string
  headline?: string
  bio?: string
  colorCode: string
  isPrimary?: boolean
  preferences?: Partial<CareerProfilePreferenceDto>
  addresses?: Partial<CareerProfileAddressDto>[]
}

export interface UpdateCareerProfileDto {
  name: string
  relationship: string
  email: string
  phoneNumber?: string
  headline?: string
  bio?: string
  colorCode: string
  isPrimary?: boolean
  preferences?: Partial<CareerProfilePreferenceDto>
  addresses?: Partial<CareerProfileAddressDto>[]
}

export interface ProfileSummaryCardDto {
  profileId: string
  name: string
  slug: string
  relationship: string
  colorCode: string
  headline?: string
  isPrimary: boolean
  isActive: boolean
  activeApplications: number
  upcomingInterviews: number
  totalOffers: number
  expiringDocuments: number
  targetRole: string
}

export interface FamilyActivityDto {
  profileId: string
  profileName: string
  colorCode: string
  action: string
  title: string
  timestamp: string
  type: string
}

export interface FamilyDashboardDto {
  totalProfiles: number
  totalApplications: number
  totalDocuments: number
  totalInterviews: number
  totalOffers: number
  profileSummaries: ProfileSummaryCardDto[]
  recentActivities: FamilyActivityDto[]
}

export interface ProfileAnalyticsDto {
  profileId: string
  profileName: string
  totalApplications: number
  interviewing: number
  offers: number
  rejections: number
  conversionRate: number
  totalDocuments: number
  expiringDocuments: number
  totalContacts: number
}

const API_BASE = '/api/profiles'

export const profilesApi = {
  getAll: async (): Promise<CareerProfileDto[]> => {
    const res = await axios.get(API_BASE)
    return res.data
  },

  getActive: async (): Promise<CareerProfileDto> => {
    const res = await axios.get(`${API_BASE}/active`)
    return res.data
  },

  setActive: async (id: string): Promise<void> => {
    await axios.post(`${API_BASE}/active/${id}`)
  },

  getById: async (id: string): Promise<CareerProfileDto> => {
    const res = await axios.get(`${API_BASE}/${id}`)
    return res.data
  },

  getBySlug: async (slug: string): Promise<CareerProfileDto> => {
    const res = await axios.get(`${API_BASE}/slug/${slug}`)
    return res.data
  },

  getFamilyDashboard: async (): Promise<FamilyDashboardDto> => {
    const res = await axios.get(`${API_BASE}/family-dashboard`)
    return res.data
  },

  getAnalytics: async (id: string): Promise<ProfileAnalyticsDto> => {
    const res = await axios.get(`${API_BASE}/${id}/analytics`)
    return res.data
  },

  create: async (dto: CreateCareerProfileDto): Promise<CareerProfileDto> => {
    const res = await axios.post(API_BASE, dto)
    return res.data
  },

  update: async (id: string, dto: UpdateCareerProfileDto): Promise<CareerProfileDto> => {
    const res = await axios.put(`${API_BASE}/${id}`, dto)
    return res.data
  },

  delete: async (id: string): Promise<void> => {
    await axios.delete(`${API_BASE}/${id}`)
  },
}
