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

    [SerializeField] Transform personalCamera;
    [SerializeField] float speed;
    [SerializeField] float gravityValue;
    [SerializeField] float jumpHeight;

    [SerializeField] GameObject flyingSphere;
    public enum GameState { Jumping, Flying, End };
    GameState _currentState;
    public GameState currentState //if this is flying, automatically make the flying aura appear
    {
        get { return _currentState; }
        set { _currentState = value; flyingSphere.SetActive(value == GameState.Flying); }
    }

    float currentYVelocity;

    void Awake()
    {
        instance = this;
        charController = GetComponent<CharacterController>();
        currentState = GameState.Jumping;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (currentState == GameState.Jumping) //if jumping around, calculate movement for jumping/falling
        {
            if (charController.isGrounded) //automatically jump when this is grounded
                currentYVelocity = Mathf.Sqrt(jumpHeight * gravityValue);

            move.y = currentYVelocity;
            currentYVelocity += gravityValue * Time.deltaTime;

            if (this.transform.position.y < -10f) //die if you fall below the level
                Maze2Generator.instance.EndGame(false);
        }
        else if (currentState == GameState.Flying) //if flying, don't move up/down
        {
            move.y = 0;
        }
        
        if (currentState != GameState.End) //if the game hasn't ended, move player and camera
        {
            charController.Move(speed * Time.deltaTime * move);
            personalCamera.transform.position = new Vector3(this.transform.position.x, 30, this.transform.position.z - 12.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Jewel")) //collect the jewel and destroy it, whether flying or jumping
        {
            Maze2Generator.instance.CollectJewel(other.GetComponent<Jewel>());
            Destroy(other.gameObject);
        }

        if (currentState == GameState.Jumping) //only when jumping around
        {
            if (other.CompareTag("Falling Platform")) //make the platform start shrinking
            {
                Maze2Cell cell = other.GetComponent<Maze2Cell>();
                if (!cell.shrinking)
                {
                    cell.shrinking = true;
                    Maze2Generator.instance.destroyedTiles++;
                    StartCoroutine(cell.ShrinkAway());
                }
            }
            else if (other.CompareTag("Death")) //die if you hit a death
            {
                Maze2Generator.instance.EndGame(false);
            }
            else if (other.CompareTag("Flying")) //trigger flying for 2 seconds, where you have faster speed
            {
                Destroy(other.gameObject);
                speed *= 1.5f;
                currentState = GameState.Flying;
                Invoke(nameof(StopFlying), 2f);
            }
        }
    }

    void StopFlying()
    {
        if (currentState == GameState.Flying) //if this is flying, stop flying
        {
            speed /= 1.5f;
            currentState = GameState.Jumping;
        }
    }
}
