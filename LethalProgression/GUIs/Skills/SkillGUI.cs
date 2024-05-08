using LethalProgression.Skills;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LethalProgression.GUI.Skills;


internal class SkillsGUI
{
    private GameObject mainPanel;
    private GameObject infoPanel;
    private List<GameObject> skillButtonsList = new List<GameObject>();

    public bool isMenuOpen = false;

    public void OpenSkillMenu()
    {
        CreateAllObjectsIfRequired();

        isMenuOpen = true;
        mainPanel.SetActive(true);

        GameObject mainButtons = GameObject.Find("Systems/UI/Canvas/QuickMenu/MainButtons");
        mainButtons.SetActive(false);

        GameObject playerList = GameObject.Find("Systems/UI/Canvas/QuickMenu/PlayerList");
        playerList.SetActive(false);

        RealTimeUpdateInfo();
    }

    public void CloseSkillMenu()
    {
        isMenuOpen = false;
        mainPanel?.SetActive(false);
    }

    public void CleanupGUI()
    {
        isMenuOpen = false;
        skillButtonsList = new List<GameObject>();
    }

    private void CreateAllObjectsIfRequired()
    {
        if (!mainPanel)
            CreateMainSkillMenu();

        if (!infoPanel)
            CreateInfoPanel();
    }

    private void CreateMainSkillMenu()
    {
        if (mainPanel)
            return;

        mainPanel = GameObject.Instantiate(LethalPlugin.skillBundle.LoadAsset<GameObject>("SkillMenu"));

        mainPanel.name = "SkillMenu";
        mainPanel.SetActive(false);
    }

    private void CreateInfoPanel()
    {
        infoPanel = mainPanel.transform.GetChild(1).gameObject;
        infoPanel.SetActive(false);

        GameObject backButton = mainPanel.transform.GetChild(4).gameObject;
        backButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        backButton.GetComponent<Button>().onClick.AddListener(BackButton);

        if (LP_NetworkManager.xpInstance.skillList.skills == null)
            return;

        foreach (KeyValuePair<UpgradeType, Skill> skill in LP_NetworkManager.xpInstance.skillList.skills)
        {
            LethalPlugin.Log.LogInfo("Creating button for " + skill.Value.GetShortName());
            GameObject skillButton = SetupUpgradeButton(skill.Value);
            LethalPlugin.Log.LogInfo("Setup passed!");

            skillButtonsList.Add(skillButton);
            LethalPlugin.Log.LogInfo("Added to skill list..");
            LoadSkillData(skill.Value, skillButton);
        }

        TeamLootHudUpdate(0);
    }

    public void BackButton()
    {
        CreateAllObjectsIfRequired();

        isMenuOpen = false;
        
        GameObject mainButtons = GameObject.Find("Systems/UI/Canvas/QuickMenu/MainButtons");
        mainButtons.SetActive(true);

        GameObject playerList = GameObject.Find("Systems/UI/Canvas/QuickMenu/PlayerList");
        playerList.SetActive(true);
    }

    public void SetUnspec(bool show)
    {
        CreateAllObjectsIfRequired();

        GameObject minusFive = infoPanel.transform.GetChild(6).gameObject;
        GameObject minusTwo = infoPanel.transform.GetChild(7).gameObject;
        GameObject minusOne = infoPanel.transform.GetChild(8).gameObject;

        minusFive.SetActive(show);
        minusTwo.SetActive(show);
        minusOne.SetActive(show);

        if (!bool.Parse(LethalPlugin.ModConfig.hostConfig["Disable Unspec"]))
        {
            GameObject unSpecHelpText = infoPanel.transform.GetChild(9).gameObject;
            unSpecHelpText.SetActive(!show);
        }

        if (bool.Parse(LethalPlugin.ModConfig.hostConfig["Unspec in Orbit Only"]))
        {
            GameObject unSpecHelpText = infoPanel.transform.GetChild(9).gameObject;
            unSpecHelpText.transform.GetComponent<TextMeshProUGUI>().SetText("Return to orbit to unspec.");
        }
    }

