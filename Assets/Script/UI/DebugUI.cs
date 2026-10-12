using UnityEngine;
using UnityEngine.UI;

public class DebugUI : UIBase
{
    [SerializeField] private Button restoreHealthButton;
    [SerializeField] private Button restoreManaButton;

    private PlayerResourceSystem resource;

    private void OnEnable()
    {
        restoreHealthButton.onClick.AddListener(RestoreHealth);
        restoreManaButton.onClick.AddListener(RestoreMana);
    }

    private void OnDisable()
    {
        restoreHealthButton.onClick.RemoveListener(RestoreHealth);
        restoreManaButton.onClick.RemoveListener(RestoreMana);
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
}