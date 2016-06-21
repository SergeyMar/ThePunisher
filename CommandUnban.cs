﻿using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;

namespace rawrfuls.ThePunisher
{
    public class CommandUnban : IRocketCommand
    {
        public string Help
        {
            get { return "Unbans a player"; }
        }

        public string Name
        {
            get { return "unban"; }
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
                return new List<string>() { "thepunisher.unban" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }

            rawrfuls.ThePunisher.DatabaseManager.UnbanResult name = ThePunisher.Instance.Database.UnbanPlayer(command[0]);
            if (!SteamBlacklist.unban(new CSteamID(name.Id)) && String.IsNullOrEmpty(name.Name))
            {
                UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_player_not_found"));
                return;
            }
            else
            {
                UnturnedChat.Say("The player " + name.Name + " was unbanned");
            }
        }

    }
}
