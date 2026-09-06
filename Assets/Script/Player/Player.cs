using UnityEngine;
public class Player:Singleton<Player>
{
    [Header("Sub Systems / Components")]
    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerMove move;
    [SerializeField] private PlayerStatus status;
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private Animator animator;

    // 외부 공개용 프로퍼티 (Get)
    public PlayerController Controller => controller;
    public PlayerMove Move => move;
    public PlayerStatus Status => status;
    public PlayerInputHandler Input => input;
    public PlayerStats PlayerStats => playerStats;

    
    public Transform lockOnTarget;

    [Header("Global Player State Flags")]
    public bool isDead { get; private set; }

    protected override void Awake()
    {
        InitComponents();

    }
    private void OnDestroy()
    {
        if (playerStats != null)
        {
            //playerStats.OnDeath -= HandleDeath;
        }
    }

    private void InitComponents()
    {
        if (controller == null) controller = GetComponent<PlayerController>();
        if (move == null) move = GetComponent<PlayerMove>();
        if (status == null) status = GetComponent<PlayerStatus>();
        if (input == null) input = GetComponent<PlayerInputHandler>();
        if (playerStats == null) playerStats = GetComponent<PlayerStats>();
    }
    // --- 라이프사이클 및 전체 시스템 제어 ---

    /// <summary> 플레이어 사망 시 서브시스템들의 동작을 일괄 제어 </summary>
    private void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        // 사망 시 컨트롤러 및 이동 연산 비활성화
        controller.enabled = false;
        move.ResetCache();
        
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
