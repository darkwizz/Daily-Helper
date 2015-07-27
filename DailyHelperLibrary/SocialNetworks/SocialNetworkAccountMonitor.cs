using System.Collections.Generic;

namespace DailyHelperLibrary.SocialNetworks
{
    /// <summary>
    /// Base class to social network accounts monitoring
    /// TODO: create VKAccountMonitor, TwitterAccountMonitor, FacebookAccountMonitor
    /// </summary>
    abstract class SocialNetworkAccountMonitor
    {
        public string Email { get; private set; }
        public string Password { get; private set; }

        public SocialNetworkAccountMonitor(string email, string password)
        {
            Email = email;
            Password = password;
        }

        /// <summary>
        /// Logs into specified in constructor account
        /// </summary>
        /// <returns><code>true</code>, if specified login and password are verified</returns>
        public abstract bool Authorize();
        /// <summary>
        /// Blocks current thread until it accepts some response from <code>SocialNetwork</code> long-poll
        /// server and returns its string representation. Works only after <code>Authorize()</code> calling.
        /// </summary>
        /// <returns>strign representation of server response</returns>
        public abstract string GetServerResponse();
        /// <summary>
        /// Checks accepted server response on having some new unread messages
        /// </summary>
        /// <param name="response">Social network Server response</param>
        /// <returns><code>true</code>, if there are some new unread messages</returns>
        public abstract bool HasNewUnreadMessages(string response);
        /// <summary>
        /// If there are some new unread messages, this method returns list of
        /// messages authors
        /// </summary>
        /// <param name="response">Social network Server response</param>
        /// <returns>List of unread messages authors</returns>
        public abstract List<string> GetUnreadMessagesAuthors(string response);
    }
}
