using System.Data.SQLite;

using ElementalAdventure.Server.Models;

namespace ElementalAdventure.Server.Storage;

public class SQLiteDatabase : IDatabase {
    private readonly SQLiteConnection _connection;

    public SQLiteDatabase(string connectionString) {
        _connection = new SQLiteConnection(connectionString);
    }

    public void Connect() {
        _connection.Open();

        using SQLiteCommand pragma = new SQLiteCommand("PRAGMA foreign_keys = ON;", _connection);
        pragma.ExecuteNonQuery();
        using SQLiteCommand commandPlayerProfiles = new SQLiteCommand("""
            CREATE TABLE IF NOT EXISTS PlayerProfiles (
                uid INTEGER PRIMARY KEY AUTOINCREMENT);
            """, _connection);
        commandPlayerProfiles.ExecuteNonQuery();
        using SQLiteCommand commandClientTokens = new SQLiteCommand("""
            CREATE TABLE IF NOT EXISTS ClientTokens (
                provider TEXT,
                token TEXT,
                uid INTEGER,

                PRIMARY KEY(provider, token),
                CHECK (provider IN ('guest')),
                CHECK (LENGTH(token) = 36),
                FOREIGN KEY(uid) REFERENCES PlayerProfiles(uid) ON DELETE CASCADE);
            """, _connection);
        commandClientTokens.ExecuteNonQuery();
    }

    public void Disconnect() {
        _connection.Close();
    }

    public PlayerProfile CreatePlayerProfile() {
        using SQLiteCommand command = new SQLiteCommand("INSERT INTO PlayerProfiles DEFAULT VALUES RETURNING uid", _connection);
        return new PlayerProfile((long)command.ExecuteScalar());
    }

    public PlayerProfile? GetPlayerProfile(long uid) {
        using SQLiteCommand command = new SQLiteCommand("SELECT uid FROM PlayerProfiles WHERE uid = @uid", _connection);
        command.Parameters.AddWithValue("@uid", uid);
        using SQLiteDataReader reader = command.ExecuteReader();
        return reader.Read() ? new PlayerProfile(reader.GetInt64(0)) : null;
    }

    public ClientToken CreateClientToken(string provider, string token, long uid) {
        using SQLiteCommand command = new SQLiteCommand("INSERT INTO ClientTokens (provider, token, uid) VALUES (@provider, @token, @uid)", _connection);
        command.Parameters.AddWithValue("@provider", provider);
        command.Parameters.AddWithValue("@token", token);
        command.Parameters.AddWithValue("@uid", uid);
        command.ExecuteNonQuery();
        return new ClientToken(command.Parameters["@provider"].Value.ToString()!, command.Parameters["@token"].Value.ToString()!, uid);
    }

    public ClientToken? GetClientToken(string provider, string token) {
        using SQLiteCommand command = new SQLiteCommand("SELECT provider, token, uid FROM ClientTokens WHERE provider = @provider AND token = @token", _connection);
        command.Parameters.AddWithValue("@provider", provider);
        command.Parameters.AddWithValue("@token", token);
        using SQLiteDataReader reader = command.ExecuteReader();
        return reader.Read() ? new ClientToken(reader.GetString(0), reader.GetString(1), reader.GetInt64(2)) : null;
    }
}