using UnityEngine;

public abstract class EnemyState : State
{
    protected Entity entity;

    public EnemyState(Entity entity)
    {
        this.entity = entity;
    }
}
