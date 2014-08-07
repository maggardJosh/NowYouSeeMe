using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Chest : InteractableObject
{

    public Chest(Vector2 pos) : base()
    {
        X_INTERACT_DIST = 24;
        this.SetPosition(pos);
        interactSprite = new FAnimatedSprite("MagicChest/magicChest");
        interactSprite.addAnimation(new FAnimation("interactable", new int[] { 1 }, 100, false));
        interactSprite.addAnimation(new FAnimation("uninteractable", new int[] { 2 }, 100, false));
        interactSprite.addAnimation(new FAnimation("hover", new int[] { 2 }, 100, false));
        interactSprite.addAnimation(new FAnimation("spawnPlayer", new int[] { 1, 2, 1, 2, 1, 2, 1 }, 100, false));
        this.AddChild(interactSprite);
    }

    public void deactivate()
    {
        interactable = true;
    }

    public void activate()
    {
        interactable = false;
    }

    public void spawnPlayer()
    {
        C.isSpawning = true;
        Futile.instance.SignalUpdate += Update;
        interactSprite.play("spawnPlayer", true);
    }

    private void Update()
    {
        if (interactSprite.IsStopped)
        {
            C.isSpawning = false;
            interactSprite.play("uninteractable", true);
            Futile.instance.SignalUpdate -= Update;
        }
    }
    float particleXSpeed = 30;
    float particleYSpeed = 30;
    int numParticles = 20;

    float particleDist = 15;
    public override void interact(Player p)
    {
        p.activateChest(this);
        for (int x = 0; x < numParticles; x++)
        {
            VanishParticle particle = VanishParticle.getParticle();
            float angle = (RXRandom.Float() * Mathf.PI * 2);
            Vector2 pos = this.GetPosition() + new Vector2(Mathf.Cos(angle) * particleDist, Mathf.Sin(angle) * particleDist);
            particle.activate(pos, new Vector2(RXRandom.Float() * particleXSpeed * 2 - particleXSpeed, RXRandom.Float() * particleYSpeed * 2 - particleYSpeed), Vector2.zero, 360);
            this.container.AddChild(particle);
        }
    }
}

