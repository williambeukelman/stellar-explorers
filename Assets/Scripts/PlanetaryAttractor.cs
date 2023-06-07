using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlanetaryAttractor : MonoBehaviour
{
    private GameObject[] planets;
    private GameObject Camera;
    private float orbitSpeed;
    private GameObject line;
    private bool orbiter = true;
    private Transform nearestPlanet;
    // Start is called before the first frame update
    void Start()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet");
        nearestPlanet = planets.OrderBy(planet => (transform.position - planet.transform.position).sqrMagnitude).First().transform;
        //Get Camera
        Camera = AssetManager.manager.Camera;
        orbitSpeed = Random.Range(1.0f,10.0f);
        line = new GameObject { name = "circle_"+name };
        line.DrawCircle((new Vector3(0,0,0) - transform.position).magnitude, .5f, new Color[]{new Color(1, 0, 1, 0.05f)});
    }

    // Update is called once per frame
    void Update()
    {
        var renderer = GetComponent<Renderer>();
        var camdistance = transform.position - Camera.transform.position;
        if (camdistance.magnitude > 150){
            //Turn off/on renderer
            renderer.enabled = false;
        } else {
            renderer.enabled = true;
        }
        //transform.RotateAround(new Vector3(0,0,0), Vector3.up, orbitSpeed * Time.deltaTime);
        
        //GetComponent<Rigidbody>().AddForce((new Vector3(0,0,0) - transform.position).normalized * orbitSpeed * Time.deltaTime);
        //Draw a circle around object
        if(!orbiter){
            Destroy(line);
        }
    }

    void FixedUpdate() {
        if(Mathf.RoundToInt(Time.realtimeSinceStartup)%2==0){
            nearestPlanet = planets.OrderBy(planet => (transform.position - planet.transform.position).sqrMagnitude).First().transform;
        }
        /* if(Mathf.RoundToInt(Time.realtimeSinceStartup)%30==0 && Random.Range(0, 2) == 1){
            orbiter = false;
            transform.LookAt(nearestPlanet);
            GetComponent<Rigidbody>().AddForce(Vector3.forward * orbitSpeed * 2 * Time.deltaTime);
        } */
        if(orbiter){
            transform.RotateAround(new Vector3(0,0,0), Vector3.up, orbitSpeed * Time.deltaTime);
        } else {
            var distance = nearestPlanet.position - transform.position;
            GetComponent<Rigidbody>().AddForce(distance.normalized * (1/distance.magnitude) * Time.deltaTime);
        }
        //transform.LookAt(new Vector3(0,0,0));

        //Destroy if it gets too far from sun
        if((new Vector3(0,0,0) - transform.position).magnitude > 800){
            Destroy(this);
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Planet" || collision.gameObject.tag == "Sun")
        {
            Destroy(this);
        } else {
            orbiter = false;
            //transform.LookAt(nearestPlanet);
            GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)) * 10, ForceMode.Impulse);
        }
    }

}
