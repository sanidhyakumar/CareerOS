import client from './client'
import type { Tag } from './tags'

export const DocumentCategory = {
  General: 0,
  Identity: 1,
  Academics: 2,
  Certifications: 3,
  Employment: 4,
  Immigration: 5,
} as const
export type DocumentCategory = typeof DocumentCategory[keyof typeof DocumentCategory]

export interface DocumentUsage {
  id: string
  documentId: string
  entityType: string
  entityId: string
  entityName?: string
  purpose?: string
  usedAt: string
}

export interface Document {
  id: string
  fileName: string
  originalFileName: string
  fileExtension: string
  contentType: string
  fileSizeBytes: number
  storagePath: string
  sha256Hash: string
  version: number
  isLatestVersion: boolean
  description?: string
  category: DocumentCategory
  categoryName: string
  documentNumber?: string
  issuingAuthority?: string
  issueDate?: string
  expiryDate?: string
  isExpired: boolean
  isExpiringSoon: boolean
  daysUntilExpiry?: number
  usageCount: number
  usages: DocumentUsage[]
  rootDocumentId?: string
  countryId?: string
  countryName?: string
  regionId?: string
  regionName?: string
  tags: Tag[]
  createdAt: string
  updatedAt: string
}

export interface DocumentVersion {
  id: string
  version: number
  isLatestVersion: boolean
  originalFileName: string
  fileSizeBytes: number
  sha256Hash: string
  category: DocumentCategory
  categoryName: string
  description?: string
  createdAt: string
}

export interface DocumentSearchParams {
  searchTerm?: string
  countryId?: string
  regionId?: string
  tagId?: string
  category?: DocumentCategory
  isExpired?: boolean
  expiringWithinDays?: number
  latestOnly?: boolean
}

export interface UploadDocumentParams {
  file: File
  description?: string
  countryId?: string
  regionId?: string
  tagIds?: string[]
  subFolder?: string
  category?: DocumentCategory
  documentNumber?: string
  issuingAuthority?: string
  issueDate?: string | null
  expiryDate?: string | null
}

export interface UploadVersionParams {
  id: string
  file: File
  description?: string
  category?: DocumentCategory
  documentNumber?: string
  issuingAuthority?: string
  issueDate?: string | null
  expiryDate?: string | null
}

export interface AddDocumentUsageRequest {
  entityType: string
  entityId: string
  entityName?: string
  purpose?: string
}

export const documentsApi = {
  getAll: (params?: DocumentSearchParams) =>
    client.get<Document[]>('/documents', { params }).then((r) => r.data),

  getExpiring: (daysThreshold = 90) =>
    client.get<Document[]>('/documents/expiring', { params: { daysThreshold } }).then((r) => r.data),

  getById: (id: string) =>
    client.get<Document>(`/documents/${id}`).then((r) => r.data),

  getVersions: (id: string) =>
    client.get<DocumentVersion[]>(`/documents/${id}/versions`).then((r) => r.data),

  upload: (params: UploadDocumentParams) => {
    const formData = new FormData()
    formData.append('file', params.file)
    if (params.description) formData.append('description', params.description)
    if (params.countryId) formData.append('countryId', params.countryId)
    if (params.regionId) formData.append('regionId', params.regionId)
    if (params.subFolder) formData.append('subFolder', params.subFolder)
    if (params.category !== undefined) formData.append('category', params.category.toString())
    if (params.documentNumber) formData.append('documentNumber', params.documentNumber)
    if (params.issuingAuthority) formData.append('issuingAuthority', params.issuingAuthority)
    if (params.issueDate) formData.append('issueDate', params.issueDate)
    if (params.expiryDate) formData.append('expiryDate', params.expiryDate)
    if (params.tagIds && params.tagIds.length > 0) {
      params.tagIds.forEach((t) => formData.append('tagIds', t))
    }

    return client.post<Document>('/documents/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    }).then((r) => r.data)
  },

  uploadVersion: (params: UploadVersionParams) => {
    const formData = new FormData()
    formData.append('file', params.file)
    if (params.description) formData.append('description', params.description)
    if (params.category !== undefined) formData.append('category', params.category.toString())
    if (params.documentNumber) formData.append('documentNumber', params.documentNumber)
    if (params.issuingAuthority) formData.append('issuingAuthority', params.issuingAuthority)
    if (params.issueDate) formData.append('issueDate', params.issueDate)
    if (params.expiryDate) formData.append('expiryDate', params.expiryDate)

    return client.post<Document>(`/documents/${params.id}/versions`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    }).then((r) => r.data)
  },

  move: (id: string, targetSubFolder: string) =>
    client.put<Document>(`/documents/${id}/move`, { targetSubFolder }).then((r) => r.data),

  delete: (id: string, deletePhysicalFile = true) =>
    client.delete(`/documents/${id}`, { params: { deletePhysicalFile } }),

  getDownloadUrl: (id: string) => `/api/documents/${id}/download`,

  // Usage Tracking
  getUsages: (id: string) =>
    client.get<DocumentUsage[]>(`/documents/${id}/usage`).then((r) => r.data),

  addUsage: (id: string, data: AddDocumentUsageRequest) =>
    client.post<DocumentUsage>(`/documents/${id}/usage`, data).then((r) => r.data),

  deleteUsage: (usageId: string) =>
    client.delete(`/documents/usage/${usageId}`),
}
