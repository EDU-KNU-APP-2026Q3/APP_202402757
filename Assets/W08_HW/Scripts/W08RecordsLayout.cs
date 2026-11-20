using UnityEngine;

[ExecuteAlways]
public class W08RecordsLayout : MonoBehaviour
{
    public RectTransform heading;
    public RectTransform bestCard;
    public RectTransform rankingCard;
    public RectTransform footer;
    public RectTransform panel;

    void LateUpdate() => Apply();

    public void Apply()
    {
        var size = ((RectTransform)transform).rect.size;
        if (size.x < 1 || size.y < 1 || heading == null) return;
        // The plan keeps a portrait panel centered even on wide screens.
        float width = Mathf.Min(size.x - 120f, 900f);
        if (panel != null) Place(panel, Vector2.one * .5f, Vector2.zero, new Vector2(width, 1560));
        Place(heading, new Vector2(.5f, 1), new Vector2(0, -155), new Vector2(width - 100, 170));
        Place(bestCard, Vector2.one * .5f, new Vector2(0, 345), new Vector2(width - 100, 240));
        Place(rankingCard, Vector2.one * .5f, new Vector2(0, -105), new Vector2(width - 100, 560));
        Place(footer, new Vector2(.5f, 0), new Vector2(0, 180), new Vector2(width - 100, 200));
    }

    static void Place(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = anchor;
        rect.pivot = Vector2.one * .5f;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }
}
