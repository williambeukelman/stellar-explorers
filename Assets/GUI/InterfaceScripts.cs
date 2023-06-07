using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;

public class card
    {
        public string Label, Value;
    }

public class resource{
    public int PopVal;
    public int MaxPop;
    public float OreVal;
    public int MineProdVal;
    public int EnergyProdVal;
    public int EnergyUse;
}

public class building
    {
        public string Name;
        public Texture2D Image;
        public string Description;
        public int EnergyCost;
        public int EnergyProd;
        public int OreProd;
        public int MaterialCost;
}

public class InterfaceScripts : MonoBehaviour
{
    public UIDocument document;

    //Externally modifiable
    [HideInInspector]
    public bool globalView;
    public bool hasCiv;
    private VisualElement root;

    //Main Buttons
    private Button BtnOpenBuildMenu;
    private Button BtnResourceMenu;
    private Button BtnInfoMenu;

    //Menus
    private VisualElement BuildMenuBorder;
    private VisualElement ResourceMenuBorder;
    private VisualElement InfoMenuBorder;
    public VisualElement InfoMenu;
    private VisualElement BuildMenu;
    private VisualElement ResourceMenu;
    private VisualElement BuildDetailsMenu;

    //Settings Elements
    private VisualElement SettingsBtn;
    private VisualElement SettingsOverlay;
    private VisualElement ExitBtn;
    private VisualElement SaveBtn;
    private Slider EffectsSlider;
    private Slider MusicSlider;
    private Slider InterfaceSlider;

    //Launch Menu
    private VisualElement LaunchOverlay;
    public RadioButtonGroup PlanetTargets;
    private VisualElement LaunchBtn;

    //Colony Menu
    private VisualElement ColonyOverlay;
    private VisualElement ColonyCloseBtn;
    private VisualElement ColonyMineBtn;
    private VisualElement ColonyCloneBtn;
    private VisualElement ColonyBuildBtn;
    private VisualElement ColonyWanderBtn;

    //WarningOverlay and Timer
    private VisualElement WarningOverlay;
    private VisualElement WarningBtn;
    private VisualElement Timer;
    private Label TimerValue;

    //Booleans
    public bool BuildMenuOpen = false;
    private bool ResourceMenuOpen = false;
    public bool InfoMenuOpen = false;
    public VisualElement infoGrid;
    public VisualElement buildGrid;
    private VisualTreeAsset CardRow;
    private VisualTreeAsset GenericBtn;
    private VisualTreeAsset BuildOptionBox;
    private BuildManager buildManager;
    private VisualElement currBtn;
    private VisualElement Notifications;
    private Label NotificationValue;
    private VisualElement DeathScreen;

    //Controllers
    private UserControls Controls;
    private CivManager civ;
    private AudioSource UIaudio;

    //Audio Clips
    public AudioClip audiomenuopen;
    public AudioClip audiomenuclose;
    public AudioClip audiohoveroption;
    public AudioClip audioerror;
    public AudioClip audioplaceheavy;
    public AudioSource Musicbox;
    public float UIVolume = 1f;
    public float UIMusicVolume = 1f;

    //Timer
    public float initialTimer = 15*60;
    public float TimeLeft = 3600;
    
    // Start is called before the first frame update