    public GameObject SetupUpgradeButton(Skill skill)
    {
        CreateAllObjectsIfRequired();

        GameObject templateButton = mainPanel.transform.GetChild(0).gameObject;
        GameObject button = GameObject.Instantiate(templateButton);

        if (!templateButton)
        {
            LethalPlugin.Log.LogError("Couldn't find template button!");
            return null;
        }

        button.name = skill.GetShortName();

        GameObject skillScroller = mainPanel.transform.GetChild(3).gameObject;
        GameObject skillContents = skillScroller.transform.GetChild(1).gameObject;
        button.transform.SetParent(skillContents.transform, false);

        GameObject displayLabel = button.transform.GetChild(0).gameObject;
        displayLabel.GetComponent<TextMeshProUGUI>().SetText(skill.GetShortName());

        GameObject bonusLabel = button.transform.GetChild(1).gameObject;
        bonusLabel.GetComponent<TextMeshProUGUI>().SetText(skill.GetLevel().ToString());
        GameObject attributeLabel = button.transform.GetChild(2).gameObject;
        attributeLabel.GetComponent<TextMeshProUGUI>().SetText("(" + skill.GetLevel() + " " + skill.GetAttribute() + ")");

        button.GetComponentInChildren<TextMeshProUGUI>().SetText(skill.GetShortName() + ":");

        button.SetActive(true);

        button.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        button.GetComponent<Button>().onClick.AddListener(delegate { UpdateStatInfo(skill); });
        return button;
    }

    public void LoadSkillData(Skill skill, GameObject skillButton)
    {
        CreateAllObjectsIfRequired();

        if (skill._teamShared)
            return;
        
        GameObject bonusLabel = skillButton.transform.GetChild(1).gameObject;
        bonusLabel.GetComponent<TextMeshProUGUI>().SetText(skill.GetLevel().ToString());
        GameObject attributeLabel = skillButton.transform.GetChild(2).gameObject;
        attributeLabel.GetComponent<TextMeshProUGUI>().SetText($"(+{skill.GetLevel() * skill.GetMultiplier()}% {skill.GetAttribute()})");

        skillButton.GetComponentInChildren<TextMeshProUGUI>().SetText($"{skill.GetShortName()}:");
    }

    public void UpdateAllStats()
    {
        CreateAllObjectsIfRequired();

        foreach (KeyValuePair<UpgradeType, Skill> skill in LP_NetworkManager.xpInstance.skillList.skills)
        {
            if (skill.Value._teamShared)
                continue;

            GameObject skillButton = skillButtonsList.Find(x => x.name == skill.Value.GetShortName());
            LoadSkillData(skill.Value, skillButton);
        }
    }

