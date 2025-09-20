using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using UnityEngine;

using Monolith;
[HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
public static class PVent
{
    public static void Postfix(Vent __instance, NetworkedPlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
    {
        if (Toggle.UseVents == true)
        {
            float Number = float.MaxValue;
            PlayerControl PlayerObj = pc.Object;
            Vector3 Center = PlayerObj.Collider.bounds.center;
            Vector3 Position = __instance.transform.position;
            Number = Vector3.Distance(Center, Position);
            canUse = Number <= __instance.UsableDistance && !PhysicsHelpers.AnythingBetween(PlayerObj.Collider, Center, Position, Constants.ShipOnlyMask, false);
            couldUse = true;
            __result = Number;
        };
    }
};
