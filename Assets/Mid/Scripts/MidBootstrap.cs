using UnityEngine;

// Mid scenes use their own manager prefab and scene routes; W06 remains intact.
[DefaultExecutionOrder(-250)]
public class MidBootstrap : MonoBehaviour
{
    public GameObject managersPrefab;
    public bool startRound;

    void Awake()
    {
        if (GameManager.Instance == null && managersPrefab != null)
            Instantiate(managersPrefab);
    }

    void Start()
    {
        if (!startRound) return;
        ScoreManager.Instance.ResetCurrentScore();
        GameManager.Instance.SetState(GameState.Playing);
    }
}
