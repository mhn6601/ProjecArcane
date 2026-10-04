#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpellTreeManager))]
public class SpellTreeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SpellTreeManager manager = (SpellTreeManager )target;

        GUILayout.Space(10);
        if (GUILayout.Button("선 연결 상태 즉시 확인 (Refresh Lines)", GUILayout.Height(30)))
        {
            manager.RefreshSpellTreeLines();
            EditorUtility.SetDirty(manager);
        }
    }
}
#endif