using UnityEngine;
using UnityEngine.EventSystems;

public class UIZoomAndScroll : MonoBehaviour, IScrollHandler
{
    [Header("Zoom Settings")]
    [SerializeField] private RectTransform contentRect; // ScrollView의 Content
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 0.5f;
    [SerializeField] private float maxZoom = 2.5f;

    public void OnScroll(PointerEventData eventData)
    {
        if (contentRect == null) return;

        // 마우스 휠 입력값 감지 (위로 밀면 양수, 아래로 당기면 음수)
        float scrollDelta = eventData.scrollDelta.y;
        if (Mathf.Abs(scrollDelta) < 0.01f) return;

        // 현재 Scale 계산
        Vector3 currentScale = contentRect.localScale;
        float targetScale = Mathf.Clamp(currentScale.x + (scrollDelta * zoomSpeed), minZoom, maxZoom);

        // 마우스 커서 위치를 기준점으로 삼기 위한 위치 보정
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            contentRect,
            eventData.position,
            eventData.pressEventCamera,
            out mousePos
        );

        Vector3 scaleChange = new Vector3(targetScale, targetScale, 1f) - currentScale;

        // Scale 적용 및 커서 위치 중심으로 Pivot 이동 보정
        contentRect.localScale = new Vector3(targetScale, targetScale, 1f);
        contentRect.anchoredPosition -= Vector2.Scale(mousePos, scaleChange);
    }
}