using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace rawrfuls.ThePunisher
{
    public class CommandUnmute : IRocketCommand
    {
        public string Help
        {
            get { return "Unmutes a player from chat"; }
        }

        public string Name
        {
            get { return "unmute"; }
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
                return new List<string>() { "thepunisher.unmute" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_invalid_parameter"), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PrivateMessageColor));
                return;
            }

            rawrfuls.ThePunisher.DatabaseManager.UnbanResult name = ThePunisher.Instance.Database.UnChatbanPlayer(command[0]);
            if (ThePunisher.Instance.Database.IsChatBanned(name.Id.ToString()) == null && String.IsNullOrEmpty(name.Name))
            {
                UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_player_not_found"), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PrivateMessageColor));
                return;
            }
            else
            {
                UnturnedChat.Say(ThePunisher.Instance.Translate("command_unmute_public", name.Name), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
            }
        }

    }
}
