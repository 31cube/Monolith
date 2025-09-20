using AmongUs.Data;
using BepInEx.Logging;
using HarmonyLib;
using Monolith;
using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Security.Cryptography;

[HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
public static class PHatManager
{
    public static void Postfix(HatManager Instance)
    {
        foreach (var Bundle in Instance.allBundles)
        {
            Bundle.Free = true;
        };
        foreach (var AllBundle in Instance.allFeaturedBundles)
        {
            AllBundle.Free = true;
        };
        foreach (var Cube in Instance.allFeaturedCubes)
        {
            Cube.Free = true;
        };
        foreach (var Item in Instance.allFeaturedItems)
        {
            Item.Free = true;
        };
        foreach (var Hat in Instance.allHats)
        {
            Hat.Free = true;
        };
        foreach (var NamePlate in Instance.allNamePlates)
        {
            NamePlate.Free = true;
        };
        foreach (var Pet in Instance.allPets)
        {
            Pet.Free = true;
        };
        foreach (var Skin in Instance.allSkins)
        {
            Skin.Free = true;
        };
        foreach (var StarBundle in Instance.allStarBundles)
        {
            StarBundle.price = 0;
        };
        foreach (var Visor in Instance.allVisors)
        {
            Visor.Free = true;
        };
    }
};

[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
public static class PVersionShower
{
    public static void Postfix(VersionShower Instance)
    {
        if (Instance == null) return;
        Instance.text.text = $"Monolith ~ {Application.version}";
    }
};

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Update))]
public static class PAmongUsClient
{
    public static bool SpoofComplete = false;
    private static float NextColorTime = 0f;
    private static float NextNameTime = 0f;
    public static void Postfix()
    {
        var Local = PlayerControl.LocalPlayer;
        if (Local != null)
        {
            if (Toggle.ColorMix && Time.time >= NextColorTime)
            {
                byte ColorIndex = (byte)UnityEngine.Random.Range(1, 18);
                Local.CmdCheckColor(ColorIndex);
                NextColorTime = Time.time + 0.1f;
            };
            if (Toggle.NameMix && Time.time >= NextNameTime)
            {
                string RandomName = DestroyableSingleton<AccountManager>.Instance.GetRandomName();
                if (!string.IsNullOrEmpty(RandomName))
                {
                    Local.CmdCheckName(RandomName);
                    NextNameTime = Time.time + 0.1f;
                };
            };
        };
        if (EOSManager.Instance != null && EOSManager.Instance.loginFlowFinished && !SpoofComplete)
        {
            string Name = DestroyableSingleton<AccountManager>.Instance.GetRandomName() ?? "Player";
            Span<byte> Bytes = stackalloc byte[2];
            RandomNumberGenerator.Fill(Bytes);
            var Level = BitConverter.ToUInt16(Bytes);
            DataManager.Player.stats.level = Level;
            DataManager.Player.Save();
            var Edit = EOSManager.Instance.editAccountUsername;
            if (Edit != null)
            {
                Edit.UsernameText.SetText(Name);
                Edit.SaveUsername();
            };
            EOSManager.Instance.FriendCode = Name;
            DataManager.Player.Account.LoginStatus = EOSManager.AccountLoginStatus.LoggedIn;
            SpoofComplete = true;
        };
    }
};

[HarmonyPatch(typeof(PlatformSpecificData), nameof(PlatformSpecificData.Serialize))]
public static class PPlatformSpecificData
{
    public static void Postfix(PlatformSpecificData Instance)
    {
        int Num = new int[] { 10, 9, 8 }[UnityEngine.Random.Range(0, 3)];
        Instance.Platform = (Platforms)Num;
    }
};