import client from './client'

export interface ProfileCitizenship {
  id: string
  userProfileId: string
  countryId: string
  countryName: string
  countryCode: string
  isPrimary: boolean
  acquisitionDate?: string
  passportNumber?: string
  notes?: string
}

export const WorkAuthorizationType = {
  Citizen: 0,
  PermanentResident: 1,
  WorkVisa: 2,
  StudentOptCpt: 3,
  RequiresSponsorship: 4,
  FreelanceContractor: 5,
  Other: 6,
} as const
export type WorkAuthorizationType = typeof WorkAuthorizationType[keyof typeof WorkAuthorizationType]

export interface WorkAuthorization {
  id: string
  userProfileId: string
  countryId: string
  countryName: string
  countryCode: string
  authorizationType: WorkAuthorizationType
  authorizationTypeName: string
  visaType?: string
  requiresSponsorship: boolean
  validFrom?: string
  validTo?: string
  documentId?: string
  documentName?: string
  notes?: string
}

export const SocialPlatform = {
  LinkedIn: 0,
  GitHub: 1,
  PersonalWebsite: 2,
  TwitterX: 3,
  StackOverflow: 4,
  MediumSubstack: 5,
  YouTube: 6,
  Other: 7,
} as const
export type SocialPlatform = typeof SocialPlatform[keyof typeof SocialPlatform]

export interface SocialLink {
  id: string
  userProfileId: string
  platform: SocialPlatform
  platformName: string
  url: string
  displayLabel?: string
  displayOrder: number
}

export const AddressType = {
  CurrentHome: 0,
  Mailing: 1,
  RelocationTarget: 2,
  Previous: 3,
} as const
export type AddressType = typeof AddressType[keyof typeof AddressType]

export interface Address {
  id: string
  userProfileId: string
  addressType: AddressType
  addressTypeName: string
  street1: string
  street2?: string
  city: string
  stateProvince: string
  regionId?: string
  regionName?: string
  regionCode?: string
  postalCode: string
  countryId: string
  countryName: string
  countryCode: string
  isPrimary: boolean
  willingToRelocate: boolean
  openToRemote: boolean
}

export interface UserProfile {
  id: string
  firstName: string
  middleName?: string
  lastName: string
  fullName: string
  preferredName?: string
  headline?: string
  email: string
  secondaryEmail?: string
  phoneNumber?: string
  secondaryPhone?: string
  bio?: string
  dateOfBirth?: string
  avatarUrl?: string
  citizenships: ProfileCitizenship[]
  workAuthorizations: WorkAuthorization[]
  socialLinks: SocialLink[]
  addresses: Address[]
  createdAt: string
  updatedAt: string
}

export interface UpdatePersonalInfoRequest {
  firstName: string
  middleName?: string
  lastName: string
  preferredName?: string
  headline?: string
  email: string
  secondaryEmail?: string
  phoneNumber?: string
  secondaryPhone?: string
  bio?: string
  dateOfBirth?: string | null
  avatarUrl?: string
}

export interface AddCitizenshipRequest {
  countryId: string
  isPrimary?: boolean
  acquisitionDate?: string | null
  passportNumber?: string
  notes?: string
}

export interface WorkAuthorizationRequest {
  countryId: string
  authorizationType: WorkAuthorizationType
  visaType?: string
  requiresSponsorship: boolean
  validFrom?: string | null
  validTo?: string | null
  documentId?: string | null
  notes?: string
}

export interface SocialLinkRequest {
  platform: SocialPlatform
  url: string
  displayLabel?: string
  displayOrder?: number
}

export interface AddressRequest {
  addressType: AddressType
  street1: string
  street2?: string
  city: string
  stateProvince: string
  regionId?: string | null
  postalCode: string
  countryId: string
  isPrimary?: boolean
  willingToRelocate?: boolean
  openToRemote?: boolean
}

export const profileApi = {
  getProfile: () => client.get<UserProfile>('/profile').then((r) => r.data),
  updatePersonalInfo: (data: UpdatePersonalInfoRequest) =>
    client.put<UserProfile>('/profile/personal-info', data).then((r) => r.data),

  // Citizenships
  addCitizenship: (data: AddCitizenshipRequest) =>
    client.post<ProfileCitizenship>('/profile/citizenships', data).then((r) => r.data),
  deleteCitizenship: (id: string) => client.delete(`/profile/citizenships/${id}`),
  setPrimaryCitizenship: (id: string) =>
    client.put<ProfileCitizenship>(`/profile/citizenships/${id}/primary`).then((r) => r.data),

  // Work Authorizations
  addWorkAuthorization: (data: WorkAuthorizationRequest) =>
    client.post<WorkAuthorization>('/profile/work-authorizations', data).then((r) => r.data),
  updateWorkAuthorization: (id: string, data: WorkAuthorizationRequest) =>
    client.put<WorkAuthorization>(`/profile/work-authorizations/${id}`, data).then((r) => r.data),
  deleteWorkAuthorization: (id: string) => client.delete(`/profile/work-authorizations/${id}`),

  // Social Links
  addSocialLink: (data: SocialLinkRequest) =>
    client.post<SocialLink>('/profile/social-links', data).then((r) => r.data),
  updateSocialLink: (id: string, data: SocialLinkRequest) =>
    client.put<SocialLink>(`/profile/social-links/${id}`, data).then((r) => r.data),
  deleteSocialLink: (id: string) => client.delete(`/profile/social-links/${id}`),

  // Addresses
  addAddress: (data: AddressRequest) =>
    client.post<Address>('/profile/addresses', data).then((r) => r.data),
  updateAddress: (id: string, data: AddressRequest) =>
    client.put<Address>(`/profile/addresses/${id}`, data).then((r) => r.data),
  deleteAddress: (id: string) => client.delete(`/profile/addresses/${id}`),
  setPrimaryAddress: (id: string) =>
    client.put<Address>(`/profile/addresses/${id}/primary`).then((r) => r.data),
}

