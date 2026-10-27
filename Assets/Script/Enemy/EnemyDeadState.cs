using UnityEngine;

public class EnemyDeadMovementState : EnemyState
{
    private static readonly int DeathHash = Animator.StringToHash("Death");
    public EnemyDeadMovementState (Enemy enemy, Animator animator) : base(enemy, animator)
    {

    }
   
    public override void OnEnter()
    {
        animator.CrossFade(DeathHash, 0.1f);
    }
    public override void Update()
    {

    }
    public override void OnExit()
    {
    }
}
public class EnemyDeadActionState : EnemyState
{
    public EnemyDeadActionState(Enemy enemy, Animator animator) : base(enemy, animator)
    {
    }
    public override void OnEnter()
    {
    }
    public override void Update()
    {

    }
    public override void OnExit()
    {
    }
}