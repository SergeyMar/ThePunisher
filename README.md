# ThePunisher
The Punisher is a punishment system plugin for Rocketmod & Unturned meant to extend and replace GlobalBan (as it uses some of the same commands).

## Usage


/kill {player} [silent] -Causes the specified player to suicide (if "silent" is used, no message will be displayed in the chatbox)

/reports -checks all the active reports (coming in V1.0.4)

/givereward {player} currency {amount} - gives a player {x} amount of uconomy currency as a reward for a report, then moves the report to a log table (if uconomy is present).

/givereward {player} {item|car} {id} [amount] - gives a player a vehicle or an item as a reward for a report, then moves the report to a log table. If {amount} is set then the player is given {x} amount of item {y} ([amount] does not work for vehicles).

/report {player} {reason} - Adds a player to the report table (players should have access to this command if you wish to use the report system)

/warn {player} [warning] -warns a player

/mute {player} [reason] [duration] - bans a player from talking in the chatbox

/slay {player} [reason] - kills a player then bans him for 1 year

/kick {player} [reason] - kicks a player from the server

/unmute {player} - removes a mute on the specified player

/unban {player} - unbans the specified player

/ban {player} [reason] [duration] -bans the specified player


## Required Permissions:

thepunisher.reports – allows the group to check current player reports

thepunisher.ban – allows the group to ban a player

thepunisher.mute – allows the group to ban a player from chat

thepunisher.report – allows the group to report a player (should be on all groups)

thepunisher.kick – allows the group to kick a player

thepunisher.kill – allows the group to make a player suicide

thepunisher.slay – allows the group to slay a player (kill him and ban him for 1 year)

thepunisher.unban – allows the group to unban a banned player

thepunisher.unmute – allows the group to un-chat ban a player

thepunisher.warn – allows a group to warn a player

thepunisher.givereward – allows a group to reward players for reports

thepunisher.whitelist – adds a player to the whitelist

thepunisher.blacklist – removes a player from the whitelist


##Changelog:
V1.0.3

Fixed a bug where /slay wouldn’t kill the player, only ban them for a year.

Fixed a bug where /report would display the message publicly (no tattletales here!)

Fixed a translation in rewarding a player, would show the public announcement to the receiver instead of the private one.

Fixed a bug where muting a player would kick them

Add option to give a vehicle as a reward

Added Option in config to color the messages shown to both users and the command caller.

Added “Whitelist” Mode. Only allow specified players on the server.

Changed /kill command. “/kill {user}” will suicide the player with a public message, “/kill {user} silent” still suicide the player, but won’t display a message saying who caused it.

Added translation for “The player {x} was unbanned”

Added translation for “The player {x} was banned”

Cleaned up translations in the code

Added Whitelisting translations

Added translations for upcoming “/reports” command

Added translations for upcoming “/removereport” command

Added debug output to /ban command

changed /chatban to /mute

changed /unchatban to /unmute

Changed config option “ShowDebugInfo” to false by default. Let’s keep that console all nice n tidy


V1.0.2:

Fixed missing translation for /kill

Added /givereward (works for items and currency, kits in v1.0.4)

Added “ShowDebugInfo” to config. Enable to see all debug messages made by ThePunisher. Useful for errors.

Added “RewardLogs”, when a player is rewarded the reward is logged to the db for reference

Forced one report per player before admin review (ie, Gigawiz can only report Gigawiz once before the staff reviews the report)

Added checks for reports against own name (above example should actually not work)


V1.0.1:

Added commands from fr34kyn01535.Globalban

Fixed a bug where /warn would not tell the user that he was warned

Fixed a bug where /report would not tell the caller that it was successful.

Added /kill

Added /report

Added /warn

Added /chatban

Added /unchatban