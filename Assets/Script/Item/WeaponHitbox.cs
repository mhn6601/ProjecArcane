// WeaponHitbox.cs (무기 프리팹에 부착)
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [SerializeField] private Collider[] hitColliders;
    [SerializeField] private PlayerAttackData currentAttackData; // 공격 시작 시 캐릭터가 세팅
    private HashSet<Collider> alreadyHit = new(); // 한 스윙에 중복 히트 방지

    public void EnableHitbox()
    {
        alreadyHit.Clear();
        foreach (var c in hitColliders) c.enabled = true;
    }

    public void DisableHitbox()
    {
        foreach (var c in hitColliders) c.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyHit.Contains(other)) return;
        if (!other.TryGetComponent<Hurtbox>(out var hurtbox)) return;

        alreadyHit.Add(other);
        hurtbox.TakeHit(currentAttackData, transform);
    }
}