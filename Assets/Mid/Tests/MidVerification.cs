#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Opt-in verification only. Uses the separate "Mid Verification" player's preferences.
public class MidVerification : MonoBehaviour
{
    const string Output = "D:/APP_202402757/Mid_Submission";
    readonly List<string> checks = new List<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Begin()
    {
        if (!Environment.GetCommandLineArgs().Contains("-mid-verify")) return;
        if (Application.productName != "Mid Verification")
            throw new InvalidOperationException("Use the isolated verification player.");
        Application.runInBackground = true;
        var go = new GameObject("Mid Verification Runner");
        DontDestroyOnLoad(go);
        go.AddComponent<MidVerification>().StartCoroutine(go.GetComponent<MidVerification>().Run());
    }

    IEnumerator Run()
    {
        Directory.CreateDirectory(Output);
        Application.logMessageReceived += RecordError;
        var routines = new Stack<IEnumerator>();
        routines.Push(Verify());
        while (routines.Count > 0)
        {
            object current;
            try
            {
                var routine = routines.Peek();
                if (!routine.MoveNext()) { routines.Pop(); continue; }
                current = routine.Current;
                if (current is IEnumerator nested) { routines.Push(nested); continue; }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Output + "/verification-failed.txt", ex.ToString());
                Application.Quit(1);
                yield break;
            }
            yield return current;
        }
        File.WriteAllLines(Output + "/verification-passed.txt", checks);
        Application.Quit(0);
    }

    void RecordError(string message, string stack, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            File.AppendAllText(Output + "/runtime-errors.txt", message + "\n" + stack + "\n");
    }

    void Check(bool condition, string name)
    {
        if (!condition) throw new Exception(name);
        checks.Add("PASS " + name);
    }

    static void ClearTestScores()
    {
        PlayerPrefs.DeleteKey("score_highscore");
        for (int i = 0; i < 3; i++) PlayerPrefs.DeleteKey("score_top_" + i);
        PlayerPrefs.Save();
    }

    IEnumerator ReloadScores()
    {
        if (ScoreManager.Instance != null) Destroy(ScoreManager.Instance.transform.root.gameObject);
        yield return null;
        yield return SceneManager.LoadSceneAsync("Mid_GameOver");
        yield return null;
    }

    static MidScoreBoardUI Board() => FindObjectsByType<MidScoreBoardUI>(FindObjectsInactive.Include, FindObjectsSortMode.None).Single();

    IEnumerator Verify()
    {
        ClearTestScores();
        yield return ReloadScores();
        var ui = Board(); ui.Open();
        Check(ui.statusText.text == "아직 기록이 없어요" && ui.rankTexts.All(t => t.text == "-"), "empty record message and three dashes");
        var scores = ScoreManager.Instance;
        scores.ResetCurrentScore(); scores.FinalizeRound(); scores.FinalizeRound(); ui.Refresh();
        Check(scores.TopScores.Count == 1 && ui.rankTexts[0].text == "0" && ui.highScoreText.text == "0", "zero points is a valid record; duplicate finalize ignored");
        ClearTestScores();
        yield return ReloadScores();
        scores = ScoreManager.Instance;
        foreach (int value in new[] { 120, 340, 220, 80 })
        {
            scores.ResetCurrentScore(); scores.AddScore(value); scores.FinalizeRound(); scores.FinalizeRound();
        }
        Check(scores.HighScore == 340 && scores.TopScores.SequenceEqual(new[] { 340, 220, 120 }), "highest score and descending Top 3 after four rounds");
        yield return ReloadScores();
        scores = ScoreManager.Instance;
        Check(scores.HighScore == 340 && scores.TopScores.SequenceEqual(new[] { 340, 220, 120 }), "PlayerPrefs restored after manager and scene reload");
        ui = Board();
        Check(!ui.gameObject.activeSelf, "records panel is initially closed");
        var recordsButton = FindObjectsByType<Button>(FindObjectsSortMode.None).Single(b => b.name == "Open Records");
        recordsButton.onClick.Invoke();
        Check(ui.gameObject.activeSelf, "serialized record button opens the panel");
        yield return Capture(ui, 1080, 1920, "01_portrait_9x16.png");
        yield return Capture(ui, 1920, 1080, "02_landscape_16x9.png");
        yield return Capture(ui, 1080, 2340, "03_tall_19.5x9.png");
        ui.GetComponentsInChildren<Button>().Single(b => b.name == "Close Records").onClick.Invoke();
        Check(!ui.gameObject.activeSelf, "serialized close button closes the panel");
        ClearTestScores();
        yield return ReloadScores();
        ui = Board(); ui.Open();
        Check(ui.statusText.text == "아직 기록이 없어요" && ui.rankTexts.All(t => t.text == "-"), "no-record state after reload");
        yield return Capture(ui, 1080, 1920, "04_empty_records.png");
        ui.Close();
        FindObjectsByType<Button>(FindObjectsSortMode.None).Single(b => b.name == "Play").onClick.Invoke();
        yield return new WaitForSeconds(1);
        Check(SceneManager.GetActiveScene().name == "Mid", "restart routes to the W06-derived Mid game");
        Check(FindAnyObjectByType<PlayerController>() != null && FindAnyObjectByType<WaveSpawner>() != null, "W06 player and enemy spawner preserved");
        FindObjectsByType<Button>(FindObjectsSortMode.None).Single(b => b.name == "End Round").onClick.Invoke();
        yield return new WaitForSeconds(.5f);
        Check(SceneManager.GetActiveScene().name == "Mid_GameOver", "end round routes to Mid Game Over");
        FindObjectsByType<Button>(FindObjectsSortMode.None).Single(b => b.name == "Home").onClick.Invoke();
        yield return new WaitForSeconds(.5f);
        Check(SceneManager.GetActiveScene().name == "Mid_MainMenu", "home routes to Mid Main Menu");
    }

    IEnumerator Capture(MidScoreBoardUI ui, int width, int height, string file)
    {
        var canvas = ui.GetComponentInParent<Canvas>();
        var scaler = canvas.GetComponent<CanvasScaler>();
        var go = new GameObject("Capture Camera", typeof(Camera));
        var camera = go.GetComponent<Camera>();
        camera.GetUniversalAdditionalCameraData().renderPostProcessing = false;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color32(248, 244, 252, 255);
        camera.orthographic = true;
        camera.nearClipPlane = .1f; camera.farClipPlane = 100;
        var rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        rt.Create(); camera.targetTexture = rt;
        canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera;
        canvas.planeDistance = 10;
        scaler.enabled = false;
        // Exactly the same Expand scaling as the scene's CanvasScaler.
        canvas.scaleFactor = Mathf.Min(width / 1080f, height / 1920f);
        yield return null; yield return null;
        Canvas.ForceUpdateCanvases();
        ui.GetComponentInChildren<MidRecordsLayout>().Apply();
        foreach (var text in ui.GetComponentsInChildren<TMP_Text>()) text.ForceMeshUpdate();
        yield return new WaitForEndOfFrame();
        var layout = ui.GetComponentInChildren<MidRecordsLayout>();
        var rects = new[] { layout.heading, layout.bestCard, layout.rankingCard, layout.footer };
        var boxes = rects.Select(r => Bounds(r, camera)).ToArray();
        for (int i = 0; i < boxes.Length; i++)
        {
            var box = boxes[i];
            Check(box.xMin >= -1 && box.yMin >= -1 && box.xMax <= width + 1 && box.yMax <= height + 1, file + " inside screen: " + rects[i].name);
            for (int j = i + 1; j < boxes.Length; j++)
                Check(!box.Overlaps(boxes[j]), file + " no overlap: " + rects[i].name + "/" + rects[j].name);
        }
        foreach (var text in ui.GetComponentsInChildren<TMP_Text>())
            Check(!text.isTextOverflowing, file + " text fits: " + text.name);
        RenderPipeline.SubmitRenderRequest(camera, new UniversalRenderPipeline.SingleCameraRequest { destination = rt });
        var previous = RenderTexture.active; RenderTexture.active = rt;
        var image = new Texture2D(width, height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, width, height), 0, 0); image.Apply();
        File.WriteAllBytes(Output + "/" + file, image.EncodeToPNG());
        RenderTexture.active = previous;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.worldCamera = null; scaler.enabled = true;
        camera.targetTexture = null; rt.Release(); Destroy(rt); Destroy(image); Destroy(go);
        yield return null;
    }

    static Rect Bounds(RectTransform rect, Camera camera)
    {
        var corners = new Vector3[4]; rect.GetWorldCorners(corners);
        var min = camera.WorldToScreenPoint(corners[0]); var max = camera.WorldToScreenPoint(corners[2]);
        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }
}
#endif
