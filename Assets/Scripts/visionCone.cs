using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visionCone : MonoBehaviour
{
    SpriteRenderer body;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get the Screen positions of the object
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint (transform.position);
         
        //Get the Screen position of the mouse
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);
        
        //Get the angle between the points
        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
 
        transform.rotation = Quaternion.Euler (new Vector3(0f,0f,angle));
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    public void showCone()
    {
        body.enabled = true;
    }

    public void hideCone()
    {
        body.enabled = false;
    }
}
