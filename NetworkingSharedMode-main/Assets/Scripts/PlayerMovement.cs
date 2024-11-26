using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : NetworkBehaviour
{
    bool eventCalled = false;
    private Vector3 _velocity;
    private bool _jumpPressed;
    public GameObject playerHUD;
    public int playerScore;
    public static PlayerMovement Local { get; set; }
    public CharacterController _controller;
    public Transform playerModel;
            

    [FormerlySerializedAs("PlayerSpeed")] public float playerSpeed = 2f;

    [FormerlySerializedAs("JumpForce")] public float jumpForce = 5f;
    [FormerlySerializedAs("GravityValue")] public float gravityValue = -9.81f;

    [FormerlySerializedAs("Camera")] public Camera mainCamera;
    private void Awake()
    {

        Cursor.lockState = CursorLockMode.Locked;
        _controller = GetComponent<CharacterController>();
    }
    
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            
            
        }
        
        if (!HasStateAuthority) return;
        Local = this;
        playerHUD.gameObject.SetActive(true);
        mainCamera = Camera.main;
        if (mainCamera != null) 
            mainCamera.GetComponent<FirstPersonCamera>().Target = transform;
        mainCamera.GetComponent<FirstPersonCamera>().Authority = this.gameObject;
       
        if (HasInputAuthority)
        {
            Local = this;
            playerHUD.gameObject.SetActive(true);
        }
        else
        {
            playerHUD.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // FixedUpdateNetwork is only executed on the StateAuthority

        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }

        var cameraRotationY = Quaternion.Euler(0, mainCamera.transform.rotation.eulerAngles.y, 0);
        var move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * playerSpeed;

        _velocity.y += gravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded)
        {
            _velocity.y += jumpForce;
        }
        _controller.Move(move + _velocity * Runner.DeltaTime);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        _jumpPressed = false;
    }
}