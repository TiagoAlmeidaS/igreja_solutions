import { handleAuthLogin } from '../../src/tools/auth.js';
import { getApiClient } from '../../src/client/apiClient.js';
import { jwtManager } from '../../src/auth/jwtManager.js';
import { createFakeLoginRequest, createFakeLoginResponse } from '../helpers/faker.js';

jest.mock('../../src/client/apiClient.js');

describe('Auth Tool', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    jwtManager.clearToken();
  });

  afterEach(() => {
    jwtManager.clearToken();
  });

  describe('handleAuthLogin', () => {
    it('deve realizar login com sucesso e armazenar token', async () => {
      const loginRequest = createFakeLoginRequest();
      const loginResponse = createFakeLoginResponse();

      const mockApiClient = {
        post: jest.fn().mockResolvedValue(loginResponse),
      };
      (getApiClient as jest.Mock).mockReturnValue(mockApiClient);

      const result = await handleAuthLogin(loginRequest);

      expect(mockApiClient.post).toHaveBeenCalledWith('/api/auth/login', loginRequest);
      expect(jwtManager.getToken()).toBe(loginResponse.token);
      
      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(true);
      expect(content.user).toEqual(loginResponse.user);
      expect(content.tokenStored).toBe(true);
    });

    it('deve retornar erro quando email está faltando', async () => {
      const result = await handleAuthLogin({ password: 'password' });

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toContain('Email');
    });

    it('deve retornar erro quando senha está faltando', async () => {
      const result = await handleAuthLogin({ email: 'test@example.com' });

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toContain('Senha');
    });

    it('deve retornar erro quando email é inválido', async () => {
      const result = await handleAuthLogin({ 
        email: 'invalid-email', 
        password: 'password' 
      });

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toContain('Email inválido');
    });

    it('deve tratar erro de credenciais inválidas', async () => {
      const loginRequest = createFakeLoginRequest();
      const error = {
        response: {
          status: 401,
          data: { message: 'Credenciais inválidas' },
        },
      };

      const mockApiClient = {
        post: jest.fn().mockRejectedValue(error),
      };
      (getApiClient as jest.Mock).mockReturnValue(mockApiClient);

      const result = await handleAuthLogin(loginRequest);

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toBe('Credenciais inválidas');
      expect(content.statusCode).toBe(401);
    });

    it('deve tratar erro genérico da API', async () => {
      const loginRequest = createFakeLoginRequest();
      const error = new Error('Network error');

      const mockApiClient = {
        post: jest.fn().mockRejectedValue(error),
      };
      (getApiClient as jest.Mock).mockReturnValue(mockApiClient);

      const result = await handleAuthLogin(loginRequest);

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toBe('Network error');
    });
  });
});

