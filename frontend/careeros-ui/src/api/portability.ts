import axios from 'axios';

const API_BASE = '/api/portability';

export interface ImportSummary {
  success: boolean;
  message: string;
  recordsImported: Record<string, number>;
  warnings: string[];
}

// Helper to trigger browser file download from Blob
const downloadBlob = (data: Blob, filename: string) => {
  const url = window.URL.createObjectURL(data);
  const link = document.createElement('a');
  link.href = url;
  link.setAttribute('download', filename);
  document.body.appendChild(link);
  link.click();
  link.remove();
  window.URL.revokeObjectURL(url);
};

export const portabilityApi = {
  exportJson: async () => {
    const res = await axios.get(`${API_BASE}/export/json`, { responseType: 'blob' });
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    downloadBlob(res.data, `careeros-backup-${timestamp}.json`);
  },

  exportCsv: async (entity: 'jobs' | 'applications' | 'companies' | 'contacts') => {
    const res = await axios.get(`${API_BASE}/export/csv/${entity}`, { responseType: 'blob' });
    const dateStr = new Date().toISOString().slice(0, 10);
    downloadBlob(res.data, `${entity}-${dateStr}.csv`);
  },

  exportMarkdown: async () => {
    const res = await axios.get(`${API_BASE}/export/markdown`, { responseType: 'blob' });
    const dateStr = new Date().toISOString().slice(0, 10);
    downloadBlob(res.data, `careeros-wiki-markdown-${dateStr}.zip`);
  },

  exportFullZip: async () => {
    const res = await axios.get(`${API_BASE}/export/zip`, { responseType: 'blob' });
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    downloadBlob(res.data, `careeros-full-export-${timestamp}.zip`);
  },

  importJson: async (file: File, overwrite: boolean = false): Promise<ImportSummary> => {
    const formData = new FormData();
    formData.append('file', file);
    const res = await axios.post<ImportSummary>(`${API_BASE}/import/json?overwrite=${overwrite}`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return res.data;
  },
};
