import { handleHealthCheck } from '../../src/tools/health.js';
import { getApiClient } from '../../src/client/apiClient.js';

jest.mock('../../src/client/apiClient.js');

describe('Health Tool', () => {
  describe('handleHealthCheck', () => {
    it('deve retornar status ok quando API está funcionando', async () => {
      const mockApiClient = {
        healthCheck: jest.fn().mockResolvedValue({ status: 'ok' }),
      };
      (getApiClient as jest.Mock).mockReturnValue(mockApiClient);

      const result = await handleHealthCheck();

      expect(mockApiClient.healthCheck).toHaveBeenCalled();
      
      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(true);
      expect(content.status).toBe('ok');
      expect(content.message).toBe('API está funcionando corretamente');
    });

    it('deve retornar status error quando API apresenta problemas', async () => {
      const mockApiClient = {
        healthCheck: jest.fn().mockResolvedValue({ status: 'error' }),
      };
      (getApiClient as jest.Mock).mockReturnValue(mockApiClient);

      const result = await handleHealthCheck();

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(true);
      expect(content.status).toBe('error');
      expect(content.message).toBe('API apresentou problemas');
    });

    it('deve tratar erro quando healthCheck lança exceção', async () => {
      const mockApiClient = {
        healthCheck: jest.fn().mockRejectedValue(new Error('Connection failed')),
      };
      (getApiClient as jest.Mock).mockReturnValue(mockApiClient);

      const result = await handleHealthCheck();

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.status).toBe('error');
      expect(content.error).toBe('Connection failed');
    });
  });
});

