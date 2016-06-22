# ThePunisher
The Punisher is a punishment system plugin for Rocketmod & Unturned meant to extend and replace GlobalBan (as it uses some of the same commands).

## Usage

/kill -Causes the specified player to suicide

/reports
-checks all the active reports (coming in V1.0.2)

/givereward 
-Rewards a player for reporting another player. (coming in V1.0.2)

/report 
-reports a player to staff (works for everyone)

/warn 
-warns a player

/chatban 
-bans a player from talking in the chatbox

/slay 
-kills a player then bans him for 1 year

/kick 
-kicks a player from the server

/unchatban -removes a chatban on the specified player

/unban -unbans the specified player

/ban 
-bans the specified player


## Required Permissions:

thepunisher.reports – allows the group to check current player reports

thepunisher.ban – allows the group to ban a player

thepunisher.chatban – allows the group to ban a player from chat

thepunisher.report – allows the group to report a player (should be on all groups)

thepunisher.kick – allows the group to kick a player

thepunisher.kill – allows the group to make a player suicide

thepunisher.slay – allows the group to slay a player (kill him and ban him for 1 year)

thepunisher.unban – allows the group to unban a banned player

thepunisher.unchatban – allows the group to un-chat ban a player

thepunisher.warn – allows a group to warn a player

thepunisher.givereward – allows a group to reward players for reports


##Changelog:
Coming in V1.0.3:

Added /reports. Gets a list of unviewed reports.

Added /removereport. Removes a report from the database. Does not reward player. Useful if report was a fake.

V1.0.2:

Fixed missing translation for /kill

Added /givereward (works for items and currency, kits in v1.0.4)

Added "ShowDebugInfo" to config. Enable to see all debug messages made by ThePunisher. Useful for errors.

Added "RewardLogs", when a player is rewarded the reward is logged to the db for reference

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
