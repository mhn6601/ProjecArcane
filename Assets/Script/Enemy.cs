using UnityEngine;

public  class Enemy : MonoBehaviour
{
    [Header("Sub Systems / Components")]
    [SerializeField] private  EnemyAnimation animation;
    [SerializeField] private EnemyController controller;
    [SerializeField] private EnemyStatus status;
    [SerializeField] private EnemyStats stats;
    [SerializeField] private Animator animator;

    public EnemyAnimation Animation => animation;
    public EnemyController Controller=> controller;
    public EnemyStatus Status => status;
    public EnemyStats Stats=> stats;

    public bool isDead { get; private set; }

    private void Awake()
    {
        InitComponents();
    }
    private void InitComponents()
    {
        if (animation == null)
        { animation = GetComponent<EnemyAnimation>(); animator = Animation.Animator; }
        controller = GetComponent<EnemyController>();
    }
    //public bool isWalking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
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
}
