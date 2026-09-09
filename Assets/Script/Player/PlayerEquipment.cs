using UnityEngine;
using System;

public class PlayerEquipment : MonoBehaviour
{
    [Header("테스트/기본 장착 무기 (인벤토리 없을 때 인스펙터에서 지정)")]
    [SerializeField] private WeaponItemData currentWeapon;

    public WeaponItemData CurrentWeapon => currentWeapon;

    // 인벤토리 시스템이 생기면 이 메서드 하나만 호출해주면 됨
    public event Action<WeaponItemData> OnWeaponChanged;

    public void EquipWeapon(WeaponItemData newWeapon)
    {
        currentWeapon = newWeapon;
        OnWeaponChanged?.Invoke(currentWeapon);
        // TODO: weaponPrefab 손에 부착, weaponOverrideController 교체 등은 여기서 처리
    }

    public bool HasWeaponEquipped => currentWeapon != null;
}