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
            //UnregisterRocketCommand<T>();
        }

        /*private static void UnregisterRocketCommand<T>() where T : IRocketCommand
        {
            var rocketComamnds = AccessorFactory.AccessField<List<IRocketCommand>>(
                R.Commands, "commands");

            rocketComamnds.Value.RemoveAll(cmd => cmd is T);
        }*/

        #region check for plugin dependancies
        public void CheckDependancies(bool firstrun)
        {
            if (IsDependencyLoaded("Uconomy"))
            {
                UconomyLoaded = true;
                if (Configuration.Instance.ShowDebugInfo)
                    Logger.Log("Optional dependency Uconomy is present. Monetary rewards enabled.");
            }
            else
            {
                if (Configuration.Instance.ShowDebugInfo)
                    Logger.Log("Optional dependency Uconomy is not present. Monetary rewards disabled.");
            }
            if (IsDependencyLoaded("Kits"))
            {
                KitsLoaded = true;
                if (Configuration.Instance.ShowDebugInfo)
                    Logger.Log("Optional dependency Kits is present. Kit rewards enabled.");
            }
            else
            {
                if (Configuration.Instance.ShowDebugInfo)
                    Logger.Log("Optional dependency Kits is not present. Kit rewards disabled.");
            }
            if (IsDependencyLoaded("LPX"))
            {
                LightLoaded = true;
                if (Configuration.Instance.ShowDebugInfo)
                    Logger.Log("Optional dependency LPX is present. Kit rewards enabled.");
            }
            else
            {
                if (Configuration.Instance.ShowDebugInfo)
                    Logger.Log("Optional dependency LPX is not present. Kit rewards disabled.");
            }  
        }
        #endregion
        public void UnturnedPlayerEvents_OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode, ref bool cancel)
        {
            string ChatBanned = Database.IsChatBanned(player.ToString());
            if (ChatBanned != null)
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
                    {"default_banmessage","you are banned, contact the staff if you feel this is a mistake."},
                    {"command_generic_invalid_parameter","Invalid parameter"},
                    {"command_generic_player_not_found","Player not found"},
                    {"command_ban_public_reason", "The player {0} was banned for: {1}"},
                    {"command_chatban_public_reason", "The player {0} was chat banned for: {1}"},
                    {"command_ban_public","The player {0} was banned"},
                    {"command_chatban_public","The player {0} was chat banned"},
                    {"command_ban_private_default_reason","you were banned from the server"},
                    {"command_chatban_private_default_reason","you were chat banned from the server"},
                    {"command_kick_public_reason", "The player {0} was kicked for: {1}"},
                    {"command_kick_public","The player {0} was kicked"},
                    {"command_kick_private_default_reason","you were kicked from the server"},
                    {"command_warn_public_reason", "The player {0} was warned for: {1}"},
                    {"command_warn_public", "The player {0} was warned."},
                    {"command_kill_public", "The player {0} was forced to kill himself."},
                    {"command_report_success", "Successfully reported player {0} for: {1}"},
                    {"command_reward_success", "{0} has been given {1} for reporting a player"},
                    {"command_reward_success_player", "You have been given {0} for reporting a player"},
                    {"command_reward_plugin_unloaded", "Depenancy {0} has not been found. {1} rewards are disabled."},
                    {"command_reward_item_failed", "Failed to give item {0} x{1} to player {2}."},
                    {"command_reward_failed", "Failed to give {0} to player {2}."},
                    {"command_generic_player_previously_awarded", "{0} was allready awarded for his/her report!"},
                    {"command_generic_player_has_not_reported", "{0} has not recently reported anyone!"},
                    {"player_allready_reported", "You have allready reported {0}!"},
                    {"player_is_you", "You cannot report yourself!"},
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
            CheckDependancies(false);
        }
    }
}
