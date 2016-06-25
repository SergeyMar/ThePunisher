using Rocket.API;
using SDG.Unturned;
using System.Collections.Generic;
using System;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;

namespace rawrfuls.ThePunisher
{
    public class CommandKill : IRocketCommand
    {
        public string Help
        {
            get { return "Kills a player"; }
        }

        public string Name
        {
            get { return "kill"; }
        }

        public string Syntax
        {
            get { return "<player> [silent]"; }
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
                return new List<string>() { "thepunisher.kill" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {

            if (command.Length == 0 || command.Length > 2)
            {
                UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_invalid_parameter"), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PrivateMessageColor));
                return;
            }
            UnturnedPlayer playerToKill = UnturnedPlayer.FromName(command[0]);
            if (playerToKill == null)
            {
                UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_player_not_found"), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PrivateMessageColor));
                return;
            }
            if (command.Length == 2)
            {
                if (command[1] == "silent")
                {
                    /*playerToKill.Thirst = 1;
                    playerToKill.Hunger = 1;
                    playerToKill.Bleeding = true;
                    playerToKill.Broken = true;*/
                    UnturnedChat.Say(caller, "The silent switch is still a work in progress. Currently only force suicide works, however we won't display the forced suicide message.", (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PrivateMessageColor));
                    //UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_kill_public", playerToKill.SteamName), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
                    playerToKill.Suicide();
                }
                else
                {
                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_kill_public", playerToKill.SteamName), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
                    playerToKill.Suicide();
                }
            }
            else
            {
                UnturnedChat.Say(ThePunisher.Instance.Translate("command_kill_public", playerToKill.SteamName), (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PublicMessageColor));
                playerToKill.Suicide();
            }
        }
    }
}
