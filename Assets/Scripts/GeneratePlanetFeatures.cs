using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class GeneratePlanetFeatures : MonoBehaviour
{
    public string PlanetName = "Unknown";
    public string Biome = "Barren";
    public float minTemp = 0;
    public float maxTemp = 0;
    public string atmosphereType = "SO4";
    public float atmosphereQuality = 0.0f;
    public int numPlants;
    //public bool isHabitable = false;
    public int objCount = 10;
    private GameObject atmosphereObj;
    [HideInInspector]
    public List<GameObject> surfaceObjects;
    public List<GameObject> surfaceObjectsMoving;
    public float RenderRange = 150;
    //private Vector3 planet_tilt;
    //private float rotationSpeed;
    private float r;
    public float radius = 3;
    private GameObject rock;
    private GameObject tree;
    private GameObject treeInvert;
    private GameObject bush;
    private GameObject stone;
    private GameObject atmosphere;
    private GameObject Camera;

    //Update variables
    private Renderer objRenderer;
    private Vector2 polarsCoords;
    private float RotationX;
    private float RotationY;
    private Vector3 coords;
    private Vector3 position;
    private Vector3 resultUp;
    private surfaceObject surf;
    float orientation;
    Quaternion currentOrientation;
    Quaternion turnRotation;

    //Collections
    private string[] stones;
    private string[] rocks;
    private int TerrainCountModifier = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Get Camera
        Camera = AssetManager.manager.Camera;

        //Load Resources
        rock = AssetManager.manager.rock;
        tree = AssetManager.manager.tree;
        treeInvert = AssetManager.manager.treeInvert;
        bush = AssetManager.manager.bush;
        stone = AssetManager.manager.stone;
        atmosphere = AssetManager.manager.atmosphere;

        //Instatiate Atmosphere
        atmosphereObj = Instantiate(atmosphere, transform, true);
        atmosphereObj.transform.SetPositionAndRotation(transform.position, new Quaternion(0, 0, 0, 0));

        //Scale to planet
        atmosphereObj.transform.localScale = transform.localScale / 5;
        //atmosphereObj.transform.localScale = transform.InverseTransformVector(transform.localScale*3);
        /* print("Atmo Scale: "+atmosphereObj.transform.localScale);
        print("Planet Scale: "+transform.localScale);
        print("Inverse Scale: "+transform.InverseTransformVector(transform.localScale*3)); */
        //atmosphereObj.transform.localScale.Set(atmosphereObj.transform.localScale.y, atmosphereObj.transform.localScale.y, atmosphereObj.transform.localScale.z);

        //Get Planet Info
        //planet_tilt = GetComponent<Orbit>().planet_tilt;
        //rotationSpeed = GetComponent<Orbit>().rotationSpeed;
        r = transform.localScale.y / 2;
        var planet_color = GetComponent<Renderer>().material.GetColor("_Hue");

        //Variant Lists
        var pineTrees = new string[]{"tree_pineDefaultA", "tree_pineDefaultB", "tree_pineGroundA", "tree_pineGroundB", "tree_pineRoundA", "tree_pineRoundB",
            "tree_pineRoundC", "tree_pineRoundD", "tree_pineRoundE", "tree_pineRoundF", "tree_pineSmallA", "tree_pineSmallB", "tree_pineSmallC", "tree_pineSmallD",
            "tree_pineTallA", "tree_pineTallB", "tree_pineTallC", "tree_pineTallD"};
        //"tree_pineTallB_detailed", "tree_pineTallD_detailed", "tree_pineTallC_detailed", tree_pineTallA_detailed"
        var palmTrees = new string[] { "tree_palmBend", "tree_palmShort", "tree_palmTall" }; //"tree_palmDetailedShort" "tree_palmDetailedTall"
        var deciduousTrees = new string[]{"tree_blocks_dark", "tree_blocks", "tree_cone_dark", "tree_cone", "tree_default_dark", "tree_default", "tree_detailed_dark",
            "tree_detailed", "tree_fat_darkh", "tree_fat", "tree_oak_dark", "tree_oak"};
        var alienTrees = new string[]{"tree_plateau_dark", "tree_plateau", "tree_simple_dark", "tree_simple", "tree_small_dark", "tree_small",
            "tree_tall_dark", "tree_tall", "tree_thin_dark", "tree_thin"};
        var grasses = new string[] { "grass", "grass_large", "grass_leafs", "grass_leafsLarge" };
        var flowers = new string[] { "flower_redA", "flower_redB", "flower_redC" };
        var logs = new string[] { "log" };
        var mushrooms = new string[] { "mushroom_red", "mushroom_redGroup", "mushroom_redTall" };
        var bushes = new string[]{"plant_bushDetailed", "plant_bush", "plant_bushLarge", "plant_bushLargeTriangle", "plant_bushSmall",
            "plant_bushTriangle", "plant_flatShort", "plant_flatTall"};
        stones = new string[]{"stone_smallA", "stone_smallB", "stone_smallC", "stone_smallD", "stone_smallE", "stone_smallF", "stone_smallG", "stone_smallH", "stone_smallI"};
        rocks = new string[]{"rock_tallA", "rock_tallB", "rock_tallC", "rock_tallD", "rock_tallE", "rock_tallF", "rock_tallG", "rock_tallH", "rock_tallI", "rock_tallJ"};

        //Instatiate Groups (trees, bushes, stones, rocks, etc.)
        surfaceObjects = new List<GameObject>();
        surfaceObjectsMoving = new List<GameObject>();

        Material[] leafMats = AssetManager.manager.leafMats;
        Material[] xenoMats = AssetManager.manager.xenoMats;
        Material[] barkMats = AssetManager.manager.barkMats;
        Material[] flowerMats = AssetManager.manager.flowerMats;

        TerrainCountModifier = AssetManager.manager.PlanetGenerator.TerrainCountModifier;

        //Creates twice as many groups as scale, or 4 times the radius
        for (int i = 0; i < r * TerrainCountModifier; i++)
        {
            switch (Biome)
            {
                case "Coniferous":
                    GeneratePineForests(pineTrees, bushes, logs, leafMats, barkMats);
                    break;
                case "Desert":
                    GenerateDesertGroups(palmTrees, leafMats, barkMats);
                    break;
                case "Deciduous":
                    GenerateForests(deciduousTrees, bushes, logs, mushrooms, leafMats, barkMats);
                    break;
                case "Steppe":
                    GenerateGrasslands(grasses, bushes, flowers, leafMats, flowerMats);
                    break;
                case "Xenotype":
                    GenerateAlienForests(alienTrees, xenoMats);
                    break;
                case "Barren":
                    GenerateRockFeatures(planet_color);
                    break;
            }

        }

        numPlants = surfaceObjects.Where(e => e.tag == "Plant").ToList().Count;
        PolarPlacement(surfaceObjects);

    }

    public void AddSurfaceObject(GameObject spawn, Vector2 polar, float orientation = 0)
    {
        surfaceObject surf = spawn.AddComponent<surfaceObject>();
        surf.polarCoords = polar;
        surf.orientation = orientation;
        surfaceObjects.Add(spawn);
    }

    public void AddSurfaceObjectMoving(GameObject spawn)
    {
        surfaceObjectsMoving.Add(spawn);
    }

    GameObject Initialize(GameObject type, string[] variantList, Material[] mats = null)
    {
        var val = Random.Range(0, variantList.Length);
        if (variantList[val] == "tree_default"
        || variantList[val] == "tree_default_dark"
        || variantList[val] == "tree_blocks"
        || variantList[val] == "tree_blocks_dark"
        || variantList[val] == "tree_cone"
        || variantList[val] == "tree_cone_dark"
        || variantList[val] == "tree_cone_dark"
        || variantList[val] == "tree_pineDefaultB"
        || variantList[val] == "tree_pineRoundA"
        || variantList[val] == "tree_pineRoundB"
        || variantList[val] == "tree_pineTallA"
        )
        {
            type = treeInvert;
        }
        GameObject spawn;
        spawn = Instantiate(type, transform, true);
        spawn.transform.SetLocalPositionAndRotation(transform.position, new Quaternion(0, 0, 0, 0));
        spawn.GetComponent<MeshFilter>().sharedMesh = AssetManager.manager.MeshDirectory[variantList[val]]; //Resources.Load<Mesh>("Nature Assets/"+variantList[val]); 
        if (mats != null)
        {
            var reverse = new Material[] { mats[1], mats[0] };
            if (type == treeInvert || type == bush)
            {
                spawn.GetComponent<Renderer>().sharedMaterials = mats;
            }
            else
            {
                spawn.GetComponent<Renderer>().sharedMaterials = reverse;
            }

        }
        return spawn;
    }
    void GrassSpawner(int e, GameObject type, string[] collection, Material[] mats, Vector2 polar)
    {
        Vector2 variation = new Vector2(e * Random.Range(-10, 10), e * Random.Range(-10, 10));
        GameObject spawn = Initialize(type, collection);
        spawn.GetComponent<Renderer>().sharedMaterial = mats[1];
        AddSurfaceObject(spawn, polar + variation);
    }

    void Spawner(int e, GameObject type, string[] collection, Material[] mats, Vector2 polar, int Size, bool rotate = false)
    {
        Vector2 variation = Random.insideUnitCircle * Size * radius;
        GameObject spawn = Initialize(type, collection, mats);
        if (rotate)
        {
            spawn.transform.Rotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        }
        AddSurfaceObject(spawn, polar + variation);
    }

    GameObject StoneSpawner(int ForestSize, Vector2 polar, Vector2 variation, bool rockType=false){
        //Stones
        GameObject spawnStone;
        if(!rockType){
            spawnStone = Initialize(stone, stones);
        } else{
            spawnStone = Initialize(rock, rocks);
        }
        spawnStone.transform.SetPositionAndRotation(transform.position, new Quaternion(0, 0, 0, 0));
        spawnStone.transform.Rotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        AddSurfaceObject(spawnStone, polar + variation);
        return spawnStone;
    }
    void GenerateForests(string[] deciduousTrees, string[] bushes, string[] logs, string[] mushrooms, Material[] leafMats, Material[] barkMats)
    {
        var polar = new Vector2(Random.Range(20f, 330f), Random.Range(0f, 360f));
        var ForestSize = Random.Range(5, 10);
        Material[] mats = new Material[2];
        for (int e = 0; e < ForestSize; e++)
        {
            mats[0] = barkMats[Random.Range(0, barkMats.Length)];
            mats[1] = leafMats[Random.Range(0, leafMats.Length)];
            //Trees
            Spawner(e, tree, deciduousTrees, mats, polar, ForestSize);

            if (e % 3 == 0)
            {
                //Bushes
                Spawner(e, bush, bushes, mats, polar, ForestSize);
            }
            else if (e % 5 == 0)
            {
                //Logs
                Spawner(e, bush, logs, mats, polar, ForestSize, rotate: true);
            }
            else
            {
                Vector2 variation = Random.insideUnitCircle * ForestSize * radius;
                StoneSpawner(ForestSize, polar, variation);
            }
        }
    }

    void GenerateGrasslands(string[] grasses, string[] bushes, string[] flowers, Material[] leafMats, Material[] flowerMats)
    {
        var polar = new Vector2(Random.Range(20f, 330f), Random.Range(0f, 360f));
        var ForestSize = Random.Range(5, 10);
        Material[] mats = new Material[2];
        for (int e = 0; e < ForestSize; e++)
        {
            mats[0] = flowerMats[Random.Range(0, flowerMats.Length)];
            mats[1] = leafMats[Random.Range(0, leafMats.Length)];

            Vector2 variation;
            //Bushes
            GrassSpawner(e, bush, bushes, mats, polar);
            //Grasses
            GrassSpawner(e, bush, grasses, mats, polar);
            //Stones
            if (e % 5 == 0)
            {
                variation = new Vector2(e * Random.Range(-10, 10), e * Random.Range(-10, 10));
                StoneSpawner(ForestSize, polar, variation);
            }
            //Flowers
            variation = Random.insideUnitCircle * ForestSize * radius;
            var spawnFlower = Initialize(tree, flowers, mats);
            AddSurfaceObject(spawnFlower, polar + variation);
        }
    }
    void GenerateAlienForests(string[] alienTrees, Material[] xenoMats)
    {
        var polar = new Vector2(Random.Range(20f, 330f), Random.Range(0f, 360f));
        var ForestSize = Random.Range(2, 5);
        Material[] mats = new Material[2];
        for (int e = 0; e < ForestSize; e++)
        {
            mats[0] = xenoMats[Random.Range(0, xenoMats.Length)]; 
            mats[1] = xenoMats[Random.Range(0, xenoMats.Length)];
            Vector2 variation;
            variation = Random.insideUnitCircle * ForestSize * radius;
            //Trees
            Spawner(e, tree, alienTrees, mats, polar, ForestSize);
            //Stones
            if (e == 1)
            {
                variation = new Vector2(e * Random.Range(-10, 10), e * Random.Range(-10, 10));
                StoneSpawner(ForestSize, polar, variation);
            }
        }
    }

    void GenerateDesertGroups(string[] palmTrees, Material[] leafMats, Material[] barkMats)
    {
        var polar = new Vector2(Random.Range(20f, 330f), Random.Range(0f, 360f));
        var ForestSize = Random.Range(1, 3);
        Material[] mats = new Material[2];
        for (int e = 0; e < ForestSize; e++)
        {
            mats[0] = barkMats[Random.Range(0, 2)];
            mats[1] = leafMats[Random.Range(1, 3)];
            //Palms
            Spawner(e, tree, palmTrees, mats, polar, ForestSize);
            //Cacti
            var cacti = new string[] { "cactus_short", "cactus_tall" };
            Spawner(e, bush, cacti, mats, polar, ForestSize * 10);
            //Stones
            if (e == 1)
            {
                Vector2 variation = new Vector2(e * Random.Range(-10, 10), e * Random.Range(-10, 10));
                StoneSpawner(ForestSize, polar, variation);
            }
        }
    }

    void GenerateRockFeatures(Color planet_color)
    {
        var polar = new Vector2(Random.Range(20f, 330f), Random.Range(0f, 360f));
        Vector2 variation;
        for (int e = 0; e < Random.Range(1, 5); e++)
        {
            //Rocks
            variation = new Vector2(e * Random.Range(-40, 40), e * Random.Range(-40, 40));
            GameObject spawn = StoneSpawner(1, polar, variation, true);
            spawn.GetComponent<MeshRenderer>().material.color = planet_color;
        }

    }

    void GeneratePineForests(string[] pineTrees, string[] bushes, string[] logs, Material[] leafMats, Material[] barkMats)
    {
        var polar = new Vector2(Random.Range(20f, 330f), Random.Range(0f, 360f));
        var ForestSize = Random.Range(5, 10);
        Vector2 variation;
        //GameObject spawnStone;
        Material[] mats = new Material[2];
        for (int e = 0; e < ForestSize; e++)
        {
            mats[0] = barkMats[Random.Range(0, barkMats.Length)];
            mats[1] = leafMats[Random.Range(0, leafMats.Length)];
            //Trees
            Spawner(e, tree, pineTrees, mats, polar, ForestSize);

            if (e % 3 == 0)
            {
                //Bushes
                Spawner(e, bush, bushes, mats, polar, ForestSize);
            }
            else if (e % 5 == 0)
            {
                //Logs
                Spawner(e, bush, logs, mats, polar, ForestSize, rotate: true);
            }
            else
            {
                //Stones
                variation = Random.insideUnitCircle * ForestSize * radius;
                StoneSpawner(ForestSize, polar, variation);
            }

        }
    }

    public void PolarPlacement(List<GameObject> objs){
        //Update position and orientation
        foreach (var obj in objs)
            {
                //obj.SetActive(true);
                //Fix location based on polar coords
                surf = obj.GetComponent<surfaceObject>();
                polarsCoords = surf.polarCoords;
                RotationX = -transform.localEulerAngles.x;
                RotationY = -transform.localEulerAngles.y;
                coords = transform.PolarToCartesian(polarsCoords.x + RotationX, polarsCoords.y + RotationY, r);

                position = coords + transform.position;
                obj.transform.position = position;
                //obj.transform.SetPositionAndRotation(position, obj.transform.localRotation);

                float orientation = surf.orientation;

                obj.transform.LookAt(transform.position);
                obj.transform.Rotate(-90, 0, 0);

                obj.transform.rotation = obj.transform.rotation * Quaternion.Euler(0, orientation, 0);
            }
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate Distance to Camera
        Vector3 distance = transform.position - Camera.transform.position;
        //Reorient atmosphere
        atmosphereObj.transform.position = transform.position;
        atmosphereObj.transform.LookAt(AssetManager.manager.Camera.transform.position);
        if (distance.magnitude < RenderRange)
        {
            surfaceObjects.ForEach(e => e.GetComponent<Renderer>().enabled = true);
            if(surfaceObjectsMoving != null){
                PolarPlacement(surfaceObjectsMoving);
            }

            /* foreach (var obj in surfaceObjects)
            {
                if (distance.magnitude > RenderRange / 3 && (obj.name.Contains("Bush") || obj.name.Contains("Stone") || obj.name.Contains("Log")))
                {
                    //obj.SetActive(false);
                    obj.GetComponent<Renderer>().enabled = false;
                }
                else
                {
                    obj.GetComponent<Renderer>().enabled = true; */
                    //obj.SetActive(true);
                    //Fix location based on polar coords
                    /* surf = obj.GetComponent<surfaceObject>();
                    polarsCoords = surf.polarCoords;
                    RotationX = -transform.localEulerAngles.x;
                    RotationY = -transform.localEulerAngles.y;
                    coords = transform.PolarToCartesian(polarsCoords.x + RotationX, polarsCoords.y + RotationY, r);

                    position = coords + transform.position;
                    obj.transform.position = position;
                    //obj.transform.SetPositionAndRotation(position, obj.transform.localRotation);

                    float orientation = surf.orientation;

                    obj.transform.LookAt(transform.position);
                    obj.transform.Rotate(-90, 0, 0);

                    obj.transform.rotation = obj.transform.rotation * Quaternion.Euler(0, orientation, 0); */

                //}
            //}

            //Recalculate atmosphere quality
            if (numPlants > 0)
            {
                atmosphereQuality = (float)surfaceObjects.Where(e => e.tag == "Plant").ToList().Count / (float)numPlants;
            }

        } else if (distance.magnitude > RenderRange){// && surfaceObjects.ElementAt(1).activeInHierarchy == true) {
            surfaceObjects.ForEach(e => e.GetComponent<Renderer>().enabled = false);
        }

    }
}
