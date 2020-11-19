using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverLoader : MonoBehaviour
{
    public Canvas menuCanvas;
    // Start is called before the first frame update
    void Start()
    {
        Menu.instance.showMenu();
    }
}
