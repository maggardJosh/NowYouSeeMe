using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Hat : FContainer
{
    FAnimatedSprite hatSprite;
    const int NUM_IND = 10;
    FAnimatedSprite[] velocityIndicators = new FAnimatedSprite[NUM_IND];
    Player owner;
    public Hat(Player p)
    {
        hatSprite = new FAnimatedSprite("hat");
        hatSprite.addAnimation(new FAnimation("mark1", new int[] { 1 }, 300, true));
        hatSprite.addAnimation(new FAnimation("mark2", new int[] { 2 }, 300, true));
        hatSprite.addAnimation(new FAnimation("mark3", new int[] { 3 }, 300, true));
        hatSprite.addAnimation(new FAnimation("mark4", new int[] { 4 }, 300, true));
        hatSprite.play("mark1");

        for (int i = 0; i < NUM_IND; i++)
        {
            velocityIndicators[i] = new FAnimatedSprite("velInd");
            velocityIndicators[i].addAnimation(new FAnimation("active", new int[] { 1, 2, 3 }, 100, true));
            velocityIndicators[i].play("active");
            this.AddChild(velocityIndicators[i]);

        }

        this.AddChild(hatSprite);
        this.owner = p;
    }

    public override void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToStage();
    }

    public void appear()
    {
        hatSprite.play("mark" + (RXRandom.Int(4) + 1));
        this.isVisible = true;
        Go.killAllTweensWithTarget(this);
        hatSprite.scale = .1f;
        Go.to(hatSprite, .2f, new TweenConfig().floatProp("scale", 1.0f));
    }

    Vector2 startDisappearPos = Vector2.zero;
    public void disappear()
    {
        this.isVisible = false;
    }
    float lastXMove = 0;
    float lastYMove = 0;
    private void Update()
    {
        hatSprite.scale = Mathf.CeilToInt(hatSprite.scale * 6) / 6.0f;

        if (owner != null)
        {
            hatSprite.rotation = Mathf.LerpAngle(hatSprite.rotation, owner.getVelocityAngle(), 1.0f);

            float xMove = Mathf.Lerp(lastXMove, owner.xMove, .4f);
            float yMove = Mathf.Lerp(lastYMove, owner.yMove, .4f);
            lastXMove = xMove;
            lastYMove = yMove;
            for (int i = 0; i < NUM_IND; i++)
            {
                velocityIndicators[i].isVisible = true;
                int num_steps_per_ind = 4;
                if (i == 0)
                    velocityIndicators[i].SetPosition(Vector2.zero);
                else
                    velocityIndicators[i].SetPosition(velocityIndicators[i - 1].GetPosition());

                for (int j = 0; j < num_steps_per_ind; j++)
                {
                    velocityIndicators[i].x += xMove;
                    velocityIndicators[i].y += yMove;
                    yMove += Player.Gravity;
                    if (yMove < 0 && !World.getInstance().getMoveable(this.x + velocityIndicators[i].x, this.y + velocityIndicators[i].y - 20))
                        yMove = 0;
                    if (yMove > 0 && !World.getInstance().getMoveable(this.x + velocityIndicators[i].x, this.y + velocityIndicators[i].y + 12))
                        yMove = 0;
                    if (xMove > 0 && !World.getInstance().getMoveable(this.x + velocityIndicators[i].x + 12, this.y + velocityIndicators[i].y))
                        xMove = 0;
                    if (xMove < 0 && !World.getInstance().getMoveable(this.x + velocityIndicators[i].x - 12, this.y + velocityIndicators[i].y))
                        xMove = 0;

                }
            }
        }




    }
}

