using UnityEngine;

public enum ItemType
{
    Equipment,
    Consumable,
    Material,
    Misc
}
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemId;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public ItemType type;
    [Header("인벤토리 연동")]
    public bool isStackable=false;
    public int maxStackSize=1;
}
