using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class GameScript : MonoBehaviour
{
    bool isStarted = false;
    ShadowLabel pressEnter;
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
        Futile.atlasManager.LoadFont(C.smallDarkFontName, "smallFont_1", "Atlases/smallFontDark", 0, 0);

        FSprite splashScreen = new FSprite("splashScreen");
        Futile.stage.AddChild(splashScreen);

        pressEnter = new ShadowLabel("PRESS ENTER");
        pressEnter.y = -Futile.screen.halfHeight + 25;
        pressEnter.isVisible = false;
        Futile.stage.AddChild(pressEnter);

        ShadowLabel versionNumber = new ShadowLabel(C.versionNumber);
        versionNumber.y = -Futile.screen.halfHeight + versionNumber.textRect.height/2 + 2;
        Futile.stage.AddChild(versionNumber);
    }

    float count = 0;
    // Update is called once per frame
    void Update()
    {
        if (!isStarted)
        {
            count += Time.deltaTime;
            pressEnter.isVisible = count > 1.0f && ((int)count) % 3 != 0;
            if (Input.GetKeyUp(C.ACTION_KEY))
            {
                Futile.stage.RemoveAllChildren();
                isStarted = true;
                World world = new World();
                world.LoadMap("testMap");
                Futile.stage.AddChild(world);

                C.getCameraInstance().MoveToFront();
            }
        }
    }

}
