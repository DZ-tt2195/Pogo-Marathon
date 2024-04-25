using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] float startingDelay;
    float rotationSpeed;
    [SerializeField] GameObject toRotate;

    private void Start()
    {
        rotationSpeed = Random.Range(0.4f, 1f) * (Random.Range(0, 2) == 0 ? 1 : -1);
        toRotate.SetActive(false);
        Invoke(nameof(EnableBlades), startingDelay);
    }

    private void Update()
    { 
        toRotate.transform.Rotate(0f, rotationSpeed, 0f);
    }

    void EnableBlades()
    {
        toRotate.SetActive(true);
    }

}
