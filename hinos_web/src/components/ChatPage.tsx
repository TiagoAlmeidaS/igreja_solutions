import { useState, useRef, useEffect } from 'react';
import { Send, ArrowLeft, LogOut, Sparkles, Music, Loader2 } from 'lucide-react';
import { Button } from './ui/button';
import { Textarea } from './ui/textarea';
import { useAuth } from '../contexts/AuthContext';
import { ThemeToggle } from './ThemeToggle';
import { ChatMessage } from '../types/chat';
import { ScrollArea } from './ui/scroll-area';
import { Avatar } from './ui/avatar';
import { createHymn, parseLyricsText, CreateHymnRequest } from '../services/api';

interface ChatPageProps {
  onBack: () => void;
}

export function ChatPage({ onBack }: ChatPageProps) {
  const { user, logout } = useAuth();
  const [messages, setMessages] = useState<ChatMessage[]>([
    {
      id: '1',
      role: 'assistant',
      content: 'Olá! Sou a assistente virtual do Hinário Online. Posso ajudá-lo a cadastrar novos hinos. Como funciona:\n\n1. Você me fornece os dados do hino (título, número, categoria e letra)\n2. Eu vou processar e cadastrar o hino diretamente no sistema\n3. O hino ficará disponível imediatamente para consulta\n\nPara começar, me diga: qual hino você gostaria de cadastrar?',
      timestamp: new Date()
    }
  ]);
  const [input, setInput] = useState('');
  const [isTyping, setIsTyping] = useState(false);
  const [isCreatingHymn, setIsCreatingHymn] = useState(false);
  const scrollAreaRef = useRef<HTMLDivElement>(null);
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  // Estado para coletar informações do hino
  const [hymnData, setHymnData] = useState<{
    title?: string;
    number?: string;
    category?: string;
    lyrics?: string;
    step: 'initial' | 'title' | 'number' | 'category' | 'lyrics' | 'confirm';
  }>({
    step: 'initial'
  });

  useEffect(() => {
    // Auto-scroll para última mensagem
    if (scrollAreaRef.current) {
      const scrollElement = scrollAreaRef.current.querySelector('[data-radix-scroll-area-viewport]');
      if (scrollElement) {
        scrollElement.scrollTop = scrollElement.scrollHeight;
      }
    }
  }, [messages, isTyping]);

  const handleLogout = () => {
    logout();
    onBack();
  };

  const generateAIResponse = (userMessage: string): string => {
    const lowerMessage = userMessage.toLowerCase();

    // Máquina de estados para coletar dados do hino
    if (hymnData.step === 'initial') {
      setHymnData({ ...hymnData, step: 'title' });
      return 'Ótimo! Vamos começar o cadastro. Qual é o título do hino?';
    }

    if (hymnData.step === 'title') {
      setHymnData({ ...hymnData, title: userMessage, step: 'number' });
      return `Perfeito! O hino se chama "${userMessage}". Agora me diga o número do hino (ex: 101, S15, N42):`;
    }

    if (hymnData.step === 'number') {
      setHymnData({ ...hymnData, number: userMessage, step: 'category' });
      return `Número ${userMessage} anotado! Qual é a categoria deste hino?\n\nOpções:\n- Hinário\n- Cânticos\n- Suplementar\n- Novos`;
    }

    if (hymnData.step === 'category') {
      let category = 'hinario';
      if (lowerMessage.includes('cânt') || lowerMessage.includes('cant')) category = 'canticos';
      if (lowerMessage.includes('supl')) category = 'suplementar';
      if (lowerMessage.includes('nov')) category = 'novos';

      setHymnData({ ...hymnData, category, step: 'lyrics' });
      return `Categoria registrada! Agora, por favor, envie a letra completa do hino.\n\nDica: Use os marcadores [V1], [V2], [R] (refrão), [Ponte] para estruturar a letra.`;
    }

    if (hymnData.step === 'lyrics') {
      setHymnData({ ...hymnData, lyrics: userMessage, step: 'confirm' });
      return `Excelente! Recebi a letra completa.\n\n📝 **Resumo da Solicitação:**\n- Título: ${hymnData.title}\n- Número: ${hymnData.number}\n- Categoria: ${hymnData.category}\n- Letra: Recebida\n\nGostaria de confirmar o cadastro desta solicitação? (Responda "sim" ou "não")`;
    }

    if (hymnData.step === 'confirm') {
      if (lowerMessage.includes('sim') || lowerMessage.includes('confirmar')) {
        // Criar hino via API será feito no handleSend
        return 'processar_criacao';
      } else {
        setHymnData({ step: 'initial' });
        return 'Cadastro cancelado. Sem problemas! Gostaria de começar um novo cadastro?';
      }
    }

    // Respostas contextuais
    if (lowerMessage.includes('ajuda') || lowerMessage.includes('help')) {
      return 'Posso ajudá-lo a:\n\n1. Cadastrar novos hinos\n2. Verificar status de solicitações\n3. Explicar o processo de aprovação\n\nO que você gostaria de fazer?';
    }

    if (lowerMessage.includes('status') || lowerMessage.includes('solicitação')) {
      return 'Para verificar o status de suas solicitações, você pode acessar o painel de administração ou aguardar a notificação por email quando houver atualização.';
    }

    return 'Desculpe, não entendi. Gostaria de cadastrar um novo hino? Se sim, basta me dizer qual hino você quer cadastrar!';
  };

  const handleSend = async () => {
    if (!input.trim() || isTyping || isCreatingHymn) return;

    const userMessage: ChatMessage = {
      id: Date.now().toString(),
      role: 'user',
      content: input.trim(),
      timestamp: new Date()
    };

    setMessages(prev => [...prev, userMessage]);
    const userInput = input.trim();
    setInput('');
    setIsTyping(true);

    // Simular delay de digitação da IA
    setTimeout(async () => {
      const aiResponse = generateAIResponse(userInput);
      
      // Se a resposta for para processar criação, criar o hino
      if (aiResponse === 'processar_criacao') {
        setIsTyping(false);
        setIsCreatingHymn(true);
        
        try {
          // Parsear letras
          const verses = parseLyricsText(hymnData.lyrics || '');
          
          // Determinar hymnBook baseado na categoria
          const hymnBooks: Record<string, string> = {
            'hinario': 'Hinário Adventista do Sétimo Dia',
            'canticos': 'Cânticos Laicos',
            'suplementar': 'Suplementar Adventista',
            'novos': 'Novos Cânticos'
          };
          
          const hymnRequest: CreateHymnRequest = {
            number: hymnData.number || '',
            title: hymnData.title || '',
            category: (hymnData.category || 'hinario') as CreateHymnRequest['category'],
            hymnBook: hymnBooks[hymnData.category || 'hinario'] || 'Hinário Adventista do Sétimo Dia',
            verses: verses
          };

          const createdHymn = await createHymn(hymnRequest);
          
          setHymnData({ step: 'initial' });
          
          const successMessage: ChatMessage = {
            id: (Date.now() + 1).toString(),
            role: 'assistant',
            content: `✅ **Hino criado com sucesso!**\n\n🎵 Hino #${createdHymn.number} - ${createdHymn.title}\n📋 Categoria: ${hymnData.category}\n\nO hino foi cadastrado e já está disponível no sistema!\n\nGostaria de cadastrar outro hino?`,
            timestamp: new Date()
          };
          
          setMessages(prev => [...prev, successMessage]);
        } catch (error: any) {
          console.error('Erro ao criar hino:', error);
          
          const errorMessage: ChatMessage = {
            id: (Date.now() + 1).toString(),
            role: 'assistant',
            content: `❌ **Erro ao criar hino**\n\n${error.message || 'Ocorreu um erro ao cadastrar o hino. Por favor, tente novamente.'}\n\nGostaria de tentar novamente ou começar um novo cadastro?`,
            timestamp: new Date()
          };
          
          setMessages(prev => [...prev, errorMessage]);
          
          // Resetar para step 'lyrics' para permitir reenvio
          setHymnData({ ...hymnData, step: 'lyrics' });
        } finally {
          setIsCreatingHymn(false);
        }
      } else {
        const assistantMessage: ChatMessage = {
          id: (Date.now() + 1).toString(),
          role: 'assistant',
          content: aiResponse,
          timestamp: new Date()
        };

        setMessages(prev => [...prev, assistantMessage]);
        setIsTyping(false);
      }
    }, 800 + Math.random() * 1200);
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  return (
    <div className="min-h-screen bg-background flex flex-col">
      {/* Header */}
      <div className="border-b border-border bg-card/50 backdrop-blur-sm sticky top-0 z-10">
        <div className="max-w-4xl mx-auto px-4 py-3 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <Button
              variant="ghost"
              size="icon"
              onClick={onBack}
              className="rounded-full"
            >
              <ArrowLeft className="w-5 h-5" />
            </Button>
            <div className="flex items-center gap-2">
              <div className="w-8 h-8 rounded-full bg-gradient-to-br from-primary to-primary/70 flex items-center justify-center">
                <Sparkles className="w-4 h-4 text-primary-foreground" />
              </div>
              <div>
                <h2 className="text-foreground">Assistente IA</h2>
                <p className="text-xs text-muted-foreground">Cadastro de Hinos</p>
              </div>
            </div>
          </div>

          <div className="flex items-center gap-2">
            <span className="text-sm text-muted-foreground hidden sm:inline">
              {user?.name}
            </span>
            <ThemeToggle />
            <Button
              variant="ghost"
              size="icon"
              onClick={handleLogout}
              className="rounded-full"
              title="Sair"
            >
              <LogOut className="w-5 h-5" />
            </Button>
          </div>
        </div>
      </div>

      {/* Chat Messages */}
      <div className="flex-1 overflow-hidden">
        <ScrollArea className="h-[calc(100vh-140px)]" ref={scrollAreaRef}>
          <div className="max-w-4xl mx-auto px-4 py-6 space-y-6">
            {messages.map((message) => (
              <div
                key={message.id}
                className={`flex gap-3 ${
                  message.role === 'user' ? 'justify-end' : 'justify-start'
                }`}
              >
                {message.role === 'assistant' && (
                  <Avatar className="w-8 h-8 shrink-0">
                    <div className="w-full h-full rounded-full bg-gradient-to-br from-primary to-primary/70 flex items-center justify-center">
                      <Music className="w-4 h-4 text-primary-foreground" />
                    </div>
                  </Avatar>
                )}

                <div
                  className={`max-w-[80%] rounded-2xl px-4 py-3 ${
                    message.role === 'user'
                      ? 'bg-primary text-primary-foreground'
                      : 'bg-muted text-foreground'
                  }`}
                >
                  <p className="whitespace-pre-wrap break-words">{message.content}</p>
                  <p
                    className={`text-xs mt-1 ${
                      message.role === 'user'
                        ? 'text-primary-foreground/70'
                        : 'text-muted-foreground'
                    }`}
                  >
                    {message.timestamp.toLocaleTimeString('pt-BR', {
                      hour: '2-digit',
                      minute: '2-digit'
                    })}
                  </p>
                </div>

                {message.role === 'user' && (
                  <Avatar className="w-8 h-8 shrink-0">
                    <div className="w-full h-full rounded-full bg-primary/20 flex items-center justify-center">
                      <span className="text-sm text-primary">
                        {user?.name.charAt(0).toUpperCase()}
                      </span>
                    </div>
                  </Avatar>
                )}
              </div>
            ))}

            {(isTyping || isCreatingHymn) && (
              <div className="flex gap-3 justify-start">
                <Avatar className="w-8 h-8 shrink-0">
                  <div className="w-full h-full rounded-full bg-gradient-to-br from-primary to-primary/70 flex items-center justify-center">
                    {isCreatingHymn ? (
                      <Loader2 className="w-4 h-4 text-primary-foreground animate-spin" />
                    ) : (
                      <Music className="w-4 h-4 text-primary-foreground" />
                    )}
                  </div>
                </Avatar>
                <div className="bg-muted rounded-2xl px-4 py-3">
                  {isCreatingHymn ? (
                    <p className="text-sm text-muted-foreground">Criando hino...</p>
                  ) : (
                    <div className="flex gap-1">
                      <div className="w-2 h-2 rounded-full bg-muted-foreground/50 animate-bounce" style={{ animationDelay: '0ms' }} />
                      <div className="w-2 h-2 rounded-full bg-muted-foreground/50 animate-bounce" style={{ animationDelay: '150ms' }} />
                      <div className="w-2 h-2 rounded-full bg-muted-foreground/50 animate-bounce" style={{ animationDelay: '300ms' }} />
                    </div>
                  )}
                </div>
              </div>
            )}
          </div>
        </ScrollArea>
      </div>

      {/* Input Area */}
      <div className="border-t border-border bg-card/50 backdrop-blur-sm">
        <div className="max-w-4xl mx-auto px-4 py-4">
          <div className="flex gap-2 items-end">
            <Textarea
              ref={textareaRef}
              value={input}
              onChange={(e) => setInput(e.target.value)}
              onKeyDown={handleKeyDown}
              placeholder="Digite sua mensagem..."
              className="resize-none min-h-[60px] max-h-[200px]"
              rows={1}
            />
            <Button
              onClick={handleSend}
              disabled={!input.trim() || isTyping || isCreatingHymn}
              size="icon"
              className="shrink-0 h-[60px] w-[60px] rounded-xl"
            >
              {isCreatingHymn ? (
                <Loader2 className="w-5 h-5 animate-spin" />
              ) : (
                <Send className="w-5 h-5" />
              )}
            </Button>
          </div>
          <p className="text-xs text-muted-foreground mt-2 text-center">
            A IA pode cometer erros. Verifique as informações importantes.
          </p>
        </div>
      </div>
    </div>
  );
}
