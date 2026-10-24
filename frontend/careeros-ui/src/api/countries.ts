import client from './client'

export interface Country {
  id: string
  name: string
  code: string
  isActive: boolean
  regionCount: number
}

export interface Region {
  id: string
  countryId: string
  name: string
  code: string
}

export const countriesApi = {
  getAll: (activeOnly = false) => client.get<Country[]>('/countries', { params: { activeOnly } }).then((r) => r.data),
  create: (name: string, code: string) => client.post<Country>('/countries', { name, code }).then((r) => r.data),
  getRegions: (countryId: string) => client.get<Region[]>(`/countries/${countryId}/regions`).then((r) => r.data),
}