    void Start(){
        UIaudio = GetComponent<AudioSource>();
        //UIaudio.PlayOneShot(audioMusic, UIVolume*UIMusicVolume);
        Musicbox = GameObject.FindGameObjectWithTag("Musicbox").GetComponent<AudioSource>();
        //Build Manager
        buildManager = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildManager>();
        //Music
        MusicSlider.RegisterValueChangedCallback(v =>
        {
            Musicbox.volume = v.newValue;
        });
        InterfaceSlider.RegisterValueChangedCallback(v =>
        {
            UIVolume = v.newValue;
        });
        //Get Buildings
        Controls = AssetManager.manager.Controls;
    }
    void Awake()
    {
        root = document.rootVisualElement;
        //Build Menu
        BuildMenu = root.Q<VisualElement>("BuildMenu");
        BtnOpenBuildMenu = root.Q<Button>("BtnOpenBuildMenu");
        BuildMenuBorder = root.Q<VisualElement>("BuildMenuBorder");
        BtnOpenBuildMenu.RegisterCallback<ClickEvent>(OpenBuildMenu);
        BuildMenu.style.display = DisplayStyle.None;
        //Details Menu
        BuildDetailsMenu = root.Q<VisualElement>("BuildDetails");
        BuildDetailsMenu.style.display = DisplayStyle.None;
        //Resource Menu
        ResourceMenu = root.Q<VisualElement>("ResourceMenu");
        BtnResourceMenu = root.Q<Button>("BtnResourceMenu");
        ResourceMenuBorder = root.Q<VisualElement>("ResourceMenuBorder");
        BtnResourceMenu.RegisterCallback<ClickEvent>(OpenResourceMenu);
        ResourceMenu.style.display = DisplayStyle.None;
        //Info Menu
        InfoMenu = root.Q<VisualElement>("InfoMenu");
        BtnInfoMenu = root.Q<Button>("BtnInfoMenu");
        InfoMenuBorder = root.Q<VisualElement>("InfoMenuBorder");
        BtnInfoMenu.RegisterCallback<ClickEvent>(OpenInfoMenu);
        InfoMenu.style.display = DisplayStyle.None;
        //Settings Menu
        SettingsBtn = root.Q<VisualElement>("SettingsBtn");
        var SettingsCloseBtn = root.Q<VisualElement>("SettingsCloseBtn");
        SettingsBtn.RegisterCallback<ClickEvent>(OpenSettingsMenu);
        SettingsCloseBtn.RegisterCallback<ClickEvent>(OpenSettingsMenu);
        SettingsOverlay = root.Q<VisualElement>("Overlay");
        SettingsOverlay.style.display = DisplayStyle.None;
        ExitBtn = root.Q<VisualElement>("ExitBtn");
        ExitBtn.RegisterCallback<ClickEvent>(ExitGame);
        SaveBtn = root.Q<VisualElement>("SaveBtn");
        EffectsSlider = root.Q<Slider>("EffectsSlider");
        MusicSlider = root.Q<Slider>("MusicSlider");
        InterfaceSlider = root.Q<Slider>("InterfaceSlider");
        //LaunchMenu
        var LaunchCloseBtn = root.Q<VisualElement>("LaunchCloseBtn");
        LaunchCloseBtn.RegisterCallback<ClickEvent>(OpenLaunchMenu);
        LaunchOverlay = root.Q<VisualElement>("LaunchOverlay");
        LaunchOverlay.style.display = DisplayStyle.None;
        PlanetTargets = root.Q<RadioButtonGroup>("PlanetTargets");
        LaunchBtn = root.Q<VisualElement>("LaunchBtn");
        LaunchBtn.RegisterCallback<ClickEvent>(Launch);
        //ColonyMenu
        ColonyOverlay = root.Q<VisualElement>("ColonyOverlay");
        ColonyOverlay.style.display = DisplayStyle.None;
        ColonyCloseBtn = root.Q<VisualElement>("ColonyCloseBtn");
        ColonyCloseBtn.RegisterCallback<ClickEvent>(ColonyClose);
        ColonyMineBtn = root.Q<VisualElement>("ColonyMineBtn");
        ColonyMineBtn.RegisterCallback<ClickEvent>(ColonyMine);
        ColonyCloneBtn = root.Q<VisualElement>("ColonyCloneBtn");
        ColonyCloneBtn.RegisterCallback<ClickEvent>(ColonyClone);
        ColonyBuildBtn = root.Q<VisualElement>("ColonyBuildBtn");
        ColonyBuildBtn.RegisterCallback<ClickEvent>(ColonyBuild);
        ColonyWanderBtn = root.Q<VisualElement>("ColonyWanderBtn");
        ColonyWanderBtn.RegisterCallback<ClickEvent>(ColonyWander);
        //WarningOverlay and Timer
        WarningOverlay = root.Q<VisualElement>("WarningOverlay");
        WarningOverlay.style.display = DisplayStyle.Flex;
        WarningBtn = root.Q<VisualElement>("WarningBtn");
        WarningBtn.RegisterCallback<ClickEvent>(OpenWarning);
        Timer = root.Q<VisualElement>("Timer");
        Timer.RegisterCallback<ClickEvent>(OpenWarning);
        TimerValue = root.Q<Label>("TimerValue");
        //Info Grid
        infoGrid = root.Q<VisualElement>("InfoGrid");
        //Build Menu Grid
        buildGrid = root.Q<VisualElement>("BuildGrid");
        //Notifications
        Notifications = root.Q<VisualElement>("Notifications");
        NotificationValue = Notifications.Q<Label>("NotificationValue");
        Notifications.style.opacity = 0;
        //Templates
        CardRow = Resources.Load<VisualTreeAsset>("GUI/CardRow"); 
        GenericBtn = Resources.Load<VisualTreeAsset>("GUI/GenericBtn"); 
        BuildOptionBox = Resources.Load<VisualTreeAsset>("GUI/BuildOptionBox"); 
        //Death Screen
        DeathScreen = root.Q<VisualElement>("DeathScreen");
        DeathScreen.RegisterCallback<ClickEvent>(CloseDeath);
        DeathScreen.style.display = DisplayStyle.None;
        //civ = Controls.trackedObject.GetComponent<CivManager>();
    }

