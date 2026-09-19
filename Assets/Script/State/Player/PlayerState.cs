using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class PlayerState : State
{
    protected Player player;
    protected Animator animator;

    public PlayerState(Player player,Animator animator)
        {
        this.player = player;
        this.animator =animator;
    }
}
