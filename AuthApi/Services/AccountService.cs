using AuthApi.Entities;
using AuthApi.Models;
using Shared.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using AutoMapper;
using Shared;
using AuthApi.Auth.Models;
using System.Security.Principal;
using AuthApi.Data;

namespace AuthApi.Services
{
    public interface IAccountService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        UserResponse Register(CreateUserRequest model);
        //IEnumerable<UserResponse> GetAll();
        //UserResponse GetById(int id);
        //UserResponse Create(CreateUserRequest model);
        //UserResponse Update(int id, UpdateUserRequest model);
        //void Delete(int id);
    }

    public class AccountService : IAccountService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public AccountService(
            DataContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _context.Users.SingleOrDefault(x => x.Email == model.Email);
            var response = _mapper.Map<AuthenticateResponse>(user);

            // validate
            if (user is null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return response;
            }
            //if (account == null ||  !BCrypt.Net.BCrypt.Verify(model.Password, account.PasswordHash))
            //    throw new AppException("Email or password is incorrect");

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtExtensions.SecurityKey));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var expirationTimeStamp = DateTime.Now.AddMinutes(5);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Name, user.Email),
            new Claim("Role",  Enum.GetName(typeof(Role), user.Role))
        };

            var tokenOptions = new JwtSecurityToken(
                issuer: "https://localhost:5002",
                claims: claims,
                expires: expirationTimeStamp,
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            response.JwtToken = tokenString;
            response.ExpiresInSeconds = (int)expirationTimeStamp.Subtract(DateTime.Now).TotalSeconds;
            return response;

        }


        public UserResponse Register(CreateUserRequest model)
        {
            var user = _mapper.Map<User>(model);
            
            // validate
            if (_context.Users.Any(x => x.Email == model.Email))
            {
                throw new AppException("User is Already Exist");
            }

            // first registered account is an admin
            user.Role = (Role)Enum.Parse(typeof(Role), model.Role);
            user.Created = DateTime.UtcNow;

            // hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // save account
            _context.Users.Add(user);
            _context.SaveChanges();

            var response = _mapper.Map<UserResponse>(user);

            return response;
        }

        //public void VerifyEmail(string token)
        //{
        //    var account = _context.Accounts.SingleOrDefault(x => x.VerificationToken == token);

        //    if (account == null)
        //        throw new AppException("Verification failed");

        //    account.Verified = DateTime.UtcNow;
        //    account.VerificationToken = null;

        //    _context.Accounts.Update(account);
        //    _context.SaveChanges();
        //}

        //public TokenResponse ForgotPassword(ForgotPasswordRequest model, string origin)
        //{
        //    var response = new TokenResponse();
        //    var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

        //    // always return ok response to prevent email enumeration
        //    if (account == null) return response;

        //    // create reset token that expires after 1 day
        //    account.ResetToken = generateResetToken();
        //    account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

        //    _context.Accounts.Update(account);
        //    _context.SaveChanges();

        //    // send email
        //    //sendPasswordResetEmail(account, origin);
        //    response.Message = "Please use the above token to reset your password with the /accounts/reset-password api route";
        //    response.Token = account.ResetToken;

        //    return response;
        //}

        //public void ValidateResetToken(ValidateResetTokenRequest model)
        //{
        //    getAccountByResetToken(model.Token);
        //}

        //public void ResetPassword(ResetPasswordRequest model)
        //{
        //    var account = getAccountByResetToken(model.Token);

        //    // update password and remove reset token
        //    account.PasswordHash = BCrypt.HashPassword(model.Password);
        //    account.PasswordReset = DateTime.UtcNow;
        //    account.ResetToken = null;
        //    account.ResetTokenExpires = null;

        //    _context.Accounts.Update(account);
        //    _context.SaveChanges();
        //}

        //public IEnumerable<UserResponse> GetAll()
        //{
        //    var accounts = _context.Accounts;
        //    return _mapper.Map<IList<UserResponse>>(accounts);
        //}

        //public UserResponse GetById(int id)
        //{
        //    var account = getAccount(id);
        //    return _mapper.Map<UserResponse>(account);
        //}

        //public UserResponse Create(CreateUserRequest model)
        //{
        //    // validate
        //    if (_context.Accounts.Any(x => x.Email == model.Email))
        //        throw new AppException($"Email '{model.Email}' is already registered");

        //    // map model to new account object
        //    var account = _mapper.Map<Account>(model);
        //    account.Created = DateTime.UtcNow;
        //    account.Verified = DateTime.UtcNow;

        //    // hash password
        //    account.PasswordHash = BCrypt.HashPassword(model.Password);

        //    // save account
        //    _context.Accounts.Add(account);
        //    _context.SaveChanges();

        //    return _mapper.Map<UserResponse>(account);
        //}

        //public UserResponse Update(int id, UpdateUserRequest model)
        //{
        //    var account = getAccount(id);

        //    // validate
        //    if (account.Email != model.Email && _context.Accounts.Any(x => x.Email == model.Email))
        //        throw new AppException($"Email '{model.Email}' is already registered");

        //    // hash password if it was entered
        //    if (!string.IsNullOrEmpty(model.Password))
        //        account.PasswordHash = BCrypt.HashPassword(model.Password);

        //    // copy model to account and save
        //    _mapper.Map(model, account);
        //    account.Updated = DateTime.UtcNow;
        //    _context.Accounts.Update(account);
        //    _context.SaveChanges();

        //    return _mapper.Map<UserResponse>(account);
        //}

        //public void Delete(int id)
        //{
        //    var account = getAccount(id);
        //    _context.Accounts.Remove(account);
        //    _context.SaveChanges();
        //}

        //// helper methods

        //private Account getAccount(int id)
        //{
        //    var account = _context.Accounts.Find(id);
        //    if (account == null) throw new KeyNotFoundException("Account not found");
        //    return account;
        //}

        //private Account getAccountByRefreshToken(string token)
        //{
        //    var account = _context.Accounts.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
        //    if (account == null) throw new AppException("Invalid token");
        //    return account;
        //}

        //private Account getAccountByResetToken(string token)
        //{
        //    var account = _context.Accounts.SingleOrDefault(x =>
        //        x.ResetToken == token && x.ResetTokenExpires > DateTime.UtcNow);
        //    if (account == null) throw new AppException("Invalid token");
        //    return account;
        //}

        //private string generateJwtToken(Account account)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()) }),
        //        Expires = DateTime.UtcNow.AddMinutes(15),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}

        //private string generateResetToken()
        //{
        //    // token is a cryptographically strong random sequence of values
        //    var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        //    // ensure token is unique by checking against db
        //    var tokenIsUnique = !_context.Accounts.Any(x => x.ResetToken == token);
        //    if (!tokenIsUnique)
        //        return generateResetToken();

        //    return token;
        //}

        //private string generateVerificationToken()
        //{
        //    // token is a cryptographically strong random sequence of values
        //    var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        //    // ensure token is unique by checking against db
        //    var tokenIsUnique = !_context.Accounts.Any(x => x.VerificationToken == token);
        //    if (!tokenIsUnique)
        //        return generateVerificationToken();

        //    return token;
        //}

        //private RefreshToken rotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        //{
        //    var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        //    revokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
        //    return newRefreshToken;
        //}

        //private void removeOldRefreshTokens(Account account)
        //{
        //    account.RefreshTokens.RemoveAll(x =>
        //        !x.IsActive &&
        //        x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        //}

        //private void revokeDescendantRefreshTokens(RefreshToken refreshToken, Account account, string ipAddress, string reason)
        //{
        //    // recursively traverse the refresh token chain and ensure all descendants are revoked
        //    if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
        //    {
        //        var childToken = account.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
        //        if (childToken.IsActive)
        //            revokeRefreshToken(childToken, ipAddress, reason);
        //        else
        //            revokeDescendantRefreshTokens(childToken, account, ipAddress, reason);
        //    }
        //}

        //private void revokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        //{
        //    token.Revoked = DateTime.UtcNow;
        //    token.RevokedByIp = ipAddress;
        //    token.ReasonRevoked = reason;
        //    token.ReplacedByToken = replacedByToken;
        //}

        //private void sendVerificationEmail(Account account, string origin)
        //{
        //    string message;
        //    if (!string.IsNullOrEmpty(origin))
        //    {
        //        // origin exists if request sent from browser single page app (e.g. Angular or React)
        //        // so send link to verify via single page app
        //        var verifyUrl = $"{origin}/account/verify-email?token={account.VerificationToken}";
        //        message = $@"<p>Please click the below link to verify your email address:</p>
        //                    <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
        //    }
        //    else
        //    {
        //        // origin missing if request sent directly to api (e.g. from Postman)
        //        // so send instructions to verify directly with api
        //        message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
        //                    <p><code>{account.VerificationToken}</code></p>";
        //    }

        //    _emailService.Send(
        //        to: account.Email,
        //        subject: "Sign-up Verification API - Verify Email",
        //        html: $@"<h4>Verify Email</h4>
        //                <p>Thanks for registering!</p>
        //                {message}"
        //    );
        //}

        //private void sendAlreadyRegisteredEmail(string email, string origin)
        //{
        //    string message;
        //    if (!string.IsNullOrEmpty(origin))
        //        message = $@"<p>If you don't know your password please visit the <a href=""{origin}/account/forgot-password"">forgot password</a> page.</p>";
        //    else
        //        message = "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

        //    _emailService.Send(
        //        to: email,
        //        subject: "Sign-up Verification API - Email Already Registered",
        //        html: $@"<h4>Email Already Registered</h4>
        //                <p>Your email <strong>{email}</strong> is already registered.</p>
        //                {message}"
        //    );
        //}

        //private void sendPasswordResetEmail(Account account, string origin)
        //{
        //    string message;
        //    if (!string.IsNullOrEmpty(origin))
        //    {
        //        var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
        //        message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
        //                    <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
        //    }
        //    else
        //    {
        //        message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
        //                    <p><code>{account.ResetToken}</code></p>";
        //    }

        //    _emailService.Send(
        //        to: account.Email,
        //        subject: "Sign-up Verification API - Reset Password",
        //        html: $@"<h4>Reset Password Email</h4>
        //                {message}"
        //    );
        //}
    }
}
