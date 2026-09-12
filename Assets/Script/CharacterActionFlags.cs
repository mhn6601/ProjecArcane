using System;

[Flags]
public enum CharacterActionFlags
{
    None = 0,
    CanMove = 1 << 0, //  이동 가능 여부
    CanRotate = 1 << 1, // : 회전 가능 여부
    CanAttack = 1 << 2, // 공격 명령 수행 가능 여부
    CanRoll= 1 << 3,//구르기 가능 여부
    CanJump = 1 << 4, // 점프 가능 여부
    CanGuard = 1 << 5, //  가드 가능 여부
    CanParry = 1 << 6, // 패링 가능 여부
    
    isRunning = 1 << 7, // 달리기 상태
    isLockOn = 1 << 8, // 락온 상태
    IsInvincible = 1 << 9, // 피격 시 무적 판정
    IsSuperArmor = 1 << 10  //  경직 무시 판정
}