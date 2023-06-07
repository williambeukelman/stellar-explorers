using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivManager : MonoBehaviour
{
    public int Population;
    public int MaxPopulation;
    public float OreValue;
    public int MiningProduction;
    public int EnergyProduction;
    public int EnergyUse;
    BuildManager buildManager;

    public IEnumerator MineResources(int oreQuantity)
    {
        while(true){
            yield return new WaitForSeconds(60);
            OreValue += oreQuantity;
            print("Mined: "+oreQuantity);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        buildManager = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildManager>();
        //StartCoroutine(MineResources());
    }

    public void AddPerson(Vector2 point){
        if(MaxPopulation-Population > 0){
            buildManager.PlaceUnit("ASTRONAUT", transform.gameObject, point);
            Population++;
        } else {
            AssetManager.manager.UI.SetNotification("No room for more people.");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //This can be moved to start and use a trigger on new buildings to rerun.
        /* var buildings = GetComponent<GeneratePlanetFeatures>().surfaceObjects;
        foreach (var building in buildings)
        {
            if(building.name.Contains("SolarArray")){
                EnergyProduction += 5;
            }
        } */
    }
}
