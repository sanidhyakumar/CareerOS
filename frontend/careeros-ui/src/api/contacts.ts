import client from './client'

export const ContactCategory = {
  Recruiter: 0,
  HiringManager: 1,
  HumanResources: 2,
  Referral: 3,
  ImmigrationContact: 4,
  PeerColleague: 5,
  Executive: 6,
  Other: 7,
} as const
export type ContactCategory = typeof ContactCategory[keyof typeof ContactCategory]

export interface ContactInteraction {
  id: string
  contactId: string
  interactionDate: string
  channel: string
  summary: string
  notes?: string
  followUpRequired: boolean
  followUpDate?: string
  createdAt: string
}

export interface Contact {
  id: string
  name: string
  category: ContactCategory
  categoryName: string
  email?: string
  phone?: string
  linkedIn?: string
  twitter?: string
  location?: string
  company?: string
  companyId?: string
  companyName?: string
  jobTitle?: string
  department?: string
  jobId?: string
  positionTitle?: string
  isStarred: boolean
  lastContactDate?: string
  nextFollowUpDate?: string
  relationshipNotes?: string
  notes?: string
  tags?: string
  interactionsCount: number
  interactions: ContactInteraction[]
  createdAt: string
  updatedAt?: string
}

export interface ContactStats {
  totalContacts: number
  recruitersCount: number
  hiringManagersCount: number
  humanResourcesCount: number
  referralsCount: number
  immigrationCount: number
  followUpsDueCount: number
  starredCount: number
}

export interface CreateContactRequest {
  name: string
  category: ContactCategory
  email?: string
  phone?: string
  company?: string
  companyId?: string
  jobTitle?: string
  department?: string
  linkedIn?: string
  twitter?: string
  location?: string
  isStarred?: boolean
  notes?: string
  tags?: string
  jobId?: string
}

export interface UpdateContactRequest extends CreateContactRequest {
  id: string
  nextFollowUpDate?: string
}

export interface AddInteractionRequest {
  summary: string
  interactionDate?: string
  channel?: string
  notes?: string
  followUpRequired?: boolean
  followUpDate?: string
}

export interface ContactFilterParams {
  searchTerm?: string
  category?: ContactCategory
  companyId?: string
  isStarred?: boolean
}

export const contactsApi = {
  getAll: (params?: ContactFilterParams) =>
    client.get<Contact[]>('/contacts', { params }).then((r) => r.data),

  getById: (id: string) =>
    client.get<Contact>(`/contacts/${id}`).then((r) => r.data),

  getStats: () =>
    client.get<ContactStats>('/contacts/stats').then((r) => r.data),

  create: (data: CreateContactRequest) =>
    client.post<Contact>('/contacts', data).then((r) => r.data),

  update: (id: string, data: UpdateContactRequest) =>
    client.put<Contact>(`/contacts/${id}`, data).then((r) => r.data),

  toggleStar: (id: string) =>
    client.patch<{ isStarred: boolean }>(`/contacts/${id}/star`).then((r) => r.data),

  getInteractions: (id: string) =>
    client.get<ContactInteraction[]>(`/contacts/${id}/interactions`).then((r) => r.data),

  addInteraction: (id: string, data: AddInteractionRequest) =>
    client.post<ContactInteraction>(`/contacts/${id}/interactions`, data).then((r) => r.data),

  deleteInteraction: (contactId: string, interactionId: string) =>
    client.delete(`/contacts/${contactId}/interactions/${interactionId}`),

  delete: (id: string) =>
    client.delete(`/contacts/${id}`),
}
