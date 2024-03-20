using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JellyMove : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera _camera;
    public Rigidbody2D rb;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);

        rb.position = new Vector3(mousePos.x, mousePos.y, 0);
        
    }
    
}
