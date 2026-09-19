using System.Collections.Generic;
using UnityEngine;

public class ParryBox : MonoBehaviour
{
    [Header("Collider Settings")]
    [SerializeField] private Collider[] parryColliders = System.Array.Empty<Collider>();

    // 공격을 시작할 때 외부(State 등)에서 이 데이터를 교체하여 주입
    private AttackData currentAttackData;

    // 초기 용량을 8 정도로 지정하여 런타임 Re-hash 및 GC 방지
    private readonly HashSet<Collider> alreadyParry = new(8);


    private void Awake()
    {
        // 인스펙터 세팅이 누락되었을 경우 자동 수집
        if (parryColliders == null || parryColliders.Length == 0)
        {
            parryColliders = GetComponentsInChildren<Collider>(true);
        }

        DisableParryBox();
    }

    private void Reset()
    {
        // 에디터에서 스크립트 추가/리셋 시 자동 할당
        parryColliders = GetComponentsInChildren<Collider>(true);
    }

    // 공격 시작 시 State 등에서 데이터 주입과 함께 호출
    public void InitializeAttackData(AttackData attackData)
    {
        currentAttackData = attackData;
    }

    public void EnableParryBox()
    {
        alreadyParry.Clear();

        for (int i = 0; i < parryColliders.Length; i++)
        {
            if (parryColliders[i] != null)
                parryColliders[i].enabled = true;
        }
    }

    public void DisableParryBox()
    {
        for (int i = 0; i < parryColliders.Length; i++)
        {
            if (parryColliders[i] != null)
                parryColliders[i].enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 이미 이번 스윙에 맞은 대상이면 무시
        if (alreadyParry.Contains(other)) return;

        // 적 몬스터의 Collider가 자식 오브젝트에 붙어있는 경우를 대비해 InParent 검색 권장
        if (!other.TryGetComponent<WeaponHitBox>(out var weaponHitbox))
        {
            weaponHitbox = other.GetComponentInParent<WeaponHitBox>();
            if (weaponHitbox == null) return;
        }

        alreadyParry.Add(other);

        // 패리 처리 수행 (공격 데이터 및 넉백 방향 계산을 위한 Transform 전달)
        
       // weaponHitbox.TakeHit(currentAttackData, transform);
    }
}
