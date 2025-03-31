using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Dapper;

[ApiController]
[Route("api/chart")]
[Produces("application/json")]
[Consumes("application/json")]
public class ChartController : ControllerBase
{
    [HttpPost("getData")]
    public IActionResult GetData([FromBody] DatabaseRequest request)
    {
        Console.WriteLine("API'ye istek geldi!");

        try
        {
            string connectionString = $"Host={request.Host};Port={request.Port};Username={request.Username};Password={request.Password};Database={request.Database}";

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var rawData = connection.Query($"SELECT * FROM {request.SelectedObject}").ToList();

            if (!rawData.Any())
                return NotFound("Veri bulunamadı.");

            var labels = new List<string>();
            var values = new List<double>();

            foreach (var row in rawData)
            {
                var dict = (IDictionary<string, object>)row;

                var x = dict.FirstOrDefault(k => k.Key.ToLower().Contains("year") || k.Key.ToLower().Contains("label") || k.Key.ToLower().Contains("category"));
                var y = dict.FirstOrDefault(k => k.Key.ToLower().Contains("amount") || k.Key.ToLower().Contains("value") || k.Key.ToLower().Contains("total"));

                if (x.Value == null || y.Value == null)
                    continue;

                labels.Add(x.Value.ToString());
                if (double.TryParse(y.Value.ToString(), out double yVal))
                    values.Add(yVal);
            }

            return Ok(new
            {
                labels,
                values
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HATA: {ex.Message}");
            return BadRequest(new { message = "Veri alınırken hata oluştu", detail = ex.Message });
        }
    }
}

public class DatabaseRequest
{
    public string Host { get; set; }
    public string Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Database { get; set; }
    public string SelectedObject { get; set; }
}
