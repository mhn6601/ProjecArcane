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
    private void OnEnable()
    {
        player.EquipmentManager.OnWeaponChanged += SetWeapon;
    }
    private void Start()
    {
    }
    public LightAttackStepData[] GetLightAttackStepData()
    {
        if (MainWeapon)
        {
            return MainWeapon.WeaponItemData.playerAttackData.lightAttackSteps;
        }
        else
        {
            player.EquipmentManager.EquipmentDefaultWeapons(MainWeapon,SubWeapon);
            return MainWeapon.WeaponItemData.playerAttackData.lightAttackSteps;
            //return (player.EquipmentManager.DefaultMainWeaponData as WeaponItemData).playerAttackData.lightAttackSteps;
        }
    }
    public HeavyAttackStepData[] GetHeavyAttackStepData()
    {
        if (MainWeapon)
        {
            return MainWeapon.WeaponItemData.playerAttackData.heavyAttackSteps;
        }
        else
        {
            player.EquipmentManager.EquipmentDefaultWeapons(MainWeapon, SubWeapon);
            return MainWeapon.WeaponItemData.playerAttackData.heavyAttackSteps;
            // return (player.EquipmentManager.DefaultMainWeaponData as WeaponItemData).playerAttackData.heavyAttackSteps;
        }
    }
    private void SetWeapon(EquipmentSlotType slot,WeaponItemData data)
    {
        if (data== null)
        {
            return;            
        }
        if (slot == EquipmentSlotType.MainWeapon)
        {
            MainWeapon = player.EquipmentManager.MainWeapon; 
            player.Controller.OnMainWeaponChange(
            data.playerAttackData);
        }
        else if (slot == EquipmentSlotType.SubWeapon)
        {
            SubWeapon = player.EquipmentManager.SubWeapon;
            player.Controller.OnSubWeaponChange(
            data.playerAttackData);
        }
    }
    public float GetSubWeaponParryStamina()
    {
        return SubWeapon.WeaponItemData.playerAttackData.parryStamina;
    }
    public bool GetSubWeaponCanParry()
    {
        return SubWeapon.WeaponItemData.playerAttackData.canParry;
    }
    public void EnableHitBox()
    {
        if (MainWeapon.WeaponHitBox != null)
            MainWeapon.WeaponHitBox.EnableHitBox();
        else
            Debug.Log("MainWeapon.WeaponHitBox==null");
    }

    public void DisableHitBox()
    {
        if (MainWeapon.WeaponHitBox != null)
            MainWeapon.WeaponHitBox.DisableHitBox();
        else
        Debug.Log("MainWeapon.WeaponHitBox==null");
    }
}
