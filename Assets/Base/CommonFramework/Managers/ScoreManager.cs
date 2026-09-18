using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 점수, 최고점, Top3 랭킹을 관리하는 매니저. 로컬(PlayerPrefs) 저장.
/// [정상 케이스] AddScore는 라운드 중 여러 번 호출됨. FinalizeRound는 라운드당 정확히 1회 호출.
/// [예외 케이스] 점수 0으로 라운드가 끝나도 정상적으로 랭킹에 기록되어야 함 (0점도 유효한 기록).
///              FinalizeRound를 중복 호출해도 랭킹이 중복 기록되지 않아야 함 -> _finalized 플래그로 방지.
/// </summary>
[DefaultExecutionOrder(-100)]
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    const string KEY_HIGHSCORE = "score_highscore";
    const string KEY_TOP_PREFIX = "score_top_"; // score_top_0, score_top_1, score_top_2

    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }
    public List<int> TopScores { get; private set; } = new List<int>();

    bool _finalized = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // DontDestroyOnLoad는 부모(Managers)의 ManagerRoot.cs가 대표로 한 번만 호출합니다.
        LoadScores();
    }

    void LoadScores()
    {
        HighScore = PlayerPrefs.GetInt(KEY_HIGHSCORE, 0);
        TopScores.Clear();
        for (int i = 0; i < 3; i++)
        {
            // 예외 케이스: 기록이 없는 슬롯은 -1 (UI에서 "기록 없음"으로 표시하도록)
            int v = PlayerPrefs.GetInt(KEY_TOP_PREFIX + i, -1);
            if (v >= 0) TopScores.Add(v);
        }
    }

    public void ResetCurrentScore()
    {
        CurrentScore = 0;
        _finalized = false;
        GameEvents.RaiseScoreChanged(CurrentScore);
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        GameEvents.RaiseScoreChanged(CurrentScore);
    }

    /// <summary>라운드 종료 시 정확히 1회 호출. 최고점/Top3 갱신 및 저장.</summary>
    public void FinalizeRound()
    {
        if (_finalized) return; // 중복 호출 방지
        _finalized = true;

        bool isNewHigh = CurrentScore > HighScore;
        if (isNewHigh)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt(KEY_HIGHSCORE, HighScore);
        }

        TopScores.Add(CurrentScore);
        TopScores = TopScores.OrderByDescending(s => s).Take(3).ToList();
        for (int i = 0; i < TopScores.Count; i++)
            PlayerPrefs.SetInt(KEY_TOP_PREFIX + i, TopScores[i]);

        PlayerPrefs.Save();

        if (isNewHigh) GameEvents.RaiseNewHighScore();
    }
}
