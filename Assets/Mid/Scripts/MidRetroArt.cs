using UnityEngine;
using UnityEngine.UI;

public static class MidRetroArt
{
    public static readonly Color Ink = new Color32(66, 47, 89, 255);
    public static readonly Color Paper = new Color32(255, 251, 255, 255);
    public static readonly Color Blue = new Color32(194, 208, 255, 255);
    public static readonly Color Lilac = new Color32(221, 201, 249, 255);
    public static readonly Color Pink = new Color32(255, 201, 228, 255);
    public static readonly Color Mint = new Color32(199, 238, 227, 255);
    public static readonly Color Gold = new Color32(255, 228, 147, 255);
    internal static readonly Color Shadow = new Color32(100, 68, 142, 82);

    private static readonly string[] Sparkle =
    {
        "..#..",
        "..#..",
        "#####",
        "..#..",
        "..#.."
    };

    private static readonly string[] Heart =
    {
        ".##.##.",
        "#######",
        "#######",
        ".#####.",
        "..###..",
        "...#..."
    };

    internal static void Box(VertexHelper mesh, Rect rect, Color color) => Gradient(mesh, rect, color, color);

    internal static void Gradient(VertexHelper mesh, Rect rect, Color bottom, Color top)
    {
        if (rect.width <= 0f || rect.height <= 0f) return;
        int first = mesh.currentVertCount;
        Vector2 uv = new Vector2(.5f, .5f);
        mesh.AddVert(new Vector2(rect.xMin, rect.yMin), bottom, uv);
        mesh.AddVert(new Vector2(rect.xMin, rect.yMax), top, uv);
        mesh.AddVert(new Vector2(rect.xMax, rect.yMax), top, uv);
        mesh.AddVert(new Vector2(rect.xMax, rect.yMin), bottom, uv);
        mesh.AddTriangle(first, first + 1, first + 2);
        mesh.AddTriangle(first, first + 2, first + 3);
    }

    internal static Rect Inset(Rect rect, float inset) =>
        Rect.MinMaxRect(rect.xMin + inset, rect.yMin + inset, rect.xMax - inset, rect.yMax - inset);

    internal static void StepCorners(VertexHelper mesh, Rect rect, float step, Color color)
    {
        step = Mathf.Min(step, Mathf.Min(rect.width, rect.height) / 2f);
        float half = step * .5f;
        Box(mesh, Rect.MinMaxRect(rect.xMin, rect.yMin + step, rect.xMax, rect.yMax - step), color);
        Box(mesh, new Rect(rect.xMin + half, rect.yMin + half, rect.width - step, half), color);
        Box(mesh, new Rect(rect.xMin + half, rect.yMax - step, rect.width - step, half), color);
        Box(mesh, new Rect(rect.xMin + step, rect.yMin, rect.width - 2f * step, half), color);
        Box(mesh, new Rect(rect.xMin + step, rect.yMax - half, rect.width - 2f * step, half), color);
    }

    internal static void Stamp(VertexHelper mesh, Vector2 center, float pixel, Color color, bool heart = false)
    {
        string[] pattern = heart ? Heart : Sparkle;
        float left = center.x - pattern[0].Length * pixel * .5f;
        float top = center.y + pattern.Length * pixel * .5f;
        for (int row = 0; row < pattern.Length; row++)
        {
            for (int column = 0; column < pattern[row].Length; column++)
            {
                if (pattern[row][column] != '#') continue;
                Box(mesh, new Rect(left + column * pixel, top - (row + 1) * pixel, pixel, pixel), color);
            }
        }
    }
}
