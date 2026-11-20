using UnityEngine;
using UnityEngine.UI;

// Resolution-independent rounded UI geometry, with a one-screen-pixel soft edge.
[AddComponentMenu("UI/W08 Smooth Rounded Image")]
public class W08RoundedImage : Image
{
    public bool capsule;
    public float cornerRadius = 32f;
    public override Texture mainTexture => Texture2D.whiteTexture;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect rect = GetPixelAdjustedRect();
        if (rect.width <= 0 || rect.height <= 0) return;
        float radius = capsule ? Mathf.Min(rect.width, rect.height) * .5f
            : Mathf.Min(cornerRadius, Mathf.Min(rect.width, rect.height) * .5f);
        float feather = Mathf.Min(1f / Mathf.Max(.01f, canvas != null ? canvas.scaleFactor : 1f), radius);
        const int segments = 32;
        const int count = 4 * (segments + 1);
        Color solid = color;
        Color clear = color; clear.a = 0;
        vh.AddVert(rect.center, solid, new Vector2(.5f, .5f));
        for (int corner = 0; corner < 4; corner++)
        {
            Vector2 center = new Vector2(corner < 2 ? rect.xMax - radius : rect.xMin + radius,
                corner == 0 || corner == 3 ? rect.yMin + radius : rect.yMax - radius);
            for (int i = 0; i <= segments; i++)
            {
                float angle = (-90f + corner * 90f + i * 90f / segments) * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                vh.AddVert(center + direction * (radius - feather), solid, new Vector2(.5f, .5f));
                vh.AddVert(center + direction * radius, clear, new Vector2(.5f, .5f));
            }
        }
        for (int i = 0; i < count; i++)
        {
            int a = 1 + i * 2, b = 1 + ((i + 1) % count) * 2;
            vh.AddTriangle(0, a, b);
            vh.AddTriangle(a, a + 1, b + 1);
            vh.AddTriangle(a, b + 1, b);
        }
    }
}
