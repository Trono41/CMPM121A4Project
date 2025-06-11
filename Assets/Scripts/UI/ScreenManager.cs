using System.ComponentModel;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{

    public GameObject[] screens;
    public GameObject curr_screen;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize curr_screen with a default screen if ClassSelectorScreen is not found
        curr_screen = GameObject.Find("MainMenuScreen");
        if (curr_screen == null && screens != null && screens.Length > 0)
        {
            curr_screen = screens[0];
            Debug.LogWarning("MainMenuScreen not found, using first screen in array as default");
        }
        
        if (EventBus.Instance != null)
        {
            EventBus.Instance.OnWaveEnd += SetRewardScreen;
        }
        else
        {
            Debug.LogError("EventBus.Instance is null!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            if (screens != null && screens.Length > 3 && screens[3] != null)
            {
                SwitchScreen(screens[3]);
            }
            else
            {
                Debug.LogError("Reward screen (screens[3]) is not set in the inspector!");
            }
        }
        else if (screens != null && screens.Length > 3 && screens[3] != null)
        {
            screens[3].SetActive(false);
        }
    }

    public void SwitchScreen(GameObject new_screen)
    {
        if (curr_screen == null)
        {
            Debug.LogError("Current screen is null!");
            return;
        }
        
        if (new_screen == null)
        {
            Debug.LogError("New screen is null!");
            return;
        }

        curr_screen.SetActive(false);
        new_screen.SetActive(true);
        curr_screen = new_screen;
    }

    public void SetRewardScreen()
    {
        if (screens != null && screens.Length > 3 && screens[3] != null)
        {
            SwitchScreen(screens[3]);
        }
        else
        {
            Debug.LogError("Reward screen (screens[3]) is not set in the inspector!");
        }
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting game!");
        Application.Quit();
    }
}
