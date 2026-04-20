using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransaksiController : ControllerBase
{
    private readonly IConfiguration _config;
    private string Conn => _config.GetConnectionString("DefaultConnection")!;

    public TransaksiController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        using var conn = new NpgsqlConnection(Conn);
        conn.Open();

        var cmd = new NpgsqlCommand(@"
            SELECT t.id_transaksi, t.jumlah, t.total_harga, t.created_at, 
                   u.nama as user_nama, a.nama_alat
            FROM transaksi t
            JOIN users u ON t.id_user = u.id_user
            JOIN alat a ON t.id_alat = a.id_alat
            ORDER BY t.id_transaksi", conn);

        var reader = cmd.ExecuteReader();
        var list = new List<object>();
        while (reader.Read())
        {
            list.Add(new
            {
                id = reader["id_transaksi"],
                user = reader["user_nama"],
                alat = reader["nama_alat"],
                jumlah = reader["jumlah"],
                total_harga = reader["total_harga"],
                created_at = reader["created_at"]
            });
        }

        return Ok(ApiResponse<object>.Ok(list, "Data transaksi berhasil diambil"));
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateTransaksiDto dto)
    {
        using var conn = new NpgsqlConnection(Conn);
        conn.Open();

        // Cek stok alat
        var stokCmd = new NpgsqlCommand("SELECT harga, stok FROM alat WHERE id_alat = @id", conn);
        stokCmd.Parameters.AddWithValue("@id", dto.IdAlat);
        var reader = stokCmd.ExecuteReader();

        if (!reader.Read())
            return BadRequest(ApiResponse<object>.Error("Alat tidak ditemukan"));

        int harga = Convert.ToInt32(reader["harga"]);
        int stok = Convert.ToInt32(reader["stok"]);
        reader.Close();

        if (stok < dto.Jumlah)
            return BadRequest(ApiResponse<object>.Error("Stok tidak mencukupi"));

        int totalHarga = harga * dto.Jumlah;

        // Insert transaksi
        var insertCmd = new NpgsqlCommand(
            @"INSERT INTO transaksi (id_user, id_alat, jumlah, total_harga) 
              VALUES (@id_user, @id_alat, @jumlah, @total) RETURNING id_transaksi", conn);
        insertCmd.Parameters.AddWithValue("@id_user", dto.IdUser);
        insertCmd.Parameters.AddWithValue("@id_alat", dto.IdAlat);
        insertCmd.Parameters.AddWithValue("@jumlah", dto.Jumlah);
        insertCmd.Parameters.AddWithValue("@total", totalHarga);

        var newId = (int)insertCmd.ExecuteScalar()!;

        // Update stok alat
        var updateStok = new NpgsqlCommand("UPDATE alat SET stok = stok - @jml WHERE id_alat = @id", conn);
        updateStok.Parameters.AddWithValue("@jml", dto.Jumlah);
        updateStok.Parameters.AddWithValue("@id", dto.IdAlat);
        updateStok.ExecuteNonQuery();

        return Ok(ApiResponse<object>.Ok(new { id_transaksi = newId, total_harga = totalHarga }, "Transaksi berhasil"));
    }
}

public class CreateTransaksiDto
{
    public int IdUser { get; set; }
    public int IdAlat { get; set; }
    public int Jumlah { get; set; }
}