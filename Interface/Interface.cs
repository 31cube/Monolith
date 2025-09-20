using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Monolith
{
    public class Interface : MonoBehaviour
    {
        private static Interface Instance;
        private bool Visible = true;
        private Rect WindowRect;
        private Texture2D BlackTex;
        private Texture2D BorderTex;
        private GUISkin RuntimeSkin;
        private GUIStyle BorderBoxStyle;
        private GUIStyle InnerBoxStyle;
        private GUIStyle TabButtonStyle;
        private GUIStyle ToggleStyle;
        private GUIStyle LabelStyle;
        private GUIStyle TitleStyle;
        private enum TabCase { Sabotage, Player, Misc, ESP }
        private TabCase CurrentTab = TabCase.Sabotage;
        private readonly List<string> Sabotages = new List<string> { "Oxygen", "Reactor", "Lights", "Comms", "Doors", "MushroomMixup", "BrokenLights", "BreakVents" };
        private readonly List<string> Miscs = new List<string> { "UseVents", "ColorMix", "NameMix" };
        private readonly List<string> Players = new List<string>();
        private readonly int WindowId = 0xF00D;
        private static readonly Dictionary<string, FieldInfo> ToggleFieldCache = new Dictionary<string, FieldInfo>();

        private static FieldInfo GetToggleField(string Name)
        {
            if (ToggleFieldCache.TryGetValue(Name, out var Fi)) return Fi;
            var T = typeof(Toggle);
            Fi = T.GetField(Name, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
            ToggleFieldCache[Name] = Fi;
            return Fi;
        }

        private static bool GetToggleValue(string Name)
        {
            var Fi = GetToggleField(Name);
            if (Fi != null && Fi.FieldType == typeof(bool)) return (bool)Fi.GetValue(null);
            return false;
        }

        private static void SetToggleValue(string Name, bool Value)
        {
            var Fi = GetToggleField(Name);
            if (Fi != null && Fi.FieldType == typeof(bool)) Fi.SetValue(null, Value);
        }

        public static void Load()
        {
            if (Instance != null) return;
            var Go = new GameObject(Monolith.AppName);
            DontDestroyOnLoad(Go);
            Instance = Go.AddComponent<Interface>();
        }

        private void Start()
        {
            Instance = this;
            UpdateWindowSize(true);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (BlackTex != null) Destroy(BlackTex);
            if (BorderTex != null) Destroy(BorderTex);
            if (RuntimeSkin != null) Destroy(RuntimeSkin);
        }

        private void EnsureTexturesAndSkin()
        {
            if (BlackTex == null)
            {
                BlackTex = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.DontSave };
                BlackTex.SetPixel(0, 0, new Color(0f, 0f, 0f, 1f));
                BlackTex.Apply();
            };
            if (BorderTex == null)
            {
                BorderTex = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.DontSave };
                BorderTex.SetPixel(0, 0, new Color(0.08f, 0.08f, 0.08f, 1f));
                BorderTex.Apply();
            };
            if (RuntimeSkin == null)
            {
                RuntimeSkin = ScriptableObject.CreateInstance<GUISkin>();
                RuntimeSkin.hideFlags = HideFlags.DontSave;
                BorderBoxStyle = new GUIStyle();
                BorderBoxStyle.normal.background = BorderTex;
                BorderBoxStyle.margin = new RectOffset();
                BorderBoxStyle.margin.left = 0;
                BorderBoxStyle.margin.right = 0;
                BorderBoxStyle.margin.top = 0;
                BorderBoxStyle.margin.bottom = 0;
                BorderBoxStyle.padding = new RectOffset();
                BorderBoxStyle.padding.left = 0;
                BorderBoxStyle.padding.right = 0;
                BorderBoxStyle.padding.top = 0;
                BorderBoxStyle.padding.bottom = 0;
                InnerBoxStyle = new GUIStyle();
                InnerBoxStyle.normal.background = BlackTex;
                InnerBoxStyle.margin = new RectOffset();
                InnerBoxStyle.margin.left = 1;
                InnerBoxStyle.margin.right = 1;
                InnerBoxStyle.margin.top = 1;
                InnerBoxStyle.margin.bottom = 1;
                InnerBoxStyle.padding = new RectOffset();
                InnerBoxStyle.padding.left = 8;
                InnerBoxStyle.padding.right = 8;
                InnerBoxStyle.padding.top = 8;
                InnerBoxStyle.padding.bottom = 8;
                TabButtonStyle = new GUIStyle(GUI.skin.button);
                TabButtonStyle.alignment = TextAnchor.MiddleCenter;
                TabButtonStyle.fontSize = 12;
                TabButtonStyle.padding = new RectOffset();
                TabButtonStyle.padding.left = 6;
                TabButtonStyle.padding.right = 6;
                TabButtonStyle.padding.top = 3;
                TabButtonStyle.padding.bottom = 3;
                TabButtonStyle.margin = new RectOffset();
                TabButtonStyle.margin.left = 3;
                TabButtonStyle.margin.right = 3;
                TabButtonStyle.margin.top = 3;
                TabButtonStyle.margin.bottom = 3;
                ToggleStyle = new GUIStyle(GUI.skin.toggle);
                ToggleStyle.fontSize = 12;
                ToggleStyle.alignment = TextAnchor.MiddleLeft;
                ToggleStyle.fixedHeight = 20;
                ToggleStyle.normal.textColor = Color.white;
                LabelStyle = new GUIStyle(GUI.skin.label);
                LabelStyle.fontSize = 12;
                LabelStyle.alignment = TextAnchor.MiddleLeft;
                LabelStyle.normal.textColor = Color.white;
                TitleStyle = new GUIStyle(GUI.skin.label);
                TitleStyle.fontSize = 14;
                TitleStyle.alignment = TextAnchor.MiddleLeft;
                TitleStyle.fontStyle = FontStyle.Bold;
                TitleStyle.normal.textColor = Color.white;
                TabButtonStyle.normal.textColor = Color.white;
                RuntimeSkin.box = InnerBoxStyle;
                RuntimeSkin.button = TabButtonStyle;
                RuntimeSkin.toggle = ToggleStyle;
                RuntimeSkin.label = LabelStyle;
            };
        }

        private void UpdateWindowSize(bool InitialCenter = false)
        {
            float W = Screen.width * 0.35f;
            float H = W / 2f;
            if (InitialCenter || WindowRect.width == 0)
            {
                WindowRect = new Rect((Screen.width - W) / 2f, (Screen.height - H) / 2f, W, H);
            }
            else
            {
                float X = WindowRect.x;
                float Y = WindowRect.y;
                WindowRect.width = W;
                WindowRect.height = H;
                WindowRect.x = Mathf.Clamp(X, 0, Screen.width - WindowRect.width);
                WindowRect.y = Mathf.Clamp(Y, 0, Screen.height - WindowRect.height);
            };
        }

        private void Update()
        {
            bool Ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (Ctrl && Input.GetKeyDown(KeyCode.X)) Visible = !Visible;
            UpdateWindowSize(false);
        }

        private void OnGUI()
        {
            if (!Visible) return;
            EnsureTexturesAndSkin();
            if (WindowRect.width == 0) UpdateWindowSize(true);
            var PrevSkin = GUI.skin;
            var PrevBg = GUI.backgroundColor;
            var PrevContent = GUI.contentColor;
            GUI.depth = -1000;
            GUI.skin = RuntimeSkin;
            GUI.backgroundColor = Color.black;
            GUI.contentColor = Color.white;
            WindowRect = GUI.Window(WindowId, WindowRect, (GUI.WindowFunction)DrawWindow, Monolith.AppName);
            GUI.skin = PrevSkin;
            GUI.backgroundColor = PrevBg;
            GUI.contentColor = PrevContent;
        }

        private void DrawWindow(int Id)
        {
            if (BorderBoxStyle != null) GUI.Box(new Rect(0, 0, WindowRect.width, WindowRect.height), GUIContent.none, BorderBoxStyle);
            if (InnerBoxStyle != null) GUI.Box(new Rect(1, 1, Mathf.Max(0, WindowRect.width - 2), Mathf.Max(0, WindowRect.height - 2)), GUIContent.none, InnerBoxStyle);
            float Left = 8f;
            float Top = 20f;
            float Right = 8f;
            float Cw = WindowRect.width - Left - Right;
            float Ox = Left;
            float Oy = Top;
            GUI.Label(new Rect(Ox, 4f, Cw, 18f), Monolith.AppName, TitleStyle);
            int TabCount = 4;
            float TabSpacing = 6f;
            float TabW = (Cw - (TabSpacing * (TabCount - 1))) / TabCount;
            float TabH = 22f;
            for (int I = 0; I < TabCount; I++)
            {
                Rect TabRect = new Rect(Ox + I * (TabW + TabSpacing), Oy, TabW, TabH);
                string TabName = ((TabCase)I).ToString();
                if (GUI.Button(TabRect, TabName, TabButtonStyle)) CurrentTab = (TabCase)I;
                if (CurrentTab == (TabCase)I)
                {
                    Rect Underline = new Rect(TabRect.x + 4, TabRect.yMax - 4, Mathf.Max(24, TabRect.width - 8), 3);
                    GUI.Box(Underline, Texture2D.whiteTexture, GUIStyle.none);
                };
            };
            Oy += TabH + 8f;
            if (CurrentTab == TabCase.Sabotage)
            {
                Oy += 8f;
                int PerRow = 2;
                float Spacing = 8f;
                float ColW = Mathf.Floor((Cw - Spacing) / 2f);
                float RowH = 22f;
                var SabList = Sabotages;
                for (int I = 0, N = SabList.Count; I < N; I += PerRow)
                {
                    string S1 = SabList[I];
                    bool CurrentLeft = GetToggleValue(S1);
                    Rect R1 = new Rect(Ox, Oy, ColW, RowH);
                    bool V1 = GUI.Toggle(R1, CurrentLeft, S1, ToggleStyle);
                    if (V1 != CurrentLeft) SetToggleValue(S1, V1);
                    if (I + 1 < N)
                    {
                        string S2 = SabList[I + 1];
                        bool CurrentRight = GetToggleValue(S2);
                        Rect R2 = new Rect(Ox + ColW + Spacing, Oy, ColW, RowH);
                        bool V2 = GUI.Toggle(R2, CurrentRight, S2, ToggleStyle);
                        if (V2 != CurrentRight) SetToggleValue(S2, V2);
                    };
                    Oy += RowH + 4f;
                };
            }
            else if (CurrentTab == TabCase.Player)
            {
                Oy += 8f;
                float RowH = 22f;
                var PlayerList = Players;
                for (int I = 0, N = PlayerList.Count; I < N; ++I)
                {
                    string Name = PlayerList[I];
                    bool Cur = GetToggleValue(Name);
                    Rect R = new Rect(Ox, Oy, Cw, RowH);
                    bool V = GUI.Toggle(R, Cur, Name, ToggleStyle);
                    if (V != Cur) SetToggleValue(Name, V);
                    Oy += RowH + 4f;
                };
            }
            else if (CurrentTab == TabCase.Misc)
            {
                Oy += 8f;
                int PerRow = 2;
                float Spacing = 8f;
                float ColW = Mathf.Floor((Cw - Spacing) / 2f);
                float RowH = 22f;
                var MiscList = Miscs;
                for (int I = 0, N = MiscList.Count; I < N; I += PerRow)
                {
                    string S1 = MiscList[I];
                    bool CurrentLeft = GetToggleValue(S1);
                    Rect R1 = new Rect(Ox, Oy, ColW, RowH);
                    bool V1 = GUI.Toggle(R1, CurrentLeft, S1, ToggleStyle);
                    if (V1 != CurrentLeft) SetToggleValue(S1, V1);
                    if (I + 1 < N)
                    {
                        string S2 = MiscList[I + 1];
                        bool CurrentRight = GetToggleValue(S2);
                        Rect R2 = new Rect(Ox + ColW + Spacing, Oy, ColW, RowH);
                        bool V2 = GUI.Toggle(R2, CurrentRight, S2, ToggleStyle);
                        if (V2 != CurrentRight) SetToggleValue(S2, V2);
                    };
                    Oy += RowH + 4f;
                };
            }
            else if (CurrentTab == TabCase.ESP)
            {
                Oy += 8f;
                GUI.Label(new Rect(Ox, Oy, Cw, 20), "No ESP toggles available.", LabelStyle);
                Oy += 24f;
            };
            try { GUI.DragWindow(new Rect(0, 0, WindowRect.width, 20)); } catch { };
        }
    };
};