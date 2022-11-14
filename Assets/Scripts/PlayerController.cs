using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    //Grotte Echo//
    private Vector3 grottePoint = new Vector3(24, 1, 10);

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
        CheckEcho();
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

        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
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

    public AudioMixer mixer;
    public string MixerParameter = "Undefined Tracked Param";
    public float MinVal = 0;
    public float MaxVal = 0;

    private void CheckEcho()//put echo when inside the cave
    {            
        //Calcul de la distance au centre de la zone (ne pas oublier de transformer le vecteur centre du rep�re local au rep�re global)
        float distToT = Vector3.Distance(transform.TransformPoint(grottePoint), transform.position);
        float distanceGrotte = Vector3.Distance(transform.position, grottePoint);

        if ( distanceGrotte < 3)
        {
            //La valeur de l'effet : Min + (Max-Min) * distanceNormalis�e
            float effectVal = MinVal + (MaxVal - MinVal) * (1.0f - (distToT / 3));
            //On ajoute le param�tre au mixer
            mixer.SetFloat(MixerParameter, effectVal);
        }
        //else
        //desactive
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(grottePoint, 3);
    }
}
