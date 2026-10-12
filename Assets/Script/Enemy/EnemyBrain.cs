using UnityEngine;

public abstract class EnemyBrain : MonoBehaviour
{
    protected EnemyController controller;
    protected  virtual void Awake()
    {
        controller =GetComponent<EnemyController>();
    }
    /// <summary>매 프레임 판단. 하위 클래스가 구현한다.</summary>
    protected abstract void Think();

    protected virtual void Update()
    {
        Think();
    }
}
