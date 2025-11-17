using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.Services
{
    /// <summary>
    /// Interface for Google OAuth authentication service
    /// </summary>
    public interface IGoogleAuthService
    {
        /// <summary>
        /// Initiates the OAuth flow and returns user credentials
        /// </summary>
        Task<UserCredential> AuthenticateAsync();

        /// <summary>
        /// Gets the current user credential if already authenticated
        /// </summary>
        UserCredential? CurrentCredential { get; }

        /// <summary>
        /// Checks if user is currently authenticated
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Signs out the current user
        /// </summary>
        Task SignOutAsync();

        /// <summary>
        /// Gets the user credential, authenticating if needed
        /// </summary>
        Task<UserCredential> GetCredentialAsync();
    }
}
