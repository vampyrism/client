using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public visionCone cone;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        
        InitGame();
    }

    void InitGame()
    {

    }

    float timeLeft = 5.0f;
    public bool isDay = true;
    
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (isDay)
        {
            if (timeLeft < 0)
            {
                isDay = false;
                timeLeft += 10.0f;
                cone.showCone();
            }
        }
        else
        {
            if (timeLeft < 0)
            {
                isDay = true;
                timeLeft += 5.0f;
                cone.hideCone();
            }
        }
    }
}
