using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Chest : InteractableObject
{
    bool isEnd = false;
    string nextMap = "";

    public Chest(Vector2 pos)
        : base()
    {
        X_INTERACT_DIST = 24;
        this.SetPosition(pos);
        interactSprite = new FAnimatedSprite("MagicChest/magicChest");
        interactSprite.addAnimation(new FAnimation("interactable", new int[] { 1 }, 100, false));
        interactSprite.addAnimation(new FAnimation("uninteractable", new int[] { 2 }, 100, false));
        interactSprite.addAnimation(new FAnimation("hover", new int[] { 1, 3 }, 100, true));
        interactSprite.addAnimation(new FAnimation("spawnPlayer", new int[] { 4, 5, 6, 7, 8, 9, 10, 11, 12 }, 100, false));
        interactSprite.addAnimation(new FAnimation("endLevel", new int[] { 13, 13, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27 }, 100, false));
        this.AddChild(interactSprite);
        this.interactType = InteractType.LOOT;
    }

    public void deactivate()
    {
        interactable = true;
        interactSprite.play("interactable");
        Futile.instance.SignalUpdate -= ActiveUpdate;
    }

    public void activate()
    {
        interactable = false;
        interactSprite.play("uninteractable");
        Futile.instance.SignalUpdate += ActiveUpdate;
    }

    private void ActiveUpdate()
    {
        if (RXRandom.Float() < .1)
            spawnSparkleParticles(1, 10);
    }
    public void spawnPlayer(Player p)
    {
        this.p = p;
        C.isSpawning = true;
        Futile.instance.SignalUpdate += Update;
        FSoundManager.PlaySound("Spawn");
        interactSprite.play("spawnPlayer", true);
    }

    private void Update()
    {
        if (interactSprite.IsStopped)
        {
            p.stopSpawning();
            C.isSpawning = false;
            interactSprite.play("uninteractable", true);
            Futile.instance.SignalUpdate -= Update;
        }
    }
    float particleXSpeed = 30;
    float particleYSpeed = 30;
    int numParticles = 20;

    Player p;
    float particleDist = 15;
    public override void interact(Player p)
    {
        this.p = p;
        if (!isEnd)
        {
            p.activateChest(this);
            for (int x = 0; x < numParticles; x++)
            {
                Particle particle = Particle.SparkleParticle.getParticle();
                float angle = (RXRandom.Float() * Mathf.PI * 2);
                Vector2 pos = this.GetPosition() + new Vector2(Mathf.Cos(angle) * particleDist, Mathf.Sin(angle) * particleDist);
                particle.activate(pos, new Vector2(RXRandom.Float() * particleXSpeed * 2 - particleXSpeed, RXRandom.Float() * particleYSpeed * 2 - particleYSpeed), Vector2.zero, 360);
                this.container.AddChild(particle);
            }
        }
        else
        {
            Futile.instance.SignalUpdate += EndUpdate;
            interactSprite.play("endLevel", true);
            interactable = false;
            p.endLevel();
        }
    }

    private float endCount = 0;
    private const float END_WAIT = 1.5f;
    private void EndUpdate()
    {
        if (interactSprite.IsStopped)
        {
            if (endCount == 0)
            {
                FSoundManager.PlaySound("EndLevel");
                spawnVanishParticles(40, new Vector2(0, -8), 10);
            }
            if (endCount > END_WAIT)
            {
                World.getInstance().nextLevel(nextMap);
                Futile.instance.SignalUpdate -= EndUpdate;
            }
            endCount += Time.deltaTime;
        }
    }

    internal void setEnd(string p)
    {
        nextMap = p;
        isEnd = true;
    }


    private void spawnVanishParticles(int numParticles, Vector2 disp, float particleDist = 0, bool spawnBehindPlayer = false)
    {
        float particleXSpeed = 30;
        float particleYSpeed = 20;
        for (int x2 = 0; x2 < numParticles; x2++)
        {
            Particle particle = Particle.VanishParticle.getParticle();
            Vector2 pos = this.GetPosition() + disp + new Vector2(RXRandom.Float() * particleDist * 2 - particleDist, 0);
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

    private void spawnSparkleParticles(int numParticles, float particleDist = 0, bool spawnBehindPlayer = false)
    {

        float particleXSpeed = 20;
        float particleYSpeed = 20;
        for (int x2 = 0; x2 < numParticles; x2++)
        {
            Particle particle = Particle.SparkleParticle.getParticle();
            float angle = (RXRandom.Float() * Mathf.PI * 2);
            Vector2 pos = this.GetPosition() + new Vector2(Mathf.Cos(angle) * particleDist, 3);
            particle.activate(pos, new Vector2(RXRandom.Float() * particleXSpeed * 2 - particleXSpeed, 10 + RXRandom.Float() * particleYSpeed), Vector2.zero, 360);
            particle.currentAnim.delay = 200;
            this.container.AddChild(particle);
            if (spawnBehindPlayer)
                particle.MoveToBack();
        }
    }

}

