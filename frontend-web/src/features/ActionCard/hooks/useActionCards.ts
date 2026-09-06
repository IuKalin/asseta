import { useState, useEffect, useCallback } from 'react';
import { actionCardApi } from '../api/actionCardApi';
import { getErrorMessage } from '../../../utils/errorUtils';
import {
  ActionCard,
  ActionCardFilters,
  ActionCardTemplate,
  CreateActionCardInput,
  CreateFromItemInput,
  UpdateActionCardInput,
  AddContactInput,
} from '../../../types/actionCard';

export function useActionCards(initialFilters?: ActionCardFilters) {
  const [cards, setCards] = useState<ActionCard[]>([]);
  const [filters, setFilters] = useState<ActionCardFilters | undefined>(initialFilters);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const fetchCards = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await actionCardApi.getActionCards(filters);
      setCards(data);
    } catch (err: any) {
      setError(getErrorMessage(err, 'Failed to fetch action cards'));
    } finally {
      setLoading(false);
    }
  }, [filters]);

  useEffect(() => {
    fetchCards();
  }, [fetchCards]);

  const createCard = async (data: CreateActionCardInput): Promise<ActionCard> => {
    const created = await actionCardApi.createActionCard(data);
    setCards((prev) => [created, ...prev]);
    return created;
  };

  const createFromItem = async (data: CreateFromItemInput): Promise<ActionCard> => {
    const created = await actionCardApi.createActionCardFromItem(data);
    setCards((prev) => [created, ...prev]);
    return created;
  };

  const updateCard = async (id: string, data: UpdateActionCardInput): Promise<ActionCard> => {
    const updated = await actionCardApi.updateActionCard(id, data);
    setCards((prev) => prev.map((c) => (c.id === id ? updated : c)));
    return updated;
  };

  const deleteCard = async (id: string): Promise<boolean> => {
    const success = await actionCardApi.deleteActionCard(id);
    if (success) {
      setCards((prev) => prev.filter((c) => c.id !== id));
    }
    return success;
  };

  return {
    cards,
    loading,
    error,
    filters,
    setFilters,
    fetchCards,
    createCard,
    createFromItem,
    updateCard,
    deleteCard,
  };
}

export function useActionCardDetail(cardId: string | null) {
  const [card, setCard] = useState<ActionCard | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const fetchDetail = useCallback(async () => {
    if (!cardId) {
      setCard(null);
      return;
    }
    setLoading(true);
    setError(null);
    try {
      const data = await actionCardApi.getActionCardById(cardId);
      setCard(data);
    } catch (err: any) {
      setError(getErrorMessage(err, 'Failed to load card detail'));
    } finally {
      setLoading(false);
    }
  }, [cardId]);

  useEffect(() => {
    fetchDetail();
  }, [fetchDetail]);

  const toggleStep = async (stepId: string, isCompleted: boolean) => {
    if (!card) return;
    const step = card.steps.find((s) => s.id === stepId);
    if (!step) return;

    const updated = await actionCardApi.updateStep(card.id, stepId, {
      instruction: step.instruction,
      estimatedDuration: step.estimatedDuration,
      isCompleted,
    });

    setCard((prev) => {
      if (!prev) return null;
      const updatedSteps = prev.steps.map((s) => (s.id === stepId ? updated : s));
      const allCompleted = updatedSteps.length > 0 && updatedSteps.every((s) => s.isCompleted);
      return { ...prev, steps: updatedSteps, isCompleted: allCompleted };
    });
  };

  const addStep = async (instruction: string, duration?: string) => {
    if (!card) return;
    const newStep = await actionCardApi.addStep(card.id, { instruction, estimatedDuration: duration });
    setCard((prev) => (prev ? { ...prev, steps: [...prev.steps, newStep] } : null));
  };

  const deleteStep = async (stepId: string) => {
    if (!card) return;
    await actionCardApi.deleteStep(card.id, stepId);
    setCard((prev) =>
      prev
        ? {
            ...prev,
            steps: prev.steps
              .filter((s) => s.id !== stepId)
              .map((s, idx) => ({ ...s, stepOrder: idx + 1 })),
          }
        : null
    );
  };

  const reorderSteps = async (orderedStepIds: string[]) => {
    if (!card) return;
    const reordered = await actionCardApi.reorderSteps(card.id, orderedStepIds);
    setCard((prev) => (prev ? { ...prev, steps: reordered } : null));
  };

  const addContact = async (data: AddContactInput) => {
    if (!card) return;
    const newContact = await actionCardApi.addContact(card.id, data);
    setCard((prev) => (prev ? { ...prev, contacts: [...prev.contacts, newContact] } : null));
  };

  const deleteContact = async (contactId: string) => {
    if (!card) return;
    await actionCardApi.deleteContact(card.id, contactId);
    setCard((prev) =>
      prev ? { ...prev, contacts: prev.contacts.filter((c) => c.id !== contactId) } : null
    );
  };

  return {
    card,
    loading,
    error,
    refetch: fetchDetail,
    toggleStep,
    addStep,
    deleteStep,
    reorderSteps,
    addContact,
    deleteContact,
  };
}

export function useActionCardTemplates(categoryCode?: string) {
  const [templates, setTemplates] = useState<ActionCardTemplate[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const fetchTemplates = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await actionCardApi.getActionCardTemplates(categoryCode);
      setTemplates(data);
    } catch (err: any) {
      setError(getErrorMessage(err, 'Failed to fetch templates'));
    } finally {
      setLoading(false);
    }
  }, [categoryCode]);

  useEffect(() => {
    fetchTemplates();
  }, [fetchTemplates]);

  return { templates, loading, error, refetch: fetchTemplates };
}
