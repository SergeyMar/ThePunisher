using MySql.Data.MySqlClient;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
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
            CheckWhitelistSchema();
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

        public int getTotalRows(string table)
        {
            MySqlConnection connection = createConnection();
            using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM `" + table + "`", connection))
            {
                connection.Open();
                object totalrows = cmd.ExecuteScalar();
                connection.Close();
                return Convert.ToInt32(totalrows);
            }
        }

        public void GetReports(IRocketPlayer caller, int page)
        {
            try
            {
                int maxPerPage = 2;
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Set max items per page to " + maxPerPage.ToString()); }
                int total_rows = getTotalRows(ThePunisher.Instance.Configuration.Instance.DatabaseReportTableName);
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Got total rows in db:" + total_rows.ToString()); }
                int offset = 0;
                if (page > 0) { offset = maxPerPage * page; }
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Set offset to: " + offset.ToString()); }
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Successfully created mysql connection"); }
                command.Parameters.AddWithValue("@maxPerPage", maxPerPage);
                command.Parameters.AddWithValue("@page", page);
                command.CommandText = "select * from `" + ThePunisher.Instance.Configuration.Instance.DatabaseReportTableName + "` LIMIT " + offset + "," + maxPerPage + ";";
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Created MySql Query"); }
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Executed MySql Query"); }
                if (reader.HasRows)
                {
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Reader object opened"); }
                    while (reader.Read())
                    {
                        Logger.Log(reader.GetUInt64(1).ToString());
                        Logger.Log(reader.GetString(2));
                        Logger.Log(reader.GetString(3));
                        Logger.Log(reader.GetString(4));
                        Logger.Log(reader.GetString(5));
                        UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_invalid_parameter"));

                    }
                    /*command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@steamId", steamId);
                    command.Parameters.AddWithValue("@reportee", reportee);
                    command.Parameters.AddWithValue("@reportMessage", reportMessage);
                    command.Parameters.AddWithValue("@characterName", characterName);
                    command.Parameters.AddWithValue("@reportTime", reportTime);
                    command.Parameters.AddWithValue("@rewarded", rewarded);
                    command.CommandText = "INSERT INTO `" + ThePunisher.Instance.Configuration.Instance.DatabaseRewardlogTableName + "` (`steamId`, `reportee`, `reportMessage`, `charactername`, `reportTime`, `rewarded`) VALUES (@steamId, @reportee, @reportMessage, @characterName, @reportTime, @rewarded);";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    /*command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@steamId", steamId);
                    command.Parameters.AddWithValue("@reportee", reportee);
                    command.CommandText = "delete from `" + ThePunisher.Instance.Configuration.Instance.DatabaseReportTableName + "` where `steamId` = @steamId AND `reportee`= @reportee;";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();*/
                }
                else
                {
                    Logger.Log("No rows found.");
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
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

        public string IsWhitelisted(string steamId)
        {
            string output = null;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `characterName` from `" + ThePunisher.Instance.Configuration.Instance.DatabaseWhiteListTableName + "` where `steamId` = '" + steamId + "';";
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
        private bool WhitelistLoaded = false;
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
        public void CheckWhitelistSchema()
        {
            if (!ThePunisher.Instance.Configuration.Instance.WhiteListEnabled)
                return;

            try
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                {
                    Logger.Log("Checking Whitelist Table...");
                }
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + ThePunisher.Instance.Configuration.Instance.DatabaseWhiteListTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    {
                        Logger.Log("Creating Whitelist Table...");
                    }
                    command.CommandText = "CREATE TABLE `" + ThePunisher.Instance.Configuration.Instance.DatabaseWhiteListTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamId` varchar(32) NOT NULL,`admin` varchar(32) NOT NULL,`charactername` varchar(255) DEFAULT NULL,`added_on` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,`chatbanned` TEXT NULL,PRIMARY KEY (`id`));";
                    command.ExecuteNonQuery();
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    {
                        Logger.Log("Whitelist Table Successfully created...");
                    }
                }
                connection.Close();
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                {
                    Logger.Log("Whitelist Table Schema successfully validated...");
                }
                WhitelistLoaded = true;
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
            if (!WhitelistLoaded && ThePunisher.Instance.Configuration.Instance.WhiteListEnabled)
            {
                Logger.LogError("Whitelist table failed to initialize! Unloading now!");
                return;
            }
            Logger.Log("All tables successfully validated and loaded!");
        }
        #endregion

        public void WhiteListPlayer(string characterName, string steamid, string admin)
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@csteamid", steamid);
                command.Parameters.AddWithValue("@admin", admin);
                command.Parameters.AddWithValue("@charactername", characterName);
                command.CommandText = "insert into `" + ThePunisher.Instance.Configuration.Instance.DatabaseWhiteListTableName + "` (`steamId`,`admin`,`charactername`,added_on) values(@csteamid,@admin,@charactername,now());";
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
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Connection to Ban table created!"); }
                if (banMessage == null) banMessage = "";
                command.Parameters.AddWithValue("@csteamid", steamid);
                command.Parameters.AddWithValue("@admin", admin);
                command.Parameters.AddWithValue("@charactername", characterName);
                command.Parameters.AddWithValue("@banMessage", banMessage);
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Command paramaters added!"); }
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Banning: " + steamid + " - " + characterName + " from the server."); }
                if (duration == 0)
                {
                    command.Parameters.AddWithValue("@banDuration", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@banDuration", duration);
                }
                string query = "insert into `" + ThePunisher.Instance.Configuration.Instance.DatabaseTableName + "` (`steamId`,`admin`,`banMessage`,`charactername`,`banTime`,`banDuration`) values(@csteamid,@admin,@banMessage,@charactername,now(),@banDuration);";
                command.CommandText = query;
                
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Query Created: " + query); }
                connection.Open();
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Connection to database opened"); }
                command.ExecuteNonQuery();
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Command executed Successfully. Player has been banned."); }
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

        public class BlacklistResult
        {
            public ulong Id;
            public string Name;
        }

        public BlacklistResult BlacklistPlayer(string player)
        {
            try
            {
                MySqlConnection connection = createConnection();

                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@player", "%" + player + "%");
                command.CommandText = "select steamId,charactername from `" + ThePunisher.Instance.Configuration.Instance.DatabaseWhiteListTableName + "` where `steamId` like @player or `charactername` like @player limit 1;";
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ulong steamId = reader.GetUInt64(0);
                    string charactername = reader.GetString(1);
                    connection.Close();
                    command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@steamId", steamId);
                    command.CommandText = "delete from `" + ThePunisher.Instance.Configuration.Instance.DatabaseWhiteListTableName + "` where `steamId` = @steamId;";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    return new BlacklistResult() { Id = steamId, Name = charactername };
                }
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
            return null;
        }

        public class ReportDeleteResult
        {
            public ulong Id;
            public string Reporter;
            public string Reportee;
        }

        public ReportDeleteResult RemoveReport(string id = null, string player = null, string reporter = null)
        {
            if (id == null && player == null && reporter == null)
            {
                return null;
            }
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                if (String.IsNullOrEmpty(player))
                { }
                command.Parameters.AddWithValue("@id", "%" + id + "%");
                command.Parameters.AddWithValue("@player", "%" + player + "%");
                command.Parameters.AddWithValue("@reportee", "%" + reporter + "%");
                string cmd = "select steamId,charactername,reportee from `" + ThePunisher.Instance.Configuration.Instance.DatabaseReportTableName + "` where ";
                if (id != null)
                {
                    cmd = cmd + "`id` like @id ";
                }
                if (player != null)
                {
                    if (id != null)
                    {
                        cmd = cmd + " OR";
                    }
                    cmd = cmd + " ";
                }
                if (reporter != null)
                {
                    if (player != null)
                    {
                        cmd = cmd + " OR";
                    }
                    cmd = cmd + " `reportee` like @reporter";
                }
                cmd = cmd + " limit 1;";
                command.CommandText = cmd;
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ulong steamId = reader.GetUInt64(0);
                    string charactername = reader.GetString(1);
                    string reportee = reader.GetString(2);
                    connection.Close();
                    command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@steamId", steamId);
                    command.Parameters.AddWithValue("@reportee", reportee);
                    command.CommandText = "delete from `" + ThePunisher.Instance.Configuration.Instance.DatabaseReportTableName + "` where `steamId` = @steamId AND `reportee` = @reportee;";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    return new ReportDeleteResult() { Id = steamId, Reporter = charactername, Reportee = reportee };
                }
            }
            catch (Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo)
                    Logger.LogException(ex);
            }
            return null;
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
