import axios from 'axios';
import {
  ApiResponse,
  AssessmentAnswer,
  AssessmentResult,
  ContinuityGap,
  ContinuityItem,
  ContinuityMapData,
  CreateContinuityItemPayload,
  UpdateContinuityItemPayload,
} from '../../../types/continuity';

const API_BASE_URL = import.meta.env.VITE_API_URL || (import.meta.env.PROD ? '/api/v1' : 'http://localhost:5000/api/v1');

export const continuityClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor to attach X-Correlation-Id, Idempotency-Key and Authorization Bearer
continuityClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('asseta_auth_token');
  if (token && !config.headers['Authorization']) {
    config.headers['Authorization'] = `Bearer ${token}`;
  }

  if (!config.headers['X-Correlation-Id']) {
    config.headers['X-Correlation-Id'] = crypto.randomUUID();
  }

  const method = config.method?.toUpperCase();
  if (method === 'POST' || method === 'PUT' || method === 'DELETE') {
    if (!config.headers['Idempotency-Key']) {
      config.headers['Idempotency-Key'] = crypto.randomUUID();
    }
  }

  return config;
});

export const continuityApi = {
  async getContinuityMap(ownerId?: string): Promise<ContinuityMapData> {
    const url = ownerId ? `/continuity-map?ownerId=${ownerId}` : '/continuity-map';
    const response = await continuityClient.get<ApiResponse<ContinuityMapData>>(url);
    return response.data.data;
  },

  async getContinuityGaps(ownerId?: string): Promise<ContinuityGap[]> {
    const url = ownerId ? `/continuity-map/gaps?ownerId=${ownerId}` : '/continuity-map/gaps';
    const response = await continuityClient.get<ApiResponse<ContinuityGap[]>>(url);
    return response.data.data;
  },

  async getContinuityItemById(id: string): Promise<ContinuityItem> {
    const response = await continuityClient.get<ApiResponse<ContinuityItem>>(`/continuity-items/${id}`);
    return response.data.data;
  },

  async createContinuityItem(payload: CreateContinuityItemPayload): Promise<ContinuityItem> {
    const response = await continuityClient.post<ApiResponse<ContinuityItem>>('/continuity-items', payload);
    return response.data.data;
  },

  async updateContinuityItem(id: string, payload: UpdateContinuityItemPayload): Promise<ContinuityItem> {
    const response = await continuityClient.put<ApiResponse<ContinuityItem>>(`/continuity-items/${id}`, payload);
    return response.data.data;
  },

  async deleteContinuityItem(id: string): Promise<{ deleted: boolean; id: string }> {
    const response = await continuityClient.delete<ApiResponse<{ deleted: boolean; id: string }>>(`/continuity-items/${id}`);
    return response.data.data;
  },

  async reorderContinuityItems(categoryId: string, orderedItemIds: string[]): Promise<{ reordered: boolean }> {
    const response = await continuityClient.put<ApiResponse<{ reordered: boolean }>>('/continuity-items/reorder', {
      categoryId,
      orderedItemIds,
    });
    return response.data.data;
  },

  async submitAssessment(answers: AssessmentAnswer[], version: string = 'v1'): Promise<AssessmentResult> {
    const response = await continuityClient.post<ApiResponse<AssessmentResult>>('/continuity-map/assessment', {
      answers,
      assessmentVersion: version,
    });
    return response.data.data;
  },
};
