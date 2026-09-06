using System;

[Flags]
public enum CharacterActionFlags
{
    None = 0,
    CanMove = 1 << 0, // 1  : 이동 가능 여부
    CanRotate = 1 << 1, // 2  : 회전 가능 여부
    CanAttack = 1 << 2, // 4  : 공격 명령 수행 가능 여부
    CanJump = 1 << 3, // 8  : 점프 가능 여부
    CanParry = 1 << 4, // 16 : 패링 가능 여부
    isLockOn = 1 << 5, // 32 : 락온 상태
    IsInvincible = 1 << 6, // 64 : 피격 시 무적 판정
    IsSuperArmor = 1 << 7  // 128    : 경직 무시 판정
}