using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    private Rigidbody playerRb;
    private NoiseStatus n;
    private bool isOnGround;

    public float speed = 7;
    private float horizontalInput;
    private float verticalInput;
    private float sensitivity = 2f;

    [Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
    [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;

    Vector2 rotation = Vector2.zero;
    const string xAxis = "Mouse X"; 
    const string yAxis = "Mouse Y";

    void Start()
    {
        //rend le curseur invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        n = GetComponent<NoiseStatus>();
        n.NoiseLevel = 0;
        playerRb = GetComponent<Rigidbody>();
        isOnGround = true;

    }

    void Update()
    {
        MovePlayer();
        FirstPersonView();
    }
    void MovePlayer()
    {
        // Avancer, reculer
        verticalInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.forward * Time.deltaTime * speed * verticalInput);

        // Droite, gauche
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput);

        // Sauter
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            //n.NoiseLevel = 1;
            playerRb.AddForce(Vector3.up * 20, ForceMode.Impulse);
            isOnGround = false;
        }

        // Courir
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            n.NoiseLevel = 1;
            speed = 7;

        }else if (Input.GetKeyUp(KeyCode.LeftShift))
        {   
            n.NoiseLevel = 0;
            speed = 4;
        }
      
    }

    private void OnCollisionEnter(Collision collision)
    {
        isOnGround = true;//ne peut plus sauter tant que player est en l'air (pour empecher le double saut)
    }
    private void FirstPersonView()
    {
        rotation.x += Input.GetAxis(xAxis) * sensitivity;
        rotation.y += Input.GetAxis(yAxis) * sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);
        transform.localRotation = xQuat * yQuat;
    }
    
}
