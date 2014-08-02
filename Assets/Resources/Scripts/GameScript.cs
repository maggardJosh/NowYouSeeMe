using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class GameScript : MonoBehaviour
{
    Player player;
    void Start()
    {
        FutileParams futileParams = new FutileParams(true, false, false, false);

        futileParams.AddResolutionLevel(160.0f, 1.0f, 1.0f, ""); // Gameboy resolution 160x144px

        futileParams.origin = new Vector2(0.5f, 0.5f);
        futileParams.backgroundColor = new Color(0, 0, 0);
        futileParams.shouldLerpToNearestResolutionLevel = true;

        Futile.instance.Init(futileParams);

        Futile.atlasManager.LoadAtlas("Atlases/InGameAtlas");
        FTmxMap map = new FTmxMap();
        
        map.clipNode = C.getCameraInstance();
        map.LoadTMX("Maps/testMap");
        Futile.stage.AddChild(map);

        FCamObject camera = C.getCameraInstance();
        player = new Player();

        camera.setWorldBounds(new Rect(0, -map.height, map.width, map.height));
        camera.follow(player);
        camera.MoveToFront();

        Futile.stage.AddChild(player);
        
        foreach (XMLNode node in map.objects)
        {
            if (node.attributes.ContainsKey("name"))
            {
                switch(node.attributes["name"].ToLower())
                {
                    case "spawn":
                        player.x = float.Parse(node.attributes["x"]) + float.Parse(node.attributes["width"]) / 2;
                        player.y = -(float.Parse(node.attributes["y"]) + float.Parse(node.attributes["height"])) / 2;
                        break;
                }
            }
        }

        startLoop();
    }

    private void startLoop()
    {
        Go.killAllTweensWithTarget(FShader.OverlayBlend);
        FShader.OverlayBlend.overlayColor = new Color(.2f, .7f, .4f);
        Go.to(FShader.OverlayBlend, 2.0f, new TweenConfig().colorProp("overlayColor", new Color(.2f, .4f, .7f)).setEaseType(EaseType.QuadIn).onComplete((AbstractTween a) =>
        {
            Go.to(FShader.OverlayBlend, 2.0f, new TweenConfig().colorProp("overlayColor", new Color(.7f, .4f, .2f)).setEaseType(EaseType.QuadIn).onComplete((AbstractTween b) =>
            {
                Go.to(FShader.OverlayBlend, 2.0f, new TweenConfig().colorProp("overlayColor", new Color(.2f, .7f, .4f)).setEaseType(EaseType.QuadIn).onComplete((AbstractTween c) =>
                {
                    startLoop();
                }));
            }));
        }));
    }

    // Update is called once per frame
    void Update()
    {
      
    }

}
