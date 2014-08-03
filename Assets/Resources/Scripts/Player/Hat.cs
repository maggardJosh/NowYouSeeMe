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

    float hatReturnCount = 0;
    float disappearCount = 0;
    Vector2 startDisappearPos = Vector2.zero;
    float startRotation = 0;
    public void disappear(float hatReturnCount)
    {
        this.hatReturnCount = hatReturnCount;
        this.disappearCount = 0;
        this.startRotation = hatSprite.rotation;
        startDisappearPos = this.GetPosition();
        isDisappearing = true;
        Go.killAllTweensWithTarget(this);
        hatSprite.scale = 1.0f;
        Go.to(hatSprite, .1f, new TweenConfig().floatProp("scale", 2.0f).setIterations(2, LoopType.PingPong).setEaseType(EaseType.QuadOut));
        Go.to(hatSprite, hatReturnCount, new TweenConfig().setDelay(.2f).floatProp("scale", 0.0f).setEaseType(EaseType.QuadIn));
    }
    private void Update()
    {
        hatSprite.scale = Mathf.CeilToInt(hatSprite.scale * 6) / 6.0f;
        if (isDisappearing)
        {
            float t = disappearCount / hatReturnCount;
            this.x = Mathf.Lerp(startDisappearPos.x, owner.x + 2, t);
            this.y = Mathf.Lerp(startDisappearPos.y, owner.y + 6, t);
            hatSprite.rotation = Mathf.LerpAngle(startRotation, 90, t);
            disappearCount += Time.deltaTime;
        }

        if (owner != null && !isDisappearing)
            hatSprite.rotation = Mathf.LerpAngle(hatSprite.rotation, owner.getVelocityAngle(), .1f);
    }
}

