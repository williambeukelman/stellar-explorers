using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class planet{
        public float orbitSpeed;
        public float rotationSpeed;
        public float planetDistance;
    }
public class GeneratePlanets : MonoBehaviour
{
    public GameObject planet;
    public GameObject asteroid;
    public GameObject comet;
    private GameObject newComet;
    private bool cometAppeared = false;
    private GameObject startingPlanet;
    UserControls Controls;
    BuildManager buildManager;
    public int numberOfPlanets = 1;
    public int planetMargin = 35;
    public int asteroidCount = 10;
    public int orbitSpeedMin = 1;
    public int orbitSpeedMax = 20;
    public int rotationSpeedMin = -5;
    public int rotationSpeedMax = 5;
    public int scaleMin = 5;
    public int scaleMax = 15;
    public bool LineUp = false;
    public int TerrainCountModifier = 4;
    private List<Vector3> planetLocations = new();
    private List<string> planetNames = new();
    private planet[] Planets;
    private GameObject[] PlanetInstances;
    private GameObject[] OrbitLines;
    private string[] nameList;
    private string[] biomeList;

    private (float, float, string, Color[]) ColorPlanet(string biome){
        var palette = new Color[4];
        float minTemp = 0;
        float maxTemp = 0;
        string atmosphereType = "";
        switch(biome){
            case "Coniferous":
            minTemp = 0;
            maxTemp = 30;
            atmosphereType = "O2, N, CO2";
            //#3F1300
            //#2E221A
            //#3A3735
            //#382A13
                palette[0] = new Color(0.25f, 0.07f, 0f);
                palette[1] = new Color(0.18f, 0.13f, 0.1f);
                palette[2] = new Color(0.23f, 0.22f, 0.21f);
                palette[3] = new Color(0.22f, 0.16f, 0.07f);
                break;
            case "Desert":
            minTemp = 15;
            maxTemp = 45;
            atmosphereType = "O2, N, CO2, SO2";
            //#CFA884
            //#796E4F
            //#D4ADA3
            //#BC6A72
                palette[0] = new Color(0.81f, 0.66f, 0.52f);
                palette[1] = new Color(0.47f, 0.43f, 0.31f);
                palette[2] = new Color(0.83f, 0.68f, 0.64f);
                palette[3] = new Color(0.74f, 0.42f, 0.45f);
                break;
            case "Deciduous":
            minTemp = 20;
            maxTemp = 30;
            atmosphereType = "O2, N, CO2";
            //#7E6A4F
            //#66410F
            //#505050
            //#3E4923
                palette[0] = new Color(0.49f, 0.42f, 0.31f);
                palette[1] = new Color(0.4f, 0.25f, 0.06f);
                palette[2] = new Color(0.31f, 0.31f, 0.31f);
                palette[3] = new Color(0.24f, 0.29f, 0.14f);
                break;
            case "Steppe":
            minTemp = 10;
            maxTemp = 25;
            atmosphereType = "N, CO2, O2";
            //#3E4923
            //#606F31
            //#505050
            //#9BCDD2
                palette[0] = new Color(0.24f, 0.29f, 0.14f);
                palette[1] = new Color(0.38f, 0.44f, 0.19f);
                palette[2] = new Color(0.31f, 0.31f, 0.31f);
                palette[3] = new Color(0.61f, 0.8f, 0.82f);
                break;
            case "Xenotype":
            minTemp = 10;
            maxTemp = 40;
            atmosphereType = "N, O3, NO2, O2, CO2";
            //#526E8E
            //#71518E
            //#8E5185
            //#D5DB5C
                palette[0] = new Color(0.32f, 0.43f, 0.56f);
                palette[1] = new Color(0.44f, 0.32f, 0.56f);
                palette[2] = new Color(0.56f, 0.32f, 0.52f);
                palette[3] = new Color(0.84f, 0.86f, 0.36f);
                break;
            case "Barren":
            minTemp = -40;
            maxTemp = 50;
            atmosphereType = "H2SO4, H2, CO2, CO";
            //#21220A
            //#A5FFBA
            //#DB7B7D
            //#5B5EFF
                palette[0] = new Color(0.13f, 0.13f, 0.04f);
                palette[1] = new Color(0.65f, 1f, 0.73f);
                palette[2] = new Color(0.86f, 0.48f, 0.49f);
                palette[3] = new Color(0.36f, 0.37f, 1f);
                break;
        }
        return (minTemp, maxTemp, atmosphereType, palette);
    }

