using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MapGeneration : MonoBehaviour
{   
    public GameObject Camera;
    public Slider WidthSlider;
    private int width;
    public Slider HeightSlider;
    private int height;
    public Slider VegetationSlider;
    private int Vegetation;
    public Slider IronOreSlider;
    private int IronOres;

    public Slider WaterPoolsSlider;
    private int WaterPools;
    public Slider LakeSizeSlider;
    private int LakeSize;
    public Slider MountainPoolsSlider;
    private int MountainPools;
    public Slider MountainSizeSlider;
    private int MountainSize;
    public Slider ForestPoolsSlider;
    private int Forests;
    public Slider ForestSizeSlider;
    private int ForestSize;

    public List<GameObject> VegetationProps;
    public List<GameObject> Mushrooms;
    public GameObject Grass;
    public GameObject Water;
    public GameObject Stone;
    public GameObject Border;
    public GameObject Tree;
    public GameObject Sand;
    public GameObject Tent;
    public GameObject Plane;
    public GameObject IronOre;

    private Node[,] gridArray;
    private Node[,,] MountArray;
    private Vector3 gridOrigin;

    public List<GameObject> Tiles = new List<GameObject>();
    public List<NavMeshSurface> Surfaces = new List<NavMeshSurface>();
    private GameObject WaterTiles;
    private GameObject GrassTiles;
    private GameObject MountainTiles;
    [HideInInspector] public GameObject PropsParent;
    private GameObject SandTiles;

    public int MountainHeight;

    // Start is called before the first frame update

    private void Start() {
        PropsParent = GameObject.Find("Props");
        WaterTiles = GameObject.Find("WaterTiles");
        GrassTiles = GameObject.Find("GrassTiles");
        MountainTiles = GameObject.Find("MountainTiles");
        SandTiles = GameObject.Find("SandTiles");
    }
    public void SpawnWorld()
    {
        height = (int)HeightSlider.value;
        width = (int)WidthSlider.value;
        WaterPools = (int)WaterPoolsSlider.value;
        LakeSize = (int)LakeSizeSlider.value;
        MountainPools = (int)MountainPoolsSlider.value;
        MountainSize = (int)MountainSizeSlider.value;
        Forests = (int)ForestPoolsSlider.value;
        ForestSize = (int)ForestSizeSlider.value;
        Vegetation = (int)VegetationSlider.value;
        IronOres = (int)IronOreSlider.value;

        float MapSize = width * height;
        LakeSize *= (int)MapSize/2500;
        MountainSize *= (int)MapSize/2500;
        ForestSize *= (int)MapSize/2500;
 
        initializeMap();

        TryToSpawnLakes();
       

        TryToSpawnMountains();

        GenerateGrass();
        
        for(int x=0;x<MountainHeight;x++)
        RaiseMountains();

        MakeMountainsHollow();

        

        GenerateBeaches();
        RemoveIslands();
        GenerateBeaches();

        TryToSpawnForests();
        SpawnIronNodes();
        


        GenerateVegetation();

        Invoke("CombineMesh",0.1f);
    }

    public void TryToSpawnLakes(){
        for(int x=0;x<WaterPools;x++)
        {
            int tries = 0;
            while(!GenerateWaterSeeds(Random.Range(2,width-2),Random.Range(2,height-2),LakeSize))
            {
                tries++;
                if(tries > 100)
                break;
            }
        }
    }

    public void TryToSpawnMountains(){
        for(int x=0;x<MountainPools;x++)
        {
            int tries = 0;
            while(!GenerateMountainSeeds(Random.Range(2,width-2),Random.Range(2,height-2),MountainSize))
            {
                tries++;
                if(tries > 100)
                break;
            }
        }
    }

    public void TryToSpawnForests(){
        for(int x=0;x<Forests;x++)
        {
            int tries = 0;
            while(!GenerateForestSeeds(Random.Range(2,width-2),Random.Range(2,height-2),ForestSize,Tree))
            {
                tries++;

                if(tries > 100)
                break;
            }
        }
    }

    public void SpawnIronNodes(){
                for(int x=0; x<IronOres;x++)
        {
            bool foundspot = false;
            int tries = 0;
            while(!foundspot)
            {
                int _x = (int)Random.Range(2,width-2);
                int _z = (int)Random.Range(2,height-2);
                if(CanBePlaced(_x,_z,2,2))
                {
                    foundspot = true;
                    RemoveVegetation(_x,_z);
                    RemoveVegetation(_x+1,_z);
                    RemoveVegetation(_x,_z+1);
                    RemoveVegetation(_x+1,_z+1);
                    GameObject Ore = Instantiate(IronOre,new Vector3(_x,1,_z),Quaternion.identity);
                    Ore.transform.SetParent(PropsParent.transform);
                    MakeTilesOccupied(_x,_z,2,2);
                }
                tries++;
                if(tries > 100)
                break;
            }
        }
    }

    public void RemoveWorld()
    {
        foreach(Transform Object in transform)
        {
            Object.GetComponent<MeshFilter>().mesh = null;
            foreach(Transform Child in Object)
            {
                Destroy(Child.gameObject);
            }
        }
        
        Tiles = new List<GameObject>();
    }
    
    void initializeMap()
    {
        MountArray = new Node[width,50, height];
        for (int x=0; x<MountArray.GetLength(0); x++)
        {
            for (int y=0; y<MountArray.GetLength(1);y++)
            {
                for (int z=0; z<MountArray.GetLength(2);z++)
                {
                    MountArray[x,y,z] = new Node();
                }
            }
        }

        gridArray = new Node[width,height];
        gridOrigin = new Vector3(0,0,0);

        for (int x=0; x<gridArray.GetLength(0); x++)
        {
            for (int z=0; z<gridArray.GetLength(1);z++)
            {
                gridArray[x,z] = new Node();

                if(x==0 || x == width-1 || z==0 || z==height-1)
                {
                 Vector3 position = new Vector3(x,0,z) + gridOrigin;    
                GameObject Tile = Instantiate(Border, position, Quaternion.identity);
                Tile.transform.SetParent(MountainTiles.transform);
                Tile.name = "BorderStone";
                gridArray[x,z].tileExists = true;
                gridArray[x,z].Occupied = true;
                gridArray[x,z].Tile = Tile;
                }
            }
        }
    }
    private bool GenerateWaterSeeds(int x, int z,int MaxSize)
    {
        if(gridArray[x,z].tileExists)
        return false;

        Vector3 position = new Vector3(x,0,z) + gridOrigin;
        GameObject Tile = Instantiate(Water, position, Quaternion.identity);
        Tile.transform.SetParent(WaterTiles.transform);
        Tile.name = "Water";

        gridArray[x,z].Occupied = true;
        gridArray[x,z].tileExists = true;
        gridArray[x,z].Tile = Tile;
        Tiles.Add(Tile);

        GenerateWater(x-1,z,MaxSize-1, MaxSize);
        GenerateWater(x+1,z,MaxSize-1, MaxSize);
        GenerateWater(x,z+1,MaxSize-1, MaxSize);
        GenerateWater(x,z-1,MaxSize-1, MaxSize);

        return true;
    }

    void GenerateWater(int x, int z, int propability,int MaxSize)
    {
        if(x>=width || x<0 || z>=height || z<0)
        return;

        if(gridArray[x,z].tileExists || propability == 0)
        return;

        int generated = Random.Range(0,LakeSize);

        if(generated > propability)
        return;

        Vector3 position = new Vector3(x,0,z) + gridOrigin;
        GameObject Tile = Instantiate(Water, position, Quaternion.identity);
        Tile.transform.SetParent(WaterTiles.transform);
        Tile.name = "Water";
        gridArray[x,z].Occupied = true;
        gridArray[x,z].Tile = Tile;
        gridArray[x,z].tileExists = true;
        Tiles.Add(Tile);
        

        GenerateWater(x-1,z,propability-1, MaxSize);
        GenerateWater(x+1,z,propability-1, MaxSize);
        GenerateWater(x,z-1,propability-1, MaxSize);
        GenerateWater(x,z+1,propability-1, MaxSize);
    }

    private bool GenerateMountainSeeds(int x, int z,int MaxSize)
    {
        if(gridArray[x,z].tileExists)
        return false;

        Vector3 position = new Vector3(x,1,z) + gridOrigin;
        GameObject Tile = Instantiate(Stone, position, Quaternion.identity);
        GameObject Tile2 = Instantiate(Stone, position+ new Vector3(0,-1,0), Quaternion.identity);
        Tile.transform.SetParent(MountainTiles.transform);
        Tile.name = "Stone";
        Tile2.transform.SetParent(MountainTiles.transform);
        Tile2.name = "Stone";
        Tiles.Add(Tile);
        gridArray[x,z].Occupied = true;
        gridArray[x,z].Tile = Tile;
        gridArray[x,z].tileExists = true;
        MountArray[x,0,z].Tile = Tile;
        MountArray[x,1,z].Tile = Tile2;



        GenerateMountain(x-1,z,MaxSize-1, MaxSize);
        GenerateMountain(x+1,z,MaxSize-1, MaxSize);
        GenerateMountain(x,z+1,MaxSize-1, MaxSize);
        GenerateMountain(x,z-1,MaxSize-1, MaxSize);

        return true;
    }

    void GenerateMountain(int x, int z, int propability,int MaxSize)
    {
        if(x>=width || x<0 || z>=height || z<0)
        return;

        if(gridArray[x,z].Occupied || propability == 0)
        return;

        int generated = Random.Range(0,MountainSize);

        if(generated > propability)
        return;

        Vector3 position = new Vector3(x,1,z) + gridOrigin;
        GameObject Tile = Instantiate(Stone, position, Quaternion.identity);
        GameObject Tile2 = Instantiate(Stone, position+ new Vector3(0,-1,0), Quaternion.identity);
        Tile.transform.SetParent(MountainTiles.transform);
        Tile.name = "Stone";
        Tile2.transform.SetParent(MountainTiles.transform);
        Tile2.name = "Stone";
        Tiles.Add(Tile);

        gridArray[x,z].Occupied = true;
        gridArray[x,z].Tile = Tile;
        gridArray[x,z].tileExists = true;
        MountArray[x,0,z].Tile = Tile;
        MountArray[x,1,z].Tile = Tile2;
        

        GenerateMountain(x-1,z,propability-1, MaxSize);
        GenerateMountain(x+1,z,propability-1, MaxSize);
        GenerateMountain(x,z-1,propability-1, MaxSize);
        GenerateMountain(x,z+1,propability-1, MaxSize);
    }

    bool RaiseMountains()
    {
        bool raised = false;
        for (int x=0; x<gridArray.GetLength(0); x++)
        {
            for (int z=0; z<gridArray.GetLength(1);z++)
            {
                if(gridArray[x,z].Tile.name == "Stone" && MountainCount(x,z,gridArray[x,z].TileHeight) > Random.Range(2,4))
                {
                    Vector3 position = new Vector3(x, gridArray[x,z].TileHeight+1,z) + gridOrigin;
                    GameObject Tile = Instantiate(Stone, position, Quaternion.identity);
                    Tile.transform.SetParent(MountainTiles.transform);
                    Tile.name = "Stone";
                    gridArray[x,z].TileHeight++;
                    raised = true;
                    MountArray[x,gridArray[x,z].TileHeight,z].Tile = Tile;
                }
            }
        }
        
        if(raised)
        return true;
        else
        return false;
        
    }

    int MountainCount(int x, int z, int height)
    {
        int count = 0;
        if(gridArray[x-1,z].Tile.name == "Stone" && gridArray[x-1,z].TileHeight >= height)
        count++;
        if(gridArray[x+1,z].Tile.name == "Stone" && gridArray[x+1,z].TileHeight >= height)
        count++;
        if(gridArray[x,z-1].Tile.name == "Stone" && gridArray[x,z-1].TileHeight >= height)
        count++;
        if(gridArray[x,z+1].Tile.name == "Stone" && gridArray[x,z+1].TileHeight >= height)
        count++;

        return count;
    }

    void GenerateGrass()
    {
        for (int x=0; x<gridArray.GetLength(0); x++)
        {
            for (int z=0; z<gridArray.GetLength(1);z++)
            {
                if(gridArray[x,z].tileExists == false)
                {
                    Vector3 position = new Vector3(x,0,z) + gridOrigin;
                    GameObject Tile = Instantiate(Grass, position, Quaternion.identity);
                    Tile.transform.SetParent(GrassTiles.transform);
                    Tile.name = "Grass";
                    gridArray[x,z].tileExists = true;
                    gridArray[x,z].Tile = Tile;
                    Tiles.Add(Tile);             
                }   
            }
        }
    }

    void GenerateVegetation()
    {
        for (int x=0; x<gridArray.GetLength(0); x++)
        {
            for (int z=0; z<gridArray.GetLength(1);z++)
            {
                if(gridArray[x,z].Tile.name == "Grass" && !gridArray[x,z].Occupied)
                {
                    int rannum = Random.Range(1,100);
                    if(!gridArray[x,z].Decoration && rannum <= Vegetation)
                    {
                        float _x = Random.Range(-0.5f,0.5f) + x;
                        float _z = Random.Range(-0.5f,0.5f) + z;
                        rannum = Random.Range(0,VegetationProps.Count);
                        gridArray[x,z].Decoration = true;
            
                        GameObject spawnedTile = Instantiate(VegetationProps[rannum],new Vector3(_x,1,_z),Quaternion.identity); 
                        spawnedTile.name = VegetationProps[rannum].name;
                        spawnedTile.transform.SetParent(PropsParent.transform);
                        gridArray[x,z].Vegetation = spawnedTile;
                    }
                }
            }
        }
    }
    private bool GenerateForestSeeds(int x, int z,int MaxSize,GameObject tile)
    {
        if(gridArray[x,z].Occupied || gridArray[x,z].Tile.name != "Grass")
        return false;

        float _x = Random.Range(-0.5f,0.5f) + x;
        float _z = Random.Range(-0.5f,0.5f) + z;

        Vector3 position = new Vector3(_x,1,_z) + gridOrigin;
        GameObject Tile = Instantiate(tile, position, Quaternion.identity);
        Tile.transform.SetParent(PropsParent.transform);
        Tile.name = "Tree";

        





        gridArray[x,z].Occupied = true;
        int offset = Random.Range(0,3);

        GenerateForest(x-offset,z,MaxSize-1, MaxSize, tile);
        offset = Random.Range(0,3);
        GenerateForest(x+offset,z,MaxSize-1, MaxSize,tile);
        offset = Random.Range(0,3);
        GenerateForest(x,z+offset,MaxSize-1, MaxSize,tile);
        offset = Random.Range(0,3);
        GenerateForest(x,z-offset,MaxSize-1, MaxSize,tile);

        return true;
    }
    void GenerateForest(int x, int z, int propability,int MaxSize,GameObject tile)
    {
        if(x>=width-1 || x<1 || z>=height-1 || z<1)
        return;

        if(gridArray[x,z].Occupied || propability == 0 || gridArray[x,z].Tile.name != "Grass")
        return;

        int generated = Random.Range(0,ForestSize);

        if(generated > propability)
        return;

        float _x = Random.Range(-0.5f,0.5f) + x;
        float _z = Random.Range(-0.5f,0.5f) + z;
        Vector3 position = new Vector3(_x,1,_z) + gridOrigin;
        GameObject Tile = Instantiate(tile, position, Quaternion.identity);
        Tile.transform.SetParent(PropsParent.transform);
        Tile.name = "Tree";

        if(Random.Range(0,100)>50)
        {
            _x = Random.Range(-0.5f,0.5f) + x;
            _z = Random.Range(-0.5f,0.5f) + z;
            int mushroomType = Random.Range(0,Mushrooms.Count);
            position = new Vector3(_x,1,_z) + gridOrigin;
            GameObject Mushroom = Instantiate(Mushrooms[mushroomType], position, Quaternion.identity);
            Mushroom.transform.SetParent(PropsParent.transform);
        }

        gridArray[x,z].Occupied = true;


        int offset = Random.Range(0,3);
        GenerateForest(x-offset,z,propability-1, MaxSize, tile);
        offset = Random.Range(0,3);
        GenerateForest(x+offset,z,propability-1, MaxSize,tile);
        offset = Random.Range(0,3);
        GenerateForest(x,z+offset,propability-1, MaxSize,tile);
        offset = Random.Range(0,3);
        GenerateForest(x,z-offset,propability-1, MaxSize,tile);
    }

    void GenerateBeaches()
    {
        for (int x=0; x<gridArray.GetLength(0); x++)
        {
            for (int z=0; z<gridArray.GetLength(1);z++)
            {
                if(gridArray[x,z].Tile.name == "Water")
                {
                    if(gridArray[x-1,z].Tile.name == "Grass")
                        Replace(x-1,z,Sand);
                    if(gridArray[x+1,z].Tile.name == "Grass")
                        Replace(x+1,z,Sand);
                    if(gridArray[x,z-1].Tile.name == "Grass")
                        Replace(x,z-1,Sand);
                    if(gridArray[x,z+1].Tile.name == "Grass")
                        Replace(x,z+1,Sand);

                    
                    
                }   
            }
        }
    }

    void GenerateOre(GameObject Ore)
    {
        
    }

    void MakeMountainsHollow()
    {
        for (int x=1; x<MountArray.GetLength(0)-1; x++)
        {
            for (int y=0; y<MountArray.GetLength(1);y++)
            {
                for (int z=1; z<MountArray.GetLength(2)-1;z++)
                {
                    if(MountArray[x+1,y,z].Tile != null && MountArray[x+1,y,z].Tile.name == "Stone" &&
                    MountArray[x-1,y,z].Tile != null && MountArray[x-1,y,z].Tile.name == "Stone" &&
                    MountArray[x,y,z+1].Tile != null && MountArray[x,y,z+1].Tile.name == "Stone" &&
                    MountArray[x,y,z-1].Tile != null && MountArray[x,y,z-1].Tile.name == "Stone" &&
                    MountArray[x,y+1,z].Tile != null && MountArray[x,y+1,z].Tile.name == "Stone")
                    {
                        Destroy(MountArray[x,y,z].Tile);
                        Debug.Log("DestroyedStone");
                    }
                }
            }
        }
    }

    void RemoveIslands()
    {
        bool Removed = true;
        while(Removed)
        {
            Removed = false;
            for (int x=0; x<gridArray.GetLength(0); x++)
            {
                for (int z=0; z<gridArray.GetLength(1);z++)
                {
                    
                    /*
                    if(gridArray[x,z].Tile.name != "Sand")
                    return;
                    */
                    if(gridArray[x,z].Tile.name == "Sand" && IsLonelyIsland(x,z))
                    {
                        Replace(x,z,Water);
                        Removed = true;
                    }
                }
            }
        }
        
    }

    bool IsLonelyIsland(int x, int z)
    {
        if(x>=width-1 || x<1 || z>=height-1 || z<1)
        return false;

        int Count = 0;
        if(gridArray[x-1,z].Tile.name == "Water")
        Count++;
        if(gridArray[x+1,z].Tile.name == "Water")
        Count++;
        if(gridArray[x,z-1].Tile.name == "Water")
        Count++;
        if(gridArray[x,z+1].Tile.name == "Water")
        Count++;

        if(Count > 2)
        return true;
        else
        return false;
    }

    void Replace(int x, int z, GameObject NewObject)
    {
        Vector3 position = new Vector3(x,0,z) + gridOrigin;
        Tiles.Remove(gridArray[x,z].Tile);
        Destroy(gridArray[x,z].Tile);
        GameObject Tile = Instantiate(NewObject, position, Quaternion.identity);
        Tile.transform.SetParent(GameObject.Find(NewObject.name + "Tiles").transform);
        Tile.name = NewObject.name;
        gridArray[x,z].Tile = Tile;
        Tiles.Add(Tile);
    }

    void CombineMesh()
    {
        SandTiles.GetComponent<MeshCombiner>().CombineMeshes(true);
        WaterTiles.GetComponent<MeshCombiner>().CombineMeshes(true);
        GrassTiles.GetComponent<MeshCombiner>().CombineMeshes(true);
        MountainTiles.GetComponent<MeshCombiner>().CombineMeshes(true);
    }

    public bool CanBePlaced(int x, int z, int _height, int _width)
    {
        x -= (int)gridOrigin.x;
        z -= (int)gridOrigin.z;
        Debug.Log(x.ToString()+" "+ z.ToString());

        if(x+_width > width || x <= 0 || z+_height > height || z <= 0)
        return false;

        for(int i = 0; i < _height;i++)
        {
            for(int j=0;j< _width;j++)
            {
                if(gridArray[x+j,z+i].Occupied)
                    return false;                    
            }
        }
        return true;
    }
    public bool IsOnCoast(int x, int z, int _height, int _width)
    {
        int _Watertiles = 0;
        int _NonOccupiedtiles = 0;
        x -= (int)gridOrigin.x;
        z -= (int)gridOrigin.z;
        Debug.Log(x.ToString()+" "+ z.ToString());

        if(x+_width > width || x <= 0 || z+_height > height || z <= 0)
        return false;

        for(int i = 0; i < _height;i++)
        {
            for(int j=0;j< _width;j++)
            {
                if(!gridArray[x+j,z+i].Occupied)
                _NonOccupiedtiles++;

                if(gridArray[x+j,z+i].Tile.name == "Water")
                _Watertiles++;                   
            }
        }

        if(_Watertiles >= (_height * _width)/2 && _NonOccupiedtiles >= (_height * _width)/2)
        return true;
        else
        return false;
    }

    

    public void MakeTilesOccupied(int x, int z, int _height, int _width)
    {
        for(int i = 0; i < _height;i++)
        {
            for(int j=0;j< _width;j++)
            {
                gridArray[x+j,z+i].Occupied = true;
                RemoveVegetation(x+j,z+i);
            }
        }
    }

    public void RemoveVegetation(int x, int z)
    {
        if(gridArray[x,z].Decoration)
        {
            Destroy(gridArray[x,z].Vegetation);
            gridArray[x,z].Vegetation = null;
            gridArray[x,z].Decoration = false;
        }
        
    }
}

class Node
{
    public bool tileExists;
    public bool Occupied;
    public bool Decoration;
    public int TileHeight;
    public GameObject Tile;
    public GameObject Vegetation;



    private void Start() {
        TileHeight = 1;
        tileExists = false;
        Occupied = false;
        Decoration = false;
        Tile.name = "none";
    }
}


