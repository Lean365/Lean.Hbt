//===================================================================
// 项目名 : Lean.Hbt 
// 文件名 : HbtJwtHandler.cs 
// 创建者 : Lean365
// 创建时间: 2024-01-16 10:00
// 版本号 : V0.0.1
// 描述    : JWT令牌处理器实现
//===================================================================

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lean.Hbt.Domain.IServices;
using Lean.Hbt.Common.Exceptions;
using Lean.Hbt.Domain.IServices.Admin;

namespace Lean.Hbt.Infrastructure.Authentication
{
    /// <summary>
    /// JWT令牌处理器实现
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-16
    /// </remarks>
    public class HbtJwtHandler : IHbtJwtHandler
    {
        /// <summary>
        /// 配置接口
        /// </summary>
        private readonly IConfiguration _configuration;
        private readonly IHbtLocalizationService _localizationService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration">配置接口</param>
        /// <param name="localizationService">本地化服务</param>
        public HbtJwtHandler(IConfiguration configuration, IHbtLocalizationService localizationService)
        {
            _configuration = configuration;
            _localizationService = localizationService;
        }

        /// <summary>
        /// 创建访问令牌
        /// </summary>
        public async Task<string> CreateAccessToken(long userId, string userName, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = _configuration["HbtJwt:SecretKey"] ?? throw HbtException.ValidationError(await _localizationService.GetLocalizedStringAsync("Jwt.SecretKeyNotConfigured"));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["HbtJwt:Issuer"] ?? throw HbtException.ValidationError(await _localizationService.GetLocalizedStringAsync("Jwt.IssuerNotConfigured")),
                audience: _configuration["HbtJwt:Audience"] ?? throw HbtException.ValidationError(await _localizationService.GetLocalizedStringAsync("Jwt.AudienceNotConfigured")),
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["HbtJwt:ExpiryInMinutes"] ?? "30")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 创建刷新令牌
        /// </summary>
        public async Task<string> CreateRefreshToken(long userId)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("type", "refresh")
            };

            var secretKey = _configuration["HbtJwt:RefreshSecretKey"] ?? throw HbtException.ValidationError(await _localizationService.GetLocalizedStringAsync("Jwt.RefreshSecretKeyNotConfigured"));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["HbtJwt:Issuer"] ?? throw HbtException.ValidationError(await _localizationService.GetLocalizedStringAsync("Jwt.IssuerNotConfigured")),
                audience: _configuration["HbtJwt:Audience"] ?? throw HbtException.ValidationError(await _localizationService.GetLocalizedStringAsync("Jwt.AudienceNotConfigured")),
                claims: claims,
                expires: DateTime.Now.AddDays(Convert.ToDouble(_configuration["HbtJwt:RefreshExpiryInDays"] ?? "7")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 验证访问令牌
        /// </summary>
        public async Task<bool> ValidateAccessToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _configuration["HbtJwt:SecretKey"] ?? throw HbtException.ValidationError(await _localizationService.GetLocalizedStringAsync("Jwt.SecretKeyNotConfigured"));
                var key = Encoding.UTF8.GetBytes(secretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["HbtJwt:Issuer"] ?? throw HbtException.ValidationError(await _localizationService.GetLocalizedStringAsync("Jwt.IssuerNotConfigured")),
                    ValidateAudience = true,
                    ValidAudience = _configuration["HbtJwt:Audience"] ?? throw HbtException.ValidationError(await _localizationService.GetLocalizedStringAsync("Jwt.AudienceNotConfigured")),
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 验证刷新令牌
        /// </summary>
        public async Task<bool> ValidateRefreshToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _configuration["HbtJwt:RefreshSecretKey"] ?? throw HbtException.ValidationError(await _localizationService.GetLocalizedStringAsync("Jwt.RefreshSecretKeyNotConfigured"));
                var key = Encoding.UTF8.GetBytes(secretKey);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["HbtJwt:Issuer"] ?? throw HbtException.ValidationError(await _localizationService.GetLocalizedStringAsync("Jwt.IssuerNotConfigured")),
                    ValidateAudience = true,
                    ValidAudience = _configuration["HbtJwt:Audience"] ?? throw HbtException.ValidationError(await _localizationService.GetLocalizedStringAsync("Jwt.AudienceNotConfigured")),
                    ClockSkew = TimeSpan.Zero
                }, out _);

                // 验证令牌类型
                var tokenType = principal.Claims.FirstOrDefault(c => c.Type == "type")?.Value;
                return tokenType == "refresh";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 从令牌中获取用户ID
        /// </summary>
        public long GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub);
            return long.Parse(userIdClaim.Value);
        }
    }
} 