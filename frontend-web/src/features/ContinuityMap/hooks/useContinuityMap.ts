import { useCallback, useEffect, useState } from 'react';
import { CryptoService } from '../../../services/cryptoService';
import {
  AssessmentAnswer,
  AssessmentResult,
  ContinuityItem,
  ContinuityMapData,
  CreateContinuityItemPayload,
  UpdateContinuityItemPayload,
} from '../../../types/continuity';
import { continuityApi } from '../api/continuityApi';

import { useAuth } from '../../../contexts/AuthContext';

export function useContinuityMap(ownerId?: string) {
  const { isVaultUnlocked, vaultCryptoKey, unlockVault, lockVault } = useAuth();
  const [data, setData] = useState<ContinuityMapData | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetchContinuityMap = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);
      const result = await continuityApi.getContinuityMap(ownerId);
      setData(result);
    } catch (err: any) {
      const msg = err.response?.data?.error?.message || err.message || 'Failed to fetch continuity map';
      setError(msg);
    } finally {
      setIsLoading(false);
    }
  }, [ownerId]);

  useEffect(() => {
    fetchContinuityMap();
  }, [fetchContinuityMap]);

  // Backward compatibility: unlockWithPassphrase now invokes unlockVault (which strictly verifies the user's Master Key)
  const unlockWithPassphrase = useCallback(
    async (masterKey: string) => {
      await unlockVault(masterKey);
    },
    [unlockVault]
  );


  const createItem = useCallback(
    async (payload: CreateContinuityItemPayload, plainNotes?: string) => {
      let finalPayload = { ...payload };

      if (plainNotes && plainNotes.trim() !== '') {
        if (!vaultCryptoKey) {
          throw new Error('Chưa mở khóa Master Key. Vui lòng nhập Master Key để mã hóa ghi chú.');
        }
        const encrypted = await CryptoService.encrypt(vaultCryptoKey, plainNotes);
        finalPayload.cipherNotesBlob = encrypted.cipherNotesBlob;
        finalPayload.cipherNonce = encrypted.cipherNonce;
        finalPayload.cipherAuthTag = encrypted.cipherAuthTag;
      }

      const createdItem = await continuityApi.createContinuityItem(finalPayload);
      await fetchContinuityMap();
      return createdItem;
    },
    [vaultCryptoKey, fetchContinuityMap]
  );

  const updateItem = useCallback(
    async (id: string, payload: UpdateContinuityItemPayload, plainNotes?: string) => {
      let finalPayload = { ...payload };

      if (plainNotes !== undefined) {
        if (plainNotes.trim() === '') {
          finalPayload.cipherNotesBlob = undefined;
          finalPayload.cipherNonce = undefined;
          finalPayload.cipherAuthTag = undefined;
        } else {
          if (!vaultCryptoKey) {
            throw new Error('Chưa mở khóa Master Key. Vui lòng nhập Master Key để mã hóa ghi chú.');
          }
          const encrypted = await CryptoService.encrypt(vaultCryptoKey, plainNotes);
          finalPayload.cipherNotesBlob = encrypted.cipherNotesBlob;
          finalPayload.cipherNonce = encrypted.cipherNonce;
          finalPayload.cipherAuthTag = encrypted.cipherAuthTag;
        }
      }

      const updated = await continuityApi.updateContinuityItem(id, finalPayload);
      await fetchContinuityMap();
      return updated;
    },
    [vaultCryptoKey, fetchContinuityMap]
  );

  const deleteItem = useCallback(
    async (id: string) => {
      await continuityApi.deleteContinuityItem(id);
      await fetchContinuityMap();
    },
    [fetchContinuityMap]
  );

  const reorderItems = useCallback(
    async (categoryId: string, orderedItemIds: string[]) => {
      await continuityApi.reorderContinuityItems(categoryId, orderedItemIds);
      await fetchContinuityMap();
    },
    [fetchContinuityMap]
  );

  const submitAssessment = useCallback(
    async (answers: AssessmentAnswer[]): Promise<AssessmentResult> => {
      const res = await continuityApi.submitAssessment(answers);
      await fetchContinuityMap();
      return res;
    },
    [fetchContinuityMap]
  );

  const decryptNotes = useCallback(
    async (item: ContinuityItem): Promise<string> => {
      if (!item.cipherNotesBlob || !item.cipherNonce || !item.cipherAuthTag) {
        return '';
      }
      if (!vaultCryptoKey) {
        throw new Error('Chưa mở khóa Master Key. Vui lòng mở khóa trước.');
      }
      return await CryptoService.decrypt(
        vaultCryptoKey,
        item.cipherNotesBlob,
        item.cipherNonce,
        item.cipherAuthTag
      );
    },
    [vaultCryptoKey]
  );

  return {
    data,
    isLoading,
    error,
    refetch: fetchContinuityMap,
    masterKey: vaultCryptoKey,
    isKeyUnlocked: isVaultUnlocked,
    unlockWithPassphrase,
    lockKey: lockVault,
    createItem,
    updateItem,
    deleteItem,
    reorderItems,
    submitAssessment,
    decryptNotes,
  };
}
