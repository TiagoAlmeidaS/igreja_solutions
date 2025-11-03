import { ArrowLeft, Copy, Download, Check } from 'lucide-react';
import { useState, useEffect } from 'react';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Hymn } from '../types/hymn';
import { ThemeToggle } from './ThemeToggle';
import { getHymnById } from '../services/api';

interface HymnDetailPageProps {
  hymnId: string;
  onBack: () => void;
}

export function HymnDetailPage({ hymnId, onBack }: HymnDetailPageProps) {
  const [copiedPlain, setCopiedPlain] = useState(false);
  const [hymn, setHymn] = useState<Hymn | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadHymn = async () => {
      try {
        setLoading(true);
        setError(null);
        const loadedHymn = await getHymnById(hymnId);
        if (!loadedHymn) {
          setError('Hino não encontrado');
        } else {
          setHymn(loadedHymn);
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar hino');
        console.error('Erro ao carregar hino:', err);
      } finally {
        setLoading(false);
      }
    };

    loadHymn();
  }, [hymnId]);

  if (loading) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <div className="text-center">
          <p className="text-muted-foreground">Carregando hino...</p>
        </div>
      </div>
    );
  }

  if (error || !hymn) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <div className="text-center">
          <p className="text-muted-foreground mb-4">{error || 'Hino não encontrado'}</p>
          <Button onClick={onBack}>Voltar</Button>
        </div>
      </div>
    );
  }

  const getCategoryLabel = (category: string) => {
    switch (category) {
      case 'hinario': return 'Hinário';
      case 'canticos': return 'Cânticos';
      case 'suplementar': return 'Suplementar';
      case 'novos': return 'Novos';
      default: return category;
    }
  };

  const getCategoryBadgeColor = (category: string) => {
    switch (category) {
      case 'hinario': return 'bg-blue-100 text-blue-900 dark:bg-blue-900/30 dark:text-blue-300';
      case 'canticos': return 'bg-green-100 text-green-900 dark:bg-green-900/30 dark:text-green-300';
      case 'suplementar': return 'bg-purple-100 text-purple-900 dark:bg-purple-900/30 dark:text-purple-300';
      case 'novos': return 'bg-orange-100 text-orange-900 dark:bg-orange-900/30 dark:text-orange-300';
      default: return 'bg-gray-100 text-gray-900 dark:bg-gray-800 dark:text-gray-300';
    }
  };

  // Gera o texto formatado para Holyrics
  const generateHolyricsText = (hymn: Hymn): string => {
    let text = `#${hymn.number} - ${hymn.title}\n`;
    text += `${hymn.hymnBook}\n\n`;

    hymn.verses.forEach((verse, index) => {
      text += `[${verse.type}]\n`;
      verse.lines.forEach(line => {
        text += `${line}\n`;
      });
      if (index < hymn.verses.length - 1) {
        text += '\n';
      }
    });

    if (hymn.key || hymn.bpm) {
      text += '\n---\n';
      if (hymn.key) text += `Tom: ${hymn.key}`;
      if (hymn.key && hymn.bpm) text += ' | ';
      if (hymn.bpm) text += `BPM: ${hymn.bpm}`;
    }

    return text;
  };

  // Gera o texto sem marcadores (para WhatsApp)
  const generatePlainText = (hymn: Hymn): string => {
    let text = `${hymn.title}\n\n`;

    hymn.verses.forEach((verse, index) => {
      verse.lines.forEach(line => {
        text += `${line}\n`;
      });
      if (index < hymn.verses.length - 1) {
        text += '\n';
      }
    });

    return text;
  };

  const handleCopyPlain = async () => {
    const plainText = generatePlainText(hymn);
    try {
      await navigator.clipboard.writeText(plainText);
      setCopiedPlain(true);
      setTimeout(() => setCopiedPlain(false), 2000);
    } catch (err) {
      console.error('Erro ao copiar:', err);
    }
  };

  const handleDownloadHolyrics = () => {
    const holyricsText = generateHolyricsText(hymn);
    const blob = new Blob([holyricsText], { type: 'text/plain;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `hino-${hymn.number}-${hymn.title.toLowerCase().replace(/\s+/g, '-')}.txt`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  };

  return (
    <div className="min-h-screen bg-background">
      <div className="max-w-4xl mx-auto px-4 py-8">
        {/* Header */}
        <div className="mb-6">
          <div className="flex items-center justify-between mb-4">
            <Button
              variant="ghost"
              onClick={onBack}
              className="-ml-2"
            >
              <ArrowLeft className="w-4 h-4 mr-2" />
              Voltar
            </Button>
            <ThemeToggle />
          </div>

          <div className="flex items-start gap-3 mb-2 flex-wrap">
            <h1 className="text-foreground">
              #{hymn.number} – {hymn.title}
            </h1>
            <Badge className={getCategoryBadgeColor(hymn.category)}>
              {getCategoryLabel(hymn.category)}
            </Badge>
          </div>
          <p className="text-muted-foreground">{hymn.hymnBook}</p>
          
          {(hymn.key || hymn.bpm) && (
            <div className="flex gap-4 mt-3 text-muted-foreground text-sm">
              {hymn.key && <span>Tom: <strong className="text-foreground">{hymn.key}</strong></span>}
              {hymn.bpm && <span>BPM: <strong className="text-foreground">{hymn.bpm}</strong></span>}
            </div>
          )}
        </div>

        {/* Lyrics */}
        <div className="bg-card rounded-lg border border-border shadow-sm p-8 mb-6">
          <div className="space-y-6">
            {hymn.verses.map((verse, index) => (
              <div key={index}>
                <div className="text-primary mb-2">
                  [{verse.type}]
                </div>
                <div className="space-y-1">
                  {verse.lines.map((line, lineIndex) => (
                    <div key={lineIndex} className="text-foreground">
                      {line}
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Actions */}
        <div className="border-t border-border pt-6">
          <div className="flex flex-wrap gap-3">
            <Button
              onClick={handleCopyPlain}
              variant="outline"
              className="flex-1 min-w-[200px]"
            >
              {copiedPlain ? (
                <>
                  <Check className="w-4 h-4 mr-2" />
                  Copiado!
                </>
              ) : (
                <>
                  <Copy className="w-4 h-4 mr-2" />
                  Copiar letra pura
                </>
              )}
            </Button>

            <Button
              onClick={handleDownloadHolyrics}
              className="flex-1 min-w-[200px]"
            >
              <Download className="w-4 h-4 mr-2" />
              Baixar .TXT (Holyrics)
            </Button>
          </div>

          <div className="mt-4 p-4 bg-muted/50 border border-border rounded-lg">
            <p className="text-foreground text-sm">
              <strong>📋 Formato Holyrics:</strong> <span className="text-muted-foreground">O arquivo baixado está formatado para importação direta no Holyrics, OpenLP e outros softwares de projeção.</span>
            </p>
          </div>
        </div>

        {/* Preview do formato */}
        <div className="mt-8">
          <details className="bg-muted/50 rounded-lg border border-border p-4">
            <summary className="cursor-pointer text-foreground select-none">
              👁️ Visualizar formato de exportação
            </summary>
            <pre className="mt-4 p-4 bg-card rounded border border-border text-sm overflow-x-auto text-foreground whitespace-pre-wrap">
              {generateHolyricsText(hymn)}
            </pre>
          </details>
        </div>
      </div>
    </div>
  );
}
