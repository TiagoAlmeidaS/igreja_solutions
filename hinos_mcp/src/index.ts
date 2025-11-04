#!/usr/bin/env node

/**
 * Entry point do servidor MCP hinos_mcp
 * 
 * Este servidor expõe todas as rotas da API hinos_api como tools do Model Context Protocol,
 * permitindo que LLMs interajam com a API através do protocolo MCP.
 */

import { createServer } from './server.js';
// @ts-ignore - TypeScript incorrectly reports this as unused, but it's used below
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';

async function main() {
  const server = await createServer();
  // Usar StdioServerTransport para comunicação via stdio
  const transport = new StdioServerTransport();
  
  await server.connect(transport);
  
  console.error('Servidor MCP hinos_mcp iniciado');
}

main().catch((error) => {
  console.error('Erro ao iniciar servidor MCP:', error);
  process.exit(1);
});

