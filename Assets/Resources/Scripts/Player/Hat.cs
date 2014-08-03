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
        hatSprite.addAnimation(new FAnimation("mark", new int[] { 0 }, 300, true));
        hatSprite.play("mark");
        this.AddChild(hatSprite);
        this.owner = p;
    }
    bool isDisappearing = false;

    public override void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToStage();
    }

    public void appear()
    {
        isDisappearing = false;
        Go.killAllTweensWithTarget(this);
        hatSprite.scale = .1f;
        Go.to(hatSprite, .2f, new TweenConfig().floatProp("scale", 1.0f));
    }

    public void disappear(float hatReturnCount)
    {
        isDisappearing = true;
        Go.killAllTweensWithTarget(this);
        hatSprite.scale = 1.0f;
        Go.to(hatSprite, hatReturnCount, new TweenConfig().floatProp("scale", 0).setEaseType(EaseType.QuadIn));
    }
    private void Update()
    {
        hatSprite.scale = Mathf.CeilToInt(hatSprite.scale * 6) / 6.0f;


        if (owner != null && !isDisappearing)
            hatSprite.rotation = Mathf.LerpAngle(hatSprite.rotation, owner.getVelocityAngle(), .1f);
    }
}

