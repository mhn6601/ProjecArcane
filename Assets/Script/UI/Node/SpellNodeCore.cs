using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static TreeEditor.TreeEditorHelper;

public class SpellNodeCore : SpellNodeBase
{
    override protected  void BindProgress() { }
    override protected void UnbindProgress() { }

    // 자식이 현재 상태에 맞게 잠금 아이콘, 흑백, 장착 표시 등을 갱신
    override  protected void RefreshVisual() {; }

}