    public void UpdateStatInfo(Skill skill)
    {
        CreateAllObjectsIfRequired();

        if (!infoPanel.activeSelf)
            infoPanel.SetActive(true);

        TextMeshProUGUI upgradeName = infoPanel.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI upgradeAmt = infoPanel.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI upgradeDesc = infoPanel.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();

        upgradeName.SetText(skill.GetName());

        if (skill.GetMaxLevel() == 99999)
        {
            upgradeAmt.SetText($"{skill.GetLevel()}");
        }
        else
        {
            upgradeAmt.SetText($"{skill.GetLevel()} / {skill.GetMaxLevel()}");
        }

        upgradeDesc.SetText(skill.GetDescription());

        // Make all the buttons do something:
        GameObject plusFive = infoPanel.transform.GetChild(3).gameObject;
        GameObject plusTwo = infoPanel.transform.GetChild(4).gameObject;
        GameObject plusOne = infoPanel.transform.GetChild(5).gameObject;

        plusFive.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        plusFive.GetComponent<Button>().onClick.AddListener(delegate { ModifySkillPoints(skill, 5); });

        plusTwo.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        plusTwo.GetComponent<Button>().onClick.AddListener(delegate { ModifySkillPoints(skill, 2); });

        plusOne.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        plusOne.GetComponent<Button>().onClick.AddListener(delegate { ModifySkillPoints(skill, 1); });

        GameObject minusFive = infoPanel.transform.GetChild(6).gameObject;
        GameObject minusTwo = infoPanel.transform.GetChild(7).gameObject;
        GameObject minusOne = infoPanel.transform.GetChild(8).gameObject;

        minusFive.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        minusFive.GetComponent<Button>().onClick.AddListener(delegate { ModifySkillPoints(skill, -5); });

        minusTwo.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        minusTwo.GetComponent<Button>().onClick.AddListener(delegate { ModifySkillPoints(skill, -2); });

        minusOne.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        minusOne.GetComponent<Button>().onClick.AddListener(delegate { ModifySkillPoints(skill, -1); });
    }

    public void ModifySkillPoints(Skill skill, int delta)
    {
        CreateAllObjectsIfRequired();

        int currentSkillPoints = LP_NetworkManager.xpInstance.GetSkillPoints();
        int currentLevel = skill.GetLevel();
        int maxLevel = skill.GetMaxLevel();

        if (delta > 0)
        {
            // Check we have available points
            if (currentSkillPoints <= 0)
                return;
            
            // If we have less skill points than attempting to add, set to the max
            if (currentSkillPoints < delta)
                delta = currentSkillPoints;
            
            // If the delta takes us over the max level, set to difference
            if (currentLevel + delta > maxLevel)
                delta = maxLevel - currentLevel;
        }

        if (delta < 0)
        {
            // Check skill isn't currently level 0
            if (skill.GetLevel() == 0)
                return;

            // If we have less levels than delta, set to remaining levels
            if (currentLevel < Math.Abs(delta))
                delta = -currentLevel;
        }

        // Remove skill points first to stop desync
        LP_NetworkManager.xpInstance.SetSkillPoints(LP_NetworkManager.xpInstance.GetSkillPoints() - delta);

        skill.AddLevel(delta);
        UpdateStatInfo(skill);

        foreach (var button in skillButtonsList)
            if (button.name == skill.GetShortName())
                LoadSkillData(skill, button);
    }

    public void TeamLootHudUpdate(int newValue)
    {
        CreateAllObjectsIfRequired();

        foreach (var button in skillButtonsList)
        {
            if (button.name != "VAL")
                continue;
            
            Skill skill = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Value];
            LoadSkillData(skill, button);

            GameObject displayLabel = button.transform.GetChild(0).gameObject;
            displayLabel.GetComponent<TextMeshProUGUI>().SetText(skill.GetShortName());

            GameObject bonusLabel = button.transform.GetChild(1).gameObject;
            bonusLabel.GetComponent<TextMeshProUGUI>().SetText($"{skill.GetLevel()}");
            button.GetComponentInChildren<TextMeshProUGUI>().SetText($"{skill.GetShortName()}:");

            float mult = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Value].GetMultiplier();
            float value = newValue * mult;

            GameObject attributeLabel = button.transform.GetChild(2).gameObject;
            attributeLabel.GetComponent<TextMeshProUGUI>().SetText($"(+{value}% {skill.GetAttribute()})");
            LethalPlugin.Log.LogInfo($"Setting team value hud to {value}");
        }
    }

    private void RealTimeUpdateInfo()
    {
        GameObject tempObj = mainPanel.transform.GetChild(2).gameObject;
        tempObj = tempObj.transform.GetChild(1).gameObject;

        TextMeshProUGUI points = tempObj.GetComponent<TextMeshProUGUI>();
        points.text = LP_NetworkManager.xpInstance.GetSkillPoints().ToString();
    }
}