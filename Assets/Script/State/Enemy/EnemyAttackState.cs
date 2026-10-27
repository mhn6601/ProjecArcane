// EnemyAttackState.cs
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private enum Phase { Playing, Gap }

    private const float MaxStepDuration = 5f;   // 종료 감지 실패 시 안전장치

    private Phase phase;
    private int stateHash;
    private float timer;

    public EnemyAttackState(Enemy enemy, Animator animator) : base(enemy, animator) { }

    public override void OnEnter()
    {
        PlayCurrentStep();
    }

    public override void Update()
    {
        timer += Time.deltaTime;   // 히트스톱 중에는 같이 멈춤
        var attack = enemy.Attack;

        if (phase == Phase.Playing)
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            bool finished = info.shortNameHash == stateHash
                            && !animator.IsInTransition(0)
                            && info.normalizedTime >= 1f;

            if (finished || timer >= MaxStepDuration)
            {
                if (attack.HasNextStep)
                {
                    phase = Phase.Gap;
                    timer = 0f;
                }
                else
                {
                    Finish();
                }
            }
        }
        else // Gap
        {
            if (timer >= attack.CurrentStep.delayAfter)
            {
                attack.AdvanceStep();
                PlayCurrentStep();
            }
        }
    }

    public override void OnExit()
    {
        // 피격(HitStun) 등으로 중간에 끊겨도 패턴 상태를 정리
        if (enemy.Attack.CurrentPattern != null)
            enemy.Attack.EndPattern();
        enemy.Attack.DisableHitBox();
    }

    void PlayCurrentStep()
    {
        phase = Phase.Playing;
        timer = 0f;

        var step = enemy.Attack.CurrentStep;
        stateHash = Animator.StringToHash(step.animName);
        animator.CrossFadeInFixedTime(stateHash, step.crossFade);
    }

    void Finish()
    {
        enemy.Controller.ActionFSM.ChangeState(enemy.Controller.EmptyActionState);
    }
}