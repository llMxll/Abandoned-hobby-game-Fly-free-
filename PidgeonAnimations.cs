using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PidgeonAnimations : MonoBehaviour
{

    private Animator anim;

    // Use this for initialization
    void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Dive on shift click
        if (Input.GetKeyDown("left shift") == true)
        {
            anim.SetBool("Dive", true);
        }
        else if (Input.GetKeyUp("left shift") == true)
        {
            anim.SetBool("Dive", false);
        }

        // Flare on right click
        if (Input.GetMouseButtonDown(1) == true)
        {
            anim.SetBool("Flare", true);
        }
        else if (Input.GetMouseButtonUp(1) == true)
        {
            anim.SetBool("Flare", false);
        }

        // Flap on left mouse click
        if (Input.GetMouseButtonDown(0) == true)
        {
            anim.SetTrigger("Glidetoflap");
            anim.SetBool("Onground", false);
        }

        //Walk when w pressed or run if shift + w pressed
        if (Input.GetKeyDown("left shift") == true)
        {
            anim.SetBool("Run", true);
        }
        else if (Input.GetKeyUp("left shift") == true)
        {
            anim.SetBool("Run", false);
        }

        if (Input.GetKeyDown("w") == true)
        {
               anim.SetInteger("Walkspeed", 1);
        }
        //walking to idle
        else if (Input.GetKeyUp("w") == true)
        {
            anim.SetInteger("Walkspeed", 0);
        }
    }

    // Stand when hitting terrain
    private void OnCollisionEnter(Collision collision)
    {
        anim.SetBool("Onground", true);
    }

}
