using UnityEditor;
using UnityEngine;

// true: SpellNode, StatNode, SpellSpecialNode 등 모든 자식 클래스에 적용
[CustomEditor(typeof(SpellNodeBase), true)]
public class SpellNodeBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (!Application.isPlaying)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("해금 상태는 플레이 중에만 표시됩니다.", MessageType.None);
            return;
        }

        var node = (SpellNodeBase)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime State", EditorStyles.boldLabel);

        // 읽기 전용으로 표시
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.Toggle("Is Unlocked", node.IsUnlocked);
            EditorGUILayout.Toggle("Is Parent Unlocked", node.IsParentUnlocked);
            EditorGUILayout.Toggle("Can Unlock", node.CanUnlock);
            EditorGUILayout.EnumPopup("State", node.State);
        }
    }

    // 플레이 중에는 해금 상태가 바뀌어도 인스펙터가 바로 갱신되도록 매 프레임 다시 그림
    public override bool RequiresConstantRepaint()
    {
        return Application.isPlaying;
    }
}