import client from './client'

export const VisaProgramType = {
  H1B: 0,
  L1: 1,
  O1: 2,
  TN: 3,
  E3: 4,
  LMIA_GTS: 5,
  EUBlueCard: 6,
  TSS_482: 7,
  UK_SkilledWorker: 8,
  TechNation_GlobalTalent: 9,
  Other: 10,
} as const
export type VisaProgramType = typeof VisaProgramType[keyof typeof VisaProgramType]

export const VisaProgramLabels: Record<number, string> = {
  0: 'US H-1B (Specialty Occupation)',
  1: 'US L-1 (Intra-Company Transferee)',
  2: 'US O-1 (Extraordinary Ability)',
  3: 'US TN (USMCA Professional)',
  4: 'US E-3 (Australian Specialty)',
  5: 'Canada LMIA / Global Talent Stream',
  6: 'EU Blue Card (Germany / EU)',
  7: 'Australia TSS (Subclass 482)',
  8: 'UK Skilled Worker Visa',
  9: 'UK Global Talent (Tech Nation)',
  10: 'Other Global Visa Program',
}

export const SponsorshipEligibilityStatus = {
  Eligible: 0,
  ConditionallyEligible: 1,
  PendingReview: 2,
  Ineligible: 3,
  Sponsored: 4,
} as const
export type SponsorshipEligibilityStatus = typeof SponsorshipEligibilityStatus[keyof typeof SponsorshipEligibilityStatus]

export const SponsorshipEligibilityLabels: Record<number, string> = {
  0: 'Eligible',
  1: 'Conditionally Eligible',
  2: 'Pending Review',
  3: 'Ineligible',
  4: 'Sponsored / Approved',
}

export const VisaAvailabilityStatus = {
  OpenYearRound: 0,
  CapSubjectLottery: 1,
  QuotaWaitlist: 2,
  ExpeditedProcessing: 3,
  RestrictedOrPaused: 4,
} as const
export type VisaAvailabilityStatus = typeof VisaAvailabilityStatus[keyof typeof VisaAvailabilityStatus]

export const VisaAvailabilityLabels: Record<number, string> = {
  0: 'Open Year-Round',
  1: 'Cap-Subject Annual Lottery',
  2: 'Quota Waitlist',
  3: 'Expedited Processing Available',
  4: 'Restricted / Paused',
}

export interface SponsorshipEvidence {
  id: string
  sponsorshipCaseId: string
  documentId?: string
  documentName?: string
  documentType?: string
  evidenceType: string
  description: string
  isVerified: boolean
  expiryDate?: string
  createdAt: string
}

export interface SponsorshipCase {
  id: string
  countryId: string
  countryName: string
  countryCode: string
  programName: string
  programType: number
  programTypeName: string
  companyId?: string
  companyName?: string
  jobId?: string
  jobTitle?: string
  eligibility: number
  eligibilityName: string
  availability: number
  availabilityName: string
  minimumSalaryRequirement?: number
  currency?: string
  nextFiscalCycleDate?: string
  filingDeadline?: string
  estimatedProcessingMonths?: number
  supportsDualIntent: boolean
  leadsToPermanentResidency: boolean
  notes?: string
  evidenceList: SponsorshipEvidence[]
  createdAt: string
  updatedAt: string
}

export interface SponsorshipStats {
  totalCases: number
  eligibleCount: number
  conditionallyEligibleCount: number
  openYearRoundCount: number
  capSubjectLotteryCount: number
  leadsToPRCount: number
  verifiedEvidenceCount: number
  totalCountries: number
}

export interface CreateSponsorshipCaseRequest {
  countryId: string
  programName: string
  programType: number
  eligibility?: number
  availability?: number
  companyId?: string
  jobId?: string
  minimumSalaryRequirement?: number
  currency?: string
  nextFiscalCycleDate?: string
  filingDeadline?: string
  estimatedProcessingMonths?: number
  supportsDualIntent?: boolean
  leadsToPermanentResidency?: boolean
  notes?: string
}

export interface UpdateSponsorshipCaseRequest {
  id: string
  countryId: string
  programName: string
  programType: number
  eligibility: number
  availability: number
  companyId?: string
  jobId?: string
  minimumSalaryRequirement?: number
  currency?: string
  nextFiscalCycleDate?: string
  filingDeadline?: string
  estimatedProcessingMonths?: number
  supportsDualIntent: boolean
  leadsToPermanentResidency: boolean
  notes?: string
}

export interface AddSponsorshipEvidenceRequest {
  sponsorshipCaseId: string
  evidenceType: string
  description: string
  documentId?: string
  isVerified?: boolean
  expiryDate?: string
}

export interface UpdateSponsorshipEvidenceRequest {
  id: string
  evidenceType: string
  description: string
  documentId?: string
  isVerified: boolean
  expiryDate?: string
}

export interface SponsorshipFilterParams {
  countryId?: string
  programType?: number
  eligibility?: number
  availability?: number
  companyId?: string
  searchTerm?: string
}

export const sponsorshipApi = {
  getAll: async (params?: SponsorshipFilterParams): Promise<SponsorshipCase[]> => {
    const response = await client.get<SponsorshipCase[]>('/sponsorship', { params })
    return response.data
  },

  getStats: async (): Promise<SponsorshipStats> => {
    const response = await client.get<SponsorshipStats>('/sponsorship/stats')
    return response.data
  },

  getById: async (id: string): Promise<SponsorshipCase> => {
    const response = await client.get<SponsorshipCase>(`/sponsorship/${id}`)
    return response.data
  },

  create: async (data: CreateSponsorshipCaseRequest): Promise<SponsorshipCase> => {
    const response = await client.post<SponsorshipCase>('/sponsorship', data)
    return response.data
  },

  update: async (id: string, data: UpdateSponsorshipCaseRequest): Promise<SponsorshipCase> => {
    const response = await client.put<SponsorshipCase>(`/sponsorship/${id}`, data)
    return response.data
  },

  delete: async (id: string): Promise<void> => {
    await client.delete(`/sponsorship/${id}`)
  },

  addEvidence: async (caseId: string, data: AddSponsorshipEvidenceRequest): Promise<SponsorshipEvidence> => {
    const response = await client.post<SponsorshipEvidence>(`/sponsorship/${caseId}/evidence`, data)
    return response.data
  },

  updateEvidence: async (caseId: string, evidenceId: string, data: UpdateSponsorshipEvidenceRequest): Promise<SponsorshipEvidence> => {
    const response = await client.put<SponsorshipEvidence>(`/sponsorship/${caseId}/evidence/${evidenceId}`, data)
    return response.data
  },

  removeEvidence: async (caseId: string, evidenceId: string): Promise<void> => {
    await client.delete(`/sponsorship/${caseId}/evidence/${evidenceId}`)
  },
}

