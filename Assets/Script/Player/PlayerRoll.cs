using UnityEngine;

public class PlayerRoll : MonoBehaviour
{
    Player player;

    [SerializeField]
    private AnimationCurve rollSpeedCurve =
        AnimationCurve.EaseInOut(0.8f, 0.6f, 0.2f, 0.3f); // 초반 빠르고 후반 감속
    [SerializeField] private float rollDistance = 3.5f;
    [SerializeField] private float rollDuration = 0.5f; // Roll 애니메이션 길이와 대략 맞추기

    private Vector3 rollDirection;
    private float rollElapsed;

    private void Awake()
    {
        player = Player.Instance;
    }

    public void SetRollDirection(Vector3 worldDir)
    {
        worldDir.y = 0f;
        rollDirection = worldDir.sqrMagnitude > 0.001f
            ? worldDir.normalized
            : player.transform.forward;
        rollElapsed = 0f;
    }

    public void ExcuteRoll()
    {
        rollElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(rollElapsed / rollDuration);

        float baseSpeed = rollDistance / rollDuration;
        
        float speed = rollSpeedCurve.Evaluate(t) * baseSpeed;

        Vector3 horizontalMove = rollDirection * speed;
        Vector3 verticalMove = new Vector3(0f, player.Jump.GetJumpVelocity(), 0f);
        player.Controller.CC.Move((horizontalMove + verticalMove) * Time.deltaTime);
    }
}