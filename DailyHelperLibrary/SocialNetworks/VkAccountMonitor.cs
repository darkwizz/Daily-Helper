using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace DailyHelperLibrary.SocialNetworks
{
    class VkAccountMonitor : SocialNetworkAccountMonitor
    {
        private static int _appId = 4986036;
        private volatile bool _continueMonitoring = false;
        private VkApi _api = new VkApi();
        private long _currentUserId;

        public VkAccountMonitor(string email, string password, Action<string> notificationHandler) :
            base(email, password, notificationHandler)
        {
            // here can be next exceptions: broken connection; incorrect auth data
            _api.Authorize(_appId, email, password, Settings.All);
            _currentUserId = _api.UserId.Value;
        }

        public override void StartMonitoring()
        {
            _continueMonitoring = true;
            ThreadPool.QueueUserWorkItem((state) => CheckOnNewMessages());
        }

        private void CheckOnNewMessages()
        {
            var server = _api.Messages.GetLongPollServer();
            long ts = server.Ts; // ts -> see in vk docs. Shortly it is the number of last caught event
            string longPollString;
            HttpWebRequest request;
            do
            {
                longPollString = string.Format("http://{0}?act=a_check&key={1}&ts={2}&wait=25&mode=2", server.Server,
                server.Key, ts);
                request = HttpWebRequest.CreateHttp(longPollString);
                using (WebResponse response = request.GetResponse())
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    string reply = reader.ReadToEnd();
                    reader.Close();

                    if (reply == null || reply.Contains("failed"))
                    {
                        continue;
                    }

                    JToken token = JToken.Parse(reply); // response view: { ts: some_number, updates: [ ... ] }
                    ts = (long)token.SelectToken("ts");
                    token = token.SelectToken("updates");
                    string jsonUpdates = token.ToString();
                    if (jsonUpdates == null || jsonUpdates.Contains("[]"))
                    {
                        continue;
                    }

                    object[][] updates = JsonConvert.DeserializeObject<object[][]>(jsonUpdates);
                    if (updates == null || updates.Length == 0)
                    {
                        continue;
                    }
                    string notification = GetNewUnreadMessages(updates);
                    if (notification.Equals(""))
                    {
                        continue;
                    }
                    _notificationHandler(notification);
                }
            } while (_continueMonitoring);
        }

        private string GetNewUnreadMessages(object[][] updates)
        {
            string result = "";
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
                long fromId = (long)update[3];
                long messageEventCode = (long)update[0];
                long flag = (long)update[2];

                if (messageEventCode != 4)
                {
                    continue;
                }
                if ((flag & 1) == 0)
                {
                    Console.WriteLine("It isn't unread message..."); // logging
                }
                if (fromId == _currentUserId)
                {
                    Console.WriteLine("User sent message to himself..."); // logging
                    // Some actions, if user sent message to himself
                }
                User vkUser = _api.Users.Get(fromId);
                result += string.Format("You've gotten a new message from user {0} {1}\n", vkUser.FirstName, vkUser.LastName);
            }
            return result;
        }

        public override void StopMonitoring()
        {
            _continueMonitoring = false;
        }
    }
}