    public void AudioMenuOpen(){
        UIaudio.PlayOneShot(audiomenuopen, UIVolume*0.5f);
    }

    public void AudioMenuClose(){
        UIaudio.PlayOneShot(audiomenuclose, UIVolume*0.5f);
    }

    public void AudioHover(){
        UIaudio.PlayOneShot(audiohoveroption, UIVolume*0.1f);
    }

    public void AudioError(){
        UIaudio.PlayOneShot(audioerror, UIVolume*0.1f);
    }

    public void AudioPlaceHeavy(){
        UIaudio.PlayOneShot(audioplaceheavy, UIVolume*1f);
    }

    public void SetNotification(string value, int seconds=2){
        Notifications.style.opacity = 1;
        NotificationValue.text = value;
        StartCoroutine(Fade(Notifications, seconds));
    }

    public void Death(){
        Musicbox.pitch = Mathf.Lerp(Musicbox.pitch, .5f, 5);
        //Musicbox.pitch = .5f;
        DeathScreen.style.display = DisplayStyle.Flex;
        DeathScreen.style.opacity = 0;
        DeathScreen.style.opacity = 1;
    }

    private void CloseDeath(ClickEvent evt){
        StartCoroutine(Fade(DeathScreen, 5, DeathScreen));
    }

    IEnumerator Fade(VisualElement element, int time, VisualElement elementHide=null)
    {
        if(elementHide == null){
            yield return new WaitForSeconds(time);
        }
        element.style.opacity = 0;
        if(elementHide != null){
            yield return new WaitForSeconds(time);
            elementHide.style.display = DisplayStyle.None;
        }
    }

    public void ExitGame(ClickEvent evt){
        Application.Quit();
    }

    public void BuildingBtnEvent(ClickEvent evt, string[] data){
        if(civ.OreValue - Controls.Buildings.Where(e => e.Name == data[0]).First().MaterialCost >= 0 /* &&
            civ.EnergyUse + Controls.Buildings.Where(e => e.Name == data[0]).First().EnergyCost > civ.EnergyProduction */
        ){
            AudioMenuOpen();
            var targetBox = evt.target as VisualElement;
            targetBox.style.backgroundColor = new Color(0.7f, 0.88f, 1, .33f);
            buildManager.selectedBuilding = data[0];
            currBtn = targetBox;
        }
    }

    public void SetResourceValues(resource data){
        ResourceMenu.Q<Label>("PopVal").text = string.Format("{0} / {1}", data.PopVal, data.MaxPop);
        ResourceMenu.Q<Label>("OreVal").text = string.Format("{0} TONS", data.OreVal); //data.OreVal.ToString("F2") + " TONS";
        ResourceMenu.Q<Label>("MineProdVal").text = string.Format("{0} TON/MIN", data.MineProdVal);
        ResourceMenu.Q<Label>("EnergyProdVal").text = string.Format("{0} KWH", data.EnergyProdVal);
        ResourceMenu.Q<Label>("EnergyUseVal").text = string.Format("{0} KWH", data.EnergyUse);
    }

