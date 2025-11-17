namespace BlueCloudK.WpfMusicTilesAI.Models
{
    /// <summary>
    /// Represents the authentication state of the application
    /// </summary>
    public enum AuthenticationState
    {
        /// <summary>
        /// User is not authenticated
        /// </summary>
        NotAuthenticated,

        /// <summary>
        /// Authentication is in progress
        /// </summary>
        Authenticating,

        /// <summary>
        /// User is authenticated
        /// </summary>
        Authenticated
    }
}
