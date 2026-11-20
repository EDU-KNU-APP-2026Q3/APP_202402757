using UnityEngine;

// Fit the world-space backdrop to the camera, preserving the sprite's aspect ratio.
[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class MidGameBackground : MonoBehaviour
{
    public Camera targetCamera;
    void OnEnable() => Apply();
    void LateUpdate() => Apply();
    public void Apply()
    {
        var camera = targetCamera != null ? targetCamera : Camera.main;
        var renderer = GetComponent<SpriteRenderer>();
        if (camera == null || !camera.orthographic || renderer.sprite == null) return;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color32(248, 244, 252, 255);
        renderer.color = Color.white;
        Vector2 spriteSize = renderer.sprite.bounds.size;
        float height = camera.orthographicSize * 2f;
        float width = height * camera.aspect;
        float scale = Mathf.Max(width / spriteSize.x, height / spriteSize.y);
        transform.localScale = new Vector3(scale, scale, 1f);
        Vector3 spriteCenter = renderer.sprite.bounds.center;
        transform.position = new Vector3(camera.transform.position.x - spriteCenter.x * scale,
            camera.transform.position.y - spriteCenter.y * scale, transform.position.z);
    }
}
