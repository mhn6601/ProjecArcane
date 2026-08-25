using UnityEngine;
using UnityEngine.TextCore.Text;


public abstract class PlayerState : State
{
    protected PlayerController controller;
    protected Player player;



    public override void Initialization()
    {
        player = GetComponentInParent<Player>();
        controller = GetComponentInParent<PlayerController>();
    }
}
