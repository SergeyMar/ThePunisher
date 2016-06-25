using System;
using Rocket.API;

namespace rawrfuls.ThePunisher
{
    public class ThePunisherConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabaseTableName;
        public string DatabaseReportTableName;
        public string DatabaseWarningTableName;
        public string DatabaseChatbanTableName;
        public string DatabaseRewardlogTableName;
        public string DatabaseWhiteListTableName;
        public bool WhiteListEnabled;
        public int DatabasePort;
        public bool ShowDebugInfo;
        public string PublicMessageColor;
        public string PrivateMessageColor;

        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabaseTableName = "banlist";
            DatabaseReportTableName = "player_reports";
            DatabaseWarningTableName = "player_warnings";
            DatabaseChatbanTableName = "chatbans";
            DatabaseRewardlogTableName = "reward_logs";
            WhiteListEnabled = false;
            DatabaseWhiteListTableName = "whitelist";
            DatabasePort = 3306;
            ShowDebugInfo = false;
            PublicMessageColor = "Red";
            PrivateMessageColor = "Green";
        }
    }
}
