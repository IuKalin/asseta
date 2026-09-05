import { useState } from 'react';
import { Header } from './components/Header';
import { Sidebar } from './components/Sidebar';
import { ContinuityMapPage } from './features/ContinuityMap/ContinuityMapPage';
import { ActionCardList } from './features/ActionCard/ActionCardList';
import { TrustedPeopleList } from './features/TrustedPeople/TrustedPeopleList';
import { ContinuityPlanView } from './features/ContinuityPlan/ContinuityPlanView';
import { SafeActivationPanel } from './features/SafeActivation/SafeActivationPanel';

export function App() {
  const [currentTab, setCurrentTab] = useState('map');

  return (
    <div className="min-h-screen flex flex-col bg-[#090d16] text-slate-100">
      <Header />
      <div className="flex-1 flex">
        <Sidebar currentTab={currentTab} onTabChange={setCurrentTab} />
        <main className="flex-1 p-8 overflow-y-auto">
          {currentTab === 'map' && <ContinuityMapPage />}
          {currentTab === 'cards' && <ActionCardList />}
          {currentTab === 'people' && <TrustedPeopleList />}
          {currentTab === 'plan' && <ContinuityPlanView />}
          {currentTab === 'activation' && <SafeActivationPanel />}
        </main>
      </div>
    </div>
  );
}

export default App;
