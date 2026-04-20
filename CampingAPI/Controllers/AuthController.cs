    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Npgsql;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            var conn = _config.GetConnectionString("DefaultConnection");

            using var db = new NpgsqlConnection(conn);
            db.Open();

            var cmd = new NpgsqlCommand(
                "SELECT * FROM users WHERE email=@e AND password=@p", db);

            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@p", password);

            var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return Unauthorized("Login gagal");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, reader["nama"].ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }