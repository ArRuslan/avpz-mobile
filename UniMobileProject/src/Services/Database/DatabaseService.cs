using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniMobileProject.src.Services.Database.Models;

namespace UniMobileProject.src.Services.Database;

public class DatabaseService
{
    private readonly string _databasePath;

    public DatabaseService()
    {
        Batteries.Init();
        // Set the path for the database file
        _databasePath = Path.Combine(FileSystem.AppDataDirectory, "mydatabase.db");
        
        // Initialize the database
        InitializeDatabase();
    }

    public DatabaseService(string testNameDb)
    {
        Batteries.Init();
        _databasePath = Path.Combine(System.AppContext.BaseDirectory, testNameDb);

        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        // Open a connection to the SQLite database
        using var connection = new SqliteConnection($"Data Source={_databasePath}");
        connection.Open();

        // Create tables if they don't already exist
        using var command = connection.CreateCommand();
        command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Tokens (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    TokenString TEXT NOT NULL,
                    TimeSpan INTEGER
                )";
        command.ExecuteNonQuery();
    }

    public async Task<bool> AddToken(Token token)
    {
        using var connection = new SqliteConnection($"Data Source={_databasePath}");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO Tokens(TokenString, TimeSpan)
VALUES(@tokenString, @timeSpan);";
        command.Parameters.AddWithValue("@tokenString", token.TokenString);
        command.Parameters.AddWithValue("@timeSpan", token.ExpiresAtTimeSpan);
        try
        {
            int result = await command.ExecuteNonQueryAsync();
            if (result <= 0) return false;
            return true;
        }
        catch (DbException)
        {
            return false;
        }
    }

    public async Task<Token> GetToken()
    {
        using var connection = new SqliteConnection($"Data Source={_databasePath}");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM Tokens ORDER BY Id DESC LIMIT 1";
        try
        {
            using var reader = await command.ExecuteReaderAsync();
            Token token = new Token();
            while (await reader.ReadAsync())
            {
                token.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                token.TokenString = reader.GetString(reader.GetOrdinal("TokenString"));
                token.ExpiresAtTimeSpan = reader.GetInt64(reader.GetOrdinal("TimeSpan"));
                break;
            }
            return token;
        }
        catch (DbException)
        {
            return new Token();
        }
    }

    public async Task<bool> UpdateToken(Token token)
    {
        using var connection = new SqliteConnection($"Data Source={_databasePath}");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"UPDATE Tokens
SET TokenString = @tokenString,
TimeSpan = @timeSpan
WHERE
Id = @id";
        command.Parameters.AddWithValue("tokenString", token.TokenString);
        command.Parameters.AddWithValue("timeSpan", token.ExpiresAtTimeSpan);
        command.Parameters.AddWithValue("id", token.Id);
        try
        {
            int result = await command.ExecuteNonQueryAsync();
            if (result <= 0) return false;
            return true;
        }
        catch (DbException)
        {
            return false;
        }
    }
}
