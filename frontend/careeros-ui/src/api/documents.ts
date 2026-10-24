import client from './client'
import type { Tag } from './tags'

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
  createdAt: string
}

export interface DocumentSearchParams {
  searchTerm?: string
  countryId?: string
  regionId?: string
  tagId?: string
  latestOnly?: boolean
}

export const documentsApi = {
  getAll: (params?: DocumentSearchParams) =>
    client.get<Document[]>('/documents', { params }).then((r) => r.data),

  getById: (id: string) =>
    client.get<Document>(`/documents/${id}`).then((r) => r.data),

  getVersions: (id: string) =>
    client.get<DocumentVersion[]>(`/documents/${id}/versions`).then((r) => r.data),

  upload: (file: File, description?: string, countryId?: string, regionId?: string, tagIds?: string[], subFolder?: string) => {
    const formData = new FormData()
    formData.append('file', file)
    if (description) formData.append('description', description)
    if (countryId) formData.append('countryId', countryId)
    if (regionId) formData.append('regionId', regionId)
    if (subFolder) formData.append('subFolder', subFolder)
    if (tagIds && tagIds.length > 0) {
      tagIds.forEach((t) => formData.append('tagIds', t))
    }

    return client.post<Document>('/documents/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    }).then((r) => r.data)
  },

  uploadVersion: (id: string, file: File, description?: string) => {
    const formData = new FormData()
    formData.append('file', file)
    if (description) formData.append('description', description)

    return client.post<Document>(`/documents/${id}/versions`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    }).then((r) => r.data)
  },

  move: (id: string, targetSubFolder: string) =>
    client.put<Document>(`/documents/${id}/move`, { targetSubFolder }).then((r) => r.data),

  delete: (id: string, deletePhysicalFile = true) =>
    client.delete(`/documents/${id}`, { params: { deletePhysicalFile } }),

  getDownloadUrl: (id: string) => `/api/documents/${id}/download`,
}

