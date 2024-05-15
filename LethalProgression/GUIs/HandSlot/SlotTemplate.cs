using UnityEngine;

namespace LethalProgression.GUI.HandSlot;

internal class SlotTemplate
{
    private GameObject templateSlot;

    public GameObject GetTemplateSlot()
    {
        if (!templateSlot)
            CreateTemplateSlot();
        
        return templateSlot;
    }

    public void CreateTemplateSlot()
    {
        if (templateSlot)
            return;

        templateSlot = GameObject.Instantiate(GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/Inventory/Slot3"));
        
        templateSlot.name = "TemplateSlot";
        templateSlot.SetActive(false);
    }
}