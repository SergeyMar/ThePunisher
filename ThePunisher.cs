﻿using Rocket.API;
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

        public static Dictionary<CSteamID, string> Players = new Dictionary<CSteamID, string>();

        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseManager();
            UnturnedPermissions.OnJoinRequested += Events_OnJoinRequested;
            U.Events.OnPlayerConnected += RocketServerEvents_OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerChatted += UnturnedPlayerEvents_OnPlayerChatted;
        }

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
                    {"command_report_success", "Successfully reported player {0} for: {1}"},
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
        }
    }
}
