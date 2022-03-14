using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Params;
using Domain.Models.Results;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Exceptions;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public LoginController(IUserRepository userRepository, IOptions<JwtSettings> options)
        {
            _userRepository = userRepository;
            _jwtSettings = options.Value;
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<AuthenticationResult> LoginAsync(Credential credential, CancellationToken cancellationToken)
        {
            var isValidCredentials = false;
            User user = null;
            if (!string.IsNullOrWhiteSpace(credential.Email) 
                || !string.IsNullOrWhiteSpace(credential.Password))
            {
                user = await _userRepository.Get()
                    .FirstOrDefaultAsync(u => u.Email == credential.Email, cancellationToken);

                isValidCredentials = (user is not null && user.ConfirmPassword(credential.Password));
            }

            return await AuthenticateAsync(credential.Email, isValidCredentials, user, cancellationToken);
        }

        private async Task<AuthenticationResult> AuthenticateAsync(
            string credential,
            bool isValidCredentials,
            User user,
            CancellationToken cancellationToken)
        {
            if (isValidCredentials && user.IsActive)
            {
                var identity = new ClaimsIdentity
                (
                    new GenericIdentity(credential, "Login"),
                    new []
                    {
                        new Claim(AuthClaimNames.UserId, user.Id.ToString()),
                    }
                );
                
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                var jwtCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var createdAt = DateTime.Now;
                var expiresAt = createdAt + TimeSpan.FromDays(_jwtSettings.TimeToLive);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                {
                    Subject = identity,
                    Expires = expiresAt,
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience,
                    SigningCredentials = jwtCredentials
                });

                var token = handler.WriteToken(securityToken);

                return await Task.FromResult(new AuthenticationResult
                {
                    Authenticated = true,
                    ExpiresAt = expiresAt,
                    User = new LoginResult
                    {
                        Name = user.Fullname,
                        Email = user.Email,
                        AccessToken = token
                    }
                });
            }
            if (isValidCredentials && user.IsActive is false)
            {
                throw new ForbiddenException("This account has been blocked. Please, contact the administrator");
            }

            throw new ForbiddenException("Incorrect credentials");
        }
    }
}