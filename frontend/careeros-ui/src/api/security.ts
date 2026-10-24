import axios from 'axios';

const API_BASE = '/api/security';

export interface SecurityStatus {
  isMasterPasswordConfigured: boolean;
  isVaultUnlocked: boolean;
  totalCredentials: number;
  passwordsNeedingReview: number;
}

export interface CredentialSummary {
  id: string;
  serviceName: string;
  portalUrl?: string;
  username: string;
  maskedPassword: string;
  hasNotes: boolean;
  companyId?: string;
  companyName?: string;
  jobApplicationId?: string;
  jobTitle?: string;
  lastUsedAt?: string;
  passwordLastChangedAt: string;
  createdAt: string;
}

export interface DecryptedCredential {
  id: string;
  serviceName: string;
  portalUrl?: string;
  username: string;
  password: string;
  notes?: string;
  companyId?: string;
  companyName?: string;
  jobApplicationId?: string;
}

export interface CreateCredentialPayload {
  serviceName: string;
  username: string;
  password: string;
  portalUrl?: string;
  notes?: string;
  companyId?: string;
  jobApplicationId?: string;
}

export interface UpdateCredentialPayload {
  id: string;
  serviceName: string;
  username: string;
  password?: string;
  portalUrl?: string;
  notes?: string;
  companyId?: string;
}

export const securityApi = {
  getStatus: async (): Promise<SecurityStatus> => {
    const res = await axios.get<SecurityStatus>(`${API_BASE}/status`);
    return res.data;
  },

  setupMasterPassword: async (masterPassword: string): Promise<boolean> => {
    const res = await axios.post<boolean>(`${API_BASE}/setup`, { masterPassword });
    return res.data;
  },

  unlockVault: async (masterPassword: string): Promise<boolean> => {
    const res = await axios.post<{ success: boolean; message: string }>(`${API_BASE}/unlock`, { masterPassword });
    return res.data.success;
  },

  lockVault: async (): Promise<boolean> => {
    const res = await axios.post<{ success: boolean; message: string }>(`${API_BASE}/lock`);
    return res.data.success;
  },

  getCredentials: async (companyId?: string): Promise<CredentialSummary[]> => {
    const params = companyId ? { companyId } : {};
    const res = await axios.get<CredentialSummary[]>(`${API_BASE}/credentials`, { params });
    return res.data;
  },

  revealCredential: async (id: string): Promise<DecryptedCredential> => {
    const res = await axios.get<DecryptedCredential>(`${API_BASE}/credentials/${id}/reveal`);
    return res.data;
  },

  createCredential: async (payload: CreateCredentialPayload): Promise<CredentialSummary> => {
    const res = await axios.post<CredentialSummary>(`${API_BASE}/credentials`, payload);
    return res.data;
  },

  updateCredential: async (id: string, payload: UpdateCredentialPayload): Promise<CredentialSummary> => {
    const res = await axios.put<CredentialSummary>(`${API_BASE}/credentials/${id}`, payload);
    return res.data;
  },

  deleteCredential: async (id: string): Promise<void> => {
    await axios.delete(`${API_BASE}/credentials/${id}`);
  },
};
