import { useState, useEffect } from 'react';
import { Search, MessageSquarePlus, Loader2, AlertCircle } from 'lucide-react';
import { Input } from './ui/input';
import { Badge } from './ui/badge';
import { Card } from './ui/card';
import { Button } from './ui/button';
import { CategoryFilter, Hymn } from '../types/hymn';
import { ThemeToggle } from './ThemeToggle';
import { useAuth } from '../contexts/AuthContext';
import { getAllHymns, searchHymns } from '../services/api';

interface SearchPageProps {
  onSelectHymn: (hymnId: string) => void;
  onOpenChat: () => void;
}

export function SearchPage({ onSelectHymn, onOpenChat }: SearchPageProps) {
  const [searchTerm, setSearchTerm] = useState('');
  const [debouncedSearchTerm, setDebouncedSearchTerm] = useState('');
  const [activeFilter, setActiveFilter] = useState<CategoryFilter>('todos');
  const [hymns, setHymns] = useState<Hymn[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { isAuthenticated } = useAuth();

  const categories = [
    { id: 'todos' as CategoryFilter, label: 'Todos' },
    { id: 'hinario' as CategoryFilter, label: 'Hinário' },
    { id: 'canticos' as CategoryFilter, label: 'Cânticos' },
    { id: 'suplementar' as CategoryFilter, label: 'Suplementar' },
    { id: 'novos' as CategoryFilter, label: 'Novos' }
  ];

  // Debounce do termo de busca
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearchTerm(searchTerm);
    }, 500); // 500ms de delay

    return () => clearTimeout(timer);
  }, [searchTerm]);

  // Carregar hinos da API
  useEffect(() => {
    const loadHymns = async () => {
      setIsLoading(true);
      setError(null);

      try {
        let results: Hymn[];

        const categoryFilter = activeFilter !== 'todos' ? activeFilter : undefined;
        const searchTerm = debouncedSearchTerm.trim();

        if (searchTerm && categoryFilter) {
          // Usar getAllHymns com ambos os filtros (API suporta category + search simultaneamente)
          results = await getAllHymns(categoryFilter, searchTerm);
        } else if (searchTerm) {
          // Apenas busca por termo
          results = await searchHymns(searchTerm);
        } else {
          // Apenas filtro de categoria (ou todos)
          results = await getAllHymns(categoryFilter);
        }

        setHymns(results);
      } catch (err: any) {
        console.error('Erro ao carregar hinos:', err);
        setError(err.message || 'Erro ao carregar hinos. Verifique sua conexão.');
        setHymns([]);
      } finally {
        setIsLoading(false);
      }
    };

    loadHymns();
  }, [debouncedSearchTerm, activeFilter]);

  // Auto-focus no campo de busca
  useEffect(() => {
    const input = document.getElementById('hymn-search');
    if (input) {
      input.focus();
    }
  }, []);

  const getCategoryBadgeColor = (category: string) => {
    switch (category) {
      case 'hinario': return 'bg-blue-100 text-blue-900 dark:bg-blue-900/30 dark:text-blue-300';
      case 'canticos': return 'bg-green-100 text-green-900 dark:bg-green-900/30 dark:text-green-300';
      case 'suplementar': return 'bg-purple-100 text-purple-900 dark:bg-purple-900/30 dark:text-purple-300';
      case 'novos': return 'bg-orange-100 text-orange-900 dark:bg-orange-900/30 dark:text-orange-300';
      default: return 'bg-gray-100 text-gray-900 dark:bg-gray-800 dark:text-gray-300';
    }
  };

  const getCategoryLabel = (category: string) => {
    switch (category) {
      case 'hinario': return 'Hinário';
      case 'canticos': return 'Cânticos';
      case 'suplementar': return 'Suplementar';
      case 'novos': return 'Novos';
      default: return category;
    }
  };

  return (
    <div className="min-h-screen bg-background">
      <div className="max-w-5xl mx-auto px-4 py-8">
        {/* Header */}
        <div className="mb-8">
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center gap-3">
              <Search className="w-8 h-8 text-foreground/70" />
              <h1 className="text-foreground">Hinário Online</h1>
            </div>
            <div className="flex items-center gap-2">
              <Button
                onClick={onOpenChat}
                variant="outline"
                className="gap-2"
              >
                <MessageSquarePlus className="w-4 h-4" />
                <span className="hidden sm:inline">
                  {isAuthenticated ? 'Chat IA' : 'Entrar'}
                </span>
              </Button>
              <ThemeToggle />
            </div>
          </div>
          <p className="text-muted-foreground">
            Encontre hinos por número ou palavra-chave
          </p>

          {/* CTA para Chat IA */}
          {!isAuthenticated && (
            <div className="mt-6 p-4 bg-primary/5 border border-primary/20 rounded-lg">
              <div className="flex items-start justify-between gap-4">
                <div className="flex-1">
                  <h3 className="text-foreground mb-1">
                    💬 Cadastre novos hinos com IA
                  </h3>
                  <p className="text-sm text-muted-foreground">
                    Acesse o chat inteligente para solicitar o cadastro de hinos de forma simples e rápida
                  </p>
                </div>
                <Button
                  onClick={onOpenChat}
                  className="gap-2 shrink-0"
                >
                  <MessageSquarePlus className="w-4 h-4" />
                  Acessar Chat
                </Button>
              </div>
            </div>
          )}
        </div>

        {/* Search Input */}
        <div className="relative mb-6">
          <div className="relative">
            <Input
              id="hymn-search"
              type="text"
              placeholder="Digite o número ou trecho do hino..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-12 pr-4 py-6 text-lg"
            />
            <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
          </div>
        </div>

        {/* Category Filters */}
        <div className="flex flex-wrap gap-2 mb-6 justify-center">
          {categories.map(cat => (
            <button
              key={cat.id}
              onClick={() => setActiveFilter(cat.id)}
              className={`px-4 py-2 rounded-full transition-all ${
                activeFilter === cat.id
                  ? 'bg-primary text-primary-foreground'
                  : 'bg-card text-foreground hover:bg-accent border border-border'
              }`}
            >
              {cat.label}
            </button>
          ))}
        </div>

        {/* Tips */}
        {!searchTerm && (
          <div className="bg-muted/50 border border-border rounded-lg p-4 mb-8">
            <div className="flex gap-2 mb-2">
              <span>💡</span>
              <span className="text-foreground">Dicas rápidas:</span>
            </div>
            <ul className="text-muted-foreground space-y-1 ml-7 text-sm">
              <li>• "101" → Hino 101 (todos os hinários)</li>
              <li>• "graça" → busca em títulos e letras</li>
              <li>• "S" → mostra todos os Suplementares</li>
              <li>• "N" → mostra todos os Novos Cânticos</li>
            </ul>
          </div>
        )}

        {/* Results */}
        <div>
          {error && (
            <div className="mb-6 p-4 bg-destructive/10 border border-destructive/20 rounded-lg">
              <div className="flex items-start gap-3">
                <AlertCircle className="w-5 h-5 text-destructive shrink-0 mt-0.5" />
                <div className="flex-1">
                  <p className="text-destructive font-medium mb-1">Erro ao carregar hinos</p>
                  <p className="text-sm text-destructive/80">{error}</p>
                  <Button
                    variant="outline"
                    size="sm"
                    className="mt-3"
                    onClick={() => {
                      setError(null);
                      setSearchTerm('');
                      setDebouncedSearchTerm('');
                    }}
                  >
                    Tentar novamente
                  </Button>
                </div>
              </div>
            </div>
          )}

          {isLoading && (
            <div className="text-center py-12">
              <Loader2 className="w-8 h-8 animate-spin text-primary mx-auto mb-4" />
              <p className="text-muted-foreground">Carregando hinos...</p>
            </div>
          )}

          {!isLoading && !error && (
            <>
              {searchTerm && (
                <div className="mb-4 text-muted-foreground">
                  {hymns.length} {hymns.length === 1 ? 'resultado encontrado' : 'resultados encontrados'}
                </div>
              )}

              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                {hymns.map(hymn => (
                  <Card
                    key={hymn.id}
                    onClick={() => onSelectHymn(hymn.id)}
                    className="p-4 hover:shadow-lg transition-all cursor-pointer hover:border-primary/50 bg-card"
                  >
                    <div className="flex items-start justify-between gap-2 mb-2">
                      <div className="text-primary shrink-0">
                        #{hymn.number}
                      </div>
                      <Badge className={`${getCategoryBadgeColor(hymn.category)} shrink-0`}>
                        {getCategoryLabel(hymn.category)}
                      </Badge>
                    </div>
                    <h3 className="text-foreground mb-1">{hymn.title}</h3>
                    <p className="text-muted-foreground text-sm">{hymn.hymnBook}</p>
                    {hymn.verses[0] && (
                      <p className="text-muted-foreground text-sm mt-2 line-clamp-2">
                        {hymn.verses[0].lines[0]}...
                      </p>
                    )}
                  </Card>
                ))}
              </div>

              {hymns.length === 0 && !searchTerm && !isLoading && (
                <div className="text-center py-12">
                  <div className="text-muted-foreground mb-2 text-4xl">🎵</div>
                  <p className="text-foreground">
                    Digite um termo para buscar hinos
                  </p>
                  <p className="text-muted-foreground text-sm mt-2">
                    Ou selecione uma categoria acima
                  </p>
                </div>
              )}

              {hymns.length === 0 && searchTerm && !isLoading && (
                <div className="text-center py-12">
                  <div className="text-muted-foreground mb-2 text-4xl">🔍</div>
                  <p className="text-foreground">
                    Nenhum hino encontrado para "{searchTerm}"
                  </p>
                  <p className="text-muted-foreground text-sm mt-2">
                    Tente usar palavras-chave diferentes ou o número do hino
                  </p>
                </div>
              )}
            </>
          )}
        </div>
      </div>
    </div>
  );
}
