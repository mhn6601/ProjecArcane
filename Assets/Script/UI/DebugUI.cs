using UnityEngine;
using UnityEngine.UI;

public class DebugUI : UIBase
{
    [SerializeField] private Button CloseButton;
    [SerializeField] private Button restoreHealthButton;
    [SerializeField] private Button restoreManaButton;
    [SerializeField] private Button AddAllSpecialNodeButton;

    private PlayerResourceSystem resource;

    private void OnEnable()
    {
        CloseButton.onClick.AddListener(Close);
        restoreHealthButton.onClick.AddListener(RestoreHealth);
        restoreManaButton.onClick.AddListener(RestoreMana);
        AddAllSpecialNodeButton.onClick.AddListener(AddAllSpecialNodes);
    }

    private void OnDisable()
    {
        CloseButton.onClick.RemoveListener(Close);
        restoreHealthButton.onClick.RemoveListener(RestoreHealth);
        restoreManaButton.onClick.RemoveListener(RestoreMana);
        AddAllSpecialNodeButton.onClick.RemoveListener(AddAllSpecialNodes);
    }

    public override void Open()
    {
        base.Open();
        GamePause.Push();
    }

    public override void Close()
    {
        base.Close();
        GamePause.Pop();
    }

    private PlayerResourceSystem Resource
    {
        get
        {
            if (resource == null)
                resource = Player.Instance.ResourceSystem;
            return resource;
        }
    }

    private void RestoreHealth()
    {
        if (Resource == null) return;
        Resource.HealHP(Resource.MaxHealth);
    }

    private void RestoreMana()
    {
        if (Resource == null) return;
        Resource.AddMana(Resource.MaxMana);
    }
    private void AddAllSpecialNodes()
    {
        SpecialNodeInventoryManager.Instance.AddAllSpecialNodes();
    }
}