    private void CreatePlanet(string zone, int index){
        var startRange = 75;
        Vector3 location;
        if(planetLocations.Count < 1){
            location = new Vector3(startRange, 0, 0);
            planetLocations.Add(location);
        } else{
            location = planetLocations.Last() + new Vector3(planetMargin,0,0);
            planetLocations.Add(location);
        }
        //Create Planets
        var newPlanet = Instantiate(planet, location, Quaternion.identity);
        //var Orbit = newPlanet.GetComponent<Orbit>();
        var Trans = newPlanet.GetComponent<Transform>();
        //Orbit.orbitSpeed = Random.Range(orbitSpeedMin, orbitSpeedMax);
        //Orbit.rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);
        //Place in random part of orbit
        if(!LineUp){
            Trans.RotateAround(new Vector3(0, 0, 0), Vector3.up, Random.Range(0, 360));
        }
        //Orbit.planet_tilt = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        var scaleFactor = Random.Range(scaleMin, scaleMax);
        Trans.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        var features = newPlanet.AddComponent<GeneratePlanetFeatures>();
        //Change biome based on distance
        var biome = "Barren";
        //"Desert", "Deciduous", "Coniferous", "Steppe", "Xenotype"
        switch(zone){
            case("NEAR"):
                biome = biomeList[Random.Range(1, 3)];
                break;
            case("GOLD"):
                biome = biomeList[Random.Range(2,4)];
                break;
            case("FAR"):
                biome = biomeList[Random.Range(4,6)];
                break;
            case("EXO"):
                biome = "Barren";
                break;
        }
        //Choose and set colors
        var (minTemp, maxTemp, atmosphereType, palette) = ColorPlanet(biome);
        Color c = palette[Random.Range(0,4)];
        float adjustment = Random.Range(-0.05f, 0.05f);
        newPlanet.GetComponent<Renderer>().material.SetColor("_Hue", new Color(c.r+adjustment, c.g+adjustment, c.b+adjustment));
        //Set planet features
        var PlanetName = nameList[Random.Range(0,62)];
        int retry = 0;
        while(planetNames.Contains(PlanetName) && retry < 63){
            PlanetName = nameList[Random.Range(0,62)];
            retry++;
        }
        planetNames.Add(PlanetName);
        features.PlanetName = PlanetName;
        features.Biome = biome;
        features.minTemp = minTemp;
        features.maxTemp = maxTemp;
        features.atmosphereType = atmosphereType;
        newPlanet.name = string.Format("{0} - {1}", PlanetName, biome);
        Planets[index] = new planet{
            orbitSpeed = Random.Range(orbitSpeedMin, orbitSpeedMax),
            rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax)
        };
        PlanetInstances[index] = newPlanet;
        OrbitLines[index] = new GameObject { name = "circle_"+PlanetName };
        float planetDistance = (newPlanet.transform.position - new Vector3(0,0,0)).magnitude;
        OrbitLines[index].DrawCircle(planetDistance, .5f);
    }

    IEnumerator BuildCiv(GameObject startingPlanet)
    {
        //print(startingPlanet.GetComponent<GeneratePlanetFeatures>().PlanetName);
        var civ = startingPlanet.AddComponent<CivManager>();
        yield return new WaitUntil(() => startingPlanet.GetComponent<GeneratePlanetFeatures>().surfaceObjects != null);
        Vector2 startingLocation = new(300, 400);
        float BuildMargin = 10 + Mathf.Abs((18-startingPlanet.transform.localScale.y)*1.67f);
        buildManager.PlaceBuilding("COLONY CENTER", startingPlanet, startingLocation);
        buildManager.PlaceBuilding("HOUSE", startingPlanet, startingLocation + new Vector2(BuildMargin,0));
        buildManager.PlaceBuilding("HOUSE", startingPlanet, startingLocation + new Vector2(BuildMargin,BuildMargin));
        buildManager.PlaceBuilding("SOLAR ARRAY", startingPlanet, startingLocation + new Vector2(0,BuildMargin));
        buildManager.PlaceBuilding("SOLAR ARRAY", startingPlanet, startingLocation + new Vector2(0,-BuildMargin));
        //Place Astronauts
        for (int i = 0; i < 5; i++)
        {
            buildManager.PlaceUnit("ASTRONAUT", startingPlanet, startingLocation + new Vector2(-BuildMargin/2, i-2*5));
        }
        civ.Population = 5;
        civ.MaxPopulation = 6;
        civ.OreValue = 15;
        //Align buildings to planet surface
        GeneratePlanetFeatures features = startingPlanet.GetComponent<GeneratePlanetFeatures>();
        features.PolarPlacement(features.surfaceObjects);
    }

    // Start is called before the first frame update
    void Start()
    {
        Planets = new planet[numberOfPlanets];
        PlanetInstances = new GameObject[numberOfPlanets];
        OrbitLines = new GameObject[numberOfPlanets];
        //Get Managers
        Controls = AssetManager.manager.Controls;
        buildManager = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildManager>();

        nameList = new string[]{ "Orayous", "Neptinol", "Poptomas", "Oxol", "Dearis", "Aste", "Astra", "Orphenious", "Morphfren", "Kelck", "Phanith", "Phlome", "Fiyl", "Sphenrix", "Rereni", "Warenk", "Eriyt", "Ede", "Odo", "Adrt", "Ase", "Yte", "Oob", "Onyth", "Malth", "Mon", "Hornous", "Horth", "Homia", "Phoss", "Phosno", "Pheb", "Redtra", "Yredtra", "Yrith", "Malca", "Jutra", "Jtr", "Ptr", "Atr", "Ontopontalis", "Luetrantri", "Kya", "Kyyala", "Olawa", "Qew", "Brineda", "Brogh", "Brouwe", "Ghon", "Gho", "Gumpho", "Ortalis", "Orteilli", "Pinnusas", "Vornioa", "Vackrin", "Vkrin", "Ninith", "Capitulonous", "Citiaul", "Romhn", "Ronannea" };
        biomeList = new string[]{ "Barren", "Desert", "Deciduous", "Coniferous", "Steppe", "Xenotype" };

        //Remove placeholder planet on start
        GameObject.FindGameObjectWithTag("Planet").SetActive(false);

        //Generate Asteroids
        for (int i = 0; i < asteroidCount; i++)
        {
            var point = Random.insideUnitCircle.normalized * 150;
            var location = new Vector3(point.x, 0, point.y);
            var Asteroid = Instantiate(asteroid, transform, true);
            Asteroid.transform.SetPositionAndRotation(location, Quaternion.identity);
        }

        //Generate Planets
        //MINX = 75
        //CLOSE PLANETS = 75 to 100 BARREN, DESERT
        //GOLDYLOCKS = 150 to 200 DECIDUOUS, CONIFEROUS
        //FAR PLANETS = 250 to 300, XENO, STEPPE
        //EXO PLANETS = 300 to 400 BARREN
        //MAXX = 400
        int nearPlanets = Mathf.RoundToInt(numberOfPlanets*0.1f);
        int midPlanets = Mathf.RoundToInt(numberOfPlanets*0.6f);
        int farPlanets = Mathf.RoundToInt(numberOfPlanets*0.3f);
        farPlanets += numberOfPlanets-(nearPlanets+midPlanets+farPlanets);
        //numberOfPlanets = nearPlanets + midPlanets + farPlanets;
        int index = 0;
        for (int i = 0; i < nearPlanets; i++)
        {
            CreatePlanet("NEAR", index);
            index++;
        }
        for (int i = 0; i < midPlanets; i++)
        {
            CreatePlanet((new string[]{"GOLD", "GOLD", "FAR"})[Random.Range(0,3)], index);
            index++;
        }
        for (int i = 0; i < farPlanets; i++)
        {
            CreatePlanet("EXO", index);
            index++;
        }

        //Choose a starting planet
        //GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        startingPlanet = PlanetInstances[Random.Range(1,3)];
        Controls.trackedObject = startingPlanet;
        StartCoroutine(BuildCiv(startingPlanet));  

        //print("numberOfPlanets: "+numberOfPlanets+" Planets.Length "+Planets.Length);
    }

    void Update(){
        if(Controls.UI.TimeLeft < 10 && cometAppeared == false){
            //Create comet
            newComet = Instantiate(comet, new Vector3(0, 200, 0), Quaternion.identity);
            newComet.GetComponent<Seeking>().target = startingPlanet;
            cometAppeared = true;
        }
        
    }

    void FixedUpdate(){
        //Orbit Stuff
        for (int i = 0; i < Planets.Length; i++)
        {
            //float planetDistance = (PlanetInstances[i].transform.position - new Vector3(0,0,0)).magnitude;
            //OrbitLines[i].DrawCircle(planetDistance, .5f);
            PlanetInstances[i].transform.RotateAround(new Vector3(0,0,0), Vector3.up, Planets[i].orbitSpeed * Time.deltaTime);
            PlanetInstances[i].transform.RotateAround(transform.position, Vector3.up, Planets[i].rotationSpeed * Time.deltaTime);
        }
    }
}
