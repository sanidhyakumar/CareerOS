import client from './client'

export interface DashboardStats {
  totalJobs: number
  totalContacts: number
  totalResumes: number
  appliedJobs: number
  interviewJobs: number
  offersReceived: number
}

export const dashboardApi = {
  getStats: () => client.get<DashboardStats>('/dashboard/stats').then((r) => r.data),
}

