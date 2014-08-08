using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Enemy : FContainer
{
    private FAnimatedSprite enemySprite;
    private float animSpeed = 200;
    private Player p;

    private float xMove;
    private float yMove;

    private State currentState = State.PATROL;
    private bool isFacingLeft = false;

    public enum State
    {
        PATROL,
        CHASE,
        CONFUSED
    }

    bool isMoving = false;
    World world;
    public Enemy(World world)
    {
        this.world = world;
        enemySprite = new FAnimatedSprite("enemy01");
        enemySprite.addAnimation(new FAnimation("idle", new int[] { 1 }, 200, true));
        enemySprite.addAnimation(new FAnimation("walk", new int[] { 1, 2 }, 200, true));
        enemySprite.addAnimation(new FAnimation("chase", new int[] { 2, 3 }, 200, true));
        enemySprite.addAnimation(new FAnimation("confusion", new int[] { 3, 1, 3, 1, 3, 1 }, 200, true));
        enemySprite.play("idle");
        this.AddChild(enemySprite);
    }

    const float MAX_Y_VEL = 10f;
    const float MIN_Y_VEL = -6f;
    const float MAX_X_VEL = 4f;

    public void Update()
    {


        string animToPlay = "";
        switch (currentState)
        {
            case State.PATROL:
                PatrolLogic();
                if (xMove == 0)
                    animToPlay = "idle";
                else
                    animToPlay = "walk";
                break;
            case State.CHASE:

                break;
            case State.CONFUSED:

                break;
        }

        if (xMove > 0)
            tryMoveRight(xMove);
        else if (xMove < 0)
            tryMoveLeft(xMove);

        enemySprite.play(animToPlay, false);
        //Flip if facing left
        enemySprite.scaleX = isFacingLeft ? -1 : 1;


    }

    private float patrolSpeed = 50;
    private float turnCount = 0;
    private float turnMax = .8f;
    private void PatrolLogic()
    {
        xMove = (isFacingLeft ? -1 : 1) * (turnCount <= 0 ? patrolSpeed : 0) * Time.deltaTime;

        if (turnCount > 0)
        {
            turnCount -= Time.deltaTime;
            if (turnCount <= 0)
            {
                turnCount = 0;
                isFacingLeft = !isFacingLeft;
            }
        }
    }

    private void turn()
    {
        turnCount = turnMax;
    }
    public const float collisionWidth = 12;
    public const float collisionHeight = 20;
    private void tryMoveRight(float xMove)
    {
        while (xMove > 0)
        {
            float xStep = Math.Min(xMove, world.collision.tileWidth);
            if (world.getMoveable(x + collisionWidth / 2 + xStep, y - collisionHeight * .9f / 2) &&
                world.getMoveable(x + collisionWidth / 2 + xStep, y + collisionHeight * .9f / 2) &&
                !(world.getMoveable(x + collisionWidth + xStep, y - collisionHeight) && !world.getOneWay(x + collisionWidth + xStep, y - collisionHeight)))
            {
                Door doorCollision = world.checkDoor(x, x + collisionWidth / 2 + xStep, y);
                if (doorCollision != null)
                {
                    this.turn();
                    return;
                }
                else
                {
                    this.x += xStep;
                    xMove -= xStep;
                }
            }
            else
            {
                this.turn();
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
                world.getMoveable(x - collisionWidth / 2 + xStep, y + collisionHeight * .9f / 2) &&
                !(world.getMoveable(x - collisionWidth + xStep, y - collisionHeight) && !world.getOneWay(x - collisionWidth + xStep, y - collisionHeight)))
            {
                Door doorCollision = world.checkDoor(x, x - collisionWidth / 2 + xStep, y);
                if (doorCollision != null)
                {
                    this.turn();
                    return;
                }
                else
                {
                    this.x += xStep;
                    xMove -= xStep;
                }
            }
            else
            {
                this.turn();
                return;
            }
        }
    }


    private void tryMoveDown(float yMove)
    {
        //  bool onOneWay = world.getOneWay(x, y - playerSprite.height / 2 + yMove);
        //  PressurePlate p = world.checkPlates(this.x, this.y - playerSprite.height / 2 + yMove);
        //  if (p != null)
        //  {
        //      this.y = p.y - 6 + collisionHeight / 2 + PressurePlate.PRESSED_HEIGHT;
        //      this.yMove = 0;
        //      this.isGrounded = true;
        //      this.jumpsLeft = 1;
        //      return;
        //  }
        //  if (world.getMoveable(x - collisionWidth * .9f / 2, y - playerSprite.height / 2 + yMove) &&
        //      world.getMoveable(x + collisionWidth * .9f / 2, y - playerSprite.height / 2 + yMove) && !onOneWay)
        //  {
        //      this.y += yMove;
        //      isGrounded = false;
        //
        //
        //
        //  }
        //  else
        //  {
        //      if (onOneWay && downJumpCount > 0)
        //          this.y += yMove;
        //      else
        //      {
        //          this.y = Mathf.CeilToInt((this.y - playerSprite.height / 2 + yMove) / world.collision.tileHeight) * world.collision.tileHeight + playerSprite.height / 2;
        //          this.yMove = 0;
        //          this.jumpsLeft = 1;
        //          if (!isGrounded)
        //          {
        //              isGrounded = true;
        //              spawnFootParticles(4);
        //          }
        //      }
        //  }
    }
}

