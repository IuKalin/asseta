import axios from 'axios';
import { ApiResponse } from '../types/continuity';
import { AuthResponse, LoginPayload, RegisterPayload, User, VerifyMasterKeyResponse } from '../types/auth';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api/v1';

export const authClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const authApi = {
  async register(payload: RegisterPayload): Promise<AuthResponse> {
    const response = await authClient.post<ApiResponse<AuthResponse>>('/auth/register', payload);
    return response.data.data;
  },

  async login(payload: LoginPayload): Promise<AuthResponse> {
    const response = await authClient.post<ApiResponse<AuthResponse>>('/auth/login', payload);
    return response.data.data;
  },

  async refreshToken(refreshToken: string): Promise<AuthResponse> {
    const response = await authClient.post<ApiResponse<AuthResponse>>('/auth/refresh', { refreshToken });
    return response.data.data;
  },

  async getMe(token: string): Promise<User> {
    const response = await authClient.get<ApiResponse<User>>('/auth/me', {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data.data;
  },

  async verifyMasterKey(masterKey: string, token?: string): Promise<VerifyMasterKeyResponse> {
    const authToken = token || localStorage.getItem('asseta_auth_token');
    const response = await authClient.post<ApiResponse<VerifyMasterKeyResponse>>(
      '/auth/verify-master-key',
      { masterKey },
      {
        headers: authToken ? { Authorization: `Bearer ${authToken}` } : {},
      }
    );
    return response.data.data;
  },
};

