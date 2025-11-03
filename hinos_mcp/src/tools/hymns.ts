import { Tool } from '@modelcontextprotocol/sdk/types.js';
import { getApiClient } from '../client/apiClient.js';
import { validateRequired, validatePositiveNumber } from '../utils/validation.js';
import type {
  HymnResponse,
  CreateHymnDto,
  UpdateHymnDto,
} from '../types/api.js';
import type {
  HymnListParams,
  HymnGetParams,
  HymnSearchParams,
  HymnCreateParams,
  HymnUpdateParams,
  HymnDeleteParams,
} from '../types/hymn.js';

/**
 * Tool para listar hinos - GET /api/hymns
 */
export const hymnsListTool: Tool = {
  name: 'hymns_list',
  description: 'Lista todos os hinos cadastrados. Permite filtros opcionais por categoria (hinario, canticos, suplementar, novos) e busca por termo (número, título, hinário ou conteúdo dos versos).',
  inputSchema: {
    type: 'object',
    properties: {
      category: {
        type: 'string',
        enum: ['hinario', 'canticos', 'suplementar', 'novos'],
        description: 'Filtrar por categoria',
      },
      search: {
        type: 'string',
        description: 'Termo de busca (pesquisa em número, título, hinário e letras)',
      },
    },
    required: [],
  },
};

export async function handleHymnsList(args: HymnListParams): Promise<{ content: Array<{ type: string; text: string }> }> {
  try {
    const apiClient = getApiClient();
    const params: Record<string, string> = {};

    if (args.category) {
      params.category = args.category;
    }
    if (args.search) {
      params.search = args.search;
    }

    const hymns = await apiClient.get<HymnResponse[]>('/api/hymns', params);

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify({
            success: true,
            count: hymns.length,
            hymns,
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
            error: error.response?.data?.message || error.message || 'Erro ao listar hinos',
          }, null, 2),
        },
      ],
    };
  }
}

/**
 * Tool para buscar hino por ID - GET /api/hymns/{id}
 */
export const hymnsGetTool: Tool = {
  name: 'hymns_get',
  description: 'Busca um hino específico por ID. Retorna os detalhes completos do hino, incluindo todos os seus versos.',
  inputSchema: {
    type: 'object',
    properties: {
      id: {
        type: 'number',
        description: 'ID do hino',
      },
    },
    required: ['id'],
  },
};

export async function handleHymnsGet(args: HymnGetParams): Promise<{ content: Array<{ type: string; text: string }> }> {
  try {
    validateRequired(args.id, 'ID');
    const id = validatePositiveNumber(args.id, 'ID');

    const apiClient = getApiClient();
    const hymn = await apiClient.get<HymnResponse>(`/api/hymns/${id}`);

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify({
            success: true,
            hymn,
          }, null, 2),
        },
      ],
    };
  } catch (error: any) {
    const statusCode = error.response?.status;
    const isNotFound = statusCode === 404;

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify({
            success: false,
            error: isNotFound ? 'Hino não encontrado' : (error.response?.data?.message || error.message || 'Erro ao buscar hino'),
            statusCode,
          }, null, 2),
        },
      ],
    };
  }
}

/**
 * Tool para buscar hinos por termo - GET /api/hymns/search
 */
export const hymnsSearchTool: Tool = {
  name: 'hymns_search',
  description: 'Realiza uma busca completa por termo nos hinos, pesquisando em número, título, hinário e conteúdo dos versos.',
  inputSchema: {
    type: 'object',
    properties: {
      term: {
        type: 'string',
        description: 'Termo de busca',
      },
    },
    required: ['term'],
  },
};

export async function handleHymnsSearch(args: HymnSearchParams): Promise<{ content: Array<{ type: string; text: string }> }> {
  try {
    validateRequired(args.term, 'Termo de busca');

    const apiClient = getApiClient();
    const hymns = await apiClient.get<HymnResponse[]>('/api/hymns/search', { term: args.term });

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify({
            success: true,
            count: hymns.length,
            term: args.term,
            hymns,
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
            error: error.response?.data?.message || error.message || 'Erro ao buscar hinos',
          }, null, 2),
        },
      ],
    };
  }
}

/**
 * Tool para criar hino - POST /api/hymns
 */
export const hymnsCreateTool: Tool = {
  name: 'hymns_create',
  description: 'Cria um novo hino no sistema. O número do hino deve ser único. Campos obrigatórios: number, title, category, hymnBook. Verses são opcionais.',
  inputSchema: {
    type: 'object',
    properties: {
      number: {
        type: 'string',
        description: 'Número do hino (deve ser único)',
      },
      title: {
        type: 'string',
        description: 'Título do hino',
      },
      category: {
        type: 'string',
        enum: ['hinario', 'canticos', 'suplementar', 'novos'],
        description: 'Categoria do hino',
      },
      hymnBook: {
        type: 'string',
        description: 'Nome do hinário',
      },
      key: {
        type: 'string',
        description: 'Tom musical (opcional)',
      },
      bpm: {
        type: 'number',
        description: 'Batidas por minuto (opcional)',
      },
      verses: {
        type: 'array',
        items: {
          type: 'object',
          properties: {
            type: {
              type: 'string',
              description: 'Tipo do verso (V1, V2, V3, V4, R, Ponte, C)',
            },
            lines: {
              type: 'array',
              items: { type: 'string' },
              description: 'Linhas do verso',
            },
          },
          required: ['type', 'lines'],
        },
        description: 'Lista de versos do hino (opcional)',
      },
    },
    required: ['number', 'title', 'category', 'hymnBook'],
  },
};

