using UnityEngine;

public abstract class EnemyState : State
{
    protected Enemy enemy;
    protected Animator animator;

    public EnemyState(Enemy enemy, Animator animator)
    {
        this.enemy = enemy;
        this.animator = animator;
    }
}
