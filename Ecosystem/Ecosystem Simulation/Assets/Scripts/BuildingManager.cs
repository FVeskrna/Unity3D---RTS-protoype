using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingManager : MonoBehaviour
{
    public GameObject CantBuildText;
    private bool TextActive;
    public MapGeneration Mapscript;
    public bool BuildingActive;
    public GameObject ObjectToSpawn;
    public int BuildingWidth;
    public int BuildingHeight;
    private bool CanBePlaced;
    public GameObject ActiveObject;
    public GameObject SpawnedObject;
    private bool Spawned;

    public int X;
    public int Z;
    public int asd=0;

    Vector3 WhereToSpawn;
    void Update()
    {   
        if(Input.GetKeyDown(KeyCode.X) && BuildingActive)
        {
            SpawnedObject.transform.GetChild(0).eulerAngles += new Vector3(0,90,0);
        }

        if(Input.GetKeyDown(KeyCode.Escape) && BuildingActive)
        {
            BuildingActive = false;
            Destroy(SpawnedObject);
            BuildingHeight = 0;
            BuildingWidth = 0;
            ObjectToSpawn = null;
        }

        if(Input.GetMouseButton(0) && CanBePlaced && BuildingActive)
        {
            GameObject Building =  Instantiate(SpawnedObject,WhereToSpawn,SpawnedObject.transform.rotation);
            Building.transform.SetParent(Mapscript.PropsParent.transform);
            BuildingActive = false;
            Destroy(SpawnedObject);
            Mapscript.MakeTilesOccupied(X,Z,BuildingHeight,BuildingWidth);
        }

        else if(Input.GetMouseButton(0) && !CanBePlaced && BuildingActive)
        {
            CancelInvoke("HideObject");
            CantBuildText.SetActive(true);
            Invoke("HideObject",2f);
        }

        
        

        if(!Spawned && BuildingActive)
        {
            Spawned = true;
            SpawnedObject = Instantiate(ObjectToSpawn);
            CanBePlaced = false;
        }

        Vector3 Mouseposition = Input.mousePosition;
        Vector3 MousepositionOnGrid = new Vector3(Mathf.RoundToInt(GetMousePosition().x),1f,Mathf.RoundToInt(GetMousePosition().z));
        X = (int)MousepositionOnGrid.x;
        Z = (int)MousepositionOnGrid.z;
        WhereToSpawn = MousepositionOnGrid;
        if(BuildingActive)
        {
            if(ObjectToSpawn.name == "Pier")
            {
                if(Mapscript.IsOnCoast(X,Z,BuildingHeight,BuildingWidth))
                    CanBePlaced = true;   
                else
                    CanBePlaced = false;
            }
            else
            {
                if(Mapscript.CanBePlaced(X,Z,BuildingHeight,BuildingWidth))
                    CanBePlaced = true;    
                else
                    CanBePlaced = false;
            }

            SpawnedObject.transform.position = MousepositionOnGrid;
        }
        else
        Spawned = false;
    }

    private Vector3 GetMousePosition()
    {
        int layerMask = 1 << 6;
        layerMask = ~layerMask;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out RaycastHit raycastHit,9999f,layerMask))
            return raycastHit.point;
        else
            return Vector3.zero;
    }

    void HideObject()
    {
        CantBuildText.SetActive(false);
    }
}
