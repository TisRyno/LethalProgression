# Lethal Progression
This mod adds a XP based GUI levelling system with a range of upgrade types to spend your points on.

The original mod was created by [Stoneman2](https://github.com/stoneman2/LethalProgression) but had compatibility issues leading to this version existing.

# !!! Everyone needs this mod for it to work

## How does it work?

After installing the mod, just hit the Esc key and you should see the Skills menu option on your Pause menu.

![image](https://github.com/stoneman2/LethalProgression/assets/34432122/908c3bd3-4a14-4a0e-b991-17393c9d4e54)

### Current Available Upgrades:
- Health Regeneration
- Stamina
- Battery Life
- Hand Slots (Disabled if you have ReservedItemCore)
- Loot Value (Shared by whole team)
- Strength
- Jump Height
- Sprint speed

![image](https://github.com/stoneman2/LethalProgression/assets/34432122/476c067c-1c06-4a69-bc15-d9ca9b6a80eb)

### Level up yourself and the crew:
- Assigning points is as simple as 2 clicks!
- Build your character's stats and coordinate with your team on what role you should be.

### General Configuration:
- Levels can be configured within the config editor from Thunderstore or r2modman.
- You can configure:
    - Minimum XP to level up
    - Maximum XP to level up
    - Level requirement increase per player (balances larger lobbies) 
    - Level requirement increase per new quota.
    - Change when players can remove points from skills
    - Keep progress after being fired

### Skill-based Configuration
Each skill is built up of 3 settings:

#### Skill Enabled
If the skill is enabled and available to players.

#### Skill Max Level
The max level is the total allowed levels to be put into a skill.

#### Skill Multiplier
The multiplier applied per level put into a skill.
As an example, if the Battery Life skill has a multiplier of 5, then for each skill point put into Battery Life the battery will extend by 5% and
4 points in Battery Life will increase the battery by 20%.

### XP gain:
- Depositing loot on the ship adds the Loot value as the XP amount.
    - The multiplier from the Loot Value skill is ignored.
- Killing enemies give everyone XP.
    - This is currently a hard-coded list of enemies and their relative value, eventually this will be a configurable setting.

## Compatibility:
- Any mod which affects Hand Slots is likely to be incompatible

## Credits:
- Dat1Mew for the updated logo
- daisuu.__ for bug-testing
- Stoneman2 for original mod
- CatsArmy for critical bug fix

# Installation
- Install [BepInEx](https://thunderstore.io/c/lethal-company/p/BepInEx/BepInExPack/)
- Unzip the zip to your `Lethal Company/BepInEx` folder.
- Thunderstore Manager / r2modman will also work.

# !!! Everyone needs this mod for it to work