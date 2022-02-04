using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using WebApiTiempo.Settings;

using WebApiTiempo.Models;

namespace WebApiTiempo.Services
{
    public interface IAuthService
    {
        AuthResponse Authenticate(AuthRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }

    public class AuthService : IAuthService
    {
        // users list
        private List<User> _users = new List<User>
        {
            new User {  Id = 1, FirstName = "Test", LastName = "User", 
                        Role = "User",
                        Username = "test", Password = "test" },
            new User {  Id = 2, FirstName = "Ander", LastName = "Redondo", 
                        Role = "Admin",
                        Username = "anderr", Password = "1234" }
        };

        private readonly AppSettings _appSettings;

        public AuthService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public AuthResponse Authenticate(AuthRequest req)
        {
            var user = _users.SingleOrDefault(u => u.Username == req.Username && u.Password == req.Password);

            // 1.- control null
            if (user == null) return null;
            // 2.- control db


            // autenticacion válida -> generamos jwt
            var (token, validTo) = generateJwtToken(user);

            // Devolvemos lo que nos interese
            return new AuthResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Token = token,
                ValidTo = validTo
            };

        }

        public IEnumerable<User> GetAll()
        {
            return _users;
        }

        public User GetById(int id)
        {
            return _users.FirstOrDefault(x => x.Id == id);
        }

        // internos
        private (string token, DateTime validTo) generateJwtToken(User user)
        {
            // generamos un token válido para 7 días
            var dias = 7;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                { 
                    new Claim("id", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                }),
                Expires = DateTime.UtcNow.AddDays(dias),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (token: tokenHandler.WriteToken(token), validTo: token.ValidTo);
        }
    }
}