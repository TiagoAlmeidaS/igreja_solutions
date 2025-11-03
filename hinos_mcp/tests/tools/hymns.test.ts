import {
  handleHymnsList,
  handleHymnsGet,
  handleHymnsSearch,
  handleHymnsCreate,
  handleHymnsUpdate,
  handleHymnsDelete,
} from '../../src/tools/hymns.js';
import { getApiClient } from '../../src/client/apiClient.js';
import {
  createFakeHymnResponse,
  createFakeHymnList,
  createFakeCreateHymnDto,
} from '../helpers/faker.js';

jest.mock('../../src/client/apiClient.js');

describe('Hymns Tools', () => {
  let mockApiClient: any;

  beforeEach(() => {
    jest.clearAllMocks();
    mockApiClient = {
      get: jest.fn(),
      post: jest.fn(),
      put: jest.fn(),
      delete: jest.fn(),
    };
    (getApiClient as jest.Mock).mockReturnValue(mockApiClient);
  });

  describe('handleHymnsList', () => {
    it('deve listar todos os hinos sem filtros', async () => {
      const hymns = createFakeHymnList(3);
      mockApiClient.get.mockResolvedValue(hymns);

      const result = await handleHymnsList({});

      expect(mockApiClient.get).toHaveBeenCalledWith('/api/hymns', {});
      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(true);
      expect(content.count).toBe(3);
      expect(content.hymns).toEqual(hymns);
    });

    it('deve filtrar por categoria', async () => {
      const hymns = createFakeHymnList(2);
      mockApiClient.get.mockResolvedValue(hymns);

      const result = await handleHymnsList({ category: 'hinario' });

      expect(mockApiClient.get).toHaveBeenCalledWith('/api/hymns', {
        category: 'hinario',
      });
      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(true);
    });

    it('deve buscar por termo', async () => {
      const hymns = createFakeHymnList(1);
      mockApiClient.get.mockResolvedValue(hymns);

      const result = await handleHymnsList({ search: 'graça' });

      expect(mockApiClient.get).toHaveBeenCalledWith('/api/hymns', {
        search: 'graça',
      });
      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(true);
    });

    it('deve tratar erro ao listar', async () => {
      mockApiClient.get.mockRejectedValue(new Error('API Error'));

      const result = await handleHymnsList({});

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toBe('API Error');
    });
  });

  describe('handleHymnsGet', () => {
    it('deve buscar hino por ID', async () => {
      const hymn = createFakeHymnResponse({ id: 1 });
      mockApiClient.get.mockResolvedValue(hymn);

      const result = await handleHymnsGet({ id: 1 });

      expect(mockApiClient.get).toHaveBeenCalledWith('/api/hymns/1');
      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(true);
      expect(content.hymn).toEqual(hymn);
    });

    it('deve retornar erro quando ID é inválido', async () => {
      const result = await handleHymnsGet({ id: -1 as any });

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toContain('deve ser um número positivo');
    });

    it('deve tratar erro 404 quando hino não encontrado', async () => {
      const error = {
        response: {
          status: 404,
          data: { message: 'Hino não encontrado' },
        },
      };
      mockApiClient.get.mockRejectedValue(error);

      const result = await handleHymnsGet({ id: 999 });

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toBe('Hino não encontrado');
      expect(content.statusCode).toBe(404);
    });
  });

  describe('handleHymnsSearch', () => {
    it('deve buscar hinos por termo', async () => {
      const hymns = createFakeHymnList(2);
      mockApiClient.get.mockResolvedValue(hymns);

      const result = await handleHymnsSearch({ term: 'amor' });

      expect(mockApiClient.get).toHaveBeenCalledWith('/api/hymns/search', {
        term: 'amor',
      });
      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(true);
      expect(content.term).toBe('amor');
      expect(content.hymns).toEqual(hymns);
    });

    it('deve retornar erro quando termo não é fornecido', async () => {
      const result = await handleHymnsSearch({} as any);

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toContain('Termo de busca');
    });
  });

  describe('handleHymnsCreate', () => {
    it('deve criar novo hino', async () => {
      const createDto = createFakeCreateHymnDto();
      const createdHymn = createFakeHymnResponse();
      mockApiClient.post.mockResolvedValue(createdHymn);

      const result = await handleHymnsCreate(createDto);

      expect(mockApiClient.post).toHaveBeenCalledWith('/api/hymns', createDto);
      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(true);
      expect(content.message).toBe('Hino criado com sucesso');
      expect(content.hymn).toEqual(createdHymn);
    });

    it('deve retornar erro quando campos obrigatórios faltam', async () => {
      const result = await handleHymnsCreate({} as any);

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toContain('obrigatório');
    });

    it('deve tratar erro 409 quando número já existe', async () => {
      const createDto = createFakeCreateHymnDto();
      const error = {
        response: {
          status: 409,
          data: { message: 'Já existe um hino com o número 101' },
        },
      };
      mockApiClient.post.mockRejectedValue(error);

      const result = await handleHymnsCreate(createDto);

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toContain(createDto.number);
      expect(content.statusCode).toBe(409);
    });
  });

  describe('handleHymnsUpdate', () => {
    it('deve atualizar hino existente', async () => {
      const updateParams = {
        id: 1,
        ...createFakeCreateHymnDto(),
      };
      const updatedHymn = createFakeHymnResponse({ id: 1 });
      mockApiClient.put.mockResolvedValue(updatedHymn);

      const result = await handleHymnsUpdate(updateParams);

      expect(mockApiClient.put).toHaveBeenCalledWith('/api/hymns/1', expect.objectContaining({
        number: updateParams.number,
        title: updateParams.title,
      }));
      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(true);
      expect(content.message).toBe('Hino atualizado com sucesso');
    });

    it('deve tratar erro 404 quando hino não encontrado', async () => {
      const updateParams = {
        id: 999,
        ...createFakeCreateHymnDto(),
      };
      const error = {
        response: {
          status: 404,
          data: { message: 'Hino não encontrado' },
        },
      };
      mockApiClient.put.mockRejectedValue(error);

      const result = await handleHymnsUpdate(updateParams);

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toBe('Hino não encontrado');
      expect(content.statusCode).toBe(404);
    });

    it('deve tratar erro 409 quando número já existe', async () => {
      const updateParams = {
        id: 1,
        ...createFakeCreateHymnDto({ number: '101' }),
      };
      const error = {
        response: {
          status: 409,
          data: { message: 'Já existe um hino com o número 101' },
        },
      };
      mockApiClient.put.mockRejectedValue(error);

      const result = await handleHymnsUpdate(updateParams);

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toContain('101');
      expect(content.statusCode).toBe(409);
    });
  });

  describe('handleHymnsDelete', () => {
    it('deve deletar hino', async () => {
      mockApiClient.delete.mockResolvedValue({});

      const result = await handleHymnsDelete({ id: 1 });

      expect(mockApiClient.delete).toHaveBeenCalledWith('/api/hymns/1');
      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(true);
      expect(content.message).toBe('Hino removido com sucesso');
      expect(content.id).toBe(1);
    });

    it('deve tratar erro 404 quando hino não encontrado', async () => {
      const error = {
        response: {
          status: 404,
          data: { message: 'Hino não encontrado' },
        },
      };
      mockApiClient.delete.mockRejectedValue(error);

      const result = await handleHymnsDelete({ id: 999 });

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toBe('Hino não encontrado');
      expect(content.statusCode).toBe(404);
    });

    it('deve retornar erro quando ID é inválido', async () => {
      const result = await handleHymnsDelete({ id: 'invalid' as any });

      const content = JSON.parse(result.content[0].text);
      expect(content.success).toBe(false);
      expect(content.error).toContain('número');
    });
  });
});

