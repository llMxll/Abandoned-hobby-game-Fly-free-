using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movementscript : MonoBehaviour {
    public float speed = 10.0f;
    public float maxglidespeed;
    public float minglidespeed;
    public float forwarddrag;
    public float downdrag;
    public float sidedrag;
    public float flaredrag;
    public int movementmode = 2;
    public bool running;
    public bool flareing;
    public bool diving;
    private float tempdowndrag;
    private float tempforwarddrag;
    private float tempsidedrag;


    // Movement modes 1 = ground, 2 = air

    // Use this for initialization
    void Start () {
        tempforwarddrag = forwarddrag;
        tempdowndrag = downdrag;
        tempsidedrag = sidedrag;
	}

    // Update is called once per frame
    void Update() {
        // Camera
        Vector3 moveCamTo = transform.position - transform.forward * 12.0f + Vector3.up * 5.0f;
        float bias = 0.96f;
        Camera.main.transform.position = Camera.main.transform.position * bias + moveCamTo * (1.0f - bias);
        Camera.main.transform.LookAt(transform.position + transform.forward * 30.0f);

        //Check movementmode
        switch (movementmode) {

            //walk ------------------
            case 1:
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
                    Debug.Log("running");
                }
                else if (Input.GetKeyUp("left shift") == true)
                {
                    running = false;
                    Debug.Log("walking");
                }
                
                //Forward movement
                if (running == false)
                {
                    transform.Translate(0f, 0f, Input.GetAxis("Vertical") * 6 * Time.deltaTime);
                }
                else if (running == true)
                {
                    transform.Translate(0f, 0f, Input.GetAxis("Vertical") * 18 * Time.deltaTime);
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

                var locVel = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
                Debug.Log(locVel);

                /* //forward movement
                 transform.position += transform.forward * Time.deltaTime * speed;

                 //gain/loss speed with angle
                 speed -= transform.forward.y * Time.deltaTime * 20.0f;

                 //Gain speed with flap
                 if (Input.GetMouseButtonDown(0) == true)
                 {
                     speed += 5;              
                 }

                 //Lose speed with flare
                 if (Input.GetMouseButtonDown(1) == true)
                 {
                     flareing = true;
                 }

                 else if (Input.GetMouseButtonUp(1) == true)
                 {
                     flareing = false;
                 }

                 if (flareing == true)
                 {
                     speed -= 20 * Time.deltaTime;
                 }


                 //Max and min glide speed
                 if (speed > maxglidespeed)
                 {
                     speed = maxglidespeed;
                 }

                 if (speed < minglidespeed)
                 {
                     speed = minglidespeed;
                 }
                 */

                //forward or back movement
                if (Input.GetMouseButtonDown(0) == true)
                {
                    GetComponent<Rigidbody>().AddForce(transform.forward * 700);
                }
                //Up/down movement
                if (diving == false)
                {
                    GetComponent<Rigidbody>().AddForce(transform.up * locVel.z / 4);
                    GetComponent<Rigidbody>().AddForce(transform.up * locVel.y / 4);
                }

                if (Input.GetMouseButtonDown(0) == true)
                {
                    GetComponent<Rigidbody>().AddForce(transform.up * 250);
                }

                // Drag
                GetComponent<Rigidbody>().AddForce(transform.up * locVel.y * downdrag * -1);
                GetComponent<Rigidbody>().AddForce(transform.forward * locVel.z * forwarddrag * -1);
                GetComponent<Rigidbody>().AddForce(transform.right * locVel.x * sidedrag * -1);

                //Lose speed with flare
                if (Input.GetMouseButtonDown(1) == true)
                {
                    flareing = true;
                }

                else if (Input.GetMouseButtonUp(1) == true)
                {
                    flareing = false;
                }

                if (flareing == true)
                {
                    GetComponent<Rigidbody>().AddForce(transform.forward * locVel.z * flaredrag * -1);
                }

                //Gain speed with dive
                if (Input.GetKeyDown("left shift") == true)
                {
                    diving = true;
                }

                else if (Input.GetKeyUp("left shift") == true)
                {
                    diving = false;
                }

                if (diving == true)
                {
                    forwarddrag = 0;
                    downdrag = 0;
                    sidedrag = 0;
                }

                if (diving == false)
                {
                    forwarddrag = tempforwarddrag;
                    downdrag = tempdowndrag;
                    sidedrag = tempsidedrag;
                }

                //Max and min glide speed


                // pitch and yaw
                transform.Rotate(Input.GetAxis("Vertical") * 2, 0.0f, -Input.GetAxis("Horizontal") * 4);
                break;
    }
        // collide with terrain
        float terrainHeightWhereWeAre = Terrain.activeTerrain.SampleHeight(transform.position);

        if (terrainHeightWhereWeAre > transform.position.y)
        {
            transform.position = new Vector3(transform.position.x, terrainHeightWhereWeAre, transform.position.z);
            GetComponent<Rigidbody>().isKinematic = true;
            movementmode = 1;
            speed = 30;
            flareing = false;
            diving = false;
        }
        //end void update
	}
}
