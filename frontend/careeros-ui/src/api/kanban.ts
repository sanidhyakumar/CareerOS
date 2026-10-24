import client from './client'

export const KanbanColumnType = {
  Interested: 0,
  Saved: 1,
  Applied: 2,
  Interviewing: 3,
  Offer: 4,
  Rejected: 5,
} as const
export type KanbanColumnType = typeof KanbanColumnType[keyof typeof KanbanColumnType]

export const KanbanColumnLabels: Record<number, string> = {
  0: 'Interested',
  1: 'Saved',
  2: 'Applied',
  3: 'Interviewing',
  4: 'Offer',
  5: 'Rejected',
}

export const KanbanColumnColors: Record<number, string> = {
  0: '#9c27b0', // Purple
  1: '#0288d1', // Light Blue
  2: '#1976d2', // Blue
  3: '#ed6c02', // Orange
  4: '#2e7d32', // Green
  5: '#d32f2f', // Red
}

export interface KanbanCard {
  id: string
  column: number
  columnName: string
  position: number
  title: string
  companyName: string
  companyId?: string
  jobId?: string
  applicationId?: string
  priority: number
  priorityName: string
  salary?: string
  location?: string
  notes?: string
  tags?: string
  tagsList: string[]
  dueDate?: string
  createdAt: string
  updatedAt: string
}

export interface KanbanColumn {
  columnId: number
  title: string
  cards: KanbanCard[]
}

export interface KanbanBoard {
  columns: KanbanColumn[]
  totalCards: number
}

export interface CreateKanbanCardRequest {
  title: string
  companyName: string
  column?: number
  position?: number
  companyId?: string
  jobId?: string
  applicationId?: string
  priority?: number
  salary?: string
  location?: string
  notes?: string
  tags?: string
  dueDate?: string
}

export interface UpdateKanbanCardRequest {
  id: string
  title: string
  companyName: string
  column: number
  position: number
  companyId?: string
  jobId?: string
  applicationId?: string
  priority: number
  salary?: string
  location?: string
  notes?: string
  tags?: string
  dueDate?: string
}

export interface MoveKanbanCardRequest {
  targetColumn: number
  newPosition: number
}

export const kanbanApi = {
  getBoard: async (): Promise<KanbanBoard> => {
    const response = await client.get<KanbanBoard>('/kanban')
    return response.data
  },

  createCard: async (data: CreateKanbanCardRequest): Promise<KanbanCard> => {
    const response = await client.post<KanbanCard>('/kanban/cards', data)
    return response.data
  },

  updateCard: async (id: string, data: UpdateKanbanCardRequest): Promise<KanbanCard> => {
    const response = await client.put<KanbanCard>(`/kanban/cards/${id}`, data)
    return response.data
  },

  moveCard: async (id: string, data: MoveKanbanCardRequest): Promise<KanbanCard> => {
    const response = await client.patch<KanbanCard>(`/kanban/cards/${id}/move`, data)
    return response.data
  },

  deleteCard: async (id: string): Promise<void> => {
    await client.delete(`/kanban/cards/${id}`)
  },
}

