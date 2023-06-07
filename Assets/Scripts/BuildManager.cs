using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildManager : MonoBehaviour
{
    public GameObject ConstructionGhost;
    public GameObject HouseBuilding;
    public GameObject SolarArray;
    public GameObject MiningRig;
    public GameObject ColonyCenter;
    public GameObject Spacecraft;
    public GameObject Astronaut;
    public string selectedBuilding = "None";
    public bool canBuild = true;
    private bool ghostBuild = false;
    GameObject ghost;
    public UserControls Controls;
    public LayerMask BuildingLayerMask;
    private float orientation;

    // Start is called before the first frame update
    void Start()
    {
        //Get UserControls
        Controls = AssetManager.manager.Controls;
    }

    void cancelSelection(){
        ghostBuild = false;
        ghost.SetActive(false);
        selectedBuilding = "None";
        //Turn off colliders
        /* foreach (var obj in Controls.trackedObject.GetComponent<GeneratePlanetFeatures>().surfaceObjects)
        {
            obj.GetComponent<Collider>().enabled = false;
        } */
    }

    public void TerrainColliders(GeneratePlanetFeatures features, bool enable){
        foreach (var terrainObj in features.surfaceObjects)
        {
            if (terrainObj.tag == "Plant" || terrainObj.tag == "Terrain")
            {
                terrainObj.GetComponent<Collider>().enabled = enable;
            }
        }
    }

    public void PlaceUnit(string type, GameObject parent, Vector2 polar){
        //var civ = parent.GetComponent<CivManager>();
        var planetFeatures = parent.GetComponent<GeneratePlanetFeatures>();
        //var radius = planetFeatures.radius;
        GameObject newUnit = Instantiate(Astronaut, parent.transform, true);
        var RotationX = -planetFeatures.transform.localEulerAngles.x;
        var RotationY = -planetFeatures.transform.localEulerAngles.y;
        newUnit.transform.SetPositionAndRotation(
            transform.PolarToCartesian(polar.x+RotationX, polar.y+RotationY, planetFeatures.transform.localScale.y/2)+planetFeatures.transform.position, Quaternion.identity);
        planetFeatures.AddSurfaceObject(newUnit, polar);
        planetFeatures.AddSurfaceObjectMoving(newUnit);
    }

    public void PlaceBuilding(string selected, GameObject parent, Vector2 polar, GameObject ghost=null, float orientation=0){
        GameObject building = null;
        var civ = parent.GetComponent<CivManager>();
        var buildAttrs = Controls.Buildings.Where(e => e.Name == selected).First();
        switch(selected){
            case "HOUSE":
                building = HouseBuilding;
                break;
            case "COLONY CENTER":
                building = ColonyCenter;
                break;
            case "SOLAR ARRAY":
                building = SolarArray;
                break;
            case "MINING RIG":
                building = MiningRig;
                StartCoroutine(civ.MineResources(buildAttrs.OreProd));
                break;
            case "SPACECRAFT":
                building = Spacecraft;
                break;
        }
        //Set attributes
        civ.OreValue -= buildAttrs.MaterialCost;
        civ.EnergyUse += buildAttrs.EnergyCost;
        civ.EnergyProduction += buildAttrs.EnergyProd;
        civ.MiningProduction += buildAttrs.OreProd;

        var planetFeatures = parent.GetComponent<GeneratePlanetFeatures>();
        //Turn on colliders
        TerrainColliders(planetFeatures, true);
        var radius = planetFeatures.radius;
        GameObject newBuild = Instantiate(building, parent.transform, true);
        var state = newBuild.GetComponent<BuildingState>();
        state.Name = selected;
        state.Desc = buildAttrs.Description;
        if(ghost){
            newBuild.transform.SetPositionAndRotation(ghost.transform.position, ghost.transform.rotation);
            polar = transform.CartesianToPolar(ghost.transform.eulerAngles.x, ghost.transform.eulerAngles.y, ghost.transform.eulerAngles.z, radius);
            //Compensate for planetary motion on y-axis in case of spinning planet
            polar.y += parent.transform.localEulerAngles.y;
        } else {
            var RotationX = -planetFeatures.transform.localEulerAngles.x;
            var RotationY = -planetFeatures.transform.localEulerAngles.y;
            newBuild.transform.SetPositionAndRotation(
                transform.PolarToCartesian(polar.x+RotationX, polar.y+RotationY, planetFeatures.transform.localScale.y/2)+planetFeatures.transform.position, Quaternion.identity);
        }
        planetFeatures.AddSurfaceObject(newBuild, polar, orientation);
        Collider[] colliders;
        if(ghost){
            colliders = Physics.OverlapBox(ghost.transform.position, new Vector3(.75f, .75f, .75f));
        } else {
            colliders = Physics.OverlapBox(newBuild.transform.position, new Vector3(.75f, .75f, .75f));
        }
        GameObject obj;
        var surface = planetFeatures.surfaceObjects;
        foreach (var col in colliders){
            obj = col.GetComponent<Collider>().gameObject;
            if (obj.tag == "Plant" || obj.tag == "Terrain")
            {
                surface.Remove(obj);
                Destroy(obj);
            }
        }
        //Turn off colliders
        TerrainColliders(planetFeatures, false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Controls.trackedObject != null && Controls.trackedObject.tag == "Planet"){
            /* if (Input.GetKey(KeyCode.G)){
                ghostBuild = true;
            } */
            if(selectedBuilding != "None"){
                ghostBuild = true;
                /* //Turn on colliders
                foreach (var obj in Controls.trackedObject.GetComponent<GeneratePlanetFeatures>().surfaceObjects)
                {
                    obj.GetComponent<Collider>().enabled = true;
                } */
            }
        }

        //Construction Management
        if(ghostBuild){
            Ray ray = Controls.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)){
                var obj = hit.collider.gameObject;
                if(Controls.trackedObject == obj){
                    if(!ghost){
                        ghost = Instantiate(ConstructionGhost, hit.point, new Quaternion(0,0,0,0));
                    } else if(ghost.activeInHierarchy == true) {
                        var flashing = ghost.GetComponent<Renderer>().sharedMaterial;
                        switch(selectedBuilding){
                            case "HOUSE":
                                ghost.GetComponent<MeshFilter>().sharedMesh = HouseBuilding.GetComponent<MeshFilter>().sharedMesh;
                                ghost.GetComponent<Renderer>().materials = HouseBuilding.GetComponent<Renderer>().sharedMaterials;
                                break;
                            case "SOLAR ARRAY":
                                ghost.GetComponent<MeshFilter>().sharedMesh = SolarArray.GetComponent<MeshFilter>().sharedMesh;
                                ghost.GetComponent<Renderer>().materials = SolarArray.GetComponent<Renderer>().sharedMaterials;
                                break;
                            case "MINING RIG":
                                ghost.GetComponent<MeshFilter>().sharedMesh = MiningRig.GetComponent<MeshFilter>().sharedMesh;
                                ghost.GetComponent<Renderer>().materials = MiningRig.GetComponent<Renderer>().sharedMaterials;
                                break;
                            case "SPACECRAFT":
                                ghost.GetComponent<MeshFilter>().sharedMesh = Spacecraft.GetComponent<MeshFilter>().sharedMesh;
                                ghost.GetComponent<Renderer>().materials = Spacecraft.GetComponent<Renderer>().sharedMaterials;
                                break;
                        }
                        Material[] flashingarray = new Material[ghost.GetComponent<Renderer>().sharedMaterials.Length];
                        for (int i = 0; i < flashingarray.Length; i++)
                        {
                            flashingarray[i] = flashing;
                        }
                        ghost.GetComponent<Renderer>().sharedMaterials = flashingarray;
                        ghost.transform.position = hit.point;

                        ghost.transform.LookAt(hit.transform.position);
                        ghost.transform.Rotate(-90, 0, 0);

                        ghost.transform.rotation = ghost.transform.rotation * Quaternion.Euler(0, orientation, 0);
                        
                        //Orient object tangential to planet
                        //Vector3 resultUp = ghost.transform.position - hit.transform.position;
                        //ghost.transform.up = resultUp; 

                        //Change rotation in relation to forward vector
                        //Quaternion currentOrientation = ghost.transform.localRotation; 
                        //Quaternion turnRotation = Quaternion.Euler(0, ghost.transform.localRotation.y+orientation, 0); 
                        //ghost.transform.localRotation = currentOrientation * turnRotation; 
                        

                        //Change orientation using keys
                        if(Input.GetKey(KeyCode.LeftBracket)){
                            orientation += 5;
                        }
                        if(Input.GetKey(KeyCode.RightBracket)){
                            orientation -= 5;
                        }

                        //ghost.transform.LookAt(hit.transform.position);
                        //ghost.transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
                    } else {
                        ghost.SetActive(true);
                        canBuild = true;
                    }
                } else {
                    if(ghost != null){
                        //When raycast is lost turn off ghost (i.e mouse over building)
                        ghost.SetActive(false);
                    }
                }
            } else {
                if(ghost != null){
                    ghost.SetActive(false);
                    canBuild = false;
                }
            }
            if (Input.GetMouseButtonDown(0)){ 
                if(ghost.activeInHierarchy == false){
                    cancelSelection();
                }
                if(canBuild){
                    ghostBuild = false;
                    ghost.SetActive(false);
                    //Hard set orientation before placement
                    ghost.transform.LookAt(hit.transform.position);
                    ghost.transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
                    //Instantiate actual 
                    Controls.UI.AudioPlaceHeavy();
                    if(Controls.trackedObject && ghost){
                        PlaceBuilding(selectedBuilding, Controls.trackedObject, new Vector2(0,0), ghost, orientation);
                    }
                    cancelSelection();
                } else {
                    Controls.UI.SetNotification("Obstruction preventing construction.");
                    Controls.UI.AudioError();
                }
            }
            //Cancel on right click
            if (Input.GetMouseButtonDown(1)){ 
                if(canBuild){
                    cancelSelection();
                }
            }
        }
    }
}

