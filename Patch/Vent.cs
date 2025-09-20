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
    public static void Postfix(Vent Instance, NetworkedPlayerInfo Pc, ref bool CanUse, ref bool CouldUse, ref float Result)
    {
        if (Toggle.UseVents == true)
        {
            float Number = float.MaxValue;
            PlayerControl PlayerObj = Pc.Object;
            Vector3 Center = PlayerObj.Collider.bounds.center;
            Vector3 Position = Instance.transform.position;
            Number = Vector3.Distance(Center, Position);
            CanUse = Number <= Instance.UsableDistance && !PhysicsHelpers.AnythingBetween(PlayerObj.Collider, Center, Position, Constants.ShipOnlyMask, false);
            CouldUse = true;
            Result = Number;
        };
    }
};