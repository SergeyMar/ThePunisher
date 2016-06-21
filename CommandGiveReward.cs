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
    public class CommandGiveReward : IRocketCommand
    {
        public string Help
        {
            get { return  "Rewards a player for a report"; }
        }

        public string Name
        {
            get { return "givereward"; }
        }

        public string Syntax
        {
            get { return "<player> <reward>"; }
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
                return new List<string>() { "thepunisher.givereward" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            try
            {
                if (command.Length == 0 || command.Length > 2)
                {
                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_invalid_parameter"));
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
                            UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_player_not_found"));
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

                string adminName = "Console";
                if (caller != null) adminName = caller.ToString();

                 if (command.Length == 2)
                {

                    //ThePunisher.Instance.Database.WarnPlayer(charactername, steamid.ToString(), adminName, command[1]);
                    //UnturnedChat.Say(ThePunisher.Instance.Translate("command_warn_public_reason", charactername, command[1]));
                    UnturnedChat.Say(caller, "This feature is not available yet", UnityEngine.Color.yellow);
                }
                else
                {
                    //ThePunisher.Instance.Database.WarnPlayer(charactername, steamid.ToString(), adminName, "");
                    //UnturnedChat.Say(ThePunisher.Instance.Translate("command_warn_public", charactername));
                    UnturnedChat.Say(caller, "This feature is not available yet", UnityEngine.Color.yellow);
                }

            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