    public void OpenBuildDetails(MouseEnterEvent evt, building data){
        AudioHover();
        BuildDetailsMenu.style.display = DisplayStyle.Flex;
        BuildDetailsMenu.Q<Label>("BuildDetailsName").text = data.Name;
        BuildDetailsMenu.Q<Label>("Description").text = data.Description;
        BuildDetailsMenu.Q<Label>("EnergyCost").text = string.Format("{0}KWH", data.EnergyCost);
        BuildDetailsMenu.Q<Label>("EnergyProd").text = string.Format("{0}KWH", data.EnergyProd);
        BuildDetailsMenu.Q<Label>("MaterialCost").text = string.Format("{0}TONS", data.MaterialCost);
        if(data.EnergyCost == 0){
            BuildDetailsMenu.Q<Label>("EnergyCost").parent.style.display = DisplayStyle.None;
        } else{
            BuildDetailsMenu.Q<Label>("EnergyCost").parent.style.display = DisplayStyle.Flex;
        }
        if(data.EnergyProd == 0){
            BuildDetailsMenu.Q<Label>("EnergyProd").parent.style.display = DisplayStyle.None;
        } else{
            BuildDetailsMenu.Q<Label>("EnergyProd").parent.style.display = DisplayStyle.Flex;
        }
        if(data.MaterialCost == 0){
            BuildDetailsMenu.Q<Label>("MaterialCost").parent.style.display = DisplayStyle.None;
        } else{
            BuildDetailsMenu.Q<Label>("MaterialCost").parent.style.display = DisplayStyle.Flex;
        }
        //Set values red if outside range
        if(civ.OreValue - Controls.Buildings.Where(e => e.Name == data.Name).First().MaterialCost < 0){
            BuildDetailsMenu.Q<Label>("MaterialCost").parent.style.color = Color.red;
        } else{
            BuildDetailsMenu.Q<Label>("MaterialCost").parent.style.color = new Color(0.7f, 0.88f, 1, 1f);
        }
        //Set values red if outside range
        if(civ.EnergyUse + Controls.Buildings.Where(e => e.Name == data.Name).First().EnergyCost > civ.EnergyProduction){
            BuildDetailsMenu.Q<Label>("EnergyCost").parent.style.color = Color.red;
        } else{
            BuildDetailsMenu.Q<Label>("EnergyCost").parent.style.color = new Color(0.7f, 0.88f, 1, 1f);
        }

    }

    public void CloseBuildDetails(MouseLeaveEvent evt){
        BuildDetailsMenu.style.display = DisplayStyle.None;
    }

    public void SetBuildings(building[] props){
        ClearBuildings();
        foreach (var build in props)
        {
            var building = BuildOptionBox.CloneTree();
            building.Q<Label>("Label").text = build.Name;
            building.Q<VisualElement>("HouseAvatar").style.backgroundImage = new StyleBackground(Background.FromTexture2D(build.Image));
            var data = new string[]{
                build.Name
            };
            building.RegisterCallback<ClickEvent, string[]>(BuildingBtnEvent, data);
            building.RegisterCallback<MouseEnterEvent, building>(OpenBuildDetails, build);
            building.RegisterCallback<MouseLeaveEvent>(CloseBuildDetails);
            buildGrid.Add(building);
        }
    }

    public void ClearBuildings(){
        buildGrid.Clear();
    }

    public void SetInfo(string name, card[] props, string btn=null){
        root.Q<Label>("InfoLabel").text = name;
        //Add Items to Info Grid
        infoGrid.Clear();
        //var Cards = new []{new {Label="BIOME", Value="DESERT"}};
        foreach (var row in props)
        {
            var card1 = CardRow.CloneTree();
            card1.Q<Label>("Label").text = row.Label;
            card1.Q<Label>("Value").text = row.Value;
            card1.style.width = Length.Percent(100);
            infoGrid.Add(card1);
        }
        if(btn != null && btn.Length > 1){
            var button = GenericBtn.CloneTree();
            button.Q<Label>("").text = btn;
            if(btn == "Launch Menu"){
                button.RegisterCallback<ClickEvent>(OpenLaunchMenu);
            }
            if(btn == "Colony Menu"){
                button.RegisterCallback<ClickEvent>(OpenColonyMenu);
            }
            infoGrid.Add(button);
        }
    }

    public void ClearInfo(){
        root.Q<Label>("InfoLabel").text = "Info Menu";
        infoGrid.Clear();
    }

