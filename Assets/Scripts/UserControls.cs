using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserControls : MonoBehaviour
{
    public int StepSize = 1;
    public Vector3 HomePos = new Vector3(8, 50, -60);
    public Quaternion HomeRotation = Quaternion.Euler(45, 6, 4);
    public Vector3 cameraFollowingOffset = new Vector3(0, 8, -8);
    public GameObject trackedObject;
    public GameObject selectionObj;
    public GameObject selectedRocket;
    public float cameraOffsetScale = 2f;
    private float scrollValue;
    private List<GameObject> planets = new();
    public InterfaceScripts UI;
    public building[] Buildings;
    public bool dead = false;
 
    // Start is called before the first frame update
    void Start()
    {
        UI = AssetManager.manager.UI;
        //Disable build menu and resource menu
        //UI.globalView = true;
        //Load buildings into build menu
        var house_avatar = Resources.Load("GUI/house_img") as Texture2D;
        var solararray_avatar = Resources.Load("GUI/solar_img") as Texture2D;
        var miningrig_avatar = Resources.Load("GUI/miningRig_img") as Texture2D;
        var rocket_img = Resources.Load("GUI/rocket_img") as Texture2D;
        var colonycenter_avatar = Resources.Load("GUI/center_img") as Texture2D;
        Buildings = new building[]{
                    new building{
                                    Name="HOUSE", 
                                    Image=house_avatar,
                                    Description="Basic habitation for the people of your colony.",
                                    EnergyCost=5,
                                    EnergyProd=0,
                                    OreProd=0,
                                    MaterialCost=2
                                },
                    new building{
                                    Name="SOLAR ARRAY", 
                                    Image=solararray_avatar,
                                    Description="Used to produce power for buildings.",
                                    EnergyCost=0,
                                    EnergyProd=10,
                                    OreProd=0,
                                    MaterialCost=5
                                },
                    new building{
                                    Name="MINING RIG", 
                                    Image=miningrig_avatar,
                                    Description="Gradually produces raw materials.",
                                    EnergyCost=10,
                                    EnergyProd=0,
                                    OreProd=5,
                                    MaterialCost=10
                                },
                    new building{
                                    Name="COLONY CENTER", 
                                    Image=colonycenter_avatar,
                                    Description="The organizational command center for your colony.",
                                    EnergyCost=0,
                                    EnergyProd=0,
                                    OreProd=0,
                                    MaterialCost=50
                                },
                    new building{
                                    Name="SPACECRAFT", 
                                    Image=rocket_img,
                                    Description="Expand your colony by traveling to other worlds. Does not come with lasers.",
                                    EnergyCost=0,
                                    EnergyProd=0,
                                    OreProd=0,
                                    MaterialCost=30
                                }
                    };
        UI.SetBuildings(Buildings);
        //Get the first planets for numbers
        planets.AddRange(GameObject.FindGameObjectsWithTag("Planet"));
        //Sort Planets based on distance from sun
        planets.Sort( 
            (planet1, planet2) => 
            (planet1.transform.position - new Vector3(0,0,0)).magnitude.CompareTo(
            (planet2.transform.position - new Vector3(0,0,0)).magnitude) 
        );
        List<string> planet_names = new List<string>{};
        planets.ForEach(e => planet_names.Add(e.GetComponent<GeneratePlanetFeatures>().PlanetName));
        //print(UI.PlanetTargets.choices);
        UI.PlanetTargets.choices = planet_names.ToArray();

    }

    void OnGUI(){
        Event e = Event.current;
        if(e.type == EventType.ScrollWheel){
            scrollValue = Event.current.delta.y;
        }
        else if (e.type == EventType.KeyDown)
        {
            int k = (int)(e.keyCode - KeyCode.Alpha0);
            if (k >= 0 && k <= 9)
            {
                CyclePlanet(k);
            }
        }
    }

    public void SetPlanetInfo(GameObject tracked){
        var planet = tracked.GetComponent<GeneratePlanetFeatures>();
        var Cards = new card[]{
            new card{Label="BIOME", Value=planet.Biome},
            new card{Label="TEMP", Value=string.Format("{0} to {1} C", planet.minTemp, planet.maxTemp)},
            new card{Label="ATMOSPHERE", Value=planet.atmosphereType},
            new card{Label="ATMOSPHERE QUALITY", Value=string.Format("{0} %", Mathf.Round(planet.atmosphereQuality*100))}
            };
        UI.SetInfo(planet.PlanetName, Cards);
    }

    private void CyclePlanet(int k){
        if(planets.Count >= k){
            trackedObject = planets.ElementAt(k-1);
            SetPlanetInfo(trackedObject);
        }        
    }

    // Update is called once per frame
    Vector2 orbitAngles = new Vector2(45f, 0f);
    void LateUpdate()
    {
        //Global Zoom
        if(Input.mouseScrollDelta.y != 0){
            transform.position += new Vector3(1, -1, 1) * scrollValue * 5f;
        }
        if(Input.GetKey(KeyCode.Equals)){
            transform.position += new Vector3(1, -1, 1) * -.5f;
        }
        if(Input.GetKey(KeyCode.Minus)){
            transform.position += new Vector3(1, -1, 1) * .5f;
        }

        //Use tab to cycle planets
        if (Input.GetKeyUp(KeyCode.Tab) && planets.Count() >= 1){
            selectionObj = null;
            var ind = planets.IndexOf(trackedObject);
            if(ind+1 < planets.Count){
                trackedObject = planets.ElementAt(ind+1);
            } else {
                trackedObject = planets.ElementAt(0);
            }
            
        }

        //Bring up settings menu
        if(Input.GetKeyUp(KeyCode.Escape)){
            UI.OpenSettings();
        }

        if(!trackedObject){
            //Disable UI Elements
            UI.ClearInfo();
            UI.globalView = true;
            var Cards = new card[]{
                new card{Label="TEMP", Value=string.Format("{0} to {1} C", -270, -250)},
                new card{Label="", Value="Nothing interesting here."}
                };
            UI.SetInfo("Void", Cards);

            if (Input.GetKey(KeyCode.W)){
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + StepSize);
            }
            if (Input.GetKey(KeyCode.A)){
                transform.position = new Vector3(transform.position.x - StepSize, transform.position.y, transform.position.z);
            }
            if (Input.GetKey(KeyCode.S)){
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - StepSize);
            }
            if (Input.GetKey(KeyCode.D)){
                transform.position = new Vector3(transform.position.x + StepSize, transform.position.y, transform.position.z);
            }
            if (Input.GetKey(KeyCode.Home)){
                transform.position = HomePos;
                transform.rotation = HomeRotation;
            }
            if (Input.GetMouseButtonDown(0)){ 
                Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit selectedObject;
                if (Physics.Raycast(ray, out selectedObject)){
                    var obj = selectedObject.collider.gameObject;
                    trackedObject = obj;
                    //Do once actions
                    if(trackedObject.tag == "Planet"){
                        //Send planet details to UI
                        SetPlanetInfo(trackedObject);
                    }
                }
            }
        } else{
            //Check it is a planet
            if(trackedObject.tag == "Planet"){
                if(selectionObj == null){
                    //If nothing is selected assume planet info
                    SetPlanetInfo(trackedObject);
                }
                //Enable UI Elements
                UI.globalView = false;
                //Check if you can build there
                var civ = trackedObject.GetComponent<CivManager>();
                if(civ == null){
                    //UI.hasCiv = false;
                    UI.globalView = true;
                } else {
                    //UI.hasCiv = true;
                    UI.globalView = false;
                    var resources = new resource{
                        PopVal = civ.Population,
                        MaxPop = civ.MaxPopulation,
                        OreVal = civ.OreValue,
                        MineProdVal = civ.MiningProduction,
                        EnergyProdVal = civ.EnergyProduction,
                        EnergyUse = civ.EnergyUse
                    };
                    UI.SetResourceValues(resources);
                }
                //For debugging only, disable in full-game
                /* if(Input.GetKey(KeyCode.K)){
                    civ.OreValue += 1;
                } */
                var objectPos = trackedObject.GetComponent<Transform>().position;
                var scaleDistance = trackedObject.GetComponent<Transform>().localScale.x * cameraOffsetScale;
                if(Input.mouseScrollDelta.y != 0){
                    cameraOffsetScale -= scrollValue * 0.2f;
                }
                if(Input.GetKey(KeyCode.Equals)){
                    cameraOffsetScale -= 0.1f;
                }
                if(Input.GetKey(KeyCode.Minus)){
                    cameraOffsetScale += 0.1f;
                }

                cameraOffsetScale = (cameraOffsetScale < 0.7) ? 0.7f : (cameraOffsetScale > 5) ? 5f : cameraOffsetScale;

                //if(cameraOffsetScale < 0.7){ cameraOffsetScale = 0.7f; }
                //if(cameraOffsetScale > 5){ cameraOffsetScale = 5f; }

                if (Input.GetKey(KeyCode.W)){
                    orbitAngles.x += 90f * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.A)){
                    orbitAngles.y += 90f * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.S)){
                    orbitAngles.x -= 90f * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.D)){
                    orbitAngles.y -= 90f * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.Space)){
                    trackedObject = null;
                    transform.rotation = HomeRotation;
                } else { //Else block makes sur home rotation isn't overwritten during update
                    Quaternion lookRotation = Quaternion.Euler(orbitAngles);
                    Vector3 lookDirection = lookRotation * Vector3.forward;
                    Vector3 lookPosition = objectPos - lookDirection * scaleDistance;
                    transform.SetPositionAndRotation(lookPosition, lookRotation);
                }

                //Click on buildings for info
                if (Input.GetMouseButtonDown(0)){ 
                    Ray bray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                    RaycastHit bselectedObject;
                    if (Physics.Raycast(bray, out bselectedObject)){
                        selectionObj = bselectedObject.collider.gameObject;
                        if(selectionObj.tag == "Planet"){
                            //Set the info back to planet details
                            SetPlanetInfo(trackedObject);
                        } else if (selectionObj.tag == "Building"){
                            //Set info to building info
                            //print(selectionObj);
                            var state = selectionObj.GetComponent<BuildingState>();
                            var Cards = new card[]{
                                new card{Label=state.Desc, Value=""}
                                /* new card{Label="CONDITION", Value="Good"},
                                new card{Label="OCCUPANCY", Value="2/5"} */
                                };
                            string btn = "";
                            string title = state.Name;
                            if(state.Name == "COLONY CENTER"){
                                btn = "Colony Menu";
                            }
                            if(state.Name == "SPACECRAFT"){
                                selectedRocket = selectionObj;
                                switch(selectedRocket.GetComponent<Navigation>().buildPhase){
                                    case 0:
                                        title = "Phase 1";
                                        Cards = new card[]{
                                        new card{Label="Construction has just begun. Check back for updates", Value=""}
                                        };
                                        break;
                                    case 1:
                                        title = "Phase 2";
                                        Cards = new card[]{
                                        new card{Label="Construction is underway. Check back for updates", Value=""}
                                        };
                                        break;
                                    case 2:
                                        title = "Phase 3";
                                        Cards = new card[]{
                                        new card{Label="Progress looks good. Check back for updates", Value=""}
                                        };
                                        break;
                                    case 3:
                                        title = "Phase 4";
                                        Cards = new card[]{
                                        new card{Label="Construction is almost done. Check back for updates", Value=""}
                                        };
                                        break;
                                    case 4:
                                        title = "Phase 5";
                                        Cards = new card[]{
                                        new card{Label="Fueling of the rocket is underway. Check back for updates.", Value=""}
                                        };
                                        break;
                                    case 5:
                                        btn = "Launch Menu";
                                        title = state.Name;
                                        Cards = new card[]{
                                        new card{Label=state.Desc, Value=""}
                                        };
                                        break;
                                }
                                //Temp launch on click
                                //trackedObject.GetComponent<GeneratePlanetFeatures>().surfaceObjects.Remove(selectedRocket);
                                //selectedRocket.GetComponent<Navigation>().GuidedFlight();
                            }
                            UI.SetInfo(title, Cards, btn);
                        } else if (selectionObj.tag == "People"){
                            //print(selectionObj);
                            var Cards = new card[]{
                                new card{Label="DESCRIPTION", Value="An unamed clone."},
                                new card{Label="TASK", Value=selectionObj.GetComponent<Movement>().state}
                                };
                            UI.SetInfo("Clone", Cards);
                        }
                        if(!UI.InfoMenuOpen){
                            UI.OpenInfo();
                        }
                    }
                }

            } else{
                trackedObject = null;
            }

            //Check if you have explored other worlds
            if(!dead){
                var civCount = 0;
                foreach (var p in GameObject.FindGameObjectsWithTag("Planet"))
                {
                    if(p.GetComponent<CivManager>()){
                        civCount++;
                    }
                }
                if(civCount < 1 && GameObject.Find("Rocket") == null){
                    UI.Death();
                    dead = true;
                }
            }
            
            
        }
        //End of Update Code
    }

}
