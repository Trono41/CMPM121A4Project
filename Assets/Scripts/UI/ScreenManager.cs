using System.ComponentModel;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{

    public GameObject[] screens;
    public GameObject curr_screen;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curr_screen = GameObject.Find("ClassSelectorScreen");
        EventBus.Instance.OnWaveEnd += SetRewardScreen;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            SwitchScreen(screens[3]);
        }
        else
        {
            screens[3].SetActive(false);
        }
    }

    public void SwitchScreen(GameObject new_screen)
    {
        curr_screen.SetActive(false);
        new_screen.SetActive(true);
        curr_screen = new_screen;
    }

    public void SetRewardScreen()
    {
        SwitchScreen(screens[3]);
    }
    
}
