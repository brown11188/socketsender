using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mateo.UILogic.SocketMessage
{
    public class CommandMessage : BaseMessage
    {
        [JsonProperty(PropertyName = "username")]
        public string Username;
        [JsonProperty(PropertyName = "command_type")]
        public string CommandType;
        [JsonProperty(PropertyName = "username")]
        public string Message;
        [JsonProperty(PropertyName = "company_name")]
        public string CompanyName;
        [JsonProperty(PropertyName = "time_stamp")]
        public string TimeStamp;

        public CommandMessage()
        {
        }

        public CommandMessage(string username, string commandType, string message, string companyName, string timeStamp)
        {
            Username = username;
            CommandType = commandType;
            Message = message;
            CompanyName = companyName;
            TimeStamp = timeStamp;
        }
    }
}
