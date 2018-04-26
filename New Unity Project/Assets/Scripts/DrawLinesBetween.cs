using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLinesBetween : MonoBehaviour
{

    [SerializeField] List<Transform> objects = new List<Transform>();
    [SerializeField] List<Color> colors = new List<Color>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] != null)
            {

                Debug.DrawLine(transform.position, objects[i].position, colors[i]);
            }
        }
    }
}
