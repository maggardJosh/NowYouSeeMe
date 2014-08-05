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

    public World()
    {

    }

    public void LoadMap(string mapName)
    {
        player = new Player(this);
        map = new FTmxMap();
        map.clipNode = C.getCameraInstance();
        map.LoadTMX("Maps/" + mapName);
        this.AddChild(map);
        C.getCameraInstance().setWorldBounds(new Rect(0, -map.height, map.width, map.height));

        foreach (XMLNode node in map.objects)
        {
            if (node.attributes.ContainsKey("name"))
            {
                switch (node.attributes["name"].ToLower())
                {
                    case "spawn":
                        player.x = float.Parse(node.attributes["x"]) + float.Parse(node.attributes["width"]) / 2;
                        player.y = -(float.Parse(node.attributes["y"]) + float.Parse(node.attributes["height"])) / 2;
                        break;
                }
            }
        }

        collision = (FTilemap)map.getLayerNamed("Collision");
        
        this.AddChild(player);
        
    }

    public bool getMoveable(float xPos, float yPos)
    {
        return collision.getFrameNumAt(xPos, yPos) != 1 &&
            collision.getFrameNumAt(xPos, yPos) != -1; 
    }
}
