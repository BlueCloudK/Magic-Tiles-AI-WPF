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

        public GoogleAuthService(string clientId, string clientSecret)
        {
            _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            _clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
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

                // For desktop apps, GoogleWebAuthorizationBroker automatically uses LocalServerCodeReceiver
                // which starts a local HTTP server on a random port (e.g., http://localhost:xxxxx)
                // No need to specify custom redirect URI - Google Cloud Console automatically accepts
                // http://localhost for Desktop app type OAuth clients
                System.Diagnostics.Debug.WriteLine("Starting OAuth authentication flow...");

                _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)
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
