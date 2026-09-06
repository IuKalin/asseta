import { useState } from 'react';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { LoginPage } from './features/Auth/LoginPage';
import { RegisterPage } from './features/Auth/RegisterPage';
import { MasterKeyBackupModal } from './features/Auth/components/MasterKeyBackupModal';
import { Header } from './components/Header';
import { Sidebar } from './components/Sidebar';
import { Footer } from './components/Footer';
import { ContinuityMapPage } from './features/ContinuityMap/ContinuityMapPage';
import { ActionCardList } from './features/ActionCard/ActionCardList';
import { TrustedPeopleList } from './features/TrustedPeople/TrustedPeopleList';
import { ContinuityPlanView } from './features/ContinuityPlan/ContinuityPlanView';
import { SafeActivationPanel } from './features/SafeActivation/SafeActivationPanel';
import { ErrorBoundary } from './components/ErrorBoundary';
import { ShieldCheck, Loader2 } from 'lucide-react';

function MainLayout() {
  const { isAuthenticated, isLoading, user, newMasterKey, clearNewMasterKey } = useAuth();
  const [authView, setAuthView] = useState<'login' | 'register'>('login');
  const [currentTab, setCurrentTab] = useState('map');

  if (isLoading) {
    return (
      <div className="min-h-screen bg-[#F7F7F7] flex flex-col items-center justify-center text-[#222222]">
        <div className="w-14 h-14 rounded-3xl bg-rose-50 border border-rose-200 flex items-center justify-center text-[#FF385C] mb-4 shadow-xs animate-pulse motion-reduce:animate-none">
          <ShieldCheck className="w-8 h-8" />
        </div>
        <div className="flex items-center gap-2 text-sm text-[#717171] font-medium">
          <Loader2 className="w-4 h-4 animate-spin text-[#FF385C]" />
          <span>Đang tải thông tin két Asseta…</span>
        </div>
      </div>
    );
  }

  if (!isAuthenticated) {
    if (authView === 'register') {
      return <RegisterPage onNavigateToLogin={() => setAuthView('login')} />;
    }
    return <LoginPage onNavigateToRegister={() => setAuthView('register')} />;
  }

  return (
    <div className="min-h-screen flex flex-col bg-[#F7F7F7] text-[#222222]">
      <Header />
      <div className="flex-1 flex min-h-[calc(100vh-4rem)]">
        <Sidebar currentTab={currentTab} onTabChange={setCurrentTab} />
        <div className="flex-1 flex flex-col min-w-0">
          <main className="flex-1 p-6 md:p-8">
            {currentTab === 'map' && <ContinuityMapPage />}
            {currentTab === 'cards' && <ActionCardList />}
            {currentTab === 'people' && <TrustedPeopleList />}
            {currentTab === 'plan' && <ContinuityPlanView />}
            {currentTab === 'activation' && <SafeActivationPanel />}
          </main>
          {/* Footer nằm ở chân trang cuộn nội dung, Sidebar giữ nguyên vị trí */}
          <Footer />
        </div>
      </div>

      {newMasterKey && user && (
        <MasterKeyBackupModal
          masterKey={newMasterKey}
          email={user.email}
          onClose={clearNewMasterKey}
        />
      )}
    </div>
  );
}

export function App() {
  return (
    <ErrorBoundary>
      <AuthProvider>
        <MainLayout />
      </AuthProvider>
    </ErrorBoundary>
  );
}

export default App;
