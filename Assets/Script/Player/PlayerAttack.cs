using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Player player;

    [SerializeField]Weapon mainWeapon;
    [SerializeField]Weapon subWeapon;
   public Weapon MainWeapon=>mainWeapon;
   public Weapon SubWeapon=>subWeapon;

    [Header("Attack Data Settings")]
    [SerializeField] private PlayerAttackData mainAttackData;
    [SerializeField] private PlayerAttackData subAttackData;
    public PlayerAttackData MainAttackData => mainAttackData;
    public PlayerAttackData SubAttackData => subAttackData;
    [SerializeField] private PlayerAttackData defaultMainAttackData;
    [SerializeField] private PlayerAttackData defaultSubAttackData;
    public PlayerAttackData DefaultMainAttackData => defaultMainAttackData;
    public PlayerAttackData DefaultSubAttackData => defaultSubAttackData;

    

    private void Awake()
    {
        player = Player.Instance;

    }
    private void OnEnable()
    {
        
        player.EquipmentManager.OnWeaponChanged += SetCurrentAttackData;

    }
    public LightAttackStepData[] GetLightAttackStepData()
    {
        if (player != null && player.EquipmentManager != null)
        {
            WeaponItemData weapon = player.EquipmentManager.GetMainWeaponData();

            if (weapon != null && weapon.playerAttackData != null)
            {
                return weapon.playerAttackData.lightAttackSteps;
            }
        }

        // 무기가 없거나, 데이터가 없거나, EquipmentManager가 아직 Null이면 맨손 기본 데이터 반환
        return defaultMainAttackData != null ? defaultMainAttackData.lightAttackSteps : null;
    }
    public HeavyAttackStepData[] GetHeavyAttackStepData()
    {
        if (player != null && player.EquipmentManager != null)
        {
            WeaponItemData weapon = player.EquipmentManager.GetMainWeaponData();

            if (weapon != null && weapon.playerAttackData != null)
            {
                return weapon.playerAttackData.heavyAttackSteps;
            }
        }

        return defaultMainAttackData != null ? defaultMainAttackData.heavyAttackSteps : null;
    }
    public void SetCurrentAttackData(EquipmentSlot slot, WeaponItemData data)
    {
        // 1. EquipmentManager를 통해 현재 장착된 무기 데이터 가져오기
        if (slot == EquipmentSlot.MainWeapon)
        {
            mainAttackData = data.playerAttackData;
        }
        
        if (slot == EquipmentSlot.SubWeapon) 
        {
            subAttackData = data.playerAttackData;
        }

    }
}
