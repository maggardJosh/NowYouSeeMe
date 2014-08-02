using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FContainer
{
    FAnimatedSprite playerSprite;
    public Player()
    {
        playerSprite = new FAnimatedSprite("player");
        playerSprite.addAnimation(new FAnimation("idle", new int[] { 0 }, 2000, true));
        playerSprite.play("idle");
        this.AddChild(playerSprite);
    }

    public override void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToStage();
    }

    float speed = 50;
    private void Update()
    {
        if (Input.GetKey(C.LEFT_KEY))
            x -= Time.deltaTime * speed;
        if (Input.GetKey(C.RIGHT_KEY))
            x += Time.deltaTime * speed;

    }
}

