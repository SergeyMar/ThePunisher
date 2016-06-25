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
using System;
using UnityEngine;

namespace rawrfuls.ThePunisher
{
    public class CommandReports : IRocketCommand
    {
        public string Help
        {
            get { return  "Gets all recent player reports"; }
        }

        public string Name
        {
            get { return "reports"; }
        }

        public string Syntax
        {
            get { return ""; }
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
                return new List<string>() { "thepunisher.reports" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            try
            {
                //if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log(""); }
                /*if (command.Length > 1)
                {
                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_invalid_parameter"));
                    return;
                }
                string adminName = "Console";
                if (caller != null) adminName = caller.ToString();
                if (command.Length == 0)
                {
                    ThePunisher.Instance.Database.GetReports(caller, 0);
                    return;
                }
                else if (command.Length == 1)
                {
                    ThePunisher.Instance.Database.GetReports(caller, Convert.ToInt32(command[0]));
                    return;
                }
                else
                {
                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_invalid_parameter"));
                    return;
                }*/
                UnturnedChat.Say(caller, "Report checks is still being worked on!", (Color)ThePunisher.Instance.getColor(ThePunisher.Instance.Configuration.Instance.PrivateMessageColor));
            }
            catch (System.Exception ex)
            {
                if (ThePunisher.Instance.Configuration.Instance.ShowDebugInfo) { Logger.Log("Overall Exception"); }
                Logger.LogException(ex);
            }
        }
    }
}
