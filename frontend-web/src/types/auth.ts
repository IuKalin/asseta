export interface User {
  id: string;
  email: string;
  fullName: string;
  phoneNumber?: string;
  encryptionSalt: string;
  status: string;
  createdAtUtc: string;
  masterKeyVerifier?: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresInSeconds: number;
  user: User;
  masterKey?: string;
}

export interface LoginPayload {
  email: string;
  password: string;
}

export interface RegisterPayload {
  email: string;
  password: string;
  fullName: string;
  phoneNumber?: string;
}

export interface VerifyMasterKeyResponse {
  isValid: boolean;
  encryptionSalt: string;
}
