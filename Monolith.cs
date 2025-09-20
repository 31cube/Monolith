using System;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using Epic.OnlineServices.UI;
using HarmonyLib;
using System.Threading;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace Monolith;
[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
public partial class Monolith : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);
    public static Interface UI = null;
    public static Blocker Blocker;
    public static string AppName = "Monolith";
    public override void Load()
    {
        new Thread(() =>
        {
            Harmony.PatchAll();
            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((Scene, _) =>
            {
                if (Scene.name == "MainMenu")
                {
                    if (!UI)
                    {
                        UI = AddComponent<Interface>();
                        Blocker = AddComponent<Blocker>();
                    };
                };
            }));
        }).Start();
    }
};