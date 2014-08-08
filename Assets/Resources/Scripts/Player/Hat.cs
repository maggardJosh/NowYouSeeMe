using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Hat : FContainer
{
    FAnimatedSprite hatSprite;
    Player owner;
    public Hat(Player p)
    {
        hatSprite = new FAnimatedSprite("hat");
        hatSprite.addAnimation(new FAnimation("mark1", new int[] { 1 }, 300, true));
        hatSprite.addAnimation(new FAnimation("mark2", new int[] { 2 }, 300, true));
        hatSprite.addAnimation(new FAnimation("mark3", new int[] { 3 }, 300, true));
        hatSprite.addAnimation(new FAnimation("mark4", new int[] { 4 }, 300, true));
        hatSprite.play("mark1");
        this.AddChild(hatSprite);
        this.owner = p;
    }
    
    public override void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToStage();
    }

    public void appear()
    {
        hatSprite.play("mark" + (RXRandom.Int(4) + 1));
        this.isVisible = true;
        Go.killAllTweensWithTarget(this);
        hatSprite.scale = .1f;
        Go.to(hatSprite, .2f, new TweenConfig().floatProp("scale", 1.0f));
    }

    Vector2 startDisappearPos = Vector2.zero;
    public void disappear()
    {
        this.isVisible = false;
    }
    private void Update()
    {
        hatSprite.scale = Mathf.CeilToInt(hatSprite.scale * 6) / 6.0f;
        
        if (owner != null)
            hatSprite.rotation = Mathf.LerpAngle(hatSprite.rotation, owner.getVelocityAngle(), .1f);
    }
}

