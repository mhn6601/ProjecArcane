using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class SpellNodeLineRenderer : Graphic
{
    [Header("Line Settings")]
    [SerializeField] private float thickness = 4f; // 선 두께

    // 연결할 두 노드의 AnchoredPosition 쌍을 담는 리스트
    private readonly List<(Vector2 start, Vector2 end)> lineSegments = new List<(Vector2, Vector2)>();

    public float Thickness
    {
        get => thickness;
        set { thickness = value; SetVerticesDirty(); }
    }

    /// <summary>
    /// 외부(스킬 트리 매니저 등)에서 노드 리스트를 전달받아 1:N 선 좌표를 일괄 등록
    /// </summary>
    public void SetLines(List<SpellNodeBase> spellNodes)
    {
        lineSegments.Clear();
        if (spellNodes == null) return;

        List<(Vector2, Vector2)> newLines = new List<(Vector2, Vector2)>();
        RectTransform lineRendererRect = rectTransform;
        foreach (var node in spellNodes)
        {
            if (node == null || node.childNodes == null || node.childNodes.Count == 0)
                continue;

            // 1. 부모 노드의 월드 좌표를 라인 렌더러 기준 로컬 좌표로 변환
            Vector3 startWorldPos = node.RectTransform.position;
            Vector2 startPos = lineRendererRect.InverseTransformPoint(startWorldPos);

            foreach (var childTransform in node.childNodes)
            {
                if (childTransform == null) continue;

                // 자식 노드의 RectTransform에서 anchoredPosition 추출
                if (childTransform.TryGetComponent<RectTransform>(out var childRect))
                {
                    // 2. 자식 노드의 월드 좌표를 라인 렌더러 기준 로컬 좌표로 변환
                    Vector3 childWorldPos = childTransform.position;
                    Vector2 endPos = lineRendererRect.InverseTransformPoint(childWorldPos);
                    newLines.Add((startPos, endPos));
                }
            }
        }

        lineSegments.AddRange(newLines);
        SetVerticesDirty(); // UGUI 엔진에 메쉬 재연산 요청
    }

    public void ClearLines()
    {
        lineSegments.Clear();
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (lineSegments.Count == 0) return;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        for (int i = 0; i < lineSegments.Count; i++)
        {
            Vector2 start = lineSegments[i].start;
            Vector2 end = lineSegments[i].end;

            Vector2 dir = (end - start).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x) * (thickness * 0.5f);

            Vector2 v1 = start - normal;
            Vector2 v2 = start + normal;
            Vector2 v3 = end + normal;
            Vector2 v4 = end - normal;

            int indexOffset = vh.currentVertCount;

            vertex.position = v1; vh.AddVert(vertex);
            vertex.position = v2; vh.AddVert(vertex);
            vertex.position = v3; vh.AddVert(vertex);
            vertex.position = v4; vh.AddVert(vertex);

            vh.AddTriangle(indexOffset + 0, indexOffset + 1, indexOffset + 2);
            vh.AddTriangle(indexOffset + 2, indexOffset + 3, indexOffset + 0);
        }
    }
}