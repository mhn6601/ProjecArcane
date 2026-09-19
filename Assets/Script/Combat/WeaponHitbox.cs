using System.Collections.Generic;
using UnityEngine;


public class WeaponHitBox : MonoBehaviour
{
    [Header("Collider Settings")]
    [SerializeField] private Collider[] hitColliders = System.Array.Empty<Collider>();

    // 공격을 시작할 때 외부(State 등)에서 이 데이터를 교체하여 주입
    private AttackData currentAttackData;

    // 초기 용량을 8 정도로 지정하여 런타임 Re-hash 및 GC 방지
    private readonly HashSet<Collider> alreadyHit = new(8);

    [SerializeField]private GameObject testObject;

    private void Awake()
    {
        // 인스펙터 세팅이 누락되었을 경우 자동 수집
        if (hitColliders == null || hitColliders.Length == 0)
        {
            hitColliders = GetComponentsInChildren<Collider>(true);
        }

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
        // 이미 이번 스윙에 맞은 대상이면 무시
        if (alreadyHit.Contains(other)) return;

        // 적 몬스터의 Collider가 자식 오브젝트에 붙어있는 경우를 대비해 InParent 검색 권장
        if (!other.TryGetComponent<HurtBox>(out var hurtbox))
        {
            hurtbox = other.GetComponentInParent<HurtBox>();
            if (hurtbox == null) return;
        }

        alreadyHit.Add(other);

        Debug.Log("hitboxTriggered");
        // 피격 처리 수행 (공격 데이터 및 넉백 방향 계산을 위한 Transform 전달)
        hurtbox.TakeHit(currentAttackData, transform);
    }
}