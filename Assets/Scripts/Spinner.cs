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
        StartCoroutine(Delay());
    }

    private void Update()
    {
        toRotate.transform.localEulerAngles = new Vector3(
            toRotate.transform.localEulerAngles.x,
            toRotate.transform.localEulerAngles.y + rotationSpeed,
            toRotate.transform.localEulerAngles.z);
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(startingDelay);
        toRotate.SetActive(true);
    }
}
