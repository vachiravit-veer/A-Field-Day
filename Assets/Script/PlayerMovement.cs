using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public GameObject player;        //Public variable to store a reference to the player game object


    private Vector3 offset;            //Private variable to store the offset distance between the player and camera
    private Vector3 curPos,lastPos;

    private bool onMove, doneMove;
    // Use this for initialization
    private Vector3 originalPos;

    public float smooth = 0.25f;


    void Start()
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
        originalPos = transform.position;
 
    }

    void Update()
    {

        curPos = player.transform.position;
        if (curPos == lastPos)
        {
            if(onMove)
            {
                doneMove = true;
                player.transform.LookAt(transform);
            }
            print("Not moving");
            onMove = false;
            
        }
        else
        {
            onMove = true;
        }

        if(!onMove&&doneMove)
        {
            transform.position = originalPos;
            doneMove = false;
        }


        lastPos = curPos;

    }

    private void LateUpdate()
    {
  
        if (onMove)
        {
            Vector3 des = player.transform.position + offset;
            Vector3 smoothdes = Vector3.Lerp(transform.position, des, smooth*Time.deltaTime);

            //transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, Smoothing);
            transform.position = smoothdes;

        }

    }

}
