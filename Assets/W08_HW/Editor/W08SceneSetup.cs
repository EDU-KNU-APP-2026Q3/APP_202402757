using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class W08SceneSetup
{
    const string Root = "Assets/W08_HW";
    static UITheme theme;
    static Sprite rounded;
    static GameObject managers;
    static readonly Color Ink = new Color32(64, 47, 86, 255);
    static readonly Color Pink = new Color32(251, 213, 230, 255);
    static readonly Color Paper = new Color32(248, 244, 252, 255);

    public static void RebuildFromPlan()
    {
        theme = AssetDatabase.LoadAssetAtPath<UITheme>(Root + "/UI/W08_Theme.asset");
        var font = theme.primaryFont;
        font.atlasPopulationMode = AtlasPopulationMode.Dynamic;
        if (!font.TryAddCharacters("파스텔 스마일 돌아가기 아직 기록이 없어요 나의 최고 기록에 도전해 보세요 이 기기 / 브라우저에 저장된 기록 점", out string missing) && !string.IsNullOrEmpty(missing))
            throw new Exception("Missing plan glyphs: " + missing);
        font.atlasPopulationMode = AtlasPopulationMode.Static;
        EditorUtility.SetDirty(font);
        rounded = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/UI/RoundedPanel.png");
        managers = AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Prefabs/W08_Managers.prefab");
        MakeRecordsPrefab();
        MakeMenu(false);
        MakeMenu(true);
        FitText();
        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene(Root + "/W08_MainMenu.unity");
    }

    // Run once to author actual, editable scene/prefab assets. Never regenerate on import.
    public static void Create()
    {
        if (File.Exists(Root + "/W08_HW.unity"))
            throw new InvalidOperationException("W08 scenes already exist; refusing to overwrite them.");
        Directory.CreateDirectory(Root + "/UI");
        Directory.CreateDirectory(Root + "/Prefabs");
        AssetDatabase.Refresh();
        MakeTheme();
        MakeRoundedSprite();
        MakeManagers();
        MakeRecordsPrefab();
        MakeMenu(false);
        MakeMenu(true);
        MakeGame();
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(Root + "/W08_MainMenu.unity", true),
            new EditorBuildSettingsScene(Root + "/W08_HW.unity", true),
            new EditorBuildSettingsScene(Root + "/W08_GameOver.unity", true)
        };
        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene(Root + "/W08_HW.unity");
        Debug.Log("[W08] Created scenes, score board prefab, theme and scene routes.");
    }

    static void MakeTheme()
    {
        var font = TMP_FontAsset.CreateFontAsset(AssetDatabase.LoadAssetAtPath<Font>("Assets/W03/BagelFatOne-Regular.ttf"),
            80, 9, GlyphRenderMode.SDFAA, 2048, 2048, AtlasPopulationMode.Dynamic);
        font.name = "W08_Korean";
        AssetDatabase.CreateAsset(font, Root + "/UI/W08_Korean.asset");
        font.material.hideFlags = HideFlags.None;
        AssetDatabase.AddObjectToAsset(font.material, font);
        foreach (var texture in font.atlasTextures)
        {
            texture.hideFlags = HideFlags.None;
            AssetDatabase.AddObjectToAsset(texture, font);
        }
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789,-.! /" +
            "나의기록최고점수아직없습니다다음도전에서을높여보세요이기에저장됩니다닫" +
            "순위좋았어게임오버이번다시하기처음으로시작보기새로운한판준비됐나요끝내기" +
            "수고했어요달콤한도전캐릭터움직이기방향키자동공격";
        if (!font.TryAddCharacters(chars, out string missing) && !string.IsNullOrEmpty(missing))
            throw new Exception("Missing font characters: " + missing);
        font.atlasPopulationMode = AtlasPopulationMode.Static;
        EditorUtility.SetDirty(font);
        theme = ScriptableObject.CreateInstance<UITheme>();
        theme.primaryFont = theme.headingFont = font;
        theme.textColor = Ink;
        theme.backgroundColor = Paper;
        theme.accentColor = Pink;
        theme.buttonColor = Pink;
        AssetDatabase.CreateAsset(theme, Root + "/UI/W08_Theme.asset");
    }

    static void MakeRoundedSprite()
    {
        var texture = new Texture2D(96, 96, TextureFormat.RGBA32, false);
        for (int y = 0; y < 96; y++)
        for (int x = 0; x < 96; x++)
        {
            float dx = Mathf.Max(0, Mathf.Abs(x - 47.5f) - 23.5f);
            float dy = Mathf.Max(0, Mathf.Abs(y - 47.5f) - 23.5f);
            texture.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01(24.5f - Mathf.Sqrt(dx * dx + dy * dy))));
        }
        texture.Apply();
        string path = Root + "/UI/RoundedPanel.png";
        File.WriteAllBytes(path, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteBorder = new Vector4(26, 26, 26, 26);
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();
        rounded = AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static void MakeManagers()
    {
        var go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Managers.prefab"));
        var flow = new SerializedObject(go.GetComponentInChildren<SceneFlowManager>(true));
        flow.FindProperty("mainMenuScene").stringValue = "W08_MainMenu";
        flow.FindProperty("gamePlayScene").stringValue = "W08_HW";
        flow.FindProperty("gameOverScene").stringValue = "W08_GameOver";
        flow.ApplyModifiedPropertiesWithoutUndo();
        managers = PrefabUtility.SaveAsPrefabAsset(go, Root + "/Prefabs/W08_Managers.prefab");
        Object.DestroyImmediate(go);
    }

    static RectTransform Rect(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    static void Stretch(RectTransform r, float inset = 0)
    {
        r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.one * inset; r.offsetMax = Vector2.one * -inset;
    }

    static void At(RectTransform r, float x, float y, float w, float h, float ax = .5f, float ay = .5f)
    {
        r.anchorMin = r.anchorMax = new Vector2(ax, ay);
        r.pivot = Vector2.one * .5f; r.sizeDelta = new Vector2(w, h);
        r.anchoredPosition = new Vector2(x, y);
    }

    static Image Panel(string name, Transform parent, Color color, bool round = true)
    {
        var r = Rect(name, parent);
        Image image = round ? r.gameObject.AddComponent<W08RoundedImage>() : r.gameObject.AddComponent<Image>(); image.color = color;
        return image;
    }

    static TextMeshProUGUI Label(string name, Transform parent, string value, float size, bool heading = false)
    {
        var r = Rect(name, parent);
        var text = r.gameObject.AddComponent<TextMeshProUGUI>();
        text.font = theme.primaryFont; text.text = value; text.fontSize = size;
        text.enableAutoSizing = true; text.fontSizeMin = Mathf.Max(18, size * .5f); text.fontSizeMax = size;
        text.color = Ink; text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.raycastTarget = false;
        var themed = r.gameObject.AddComponent<ThemedText>();
        themed.theme = theme; themed.isHeading = heading;
        return text;
    }

    static Button Button(string name, Transform parent, string caption)
    {
        var image = Panel(name, parent, Pink);
        ((W08RoundedImage)image).capsule = true;
        var button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        var color = button.colors; color.highlightedColor = new Color(1, .93f, .97f); color.pressedColor = new Color(.87f, .78f, .88f); button.colors = color;
        image.gameObject.AddComponent<ThemedButton>().theme = theme;
        var text = Label("Label", image.transform, caption, 42);
        Stretch(text.rectTransform, 14);
        return button;
    }

    static Canvas MakeCanvas()
    {
        var root = new GameObject("W08 Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        return canvas;
    }

    static RectTransform SafeArea(Transform parent)
    {
        var safe = Rect("Safe Area", parent); Stretch(safe);
        safe.gameObject.AddComponent<W08SafeArea>(); return safe;
    }

    static void Background(Transform parent)
    {
        var bg = Panel("Paper", parent, Paper, false); Stretch(bg.rectTransform);
        var confetti = Panel("W06 Background", parent, new Color(1, 1, 1, .11f), false);
        confetti.sprite = AssetDatabase.LoadAllAssetsAtPath("Assets/W04_HW/greenlightideas-confetti-9182057_1920.png").OfType<Sprite>().FirstOrDefault();
        Stretch(confetti.rectTransform); confetti.raycastTarget = false;
    }

    static void MakeRecordsPrefab()
    {
        var root = Rect("ScoreBoard", null); Stretch(root);
        Background(root);
        var safe = SafeArea(root);
        var ui = root.gameObject.AddComponent<W08ScoreBoardUI>();
        var layout = safe.gameObject.AddComponent<W08RecordsLayout>();
        var panel = Panel("Portrait Records Panel", safe, Color.white); layout.panel = panel.rectTransform;
        var header = Rect("Heading", panel.transform); layout.heading = header;
        var kicker = Label("Kicker", header, "파스텔 스마일", 28); At(kicker.rectTransform, 0, 55, 760, 50);
        var title = Label("Title", header, "기록", 64, true); At(title.rectTransform, 0, -22, 760, 105);
        var divider = Panel("Divider", header, new Color32(226, 221, 234, 255), false);
        At(divider.rectTransform, 0, -105, 720, 3);

        var best = Panel("Best Score Card", panel.transform, new Color32(232, 242, 247, 255)); layout.bestCard = best.rectTransform;
        var bestTitle = Label("Best Label", best.transform, "최고 기록", 40); At(bestTitle.rectTransform, 0, 60, 600, 70);
        ui.highScoreText = Label("High Score", best.transform, "-", 100, true); At(ui.highScoreText.rectTransform, 0, -40, 700, 140);

        var ranking = Panel("Top 3 Card", panel.transform, Color.white); layout.rankingCard = ranking.rectTransform;
        var rankTitle = Label("Top 3 Title", ranking.transform, "TOP 3", 32, true); At(rankTitle.rectTransform, 0, -35, 700, 60, .5f, 1);
        var rows = Rect("Rank Rows", ranking.transform);
        rows.anchorMin = new Vector2(0, .23f); rows.anchorMax = new Vector2(1, .78f);
        rows.offsetMin = new Vector2(48, 0); rows.offsetMax = new Vector2(-48, 0);
        var group = rows.gameObject.AddComponent<VerticalLayoutGroup>();
        group.spacing = 20; group.childControlWidth = group.childControlHeight = true;
        group.childForceExpandHeight = group.childForceExpandWidth = true;
        ui.rankTexts = new TMP_Text[3];
        for (int i = 0; i < 3; i++)
        {
            var row = Panel("Rank " + (i + 1), rows, Color.white);
            var rank = Label("Rank Number", row.transform, (i + 1) + "위", 40);
            At(rank.rectTransform, 80, 0, 120, 70, 0, .5f);
            var score = Label("Score", row.transform, "-", 52);
            score.alignment = TextAlignmentOptions.Right;
            score.rectTransform.anchorMin = new Vector2(.28f, 0); score.rectTransform.anchorMax = Vector2.one;
            score.rectTransform.offsetMin = new Vector2(0, 12); score.rectTransform.offsetMax = new Vector2(-42, -12);
            ui.rankTexts[i] = score;
        }
        ui.statusText = Label("Record Status", ranking.transform, "아직 기록이 없어요", 30);
        At(ui.statusText.rectTransform, 0, 30, 740, 60, .5f, 0);

        var footer = Rect("Footer", panel.transform); layout.footer = footer;
        var note = Label("Storage Note", footer, "이 기기 / 브라우저에 저장된 기록", 26); At(note.rectTransform, 0, 67, 740, 50);
        var close = Button("Close Records", footer, "돌아가기"); At((RectTransform)close.transform, 0, -35, 720, 120);
        UnityEventTools.AddPersistentListener(close.onClick, ui.ReturnToMenu);
        layout.Apply();
        PrefabUtility.SaveAsPrefabAsset(root.gameObject, Root + "/Prefabs/ScoreBoard.prefab");
        Object.DestroyImmediate(root.gameObject);
    }

    static void Bootstrap(bool game)
    {
        var boot = new GameObject("W08 Bootstrap").AddComponent<W08Bootstrap>();
        boot.managersPrefab = managers; boot.startRound = game;
    }

    static void MakeMenu(bool gameOver)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
        camera.tag = "MainCamera"; camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = Paper;
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        Bootstrap(false);
        var canvas = MakeCanvas(); Background(canvas.transform);
        var safe = SafeArea(canvas.transform);
        var actions = new GameObject("W08 Actions").AddComponent<W08Actions>();
        var card = Panel("Menu Card", safe, Color.white); At(card.rectTransform, 0, 0, 920, 1260);
        var title = Label("Title", card.transform, gameOver ? "게임 오버" : "파스텔 스마일", 76, true);
        At(title.rectTransform, 0, 470, 800, 130);
        var subtitle = Label("Subtitle", card.transform, gameOver ? "이번 도전도 수고했어요!" : "새로운 한 판, 준비됐나요?", 36);
        At(subtitle.rectTransform, 0, 350, 820, 85);
        var icon = Panel("W06 Character", card.transform, Color.white, false);
        icon.sprite = AssetDatabase.LoadAllAssetsAtPath("Assets/W06_HW/Sprites_my/Enemy_Slime_1.png").OfType<Sprite>().FirstOrDefault();
        icon.preserveAspect = true; icon.raycastTarget = false; At(icon.rectTransform, 0, 120, 230, 230);
        if (gameOver)
        {
            actions.roundScoreText = Label("Round Score", card.transform, "이번 점수  0", 44);
            At(actions.roundScoreText.rectTransform, 0, -55, 800, 85);
        }
        else
        {
            var hint = Label("Controls", card.transform, "방향키 / WASD   자동 공격", 30);
            At(hint.rectTransform, 0, -55, 820, 85);
        }
        var play = Button("Play", card.transform, gameOver ? "다시 하기" : "시작하기");
        At((RectTransform)play.transform, 0, -220, 720, 120);
        UnityEventTools.AddPersistentListener(play.onClick, actions.StartGame);
        var records = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Prefabs/ScoreBoard.prefab"), canvas.transform);
        Stretch((RectTransform)records.transform);
        var ui = records.GetComponent<W08ScoreBoardUI>();
        var open = Button("Open Records", card.transform, "기록 보기"); At((RectTransform)open.transform, 0, -375, 720, 120);
        UnityEventTools.AddPersistentListener(open.onClick, ui.Open);
        if (gameOver)
        {
            var home = Button("Home", card.transform, "처음으로"); At((RectTransform)home.transform, 0, -530, 720, 110);
            UnityEventTools.AddPersistentListener(home.onClick, actions.MainMenu);
        }
        records.SetActive(false);
        EditorSceneManager.SaveScene(scene, Root + (gameOver ? "/W08_GameOver.unity" : "/W08_MainMenu.unity"));
    }

    static void MakeGame()
    {
        AssetDatabase.CopyAsset("Assets/W06_HW/W06_HW.unity", Root + "/W08_HW.unity");
        var scene = EditorSceneManager.OpenScene(Root + "/W08_HW.unity");
        foreach (var obj in scene.GetRootGameObjects())
            if (obj.GetComponent<ManagerRoot>() != null || obj.GetComponent<Canvas>() != null || obj.name == "UIController") Object.DestroyImmediate(obj);
        foreach (var old in Object.FindObjectsByType<SceneBootstrapper>(FindObjectsSortMode.None)) Object.DestroyImmediate(old);
        var backdrop = GameObject.Find("Background");
        if (backdrop != null)
        {
            var fit = backdrop.AddComponent<W08GameBackground>();
            fit.targetCamera = Camera.main;
            fit.Apply();
        }
        Bootstrap(true);
        var canvas = MakeCanvas(); var safe = SafeArea(canvas.transform);
        var plate = Panel("Score Plate", safe, new Color(1, 1, 1, .94f));
        At(plate.rectTransform, 230, -115, 340, 110, 0, 1);
        var label = Label("Current Score", plate.transform, "Score: 0", 38); Stretch(label.rectTransform, 12);
        var hud = new GameObject("W08 HUD").AddComponent<GamePlayScoreDisplay>(); hud.scoreText = label;
        var actions = hud.gameObject.AddComponent<W08Actions>();
        var exit = Button("End Round", safe, "끝내기"); At((RectTransform)exit.transform, -210, -115, 300, 110, 1, 1);
        UnityEventTools.AddPersistentListener(exit.onClick, actions.EndGame);
        EditorSceneManager.SaveScene(scene);
    }

    public static void BuildVerificationPlayer()
    {
        FitText();
        string product = PlayerSettings.productName;
        try
        {
            // Separate PlayerPrefs namespace; screenshots never erase the user's real records.
            PlayerSettings.productName = "W08 Verification";
            Directory.CreateDirectory("D:/APP_202402757/W08_Submission/VerificationPlayer");
            var report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                "D:/APP_202402757/W08_Submission/VerificationPlayer/W08.exe", BuildTarget.StandaloneWindows64, BuildOptions.Development);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) throw new Exception(report.summary.result.ToString());
        }
        finally { PlayerSettings.productName = product; AssetDatabase.SaveAssets(); }
    }

    public static void BuildPlanVerification()
    {
        RebuildFromPlan();
        BuildVerificationPlayer();
    }

    static void FitText()
    {
        string prefabPath = Root + "/Prefabs/ScoreBoard.prefab";
        var prefab = PrefabUtility.LoadPrefabContents(prefabPath);
        FitLabels(prefab);
        PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefab);
        foreach (var path in new[] { "/W08_MainMenu.unity", "/W08_HW.unity", "/W08_GameOver.unity" })
        {
            var scene = EditorSceneManager.OpenScene(Root + path);
            foreach (var root in scene.GetRootGameObjects()) FitLabels(root);
            EditorSceneManager.SaveScene(scene);
        }
    }

    static void FitLabels(GameObject root)
    {
        foreach (var text in root.GetComponentsInChildren<TMP_Text>(true))
        {
            float size = text.enableAutoSizing ? text.fontSizeMax : text.fontSize;
            text.enableAutoSizing = true;
            text.fontSizeMin = Mathf.Max(18, size * .5f);
            text.fontSizeMax = size;
        }
    }
}
