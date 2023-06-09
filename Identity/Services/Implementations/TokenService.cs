﻿using Identity.Db.Context;
using Identity.Dtos;
using Identity.Helpers;
using Identity.Models;
using Identity.Options;
using Identity.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IdentityContext _db;
        private readonly IdentityOptions _identityOptions;
        public TokenService(IdentityContext db, IOptions<IdentityOptions> options)
        {
            _db = db;
            _identityOptions = options.Value;
        }


        public async Task<Token> GetUserTokenAsync(UserTokenDto userToken)
        {
            userToken.Lang = userToken.Lang.ToLower();

            var user = userToken.User ?? await _db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == userToken.UserId);

            var session = userToken.Session ?? await _db.Sessions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == userToken.SessionId);

            var userRoles = await _db.UserRoles.AsNoTracking().Include(s => s.Role).Where(s => s.UserId == userToken.UserId).Select(s => s.Role).ToListAsync();

            var userRolesIds = userRoles.Select(s => s.Id);

            var claims = new List<System.Security.Claims.Claim>();

            claims.Add(new System.Security.Claims.Claim(ConstantsClaimTypes.UserId, user.Id.ToString()));
            claims.Add(new System.Security.Claims.Claim(ConstantsClaimTypes.Login, user.Login));
            claims.Add(new System.Security.Claims.Claim(ConstantsClaimTypes.SessionId, session.Id.ToString()));
            claims.Add(new System.Security.Claims.Claim(ConstantsClaimTypes.AuthenticationMethod, session.Type));
            claims.Add(new System.Security.Claims.Claim(ConstantsClaimTypes.Language, session.Language));

            foreach (var role in userRoles)
            {
                claims.Add(new System.Security.Claims.Claim(ConstantsClaimTypes.Role, role.Name));
            }

            var claimsFromRole = await _db.RoleClaims.AsNoTracking().Include(s => s.Claim).Where(s => userRolesIds.Contains(s.RoleId)).Select(s => s.Claim).ToListAsync();

            var claimsFromApp = await _db.AppClaims.AsNoTracking().Include(s => s.Claim).Where(s => s.AppId == session.App.Id).Select(s => s.Claim).ToListAsync();

            var claimsForToken = GetUniqClaims(new List<IEnumerable<Identity.Models.Claim>> { claimsFromRole, claimsFromApp });

            foreach (var claim in claimsForToken)
            {
                claims.Add(new System.Security.Claims.Claim(claim.Type, claim.Value));
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);


            var now = DateTime.Now;
            var expiredAt = now.AddMinutes(_identityOptions.Token.LifeTimeInMinutes);
            var jwt = new JwtSecurityToken(
                issuer: _identityOptions.Token.Issuer,
                audience: _identityOptions.Token.Audience,
                notBefore: now,
                claims: claimsIdentity.Claims,
                expires: expiredAt,
                signingCredentials: new SigningCredentials(_identityOptions.Token.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new Token
            {
                JwtToken = encodedJwt,
                ExpiredAt = expiredAt,
                SessionId = session.Id,
                Session = session
            };
        }

        private List<Identity.Models.Claim> GetUniqClaims(IEnumerable<IEnumerable<Identity.Models.Claim>> source)
        {
            var claims = new List<Identity.Models.Claim>();

            foreach (var enumerable in source)
            {
                foreach (var claim in enumerable)
                {
                    if (!claims.Any(s => s.Type == claim.Type && s.Value == claim.Value))
                        claims.Add(claim);
                }
            }

            return claims;
        }
    }
}