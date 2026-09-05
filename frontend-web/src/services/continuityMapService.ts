import { apiClient } from './apiClient';
import { Asset, ApiResponse } from '../types';

export const continuityMapService = {
  getContinuityMap: async (ownerId?: string): Promise<Asset[]> => {
    try {
      const response = await apiClient.get<ApiResponse<Asset[]>>('/ContinuityMap', {
        params: { ownerId }
      });
      return response.data.data;
    } catch {
      // Fallback sample data if backend is offline
      return [
        {
          id: '1',
          name: 'Real Estate Villa',
          description: 'Primary family residence',
          type: 2,
          estimatedValue: 500000,
          createdAtUtc: new Date().toISOString()
        },
        {
          id: '2',
          name: 'Global Equity Portfolio',
          description: 'Investment stocks & bonds',
          type: 1,
          estimatedValue: 250000,
          createdAtUtc: new Date().toISOString()
        },
        {
          id: '3',
          name: 'Cold Storage Vault',
          description: 'Digital reserve (BTC/ETH)',
          type: 3,
          estimatedValue: 120000,
          createdAtUtc: new Date().toISOString()
        }
      ];
    }
  }
};
