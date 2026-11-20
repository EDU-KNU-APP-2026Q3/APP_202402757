using TMPro;
using UnityEngine;

public class W08Actions : MonoBehaviour
{
    public TMP_Text roundScoreText;

    void Start()
    {
        if (roundScoreText != null && ScoreManager.Instance != null)
            roundScoreText.text = "이번 점수  " + ScoreManager.Instance.CurrentScore.ToString("N0");
    }

    public void StartGame() => GameManager.Instance.RestartGame();
    public void MainMenu() => GameManager.Instance.QuitToMainMenu();
    public void EndGame()
    {
        ScoreManager.Instance.FinalizeRound();
        GameManager.Instance.SetState(GameState.GameOver);
        SceneFlowManager.Instance.GoToGameOver();
    }
}
