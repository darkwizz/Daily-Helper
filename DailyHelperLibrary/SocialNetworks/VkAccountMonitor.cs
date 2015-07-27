using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using DailyHelperLibrary.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Utils;

namespace DailyHelperLibrary.SocialNetworks
{
    class VkAccountMonitor : SocialNetworkAccountMonitor
    {
        private static int _appId = 4986036;
        private VkApi _api = new VkApi();
        private long _currentUserId;
        private long _lastTs; // ts -> see in vk docs. Shortly it is the number of last caught event

        public VkAccountMonitor(string email, string password) :
            base(email, password)
        { }

        public override bool Authorize()
        {
            // here can be next exceptions: broken connection; incorrect auth data
            try
            {
                _api.Authorize(_appId, Email, Password, Settings.All);
                _currentUserId = _api.UserId.Value;
                return true;
            }
            catch (VkApiAuthorizationException ex)
            {
                Console.WriteLine(ex.Message); // logging
                return false;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message); // logging
                //if (ex.Status == WebExceptionStatus.ConnectFailure ||
                //    ex.Status == WebExceptionStatus.ConnectionClosed)
                //{
                    throw new ConnectionFailedException(ex);
                //}
            }
        }

        public override string GetServerResponse()
        {
            try
            {
                var server = _api.Messages.GetLongPollServer();
                _lastTs = server.Ts;
                string longPollString = string.Format("http://{0}?act=a_check&key={1}&ts={2}&wait=25&mode=2", server.Server,
                    server.Key, _lastTs);
                HttpWebRequest request = HttpWebRequest.CreateHttp(longPollString);
                using (WebResponse response = request.GetResponse())
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    string reply = reader.ReadToEnd();
                    reader.Close();
                    return reply;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message); // logging
                //if (ex.Status == WebExceptionStatus.ConnectFailure ||
                //    ex.Status == WebExceptionStatus.ConnectionClosed)
                //{
                throw new ConnectionFailedException(ex);
                //}
            }
        }

        public override bool HasNewUnreadMessages(string response)
        {
            if (response == null || response.Contains("failed"))
            {
                return false;
            }
            JToken token = JToken.Parse(response); // response view: { ts: some_number, updates: [ ... ] }
            _lastTs = (long)token.SelectToken("ts");
            token = token.SelectToken("updates");
            string jsonUpdates = token.ToString();

            object[][] updates = GetMessageUpdates(jsonUpdates);
            if (updates == null || updates.Length == 0)
            {
                return false;
            }
            return HasUnreadMessages(updates);
        }

        private object[][] GetMessageUpdates(string strUpdates)
        {
            if (strUpdates == null || strUpdates.Contains("[]"))
            {
                return null;
            }

            object[][] updates = JsonConvert.DeserializeObject<object[][]>(strUpdates);
            return updates;
        }

        private bool HasUnreadMessages(object[][] updates)
        {
            bool hasUnread = false;
            foreach (var update in updates)
            {
                long messageEventCode = (long)update[0];
                if (messageEventCode == 4)
                {
                    hasUnread = true;
                }
            }
            return hasUnread;
        }

        /// <summary>
        /// If there are some new unread messages, this method returns list of
        /// messages authors. BUT! It can read info only when <code>Authorize</code>
        /// is called
        /// </summary>
        /// <param name="response">Social network Server response</param>
        /// <returns>List of unread messages authors</returns>
        public override List<string> GetUnreadMessagesAuthors(string response)
        {
            List<string> authors = new List<string>();
            JToken token = JToken.Parse(response);
            token = token.SelectToken("updates");
            string jsonUpdates = token.ToString();
            object[][] updates = GetMessageUpdates(jsonUpdates);
            if (updates == null || updates.Length == 0)
            {
                return authors;
            }

            try
            {
                foreach (var update in updates)
                {
                    /*
                     * The event's code is rendered as the first parameter, the following event codes are supported:
                
                     * 0,$message_id,0 -- delete a message with the local_id indicated;
                     * 1,$message_id,$flags -- replace message flags (FLAGS:=$flags);
                     * 2,$message_id,$mask[,$user_id] -- install message flags (FLAGS|=$mask);
                     * 3,$message_id,$mask[,$user_id] -- reset message flags (FLAGS&=~$mask);
                     * 4,$message_id,$flags,$from_id,$timestamp,$subject,$text,$attachments -- add a new message;
                     * ...
                     */
                    long messageEventCode = (long)update[0];
                    if (messageEventCode != 4)
                    {
                        continue;
                    }

                    long fromId = (long)update[3];
                    long flag = (long)update[2];
                    if ((flag & 1) == 0)
                    {
                        Console.WriteLine("It isn't unread message..."); // logging
                        continue;
                    }
                    if (fromId == _currentUserId)
                    {
                        Console.WriteLine("User sent message to himself..."); // logging
                        // Some actions, if user sent message to himself
                    }
                    User vkUser = _api.Users.Get(fromId);
                    // User vkUser = GetUser(fromId);
                    string author = vkUser.FirstName + " " + vkUser.LastName;
                    authors.Add(author);
                }
                return authors;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message); // logging
                //if (ex.Status == WebExceptionStatus.ConnectFailure ||
                //    ex.Status == WebExceptionStatus.ConnectionClosed)
                //{
                throw new ConnectionFailedException(ex);
                //}
            }
        }

        // This is method is experimental
        // it can load user data even without authorization.
        // BUT it reads only first name and last name
        private User GetUser(long id)
        {
            Dictionary<string, string> parameters = new Dictionary<string,string>();
            parameters.Add("fields", null);
            parameters.Add("name_case", null);
            parameters.Add("user_ids", id.ToString());

            string response = _api.Invoke("users.get", parameters, true);
            JObject json = JObject.Parse(response);

            var rawResponse = json["response"];
            JArray array = rawResponse.SelectToken("items", false) == null ?
                (JArray)rawResponse : (JArray)(rawResponse["items"]);
            JToken item = array[0];
            User user = new User();
            user.FirstName = item["first_name"].ToString();
            user.LastName = item["last_name"].ToString();
            return user;
        }
    }
}
