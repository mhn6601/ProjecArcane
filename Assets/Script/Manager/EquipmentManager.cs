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
    private Weapon defaultMainWeapon;
    [SerializeField]
    private Weapon defaultSubWeapon;
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

    public event Action< WeaponItemData>
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


        // 기존 무기 GameObject 제거
        if (slot == EquipmentSlotType.MainWeapon)
        {
            mainWeapon = (item as WeaponItemData).weaponPrefab.GetComponent<Weapon>();
            player.Attack.MainWeapon = MainWeapon;
            RemoveWeaponVisual(item);
            CreateWeaponVisual(item);
        }
        if (slot == EquipmentSlotType.SubWeapon)
        {
            subWeapon = (item as WeaponItemData).weaponPrefab.GetComponent<Weapon>();
            player.Attack.SubWeapon = subWeapon;
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
            OnWeaponChanged?.Invoke(
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
            player.Attack.MainWeapon = defaultMainWeapon.GetComponent<Weapon>();
        }
        else if (slot == EquipmentSlotType.SubWeapon)
        {
            RemoveWeaponVisual(item);
            subWeapon = null;
            player.Attack.SubWeapon = defaultSubWeapon.GetComponent<Weapon>(); ;
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


        if (item is WeaponItemData weaponItem)
        {
            OnWeaponChanged?.Invoke(InventoryManager.Instance.GetItemDatabase().GetItemByID(DefaultWeapon.defaultWeapon.ToString()) as WeaponItemData);
               
            
        }
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
            if (defaultMainWeapon == null)
            {
            }
            else
            {
                player.Attack.MainWeapon = defaultMainWeapon;
            }
        }

        if (!sub)
        {
            if (defaultSubWeapon == null)
            {
            }
            else
            {
                player.Attack.SubWeapon = defaultSubWeapon;
            }
        }
    }
}