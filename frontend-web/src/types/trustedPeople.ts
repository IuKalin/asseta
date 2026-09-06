export type TrustedPersonStatus = 'Invited' | 'Active' | 'Suspended' | 'Revoked';

export type PermissionType = 'Category' | 'ActionCard';

export interface ScopedPermission {
  id: string;
  trustedPersonId: string;
  permissionType: PermissionType;
  targetCategoryId?: string | null;
  categoryNameVi?: string | null;
  targetActionCardId?: string | null;
  actionCardTitle?: string | null;
  canView: boolean;
}

export interface TrustedPerson {
  id: string;
  ownerId: string;
  delegateUserId?: string | null;
  fullName: string;
  email: string;
  phoneNumber: string;
  relationship: string;
  roleDescription?: string | null;
  trustLevel: number; // 1: Notice Only, 2: Scoped Delegate, 3: Primary Delegate
  status: TrustedPersonStatus;
  activePairingCode?: string | null;
  pairingExpiresAt?: string | null;
  rowVersion: number;
  createdAtUtc: string;
  permissions: ScopedPermission[];
}

export interface CreateTrustedPersonInput {
  fullName: string;
  email: string;
  phoneNumber: string;
  relationship: string;
  trustLevel: number;
  roleDescription?: string | null;
}

export interface UpdateTrustedPersonInput {
  fullName: string;
  email: string;
  phoneNumber: string;
  relationship: string;
  trustLevel: number;
  expectedRowVersion: number;
  roleDescription?: string | null;
}

export interface ClaimPairingResult {
  trustedPersonId: string;
  ownerId: string;
  ownerDisplayName: string;
  assignedRole: string;
  status: string;
  pairedAtUtc: string;
}

export interface CategoryPermissionInput {
  categoryId: string;
  canView: boolean;
}

export interface ActionCardPermissionInput {
  actionCardId: string;
  canView: boolean;
}

export interface UpdateScopedPermissionsInput {
  categoryPermissions?: CategoryPermissionInput[];
  actionCardPermissions?: ActionCardPermissionInput[];
}

export interface DelegatedRole {
  trustedPersonId: string;
  ownerId: string;
  ownerDisplayName: string;
  relationship: string;
  roleDescription?: string | null;
  trustLevel: number;
  status: string;
  createdAtUtc: string;
}
