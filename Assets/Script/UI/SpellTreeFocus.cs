using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpellTreeFocus : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float duration = 0.25f;

    private RectTransform Content => scrollRect.content;
    private RectTransform Viewport => scrollRect.viewport != null
        ? scrollRect.viewport
        : (RectTransform)scrollRect.transform;

    private Coroutine routine;

    /// <summary>해당 노드의 중심이 뷰포트 중심에 오도록 Content 이동</summary>
    public void FocusOn(RectTransform node)
    {
        if (node == null) return;

        // 노드 중심과 뷰포트 중심의 월드 좌표 차이를 Content 부모 공간의 이동량으로 변환
        Vector3 nodeWorld = node.TransformPoint(node.rect.center);
        Vector3 viewWorld = Viewport.TransformPoint(Viewport.rect.center);
        Vector2 delta = Content.parent.InverseTransformVector(viewWorld - nodeWorld);

        Vector2 target = Content.anchoredPosition + delta;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(MoveTo(target));
    }

    public void StopFocus()
    {
        if (routine != null) StopCoroutine(routine);
        routine = null;
    }

    private IEnumerator MoveTo(Vector2 target)
    {
        scrollRect.StopMovement();   // 관성 중이던 스크롤 정지
        Vector2 start = Content.anchoredPosition;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(0.01f, duration);   // 일시정지(timeScale 0)에서도 동작
            float eased = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);    // ease-out
            Content.anchoredPosition = Vector2.LerpUnclamped(start, target, eased);
            yield return null;
        }

        Content.anchoredPosition = target;
        routine = null;
    }
}