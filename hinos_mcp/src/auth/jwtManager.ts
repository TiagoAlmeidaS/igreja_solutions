/**
 * JWT Manager - Gerencia tokens JWT para autenticação
 */

class JwtManager {
  private token: string | null = null;

  /**
   * Armazena o token JWT após login bem-sucedido
   */
  setToken(token: string): void {
    this.token = token;
  }

  /**
   * Retorna o token atual, se existir
   */
  getToken(): string | null {
    return this.token;
  }

  /**
   * Verifica se há um token armazenado
   */
  hasToken(): boolean {
    return this.token !== null;
  }

  /**
   * Remove o token (logout)
   */
  clearToken(): void {
    this.token = null;
  }

  /**
   * Retorna o header de autorização formatado, se houver token
   */
  getAuthorizationHeader(): string | null {
    if (!this.token) {
      return null;
    }
    return `Bearer ${this.token}`;
  }
}

// Singleton instance
export const jwtManager = new JwtManager();

