using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstronautScript : MonoBehaviour
{
    public bool abducted = false;

    void Update()
    {
        if (!abducted && transform.position.y != -90)
        {
            transform.position -= new Vector3(0, 30, 0) * Time.deltaTime;
            if (transform.position.y <= -90)
            {
                Destroy(gameObject);
            }
        }
    }
}
