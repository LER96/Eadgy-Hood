using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BGAudio : MonoBehaviour
{
    public List<AudioSource> AudioList;
    private bool _isOpening;
    private bool _ranOpening = false;

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("AudioManager");
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
        {
            _isOpening = true;
        }
        else
        {
            _isOpening = false;
            _ranOpening = false;
        }

        if (_isOpening == true)
        {
            if (_ranOpening == false)
            {
                _ranOpening = true;
                AudioList[0].volume = 1;
                AudioList[2].volume = 0;
                AudioList[0].Play();
                AudioList[2].Play();
                ChangeToLoopingBG();
            }
        }
    }

    private void ChangeToLoopingBG()
    {
        AudioList[1].volume = 1;
        AudioList[3].volume = 0;
        AudioList[1].PlayDelayed(AudioList[0].clip.length);
        AudioList[3].PlayDelayed(AudioList[2].clip.length);
    }

    public void ChangeMusicStyle(bool isHooded)
    {
        if (isHooded == false)
        {
            AudioList[0].volume = 1;
            AudioList[1].volume = 1;
            AudioList[2].volume = 0;
            AudioList[3].volume = 0;
        }
        else
        {
            AudioList[0].volume = 0;
            AudioList[1].volume = 0;
            AudioList[2].volume = 1;
            AudioList[3].volume = 1;
        }
    }
}
