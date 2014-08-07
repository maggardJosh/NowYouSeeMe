using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class InteractableObject : FContainer
{

    protected bool toggleable = false;
    protected bool on = true;
    protected FAnimatedSprite interactSprite;
    protected float X_INTERACT_DIST = 50;
    protected float Y_INTERACT_DIST = 10;
    const float MIN_INTERACT_DIST = 20;
    const float MIN_INTERACT_DIST_SQR = MIN_INTERACT_DIST * MIN_INTERACT_DIST;

    public bool Open { get { return on; } }

    protected bool interactable = true;
    public InteractableObject()
    {

    }

    public void turnOffInteractInd()
    {
        if (!toggleable)
            interactSprite.play("interactable");
        else
            interactSprite.play(on ? "on" : "off");
    }

    public bool checkInteractDist(Player p)
    {
        if (!interactable)
            return false;
        if (p.currentState == Player.State.VANISHING)
        {
            if (!toggleable)
                interactSprite.play("interactable");
            else
                interactSprite.play(on ? "on" : "off");
            return false;
        }
        if (x - X_INTERACT_DIST < p.x &&
            x + X_INTERACT_DIST > p.x &&
            y - Y_INTERACT_DIST < p.y &&
            y + Y_INTERACT_DIST > p.y)
        {
            if (!toggleable)
                interactSprite.play("hover");
            else
                interactSprite.play((on ? "on" : "off") + "Hover");
            
            return true;
        }
        else
        {
            if (!toggleable)
                interactSprite.play("interactable");
            else
                interactSprite.play(on ? "on" : "off");
            return false;
        }

    }

    public abstract void interact(Player p);

}

