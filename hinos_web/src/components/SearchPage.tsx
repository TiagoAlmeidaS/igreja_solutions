import { useState, useEffect } from 'react';
import { Search } from 'lucide-react';
import { Input } from './ui/input';
import { Badge } from './ui/badge';
import { Card } from './ui/card';
import { CategoryFilter, Hymn } from '../types/hymn';
import { ThemeToggle } from './ThemeToggle';
import { getAllHymns, searchHymns } from '../services/api';

interface SearchPageProps {
  onSelectHymn: (hymnId: string) => void;
}

export function SearchPage({ onSelectHymn }: SearchPageProps) {
  const [searchTerm, setSearchTerm] = useState('');
  const [activeFilter, setActiveFilter] = useState<CategoryFilter>('todos');
  const [hymns, setHymns] = useState<Hymn[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const categories = [
    { id: 'todos' as CategoryFilter, label: 'Todos' },
    { id: 'hinario' as CategoryFilter, label: 'Hinário' },
    { id: 'canticos' as CategoryFilter, label: 'Cânticos' },
    { id: 'suplementar' as CategoryFilter, label: 'Suplementar' },
    { id: 'novos' as CategoryFilter, label: 'Novos' }
  ];

  // Carregar hinos ao montar o componente e quando o filtro ou busca mudar
  useEffect(() => {
    const loadHymns = async () => {
      try {
        setLoading(true);
        setError(null);
        
        let result;
        if (searchTerm.trim()) {
          result = await searchHymns(searchTerm.trim());
        } else {
          result = await getAllHymns(activeFilter !== 'todos' ? activeFilter : undefined);
        }
        
        // Aplicar filtro de categoria se houver busca
        if (searchTerm.trim() && activeFilter !== 'todos') {
          result = result.filter(hymn => hymn.category === activeFilter);
        }
        
        setHymns(result);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar hinos');
        console.error('Erro ao carregar hinos:', err);
      } finally {
        setLoading(false);
      }
    };

    loadHymns();
  }, [searchTerm, activeFilter]);

  const filteredHymns = hymns;

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
            <ThemeToggle />
          </div>
          <p className="text-muted-foreground">
            Encontre hinos por número ou palavra-chave
          </p>
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
          {loading && (
            <div className="text-center py-12">
              <div className="text-muted-foreground">Carregando hinos...</div>
            </div>
          )}

          {error && (
            <div className="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-4 mb-6">
              <p className="text-red-800 dark:text-red-200">Erro: {error}</p>
            </div>
          )}

          {!loading && !error && (
            <>
              {searchTerm && (
                <div className="mb-4 text-muted-foreground">
                  {filteredHymns.length} {filteredHymns.length === 1 ? 'resultado encontrado' : 'resultados encontrados'}
                </div>
              )}

              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                {filteredHymns.map(hymn => (
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

              {filteredHymns.length === 0 && searchTerm && (
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
