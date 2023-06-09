﻿using Extensions.DeviceDetector;
using Extensions.Password;
using Google.Authenticator;
using Identity.Db.Context;
using Identity.Dtos;
using Identity.Extensions;
using Identity.Helpers;
using Identity.Models;
using Identity.Options;
using Identity.Services.Interfaces;
using Identity.SessionHandlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Identity.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IdentityContext _db;
        private readonly IdentityOptions _identityOptions;
        private readonly IDetector _detector;
        private readonly IIdentityService _identityService;
        private readonly ILocationService _locationService;
        private readonly ITokenService _tokenService;
        private readonly ISessionManager _sessionManager;
        public AuthService(IdentityContext db, IDetector detector, IIdentityService identityService, ILocationService locationService, ITokenService tokenService, ISessionManager sessionManager, IOptions<IdentityOptions> options)
        {
            _db = db;
            _detector = detector;
            _identityService = identityService;
            _locationService = locationService;
            _tokenService = tokenService;
            _sessionManager = sessionManager;
            _identityOptions = options.Value;
        }

        public async Task<Result<bool>> ChangeLoginAsync(NewLoginDto loginDto)
        {
            var sessionId = _identityService.GetCurrentSessionId();
            var userId = _identityService.GetUserId();
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == userId);
            if (user == null)
                return Result<bool>.NotFound("User not found");

            if (await _db.Users.AsNoTracking().AnyAsync(s => s.Login == loginDto.NewLogin))
                return Result<bool>.Error("Login is busy");

            if (user.MFA)
            {
                var twoFactor = new TwoFactorAuthenticator();

                var result = twoFactor.ValidateTwoFactorPIN(user.MFASecretKey, loginDto.CodeMFA);

                if (!result)
                    return Result<bool>.Error("Code is incorrect");
            }

            var sessionsToClose = await _db.Sessions.AsNoTracking().Where(s => s.IsActive && s.UserId == userId).ToListAsync();

            sessionsToClose.ForEach(s =>
            {
                s.IsActive = false;
                s.Status = SessionStatus.Close;
                s.DeactivatedAt = DateTime.Now;
                s.DeactivatedBySessionId = sessionId;
            });

            var newLoginChange = new LoginChange
            {
                OldLogin = user.Login,
                NewLogin = loginDto.NewLogin,
                UserId = userId
            };

            user.Login = loginDto.NewLogin;

            _db.Users.Update(user);
            _db.Sessions.UpdateRange(sessionsToClose);
            await _db.LoginChanges.AddAsync(newLoginChange);
            await _db.SaveChangesAsync();

            _sessionManager.RemoveSessions(sessionsToClose.MapSessionIds());

            return Result<bool>.SuccessWithData(true);
        }

        public async Task<Result<bool>> ChangePasswordAsync(NewPasswordDto passwordDto)
        {
            if (passwordDto.OldPassword == passwordDto.NewPassword)
                return Result<bool>.Error("Passwords cannot be the same");

            var userId = _identityService.GetUserId();

            var now = DateTime.Now;

            var sessionId = _identityService.GetCurrentSessionId();

            var user = await _db.Users.AsNoTracking().Include(s => s.Passwords).FirstOrDefaultAsync(s => s.Id == userId);

            if (!passwordDto.OldPassword.VerifyPasswordHash(user.PasswordHash))
                return Result<bool>.Error("Password is incorrect");

            if (user.MFA)
            {
                var twoFactor = new TwoFactorAuthenticator();

                var result = twoFactor.ValidateTwoFactorPIN(user.MFASecretKey, passwordDto.CodeMFA);

                if (!result)
                    return Result<bool>.Error("Code is incorrect");
            }

            var currentPassword = user.Passwords.FirstOrDefault(s => s.PasswordHash == user.PasswordHash && s.IsActive);

            currentPassword.DeactivatedAt = now;
            currentPassword.IsActive = false;

            var newPasswordHash = passwordDto.NewPassword.GeneratePasswordHash();

            user.PasswordHash = newPasswordHash;

            var newPassword = new Password
            {
                PasswordHash = newPasswordHash,
                UserId = userId,
                Answer = passwordDto.Answer,
                Question = passwordDto.Question,
                IsActive = true,
            };

            var sessionsToClose = await _db.Sessions.AsNoTracking().Where(s => s.UserId == userId && s.IsActive).ToListAsync();

            sessionsToClose.ForEach(session =>
            {
                session.IsActive = false;
                session.Status = SessionStatus.Close;
                session.DeactivatedAt = now;
                session.DeactivatedBySessionId = sessionId;
            });

            _db.Users.Update(user);
            _db.Sessions.UpdateRange(sessionsToClose);
            await _db.Passwords.AddAsync(newPassword);
            await _db.SaveChangesAsync();

            _sessionManager.RemoveSessions(sessionsToClose.Select(s => s.Id));

            return Result<bool>.SuccessWithData(true);
        }

        public async Task<Result<bool>> ConfirmAccountAsync(string code, int userId)
        {
            var confirmRequest = await _db.Confirms.AsNoTracking()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Code == code && s.UserId == userId && s.Type == ConfirmType.Account);

            if (confirmRequest == null)
                return Result<bool>.NotFound("Code not found");

            if (confirmRequest.IsActivated)
                return Result<bool>.Error("Code already activated");

            var now = DateTime.Now;

            if (!confirmRequest.IsActualyRequest(now))
                return Result<bool>.Error("Code was expired. Please request new");

            confirmRequest.IsActivated = true;
            confirmRequest.ActivetedAt = now;

            var user = confirmRequest.User;

            user.IsConfirmed = true;

            _db.Confirms.Update(confirmRequest);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return Result<bool>.Success();
        }

        public async Task<Result<bool>> DisableMFAAsync(string code)
        {
            var userId = _identityService.GetUserId();

            var userForDisableMFA = await _db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == userId);

            if (userForDisableMFA == null)
                return Result<bool>.NotFound("User not found");

            if (!userForDisableMFA.MFA)
                return Result<bool>.NotFound("MFA already diactivated");

            var twoFactor = new TwoFactorAuthenticator();

            if (!twoFactor.ValidateTwoFactorPIN(userForDisableMFA.MFASecretKey, code))
                return Result<bool>.Error("Code is incorrect");


            userForDisableMFA.MFA = false;
            userForDisableMFA.MFASecretKey = null;

            var activeMFA = await _db.MFAs.FirstOrDefaultAsync(s => s.UserId == userId && s.IsActivated);
            if (activeMFA == null)
                return Result<bool>.Error("Some error, please contact support");

            activeMFA.Diactived = DateTime.Now;
            activeMFA.DiactivedBySessionId = _identityService.GetCurrentSessionId();

            _db.Users.Update(userForDisableMFA);
            _db.MFAs.UpdateRange(activeMFA);
            await _db.SaveChangesAsync();

            return Result<bool>.Success();
        }

        public async Task<Result<MFADto>> EnableMFAAsync(string code = null)
        {
            var userId = _identityService.GetUserId();

            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == userId);

            if (user == null)
                return Result<MFADto>.NotFound("User not found");

            if (code == null)
            {
                var existMFA = await _db.MFAs.FirstOrDefaultAsync(s => s.UserId == userId && !s.IsActivated);
                if (existMFA == null)
                {
                    var secretKey = Guid.NewGuid().ToString("N");
                    var twoFactor = new TwoFactorAuthenticator();
                    var setupInfo = twoFactor.GenerateSetupCode("XMessenger", user.Login, secretKey, false, 3);

                    user.MFASecretKey = secretKey;
                    user.MFA = false;

                    var newMFA = new MFA
                    {
                        UserId = userId,
                        EntryCode = setupInfo.ManualEntryKey,
                        QrCodeBase64 = setupInfo.QrCodeSetupImageUrl,
                        Secret = secretKey,
                        IsActivated = false,
                        Activated = null,
                        ActivatedBySessionId = null
                    };

                    _db.Users.Update(user);
                    await _db.MFAs.AddAsync(newMFA);
                    await _db.SaveChangesAsync();

                    return Result<MFADto>.SuccessWithData(new MFADto
                    {
                        QrCodeImage = setupInfo.QrCodeSetupImageUrl,
                        ManualEntryKey = setupInfo.ManualEntryKey,
                    });
                }
                else
                {
                    return Result<MFADto>.SuccessWithData(new MFADto
                    {
                        QrCodeImage = existMFA.QrCodeBase64,
                        ManualEntryKey = existMFA.EntryCode
                    });
                }
            }
            else
            {
                if (string.IsNullOrEmpty(user.MFASecretKey))
                    return Result<MFADto>.Error("Unable to activate MFA");

                var mfaToActivate = await _db.MFAs.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == userId && !s.IsActivated);

                if (mfaToActivate == null)
                    return Result<MFADto>.Error("Unable to activate MFA");

                if (mfaToActivate.Secret != user.MFASecretKey)
                    return Result<MFADto>.Error("Please write to support as soon as possible");

                var twoFactor = new TwoFactorAuthenticator();

                if (!twoFactor.ValidateTwoFactorPIN(mfaToActivate.Secret, code))
                    return Result<MFADto>.Error("Code is incorrect");

                user.MFA = true;

                mfaToActivate.IsActivated = true;
                mfaToActivate.Activated = DateTime.Now;
                mfaToActivate.ActivatedBySessionId = _identityService.GetCurrentSessionId();
                mfaToActivate.RestoreCodes = Generator.GetRestoreCodes();

                _db.Users.Update(user);
                _db.MFAs.Update(mfaToActivate);
                await _db.SaveChangesAsync();

                return Result<MFADto>.SuccessWithData(new MFADto
                {
                    RestoreCodes = mfaToActivate.RestoreCodes,
                });
            }
        }

        public async Task<Result<JwtTokenDto>> LoginByMFAAsync(LoginMFADto mfaDto)
        {
            var sessionId = Guid.Parse(mfaDto.SessionId);

            var session = await _db.Sessions.AsNoTracking().Include(s => s.User).FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                return Result<JwtTokenDto>.NotFound($"Session with ID: {sessionId} not found");

            var secretKey = session.User.MFASecretKey;

            var twoFactor = new TwoFactorAuthenticator();

            var result = twoFactor.ValidateTwoFactorPIN(secretKey, mfaDto.Code);

            if (!result)
                return Result<JwtTokenDto>.Error("Code is incorrect");

            var jwtToken = await _tokenService.GetUserTokenAsync(new UserTokenDto
            {
                AuthType = AuthType.Password,
                UserId = session.UserId,
                User = session.User,
                Lang = session.Language,
                SessionId = session.Id,
                Session = session
            });

            session.Tokens = new List<Token> { jwtToken };

            await _db.LoginAttempts.AddAsync(new LoginAttempt
            {
                Login = session.User.Login,
                Device = session.Device,
                Location = session.Location,
                IsSuccess = true,
                UserId = session.UserId
            });

            session.Status = SessionStatus.Active;

            _db.Sessions.Update(session);

            await _db.SaveChangesAsync();

            _sessionManager.AddSession(new SessionModel
            {
                Id = session.Id,
                RefreshToken = session.RefreshToken,
                UserId = session.UserId,
                Tokens = new List<TokenModel> { new TokenModel { ExpiredAt = jwtToken.ExpiredAt, Token = jwtToken.JwtToken } }
            });

            return Result<JwtTokenDto>.SuccessWithData(new JwtTokenDto
            {
                ExpiredAt = jwtToken.ExpiredAt,
                RefreshToken = session.RefreshToken,
                Token = jwtToken.JwtToken
            });
        }

        public async Task<Result<JwtTokenDto>> LoginByPasswordAsync(LoginDto loginDto)
        {
            var app = await _db.Apps.AsNoTracking().FirstOrDefaultAsync(s => s.ClientId == loginDto.App.Id && s.ClientSecret == loginDto.App.Secret);

            if (app == null)
                return Result<JwtTokenDto>.NotFound("App not found");

            if (!app.IsActive)
                return Result<JwtTokenDto>.Error("App unactive");

            if (!app.IsActiveByTime())
                return Result<JwtTokenDto>.Error("App is expired");

            var location = await _locationService.GetIpInfoAsync(_identityService.GetIP());

            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Login == loginDto.Login);
            if (user == null)
            {
                await _db.LoginAttempts.AddAsync(new LoginAttempt
                {
                    Login = loginDto.Login,
                    Password = loginDto.Password,
                    Device = loginDto.Device,
                    Location = location,
                    IsSuccess = false
                });
                return Result<JwtTokenDto>.NotFound("Check your credentials");
            }

            if (!user.IsConfirmed)
                return Result<JwtTokenDto>.Error("First approve your account");


            if (user.LockoutEnabled)
            {
                if (user.IsLocked())
                {
                    return Result<JwtTokenDto>.Error($"Your account has been locked up to {user.LockoutEnd.Value.ToString("HH:mm (dd.MM.yyyy)")}");
                }

                if (user.AccessFailedCount == 5)
                {
                    user.AccessFailedCount = 0;
                    user.LockoutEnd = DateTime.Now.AddHours(1);

                    //ToDo: send notify about blocking account

                    _db.Users.Update(user);
                    await _db.SaveChangesAsync();

                    return Result<JwtTokenDto>.Error($"Account locked up to {user.LockoutEnd.Value.ToString("HH:mm (dd.MM.yyyy)")}");
                }
            }

            if (loginDto.Device == null)
                loginDto.Device = _detector.GetClientInfo();

            if (!loginDto.Password.VerifyPasswordHash(user.PasswordHash))
            {
                user.AccessFailedCount++;

                //ToDo: send notify about fail attempt login

                await _db.LoginAttempts.AddAsync(new LoginAttempt
                {
                    Login = loginDto.Login,
                    Password = loginDto.Password,
                    Device = loginDto.Device,
                    Location = location,
                    IsSuccess = false,
                    UserId = user.Id
                });
                _db.Users.Update(user);

                await _db.SaveChangesAsync();
                return Result<JwtTokenDto>.Error("Password is incorrect");
            }

            var appInfo = new AppInfo
            {
                Id = app.Id,
                Name = app.Name,
                Description = app.Description,
                ShortName = app.ShortName,
                Version = loginDto.App.Version,
                Image = app.Image
            };

            var session = new Session
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                App = appInfo,
                Device = loginDto.Device,
                Location = location,
                UserId = user.Id,
                RefreshToken = Guid.NewGuid().ToString("N"),
                Type = AuthScheme.Password,
                ViaMFA = user.MFA,
                Status = SessionStatus.New,
                Language = loginDto.Lang
            };

            user.AccessFailedCount = 0;

            if (user.MFA)
            {
                _db.Users.Update(user);
                await _db.Sessions.AddAsync(session);
                await _db.SaveChangesAsync();

                return new Result<JwtTokenDto>(false, false, false, false, "Need MFA", null, new JwtTokenDto
                {
                    SessionId = session.Id.ToString()
                });
            }

            var jwtToken = await _tokenService.GetUserTokenAsync(new UserTokenDto
            {
                AuthType = AuthType.Password,
                UserId = user.Id,
                User = user,
                Lang = loginDto.Lang,
                SessionId = session.Id,
                Session = session
            });

            session.Status = SessionStatus.Active;

            session.Tokens = new List<Token>();
            session.Tokens.Add(jwtToken);

            //ToDo: create notify about success login

            await _db.LoginAttempts.AddAsync(new LoginAttempt
            {
                Login = loginDto.Login,
                Device = loginDto.Device,
                Location = location,
                IsSuccess = true,
                UserId = user.Id
            });

            _db.Users.Update(user);
            await _db.Sessions.AddAsync(session);
            await _db.SaveChangesAsync();

            _sessionManager.AddSession(new SessionModel
            {
                Id = session.Id,
                RefreshToken = session.RefreshToken,
                UserId = user.Id,
                Tokens = new List<TokenModel> { new TokenModel { ExpiredAt = jwtToken.ExpiredAt, Token = jwtToken.JwtToken } }
            });

            return Result<JwtTokenDto>.SuccessWithData(new JwtTokenDto
            {
                ExpiredAt = jwtToken.ExpiredAt,
                RefreshToken = session.RefreshToken,
                Token = jwtToken.JwtToken
            });
        }

        public async Task<Result<bool>> LogoutAsync()
        {
            var token = _identityService.GetBearerToken();

            if (!_sessionManager.IsActiveSession(token))
                return Result<bool>.Error("Session is already expired");

            var sessionId = _identityService.GetCurrentSessionId();

            var sessionToClose = await _db.Sessions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sessionId);
            if (sessionToClose == null)
                return Result<bool>.NotFound("Session not found");

            var now = DateTime.Now;

            sessionToClose.Status = SessionStatus.Close;
            sessionToClose.IsActive = false;
            sessionToClose.DeactivatedAt = now;
            sessionToClose.DeactivatedBySessionId = sessionId;

            //ToDo: add notify about close session

            _db.Sessions.Update(sessionToClose);
            await _db.SaveChangesAsync();

            _sessionManager.RemoveSession(sessionId);

            return Result<bool>.SuccessWithData(true);
        }

        public async Task<Result<JwtTokenDto>> RefreshTokenAsync(string refreshToken)
        {
            var session = await _db.Sessions.AsNoTracking().Include(s => s.User).FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);
            if (session == null)
                return Result<JwtTokenDto>.NotFound("Invalid refresh token");

            var jwtToken = await _tokenService.GetUserTokenAsync(new UserTokenDto
            {
                AuthType = AuthType.Password,
                UserId = session.UserId,
                User = session.User,
                Lang = session.Language,
                SessionId = session.Id,
                Session = session
            });

            jwtToken.Session = null;

            await _db.Tokens.AddAsync(jwtToken);
            await _db.SaveChangesAsync();

            _sessionManager.AddToken(session.Id, new TokenModel { ExpiredAt = jwtToken.ExpiredAt, Token = jwtToken.JwtToken });

            return Result<JwtTokenDto>.SuccessWithData(new JwtTokenDto
            {
                ExpiredAt = jwtToken.ExpiredAt,
                RefreshToken = session.RefreshToken,
                Token = jwtToken.JwtToken
            });
        }

        public async Task<Result<bool>> RegisterAsync(RegisterDto registerDto)
        {
            var userWithEmail = await _db.Users.AsNoTracking().Select(s => new User { Id = s.Id, Login = s.Login, Name = s.Name }).FirstOrDefaultAsync(s => s.Login == registerDto.Login);
            if (userWithEmail != null)
                return Result<bool>.Error("Login is busy");

            if (registerDto.UserName != null && await _db.Users.AsNoTracking().AnyAsync(s => s.UserName == registerDto.UserName))
                return Result<bool>.Error("UserName is busy");

            var now = DateTime.Now;

            var res = Regex.IsMatch(registerDto.Password, _identityOptions.Account.Password.RegexTemplate);

            if (!res)
                return Result<bool>.Error(_identityOptions.Account.Password.RegexErrorMessage);

            var passwordHash = registerDto.Password.GeneratePasswordHash();

            var newPassword = new Password
            {
                Question = registerDto.Question,
                Answer = registerDto.KeyForPassword,
                PasswordHash = passwordHash,
                IsActive = true
            };

            var role = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(s => s.NameNormalized == DefaultsRoles.User.ToUpper());

            var newUser = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.UserName,
                Login = registerDto.Login,
                PasswordHash = passwordHash,
                Email = registerDto.Login,
                IsConfirmed = _identityOptions.Features.IsAvailableConfirm ? false : true,
                AccessFailedCount = 0,
                LockoutEnabled = true,
                LockoutEnd = null,
                MFA = false,
                MFASecretKey = null,
                Name = $"{registerDto.FirstName} {registerDto.LastName}",
                Confirms = _identityOptions.Features.IsAvailableConfirm ?
                    new List<Confirm>
                    {
                        new Confirm
                        {
                            ActiveFrom = now,
                            ActiveTo = now.AddDays(1),
                            Code = Generator.GetConfirmCode(),
                            Type = ConfirmType.Account,
                            IsActivated = false,
                            ActivetedAt = null
                        }
                    } : null,
                Passwords = new List<Password> { newPassword }
            };

            var newUserRole = new UserRole
            {
                User = newUser,
                RoleId = role.Id,
                IsActive = true,
                ActiveFrom = now,
                ActiveTo = now.AddYears(10),
            };

            await _db.Users.AddAsync(newUser);
            await _db.UserRoles.AddAsync(newUserRole);
            await _db.SaveChangesAsync();

            return Result<bool>.Success();
        }

        public async Task<Result<bool>> RestorePasswordAsync(RestorePasswordDto restorePasswordDto)
        {
            var now = DateTime.Now;

            if (restorePasswordDto.NewPassword.IsNullOrWhiteSpace() && restorePasswordDto.Code.IsNullOrWhiteSpace())
            {
                var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Login == restorePasswordDto.Login);
                if (user == null)
                    return Result<bool>.Error("User not found");

                var newConfirm = new Confirm
                {
                    UserId = user.Id,
                    Type = ConfirmType.RestorePassword,
                    Code = Generator.GetConfirmCode(),
                    ActiveFrom = now,
                    ActiveTo = now.AddDays(1)
                };

                await _db.Confirms.AddAsync(newConfirm);
                await _db.SaveChangesAsync();

                //ToDo: send email on restore page

                return Result<bool>.SuccessWithData(true);
            }
            else
            {
                var confirm = await _db.Confirms.AsNoTracking().Include(s => s.User).ThenInclude(s => s.Passwords).FirstOrDefaultAsync(s => s.Code == restorePasswordDto.Code && s.Type == ConfirmType.RestorePassword);

                if (confirm == null)
                    return Result<bool>.NotFound("Code not found");

                if (!confirm.IsActualyRequest(now))
                    return Result<bool>.Error("Code for change password is expired");

                if (confirm.IsActivated)
                    return Result<bool>.Error("Code is already activated");

                var user = confirm.User;

                var newPasswordHash = restorePasswordDto.NewPassword.GeneratePasswordHash();

                var activePassword = user.Passwords.FirstOrDefault(s => s.PasswordHash == user.PasswordHash && s.IsActive);
                activePassword.IsActive = false;
                activePassword.DeactivatedAt = now;

                var newPassword = new Password
                {
                    PasswordHash = newPasswordHash,
                    IsActive = true,
                    Question = restorePasswordDto.Question,
                    Answer = restorePasswordDto.Answer,
                    UserId = user.Id
                };

                user.PasswordHash = newPasswordHash;

                var sessionsToClose = await _db.Sessions.AsNoTracking().Where(s => s.UserId == user.Id && s.IsActive).ToListAsync();

                sessionsToClose.ForEach(session =>
                {
                    session.IsActive = false;
                    session.Status = SessionStatus.Close;
                    session.DeactivatedAt = now;
                    session.DeactivatedBySessionId = null;
                });

                confirm.IsActivated = true;
                confirm.ActivetedAt = now;

                _db.Confirms.Update(confirm);
                _db.Sessions.UpdateRange(sessionsToClose);
                _db.Users.Update(user);
                _db.Passwords.Update(activePassword);
                await _db.Passwords.AddAsync(newPassword);
                await _db.SaveChangesAsync();

                _sessionManager.RemoveSessions(sessionsToClose.Select(s => s.Id));

                //ToDo: send notification about change password

                return Result<bool>.SuccessWithData(true);
            }
        }

        public async Task<Result<bool>> SendConfirmAsync(int userId)
        {
            var isUserExist = await _db.Users.AsNoTracking().AnyAsync(s => s.Id == userId);
            if (!isUserExist)
                return Result<bool>.Error("User not found");

            var now = DateTime.Now;

            var newConfirm = new Confirm
            {
                ActiveFrom = now,
                ActiveTo = now.AddDays(1),
                Code = Generator.GetConfirmCode(),
                IsActivated = false,
                ActivetedAt = null,
                UserId = userId
            };

            await _db.Confirms.AddAsync(newConfirm);
            await _db.SaveChangesAsync();

            //ToDo: send confirmation on email

            return Result<bool>.Success();
        }
    }
}
