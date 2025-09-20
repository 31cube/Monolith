using HarmonyLib;
using Monolith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class PHudManager
{
    public static void Postfix(HudManager __instance)
    {
        try
        {
            if (!PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                __instance.ImpostorVentButton.gameObject.SetActive(Toggle.UseVents);
            };
        }
        catch {};
    }
};