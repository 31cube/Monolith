using AmongUs.GameOptions;
using HarmonyLib;
using Monolith;
using System;
using UnityEngine;
using static Rewired.ReInput;
using System.Reflection;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class PShipStatus
{
    private static bool ReactorCache;
    private static bool OxygenCache;
    private static bool CommsCache;
    private static bool LightsCache;
    private static bool DoorsCache;
    private static bool BreakVentsCache;
    private static float NextVentTime = 0f;
    private const float VentInterval = 0.25f;
    private const byte FlagSab = 128;
    private const byte FlagRestore = 16;
    private static string LastMap;
    private static bool OxygenNotified;
    private static bool ReactorNotified;
    private static bool CommsNotified;
    private static bool LightsNotified;
    private static bool DoorsNotified;
    private static bool VentsNotified;

    public static void Postfix(ShipStatus __instance)
    {
        var (Map, Mode) = Util.GameInfo();
        if (Toggle.ReportBody)
        {
            Util.ReportBody(PlayerControl.LocalPlayer.Data);
            Toggle.ReportBody = false;
        };
        if (string.Equals(Map, "None", StringComparison.OrdinalIgnoreCase))
        {
            Toggle.Oxygen = false;
            Toggle.Reactor = false;
            Toggle.Comms = false;
            Toggle.Lights = false;
            Toggle.Doors = false;
            Toggle.BreakVents = false;
            ReactorCache = OxygenCache = CommsCache = LightsCache = DoorsCache = BreakVentsCache = false;
            LastMap = null;
            OxygenNotified = ReactorNotified = CommsNotified = LightsNotified = DoorsNotified = VentsNotified = false;
            return;
        };
        if (!string.Equals(LastMap, Map, StringComparison.OrdinalIgnoreCase))
        {
            LastMap = Map;
            OxygenNotified = ReactorNotified = CommsNotified = LightsNotified = DoorsNotified = VentsNotified = false;
            OxygenCache = Toggle.Oxygen;
            ReactorCache = Toggle.Reactor;
            CommsCache = Toggle.Comms;
            LightsCache = Toggle.Lights;
            DoorsCache = Toggle.Doors;
            BreakVentsCache = Toggle.BreakVents;
        };
        var Systems = __instance.Systems;
        bool MapHasOxygen = string.Equals(Map, "Skeld", StringComparison.OrdinalIgnoreCase) || (Systems[SystemTypes.LifeSupp] != null);
        if (MapHasOxygen)
        {
            if (Toggle.Oxygen != OxygenCache)
            {
                __instance.RpcUpdateSystem(SystemTypes.LifeSupp, Toggle.Oxygen ? FlagSab : FlagRestore);
                OxygenCache = Toggle.Oxygen;
                OxygenNotified = false;
            };
            var Life = Systems[SystemTypes.LifeSupp];
            if (Life != null)
            {
                var IsActiveProp = Life.GetType().GetProperty("IsActive");
                if (IsActiveProp != null)
                {
                    var Val = IsActiveProp.GetValue(Life);
                    if (Val is bool BoolVal)
                    {
                        Toggle.Oxygen = OxygenCache = BoolVal;
                    };
                };
            };
        }
        else
        {
            if (Toggle.Oxygen && (Toggle.Oxygen != OxygenCache) && !OxygenNotified)
            {
                HudManager.Instance.Notifier.AddDisconnectMessage("Oxygen not present on " + Map);
                Toggle.Oxygen = OxygenCache;
                OxygenNotified = true;
            }
            else
            {
                Toggle.Oxygen = OxygenCache;
            };
        };
        bool MapHasPolusReactor = string.Equals(Map, "Polus", StringComparison.OrdinalIgnoreCase);
        bool MapHasAirshipReactor = string.Equals(Map, "Airship", StringComparison.OrdinalIgnoreCase);
        bool MapHasDefaultReactor = (Systems[SystemTypes.Reactor] != null);
        bool MapHasReactor = MapHasPolusReactor || MapHasAirshipReactor || MapHasDefaultReactor;
        if (MapHasReactor)
        {
            if (Toggle.Reactor != ReactorCache)
            {
                if (MapHasPolusReactor)
                {
                    if (Systems[SystemTypes.Laboratory] != null)
                    {
                        __instance.RpcUpdateSystem(SystemTypes.Laboratory, Toggle.Reactor ? FlagSab : FlagRestore);
                    };
                }
                else if (MapHasAirshipReactor)
                {
                    if (Systems[SystemTypes.HeliSabotage] != null)
                    {
                        __instance.RpcUpdateSystem(SystemTypes.HeliSabotage, Toggle.Reactor ? FlagSab : FlagRestore);
                        if (!Toggle.Reactor)
                        {
                            __instance.RpcUpdateSystem(SystemTypes.HeliSabotage, (byte)0);
                        };
                    };
                }
                else
                {
                    if (Systems[SystemTypes.Reactor] != null)
                    {
                        __instance.RpcUpdateSystem(SystemTypes.Reactor, Toggle.Reactor ? FlagSab : FlagRestore);
                    };
                };
                ReactorCache = Toggle.Reactor;
                ReactorNotified = false;
            };
            ReactorSystemType ReactorSystem = null;
            if (MapHasPolusReactor && Systems[SystemTypes.Laboratory] != null)
            {
                ReactorSystem = Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
            }
            else if (MapHasAirshipReactor && Systems[SystemTypes.HeliSabotage] != null)
            {
                var Heli = Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();
                if (Heli != null)
                {
                    Toggle.Reactor = ReactorCache = Heli.IsActive;
                };
            }
            else if (Systems[SystemTypes.Reactor] != null)
            {
                ReactorSystem = Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
            };
            if (ReactorSystem != null)
            {
                Toggle.Reactor = ReactorCache = ReactorSystem.IsActive;
            };
        }
        else
        {
            if (Toggle.Reactor && (Toggle.Reactor != ReactorCache) && !ReactorNotified)
            {
                HudManager.Instance.Notifier.AddDisconnectMessage("Reactor not present on " + Map);
                Toggle.Reactor = ReactorCache;
                ReactorNotified = true;
            }
            else
            {
                Toggle.Reactor = ReactorCache;
            };
        };
        if (Systems[SystemTypes.Comms] != null)
        {
            var CommObj = Systems[SystemTypes.Comms];
            bool handled = false;
            if (Toggle.Comms != CommsCache)
            {
                __instance.RpcUpdateSystem(SystemTypes.Comms, Toggle.Comms ? FlagSab : FlagRestore);
                CommsCache = Toggle.Comms;
                CommsNotified = false;
            };
            if (CommObj != null)
            {
                var type = CommObj.GetType();
                var isActiveProp = type.GetProperty("IsActive");
                if (isActiveProp != null)
                {
                    var val = isActiveProp.GetValue(CommObj);
                    if (val is bool b)
                    {
                        Toggle.Comms = CommsCache = b;
                        handled = true;
                    };
                };
                if (!handled)
                {
                    try
                    {
                        var CommHq = CommObj.Cast<HqHudSystemType>();
                        if (CommHq != null)
                        {
                            Toggle.Comms = CommsCache = CommHq.IsActive;
                            handled = true;
                        };
                    }
                    catch (InvalidCastException) { };
                };
                if (!handled)
                {
                    try
                    {
                        var CommHud = CommObj.Cast<HudOverrideSystemType>();
                        if (CommHud != null)
                        {
                            Toggle.Comms = CommsCache = CommHud.IsActive;
                            handled = true;
                        };
                    }
                    catch (InvalidCastException) { };
                };
                if (!handled)
                {
                    string runtimeTypeName = type?.Name;
                    HudManager.Instance.Notifier.AddDisconnectMessage("Unknown Comms system type: " + runtimeTypeName);
                    Toggle.Comms = false;
                    CommsCache = false;
                };
            };
        }
        else
        {
            if (Toggle.Comms && (Toggle.Comms != CommsCache) && !CommsNotified)
            {
                HudManager.Instance.Notifier.AddDisconnectMessage("Comms not present on " + Map);
                Toggle.Comms = CommsCache;
                CommsNotified = true;
            }
            else
            {
                Toggle.Comms = CommsCache;
            };
        };
        if (Toggle.Lights)
        {
            if (Toggle.Lights != LightsCache)
            {
                if (Systems[SystemTypes.Electrical] != null)
                {
                    var SwitchSystemVar = Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    byte ByteMask = 4;
                    for (int I = 0; I < 5; I++)
                    {
                        if (BoolRange.Next(0.5f))
                        {
                            ByteMask |= (byte)(1 << I);
                        };
                    };
                    __instance.RpcUpdateSystem(SystemTypes.Electrical, (byte)(ByteMask | FlagSab));
                    LightsCache = Toggle.Lights;
                    LightsNotified = false;
                }
                else
                {
                    if (!LightsNotified)
                    {
                        HudManager.Instance.Notifier.AddDisconnectMessage("Electrical not present on " + Map);
                        LightsNotified = true;
                    };
                    Toggle.Lights = false;
                    LightsCache = false;
                };
            };
            if (Systems[SystemTypes.Electrical] != null)
            {
                var SwitchSystemVar = Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                if (!Toggle.Lights)
                {
                    if (Toggle.Lights != LightsCache)
                    {
                        for (int I = 0; I < 5; I++)
                        {
                            var SwitchMask = 1 << (I & 0x1F);
                            if ((SwitchSystemVar.ActualSwitches & SwitchMask) != (SwitchSystemVar.ExpectedSwitches & SwitchMask))
                            {
                                __instance.RpcUpdateSystem(SystemTypes.Electrical, (byte)I);
                            };
                        };
                        LightsCache = Toggle.Lights;
                    };
                };
                if (SwitchSystemVar != null)
                {
                    Toggle.Lights = LightsCache = SwitchSystemVar.IsActive;
                };
            }
            else
            {
                if (!string.Equals(Map, "Fungle", StringComparison.OrdinalIgnoreCase))
                {
                    if (!LightsNotified)
                    {
                        HudManager.Instance.Notifier.AddDisconnectMessage("Electrical not present on " + Map);
                        LightsNotified = true;
                    };
                    Toggle.Lights = false;
                    LightsCache = false;
                };
            };
        };
        if (Toggle.Doors)
        {
            if (Toggle.Doors != DoorsCache)
            {
                var AllDoors = ShipStatus.Instance.AllDoors;
                for (int I = 0, N = AllDoors.Count; I < N; ++I)
                {
                    var OpenableDoorVar = AllDoors[I];
                    __instance.RpcCloseDoorsOfType(OpenableDoorVar.Room);
                };
                DoorsCache = Toggle.Doors;
                Toggle.Doors = false;
                DoorsNotified = false;
            };
        };
        if (Toggle.BreakVents)
        {
            if (Toggle.BreakVents != BreakVentsCache)
            {
                BreakVentsCache = Toggle.BreakVents;
                VentsNotified = false;
            };
            if (Time.time >= NextVentTime)
            {
                NextVentTime = Time.time + VentInterval;
                var Vents = ShipStatus.Instance?.AllVents;
                if (Vents != null)
                {
                    for (int I = 0, N = Vents.Count; I < N; ++I)
                    {
                        var Vent = Vents[I];
                        VentilationSystem.Update(VentilationSystem.Operation.BootImpostors, Vent.Id);
                    };
                };
            };
        }
        else
        {
            BreakVentsCache = Toggle.BreakVents;
        };
    }
};
