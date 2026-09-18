using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Player player;
    public Weapon MainWeapon;
    public Weapon SubWeapon ;

    

    private void Awake()
    {
        
        player = Player.Instance;

    }
    private void Start()
    {

        player.EquipmentManager.EquipmentDefaultWeapons(MainWeapon != null, SubWeapon != null);
    }
    private void OnEnable()
    {
        
        player.EquipmentManager.OnWeaponChanged += SetCurrentAttackData;

    }
    public LightAttackStepData[] GetLightAttackStepData()
    {
        if (MainWeapon)
            return MainWeapon.WeaponItemData.playerAttackData.lightAttackSteps;
        else
            return null;
    }
    public HeavyAttackStepData[] GetHeavyAttackStepData()
    {
        if (MainWeapon)
            return MainWeapon.WeaponItemData.playerAttackData.heavyAttackSteps;
        else
            return null;
    }
    public void SetCurrentAttackData( WeaponItemData data)
    {
        player.Controller.OnMainWeaponChange(MainWeapon.WeaponItemData.playerAttackData);
        
    }
    public float GetSubWeaponParryStamina()
    {
        return SubWeapon.WeaponItemData.playerAttackData.parryStamina;
    }
    public bool GetSubWeaponCanParry()
    {
        return SubWeapon.WeaponItemData.playerAttackData.canParry;
    }
}
