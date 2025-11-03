import axios, { AxiosInstance, AxiosError } from 'axios';
import { jwtManager } from '../auth/jwtManager.js';

/**
 * API Client - Cliente HTTP para comunicação com hinos_api
 */
class ApiClient {
  private client: AxiosInstance;

  constructor(baseUrl: string) {
    this.client = axios.create({
      baseURL: baseUrl,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Interceptor para adicionar token JWT automaticamente
    this.client.interceptors.request.use(
      (config) => {
        const authHeader = jwtManager.getAuthorizationHeader();
        if (authHeader) {
          config.headers.Authorization = authHeader;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Interceptor para tratamento de erros
    this.client.interceptors.response.use(
      (response) => response,
      (error: AxiosError) => {
        // Limpar token se receber 401
        if (error.response?.status === 401) {
          jwtManager.clearToken();
        }
        return Promise.reject(error);
      }
    );
  }

  /**
   * GET request
   */
  async get<T>(url: string, params?: Record<string, any>): Promise<T> {
    const response = await this.client.get<T>(url, { params });
    return response.data;
  }

  /**
   * POST request
   */
  async post<T>(url: string, data?: any): Promise<T> {
    const response = await this.client.post<T>(url, data);
    return response.data;
  }

  /**
   * PUT request
   */
  async put<T>(url: string, data?: any): Promise<T> {
    const response = await this.client.put<T>(url, data);
    return response.data;
  }

  /**
   * DELETE request
   */
  async delete<T>(url: string): Promise<T> {
    const response = await this.client.delete<T>(url);
    return response.data;
  }

  /**
   * Health check
   */
  async healthCheck(): Promise<{ status: string }> {
    try {
      await this.client.get('/health');
      return { status: 'ok' };
    } catch (error) {
      return { status: 'error' };
    }
  }
}

// Export singleton instance factory
let apiClientInstance: ApiClient | null = null;

export function getApiClient(baseUrl?: string): ApiClient {
  if (!apiClientInstance) {
    const url = baseUrl || process.env.API_BASE_URL || 'http://localhost:5000';
    apiClientInstance = new ApiClient(url);
  }
  return apiClientInstance;
}

export function resetApiClient(): void {
  apiClientInstance = null;
}

