using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace rawrfuls.ThePunisher
{
    public class CommandBlacklist : IRocketCommand
    {
        public string Help
        {
            get { return "Removes a player from the whitelist"; }
        }

        public string Name
        {
            get { return "blacklist"; }
        }

        public string Syntax
        {
            get { return "<player>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Both; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "thepunisher.blacklist" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (!ThePunisher.Instance.Configuration.Instance.WhiteListEnabled)
            {
                UnturnedChat.Say(caller, ThePunisher.Instance.Translate("whitelist_disabled_error"), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PrivateMessageColor));
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log(ThePunisher.Instance.Translate("whitelist_disabled_error")); }
                return;
            }
            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_invalid_parameter"), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PrivateMessageColor));
                return;
            }
            UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
            if (ThePunisher.Instance.Database.IsWhitelisted(otherPlayer.CSteamID.ToString()) == null)
            {
                UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_player_not_found"), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PrivateMessageColor));
                return;
            }
            else
            {
                rawrfuls.ThePunisher.DatabaseManager.BlacklistResult name = ThePunisher.Instance.Database.BlacklistPlayer(command[0]);
                UnturnedChat.Say(ThePunisher.Instance.Translate("command_whitelist_remove_public", command[0]), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
                try
                {
                    otherPlayer.Kick("You have been removed from the whitelist!");
                }
                catch (Exception)
                {

                }
            }
        }

    }
}
