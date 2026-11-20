using TMPro;
using UnityEngine;

public class MidScoreBoardUI : MonoBehaviour
{
    public TMP_Text highScoreText;
    public TMP_Text statusText;
    public TMP_Text[] rankTexts;

    void OnEnable() => Refresh();

    public void Refresh()
    {
        if (highScoreText == null || statusText == null || rankTexts == null) return;
        highScoreText.enableAutoSizing = true;
        highScoreText.fontSizeMin = 48; highScoreText.fontSizeMax = 142;
        var scores = ScoreManager.Instance;
        bool hasRecords = scores != null && scores.TopScores.Count > 0;
        // A completed round with zero points is a record, not an empty slot.
        highScoreText.text = hasRecords ? scores.HighScore.ToString("N0") : "-";
        statusText.text = hasRecords ? "다음 도전에서 기록을 높여 보세요!" : "아직 기록이 없습니다";
        for (int i = 0; i < rankTexts.Length; i++)
        {
            rankTexts[i].enableAutoSizing = true;
            rankTexts[i].fontSizeMin = 24; rankTexts[i].fontSizeMax = 52;
            rankTexts[i].text = hasRecords && i < scores.TopScores.Count
                ? scores.TopScores[i].ToString("N0") : "-";
        }
    }

    public void Open() => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);
}
