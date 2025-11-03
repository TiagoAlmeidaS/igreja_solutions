import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import {
  CallToolRequestSchema,
  ListToolsRequestSchema,
} from '@modelcontextprotocol/sdk/types.js';

import { getApiClient } from './client/apiClient.js';

// Import tools
import { authLoginTool, handleAuthLogin } from './tools/auth.js';
import { healthCheckTool, handleHealthCheck } from './tools/health.js';
import {
  hymnsListTool,
  hymnsGetTool,
  hymnsSearchTool,
  hymnsCreateTool,
  hymnsUpdateTool,
  hymnsDeleteTool,
  handleHymnsList,
  handleHymnsGet,
  handleHymnsSearch,
  handleHymnsCreate,
  handleHymnsUpdate,
  handleHymnsDelete,
} from './tools/hymns.js';

/**
 * Configura e inicia o servidor MCP
 */
export async function createServer() {
  // Carregar variáveis de ambiente
  if (typeof process !== 'undefined' && process.env) {
    const { config } = await import('dotenv');
    config();
  }

  // Inicializar API client
  const apiBaseUrl = process.env.API_BASE_URL || 'http://localhost:5000';
  getApiClient(apiBaseUrl);

  // Criar servidor MCP
  const server = new Server(
    {
      name: process.env.MCP_SERVER_NAME || 'hinos_mcp',
      version: process.env.MCP_SERVER_VERSION || '1.0.0',
    },
    {
      capabilities: {
        tools: {},
      },
    }
  );

  // Registrar handler para listar tools
  server.setRequestHandler(ListToolsRequestSchema, async () => {
    return {
      tools: [
        authLoginTool,
        healthCheckTool,
        hymnsListTool,
        hymnsGetTool,
        hymnsSearchTool,
        hymnsCreateTool,
        hymnsUpdateTool,
        hymnsDeleteTool,
      ],
    };
  });

  // Registrar handler para chamar tools
  server.setRequestHandler(CallToolRequestSchema, async (request) => {
    const { name, arguments: args } = request.params;

    try {
      switch (name) {
        case 'auth_login':
          return await handleAuthLogin(args as any);

        case 'health_check':
          return await handleHealthCheck();

        case 'hymns_list':
          return await handleHymnsList(args as any);

        case 'hymns_get':
          return await handleHymnsGet(args as any);

        case 'hymns_search':
          return await handleHymnsSearch(args as any);

        case 'hymns_create':
          return await handleHymnsCreate(args as any);

        case 'hymns_update':
          return await handleHymnsUpdate(args as any);

        case 'hymns_delete':
          return await handleHymnsDelete(args as any);

        default:
          throw new Error(`Tool desconhecida: ${name}`);
      }
    } catch (error: any) {
      return {
        content: [
          {
            type: 'text',
            text: JSON.stringify({
              success: false,
              error: error.message || 'Erro ao executar tool',
            }, null, 2),
          },
        ],
        isError: true,
      };
    }
  });

  return server;
}

