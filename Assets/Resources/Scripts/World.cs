using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class World : FContainer
{
    FTmxMap map;
    Player player;

    public FTilemap collision;
    public List<Trampoline> trampolineList = new List<Trampoline>();

    private FContainer bgLayer = new FContainer();
    private FContainer objectLayer = new FContainer();
    private FContainer playerLayer = new FContainer();
    private FContainer fgLayer = new FContainer();

    public World()
    {

    }

    public void LoadMap(string mapName)
    {
        trampolineList.Clear();
        player = new Player(this);
        map = new FTmxMap();
        map.clipNode = C.getCameraInstance();
        map.LoadTMX("Maps/" + mapName);
        C.getCameraInstance().setWorldBounds(new Rect(0, -map.height, map.width, map.height));
        C.getCameraInstance().setPlayer(player);
        foreach (XMLNode node in map.objects)
        {
            if (node.attributes.ContainsKey("name"))
            {
                switch (node.attributes["name"].ToLower())
                {
                    case "spawn":
                        player.x = float.Parse(node.attributes["x"]) + float.Parse(node.attributes["width"]) / 2;
                        player.y = -(float.Parse(node.attributes["y"])) + float.Parse(node.attributes["height"]) / 2;
                        break;
                    case "trampoline":
                        Trampoline trampoline = new Trampoline(new Vector2(float.Parse(node.attributes["x"]) + map.tileWidth / 2, - float.Parse(node.attributes["y"]) + map.tileHeight / 2));
                        trampolineList.Add(trampoline);
                        objectLayer.AddChild(trampoline);
                        break;
                }
            }
        }

        collision = (FTilemap)map.getLayerNamed("Collision");

        bgLayer.AddChild(map.getLayerNamed("Background"));
        fgLayer.AddChild(map.tilemaps[1]);//fgLayer.AddChild(map.getLayerNamed("Foreground"));

        playerLayer.AddChild(player);

        this.AddChild(bgLayer);
        this.AddChild(objectLayer);
        this.AddChild(playerLayer);
        this.AddChild(fgLayer);

    }

    public bool getMoveable(float xPos, float yPos)
    {
        int frameNum = collision.getFrameNumAt(xPos, yPos);
        return frameNum != 1 &&
               frameNum != -1;
    }
    public bool getOneWay(float xPos, float yPos)
    {
        int frameNum = collision.getFrameNumAt(xPos, yPos);
        return frameNum == 2;
    }
}
