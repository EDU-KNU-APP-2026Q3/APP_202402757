using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 전환을 전담하는 매니저. 학생은 씬 이름을 몰라도 이 함수들만 호출하면 됩니다.
/// 씬 이름은 Unity Build Settings에 등록된 이름과 정확히 일치해야 합니다.
/// [정상 케이스] 씬 이름이 Build Settings에 등록되어 있음
/// [예외 케이스] 등록 안 된 씬 이름 호출 시 Console에 명확한 에러 로그를 남김 (조용히 실패하지 않음)
/// </summary>
[DefaultExecutionOrder(-100)]
public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance { get; private set; }

    [SerializeField] string mainMenuScene = "MainMenu";
    [SerializeField] string gamePlayScene = "GamePlay";
    [SerializeField] string gameOverScene = "GameOver";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // DontDestroyOnLoad는 부모(Managers)의 ManagerRoot.cs가 대표로 한 번만 호출합니다.
    }

    public void GoToMainMenu() => LoadScene(mainMenuScene);
    public void GoToGamePlay() => LoadScene(gamePlayScene);
    public void GoToGameOver() => LoadScene(gameOverScene);

    void LoadScene(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"[SceneFlowManager] '{sceneName}' 씬이 Build Settings에 등록되어 있지 않습니다. " +
                            "File > Build Settings에서 씬을 추가해주세요.");
        }
    }
}
