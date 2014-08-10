using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PressurePlate : Switch
{
    public const float COLLISION_WIDTH = 24;
    public const float COLLISION_HEIGHT = 12;
    public const float PRESSED_HEIGHT = 6;
    public Player playerOn = null;
    public PressurePlate(Vector2 pos, string doorName, string actionType, float time)
        : base(pos, doorName, actionType, time)
    {
        this.X_INTERACT_DIST = 24;
        this.SetPosition(pos);
        interactSprite.RemoveFromContainer();
        interactSprite = new FAnimatedSprite("PressurePlate/pressurePlate");
        interactSprite.addAnimation(new FAnimation("up", new int[] { 1 }, 100, true));
        interactSprite.addAnimation(new FAnimation("lock", new int[] { 2 }, 100, true));
        interactSprite.addAnimation(new FAnimation("down", new int[] { 3 }, 100, true));

        if (actionType.CompareTo("close") == 0 && time <= 0)
            interactSprite.play("lock");
        else
        interactSprite.play("up");
        this.AddChild(interactSprite);
    }

    bool pressed = false;
    public void press(Player p)
    {
        interactSprite.play("down");
        timeCount = 0;
        if (!pressed)
        {
            pressed = true;
            this.interact(p);
        }
    }
    public void unpress()
    {
        pressed = false;

        if (actionType.CompareTo("close") == 0 && time <= 0)
            interactSprite.play("lock");
        else
            interactSprite.play("up");
    }

}

