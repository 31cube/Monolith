using AmongUs.Data;
using AmongUs.Data.Player;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Monolith;
using UnityEngine;

/*
[HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
public static class PShapeshifterMinigame
{
    public static bool Prefix(ShapeshifterMinigame __instance)
    {
        if (Util.PPMActive)
        {
            __instance.potentialVictims = new List<ShapeshifterPanel>();
            List<UiElement> List = new List<UiElement>();
            for (int i = 0; i < Util.PPMPlayerList.Count; i++)
            {
                NetworkedPlayerInfo PlayerData = Util.PPMPlayerList[i];
                int Column = i % 3;
                int Row = i / 3;
                ShapeshifterPanel Panel = Object.Instantiate(__instance.PanelPrefab, __instance.transform);
                Panel.transform.localPosition = new Vector3(
                    __instance.XStart + Column * __instance.XOffset,
                    __instance.YStart + Row * __instance.YOffset,
                    -1f
                );
                Panel.SetPlayer(i, PlayerData, (Il2CppSystem.Action)(() =>
                {
                    Util.PPMTarget = PlayerData;
                    Util.PPMAction.Invoke();
                    __instance.Close();
                }));
                if (PlayerData.Object != null)
                {
                    Panel.NameText.text = PlayerData.DefaultOutfit.PlayerName;
                };
                __instance.potentialVictims.Add(Panel);
                List.Add(Panel.Button);
            };
            return false;
        };
        return true;
    }
};

[HarmonyPatch(typeof(ShapeshifterPanel), nameof(ShapeshifterPanel.SetPlayer))]
public static class PShapeshifterPanel
{
    public static bool Prefix(ShapeshifterPanel __instance, int index, NetworkedPlayerInfo playerInfo, Il2CppSystem.Action onShift)
    {
        if (Util.PPMActive)
        {
            __instance.shapeshift = onShift;
            __instance.PlayerIcon.SetFlipX(false);
            __instance.PlayerIcon.ToggleName(false);
            SpriteRenderer[] componentsInChildren = __instance.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].material.SetInt(PlayerMaterial.MaskLayer, index + 2);
            };
            __instance.PlayerIcon.SetMaskLayer(index + 2);
            __instance.PlayerIcon.UpdateFromEitherPlayerDataOrCache(playerInfo, PlayerOutfitType.Default, PlayerMaterial.MaskType.ComplexUI, false, null);
            __instance.LevelNumberText.text = ProgressionManager.FormatVisualLevel(playerInfo.PlayerLevel);
            __instance.NameText.text = playerInfo.PlayerName;
            DataManager.Settings.Accessibility.OnColorBlindModeChanged += (Il2CppSystem.Action)__instance.SetColorblindText;
            __instance.SetColorblindText();
            return false;
        };
        return true;
    }
};
*/
