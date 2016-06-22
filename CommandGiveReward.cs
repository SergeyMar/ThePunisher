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
            get { return "<player> <reward> <amount>"; }
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
                if (command.Length < 3 || command.Length > 4)
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
                if (ThePunisher.Instance.Database.HasBeenRewarded(steamid.ToString()) != null)
                {
                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_player_previously_awarded", charactername));
                    return;
                }
                else
                {
                    if (ThePunisher.Instance.Database.HasReported(steamid.ToString()) == null)
                    {
                        UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_player_has_not_reported", charactername));
                        return;
                    }
                }
                string adminName = "Console";
                if (caller != null) adminName = caller.ToString();
                if (command.Length == 4)
                {
                    if (command[1] == "kit")
                    {
                        #region kit rewards
                        /*if (ThePunisher.Instance.KitsLoaded)
                        {
                            ThePunisher.ExecuteDependencyCode("Kits", (IRocketPlugin plugin) =>
                            {
                                fr34kyn01535.Kits.Kits kits = (fr34kyn01535.Kits.Kits)plugin;
                                
                                UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_reward_success", charactername, command[2] + " kit"), UnityEngine.Color.green);
                                UnturnedChat.Say(steamid, ThePunisher.Instance.Translate("command_reward_success", command[2] + " kit"), UnityEngine.Color.green);
                            });
                        }
                        else if (ThePunisher.Instance.LightLoaded)
                        {

                        }
                        else
                        {
                            UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_reward_plugin_unloaded", "Kits or Light/LPX", command[1]), UnityEngine.Color.red);
                        }*/
                        UnturnedChat.Say(caller, "Kit rewards coming soon!", UnityEngine.Color.red);
                        #endregion
                    }
                    else if (command[1] == "item")
                    {
                        #region item rewards
                        try
                        {
                            
                            if (!otherPlayer.GiveItem(Convert.ToUInt16(command[2]),Convert.ToByte(command[3])))
                            {
                                UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_reward_item_failed", command[2], command[3], command[0]));
                            }
                            else
                            {
                                UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_reward_success", command[0], command[2]), UnityEngine.Color.green);
                                UnturnedChat.Say(steamid, ThePunisher.Instance.Translate("command_reward_success", command[0], command[2]), UnityEngine.Color.green);
                                ThePunisher.Instance.Database.RewardForReport(steamid.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_reward_item_failed", command[2], command[3], command[0]));
                            Logger.LogException(ex, "Failed giving item " + command[2] + " to player" + command[0]);
                        }
                        #endregion
                    }
                    else
                    {
                        UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_invalid_parameter"));
                    }
                    
                }
                else if (command.Length == 3)
                {
                    if (command[1] == "currency")
                    {
                        #region currency rewards
                        if (ThePunisher.Instance.UconomyLoaded)
                        {
                            ThePunisher.ExecuteDependencyCode("Uconomy", (IRocketPlugin plugin) =>
                            {
                                string currencyname = "currency";
                                try
                                {
                                    fr34kyn01535.Uconomy.Uconomy Uconomy = (fr34kyn01535.Uconomy.Uconomy)plugin;
                                    currencyname = Uconomy.Configuration.Instance.MoneyName;
                                    Uconomy.Database.IncreaseBalance(steamid.ToString(), Convert.ToDecimal(command[2]));
                                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_reward_success", charactername, command[2] + " " + currencyname), UnityEngine.Color.green);
                                    UnturnedChat.Say(steamid, ThePunisher.Instance.Translate("command_reward_success_player", command[2] + " " + currencyname), UnityEngine.Color.green);
                                    ThePunisher.Instance.Database.RewardForReport(steamid.ToString());
                                }
                                catch (Exception ex)
                                {
                                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_reward_failed", charactername, command[2] + " " + currencyname), UnityEngine.Color.green);
                                }
                            });
                        }
                        else
                        {
                            UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_reward_plugin_unloaded", "Uconomy", command[1]), UnityEngine.Color.red);
                        }
                        #endregion
                    }
                    else if (command[1] == "item")
                    {
                        #region item rewards
                        try
                        {

                            if (!otherPlayer.GiveItem(Convert.ToUInt16(command[2]), 1))
                            {
                                UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_reward_item_failed", command[2], 1, command[0]));
                            }
                            else
                            {
                                UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_reward_success", command[0], command[2]), UnityEngine.Color.green);
                                UnturnedChat.Say(steamid, ThePunisher.Instance.Translate("command_reward_success", command[0], command[2]), UnityEngine.Color.green);
                                ThePunisher.Instance.Database.RewardForReport(steamid.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_reward_item_failed", command[2], 1, command[0]));
                            Logger.LogException(ex, "Failed giving item " + command[2] + " to player" + command[0]);
                        }
                        #endregion
                    }
                }
                else
                {
                    UnturnedChat.Say(caller, ThePunisher.Instance.Translate("command_generic_invalid_parameter"));
                }

            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
