import { Tool } from '@modelcontextprotocol/sdk/types.js';
import { getApiClient } from '../client/apiClient.js';

/**
 * Tool de health check - GET /health
 */
export const healthCheckTool: Tool = {
  name: 'health_check',
  description: 'Verifica o status de saúde da API. Retorna se a API está funcionando corretamente.',
  inputSchema: {
    type: 'object',
    properties: {},
    required: [],
  },
};

export async function handleHealthCheck(): Promise<{ content: Array<{ type: string; text: string }> }> {
  try {
    const apiClient = getApiClient();
    const result = await apiClient.healthCheck();

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify({
            success: true,
            status: result.status,
            message: result.status === 'ok' ? 'API está funcionando corretamente' : 'API apresentou problemas',
          }, null, 2),
        },
      ],
    };
  } catch (error: any) {
    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify({
            success: false,
            status: 'error',
            error: error.message || 'Erro ao verificar saúde da API',
          }, null, 2),
        },
      ],
    };
  }
}

