using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{
    public UIDocument document;
    private VisualElement root;
    private Button StartGame;
    private Button ExitGame;
    public GameObject Cam;
    public float speed = 0.05f;
    public int xA = 0;
    public int yA = -50;
    public int zA = 0;
    public float dist = 10;
    private AudioSource UIaudio;
    public AudioClip audiohoveroption;
    public AudioClip audioMusic;

    public void AudioHover(MouseEnterEvent evt){
        UIaudio.PlayOneShot(audiohoveroption, .5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        UIaudio = GetComponent<AudioSource>();
        UIaudio.PlayOneShot(audioMusic);
        root = document.rootVisualElement;
        StartGame = root.Q<Button>("StartGame");
        StartGame.RegisterCallback<ClickEvent>(StartGameFunc);
        StartGame.RegisterCallback<MouseEnterEvent>(AudioHover);
        ExitGame = root.Q<Button>("ExitGame");
        ExitGame.RegisterCallback<ClickEvent>(ExitGameFunc);
        ExitGame.RegisterCallback<MouseEnterEvent>(AudioHover);
    }
    IEnumerator LoadAndStart() {
        StartGame.text = "Loading...";
        //SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync(1);
        yield return new WaitUntil(() => SceneManager.sceneCount > 1);
        //SceneManager.UnloadSceneAsync("StartMenu");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game"));
    }

    void StartGameFunc(ClickEvent clickevt){
        StartCoroutine(LoadAndStart());
        //SceneManager.LoadScene("Game", LoadSceneMode.Additive);
    }

    void ExitGameFunc(ClickEvent clickevt){
        print("Quit");
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        //Cam.transform.position = new Vector3(dist, Cam.transform.position.y, Cam.transform.position.z);
        Cam.transform.RotateAround(new Vector3(0,0,0), Vector3.up, speed);
        //Cam.transform.RotateAround(Cam.transform.position, Vector3.up, tiltSpeed);
        Cam.transform.LookAt(new Vector3(0,0,0));
        Cam.transform.Rotate(new Vector3(xA, yA, zA), Space.Self);
    }
}
