using System;
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
        MARKING,
        VANISHING,
        COOLDOWN,
        SPAWNING,
        ENDING_LEVEL,
        GETTING_CAUGHT
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
    bool isLooting = false;
    bool isInteracting = false;

    public GUICounter cashCounter;
    public GUICounter panacheCounter;

    const float INDICATOR_Y = 20;
    const float INDICATOR_BOUNCE = 5;

    int animSpeed = 200;
    string nextLevel = "";
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
        playerSprite.addAnimation(new FAnimation("hatless_idle", new int[] { 14 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hatless_walk", new int[] { 7, 8, 9, 10 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hatless_run", new int[] { 7, 8, 9, 10 }, animSpeed / 2, true));
        playerSprite.addAnimation(new FAnimation("hatless_air_up", new int[] { 11 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hatless_air_down", new int[] { 12 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hatless_loot", new int[] { 16 }, 500, false));
        playerSprite.addAnimation(new FAnimation("hatless_interact", new int[] { 8 }, 250, true));

        playerSprite.addAnimation(new FAnimation("endLevel", new int[] { 5, 6, 5, 6 }, animSpeed, false));


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
        switch (currentState)
        {
            case State.MARKING: return hasLeftMarkPos ? 1 - stateCount / MARK_MAX_COUNT : 1;
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
        if (isLooting)
            isLooting = false;
        VanishCloud cloud = new VanishCloud();
        cloud.SetPosition(this.GetPosition());
        this.container.AddChild(cloud);

        VanishCloud newPosCloud = new VanishCloud(false);
        newPosCloud.SetPosition(hat.GetPosition());
        playerSprite.isVisible = false;

        currentState = State.VANISHING;
        Go.to(this, VANISH_DURATION, new TweenConfig().floatProp("x", hat.x).floatProp("y", hat.y).setEaseType(EaseType.CircInOut).onComplete((a) => { currentState = State.COOLDOWN; hat.disappear(); playerSprite.isVisible = true; this.container.AddChild(newPosCloud); }));
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
        currentState = State.COOLDOWN;
    }

    private void CancelVanish()
    {
        VanishCloud cloud = new VanishCloud(false);
        cloud.SetPosition(this.GetPosition());
        this.container.AddChild(cloud);

        VanishCloud newPosCloud = new VanishCloud(false);
        newPosCloud.SetPosition(hat.GetPosition());
        this.container.AddChild(newPosCloud);

        hat.disappear();
        currentState = State.IDLE;
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
        if (currentState == State.MARKING)
            CancelVanish();

        yMove = 0;
        xMove = 0;
        isGrounded = true;
        VanishCloud cloud = new VanishCloud(false);
        cloud.SetPosition(this.GetPosition());
        this.container.AddChild(cloud);
        this.isVisible = false;

        currentState = State.VANISHING;
        Go.to(this, VANISH_DURATION * 2, new TweenConfig().floatProp("x", activatedChest.x).floatProp("y", activatedChest.y + collisionHeight / 3).setEaseType(EaseType.CircInOut).onComplete((a) =>
        {
            this.addCash(-cashCounter.actualValue / 2);
            tryMoveDown(-.1f); currentState = State.SPAWNING; activatedChest.spawnPlayer();
        }));
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
            activatedChest.spawnPlayer();
        }
        else
            throw new Exception("No Activated Chest");
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

    public void addCash(int amount)
    {
        if (amount == 0)
            return;
        cashCounter.addAmount(amount);
        LabelIndicator cashInd = new LabelIndicator( (amount > 0 ? "+" : "-" ) + "$" + Math.Abs(amount),"moneyInd", amount < 0);
        cashInd.SetPosition(this.GetPosition() + Vector2.up * 10);
        this.container.AddChild(cashInd);
    }

    public void addPanache(int amount)
    {
        if (amount == 0)
            return;
        panacheCounter.addAmount(amount);
        LabelIndicator panacheInd = new LabelIndicator((amount > 0 ? "+" : "-") + Math.Abs(amount),"panacheInd", amount < 0);
        panacheInd.SetPosition(this.GetPosition() + Vector2.up * 10);
        this.container.AddChild(panacheInd);

    }
    float interactCount = 0;
    private void ControlUpdate()
    {
        if (currentState != lastState)
            stateCount = 0;

        lastState = currentState;

        switch (currentState)
        {
            case State.IDLE:
                if (Input.GetKey(C.ACTION_KEY))
                {
                    currentState = State.MARKING;
                    Mark();
                }
                break;
            case State.MARKING:
                if (!hasLeftMarkPos)
                    stateCount = 0;
                if (stateCount >= MARK_MAX_COUNT)
                    MarkTimeOut();
                else
                    if (!Input.GetKey(C.ACTION_KEY))
                    {
                        if (hasLeftMarkPos)
                            Vanish();
                        else
                            CancelVanish();
                    }
                break;
            case State.VANISHING:
            case State.SPAWNING:
                interactInd.isVisible = false;
                isInteracting = false;
                isLooting = false;
                return;     //Don't allow controls past this if vanishing
            case State.COOLDOWN:
                if (stateCount > HAT_RETURN_COUNT)
                    currentState = State.IDLE;
                break;
            case State.GETTING_CAUGHT:
                interactInd.isVisible = false;
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
        if (isInteracting || isLooting)
        {
            interactInd.isVisible = false;

        }
        stateCount += Time.deltaTime;

        if (isLooting)
            return;
        if (Input.GetKeyDown(KeyCode.T))
            respawn();
        if (Input.GetKeyDown(C.UP_KEY) && currentInteractable != null)
        {
            switch (currentInteractable.interactType)
            {
                case InteractableObject.InteractType.LOOT:
                    this.x = currentInteractable.x;
                    isLooting = true;
                    this.playerSprite.play((currentState == State.MARKING ? "hatless" : "hat") + "_loot", true);
                    xMove = 0;
                    yMove = 0;

                    break;
                case InteractableObject.InteractType.INTERACT:

                    this.playerSprite.play("interact", true);
                    isInteracting = true;
                    currentInteractable.interact(this);
                    spawnFootParticles(2, new Vector2(isFacingLeft ? -15 : 15, 5));
                    break;
            }
            return;
        }
        if (downJumpCount > 0)
            downJumpCount = Math.Max(0, downJumpCount - Time.deltaTime);
        if (Input.GetKeyDown(C.JUMP_KEY) && jumpsLeft > 0)
        {
            if (Input.GetKey(C.DOWN_KEY))
                downJumpCount = DOWN_JUMP_TIME;
            yMove = jumpStrength * (Input.GetKey(C.DOWN_KEY) ? .4f : 1);
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
    bool hasLeftMarkPos = false;
    const float MARK_MIN_DIST = 20;     //Distance from mark that causes the timer to start
    const float MIN_MOVEMENT_X = .1f;
    public const float Gravity = -.3f;
    bool hitMaxXVel = false;
    float maxVelTime = 0;
    bool isSprinting = false;
    bool wasSprinting = false;
    bool wasMaxSpeed = false;
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
                if (!C.isSpawning)
                {
                    currentState = State.IDLE;
                    this.isVisible = true;
                }
                return;
            case State.ENDING_LEVEL:
                if (playerSprite.IsStopped && !String.IsNullOrEmpty(nextLevel))
                {
                    world.nextLevel(nextLevel);
                    nextLevel = "";
                }
                return;
            case State.GETTING_CAUGHT:
                //After the enemy's anim gets done playing we respawn
                return;
        }
        if (isLooting)
        {
            if (RXRandom.Float() < .7f)
                spawnFootParticles(1, new Vector2(0, collisionHeight / 5), 8, true);
            if (playerSprite.IsStopped)
            {
                currentInteractable.interact(this);
                currentInteractable = null;
                isLooting = false;
            }
            return;
        }

        if (Input.GetKey(C.LEFT_KEY))
        {
            xAcc = isGrounded ? -speed : -airSpeed;
            if (xMove > 0)
            {
                xAcc *= 4;
                if (xMove > MAX_X_VEL / 2 && isGrounded)
                {
                    spawnFootParticles(1, Vector2.right * 10);
                }
            }
            if (xMove > -MAX_X_VEL / 2)
            {
                wasSprinting = false;
                wasMaxSpeed = false;
            }
            isFacingLeft = true;
        }
        if (Input.GetKey(C.RIGHT_KEY))
        {
            xAcc = isGrounded ? speed : airSpeed;
            if (xMove < 0)
            {
                xAcc *= 4;
                if (xMove < -MAX_X_VEL / 2 && isGrounded)
                {
                    spawnFootParticles(1, Vector2.right * -10);
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
                    spawnFootParticles(1);
            }
            if ((xMove > 0 && Input.GetKey(C.RIGHT_KEY)) ||
               (xMove < 0 && Input.GetKey(C.LEFT_KEY)))
            {
                if (maxVelTime > 1.0f)
                {
                    if (RXRandom.Float() < .8f)
                        spawnFootParticles(1);
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
            isGrounded = false;

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
                        spawnFootParticles(1, Vector2.right * 10);
                    else if (xMove < 0)
                        spawnFootParticles(1, Vector2.right * -10);

            }
            else
                xMove *= airFriction;
        }

        if (Math.Abs(xMove) < MIN_MOVEMENT_X)
            xMove = 0;
        isMoving = xAcc != 0;
        string animToPlay = "";

        switch (currentState)
        {
            case State.MARKING:
                animToPlay += "hatless_";
                break;
            default:
                animToPlay += "hat_";
                break;
        }
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

        if (currentState == State.MARKING && !hasLeftMarkPos)
            if ((hat.GetPosition() - this.GetPosition()).sqrMagnitude > MARK_MIN_DIST * MARK_MIN_DIST)
                hasLeftMarkPos = true;

        updateWorld();
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

    private void spawnFootParticles(int numParticles, Vector2 disp, float particleDist = 2, bool spawnBehindPlayer = false)
    {
        if (!isGrounded)
            return;
        float particleXSpeed = 20;
        float particleYSpeed = 20;

        for (int x2 = 0; x2 < numParticles; x2++)
        {
            VanishParticle particle = VanishParticle.getParticle();
            float angle = (RXRandom.Float() * Mathf.PI * 2);
            Vector2 pos = this.GetPosition() + disp - Vector2.up * collisionHeight / 2 + new Vector2(Mathf.Cos(angle) * particleDist, Mathf.Sin(angle) * particleDist);
            particle.activate(pos, new Vector2(RXRandom.Float() * particleXSpeed * 2 - particleXSpeed, RXRandom.Float() * particleYSpeed * 2 - particleYSpeed), Vector2.zero, 360);
            this.container.AddChild(particle);
            if (spawnBehindPlayer)
                particle.MoveToBack();
        }
    }
    private void spawnFootParticles(int numParticles)
    {
        spawnFootParticles(numParticles, Vector2.zero);
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
                    spawnFootParticles(4);
                }
            }
        }
    }

    internal void endLevel(string nextMap)
    {
        this.nextLevel = nextMap;
        this.currentState = State.ENDING_LEVEL;
        this.playerSprite.play("endLevel", true);
    }

    internal void clearVars()
    {
        this.cashCounter.reset();
        this.panacheCounter.reset();
    }

    internal void getCaught()
    {
        if (currentState == State.MARKING)
            CancelVanish();
        currentState = Player.State.GETTING_CAUGHT;
        this.addPanache(-panacheCounter.actualValue);
        this.isVisible = false;
    }
}

