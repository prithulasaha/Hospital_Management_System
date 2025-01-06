using E_Commerce.Domain.Entities;
using HMSphere.Application.DTOs;
using HMSphere.Application.Interfaces;
using HMSphere.Application.Mailing;
using HMSphere.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HMSphere.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IUserRoleFactory _userRoleFactory;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMailingService _mailingService;

		public AccountService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IUserRoleFactory userRoleFactory, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IMailingService mailingService)
		{
			_userManager = userManager;
			_configuration = configuration;
			_userRoleFactory = userRoleFactory;
			_roleManager = roleManager;
			_signInManager = signInManager;
			_mailingService = mailingService;
		}

		public async Task<ApplicationUser> GetCurrentUser(string email)
        {
            var currentUser= await _userManager.FindByEmailAsync(email);
            if(currentUser != null)
            {
                return currentUser;
            }
            return null;
		}

		public async Task<AuthDto> RegisterAsync(RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                return new AuthDto { Message = "Email is already Registered!" };

            }
            if (await _userManager.FindByNameAsync(model.Username) != null)
            {
                return new AuthDto { Message = "Username is already Registered!" };

            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                NID=model.NID,
                Gender=model.Gender,
                Address=model.Address,
                PhoneNumber=model.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} , ";
                }
                return new AuthDto { Message = errors };
            }

			if (!await _roleManager.RoleExistsAsync(model.Role))
			{
				var roleResult = await _roleManager.CreateAsync(new IdentityRole(model.Role));
				if (!roleResult.Succeeded)
				{
					return new AuthDto { IsAuthenticated = false,Message = "Error occured, try again later!" };
				}
			}

			var addResult=await _userManager.AddToRoleAsync(user, model.Role);
			if (!addResult.Succeeded)
			{
				return new AuthDto { IsAuthenticated = false, Message = "Error occured, try again later!" };
			}

			await _userRoleFactory.CreateUserEntity(model,user.Id);

            await _signInManager.SignInAsync(user, isPersistent: false);
            //var Token = await CreateToken(user);
            await _mailingService.SendMailAsync(user, "Registeration Completed", "");
            return new AuthDto
            {
                Email = user.Email,
                IsAuthenticated = true,
                Roles = new List<string> { model.Role },
                UserName = user.UserName,
            };
        }
        public async Task<AuthDto> LoginAsync(LoginDto model)
        {
            var authModel = new AuthDto();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect";
                return authModel;
            }


            //var Token = await CreateToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (result.Succeeded)
            {
                authModel.IsAuthenticated = true;
                authModel.Email = user.Email;
                authModel.UserName = user.UserName;
                authModel.Roles = roles.ToList();
            }

            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var RefreshToken = GenerateRefreshToken();
                authModel.RefreshToken = RefreshToken.Token;
                authModel.RefreshTokenExpiration = RefreshToken.ExpiresOn;
                user.RefreshTokens.Add(RefreshToken);
                await _userManager.UpdateAsync(user);
            }
            return authModel;
        }

        private async Task<JwtSecurityToken> CreateToken(ApplicationUser User)
        {
            var claims = new List<Claim>
                      {
                     new Claim(ClaimTypes.Name, User.UserName),
                     new Claim(ClaimTypes.NameIdentifier, User.Id),
                     new Claim(JwtRegisteredClaimNames.Sub, User.UserName),
                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                     new Claim(JwtRegisteredClaimNames.Email, User.Email!),
                     new Claim("uid", User.Id)
                     };
            var roles = await _userManager.GetRolesAsync(User);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            SecurityKey Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            SigningCredentials signingCred = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256Signature);
            var Token = new JwtSecurityToken(
                issuer: _configuration["JWT:issuer"],
                audience: _configuration["JWT:audience"],
                claims: claims,
                signingCredentials: signingCred,
                expires: DateTime.Now.AddDays(1)
                );
            return Token;
        }

        #region JwtToken with TokenHandler
        //    private async Task<string> CreateToken(ApplicationUser User)
        //    {
        //        var tokenHandler=new JwtSecurityTokenHandler();
        //        var tokenDescriptor = new SecurityTokenDescriptor
        //        {
        //            Subject = new ClaimsIdentity(new Claim[]
        //            {
        //                new Claim(ClaimTypes.Name, User.UserName),
        //                new Claim(ClaimTypes.NameIdentifier, User.Id),
        //	new Claim(JwtRegisteredClaimNames.Sub, User.UserName),
        //	new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //}),
        //            Expires = DateTime.Now.AddHours(24),
        //            Issuer = _configuration["issuer"],
        //            Audience = _configuration["audience"],
        //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
        //            , SecurityAlgorithms.HmacSha256Signature)
        //        };

        //        var token = tokenHandler.CreateToken(tokenDescriptor);
        //        return tokenHandler.WriteToken(token);
        //    } 
        #endregion

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }

        public async Task<AuthDto> LogoutAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthDto { IsAuthenticated = false, Message = "User not found." };
            }

            // Mark all refresh tokens as inactive by setting RevokedOn
            var activeTokens = user.RefreshTokens.Where(t => t.IsActive).ToList();
            foreach (var token in activeTokens)
            {
                token.RevokedOn = DateTime.UtcNow; // Setting RevokedOn marks the token as inactive.
            }

            await _userManager.UpdateAsync(user);

            // Sign out the user
            await _signInManager.SignOutAsync();

            return new AuthDto
            {
                IsAuthenticated = false,
                Message = "User has been logged out successfully."
            };
        }

    }
}
