using Npgsql;

var connString = "Host=localhost;Port=5432;Database=AutoTallerManager;Username=postgres;Password=1182007";
using var conn = new NpgsqlConnection(connString);
conn.Open();

Console.WriteLine("--- DOMINIOS CORREO ---");
using var cmd1 = new NpgsqlCommand("SELECT \"IdDominioCorreo\", \"Dominio\", \"Activo\" FROM \"DominiosCorreo\"", conn);
using var reader1 = cmd1.ExecuteReader();
while (reader1.Read())
{
    Console.WriteLine($"ID: {reader1.GetInt32(0)}, Domain: {reader1.GetString(1)}, Active: {reader1.GetBoolean(2)}");
}
reader1.Close();

Console.WriteLine("--- PERSONAS CORREOS ---");
using var cmd2 = new NpgsqlCommand("SELECT \"IdPersonaCorreo\", \"IdPersona\", \"IdDominioCorreo\", \"UsuarioCorreo\" FROM \"PersonasCorreos\"", conn);
using var reader2 = cmd2.ExecuteReader();
while (reader2.Read())
{
    Console.WriteLine($"ID: {reader2.GetInt32(0)}, PersonId: {reader2.GetInt32(1)}, DomainId: {reader2.GetInt32(2)}, User: {reader2.GetString(3)}");
}
reader2.Close();
