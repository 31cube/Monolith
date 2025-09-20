using AmongUs.Data.Player;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using InnerNet;
using Sentry.Internal.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Monolith;
public class Util
{
    public static ShapeshifterMinigame PPM;
    public static List<NetworkedPlayerInfo> PlayerList;
    public static bool Active = false;
    public static Il2CppSystem.Action ILAction;

    public static void PlayerPickerMenu(List<NetworkedPlayerInfo> List, Il2CppSystem.Action Action)
    {
        if (!Active)
        {
            Active = true;
            PlayerList = List;
            ILAction = Action;
            var PPM = Object.Instantiate(
                RoleManager.Instance.AllRoles
                    .FirstOrDefault(r => r && r.Role == RoleTypes.Shapeshifter)
                    .Cast<ShapeshifterRole>(),
                GameData.Instance.transform
            ).ShapeshifterMenu;
            PPM.transform.SetParent(Camera.main.transform, false);
            PPM.transform.localPosition = new Vector3(0f, 0f, -50f);
            PPM.Begin(null);
        };
    }

    public static NetworkedPlayerInfo CustomMenuChoice(string PlayerName, NetworkedPlayerInfo.PlayerOutfit PlayerOutfit, RoleBehaviour AssignedRole = null)
    {
        var PlayerInfo = Object.Instantiate(GameData.Instance.PlayerInfoPrefab);
        PlayerOutfit.PlayerName = PlayerName;
        PlayerInfo.Outfits[PlayerOutfitType.Default] = PlayerOutfit;
        if (AssignedRole != null)
        {
            PlayerInfo.Role = AssignedRole;
        };
        return PlayerInfo;
    }

    public static (string Map, string Mode) GameInfo()
    {
        var host = AmongUsClient.Instance?.GetHost();
        if (host?.Character != null && host.Character.Data != null && !host.Character.Data.Disconnected)
        {
            string map = "Skeld";
            string mode = "Normal";
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
            {
                mode = "HideNSeek";
            };
            switch ((MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId)
            {
                case MapNames.Mira: map = "Mira"; break;
                case MapNames.Polus: map = "Polus"; break;
                case MapNames.Airship: map = "Airship"; break;
                case MapNames.Fungle: map = "Fungle"; break;
            };
            return (map, mode);
        }
        else
        {
            return ("None", "None");
        };
    }

    public static string GameStatus()
    {
        if (ExileController.Instance != null &&
            !((MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Airship &&
              SpawnInMinigame.Instance != null && SpawnInMinigame.Instance.isActiveAndEnabled))
        {
            return "Exiling";
        };
        if (MeetingHud.Instance != null &&
            MeetingHud.Instance.state == MeetingHud.VoteStates.Proceeding)
        {
            return "Proceeding";
        };
        if (MeetingHud.Instance != null &&
            (MeetingHud.Instance.state == MeetingHud.VoteStates.Voted ||
             MeetingHud.Instance.state == MeetingHud.VoteStates.NotVoted))
        {
            return "Voting";
        };
        if (MeetingHud.Instance != null)
        {
            return "Meeting";
        };
        if (AmongUsClient.Instance != null &&
            AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started &&
            PlayerControl.LocalPlayer != null)
        {
            return "Playing";
        };
        if (AmongUsClient.Instance != null &&
            AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined &&
            !(AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay))
        {
            return "Lobby";
        };
        if (AmongUsClient.Instance != null &&
            AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
        {
            return "FreePlay";
        };
        return "Menu";
    }

    public static void ReportBody(NetworkedPlayerInfo PlayerData)
    {
        MessageWriter Connection = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.ReportDeadBody, SendOption.None, AmongUsClient.Instance.GetHost().Id);
        Connection.Write(PlayerData.PlayerId);
        AmongUsClient.Instance.FinishRpcImmediately(Connection);
    }

    public static string GetRole(NetworkedPlayerInfo PlayerData)
    {
        string Role = DestroyableSingleton<TranslationController>.Instance.GetString(PlayerData.Role.StringName, Il2CppSystem.Array.Empty<Il2CppSystem.Object>());
        if (!PlayerData.RoleWhenAlive.HasValue)
        {
            Role = "Ghost";
        };
        return Role;
    }

    public static void CompleteAll()
    {
        foreach (PlayerTask Task in PlayerControl.LocalPlayer.myTasks)
        {
            if (!Task.IsComplete)
            {

                foreach (var Item in PlayerControl.AllPlayerControls)
                {
                    MessageWriter Connection = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.CompleteTask, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(Item));
                    Connection.WritePacked(Task.Id);
                    AmongUsClient.Instance.FinishRpcImmediately(Connection);
                };
            };
        };
    }

    public static void PopUp(string text)
    {
        var PopUp = Object.Instantiate(DiscordManager.Instance.discordPopup, Camera.main!.transform);
        var Background = PopUp.transform.Find("Background").GetComponent<SpriteRenderer>();
        var Size = Background.size;
        Size.x *= 2.5f;
        Background.size = Size;
        PopUp.TextAreaTMP.fontSizeMin = 2;
        PopUp.Show(text);
    }
};
