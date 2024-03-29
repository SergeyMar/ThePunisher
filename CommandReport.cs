﻿using Rocket.API;
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
    public class CommandReport : IRocketCommand
    {
        public string Help
        {
            get { return  "Reports a player"; }
        }

        public string Name
        {
            get { return "report"; }
        }

        public string Syntax
        {
            get { return "<player> [reason]"; }
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
                return new List<string>() { "thepunisher.report" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            try
            {
                if (command.Length == 0 || command.Length > 2)
                {
                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_invalid_parameter"), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
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
                            UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_player_not_found"), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
                            return;
                        }
                    }
                }
                else
                {
                    isOnline = true;
                    steamid = otherPlayer.CSteamID;
                    charactername = otherPlayer.CharacterName;
                }
                if (caller.ToString() == steamid.ToString())
                {
                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("player_is_you"), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
                    return;
                }
                if (ThePunisher.Instance.Database.HasReported(steamid.ToString(), caller.ToString()) != null)
                {
                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("player_allready_reported", charactername), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
                    return;
                }
                string adminName = "Console";
                if (caller != null) adminName = caller.ToString();

                if (command.Length == 2)
                {
                    ThePunisher.Instance.Database.ReportPlayer(charactername, steamid.ToString(), adminName, command[1]);
                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_report_success", charactername, command[1]), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
                }
                else
                {
                    ThePunisher.Instance.Database.ReportPlayer(charactername, steamid.ToString(), adminName, "");
                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_report_success", charactername, command[1]), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
                }

            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
