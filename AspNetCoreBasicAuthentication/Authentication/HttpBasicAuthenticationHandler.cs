using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AspNetCoreBasicAuthentication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCoreBasicAuthentication.Authentication
{
    public class HttpBasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private BasicAuthenticationHeaderValue _headerValue;

        public HttpBasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
            : base(options, logger, encoder, clock)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.User.Identity.IsAuthenticated)
            {
                GetBasicAuthenticationHeaderValue();
                if (_headerValue.IsValidBasicAuthenticationHeaderValue)
                {
                    var principal = await GetPrincipalOrNullFromHeader();
                    if (principal != null)
                    {
                        var ticket = new AuthenticationTicket(principal, "Basic");
                        var authResult = AuthenticateResult.Success(ticket);
                        return authResult;
                    }
                }
            }
            return AuthenticateResult.NoResult();
        }

        private void GetBasicAuthenticationHeaderValue()
        {
            var basicAuthenticationHeader = Context.Request.Headers["Authorization"]
                .FirstOrDefault(header => header.StartsWith("Basic", StringComparison.OrdinalIgnoreCase));
            var decodedHeader = new BasicAuthenticationHeaderValue(basicAuthenticationHeader);
            _headerValue = decodedHeader;
        }

        private async Task<ClaimsPrincipal> GetPrincipalOrNullFromHeader()
        {
            var user = await GetUserByUsernameOrEmail();
            if (user != null)
            {
                var principal = await GetPrincipalIfPasswordIsCorrect(user);
                return principal;
            }
            return null;
        }

        private async Task<ApplicationUser> GetUserByUsernameOrEmail()
        {
            var user = await _userManager.FindByEmailAsync(_headerValue.UserIdentifier)
                       ?? await _userManager.FindByNameAsync(_headerValue.UserIdentifier);
            return user;
        }

        private async Task<ClaimsPrincipal> GetPrincipalIfPasswordIsCorrect(ApplicationUser user)
        {
            if (await _userManager.CheckPasswordAsync(user, _headerValue.UserPassword))
            {
                var principal = await _signInManager.CreateUserPrincipalAsync(user);
                return principal;
            }
            return null;
        }
    }
}
