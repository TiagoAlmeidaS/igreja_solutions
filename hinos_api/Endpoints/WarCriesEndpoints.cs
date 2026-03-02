using Microsoft.AspNetCore.Mvc;
using hinos_api.DTOs;
using hinos_api.Services;

namespace hinos_api.Endpoints;

public static class WarCriesEndpoints
{
    public static WebApplication MapWarCriesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/warcries").WithTags("WarCries");

        group.MapGet("/", async ([FromServices] WarCryService warCryService, [FromQuery] string? search) =>
        {
            var warCries = await warCryService.GetAllAsync(search);
            return Results.Ok(warCries);
        })
        .WithName("GetWarCries")
        .WithSummary("Lista todos os Gritos de Guerra")
        .WithDescription("Retorna uma lista de todos os Gritos de Guerra cadastrados. Permite filtro opcional por termo de busca.")
        .Produces<List<WarCryListItemDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", async ([FromServices] WarCryService warCryService, int id) =>
        {
            var warCry = await warCryService.GetByIdAsync(id);
            if (warCry == null)
            {
                return Results.NotFound(new { message = "Grito de Guerra não encontrado" });
            }
            return Results.Ok(warCry);
        })
        .WithName("GetWarCryById")
        .WithSummary("Busca um Grito de Guerra por ID")
        .WithDescription("Retorna os detalhes completos de um Grito de Guerra específico.")
        .Produces<WarCryResponseDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/search", async ([FromServices] WarCryService warCryService, [FromQuery] string term) =>
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Results.BadRequest(new { message = "O termo de busca é obrigatório" });
            }
            var warCries = await warCryService.SearchAsync(term);
            return Results.Ok(warCries);
        })
        .WithName("SearchWarCries")
        .WithSummary("Busca Gritos de Guerra por termo")
        .WithDescription("Busca Gritos de Guerra por termo no título, conteúdo ou tema.")
        .Produces<List<WarCryListItemDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/sync", async ([FromServices] WarCrySyncBackgroundService syncService, [FromBody] WarCrySyncRequestDto? request) =>
        {
            if (syncService.IsRunning)
            {
                return Results.Conflict(new { message = "Sincronização já está em andamento" });
            }

            var incremental = request == null || !request.FullSync;
            var status = await syncService.SyncAsync(incremental: incremental);
            
            return Results.Accepted(null, new 
            { 
                message = incremental ? "Sincronização incremental iniciada" : "Sincronização completa iniciada",
                status = status
            });
        })
        .WithName("TriggerWarCrySync")
        .WithSummary("Força sincronização manual (incremental)")
        .WithDescription("Inicia manualmente a sincronização incremental de Gritos de Guerra com o OneDrive. Apenas processa arquivos novos ou alterados.")
        .Produces(StatusCodes.Status202Accepted)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPost("/sync/full", async ([FromServices] WarCrySyncBackgroundService syncService) =>
        {
            if (syncService.IsRunning)
            {
                return Results.Conflict(new { message = "Sincronização já está em andamento" });
            }

            var status = await syncService.SyncAsync(incremental: false);
            
            return Results.Accepted(null, new 
            { 
                message = "Sincronização completa iniciada",
                status = status
            });
        })
        .WithName("TriggerFullWarCrySync")
        .WithSummary("Força sincronização completa")
        .WithDescription("Inicia sincronização completa re-baixando todos os arquivos e verificando órfãos. Útil para correções.")
        .Produces(StatusCodes.Status202Accepted)
        .Produces(StatusCodes.Status409Conflict);

        group.MapGet("/sync/status", ([FromServices] WarCrySyncBackgroundService syncService) =>
        {
            var status = syncService.GetStatus();
            return Results.Ok(status);
        })
        .WithName("GetWarCrySyncStatus")
        .WithSummary("Retorna status da sincronização")
        .WithDescription("Retorna informações sobre a última sincronização realizada.")
        .Produces<WarCrySyncStatusDto>(StatusCodes.Status200OK);

        return app;
    }
}
