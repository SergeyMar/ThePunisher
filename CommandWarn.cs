using System;
using System.Collections.Generic;
using System.Linq;

using Rocket.API;
using Rocket.Core;
using Rocket.Core.Permissions;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Events;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Player;

using UnityEngine;
using SDG;
using SDG.Unturned;
using Steamworks;
using Rocket.Core.Logging;

namespace rawrfuls.ThePunisher
{
    public class CommandWarn : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Both; }
        }

        public string Name
        {
            get
            {
                return "warn";
            }
        }
        public string Help
        {
            get
            {
                return "Warns a player for breaking the rules.";
            }
        }
        public string Syntax
        {
            get
            {
                return "<name> \"[reason]\" [amt]";
            }
        }
        public List<string> Aliases
        {
            get { return new List<string>(); }
        }
        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "warnings.others" };
            }
        }

        public void Execute(IRocketPlayer playerid, string[] msg)
        {
            if (playerid == null) return;
            if (msg.Length <= 0 || msg.Length > 3)
            {
                UnturnedChat.Say(playerid, ThePunisher.Instance.Translate("warn_command_usage", new object[] { }));
                return;
            }
            UnturnedPlayer warnee = UnturnedPlayer.FromName(msg[0]);
            if (playerid.ToString() == warnee.CSteamID.ToString())
            {
                UnturnedChat.Say(playerid, ThePunisher.Instance.Translate("warn_player_is_you"));
                return;
            }
            if (warnee == null)
            {
                UnturnedChat.Say(playerid, ThePunisher.Instance.Translate("invalid_name_provided", new object[] { }));
                return;
            }
            if (warnee.CharacterName == playerid.ToString())
            {
                UnturnedChat.Say(playerid, ThePunisher.Instance.Translate("not_warn_yourself", new object[] { }));
                return;
            }
            string reason = "";
            if (msg.Length >= 2)
            {
                reason = msg[1];
            }
            short amt = 1;
            if (msg.Length == 3)
            {
                short.TryParse(msg[2], out amt);
            }
            // Check their current warning level.
            byte currentlevel = ThePunisher.Instance.Database.GetWarnings(warnee.CSteamID);
            if (amt < 0)
            {
                if (currentlevel + amt < 0) amt = (short)currentlevel;
                if (!ThePunisher.Instance.Database.EditWarning(warnee.CSteamID, amt))
                {
                    UnturnedChat.Say(playerid, ThePunisher.Instance.Translate("error_warning_player", new object[] { warnee.CharacterName }));
                    return;
                }
                else
                {
                    UnturnedChat.Say(playerid, ThePunisher.Instance.Translate("warn_reduced_warner_msg", new object[] {
                        warnee.CharacterName,
                        amt.ToString(),
                        ((short)currentlevel + amt).ToString()
                    }));
                    return;
                }
            }
            if ((((short)currentlevel + amt) >= ThePunisher.Instance.Configuration.Instance.WarningstoBan) && ThePunisher.Instance.Configuration.Instance.WarningtoBanOn)
            {
                if (!ThePunisher.Instance.Database.EditWarning(warnee.CSteamID, amt))
                {
                    UnturnedChat.Say(playerid, ThePunisher.Instance.Translate("error_warning_player", new object[] { warnee.CharacterName }));
                    return;
                }
                else
                {
                    UnturnedChat.Say(ThePunisher.Instance.Translate("warn_ban_public_msg", warnee.CharacterName, playerid.DisplayName, reason, ((short)currentlevel + amt).ToString()));
                    int duration = 0;
                    if (ThePunisher.Instance.Configuration.Instance.AutoBanDuration != null)
                    {
                        int.TryParse(ThePunisher.Instance.Database.convertTimeToSeconds(ThePunisher.Instance.Configuration.Instance.AutoBanDuration), out duration);
                    }
                    if (reason == "" || ThePunisher.Instance.Database.convertTimeToSeconds(reason) == reason || reason == null)
                    {
                        if (ThePunisher.Instance.Configuration.Instance.AutoBanDuration != null)
                        {
                            reason = ThePunisher.Instance.Translate("warn_ban_reason", ((short)currentlevel + amt).ToString(), ThePunisher.Instance.Configuration.Instance.AutoBanDuration);
                        }
                    }
                    ThePunisher.Instance.Database.BanPlayer(warnee.CharacterName, warnee.CSteamID.ToString(), playerid.ToString(), playerid.DisplayName, reason, duration);
                    warnee.Kick(reason);
                    return;
                }
            }
            else if ((((short)currentlevel + amt) >= ThePunisher.Instance.Configuration.Instance.WarningstoKick) && ThePunisher.Instance.Configuration.Instance.WarningtoKickOn)
            {
                if (!ThePunisher.Instance.Database.EditWarning(warnee.CSteamID, amt))
                {
                    UnturnedChat.Say(playerid, ThePunisher.Instance.Translate("error_warning_player", new object[] { warnee.CharacterName }));
                    return;
                }
                else
                {
                    UnturnedChat.Say(ThePunisher.Instance.Translate("warn_kick_public_msg", warnee.CharacterName, playerid.ToString(), reason, ((short)currentlevel + amt).ToString()));
                    if (reason == "")
                    {
                        reason = ThePunisher.Instance.Translate("warn_kick_reason", ((short)currentlevel + amt).ToString());
                    }
                    warnee.Kick(reason);
                    return;
                }
            }
            else
            {
                if (!ThePunisher.Instance.Database.EditWarning(warnee.CSteamID, amt))
                {
                    UnturnedChat.Say(playerid, ThePunisher.Instance.Translate("error_warning_player", new object[] { warnee.CharacterName }));
                    return;
                }
                else
                {
                    UnturnedChat.Say(ThePunisher.Instance.Translate("warn_msg", new object[] {
                        warnee.CharacterName,
                        playerid.ToString(),
                        reason,
                        ((short)currentlevel + amt).ToString()
                    }));
                    return;
                }
            }
        }
    }
}