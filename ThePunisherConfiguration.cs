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
        public int DatabasePort;
        public bool ShowDebugInfo;

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
            DatabasePort = 3306;
            ShowDebugInfo = true;
        }
    }
}
