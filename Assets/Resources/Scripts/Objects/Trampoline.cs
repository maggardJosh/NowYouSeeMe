using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Trampoline : FContainer
{
    FAnimatedSprite trampolineSprite;

    public Trampoline(Vector2 pos)
    {
        trampolineSprite = new FAnimatedSprite("Trampoline/trampoline");
        trampolineSprite.addAnimation(new FAnimation("idle", new int[] { 1 }, 100, true));
        trampolineSprite.addAnimation(new FAnimation("activate", new int[] { 2, 1 }, 100, false));
        trampolineSprite.play("idle");
        this.AddChild(trampolineSprite);
        this.SetPosition(pos);
    }

    public void bounce()
    {
        trampolineSprite.play("activate", true);
    }

    public bool contains(Vector2 point)
    {
        return this.x - trampolineSprite.width / 2 < point.x &&
            this.x + trampolineSprite.width / 2 > point.x &&
            this.y - trampolineSprite.height / 2 < point.y &&
            this.y + trampolineSprite.height / 2 > point.y;
    }
}
