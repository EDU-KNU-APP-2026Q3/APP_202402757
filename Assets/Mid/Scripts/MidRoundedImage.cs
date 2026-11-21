using UnityEngine;
using UnityEngine.UI;

// Resolution-independent rounded UI geometry, with a one-screen-pixel soft edge.
[AddComponentMenu("UI/Mid Smooth Rounded Image")]
public class MidRoundedImage : Image
{
    public enum Appearance { Rounded, Window, Tile, PrimaryButton, SecondaryButton }

    public bool capsule;
    public float cornerRadius = 32f;
    public Appearance appearance;
    public Color titleBar = new Color32(194, 208, 255, 255);
    public override Texture mainTexture => Texture2D.whiteTexture;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect rect = GetPixelAdjustedRect();
        if (rect.width <= 0 || rect.height <= 0) return;
        if (appearance != Appearance.Rounded)
        {
            DrawRetro(vh, rect);
            return;
        }

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

    private void DrawRetro(VertexHelper mesh, Rect rect)
    {
        bool window = appearance == Appearance.Window;
        bool button = appearance == Appearance.PrimaryButton || appearance == Appearance.SecondaryButton;
        float edge = Mathf.Min(window ? 6f : 4f, Mathf.Min(rect.width, rect.height) * .14f);
        float step = edge * 2f;
        float shadowOffset = window ? 12f : 6f;
        var shadow = new Rect(rect.position + new Vector2(shadowOffset, -shadowOffset), rect.size);
        MidRetroArt.StepCorners(mesh, shadow, step, MidRetroArt.Shadow);
        MidRetroArt.StepCorners(mesh, rect, step, MidRetroArt.Ink);
        Rect inside = MidRetroArt.Inset(rect, edge);
        Color surface = appearance == Appearance.SecondaryButton ? titleBar : color;
        MidRetroArt.StepCorners(mesh, inside, step, surface);

        if (window)
        {
            float barHeight = Mathf.Min(76f, inside.height * .18f);
            var bar = new Rect(inside.xMin, inside.yMax - barHeight, inside.width, barHeight);
            MidRetroArt.Box(mesh, Rect.MinMaxRect(bar.xMin, bar.yMin, bar.xMax, bar.yMax - step), titleBar);
            MidRetroArt.Box(mesh, new Rect(bar.xMin + edge, bar.yMax - step, bar.width - 2f * edge, edge), titleBar);
            MidRetroArt.Box(mesh, new Rect(bar.xMin + step, bar.yMax - edge, bar.width - 2f * step, edge), titleBar);
            MidRetroArt.Box(mesh, new Rect(bar.xMin, bar.yMin, bar.width, edge), MidRetroArt.Ink);
            if (bar.width > 220f && bar.height > 40f)
            {
                var center = new Vector2(bar.xMin + 40f, bar.center.y);
                MidRetroArt.Stamp(mesh, center, 5f, MidRetroArt.Ink, true);
                MidRetroArt.Stamp(mesh, new Vector2(bar.xMax - 37f, bar.center.y), 5f, MidRetroArt.Ink);
                MidRetroArt.Box(mesh, new Rect(bar.xMax - 77f, bar.center.y - 3f, 6f, 6f), MidRetroArt.Ink);
            }
        }
        else if (button)
        {
            Rect bevel = MidRetroArt.Inset(inside, edge);
            MidRetroArt.Box(mesh, new Rect(bevel.xMin, bevel.yMax - edge, bevel.width, edge),
                Color.Lerp(surface, Color.white, .65f));
            MidRetroArt.Box(mesh, new Rect(bevel.xMin, bevel.yMin, bevel.width, edge),
                Color.Lerp(surface, MidRetroArt.Ink, .25f));
        }
    }
}
