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
    float speed = 1f;
    float airSpeed = .2f;
    float friction = .5f;
    float airFriction = .99f;
    float jumpStrength = 10;
    const float MAX_Y_VEL = 6f;
    const float MAX_X_VEL = 6f;
    bool isGrounded = true;
    private void Update()
    {
        if (Input.GetKey(C.LEFT_KEY))
            xMove -= isGrounded ? speed : airSpeed;
        if (Input.GetKey(C.RIGHT_KEY))
            xMove += isGrounded ? speed : airSpeed;

        if (xMove > 0)
            tryMoveRight(xMove);
        else if (xMove < 0)
            tryMoveLeft(xMove);

        yMove += World.Gravity;


        if (yMove > 0)
            tryMoveUp(yMove);
        else if (yMove < 0)
            tryMoveDown(yMove);
        yMove = Mathf.Clamp(yMove, -MAX_Y_VEL, MAX_Y_VEL);

        if (yMove > 0)
            isGrounded = false;

        if (isGrounded)
            xMove *= friction;
        else
            xMove *= airFriction;

    }
    float collisionWidth = 12;
    float collisionHeight = 24;
    private void tryMoveRight(float xMove)
    {
        while (xMove > 0)
        {
            float xStep = Math.Min(xMove, world.collision.tileWidth);
            if (world.getMoveable(x + collisionWidth / 2 + xStep, y - collisionHeight * .9f / 2) &&
                world.getMoveable(x + collisionWidth / 2 + xStep, y + collisionHeight * .9f / 2))
            {
                this.x += xStep;
                xMove -= xStep;
            }
            else
            {
                this.x = Mathf.FloorToInt((this.x + xMove) / world.collision.tileWidth) * world.collision.tileWidth + (world.collision.tileWidth - collisionWidth / 2);
                this.xMove = 0;
                return;
            }
        }
    }

    private void tryMoveLeft(float xMove)
    {
        while (xMove < 0)
        {
            float xStep = Math.Max(xMove, -world.collision.tileWidth);
            if (world.getMoveable(x - collisionWidth / 2 + xStep, y - collisionHeight * .9f / 2) &&
                world.getMoveable(x - collisionWidth / 2 + xStep, y + collisionHeight * .9f / 2))
            {
                this.x += xMove;
                xMove -= xStep;
            }
            else
            {
                this.x = Mathf.CeilToInt((this.x + xStep) / world.collision.tileWidth) * world.collision.tileWidth - (world.collision.tileWidth - collisionWidth / 2);
                this.xMove = 0;
                return;
            }
        }
    }

    private void tryMoveUp(float yMove)
    {
        if (world.getMoveable(x - collisionWidth * .9f / 2, y + collisionHeight / 2 + yMove) &&
            world.getMoveable(x + collisionWidth * .9f / 2, y + collisionHeight / 2 + yMove))
            this.y += yMove;
        else
            this.y = Mathf.FloorToInt((this.y + yMove) / world.collision.tileHeight) * world.collision.tileHeight + (world.collision.tileHeight - collisionHeight / 2);
    }

    private void tryMoveDown(float yMove)
    {
        if (world.getMoveable(x - collisionWidth * .9f / 2, y - playerSprite.height / 2 + yMove) &&
            world.getMoveable(x + collisionWidth * .9f / 2, y - playerSprite.height / 2 + yMove))
            this.y += yMove;
        else
        {
            this.y = Mathf.CeilToInt((this.y - playerSprite.height / 2 + yMove) / world.collision.tileHeight) * world.collision.tileHeight + playerSprite.height / 2;
           // this.y = Mathf.CeilToInt((this.y + yMove) / world.collision.tileHeight) * world.collision.tileHeight - (world.collision.tileHeight - collisionHeight / 2);
            this.yMove = 0;
            this.jumpsLeft = 1;
            isGrounded = true;
        }
    }
}

