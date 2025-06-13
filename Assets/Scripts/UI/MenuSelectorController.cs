using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TMPro;

public class MenuSelectorController : MonoBehaviour
{
    private AudioClip music;
    public TextMeshProUGUI label;
    public string level;
    public EnemySpawner spawner;
    public ClassSelector class_selector;
    public PlayerController player;
    public JToken class_stats;

    public ScreenManager screen_manager;
    public GameObject next_screen;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screen_manager = GameObject.Find("ScreenManager").GetComponent<ScreenManager>();
        next_screen = GameObject.Find("DifficultySelector");

        music = Resources.Load<AudioClip>("Sounds/Music");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevel(string text)
    {
        level = text;
        label.text = text;
    }

    public void GetClass(string text, JToken c)
    {
        class_stats = c;
        level = text;
        label.text = text;
    }

    public void StartLevel()
    {
        spawner.StartLevel(level);
        SoundManager.instance.playSound(music, transform, .1f);
    }

    public void SetClass()
    {
        player.SetClass(class_stats);

        screen_manager.SwitchScreen(next_screen);
    }

}
