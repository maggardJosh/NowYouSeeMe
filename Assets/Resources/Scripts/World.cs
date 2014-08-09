using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class World : FContainer
{
    FTmxMap map;
    Player player;

    private static World instance;
    public static World getInstance()
    {
        if (instance == null)
            instance = new World();
        return instance;
    }
    public FTilemap collision;
    public List<Trampoline> trampolineList = new List<Trampoline>();
    public List<Door> doorList = new List<Door>();
    public List<PressurePlate> plateList = new List<PressurePlate>();
    public List<InteractableObject> interactObjectList = new List<InteractableObject>();
    public List<Enemy> enemyList = new List<Enemy>();
    public List<VanishCloud> panacheEnabledClouds = new List<VanishCloud>();

    private FContainer bgLayer = new FContainer();
    private FContainer objectLayer = new FContainer();
    private FContainer playerLayer = new FContainer();
    private FContainer fgLayer = new FContainer();

    private const int TRAMPOLINE_IND = 0;
    private const int CHEST_IND = 1;
    private const int CONTAINER_IND = 2;
    private const int DOOR_IND = 3;
    private const int SWITCH_IND = 4;
    private const int PRESSURE_PLATE_IND = 5;
    private const int ENEMY_IND = 6;

    private World()
    {
        Futile.instance.SignalUpdate += Update;
    }

    public void LoadMap(string mapName)
    {
        trampolineList.Clear();
        interactObjectList.Clear();
        doorList.Clear();
        plateList.Clear();
        enemyList.Clear();
        bgLayer.RemoveAllChildren();
        fgLayer.RemoveAllChildren();
        playerLayer.RemoveAllChildren();
        objectLayer.RemoveAllChildren();
        if (player == null)
            player = new Player(this);
        else
            player.clearVars();


        map = new FTmxMap();
        map.clipNode = C.getCameraInstance();
        map.LoadTMX("Maps/" + mapName);
        C.getCameraInstance().setWorldBounds(new Rect(0, -map.height, map.width, map.height));
        C.getCameraInstance().setPlayer(player);

        foreach (XMLNode node in map.objects)
        {
            if (node.attributes.ContainsKey("gid"))
            {
                switch (int.Parse(node.attributes["gid"]) - map.objectLayerStartGID)
                {
                    case TRAMPOLINE_IND:
                        parseTrampoline(node);
                        break;
                    case CHEST_IND:
                        parseChest(node);
                        break;
                    case CONTAINER_IND:
                        parseContainer(node);
                        break;
                    case DOOR_IND:
                        parseDoor(node);
                        break;
                    case SWITCH_IND:
                        parseSwitch(node);
                        break;
                    case PRESSURE_PLATE_IND:
                        parsePlate(node);
                        break;
                    case ENEMY_IND:
                        parseEnemy(node);
                        break;
                }
            }
            if (node.attributes.ContainsKey("type"))
            {
                switch (node.attributes["type"].ToLower())
                {
                    case "stairwell":
                        parseStairwell(node);
                        break;
                }
            }
        }

        foreach (InteractableObject o in interactObjectList)
            if (o is Switch)
                ((Switch)o).findDoor(doorList);

        foreach (PressurePlate p in plateList)
            p.findDoor(doorList);

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
                            player.x = chest.x;
                            player.y = chest.y;
                            player.activateChest(chest);
                            spawnChest = chest;
                            break;
                        case "end":
                            chest.setEnd(property.attributes["value"]);
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
        string doorName = "";
        if (node.attributes.ContainsKey("name"))
            doorName = node.attributes["name"];
        bool locked = false;
        bool open = false;
        if (node.children.Count > 0)
            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                if (property.attributes.ContainsKey("name"))
                    switch (property.attributes["name"].ToLower())
                    {
                        case "locked":
                            locked = true;
                            break;
                        case "open":
                            open = true;
                            break;
                    }
            }
        Door door = new Door(new Vector2(float.Parse(node.attributes["x"]) + map.tileWidth / 2, -float.Parse(node.attributes["y"]) + map.tileHeight / 2), doorName, locked, open);
        doorList.Add(door);
        interactObjectList.Add(door);
        objectLayer.AddChild(door);
    }

    private void parseSwitch(XMLNode node)
    {
        string doorName = "";
        string actionType = "";
        float timerValue = -1;
        if (node.children.Count > 0)
            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                if (property.attributes.ContainsKey("name"))
                    switch (property.attributes["name"].ToLower())
                    {
                        case "door":
                            doorName = property.attributes["value"];
                            break;
                        case "action":
                            actionType = property.attributes["value"];
                            break;
                        case "time":
                            if (!float.TryParse(property.attributes["value"], out timerValue))
                                RXDebug.Log("invalid timer value");
                            break;
                    }
            }
        Switch s = new Switch(new Vector2(float.Parse(node.attributes["x"]) + map.tileWidth / 2, -float.Parse(node.attributes["y"]) + map.tileHeight / 2), doorName, actionType, timerValue);
        interactObjectList.Add(s);
        objectLayer.AddChild(s);
    }

    private void parsePlate(XMLNode node)
    {
        string doorName = "";
        string actionType = "";
        float timerValue = -1;
        if (node.children.Count > 0)
            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                if (property.attributes.ContainsKey("name"))
                    switch (property.attributes["name"].ToLower())
                    {
                        case "door":
                            doorName = property.attributes["value"];
                            break;
                        case "action":
                            actionType = property.attributes["value"];
                            break;
                        case "time":
                            if (!float.TryParse(property.attributes["value"], out timerValue))
                                RXDebug.Log("invalid timer value");
                            break;
                    }
            }
        PressurePlate p = new PressurePlate(new Vector2(float.Parse(node.attributes["x"]) + map.tileWidth / 2, -float.Parse(node.attributes["y"]) + map.tileHeight / 2), doorName, actionType, timerValue);
        plateList.Add(p);
        objectLayer.AddChild(p);
    }

    private void parseEnemy(XMLNode node)
    {
        bool faceLeft = false;
        if (node.children.Count > 0)
            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                if (property.attributes.ContainsKey("name"))
                    switch (property.attributes["name"].ToLower())
                    {
                        case "left":
                            faceLeft = true;
                            break;
                    }
            }
        Enemy e = new Enemy(this, faceLeft);
        e.SetPosition(new Vector2(float.Parse(node.attributes["x"]) + map.tileWidth / 2, -float.Parse(node.attributes["y"]) + Enemy.collisionHeight / 2));

        enemyList.Add(e);
        playerLayer.AddChild(e);
    }

    private void parseStairwell(XMLNode node)
    {
        if (node.children.Count > 0)
        {
            XMLNode polyLine = (XMLNode)node.children[0];
            string[] points = polyLine.attributes["points"].Split(' ');
            if (points.Length != 2)
                return;
            Vector2 disp = new Vector2(float.Parse(node.attributes["x"]), -float.Parse(node.attributes["y"]) + map.tileHeight / 2);
            Vector2[] stairwellPositions = new Vector2[2];
            for (int x = 0; x < 2; x++)
            {
                string[] xyStrings = points[x].Split(',');
                stairwellPositions[x] = (disp - new Vector2(float.Parse(xyStrings[0]), float.Parse(xyStrings[1])));
            }
            Stairwell s = new Stairwell(stairwellPositions[0], stairwellPositions[1]);
            interactObjectList.Add(s.individualStairwells[0]);
            interactObjectList.Add(s.individualStairwells[1]);
            objectLayer.AddChild(s);
        }
        
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
            if (((d.x + Door.DOOR_COLLISION_WIDTH / 2 > oldXPos && d.x - Door.DOOR_COLLISION_WIDTH / 2 < xPos) ||
                (d.x - Door.DOOR_COLLISION_WIDTH / 2 < oldXPos && d.x + Door.DOOR_COLLISION_WIDTH / 2 > xPos)) &&
                d.y - 16 <= yPos &&
                d.y + 16 >= yPos)
                return d;
        }
        return null;
    }

    public bool checkPlate(PressurePlate p, float xPos, float yPos)
    {
        return p.x - PressurePlate.COLLISION_WIDTH / 2 < xPos &&
            p.x + PressurePlate.COLLISION_WIDTH / 2 > xPos &&
            p.y - map.tileHeight / 2 + PressurePlate.COLLISION_HEIGHT > yPos;
    }

    public bool getOneWay(float xPos, float yPos)
    {
        int frameNum = collision.getFrameNumAt(xPos, yPos);
        return frameNum == 2;
    }

    private void Update()
    {
        if (C.isTransitioning)
            return;
        InteractableObject obj = checkInteractObjects();
        if (obj != null)
            player.setInteractObject(obj);
        foreach (Enemy e in enemyList)
            e.Update(player);
    }

    public PressurePlate checkPlates(float xPos, float yPos)
    {
        PressurePlate result = null;
        for (int x = 0; x < plateList.Count; x++)
        {
            if (result == null && checkPlate(plateList[x], xPos, yPos))
            {
                plateList[x].press(player);
                result = plateList[x];
            }
            else
            {
                plateList[x].unpress();
            }
        }
        return result;
    }

    private InteractableObject checkInteractObjects()
    {
        InteractableObject foundObject = null;
        float closestXDist = 0;
        for (int x = 0; x < interactObjectList.Count; x++)
            if (interactObjectList[x].checkInteractDist(player))
            {
                float xDist = Math.Abs(player.x - interactObjectList[x].x);
                if (foundObject == null)
                {
                    foundObject = interactObjectList[x];
                    closestXDist = xDist;
                }
                else
                {
                    if (xDist < closestXDist)
                    {
                        closestXDist = xDist;
                        foundObject.turnOffInteractInd();
                        foundObject = interactObjectList[x];
                    }
                    else
                    {
                        interactObjectList[x].turnOffInteractInd();
                    }
                }
            }
        //If we made it this far then 
        if (foundObject == null)
            player.clearInteractable();
        return foundObject;
    }

    public void addVanishCloud(VanishCloud v)
    {
        this.panacheEnabledClouds.Add(v);
    }

    public void removeVanishCloud(VanishCloud v)
    {
        if (this.panacheEnabledClouds.Contains(v))
            panacheEnabledClouds.Remove(v);
    }

    internal void nextLevel(string nextLevel)
    {
        C.getCameraInstance().endLevel(nextLevel);
    }
}
