using LethalProgression.Config;
using LethalProgression.LessShitConfig;
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
        
        skillButtonsList.ForEach(go => GameObject.Destroy(go));
        GameObject.Destroy(mainPanel);
        GameObject.Destroy(infoPanel);

        skillButtonsList = new List<GameObject>();
        mainPanel = null;
        infoPanel = null;
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
            LethalPlugin.Log.LogDebug("Creating button for " + skill.Value.ShortName);
            GameObject skillButton = SetupUpgradeButton(skill.Value);
            LethalPlugin.Log.LogDebug("Setup passed!");

            skillButtonsList.Add(skillButton);
            LethalPlugin.Log.LogDebug("Added to skill list..");
            LoadSkillData(skill.Value, skillButton);
        }

        TeamLootButtonUpdate(0);
        TeamShipDoorButtonUpdate(0);
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

        IGeneralConfig generalConfig = LessShitConfigSystem.GetActive<IGeneralConfig>();

        if (!generalConfig.disableUnspec)
        {
            GameObject unSpecHelpText = infoPanel.transform.GetChild(9).gameObject;
            unSpecHelpText.SetActive(!show);
        }

        if (generalConfig.enableUnspecInOrbit)
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

        button.name = skill.ShortName;

        GameObject skillScroller = mainPanel.transform.GetChild(3).gameObject;
        RectTransform skillContents = (RectTransform) skillScroller.transform.GetChild(1).transform;
        
        skillContents.GetComponent<VerticalLayoutGroup>().spacing = 0f;

        button.transform.SetParent(skillContents, false);

        GameObject displayLabel = button.transform.GetChild(0).gameObject;
        displayLabel.GetComponent<TextMeshProUGUI>().SetText(skill.ShortName);

        GameObject bonusLabel = button.transform.GetChild(1).gameObject;
        bonusLabel.GetComponent<TextMeshProUGUI>().SetText(skill.CurrentLevel.ToString());
        GameObject attributeLabel = button.transform.GetChild(2).gameObject;
        attributeLabel.GetComponent<TextMeshProUGUI>().SetText("(" + skill.CurrentLevel + " " + skill.Attribute + ")");

        button.GetComponentInChildren<TextMeshProUGUI>().SetText(skill.ShortName + ":");

        button.SetActive(true);

        button.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        button.GetComponent<Button>().onClick.AddListener(delegate { UpdateStatInfo(skill); });
        return button;
    }

    public void LoadSkillData(Skill skill, GameObject skillButton)
    {
        CreateAllObjectsIfRequired();

        if (skill.IsTeamShared)
            return;
        
        GameObject bonusLabel = skillButton.transform.GetChild(1).gameObject;
        bonusLabel.GetComponent<TextMeshProUGUI>().SetText(skill.CurrentLevel.ToString());
        GameObject attributeLabel = skillButton.transform.GetChild(2).gameObject;
        attributeLabel.GetComponent<TextMeshProUGUI>().SetText($"(+{skill.CurrentLevel * skill.Multiplier}% {skill.Attribute})");

        skillButton.GetComponentInChildren<TextMeshProUGUI>().SetText($"{skill.ShortName}:");
    }

    public void UpdateAllStats()
    {
        CreateAllObjectsIfRequired();

        foreach (KeyValuePair<UpgradeType, Skill> skill in LP_NetworkManager.xpInstance.skillList.skills)
        {
            if (skill.Value.IsTeamShared)
                continue;

            GameObject skillButton = skillButtonsList.Find(x => x.name == skill.Value.ShortName);
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

        upgradeName.SetText(skill.Name);

        if (skill.MaxLevel == 99999)
        {
            upgradeAmt.SetText($"{skill.CurrentLevel}");
        }
        else
        {
            upgradeAmt.SetText($"{skill.CurrentLevel} / {skill.MaxLevel}");
        }

        upgradeDesc.SetText(skill.Description);

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
        int currentLevel = skill.CurrentLevel;
        int maxLevel = skill.MaxLevel;

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
            if (skill.CurrentLevel == 0)
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
            if (button.name == skill.ShortName)
                LoadSkillData(skill, button);
    }

    public void TeamLootButtonUpdate(int newValue)
    {
        SharedButtonUpdate(newValue, "VAL", LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.Value]);
    }

    public void TeamShipDoorButtonUpdate(int newValue)
    {
        SharedButtonUpdate(newValue, "DRB", LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.ShipDoorBattery]);
    }

    private void SharedButtonUpdate(int newValue, string name, Skill skill)
    {
        CreateAllObjectsIfRequired();

        foreach (GameObject button in skillButtonsList)
        {
            if (button.name != name)
                continue;

            GameObject displayLabel = button.transform.GetChild(0).gameObject;
            displayLabel.GetComponent<TextMeshProUGUI>().SetText(skill.ShortName);

            GameObject bonusLabel = button.transform.GetChild(1).gameObject;
            bonusLabel.GetComponent<TextMeshProUGUI>().SetText($"{skill.CurrentLevel}");
            button.GetComponentInChildren<TextMeshProUGUI>().SetText($"{skill.ShortName}:");

            float mult = skill.Multiplier;
            float value = newValue * mult;

            GameObject attributeLabel = button.transform.GetChild(2).gameObject;
            attributeLabel.GetComponent<TextMeshProUGUI>().SetText($"(+{value}% {skill.Attribute})");

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