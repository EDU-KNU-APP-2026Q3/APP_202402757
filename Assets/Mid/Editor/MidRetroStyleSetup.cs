using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static partial class MidSceneSetup
{
    [MenuItem("Tools/Mid/Apply Pastel Retro Style")]
    public static void ApplyRetroStyle()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Exit Play Mode before styling Mid scenes.");

        var assets = new[] { "Mid", "Mid_MainMenu", "Mid_GameOver" };
        foreach (string asset in assets)
        {
            var scene = SceneManager.GetSceneByPath(Root + "/" + asset + ".unity");
            if (scene.IsValid() && scene.isDirty)
                throw new InvalidOperationException("Save the open Mid scene first: " + scene.path);
        }

        theme = AssetDatabase.LoadAssetAtPath<UITheme>(Root + "/UI/Mid_Theme.asset");
        theme.buttonColor = MidRetroArt.Pink;
        theme.backgroundColor = MidRetroArt.Paper;
        theme.accentColor = MidRetroArt.Blue;
        theme.textColor = MidRetroArt.Ink;
        EditorUtility.SetDirty(theme);
        AddScreenGlyphs(theme.primaryFont);

        Sprite enemySprite = null;
        StyleScene("Mid", canvas =>
        {
            var spawner = canvas.gameObject.scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<WaveSpawner>(true)).Single();
            enemySprite = spawner.enemyPrefab.GetComponentInChildren<SpriteRenderer>(true).sprite;
            if (enemySprite == null) throw new InvalidOperationException("The Mid enemy prefab has no sprite.");
            ApplyPinkSky(canvas.gameObject.scene);
            StyleHud(canvas);
            StyleDialogs(canvas);
        });

        StyleRecordsPrefab();
        StyleScene("Mid_MainMenu", canvas => StyleMainMenu(canvas, enemySprite));
        StyleScene("Mid_GameOver", canvas => StyleResultMenu(canvas, enemySprite));
        AssetDatabase.SaveAssets();
    }

    private static void ApplyPinkSky(Scene scene)
    {
        var sprite = ImportArtSprite(Root + "/Art/PinkPixelSky.png", transparent: false);
        var background = scene.GetRootGameObjects().Single(root => root.name == "Background");
        background.GetComponent<SpriteRenderer>().sprite = sprite;
        var fit = background.GetComponent<MidGameBackground>();
        fit.verticalFocus = .9f;
        fit.Apply();
    }

    private static Sprite ImportArtSprite(string path, bool transparent, int maxSize = 2048)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) throw new InvalidOperationException("Import the Mid art asset first: " + path);
        if (importer.textureType != TextureImporterType.Sprite || importer.filterMode != FilterMode.Point ||
            importer.textureCompression != TextureImporterCompression.Uncompressed || importer.mipmapEnabled ||
            importer.spriteImportMode != SpriteImportMode.Single || importer.alphaIsTransparency != transparent ||
            importer.maxTextureSize != maxSize)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = transparent;
            importer.maxTextureSize = maxSize;
            importer.SaveAndReimport();
        }

        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null) throw new InvalidOperationException("Could not load the Mid sprite: " + path);
        return sprite;
    }

    private static void StyleScene(string asset, Action<Canvas> style)
    {
        string path = Root + "/" + asset + ".unity";
        var scene = SceneManager.GetSceneByPath(path);
        bool openedHere = !scene.IsValid() || !scene.isLoaded;
        if (openedHere) scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        try
        {
            var canvas = scene.GetRootGameObjects().Single(root => root.name == "Mid Canvas").GetComponent<Canvas>();
            style(canvas);
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene))
                throw new InvalidOperationException("Could not save " + path);
        }
        finally
        {
            if (openedHere) EditorSceneManager.CloseScene(scene, true);
        }
    }

    private static void SetWallpaper(Transform parent, bool results = false)
    {
        var layer = parent.Find("Pastel Grid");
        if (layer == null)
        {
            var rect = Rect("Pastel Grid", parent);
            Stretch(rect);
            rect.SetSiblingIndex(0);
            rect.gameObject.AddComponent<MidRetroWallpaper>().raycastTarget = false;
            layer = rect;
        }
        EnsureCanvasRenderer(layer);
        var wallpaper = layer.GetComponent<MidRetroWallpaper>();
        wallpaper.results = results;
        wallpaper.SetVerticesDirty();
        foreach (string name in new[] { "Paper", "W06 Background" })
        {
            var old = parent.Find(name);
            if (old != null) old.gameObject.SetActive(false);
        }
    }

    private static void StyleMainMenu(Canvas canvas, Sprite enemySprite)
    {
        SetWallpaper(canvas.transform);
        Transform menu = canvas.transform.Find("Safe Area/Menu Card");
        var card = menu.GetComponent<MidRoundedImage>();
        Window(card, MidRetroArt.Blue, MidRetroArt.Paper);
        SetTitleLogo(menu);
        At((RectTransform)menu.Find("Subtitle"), 0, 364, 785, 80);

        var cloud = (menu.Find("Cloud Mascot") ?? menu.Find("W06 Character")).GetComponent<Image>();
        cloud.name = "Cloud Mascot";
        cloud.sprite = enemySprite;
        cloud.preserveAspect = true;
        var tile = menu.Find("Cloud Portrait");
        if (tile == null)
        {
            var rect = Rect("Cloud Portrait", menu);
            rect.SetSiblingIndex(cloud.transform.GetSiblingIndex());
            rect.gameObject.AddComponent<MidRetroWallpaper>().inset = true;
            rect.GetComponent<MidRetroWallpaper>().raycastTarget = false;
            tile = rect;
        }
        At((RectTransform)tile, 0, 126, 716, 338);
        EnsureCanvasRenderer(tile);
        // The original gameplay sprite contains transparent padding around the cloud.
        At(cloud.rectTransform, 0, 150, 762, 508);
        foreach (var button in menu.GetComponentsInChildren<Button>(true)) StyleButton(button);
        StyleDialogs(canvas);
    }

    [MenuItem("Tools/Mid/Apply Pastel Smile Logo")]
    private static void ApplyPastelSmileLogo()
    {
        var active = SceneManager.GetSceneByPath(Root + "/Mid_MainMenu.unity");
        if (EditorApplication.isPlayingOrWillChangePlaymode || (active.IsValid() && active.isDirty))
            throw new InvalidOperationException("Save the Main Menu scene and exit Play Mode before changing the logo.");

        StyleScene("Mid_MainMenu", canvas => SetTitleLogo(canvas.transform.Find("Safe Area/Menu Card")));
    }

    private static void SetTitleLogo(Transform menu)
    {
        var sprite = ImportArtSprite(Root + "/Art/PastelSmileLogo.png", transparent: true);
        var title = menu.Find("Title");
        title.GetComponent<TMP_Text>().text = "Pastel Smile";
        title.gameObject.SetActive(false);
        var controls = menu.Find("Controls");
        if (controls != null) UnityEngine.Object.DestroyImmediate(controls.gameObject);

        var portrait = (RectTransform)menu.Find("Cloud Portrait");
        if (portrait != null) At(portrait, 0, 126, 716, 338);
        var mascot = (RectTransform)menu.Find("Cloud Mascot");
        if (mascot != null) At(mascot, 0, 150, 762, 508);

        var logo = menu.Find("Pastel Smile Logo")?.GetComponent<Image>();
        if (logo == null)
            logo = Rect("Pastel Smile Logo", menu).gameObject.AddComponent<Image>();
        logo.sprite = sprite;
        logo.preserveAspect = true;
        logo.raycastTarget = false;
        At(logo.rectTransform, 0, 507, 802, 198);
    }

    private static void StyleResultMenu(Canvas canvas, Sprite enemySprite)
    {
        SetWallpaper(canvas.transform, results: true);
        Transform menu = canvas.transform.Find("Safe Area/Menu Card");
        menu.GetComponent<MidRoundedImage>().enabled = false;
        var oldPortrait = menu.Find("Cloud Portrait");
        if (oldPortrait != null) oldPortrait.gameObject.SetActive(false);

        var summary = ResultPlate(menu, "Round Summary", MidRetroArt.Paper);
        At(summary.rectTransform, 0, 330, 920, 700);
        Window(summary, new Color32(186, 179, 244, 255), MidRetroArt.Paper);
        var title = MoveTo(menu, summary.transform, "Title");
        At(title, 0, 192, 790, 112);
        var subtitle = MoveTo(menu, summary.transform, "Subtitle");
        At(subtitle, 0, 92, 790, 76);

        var scorePlate = ResultPlate(summary.transform, "Round Score Plate", new Color32(255, 227, 244, 255));
        At(scorePlate.rectTransform, 0, -128, 764, 248);
        var caption = scorePlate.transform.Find("Score Caption")?.GetComponent<TMP_Text>();
        if (caption == null) caption = Label("Score Caption", scorePlate.transform, "이번 점수", 38);
        At(caption.rectTransform, -172, 63, 326, 60);
        var score = MoveTo(menu, scorePlate.transform, "Round Score").GetComponent<TMP_Text>();
        score.text = "0";
        score.fontSizeMax = 108;
        score.fontSizeMin = 24;
        At(score.rectTransform, -172, -33, 326, 121);

        string mascotName = menu.GetComponentsInChildren<Image>(true).Any(image => image.name == "Cloud Mascot")
            ? "Cloud Mascot" : "W06 Character";
        var cloud = MoveTo(menu, scorePlate.transform, mascotName).GetComponent<Image>();
        cloud.name = "Cloud Mascot";
        cloud.sprite = enemySprite;
        cloud.preserveAspect = true;
        cloud.raycastTarget = false;
        // Allow transparent sprite padding while keeping the silhouette inside the right column.
        At(cloud.rectTransform, 176, 17, 570, 380);

        var actions = ResultPlate(menu, "Result Actions", new Color32(247, 241, 255, 255));
        At(actions.rectTransform, 0, -360, 920, 585);
        PlaceResultButton(menu, actions.transform, "Play", 0, 152, 760, 125);
        PlaceResultButton(menu, actions.transform, "Open Records", -195, -12, 370, 112);
        PlaceResultButton(menu, actions.transform, "Open Result Details", 195, -12, 370, 112);
        PlaceResultButton(menu, actions.transform, "Home", 0, -160, 760, 110);
        StyleDialogs(canvas);
    }

    private static MidRoundedImage ResultPlate(Transform parent, string name, Color color)
    {
        var image = parent.Find(name)?.GetComponent<MidRoundedImage>();
        if (image == null) image = (MidRoundedImage)Panel(name, parent, color);
        image.appearance = MidRoundedImage.Appearance.Tile;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static RectTransform MoveTo(Transform root, Transform parent, string name)
    {
        var item = root.GetComponentsInChildren<RectTransform>(true).Single(item => item.name == name);
        if (item.parent != parent) item.SetParent(parent, false);
        return item;
    }

    private static void PlaceResultButton(Transform root, Transform parent, string name, float x, float y,
        float width, float height)
    {
        var rect = MoveTo(root, parent, name);
        At(rect, x, y, width, height);
        StyleButton(rect.GetComponent<Button>());
    }

    private static void StyleHud(Canvas canvas)
    {
        var safe = canvas.transform.Find("Safe Area");
        var score = safe.Find("Score Plate").GetComponent<MidRoundedImage>();
        score.appearance = MidRoundedImage.Appearance.Tile;
        score.color = MidRetroArt.Paper;
        foreach (var button in safe.GetComponentsInChildren<Button>(true)) StyleButton(button);
        var stick = safe.Find("Touch Joystick").GetComponent<MidRoundedImage>();
        stick.color = new Color32(223, 206, 250, 214);
        stick.transform.Find("Handle").GetComponent<Image>().color = new Color32(116, 85, 163, 240);
        if (AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "/Art/PixelHeart.png") != null)
            SetHealthHud(canvas);
    }

    [MenuItem("Tools/Mid/Apply Pixel Health HUD")]
    private static void ApplyHealthHud()
    {
        var active = SceneManager.GetSceneByPath(Root + "/Mid.unity");
        if (EditorApplication.isPlayingOrWillChangePlaymode || (active.IsValid() && active.isDirty))
            throw new InvalidOperationException("Save the Mid gameplay scene and exit Play Mode before changing the HUD.");
        theme = AssetDatabase.LoadAssetAtPath<UITheme>(Root + "/UI/Mid_Theme.asset");
        StyleScene("Mid", SetHealthHud);
    }

    private static void SetHealthHud(Canvas canvas)
    {
        var sprite = ImportArtSprite(Root + "/Art/PixelHeart.png", transparent: true, maxSize: 128);
        var safe = canvas.transform.Find("Safe Area");
        var plate = ResultPlate(safe, "Remaining Lives", MidRetroArt.Paper);
        At(plate.rectTransform, 366, -223, 612, 85, 0f, 1f);

        var row = (RectTransform)plate.transform.Find("Heart Row");
        if (row == null)
        {
            row = Rect("Heart Row", plate.transform);
            var group = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            group.spacing = 4;
            group.childControlWidth = group.childControlHeight = true;
            group.childForceExpandWidth = group.childForceExpandHeight = true;
        }
        At(row, -62, 0, 462, 47);
        var counter = plate.transform.Find("Lives Counter")?.GetComponent<TMP_Text>();
        if (counter == null) counter = Label("Lives Counter", plate.transform, "10/10", 35);
        At(counter.rectTransform, 235, 0, 118, 60);

        var hud = plate.GetComponent<MidHealthHud>();
        if (hud == null) hud = plate.gameObject.AddComponent<MidHealthHud>();
        var roots = canvas.gameObject.scene.GetRootGameObjects();
        hud.player = roots.SelectMany(root => root.GetComponentsInChildren<PlayerHealth>(true)).Single();
        var spawner = roots.SelectMany(root => root.GetComponentsInChildren<WaveSpawner>(true)).Single();
        hud.enemy = spawner.enemyPrefab.GetComponentInChildren<EnemyChase>(true);
        hud.heartRow = row;
        hud.heartSprite = sprite;
        hud.remainingText = counter;
        hud.DisplayHealth(hud.player.maxHealth);
    }

    private static void StyleDialogs(Canvas canvas)
    {
        foreach (Transform panel in canvas.transform.Find("Mid Extra Screens"))
        {
            Transform card = panel.Find("Safe Area/Dialog Card");
            if (card == null) continue;
            Color bar = panel.name switch
            {
                "Settings" => MidRetroArt.Lilac,
                "How To Play" => MidRetroArt.Mint,
                "Pause Panel" => MidRetroArt.Blue,
                "Exit Confirmation" => MidRetroArt.Gold,
                _ => MidRetroArt.Pink
            };
            panel.GetComponent<Image>().color = new Color32(55, 38, 83, 132);
            Window(card.GetComponent<MidRoundedImage>(), bar, MidRetroArt.Paper);
            foreach (var button in card.GetComponentsInChildren<Button>(true)) StyleButton(button);
            foreach (var image in card.GetComponentsInChildren<MidRoundedImage>(true))
            {
                if (image.name == "Track" || image.name == "Mute" || image.name == "키보드" ||
                    image.name == "터치" || image.name == "처치한 적" || image.name == "피격 횟수")
                    image.appearance = MidRoundedImage.Appearance.Tile;
            }
        }
    }

    private static void StyleRecordsPrefab()
    {
        string path = Root + "/Prefabs/ScoreBoard.prefab";
        var prefab = PrefabUtility.LoadPrefabContents(path);
        try
        {
            SetWallpaper(prefab.transform);
            var layout = prefab.GetComponentInChildren<MidRecordsLayout>(true);
            var heading = layout.heading.GetComponent<MidRoundedImage>();
            if (heading == null) heading = layout.heading.gameObject.AddComponent<MidRoundedImage>();
            heading.raycastTarget = false;
            heading.color = MidRetroArt.Paper;
            heading.appearance = MidRoundedImage.Appearance.Tile;
            Window(layout.bestCard.GetComponent<MidRoundedImage>(), MidRetroArt.Pink, new Color32(255, 237, 247, 255));
            Window(layout.rankingCard.GetComponent<MidRoundedImage>(), MidRetroArt.Blue, MidRetroArt.Paper);
            At((RectTransform)layout.bestCard.Find("Best Label"), 0, -41, 720, 58, .5f, 1f);
            At((RectTransform)layout.rankingCard.Find("Top 3 Title"), 0, -41, 720, 58, .5f, 1f);
            prefab.GetComponent<MidScoreBoardUI>().statusText.text = "아직 기록이 없어요";
            var rows = layout.rankingCard.Find("Rank Rows");
            for (int i = 0; i < rows.childCount; i++)
            {
                var row = rows.GetChild(i).GetComponent<MidRoundedImage>();
                row.appearance = MidRoundedImage.Appearance.Tile;
                row.color = i == 0 ? new Color32(255, 228, 242, 255) : new Color32(241, 232, 254, 255);
            }
            foreach (var button in prefab.GetComponentsInChildren<Button>(true)) StyleButton(button);
            PrefabUtility.SaveAsPrefabAsset(prefab, path);
        }
        finally { PrefabUtility.UnloadPrefabContents(prefab); }
    }

    private static void EnsureCanvasRenderer(Transform layer)
    {
        if (layer.GetComponent<CanvasRenderer>() == null)
            layer.gameObject.AddComponent<CanvasRenderer>();
    }

    private static void Window(MidRoundedImage image, Color bar, Color surface)
    {
        image.appearance = MidRoundedImage.Appearance.Window;
        image.titleBar = bar;
        image.color = surface;
    }

    private static void StyleButton(Button button)
    {
        var image = button.GetComponent<MidRoundedImage>();
        bool primary = button.name == "Play" || button.name == "Resume" || button.name == "Confirm Exit";
        image.appearance = primary
            ? MidRoundedImage.Appearance.PrimaryButton
            : MidRoundedImage.Appearance.SecondaryButton;
        image.titleBar = button.name switch
        {
            "Open Records" or "Preview Effects" or "Open Result Details" => MidRetroArt.Blue,
            "Open How To Play" or "Back" or "Close Records" => MidRetroArt.Mint,
            _ => MidRetroArt.Lilac
        };
    }
}
