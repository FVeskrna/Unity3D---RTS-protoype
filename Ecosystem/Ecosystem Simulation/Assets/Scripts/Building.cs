using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingManager BuildingManager;
    public GameObject BuildingModel;
    public GameObject BuildingPreview;

    public int Height;
    public int width;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void build()
    {
        if(!BuildingManager.BuildingActive)
        {
            BuildingManager.BuildingActive = true;
            BuildingManager.BuildingHeight = Height;
            BuildingManager.BuildingWidth = width;
            BuildingManager.ObjectToSpawn = BuildingModel;

        }
    }
}
