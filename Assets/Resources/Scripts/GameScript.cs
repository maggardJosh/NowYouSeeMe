using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class GameScript : MonoBehaviour
{
    FAnimatedSprite playerAnim;
    void Start()
    {
        FutileParams futileParams = new FutileParams(true, false, false, false);

        futileParams.AddResolutionLevel(160.0f, 1.0f, 1.0f, ""); // Gameboy resolution 160x144px

        futileParams.origin = new Vector2(0.5f, 0.5f);
        futileParams.backgroundColor = new Color(0, 0, 0);
        futileParams.shouldLerpToNearestResolutionLevel = true;

        Futile.instance.Init(futileParams);

        Futile.atlasManager.LoadAtlas("Atlases/InGameAtlas");

        FSprite bg = new FSprite("testBG");
        Futile.stage.AddChild(bg);

        playerAnim = new FAnimatedSprite("player");
        playerAnim.addAnimation(new FAnimation("idle", new int[] { 0, 1 }, 2000, true));
        playerAnim.play("idle");
        Futile.stage.AddChild(playerAnim);
        startLoop();
        playerAnim.y = .5f;
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

    float speed = 10;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            playerAnim.x -= Time.deltaTime * speed;
        if (Input.GetKey(KeyCode.D))
            playerAnim.x += Time.deltaTime * speed;
    }

}
