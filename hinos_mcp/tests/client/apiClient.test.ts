import { getApiClient, resetApiClient } from '../../src/client/apiClient.js';
import { jwtManager } from '../../src/auth/jwtManager.js';
import { createFakeLoginResponse, createFakeHymnResponse } from '../helpers/faker.js';

// Create shared mock instance
const mockAxiosInstance = {
  get: jest.fn(),
  post: jest.fn(),
  put: jest.fn(),
  delete: jest.fn(),
  interceptors: {
    request: { use: jest.fn() },
    response: { use: jest.fn() },
  },
};

jest.mock('axios', () => {
  return {
    __esModule: true,
    default: {
      create: jest.fn(() => mockAxiosInstance),
    },
  };
});

describe('ApiClient', () => {
  const baseUrl = 'http://localhost:5000';

  beforeEach(() => {
    jest.clearAllMocks();
    jwtManager.clearToken();
    
    // Reset singleton
    resetApiClient();
  });

  afterEach(() => {
    jwtManager.clearToken();
    resetApiClient();
  });

  describe('GET requests', () => {
    it('deve fazer GET request', async () => {
      const apiClient = getApiClient(baseUrl);
      const mockResponse = { data: [createFakeHymnResponse()] };
      mockAxiosInstance.get.mockResolvedValue(mockResponse);

      const result = await apiClient.get('/api/hymns');

      expect(mockAxiosInstance.get).toHaveBeenCalledWith('/api/hymns', { params: undefined });
      expect(result).toEqual(mockResponse.data);
    });

    it('deve fazer GET request com parâmetros', async () => {
      const apiClient = getApiClient(baseUrl);
      const mockResponse = { data: [createFakeHymnResponse()] };
      mockAxiosInstance.get.mockResolvedValue(mockResponse);

      await apiClient.get('/api/hymns', { category: 'hinario' });

      expect(mockAxiosInstance.get).toHaveBeenCalledWith('/api/hymns', { params: { category: 'hinario' } });
    });
  });

  describe('POST requests', () => {
    it('deve fazer POST request com dados', async () => {
      const apiClient = getApiClient(baseUrl);
      const mockData = { email: 'test@example.com', password: 'password' };
      const mockResponse = { data: createFakeLoginResponse() };
      mockAxiosInstance.post.mockResolvedValue(mockResponse);

      const result = await apiClient.post('/api/auth/login', mockData);

      expect(mockAxiosInstance.post).toHaveBeenCalledWith('/api/auth/login', mockData);
      expect(result).toEqual(mockResponse.data);
    });
  });

  describe('PUT requests', () => {
    it('deve fazer PUT request com dados', async () => {
      const apiClient = getApiClient(baseUrl);
      const mockData = { title: 'Updated Title' };
      const mockResponse = { data: createFakeHymnResponse() };
      mockAxiosInstance.put.mockResolvedValue(mockResponse);

      const result = await apiClient.put('/api/hymns/1', mockData);

      expect(mockAxiosInstance.put).toHaveBeenCalledWith('/api/hymns/1', mockData);
      expect(result).toEqual(mockResponse.data);
    });
  });

  describe('DELETE requests', () => {
    it('deve fazer DELETE request', async () => {
      const apiClient = getApiClient(baseUrl);
      mockAxiosInstance.delete.mockResolvedValue({ data: {} });

      await apiClient.delete('/api/hymns/1');

      expect(mockAxiosInstance.delete).toHaveBeenCalledWith('/api/hymns/1');
    });
  });

  describe('healthCheck', () => {
    it('deve retornar status ok quando API está funcionando', async () => {
      const apiClient = getApiClient(baseUrl);
      mockAxiosInstance.get.mockResolvedValue({ data: {} });

      const result = await apiClient.healthCheck();

      expect(result.status).toBe('ok');
    });

    it('deve retornar status error quando API falha', async () => {
      const apiClient = getApiClient(baseUrl);
      mockAxiosInstance.get.mockRejectedValue(new Error('Network error'));

      const result = await apiClient.healthCheck();

      expect(result.status).toBe('error');
    });
  });
});

