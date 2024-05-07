using HarmonyLib;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LethalProgression.Patches;

[HarmonyPatch]
internal class HUDManagerPatch
{
    private static GameObject _tempBar;
    private static TextMeshProUGUI _tempText;
    private static float _tempBarTime;

    private static GameObject levelText;
    private static float levelTextTime;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HUDManager), "PingScan_performed")]
    private static void DebugScan()
    {
       LP_NetworkManager.xpInstance.updateTeamXPClientMessage.SendServer(10);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HUDManager), "AddNewScrapFoundToDisplay")]
    private static void GiveXPForScrap(GrabbableObject GObject)
    {
        if (!GameNetworkManager.Instance.isHostingGame)
            return;

        // Now we got the loot list that's about to be displayed, we add XP for each one that gets shown.
        int scrapCost = GObject.scrapValue;

        // Give XP for the amount of money this scrap costs.
        LP_NetworkManager.xpInstance.updateTeamXPClientMessage.SendServer(scrapCost);
    }

    public static void ShowXPUpdate()
    {
        // Makes one if it doesn't exist on screen yet.
        if (!_tempBar)
            MakeBar();
        
        LC_XP xpInstance = LP_NetworkManager.xpInstance;

        GameObject _tempprogress = GameObject.Find("/Systems/UI/Canvas/IngamePlayerHUD/BottomMiddle/XPUpdate/XPBarProgress");

        _tempprogress.GetComponent<Image>().fillAmount = xpInstance.teamXP.Value / (float)xpInstance.CalculateXPRequirement();
        _tempText.text = xpInstance.teamXP.Value + " / " + (float)xpInstance.CalculateXPRequirement();

        _tempBarTime = 2f;

        if (!_tempBar.activeSelf)
            GameNetworkManager.Instance.StartCoroutine(XPBarCoroutine());
    }

    private static IEnumerator XPBarCoroutine()
    {
        _tempBar.SetActive(true);
        while (_tempBarTime > 0f)
        {
            float time = _tempBarTime;
            _tempBarTime = 0f;
            yield return new WaitForSeconds(time);
        }
        _tempBar.SetActive(false);
    }

    public static void ShowLevelUp()
    {
        if (!levelText)
            MakeLevelUp();

        levelTextTime = 5f;

        if (!levelText.gameObject.activeSelf)
            GameNetworkManager.Instance.StartCoroutine(LevelUpCoroutine());
    }

    public static void MakeLevelUp()
    {
        levelText = GameObject.Instantiate(LethalPlugin.skillBundle.LoadAsset<GameObject>("LevelUp"));

        levelText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Level Up! Spend your skill points.";

        // Make this not active
        levelText.gameObject.SetActive(false);
    }

    private static IEnumerator LevelUpCoroutine()
    {
        levelText.gameObject.SetActive(true);
        while (levelTextTime > 0f)
        {
            float time = levelTextTime;
            levelTextTime = 0f;
            yield return new WaitForSeconds(time);
        }
        levelText.gameObject.SetActive(false);
    }

    private static void MakeBar()
    {
        LethalPlugin.XPBarGUI.CreateAllObjectsIfRequired();

        GameObject _xpBar = GameObject.Find("/Systems/UI/Canvas/QuickMenu/XpInfoContainer/XPBar");
        _tempBar = GameObject.Instantiate(_xpBar);
        _tempBar.name = "XPUpdate";

        _tempText = _tempBar.GetComponentInChildren<TextMeshProUGUI>();

        GameObject _igHud = GameObject.Find("/Systems/UI/Canvas/IngamePlayerHUD/BottomMiddle");
        _tempBar.transform.SetParent(_igHud.transform, false);
        _tempBar.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        GameObject _xpBarLevel = GameObject.Find("/Systems/UI/Canvas/IngamePlayerHUD/BottomMiddle/XPUpdate/XPLevel");
        GameObject.Destroy(_xpBarLevel);

        GameObject _xpBarProfit = GameObject.Find("/Systems/UI/Canvas/IngamePlayerHUD/BottomMiddle/XPUpdate/XPProfit");
        GameObject.Destroy(_xpBarProfit);

        _tempBar.transform.Translate(3.1f, -2.1f, 0f);
        Vector3 localPos = _tempBar.transform.localPosition;

        _tempBar.transform.localPosition = new Vector3(localPos.x, localPos.y + 5f, localPos.z);

        _tempBar.SetActive(false);
    }
}
