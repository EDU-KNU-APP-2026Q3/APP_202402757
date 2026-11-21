using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static partial class MidSceneSetup
{
    private static readonly Color Lilac = new Color32(236, 227, 249, 255);
    private static readonly Color Mint = new Color32(222, 240, 236, 255);
    private static readonly Color AccentInk = new Color32(115, 79, 146, 255);

    [MenuItem("Tools/Mid/Add Project Screens")]
    public static void AddProjectScreens()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Exit Play Mode before adding Mid screens.");

        theme = AssetDatabase.LoadAssetAtPath<UITheme>(Root + "/UI/Mid_Theme.asset");
        AddScreenGlyphs(theme.primaryFont);
        UpdateScene("Mid_MainMenu", AddMainMenuScreens);
        UpdateScene("Mid", AddGameplayScreens);
        UpdateScene("Mid_GameOver", AddResultScreen);
        AssetDatabase.SaveAssets();
    }

    private static void AddScreenGlyphs(TMP_FontAsset font)
    {
        const string captions =
            "설정 배경음 효과음 음소거 효과음 듣기 돌아가기 게임 방법 움직이기 " +
            "키보드 터치 방향키 화면 아래 조이스틱을 밀어 이동해요 가까운 적을 자동으로 공격해요 " +
            "적을 피하며 더 높은 점수에 도전하세요 일시정지 계속하기 시작 화면으로 " +
            "시작 화면으로 돌아갈까요 지금 도전은 기록에 저장되지 않아요 취소 나가기 " +
            "이번 도전 결과 도전 결과 보기 처치한 적 피격 횟수 아직 기록이 없어요 파스텔 스마일 0123456789%,.? /:WASDONF";
        string newGlyphs = new string(captions.Distinct().Where(c => !font.HasCharacter(c)).ToArray());
        if (newGlyphs.Length == 0 && font.atlasPopulationMode == AtlasPopulationMode.Static) return;
        if (newGlyphs.Length > 0)
        {
            var serialized = new SerializedObject(font);
            serialized.FindProperty("m_SourceFontFile").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Font>("Assets/W03/BagelFatOne-Regular.ttf");
            serialized.ApplyModifiedPropertiesWithoutUndo();
            try
            {
                font.atlasPopulationMode = AtlasPopulationMode.Dynamic;
                if (!font.TryAddCharacters(newGlyphs, out string missing))
                    throw new InvalidOperationException("Missing Mid glyphs: " + missing);
            }
            finally { font.atlasPopulationMode = AtlasPopulationMode.Static; }
        }
        else font.atlasPopulationMode = AtlasPopulationMode.Static;
        EditorUtility.SetDirty(font);
        foreach (var atlas in font.atlasTextures) EditorUtility.SetDirty(atlas);
    }

    private static void UpdateScene(string sceneName, Action<Canvas> update)
    {
        string path = Root + "/" + sceneName + ".unity";
        var scene = SceneManager.GetSceneByPath(path);
        bool alreadyOpen = scene.IsValid() && scene.isLoaded;
        if (alreadyOpen && scene.isDirty)
            throw new InvalidOperationException("Save the open scene before modifying " + path);
        if (!alreadyOpen) scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        try
        {
            var canvas = scene.GetRootGameObjects().Single(go => go.name == "Mid Canvas").GetComponent<Canvas>();
            if (canvas.transform.Find("Mid Extra Screens") != null) return;
            update(canvas);
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene))
                throw new InvalidOperationException("Could not save " + path);
        }
        finally
        {
            if (!alreadyOpen) EditorSceneManager.CloseScene(scene, true);
        }
    }

    private static Transform OverlayLayer(Canvas canvas)
    {
        var layer = Rect("Mid Extra Screens", canvas.transform);
        Stretch(layer);
        return layer;
    }

    private static MidAudio AddAudio(Transform parent)
    {
        var go = new GameObject("Mid Audio", typeof(AudioSource));
        go.transform.SetParent(parent, false);
        var source = go.GetComponent<AudioSource>();
        source.clip = AssetDatabase.LoadAssetAtPath<AudioClip>(Root + "/Audio/PastelLoop.wav");
        source.loop = true;
        source.playOnAwake = false;
        var audio = go.AddComponent<MidAudio>();
        audio.effectsClip = AssetDatabase.LoadAssetAtPath<AudioClip>(Root + "/Audio/SmileChime.wav");
        return audio;
    }

    private static void AddMainMenuScreens(Canvas canvas)
    {
        var layer = OverlayLayer(canvas);
        var audio = AddAudio(layer);
        var menu = (RectTransform)canvas.transform.Find("Safe Area/Menu Card");
        canvas.GetComponentInChildren<MidScoreBoardUI>(true).returnMenu = menu.gameObject;
        menu.sizeDelta = new Vector2(920, 1440);
        MoveMenuButton(menu, "Play", -206);
        MoveMenuButton(menu, "Open Records", -342);

        var settings = Dialog(layer, "Settings", "설정", 1120, out var settingsPanel);
        var ui = settingsPanel.AddComponent<SettingsScreenUI>();
        ui.audioController = audio;
        ui.music = Volume(settings, "배경음", 242, out ui.musicValue);
        ui.effects = Volume(settings, "효과음", 52, out ui.effectsValue);
        UnityEventTools.AddPersistentListener(ui.music.onValueChanged, ui.SetMusic);
        UnityEventTools.AddPersistentListener(ui.effects.onValueChanged, ui.SetEffects);
        AddMute(settings, ui);
        ActionButton(settings, "Preview Effects", "효과음 듣기", -255, audio.PreviewEffects);
        AddBack(settings, settingsPanel, menu, -410);
        SetPanelState(MenuButton(menu, "Open Settings", "설정", -478), settingsPanel, true, menu.gameObject, false);

        var guide = Dialog(layer, "How To Play", "게임 방법", 1060, out var guidePanel);
        var motion = Label("Movement Title", guide, "움직이기", 44, true);
        At(motion.rectTransform, 0, 240, 720, 70);
        GuideRow(guide, 134, "키보드", "방향키 / WASD");
        GuideRow(guide, -15, "터치", "화면 아래 조이스틱을 밀어 이동해요");
        var attack = Label("Attack", guide, "가까운 적을 자동으로 공격해요", 41, true);
        At(attack.rectTransform, 0, -163, 750, 70);
        var hint = Label("Goal", guide, "적을 피하며 더 높은 점수에 도전하세요", 32);
        At(hint.rectTransform, 0, -235, 750, 60);
        AddBack(guide, guidePanel, menu, -375);
        SetPanelState(MenuButton(menu, "Open How To Play", "게임 방법", -614), guidePanel, true, menu.gameObject, false);
    }

    private static void AddGameplayScreens(Canvas canvas)
    {
        var layer = OverlayLayer(canvas);
        AddAudio(layer);
        layer.gameObject.AddComponent<MidRoundStats>();
        var controller = layer.gameObject.AddComponent<MidPauseMenu>();
        var safe = canvas.transform.Find("Safe Area");
        controller.hud = safe.gameObject.AddComponent<CanvasGroup>();
        var end = (RectTransform)safe.Find("End Round");
        At(end, -145, -115, 200, 110, 1, 1);
        var pause = Button("Pause", safe, "일시정지");
        At((RectTransform)pause.transform, -380, -115, 240, 110, 1, 1);
        UnityEventTools.AddPersistentListener(pause.onClick, controller.Pause);

        var stick = Panel("Touch Joystick", safe, Lilac);
        ((MidRoundedImage)stick).capsule = true;
        At(stick.rectTransform, 195, 205, 270, 270, 0, 0);
        var handle = Panel("Handle", stick.transform, AccentInk);
        ((MidRoundedImage)handle).capsule = true;
        At(handle.rectTransform, 0, 0, 105, 105);
        handle.raycastTarget = false;
        var joystick = stick.gameObject.AddComponent<MidJoystick>();
        joystick.player = canvas.gameObject.scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<PlayerController>(true)).Single();
        joystick.handle = handle.rectTransform;

        var paused = Dialog(layer, "Pause Panel", "일시정지", 660, out var pausePanel);
        controller.pausePanel = pausePanel;
        ActionButton(paused, "Resume", "계속하기", 24, controller.Resume);
        ActionButton(paused, "Ask To Exit", "시작 화면으로", -130, controller.AskToExit);

        var confirmation = Dialog(layer, "Exit Confirmation", "시작 화면으로 돌아갈까요?", 730, out var confirmPanel);
        controller.confirmPanel = confirmPanel;
        var message = Label("Unsaved", confirmation, "지금 도전은 기록에 저장되지 않아요", 36);
        At(message.rectTransform, 0, 86, 750, 70);
        ActionButton(confirmation, "Cancel Exit", "취소", -50, controller.CancelExit);
        ActionButton(confirmation, "Confirm Exit", "나가기", -201, controller.Exit);
    }

    private static void AddResultScreen(Canvas canvas)
    {
        var layer = OverlayLayer(canvas);
        AddAudio(layer);
        var menu = (RectTransform)canvas.transform.Find("Safe Area/Menu Card");
        canvas.GetComponentInChildren<MidScoreBoardUI>(true).returnMenu = menu.gameObject;
        menu.sizeDelta = new Vector2(920, 1440);
        MoveMenuButton(menu, "Play", -206);
        MoveMenuButton(menu, "Open Records", -342);
        MoveMenuButton(menu, "Home", -614);
        var details = Dialog(layer, "Result Details", "이번 도전 결과", 890, out var panel);
        var ui = panel.AddComponent<MidResultDetails>();
        ui.defeated = ResultRow(details, "처치한 적", 121, Mint);
        ui.hits = ResultRow(details, "피격 횟수", -51, Lilac);
        AddBack(details, panel, menu, -269);
        SetPanelState(MenuButton(menu, "Open Result Details", "도전 결과 보기", -478), panel, true, menu.gameObject, false);
    }

    private static RectTransform Dialog(Transform layer, string name, string title, float height, out GameObject overlay)
    {
        var scrim = Panel(name, layer, new Color32(46, 31, 62, 155), false);
        Stretch(scrim.rectTransform);
        overlay = scrim.gameObject;
        overlay.SetActive(false);
        var safe = SafeArea(scrim.transform);
        var card = Panel("Dialog Card", safe, Paper);
        At(card.rectTransform, 0, 0, 890, height);
        var heading = Label("Title", card.transform, title, 67, true);
        At(heading.rectTransform, 0, -123, 760, 105, .5f, 1);
        return card.rectTransform;
    }

    private static void MoveMenuButton(Transform menu, string name, float y)
    {
        At((RectTransform)menu.Find(name), 0, y, 720, 110);
    }

    private static Button MenuButton(Transform parent, string name, string label, float y)
    {
        var button = Button(name, parent, label);
        At((RectTransform)button.transform, 0, y, 720, 110);
        return button;
    }

    private static void ActionButton(Transform parent, string name, string label, float y, UnityAction action)
    {
        UnityEventTools.AddPersistentListener(MenuButton(parent, name, label, y).onClick, action);
    }

    private static void AddBack(Transform card, GameObject panel, Transform menu, float y)
    {
        SetPanelState(MenuButton(card, "Back", "돌아가기", y), menu.gameObject, true, panel, false);
    }

    private static void SetPanelState(Button button, GameObject show, bool shown, GameObject hide, bool hidden)
    {
        UnityEventTools.AddBoolPersistentListener(button.onClick, show.SetActive, shown);
        UnityEventTools.AddBoolPersistentListener(button.onClick, hide.SetActive, hidden);
    }

    private static Slider Volume(Transform parent, string caption, float y, out TMP_Text value)
    {
        var label = Label(caption, parent, caption, 42);
        At(label.rectTransform, -243, y + 64, 230, 66);
        label.alignment = TextAlignmentOptions.Left;
        value = Label(caption + " Value", parent, "100%", 40);
        At(value.rectTransform, 257, y + 64, 192, 66);
        value.alignment = TextAlignmentOptions.Right;

        var touch = Panel(caption + " Slider", parent, Color.clear, false);
        At(touch.rectTransform, 0, y - 9, 720, 110);
        var track = Panel("Track", touch.transform, Lilac);
        At(track.rectTransform, 0, 0, 668, 36);
        track.raycastTarget = false;
        var fill = Panel("Fill", track.transform, AccentInk, false);
        Stretch(fill.rectTransform);
        fill.raycastTarget = false;
        var travel = Rect("Handle Travel", touch.transform);
        travel.anchorMin = new Vector2(0f, .5f);
        travel.anchorMax = new Vector2(1f, .5f);
        travel.offsetMin = new Vector2(26, 0);
        travel.offsetMax = new Vector2(-26, 0);
        var handle = Panel("Thumb", travel, Color.white);
        ((MidRoundedImage)handle).capsule = true;
        At(handle.rectTransform, 0, 0, 56, 56);
        var slider = touch.gameObject.AddComponent<Slider>();
        slider.fillRect = fill.rectTransform;
        slider.handleRect = handle.rectTransform;
        slider.targetGraphic = handle;
        slider.SetValueWithoutNotify(1f);
        return slider;
    }

    private static void AddMute(Transform parent, SettingsScreenUI ui)
    {
        var plate = Panel("Mute", parent, Lilac);
        At(plate.rectTransform, 0, -120, 720, 104);
        var caption = Label("Mute Label", plate.transform, "음소거", 42);
        At(caption.rectTransform, -239, 0, 220, 70);
        caption.alignment = TextAlignmentOptions.Left;
        var state = Panel("Switch", plate.transform, Color.white);
        At(state.rectTransform, 242, 0, 166, 74);
        var active = Panel("On Highlight", state.transform, Mint);
        Stretch(active.rectTransform);
        active.raycastTarget = false;
        ui.muteValue = Label("State", state.transform, "OFF", 36);
        Stretch(ui.muteValue.rectTransform);
        ui.mute = plate.gameObject.AddComponent<Toggle>();
        ui.mute.targetGraphic = plate;
        ui.mute.graphic = active;
        UnityEventTools.AddPersistentListener(ui.mute.onValueChanged, ui.SetMute);
    }

    private static void GuideRow(Transform parent, float y, string key, string instructions)
    {
        var plate = Panel(key, parent, key == "키보드" ? Lilac : Mint);
        At(plate.rectTransform, 0, y, 720, 124);
        var label = Label("Input", plate.transform, key, 28);
        At(label.rectTransform, 0, 30, 680, 40);
        var content = Label("Instruction", plate.transform, instructions, 35);
        At(content.rectTransform, 0, -21, 680, 52);
    }

    private static TMP_Text ResultRow(Transform parent, string caption, float y, Color color)
    {
        var plate = Panel(caption, parent, color);
        At(plate.rectTransform, 0, y, 720, 136);
        var label = Label("Metric", plate.transform, caption, 41);
        At(label.rectTransform, -180, 0, 280, 84);
        label.alignment = TextAlignmentOptions.Left;
        var value = Label("Count", plate.transform, "0", 66, true);
        At(value.rectTransform, 172, 0, 290, 84);
        value.alignment = TextAlignmentOptions.Right;
        return value;
    }
}
