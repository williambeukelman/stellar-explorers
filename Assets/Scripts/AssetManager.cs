using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager manager;
    public GameObject rock;
    public GameObject tree;
    public GameObject treeInvert;
    public GameObject bush;
    public GameObject stone;
    public GameObject atmosphere;
    public GameObject Camera;
    public Material[] leafMats;
    public Material[] xenoMats;
    public Material[] barkMats;
    public Material[] flowerMats;
    public Dictionary<string, Mesh> MeshDirectory;
    public InterfaceScripts UI;
    public UserControls Controls;
    public GeneratePlanets PlanetGenerator;

    private Material[] LoadMaterials(string path, string name, int length){
        Material[] materials = new Material[length];
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = Resources.Load<Material>(path+name+(i+1));
        }
        return materials;
    }
    private void Awake()
    {
        CreateSingleton();

        //Get Camera
        Camera = GameObject.FindWithTag("MainCamera");

        //Get UserControls
        Controls = Camera.GetComponent<UserControls>();

        //Planet Generator
        PlanetGenerator = GameObject.FindGameObjectWithTag("PlanetGen").GetComponent<GeneratePlanets>();

        //User Interface
        UI = GameObject.FindGameObjectWithTag("UI").GetComponent<InterfaceScripts>();

        //Load Resources
        rock = Resources.Load<GameObject>("PlanetFeatures/Rock");
        tree = Resources.Load<GameObject>("PlanetFeatures/Tree");
        treeInvert = Resources.Load<GameObject>("PlanetFeatures/TreeInvert");
        bush = Resources.Load<GameObject>("PlanetFeatures/Bush");
        stone = Resources.Load<GameObject>("PlanetFeatures/Stone");
        atmosphere = Resources.Load<GameObject>("Atmosphere");

        //Leaf Materials
        leafMats = LoadMaterials("PlanetFeatures/LeafMats/", "leafsGreen", 4);
        //Xeno Materials
        xenoMats = LoadMaterials("PlanetFeatures/LeafMats/", "leafsXeno", 4);
        //Bark Materials
        barkMats = LoadMaterials("PlanetFeatures/BarkMats/", "woodBark", 4);
        //Flower Materials
        flowerMats = LoadMaterials("PlanetFeatures/FlowerMats/", "flowerColor", 4);

        //Load meshes and add to mesh array
        MeshDirectory = new Dictionary<string, Mesh>();
        var meshes = new string[]{"tree_pineDefaultA", "tree_pineDefaultB", "tree_pineGroundA", "tree_pineGroundB", "tree_pineRoundA", "tree_pineRoundB", 
            "tree_pineRoundC", "tree_pineRoundD", "tree_pineRoundE", "tree_pineRoundF", "tree_pineSmallA", "tree_pineSmallB", "tree_pineSmallC", "tree_pineSmallD", 
            "tree_pineTallA", "tree_pineTallB", "tree_pineTallC", "tree_pineTallD", 
            "tree_palmBend", "tree_palmShort", "tree_palmTall", 
            "tree_blocks_dark", "tree_blocks", "tree_cone_dark", "tree_cone", "tree_default_dark", "tree_default", "tree_detailed_dark", 
            "tree_detailed", "tree_fat_darkh", "tree_fat", "tree_oak_dark", "tree_oak",
            "tree_plateau_dark", "tree_plateau", "tree_simple_dark", "tree_simple", "tree_small_dark", "tree_small", 
            "tree_tall_dark", "tree_tall", "tree_thin_dark", "tree_thin",
            "grass", "grass_large", "grass_leafs", "grass_leafsLarge",
            "flower_redA", "flower_redB", "flower_redC",
            "log",
            "mushroom_red", "mushroom_redGroup", "mushroom_redTall",
            "plant_bushDetailed", "plant_bush", "plant_bushLarge", "plant_bushLargeTriangle", "plant_bushSmall", 
            "plant_bushTriangle", "plant_flatShort", "plant_flatTall",
            "cactus_short", "cactus_tall",
            "stone_smallA", "stone_smallB", "stone_smallC", "stone_smallD", "stone_smallE", "stone_smallF", "stone_smallG", "stone_smallH", "stone_smallI",
            "rock_tallA", "rock_tallB", "rock_tallC", "rock_tallD", "rock_tallE", "rock_tallF", "rock_tallG", "rock_tallH", "rock_tallI", "rock_tallJ"
            };
        for (int i = 0; i < meshes.Length; i++)
        {
            Mesh mesh = Resources.Load<Mesh>("Nature Assets/"+meshes[i]);
            MeshDirectory.Add(meshes[i], mesh);
            //MeshDirectory[i] = Resources.Load<Mesh>("Nature Assets/"+meshes[i]);
        }

    }

    void CreateSingleton()
    {
        if (manager == null){
            manager = this;
        }
        else{
            Destroy(gameObject);
        }
            

        DontDestroyOnLoad(gameObject);
    }
}