using UnityEngine;

// W08 scenes use their own manager prefab and scene routes; W06 remains intact.
[DefaultExecutionOrder(-250)]
public class W08Bootstrap : MonoBehaviour
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
