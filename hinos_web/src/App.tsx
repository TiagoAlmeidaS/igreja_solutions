import { useState } from 'react';
import { SearchPage } from './components/SearchPage';
import { HymnDetailPage } from './components/HymnDetailPage';
import { LoginPage } from './components/LoginPage';
import { ChatPage } from './components/ChatPage';
import { ThemeProvider } from './contexts/ThemeContext';
import { AuthProvider, useAuth } from './contexts/AuthContext';

type View = 'search' | 'detail' | 'login' | 'chat';

function AppContent() {
  const [currentView, setCurrentView] = useState<View>('search');
  const [selectedHymnId, setSelectedHymnId] = useState<string | null>(null);
  const { isAuthenticated } = useAuth();

  const handleSelectHymn = (hymnId: string) => {
    setSelectedHymnId(hymnId);
    setCurrentView('detail');
  };

  const handleBack = () => {
    setCurrentView('search');
    setSelectedHymnId(null);
  };

  const handleOpenChat = () => {
    if (isAuthenticated) {
      setCurrentView('chat');
    } else {
      setCurrentView('login');
    }
  };

  const handleLoginSuccess = () => {
    setCurrentView('chat');
  };

  return (
    <>
      {currentView === 'search' && (
        <SearchPage 
          onSelectHymn={handleSelectHymn}
          onOpenChat={handleOpenChat}
        />
      )}
      
      {currentView === 'detail' && selectedHymnId && (
        <HymnDetailPage hymnId={selectedHymnId} onBack={handleBack} />
      )}

      {currentView === 'login' && (
        <LoginPage 
          onLoginSuccess={handleLoginSuccess}
          onBack={handleBack}
        />
      )}

      {currentView === 'chat' && isAuthenticated && (
        <ChatPage onBack={handleBack} />
      )}
    </>
  );
}

export default function App() {
  return (
    <ThemeProvider>
      <AuthProvider>
        <AppContent />
      </AuthProvider>
    </ThemeProvider>
  );
}