export async function handleHymnsCreate(args: HymnCreateParams): Promise<{ content: Array<{ type: string; text: string }> }> {
  try {
    validateRequired(args.number, 'Número');
    validateRequired(args.title, 'Título');
    validateRequired(args.category, 'Categoria');
    validateRequired(args.hymnBook, 'Hinário');

    const apiClient = getApiClient();
    const createDto: CreateHymnDto = {
      number: args.number,
      title: args.title,
      category: args.category,
      hymnBook: args.hymnBook,
      key: args.key,
      bpm: args.bpm,
      verses: args.verses || [],
    };

    const hymn = await apiClient.post<HymnResponse>('/api/hymns', createDto);

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify({
            success: true,
            message: 'Hino criado com sucesso',
            hymn,
          }, null, 2),
        },
      ],
    };
  } catch (error: any) {
    const statusCode = error.response?.status;
    const isConflict = statusCode === 409;

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify({
            success: false,
            error: isConflict
              ? `Já existe um hino com o número ${args.number}`
              : (error.response?.data?.message || error.message || 'Erro ao criar hino'),
            statusCode,
          }, null, 2),
        },
      ],
    };
  }
}

/**
 * Tool para atualizar hino - PUT /api/hymns/{id}
 */
export const hymnsUpdateTool: Tool = {
  name: 'hymns_update',
  description: 'Atualiza os dados de um hino existente. O número do hino deve ser único e não pode estar sendo usado por outro hino. Os versos serão completamente substituídos.',
  inputSchema: {
    type: 'object',
    properties: {
      id: {
        type: 'number',
        description: 'ID do hino a ser atualizado',
      },
      number: {
        type: 'string',
        description: 'Número do hino (deve ser único)',
      },
      title: {
        type: 'string',
        description: 'Título do hino',
      },
      category: {
        type: 'string',
        enum: ['hinario', 'canticos', 'suplementar', 'novos'],
        description: 'Categoria do hino',
      },
      hymnBook: {
        type: 'string',
        description: 'Nome do hinário',
      },
      key: {
        type: 'string',
        description: 'Tom musical (opcional)',
      },
      bpm: {
        type: 'number',
        description: 'Batidas por minuto (opcional)',
      },
      verses: {
        type: 'array',
        items: {
          type: 'object',
          properties: {
            type: {
              type: 'string',
              description: 'Tipo do verso (V1, V2, V3, V4, R, Ponte, C)',
            },
            lines: {
              type: 'array',
              items: { type: 'string' },
              description: 'Linhas do verso',
            },
          },
          required: ['type', 'lines'],
        },
        description: 'Lista de versos do hino (opcional)',
      },
    },
    required: ['id', 'number', 'title', 'category', 'hymnBook'],
  },
};

export async function handleHymnsUpdate(args: HymnUpdateParams): Promise<{ content: Array<{ type: string; text: string }> }> {
  try {
    validateRequired(args.id, 'ID');
    const id = validatePositiveNumber(args.id, 'ID');
    validateRequired(args.number, 'Número');
    validateRequired(args.title, 'Título');
    validateRequired(args.category, 'Categoria');
    validateRequired(args.hymnBook, 'Hinário');

    const apiClient = getApiClient();
    const updateDto: UpdateHymnDto = {
      number: args.number,
      title: args.title,
      category: args.category,
      hymnBook: args.hymnBook,
      key: args.key,
      bpm: args.bpm,
      verses: args.verses || [],
    };

    const hymn = await apiClient.put<HymnResponse>(`/api/hymns/${id}`, updateDto);

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify({
            success: true,
            message: 'Hino atualizado com sucesso',
            hymn,
          }, null, 2),
        },
      ],
    };
  } catch (error: any) {
    const statusCode = error.response?.status;
    const isNotFound = statusCode === 404;
    const isConflict = statusCode === 409;

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify({
            success: false,
            error: isNotFound
              ? 'Hino não encontrado'
              : isConflict
              ? `Já existe um hino com o número ${args.number}`
              : (error.response?.data?.message || error.message || 'Erro ao atualizar hino'),
            statusCode,
          }, null, 2),
        },
      ],
    };
  }
}

/**
 * Tool para deletar hino - DELETE /api/hymns/{id}
 */
export const hymnsDeleteTool: Tool = {
  name: 'hymns_delete',
  description: 'Remove um hino do sistema. Todos os versos associados serão removidos automaticamente (cascade delete).',
  inputSchema: {
    type: 'object',
    properties: {
      id: {
        type: 'number',
        description: 'ID do hino a ser removido',
      },
    },
    required: ['id'],
  },
};

export async function handleHymnsDelete(args: HymnDeleteParams): Promise<{ content: Array<{ type: string; text: string }> }> {
  try {
    validateRequired(args.id, 'ID');
    const id = validatePositiveNumber(args.id, 'ID');

    const apiClient = getApiClient();
    await apiClient.delete(`/api/hymns/${id}`);

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify({
            success: true,
            message: 'Hino removido com sucesso',
            id,
          }, null, 2),
        },
      ],
    };
  } catch (error: any) {
    const statusCode = error.response?.status;
    const isNotFound = statusCode === 404;

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify({
            success: false,
            error: isNotFound ? 'Hino não encontrado' : (error.response?.data?.message || error.message || 'Erro ao deletar hino'),
            statusCode,
          }, null, 2),
        },
      ],
    };
  }
}

