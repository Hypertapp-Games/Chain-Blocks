using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePosition : MonoBehaviour
{
    public Vector3 pos;

    private void Start()
    {
        pos = gameObject.transform.position;
    }
    
}
