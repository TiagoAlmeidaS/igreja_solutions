import { useState } from 'react';
import { LogIn, Music } from 'lucide-react';
import { Input } from './ui/input';
import { Button } from './ui/button';
import { Label } from './ui/label';
import { useAuth } from '../contexts/AuthContext';
import { ThemeToggle } from './ThemeToggle';

interface LoginPageProps {
  onLoginSuccess: () => void;
  onBack: () => void;
}

export function LoginPage({ onLoginSuccess, onBack }: LoginPageProps) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const success = await login(email, password);
      if (success) {
        onLoginSuccess();
      } else {
        setError('Email ou senha incorretos');
      }
    } catch (err) {
      setError('Erro ao fazer login. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-background flex items-center justify-center px-4">
      <div className="absolute top-4 right-4">
        <ThemeToggle />
      </div>

      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <div className="flex items-center justify-center gap-2 mb-4">
            <Music className="w-10 h-10 text-primary" />
            <h1 className="text-foreground">Hinário Online</h1>
          </div>
          <p className="text-muted-foreground">
            Faça login para acessar o chat de cadastro
          </p>
        </div>

        <div className="bg-card border border-border rounded-lg p-8 shadow-lg">
          <form onSubmit={handleSubmit} className="space-y-5">
            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                type="email"
                placeholder="seu@email.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                autoComplete="email"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="password">Senha</Label>
              <Input
                id="password"
                type="password"
                placeholder="••••••••"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                autoComplete="current-password"
              />
            </div>

            {error && (
              <div className="bg-destructive/10 border border-destructive/50 text-destructive rounded-lg p-3 text-sm">
                {error}
              </div>
            )}

            <Button
              type="submit"
              className="w-full"
              disabled={loading}
            >
              <LogIn className="w-4 h-4 mr-2" />
              {loading ? 'Entrando...' : 'Entrar'}
            </Button>
          </form>

          <div className="mt-6 p-4 bg-muted/50 rounded-lg">
            <p className="text-sm text-muted-foreground mb-2">
              <strong className="text-foreground">Credenciais de teste:</strong>
            </p>
            <p className="text-sm text-muted-foreground">
              Email: <code className="text-foreground">admin@hinario.com</code>
            </p>
            <p className="text-sm text-muted-foreground">
              Senha: <code className="text-foreground">admin123</code>
            </p>
          </div>
        </div>

        <div className="text-center mt-6">
          <Button
            variant="ghost"
            onClick={onBack}
          >
            ← Voltar para busca
          </Button>
        </div>
      </div>
    </div>
  );
}
