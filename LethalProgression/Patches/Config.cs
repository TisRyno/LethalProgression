using System.Collections.Generic;

namespace LethalProgression.Config
{
    internal class SkillConfig
    {
        public static IDictionary<string, string> hostConfig = new Dictionary<string, string>();
        public static void InitConfig()
        {
            LethalPlugin.Instance.BindConfig<int>(
                "General",
                "Person Multiplier",
                35,
                "How much does XP cost to level up go up per person?"
            );

            LethalPlugin.Instance.BindConfig<int>(
                "General",
                "Quota Multiplier",
                30,
                "How much more XP does it cost to level up go up per quota? (Percent)"
            );
            LethalPlugin.Instance.BindConfig<int>(
                "General",
                "XP Minimum",
                40,
                "Minimum XP to level up."
            );
            LethalPlugin.Instance.BindConfig<int>(
                "General",
                "XP Maximum",
                750,
                "Maximum XP to level up."
            );
            LethalPlugin.Instance.BindConfig<bool>(
                "General",
                "Unspec in Ship Only",
                false,
                "Disallows unspecing stats if you're not currently on the ship."
            );
            LethalPlugin.Instance.BindConfig<bool>(
                "General",
                "Unspec in Orbit Only",
                true,
                "Disallows unspecing stats if you're not currently in orbit."
            );
            LethalPlugin.Instance.BindConfig<bool>(
                "General",
                "Disable Unspec",
                false,
                "Disallows unspecing altogether."
            );

            // Skill Configs
            LethalPlugin.Instance.BindConfig<bool>(
                "Health",
                "Health Regen Enabled",
                true,
                "Enable the Health Regen skill?"
            );

            LethalPlugin.Instance.BindConfig<int>(
                "Health",
                "Health Regen Max Level",
                20,
                "Maximum level for the health regen."
            );

            LethalPlugin.Instance.BindConfig<float>(
                "Health",
                "Health Regen Multiplier",
                0.05f,
                "How much does the health regen skill increase per level?"
            );

            //
            LethalPlugin.Instance.BindConfig<bool>(
                "Stamina",
                "Stamina Enabled",
                true,
                "Enable the Stamina skill?"
            );

            LethalPlugin.Instance.BindConfig<int>(
                "Stamina",
                "Stamina Max Level",
                99999,
                "Maximum level for the stamina."
            );

            LethalPlugin.Instance.BindConfig<float>(
                "Stamina",
                "Stamina Multiplier",
                2,
                "How much does the stamina skill increase per level?"
            );

            //
            LethalPlugin.Instance.BindConfig<bool>(
                "Battery",
                "Battery Life Enabled",
                true,
                "Enable the Battery Life skill?"
            );

            LethalPlugin.Instance.BindConfig<int>(
                "Battery",
                "Battery Life Max Level",
                99999,
                "Maximum level for the battery life."
            );

            LethalPlugin.Instance.BindConfig<float>(
                "Battery",
                "Battery Life Multiplier",
                5,
                "How much does the battery life skill increase per level?"
            );

            //
            LethalPlugin.Instance.BindConfig<bool>(
                "Hand Slots",
                "Hand Slots Enabled",
                true,
                "Enable the Hand Slots skill?"
            );

            LethalPlugin.Instance.BindConfig<int>(
                "Hand Slots",
                "Hand Slots Max Level",
                30,
                "Maximum level for the hand slots."
            );

            LethalPlugin.Instance.BindConfig<float>(
                "Hand Slots",
                "Hand Slots Multiplier",
                10,
                "How much does the hand slots skill increase per level?"
            );

            //
            LethalPlugin.Instance.BindConfig<bool>(
                "Loot Value",
                "Loot Value Enabled",
                true,
                "Enable the Loot Value skill?"
            );

            LethalPlugin.Instance.BindConfig<int>(
                "Loot Value",
                "Loot Value Max Level",
                250,
                "Maximum level for the loot value."
            );

            LethalPlugin.Instance.BindConfig<float>(
                "Loot Value",
                "Loot Value Multiplier",
                0.1f,
                "How much does the loot value skill increase per level?"
            );

            //
            LethalPlugin.Instance.BindConfig<bool>(
                "Oxygen",
                "Oxygen Enabled",
                true,
                "Enable the Oxygen skill?"
            );

            LethalPlugin.Instance.BindConfig<int>(
                "Oxygen",
                "Oxygen Max Level",
                99999,
                "Maximum level for Oxygen."
            );

            LethalPlugin.Instance.BindConfig<float>(
                "Oxygen",
                "Oxygen Multiplier",
                1f,
                "How much does the Oxygen skill increase per level?"
            );

            //
            LethalPlugin.Instance.BindConfig<bool>(
                "Jump Height",
                "Jump Height Enabled",
                true,
                "Enable the Jump Height skill?"
            );

            LethalPlugin.Instance.BindConfig<int>(
                "Jump Height",
                "Jump Height Max Level",
                99999,
                "Maximum level for Jump Height."
            );

            LethalPlugin.Instance.BindConfig<float>(
                "Jump Height",
                "Jump Height Multiplier",
                3f,
                "How much does the Jump Height skill increase per level?"
            );

            //
            LethalPlugin.Instance.BindConfig<bool>(
                "Sprint Speed",
                "Sprint Speed Enabled",
                true,
                "Enable the Sprint Speed skill?"
            );

            LethalPlugin.Instance.BindConfig<int>(
                "Sprint Speed",
                "Sprint Speed Max Level",
                99999,
                "Maximum level for Sprint Speed."
            );

            LethalPlugin.Instance.BindConfig<float>(
                "Sprint Speed",
                "Sprint Speed Multiplier",
                0.75f,
                "How much does the Sprint Speed skill increase per level?"
            );

            //
            LethalPlugin.Instance.BindConfig<bool>(
                "Strength",
                "Strength Enabled",
                true,
                "Enable the strength skill?"
            );

            LethalPlugin.Instance.BindConfig<int>(
                "Strength",
                "Strength Max Level",
                75,
                "Maximum level for Strength."
            );

            LethalPlugin.Instance.BindConfig<float>(
                "Strength",
                "Strength Multiplier",
                1f,
                "How much does the Strength skill increase per level?"
            );
        }
    }
}