    public void Launch(ClickEvent evt){
        
        List<RadioButton> radios = PlanetTargets.Query<RadioButton>().ToList();
        string PlanetName = radios.Where(e => e.value == true).First().Q<Label>().text;
        if(PlanetName != Controls.trackedObject.GetComponent<GeneratePlanetFeatures>().PlanetName){
            //Close menu
            OpenLaunch();
            ClearInfo();
            OpenInfo();
            //print(PlanetName);
            GameObject selectedRocket = Controls.selectedRocket;
            Controls.trackedObject.GetComponent<GeneratePlanetFeatures>().surfaceObjects.Remove(selectedRocket);
            selectedRocket.GetComponent<Navigation>().GuidedFlight(PlanetName);
        } else{
            SetNotification("Cannot launch to own planet", 3);
        }
        
    }

    private void Warning(){
        if(WarningOverlay.style.display == DisplayStyle.None){
            AudioMenuOpen();
            WarningOverlay.style.display = DisplayStyle.Flex;
        } else {
            AudioMenuClose();
            WarningOverlay.style.display = DisplayStyle.None;
        }
    }

    private void OpenWarning(ClickEvent evt){
        Warning();
    }

    public void OpenLaunch(){
        //print("Launch menu: "+LaunchOverlay.style.display);
        if(LaunchOverlay.style.display == DisplayStyle.None){
            AudioMenuOpen();
            LaunchOverlay.style.display = DisplayStyle.Flex;
        } else
        {
            AudioMenuClose();
            LaunchOverlay.style.display = DisplayStyle.None;
        }
    }

    public void OpenColony(){
        if(ColonyOverlay.style.display == DisplayStyle.None){
            AudioMenuOpen();
            ColonyOverlay.style.display = DisplayStyle.Flex;
        } else
        {
            AudioMenuClose();
            ColonyOverlay.style.display = DisplayStyle.None;
        }
    }

    private void GiveColonyCommand(string command){
        //List<GameObject> people = new(){};
        GeneratePlanetFeatures planetFeatures = Controls.trackedObject.GetComponent<GeneratePlanetFeatures>();
        if(planetFeatures){
            int numPeople = 0;
            foreach (var item in planetFeatures.surfaceObjects)
            {
                if(item.name.Contains("Astronaut")){
                    numPeople++;
                    item.GetComponent<Movement>().GiveCommand(command);
                    //people.Add(item);
                }
            }
            if(numPeople < 1){
                SetNotification("There is no one here to command", 3);
            }
        }
    }

    private void ColonyClose(ClickEvent evt){
        OpenColony();
    }

    private void ColonyMine(ClickEvent evt){
        GiveColonyCommand("Mine");
    }

    private void ColonyClone(ClickEvent evt){
        GiveColonyCommand("Clone");
    }

    private void ColonyBuild(ClickEvent evt){
        GiveColonyCommand("Build");
    }

    private void ColonyWander(ClickEvent evt){
        GiveColonyCommand("Wander");
    }


    public void OpenColonyMenu(ClickEvent evt){
        OpenColony();
    }

    public void OpenLaunchMenu(ClickEvent evt){
        OpenLaunch();
    }

    public void OpenSettings(){
        if(SettingsOverlay.style.display == DisplayStyle.None){
            AudioMenuOpen();
            SettingsOverlay.style.display = DisplayStyle.Flex;
        } else
        {
            AudioMenuClose();
            SettingsOverlay.style.display = DisplayStyle.None;
        }
    }

    private void OpenSettingsMenu(ClickEvent evt){
        OpenSettings();
    }

    public void OpenInfo(){
        if(!InfoMenuOpen){
            AudioMenuOpen();
            InfoMenuOpen = true;
            //BtnInfoMenu.style.right = 313;
            //InfoMenuBorder.style.right = 311;
            InfoMenuBorder.style.width = 350;
            InfoMenuBorder.style.height = 200;
            InfoMenu.style.display = DisplayStyle.Flex;
            InfoMenuBorder.SendToBack();
            BtnInfoMenu.BringToFront();
        } else {
            AudioMenuClose();
            InfoMenuOpen = false;
            //BtnInfoMenu.style.right = 2;
            BtnInfoMenu.style.width = 39;
            BtnInfoMenu.style.height = 39;
            //InfoMenuBorder.style.right = 0;
            InfoMenuBorder.style.width = 39;
            InfoMenuBorder.style.height = 39;
            InfoMenu.style.display = DisplayStyle.None;
        }
    }

