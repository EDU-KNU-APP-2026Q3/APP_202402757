using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Mid Retro Wallpaper")]
[RequireComponent(typeof(CanvasRenderer))]
public class MidRetroWallpaper : MaskableGraphic
{
    public bool inset;
    public bool results;

    protected override void OnPopulateMesh(VertexHelper mesh)
    {
        mesh.Clear();
        Rect rect = GetPixelAdjustedRect();
        if (rect.width < 1f || rect.height < 1f) return;
        if (inset)
        {
            MidRetroArt.Box(mesh, rect, MidRetroArt.Ink);
            rect = MidRetroArt.Inset(rect, 5f);
        }

        Color bottom = results ? new Color32(194, 163, 225, 255) :
            inset ? new Color32(184, 214, 255, 255) : new Color32(189, 199, 247, 255);
        Color middle = results ? new Color32(209, 194, 240, 255) : MidRetroArt.Lilac;
        Color top = results ? new Color32(163, 182, 231, 255) :
            inset ? new Color32(255, 219, 238, 255) : new Color32(251, 200, 229, 255);
        Rect lower = Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMax, rect.center.y);
        Rect upper = Rect.MinMaxRect(rect.xMin, rect.center.y, rect.xMax, rect.yMax);
        MidRetroArt.Gradient(mesh, lower, bottom, middle);
        MidRetroArt.Gradient(mesh, upper, middle, top);

        float spacing = Mathf.Max(inset ? 44f : 76f, Mathf.Max(rect.width, rect.height) / 100f);
        Color grid = new Color(1f, 1f, 1f, inset ? .37f : .56f);
        for (float x = rect.xMin + spacing; x < rect.xMax - 2f; x += spacing)
            MidRetroArt.Box(mesh, new Rect(x, rect.yMin, 2f, rect.height), grid);
        for (float y = rect.yMin + spacing; y < rect.yMax - 2f; y += spacing)
            MidRetroArt.Box(mesh, new Rect(rect.xMin, y, rect.width, 2f), grid);

        if (inset)
        {
            StampAt(mesh, rect, .16f, .66f, 8f, MidRetroArt.Gold);
            StampAt(mesh, rect, .84f, .31f, 8f, Color.white);
            StampAt(mesh, rect, .76f, .77f, 5f, Color.white);
            return;
        }

        StampAt(mesh, rect, .13f, .91f, 9f, MidRetroArt.Gold);
        StampAt(mesh, rect, .84f, .84f, 7f, MidRetroArt.Paper);
        StampAt(mesh, rect, .15f, .24f, 6f, MidRetroArt.Paper);
        StampAt(mesh, rect, .85f, .15f, 8f, MidRetroArt.Gold);
        StampAt(mesh, rect, .28f, .07f, 7f, MidRetroArt.Pink, true);
        StampAt(mesh, rect, .71f, .95f, 5f, MidRetroArt.Paper);
    }

    private static void StampAt(VertexHelper mesh, Rect rect, float x, float y, float size, Color color, bool heart = false)
    {
        float pixel = Mathf.Min(size, Mathf.Min(rect.width, rect.height) / 30f);
        Vector2 point = Rect.NormalizedToPoint(rect, new Vector2(x, y));
        MidRetroArt.Stamp(mesh, point + new Vector2(pixel, -pixel), pixel, MidRetroArt.Shadow, heart);
        MidRetroArt.Stamp(mesh, point, pixel, color, heart);
    }
}
