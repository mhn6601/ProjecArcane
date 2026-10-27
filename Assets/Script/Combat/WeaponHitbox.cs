using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class WeaponHitBox : MonoBehaviour
{
    [Header("Collider Settings")]
    [SerializeField] private Collider[] hitColliders = System.Array.Empty<Collider>();

    // 공격을 시작할 때 외부(State 등)에서 이 데이터를 교체하여 주입
    [SerializeField]
    private AttackData currentAttackData;

    // 초기 용량을 8 정도로 지정하여 런타임 Re-hash 및 GC 방지
    private readonly HashSet<CharacterCombat> alreadyHit = new(8);

    [SerializeField]private GameObject testObject;
    private CharacterCombat owner;

    private void Awake()
    {
        // 인스펙터 세팅이 누락되었을 경우 자동 수집
        if (hitColliders == null || hitColliders.Length == 0)
        {
            hitColliders = GetComponentsInChildren<Collider>(true);
        }
        owner = GetComponentInParent<CharacterCombat>();
        DisableHitBox();
    }

    private void Reset()
    {
        // 에디터에서 스크립트 추가/리셋 시 자동 할당
        hitColliders = GetComponentsInChildren<Collider>(true);
    }

    // 공격 시작 시 State 등에서 데이터 주입과 함께 호출
    public void InitializeAttackData(AttackData attackData)
    {
        currentAttackData = attackData;
    }

    public void EnableHitBox()
    {
        //Debug.Log("weaponhitbox-EnableHitBox");
        alreadyHit.Clear();

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i] != null)
                hitColliders[i].enabled = true;
        }
       // Debug.Log(hitColliders[0].enabled);
        if (testObject)
        {
           // Debug.Log($"Before : activeSelf = {testObject.activeSelf}, activeInHierarchy = {testObject.activeInHierarchy}");

            testObject.SetActive(true);
            //Debug.Log($"After : activeSelf = {testObject.activeSelf}, activeInHierarchy = {testObject.activeInHierarchy}");
        }

    }

    public void DisableHitBox()
    {
       // Debug.Log("weaponhitbox-DisableHitBox");
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i] != null)
                hitColliders[i].enabled = false;
        }
        if (testObject)
        {
            testObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HurtBox hurtbox = other.GetComponent<HurtBox>();

        if (hurtbox == null)
            hurtbox = other.GetComponentInParent<HurtBox>();

        if (hurtbox == null)
            return;

        CharacterCombat target = hurtbox.owner;

        // 자기 자신
        if (target == owner)
            return;

        // 이번 공격에서 이미 맞은 캐릭터
        if (alreadyHit.Contains(target))
            return;

        alreadyHit.Add(target);

        // 공격자(소유자)가 자기 공식으로 최종 데미지를 계산, 에셋 데이터는 건드리지 않고 복사본을 전달
        AttackData hit = currentAttackData.CreateHit(owner.CalculateDamage(currentAttackData));

        target.ResolveHit(
            hit,
            owner.gameObject.transform,
            transform,
            hurtbox.HitPart
        );
    }
    //public void SetOwner(PlayerCombat playerCombat)
    //{
    //    owner = playerCombat;
    //}
}