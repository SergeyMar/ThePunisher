using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Commands;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace rawrfuls.ThePunisher
{
    public class ThePunisher : RocketPlugin<ThePunisherConfiguration>
    {
        public static ThePunisher Instance;
        public DatabaseManager Database;
        public bool UconomyLoaded = false;
        public bool KitsLoaded = false;
        public bool LightLoaded = false;
        public static Dictionary<CSteamID, string> Players = new Dictionary<CSteamID, string>();

        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseManager();
            CheckDependancies(true);
            UnturnedPermissions.OnJoinRequested += Events_OnJoinRequested;
            U.Events.OnPlayerConnected += RocketServerEvents_OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerChatted += UnturnedPlayerEvents_OnPlayerChatted;
        }

        #region check for plugin dependancies
        public void CheckDependancies(bool firstrun)
        {
            if (IsDependencyLoaded("Uconomy"))
            {
                UconomyLoaded = true;
                if (Configuration.Instance.ShowDebugInfo && !firstrun)
                    Logger.Log("Optional dependency Uconomy is present. Monetary rewards enabled.");
            }
            else
            {
                if (Configuration.Instance.ShowDebugInfo && !firstrun)
                    Logger.Log("Optional dependency Uconomy is not present. Monetary rewards disabled.");
            }
            if (IsDependencyLoaded("Kits"))
            {
                KitsLoaded = true;
                if (Configuration.Instance.ShowDebugInfo && !firstrun)
                    Logger.Log("Optional dependency Kits is present. Kit rewards enabled.");
            }
            else
            {
                if (Configuration.Instance.ShowDebugInfo && !firstrun)
                    Logger.Log("Optional dependency Kits is not present. Kit rewards disabled.");
            }
            if (IsDependencyLoaded("LPX"))
            {
                LightLoaded = true;
                if (Configuration.Instance.ShowDebugInfo && !firstrun)
                    Logger.Log("Optional dependency LPX is present. Kit rewards enabled.");
            }
            else
            {
                if (Configuration.Instance.ShowDebugInfo && !firstrun)
                    Logger.Log("Optional dependency LPX is not present. Kit rewards disabled.");
            }  
        }
        #endregion

        public void UnturnedPlayerEvents_OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode, ref bool cancel)
        {
            if (Database.IsChatBanned(player.ToString()) != null)
            {
                cancel = true;
            }
        }

        protected override void Unload()
        {
            UnturnedPermissions.OnJoinRequested -= Events_OnJoinRequested;
            U.Events.OnPlayerConnected -= RocketServerEvents_OnPlayerConnected;
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList() {
                    /*GlobalBan stuff*/
                    {"default_banmessage","you are banned, contact the staff if you feel this is a mistake."},
                    {"command_generic_invalid_parameter","Invalid parameter"},
                    {"command_generic_player_not_found","Player not found"},
                    {"command_ban_public_reason", "The player {0} was banned for: {1}"},
                    {"command_ban_public","The player {0} was banned"},
                    {"ban_player_is_you", "You cannot ban yourself!"},
                    {"command_unban_public", "The player {0} was unbanned!"},
                    {"command_ban_private_default_reason","you were banned from the server"},
                    {"command_kick_public_reason", "The player {0} was kicked for: {1}"},
                    {"command_kick_public","The player {0} was kicked"},
                    {"command_kick_private_default_reason","you were kicked from the server"},
                    /*mute stuff*/
                    {"command_mute_public_reason", "The player {0} was muted for: {1}"},
                    {"command_mute_public","The player {0} was muted."},
                    {"command_mute_private_default_reason","You were muted from the server for {1}"},
                    {"command_mute_private_default","You were muted from the server."},
                    /*unmute stuff*/
                    {"command_unmute_public","The player {0} was unmuted."},
                    /*Warning Stuff*/
                    /*{"command_warn_public_reason", "The player {0} was warned for: {1}"},
                    {"command_warn_public", "The player {0} was warned."},*/
                    {"warn_command_usage","/warn <name> \"[reason]\" [amt] Amt can be negative to reduce warning level."},
                    {"invalid_name_provided","An invalid player name was provided."},
                    {"not_warn_yourself","You cannot warn yourself!"},
                    {"error_warning_player", "There was an error warning {0}." },
                    {"warn_msg", "{0} was warned by {1} for {2}. This is warning {3}." },
                    {"warn_kick_public_msg", "{0} was warned by {1} for {2}.  This is warning {3} and {0} has been kicked." },
                    {"warn_kick_reason", "This is warning {0}, so you have been kicked." },
                    {"warn_ban_reason", "This is warning {0}, so you have been banned for {1}." },
                    {"warn_ban", "This is warning {0}, so have been banned." },
                    {"warn_ban_public_msg", "{0} was warned by {1} for {2}.  This is warning {3} and {0} has been banned." },
                    {"warn_reduced_warner_msg","You have reduced {0}'s warning level by {1}.  It is now {2}."},
                    {"warnings_command_usage","/warnings [name] Will display yours or someone else's warning level."},
                    {"warnings_no_permission_others","You do not have permission to view others warning level."},
                    {"warnings_current_level","Your current warnings level is {0}."},
                    { "warnings_current_level_others", "{0}'s current warnings level is {1}."},
                    {"warn_player_is_you", "You cannot warn yourself!"},
                    /*Kill stuff*/
                    {"command_kill_public", "The player {0} was forced to kill himself."},
                    /*reporting stuff*/
                    {"command_report_success", "Successfully reported player {0} for: {1}"},
                    {"player_allready_reported", "You have allready reported {0}!"},
                    /*reward stuff*/
                    {"command_reward_success", "{0} has been given {1} for reporting a player"},
                    {"command_reward_success_player", "You have been given {0} for reporting a player"},
                    {"command_reward_car_success_player", "You have been given a vehicle for reporting a player"},
                    {"command_reward_item_failed", "Failed to give item {0} x{1} to player {2}."},
                    {"command_reward_failed", "Failed to give {0} to player {2}."},
                    {"command_generic_player_previously_awarded", "{0} was allready awarded for his/her report!"},
                    {"command_generic_player_has_not_reported", "{0} has not recently reported anyone!"},
                    /*report removal */
                    {"command_remove_report_success", "The report for {0} was successfully removed!"},
                    {"command_remove_report_fail", "There was an error removing the report for {0}!"},
                    /*report checking*/
                    {"command_get_report", "{0} has been reported by {1} for {2}."},
                    {"command_pagination", "Showing {0} to {1} of {2} reports. Page {3} of {4}"},
                    /*whitelist stuff*/
                    {"whitelist_deny_connection", "This server is running in Whitelist Mode. Please contact the owner for access."},
                    {"command_whitelist_public", "{0} has been whitelisted!"},
                    {"command_whitelist_remove_public", "{0} has been removed from the whitelist!"},
                    {"whitelist_disabled_error", "Whitelist mode is not enabled, therefore this command will not work!"},

                };
            }
        }

        public static KeyValuePair<CSteamID, string> GetPlayer(string search)
        {
            foreach (KeyValuePair<CSteamID, string> pair in Players)
            {
                if (pair.Key.ToString().ToLower().Contains(search.ToLower()) || pair.Value.ToLower().Contains(search.ToLower()))
                {
                    return pair;
                }
            }
            return new KeyValuePair<CSteamID, string>(new CSteamID(0), null);
        }

        void RocketServerEvents_OnPlayerConnected(UnturnedPlayer player)
        {
            if (!Players.ContainsKey(player.CSteamID))
                Players.Add(player.CSteamID, player.CharacterName);
        }

        public void Events_OnJoinRequested(CSteamID player, ref ESteamRejection? rejection)
        {
            if (Configuration.Instance.WhiteListEnabled)
            {
                try
                {
                    string whitelisted = Database.IsWhitelisted(player.ToString());
                    if (whitelisted == null)
                    {
                        string banned = Translate("whitelist_deny_connection");
                        rejection = ESteamRejection.AUTH_PUB_BAN;
                        //Provider.kick(player, ThePunisher.Instance.Translate("whitelist_deny_connection")); //doesn't work on player connect?
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                try
                {
                    string banned = Database.IsBanned(player.ToString());
                    if (banned != null)
                    {
                        if (banned == "") banned = Translate("default_banmessage");
                        rejection = ESteamRejection.AUTH_PUB_BAN;
                    }
                }
                catch (Exception)
                {

                }
            }
            CheckDependancies(false);
        }

        public UnityEngine.Color? getColor(string color)
        {
            UnityEngine.Color? retcolor = Color.green;
            try
            {
                retcolor = UnturnedChat.GetColorFromName(color, Color.green);
            }
            catch (Exception ex)
            {
                try
                {
                    retcolor = UnturnedChat.GetColorFromHex(color);
                }
                catch (Exception err)
                {
                    
                }
            }
            return retcolor;
        }
    }
}
