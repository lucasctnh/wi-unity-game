using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkInCircle : MonoBehaviour
{
    public float radius = 1f;
    public float speed = 100f;
    
    private float angle = 0;
    
    void FixedUpdate ()
    {
        float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);

        transform.localRotation = Quaternion.Euler(0, 0 - angle, 0);
        transform.localPosition = new Vector3(x, transform.localPosition.y, y);
 
        angle += speed * Time.deltaTime / radius;
    }
}