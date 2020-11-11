using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

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
        //cone = gameObject.GetComponent(typeof(VisionCone)) as VisionCone; //this does not work
        cone = GameObject.FindGameObjectWithTag("visionCone").transform.GetComponent<VisionCone>();
    }

    float timeLeft = 5.0f;
    public bool isDay = true;
    public bool isNight = false;
    
    void Update()
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
