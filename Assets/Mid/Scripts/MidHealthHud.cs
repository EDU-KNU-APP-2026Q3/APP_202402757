using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MidHealthHud : MonoBehaviour
{
    [SerializeField] private Color emptyHeart = new Color(.53f, .43f, .64f, .35f);
    public PlayerHealth player;
    public EnemyChase enemy;
    public RectTransform heartRow;
    public Sprite heartSprite;
    public TMP_Text remainingText;

    private void OnEnable() => GameEvents.OnPlayerHit += Refresh;
    private void OnDisable() => GameEvents.OnPlayerHit -= Refresh;
    private void Start() => Refresh();

    private void Refresh() => DisplayHealth(player.CurrentHealth);

    public void DisplayHealth(int health)
    {
        int damage = Mathf.Max(1, enemy.contactDamage);
        int maximum = ToHearts(player.maxHealth, damage);
        int remaining = Mathf.Min(maximum, ToHearts(health, damage));

        while (heartRow.childCount < maximum)
        {
            var slot = new GameObject("Heart " + (heartRow.childCount + 1), typeof(RectTransform), typeof(Image));
            slot.transform.SetParent(heartRow, false);
            var image = slot.GetComponent<Image>();
            image.sprite = heartSprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
        }

        for (int i = 0; i < heartRow.childCount; i++)
        {
            var slot = heartRow.GetChild(i);
            slot.gameObject.SetActive(i < maximum);
            slot.GetComponent<Image>().color = i < remaining ? Color.white : emptyHeart;
        }
        remainingText.text = remaining + "/" + maximum;
    }

    private static int ToHearts(int health, int damage) => health <= 0 ? 0 : 1 + (health - 1) / damage;
}
