using System;
using System.Collections.Generic;
using VkNet;
using VkNet.Enums.Filters;
using System.IO;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Newtonsoft.Json;

namespace VkBot
{

    public class VkBot
    {
        Dictionary<string, List<int>> UserIDs = new Dictionary<string, List<int>>();
        public int GroupID = 202482264;
        public VkApi api;
        public string Token = "";
        public VkBot()
        {
            if (File.Exists("UserIdS.txt")) UserIDs = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(File.ReadAllText("UserIdS.txt"));
            api = new VkApi();
            api.Authorize(new ApiAuthParams()
            {
                AccessToken = "2304bfa522f04e730a06703c124910c907e7f463aa8b7ec56e1196bb2e681d1fad2e620c251c5fe7e2cba",
                Settings = Settings.All
            }) ;
            api.OnTokenExpires += Api_OnTokenExpires;
        }

        private void Api_OnTokenExpires(VkApi sender)
        {
            Celendar.Writeline("Токен устарел, введите новый токен\n");
            api.Authorize(new ApiAuthParams() { AccessToken = Token, Settings = Settings.All });
        }

        public void SendMessage(string message, string Group)
        {
            if (!UserIDs.ContainsKey(Group)) return;
            try
            {
                foreach (var item in UserIDs[Group])
                    api.Messages.Send(new MessagesSendParams()
                    {
                        UserId = item,
                        Message = message,
                        RandomId = new Random().Next()
                    });
            }
            catch(Exception e)
            {
                Celendar.Writeline("VkError:" + e.Message + "\n");
            }
        }
        public void AddUser(string Group, int UserId)
        {
            if (!UserIDs.ContainsKey(Group)) UserIDs[Group] = new List<int>();
            UserIDs[Group].Add(UserId);
            SaveUserIds();
        }
        void SaveUserIds()=> File.WriteAllText("UserIdS.txt", JsonConvert.SerializeObject(UserIDs));
    }
}