    private void OpenInfoMenu(ClickEvent clickevt){
        OpenInfo();
    }

    private void OpenBuildMenu(ClickEvent clickevt){
        if(!BuildMenuOpen){
            AudioMenuOpen();
            BuildMenuOpen = true;
            BuildMenuBorder.style.width = 420;
            BuildMenuBorder.style.height = 242;
            //BuildMenuBorder.style.backgroundColor = Color.clear;
            BuildMenu.style.display = DisplayStyle.Flex;
            BuildMenuBorder.SendToBack();
            BtnOpenBuildMenu.BringToFront();
            //420 x 242
        } else {
            AudioMenuClose();
            BuildMenuOpen = false;
            BtnOpenBuildMenu.style.width = 39;
            BtnOpenBuildMenu.style.height = 39;
            BuildMenuBorder.style.width = 39;
            BuildMenuBorder.style.height = 39;
            BuildMenu.style.display = DisplayStyle.None;
        }
    }

    private void OpenResourceMenu(ClickEvent clickevt){
        if(!ResourceMenuOpen){
            AudioMenuOpen();
            ResourceMenuOpen = true;
            ResourceMenuBorder.style.width = 360;
            ResourceMenuBorder.style.height = 200;
            //ResourceMenuBorder.style.backgroundColor = Color.clear;
            ResourceMenu.style.display = DisplayStyle.Flex;
            ResourceMenuBorder.SendToBack();
            BtnResourceMenu.BringToFront();
            //420 x 242
        } else {
            AudioMenuClose();
            ResourceMenuOpen = false;
            BtnResourceMenu.style.width = 39;
            BtnResourceMenu.style.height = 39;
            ResourceMenuBorder.style.width = 39;
            ResourceMenuBorder.style.height = 39;
            ResourceMenu.style.display = DisplayStyle.None;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Update civ based on selected planet
        if(Controls.trackedObject){
            civ = Controls.trackedObject.GetComponent<CivManager>();
        }

        //Update Timer
        if(TimeLeft > 0){
            TimeLeft = initialTimer - Time.realtimeSinceStartup;
            float HrLeft = Mathf.FloorToInt(TimeLeft / 3600);
            float MinLeft = Mathf.FloorToInt(TimeLeft / 60);
            float SecLeft = Mathf.FloorToInt(TimeLeft % 60);
            TimerValue.text = string.Format("{0}:{1:00}:{2:00} hr:min:s", HrLeft, MinLeft, SecLeft );
        } else {
            TimerValue.text = string.Format("{0}:{1}:{2} hr:min:s", "0", "00", "00" );
        }

        if(Controls.dead && LaunchOverlay.style.display == DisplayStyle.Flex){
            OpenLaunch();
            ClearInfo();
            OpenInfo();
        }

        //Hide Btns in Global View or when there is no civ
        if(globalView){
            BtnOpenBuildMenu.style.display = DisplayStyle.None;
            BuildMenuBorder.style.display = DisplayStyle.None;
            BuildMenu.style.display = DisplayStyle.None;
            BtnResourceMenu.style.display = DisplayStyle.None;
            ResourceMenu.style.display = DisplayStyle.None;
            ResourceMenuBorder.style.display = DisplayStyle.None;
            ResourceMenuBorder.style.width = 39;
            ResourceMenuBorder.style.height = 39;
            BuildMenuBorder.style.width = 39;
            BuildMenuBorder.style.height = 39;
            if(LaunchOverlay.style.display == DisplayStyle.Flex){
                OpenLaunch();
                ClearInfo();
                OpenInfo();
            }
        } else {
            BtnOpenBuildMenu.style.display = DisplayStyle.Flex;
            BuildMenuBorder.style.display = DisplayStyle.Flex;
            BtnResourceMenu.style.display = DisplayStyle.Flex;
            ResourceMenuBorder.style.display = DisplayStyle.Flex;
            /* ResourceMenuBorder.style.width = 360;
            ResourceMenuBorder.style.height = 200;
            BuildMenuBorder.style.width = 420;
            BuildMenuBorder.style.height = 242; */
        }


        if(buildManager.selectedBuilding == "None" && currBtn != null){
            currBtn.style.backgroundColor = Color.clear;
        }
    }
}
