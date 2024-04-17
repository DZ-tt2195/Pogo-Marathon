using System.Collections;
using UnityEngine;

public class Chaser : MonoBehaviour
{
    int currentIndex = 0;
    [SerializeField] float delay;

    private void Start()
    {
        StartCoroutine(ChasePlayer());
    }

    IEnumerator ChasePlayer()
    {
        yield return new WaitForSeconds(delay);
        while (true)
        {
            this.transform.position = Player.instance.listOfLocations[currentIndex];
            currentIndex++;
            yield return null;
        }

        /*
        while (true)
        {
            float elapsedTime = 0f;
            float totalTime = 0.2f;

            while (Player.instance.listOfLocations.Count-4 < currentIndex)
                yield return null;

            Vector3 originalLocation = this.transform.position;
            Vector3 targetLocation = Player.instance.listOfLocations[currentIndex];

            while (elapsedTime < totalTime)
            {
                Vector3 newJump = Vector3.Lerp(originalLocation, targetLocation, elapsedTime / totalTime);
                this.transform.position = newJump;

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            currentIndex++;
        }*/
    }
}
