using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.Services
{
    /// <summary>
    /// Service for Google OAuth authentication using OAuth 2.0 for Desktop Apps
    /// </summary>
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string? _redirectUri;
        private UserCredential? _credential;

        // Required scopes for Generative AI
        private static readonly string[] Scopes = {
            "https://www.googleapis.com/auth/generative-language.retriever",
            "https://www.googleapis.com/auth/generative-language.tuning",
            "openid",
            "profile",
            "email"
        };

        public UserCredential? CurrentCredential => _credential;
        public bool IsAuthenticated => _credential != null && !string.IsNullOrEmpty(_credential.Token.AccessToken);

        public GoogleAuthService(string clientId, string clientSecret, string? redirectUri = null)
        {
            _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            _clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
            _redirectUri = redirectUri;
        }

        public async Task<UserCredential> AuthenticateAsync()
        {
            try
            {
                var clientSecrets = new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                };

                // Store token in AppData
                var credPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "MagicTilesAI",
                    "token.json"
                );

                // Use custom redirect URI if provided
                ICodeReceiver? codeReceiver = null;
                if (!string.IsNullOrEmpty(_redirectUri) && Uri.TryCreate(_redirectUri, UriKind.Absolute, out var uri))
                {
                    // Extract port from redirect URI (e.g., http://127.0.0.1:58291/signin-google)
                    var port = uri.Port;
                    var path = uri.AbsolutePath;

                    System.Diagnostics.Debug.WriteLine($"Using custom redirect URI: {_redirectUri} (Port: {port}, Path: {path})");
                    codeReceiver = new Google.Apis.Auth.OAuth2.LocalServerCodeReceiver("127.0.0.1", port);
                }

                _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true),
                    codeReceiver
                );

                return _credential;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to authenticate with Google: {ex.Message}", ex);
            }
        }

        public async Task SignOutAsync()
        {
            if (_credential != null)
            {
                await _credential.RevokeTokenAsync(CancellationToken.None);
                _credential = null;

                // Delete stored tokens
                var credPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "MagicTilesAI",
                    "token.json"
                );

                if (Directory.Exists(credPath))
                {
                    Directory.Delete(credPath, true);
                }
            }
        }

        public async Task<UserCredential> GetCredentialAsync()
        {
            if (_credential == null)
            {
                await AuthenticateAsync();
            }
            return _credential!;
        }
    }
}
