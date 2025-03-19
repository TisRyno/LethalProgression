using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LethalProgression.GUI.XPBar;

internal class XPBarGUI
{
    private GameObject xpBar;
    private GameObject xpInfoContainer;
    private GameObject xpBarProgress;
    private GameObject skillTreeButton;
    private TextMeshProUGUI xpText;
    private TextMeshProUGUI xpLevel;
    private TextMeshProUGUI profit;

    public void Show()
    {
        CreateAllObjectsIfRequired();
        
        xpBar.SetActive(true);
        xpBarProgress.SetActive(true);
    }

    public void Update(bool active)
    {
        CreateAllObjectsIfRequired();

        LC_XP xpInstance = LP_NetworkManager.xpInstance;

        // If the settings menu or exit game menu is open, we don't want to show the XP bar.
        xpInfoContainer.SetActive(active);

        // Set actual XP:
        // XP Text. Values of how much XP you need to level up.
        // XP Level, which is just the level you're on.
        // Profit, which is how much money you've made.
        xpText.text = $"{xpInstance.GetXP()} / {xpInstance.teamXPRequired.Value}";
        xpLevel.text = $"Level: {xpInstance.GetLevel()}";
        profit.text = $"You've made.. {xpInstance.GetProfit()}$";

        // Set the bar fill
        xpBarProgress.GetComponent<Image>().fillAmount = xpInstance.GetXP() / (float)xpInstance.teamXPRequired.Value;
    }

    private void CreateContainer()
    {
        if (xpInfoContainer)
            return;
        
        GameObject pauseMenu = GameObject.Find("/Systems/UI/Canvas/QuickMenu");
        
        xpInfoContainer = new GameObject("XpInfoContainer");
        // setlocal pos to be the same as old one
        xpInfoContainer.transform.SetParent(pauseMenu.transform, false);
        xpInfoContainer.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        xpInfoContainer.transform.Translate(-1.7f, 0.9f, 0f);
    }

    private void CreateXPBar()
    {
        if (xpBar)
            return;
        
        GameObject gameXPBar = GameObject.Find("/Systems/UI/Canvas/EndgameStats/LevelUp/LevelUpBox");
        GameObject gameXPText = GameObject.Find("/Systems/UI/Canvas/EndgameStats/LevelUp/Total");
        
        xpBar = GameObject.Instantiate(gameXPBar);

        xpBar.name = "XPBar";
        xpBar.transform.SetParent(xpInfoContainer.transform, false);

        ////// XP Text //////
        xpText = GameObject.Instantiate(gameXPText).GetComponent<TextMeshProUGUI>();

        xpText.name = "XPText";
        xpText.SetText("0/1000");
        xpText.alignment = TextAlignmentOptions.Center;
        xpText.color = new Color(1f, 0.6f, 0f, 1f);

        xpText.transform.SetParent(xpBar.transform, false);
        xpText.transform.Translate(-0.75f, 0.21f, 0f);

        ////// Level Text /////
        xpLevel = GameObject.Instantiate(gameXPText).GetComponent<TextMeshProUGUI>();

        xpLevel.name = "XPLevel";
        xpLevel.SetText("Level: 0");
        xpLevel.alignment = TextAlignmentOptions.Center;
        xpLevel.color = new Color(1f, 0.6f, 0f, 1f);

        xpLevel.transform.SetParent(xpInfoContainer.transform, false);
        xpLevel.transform.position = new Vector3(xpBar.transform.position.x, xpBar.transform.position.y, xpBar.transform.position.z);
        xpLevel.transform.Translate(-0.3f, 0.2f, 0f);

        ///// PROFIT! /////
        profit = GameObject.Instantiate(gameXPText).GetComponent<TextMeshProUGUI>();

        profit.name = "XPProfit";
        profit.SetText("You've made.. 0$.");
        profit.color = new Color(1f, 0.6f, 0f, 1f);
        profit.alignment = TextAlignmentOptions.Center;

        profit.transform.SetParent(xpInfoContainer.transform, false);
        profit.transform.position = new Vector3(xpBar.transform.position.x, xpBar.transform.position.y, xpBar.transform.position.z);
        profit.transform.Translate(-0.10f, -0.2f, 0f);
    }

    private void CreateXPBarProgress()
    {
        if (xpBarProgress)
            return;

        GameObject gameXPBarProgress = GameObject.Find("/Systems/UI/Canvas/EndgameStats/LevelUp/LevelUpMeter");

        xpBarProgress = GameObject.Instantiate(gameXPBarProgress);

        xpBarProgress.name = "XPBarProgress";
        xpBarProgress.GetComponent<Image>().fillAmount = 0f;

        xpBarProgress.transform.SetParent(xpBar.transform, false);
        xpBarProgress.transform.SetAsFirstSibling();
        xpBarProgress.transform.localScale = new Vector3(0.597f, 5.21f, 1f);
        xpBarProgress.transform.Translate(-0.8f, 0.2f, 0f);
        xpBarProgress.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    private void CreateSkillTreeButton()
    {
        GameObject ResumeButton = GameObject.Find("Systems/UI/Canvas/QuickMenu/MainButtons/Resume");
        GameObject MainButtons = GameObject.Find("Systems/UI/Canvas/QuickMenu/MainButtons");

        skillTreeButton = GameObject.Instantiate(ResumeButton);

        skillTreeButton.name = "Skills";
        skillTreeButton.GetComponentInChildren<TextMeshProUGUI>().text = "> Skills";

        skillTreeButton.transform.SetParent(MainButtons.transform, false);
        skillTreeButton.transform.position = new Vector3(0.55f + xpBar.transform.position.x, 1.09f + xpBar.transform.position.y, xpBar.transform.position.z);

        Transform form = xpText.transform;
        skillTreeButton.transform.localPosition = new Vector3(form.position.x, form.position.y, form.position.z);
        skillTreeButton.transform.position += new Vector3(-0.15f, 1.056f);

        // Change the onClick event to our own.
        Button.ButtonClickedEvent OnClickEvent = new();

        OnClickEvent.AddListener(OpenSkillTree);
        skillTreeButton.GetComponent<Button>().onClick = OnClickEvent;

        // Make the level bar clickable.
        Button button = xpBar.GetComponent<Button>();

        if (button is null)
        {
            button = xpBar.AddComponent<Button>();
            button.onClick = OnClickEvent;
        }

        button = xpBarProgress.GetComponent<Button>();
        if (button is null)
        {
            button = xpBarProgress.AddComponent<Button>();
            button.onClick = OnClickEvent;
        }
    }

    public void CreateAllObjectsIfRequired()
    {
        //Container => [XpBar => BarProgression], [Profit,Level]
        if (!xpInfoContainer)
            CreateContainer();

        if (!xpBar)
            CreateXPBar();

        if (!xpBarProgress)
            CreateXPBarProgress();
        
        if (!skillTreeButton)
            CreateSkillTreeButton();
    }

    private static void OpenSkillTree()
    {
        LethalPlugin.SkillsGUI.OpenSkillMenu();
    }
}