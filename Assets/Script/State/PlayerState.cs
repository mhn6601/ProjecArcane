using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class PlayerState : State
{
    protected PlayerController controller;

    public PlayerState(PlayerController controller)
        {
        this.controller = controller;
    }
}
