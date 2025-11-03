using Microsoft.Data.Sqlite;
using System.Text;

namespace hinos_api.Scripts;

public static class HinarioDbAnalyzer
{
    public static string Analyze(string dbPath)
    {
        if (!File.Exists(dbPath))
        {
            return $"❌ Arquivo não encontrado: {dbPath}";
        }

        var sb = new StringBuilder();
        sb.AppendLine($"📊 Analisando banco de dados: {dbPath}\n");
        sb.AppendLine(new string('=', 80));

        var connectionString = $"Data Source={dbPath};Mode=ReadOnly";
        
        try
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            // Obter todas as tabelas
            var tables = new List<string>();
            var tablesCommand = connection.CreateCommand();
            tablesCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name;";
            
            using var reader = tablesCommand.ExecuteReader();
            while (reader.Read())
            {
                tables.Add(reader.GetString(0));
            }

            sb.AppendLine("\n📋 TABELAS ENCONTRADAS:");
            sb.AppendLine(new string('-', 80));
            foreach (var table in tables)
            {
                sb.AppendLine($"  • {table}");
            }

            if (tables.Count == 0)
            {
                sb.AppendLine("  Nenhuma tabela encontrada.");
                return sb.ToString();
            }

            // Para cada tabela, obter estrutura e amostra de dados
            foreach (var table in tables)
            {
                sb.AppendLine($"\n\n{new string('=', 80)}");
                sb.AppendLine($"📋 TABELA: {table}");
                sb.AppendLine(new string('-', 80));

                // Obter estrutura da tabela
                var schemaCommand = connection.CreateCommand();
                schemaCommand.CommandText = $"PRAGMA table_info({table});";
                
                sb.AppendLine("\n📐 ESTRUTURA DA TABELA:");
                var columns = new List<(string name, string type, bool notNull, bool isPk)>();
                
                using var schemaReader = schemaCommand.ExecuteReader();
                while (schemaReader.Read())
                {
                    var name = schemaReader.GetString(1);
                    var type = schemaReader.GetString(2);
                    var notNull = schemaReader.GetInt32(3) > 0;
                    var pk = schemaReader.GetInt32(5) > 0;
                    
                    columns.Add((name, type, notNull, pk));
                    sb.AppendLine($"  {(pk ? "🔑" : "  ")} {name,-30} {type,-15} {(notNull ? "NOT NULL" : "NULL    ")}");
                }

                // Contar registros
                var countCommand = connection.CreateCommand();
                countCommand.CommandText = $"SELECT COUNT(*) FROM {table};";
                var count = countCommand.ExecuteScalar();
                sb.AppendLine($"\n📊 Total de registros: {count}");

                // Amostra de dados (primeiros 5 registros)
                if (columns.Count > 0)
                {
                    var sampleCommand = connection.CreateCommand();
                    sampleCommand.CommandText = $"SELECT * FROM {table} LIMIT 5;";
                    
                    using var sampleReader = sampleCommand.ExecuteReader();
                    if (sampleReader.HasRows)
                    {
                        sb.AppendLine("\n📝 AMOSTRA DE DADOS (primeiros 5 registros):");
                        int rowNum = 0;
                        while (sampleReader.Read() && rowNum < 5)
                        {
                            sb.AppendLine($"\n  ┌─ Registro #{rowNum + 1} ─────────────────────────────────────────");
                            for (int i = 0; i < sampleReader.FieldCount; i++)
                            {
                                var columnName = columns[i].name;
                                var value = sampleReader.IsDBNull(i) 
                                    ? "[NULL]" 
                                    : sampleReader.GetValue(i)?.ToString() ?? "[NULL]";
                                
                                // Truncar valores muito longos
                                var displayValue = value.Length > 150 
                                    ? value.Substring(0, 150) + "..." 
                                    : value;
                                
                                sb.AppendLine($"  │ {columnName,-25}: {displayValue}");
                            }
                            sb.AppendLine($"  └─────────────────────────────────────────────────────────");
                            rowNum++;
                        }
                    }
                }

                // Verificar índices
                var indexesCommand = connection.CreateCommand();
                indexesCommand.CommandText = $"SELECT name, sql FROM sqlite_master WHERE type='index' AND tbl_name='{table}';";
                sb.AppendLine("\n🔍 ÍNDICES:");
                using var indexesReader = indexesCommand.ExecuteReader();
                var hasIndexes = false;
                while (indexesReader.Read())
                {
                    hasIndexes = true;
                    var idxName = indexesReader.GetString(0);
                    var idxSql = indexesReader.IsDBNull(1) ? "" : indexesReader.GetString(1);
                    sb.AppendLine($"  • {idxName}");
                    if (!string.IsNullOrEmpty(idxSql))
                    {
                        sb.AppendLine($"    {idxSql}");
                    }
                }
                if (!hasIndexes)
                {
                    sb.AppendLine("  Nenhum índice encontrado.");
                }
            }

            connection.Close();
            sb.AppendLine($"\n\n{new string('=', 80)}");
            sb.AppendLine("✅ Análise concluída!");
            
            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"❌ Erro ao analisar banco de dados: {ex.Message}\n\nDetalhes:\n{ex}";
        }
    }
}

