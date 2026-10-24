import client from './client'

export const WikiCategory = {
  General: 0,
  StarStory: 1,
  SystemDesign: 2,
  CompanyResearch: 3,
  InterviewNotes: 4,
  CodingPatterns: 5,
  CareerStrategy: 6,
} as const

export type WikiCategory = typeof WikiCategory[keyof typeof WikiCategory]

export const WikiCategoryLabels: Record<number, string> = {
  0: 'General',
  1: 'STAR Story',
  2: 'System Design',
  3: 'Company Research',
  4: 'Interview Notes',
  5: 'Coding Patterns',
  6: 'Career Strategy',
}

export interface WikiBacklink {
  articleId: string
  title: string
  slug: string
  categoryName: string
}

export interface WikiOutlink {
  targetSlug: string
  targetTitle?: string
  targetArticleId?: string
  linkText?: string
}

export interface WikiArticle {
  id: string
  title: string
  slug: string
  category: number
  categoryName: string
  content: string
  summary?: string
  tags?: string
  tagsList: string[]
  companyId?: string
  companyName?: string
  jobId?: string
  jobTitle?: string
  isStarred: boolean
  isPinned: boolean
  viewCount: number
  backlinks: WikiBacklink[]
  outlinks: WikiOutlink[]
  createdAt: string
  updatedAt: string
}

export interface WikiStats {
  totalArticles: number
  starStoriesCount: number
  systemDesignCount: number
  companyResearchCount: number
  interviewNotesCount: number
  starredCount: number
  tagsCount: number
}

export interface CreateWikiArticleRequest {
  title: string
  content: string
  category: number
  slug?: string
  summary?: string
  tags?: string
  companyId?: string
  jobId?: string
  isStarred?: boolean
  isPinned?: boolean
}

export interface UpdateWikiArticleRequest {
  id: string
  title: string
  content: string
  category: number
  slug?: string
  summary?: string
  tags?: string
  companyId?: string
  jobId?: string
  isStarred?: boolean
  isPinned?: boolean
}

export interface WikiFilterParams {
  searchTerm?: string
  category?: number
  tag?: string
  companyId?: string
  isStarred?: boolean
}

export const wikiApi = {
  getAll: async (params?: WikiFilterParams): Promise<WikiArticle[]> => {
    const response = await client.get<WikiArticle[]>('/wiki', { params })
    return response.data
  },

  getStats: async (): Promise<WikiStats> => {
    const response = await client.get<WikiStats>('/wiki/stats')
    return response.data
  },

  getTags: async (): Promise<string[]> => {
    const response = await client.get<string[]>('/wiki/tags')
    return response.data
  },

  getById: async (id: string, incrementView = true): Promise<WikiArticle> => {
    const response = await client.get<WikiArticle>(`/wiki/${id}`, {
      params: { incrementView },
    })
    return response.data
  },

  getBySlug: async (slug: string, incrementView = true): Promise<WikiArticle> => {
    const response = await client.get<WikiArticle>(`/wiki/slug/${slug}`, {
      params: { incrementView },
    })
    return response.data
  },

  create: async (data: CreateWikiArticleRequest): Promise<WikiArticle> => {
    const response = await client.post<WikiArticle>('/wiki', data)
    return response.data
  },

  update: async (id: string, data: UpdateWikiArticleRequest): Promise<WikiArticle> => {
    const response = await client.put<WikiArticle>(`/wiki/${id}`, data)
    return response.data
  },

  toggleStar: async (id: string): Promise<WikiArticle> => {
    const response = await client.patch<WikiArticle>(`/wiki/${id}/star`)
    return response.data
  },

  delete: async (id: string): Promise<void> => {
    await client.delete(`/wiki/${id}`)
  },
}

