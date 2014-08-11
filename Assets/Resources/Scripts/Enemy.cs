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

    private int panacheLeft = 500;
    private int panache2Left = 500;

    private State currentState = State.PATROL;
    private bool isFacingLeft = false;

    public enum State
    {
        PATROL,
        SEE_PLAYER,
        CHASE,
        CONFUSED,
        CATCHING,
        FINDING_STAIRWELL,
        ENTERING_STAIRWELL,
        TRANSITION_STAIRWELL,
        EXITING_STAIRWELL
    }

    IndividualStairwell inStair;
    IndividualStairwell outStair;
    bool chaseAfterStair = false;
    bool isMoving = false;
    World world;
    public Enemy(World world, bool faceLeft)
    {
        this.isFacingLeft = faceLeft;
        this.world = world;
        enemySprite = new FAnimatedSprite("enemy01");
        enemySprite.addAnimation(new FAnimation("idle", new int[] { 1 }, 200, true));
        enemySprite.addAnimation(new FAnimation("walk", new int[] { 3, 4, 5, 6 }, 200, true));
        enemySprite.addAnimation(new FAnimation("chase", new int[] { 7, 8, 9, 10 }, 200, true));
        enemySprite.addAnimation(new FAnimation("seePlayer", new int[] { 2, 2, 2, 2 }, 200, false));
        enemySprite.addAnimation(new FAnimation("confusion", new int[] { 2, 2, 2, 2, 2, 2, 2 }, 200, false));
        enemySprite.addAnimation(new FAnimation("catchPlayer", new int[] { 11, 12, 11, 12 }, 200, false));
        enemySprite.addAnimation(new FAnimation("enterStairwell", new int[] { 13, 14, 15 }, 200, false));
        enemySprite.addAnimation(new FAnimation("exitStairwell", new int[] { 16, 17, 18 }, 200, false));
        enemySprite.play("idle");
        this.AddChild(enemySprite);
    }

    private State lastState = State.PATROL;
    private float stateCount = 0;
    const float MIN_STAIR_DIST = 12;
    public void Update(Player p)
    {
        if (lastState != currentState)
        {
            lastState = currentState;
            stateCount = 0;
        }
        checkPlayerCaught(p);
        string animToPlay = "";
        stateCount += Time.deltaTime;
        switch (currentState)
        {
            case State.PATROL:
                chaseAfterStair = false;
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
            case State.FINDING_STAIRWELL:
                chaseAfterStair = true;
                animToPlay = "chase";
                enemySprite.scaleX = inStair.x - this.x < 0 ? -1 : 1;
                if (Math.Abs(inStair.x - this.x) > MIN_STAIR_DIST)
                {
                    xMove = Mathf.Lerp(xMove, inStair.x < this.x ? -chaseSpeed : chaseSpeed, .1f);
                }
                else
                {
                    switch (p.currentState)
                    {
                        case Player.State.ENTERING_STAIRWELL:
                            if (p.outStairwell == this.inStair)
                            {
                                if (chaseAfterStair)
                                {
                                    p.getCaught();
                                    p.SetPosition(this.GetPosition());
                                    this.currentState = State.CATCHING;
                                    enemySprite.play("catchPlayer", true);
                                    return;
                                }
                                else
                                {
                                    outStair = inStair;
                                    chaseAfterStair = true;
                                }
                            }
                            break;
                    }
                    FSoundManager.PlaySound("Stairwell");
                    this.x = inStair.x;
                    currentState = State.ENTERING_STAIRWELL;
                    enemySprite.play("enterStairwell", true);
                    enemySprite.scaleX = inStair.scaleX;
                    return;
                }
                break;
            case State.ENTERING_STAIRWELL:

                enemySprite.scaleX = inStair.scaleX;
                if (enemySprite.IsStopped)
                {
                    currentState = State.TRANSITION_STAIRWELL;
                    enemySprite.isVisible = false;
                }
                return;
            case State.TRANSITION_STAIRWELL:
                if (stateCount > Player.STAIR_TRANS_TIME)
                {
                    currentState = State.EXITING_STAIRWELL;
                    enemySprite.isVisible = true;
                    this.SetPosition(outStair.GetPosition() + Vector2.up * (collisionHeight / 2 - 6));
                    enemySprite.play("exitStairwell", true);
                    FSoundManager.PlaySound("Stairwell");
                }
                return;
            case State.EXITING_STAIRWELL:
                switch (p.currentState)
                {
                    case Player.State.ENTERING_STAIRWELL:
                        if (p.inStairwell == this.outStair)
                        {
                            if (chaseAfterStair)
                            {
                                p.getCaught();
                                p.SetPosition(this.GetPosition());
                                this.currentState = State.CATCHING;
                                enemySprite.play("catchPlayer", true);
                                return;
                            }
                            else
                            {
                                SeePlayer();
                                return;
                            }
                        }
                        break;
                }
                if (enemySprite.IsStopped)
                {
                    xMove = 0;
                    if (chaseAfterStair)
                        currentState = State.CHASE;
                    else
                        currentState = State.PATROL;
                }
                return;
        }

        //Flip if facing left
        enemySprite.scaleX = isFacingLeft ? -1 : 1;

        if (Math.Abs(xMove) > chaseSpeed / 2)
        {
            if (RXRandom.Float() < .8f)
                spawnSparkleParticles(1, -Vector2.up * enemySprite.height / 2);
        }
        else
        {
            if (Math.Abs(xMove) > chaseSpeed / 3)
            {
                if (RXRandom.Float() < .05f)
                    spawnSparkleParticles(3, -Vector2.up * enemySprite.height / 2);
            }
        }
        if (xMove > 0)
            tryMoveRight(xMove * Time.deltaTime);
        else if (xMove < 0)
            tryMoveLeft(xMove * Time.deltaTime);
        //Test for new state
        switch (currentState)
        {
            case State.CONFUSED:
            case State.SEE_PLAYER:
            case State.CATCHING:
                return;
        }
        enemySprite.play(animToPlay, false);


    }

    public void enterStairwell(IndividualStairwell inStair, IndividualStairwell outStair)
    {
        this.inStair = inStair;
        this.outStair = outStair;
        this.SetPosition(inStair.GetPosition() + Vector2.up * collisionHeight / 2);
        isFacingLeft = inStair.scaleX == 1 ? false : true;
        enemySprite.play("enterStairwell", true);
        currentState = State.ENTERING_STAIRWELL;
    }

    private void checkPlayerCaught(Player p)
    {
        switch (p.currentState)
        {
            case Player.State.VANISHING:
            case Player.State.SPAWNING:
            case Player.State.GETTING_CAUGHT:
            case Player.State.ENTERING_STAIRWELL:
            case Player.State.EXITING_STAIRWELL:
            case Player.State.TRANSITION_STAIRWELL:
                return;
        }
        switch (currentState)
        {
            case State.EXITING_STAIRWELL:
            case State.ENTERING_STAIRWELL:
            case State.TRANSITION_STAIRWELL:
                return;
        }
        if (currentState == State.SEE_PLAYER)
            return;
        if (p.x - Player.collisionWidth / 2 < this.x + collisionWidth / 2 &&
            p.x + Player.collisionWidth / 2 > this.x - collisionWidth / 2 &&
            p.y + Player.collisionHeight / 2 > this.y - collisionHeight / 2 &&
            p.y - Player.collisionHeight / 2 < this.y + collisionHeight / 2)
        {
            if (currentState == State.PATROL)
            {
                SeePlayer();
            }
            else
            {

                p.getCaught();
                this.x = p.x;   //Put our x to the player's
                p.y = this.y;   //Put the player on the ground
                this.currentState = State.CATCHING;
                enemySprite.play("catchPlayer", true);
            }
        }
    }

    private void SeePlayer()
    {
        FSoundManager.PlaySound("GuardSeePlayer");
        enemySprite.play("seePlayer", true);
        currentState = State.SEE_PLAYER;
    }

    private void Confuse()
    {
        FSoundManager.PlaySound("GuardConfuse");
        enemySprite.play("confusion", true);
        currentState = State.CONFUSED;
    }

    private const float LOSE_SIGHT_DIST = 12 * 15;
    private const float LOSE_SIGHT_Y_DIST = 12 * 5;
    private float chaseSpeed = 130;
    private float patrolSpeed = 50;
    private float turnCount = 0;
    private float turnMax = .8f;
    private float seeDist = 12 * 6;
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
                {

                    sawPlayer = this.x - seeDist < p.x && this.x > p.x;
                    if (sawPlayer)
                        for (float xDist = this.x; xDist > p.x; xDist -= World.getInstance().map.tileWidth / 2)
                        {
                            if (!world.getMoveable(xDist, this.y))
                            {
                                sawPlayer = false;
                                return;
                            }
                        }
                }
                else
                {
                    sawPlayer = this.x + seeDist > p.x && this.x < p.x;
                    if (sawPlayer)
                        for (float xDist = this.x; xDist < p.x; xDist += World.getInstance().map.tileWidth / 2)
                        {
                            if (!world.getMoveable( xDist, this.y))
                            {
                                sawPlayer = false;
                                return;
                            }
                        }
                }
                if (sawPlayer)
                    SeePlayer();
            }
        }
    }
    public const float PANACHE_X_DIST_2 = 12 * 3;
    public const float PANACHE_X_DIST = 12 * 2;
    public const float PANACHE_Y_DIST = 12 * 2;
    private void checkVanishPanache(Player p)
    {
        int panacheToAdd = 0;
        foreach (VanishCloud v in world.panacheEnabledClouds)
        {
            if (Math.Abs(v.y - this.y) < PANACHE_Y_DIST)
                if (Math.Abs(v.x - this.x) < PANACHE_X_DIST_2)
                {
                    panacheToAdd += panacheLeft;
                    panacheLeft = 0;
                    if (Math.Abs(v.x - this.x) < PANACHE_X_DIST)
                    {
                        panacheToAdd += panache2Left;
                        panache2Left = 0;
                    }
                }
        }
        p.addPanache(panacheToAdd);
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

            case Player.State.ENTERING_STAIRWELL:
                currentState = State.FINDING_STAIRWELL;
                inStair = p.inStairwell;
                outStair = p.outStairwell;
                return;
        }
        if (Math.Abs(p.x - this.x) > LOSE_SIGHT_DIST ||
            Math.Abs(p.y - this.y) > LOSE_SIGHT_Y_DIST)
        {
            Confuse();
            return;
        }
        xMove = Mathf.Lerp(xMove, p.x < this.x ? -chaseSpeed : chaseSpeed, .3f);

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
        if (p.currentState == Player.State.ENTERING_STAIRWELL)
        {
            currentState = State.FINDING_STAIRWELL;
            inStair = p.inStairwell;
            outStair = p.outStairwell;
            return;
        }
        if (p.currentState == Player.State.VANISHING)
        {
            Confuse();
            return;
        }

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
            if (world.getMoveable(x + collisionWidth / 2 + xStep, y - collisionHeight / 4) &&
                !(world.getMoveable(x + collisionWidth / 2 + xStep, y - collisionHeight / 2 - world.collision.tileHeight / 2) && !world.getOneWay(x + collisionWidth / 2 + xStep, y - collisionHeight / 2 - world.collision.tileHeight / 2)))
            {
                Door doorCollision = world.checkDoor(x, x + collisionWidth / 2 + xStep, y);
                if (doorCollision != null)
                {
                    this.turn();
                    this.x = doorCollision.x - collisionWidth / 2 - Door.DOOR_COLLISION_WIDTH / 2;
                    if (currentState == State.CHASE)
                        Confuse();
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
            if (world.getMoveable(x - collisionWidth / 2 + xStep, y - collisionHeight / 4) &&
                !(world.getMoveable(x - collisionWidth / 2 + xStep, y - collisionHeight / 2 - world.collision.tileHeight / 2) && !world.getOneWay(x - collisionWidth / 2 + xStep, y - collisionHeight / 2 - world.collision.tileHeight / 2)))
            {
                Door doorCollision = world.checkDoor(x, x - collisionWidth / 2 + xStep, y);
                if (doorCollision != null)
                {
                    this.turn();
                    this.x = doorCollision.x + collisionWidth / 2 + Door.DOOR_COLLISION_WIDTH / 2;
                    if (currentState == State.CHASE)
                        Confuse();
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


    private void spawnSparkleParticles(int numParticles, Vector2 disp, float particleDist = 0, bool spawnBehindPlayer = false)
    {
        float particleXSpeed = 20;
        float particleYSpeed = 20;
        for (int x2 = 0; x2 < numParticles; x2++)
        {
            Particle particle = Particle.SparkleParticle.getParticle();
            float angle = (RXRandom.Float() * Mathf.PI * 2);
            Vector2 pos = this.GetPosition() + disp + new Vector2(Mathf.Cos(angle) * particleDist, Mathf.Sin(angle) * particleDist);
            particle.activate(pos, new Vector2(RXRandom.Float() * particleXSpeed * 2 - particleXSpeed, RXRandom.Float() * particleYSpeed * 2 - particleYSpeed), Vector2.zero, 360);
            this.container.AddChild(particle);
            if (spawnBehindPlayer)
                particle.MoveToBack();
        }
    }
    private void spawnSparkleParticles(int numParticles)
    {
        spawnSparkleParticles(numParticles, Vector2.zero);
    }
}


