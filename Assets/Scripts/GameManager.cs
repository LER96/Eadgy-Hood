using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public  GameObject GameMenu;
    public  GameObject Player;
    public bool GamePaused = false;

    private bool _inMainMenu;


    private void Awake()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
        {
            _inMainMenu = true;
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(2))
        {
            GamePaused = true;
        }
        else
        {
            _inMainMenu = false;
        }

        if (_inMainMenu == true)
        {
            PauseMode(true);

        }
        else
        {
            PauseMode(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {  
        if (_inMainMenu == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !GamePaused)
            {
                PauseMode(true);
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && GamePaused)
            {
                PauseMode(false);
            }
        }
    }

    public void PauseMode(bool paused)
    {
        GamePaused = paused;
        OnPause(paused);
        ChangeCursorMode(paused);
    }

    public void OnPause(bool paused)
    {
        if (paused)
        {
            GameMenu.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            GameMenu.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    private void ChangeCursorMode(bool unlocked)
    {
        if (unlocked)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Play()
    {
        PauseMode(false);
        Player.GetComponent<Animator>().SetBool("StartIntro", true);
    }

    public void Reset()
    {
        PauseMode(false);
        Player.GetComponent<Animator>().SetBool("Reset", true);
    }

    public void OnAnimationEndDo()
    {
        if (_inMainMenu == true)
        {
            Player.GetComponent<Animator>().SetBool("StartIntro", false);
            SceneManager.LoadScene(1);
        }
        else
        {
            Player.GetComponent<Animator>().SetBool("Reset", false);
            Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

}
