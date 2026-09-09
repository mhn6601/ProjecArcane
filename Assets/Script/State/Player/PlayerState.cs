using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class PlayerState : State
{
    protected Player player;

    public PlayerState(Player player)
        {
        this.player = player;
    }
}
