using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public UIControler uIControler;
    public BulletScript bullet; 

    [SerializeField] public float playerHealth = 300f;
    [SerializeField] private float playerSpeed = 4f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 8f;
    private float getDamage;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private Transform cameraTransform;
    private Animator playerAnimator;
    private LevelManager levelManager;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    

    private bool groundedPlayer;

    private void Awake()
    {
        levelManager = FindObjectOfType<LevelManager>();
        
        playerAnimator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start() 
    {
        getDamage = bullet.bulletDamage;

        uIControler = GameObject.FindGameObjectWithTag("UISystem").GetComponent<UIControler>();
        uIControler.SetHealth(playerHealth.ToString() + " HP");
    }


    void Update()
    {
        if(!PauseMenu.isPaused)
        {
            GroundedCheck();
            Move();
            CameraRotation();
            Jump();
        }
        
    }
    
    private void GroundedCheck()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y <= 0)
        {
            playerVelocity.y = 0f;
            playerAnimator.SetBool("inAir", false);
            playerAnimator.SetBool("Grounded" , true);
        }

        if(!groundedPlayer)
        {
            playerAnimator.SetBool("inAir", true);
        }
    }

    private void Move()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x , 0f, input.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;

        if (sprintAction.IsPressed())
        {
            controller.Move(move * Time.deltaTime * sprintSpeed);
        }
        else
        {
            controller.Move(move * Time.deltaTime * playerSpeed);
        }

        if(moveAction.IsPressed() && !sprintAction.IsPressed())
        {
            playerAnimator.SetBool("Walk", true);
            playerAnimator.SetBool("Run", false);
        }
        else if(sprintAction.IsPressed() && moveAction.IsPressed())
        {
            playerAnimator.SetBool("Run", true);
            playerAnimator.SetBool("Walk", false);
        }
        else
        {
            playerAnimator.SetBool("Walk", false);
            playerAnimator.SetBool("Run", false);
        }
    }

    private void Jump()
    {
        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            playerAnimator.SetTrigger("Jump");
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void CameraRotation()
    {
        float targetAngle = cameraTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "Bullet")
        {
            TakeDamage(getDamage);
        }
        uIControler.SetHealth(playerHealth.ToString() + " HP");
    }

    public void TakeDamage(float amount)
    {
        playerHealth -= amount;

        if (playerHealth <= 0f)
        {
            Die();
            levelManager.LoadGameOver();
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
