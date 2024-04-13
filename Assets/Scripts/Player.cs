using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private CharacterController charController;
    [SerializeField] float speed;

    [SerializeField] Transform personalCamera;
    [SerializeField] float gravityValue;
    [SerializeField] float jumpHeight;

    float currentYVelocity;

    void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (/*Input.GetKeyDown(KeyCode.Space) && */ charController.isGrounded)
            currentYVelocity = Mathf.Sqrt(jumpHeight * gravityValue);
        else if (charController.isGrounded)
            currentYVelocity = -0.5f;

        move.y = currentYVelocity;
        currentYVelocity += gravityValue * Time.deltaTime;
        charController.Move(speed * Time.deltaTime * move);
        personalCamera.transform.position = new Vector3(this.transform.position.x, 30, this.transform.position.z - 12.5f);

        if (this.transform.position.y < -10f)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Falling Platform"))
        {
            Debug.Log($"trigger drop: {other.name}");
            StartCoroutine(PlatformFall(other.transform));
        }
    }

    IEnumerator PlatformFall(Transform platform)
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            platform.transform.Translate(Vector3.down);
            yield return null;
        }
    }
}
