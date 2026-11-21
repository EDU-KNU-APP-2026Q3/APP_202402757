using UnityEngine;

public class MidRoundStats : MonoBehaviour
{
    public static int EnemiesDefeated { get; private set; }
    public static int HitsTaken { get; private set; }

    private void Awake()
    {
        EnemiesDefeated = 0;
        HitsTaken = 0;
    }

    private void OnEnable()
    {
        GameEvents.OnEnemyDefeated += CountEnemy;
        GameEvents.OnPlayerHit += CountHit;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyDefeated -= CountEnemy;
        GameEvents.OnPlayerHit -= CountHit;
    }

    private void CountEnemy() => EnemiesDefeated++;
    private void CountHit() => HitsTaken++;
}
