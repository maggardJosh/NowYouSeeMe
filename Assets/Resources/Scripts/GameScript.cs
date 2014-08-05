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

        Futile.atlasManager.LoadFont(C.smallFontName, "smallFont_0", "Atlases/smallFont", 0, 0);
        World world = new World();
        world.LoadMap("testMap");
        Futile.stage.AddChild(world);
        FShader.OverlayBlend.overlayColor = new Color(.4f, .8f, .2f);

        
    }

    // Update is called once per frame
    void Update()
    {
      
    }

}
