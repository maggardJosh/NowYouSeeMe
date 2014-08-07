using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Chest : InteractableObject
{
    FAnimatedSprite chestSprite;

    public Chest(Vector2 pos) : base()
    {
        this.SetPosition(pos);
        chestSprite = new FAnimatedSprite("MagicChest/magicChest");
        chestSprite.addAnimation(new FAnimation("inactive", new int[] { 1 }, 100, false));
        chestSprite.addAnimation(new FAnimation("active", new int[] { 2 }, 100, false));
        chestSprite.addAnimation(new FAnimation("spawnPlayer", new int[] { 1,2,1,2,1,2,1 }, 100, false));
        this.AddChild(chestSprite);
    }

    public void deactivate()
    {
        interactable = true;
        chestSprite.play("inactive", true);
    }

    public void activate()
    {
        interactable = false;
        chestSprite.play("active", true);
    }

    public void spawnPlayer()
    {
        C.isSpawning = true;
        Futile.instance.SignalUpdate += Update;
        chestSprite.play("spawnPlayer", true);
    }

    private void Update()
    {
        if (chestSprite.IsStopped)
        {
            C.isSpawning = false;
            chestSprite.play("active", true);
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
        base.interact(p);
    }
}

