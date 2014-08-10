﻿using System;
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
        interactSprite.addAnimation(new FAnimation("endLevel", new int[] { 12, 11, 10, 9, 8, 7, 6, 5, 4 }, 100, false));
        this.AddChild(interactSprite);
        this.interactType = InteractType.LOOT;
    }

    public void deactivate()
    {
        interactable = true;
        interactSprite.play("interactable");
    }

    public void activate()
    {
        interactable = false;
        interactSprite.play("uninteractable");
    }

    public void spawnPlayer(Player p)
    {
        this.p = p;
        C.isSpawning = true;
        Futile.instance.SignalUpdate += Update;
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

    private void EndUpdate()
    {
        if (interactSprite.IsStopped)
        {
            World.getInstance().nextLevel(nextMap);
            Futile.instance.SignalUpdate -= EndUpdate;
        }
    }

    internal void setEnd(string p)
    {
        nextMap = p;
        isEnd = true;
    }
}

