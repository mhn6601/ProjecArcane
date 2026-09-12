using UnityEngine;

public  class Entity : MonoBehaviour
{
    Animator animator;
    public Animator Animator => animator;

    EnemyController enemyController;
    public EnemyController EnemyController=> enemyController;

    [SerializeField] private EnemyData enemyData;
    public EnemyData EnemyData=>enemyData;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyController = GetComponent<EnemyController>(); 
    }
    //public bool isWalking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
