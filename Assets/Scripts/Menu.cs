using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public string theName;
    private GameObject inputFieldText;
    private GameObject myText;
    private GameObject menu;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        inputFieldText = GameObject.Find("InputText");
        myText = GameObject.Find("MyText");
        menu = GameObject.Find("Menu");
        showMenu();
    }
    
    private void showMenu() {
        menu.SetActive(true);
    }

    private void hideMenu() {
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
        hideMenu();
        SceneManager.LoadScene("Game");
    }
    
}
