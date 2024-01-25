using UnityEngine;

public class PlayerController : MonoBehaviour
{
    protected GameObject go_lookingAtObject;
    protected GameObject go_heldPosition;
    protected GameObject go_heldObject;
    public GameObject Go_heldObject {  get { return go_heldObject; } }

    protected GameObject go_cameraContainer;

    protected Vector3 v3_heldPositionReset;

    protected Rigidbody rb_player;
    public enum State
    {
        inactive,
        active
    }

    protected State en_state = State.inactive;
    public State En_state { get { return en_state; } set { en_state = value; } }

    protected float flt_cameraVertical = 0;
    protected float flt_playerRotate;

    public Ray ray_playerView;

    public bool bl_hasJumped = false;
    public bool bl_isGrounded = true;

    public LayerMask lm;
    void Start()
    {
        go_lookingAtObject = GameObject.Find("Floor");
        go_heldPosition = GameObject.Find("HeldPosition");
        v3_heldPositionReset = go_heldPosition.transform.localPosition;

        // Cursor.lockState = CursorLockMode.Locked;

        rb_player = GetComponent<Rigidbody>();
        go_cameraContainer = GameObject.Find("Player/CameraContainer");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(bl_hasJumped + "|" + bl_isGrounded);

        DoPlayerView();

        if (Input.GetKeyDown(KeyCode.E)) Interact();

        if (en_state == State.active)
        {
            MoveCamera();

            // This allows the player to use the reach mechanic with their mousewheel to put props on hard to reach surfaces.
            DoPlayerReach();

            if (Input.GetKeyDown(KeyCode.Space) && bl_isGrounded)
            {
                bl_hasJumped = true;
                bl_isGrounded = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift)) LeanTween.moveLocalY(go_cameraContainer, 0.2f, 0.25f); // bl_isCrouching = true;
            if (Input.GetKeyUp(KeyCode.LeftShift)) LeanTween.moveLocalY(go_cameraContainer, 0.5f, 0.25f); // bl_isCrouching = false;
        }        
    }
    void FixedUpdate()
    {
        if (go_heldObject != null && en_state == State.active)
        {
            Vector3 direction = go_heldObject.transform.position - go_heldPosition.transform.position;
            float distance = direction.magnitude;
            Vector3 force = direction.normalized;

            if (distance > 0) go_heldObject.GetComponent<Rigidbody>().AddForce(-force * distance * 10, ForceMode.VelocityChange);
            go_heldObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            go_heldObject.transform.rotation = transform.rotation;

            go_heldObject.transform.Rotate(go_heldObject.GetComponent<Pickupable>().v3_heldRotationMod);
        }
        else if(go_heldObject != null && en_state == State.inactive)
        {
            Vector3 heldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.5f));
            Vector3 direction = go_heldObject.transform.position - heldPosition;
            float distance = direction.magnitude;
            Vector3 force = direction.normalized;

            if (distance > 0) go_heldObject.GetComponent<Rigidbody>().AddForce(-force * distance * 10, ForceMode.VelocityChange);

            go_heldObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            go_heldObject.transform.rotation = transform.rotation;

            go_heldObject.transform.Rotate(go_heldObject.GetComponent<Pickupable>().v3_heldRotationMod);
        }

        if (en_state == State.active)
        {
            if (bl_hasJumped)
            {
                rb_player.AddRelativeForce(Vector3.up * Settings.int_playerJumpForce, ForceMode.Impulse);
                bl_hasJumped = false;
            }
            if (!bl_isGrounded)
            {
                rb_player.AddForce(-Vector3.up * Settings.int_playerJumpForce);
            }

            DoPlayerMovement();
        }
    }

    // This handles the player's ability to look up and down, and rotate the player.
    void MoveCamera()
    {
        float camV = flt_cameraVertical + Input.GetAxis("Mouse Y");

        flt_cameraVertical = Mathf.Clamp(camV, -90f, 80f);

        float flipCamV = camV * -1;

        go_cameraContainer.transform.localRotation = Quaternion.Euler(flipCamV, 0, 0);

        flt_playerRotate = Input.GetAxis("Mouse X");

        transform.Rotate(0.0f, flt_playerRotate * Settings.flt_lookSensitivity, 0.0f);
    }

    public void LookAt(Vector3 position)
    {
        LeanTween.rotateLocal(go_cameraContainer, position, 0.25f);
    }

    // This handles the player's basic forward/backward/left/right movement, mapped to the WASD keys.
    void DoPlayerMovement()
    {
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            rb_player.AddForce(transform.forward * Settings.int_playerSpeed);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            rb_player.AddForce(transform.forward * Settings.int_playerSpeed * 0.75f);
            rb_player.AddForce(transform.right * Settings.int_playerSpeed * 0.75f);
        }

        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            rb_player.AddForce(transform.right * Settings.int_playerSpeed);
        }

        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        {
            rb_player.AddForce(-transform.forward * Settings.int_playerSpeed * 0.75f);
            rb_player.AddForce(transform.right * Settings.int_playerSpeed * 0.75f);
        }

        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            rb_player.AddForce(-transform.forward * Settings.int_playerSpeed);
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            rb_player.AddForce(-transform.forward * Settings.int_playerSpeed * 0.75f);
            rb_player.AddForce(-transform.right * Settings.int_playerSpeed * 0.75f);
        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            rb_player.AddForce(-transform.right * Settings.int_playerSpeed);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            rb_player.AddForce(transform.forward * Settings.int_playerSpeed * 0.75f);
            rb_player.AddForce(-transform.right * Settings.int_playerSpeed * 0.75f);
        }
    }

    // This handles the player's view at the crosshair and if pointed at an Interactable object, will activate the object's outline to indicate it can be interacted with.
    void DoPlayerView()
    {
        ray_playerView = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray_playerView, out hit, 5, lm))
        {
            if (hit.collider.gameObject != go_lookingAtObject)
            {
                if (go_lookingAtObject != null && go_lookingAtObject.CompareTag("Interactable")) go_lookingAtObject.GetComponent<Outline>().enabled = false;

                go_lookingAtObject = hit.collider.gameObject;

                if (go_lookingAtObject.CompareTag("Interactable")) go_lookingAtObject.GetComponent<Outline>().enabled = true;
            }
        }
        if (!Physics.Raycast(ray_playerView, out hit, 3, lm) && go_lookingAtObject != null)
        {
            if (go_lookingAtObject.CompareTag("Interactable")) go_lookingAtObject.GetComponent<Outline>().enabled = false;
            go_lookingAtObject = null;
        }
    }

    void DoPlayerReach()
    {
        if (go_heldPosition.transform.localPosition.z >= 1.0f && go_heldPosition.transform.localPosition.z <= 2.0f)
        {
            go_heldPosition.transform.localPosition = new Vector3(go_heldPosition.transform.localPosition.x, go_heldPosition.transform.localPosition.y, go_heldPosition.transform.localPosition.z + Input.mouseScrollDelta.y * 0.1f);

            if (go_heldPosition.transform.localPosition.z > 2) go_heldPosition.transform.localPosition = go_heldPosition.transform.localPosition = new Vector3(go_heldPosition.transform.localPosition.x, go_heldPosition.transform.localPosition.y, 2.0f);
            if (go_heldPosition.transform.localPosition.z < 1) go_heldPosition.transform.localPosition = go_heldPosition.transform.localPosition = new Vector3(go_heldPosition.transform.localPosition.x, go_heldPosition.transform.localPosition.y, 1.0f);
        }
    }

    // This handles the interactions between the player and the props and environment.
    void Interact()
    {
        go_heldPosition.transform.localPosition = v3_heldPositionReset;

        if (go_heldObject == null && go_lookingAtObject != null && go_lookingAtObject.CompareTag("Interactable"))
        {
            Pickupable pickupable = go_lookingAtObject.GetComponent<Pickupable>();

            if (pickupable != null)
            {
                go_heldObject = go_lookingAtObject;
                Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>());

                go_heldObject.GetComponent<Rigidbody>().useGravity = false;
                go_heldObject.GetComponent<Outline>().enabled = false;

                //heldObject.transform.position = midHold.transform.position;

                int layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                go_heldObject.layer = layerIgnoreRaycast;
                if(go_heldObject == GameManager.ghost.go_curHeldItem)
                {
                    GameManager.ghost.GetRobbed();
                }
            }
            go_lookingAtObject.GetComponent<Interactable>().Interact();
        }
        else if(go_heldObject != null && (go_lookingAtObject == null || go_lookingAtObject.tag != "Interactable"))
        {
            go_heldObject.layer = 0;
            go_heldObject.GetComponent<Rigidbody>().useGravity = true;
            Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
            go_heldObject = null;
        }
        else if (go_heldObject != null && go_lookingAtObject.CompareTag("Interactable"))
        {
            Pickupable pickupable = go_lookingAtObject.GetComponent<Pickupable>();

            if (pickupable != null)
            {
                go_heldObject.layer = 0;
                go_heldObject.GetComponent<Rigidbody>().useGravity = true;
                Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
                go_heldObject = null;

                if (go_heldObject != null)
                    Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
                go_heldObject = go_lookingAtObject;
                Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>());

                go_heldObject.GetComponent<Rigidbody>().useGravity = false;
                go_heldObject.GetComponent<Outline>().enabled = false;

                //heldObject.transform.position = midHold.transform.position;

                int layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                go_heldObject.layer = layerIgnoreRaycast;
                if (go_heldObject == GameManager.ghost.go_curHeldItem)
                {
                    GameManager.ghost.GetRobbed();
                }
            }
            go_lookingAtObject.GetComponent<Interactable>().Interact();
        }
    }
    public void Die()
    {
        en_state = State.inactive;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") bl_isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Floor") bl_isGrounded = false;
    }

    public void TogglePlayerControl()
    {
        switch(en_state)
        {
            case State.active:
                en_state = State.inactive;
                Cursor.lockState = CursorLockMode.Confined;
                break;

            case State.inactive:
                en_state = State.active;
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }
}
