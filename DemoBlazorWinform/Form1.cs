using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Identity.Client;
using static System.Formats.Asn1.AsnWriter;
using System.Diagnostics;

namespace DemoBlazorWinform
{
    public partial class Form1 : Form
    {
        private static readonly string[] scopes = { "user.read" };
        private ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        public Form1()
        {
            InitializeComponent();
            var services = new ServiceCollection();
            services.AddWindowsFormsBlazorWebView();
            blazorWebView1.HostPage = "wwwroot\\index.html";
            //blazorWebView1.Services = services.BuildServiceProvider();

            //Opt1
            services.AddAuthorizationCore();
            services.TryAddScoped<AuthenticationStateProvider, ExternalAuthStateProvider>();
            services.AddSingleton<ExternalAuthService>();

            //Opt2
            //services.AddAuthorizationCore();
            //services.TryAddScoped<AuthenticationStateProvider, ExternalAuthStateProvider>();


            var serviceCollection = services.BuildServiceProvider();
            blazorWebView1.Services = serviceCollection;

            blazorWebView1.RootComponents.Add<Counter>("#app");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var authService = blazorWebView1.Services.GetRequiredService<ExternalAuthService>();
            var result = LoginMsal().Result;
            var authenticatedUser = result.ClaimsPrincipal;

            authService.CurrentUser = authenticatedUser;

        }

        private async Task<AuthenticationResult> LoginMsal()
        {
            AuthenticationResult authResult = null;
            var accounts = await Program.PublicClientApp.GetAccountsAsync();
            var firstAccount = accounts.FirstOrDefault();

            try
            {
                authResult = await Program.PublicClientApp.AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilent.
                // This indicates you need to call AcquireTokenInteractive to acquire a token
                System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await Program.PublicClientApp.AcquireTokenInteractive(scopes)
                        .WithAccount(accounts.FirstOrDefault())
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    Debug.WriteLine($"Error Acquiring Token:{System.Environment.NewLine}{msalex}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
            }
            return authResult;
        }
    }
}