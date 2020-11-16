using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    
    private GameObject menuImage;
    private bool onMenu = false;

    private Pathfinding pathfinding;
    private VisionCone cone;

    public int gridHeight = 25;
    public int gridWidth = 25;

    public GameObject enemyPrefab;
    public GameObject playerPrefab;

    float timeLeft = 5.0f;
    public bool isDay = true;
    public bool isNight = false;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) {
            Destroy(gameObject);
            return;
        }

        pathfinding = new Pathfinding(gridHeight, gridWidth);

        DontDestroyOnLoad(gameObject);
        
        InitGame();
    }

    void InitGame()
    {
        //SetupMenu();

        //cone = GameObject.FindGameObjectWithTag("visionCone").transform.GetComponent<VisionCone>();

        Instantiate(enemyPrefab, new Vector3(20f, 20f), Quaternion.identity);
        Instantiate(playerPrefab, new Vector3(28f, 28f), Quaternion.identity);
    }

    void SetupMenu()
    {
        menuImage = GameObject.Find("MenuImage");
        menuImage.SetActive(true);
        onMenu = true;
    }

    private void hideMenuImage()
    {
        menuImage.SetActive(false);
    }
    
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
                    //cone.showCone();
                }
            }
            else
            {
                if (timeLeft < 0)
                {
                    isDay = true;
                    isNight = false;
                    timeLeft += 5.0f;
                    //cone.hideCone();
                }
            }
        }
    }
}
