﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FContainer
{
    // State for player
    // Marking is when the hat is placed on the map
    // Vanishing is when the player is vanishing back to the hat
    public enum State
    {
        IDLE,
        LOOTING,
        VANISHING,
        COOLDOWN,
        SPAWNING,
        ENDING_LEVEL,
        GETTING_CAUGHT,
        ENTERING_STAIRWELL,
        TRANSITION_STAIRWELL,
        EXITING_STAIRWELL,
        SUICIDE
    }
    FAnimatedSprite interactInd;
    FAnimatedSprite playerSprite;
    public State currentState = State.IDLE;
    private State lastState = State.IDLE;
    Hat hat;
    World world;
    int jumpsLeft = 0;
    public float xMove = 0;
    public float yMove = 0;
    float stateCount = 0;
    bool isFacingLeft = false;
    public bool isMarking = false;
    bool isInteracting = false;

    private float markCount = 0;

    public GUICounter cashCounter;
    public GUICounter panacheCounter;

    const float INDICATOR_Y = 20;
    const float INDICATOR_BOUNCE = 5;
    public IndividualStairwell inStairwell = null;
    public IndividualStairwell outStairwell = null;

    int animSpeed = 200;
    public Player(World world)
    {
        playerSprite = new FAnimatedSprite("player");
        playerSprite.addAnimation(new FAnimation("hat_idle", new int[] { 13 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hat_walk", new int[] { 1, 2, 3, 4 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hat_run", new int[] { 1, 2, 3, 4 }, animSpeed / 2, true));
        playerSprite.addAnimation(new FAnimation("hat_air_up", new int[] { 5 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hat_air_down", new int[] { 6 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hat_loot", new int[] { 15 }, 500, false));
        playerSprite.addAnimation(new FAnimation("hat_interact", new int[] { 2 }, 250, false));
        playerSprite.addAnimation(new FAnimation("hat_stairwellEnter", new int[] { 17, 18, 19 }, 150, false));
        playerSprite.addAnimation(new FAnimation("hat_stairwellExit", new int[] { 20, 21, 22 }, 150, false));
        playerSprite.addAnimation(new FAnimation("hatless_idle", new int[] { 14 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hatless_walk", new int[] { 7, 8, 9, 10 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hatless_run", new int[] { 7, 8, 9, 10 }, animSpeed / 2, true));
        playerSprite.addAnimation(new FAnimation("hatless_air_up", new int[] { 11 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hatless_air_down", new int[] { 12 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hatless_loot", new int[] { 16 }, 500, false));
        playerSprite.addAnimation(new FAnimation("hatless_interact", new int[] { 8 }, 250, true));
        playerSprite.addAnimation(new FAnimation("hatless_stairwellEnter", new int[] { 23, 24, 25 }, 150, false));
        playerSprite.addAnimation(new FAnimation("hatless_stairwellExit", new int[] { 26, 27, 28 }, 150, false));

        playerSprite.addAnimation(new FAnimation("suicide", new int[] { 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 30, 30, 30, 30, 31, 32, 33, 34, 35, 36, 37, 38 }, animSpeed / 4, false));


        playerSprite.play("hat_idle");
        this.AddChild(playerSprite);
        this.world = world;
        C.getCameraInstance().follow(this);
        hat = new Hat(this);
        cashCounter = new GUICounter();

        panacheCounter = new GUICounter();

        interactInd = new FAnimatedSprite("InteractIndicator/interactIndicator");
        interactInd.addAnimation(new FAnimation("active", new int[] { 1, 2 }, 400, true));
        interactInd.play("active");
        this.AddChild(interactInd);
        interactInd.isVisible = false;
        interactInd.y = INDICATOR_Y;
        Go.to(interactInd, 1.0f, new TweenConfig().floatProp("y", INDICATOR_BOUNCE, true).setIterations(-1, LoopType.PingPong).setEaseType(EaseType.QuadOut));
        interactInd.isVisible = false;

    }
    #region GUI Stuff
    public float getVelocityAngle()
    {
        if (xMove == 0 && yMove == 0)
            return 90;     //If we aren't moving just point the hat up
        else
            return Mathf.Atan2(-yMove, xMove) * 180.0f / Mathf.PI;
    }

    public float GetVanishPercent()
    {
        if (isMarking)
            return hasLeftMarkPos ? 1 - markCount / MARK_MAX_COUNT : 1;
        switch (currentState)
        {
            case State.VANISHING: return 0;
            case State.COOLDOWN: return stateCount / HAT_RETURN_COUNT;
            default: return 1.0f;
        }
    }
    #endregion

    public override void HandleAddedToStage()
    {
        Futile.instance.SignalFixedUpdate += Update;
        Futile.instance.SignalUpdate += ControlUpdate;
        base.HandleAddedToStage();
    }

    public override void HandleRemovedFromStage()
    {
        Futile.instance.SignalFixedUpdate -= Update;
        Futile.instance.SignalUpdate -= ControlUpdate;
        base.HandleRemovedFromStage();
    }

    #region Mark/Vanish Stuff
    private void Mark()
    {
        this.container.AddChild(hat);
        hat.appear();
        hat.SetPosition(this.GetPosition());
        hasLeftMarkPos = false;

    }
    const float VANISH_DURATION = 1.0f;
    const float HAT_RETURN_COUNT = 1.0f;
    private const float MARK_MAX_COUNT = 3.0f;

    private void Vanish()
    {
        VanishCloud cloud = new VanishCloud();
        cloud.SetPosition(this.GetPosition());
        this.container.AddChild(cloud);

        VanishCloud newPosCloud = new VanishCloud(false);
        newPosCloud.SetPosition(hat.GetPosition());
        playerSprite.isVisible = false;

        jumpsLeft = 0;

        FSoundManager.PlaySound("VanishCloud");
        currentState = State.VANISHING;
        isMarking = false;
        if (yMove > 0)
        {
            yMove = Math.Max(yMove, jumpStrength);
        }
        Go.to(this, VANISH_DURATION, new TweenConfig().floatProp("x", hat.x).floatProp("y", hat.y).setEaseType(EaseType.CircInOut).onComplete((a) => { currentState = State.COOLDOWN; FSoundManager.PlaySound("VanishCloud"); hat.disappear(); playerSprite.isVisible = true; this.container.AddChild(newPosCloud); }));
    }

    private void MarkTimeOut()
    {
        VanishCloud cloud = new VanishCloud(false);
        cloud.SetPosition(this.GetPosition());
        this.container.AddChild(cloud);

        VanishCloud newPosCloud = new VanishCloud(false);
        newPosCloud.SetPosition(hat.GetPosition());
        this.container.AddChild(newPosCloud);

        hat.disappear();
        isMarking = false;
        currentState = State.COOLDOWN;
    }

    private void CancelVanish()
    {
        spawnVanishParticles(3, Vector2.up * 10);

        hat.disappear();
        isMarking = false;
    }
    #endregion

    private Chest activatedChest;
    public void activateChest(Chest chest)
    {
        if (activatedChest != null)
            activatedChest.deactivate();
        chest.activate();
        activatedChest = chest;
    }

    
    public void respawn()
    {
        if (isMarking)
            CancelVanish();

        yMove = 0;
        xMove = 0;
        isGrounded = true;
        VanishCloud cloud = new VanishCloud(false);
        cloud.SetPosition(this.GetPosition());
        this.container.AddChild(cloud);
        this.isVisible = false;
        FSoundManager.PlaySound("VanishCloud");

        currentState = State.VANISHING;
        Go.to(this, VANISH_DURATION * 2, new TweenConfig().floatProp("x", activatedChest.x).floatProp("y", activatedChest.y + collisionHeight / 3).setEaseType(EaseType.CircInOut).onComplete((a) =>
        {
            this.addPanache(-panacheCounter.actualValue, true);
            isFacingLeft = false;
            tryMoveDown(-.1f); currentState = State.SPAWNING; activatedChest.spawnPlayer(this);
            
        }));
    }
    internal void preSpawn()
    {
        currentState = State.SPAWNING;
        this.isVisible = false;
        this.x = activatedChest.x;
        this.y = activatedChest.y;
        tryMoveDown(-.1f);
        isFacingLeft = false;
    }
    public void spawn()
    {
        currentState = State.SPAWNING;
        if (activatedChest != null)
        {
            this.x = activatedChest.x;
            this.y = activatedChest.y;
            tryMoveDown(-.1f);
            this.isVisible = false;
            isFacingLeft = false;
            activatedChest.spawnPlayer(this);
        }
        else
            throw new Exception("No Activated Chest");
    }

    public void enterStairwell(IndividualStairwell inStair, IndividualStairwell outStair)
    {
        if (isMarking)
            hasLeftMarkPos = true;
        this.SetPosition(inStair.GetPosition() + Vector2.up * collisionHeight / 2);
        playerSprite.scaleX = inStair.scaleX;
        this.inStairwell = inStair;
        this.outStairwell = outStair;
        xMove = 0;
        yMove = 0;

        FSoundManager.PlaySound("Stairwell");
        playerSprite.play(getHatAnimPrefix() + "stairwellEnter");
        currentState = State.ENTERING_STAIRWELL;

    }

    private InteractableObject currentInteractable;
    public void setInteractObject(InteractableObject o)
    {
        currentInteractable = o;
        interactInd.isVisible = true;
    }
    public void clearInteractable()
    {
        currentInteractable = null;
        interactInd.isVisible = false;
    }

    public void addCash(int amount, bool forcedDisplay = false)
    {
        if (amount == 0 && !forcedDisplay)
            return;
        cashCounter.addAmount(amount);
        LabelIndicator cashInd = new LabelIndicator((amount > 0 ? "+" : "-") + "$" + Math.Abs(amount), "moneyInd", amount < 0);
        cashInd.SetPosition(this.GetPosition() + Vector2.up * 10);
        this.container.AddChild(cashInd);
    }

    public void addPanache(int amount, bool forcedDisplay = false)
    {
        if (amount == 0 && !forcedDisplay)
            return;
        panacheCounter.addAmount(amount);
        LabelIndicator panacheInd = new LabelIndicator((amount > 0 ? "+" : "-") + Math.Abs(amount), "panacheInd", amount < 0);
        panacheInd.SetPosition(this.GetPosition() + Vector2.up * 10);
        this.container.AddChild(panacheInd);

    }
    float interactCount = 0;
    private void ControlUpdate()
    {
        if (isMarking)
        {
            if (!hasLeftMarkPos)
                markCount = 0;
            if (markCount >= MARK_MAX_COUNT)
                MarkTimeOut();
            else
                if (!C.getActionPressed())
                {
                    if (hasLeftMarkPos)
                        Vanish();
                    else
                        CancelVanish();
                }

        }
        if (currentState != lastState)
            stateCount = 0;

        lastState = currentState;

        switch (currentState)
        {
            case State.IDLE:
                if (!isMarking)
                {
                    if (C.getActionPressed())
                    {
                        if (isGrounded && C.getDownPressed())
                        {
                            currentState = State.SUICIDE;
                            playerSprite.play("suicide", true);
                        }
                        else
                        {
                            isMarking = true;
                            Mark();
                        }
                    }
                }
                break;

            case State.VANISHING:
            case State.SPAWNING:
            case State.ENTERING_STAIRWELL:
            case State.EXITING_STAIRWELL:
                if (isMarking)
                    markCount += Time.deltaTime;
                interactInd.isVisible = false;
                isInteracting = false;
                return;
            case State.TRANSITION_STAIRWELL:
                interactInd.isVisible = false;
                isInteracting = false;
                return;     //Don't allow controls past this if vanishing
            case State.COOLDOWN:
                if (stateCount > HAT_RETURN_COUNT)
                    currentState = State.IDLE;
                break;
            case State.GETTING_CAUGHT:
                interactInd.isVisible = false;
                return;
            case State.LOOTING:
                interactInd.isVisible = false;
                if (isMarking)
                    markCount += Time.deltaTime;

                return;
            case State.SUICIDE:
                return;
            case State.ENDING_LEVEL:
                return;
        }
        if (isInteracting)
        {
            currentInteractable = null;
            if (interactCount > .3f)
            {
                interactCount = 0;
                isInteracting = false;
            }
            else
            {
                interactCount += Time.deltaTime;
            }
        }
        stateCount += Time.deltaTime;
        if (isMarking)
            markCount += Time.deltaTime;

        if (C.getUpPress() && currentInteractable != null)
        {
            switch (currentInteractable.interactType)
            {
                case InteractableObject.InteractType.LOOT:
                    this.x = currentInteractable.x;
                    currentState = State.LOOTING;
                    FSoundManager.PlaySound("Loot");
                    this.playerSprite.play(getHatAnimPrefix() + "loot", true);
                    xMove = 0;
                    yMove = 0;
                    break;
                case InteractableObject.InteractType.INTERACT:
                    this.playerSprite.play("interact", true);
                    isInteracting = true;
                    currentInteractable.interact(this);
                    spawnVanishParticles(2, new Vector2(isFacingLeft ? -15 : 15, 0));
                    break;
            }
            return;
        }
        if (downJumpCount > 0)
            downJumpCount = Math.Max(0, downJumpCount - Time.deltaTime);
        if (C.getJumpPress() && jumpsLeft > 0)
        {
            FSoundManager.PlaySound("Jump");
            if (C.getDownPressed())
                downJumpCount = DOWN_JUMP_TIME;
            yMove = jumpStrength * (C.getDownPressed() ? .4f : 1);
            jumpsLeft--;
        }
    }


    float downJumpCount = 0;
    const float DOWN_JUMP_TIME = .4f;       //Time to allow a down jump

    float speed = .1f;
    float airSpeed = .1f;
    float highVelFriction = .9f;
    float normalFriction = .8f;
    float airFriction = .99f;
    float jumpStrength = 6;
    const float MAX_Y_VEL = 10f;
    const float MIN_Y_VEL = -6f;
    const float MAX_X_VEL = 2f;
    bool isGrounded = true;
    bool isMoving = false;
    public bool hasLeftMarkPos = false;
    const float MARK_MIN_DIST = 20;     //Distance from mark that causes the timer to start
    const float MIN_MOVEMENT_X = .1f;
    public const float Gravity = -.3f;
    bool hitMaxXVel = false;
    float maxVelTime = 0;
    bool isSprinting = false;
    bool wasSprinting = false;
    bool wasMaxSpeed = false;
    public const float STAIR_TRANS_TIME = 1.0f;
    float lastXInc = 0;
    private void Update()
    {
        if (C.isTransitioning)
            return;
        float xAcc = 0;
        switch (currentState)
        {
            case State.VANISHING:
                return;
            case State.SPAWNING:
                return;
            case State.ENDING_LEVEL:
                return;
            case State.ENTERING_STAIRWELL:
                if (playerSprite.IsStopped)
                {
                    currentState = State.TRANSITION_STAIRWELL;
                    this.isVisible = false;
                    Go.to(this, STAIR_TRANS_TIME, new TweenConfig().floatProp("x", outStairwell.x).floatProp("y", outStairwell.y + collisionHeight / 2).setEaseType(EaseType.CircInOut).onComplete((a) => { this.isVisible = true; playerSprite.play(getHatAnimPrefix() + "stairwellExit", true); FSoundManager.PlaySound("Stairwell"); currentState = State.EXITING_STAIRWELL; }));
                }
                return;
            case State.TRANSITION_STAIRWELL:
                return;
            case State.EXITING_STAIRWELL:
                if (playerSprite.IsStopped)
                    currentState = State.IDLE;
                return;
            case State.GETTING_CAUGHT:
                //After the enemy's anim gets done playing we respawn
                return;
            case State.LOOTING:
                if (RXRandom.Float() < .7f)
                    spawnVanishParticles(1, Vector2.zero - Vector2.up * collisionHeight / 4, 8, true);
                if (playerSprite.IsStopped)
                {
                    currentState = State.IDLE;
                    currentInteractable.interact(this);
                    currentInteractable = null;
                }
                return;
            case State.SUICIDE:
                this.x -= lastXInc;
                if (!C.getDownPressed() || !C.getActionPressed())
                {
                    currentState = State.IDLE;
                }
                else
                {
                    if (playerSprite.currentFrame == 29)
                    {
                        lastXInc = RXRandom.Float() * 2 - 1;
                        this.x += lastXInc;
                    }
                    else
                    {
                        lastXInc = 0;
                    }
                    if (playerSprite.IsStopped)
                    {
                        spawnVanishParticles(20);
                        this.addCash(-cashCounter.actualValue / 2, true);
                        respawn();
                    }
                }
                return;
        }

        if (C.getLeftPressed())
        {
            xAcc = isGrounded ? -speed : -airSpeed;
            if (xMove > 0)
            {
                xAcc *= 4;
                if (xMove > MAX_X_VEL / 2 && isGrounded)
                {
                    spawnSparkleParticles(1, Vector2.right * 10 - Vector2.up * playerSprite.height / 2);
                }
            }
            if (xMove > -MAX_X_VEL / 2)
            {
                wasSprinting = false;
                wasMaxSpeed = false;
            }
            isFacingLeft = true;
        }
        if (C.getRightPressed())
        {
            xAcc = isGrounded ? speed : airSpeed;
            if (xMove < 0)
            {
                xAcc *= 4;
                if (xMove < -MAX_X_VEL / 2 && isGrounded)
                {
                    spawnSparkleParticles(1, Vector2.right * -10 - Vector2.up * playerSprite.height / 2);
                }
            }
            if (xMove < MAX_X_VEL / 2)
            {
                wasSprinting = false;
                wasMaxSpeed = false;
            }
            isFacingLeft = false;
        }

        xMove += xAcc;

        xMove = Mathf.Clamp(xMove, -MAX_X_VEL, MAX_X_VEL);
        if (hitMaxXVel)
        {
            hitMaxXVel = Math.Abs(xMove) == MAX_X_VEL;
            if (!hitMaxXVel)
                maxVelTime = 0;
            else
            {
                if (RXRandom.Float() < .3f)
                    spawnSparkleParticles(1, -Vector2.up * playerSprite.height / 2);
            }
            if ((xMove > 0 && C.getRightPressed()) ||
               (xMove < 0 && C.getLeftPressed()))
            {
                if (maxVelTime > 1.0f)
                {
                    if (RXRandom.Float() < .8f)
                        spawnSparkleParticles(1, -Vector2.up * playerSprite.height / 2);
                    xMove *= 1.2f;
                    isSprinting = true;
                    wasSprinting = true;
                }
                else
                {
                    maxVelTime += Time.deltaTime;
                }
            }
        }
        else
        {
            isSprinting = false;
            if (isGrounded && Math.Abs(xMove) == MAX_X_VEL)
            {
                wasMaxSpeed = true;
                hitMaxXVel = true;
            }
        }


        if (xMove > 0)
            tryMoveRight(xMove);
        else if (xMove < 0)
            tryMoveLeft(xMove);

        yMove += Gravity;

        yMove = Mathf.Clamp(yMove, MIN_Y_VEL, MAX_Y_VEL);

        if (yMove > 0)
            tryMoveUp(yMove);
        else if (yMove < 0)
            tryMoveDown(yMove);

        if (yMove > 0)
        {
            isGrounded = false;
            jumpsLeft = 0;
        }

        if (xAcc == 0)
        {
            if (isGrounded)
            {
                if (!wasMaxSpeed)
                    xMove *= normalFriction;
                else
                    xMove *= highVelFriction;
                if (wasSprinting || (wasMaxSpeed && RXRandom.Float() < .3f))
                    if (xMove > 0)
                        spawnSparkleParticles(1, Vector2.right * 10 - Vector2.up * playerSprite.height / 2);
                    else if (xMove < 0)
                        spawnSparkleParticles(1, Vector2.right * -10 - Vector2.up * playerSprite.height / 2);

            }
            else
                xMove *= airFriction;
        }

        if (Math.Abs(xMove) < MIN_MOVEMENT_X)
            xMove = 0;
        isMoving = xAcc != 0;
        string animToPlay = getHatAnimPrefix();
        if (yMove == 0)
        {
            if (isMoving)
            {
                if (isSprinting)
                    animToPlay += "run";
                else
                    animToPlay += "walk";
            }
            else
                animToPlay += isInteracting ? "interact" : "idle";
        }
        else
        {
            if (yMove > 0)
                animToPlay += "air_up";
            else
                animToPlay += "air_down";

        }
        playerSprite.play(animToPlay, false);
        //Flip if facing left
        playerSprite.scaleX = isFacingLeft ? -1 : 1;

        if (isMarking && !hasLeftMarkPos)
            if ((hat.GetPosition() - this.GetPosition()).sqrMagnitude > MARK_MIN_DIST * MARK_MIN_DIST)
                hasLeftMarkPos = true;

        updateWorld();
    }

    public void stopSpawning()
    {
        this.isVisible = true;
        currentState = State.IDLE;
        playerSprite.isVisible = true;
        playerSprite.play("hat_idle", true);
        playerSprite.scaleX = 1;
    }

    private string getHatAnimPrefix()
    {
        return isMarking ? "hatless_" : "hat_";
    }

    private void updateWorld()
    {
        if (yMove < 0)
            foreach (Trampoline t in world.trampolineList)
                checkTrampoline(t);
    }

    private void checkTrampoline(Trampoline t)
    {
        Vector2 checkPoint = new Vector2(x + collisionWidth * .9f / 2, y - playerSprite.height / 2 + yMove);
        if (t.contains(checkPoint))
            TrampolineBounce(t);
    }

    private void TrampolineBounce(Trampoline t)
    {
        t.bounce();
        FSoundManager.PlaySound("Trampoline");
        this.yMove = Mathf.Clamp(Math.Abs(yMove) * 2, 2, 10);
    }

    public const float collisionWidth = 12;
    public const float collisionHeight = 20;
    private void tryMoveRight(float xMove)
    {
        while (xMove > 0)
        {
            float xStep = Math.Min(xMove, world.collision.tileWidth);
            if (world.getMoveable(x + collisionWidth / 2 + xStep, y - collisionHeight * .9f / 2) &&
                world.getMoveable(x + collisionWidth / 2 + xStep, y + collisionHeight * .9f / 2))
            {
                Door doorCollision = world.checkDoor(x, x + collisionWidth / 2 + xStep, y);
                if (doorCollision != null)
                {
                    this.x = doorCollision.x - collisionWidth / 2 - Door.DOOR_COLLISION_WIDTH / 2;
                    this.xMove = 0;
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
                Door doorCollision = world.checkDoor(x, x - collisionWidth / 2 + xStep, y);
                if (doorCollision != null)
                {
                    this.x = doorCollision.x + collisionWidth / 2 + Door.DOOR_COLLISION_WIDTH / 2;
                    this.xMove = 0;
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
        {
            this.y = Mathf.FloorToInt((this.y + yMove) / world.collision.tileHeight) * world.collision.tileHeight + (world.collision.tileHeight - collisionHeight / 2);
            this.yMove *= .5f;
        }
    }

    private void spawnSparkleParticles(int numParticles, Vector2 disp, float particleDist = 0, bool spawnBehindPlayer = false)
    {

        if (!isGrounded)
            return;
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


    private void spawnVanishParticles(int numParticles, Vector2 disp, float particleDist = 0, bool spawnBehindPlayer = false)
    {
        if (!isGrounded)
            return;
        float particleXSpeed = 30;
        float particleYSpeed = 30;
        for (int x2 = 0; x2 < numParticles; x2++)
        {
            Particle particle = Particle.VanishParticle.getParticle();
            float angle = (RXRandom.Float() * Mathf.PI * 2);
            Vector2 pos = this.GetPosition() + disp + new Vector2(Mathf.Cos(angle) * particleDist, Mathf.Sin(angle) * particleDist);
            particle.activate(pos, new Vector2(RXRandom.Float() * particleXSpeed * 2 - particleXSpeed, RXRandom.Float() * particleYSpeed * 2 - particleYSpeed), Vector2.zero, 360);
            this.container.AddChild(particle);
            if (spawnBehindPlayer)
                particle.MoveToBack();
        }
    }
    private void spawnVanishParticles(int numParticles)
    {
        spawnVanishParticles(numParticles, Vector2.zero);
    }


    private void tryMoveDown(float yMove)
    {
        bool onOneWay = world.getOneWay(x, y - playerSprite.height / 2 + yMove);
        PressurePlate p = world.checkPlates(this.x, this.y - playerSprite.height / 2 + yMove);
        if (p != null)
        {
            this.y = p.y - 6 + collisionHeight / 2 + PressurePlate.PRESSED_HEIGHT;
            this.yMove = 0;
            this.isGrounded = true;
            this.jumpsLeft = 1;
            return;
        }
        if (world.getMoveable(x - collisionWidth * .9f / 2, y - playerSprite.height / 2 + yMove) &&
            world.getMoveable(x + collisionWidth * .9f / 2, y - playerSprite.height / 2 + yMove) && !onOneWay)
        {
            this.y += yMove;
            isGrounded = false;
        }
        else
        {
            if (onOneWay && downJumpCount > 0)
                this.y += yMove;
            else
            {
                this.y = Mathf.CeilToInt((this.y - playerSprite.height / 2 + yMove - .2f) / world.collision.tileHeight) * world.collision.tileHeight + playerSprite.height / 2;
                this.yMove = 0;
                this.jumpsLeft = 1;
                if (!isGrounded)
                {
                    isGrounded = true;
                    spawnSparkleParticles(4, -Vector2.up * playerSprite.height / 2);
                    FSoundManager.PlaySound("HitGround");
                }
            }
        }
    }

    internal void endLevel()
    {
        this.currentState = State.ENDING_LEVEL;
        FSoundManager.PlaySound("Spawn");
        this.isVisible = false;
    }



    internal void clearVars()
    {
        this.cashCounter.reset();
        this.panacheCounter.reset();
    }

    internal void getCaught()
    {
        if (isMarking)
            CancelVanish();
        FSoundManager.PlaySound("Catch");
        currentState = Player.State.GETTING_CAUGHT;
        this.addCash(-cashCounter.actualValue / 2, true);
        this.isVisible = false;
    }

}

