using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{

    public Transform target;
    public float speed,speed2;
    public GameObject objectToMove, uiToDisable;
    public Camera cam;

    private bool needToMove, isZoomed, reZoomed = false;

    public float targetFov = 13.0f;
    public float normalFov = 20.0f;
    private float step = 0.0f;
    private float smooth = 3.0f;
    private Vector3 offset,originalPos;        
    private Quaternion originalRot;

    void Start()
    {

        stateManager = FindObjectOfType<StateManager>();
        //normalFov = Camera.main.fieldOfView;
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.

        originalPos = transform.position;
        originalRot = transform.rotation;
        normalFov = 20.0f;
    }

    StateManager stateManager;
    private bool started = false, doneRotate = false;

    void Update()
    {

        if (doneRotate == false)
        {
            return;
        }

        if (isZoomed)
        {
            //cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, 2.0f * Time.deltaTime);
           cam.fieldOfView = 13.0f;
        }
        if (reZoomed)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normalFov, 2.0f * Time.deltaTime);
            //cam.fieldOfView = 20.0f;
        }

        if (objectToMove.transform.position == target.position)
        {
            objectToMove.GetComponent<Animator>().SetFloat("MoveSpeed", 0);
            reZoomed = true;
            needToMove = false;

        }
        if (needToMove)
        {
            Vector3 des = objectToMove.transform.position + offset;
            Vector3 smoothdes = Vector3.Lerp(transform.position, des, smooth * Time.deltaTime);

            //transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, Smoothing);
            transform.position = smoothdes;
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, target.position, smooth* Time.deltaTime);
            return;
        }

        if (!reZoomed && cam.fieldOfView > targetFov)
        {
            isZoomed = true;
            return;
        }
        else
        {
            isZoomed = false;
            needToMove = true;

        }


        if (cam.fieldOfView >= normalFov-0.7 && reZoomed)
        {

     
            //done 
  
            transform.rotation = originalRot;
            transform.position = originalPos;
            objectToMove.transform.LookAt(transform);
            uiToDisable.SetActive(true);
            stateManager.IsDoneAnimating = true;
            started = false;
            doneRotate = false;
            isZoomed = false;
            needToMove = false;
            reZoomed = false;
            return;
        }


    }

    void LateUpdate()
    {


        if (started == false)
        {
            return;
        }
        if (doneRotate)
        {
            return;
        }

        if (step <= 1.1)
        {
            lookAt();
        }
        else
        {
         
            isZoomed = true;
            doneRotate = true;
        }
    }

    public void lookAt()
    {
        step += speed * Time.deltaTime;
        // transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.position - objectToMove.transform.position), Time.deltaTime * 0.5f);
        Quaternion OriginalRot = transform.rotation;
        transform.LookAt(objectToMove.transform);
        Quaternion NewRot = transform.rotation;
        transform.rotation = OriginalRot;
        transform.rotation = Quaternion.Lerp(transform.rotation, NewRot, step);
       
    }


    public void MoveMe(GameObject objectToMove, Transform target, float speed)
    {
        this.objectToMove = objectToMove;
        this.target = target;
        this.speed = speed;
        stateManager.IsDoneAnimating = false;
        BoardHighlighter.Instance.HideHighlights();
        objectToMove.transform.LookAt(target);
        uiToDisable.SetActive(false);
        offset = transform.position - objectToMove.transform.position;
        objectToMove.GetComponent<Animator>().SetFloat("MoveSpeed", 999);
        started = true;

    }



}
