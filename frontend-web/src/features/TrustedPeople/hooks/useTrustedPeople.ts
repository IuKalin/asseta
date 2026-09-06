import { useState, useEffect, useCallback } from 'react';
import { trustedPeopleService } from '../../../services/trustedPeopleService';
import {
  TrustedPerson,
  CreateTrustedPersonInput,
  UpdateTrustedPersonInput,
  UpdateScopedPermissionsInput,
  ScopedPermission,
} from '../../../types/trustedPeople';

export function useTrustedPeople() {
  const [people, setPeople] = useState<TrustedPerson[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const fetchPeople = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await trustedPeopleService.getTrustedPeople();
      setPeople(data);
    } catch (err: any) {
      setError(err?.response?.data?.error?.message || err.message || 'Không thể tải danh sách Người Ủy Thác.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchPeople();
  }, [fetchPeople]);

  const createPerson = async (input: CreateTrustedPersonInput): Promise<TrustedPerson> => {
    const created = await trustedPeopleService.createTrustedPerson(input);
    setPeople((prev) => [...prev, created]);
    return created;
  };

  const updatePerson = async (id: string, input: UpdateTrustedPersonInput): Promise<TrustedPerson> => {
    const updated = await trustedPeopleService.updateTrustedPerson(id, input);
    setPeople((prev) => prev.map((p) => (p.id === id ? updated : p)));
    return updated;
  };

  const revokePerson = async (id: string): Promise<boolean> => {
    await trustedPeopleService.revokeTrustedPerson(id);
    setPeople((prev) => prev.filter((p) => p.id !== id));
    return true;
  };

  const regeneratePairingCode = async (id: string): Promise<TrustedPerson> => {
    const updated = await trustedPeopleService.regeneratePairingCode(id);
    setPeople((prev) => prev.map((p) => (p.id === id ? updated : p)));
    return updated;
  };

  const updatePermissions = async (
    id: string,
    input: UpdateScopedPermissionsInput
  ): Promise<ScopedPermission[]> => {
    const permissions = await trustedPeopleService.updateScopedPermissions(id, input);
    setPeople((prev) =>
      prev.map((p) => (p.id === id ? { ...p, permissions } : p))
    );
    return permissions;
  };

  return {
    people,
    loading,
    error,
    refresh: fetchPeople,
    createPerson,
    updatePerson,
    revokePerson,
    regeneratePairingCode,
    updatePermissions,
  };
}
