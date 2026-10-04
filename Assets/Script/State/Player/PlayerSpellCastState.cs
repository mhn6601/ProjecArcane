using UnityEngine;

public class PlayerSpellCastState : PlayerState
{
    private RuntimeSpellInstance spellInstance;

    public PlayerSpellCastState(Player player, Animator animator, RuntimeSpellInstance spellInstance)
        : base(player, animator)
    {
        this.spellInstance = spellInstance;
    }

    public override void OnEnter()
    {
        // 1. 발사 위치 가져오기 (MagicSocketHandler 활용)
        MagicSocket socket = player.MagicSocketHandler.GetSocket(MagicSocketType.RightHand);
        Transform spawnPoint = socket != null ? socket.SocketTransform : player.transform;

        // 2. 스킬 시전 정보(Context) 패킷 생성
        SpellCastContext context = new SpellCastContext(
            player.gameObject,
            spawnPoint,
            player.LockOn.CurrentTarget,
            spellInstance,
            player.Stats.SpellPower
        );

        // 3. spellInstance에서 발사할 Prefab을 가져와 CastSpell 호출
        // (※ RuntimeSpellInstance 내부 프로퍼티명에 맞게 spellInstance.SpellData.Prefab 등으로 맞춰주세요)
        GameObject spellPrefab = spellInstance?.baseData?.VfxPrefab;

        if (spellPrefab != null)
        {
            player.MagicSocketHandler.CastSpell(spellPrefab, MagicSocketType.RightHand, context);
        }
        else
        {
            Debug.LogWarning("[PlayerSpellCastState] 발사할 spellPrefab이 null입니다.");
        }
    }

    public override void Update()
    {

    }

    public override void OnExit()
    {

    }

    public void Execute()
    {

    }
}