using UnityEngine;
public class Player:Singleton<Player>
{

    [Header("Sub Systems / Components")]
    [SerializeField] private PlayerAnimation animation;
    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerMove move;
    [SerializeField] private PlayerJump jump;
    [SerializeField] private PlayerRoll roll;
    [SerializeField] private PlayerAttack attack;
    [SerializeField] private PlayerStatus status;
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private PlayerResourceSystem resourceSystem;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private PlayerLockOn lockOn;
    [SerializeField] private Animator animator;

    // 외부 공개용 프로퍼티 (Get)
    public PlayerAnimation Animation => animation;
    public Animator Animator => animator;
    public PlayerController Controller => controller;
    public PlayerMove Move => move;
    public PlayerJump Jump => jump;
    public PlayerRoll Roll => roll;
    public PlayerAttack Attack => attack;
    public PlayerStatus Status => status;
    public PlayerInputHandler Input => input;
    public PlayerStats Stats => stats;
    public PlayerResourceSystem ResourceSystem => resourceSystem;
    public EquipmentManager EquipmentManager => equipmentManager;
    public PlayerLockOn LockOn => lockOn;


    [Header("Global Player State Flags")]
    public bool isDead { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        InitComponents();
       

    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (stats != null)
        {
            //playerStats.OnDeath -= HandleDeath;
        }
    }
    private void OnEnable()
    {
        input.OnLookInput += lockOn.HandleLookInput;
            
    }
    private void OnDisable()
    {
        input.OnLookInput -= lockOn.HandleLookInput;
    }

    private void InitComponents()
    {
        if (animation == null) 
        { animation = GetComponent<PlayerAnimation>(); animator = Animation.Animator; } 
        if (controller == null) controller = GetComponent<PlayerController>();
        if (move == null) move = GetComponent<PlayerMove>();
        if (jump == null) jump = GetComponent<PlayerJump>();
        if (roll == null) roll = GetComponent<PlayerRoll>();
        if (attack == null) attack = GetComponent<PlayerAttack>();
        if (status == null) status = GetComponent<PlayerStatus>();
        if (input == null) input = GetComponent<PlayerInputHandler>();
        if (stats == null) stats = GetComponent<PlayerStats>();
        if (resourceSystem == null) resourceSystem = GetComponent<PlayerResourceSystem>();
        if (status == null) status = GetComponent<PlayerStatus>();
        if (equipmentManager == null) equipmentManager = GetComponent<EquipmentManager>();
        if (lockOn == null) lockOn = GetComponent<PlayerLockOn>();
    }
    // --- 라이프사이클 및 전체 시스템 제어 ---

    /// <summary> 플레이어 사망 시 서브시스템들의 동작을 일괄 제어 </summary>
    private void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        // 사망 시 컨트롤러 및 이동 연산 비활성화
        controller.enabled = false;
        
        // 사망 애니메이션 실행
        if (animator != null)
        {
            animator.SetTrigger("isDead");
        }
    }

    /// <summary> 대화, 컷씬, 스턴 등 플레이어 입력을 켜고 끌 때 사용 </summary>
    public void SetInputEnable(bool isEnable)
    {
        if (controller != null) controller.enabled = isEnable;
        if (input != null) input.enabled = isEnable;
    }

}
