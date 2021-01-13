using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static Menu instance = null;
    public string theName;
    public string LobbyId;
    private GameObject inputFieldText;
    private GameObject lobbyIDField;
    private GameObject myText;
    private GameObject menu;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        inputFieldText = GameObject.Find("InputText");
        lobbyIDField = GameObject.Find("LobbyID");
        myText = GameObject.Find("MyText");
        menu = GameObject.Find("Menu");

        Debug.Log("Trying to set name...");
        try
        {
            DiscordController.instance.GetAndSetTextComponentName(this.SetName);
        } catch(System.Exception e)
        {
            Debug.LogWarning(e);
        }
        

        showMenu();
    }

    public void SetName(string name)
    {
        GameObject.Find("InputText").GetComponentInParent<InputField>().text = name;
    }
    
    public void showMenu() {
        menu.SetActive(true);
    }

    public void hideMenu() {
        menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) {
            StoreName();
        }
    }

    public void StoreName()
    {
        theName = inputFieldText.GetComponent<Text>().text;
        myText.GetComponent<Text>().text = "Username: " + theName;
        Debug.Log(theName);
        this.LobbyId = lobbyIDField.GetComponent<Text>().text;
        hideMenu();
        SceneManager.LoadScene("Game");
    }
    
}
