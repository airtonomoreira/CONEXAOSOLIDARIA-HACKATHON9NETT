using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        AuthDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] DoadorRequest request)
    {
        // Validar CPF
        if (!IsValidCpf(request.Cpf))
        {
            return BadRequest("CPF inválido");
        }

        // Verificar se email já existe
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest("Email já cadastrado");
        }

        // Criar usuário Identity
        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Senha);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var roleToAssign = string.IsNullOrWhiteSpace(request.Role) ? "Doador" : request.Role;
        if (!await _roleManager.RoleExistsAsync(roleToAssign))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleToAssign));
        }

        // Adicionar role
        await _userManager.AddToRoleAsync(user, roleToAssign);

        // Criar registro de Doador
        var doador = new Doador
        {
            NomeCompleto = request.NomeCompleto,
            Email = request.Email,
            Cpf = request.Cpf,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            Role = roleToAssign
        };

        _context.Doadores.Add(doador);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Usuário cadastrado com sucesso" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized("Usuário não encontrado");
        }

        var result = await _userManager.CheckPasswordAsync(user, request.Senha);
        if (!result)
        {
            return Unauthorized("Senha incorreta");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user.Email ?? string.Empty, roles);

        return Ok(new { token, email = user.Email, roles });
    }

    [Authorize(Roles = "GestorONG")]
    [HttpPost("promote-to-gestor")]
    public async Task<IActionResult> PromoteToGestor([FromBody] PromoteRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return NotFound("Usuário não encontrado");
        }

        var result = await _userManager.AddToRoleAsync(user, "GestorONG");
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        // Atualizar role no Doador
        var doador = await _context.Doadores.FirstOrDefaultAsync(d => d.Email == request.Email);
        if (doador != null)
        {
            doador.Role = "GestorONG";
            await _context.SaveChangesAsync();
        }

        return Ok(new { message = "Usuário promovido a GestorONG com sucesso" });
    }

    private string GenerateJwtToken(string email, IList<string> roles)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? string.Empty);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private bool IsValidCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;
        cpf = new string(cpf.Where(char.IsDigit).ToArray());
        if (cpf.Length != 11)
            return false;

        if (cpf.All(c => c == cpf[0]))
            return false;

        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        string digito = resto.ToString();
        tempCpf = tempCpf + digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto <= 2 ? 0 : 11 - resto;

        digito = digito + resto.ToString();
        return cpf.EndsWith(digito);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class PromoteRequest
{
    public string Email { get; set; } = string.Empty;
}
