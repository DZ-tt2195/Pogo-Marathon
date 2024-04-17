using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController charController;
    public static Player instance;
    public List<Vector3> listOfLocations = new();

    [SerializeField] Transform personalCamera;
    [SerializeField] float speed;
    [SerializeField] float gravityValue;
    [SerializeField] float jumpHeight;

    float currentYVelocity;

    void Awake()
    {
        instance = this;
        charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        listOfLocations.Add(this.transform.position);

        if (charController.isGrounded)
        {
            currentYVelocity = Mathf.Sqrt(jumpHeight * gravityValue);
            //listOfLocations.Add(this.transform.position);
        }

        move.y = currentYVelocity;
        /*
        if (currentYVelocity > 0 && (currentYVelocity + (gravityValue*Time.deltaTime) < 0))
            listOfLocations.Add(this.transform.position);
        */
        currentYVelocity += gravityValue * Time.deltaTime;
        charController.Move(speed * Time.deltaTime * move);
        personalCamera.transform.position = new Vector3(this.transform.position.x, 30, this.transform.position.z - 12.5f);

        if (this.transform.position.y < -10f)
            Death();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Falling Platform"))
        {
            Maze2Cell cell = other.GetComponent<Maze2Cell>();
            if (!cell.shrinking)
            {
                cell.shrinking = true;
                StartCoroutine(cell.ShrinkAway());
            }
        }
        else if (other.CompareTag("Jewel"))
        {
            Maze2Generator.instance.CollectJewel(other.GetComponent<Jewel>());
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Death"))
        {
            Death();
        }
    }

    void Death()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
