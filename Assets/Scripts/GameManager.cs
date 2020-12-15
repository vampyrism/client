﻿using Assets.Server;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject otherPlayerPrefab;
    [SerializeField] private GameObject bow;
    [SerializeField] private GameObject crossbow;
    [SerializeField] private GameObject gameCanvas;

    public bool isDay = true;
    public bool isNight = false;
    public bool coneActivated = true;

    private float timeLeft;

    private GameObject[] enemyList;

    public ConcurrentQueue<Action> TaskQueue { get; private set; } = new ConcurrentQueue<Action>();
    public ConcurrentDictionary<UInt32, Entity> Entities { get; private set; } = new ConcurrentDictionary<UInt32, Entity>();

    public Player currentPlayer;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) {
            Destroy(gameObject);
            return;
        }
        
        InitGame();
    }

    private void Start() {
        enemyList = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public void HandleAttack(UInt32 playerId, UInt32 targetId, short weaponType, Vector2 attackDirection)
    {
        AttackMessage m = new AttackMessage(0, playerId, 0, 0, 0, weaponType, 0, 0, attackDirection.x, attackDirection.y, 1);
        Debug.Log(m);
        NetworkClient.GetInstance().MessageQueue.Enqueue(m);
    }

    public void AttackTrigger(UInt32 playerID, UInt32 targetID, Vector2 targetPos, short weaponType)
    {
        AttackMessage m = new AttackMessage(0, playerID, 0, 0, targetID, weaponType, 0, 0, targetPos.x, targetPos.y, 1);
        Debug.Log(m);
        NetworkClient.GetInstance().MessageQueue.Enqueue(m);
    }

    void InitGame()
    {
        timeLeft = 5.0f;
        Transform tilemap = Instantiate(tileMap, new Vector3(0f, 100f), Quaternion.identity);
        cone = Instantiate(cone);
        pathfinding = new Pathfinding(gridHeight, gridWidth, cellSize);

        Instantiate(gameCanvas, new Vector3(0, 0), Quaternion.identity);
        /*Instantiate(enemyPrefab, new Vector3(42f, 44f), Quaternion.identity);
        Instantiate(playerPrefab, new Vector3(34f, 34f), Quaternion.identity);
        Instantiate(otherPlayerPrefab, new Vector3(47f, 47f), Quaternion.identity);
        Instantiate(otherPlayerPrefab, new Vector3(4f, 4f), Quaternion.identity);
        Instantiate(bow, new Vector3(34f, 32f), Quaternion.identity);
        Instantiate(crossbow, new Vector3(32f, 32f), Quaternion.identity);*/

        NetworkClient c = NetworkClient.GetInstance();
        c.Init();
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

    private void FixedUpdate()
    {
        while (this.TaskQueue.Count > 0)
        {
            bool s = this.TaskQueue.TryDequeue(out Action a);

            if (s)
            {
                a.Invoke();
            }
        }

        NetworkClient.GetInstance().FixedUpdate();
    }

    public void HandleKilledPlayer(Transform killedPlayer) {
        foreach (GameObject enemyGameObject in enemyList) {
            enemyGameObject.GetComponent<Enemy>().RemovePlayerFromTargets(killedPlayer);
            }
    }

    public void GameOver() {
        SceneManager.LoadScene("GameOver");
    }

    UInt16 mvseq = 0;
    public void UpdateEntityPosition(Entity e)
    {
        MovementMessage m = new MovementMessage(
            this.mvseq,
            e.ID,
            0,
            0,
            e.X,
            e.Y,
            e.Rotation,
            e.DX,
            e.DY
            );
        this.mvseq += 1;

        NetworkClient.GetInstance().MessageQueue.Enqueue(m);
    }

    public void DestroyEntityID(uint entityID) {
        if (Entities.TryGetValue(entityID, out Entity e)) {
            Destroy(e.gameObject);
        } else {
            Debug.Log("Trying to destroy entity ID: " + entityID + ", but couldn't find it.");
        }
    }
}
