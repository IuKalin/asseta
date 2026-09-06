import React, { createContext, useCallback, useContext, useEffect, useState } from 'react';
import { AuthResponse, LoginPayload, RegisterPayload, User } from '../types/auth';
import { authApi } from '../services/authApi';
import { CryptoService } from '../services/cryptoService';

interface AuthContextType {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  newMasterKey: string | null;
  isVaultUnlocked: boolean;
  vaultCryptoKey: CryptoKey | null;
  unlockedMasterKey: string | null;
  login: (payload: LoginPayload) => Promise<void>;
  register: (payload: RegisterPayload) => Promise<string | undefined>;
  logout: () => void;
  clearNewMasterKey: () => void;
  unlockVault: (masterKey: string) => Promise<void>;
  lockVault: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

const TOKEN_KEY = 'asseta_auth_token';
const REFRESH_TOKEN_KEY = 'asseta_refresh_token';
const USER_KEY = 'asseta_user';
const SALT_KEY = 'asseta_continuity_salt';

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(() => {
    const saved = localStorage.getItem(USER_KEY);
    return saved ? JSON.parse(saved) : null;
  });
  const [token, setToken] = useState<string | null>(() => localStorage.getItem(TOKEN_KEY));
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [newMasterKey, setNewMasterKey] = useState<string | null>(null);

  // Unified In-Memory Vault State (Zero-Knowledge: never persisted to localStorage)
  const [isVaultUnlocked, setIsVaultUnlocked] = useState<boolean>(false);
  const [vaultCryptoKey, setVaultCryptoKey] = useState<CryptoKey | null>(null);
  const [unlockedMasterKey, setUnlockedMasterKey] = useState<string | null>(null);

  const saveAuthSession = (auth: AuthResponse) => {
    localStorage.setItem(TOKEN_KEY, auth.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, auth.refreshToken);
    localStorage.setItem(USER_KEY, JSON.stringify(auth.user));
    if (auth.user.encryptionSalt) {
      localStorage.setItem(SALT_KEY, auth.user.encryptionSalt);
    }
    setToken(auth.accessToken);
    setUser(auth.user);
  };

  const lockVault = useCallback(() => {
    setVaultCryptoKey(null);
    setUnlockedMasterKey(null);
    setIsVaultUnlocked(false);
  }, []);

  const clearAuthSession = () => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    localStorage.removeItem(SALT_KEY);
    setToken(null);
    setUser(null);
    setNewMasterKey(null);
    lockVault();
  };

  // Verify session on mount
  useEffect(() => {
    const checkAuth = async () => {
      const savedToken = localStorage.getItem(TOKEN_KEY);
      if (savedToken) {
        try {
          const profile = await authApi.getMe(savedToken);
          setUser(profile);
          localStorage.setItem(USER_KEY, JSON.stringify(profile));
          if (profile.encryptionSalt) {
            localStorage.setItem(SALT_KEY, profile.encryptionSalt);
          }
        } catch {
          // Attempt refresh token
          const savedRefreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
          if (savedRefreshToken) {
            try {
              const refreshed = await authApi.refreshToken(savedRefreshToken);
              saveAuthSession(refreshed);
            } catch {
              clearAuthSession();
            }
          } else {
            clearAuthSession();
          }
        }
      }
      setIsLoading(false);
    };

    checkAuth();
  }, []);

  const login = useCallback(async (payload: LoginPayload) => {
    setIsLoading(true);
    try {
      const res = await authApi.login(payload);
      saveAuthSession(res);
      lockVault();
    } finally {
      setIsLoading(false);
    }
  }, [lockVault]);

  const unlockVault = useCallback(async (rawMasterKey: string) => {
    const normalizedKey = rawMasterKey.trim().toUpperCase();
    if (!normalizedKey) {
      throw new Error('Vui lòng nhập Master Key.');
    }

    if (!user) {
      throw new Error('Người dùng chưa đăng nhập.');
    }

    // 1. Fast local verification if masterKeyVerifier is in user profile
    if (user.masterKeyVerifier) {
      const localVerifier = await CryptoService.computeMasterKeyVerifier(normalizedKey);
      if (localVerifier !== user.masterKeyVerifier.toLowerCase()) {
        throw new Error(
          'Master Key không chính xác. Vui lòng nhập đúng mã khóa bảo mật được cấp khi đăng ký tài khoản (định dạng AK-XXXX-XXXX-XXXX-XXXX hoặc khóa Demo: AK-DEMO-2026-ASSETA-VAULT).'
        );
      }
    }

    // 2. Server verification
    const res = await authApi.verifyMasterKey(normalizedKey);
    const saltHex = res.encryptionSalt || user.encryptionSalt;
    const saltBytes = CryptoService.hexToBytes(saltHex);

    // 3. Derive AES-256-GCM CryptoKey using verified Master Key & user's persistent DB salt
    const derivedKey = await CryptoService.deriveMasterKey(normalizedKey, saltBytes);

    setVaultCryptoKey(derivedKey);
    setUnlockedMasterKey(normalizedKey);
    setIsVaultUnlocked(true);
  }, [user]);

  const register = useCallback(async (payload: RegisterPayload) => {
    setIsLoading(true);
    try {
      const res = await authApi.register(payload);
      saveAuthSession(res);
      if (res.masterKey) {
        setNewMasterKey(res.masterKey);
        // Automatically unlock vault with the newly minted Master Key
        try {
          const saltBytes = CryptoService.hexToBytes(res.user.encryptionSalt);
          const key = await CryptoService.deriveMasterKey(res.masterKey, saltBytes);
          setVaultCryptoKey(key);
          setUnlockedMasterKey(res.masterKey);
          setIsVaultUnlocked(true);
        } catch (e) {
          console.error('Failed to auto-derive key on register:', e);
        }
      }
      return res.masterKey;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const logout = useCallback(() => {
    clearAuthSession();
  }, [lockVault]);

  const clearNewMasterKey = useCallback(() => {
    setNewMasterKey(null);
  }, []);

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!token && !!user,
        isLoading,
        newMasterKey,
        isVaultUnlocked,
        vaultCryptoKey,
        unlockedMasterKey,
        login,
        register,
        logout,
        clearNewMasterKey,
        unlockVault,
        lockVault,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
