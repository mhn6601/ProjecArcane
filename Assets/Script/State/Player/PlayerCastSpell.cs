using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCastSpell : MonoBehaviour
{
    private Player player;

    // spellId → 쿨타임이 끝나는 시각 (같은 스펠을 슬롯 이동해도 쿨타임 유지)
    private readonly Dictionary<string, float> cooldownReadyTime = new Dictionary<string, float>();

    /// <summary>시전 성공 시 (슬롯 인덱스, 쿨타임 길이). 우하단 SpellSlot HUD가 쿨타임 표시에 구독.</summary>
    public event Action<int, float> OnSpellCast;
    public event Action OnSpellFire;

    // 클립의 Animation Event Function 이름과 동일해야 함
    public void SpellFire() => OnSpellFire?.Invoke();
    private void Awake()
    {
        player = GetComponent<Player>();
    }
    private void Start()
    {

    }

    /// <summary>해당 슬롯 스펠의 남은 쿨타임 비율 (0 = 사용 가능, 1 = 방금 시전). HUD용.</summary>
    public float GetCooldownRatio(int slotIndex)
    {
        SpellData spell = SpellProgressManager.Instance.GetEquipped(slotIndex);
        if (spell == null || spell.cooldown <= 0f) return 0f;

        float remain = GetRemainingCooldown(spell);
        return Mathf.Clamp01(remain / spell.cooldown);
    }

    public float GetRemainingCooldown(SpellData spell)
    {
        if (spell == null) return 0f;
        if (!cooldownReadyTime.TryGetValue(spell.spellId, out float readyTime)) return 0f;
        return Mathf.Max(0f, readyTime - Time.time);
    }

    /// <summary>
    /// 슬롯 시전 시도. 성공하면 true.
    /// 상태 전환이 가능한 상태(EmptyActionState 등)에서만 호출할 것.
    /// </summary>
    public bool TryCast(int slotIndex)
    {
        SpellData spell = SpellProgressManager.Instance.GetEquipped(slotIndex);
        if (spell == null) return false;                        // 빈 슬롯

        if (GetRemainingCooldown(spell) > 0f) return false;     // 쿨타임 중

        // MP 차감은 가장 마지막 검사 (앞 조건에서 실패했는데 MP만 깎이는 일 방지)
        if (!player.ResourceSystem.UseMana(spell.baseMpCost)) return false;

        cooldownReadyTime[spell.spellId] = Time.time + spell.cooldown;

        player.Controller.ActionFSM.ChangeState(new PlayerSpellCastState(player, player.Animation.Animator, spell));

        OnSpellCast?.Invoke(slotIndex, spell.cooldown);
        return true;
    }
}