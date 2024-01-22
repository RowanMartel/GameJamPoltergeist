using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameObject lookingAtObject;
    public GameObject heldObject;

    GameObject heldPosition;
    Vector3 heldPositionReset;
    public enum State
    {
        inactive,
        active
    }

    public State state = State.inactive;

    public float speed;
    bool jump = false;
    public float jumpForce = 8f;

    Rigidbody rb;
    GameObject cameraContainer;

    float cameraVertical = 0;

    float playerForward;
    float playerSideStep;
    float playerRotate;

    public bool isGrounded = true;

    public Ray playerView;
    float reachFactor;
    float mouseScrollFactor;
    void Start()
    {
        lookingAtObject = GameObject.Find("Floor");
        heldPosition = GameObject.Find("HeldPosition");
        heldPositionReset = heldPosition.transform.localPosition;

        // Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        cameraContainer = GameObject.Find("Player/CameraContainer");
    }

    // Update is called once per frame
    void Update()
    {
        playerView = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(playerView, out hit, 5))
        {
            if(hit.collider.gameObject != lookingAtObject)
            {
                if (lookingAtObject != null && lookingAtObject.CompareTag("Interactable")) lookingAtObject.GetComponent<Outline>().enabled = false;

                lookingAtObject = hit.collider.gameObject;

                if(lookingAtObject.CompareTag("Interactable")) lookingAtObject.GetComponent<Outline>().enabled = true;
            }
        }
        if (!Physics.Raycast(playerView, out hit, 3) && lookingAtObject != null)
        {
            if(lookingAtObject.CompareTag("Interactable")) lookingAtObject.GetComponent<Outline>().enabled = false;
            lookingAtObject = null;
        }

        if (Input.GetKeyDown(KeyCode.E)) Interact();

        if (state == State.active)
        {
            if (heldPosition.transform.localPosition.z >= 1.0f && heldPosition.transform.localPosition.z <= 2.0f)
            {
                heldPosition.transform.localPosition = new Vector3(heldPosition.transform.localPosition.x, heldPosition.transform.localPosition.y, heldPosition.transform.localPosition.z + Input.mouseScrollDelta.y * 0.1f);

                if (heldPosition.transform.localPosition.z > 2) heldPosition.transform.localPosition = heldPosition.transform.localPosition = new Vector3(heldPosition.transform.localPosition.x, heldPosition.transform.localPosition.y, 2.0f);
                if (heldPosition.transform.localPosition.z < 1) heldPosition.transform.localPosition = heldPosition.transform.localPosition = new Vector3(heldPosition.transform.localPosition.x, heldPosition.transform.localPosition.y, 1.0f);
            }

            MoveCamera();

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                jump = true;
                isGrounded = false;
            }

            playerForward = Input.GetAxis("Vertical");
            playerSideStep = Input.GetAxis("Horizontal");
            playerRotate = Input.GetAxis("Mouse X");

            transform.Rotate(0.0f, playerRotate, 0.0f);
        }
    }
    void FixedUpdate()
    {
        if (heldObject != null && state == State.active)
        {
            Vector3 direction = heldObject.transform.position - heldPosition.transform.position;
            float distance = direction.magnitude;
            Vector3 force = direction.normalized;

            if (distance > 0) heldObject.GetComponent<Rigidbody>().AddForce(-force * distance * 10, ForceMode.VelocityChange);
            heldObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            heldObject.transform.rotation = transform.rotation;

            heldObject.transform.Rotate(heldObject.GetComponent<Pickupable>().heldRotationMod);
        }
        else if(heldObject != null && state == State.inactive)
        {
            Vector3 heldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.5f));
            Vector3 direction = heldObject.transform.position - heldPosition;
            float distance = direction.magnitude;
            Vector3 force = direction.normalized;

            if (distance > 0) heldObject.GetComponent<Rigidbody>().AddForce(-force * distance * 10, ForceMode.VelocityChange);

            heldObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            heldObject.transform.rotation = transform.rotation;

            heldObject.transform.Rotate(heldObject.GetComponent<Pickupable>().heldRotationMod);
        }

        if (state == State.active)
        {
            if (jump)
            {
                rb.AddRelativeForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jump = false;
            }
            if (!isGrounded)
            {
                rb.AddForce(-Vector3.up * jumpForce);
            }

            DoPlayerMovement();
        }
    }

    void MoveCamera()
    {
        float camV = cameraVertical + Input.GetAxis("Mouse Y");

        cameraVertical = Mathf.Clamp(camV, -90f, 80f);

        float flipCamV = camV * -1;

        cameraContainer.transform.localRotation = Quaternion.Euler(flipCamV, 0, 0);
    }

    void DoPlayerMovement()
    {
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.forward * speed);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.forward * speed * 0.75f);
            rb.AddForce(transform.right * speed * 0.75f);
        }

        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            rb.AddForce(transform.right * speed);
        }

        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * speed * 0.75f);
            rb.AddForce(transform.right * speed * 0.75f);
        }

        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-transform.forward * speed);
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-transform.forward * speed * 0.75f);
            rb.AddForce(-transform.right * speed * 0.75f);
        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.right * speed);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            rb.AddForce(transform.forward * speed * 0.75f);
            rb.AddForce(-transform.right * speed * 0.75f);
        }
    }

    void Interact()
    {
        heldPosition.transform.localPosition = heldPositionReset;

        if (heldObject == null && lookingAtObject != null && lookingAtObject.CompareTag("Interactable"))
        {
            Pickupable pickupable = lookingAtObject.GetComponent<Pickupable>();

            if (pickupable != null)
            {
                heldObject = lookingAtObject;
                Physics.IgnoreCollision(heldObject.GetComponent<Collider>(), GetComponent<Collider>());

                heldObject.GetComponent<Rigidbody>().useGravity = false;
                heldObject.GetComponent<Outline>().enabled = false;

                //heldObject.transform.position = midHold.transform.position;

                int layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                heldObject.layer = layerIgnoreRaycast;
            }
            lookingAtObject.GetComponent<Interactable>().Interact();
        }
        else if(heldObject != null && (lookingAtObject == null || lookingAtObject.tag != "Interactable"))
        {
            heldObject.layer = 0;
            heldObject.GetComponent<Rigidbody>().useGravity = true;
            Physics.IgnoreCollision(heldObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
            heldObject = null;
        }
        else if (heldObject != null && lookingAtObject.CompareTag("Interactable"))
        {
            Pickupable pickupable = lookingAtObject.GetComponent<Pickupable>();

            if (pickupable != null)
            {
                heldObject.layer = 0;
                heldObject.GetComponent<Rigidbody>().useGravity = true;
                Physics.IgnoreCollision(heldObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
                heldObject = null;

                if (heldObject != null)
                    Physics.IgnoreCollision(heldObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
                heldObject = lookingAtObject;
                Physics.IgnoreCollision(heldObject.GetComponent<Collider>(), GetComponent<Collider>());

                heldObject.GetComponent<Rigidbody>().useGravity = false;
                heldObject.GetComponent<Outline>().enabled = false;

                //heldObject.transform.position = midHold.transform.position;

                int layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                heldObject.layer = layerIgnoreRaycast;
            }
            lookingAtObject.GetComponent<Interactable>().Interact();
        }
    }

    public void Die()
    {
        state = State.inactive;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") isGrounded = true;
    }

    public void TogglePlayerControl()
    {
        switch(state)
        {
            case State.active:
                state = State.inactive;
                Cursor.lockState = CursorLockMode.Confined;
                break;

            case State.inactive:
                state = State.active;
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }
}
