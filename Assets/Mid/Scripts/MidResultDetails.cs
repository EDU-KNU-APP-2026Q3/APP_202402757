using TMPro;
using UnityEngine;

public class MidResultDetails : MonoBehaviour
{
    public TMP_Text defeated;
    public TMP_Text hits;

    private void OnEnable()
    {
        defeated.text = MidRoundStats.EnemiesDefeated.ToString("N0");
        hits.text = MidRoundStats.HitsTaken.ToString("N0");
    }
}
