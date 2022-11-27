using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopMovement : NetworkBehaviour
{

    [SerializeField] private float mouseSens = 100f;
    [SerializeField] private float xRotation = 0f;
    [SerializeField] private float yRotation = 0f;
    [SerializeField] private float _Speed = 3f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Camera FPCamera;

    private CharacterController characterController;
    private PlayerController playerController;

    private float thisFrameMouse;
    private bool isGrounded;
    private bool isAiming;
    private bool isFalling;
    private float gravity;
    private float[] thisFrameMovement;

    private void Awake()
    {
        //_VelocityForNonRigidBodies = GetComponent<VelocityForNonRigidBodies>();
        //_DesktopAnimationStateController = GetComponent<DesktopAnimationStateController>();
        //_PlayerController = GetComponent<PlayerController>();
        thisFrameMovement = new float[2];
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (base.IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            characterController = GetComponent<CharacterController>();
            Camera.main.gameObject.SetActive(false);
            //_PlayerController.PlayerCamera.gameObject.SetActive(true);
            //_PlayerController.Ragdoll.gameObject.SetActive(false);
            //transform.SetLayerRecursively(LayerMask.NameToLayer("LocalPlayer"));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!base.IsOwner)
            return;

        AimWithMouse();
        MoveWithKeyboard();
        CheckForGrounded();

    }

    /// <summary>
    /// Calculate camera/transform rotation using mouse axis
    /// </summary>
    private void AimWithMouse()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        thisFrameMouse = mouseX;

        //Apply character/camera rotations
        Camera.main.transform.parent.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void CheckForGrounded()
    {
        RaycastHit ray;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out ray, 0.2f, ~GamemodeTest.instance.GetLocalPlayerLayer))
        {
            if (gravity <= 0)
            {
                isGrounded = true;
                gravity = 0;
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    /// <summary>
    /// Scan keyboard for movement keys and apply to Character Controller
    /// </summary>
    private void MoveWithKeyboard()
    {
        float movementX = Input.GetAxis("Horizontal");
        float movementZ = Input.GetAxis("Vertical");
        float movementY = 0f;

        //Sprint input
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementX *= 2f;
            movementZ *= 2f;
        }

        //Jump input
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            gravity = 3.5f;
            isGrounded = false;
        }

        //The frame movement used for Controller Move
        Vector3 frameMovement = transform.right * movementX + transform.forward * movementZ + transform.up * movementY;

        //Apply gravity
        if (!isGrounded)
        {
            gravity -= 9.81f * Time.deltaTime;
            frameMovement.y += gravity;
        }

        if (gravity < 0 && !isGrounded)
            isFalling = true;
        else
            isFalling = false;

        //Apply character movement
        Vector3 frameVelocity = frameMovement;
        characterController.Move(frameVelocity * _Speed * Time.deltaTime);

        //Set thisFrameMovement for the Animator
        thisFrameMovement[0] = movementX;
        thisFrameMovement[1] = movementY;
    }
}
