using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private Pathfinding pathfinding;
    
    public VisionCone cone;
    public Transform tileMap;

    public int gridHeight = 25;
    public int gridWidth = 25;
    public int cellSize = 2;

    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    public GameObject bow;

    public bool isDay = true;
    public bool isNight = false;
    public bool coneActivated = true;

    private float timeLeft;


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

    void InitGame()
    {
        timeLeft = 5.0f;
        Instantiate(tileMap, new Vector3(0f, 50f), Quaternion.identity);
        cone = Instantiate(cone);
        pathfinding = new Pathfinding(gridHeight, gridWidth);

        Instantiate(enemyPrefab, new Vector3(42f, 44f), Quaternion.identity);
        Instantiate(playerPrefab, new Vector3(34f, 34f), Quaternion.identity);
        Instantiate(bow, new Vector3(34f, 32f), Quaternion.identity);
    }
    
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
                if (coneActivated) {
                    cone.showCone();
                }
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
