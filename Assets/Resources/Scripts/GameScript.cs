using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class GameScript : MonoBehaviour
{
  
    void Start()
    {
        FutileParams futileParams = new FutileParams(true, false, false, false);

        futileParams.AddResolutionLevel(160.0f, 1.0f, 1.0f, ""); // Gameboy resolution 160x144px

        futileParams.origin = new Vector2(0.5f, 0.5f);
        futileParams.backgroundColor = new Color(0, 0, 0);
        futileParams.shouldLerpToNearestResolutionLevel = true;

        Futile.instance.Init(futileParams);

        Futile.atlasManager.LoadAtlas("Atlases/InGameAtlas");

        FAnimatedSprite playerAnim = new FAnimatedSprite("player");
        playerAnim.addAnimation(new FAnimation("idle", new int[] { 0, 1 }, 100, true));
        playerAnim.play("idle");
        Futile.stage.AddChild(playerAnim);
      
    }

    // Update is called once per frame
    void Update()
    {
     
    }

}
