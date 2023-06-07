using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Navigation : MonoBehaviour
{
    public GameObject target;
    public GameObject parentPlanet;
    private BuildManager buildManager;
    private UserControls Controls;
    private InterfaceScripts UI;
    public float parentSpeed;
    public float targetSpeed;
    public float parentDist;
    public float targetDist;
    public float rocketSpeed = 0;
    bool traveling = false;
    bool orienting = false;
    public float lerpSpeed = .25f;

    //Construction Stuff
    public GameObject rocketStage1;
    public GameObject rocketStage2;
    public GameObject rocketStage3;
    public GameObject rocketStage4;
    public GameObject rocket;
    public int buildPhase = 0;
    public ParticleSystem exp;
    public float buildModifer = 0;

    // Start is called before the first frame update
    void Start()
    {
        exp = GetComponent<ParticleSystem>();
        Controls = AssetManager.manager.Controls;
        UI = AssetManager.manager.UI;
        buildManager = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildManager>();
        //Construction start
        GetComponent<MeshFilter>().sharedMesh = rocketStage1.GetComponent<MeshFilter>().sharedMesh;
        GetComponent<Renderer>().materials = rocketStage1.GetComponent<Renderer>().sharedMaterials;
        StartCoroutine(BuildPhase(30));
    }

    public IEnumerator BuildPhase(int phaseTime=5){
        yield return new WaitForSeconds(phaseTime-buildModifer);
        GetComponent<MeshFilter>().sharedMesh = rocketStage2.GetComponent<MeshFilter>().sharedMesh;
        GetComponent<Renderer>().materials = rocketStage2.GetComponent<Renderer>().sharedMaterials;
        buildPhase = 1;
        yield return new WaitForSeconds(phaseTime-buildModifer);
        GetComponent<MeshFilter>().sharedMesh = rocketStage3.GetComponent<MeshFilter>().sharedMesh;
        GetComponent<Renderer>().materials = rocketStage3.GetComponent<Renderer>().sharedMaterials;
        buildPhase = 2;
        yield return new WaitForSeconds(phaseTime-buildModifer);
        GetComponent<MeshFilter>().sharedMesh = rocketStage3.GetComponent<MeshFilter>().sharedMesh;
        GetComponent<Renderer>().materials = rocketStage3.GetComponent<Renderer>().sharedMaterials;
        buildPhase = 3;
        yield return new WaitForSeconds(phaseTime-buildModifer);
        GetComponent<MeshFilter>().sharedMesh = rocketStage4.GetComponent<MeshFilter>().sharedMesh;
        GetComponent<Renderer>().materials = rocketStage4.GetComponent<Renderer>().sharedMaterials;
        buildPhase = 4;
        //Get one astronaut
        GeneratePlanetFeatures features = transform.parent.gameObject.GetComponent<GeneratePlanetFeatures>();
        GameObject person = features.surfaceObjects.Where(e => e.name.Contains("Astronaut")).First();
        person.GetComponent<Movement>().BoardShip(gameObject.GetComponent<surfaceObject>().polarCoords);
        yield return new WaitForSeconds(phaseTime-buildModifer);
        GetComponent<MeshFilter>().sharedMesh = rocket.GetComponent<MeshFilter>().sharedMesh;
        GetComponent<Renderer>().materials = rocket.GetComponent<Renderer>().sharedMaterials;
        buildPhase = 5;
    }

    public IEnumerator BurnFuel()
    {
        yield return new WaitForSeconds(30);
        UI.SetNotification("Your rocket is running low on fuel", 10);
        exp.Stop();
        yield return new WaitForSeconds(30);
        Destroy(gameObject);
    }
    public IEnumerator OrientSpacecraft(GameObject target)
    {
        exp.Play();
        parentPlanet = transform.parent.gameObject;
        parentPlanet.GetComponent<CivManager>().Population -= 1;
        yield return new WaitForSeconds(2);
        //Launch
        GetComponent<AudioSource>().Play();
        GetComponent<Rigidbody>().AddForce(Vector3.up * .8f, ForceMode.Impulse);
        yield return new WaitForSeconds(1);
        orienting = true;
        yield return new WaitForSeconds(2);
        traveling = true;
        yield return new WaitForSeconds(2);
        transform.parent = null;
        yield return null;
    }

    public void GuidedFlight(string planetName){
        
        //Target first planet
        target = GameObject.FindGameObjectsWithTag("Planet").Where(e => e.name.Contains(planetName)).First();
        StartCoroutine(OrientSpacecraft(target));
        StartCoroutine(BurnFuel());
        //traveling = true;
        //transform.parent = null;
        name = "Rocket";
        
    }

    void OnTriggerEnter(Collider collision){
        if(traveling && collision.gameObject.CompareTag("Planet") && collision.gameObject != parentPlanet){
            //Landed on planet
            GameObject planet = collision.gameObject;
            transform.LookAt(planet.transform.position);
            transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
            if(planet.GetComponent<GeneratePlanetFeatures>().Biome != "Barren"){
                CivManager civ = planet.AddComponent<CivManager>();
                //Instantiate actual 
                Controls.UI.AudioPlaceHeavy();
                buildManager.PlaceBuilding("COLONY CENTER", planet, new Vector2(0,0));
                Vector2 colonyCenterPos = planet.GetComponent<GeneratePlanetFeatures>().surfaceObjects.Where(b => b.name.Contains("ColonyCenter")).First().GetComponent<surfaceObject>().polarCoords;
                buildManager.PlaceUnit("ASTRONAUT", planet, colonyCenterPos + new Vector2(-5, 5));
                civ.Population = 1;
                civ.MaxPopulation = 1;
                civ.OreValue = 15;
            } else {
                UI.SetNotification("Cannot survive on barren world", 10);
            }
            Controls.trackedObject = collision.gameObject;
            //Destroy spaceship
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(orienting){
            /* parentSpeed = parentPlanet.GetComponent<Orbit>().orbitSpeed;
            targetSpeed = target.GetComponent<Orbit>().orbitSpeed;
            parentDist = parentPlanet.GetComponent<Orbit>().planetDistance;
            targetDist = target.GetComponent<Orbit>().planetDistance; */

            //Vector3 resultUp = transform.position - target.transform.position;
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(resultUp), Time.deltaTime);

            //Orient transform up towards target
            

            //Debug.DrawRay(transform.position, transform.up * resultUp.magnitude, Color.blue);

            //Vector3 newDirection = Vector3.RotateTowards(transform.forward, resultUp, lerpSpeed * Time.deltaTime, 0.0f);
            //transform.rotation = Quaternion.LookRotation(newDirection);
            //transform.up = -transform.forward;

            /* Quaternion rotation = Quaternion.LookRotation(target.transform.position - transform.position, new Vector3(x,y,x));
            //Quaternion rotation = Quaternion.LookRotation(new Vector3(0,0,0), transform.forward);
            Quaternion current = transform.localRotation;
            transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime * .33f);
            transform.localRotation = Quaternion.Slerp(current, Quaternion.AngleAxis(-90, new Vector3(1,0,0)), Time.deltaTime * .33f); */

            //obj.transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
            //transform.localRotation = Quaternion.Slerp(current, new Quaternion(0, 0, 1, 1), Time.deltaTime * .33f);
            //transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.AngleAxis(x, transform.forward), Time.deltaTime * 1);

            //transform.RotateAround(new Vector3(0,0,0), Vector3.up, parentSpeed * Time.deltaTime);

            //Vector3 travelPoint = -transform.forward * (parentDist - targetDist);
            //print(travelPoint);
            //travelPoint = new Vector3(travelPoint.x, 0, travelPoint.z);
            //Vector3 travelDirection = travelPoint - transform.position;
           // Vector3 targetDirection = target.transform.position - transform.position;
            //transform.LookAt(travelPoint);
            float boost = 0;
            if(!traveling){
                Vector3 resultUp = transform.position - target.transform.position;
                boost = resultUp.magnitude/50;
                //transform.up = -Vector3.Lerp(transform.up, transform.position - target.transform.position, Time.deltaTime);
                //print(transform.up);
                //transform.up = -resultUp; 

                Quaternion toRotation = Quaternion.FromToRotation(transform.up, -resultUp.normalized); // instead of LookRotation( )
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, lerpSpeed * Time.deltaTime); 
            }
            else{
                Vector3 resultUp = transform.position - target.transform.position;
                GetComponent<Rigidbody>().velocity = transform.up * (rocketSpeed+boost);
                transform.up = -resultUp; 
            }
            
            //GetComponent<Rigidbody>().AddForce(Vector3.up * .5f, ForceMode.Impulse);

            //transform.Translate(transform.forward);

            //x -= Time.deltaTime;
            //Slowly rotate around one axis
            //transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.AngleAxis(x, transform.forward), Time.deltaTime * 1);
            //transform.Translate(transform.up * Time.deltaTime);
            
            // Draw a ray pointing at our target in
            //Debug.DrawRay(transform.position, travelDirection.normalized * travelDirection.magnitude, Color.blue);
            //Debug.DrawRay(transform.position, targetDirection.normalized * targetDirection.magnitude, Color.red);

            
        }
    }
}
