using Unity.Mathematics;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float maxHealth ;
    [SerializeField] private float maxStamina;
    [SerializeField] private float reduceDamagePercent;

    
    public float CurrentHealth { get; private set; }
    public float CurrentStamina { get; private set; }
    public float ReduceDamagePercent 
    {
        get 
        {
            ReduceDamagePercent= math.clamp(reduceDamagePercent,  -5f,1f);
            return reduceDamagePercent; }
        set { reduceDamagePercent = value; }
    }

    // 비트마스크 기반 액션 제한 플래그 (기본값: 이동, 회전, 공격 가능)
    public CharacterActionFlags CurrentFlags { get; private set; } =
        CharacterActionFlags.CanMove | CharacterActionFlags.CanRotate | CharacterActionFlags.CanAttack | CharacterActionFlags.CanParry;

    // 방패 막기, 무적 등 개별 상태 프로퍼티
    public bool IsGuarding { get; set; } = false;
    public bool IsDead => CurrentHealth <= 0;

    // --- Fast Flag Check Getters ---
    public bool CanMove => HasFlag(CharacterActionFlags.CanMove);
    public bool CanRotate => HasFlag(CharacterActionFlags.CanRotate);
    public bool CanAttack => HasFlag(CharacterActionFlags.CanAttack);
    public bool CanJump => HasFlag(CharacterActionFlags.CanJump);
    public bool CanParry => HasFlag(CharacterActionFlags.CanParry);
    public bool isLockOn => HasFlag(CharacterActionFlags.isLockOn);
    public bool IsInvincible => HasFlag(CharacterActionFlags.IsInvincible);

    private void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentStamina = maxStamina;
    }

    // ========================================================================
    // [비트 연산 기반 Flag 제어 메서드]
    // ========================================================================

    /// <summary> 특정 플래그 추가 (예: Status.AddFlag(CharacterActionFlags.IsInvincible)) </summary>
    public void AddFlag(CharacterActionFlags flag)
    {
        CurrentFlags |= flag;
    }

    /// <summary> 특정 플래그 제거 (예: Status.RemoveFlag(CharacterActionFlags.CanMove)) </summary>
    public void RemoveFlag(CharacterActionFlags flag)
    {
        CurrentFlags &= ~flag;
    }

    /// <summary> 특정 플래그 포함 여부 확인 </summary>
    public bool HasFlag(CharacterActionFlags flag)
    {
        return (CurrentFlags & flag) == flag;
    }

    /// <summary> 플래그 완전 초기화 (기본 상태 복귀) </summary>
    public void ResetFlags()
    {
        CurrentFlags = CharacterActionFlags.CanMove | CharacterActionFlags.CanRotate | CharacterActionFlags.CanAttack;
    }

    // ========================================================================
    // [피격 및 데미지 연산]
    // ========================================================================

    public void TakeDamage(float damageAmount)
    {
        if (IsDead || IsInvincible) return;

        // 방패 막기 중일 경우 데미지 감쇄 처리
        if (IsGuarding)
        {

            damageAmount *= (1- reduceDamagePercent); // reduceDamage% 데미지 경감
            // 스태미너 소모 로직 추가 가능
        }

        CurrentHealth = Mathf.Max(0f, CurrentHealth - damageAmount);
    }
}