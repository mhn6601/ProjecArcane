using UnityEngine;

public  class Enemy : MonoBehaviour
{
    EnemyAnimation animation;
    public EnemyAnimation Animation => animation;

    EnemyController enemyController;
    public EnemyController EnemyController=> enemyController;

    [SerializeField] private EnemyData enemyData;
    public EnemyData EnemyData=>enemyData;


    private void Awake()
    {
        InitComponents();
    }
    private void InitComponents()
    {
        animation = GetComponent<EnemyAnimation>();
        enemyController = GetComponent<EnemyController>();
    }
    //public bool isWalking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

}
