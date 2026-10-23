using UnityEngine;
using System.Collections;

public class PlayerDeadActionState : PlayerState
{
    private static readonly int DeathHash = Animator.StringToHash("Death");

    public PlayerDeadActionState(Player player, Animator animator) : base(player, animator) { }

    public override void OnEnter()
    {
        UIManager.Instance?.CloseAllUI();
        player.Input.SetInputEnabled(false);              // controller는 건드리지 않음
        player.Move.StopAllCoroutines();        // RotateSnap 정지
        player.LockOn.ReleaseLockOn();                

        Time.timeScale = 1f;                    // 히트스톱 중 사망해도 복구
        animator.CrossFade(DeathHash, 0.1f);
        player.StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        GameManager.Instance.OnPlayerDied();
    }
    public override void Update()
    {
        
    }
    public override void OnExit() { }
}

public class PlayerDeadMovementState : PlayerState
{
    public PlayerDeadMovementState(Player player, Animator animator) : base(player, animator) { }
    public override void OnEnter() { }
    public override void Update() { }
    public override void OnExit() { }
}