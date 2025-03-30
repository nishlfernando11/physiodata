using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Npgsql;
using System.IO;


using SemParserLibrary;
using Equivital.DongleExtension;
using static SemParserLibrary.SemDevice;
using System.IO.Ports;

using System.Reflection;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ECGDataStream
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager()
        {
 
            string conString = null;

            try
            {
                Config.LoadEnvVariables("..\\..\\.env"); // Change to your .env file

                conString = Environment.GetEnvironmentVariable("DB_CONN_STRING");

                // Connection string to local PostgreSQL
                _connectionString = conString.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading .env file: {ex.Message}");
            }
        }

        public void InsertData(string tableName, object data)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                // Serialize object to JSON and parse it
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                var jsonObject = JObject.Parse(jsonData);

                // Extract keys (column names) and values
                var columns = jsonObject.Properties().Select(p => p.Name).ToList();
                var parameters = columns.Select(c => $"@{c}").ToList();

                // Build the query
                string columnNames = string.Join(", ", columns);
                string paramNames = string.Join(", ", parameters);
                string query = $"INSERT INTO {tableName} ({columnNames}) VALUES ({paramNames})";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    // Bind values to parameters
                    foreach (var property in jsonObject.Properties())
                    {
                        object value;

                        if (property.Value.Type == JTokenType.Array) // Handle arrays/lists
                        {
                            value = property.Value.ToObject<object[]>();
                        }
                        else if (property.Value.Type == JTokenType.Null) // Handle null values
                        {
                            value = DBNull.Value;
                        }
                        else
                        {
                            value = property.Value.ToObject<object>();
                        }

                        cmd.Parameters.AddWithValue($"@{property.Name}", value);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }
       
        public void UpdateData(string tableName, object data, Dictionary<string, object> whereConditions)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                // Serialize object to JSON and parse it
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                var jsonObject = JObject.Parse(jsonData);

                // Extract keys (column names) and values
                var columns = jsonObject.Properties().Select(p => p.Name).ToList();
                var setClauses = columns.Select(c => $"{c} = @{c}").ToList();

                // Construct the WHERE clause dynamically
                var whereClauses = whereConditions.Keys.Select(k => $"{k} = @{k}").ToList();

                // Build the query
                string setClause = string.Join(", ", setClauses);
                string whereClause = string.Join(" AND ", whereClauses);
                string query = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    // Bind values to parameters
                    foreach (var property in jsonObject.Properties())
                    {
                        object value;

                        if (property.Value.Type == JTokenType.Array) // Handle arrays/lists
                        {
                            value = property.Value.ToObject<object[]>();
                        }
                        else if (property.Value.Type == JTokenType.Null) // Handle null values
                        {
                            value = DBNull.Value;
                        }
                        else
                        {
                            value = property.Value.ToObject<object>();
                        }

                        cmd.Parameters.AddWithValue($"@{property.Name}", value);
                    }

                    // Bind WHERE condition values
                    foreach (var condition in whereConditions)
                    {
                        object value = condition.Value ?? DBNull.Value;
                        cmd.Parameters.AddWithValue($"@{condition.Key}", value);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
