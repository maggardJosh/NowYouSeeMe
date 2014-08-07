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
    private enum State
    {
        IDLE,
        MARKING,
        VANISHING,
        COOLDOWN,
        SPAWNING
    }
    FAnimatedSprite playerSprite;
    private State currentState = State.IDLE;
    private State lastState = State.IDLE;
    Hat hat;
    World world;
    int jumpsLeft = 0;
    float xMove = 0;
    float yMove = 0;
    float stateCount = 0;
    bool isFacingLeft = false;

    public GUICounter cashCounter;
    public GUICounter panacheCounter;

    int animSpeed = 200;
    public Player(World world)
    {
        playerSprite = new FAnimatedSprite("player");
        playerSprite.addAnimation(new FAnimation("hat_idle", new int[] { 13 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hat_walk", new int[] { 1, 2, 3, 4 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hat_air_up", new int[] { 5 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hat_air_down", new int[] { 6 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hatless_idle", new int[] { 14 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hatless_walk", new int[] { 7, 8, 9, 10 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hatless_air_up", new int[] { 11 }, animSpeed, true));
        playerSprite.addAnimation(new FAnimation("hatless_air_down", new int[] { 12 }, animSpeed, true));

        playerSprite.play("hat_idle");
        this.AddChild(playerSprite);
        this.world = world;
        C.getCameraInstance().follow(this);
        hat = new Hat(this);
        cashCounter = new GUICounter();

        panacheCounter = new GUICounter();

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


    #region Mark/Vanish Stuff
    private void Mark()
    {
        this.container.AddChild(hat);
        hat.appear();
        hat.SetPosition(this.GetPosition());
        hasLeftMarkPos = false;

    }
    const float VANISH_DURATION = .5f;
    const float HAT_RETURN_COUNT = 1.0f;
    private const float MARK_MAX_COUNT = 2.0f;

    private void Vanish()
    {
        VanishCloud cloud = new VanishCloud();
        cloud.SetPosition(this.GetPosition());
        this.container.AddChild(cloud);

        VanishCloud newPosCloud = new VanishCloud();
        newPosCloud.SetPosition(hat.GetPosition());
        playerSprite.isVisible = false;

        currentState = State.VANISHING;
        Go.to(this, VANISH_DURATION, new TweenConfig().floatProp("x", hat.x).floatProp("y", hat.y).setEaseType(EaseType.CircInOut).onComplete((a) => { currentState = State.COOLDOWN; hat.disappear(); playerSprite.isVisible = true; this.container.AddChild(newPosCloud); }));
    }

    private void MarkTimeOut()
    {
        VanishCloud cloud = new VanishCloud();
        cloud.SetPosition(this.GetPosition());
        this.container.AddChild(cloud);

        VanishCloud newPosCloud = new VanishCloud();
        newPosCloud.SetPosition(hat.GetPosition());
        this.container.AddChild(newPosCloud);

        hat.disappear();
        currentState = State.COOLDOWN;
    }

    private void CancelVanish()
    {
        VanishCloud cloud = new VanishCloud();
        cloud.SetPosition(this.GetPosition());
        this.container.AddChild(cloud);

        VanishCloud newPosCloud = new VanishCloud();
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
        VanishCloud cloud = new VanishCloud();
        cloud.SetPosition(this.GetPosition());
        this.container.AddChild(cloud);
        this.isVisible = false;

        currentState = State.VANISHING;
        Go.to(this, VANISH_DURATION * 2, new TweenConfig().floatProp("x", activatedChest.x).floatProp("y", activatedChest.y + collisionHeight/3).setEaseType(EaseType.CircInOut).onComplete((a) =>
        {
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
    }
    public void clearInteractable()
    {
        currentInteractable = null;
    }

    public void addCash(int amount)
    {
        cashCounter.addAmount(amount);
        LabelIndicator cashInd = new LabelIndicator("+$" + amount);
        cashInd.SetPosition(this.GetPosition() + Vector2.up * 10);
        this.container.AddChild(cashInd);
    }

    public void addPanache(int amount)
    {
        panacheCounter.addAmount(amount);
        LabelIndicator panacheInd = new LabelIndicator("+" + amount);
        panacheInd.SetPosition(this.GetPosition() + Vector2.up * 10);
        this.container.AddChild(panacheInd);

    }
    private void ControlUpdate()
    {
        if (currentState != lastState)
            stateCount = 0;

        lastState = currentState;

        if (Input.GetKeyDown(KeyCode.T))
            respawn();
        if (Input.GetKeyDown(KeyCode.C))
            addCash(100);
        if (Input.GetKeyDown(KeyCode.P))
            addPanache(100);
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
                return;     //Don't allow controls past this if vanishing
            case State.COOLDOWN:
                if (stateCount > HAT_RETURN_COUNT)
                    currentState = State.IDLE;
                break;
        }
        if (Input.GetKeyDown(C.UP_KEY) && currentInteractable != null)
        {
            currentInteractable.interact(this);
            currentInteractable = null;
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
        stateCount += Time.deltaTime;
    }


    float downJumpCount = 0;
    const float DOWN_JUMP_TIME = .4f;       //Time to allow a down jump

    float speed = .1f;
    float airSpeed = .1f;
    float friction = .7f;
    float airFriction = .99f;
    float jumpStrength = 6;
    const float MAX_Y_VEL = 10f;
    const float MIN_Y_VEL = -6f;
    const float MAX_X_VEL = 5f;
    bool isGrounded = true;
    bool isMoving = false;
    bool hasLeftMarkPos = false;
    const float MARK_MIN_DIST = 20;     //Distance from mark that causes the timer to start
    const float MIN_MOVEMENT_X = .1f;
    public const float Gravity = -.3f;

    private void Update()
    {
        float xAcc = 0;
        if (currentState == State.VANISHING)
            return;
        if (currentState == State.SPAWNING)
        {
            if (!C.isSpawning)
            {
                currentState = State.IDLE;
                this.isVisible = true;
            }
            return;
        }
        if (Input.GetKey(C.LEFT_KEY))
        {
            xAcc = isGrounded ? -speed : -airSpeed;
            if (xMove > 0)
                xAcc *= 4;
            isFacingLeft = true;
        }
        if (Input.GetKey(C.RIGHT_KEY))
        {
            xAcc = isGrounded ? speed : airSpeed;
            if (xMove < 0)
                xAcc *= 4;
            isFacingLeft = false;
        }

        xMove += xAcc;

        xMove = Mathf.Clamp(xMove, -MAX_X_VEL, MAX_X_VEL);

        if (xMove > 0)
            tryMoveRight(xMove);
        else if (xMove < 0)
            tryMoveLeft(xMove);

        isGrounded = false;
        yMove += Gravity;

        yMove = Mathf.Clamp(yMove, MIN_Y_VEL, MAX_Y_VEL);

        if (yMove > 0)
            tryMoveUp(yMove);
        else if (yMove < 0)
            tryMoveDown(yMove);

        if (yMove > 0)
            isGrounded = false;

        if (xAcc == 0)
            if (isGrounded)
                xMove *= friction;
            else
                xMove *= airFriction;

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
                animToPlay += "walk";
            else
                animToPlay += "idle";
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

    float collisionWidth = 12;
    float collisionHeight = 20;
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
        {
            this.y = Mathf.FloorToInt((this.y + yMove) / world.collision.tileHeight) * world.collision.tileHeight + (world.collision.tileHeight - collisionHeight / 2);
            this.yMove *= .5f;
        }
    }

    private void tryMoveDown(float yMove)
    {
        bool onOneWay = world.getOneWay(x, y - playerSprite.height / 2 + yMove);
        if (world.getMoveable(x - collisionWidth * .9f / 2, y - playerSprite.height / 2 + yMove) &&
            world.getMoveable(x + collisionWidth * .9f / 2, y - playerSprite.height / 2 + yMove) && !onOneWay)
        {
            this.y += yMove;
        }
        else
        {
            if (onOneWay && downJumpCount > 0)
                this.y += yMove;
            else
            {
                this.y = Mathf.CeilToInt((this.y - playerSprite.height / 2 + yMove) / world.collision.tileHeight) * world.collision.tileHeight + playerSprite.height / 2;
                this.yMove = 0;
                this.jumpsLeft = 1;
                isGrounded = true;
            }
        }
    }
}

