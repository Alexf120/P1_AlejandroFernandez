using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5.0f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private int extraAirJumps = 1;

    [Header("Camera / Mouse Look")]
    [SerializeField] private Transform cameraPivot;   // pivot (pitch)
    [SerializeField] private Transform cameraTransform; // ASIGNA AQUÍ la Main Camera (recomendado)
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 70f;

    private Rigidbody rb;

    private bool isGrounded = false;
    private int airJumpsUsed = 0;

    private float yaw;
    private float pitch;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraPivot != null)
            pitch = cameraPivot.localEulerAngles.x;
    }

    private void Update()
    {
        // ===== CÁMARA / ROTACIÓN CON MOUSE =====
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mx;
        pitch -= my;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // gira el jugador en Y
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // inclina la cámara en el pivot
        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // ===== MOVIMIENTO (WASD RELATIVO A LA CÁMARA) =====
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Transform cam = cameraTransform != null ? cameraTransform : transform;

        Vector3 camForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        Vector3 camRight   = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;

        Vector3 movement = (camRight * horizontal + camForward * vertical).normalized;
        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        // ===== SALTO (PERMITE EN EL AIRE) =====
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
                airJumpsUsed = 0;
            }
            else if (airJumpsUsed < extraAirJumps)
            {
                Jump();
                airJumpsUsed++;
            }
        }
    }

    private void Jump()
    {
        Vector3 v = rb.linearVelocity; // Unity 6
        v.y = 0f;
        rb.linearVelocity = v;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            airJumpsUsed = 0;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
