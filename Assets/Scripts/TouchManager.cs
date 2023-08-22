using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///   <para>Deprecated, do not use</para>
/// </summary>
public class TouchManager : MonoBehaviour
{   //deals with touches, and also updates the prefab by these touches
    public GameObject prefab;

    public TrackedImageInfoMultipleManager trackedImageInfoMultipleManager;
    
    private Touch touchOneFinger;
    private Touch touchTwoFinger_1;
    private Touch touchTwoFinger_2;
    private Touch touchThreeFinger;
    private Vector2 touchPos;

    public Transform cameraTransform;
    //public Quaternion rotationY;
    //public Quaternion rotationX;

    public float moveXOneFinger, moveYOneFinger;

    public float moveXTwoFinger_1, moveYTwoFinger_1;

    public float moveXThreeFinger, moveYThreeFinger;


    private float rotateSpeedModifier = 0.35f;
    private float zoomSpeedModifier = 0.0035f;
    private float moveSpeedModifier = 0.0005f;

    private float timeSinceLastTwoFingerTap=0f;
    private float timeSinceLastOneFingerTap = 0f;

    private Vector3 movedByPosition = Vector3.zero;


    void Update()
    {
        timeSinceLastTwoFingerTap += Time.deltaTime;
        timeSinceLastOneFingerTap += Time.deltaTime;
        //Debug.Log("timesincelasttap " + timeSinceLastTap);
        Vector3 cameraUp = cameraTransform.rotation * Vector3.up;
        Vector3 cameraRight = cameraTransform.rotation * Vector3.right;
        //Debug.DrawRay(cameraTransform.position, cameraUp * 100f, Color.green);
        //Debug.DrawRay(cameraTransform.position, cameraRight * 100f, Color.red);


        var bC = FindObjectOfType<BoxCollider>();
        if (bC != null)
        {
            prefab = bC.gameObject;
        }
        if (prefab != null)
        {
            prefab.transform.position = movedByPosition+trackedImageInfoMultipleManager.attachedPosition;
        }
        if (Input.touchCount == 1)
        {

            touchOneFinger = Input.GetTouch(0);
            if (touchOneFinger.phase == TouchPhase.Began)
            {
                if (timeSinceLastOneFingerTap > 0.1f && timeSinceLastOneFingerTap < 0.3f)
                {
                    if (prefab != null)
                    {
                        movedByPosition = Vector3.zero;
                        Debug.Log("Reset");
                    }
                }
                timeSinceLastOneFingerTap = 0f;
            }
            if (touchOneFinger.phase == TouchPhase.Moved)
            {
                moveXOneFinger = -touchOneFinger.deltaPosition.x * (rotateSpeedModifier);
                moveYOneFinger = -touchOneFinger.deltaPosition.y * rotateSpeedModifier; //do theese need to be reversed? //DO THEY??? Apr/30/2021

                //rotationY = Quaternion.Euler(0f, -touch.deltaPosition.x * rotateSpeedModifier, 0f);
                //rotationX = Quaternion.Euler( touch.deltaPosition.y * rotateSpeedModifier, 0f,0f);
            }


            if (prefab != null) {
                Vector3 fromCamToPrefab = prefab.transform.position - cameraTransform.position;
                Vector3 axisForXRotation = Vector3.Cross(fromCamToPrefab, cameraRight);
                Vector3 axisForYRotation = Vector3.Cross(fromCamToPrefab, cameraUp);

                //Debug.DrawRay(prefab.transform.position, axisForXRotation * 100f, Color.magenta);
                //Debug.DrawRay(prefab.transform.position, axisForYRotation * 100f, Color.blue);
                prefab.transform.Rotate(axisForXRotation, moveXOneFinger, Space.World);
                prefab.transform.Rotate(axisForYRotation, moveYOneFinger, Space.World);


            }
            //transform.rotation = rotationY * transform.rotation;
            //transform.rotation = rotationX * transform.rotation;
            //prefab.transform.rotation = transform.rotation;
        }

        if (Input.touchCount== 2)
        {
            touchTwoFinger_1 = Input.GetTouch(0);
            if (touchTwoFinger_1.phase == TouchPhase.Began)
            {
                if (timeSinceLastTwoFingerTap > 0.1f && timeSinceLastTwoFingerTap < 0.3f)
                {
                    if (prefab != null)
                    {
                        trackedImageInfoMultipleManager.modelLocked = !trackedImageInfoMultipleManager.modelLocked;
                        Debug.Log("LOCKED LOCKED LOCKED");
                    }
                }
                timeSinceLastTwoFingerTap = 0f;
            }
            if (touchTwoFinger_1.phase == TouchPhase.Moved)
            {
                moveYTwoFinger_1 = touchTwoFinger_1.deltaPosition.y * (zoomSpeedModifier);
                float newScaleFloat = trackedImageInfoMultipleManager.scaleFloat + moveYTwoFinger_1;
                //Vector3 newScale = trackedImageInfoMultipleManager.scaleFactor + Vector3.one * moveYTwoFinger_1;

                if (newScaleFloat < 3.322f)
                {
                    newScaleFloat = 3.322f;
                   // Debug.Log("too small "+ newScale.x);
                }
                if ( newScaleFloat> 8.966f)
                {
                    newScaleFloat = 8.966f;
                   // Debug.Log("too big "+newScale.x);

                }
                //  Debug.Log("this is what the new scale is " + newScale.x);
                //trackedImageInfoMultipleManager.scaleFactor = newScale;
                trackedImageInfoMultipleManager.scaleFloat = newScaleFloat;
                    if (prefab != null)
                {
                    prefab.transform.localScale = Vector3.one * trackedImageInfoMultipleManager.scaleFactorMod * Mathf.Pow(2f, trackedImageInfoMultipleManager.scaleFloat);
                }
            }

        }

        if (Input.touchCount == 3)
        {
            touchThreeFinger = Input.GetTouch(0);
            
            if (touchThreeFinger.phase == TouchPhase.Moved)
            {
                moveXThreeFinger = touchThreeFinger.deltaPosition.x*moveSpeedModifier;
                moveYThreeFinger = touchThreeFinger.deltaPosition.y * moveSpeedModifier;

            }
            if (prefab != null)
            {
                Vector3 combinedVector = cameraRight * moveXThreeFinger + Vector3.up * moveYThreeFinger;
                movedByPosition += combinedVector;
                //Vector3 fromCamToPrefab = prefab.transform.position - cameraTransform.position;
                //Vector3 axisForXRotation = Vector3.Cross(fromCamToPrefab, cameraRight);
                //Vector3 axisForYRotation = Vector3.Cross(fromCamToPrefab, cameraUp);

               /* Debug.DrawRay(prefab.transform.position, axisForXRotation * 100f, Color.magenta);
                Debug.DrawRay(prefab.transform.position, axisForYRotation * 100f, Color.blue);
                prefab.transform.Rotate(axisForXRotation, moveXOneFinger, Space.World);
                prefab.transform.Rotate(axisForYRotation, moveYOneFinger, Space.World);*/


            }
        }
   
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
