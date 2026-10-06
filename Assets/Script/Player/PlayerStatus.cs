using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    //[Header("Base Stats")]
    //[SerializeField] private float maxHealth = 100f;
    //[SerializeField] private float maxStamina = 100f;

    // 비트마스크 원본 데이터 (내부 연산용)
    public CharacterActionFlags CurrentFlags { get; private set; } =
        CharacterActionFlags.CanMove | CharacterActionFlags.CanRotate | CharacterActionFlags.CanAttack | CharacterActionFlags.CanParry | CharacterActionFlags.CanCastSpell;

    [Header("=== Realtime Flags Monitor ===")]
    [SerializeField] private bool canMove;
    [SerializeField] private bool canRotate;
    [SerializeField] private bool canAttack;
    [SerializeField] private bool canJump;
    [SerializeField] private bool canGuard;
    [SerializeField] private bool canParry;
    [SerializeField] private bool canCastSpell;
    [SerializeField] private bool isRunning;
    [SerializeField] private bool isLockOn;
    [SerializeField] private bool isInvincible;
    [SerializeField] private bool isGuarding;

    // --- Fast Flag Check Getters ---
    public bool CanMove => HasFlag(CharacterActionFlags.CanMove);
    public bool CanRotate => HasFlag(CharacterActionFlags.CanRotate);
    public bool CanAttack => HasFlag(CharacterActionFlags.CanAttack);
    public bool CanJump => HasFlag(CharacterActionFlags.CanJump);
    public bool CanGuard => HasFlag(CharacterActionFlags.CanGuard);
    public bool CanParry => HasFlag(CharacterActionFlags.CanParry);
    public bool CanCastSpell => HasFlag(CharacterActionFlags.CanCastSpell);
    public bool IsRunning => HasFlag(CharacterActionFlags.isRunning);
    public bool IsLockOn => HasFlag(CharacterActionFlags.isLockOn);
    public bool IsInvincible => HasFlag(CharacterActionFlags.IsInvincible);

    public bool IsGuarding { get; set; }

    private void Update()
    {
        // [핵심] 인스펙터 실시간 모니터링을 위해 매 프레임 bool 변수 신호 갱신
        canMove = CanMove;
        canRotate = CanRotate;
        canAttack = CanAttack;
        canJump = CanJump;
        canParry = CanParry;
        canGuard = CanGuard;
        isRunning = IsRunning;
        isLockOn = IsLockOn;
        isInvincible = IsInvincible;
        isGuarding = IsGuarding;
        canCastSpell = CanCastSpell;
    }

    public void AddFlag(CharacterActionFlags flag) => CurrentFlags |= flag;
    public void RemoveFlag(CharacterActionFlags flag) => CurrentFlags &= ~flag;
    public bool HasFlag(CharacterActionFlags flag) => (CurrentFlags & flag) == flag;
}