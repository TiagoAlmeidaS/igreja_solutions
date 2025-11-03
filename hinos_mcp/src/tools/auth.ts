import { Tool } from '@modelcontextprotocol/sdk/types.js';
import { getApiClient } from '../client/apiClient.js';
import { jwtManager } from '../auth/jwtManager.js';
import { validateEmail, validateRequired } from '../utils/validation.js';
import type { LoginRequest, LoginResponse } from '../types/api.js';

/**
 * Tool de autenticação - POST /api/auth/login
 */
export const authLoginTool: Tool = {
  name: 'auth_login',
  description: 'Realiza autenticação na API usando email e senha. Retorna um token JWT e os dados do usuário autenticado.',
  inputSchema: {
    type: 'object',
    properties: {
      email: {
        type: 'string',
        description: 'Email do usuário para autenticação',
      },
      password: {
        type: 'string',
        description: 'Senha do usuário',
      },
    },
    required: ['email', 'password'],
  },
};

export async function handleAuthLogin(args: {
  email?: string;
  password?: string;
}): Promise<{ content: Array<{ type: string; text: string }> }> {
  try {
    // Validação
    validateRequired(args.email, 'Email');
    validateRequired(args.password, 'Senha');

    if (!validateEmail(args.email!)) {
      throw new Error('Email inválido');
    }

    // Chamada à API
    const apiClient = getApiClient();
    const loginRequest: LoginRequest = {
      email: args.email!,
      password: args.password!,
    };

    const response = await apiClient.post<LoginResponse>('/api/auth/login', loginRequest);

    // Armazenar token
    jwtManager.setToken(response.token);

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify({
            success: true,
            message: 'Login realizado com sucesso',
            user: response.user,
            tokenStored: true,
          }, null, 2),
        },
      ],
    };
  } catch (error: any) {
    const errorMessage = error.response?.data?.message || error.message || 'Erro ao realizar login';
    const statusCode = error.response?.status || 500;

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify({
            success: false,
            error: errorMessage,
            statusCode,
          }, null, 2),
        },
      ],
    };
  }
}

