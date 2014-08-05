using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class VanishCloud : FContainer
{
    FAnimatedSprite cloudSprite;
    public VanishCloud()
    {
        cloudSprite = new FAnimatedSprite("vanishCloud");
        cloudSprite.addAnimation(new FAnimation("disappear", new int[] { 0, 1, 2, 3 }, 200, false));
        this.AddChild(cloudSprite);
    }

    public override void HandleAddedToContainer(FContainer container)
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToContainer(container);
    }

    private void Update()
    {
        if (cloudSprite.IsStopped)
        {
            this.RemoveFromContainer();
            Futile.instance.SignalUpdate -= Update;
        }
    }
}

