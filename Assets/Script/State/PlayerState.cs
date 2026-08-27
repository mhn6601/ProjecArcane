using UnityEngine;
using UnityEngine.TextCore.Text;


public abstract class PlayerState : State
{
    protected PlayerController controller;
    protected Player player;

    public PlayerState(PlayerController controller, Player player)
        {
        this.controller = controller;
        this.player = player;
    }
}
