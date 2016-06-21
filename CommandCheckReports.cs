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

namespace rawrfuls.ThePunisher
{
    public class CommandReports : IRocketCommand
    {
        public string Help
        {
            get { return  "Gets all active player reports"; }
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
                if (command.Length > 0)
                {
                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_invalid_parameter"));
                    return;
                }

                string adminName = "Console";
                if (caller != null) adminName = caller.ToString();
                if (caller.IsAdmin)
                {
                    string activereports = ThePunisher.Instance.Database.GetReports();
                    UnturnedChat.Say(activereports);
                }

            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
