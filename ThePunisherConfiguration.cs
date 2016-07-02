using System;
using Rocket.API;
using System.Xml.Serialization;

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
        public string DefaultBanDuration;
        public ulong KeepWarningsLengthDays;
        public bool WarningtoKickOn;
        public byte WarningstoKick;
        public bool WarningtoBanOn;
        public byte WarningstoBan;
        public string AutoBanDuration;
        public bool DisplayBanMessagePublic;
        public bool DisplayKickMessagePublic;
        //public bool AdminsExemptFromBan;

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

            DefaultBanDuration = "1d";
            KeepWarningsLengthDays = 30;
            WarningtoKickOn = true;
            WarningstoKick = 3;
            WarningtoBanOn = true;
            WarningstoBan = 10;
            AutoBanDuration = "15m";

            DisplayBanMessagePublic = true;
            DisplayKickMessagePublic = true;
            //AdminsExemptFromBan = false;
        }
    }
}
