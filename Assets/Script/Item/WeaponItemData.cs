using UnityEngine;

public enum DamageType
{
    Physical,
    Fire,
    Water,
    Lightning,
    Arcane
}
public enum DefaultWeapon 
{
    defaultWeapon = 000000
}
[CreateAssetMenu(fileName = "WeaponItem_", menuName = "Item/Equipment/Weapon")]
public class WeaponItemData : EquipmentItemData
{
    [Header("무기 전투 스탯")]
    public float basePhysicalDamage = 10f;
    public float baseSpellDamage = 10f;
    public DamageType damageType = DamageType.Physical;

    [Header("무기별 속도/스태미너 보정")]
    [Tooltip("공격 애니메이션 재생 속도 배율")]
    public float attackSpeedMultiplier = 1f;
    [Tooltip("스태미너 소모량 배율 (무기가 무거울수록 증가)")]
    public float staminaCostMultiplier = 1f;

    [Header("무기별 콤보/애니메이션 데이터 (선택)")]
    // 무기마다 콤보 애니메이션이 다르다면 여기서 스텝 데이터를 무기가 직접 들고 있게 할 수도 있음
    public PlayerAttackData playerAttackData;

    [Header("표시/이펙트")]
    public GameObject weaponPrefab;      // 손에 들리는 실제 모델
    public RuntimeAnimatorController weaponOverrideController; // 무기별 애니메이터 오버라이드

    

    
}