using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class InteractableObject : FContainer
{

    protected FAnimatedSprite interactSprite;
    protected float X_INTERACT_DIST = 50;
    protected float Y_INTERACT_DIST = 10;
    const float MIN_INTERACT_DIST = 20;
    const float MIN_INTERACT_DIST_SQR = MIN_INTERACT_DIST * MIN_INTERACT_DIST;

    protected bool interactable = true;
    public InteractableObject()
    {
     
    }

    public bool checkInteractDist(Player p)
    {
        if (!interactable)
            return false;
        if (p.currentState == Player.State.VANISHING)
        {
            interactSprite.play("interactable");
            return false;
        }
        if (x - X_INTERACT_DIST < p.x &&
            x + X_INTERACT_DIST > p.x &&
            y - Y_INTERACT_DIST < p.y &&
            y + Y_INTERACT_DIST > p.y)
        {
            interactSprite.play("hover");
            p.setInteractObject(this);
            return true;
        }
        else
        {
            interactSprite.play("interactable");
            return false;
        }
        
    }

    public abstract void interact(Player p);

}

