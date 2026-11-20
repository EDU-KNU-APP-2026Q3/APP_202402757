using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class W08SafeArea : MonoBehaviour
{
    void LateUpdate()
    {
        var canvas = GetComponentInParent<Canvas>();
        var rect = (RectTransform)transform;
        // Offscreen verification uses the full target texture, not the monitor's safe area.
        bool offscreen = canvas != null && canvas.worldCamera != null && canvas.worldCamera.targetTexture != null;
        Rect area = offscreen ? new Rect(0, 0, 1, 1) : Screen.safeArea;
        Vector2 min = offscreen ? Vector2.zero : new Vector2(area.xMin / Mathf.Max(1, Screen.width), area.yMin / Mathf.Max(1, Screen.height));
        Vector2 max = offscreen ? Vector2.one : new Vector2(area.xMax / Mathf.Max(1, Screen.width), area.yMax / Mathf.Max(1, Screen.height));
        if (rect.anchorMin != min || rect.anchorMax != max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
