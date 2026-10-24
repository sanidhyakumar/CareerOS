import axios from 'axios';

const API_BASE = '/api/search';

export interface SearchResultItem {
  entityId: string;
  entityType: 'Company' | 'Job' | 'Application' | 'Contact' | 'Document' | 'Wiki' | 'Interview' | string;
  title: string;
  subtitle: string;
  snippet: string;
  actionUrl: string;
  tags: string;
  score: number;
  profileName?: string;
  profileColor?: string;
}

export interface SearchSummary {
  query: string;
  totalMatches: number;
  elapsedMilliseconds: number;
  matchesByType: Record<string, number>;
  results: SearchResultItem[];
}

export interface SearchStats {
  totalIndexedRecords: number;
  recordsByType: Record<string, number>;
  lastIndexedAt: string;
}

export const searchApi = {
  search: async (q: string, type?: string, limit: number = 30): Promise<SearchSummary> => {
    const params: Record<string, any> = { q, limit };
    if (type && type !== 'All') {
      params.type = type;
    }
    const response = await axios.get<SearchSummary>(API_BASE, { params });
    return response.data;
  },

  getStats: async (): Promise<SearchStats> => {
    const response = await axios.get<SearchStats>(`${API_BASE}/stats`);
    return response.data;
  },

  reindex: async (): Promise<{ totalIndexed: number; message: string }> => {
    const response = await axios.post<{ totalIndexed: number; message: string }>(`${API_BASE}/reindex`);
    return response.data;
  },
};
