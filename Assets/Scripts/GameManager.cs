﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private Pathfinding pathfinding;

    public int gridHeight = 25;
    public int gridWidth = 25;

    public GameObject enemyPrefab;
    public GameObject playerPrefab;

    float timeLeft = 5.0f;
    public bool isDay = true;
    public bool isNight = false;
    private Camera cam;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) {
            Destroy(gameObject);
            return;
        }

        pathfinding = new Pathfinding(gridHeight, gridWidth);
        Grid<PathNode> grid = pathfinding.GetGrid();
        
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {

                //Check collision in the middle of the square
                Collider2D hit = Physics2D.OverlapPoint(new Vector2(x+0.5f, y+0.5f));

                //Check collision in the bottom left part of the square
                if (hit == null) {
                    hit = Physics2D.OverlapPoint(new Vector2(x+0.2f, y+0.2f));
                }

                //Check collision in the top left part of the square
                if (hit == null) {
                    hit = Physics2D.OverlapPoint(new Vector2(x+0.2f, y+0.8f));
                }

                //Check collision in the top right part of the square
                if (hit == null) {
                    hit = Physics2D.OverlapPoint(new Vector2(x+0.8f, y+0.8f));
                }

                //Check collision in the bottom right of the square
                if (hit == null) {
                    hit = Physics2D.OverlapPoint(new Vector2(x+0.8f, y+0.2f));
                }

                if (hit != null) {
                    grid.GetGridObject(x, y).isWalkable = false;
                    Debug.Log(grid.GetGridObject(x, y));

                    //Debugging the collision detection on the grid.
                    Debug.Log("Found Collision here: " + x + "," + y);
                    Debug.DrawLine(new Vector3(x, y+0.5f), new Vector3(x+1f, y+0.5f), Color.red, 100f);
                    Debug.DrawLine(new Vector3(x+0.5f, y), new Vector3(x+0.5f, y+1f), Color.red, 100f);
                }

            }
        }

        DontDestroyOnLoad(gameObject);
        
        InitGame();


        cam = Camera.main;
    }

    private VisionCone cone;

    void InitGame()
    {
        //cone = gameObject.GetComponent(typeof(VisionCone)) as VisionCone; //this does not work
        //cone = GameObject.FindGameObjectWithTag("visionCone").transform.GetComponent<VisionCone>();

        Instantiate(enemyPrefab, new Vector3(10f, 10f), Quaternion.identity);
        Instantiate(playerPrefab, new Vector3(14f, 14f), Quaternion.identity);
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown (0)) {
            Debug.Log ("mouseDown = " + cam.ScreenToWorldPoint(Input.mousePosition));
        }
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
