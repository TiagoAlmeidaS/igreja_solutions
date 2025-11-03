import { useState } from 'react';
import { SearchPage } from './components/SearchPage';
import { HymnDetailPage } from './components/HymnDetailPage';
import { ThemeProvider } from './contexts/ThemeContext';

type View = 'search' | 'detail';

export default function App() {
  const [currentView, setCurrentView] = useState<View>('search');
  const [selectedHymnId, setSelectedHymnId] = useState<string | null>(null);

  const handleSelectHymn = (hymnId: string) => {
    setSelectedHymnId(hymnId);
    setCurrentView('detail');
  };

  const handleBack = () => {
    setCurrentView('search');
    setSelectedHymnId(null);
  };

  return (
    <ThemeProvider>
      {currentView === 'search' && (
        <SearchPage onSelectHymn={handleSelectHymn} />
      )}
      
      {currentView === 'detail' && selectedHymnId && (
        <HymnDetailPage hymnId={selectedHymnId} onBack={handleBack} />
      )}
    </ThemeProvider>
  );
}
