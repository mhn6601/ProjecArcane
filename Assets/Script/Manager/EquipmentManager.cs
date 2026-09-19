using System;
using Unity.VisualScripting;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{

    [SerializeField] Weapon mainWeapon;
    [SerializeField] Weapon subWeapon;
    public Weapon MainWeapon => mainWeapon;
    public Weapon SubWeapon => subWeapon;

    [Header("Default Equipment")]
    [SerializeField]
    private EquipmentItemData defaultMainWeaponData;
    public EquipmentItemData DefaultMainWeaponData=>defaultMainWeaponData;
    
    [SerializeField]
    private EquipmentItemData defaultSubWeaponData;
    public EquipmentItemData DefaultSubWeaponData=>defaultSubWeaponData;
    // =========================================================
    // Weapon Sockets
    // =========================================================

    [Header("Weapon Sockets")]
    [SerializeField]
    private Transform mainHandSocket;

    [SerializeField]
    private Transform subHandSocket;


    // =========================================================
    // 현재 생성된 장비 GameObject
    // =========================================================

    private GameObject currentMainWeaponGO;
    private GameObject currentSubWeaponGO;


    // =========================================================
    // Events
    // =========================================================

    public event Action<EquipmentSlotType, EquipmentItemData>
        OnEquipmentChanged;

    public event Action<EquipmentSlotType, WeaponItemData>
        OnWeaponChanged;


    private PlayerStats playerStats;
    private Player player;

    // =========================================================
    // Unity
    // =========================================================

    private void Awake()
    {
        player = Player.Instance;
        playerStats = player.Stats;
    }


    // =========================================================
    // Equipment Apply
    // =========================================================

    /// <summary>
    /// InventoryManager가 결정한 장비를
    /// 실제 플레이어에게 적용한다.
    /// </summary>
    public void ApplyEquipment(EquipmentItemData item)
    {
        if (item == null)
            return;


        EquipmentSlotType slot =
            item.equipSlottype;

        if (slot == EquipmentSlotType.MainWeapon)
        {
            RemoveWeaponVisual(item);

            CreateWeaponVisual(item);
            player.Animation.SetWeaponAnimation(item as WeaponItemData);
        }
        else if (slot == EquipmentSlotType.SubWeapon)
        {
            RemoveWeaponVisual(item);

            CreateWeaponVisual(item);
        }

        // 장비 스탯 적용
        if (playerStats != null)
        {
            playerStats.SetEquipmentBonus();
        }


        // 이벤트
        OnEquipmentChanged?.Invoke(
            slot,
            item
        );


        // 무기 이벤트
        if (item is WeaponItemData weaponItem)
        {
            //Debug.Log("APply");
            OnWeaponChanged?.Invoke(weaponItem.equipSlottype,
                weaponItem
            );
        }
    }


    // =========================================================
    // Equipment Remove
    // =========================================================

    /// <summary>
    /// InventoryManager가 장착 해제를 결정하면
    /// 실제 플레이어에게 적용된 장비를 제거한다.
    /// </summary>
    public void RemoveEquipment(EquipmentItemData item)
    {
        if (item == null)
            return;


        EquipmentSlotType slot =
            item.equipSlottype;


        // 무기 외형 제거
        if (slot == EquipmentSlotType.MainWeapon)
        {
            RemoveWeaponVisual(item);
            mainWeapon = null;
            ApplyEquipment(defaultMainWeaponData);
        }
        else if (slot == EquipmentSlotType.SubWeapon)
        {
            RemoveWeaponVisual(item);
            subWeapon = null;
            ApplyEquipment(defaultSubWeaponData);
        }


        // 장비 스탯 다시 계산
        if (playerStats != null)
        {
            playerStats.SetEquipmentBonus();
        }


        // 이벤트
        OnEquipmentChanged?.Invoke(
            slot,
            null
        );
    }


    // =========================================================
    // Weapon Visual
    // =========================================================

    private void CreateWeaponVisual(
        EquipmentItemData item)
    {
        WeaponItemData weaponItem =
            item as WeaponItemData;


        if (weaponItem == null)
            return;

        if (weaponItem.weaponPrefab == null)
            return;
        if (item.equipSlottype == EquipmentSlotType.MainWeapon)
        {

            if (mainHandSocket == null)
                return;


            currentMainWeaponGO =
                Instantiate(
                    weaponItem.weaponPrefab,
                    mainHandSocket
                );
            mainWeapon = currentMainWeaponGO.GetComponent<Weapon>();
        }
        else if(item.equipSlottype == EquipmentSlotType.SubWeapon)
        {
            if (subHandSocket == null)
                return;


            currentSubWeaponGO =
                Instantiate(
                    weaponItem.weaponPrefab,
                    subHandSocket
                );
            subWeapon = currentSubWeaponGO.GetComponent<Weapon>();
        }


        //currentMainWeaponGO.transform.localPosition =
        //    Vector3.zero;

        //currentMainWeaponGO.transform.localRotation =
        //    Quaternion.identity;
    }


    private void RemoveWeaponVisual(EquipmentItemData item)
    {
        if (item.equipSlottype==EquipmentSlotType.MainWeapon)
        {

            if (currentMainWeaponGO == null)
                return;


            Destroy(currentMainWeaponGO);

            currentMainWeaponGO = null;
        }
        else if (item.equipSlottype == EquipmentSlotType.SubWeapon)
        {
            if (currentSubWeaponGO == null)
                return;


            Destroy(currentSubWeaponGO);

            currentSubWeaponGO = null;
        }
    }
    public WeaponItemData GetMainWeaponData()
    {
        if (mainWeapon != null)
        {
            return mainWeapon.WeaponItemData;
        }

        return null;
    }
    public void EquipmentDefaultWeapons(bool main, bool sub)
    {
        //Debug.Log("EquipmentDefaultWeapons");

        if (player == null)
        {
            return;
        }

        if (player.Attack == null)
        {
            return;
        }

        if (!main)
        {
            if (defaultMainWeaponData == null)
            {
            }
            else
            {
                ApplyEquipment( defaultMainWeaponData);
            }
        }

        if (!sub)
        {
            if (defaultSubWeaponData == null)
            {
            }
            else
            {
                ApplyEquipment(defaultSubWeaponData);
            }
        }
    }
}