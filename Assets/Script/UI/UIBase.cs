using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    [field: SerializeField]
    public UIType uIType { get; protected set; }

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
    public virtual void Focus()
    {
        transform.SetAsLastSibling();
    }
    public void OnClickUI()
    {
        UIManager.Instance.FocusUI(this);
    }
    
}
