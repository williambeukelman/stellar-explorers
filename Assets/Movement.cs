using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Movement : MonoBehaviour
{
    surfaceObject surface;
    List<GameObject> surfaceObjects;
    List<GameObject> surfaceObjectsMoving;
    GeneratePlanetFeatures planetFeatures;
    CivManager civ;
    Vector2 point;
    UserControls Controls;
    public Material flashing;
    public string state;
    private bool taskComplete;
    //public float adjustment = 90;
    // Start is called before the first frame update
    void Start()
    {
        //Get UserControls
        Controls = AssetManager.manager.Controls;

        surfaceObjects = transform.parent.gameObject.GetComponent<GeneratePlanetFeatures>().surfaceObjects;
        surfaceObjectsMoving = transform.parent.gameObject.GetComponent<GeneratePlanetFeatures>().surfaceObjectsMoving;
        civ = transform.parent.gameObject.GetComponent<CivManager>();
        state = "Wandering";
        surface = gameObject.GetComponent<surfaceObject>();
        point = new Vector2(surface.polarCoords.x + Random.Range(-50,50), surface.polarCoords.y + Random.Range(-50,50));
        StartCoroutine(Wander(new Vector2(10,10)));
    }

    public void BoardShip(Vector2 shipPos){
        state = "Other";
        StartCoroutine(Board(shipPos));
    }

    public void GiveCommand(string command){
        taskComplete = true;
        switch(command){
            case "Mine":
                state = "Mining";
                //StartCoroutine(Mine());
                break;
            case "Clone":
                state = "Cloning";
                //StartCoroutine(Clone());
                break;
            case "Build":
                state = "Building";
                break;
            case "Wander":
                state = "Wandering";
                //StartCoroutine(Wander(new Vector2(10,10)));
                break;
        }
    }

    public IEnumerator Board(Vector2 ship){
        point = ship;
        yield return new WaitUntil(() => (surface.polarCoords - point).magnitude <= 1);
        surfaceObjects.Remove(gameObject);
        surfaceObjectsMoving.Remove(gameObject);
        Destroy(gameObject);
    }

    public IEnumerator Mine(){
        taskComplete = false;
        //Go to nearest rock
        List<GameObject> rocks = surfaceObjects.Where(e => e.name.Contains("Stone")).ToList();
        if(rocks.Count > 0){
            GameObject nearestRock = rocks.OrderBy(rock => (surface.polarCoords - rock.GetComponent<surfaceObject>().polarCoords).sqrMagnitude).First();
            point = nearestRock.GetComponent<surfaceObject>().polarCoords + new Vector2(Random.Range(-2,2), Random.Range(-2,2));
            yield return new WaitUntil(() => (surface.polarCoords - point).magnitude <= 3);
            int PeopleWorking = 0;
            Collider[] colliders;
            colliders = Physics.OverlapBox(transform.position, new Vector3(.75f, .75f, .75f));
            GameObject obj;
            foreach (var col in colliders){
                obj = col.GetComponent<Collider>().gameObject;
                if (obj.tag == "People")
                {
                    PeopleWorking += 1;
                }
            }
            //Color rock
            Material[] flashingarray = new Material[nearestRock.GetComponent<Renderer>().sharedMaterials.Length];
            for (int i = 0; i < flashingarray.Length; i++)
            {
                flashingarray[i] = flashing;
            }
            nearestRock.GetComponent<Renderer>().sharedMaterials = flashingarray;
            yield return new WaitForSeconds(60/PeopleWorking);
            civ.OreValue += 1/PeopleWorking;
            nearestRock.SetActive(false);
            surfaceObjects.Remove(nearestRock);
        } else {
            Controls.UI.SetNotification("No ore available.");
            state = "Wandering";
            taskComplete = true;
        }
        taskComplete = true;
        yield return null;
    }

    public IEnumerator Build(){
        taskComplete = false;
        //Go to Rocket
        var rockets = surfaceObjects.Where(e => e.name.Contains("Rocket")).ToList();
        if(rockets.Count > 0){
            GameObject Rocket = rockets.First();
            point = Rocket.GetComponent<surfaceObject>().polarCoords + new Vector2(Random.Range(-3,3),Random.Range(-3,3));
            yield return new WaitUntil(() => (surface.polarCoords - point).magnitude <= 5);
            int PeopleWorking = 0;
            Collider[] colliders;
            colliders = Physics.OverlapBox(transform.position, new Vector3(.75f, .75f, .75f));
            GameObject obj;
            foreach (var col in colliders){
                obj = col.GetComponent<Collider>().gameObject;
                if (obj.tag == "People")
                {
                    PeopleWorking += 1;
                }
            }
            Rocket.GetComponent<Navigation>().buildModifer = PeopleWorking;
            yield return new WaitUntil(() => Rocket.GetComponent<Navigation>().buildPhase == 4);
            state = "Wandering";
            taskComplete = true;
        } else {
            Controls.UI.SetNotification("No rocket to build.");
            state = "Wandering";
            taskComplete = true;
        }
        taskComplete = true;
        yield return null;
    }

    public IEnumerator Clone(){
        taskComplete = false;
        //Go to Colony Center
        GameObject ColonyCenter = surfaceObjects.Where(e => e.name.Contains("ColonyCenter")).First();
        point = ColonyCenter.GetComponent<surfaceObject>().polarCoords + new Vector2(10, 0);
        int PeopleWorking = 0;
        Collider[] colliders;
        colliders = Physics.OverlapBox(transform.position, new Vector3(.75f, .75f, .75f));
        GameObject obj;
        foreach (var col in colliders){
            obj = col.GetComponent<Collider>().gameObject;
            if (obj.tag == "People")
            {
                PeopleWorking += 1;
            }
        }
        yield return new WaitForSeconds(60/PeopleWorking);
        civ.AddPerson(point);
        taskComplete = true;
        yield return null;
    }

    public IEnumerator Wander(Vector2 Variation){
        taskComplete = false;
        //Go to a random point 
        //StartCoroutine(MoveToPoint(point));
        yield return new WaitForSeconds(Random.Range(0,10));
        point = new Vector2(surface.polarCoords.x + Random.Range(-Variation.x, Variation.x), surface.polarCoords.y + Random.Range(-Variation.y,Variation.y));
        taskComplete = true;
        yield return null;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Building" || collision.gameObject.tag == "People")
        {
            point += (surface.polarCoords - collision.gameObject.GetComponent<surfaceObject>().polarCoords).normalized;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //surface.polarCoords;
        planetFeatures = transform.parent.GetComponent<GeneratePlanetFeatures>();
        var RotationX = -planetFeatures.transform.localEulerAngles.x;
        var RotationY = -planetFeatures.transform.localEulerAngles.y;
        Vector3 targetPoint = transform.PolarToCartesian(point.x+RotationX, point.y+RotationY, planetFeatures.transform.localScale.y/2)+planetFeatures.transform.position;

        Vector3 travelDirection = targetPoint - transform.position;

        Vector2 diff = (point - surface.polarCoords);

        if(diff.magnitude > 1){
            surface.orientation = Mathf.Atan2(diff.normalized.y, diff.normalized.x) * Mathf.Rad2Deg;
            surface.polarCoords += diff.normalized * .15f;
        }

        if(taskComplete){
            switch(state){
            case "Mining":
                StartCoroutine(Mine());
                break;
            case "Cloning":
                StartCoroutine(Clone());
                break;
            case "Building":
                StartCoroutine(Build());
                break;
            case "Wandering":
                StartCoroutine(Wander(new Vector2(10,10)));
                break;
            case "Other":
                break;
        }
        }
        

    }

}
