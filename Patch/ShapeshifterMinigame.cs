using AmongUs.Data;
using AmongUs.Data.Player;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Monolith;
using UnityEngine;

[HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
public static class PShapeshifterMinigame
{
    public static bool Postfix(ShapeshifterMinigame __instance)
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
