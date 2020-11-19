using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLoader : MonoBehaviour
{

    public GameObject eventSystem;
    public RectTransform menuCanvas;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(Instantiate(eventSystem));
        Instantiate(menuCanvas);
    }
}
