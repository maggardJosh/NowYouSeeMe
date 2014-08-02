using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FContainer
{
    FAnimatedSprite playerSprite;
    World world;
    int jumpsLeft = 0;
    float xMove = 0;
    float yMove = 0;
    public Player(World world)
    {
        playerSprite = new FAnimatedSprite("player");
        playerSprite.addAnimation(new FAnimation("idle", new int[] { 0 }, 2000, true));
        playerSprite.play("idle");
        this.AddChild(playerSprite);
        this.world = world;
        C.getCameraInstance().follow(this);
    }

    public override void HandleAddedToStage()
    {
        Futile.instance.SignalFixedUpdate += Update;
        Futile.instance.SignalUpdate += ControlUpdate;
        base.HandleAddedToStage();
    }

    private void ControlUpdate()
    {
        if (Input.GetKeyDown(C.JUMP_KEY) && jumpsLeft > 0)
        {
            yMove = jumpStrength;
            jumpsLeft--;
        }
    }
    float speed = 2;
    float friction = .8f;
    float jumpStrength = 10;
    const float MAX_Y_VEL = 6f;
    private void Update()
    {
        if (Input.GetKey(C.LEFT_KEY))
            xMove = -speed;
        if (Input.GetKey(C.RIGHT_KEY))
            xMove = speed;
      

        if (xMove > 0)
            tryMoveRight(xMove);
        else if (xMove < 0)
            tryMoveLeft(xMove);

        yMove += World.Gravity;
        xMove = 0;

        if (yMove > 0)
            tryMoveUp(yMove);
        else if (yMove < 0)
            tryMoveDown(yMove);
        yMove = Mathf.Clamp(yMove, -MAX_Y_VEL, MAX_Y_VEL);

    }
    float collisionWidth = 12;
    float collisionHeight = 24;
    private void tryMoveRight(float xMove)
    {
        if (world.getMoveable(x + collisionWidth / 2 + xMove, y - collisionHeight*.9f / 2) &&
            world.getMoveable(x + collisionWidth / 2 + xMove, y + collisionHeight*.9f / 2))
            this.x += xMove;
        else
        {
            this.x = Mathf.FloorToInt((this.x + xMove) / world.collision.tileWidth) * world.collision.tileWidth + (world.collision.tileWidth - collisionWidth/2);
        }
    }

    private void tryMoveLeft(float xMove)
    {
        if (world.getMoveable(x - collisionWidth / 2 + xMove, y - collisionHeight*.9f / 2) &&
            world.getMoveable(x - collisionWidth / 2 + xMove, y + collisionHeight*.9f / 2))
            this.x += xMove;
        else
        {
            this.x = Mathf.CeilToInt((this.x + xMove) / world.collision.tileWidth) * world.collision.tileWidth - (world.collision.tileWidth - collisionWidth/2);
        }
    }

    private void tryMoveUp(float yMove)
    {
        if (world.getMoveable(x - collisionWidth*.9f / 2, y + collisionHeight / 2 + yMove) &&
            world.getMoveable(x + collisionWidth*.9f / 2, y + collisionHeight / 2 + yMove))
            this.y += yMove;
        else
            this.y = Mathf.FloorToInt((this.y + yMove) / world.collision.tileHeight) * world.collision.tileHeight + (world.collision.tileHeight - collisionHeight / 2);
    }

    private void tryMoveDown(float yMove)
    {
        if (world.getMoveable(x - collisionWidth*.9f / 2, y - collisionHeight / 2 + yMove) &&
            world.getMoveable(x + collisionWidth*.9f / 2, y - collisionHeight / 2 + yMove))
            this.y += yMove;
        else
        {
            this.y = Mathf.CeilToInt((this.y + yMove) / world.collision.tileHeight) * world.collision.tileHeight - (world.collision.tileHeight - collisionHeight / 2);
            this.yMove = 0;
            this.jumpsLeft = 1;
            RXDebug.Log("LAND");
        }
    }
}

