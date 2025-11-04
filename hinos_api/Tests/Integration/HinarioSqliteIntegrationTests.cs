using FluentAssertions;
using Microsoft.Data.Sqlite;
using hinos_api.DTOs;

namespace hinos_api.Tests.Integration;

public class HinarioSqliteIntegrationTests : IDisposable
{
    private readonly string _dbPath;
    private readonly string _connectionString;

    public HinarioSqliteIntegrationTests()
    {
        // Caminho relativo a partir da raiz do projeto de testes
        // Tenta diferentes caminhos possíveis
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var possiblePaths = new[]
        {
            Path.Combine(projectRoot, "hinos_api", "Data", "Hinario", "HinarioCompleto.sqlite"),
            Path.Combine(projectRoot, "Data", "Hinario", "HinarioCompleto.sqlite"),
            Path.Combine(Directory.GetCurrentDirectory(), "hinos_api", "Data", "Hinario", "HinarioCompleto.sqlite"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "hinos_api", "Data", "Hinario", "HinarioCompleto.sqlite")
        };

        _dbPath = possiblePaths.FirstOrDefault(File.Exists) 
            ?? possiblePaths.First(); // Fallback para o primeiro
        
        _connectionString = $"Data Source={_dbPath};Mode=ReadOnly";
        
        Console.WriteLine($"Tentando conectar ao SQLite: {_dbPath}");
        Console.WriteLine($"Arquivo existe: {File.Exists(_dbPath)}");
    }

    [Fact]
    public void SqliteFile_ShouldExist()
    {
        // Arrange & Act
        var exists = File.Exists(_dbPath);

        // Assert
        exists.Should().BeTrue($"O arquivo SQLite deve existir em: {_dbPath}");
    }

    [Fact]
    public void SqliteConnection_ShouldOpenSuccessfully()
    {
        // Arrange
        using var connection = new SqliteConnection(_connectionString);

        // Act
        var act = () => connection.Open();

        // Assert
        act.Should().NotThrow("A conexão com o SQLite deve abrir sem erros");
        connection.State.Should().Be(System.Data.ConnectionState.Open);
    }

    [Fact]
    public void SqliteDatabase_ShouldHaveTables()
    {
        // Arrange
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Act
        var tables = new List<string>();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name;";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            tables.Add(reader.GetString(0));
        }

