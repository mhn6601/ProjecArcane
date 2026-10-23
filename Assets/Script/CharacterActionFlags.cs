using System;

[Flags]
public enum CharacterActionFlags
{
    None = 0,
    Dead=1<<0,
    CanMove = 1 << 1, //  이동 가능 여부
    CanRotate = 1 << 2, // : 회전 가능 여부
    CanAttack = 1 << 3, // 공격 명령 수행 가능 여부
    CanRoll= 1 << 4,//구르기 가능 여부
    CanJump = 1 << 5, // 점프 가능 여부
    CanGuard = 1 << 6, //  가드 가능 여부
    CanParry = 1 << 7, // 패링 가능 여부
    CanCastSpell=1<<8,


    isRunning = 1 << 9, // 달리기 상태
    isLockOn = 1 << 10, // 락온 상태
    IsInvincible = 1 << 11, // 피격 시 무적 판정
    IsSuperArmor = 1 << 12  //  경직 무시 판정
}