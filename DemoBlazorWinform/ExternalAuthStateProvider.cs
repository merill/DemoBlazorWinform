using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

public class ExternalAuthStateProvider : AuthenticationStateProvider
{
    private AuthenticationState currentUser;

    public ExternalAuthStateProvider(ExternalAuthService service)
    {
        currentUser = new AuthenticationState(service.CurrentUser);

        service.UserChanged += (newUser) =>
        {
            currentUser = new AuthenticationState(newUser);
            NotifyAuthenticationStateChanged(Task.FromResult(currentUser));
        };
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        Task.FromResult(currentUser);
}

public class ExternalAuthService
{
    public event Action<ClaimsPrincipal>? UserChanged;
    private ClaimsPrincipal? currentUser;

    public ClaimsPrincipal CurrentUser
    {
        get { return currentUser ?? new(); }
        set
        {
            currentUser = value;

            if (UserChanged is not null)
            {
                UserChanged(currentUser);
            }
        }
    }
}