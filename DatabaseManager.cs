using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;

namespace rawrfuls.ThePunisher
{
    public class DatabaseManager
    {
        public DatabaseManager()
        {
            new I18N.West.CP1250();
            if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
            {
                Logger.Log("Boot Sequence Initialized...");
                Logger.Log("Checking tables");
            }
            CheckBanSchema();
            CheckChatBanSchema();
            CheckWarnSchema();
            CheckReportSchema();
            CheckRewardlogSchema();
            if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                EndSchemaChecks();
        }

        private MySqlConnection createConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (ThePunisher.Instance.Configuration.Instance.DatabasePort == 0) ThePunisher.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", ThePunisher.Instance.Configuration.Instance.DatabaseAddress, ThePunisher.Instance.Configuration.Instance.DatabaseName, ThePunisher.Instance.Configuration.Instance.DatabaseUsername, ThePunisher.Instance.Configuration.Instance.DatabasePassword, ThePunisher.Instance.Configuration.Instance.DatabasePort));
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
            return connection;
        }

        public void RewardForReport(string reportee)
        {
            try
            {

                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@reportee", reportee);
                command.CommandText = "UPDATE " + ThePunisher.Instance.Configuration.Instance.DatabaseReportTableName + " SET `rewarded` = 'Yes' WHERE `reportee` = @reportee;";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                LogReward(reportee);
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
        }

        public void ReportPlayer(string characterName, string steamid, string reportee, string reportMessage)
        {
            try
            {

                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                if (reportMessage == null) reportMessage = "";
                command.Parameters.AddWithValue("@csteamid", steamid);
                command.Parameters.AddWithValue("@reportee", reportee);
                command.Parameters.AddWithValue("@charactername", characterName);
                command.Parameters.AddWithValue("@reportMessage", reportMessage);
                command.CommandText = "insert into `" + ThePunisher.Instance.Configuration.Instance.DatabaseReportTableName + "` (`steamId`,`reportee`,`reportMessage`,`charactername`,`reportTime`) values(@csteamid,@reportee,@reportMessage,@charactername,now());";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
        }

        public void WarnPlayer(string characterName, string steamid, string admin, string warnMessage)
        {
            try
            {

                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                if (warnMessage == null) warnMessage = "";
                command.Parameters.AddWithValue("@csteamid", steamid);
                command.Parameters.AddWithValue("@admin", admin);
                command.Parameters.AddWithValue("@charactername", characterName);
                command.Parameters.AddWithValue("@warnMessage", warnMessage);
                command.CommandText = "insert into `" + ThePunisher.Instance.Configuration.Instance.DatabaseWarningTableName + "` (`steamId`,`admin`,`warnMessage`,`charactername`,`warnTime`) values(@csteamid,@admin,@warnMessage,@charactername,now());";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
        }

        public string GetReports()
        {
            string output = "This feature is not available yet";
            
            return output;
        }

        public string IsBanned(string steamId)
        {
            string output = null;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `banMessage` from `" + ThePunisher.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` = '" + steamId + "' and (banDuration is null or ((banDuration + UNIX_TIMESTAMP(banTime)) > UNIX_TIMESTAMP()));";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) output = result.ToString();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
            return output;
        }

        public string HasReported(string steamId)
        {
            string output = null;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@reportee", steamId);
                command.CommandText = "select `reportMessage` from `" + ThePunisher.Instance.Configuration.Instance.DatabaseReportTableName + "` where `reportee` = @reportee and `rewarded` IS NULL;";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) output = result.ToString();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
            return output;
        }

        public string HasReported(string steamId, string reportee)
        {
            string output = null;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `reportMessage` from `" + ThePunisher.Instance.Configuration.Instance.DatabaseReportTableName + "` where `reportee` = '" + reportee + "' and `steamId` = '"+steamId+"';";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) output = result.ToString();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
            return output;
        }

        public string HasBeenRewarded(string steamId)
        {
            string output = null;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `reportMessage` from `" + ThePunisher.Instance.Configuration.Instance.DatabaseReportTableName + "` where `reportee` = '" + steamId + "' AND `rewarded` IS NOT NULL;";
                connection.Open();
                object result = command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
            return output;
        }

        public void LogReward(string steamid)
        {
            try
            {
                ulong steamId = 0;
                string reportee = "";
                string reportMessage = "";
                string characterName = "";
                string reportTime = "";
                string rewarded = "";
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@reportee", "%" + steamid + "%");
                command.CommandText = "select * from `" + ThePunisher.Instance.Configuration.Instance.DatabaseReportTableName + "` where `reportee` like @reportee AND `rewarded` IS NOT NULL;";
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    steamId = reader.GetUInt64(1);
                    reportee = reader.GetString(2);
                    reportMessage = reader.GetString(3);
                    characterName = reader.GetString(4);
                    reportTime = reader.GetString(5);
                    rewarded = reader.GetString(6);
                    connection.Close();
                    command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@steamId", steamId);
                    command.Parameters.AddWithValue("@reportee", reportee);
                    command.Parameters.AddWithValue("@reportMessage", reportMessage);
                    command.Parameters.AddWithValue("@characterName", characterName);
                    command.Parameters.AddWithValue("@reportTime", reportTime);
                    command.Parameters.AddWithValue("@rewarded", rewarded);
                    command.CommandText = "INSERT INTO `"+ThePunisher.Instance.Configuration.Instance.DatabaseRewardlogTableName+"` (`steamId`, `reportee`, `reportMessage`, `charactername`, `reportTime`, `rewarded`) VALUES (@steamId, @reportee, @reportMessage, @characterName, @reportTime, @rewarded);";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@steamId", steamId);
                    command.Parameters.AddWithValue("@reportee", reportee);
                    command.CommandText = "delete from `" + ThePunisher.Instance.Configuration.Instance.DatabaseReportTableName + "` where `steamId` = @steamId AND `reportee`= @reportee;";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
        }

        public string IsChatBanned(string steamId)
        {
            string output = null;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `banMessage` from `" + ThePunisher.Instance.Configuration.Instance.DatabaseChatbanTableName + "` where `steamId` = '" + steamId + "' and (banDuration is null or ((banDuration + UNIX_TIMESTAMP(banTime)) > UNIX_TIMESTAMP()));";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) output = result.ToString();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
            return output;
        }

        #region Schema Checks
        private bool BanLoaded = false;
        private bool ChatBanLoaded = false;
        private bool WarnLoaded = false;
        private bool ReportLoaded = false;
        private bool RewardLogLoaded = false;
        public void CheckBanSchema()
        {
            try
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                {
                    Logger.Log("Checking Ban Table...");
                }
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + ThePunisher.Instance.Configuration.Instance.DatabaseTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    {
                        Logger.Log("Creating Ban Table...");
                    }
                    command.CommandText = "CREATE TABLE `" + ThePunisher.Instance.Configuration.Instance.DatabaseTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamId` varchar(32) NOT NULL,`admin` varchar(32) NOT NULL,`banMessage` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`banDuration` int NULL,`banTime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,`chatbanned` TEXT NULL,PRIMARY KEY (`id`));";
                    command.ExecuteNonQuery();
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    {
                        Logger.Log("Ban Table Successfully created...");
                    }
                }
                connection.Close();
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                {
                    Logger.Log("Ban Table Schema successfully validated...");
                }
                BanLoaded = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        public void CheckChatBanSchema()
        {
            try
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                {
                    Logger.Log("Checking chat ban table...");
                }
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + ThePunisher.Instance.Configuration.Instance.DatabaseChatbanTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    {
                        Logger.Log("Creating Chat Ban Table...");
                    }
                    command.CommandText = "CREATE TABLE `" + ThePunisher.Instance.Configuration.Instance.DatabaseChatbanTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamId` varchar(32) NOT NULL,`admin` varchar(32) NOT NULL,`banMessage` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`banDuration` int NULL,`banTime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`id`));";
                    command.ExecuteNonQuery();
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    {
                        Logger.Log("Chat Ban Table Successfully created...");
                    }
                }
                connection.Close();
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                {
                    Logger.Log("Chat Ban Table Schema successfully validated...");
                }
                ChatBanLoaded = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        public void CheckWarnSchema()
        {
            try
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                {
                    Logger.Log("Checking warning table...");
                }
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + ThePunisher.Instance.Configuration.Instance.DatabaseWarningTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    {
                        Logger.Log("Creating Warning Table...");
                    }
                    command.CommandText = "CREATE TABLE `" + ThePunisher.Instance.Configuration.Instance.DatabaseWarningTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamId` varchar(32) NOT NULL,`admin` varchar(32) NOT NULL,`warnMessage` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`warnTime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`id`));";
                    command.ExecuteNonQuery();
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    {
                        Logger.Log("Warning Table Successfully created...");
                    }
                }
                connection.Close();
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                {
                    Logger.Log("Warning Table Schema successfully validated...");
                }
                WarnLoaded = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void CheckReportSchema()
        {
            try
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                {
                    Logger.Log("Checking Report table...");
                }
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + ThePunisher.Instance.Configuration.Instance.DatabaseReportTableName +"'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    {
                        Logger.Log("Creating Report Table...");
                    }
                    command.CommandText = "CREATE TABLE `" + ThePunisher.Instance.Configuration.Instance.DatabaseReportTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamId` varchar(32) NOT NULL,`reportee` varchar(32) NOT NULL,`reportMessage` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`reportTime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,`rewarded` TEXT NULL DEFAULT NULL,PRIMARY KEY (`id`));";
                    command.ExecuteNonQuery();
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    {
                        Logger.Log("Report table Successfully created...");
                    }
                }
                connection.Close();
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                {
                    Logger.Log("Report Table Schema successfully validated...");
                }
                ReportLoaded = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        public void CheckRewardlogSchema()
        {
            try
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                {
                    Logger.Log("Checking Reward Log table...");
                }
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + ThePunisher.Instance.Configuration.Instance.DatabaseRewardlogTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    {
                        Logger.Log("Creating Reward Log Table...");
                    }
                    command.CommandText = "CREATE TABLE `" + ThePunisher.Instance.Configuration.Instance.DatabaseRewardlogTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamId` varchar(32) NOT NULL,`reportee` varchar(32) NOT NULL,`reportMessage` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`reportTime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,`rewarded` TEXT NULL DEFAULT NULL,PRIMARY KEY (`id`));";
                    command.ExecuteNonQuery();
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    {
                        Logger.Log("Reward log table Successfully created...");
                    }
                }
                connection.Close();
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                {
                    Logger.Log("Reward log Table Schema successfully validated...");
                }
                RewardLogLoaded = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void EndSchemaChecks()
        {
            if (!BanLoaded)
            {
                Logger.LogError("Ban table failed to initialize! Unloading now!");
                return;
            }
            if (!ChatBanLoaded)
            {
                Logger.LogError("Chat Ban table failed to initialize! Unloading now!");
                return;
            }
            if (!WarnLoaded)
            {
                Logger.LogError("Warning table failed to initialize! Unloading now!");
                return;
            }
            if (!ReportLoaded)
            {
                Logger.LogError("Reports table failed to initialize! Unloading now!");
                return;
            }
            if (!RewardLogLoaded)
            {
                Logger.LogError("Reward Log table failed to initialize! Unloading now!");
                return;
            }
            Logger.Log("All tables successfully validated and loaded!");
        }
        #endregion


        public void ChatBanPlayer(string characterName, string steamid, string admin, string banMessage, int duration)
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                if (banMessage == null) banMessage = "";
                command.Parameters.AddWithValue("@csteamid", steamid);
                command.Parameters.AddWithValue("@admin", admin);
                command.Parameters.AddWithValue("@charactername", characterName);
                command.Parameters.AddWithValue("@banMessage", banMessage);
                if (duration == 0)
                {
                    command.Parameters.AddWithValue("@banDuration", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@banDuration", duration);
                }
                command.CommandText = "insert into `" + ThePunisher.Instance.Configuration.Instance.DatabaseChatbanTableName + "` (`steamId`,`admin`,`banMessage`,`charactername`,`banTime`,`banDuration`) values(@csteamid,@admin,@banMessage,@charactername,now(),@banDuration);";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
        }

        public void BanPlayer(string characterName, string steamid, string admin, string banMessage, int duration)
        {
            try
            {

                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                if (banMessage == null) banMessage = "";
                command.Parameters.AddWithValue("@csteamid", steamid);
                command.Parameters.AddWithValue("@admin", admin);
                command.Parameters.AddWithValue("@charactername", characterName);
                command.Parameters.AddWithValue("@banMessage", banMessage);
                if (duration == 0)
                {
                    command.Parameters.AddWithValue("@banDuration", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@banDuration", duration);
                }
                command.CommandText = "insert into `" + ThePunisher.Instance.Configuration.Instance.DatabaseTableName + "` (`steamId`,`admin`,`banMessage`,`charactername`,`banTime`,`banDuration`) values(@csteamid,@admin,@banMessage,@charactername,now(),@banDuration);";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
        }

        public class UnbanResult {
            public ulong Id;
            public string Name;
        }

        public UnbanResult UnChatbanPlayer(string player)
        {
            try
            {
                MySqlConnection connection = createConnection();

                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@player", "%" + player + "%");
                command.CommandText = "select steamId,charactername from `" + ThePunisher.Instance.Configuration.Instance.DatabaseChatbanTableName + "` where `steamId` like @player or `charactername` like @player limit 1;";
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ulong steamId = reader.GetUInt64(0);
                    string charactername = reader.GetString(1);
                    connection.Close();
                    command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@steamId", steamId);
                    command.CommandText = "delete from `" + ThePunisher.Instance.Configuration.Instance.DatabaseChatbanTableName + "` where `steamId` = @steamId;";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    return new UnbanResult() { Id = steamId, Name = charactername };
                }
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
            return null;
        }

        public UnbanResult UnbanPlayer(string player)
        {
            try
            {
                MySqlConnection connection = createConnection();

                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@player", "%" + player + "%");
                command.CommandText = "select steamId,charactername from `" + ThePunisher.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` like @player or `charactername` like @player limit 1;";
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ulong steamId = reader.GetUInt64(0);
                    string charactername = reader.GetString(1);
                    connection.Close();
                    command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@steamId", steamId);
                    command.CommandText = "delete from `" + ThePunisher.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` = @steamId;";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    return new UnbanResult() { Id = steamId, Name = charactername };
                }
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
            return null;
        }
    }
}
