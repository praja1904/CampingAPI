using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlatController : ControllerBase
{
    private readonly IConfiguration _config;
    private string Conn => _config.GetConnectionString("DefaultConnection")!;

    public AlatController(IConfiguration config)
    {
        _config = config;
    }

    // GET: api/alat (READ all)
    [HttpGet]
    public IActionResult GetAll()
    {
        using var conn = new NpgsqlConnection(Conn);
        conn.Open();

        var cmd = new NpgsqlCommand("SELECT id_alat, nama_alat, harga, stok, created_at, updated_at FROM alat ORDER BY id_alat", conn);
        var reader = cmd.ExecuteReader();

        var list = new List<object>();
        while (reader.Read())
        {
            list.Add(new
            {
                id = reader["id_alat"],
                nama = reader["nama_alat"],
                harga = reader["harga"],
                stok = reader["stok"],
                created_at = reader["created_at"],
                updated_at = reader["updated_at"]
            });
        }

        return Ok(ApiResponse<object>.Ok(list, "Data alat berhasil diambil"));
    }

    // GET: api/alat/{id} (READ by id)
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        using var conn = new NpgsqlConnection(Conn);
        conn.Open();

        var cmd = new NpgsqlCommand("SELECT id_alat, nama_alat, harga, stok, created_at, updated_at FROM alat WHERE id_alat = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        var reader = cmd.ExecuteReader();
        if (!reader.Read())
        {
            return NotFound(ApiResponse<object>.Error($"Alat dengan ID {id} tidak ditemukan"));
        }

        var data = new
        {
            id = reader["id_alat"],
            nama = reader["nama_alat"],
            harga = reader["harga"],
            stok = reader["stok"],
            created_at = reader["created_at"],
            updated_at = reader["updated_at"]
        };

        return Ok(ApiResponse<object>.Ok(data, "Data alat ditemukan"));
    }

    // POST: api/alat (CREATE)
    [HttpPost]
    public IActionResult Create([FromBody] CreateAlatDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NamaAlat))
            return BadRequest(ApiResponse<object>.Error("Nama alat wajib diisi"));

        using var conn = new NpgsqlConnection(Conn);
        conn.Open();

        var cmd = new NpgsqlCommand(
            "INSERT INTO alat (nama_alat, harga, stok) VALUES (@nama, @harga, @stok) RETURNING id_alat",
            conn);
        cmd.Parameters.AddWithValue("@nama", dto.NamaAlat);
        cmd.Parameters.AddWithValue("@harga", dto.Harga);
        cmd.Parameters.AddWithValue("@stok", dto.Stok);

        var newId = (int)cmd.ExecuteScalar()!;

        return Ok(ApiResponse<object>.Ok(new { id = newId }, "Alat berhasil ditambahkan"));
    }

    // PUT: api/alat/{id} (UPDATE)
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateAlatDto dto)
    {
        using var conn = new NpgsqlConnection(Conn);
        conn.Open();

        // Cek apakah data ada
        var checkCmd = new NpgsqlCommand("SELECT id_alat FROM alat WHERE id_alat = @id", conn);
        checkCmd.Parameters.AddWithValue("@id", id);
        var exists = checkCmd.ExecuteScalar();
        if (exists == null)
            return NotFound(ApiResponse<object>.Error($"Alat dengan ID {id} tidak ditemukan"));

        var cmd = new NpgsqlCommand(
            "UPDATE alat SET nama_alat = @nama, harga = @harga, stok = @stok, updated_at = CURRENT_TIMESTAMP WHERE id_alat = @id",
            conn);
        cmd.Parameters.AddWithValue("@nama", dto.NamaAlat);
        cmd.Parameters.AddWithValue("@harga", dto.Harga);
        cmd.Parameters.AddWithValue("@stok", dto.Stok);
        cmd.Parameters.AddWithValue("@id", id);

        cmd.ExecuteNonQuery();

        return Ok(ApiResponse<object>.Ok(null, "Alat berhasil diupdate"));
    }

    // DELETE: api/alat/{id} (DELETE)
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        using var conn = new NpgsqlConnection(Conn);
        conn.Open();

        var cmd = new NpgsqlCommand("DELETE FROM alat WHERE id_alat = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        int rowsAffected = cmd.ExecuteNonQuery();
        if (rowsAffected == 0)
            return NotFound(ApiResponse<object>.Error($"Alat dengan ID {id} tidak ditemukan"));

        return Ok(ApiResponse<object>.Ok(null, "Alat berhasil dihapus"));
    }
}

// DTOs
public class CreateAlatDto
{
    public string NamaAlat { get; set; } = "";
    public int Harga { get; set; }
    public int Stok { get; set; }
}

public class UpdateAlatDto
{
    public string NamaAlat { get; set; } = "";
    public int Harga { get; set; }
    public int Stok { get; set; }
}