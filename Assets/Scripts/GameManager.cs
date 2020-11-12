using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    
    private GameObject menuImage;
    private bool onMenu;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        
        InitGame();
    }

    private VisionCone cone;

    void InitGame()
    {
        onMenu = true;
        cone = GameObject.FindGameObjectWithTag("visionCone").transform.GetComponent<VisionCone>();

        menuImage = GameObject.Find("MenuImage");
        menuImage.SetActive(true);
    }

    private void hideMenuImage()
    {
        menuImage.SetActive(false);
    }

    float timeLeft = 5.0f;
    public bool isDay = true;
    public bool isNight = false;
    
    void Update()
    {
        if (onMenu)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                hideMenuImage();
                onMenu = false;
                timeLeft = 5.0f;
            }
        }
        else
        {
            timeLeft -= Time.deltaTime;
            if (isDay)
            {
                if (timeLeft < 0)
                {
                    isDay = false;
                    isNight = true;
                    timeLeft += 10.0f;
                    cone.showCone();
                }
            }
            else
            {
                if (timeLeft < 0)
                {
                    isDay = true;
                    isNight = false;
                    timeLeft += 5.0f;
                    cone.hideCone();
                }
            }
        }
    }
}
