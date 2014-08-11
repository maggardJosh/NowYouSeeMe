using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FTmxMap : FContainer
{

    private List<XMLNode> _tilesets;
    protected List<string> _layerNames;
    public List<XMLNode> objects;

    public List<FTilemap> tilemaps;
    private FNode _clipNode; // for tilemaps

    public int objectStartInt = 1;
    public int objectLayerStartGID = 0;

    public int width = 0;
    public int height = 0;

    public int tileWidth;
    public int tileHeight;
    public string actualMapName;
    public string mapName;
    public string mapDescription;

    public FTmxMap()
    {
        tilemaps = new List<FTilemap>();
        _tilesets = new List<XMLNode>();
        _layerNames = new List<string>();
        objects = new List<XMLNode>();
    }
    public void LoadTMX(string fileName)
    {
        // load xml document
        TextAsset dataAsset = (TextAsset)Resources.Load(fileName, typeof(TextAsset));
        if (!dataAsset)
        {
            Debug.Log("FTiledScene: Couldn't load the xml data from: " + fileName);
        }
        string fileContents = dataAsset.ToString();
        Resources.UnloadAsset(dataAsset);

        // parse xml string
        XMLReader parser = new XMLReader();
        XMLNode xmlNode = parser.read(fileContents);
        XMLNode rootNode = xmlNode.children[0] as XMLNode;
        int firstgid = 0;

        this.actualMapName = fileName;


        int tilesWide = int.Parse(rootNode.attributes["width"]);
        int tilesHigh = int.Parse(rootNode.attributes["height"]);
        tileWidth = int.Parse(rootNode.attributes["tilewidth"]);
        tileHeight = int.Parse(rootNode.attributes["tileheight"]);

        this.width = tilesWide * tileWidth;
        this.height = tilesHigh * tileHeight;
        // loop through all children
        foreach (XMLNode child in rootNode.children)
        {

            if (child.tagName == "properties")
            {
                foreach (XMLNode property in child.children)
                {
                    switch (property.attributes["name"].ToLower())
                    {
                        case "mapname":
                            this.mapName = property.attributes["value"];
                            break;
                        case "mapdescription":
                            this.mapDescription = property.attributes["value"];
                            break;
                    }

                }
            }
            // save references to tilesets
            if (child.tagName == "tileset")
            {
                _tilesets.Add(child);
                if (child.attributes["name"].CompareTo("objects") == 0)
                {
                    objectLayerStartGID = int.Parse(child.attributes["firstgid"]);
                }
                firstgid = int.Parse(child.attributes["firstgid"]);
            }

            // create FTilemap for layer nodes
            if (child.tagName == "layer" && child.children.Count > 0)
            {
                FTilemap tilemap = (FTilemap)this.createTilemap(child);
                tilemaps.Add(tilemap);
                AddChild(tilemap);
            }

            // create FContainers for layer nodes
            if (child.tagName == "objectgroup")
            {
                AddChild(this.createObjectLayer(child));
            }
        }

    }

    protected string getTilesetNameForID(int num)
    {
        if (_tilesets.Count < 1)
        {
            Debug.Log("FTiledScene: No Tilesets found.");
            return "";
        }

        XMLNode wantedNode = _tilesets[0];

        // loop through tilesets
        foreach (XMLNode node in _tilesets)
        {
            // check if node attribute firstgid >= num
            int firstID = int.Parse(node.attributes["firstgid"]);
            if (firstID <= num)
            {
                wantedNode = node;
            }
        }

        // return the name of the file from wantedNode
        XMLNode imageNode = wantedNode.children[0] as XMLNode;
        string sourceString = imageNode.attributes["source"];
        int startIndex = sourceString.LastIndexOf('/') + 1;
        string returnValue = sourceString.Substring(startIndex, sourceString.LastIndexOf('.') - startIndex);

        return returnValue;
    }

    protected string getTilesetExtensionForID(int num)
    {
        if (_tilesets.Count < 1)
        {
            Debug.Log("FTiledScene: No Tilesets found.");
            return "";
        }

        XMLNode wantedNode = _tilesets[0];

        // loop through tilesets
        foreach (XMLNode node in _tilesets)
        {
            // check if node attribute firstgid >= num
            int firstID = int.Parse(node.attributes["firstgid"]);
            if (firstID <= num)
            {
                wantedNode = node;
            }
        }

        // return the extension of the file from wantedNode
        XMLNode imageNode = wantedNode.children[0] as XMLNode;
        string sourceString = imageNode.attributes["source"];
        int startIndex = sourceString.LastIndexOf('.') + 1;
        string returnValue = sourceString.Substring(startIndex);

        return returnValue;
    }

    public int getTilesetFirstIDForID(int num)
    {
        if (_tilesets.Count < 1)
        {
            Debug.Log("FTiledScene: No Tilesets found.");
            return -1;
        }

        XMLNode wantedNode = _tilesets[0];

        // loop through tilesets
        foreach (XMLNode node in _tilesets)
        {
            // check if node attribute firstgid >= num
            int firstID = int.Parse(node.attributes["firstgid"]);
            if (firstID <= num)
            {
                wantedNode = node;
            }
        }

        // return the firstgid of the file from wantedNode
        int startIndex = int.Parse(wantedNode.attributes["firstgid"]);

        return startIndex;
    }

    virtual protected FNode createObjectLayer(XMLNode node)
    {
        // add objects to FContainers
        FContainer objectGroup = new FContainer();

        foreach (XMLNode fObject in node.children)
        {
            if (fObject.tagName == "object")
            {
                objects.Add(fObject);
                /*
                if (fObject.attributes.ContainsKey("gid")) {
                    // create FSprite (override that function for specific class changes)
                    objectGroup.AddChild(this.createTileObject(fObject));
                } else {
                    FNode newObject = this.createObject(fObject);
                    if (newObject != null) {
                        objectGroup.AddChild(newObject);
                    }
                }
                 * 
                 * 
                 * Took this out since I handle objects on my own
                 * 
                 */
            }
        }

        // remember name 
        _layerNames.Add(node.attributes["name"]);

        // add to self
        return objectGroup;
    }

    virtual protected FNode createTilemap(XMLNode node)
    {
        XMLNode csvData = new XMLNode();
        XMLNode properties = new XMLNode();
        foreach (XMLNode child in node.children)
        {
            if (child.tagName == "data")
            {
                csvData = child;
            }
            else if (child.tagName == "properties")
            {
                properties = child;
            }
        }

        // make sure encoding is set to csv
        if (csvData.attributes["encoding"] != "csv")
        {
            Debug.Log("FTiledScene: Could not render layer data, encoding set to: " + csvData.attributes["encoding"]);
            return null;
        }

        // remember name 
        _layerNames.Add(node.attributes["name"]);

        // skipZero, if this is true all filled tiles will be drawn without clipping
        bool skipZero = false;
        bool draw = true;

        // get text for csv data
        string csvText = csvData.value;
        string firstFrame = csvText.Substring(0, csvText.IndexOf(','));
        string[] frames = csvText.Split(',');
        for (int ind = 0; ind < frames.Length; ind++)
        {
            if (int.Parse(firstFrame) != 0)
                break;
            firstFrame = frames[ind];
        }

        int firstID = int.Parse(firstFrame);

        // find name of tileset being used, assumes all tiles are from the same tileset
        string baseName = this.getTilesetNameForID(firstID);


        // do stuff with properties
        foreach (XMLNode property in properties.children)
        {

            // check each property
            if (property.attributes["name"] == "skipZero")
            {
                skipZero = bool.Parse(property.attributes["value"]);
            }
            else if (property.attributes["name"].CompareTo("draw") == 0)
            {
                draw = bool.Parse(property.attributes["value"]);
            }
            else if (property.attributes["name"].CompareTo("tileset") == 0)
            {
                baseName = property.attributes["value"];
            }
        }


        // create tilemap
        FTilemap tilemap = new FTilemap(baseName);
        if (!skipZero)
        {
            tilemap.clipToScreen = true;
            tilemap.clipNode = _clipNode;
        }

        tilemap.LoadText(csvText, skipZero, draw, getTilesetFirstIDForID(firstID) - 1);
        return tilemap;

    }

    virtual protected FNode createTileObject(XMLNode node)
    {
        // get id numbers needed
        int id = int.Parse(node.attributes["gid"]);
        int firstID = this.getTilesetFirstIDForID(id);

        // find parts of source image
        string baseName = this.getTilesetNameForID(id);
        int actualFrame = id - firstID + objectStartInt;

        // assemble whole name
        string name = baseName + "_" + actualFrame;

        // get x,y
        int givenX = int.Parse(node.attributes["x"]);
        int givenY = int.Parse(node.attributes["y"]);

        FNode testNode = new FNode();

        // ***** Took out code for using sprites on objects ****
        //	FSprite sprite = new FSprite(name);
        //	sprite.x = givenX + sprite.width / 2;
        //	sprite.y = -givenY + sprite.height / 2;

        testNode.x = givenX;// + sprite.width / 2;
        testNode.y = -givenY;// + sprite.height / 2;

        return testNode;
    }

    virtual protected FNode createObject(XMLNode node)
    {
        // type
        string type = node.attributes["type"];

        // get x,y
        int givenX = int.Parse(node.attributes["x"]);
        int givenY = int.Parse(node.attributes["y"]);

        // remove extension from type
        int startIndex = type.LastIndexOf('/') + 1;
        int endIndex = type.LastIndexOf('.');
        int length = endIndex - startIndex;
        if (length < 0)
        {
            length = type.Length - startIndex;
        }
        string spriteElement = type.Substring(startIndex, length);

        FSprite sprite = new FSprite(spriteElement);
        sprite.x = givenX + sprite.width / 2;
        sprite.y = -givenY + sprite.height / 2;

        return sprite;
    }

    public FNode getLayerNamed(string name)
    {
        int i = 0;
        foreach (string check in _layerNames)
        {
            if (check == name)
            {
                return GetChildAt(i);
            }
            i++;
        }
        //
        Debug.Log("No layer named " + name + " found.");
        return null;
    }

    public FNode clipNode
    {
        get { return _clipNode; }
        set { _clipNode = value; }
    }
}
