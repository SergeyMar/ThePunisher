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
        public int DatabasePort;

        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabaseTableName = "banlist";
            DatabaseReportTableName = "player_reports";
            DatabaseWarningTableName = "player_warnings";
            DatabaseChatbanTableName = "Chatbans";
            DatabasePort = 3306;
        }
    }
}
