using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeking : MonoBehaviour
{
    public GameObject target;
    public GameObject explosion;
    private BuildManager buildManager;
    UserControls Controls;
    InterfaceScripts UI;
    public float speed = 5;
    // Start is called before the first frame update
    void Start()
    {
        buildManager = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildManager>();
        //Get UserControls
        Controls = AssetManager.manager.Controls;
        UI = AssetManager.manager.UI;
    }

    void OnTriggerEnter(Collider collision){
        if(collision.gameObject.CompareTag("Planet")){
            //Hit planet
            GameObject planet = collision.gameObject;
            GeneratePlanetFeatures planetFeatures = planet.GetComponent<GeneratePlanetFeatures>();
            UI.SetNotification(string.Format("{0} was hit", planetFeatures.PlanetName));
            //transform.LookAt(planet.transform.position);
            //transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
            //Turn on colliders
            buildManager.TerrainColliders(planetFeatures, true);
            GameObject obj;
            var surface = planetFeatures.surfaceObjects;
            //Reactivate everything
            foreach (var o in surface){
                o.SetActive(true);
            }
            Collider[] colliders;
            colliders = Physics.OverlapBox(transform.position, new Vector3(8f, 8f, 8f));
            for (int i = 0; i < colliders.Length; i++)
            {
                obj = colliders[i].GetComponent<Collider>().gameObject;
                if (obj.tag != "Planet" && obj.tag != "Comet")
                {
                    surface.Remove(obj);
                    Destroy(obj);
                }
            }

            List<GameObject> toDestroy = new(){};
            foreach (var item in surface)
            {
                if(item.tag == "People" || item.name.Contains("Rocket")){
                    item.name = "Skeleton";
                    toDestroy.Add(item);
                }
            }
            toDestroy.ForEach(e => surface.Remove(e));
            toDestroy.ForEach(e => Destroy(e));
            //Destroy civ manager
            Destroy(planet.GetComponent<CivManager>());
            
            //Turn off colliders
            buildManager.TerrainColliders(planetFeatures, false);
            //Destroy comet
            //Trigger explosion
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().velocity = transform.up * speed;
        Vector3 resultUp = transform.position - target.transform.position;
        transform.up = -resultUp; 
    }
}
