using Microsoft.Identity.Client;

namespace DemoBlazorWinform
{
    internal static class Program
    {
        public static string ClientId = "62f85080-9912-4a16-b0be-aff63ee2eeea";

        private static IPublicClientApplication clientApp;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            InitializeAuth();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        public static IPublicClientApplication PublicClientApp { get { return clientApp; } }

        private static void InitializeAuth()
        {

            clientApp = PublicClientApplicationBuilder.Create(ClientId)
                    .WithRedirectUri("http://localhost")
                    .Build();
        }
    }
}