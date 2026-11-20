using UnityEngine;

[ExecuteAlways]
public class MidRecordsLayout : MonoBehaviour
{
    public RectTransform heading;
    public RectTransform bestCard;
    public RectTransform rankingCard;
    public RectTransform footer;

    void LateUpdate() => Apply();

    public void Apply()
    {
        var size = ((RectTransform)transform).rect.size;
        if (size.x < 1 || size.y < 1 || heading == null) return;
        bool landscape = size.x > size.y;
        float width = Mathf.Min(size.x - 120f, landscape ? 2600f : 920f);
        Place(heading, new Vector2(.5f, 1), new Vector2(0, -170), new Vector2(width, 180));
        Place(footer, new Vector2(.5f, 0), new Vector2(0, 155), new Vector2(width, 170));
        if (landscape)
        {
            float left = width * .36f;
            float right = width - left - 70;
            Place(bestCard, Vector2.one * .5f, new Vector2(-(right + 70) / 2, 0), new Vector2(left, 950));
            Place(rankingCard, Vector2.one * .5f, new Vector2((left + 70) / 2, 0), new Vector2(right, 950));
        }
        else
        {
            Place(bestCard, Vector2.one * .5f, new Vector2(0, 445), new Vector2(width, 360));
            Place(rankingCard, Vector2.one * .5f, new Vector2(0, -185), new Vector2(width, 780));
        }
    }

    static void Place(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = anchor;
        rect.pivot = Vector2.one * .5f;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }
}
