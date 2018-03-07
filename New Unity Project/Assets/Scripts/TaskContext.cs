using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TaskContext : MonoBehaviour
{
    public static TaskContext singleton;
    public GameObject previewObject;
    public List<GameObject> objects = new List<GameObject>();
    [SerializeField] float previewRotationSpeed = 180f;
    [SerializeField] TextMeshPro nameField;
    List<GameObject> shuffledObjects = new List<GameObject>();

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Debug.LogWarning("Two TaskContexts in scene!");
        }
        Setup();
    }

    void Setup()
    {
        int objectCount = objects.Count;
        for (int i = 0; i < objectCount; i++)
        {
            int y = Random.Range(0, objects.Count);
            shuffledObjects.Add(objects[y]);
            objects.RemoveAt(y);
        }
        NextObject();
    }

    public void NextObject()
    {
        if (shuffledObjects.Count == 0)
        {
            Win();
            return;
        }
        GameObject tempObject = previewObject;
        previewObject = Instantiate(shuffledObjects[0], previewObject.transform.position, Quaternion.identity);
        previewObject.GetComponent<Rigidbody>().useGravity = false;
        Destroy(tempObject);
        shuffledObjects.Remove(shuffledObjects[0]);
        previewObject.name = previewObject.name.Replace("(Clone)", string.Empty);
        nameField.text = previewObject.name;
    }

    void Update()
    {
        previewObject.transform.RotateAround(previewObject.transform.position, Vector3.up, previewRotationSpeed * Time.deltaTime);
    }
    void Win()
    {
        nameField.text = "You did it! gz maen";
    }
}
