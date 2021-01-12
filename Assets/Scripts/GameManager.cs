using Assets.Scripts;
using Assets.Server;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public static string lobbyManager = "http://lobby.vampyrism.dev.deltafault.com";

    public string LobbyId;
    public LobbyResponse Lobby;

    private Pathfinding pathfinding;

    public VisionCone cone;
    public Transform tileMap;

    public int gridHeight = 100;
    public int gridWidth = 100;
    public int cellSize = 1;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject otherPlayerPrefab;
    [SerializeField] private GameObject bow;
    [SerializeField] private GameObject crossbow;
    [SerializeField] private GameObject gameCanvas;

    public bool isDay = true;
    public bool coneDebugOverride = false;

    private float timeLeft;

    private GameObject[] enemyList;

    public ConcurrentQueue<Action> TaskQueue { get; private set; } = new ConcurrentQueue<Action>();
    public ConcurrentDictionary<UInt32, Entity> Entities { get; private set; } = new ConcurrentDictionary<UInt32, Entity>();

    public Player currentPlayer;

    void Awake()
    {
        //Screen.SetResolution(1280, 720, false);
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

    void InitGame()
    {
        timeLeft = 5.0f;
        tileMap = Instantiate(tileMap, new Vector3(0f, 100f), Quaternion.identity);
        tileMap.Find("Grid").Find("ObstaclesOverPlayer").gameObject.GetComponent<TilemapRenderer>().sortingLayerName = "Unit";
        pathfinding = new Pathfinding(gridHeight, gridWidth, cellSize);

        Instantiate(gameCanvas, new Vector3(0, 0), Quaternion.identity);
        /*Instantiate(enemyPrefab, new Vector3(42f, 44f), Quaternion.identity);
        Instantiate(playerPrefab, new Vector3(34f, 34f), Quaternion.identity);
        Instantiate(otherPlayerPrefab, new Vector3(47f, 47f), Quaternion.identity);
        Instantiate(otherPlayerPrefab, new Vector3(4f, 4f), Quaternion.identity);
        Instantiate(bow, new Vector3(34f, 32f), Quaternion.identity);
        Instantiate(crossbow, new Vector3(32f, 32f), Quaternion.identity);*/

        NetworkClient c = NetworkClient.GetInstance();

        LobbyId = Menu.instance.LobbyId;
        Debug.Log("Finding lobby...");
        UnityWebRequest wr = UnityWebRequest.Get(lobbyManager + "/api/lobby/id/" + LobbyId);
        wr.SendWebRequest();
        while (!wr.isDone) ;
        try
        {
            LobbyResponse res = JsonUtility.FromJson<LobbyResponse>(wr.downloadHandler.text);
            Debug.Log("Setting connection to " + res.metadata.ip + ":" + res.metadata.port);
            c.ip = res.metadata.ip;
            c.port = Convert.ToInt32(res.metadata.port);
            Lobby = res;
        }
        catch (Exception e)
        {
            Debug.LogError(wr.downloadHandler.text);
            Debug.LogError(e);
        }

        c.Init();
    }

    void Update()
    {
        if (coneDebugOverride == true) {
            if (isDay == false) {
                cone.showCone();
            } else {
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

    public void HandleAttack(UInt32 playerId, Vector2 clickPosition, short weaponType)
    {
        AttackMessage m = new AttackMessage(0, playerId, 0, 0, 0, weaponType, 0, 0, clickPosition.x, clickPosition.y, 1);
        Debug.Log(m);
        NetworkClient.GetInstance().MessageQueue.Enqueue(m);
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

    public void OnConnected()
    {
        Invoke("JoinLobby", 1);
    }

    public void JoinLobby()
    {
        DiscordController.instance.JoinLobby(Lobby.id, Lobby.secret);
    }

    public void DestroyEntityID(uint entityID) {
        if (Entities.TryGetValue(entityID, out Entity e)) {
            Destroy(e.gameObject);
        } else {
            Debug.Log("Trying to destroy entity ID: " + entityID + ", but couldn't find it.");
        }
    }
    public void OnDestroy() {
        NetworkClient.GetInstance().Destroy();
    }
}
