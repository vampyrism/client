using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoader : MonoBehaviour
{

    public RectTransform menuCanvas;
    private GameObject menuImage;

    // Start is called before the first frame update
    void Awake()
    {
        Instantiate(menuCanvas);
        SetupMenu();
    }

    void SetupMenu() {
        menuImage = GameObject.Find("MenuImage");
        menuImage.SetActive(true);
    }
    private void hideMenuImage() {
        menuImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) {
            hideMenuImage();
            SceneManager.LoadScene("Game");

        }

    }
}
