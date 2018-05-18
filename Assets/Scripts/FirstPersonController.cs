using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FirstPersonController : NetworkBehaviour {
    public float mouseSensitivityX = 2;
    public float mouseSensitivityY = 2;
    public float walkSpeed = 6;
    public float jumpForce = 220;
    public LayerMask groundedMask;

    bool grounded;
    Vector3 moveAmount;
    float verticalLookRotation;
    Transform cameraTransform;
    Rigidbody rb;
    Animator anim;

    GameObject sceneCamera;

    private void Awake()
    {
        GetComponentInChildren<Camera>().enabled = false;
        GetComponentInChildren<AudioListener>().enabled = false;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        sceneCamera = Camera.main.gameObject;
    }

    public override void OnStartLocalPlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GetComponentInChildren<Renderer>().material.color = Color.magenta;
        cameraTransform = GetComponentInChildren<Camera>().transform;
        GetComponentInChildren<Camera>().enabled = true;
        GetComponentInChildren<AudioListener>().enabled = true;
        sceneCamera.SetActive(false);
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);

        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
        cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        anim.SetFloat("h", inputX);
        anim.SetFloat("v", inputY);

        Vector3 moveDir = new Vector3(inputX, 0, inputY).normalized;
        moveAmount = moveDir * walkSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
                rb.AddForce(transform.up * jumpForce);
        }

        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1 + 0.1f, groundedMask))
            grounded = true;
        else
            grounded = false;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }
    }

    void FixedUpdate()
    {
        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + localMove);
    }

    private void OnDisable()
    {
        if (isLocalPlayer)
            if (sceneCamera != null)
                sceneCamera.SetActive(true);
    }
}
