using System.Configuration;

namespace TeamClient
{
    public class ConfigureInfo
    {
        AppSettingsReader appSettings;

        public ConfigureInfo()
        {
            appSettings = new AppSettingsReader();
        }
        #region ConfigureSetting
        public string GetConfigure()
        {
            string configInfo = "akka.tcp://" + ServerActorSystemName + "@" +
                ServerIP + ":" + ServerPort.ToString() + "/user/" +
                ServerRootActorName + "/" + ClientServerActorName;

            return configInfo;
        }
        public string ClientActorName
        {
            get
            {
                return (string)appSettings.GetValue("ClientActorSystemName", typeof(string));
            }
        }
        public string ServerIP
        {
            get
            {
                return (string)appSettings.GetValue("ServerIP", typeof(string));
            }
        }
        public string ClientServerActorName
        {
            get
            {
                return (string)appSettings.GetValue("ClientServerActorName", typeof(string));
            }
        }
        public string ServerRootActorName
        {
            get
            {
                return (string)appSettings.GetValue("ServerRootActorName", typeof(string));
            }
        }
        public string ServerActorSystemName
        {
            get
            {
                return (string)appSettings.GetValue("ServerActorSystemName", typeof(string));
            }
        }
        public int ServerPort
        {
            get
            {
                return (int)appSettings.GetValue("ServerPort", typeof(int));
            }
        }
        public int RetryServerConnectTime
        {
            get
            {
                return (int)appSettings.GetValue("RetryServerConnectTime", typeof(int));
            }
        }
        #endregion
    }
}
