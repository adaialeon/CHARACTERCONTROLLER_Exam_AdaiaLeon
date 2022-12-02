using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thirdpersoncontroller : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;
    public Transform cam;
    public Transform LookAtTransform;

    //velocidad, salto y gravedad
    public float speed = 5;
    public float jumpHeight = 1;
    public float gravity = -9.81f;

    //ground sensor
    public bool isGrounded;
    public Transform groundSensor;
    public float sensorRadius = 0.1f;
    public LayerMask ground;
    private Vector3 playerVelocity;

    //rotacion del personaje
    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    //movimiento del raton con virtual camera
    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;

    //Blendtree
    public float VelX;
    public float VelZ;


    // Start is called before the first frame update
    void Start()
    {
        //Character Controller
        controller = GetComponent<CharacterController>();

        //Animator
        anim = GetComponentInChildren<Animator>();

        //Esconder icono del ratón
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        MovementTPS();
        Jump();
    }

    void MovementTPS()
    {
        float z = Input.GetAxisRaw("Vertical");
        float x = Input.GetAxisRaw("Horizontal");
        //Creamos Vector3 y asignamos imputs de movimiento
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        //Blendtree
        anim.SetFloat("VelX", x);
        anim.SetFloat("VelZ", z);




        if(move != Vector3.zero)
        {
            //Float para almacenar la posicion
            //Atan2calculo angulo donde mira el personaje
            //Mltiplicar Rad2Deg para valor en grados y le sumamos la rotacion de la camara en Y para que rote
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            //SmoothDamp transicion entre el angulo y el de la camara(rotación del personaje suave)
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.eulerAngles.y, ref turnSmoothVelocity, turnSmoothTime);

            //Rotación del personaje
            transform.rotation = Quaternion.Euler(0, angle, 0);
            
            //Vector3 para que el personaje camine hacia donde mira
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            //Multiplicamos Vector por la velocidad, para que se mueva
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
    }

    void Jump()
    {
        //Boleana isGrounded 
        //CheckSphere esfera para la poscion, radio y layer 
        //Contacto de la esfera
        isGrounded = Physics.CheckSphere(groundSensor.position, sensorRadius, ground);
        anim.SetBool("Jump", !isGrounded);

        //Playervelocity
        if(isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        //Salto del personaje
        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity); 
        }

        //Sumar valor de la gravedad
        playerVelocity.y += gravity * Time.deltaTime;
        //Empuja el personaje hacia abajo
        controller.Move(playerVelocity * Time.deltaTime);
    }
}