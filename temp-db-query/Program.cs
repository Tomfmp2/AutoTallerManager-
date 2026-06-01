using System;
using Npgsql;

class Program
{
    static void Main()
    {
        var connString = "Host=localhost;Port=5432;Database=AutoTallerManager;Username=postgres;Password=1182007";
        using var conn = new NpgsqlConnection(connString);
        conn.Open();
        
        using var cmd = new NpgsqlCommand("SELECT \"EmailUser\", \"EmailDomainId\" FROM \"PersonasCorreos\";", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine($"User: {reader.GetString(0)}, DomainId: {reader.GetInt32(1)}");
        }
    }
}
