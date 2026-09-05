export enum AssetType {
  Financial = 1,
  RealEstate = 2,
  Digital = 3,
  Business = 4,
  Personal = 5
}

export interface Asset {
  id: string;
  name: string;
  description: string;
  type: AssetType;
  estimatedValue: number;
  createdAtUtc: string;
}

export interface TrustedPerson {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  relationship: string;
  trustLevel: number;
}

export interface ActionCard {
  id: string;
  title: string;
  priority: 'High' | 'Medium' | 'Low';
  instructions: string;
  assignedToName: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  error?: string | null;
  meta: {
    timestamp: string;
    correlationId: string;
  };
}
