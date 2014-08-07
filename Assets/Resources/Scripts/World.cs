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
    public List<Door> doorList = new List<Door>();
    public List<InteractableObject> interactObjectList = new List<InteractableObject>();

    private FContainer bgLayer = new FContainer();
    private FContainer objectLayer = new FContainer();
    private FContainer playerLayer = new FContainer();
    private FContainer fgLayer = new FContainer();

    public World()
    {
        Futile.instance.SignalUpdate += Update;
    }

    public void LoadMap(string mapName)
    {
        trampolineList.Clear();
        interactObjectList.Clear();
        doorList.Clear();
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
                    case "trampoline":
                        parseTrampoline(node);
                        break;
                    case "chest":
                        parseChest(node);
                        break;
                    case "container":
                        parseContainer(node);
                        break;
                    case "door":
                        parseDoor(node);
                        break;
                }
            }
        }

        collision = (FTilemap)map.getLayerNamed("Collision");

        bgLayer.AddChild(map.getLayerNamed("Background"));
        fgLayer.AddChild(map.tilemaps[1]);

        playerLayer.AddChild(player);

        this.AddChild(bgLayer);
        this.AddChild(objectLayer);
        this.AddChild(playerLayer);
        this.AddChild(fgLayer);
        player.spawn();

    }

    private void parseTrampoline(XMLNode node)
    {
        Trampoline trampoline = new Trampoline(new Vector2(float.Parse(node.attributes["x"]) + map.tileWidth / 2, -float.Parse(node.attributes["y"]) + map.tileHeight / 2));
        trampolineList.Add(trampoline);
        objectLayer.AddChild(trampoline);
    }

    Chest spawnChest;
    private void parseChest(XMLNode node)
    {
        Chest chest = new Chest(new Vector2(float.Parse(node.attributes["x"]) + map.tileWidth / 2, -float.Parse(node.attributes["y"]) + map.tileHeight / 2));
        if (node.children.Count > 0)
            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                if (property.attributes.ContainsKey("name"))
                    switch (property.attributes["name"].ToLower())
                    {
                        case "spawn":
                            if (bool.Parse(property.attributes["value"]))
                            {
                                player.x = chest.x;
                                player.y = chest.y;
                                player.activateChest(chest);
                                spawnChest = chest;
                            }
                            break;
                    }
            }
        interactObjectList.Add(chest);
        objectLayer.AddChild(chest);
    }

    private void parseContainer(XMLNode node)
    {
        int type = 1;
        int money = 0;
        if (node.children.Count > 0)
            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                if (property.attributes.ContainsKey("name"))
                    switch (property.attributes["name"].ToLower())
                    {
                        case "type":
                            if (!int.TryParse(property.attributes["value"], out type))
                                RXDebug.Log("Unknown Container Type: " + property.attributes["value"]);
                            break;
                        case "money":
                            if (!int.TryParse(property.attributes["value"], out money))
                                RXDebug.Log("Unknown Money Amount: " + property.attributes["value"]);
                            break;
                    }
            }

        Container container = new Container(new Vector2(float.Parse(node.attributes["x"]) + map.tileWidth / 2, -float.Parse(node.attributes["y"]) + map.tileHeight / 2), type, money);
        interactObjectList.Add(container);
        objectLayer.AddChild(container);
    }

    private void parseDoor(XMLNode node)
    {
        Door door = new Door(new Vector2(float.Parse(node.attributes["x"]) + map.tileWidth / 2, -float.Parse(node.attributes["y"]) + map.tileHeight / 2));
        doorList.Add(door);
        interactObjectList.Add(door);
        objectLayer.AddChild(door);
    }

    public bool getMoveable(float xPos, float yPos)
    {
        
        int frameNum = collision.getFrameNumAt(xPos, yPos);
        return frameNum != 1 &&
               frameNum != -1;
    }

    public Door checkDoor(float oldXPos, float xPos, float yPos)
    {
        foreach (Door d in doorList)
        {
            if (d.Open)
                continue;
            if (((d.x + Door.DOOR_COLLISION_WIDTH /2 > oldXPos && d.x - Door.DOOR_COLLISION_WIDTH< xPos) ||
                (d.x - Door.DOOR_COLLISION_WIDTH / 2 < oldXPos && d.x + Door.DOOR_COLLISION_WIDTH> xPos)) &&
                d.y - 16 <= yPos &&
                d.y + 16 >= yPos)
                return d;
        }
        return null;
    }
    public bool getOneWay(float xPos, float yPos)
    {
        int frameNum = collision.getFrameNumAt(xPos, yPos);
        return frameNum == 2;
    }

    private void Update()
    {
        checkInteractObjects();
    }

    private void checkInteractObjects()
    {
        bool foundObject = false;
        for (int x = 0; x < interactObjectList.Count; x++)
            foundObject = interactObjectList[x].checkInteractDist(player) || foundObject;
        //If we made it this far then 
        if(!foundObject)
            player.clearInteractable();
    }
}
