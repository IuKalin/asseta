import { apiClient } from './apiClient';
import { ApiResponse } from '../types';
import {
  TrustedPerson,
  CreateTrustedPersonInput,
  UpdateTrustedPersonInput,
  ClaimPairingResult,
  ScopedPermission,
  UpdateScopedPermissionsInput,
  DelegatedRole,
} from '../types/trustedPeople';

const getHeaders = (idempotency: boolean = false) => {
  const headers: Record<string, string> = {
    'X-Correlation-Id': crypto.randomUUID ? crypto.randomUUID() : 'mock-uuid',
  };
  if (idempotency && crypto.randomUUID) {
    headers['Idempotency-Key'] = crypto.randomUUID();
  }
  return headers;
};

export const trustedPeopleService = {
  async getTrustedPeople(): Promise<TrustedPerson[]> {
    const res = await apiClient.get<ApiResponse<TrustedPerson[]>>('/v1/trusted-people', {
      headers: getHeaders(),
    });
    return res.data.data;
  },

  async getTrustedPersonById(id: string): Promise<TrustedPerson> {
    const res = await apiClient.get<ApiResponse<TrustedPerson>>(`/v1/trusted-people/${id}`, {
      headers: getHeaders(),
    });
    return res.data.data;
  },

  async createTrustedPerson(input: CreateTrustedPersonInput): Promise<TrustedPerson> {
    const res = await apiClient.post<ApiResponse<TrustedPerson>>('/v1/trusted-people', input, {
      headers: getHeaders(true),
    });
    return res.data.data;
  },

  async updateTrustedPerson(id: string, input: UpdateTrustedPersonInput): Promise<TrustedPerson> {
    const res = await apiClient.put<ApiResponse<TrustedPerson>>(`/v1/trusted-people/${id}`, input, {
      headers: getHeaders(true),
    });
    return res.data.data;
  },

  async revokeTrustedPerson(id: string): Promise<boolean> {
    const res = await apiClient.delete<ApiResponse<boolean>>(`/v1/trusted-people/${id}`, {
      headers: getHeaders(),
    });
    return res.data.data;
  },

  async regeneratePairingCode(id: string): Promise<TrustedPerson> {
    const res = await apiClient.post<ApiResponse<TrustedPerson>>(
      `/v1/trusted-people/${id}/pairing-code/regenerate`,
      {},
      { headers: getHeaders(true) }
    );
    return res.data.data;
  },

  async claimPairingCode(pairingCode: string): Promise<ClaimPairingResult> {
    const res = await apiClient.post<ApiResponse<ClaimPairingResult>>(
      '/v1/trusted-people/pairing/claim',
      { pairingCode },
      { headers: getHeaders(true) }
    );
    return res.data.data;
  },

  async updateScopedPermissions(
    id: string,
    input: UpdateScopedPermissionsInput
  ): Promise<ScopedPermission[]> {
    const res = await apiClient.put<ApiResponse<ScopedPermission[]>>(
      `/v1/trusted-people/${id}/permissions`,
      input,
      { headers: getHeaders(true) }
    );
    return res.data.data;
  },

  async getMyDelegatedRoles(): Promise<DelegatedRole[]> {
    const res = await apiClient.get<ApiResponse<DelegatedRole[]>>(
      '/v1/trusted-people/my-delegated-roles',
      { headers: getHeaders() }
    );
    return res.data.data;
  },
};
