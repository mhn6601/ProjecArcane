using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Player player;

    [Header("Attack Data Settings")]
    [SerializeField] private PlayerAttackData attackData;
    private void Awake()
    {
        player = Player.Instance;
    }
    //private float CalculateCurrentDamage()
    //{
    //    WeaponItemData weapon = player.Equipment.CurrentWeapon;
    //    if (weapon == null) return 0f; // 맨손 데미지 등 별도 처리 가능

    //    LightAttackStepData stepData = attackData.lightAttackSteps[currentComboIndex];
    //    return weapon.GetFinalDamage(stepData.damageMultiplier);
    //}
    public LightAttackStepData[] GetLightAttackStepData()
    {
        return attackData.lightAttackSteps;
    }
    public HeavyAttackStepData[] GetHeavyAttackStepData() 
    { 
        return attackData.heavyAttackSteps;
    }
}
