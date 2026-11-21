using UnityEngine;
using UnityEngine.EventSystems;

public class MidJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, ICancelHandler
{
    public PlayerController player;
    public RectTransform handle;
    private int? pointer;

    public void OnPointerDown(PointerEventData data)
    {
        if (pointer.HasValue) return;
        pointer = data.pointerId;
        OnDrag(data);
    }

    public void OnDrag(PointerEventData data)
    {
        if (pointer != data.pointerId || GameManager.Instance?.CurrentState != GameState.Playing) return;
        var area = (RectTransform)transform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                area, data.position, data.pressEventCamera, out var position)) return;
        float radius = Mathf.Max(1f, (area.rect.width - handle.rect.width) / 2f);
        var input = Vector2.ClampMagnitude(position / radius, 1f);
        handle.anchoredPosition = input * radius;
        if (player != null) player.SetMoveInput(input);
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (pointer == data.pointerId) ResetStick();
    }

    public void OnCancel(BaseEventData data) => ResetStick();
    private void OnDisable() => ResetStick();

    private void OnApplicationFocus(bool focused)
    {
        if (!focused) ResetStick();
    }

    private void ResetStick()
    {
        if (!pointer.HasValue) return;
        pointer = null;
        handle.anchoredPosition = Vector2.zero;
        if (player != null) player.SetMoveInput(Vector2.zero);
    }
}
