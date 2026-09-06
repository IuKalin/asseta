import { apiClient } from '../../../services/apiClient';
import { ApiResponse } from '../../../types';
import {
  ActionCard,
  ActionCardFilters,
  ActionCardStep,
  ActionCardContact,
  ActionCardTemplate,
  CreateActionCardInput,
  CreateFromItemInput,
  UpdateActionCardInput,
  AddStepInput,
  UpdateStepInput,
  AddContactInput,
} from '../../../types/actionCard';

const getHeaders = (idempotency: boolean = false) => {
  const headers: Record<string, string> = {
    'X-Correlation-Id': crypto.randomUUID(),
  };
  const token = localStorage.getItem('asseta_auth_token');
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }
  if (idempotency) {
    headers['Idempotency-Key'] = crypto.randomUUID();
  }
  return headers;
};

export const actionCardApi = {
  /**
   * Retrieves all action cards with optional urgency, category, and text filters.
   */
  async getActionCards(filters?: ActionCardFilters): Promise<ActionCard[]> {
    const params = new URLSearchParams();
    if (filters?.urgency) params.append('urgency', filters.urgency);
    if (filters?.categoryId) params.append('categoryId', filters.categoryId);
    if (filters?.search) params.append('search', filters.search);

    const response = await apiClient.get<ApiResponse<ActionCard[]>>(
      `/v1/action-cards?${params.toString()}`,
      { headers: getHeaders() }
    );
    return response.data.data;
  },

  /**
   * Retrieves a single action card by its unique identifier.
   */
  async getActionCardById(id: string): Promise<ActionCard> {
    const response = await apiClient.get<ApiResponse<ActionCard>>(
      `/v1/action-cards/${id}`,
      { headers: getHeaders() }
    );
    return response.data.data;
  },

  /**
   * Retrieves standard pre-configured templates.
   */
  async getActionCardTemplates(categoryCode?: string): Promise<ActionCardTemplate[]> {
    const params = categoryCode ? `?categoryCode=${encodeURIComponent(categoryCode)}` : '';
    const response = await apiClient.get<ApiResponse<ActionCardTemplate[]>>(
      `/v1/action-cards/templates${params}`,
      { headers: getHeaders() }
    );
    return response.data.data;
  },

  /**
   * Creates a new action card.
   */
  async createActionCard(data: CreateActionCardInput): Promise<ActionCard> {
    const response = await apiClient.post<ApiResponse<ActionCard>>(
      '/v1/action-cards',
      data,
      { headers: getHeaders(true) }
    );
    return response.data.data;
  },

  /**
   * Creates an action card linked to an existing Continuity Item.
   */
  async createActionCardFromItem(data: CreateFromItemInput): Promise<ActionCard> {
    const response = await apiClient.post<ApiResponse<ActionCard>>(
      '/v1/action-cards/from-item',
      data,
      { headers: getHeaders(true) }
    );
    return response.data.data;
  },

  /**
   * Updates an existing action card with concurrency token.
   */
  async updateActionCard(id: string, data: UpdateActionCardInput): Promise<ActionCard> {
    const response = await apiClient.put<ApiResponse<ActionCard>>(
      `/v1/action-cards/${id}`,
      data,
      { headers: getHeaders(true) }
    );
    return response.data.data;
  },

  /**
   * Soft deletes an action card.
   */
  async deleteActionCard(id: string): Promise<boolean> {
    const response = await apiClient.delete<ApiResponse<boolean>>(
      `/v1/action-cards/${id}`,
      { headers: getHeaders(true) }
    );
    return response.data.data;
  },

  /**
   * Adds an action step to a card.
   */
  async addStep(cardId: string, data: AddStepInput): Promise<ActionCardStep> {
    const response = await apiClient.post<ApiResponse<ActionCardStep>>(
      `/v1/action-cards/${cardId}/steps`,
      data,
      { headers: getHeaders(true) }
    );
    return response.data.data;
  },

  /**
   * Updates a step instruction, duration, or completion status.
   */
  async updateStep(cardId: string, stepId: string, data: UpdateStepInput): Promise<ActionCardStep> {
    const response = await apiClient.put<ApiResponse<ActionCardStep>>(
      `/v1/action-cards/${cardId}/steps/${stepId}`,
      data,
      { headers: getHeaders(true) }
    );
    return response.data.data;
  },

  /**
   * Removes an action step.
   */
  async deleteStep(cardId: string, stepId: string): Promise<boolean> {
    const response = await apiClient.delete<ApiResponse<boolean>>(
      `/v1/action-cards/${cardId}/steps/${stepId}`,
      { headers: getHeaders(true) }
    );
    return response.data.data;
  },

  /**
   * Reorders all active steps on a card.
   */
  async reorderSteps(cardId: string, orderedStepIds: string[]): Promise<ActionCardStep[]> {
    const response = await apiClient.post<ApiResponse<ActionCardStep[]>>(
      `/v1/action-cards/${cardId}/steps/reorder`,
      { orderedStepIds },
      { headers: getHeaders(true) }
    );
    return response.data.data;
  },

  /**
   * Adds a key contact to a card.
   */
  async addContact(cardId: string, data: AddContactInput): Promise<ActionCardContact> {
    const response = await apiClient.post<ApiResponse<ActionCardContact>>(
      `/v1/action-cards/${cardId}/contacts`,
      data,
      { headers: getHeaders(true) }
    );
    return response.data.data;
  },

  /**
   * Removes a key contact from a card.
   */
  async deleteContact(cardId: string, contactId: string): Promise<boolean> {
    const response = await apiClient.delete<ApiResponse<boolean>>(
      `/v1/action-cards/${cardId}/contacts/${contactId}`,
      { headers: getHeaders(true) }
    );
    return response.data.data;
  },
};
