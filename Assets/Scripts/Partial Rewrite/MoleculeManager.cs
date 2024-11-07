using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
/// <summary>
/// <para>Main controller of all model behaviour handles the calls for new models being created as well handles the
/// results of touches to modify the rotations, sizes and positions of these models</para>
/// </summary>
public class MoleculeManager : MonoBehaviour
{/// <summary>
/// ///ADD THE 'locked' functionality
/// </summary>

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private GameObject moleculeMasterPrefab;

    [SerializeField]
    private List<Visual> possibleModels = new List<Visual>();


    private float rotateSpeedModifier = 0.35f; //modifies rate of rotation
    private float zoomSpeedModifier = 0.006f; //modifies rate of scaling
    private float moveSpeedModifier = 0.0002f; //modifies rate of movement 
    private static float scaleFactorMod = 0.005f; //static value that controls the overall level of scale occuring

    private float offsetFromTrackedImage = 0.07f; //amount of space the model sits above QR code when initally added

    private bool locked; //stores if the movement is locked or not
    private float scale = 4.907f; //scale of the molecules 
    

    private List<Visual> activeVisuals = new List<Visual>(); //List of all active visuals

    private List<Visual> toRemoveBuffer = new List<Visual>(); //List of visuals stored in a buffer to be removed


    private Vector3 movedByPosition; //storage of distance visuals need to be moved since last frame, calculated from touches

    private Vector2[] twoTouchPos= new Vector2[] { new Vector2(-999, 0), new Vector2(0, 0) }; //storage of the positions of the two fingers
                                                                                              //for scaling size

    public bool AnimationScrubbingOccuring = false; //this holds if the screen has an active 3 or more finger touch occuring

    private int counter = 0; //for internal diagnostic

    public enum Tools { Idle = 0, Rotate = 1, Scale = 2, AnimationControl = 3 }; //enum defining tools, mainly for diagnostic purposes

    public Tools toolState = Tools.Idle; //storage of the active tool, default to idle ///THIS WHOLE THING IS DEPRECATED WE GOTTA DO SOME CLEANING OF THIS GODDAMN CODE GOODNESS

    private int renderingStyleStorage = 0; //storing rendering style for new molecule creation

    private int _maximumInactivityTime = 1600; //normally 800
   
    public GameObject vPrefabTest; //for internal diagnostic use (I THINK) (i think not?)

    public AnimationController testAnimation;

    public AnimationController testAnimHolder;
    float pMX = 0f;

    public bool BackgroundAnimationHappening = true;

    /// <summary>
    /// TODO still, fix the text in instructions. Figure out how to make anchor point shifting still possible whilst also letting three finger touch work for animation. Toggling on and off of animation scrolling. 
    /// </summary>
    /// Issues right now with doing the correct type of visualisation
    void Start()
    {
        //this is just for internal testing and should be removed before launch
        //-----
        /*GameObject vGO = Instantiate(vPrefabTest, Vector3.zero, Quaternion.identity);
         Visual v = vGO.GetComponent<Visual>();
         v.vName = "S-CHBrClF";
         v.RunSetup();
         v.ShowMe();
         vGO.transform.localScale = Vector3.one * scaleFactorMod * Mathf.Pow(2f, scale);
         activeVisuals.Add(v);

        GameObject vGO2 = Instantiate(vPrefabTest, Vector3.zero, Quaternion.identity);
         Visual v2 = vGO2.GetComponent<Visual>();
         v2.vName = "R-CHBrClF";
         v2.RunSetup();
         v2.ShowMe();
         vGO2.transform.position = Vector3.left;
         vGO2.transform.localScale = Vector3.one * scaleFactorMod * Mathf.Pow(2f, scale);
         activeVisuals.Add(v2);*/
        //-----
        //////testAnimHolder = Instantiate(testAnimation);
    }

