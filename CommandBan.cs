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
using System;

namespace rawrfuls.ThePunisher
{
    public class CommandBan : IRocketCommand
    {
        public string Help
        {
            get { return  "Bans a player"; }
        }

        public string Name
        {
            get { return "ban"; }
        }

        public string Syntax
        {
            get { return "<player> [reason] [duration]"; }
        }

        public List<string> Aliases {
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
                return new List<string>() { "thepunisher.ban" };
            }
        }
         
        public void Execute(IRocketPlayer caller, params string[] command)
        {
            try
            {
                if (command.Length == 0 || command.Length > 3)
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

                string adminId = "Console";
                string adminName = "Console";
                if (caller != null)
                {
                    adminId = caller.ToString();
                    adminName = caller.DisplayName;
                }
                if (adminId == otherPlayer.CSteamID.ToString())
                {
                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("ban_player_is_you"));
                    return;
                }
                if (command.Length == 3)
                {
                    int duration = 0;
                    if (int.TryParse(ThePunisher.Instance.Database.convertTimeToSeconds(command[2]).ToString(), out duration))
                    {
                        ThePunisher.Instance.Database.logItem(duration.ToString());
                        ThePunisher.Instance.Database.BanPlayer(charactername, steamid.ToString(), adminId, adminName, command[1], duration);
                        if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Player Successfully Banned."); }
                        if (ThePunisher.Instance.Configuration.Instance.DisplayBanMessagePublic)
                            UnturnedChat.Say(ThePunisher.Instance.Translate("command_ban_public_reason", charactername, command[1]), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
                        if (isOnline)
                        {
                            otherPlayer.Kick(command[1]);
                            if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Banned player has also been kicked."); }
                        }

                    }
                    else
                    {
                        UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_invalid_parameter"), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PrivateMessageColor));
                        return;
                    }
                }
                else if (command.Length == 2)
                {
                    int duration = 0;

                    if (ThePunisher.Instance.Database.convertTimeToSeconds(command[1]) != null)
                    {
                        int.TryParse(ThePunisher.Instance.Database.convertTimeToSeconds(command[1]).ToString(), out duration);
                    }
                    else
                    {
                        if (ThePunisher.Instance.Configuration.Instance.DefaultBanDuration != null)
                        {
                            if (ThePunisher.Instance.Database.convertTimeToSeconds(ThePunisher.Instance.Configuration.Instance.DefaultBanDuration) != null)
                                int.TryParse(ThePunisher.Instance.Database.convertTimeToSeconds(ThePunisher.Instance.Configuration.Instance.DefaultBanDuration).ToString(), out duration);
                        }
                    }
                    string reason = "";
                    if (ThePunisher.Instance.Database.convertTimeToSeconds(command[1]) == null)
                    {
                        reason = command[1];
                    }
                    ThePunisher.Instance.Database.BanPlayer(charactername, steamid.ToString(), adminId, adminName, reason, duration);
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Player successfully Banned."); }
                    if (ThePunisher.Instance.Configuration.Instance.DisplayBanMessagePublic)
                        UnturnedChat.Say(ThePunisher.Instance.Translate("command_ban_public_reason", charactername, command[1]), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
                    if (isOnline)
                    {
                        otherPlayer.Kick(command[1]);
                        if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Banned player has also been kicked."); }
                    }
                }
                else
                {
                    int duration = 0;
                    if (ThePunisher.Instance.Database.convertTimeToSeconds(ThePunisher.Instance.Configuration.Instance.DefaultBanDuration) != null)
                        int.TryParse(ThePunisher.Instance.Database.convertTimeToSeconds(ThePunisher.Instance.Configuration.Instance.DefaultBanDuration).ToString(), out duration);
                    ThePunisher.Instance.Database.BanPlayer(charactername, steamid.ToString(), adminId, adminName, "", duration);
                    if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Player successfully banned."); }
                    if (ThePunisher.Instance.Configuration.Instance.DisplayBanMessagePublic)
                        UnturnedChat.Say(ThePunisher.Instance.Translate("command_ban_public", charactername), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
                    if (isOnline)
                    {
                        otherPlayer.Kick(ThePunisher.Instance.Translate("command_ban_private_default_reason"));
                        if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Banned player has also been kicked!"); }
                    }
                }

            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
