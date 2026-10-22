using System.Collections.Generic;
using UnityEngine;

public class SpellCastContext 
{
    // 1. 시전 주체 및 대상 정보
    public GameObject Caster { get; private set; }           // 시전한 캐릭터 (Player/Enemy)
    public Transform SpawnTransform { get; private set; }    // 투사체/이펙트 생성 위치 (예: 지팡이 끝, 손 소켓)
    public Transform TargetTransform { get; private set; }   // 락온된 타깃 Transform (락온 마커 대상)
    public Vector3 CastDirection { get; set; }              // 마법 발사 방향 Vector3

    // 2. 스킬 런타임 정보
    public RuntimeSpellInstance SpellInstance { get; private set; } // 실행된 스킬 인스턴스
    public ElementType Element => SpellInstance.currentElement;     // 속성 (Water, Fire, Electric, Arcane)
    public SpellType SpellType => SpellInstance.spellType;     // 캐스트타입

    // 3. 연산용 동적 스탯 (노드 및 스탯 보너스가 가산된 최종 연산값)
    public float FinalDamage { get; set; }         // 최종 데미지
    public float ProjectileSpeed { get; set; }    // 투사체 속도
    public float ScaleMultiplier { get; set; }   // 범위/크기 배율 (확장 노드 대응)
    [Header("Split")]
    public int SplitCount { get; set; }          // 분열 개수 (분열 노드 대응)
    public float SplitAngleStep { get; set; }          // 갈래 사이 각도
    public float SplitDamageMultiplier { get; set; }   // 분열 시 각 발 데미지 배율
    
    [Header("Recoceht")]
    public int RecocehtCount { get; set; }// 남은 도탄 횟수
    public float RecocehtRange { get; set; }// 도탄 범위
    public float RecocehtDamageMultiplier { get; set; }   // 도탄 시 각 발 데미지 배율
    [Header("AreaStrike")]
    public List<AreaStrikeConfig> AreaStrikes { get; private set; } = new List<AreaStrikeConfig>();
    public bool IsInvincibleCast { get; set; }   // 시전 중 무적 여부 (무적 노드 대응)
    public bool HasHoming { get; set; }          // 유도성 여부 (추적 노드 대응)
    public bool UseCastDirectionRotation { get; set; }   // true면 소켓 회전 대신 CastDirection으로 생성 회전 결정 (분열 발사용)
    public SpellCastContext Clone() => (SpellCastContext)MemberwiseClone();
    // 생성자: 스킬 시전 시점의 기본값 설정
    public SpellCastContext(GameObject caster, Transform spawnPoint, Transform target, RuntimeSpellInstance spellInstance, float baseAttackBonus = 0f)
    {
        Caster = caster;
        SpawnTransform = spawnPoint;
        TargetTransform = target;
        SpellInstance = spellInstance;

        // 발사 방향 계산 (락온 타깃이 있으면 타깃 방향, 없으면 Caster 전방)
        if (TargetTransform != null)
        {
            CastDirection = (TargetTransform.position - spawnPoint.position).normalized;
        }
        else
        {
            CastDirection = caster.transform.forward;
        }

        // 스탯 기본 초기화 (장비 공격력 보너스 반영)
        FinalDamage = 0f + baseAttackBonus; 
        ProjectileSpeed = 15f;
        ScaleMultiplier = 1f;

        SplitCount = 1; 
        SplitAngleStep = 15f;
        SplitDamageMultiplier = 1f;

        RecocehtCount = 0;
        RecocehtRange = 0;
        RecocehtDamageMultiplier = 1;

        var baseStrike = spellInstance?.baseData?.areaStrike;
        if (baseStrike != null && baseStrike.prefab != null) AreaStrikes.Add(baseStrike);

        IsInvincibleCast = false;
        HasHoming = false;

        // 장착된 특수 노드 2개의 효과를 Context에 순차 적용
        ApplyNodes();
    }

    private void ApplyNodes()
    {
        if (SpellInstance == null || SpellInstance.equippedNodes == null) return;

        foreach (var node in SpellInstance.equippedNodes)
        {
            if (node == null) continue;
            if (!node.CanApplyTo(SpellInstance.baseData)) continue;   // 태그 불일치면 효과 무시

            node.ApplyNodeEffect(this);
        }
    }
}
