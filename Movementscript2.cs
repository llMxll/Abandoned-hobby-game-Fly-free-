using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movementscript2 : MonoBehaviour
{

    public int movementmode = 2;
    public bool running;
    public bool flareing;
    public bool diving;

    public float ZCoord;
    public float XCoord;

    public float WingLength;
    public float WingWidth;

    public float FlightMasterCoeff;
    public float LiftCoeff;
    public float DragZCoeff; // Forward/back
    public float DragXCoeff; // Leftright
    public float DragYCoeff; // Back
    public float AirDensity;

    private float WingArea; // WingLength x WingWidth. L proport to w
    private float VelocityFB; // L proport to v squared
    private float VelocityLR;
    private float VelocityUD;
    private float Angle; //L 100% at 14degrees, D causes stalls after 15degrees 100% at 90degrees
    private float AngleEffect;
    private float WingLoading; //Mass/Wingarea 3g/cm2 to 0.1 g/cm2 lower = glider/easier to get lift

    private float Lift; //WingArea + velocity squared
    private float Weight1; // mass x gravity
    private float DragZ; // WingArea x velocity squared x angle effect
    private float DragX;
    private float DragY;
    private float Thrust; //To do with flap?


    // Movement modes 1 = ground, 2 = air

    // Use this for initialization
    void Start()
    {
        WingArea = WingLength * WingWidth;
    }

    // Update is called once per frame
    void Update()
    {
        // Camera
        Vector3 moveCamTo = transform.position - transform.forward * 6.0f + Vector3.up * 1.0f;
        float bias = 0.96f;
        Camera.main.transform.position = Camera.main.transform.position * bias + moveCamTo * (1.0f - bias);
        Camera.main.transform.LookAt(transform.position + transform.forward * 1.0f);

        //Check movementmode
        switch (movementmode)
        {

            //walk ------------------
            case 1:
                GetComponent<Rigidbody>().velocity = new Vector3 (0f,0f,0f);
                GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);


                //Align with ground
                RaycastHit hit;
                if (Physics.Raycast(transform.position, -Vector3.up, out hit))
                {
                    Vector3 targetPosition = hit.point;
                    transform.position = targetPosition;
                    Vector3 targetRotation = new Vector3(hit.normal.x, transform.eulerAngles.y, hit.normal.z);
                    transform.eulerAngles = targetRotation;
                }

                // Check if running
                if (Input.GetKeyDown("left shift") == true)
                {
                    running = true;
                }
                else if (Input.GetKeyUp("left shift") == true)
                {
                    running = false;
                }

                //Forward movement
                if (running == false)
                {
                    transform.Translate(0f, 0f, Input.GetAxis("Vertical") * 2 * Time.deltaTime);
                }
                else if (running == true)
                {
                    transform.Translate(0f, 0f, Input.GetAxis("Vertical") * 4 * Time.deltaTime);
                }
                // turn left and right
                transform.Rotate(0f, Input.GetAxis("Horizontal"), 0f);

                //Takeoff
                if (Input.GetMouseButtonDown(0) == true)
                {
                    GetComponent<Rigidbody>().isKinematic = false;
                    transform.Translate(0f, 6, 0f);
                    movementmode = 2;
                    running = false;
                }

                break;
            // Flight --------------------
            case 2:

                // Velocity detector
                var locVel = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
                VelocityFB = locVel.z;
                VelocityLR = locVel.x;
                VelocityUD = locVel.y;

                //Air density
                AirDensity = 1 - transform.position.y / 5000;

                //Flare mode
                if (Input.GetMouseButtonDown(1) == true && diving == false)
                {
                    flareing = true;
                }

                else if (Input.GetMouseButtonUp(1) == true)
                {
                    flareing = false;
                }

                //Gain speed with dive
                if (Input.GetKeyDown("left shift") == true && flareing == false)
                {
                    diving = true;
                }

                else if (Input.GetKeyUp("left shift") == true)
                {
                    diving = false;
                }

                //Lift
                Lift = LiftCoeff * ((AirDensity * (VelocityFB * Mathf.Abs(VelocityFB))) / 2) * WingLength * 2 - (transform.forward.y);

                if (flareing == false && diving == false)
                {
                    GetComponent<Rigidbody>().AddForce(transform.up * Lift * FlightMasterCoeff * Time.deltaTime);
                }



                //Drag - split to 3 axis, z-forward/back, x-Left/right, y-up/down
                DragZ = ((AirDensity * Mathf.Abs(VelocityFB) * VelocityFB) / 2) * WingArea * 2;
                DragX = ((AirDensity * Mathf.Abs(VelocityLR) * VelocityLR) / 2) * WingArea * 2;
                DragY = ((AirDensity * Mathf.Abs(VelocityUD) * VelocityUD) / 2) * WingArea * 2;

                if (flareing == false && diving == false)
                {
                    GetComponent<Rigidbody>().AddForce(transform.forward * -DragZ * DragZCoeff * FlightMasterCoeff * Time.deltaTime);
                    GetComponent<Rigidbody>().AddForce(transform.right * -DragX * DragXCoeff * FlightMasterCoeff * Time.deltaTime);
                    GetComponent<Rigidbody>().AddForce(transform.up * -DragY * DragYCoeff * FlightMasterCoeff * Time.deltaTime);
                }

                if (flareing == true)
                {
                    GetComponent<Rigidbody>().AddForce(transform.forward * -DragZ * DragYCoeff * FlightMasterCoeff * Time.deltaTime);
                    GetComponent<Rigidbody>().AddForce(transform.right * -DragX * DragXCoeff * FlightMasterCoeff * Time.deltaTime);
                    GetComponent<Rigidbody>().AddForce(transform.up * -DragY * DragYCoeff * FlightMasterCoeff * Time.deltaTime);
                }

                if (diving == true)
                {
                    GetComponent<Rigidbody>().AddForce(((transform.forward * -DragZ * DragZCoeff)/10) * FlightMasterCoeff * Time.deltaTime);
                    GetComponent<Rigidbody>().AddForce(transform.right * -DragX * DragXCoeff * FlightMasterCoeff * Time.deltaTime);
                    GetComponent<Rigidbody>().AddForce(((transform.up * -DragY * DragZCoeff)/10) * FlightMasterCoeff * Time.deltaTime);
                }

                //Thrust
                Thrust = WingArea * AirDensity * 2000;

                //Flap
                if (Input.GetMouseButtonDown(0) == true)
                {
                    GetComponent<Rigidbody>().AddForce(transform.forward * Thrust * FlightMasterCoeff);
                }

                //Max and min glide speed

                // pitch and yaw
                transform.Rotate(Input.GetAxis("Vertical") * 1, 0.0f, -Input.GetAxis("Horizontal") * 2);
                break;
        }

        // collide with terrain

        //end void update
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collide");
        movementmode = 1;
        flareing = false;
        diving = false;
    }
}
