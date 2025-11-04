using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hinos_api.Data;
using hinos_api.DTOs;
using hinos_api.Services;

namespace hinos_api.Endpoints;

public static class HymnsEndpoints
{
    public static WebApplication MapHymnsEndpoints(this WebApplication app)
    {
        // GET /api/hymns - Listar todos os hinos (com filtros opcionais)
        app.MapGet("/api/hymns", async ([FromServices] HymnQueryService queryService, [FromQuery] string? category, [FromQuery] string? search) =>
        {
            var hymns = await queryService.GetAllAsync(category, search);
            return Results.Ok(hymns);
        })
        .WithName("GetHymns")
        .WithTags("Hymns")
        .WithSummary("Lista todos os hinos")
        .WithDescription("Retorna uma lista de todos os hinos cadastrados. Permite filtros opcionais por categoria e busca por termo (número, título, hinário ou conteúdo dos versos). Query parameters: category (hinario, canticos, suplementar, novos) e search (termo de busca).")
        .Produces<List<HymnResponseDto>>(StatusCodes.Status200OK);

        // GET /api/hymns/{id} - Buscar hino por ID
        app.MapGet("/api/hymns/{id:int}", async ([FromServices] HymnQueryService queryService, int id) =>
        {
            var hymn = await queryService.GetByIdAsync(id);

            if (hymn == null)
            {
                return Results.NotFound(new { message = "Hino não encontrado" });
            }

            return Results.Ok(hymn);
        })
        .WithName("GetHymnById")
        .WithTags("Hymns")
        .WithSummary("Busca um hino por ID")
        .WithDescription("Retorna os detalhes completos de um hino específico, incluindo todos os seus versos, com base no ID fornecido.")
        .Produces<HymnResponseDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // GET /api/hymns/search?term={term} - Buscar por termo
        app.MapGet("/api/hymns/search", async ([FromServices] HymnQueryService queryService, [FromQuery] string term) =>
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Results.BadRequest(new { message = "O termo de busca é obrigatório" });
            }

            var hymns = await queryService.SearchAsync(term);
            return Results.Ok(hymns);
        })
        .WithName("SearchHymns")
        .WithTags("Hymns")
        .WithSummary("Busca hinos por termo")
        .WithDescription("Realiza uma busca completa por termo nos hinos, pesquisando em número, título, hinário e conteúdo dos versos. Query parameter obrigatório: term (termo de busca).")
        .Produces<List<HymnResponseDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // POST /api/hymns - Criar novo hino
        app.MapPost("/api/hymns", async ([FromServices] HymnsDbContext db, [FromBody] CreateHymnDto dto) =>
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(dto.Number))
                return Results.BadRequest(new { message = "O número do hino é obrigatório" });
            
            if (string.IsNullOrWhiteSpace(dto.Title))
                return Results.BadRequest(new { message = "O título do hino é obrigatório" });

            if (string.IsNullOrWhiteSpace(dto.Category))
                return Results.BadRequest(new { message = "A categoria é obrigatória" });

            // Verificar se já existe um hino com o mesmo número
            var exists = await db.Hymns.AnyAsync(h => h.Number == dto.Number);
            if (exists)
            {
                return Results.Conflict(new { message = $"Já existe um hino com o número {dto.Number}" });
            }

            var hymn = HymnService.MapFromCreateDto(dto);
            db.Hymns.Add(hymn);
            await db.SaveChangesAsync();

            // Atualizar HymnId nos versos após salvar
            foreach (var verse in hymn.Verses)
            {
                verse.HymnId = hymn.Id;
            }
            await db.SaveChangesAsync();

            // Recarregar com versos
            var savedHymn = await db.Hymns
                .Include(h => h.Verses)
                .FirstAsync(h => h.Id == hymn.Id);

            return Results.Created($"/api/hymns/{savedHymn.Id}", HymnService.MapToDto(savedHymn));
        })
        .WithName("CreateHymn")
        .WithTags("Hymns")
        .WithSummary("Cria um novo hino")
        .WithDescription("Cria um novo hino no sistema. O número do hino deve ser único. Campos obrigatórios: Number, Title, Category, HymnBook. Verses são opcionais.")
        .Produces<HymnResponseDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status409Conflict);

        // PUT /api/hymns/{id} - Atualizar hino
        app.MapPut("/api/hymns/{id:int}", async ([FromServices] HymnsDbContext db, int id, [FromBody] UpdateHymnDto dto) =>
        {
            var hymn = await db.Hymns
                .Include(h => h.Verses)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hymn == null)
            {
                return Results.NotFound(new { message = "Hino não encontrado" });
            }

            // Validações básicas
            if (string.IsNullOrWhiteSpace(dto.Number))
                return Results.BadRequest(new { message = "O número do hino é obrigatório" });
            
            if (string.IsNullOrWhiteSpace(dto.Title))
                return Results.BadRequest(new { message = "O título do hino é obrigatório" });

            // Verificar se o número já está sendo usado por outro hino
            if (dto.Number != hymn.Number)
            {
                var exists = await db.Hymns.AnyAsync(h => h.Number == dto.Number && h.Id != id);
                if (exists)
                {
                    return Results.Conflict(new { message = $"Já existe um hino com o número {dto.Number}" });
                }
            }

            HymnService.UpdateFromDto(hymn, dto);
            
            // Atualizar HymnId nos versos
            foreach (var verse in hymn.Verses)
            {
                verse.HymnId = hymn.Id;
            }

            await db.SaveChangesAsync();

            // Recarregar com versos
            var updatedHymn = await db.Hymns
                .Include(h => h.Verses)
                .FirstAsync(h => h.Id == id);

            return Results.Ok(HymnService.MapToDto(updatedHymn));
        })
        .WithName("UpdateHymn")
        .WithTags("Hymns")
        .WithSummary("Atualiza um hino existente")
        .WithDescription("Atualiza os dados de um hino existente. O número do hino deve ser único e não pode estar sendo usado por outro hino. Os versos serão completamente substituídos.")
        .Produces<HymnResponseDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        // DELETE /api/hymns/{id} - Deletar hino
        app.MapDelete("/api/hymns/{id:int}", async ([FromServices] HymnsDbContext db, int id) =>
        {
            var hymn = await db.Hymns
                .Include(h => h.Verses)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hymn == null)
            {
                return Results.NotFound(new { message = "Hino não encontrado" });
            }

            db.Hymns.Remove(hymn);
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("DeleteHymn")
        .WithTags("Hymns")
        .WithSummary("Remove um hino")
        .WithDescription("Remove um hino do sistema. Todos os versos associados serão removidos automaticamente (cascade delete).")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        // GET /api/hymns/{id}/download/plain - Download hino em formato texto plano
        app.MapGet("/api/hymns/{id:int}/download/plain", async (
            [FromServices] HymnQueryService queryService,
            [FromServices] HymnFormatService formatService,
            int id) =>
        {
            var hymn = await queryService.GetByIdAsync(id);

            if (hymn == null)
            {
                return Results.NotFound(new { message = "Hino não encontrado" });
            }

            var plainText = formatService.GeneratePlainText(hymn);
            var fileName = formatService.GenerateFileName(hymn, "plain");

            return Results.File(
                System.Text.Encoding.UTF8.GetBytes(plainText),
                "text/plain;charset=utf-8",
                fileName
            );
        })
        .WithName("DownloadHymnPlain")
        .WithTags("Hymns")
        .WithSummary("Download hino em formato texto plano")
        .WithDescription("Baixa o hino em formato texto simples, sem marcadores de tipo de verso. Ideal para copiar e colar em WhatsApp ou outros aplicativos.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // GET /api/hymns/{id}/download/holyrics - Download hino em formato Holyrics
        app.MapGet("/api/hymns/{id:int}/download/holyrics", async (
            [FromServices] HymnQueryService queryService,
            [FromServices] HymnFormatService formatService,
            int id) =>
        {
            var hymn = await queryService.GetByIdAsync(id);

            if (hymn == null)
            {
                return Results.NotFound(new { message = "Hino não encontrado" });
            }

            var holyricsText = formatService.GenerateHolyricsText(hymn);
            var fileName = formatService.GenerateFileName(hymn, "holyrics");

            return Results.File(
                System.Text.Encoding.UTF8.GetBytes(holyricsText),
                "text/plain;charset=utf-8",
                fileName
            );
        })
        .WithName("DownloadHymnHolyrics")
        .WithTags("Hymns")
        .WithSummary("Download hino em formato Holyrics")
        .WithDescription("Baixa o hino formatado para importação direta no Holyrics, OpenLP e outros softwares de projeção. O arquivo inclui marcadores de tipo de verso [V1], [V2], [R], etc., e metadados como Tom e BPM quando disponíveis.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}

