using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class VanishCloud : FContainer
{
    FAnimatedSprite cloudSprite;
    float particleXSpeed = 30;
    float particleYSpeed = 30;
    int numParticles = 20;
    bool panache;
    bool hasAddedParticles = false;
    public VanishCloud(bool panache = true)
    {
        this.panache = panache;
        cloudSprite = new FAnimatedSprite("vanishCloud");
        cloudSprite.addAnimation(new FAnimation("disappear", new int[] { 1, 2, 3, 4 }, 100, false));
        this.AddChild(cloudSprite);

    }

    float particleDist = 15;
    public override void HandleAddedToContainer(FContainer container)
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToContainer(container);
        if (panache)
            World.getInstance().addVanishCloud(this);
        
    }

    private void Update()
    {
        if (!hasAddedParticles)
        {
            for (int x = 0; x < numParticles; x++)
            {
                Particle particle = Particle.VanishParticle.getParticle();
                float angle = (RXRandom.Float() * Mathf.PI * 2);
                Vector2 pos = this.GetPosition() + new Vector2(Mathf.Cos(angle) * particleDist, Mathf.Sin(angle) * particleDist);
                particle.activate(pos, new Vector2(RXRandom.Float() * particleXSpeed * 2 - particleXSpeed, RXRandom.Float() * particleYSpeed * 2 - particleYSpeed), Vector2.zero, 360);
                this.container.AddChild(particle);
            }
            hasAddedParticles = true;
        }
        if (cloudSprite.IsStopped)
        {
            this.RemoveFromContainer();
            World.getInstance().removeVanishCloud(this);
            Futile.instance.SignalUpdate -= Update;
        }
    }
}