    void Update()
    {
        //if (counter % 200 == 0)
        //{
        //    testAnimHolder.DebugSetToFrame(0);
        //}
        //if (counter % 200 == 100)
        //{
        //    testAnimHolder.DebugSetToFrame(1);
        //}
        //if (Input.GetMouseButton(0))
        //{
        //    float percentChange = ((Input.mousePosition.x - pMX) / Screen.width) * 100f;
        //    Debug.Log($"percent change {percentChange}");
        //    testAnimHolder.ScrubAnimation(percentChange);

        //}

        //testAnimHolder.Animate();



        //pMX = Input.mousePosition.x;
        //if (!AnimationScrubbingOccuring)
        //    testAnimHolder.Animate();


        //if (counter < 300)
        //{
        //    testAnimHolder.Animate((float)counter % 600/6f);
        //}
        //else
        //{
        //    testAnimHolder.Animate();
        //}
        //counter++;


        /*if (counter ==0)
        {
            GameObject newMol = Instantiate(vPrefabTest, Vector3.zero, Quaternion.identity);
            Visual visual = newMol.GetComponent<Visual>();
            visual.vName = "SN2Anim";
            visual.RunSetup();
            visual.ShowMe();
            visual.attachedPosition = Vector3.zero + Vector3.up * offsetFromTrackedImage;
            newMol.transform.localScale = Vector3.one * scaleFactorMod * Mathf.Pow(2f, scale);
            visual.changeRenderingMethod(renderingStyleStorage); // make sure new visual has the correct rendering method 
            activeVisuals.Add(visual);
            //testAnimHolder=visual.GetComponentInChildren<AnimationController>();
            //testAnimHolder.Animate(Mathf.Clamp(Input.mousePosition.x / Screen.width * 100f, 0, 100f));

        }*/
        //counter++;

        /*counter++;

        if (counter == 200)
        {
            UpdateRendering(1);

        }
        if (counter == 400)
        {
            UpdateRendering(2);

        }
        if (counter == 600)
        {
            UpdateRendering(3);

        }*/

        //if (scale > 100) { scale = 1; }
        //scale+=0.1f;
        //Debug.Log(scale);
        foreach (Visual mB in activeVisuals) //for every active visual
        {
            if (!mB.isAnimation)
            {
                mB.gameObject.transform.position = mB.attachedPosition + movedByPosition;
            }
            else
            {
                if (BackgroundAnimationHappening)
                {
                    mB.animController.Animate();
                }
                mB.gameObject.transform.position = mB.attachedPosition;
            } //adjust their position by the amount
              //that it has all been moved by 
        }
        if (CheckActivity()) //check if a QR code is still actively in view for each visual, and assign those not active to be removed
        {
            RemoveVisualsInToRemoveBuffer(); //if any are inactive, remove visual set for removal
        }
    }
    /// <summary>
    /// <para>checks if there are any active visuals</para>
    /// </summary>
    /// <returns> bool of if there are any active visuals</returns>
    public bool AreVisualsActive()
    {
        if (activeVisuals.Count != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// <para> 
    /// Updates rendering to the passed rendering style, variable references to enum of rendering styles,
    /// this means 0 is lone pairs+ball and stick, 1 is no lone pairs+ball and stick,
    /// 2 is lone pairs+space filling, 3 is no lone pairs+space filling</para>
    /// </summary>
    /// <param name="renderingStyle"> Int referencing rendering style enum, to set the rendering style </param>
    public void UpdateRendering(int renderingStyle)
    {
        renderingStyleStorage = renderingStyle;
        foreach (Visual mB in activeVisuals) //every active visual
        {
            Debug.Log("got here");
            mB.changeRenderingMethod(renderingStyle); //set the rendering style to new one
        }
    }

    
    /// <summary>
    /// Updates tool to the correspond with touch number
    /// </summary>
    /// <param name="touchNumber">number of tools to set tool Enum</param>
    public void UpdateTool(int touchNumber)
    {
        if (touchNumber > 3)
        {
            touchNumber = 3; //if there are more than 3 fingers, treat it as 3 fingers
        }
        toolState = (Tools)touchNumber; //set tool state based on enum of touch number
    }

    public void ControlAnimations(Vector2 position, Vector2 change)
    {
        foreach (Visual aV in activeVisuals)
        {
            if (aV.isAnimation)
            {
                aV.animController.ScrubAnimation(change.x / Screen.width * 100f);
            }
        }
        //float percentOfAnimation  = Mathf.Clamp(position.x / Screen.width * 100f, 0f, 100f);
        // testAnimHolder.Animate(percentOfAnimation);
        //Debug.Log(change.x / Screen.width * 100f + " HEY THIS IS WHAT THE CHANGE IS");
        //testAnimHolder.ScrubAnimation(change.x / Screen.width * 100f);
    }

    /// <summary>
    /// This handles both the rotate and move functions of the 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="change"></param>
    /// <param name="touchNumber"></param>
    public void ChangeTransformsOfMolecules(Vector2 position, Vector2 change, int touchNumber)
    {
        Vector3 cameraUp = cameraTransform.rotation * Vector3.up;
        Vector3 cameraRight = cameraTransform.rotation * Vector3.right;
       

        foreach (Visual mB in activeVisuals)//this loop should be inside?? to reduce extra calculations?
        {
            switch (touchNumber)
            {
                case 1:
                    Vector3 fromCamToObj = mB.gameObject.transform.position - cameraTransform.position;
                    Vector3 axisForXRotation = Vector3.Cross(fromCamToObj, cameraRight);
                    Vector3 axisForYRotation = Vector3.Cross(fromCamToObj, cameraUp);
                    mB.gameObject.transform.Rotate(axisForXRotation, -change.x * rotateSpeedModifier, Space.World);
                    mB.gameObject.transform.Rotate(axisForYRotation, -change.y * rotateSpeedModifier, Space.World);
                    break;

                case 2:
                    //float changeInScale = change.y * zoomSpeedModifier;
                    //scale = Mathf.Clamp(scale + changeInScale, 3.322f, 8.966f); //DEBUG THIS AREA IF THERE ARE PROBLEMS
                    //mB.gameObject.transform.localScale = Vector3.one * scaleFactorMod * Mathf.Pow(2f, scale);
                    Debug.Log("THERE IS AN ERROR, PLEASE LOOK INTO THIS");
                    break;

                case 3:
                    //movedByPosition += (cameraRight * change.x + Vector3.up * change.y)*moveSpeedModifier; //turned off the moving
                    break;
                default:
                    break;
            }
        }
    }

    public void ChangeTransformsOfMoleculesTwoTouch(Vector2 t1Pos, Vector2 t2Pos)
    {
        if (twoTouchPos[0].x == -999)
        {
            twoTouchPos[0] = t1Pos;
            twoTouchPos[1] = t2Pos;
            Debug.Log("hello");
        }
        else
        {
            Debug.Log("twotocuhpos[0].x "+ twoTouchPos[0].x);
            float diff = -(Vector2.Distance(twoTouchPos[0], twoTouchPos[1]) - Vector2.Distance(t1Pos, t2Pos));
            float changeInScale = diff * zoomSpeedModifier;
            scale = Mathf.Clamp(scale + changeInScale, 3.322f, 8.966f); //DEBUG THIS AREA IF THERE ARE PROBLEMS
            Debug.Log("diff for scale" + diff);
            foreach (Visual mB in activeVisuals)
            {
                mB.gameObject.transform.localScale = Vector3.one * scaleFactorMod * Mathf.Pow(2f, scale);
            }
            twoTouchPos[0] = t1Pos;
            twoTouchPos[1] = t2Pos;
        }
    }

    public void ResetTwoTouchPos()
    {
        twoTouchPos[0].x = -999;
    }

    public void ResetMoleculePositions()
    {
        movedByPosition = Vector3.zero;
        BackgroundAnimationHappening = !BackgroundAnimationHappening;

        Debug.Log("reset via mol Manager");
    }

    public void SwitchLocking()
    {
        locked = !locked;
        BackgroundAnimationHappening = !BackgroundAnimationHappening;
    }


    public void AddMolecule(ARTrackedImage trackedImage)
    {
        bool alreadyProduced = false;
        foreach( Visual toCheck in activeVisuals)
        {
            if (toCheck.vName == trackedImage.referenceImage.name)//right comparison?
            {
                alreadyProduced = true;
            }
        }
        if (!alreadyProduced)
        {
            
            GameObject newMol = Instantiate(vPrefabTest, Vector3.zero, Quaternion.identity);
            Debug.Log("tracked image, reference image name" + trackedImage.referenceImage.name);
            Visual visual = newMol.GetComponent<Visual>();
            visual.vName = trackedImage.referenceImage.name;
            visual.RunSetup();
            visual.ShowMe();
            visual.attachedPosition = trackedImage.transform.position + Vector3.up * offsetFromTrackedImage;
            newMol.transform.localScale = Vector3.one * scaleFactorMod * Mathf.Pow(2f, scale);
            visual.changeRenderingMethod(renderingStyleStorage); // make sure new visual has the correct rendering method 
            activeVisuals.Add(visual);
            
        }
    }

    public void UpdateMolecule(ARTrackedImage trackedImage)
    {
        bool moleculePresent = false;
        if (!locked)
        {
            foreach (Visual toMove in activeVisuals)
            {
                if (toMove.vName == trackedImage.referenceImage.name)//right comparison?
                {
                    moleculePresent = true;
                    toMove.attachedPosition = trackedImage.transform.position + Vector3.up * offsetFromTrackedImage;
                    toMove.timeActiveWithoutUpdate = 0;
                }

            }
            if (!moleculePresent) {
                AddMolecule(trackedImage);
            }
        }
    }

    public void SetRemovals(ARTrackedImage trackedImage)
    {
        foreach (Visual checkRemove in activeVisuals)
        {
            if (checkRemove.vName == trackedImage.referenceImage.name)//right comparison?
            {
                toRemoveBuffer.Add(checkRemove);
            }
        }
        //find molBuilder w/ the right name and remove them
    }

    public bool CheckActivity()
    {
        bool thingsRemoved = false;
        foreach(Visual vis in activeVisuals)
        {
           vis.timeActiveWithoutUpdate++; //THIS IS WHAT ALLOWS FOR REMOVAL AFTER LONG EXPOSURE OF NO INTERACTION 
            if (vis.timeActiveWithoutUpdate > _maximumInactivityTime) {
                toRemoveBuffer.Add(vis);
                thingsRemoved = true;
            }
        }
        return thingsRemoved;
    }
    public void RemoveVisualsInToRemoveBuffer()
    {
        List<Visual> toKeep = new List<Visual>();
        foreach(Visual check in activeVisuals)
        {
            bool removeMolecule = false;
            foreach(Visual checkExclude in toRemoveBuffer)
            {
                if (check == checkExclude)
                {
                    removeMolecule = true;
                }
                if (!removeMolecule)
                {
                    toKeep.Add(check);
                }
                else
                {
                    Destroy(check.gameObject);
                }
            }
        }
        activeVisuals = toKeep;
        toRemoveBuffer.Clear();
    }

   

    private void AddMolecule(string name, Vector3 pos)
    {
        bool alreadyProduced = false;
        foreach (Visual toCheck in activeVisuals)
        {
            if (toCheck.vName == name)//right comparison?
            {
                alreadyProduced = true;
            }
        }
        if (!alreadyProduced)
        {
            GameObject newMol = Instantiate(moleculeMasterPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("tracked image, reference image name" + name);
            Visual mB = newMol.GetComponent<Visual>();
            mB.vName = name;
           
            newMol.transform.position = pos + Vector3.up * 0.1f;
            newMol.transform.localScale = Vector3.one * scaleFactorMod * Mathf.Pow(2f, scale);
            activeVisuals.Add(mB);
        }
    }


    private void UpdateMolecule(string name, Vector3 pos)
    {
        foreach (Visual toMove in activeVisuals)
        {
            if (toMove.vName == name)//right comparison?
            {
                toMove.gameObject.transform.position = pos + Vector3.up * 0.1f;
            }
        }
    }
    public void SetRemovals(string name)
    {
        foreach (Visual checkRemove in activeVisuals)
        {
            if (checkRemove.vName == name)//right comparison?
            {
                toRemoveBuffer.Add(checkRemove);
            }
        }
        //find molBuilder w/ the right name and remove them
    }
}
