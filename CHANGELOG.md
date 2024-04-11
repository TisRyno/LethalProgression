# Changelog

## [1.7.3](https://github.com/TisRyno/LethalProgression/compare/v1.7.2...v1.7.3) (2024-04-11)


### Bug Fixes

* default jump height was incorrect ([1cbc7b8](https://github.com/TisRyno/LethalProgression/commit/1cbc7b841c0a1942dd660d35964192f19098b6fd))

## [1.7.2](https://github.com/TisRyno/LethalProgression/compare/v1.7.1...v1.7.2) (2024-04-11)


### Bug Fixes

* HP HUD updating for everyone when one player takes damage ([0ea240e](https://github.com/TisRyno/LethalProgression/commit/0ea240e11659ac0669f6f056d8cdfe2716e4b51f))
* jump height not syncing for all players ([a035893](https://github.com/TisRyno/LethalProgression/commit/a0358934c4ff2c974325668e26161e07c9f7a497))

## [1.7.1](https://github.com/TisRyno/LethalProgression/compare/v1.7.0...v1.7.1) (2024-04-08)


### Bug Fixes

* dummy commit to be able to release ([4d60b73](https://github.com/TisRyno/LethalProgression/commit/4d60b73feb732857949137f3f2af5520ef081a48))

## [1.7.0](https://github.com/TisRyno/LethalProgression/compare/v1.6.0...v1.7.0) (2024-04-08)


### Features

* new networking and lots of bug fixes ([214de7e](https://github.com/TisRyno/LethalProgression/commit/214de7ea872641ada627fc3400ef988a0309fb1a))


### Bug Fixes

* ensure skill point addition is pre-setting for syncing reasons ([3f9665e](https://github.com/TisRyno/LethalProgression/commit/3f9665e8bd8726be6b892c139404fe96b043e81a))
* save file issues ([42d1a89](https://github.com/TisRyno/LethalProgression/commit/42d1a8950147d239b8e6b3b0a70a0716be913dcb))

## [1.6.0](https://github.com/TisRyno/LethalProgression/compare/v1.5.3...v1.6.0) (2024-04-05)


### Features

* bump version ([e308800](https://github.com/TisRyno/LethalProgression/commit/e308800fd12f7be916adbf463b7b37055d9ed041))

## [1.5.2](https://github.com/TisRyno/LethalProgression/compare/v1.5.1...v1.5.2) (2024-04-04)


### Bug Fixes

* dll output dir ([ece3f83](https://github.com/TisRyno/LethalProgression/commit/ece3f836b61100ca188aa9389d5f332bf46384f1))

## [1.5.1](https://github.com/TisRyno/LethalProgression/compare/v1.5.0...v1.5.1) (2024-04-04)


### Bug Fixes

* thunderstore toml ([d9e30be](https://github.com/TisRyno/LethalProgression/commit/d9e30be906636e73aac370dec1a5977447258495))

## [1.5.0](https://github.com/TisRyno/LethalProgression/compare/v1.4.1...v1.5.0) (2024-04-04)


### Features

* v1.4.2 ([a8c7c40](https://github.com/TisRyno/LethalProgression/commit/a8c7c406d8e2dad3850656b53e80dbc374a865d7))

## 1.0.0
+ Initial release

## 1.0.1
+ Fixed XP resetting, quota scaling and upgrade buttons not appearing. 

## 1.1.0
+ Added configs for every skill.
+ Disables handslot upgrade if ReservedSlots is installed.
+ Fixed scaling issues with multiple skills.
+ Fixed XP being given multiple times per scrap.
+ Fixed disconnect syncing.
+ Fixed getting fired not resetting your skills.
+ Fixed typos.

## 1.1.1
+ Updated readme.

## 1.2.0
+ Added oxygen as a skill. - You can stay in water for longer, with custom oxygen bar.
+ Configs now fully sync with the host when you're a client.
+ You can now only unspec on the ship. This can be disabled in the config.
+ Added a tooltip to the upgrades menu, put your cursor over the Upgrades panel for some info on leveling.
+ Changed the skill menu into a scrollbar for more future skills.
+ Fixed hand icons being unsynced when upgrading when hand is full.
+ Made hand slots centered when there are more than 4.
+ Fixed getting fired not resetting upgrades again.. (Hopefully it will work this time.)
+ Nerfed the default value of Team Loot Value down to 0.5% per level.

## 1.3.0
- Rework 1.2.0
- Reduce loot value multiplier to 0.25 per level
- Fix hand slots wiping items

## 1.4.0
- Started using publicised assemblies
- Reworked saving system
- Added skill saving, requires Online mode
- Fix loot values being re-added on level change

### New Skills
- Jump height (still affected by fall damage)
- Sprint (increase sprint speed, low impact)
- Strength (decrease total weight by percentage)

## 1.4.1
- Include the GUI fix by @CatsArmy
- Include saving refactor by @CatsArmy
- Include fix for interaction bug on dropping item by @douglas-srs
