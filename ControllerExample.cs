using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerExample : MonoBehaviour
{
    //Impale
    public GameObject impaleOBJ;

    //What controller is this
    public bool isBirdsEyeView;
    public LayerMask layerMask;
    public Vector3 birdsEyeOffset;

    //Movement
    public float speed = 10.0f;
    private float translation;
    private float straffe;

    //Camera
    [SerializeField]
    public float sensitivity = 5.0f;
    [SerializeField]
    public float smoothing = 2.0f;
    // get the incremental value of mouse moving
    private Vector2 mouseLook;
    // smooth the mouse moving
    private Vector2 smoothV;
    public Transform cam;
    private Transform playerTransform;

    // Use this for initialization
    void Start()
    {
        playerTransform = transform;
        cam = Camera.main.transform;
        // turn off the cursor
        if (!isBirdsEyeView) { Cursor.lockState = CursorLockMode.Locked; } else { cam.transform.SetParent(null); }
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(straffe, 0, translation);

        //Camera
        if(isBirdsEyeView)
        {
            cam.transform.position = playerTransform.position + birdsEyeOffset;
        }
        else
        {
            var movementDirection = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            movementDirection = Vector2.Scale(movementDirection, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
            smoothV.x = Mathf.Lerp(smoothV.x, movementDirection.x, 1f / smoothing);
            smoothV.y = Mathf.Lerp(smoothV.y, movementDirection.y, 1f / smoothing);
            mouseLook += smoothV;
            cam.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
            transform.localRotation = Quaternion.AngleAxis(mouseLook.x, transform.up);
        }

        //Get input
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Impale();
        }
    }

    void Impale()
    {
        if (isBirdsEyeView)
        {
            RaycastHit hit;
            Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, layerMask))
            {
                //We calculate the hit position
                var spawnCalculation = transform.position + transform.forward;
                //Get the correct rotation using ray
                Quaternion rotation = Quaternion.LookRotation(hit.point - transform.position, Vector3.up);
                //Create the first impale object that wil be the parent and will duplicate itself
                Instantiate(impaleOBJ, spawnCalculation, rotation);
            }
        }
        else
        {
            //First and third person
            //We calculate our current forward direction
            var spawnCalculation = transform.position + transform.forward;
            //Create the first impale object that wil be the parent and will duplicate itself
            Instantiate(impaleOBJ, spawnCalculation, transform.rotation);
        }
    }
}