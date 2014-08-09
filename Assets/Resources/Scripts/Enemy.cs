﻿using System;
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

    private int panacheLeft = 500;

    private State currentState = State.PATROL;
    private bool isFacingLeft = false;

    public enum State
    {
        PATROL,
        SEE_PLAYER,
        CHASE,
        CONFUSED,
        CATCHING
    }

    bool isMoving = false;
    World world;
    public Enemy(World world, bool faceLeft)
    {
        this.isFacingLeft = faceLeft;
        this.world = world;
        enemySprite = new FAnimatedSprite("enemy01");
        enemySprite.addAnimation(new FAnimation("idle", new int[] { 1 }, 200, true));
        enemySprite.addAnimation(new FAnimation("walk", new int[] { 1, 2 }, 200, true));
        enemySprite.addAnimation(new FAnimation("chase", new int[] { 2, 3 }, 200, true));
        enemySprite.addAnimation(new FAnimation("seePlayer", new int[] { 3, 2, 1, 3 }, 200, false));
        enemySprite.addAnimation(new FAnimation("confusion", new int[] { 3, 1, 3, 1, 3, 1, 3, 1 }, 200, false));
        enemySprite.addAnimation(new FAnimation("catchPlayer", new int[] { 1, 2, 3, 1, 2, 3 }, 200, false));
        enemySprite.play("idle");
        this.AddChild(enemySprite);
    }

    public void Update(Player p)
    {
        checkPlayerCaught(p);
        string animToPlay = "";
        switch (currentState)
        {
            case State.PATROL:
                PatrolLogic(p);
                if (xMove == 0)
                    animToPlay = "idle";
                else
                    animToPlay = "walk";
                break;
            case State.CHASE:
                ChaseLogic(p);
                animToPlay = "chase";
                break;
            case State.CONFUSED:
                ConfuseLogic(p);
                return;
            case State.SEE_PLAYER:
                SeePlayerLogic(p);
                return;
            case State.CATCHING:
                CatchPlayerLogic(p);
                return;
        }

        //Flip if facing left
        enemySprite.scaleX = isFacingLeft ? -1 : 1;
        //Test for new state
        switch (currentState)
        {
            case State.CONFUSED:
            case State.SEE_PLAYER:
            case State.CATCHING:
                return;
        }

        if (xMove > 0)
            tryMoveRight(xMove * Time.deltaTime);
        else if (xMove < 0)
            tryMoveLeft(xMove * Time.deltaTime);

        enemySprite.play(animToPlay, false);


    }

    private void checkPlayerCaught(Player p)
    {
        if (p.currentState == Player.State.VANISHING || p.currentState == Player.State.SPAWNING || p.currentState == Player.State.GETTING_CAUGHT)
            return;
        if (p.x - Player.collisionWidth / 2 < this.x + collisionWidth / 2 &&
            p.x + Player.collisionWidth / 2 > this.x - collisionWidth / 2 &&
            p.y + Player.collisionHeight / 2 > this.y - collisionHeight / 2 &&
            p.y - Player.collisionHeight / 2 < this.y + collisionHeight / 2)
        {
            p.getCaught();
            this.x = p.x;   //Put our x to the player's
            p.y = this.y;   //Put the player on the ground
            this.currentState = State.CATCHING;
            enemySprite.play("catchPlayer", true);
        }
    }

    private void SeePlayer()
    {
        enemySprite.play("seePlayer", true);
        currentState = State.SEE_PLAYER;
    }

    private void Confuse()
    {
        enemySprite.play("confusion", true);
        currentState = State.CONFUSED;
    }

    private const float LOSE_SIGHT_DIST = 12 * 15;
    private const float LOSE_SIGHT_Y_DIST = 12 * 5;
    private float chaseSpeed = 100;
    private float patrolSpeed = 50;
    private float turnCount = 0;
    private float turnMax = .8f;
    private float seeDist = 12 * 4;
    private void PatrolLogic(Player p)
    {
        xMove = (isFacingLeft ? -1 : 1) * (turnCount <= 0 ? patrolSpeed : 0);

        if (turnCount > 0)
        {
            turnCount -= Time.deltaTime;
            if (turnCount <= 0)
            {
                turnCount = 0;
                isFacingLeft = !isFacingLeft;
            }
        }

        if (p.currentState == Player.State.VANISHING)
            return;
        if (this.y - collisionHeight / 2 < p.y &&
            this.y + collisionHeight / 2 > p.y)
        {
            Door d = world.checkDoor(this.x, p.x, this.y);
            if (d == null)
            {

                bool sawPlayer = false;
                if (isFacingLeft)
                    sawPlayer = this.x - seeDist < p.x && this.x > p.x;
                else
                    sawPlayer = this.x + seeDist > p.x && this.x < p.x;
                if (sawPlayer)
                    SeePlayer();
            }
        }
    }

    public const float PANACHE_X_DIST = 12 * 4;
    public const float PANACHE_Y_DIST = 12 * 2;
    private void checkVanishPanache(Player p)
    {
        foreach (VanishCloud v in world.panacheEnabledClouds)
        {
            if (Math.Abs(v.x - this.x) < PANACHE_X_DIST &&
                Math.Abs(v.y - this.y) < PANACHE_Y_DIST)
            {
                p.addPanache(panacheLeft);
                panacheLeft = 0;
            }
        }
    }

    private void ChaseLogic(Player p)
    {
        switch (p.currentState)
        {
            case Player.State.VANISHING:
                Confuse();
                checkVanishPanache(p);
                return;
            case Player.State.GETTING_CAUGHT:
                currentState = State.PATROL;
                return;
        }
        if (Math.Abs(p.x - this.x) > LOSE_SIGHT_DIST ||
            Math.Abs(p.y - this.y) > LOSE_SIGHT_Y_DIST)
        {
            Confuse();
            return;
        }
        xMove = Mathf.Lerp(xMove, p.x < this.x ? -chaseSpeed : chaseSpeed, .1f);
        isFacingLeft = p.x < this.x;
    }

    private void ConfuseLogic(Player p)
    {
        if (p.currentState == Player.State.VANISHING || p.currentState == Player.State.GETTING_CAUGHT)
            return;
        if (this.y - collisionHeight / 2 < p.y &&
            this.y + collisionHeight / 2 > p.y)
        {
            Door d = world.checkDoor(this.x, p.x, this.y);
            if (d == null)
            {

                bool sawPlayer = false;
                if (isFacingLeft)
                    sawPlayer = this.x - seeDist < p.x && this.x > p.x;
                else
                    sawPlayer = this.x + seeDist > p.x && this.x < p.x;
                if (sawPlayer)
                    SeePlayer();
            }
        }
        if (enemySprite.IsStopped)
            currentState = State.PATROL;
    }

    private void SeePlayerLogic(Player p)
    {
        if (enemySprite.IsStopped)
            currentState = State.CHASE;
    }

    private void CatchPlayerLogic(Player p)
    {
        if (enemySprite.IsStopped)
        {
            p.respawn();
            Confuse();
        }
    }

    private void turn()
    {
        turnCount = turnMax;
    }
    public const float collisionWidth = 12;
    public const float collisionHeight = 30;
    private void tryMoveRight(float xMove)
    {
        while (xMove > 0)
        {
            float xStep = Math.Min(xMove, world.collision.tileWidth);
            if (world.getMoveable(x + collisionWidth / 2 + xStep, y) &&
                !(world.getMoveable(x + collisionWidth / 2 + xStep, y - collisionHeight / 2 - world.collision.tileHeight / 2) && !world.getOneWay(x + collisionWidth / 2 + xStep, y - collisionHeight / 2 - world.collision.tileHeight / 2)))
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
            if (world.getMoveable(x - collisionWidth / 2 + xStep, y) &&
                !(world.getMoveable(x - collisionWidth / 2 + xStep, y - collisionHeight / 2 - world.collision.tileHeight / 2) && !world.getOneWay(x - collisionWidth / 2 + xStep, y - collisionHeight / 2 - world.collision.tileHeight / 2)))
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


