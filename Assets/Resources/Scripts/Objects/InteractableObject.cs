using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class InteractableObject : FContainer
{
    FAnimatedSprite interactInd;
    protected float X_INTERACT_DIST = 50;
    protected float Y_INTERACT_DIST = 10;
    const float MIN_INTERACT_DIST = 20;
    const float MIN_INTERACT_DIST_SQR = MIN_INTERACT_DIST * MIN_INTERACT_DIST;
    const float INDICATOR_Y = 20;
    const float INDICATOR_BOUNCE = 5;
    protected bool interactable = true;
    public InteractableObject()
    {
        interactInd = new FAnimatedSprite("InteractIndicator/interactIndicator");
        interactInd.addAnimation(new FAnimation("active", new int[] { 1, 2 }, 400, true));
        interactInd.play("active");
        this.AddChild(interactInd);
        interactInd.isVisible = false;
    }

    public float indDisp { get; set; }
    public bool checkInteractDist(Player p)
    {
        if (!interactable)
            return false;
        if (x - X_INTERACT_DIST < p.x &&
            x + X_INTERACT_DIST > p.x &&
            y - Y_INTERACT_DIST < p.y &&
            y + Y_INTERACT_DIST > p.y)
        {
            if (!interactInd.isVisible)
            {
                interactInd.isVisible = true;
                indDisp = 0;
                Go.to(this, 1.0f, new TweenConfig().floatProp("indDisp", INDICATOR_BOUNCE, true).setIterations(-1, LoopType.PingPong).setEaseType(EaseType.QuadOut));
            }
            interactInd.x = (p.x - this.x);
            interactInd.y = (p.y - this.y) + INDICATOR_Y + indDisp;
            p.setInteractObject(this);
            return true;
        }
        else
        {
            if (interactInd.isVisible)
            {
                Go.killAllTweensWithTarget(interactInd);
                interactInd.isVisible = false;
            }
            return false;
        }
    }

    public virtual void interact(Player p)
    {
        interactInd.isVisible = false;
        Go.killAllTweensWithTarget(interactInd);
    }
}