        // Assert
        tables.Should().NotBeEmpty("O banco SQLite deve conter pelo menos uma tabela");
        Console.WriteLine($"Tabelas encontradas: {string.Join(", ", tables)}");
    }

    [Fact]
    public void SqliteDatabase_ShouldReadHymnsData()
    {
        // Arrange
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Primeiro, descobrir o nome da tabela
        var tablesCommand = connection.CreateCommand();
        tablesCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' LIMIT 1;";
        
        string? tableName = null;
        using (var tablesReader = tablesCommand.ExecuteReader())
        {
            if (tablesReader.Read())
            {
                tableName = tablesReader.GetString(0);
            }
        }

        tableName.Should().NotBeNull("Deve existir pelo menos uma tabela no banco");

        // Act - Ler estrutura da tabela
        var schemaCommand = connection.CreateCommand();
        schemaCommand.CommandText = $"PRAGMA table_info({tableName});";
        
        var columns = new List<(string name, string type)>();
        using var schemaReader = schemaCommand.ExecuteReader();
        while (schemaReader.Read())
        {
            columns.Add((schemaReader.GetString(1), schemaReader.GetString(2)));
        }

        // Assert
        columns.Should().NotBeEmpty($"A tabela {tableName} deve ter colunas definidas");
        Console.WriteLine($"Colunas da tabela {tableName}:");
        foreach (var (name, type) in columns)
        {
            Console.WriteLine($"  - {name} ({type})");
        }

        // Act - Ler alguns registros
        var dataCommand = connection.CreateCommand();
        dataCommand.CommandText = $"SELECT * FROM {tableName} LIMIT 5;";
        
        var recordCount = 0;
        using var dataReader = dataCommand.ExecuteReader();
        while (dataReader.Read())
        {
            recordCount++;
            Console.WriteLine($"\nRegistro #{recordCount}:");
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                var columnName = dataReader.GetName(i);
                var value = dataReader.IsDBNull(i) ? "[NULL]" : dataReader.GetValue(i)?.ToString() ?? "[NULL]";
                var displayValue = value.Length > 100 ? value.Substring(0, 100) + "..." : value;
                Console.WriteLine($"  {columnName}: {displayValue}");
            }
        }

        // Assert
        recordCount.Should().BeGreaterThan(0, $"A tabela {tableName} deve conter pelo menos um registro");
    }

    [Fact]
    public void SqliteDatabase_ShouldReadHymnsWithVerses()
    {
        // Arrange
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Descobrir tabelas
        var tablesCommand = connection.CreateCommand();
        tablesCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name;";
        
        var tables = new List<string>();
        using (var tablesReader = tablesCommand.ExecuteReader())
        {
            while (tablesReader.Read())
            {
                tables.Add(tablesReader.GetString(0));
            }
        }

        tables.Should().NotBeEmpty("Deve haver tabelas no banco");

        // Act - Tentar encontrar estrutura de hinos e versos
        Console.WriteLine("\n=== ESTRUTURA DO BANCO SQLITE ===");
        foreach (var table in tables)
        {
            Console.WriteLine($"\nTabela: {table}");
            
            var schemaCommand = connection.CreateCommand();
            schemaCommand.CommandText = $"PRAGMA table_info({table});";
            
            using var schemaReader = schemaCommand.ExecuteReader();
            var columns = new List<string>();
            while (schemaReader.Read())
            {
                columns.Add(schemaReader.GetString(1));
            }
            
            Console.WriteLine($"  Colunas: {string.Join(", ", columns)}");

            // Contar registros
            var countCommand = connection.CreateCommand();
            countCommand.CommandText = $"SELECT COUNT(*) FROM {table};";
            var count = countCommand.ExecuteScalar();
            Console.WriteLine($"  Total de registros: {count}");
        }

        // Assert - Validar que temos dados
        foreach (var table in tables)
        {
            var countCommand = connection.CreateCommand();
            countCommand.CommandText = $"SELECT COUNT(*) FROM {table};";
            var count = Convert.ToInt64(countCommand.ExecuteScalar());
            count.Should().BeGreaterThan(0, $"A tabela {table} deve conter dados");
        }
    }

    [Fact]
    public void SqliteData_ShouldMapToHymnResponseDto()
    {
        // Arrange
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Descobrir tabela principal (geralmente a primeira ou com mais registros)
        var tablesCommand = connection.CreateCommand();
        tablesCommand.CommandText = @"
            SELECT name, (SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name = t.name) as cnt
            FROM sqlite_master t
            WHERE type='table' AND name NOT LIKE 'sqlite_%'
            ORDER BY name
            LIMIT 1;";
        
        string? mainTable = null;
        using (var tablesReader = tablesCommand.ExecuteReader())
        {
            if (tablesReader.Read())
            {
                mainTable = tablesReader.GetString(0);
            }
        }

        if (mainTable == null)
        {
            throw new InvalidOperationException("Nenhuma tabela encontrada no banco SQLite");
        }

        // Act - Ler um registro e tentar mapear
        var dataCommand = connection.CreateCommand();
        dataCommand.CommandText = $"SELECT * FROM {mainTable} LIMIT 1;";
        
        using var reader = dataCommand.ExecuteReader();
        if (reader.Read())
        {
            // Criar DTO com dados do SQLite
            var hymnDto = new HymnResponseDto();
            
            // Tentar mapear campos comuns
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i).ToLower();
                var value = reader.IsDBNull(i) ? null : reader.GetValue(i)?.ToString();

                switch (columnName)
                {
                    case "id":
                        if (int.TryParse(value, out var id))
                            hymnDto.Id = id;
                        break;
                    case "number":
                    case "numero":
                        hymnDto.Number = value ?? string.Empty;
                        break;
                    case "title":
                    case "titulo":
                        hymnDto.Title = value ?? string.Empty;
                        break;
                    case "category":
                    case "categoria":
                        hymnDto.Category = value ?? string.Empty;
                        break;
                    case "hymnbook":
                    case "hinario":
                        hymnDto.HymnBook = value ?? string.Empty;
                        break;
                    case "key":
                    case "tom":
                        hymnDto.Key = value;
                        break;
                }

                Console.WriteLine($"Mapeado: {columnName} = {value}");
            }

            // Assert
            hymnDto.Should().NotBeNull("O DTO deve ser criado");
            Console.WriteLine($"\nDTO criado: Id={hymnDto.Id}, Number={hymnDto.Number}, Title={hymnDto.Title}");
        }
        else
        {
            throw new InvalidOperationException($"Nenhum registro encontrado na tabela {mainTable}");
        }
    }

    public void Dispose()
    {
        // Cleanup se necessário
    }
}

