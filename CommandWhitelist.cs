using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Rocket.Unturned.Commands;
using SDG;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using Rocket.Core.Steam;
using UnityEngine;

namespace rawrfuls.ThePunisher
{
    public class CommandWhitelist : IRocketCommand
    {
        public string Help
        {
            get { return "Whitelists a player"; }
        }

        public string Name
        {
            get { return "whitelist"; }
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
                return new List<string>() { "thepunisher.whitelist" };
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
            try
            {
                if (command.Length > 1)
                {
                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_invalid_parameter"), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PrivateMessageColor));
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log(ThePunisher.Instance.Translate("command_generic_invalid_parameter")); }
                    return;
                }

                bool isOnline = false;

                CSteamID steamid;
                string charactername = null;


                UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
                ulong? otherPlayerID = command.GetCSteamIDParameter(0);
                if (otherPlayer == null || otherPlayer.CSteamID.ToString() == "0" || caller != null && otherPlayer.CSteamID.ToString() == caller.Id)
                {
                    KeyValuePair<CSteamID, string> player = ThePunisher.GetPlayer(command[0]);
                    if (player.Key.ToString() != "0")
                    {
                        steamid = player.Key;
                        charactername = player.Value;
                    }
                    else
                    {
                        if (otherPlayerID != null)
                        {
                            steamid = new CSteamID(otherPlayerID.Value);
                            Profile playerProfile = new Profile(otherPlayerID.Value);
                            charactername = playerProfile.SteamID;
                        }
                        else
                        {
                            UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_player_not_found"), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PrivateMessageColor));
                            return;
                        }
                    }
                }
                else
                {
                    isOnline = true;
                    steamid = otherPlayer.CSteamID;
                    charactername = otherPlayer.CharacterName;
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Found steam ID and player name of player to ban."); }
                }

                string adminName = "Console";
                if (caller != null) adminName = caller.ToString();
                ThePunisher.Instance.Database.WhiteListPlayer(charactername, steamid.ToString(), adminName);
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Player successfully whitelisted."); }
                UnturnedChat.Say(ThePunisher.Instance.Translate("command_whitelist_public", charactername), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
