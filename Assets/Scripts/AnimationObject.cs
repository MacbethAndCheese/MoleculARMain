using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AnimationObject
{
    public string objName;
    public GameObject attachedObject; //holds the main attached object can be a atom or bond
    public Vector3[] Positions; //holds position path
    public Quaternion[] Rotations; //holds rotation path
    public RotationObj[] RotationsToApply; //holds the position as a set of rotations around a point
    //public int TargetPositionFrame;
    //public int CurrentPositionFrame;
    public float[] Presences; //how much the object needs to fade or not, or controls the fade of the attached bond
    public bool IsMasterObj; //is this an object that has its position completely determined by the animation curve (true) or is determined by nearby atoms (false)
    public bool IsSubObj; //is this an object like a bond
    public bool IsSlaveObj; //completley subservient to the 
    public AnimationObject[] ConnectedMasterObjects = new AnimationObject[2]; //this used for like the elements of a bond to know its connected bonds
    public List<AnimationObject> NextDoorObjects = new List<AnimationObject>(); //this is used to contain information about like nearby atoms for setting 
    public float[] DistToMainNeighbourAtFrame;
    public float TotalAngleRequiredToRotate;

    private Color startColor;
    private MoleculeMaster _mM = GameObject.FindObjectOfType<MoleculeMaster>();
    private float _VDWScaler = .5f;

    public AnimationObject(GameObject atC, string nameC, AnimationObject[] connectedC)
    {
        attachedObject = atC;
        objName = nameC;
        ConnectedMasterObjects = connectedC;
        IsSubObj = true;
        IsMasterObj = false;
        IsSlaveObj = false;
        startColor = attachedObject.GetComponent<Renderer>().material.color;
        //  _mM = GameObject.FindObjectOfType<MoleculeMaster>();
    }
    public AnimationObject(GameObject atC, string nameC, bool isMasterC, bool isSlaveC, float tARTRC)
    {
        attachedObject = atC;
        objName = nameC;
        IsMasterObj = isMasterC;
        IsSubObj = false;
        IsSlaveObj = isSlaveC;
        TotalAngleRequiredToRotate = tARTRC;
        //  _mM = GameObject.FindObjectOfType<MoleculeMaster>();

    }

    public void SetLengths(int length)
    {
        Positions = new Vector3[length];
        Rotations = new Quaternion[length];
        Presences = new float[length];
        DistToMainNeighbourAtFrame = new float[length];
    }

    public void SetVars(Vector3 pos, Quaternion rot, float pres, int index)
    {
        Positions[index] = pos;
        Rotations[index] = rot;
        Presences[index] = pres;
    }

    public void SetRotationsToApply()
    {
        RotationsToApply = new RotationObj[Positions.Length];


        if (IsMasterObj)
        {
            for (int i = 0; i < RotationsToApply.Length; i++)
            {
                RotationsToApply[i] = new RotationObj();
            }
        }
        else if (IsSlaveObj)
        {
            for (int i = 0; i < RotationsToApply.Length; i++)
            {
                RotationsToApply[i] = new RotationObj();
            }
        }
        else
        {
            //Debug.Log(this.objName);
            AnimationObject rotPointObj = NextDoorObjects.First(item => item.IsMasterObj == true); //get first object that is a masters object attached
            Vector3 sumCrossVec = Vector3.zero;
            for (int i = 0; i < Positions.Length; i++)
            {
                if (i != 0)
                {
                    sumCrossVec += Vector3.Cross(Positions[i] - rotPointObj.Positions[i], Positions[i - 1] - rotPointObj.Positions[i-1]);
                }
                else
                {

                }
            }
            sumCrossVec.Normalize();
            float totalRot = 0f;
            for (int i = 0; i < RotationsToApply.Length; i++)
            {
                if (i != 0)
                {
                    
                    float amountToRot = -GetAngleBetweenTwoVectorsWhenRotatedAroundAxis(Positions[i] - rotPointObj.Positions[i], Positions[i - 1] - rotPointObj.Positions[i - 1], sumCrossVec); //note this does not calculate the proper amount, in relation to the sumCrossVec
                    //if (correction > 0)
                    //{
                    //    amountToRot *= 1f / correction;
                    //}
                    //Debug.Log(amountToRot + " amount to rot corrected " + objName);

                    totalRot += amountToRot;
                    RotationsToApply[i] = new RotationObj(rotPointObj.Positions[i], sumCrossVec, amountToRot);
                }
                else
                {
                    RotationsToApply[i] = new RotationObj(rotPointObj.Positions[i], sumCrossVec, 0f);
                }

            }
            
        }
    }
    public void SetStartOfFrameNextDoorDist()
    {
        for (int i = 0; i < DistToMainNeighbourAtFrame.Length; i++)
        {
            DistToMainNeighbourAtFrame[i] = (Positions[i] - NextDoorObjects[0].Positions[i]).magnitude;
        }
    }

    public void SetToFrame(int frame)
    {
        attachedObject.transform.localPosition = Positions[frame];
        attachedObject.transform.localRotation = Rotations[frame];
        // do something with Presences[frame];
    }

    public void AnimateBetweenFrames(float percentToNextFrame, int startFrame, int endFrame, Vector3 recursionAdjustmentVector , bool animateUsingPositions, string nameOfCaller = "")
    {
        if (IsMasterObj)
        {
            percentToNextFrame /= 100f;
            attachedObject.transform.localPosition = InBetweenFramePosition(percentToNextFrame, startFrame, endFrame);
            foreach (AnimationObject aO in NextDoorObjects)
            {
                if (!aO.IsMasterObj) {
                    aO.AnimateBetweenFrames(percentToNextFrame, startFrame, endFrame, Vector3.zero, animateUsingPositions, objName);
                }
            }
        }
        else
        {
            if (animateUsingPositions)
            {
            Vector3 alongPath = InBetweenFramePosition(percentToNextFrame, startFrame, endFrame) + recursionAdjustmentVector;
            Vector3 toMainNeighbourFromPath = alongPath - NextDoorObjects[0].attachedObject.transform.localPosition;
            float toMainNeighbourFromPathDist = toMainNeighbourFromPath.magnitude;
            float diffBetweenIdealAndCurrentDist = DistToMainNeighbourAtFrame[startFrame] - toMainNeighbourFromPathDist;
            Vector3 adjustmentAlongPath = toMainNeighbourFromPath.normalized * diffBetweenIdealAndCurrentDist;
            attachedObject.transform.localPosition = alongPath + adjustmentAlongPath;
                //Debug.Log(objName + "-- " + Positions[startFrame]);
                //Debug.DrawLine(NextDoorObjects[0].attachedObject.transform.localPosition, alongPath, Color.red);
            //Debug.DrawLine(alongPath, NextDoorObjects[0].attachedObject.transform.localPosition + recursionAdjustmentVector, Color.green);
            //Debug.DrawLine(InBetweenFramePosition(percentToNextFrame, startFrame, endFrame), alongPath, Color.blue);


            recursionAdjustmentVector += adjustmentAlongPath;
            //Debug.Log(nameOfCaller + " h,mmmm  "+objName);
                foreach (AnimationObject aO in NextDoorObjects)
                {
                    if (!aO.objName.Equals(nameOfCaller)) {
                        //   Debug.Log(aO.objName + " yea? "+objName);
                        aO.AnimateBetweenFrames(percentToNextFrame, startFrame, endFrame, recursionAdjustmentVector,animateUsingPositions, objName);
                    }
                }
            }
            else
            {
                if (!IsSlaveObj)
                {
                    Debug.Log(objName + "-- " + Positions[startFrame]);
                    var rTADataEF = RotationsToApply[endFrame].GetValues();
                    var rTADataSF = RotationsToApply[startFrame].GetValues();
                    Vector3 adjustedPivot = CombineVectorsBetweenFramesSmooth(percentToNextFrame, rTADataSF.Item1, rTADataEF.Item1);
                    Vector3 posDefinedByRotation = GetPositionFromRotationAroundPointAroundAxisBetweenFrame(percentToNextFrame, Positions[startFrame], adjustedPivot, rTADataEF.Item2, rTADataEF.Item3);
                    Vector3 posDefinedByPath = InBetweenFramePosition(percentToNextFrame, startFrame, endFrame);
                    if (Mathf.Abs(RotationsToApply[endFrame].rotationAmount) > 0)
                    {
                        //Vector3 alongCombinedPath = CombineVectorsBetweenFrames(percentToNextFrame, posDefinedByRotation, posDefinedByPath, 10f);
                        Vector3 alongCombinedPath = posDefinedByRotation;
                        Vector3 masterIshLocation = NextDoorObjects[0].attachedObject.transform.localPosition;
                        Vector3 fromMasterToPath = alongCombinedPath - masterIshLocation;
                        float distFromPointDefinedByRotationToMaster = fromMasterToPath.magnitude;
                        float distDifference = DistToMainNeighbourAtFrame[startFrame] - distFromPointDefinedByRotationToMaster;
                        Vector3 adjustmentAlongPath = fromMasterToPath.normalized * distDifference;
                        attachedObject.transform.localPosition = alongCombinedPath + adjustmentAlongPath;

                        //attachedObject.transform.localPosition = CombineVectorsBetweenFrames(percentToNextFrame, posDefinedByRotation, posDefinedByPath, 10f); ///LETS ADD SOME BUMPING BASED ON BOND DISTANCE
                        foreach (AnimationObject aO in NextDoorObjects)
                        {
                            if (aO.IsSlaveObj)
                            {
                                (Vector3, Vector3, float) passTup = (adjustedPivot, rTADataEF.Item2, rTADataEF.Item3);
                                //Debug.Log(aO.objName);
                                aO.SlaveAnimation(percentToNextFrame, startFrame, endFrame, passTup, adjustmentAlongPath);
                            }
                        }
                    }
                    else
                    {

                        foreach (AnimationObject aO in NextDoorObjects)
                        {
                            if (aO.IsSlaveObj)
                            {
                                attachedObject.transform.localPosition = posDefinedByPath;
                                aO.SlaveAnimation(percentToNextFrame, startFrame, endFrame);
                            }
                        }
                    }


                    //if (percentToNextFrame>0.5f)
                    //    attachedObject.transform.localPosition = GetPositionFromRotationAroundPointAroundAxisBetweenFrame(1f, Positions[startFrame], rTAData.Item1, rTAData.Item2, rTAData.Item3);
                    //else
                    //    attachedObject.transform.localPosition = GetPositionFromRotationAroundPointAroundAxisBetweenFrame(0f, Positions[startFrame], rTAData.Item1, rTAData.Item2, rTAData.Item3);

                    if (objName.Equals("Cl2") || objName.Equals("Br2") || objName.Equals("Cl2") || objName.Equals("Br1"))
                    {
                        Debug.DrawLine(NextDoorObjects[0].attachedObject.transform.localPosition, Positions[endFrame], Color.yellow);
                        Debug.DrawLine(NextDoorObjects[0].attachedObject.transform.localPosition, Positions[startFrame], Color.blue);
                        Debug.DrawRay(RotationsToApply[endFrame].rotationPoint, RotationsToApply[endFrame].rotationAxis, Color.magenta);
                    }


                    //attachedObject.transform.localPosition = RotatePointAroundPivot(Positions[startFrame],RotationsToApply[endFrame].rotationAxis)
                    ///THE TRICK HERE MIGHT BE TO LIKE PUT PUT THE VECTORS AS IF THEY ARE AROUND THE ORIGIN AND THEN DO THE ROTATIONS AND THEN PUT THEM BACK WHERE THEY NEED TO BE
                }
                else
                {

                }
            }
            
            
        }
    }

    public void SlaveAnimation(float percentToNextFrame, int startFrame, int endFrame, string nameOfCaller = "")
    {
        Vector3 alongPath = InBetweenFramePosition(percentToNextFrame, startFrame, endFrame);

        attachedObject.transform.localPosition = alongPath;
        foreach (AnimationObject aO in NextDoorObjects)
        {
            if (aO.IsSlaveObj && !aO.objName.Equals(nameOfCaller))
            {
                aO.SlaveAnimation(percentToNextFrame, startFrame, endFrame, objName);
            }
        }
    }

    public void SlaveAnimation(float percentToNextFrame, int startFrame,int endFrame, (Vector3,Vector3,float) rTAData, Vector3 recursionAdjusment, string nameOfCaller = "")
    {
        Vector3 alongSlavePath = GetPositionFromRotationAroundPointAroundAxisBetweenFrame(percentToNextFrame, Positions[startFrame], rTAData.Item1, rTAData.Item2, rTAData.Item3); 
        Vector3 alongPath = InBetweenFramePosition(percentToNextFrame, startFrame, endFrame);

        Vector3 alongCombinedPath = CombineVectorsBetweenFramesWithMiddleDominanceOfIdealPosition(percentToNextFrame, alongSlavePath, alongPath, 10f)+recursionAdjusment;
        Vector3 masterIshLocation = NextDoorObjects[0].attachedObject.transform.localPosition;
        Vector3 fromMasterToPath = alongCombinedPath - masterIshLocation;
        float distFromPointDefinedByRotationToMaster = fromMasterToPath.magnitude;
        float distDifference = DistToMainNeighbourAtFrame[startFrame] - distFromPointDefinedByRotationToMaster;
        Vector3 adjustmentAlongPath = fromMasterToPath.normalized * distDifference;
        attachedObject.transform.localPosition = alongCombinedPath + adjustmentAlongPath;
        recursionAdjusment += adjustmentAlongPath;
        //attachedObject.transform.localPosition = CombineVectorsBetweenFrames(percentToNextFrame, alongSlavePath, alongPath, 10f);

        foreach (AnimationObject aO in NextDoorObjects)
        {
            if (aO.IsSlaveObj && !aO.objName.Equals(nameOfCaller))
            {
                aO.SlaveAnimation(percentToNextFrame, startFrame, endFrame, rTAData, recursionAdjusment, objName);
            }
        }
    }

    public void AnimateBetweenFramesNonMaster(float percentToNextFrame, int startFrame, int endFrame)
    {

        Vector3 alongPath = InBetweenFramePosition(percentToNextFrame, startFrame, endFrame);
        Vector3 toMainNeighbourFromPath = alongPath - NextDoorObjects[0].attachedObject.transform.localPosition; //this should be ok, cuz it should have always been set right before the animation! but later i might to make recursion better as it only supports two layers of non master objects. See above.
        Debug.DrawLine(alongPath, NextDoorObjects[0].attachedObject.transform.localPosition, Color.red);
        float toMainNeighbourFromPathDist = toMainNeighbourFromPath.magnitude;
        float diffBetweenIdealAndCurrentDist = DistToMainNeighbourAtFrame[startFrame] - toMainNeighbourFromPathDist;
        attachedObject.transform.localPosition = alongPath + toMainNeighbourFromPath.normalized * diffBetweenIdealAndCurrentDist;
    }

    public void AnimateBetweenFramesBonds(float percentToNextFrame, int startFrame, int endFrame) //THE FADING OF BONDS IS DONE IN A VERY JANKY WAY, THIS SHOULD BE FIXED DO___
    {
        Vector3 bondDir = (ConnectedMasterObjects[0].attachedObject.transform.localPosition - ConnectedMasterObjects[1].attachedObject.transform.localPosition).normalized;
        Vector3[] atomEdges = new Vector3[]
        {
            ConnectedMasterObjects[0].attachedObject.transform.localPosition + -bondDir * _mM.atomVDW[ConnectedMasterObjects[0].objName.TrimEnd(" 1234567890_#".ToCharArray())] * _VDWScaler/3f, //is trim needed?
            ConnectedMasterObjects[1].attachedObject.transform.localPosition + bondDir * _mM.atomVDW[ConnectedMasterObjects[1].objName.TrimEnd(" 1234567890_#".ToCharArray())] * _VDWScaler/3f
        };
        Renderer renderer = this.attachedObject.GetComponent<Renderer>();
        string matName = renderer.material.name;
        int startInd = matName.IndexOf("_");
        if (matName.Substring(startInd + 1, 1).Equals("t")){
            float startLength = (ConnectedMasterObjects[0].Positions[0] - ConnectedMasterObjects[1].Positions[0]).magnitude;
            float endLength = (ConnectedMasterObjects[0].Positions[ConnectedMasterObjects[0].Positions.Length-1] - ConnectedMasterObjects[1].Positions[ConnectedMasterObjects[0].Positions.Length - 1]).magnitude;
            if ((startLength - endLength) < 0) {
                Color lerpColor = Color.Lerp(startColor, Color.clear, (0.6f*((float)startFrame)+percentToNextFrame*0.6f / 100f)-0.1f );;
                renderer.material.color = lerpColor;
            }
            else {
                Color lerpColor = Color.Lerp(Color.clear, startColor, (0.6f * ((float)startFrame) + percentToNextFrame * 0.6f / 100f) - 0.1f);
                renderer.material.color = lerpColor;
            }
        }

        Vector3 centerPoint = (atomEdges[0] + atomEdges[1]) / 2f;
        float bondLength = Vector3.Distance(atomEdges[0], atomEdges[1]);
        /*Vector3[] bondCenterPoints = new Vector3[]
        {
                centerPoint + bondDir * bondLength/2.5f,
                centerPoint - bondDir * bondLength/2.5f
        };*/
        attachedObject.transform.localPosition = centerPoint + bondDir * bondLength / 4f;
        attachedObject.transform.localRotation = LookDirection(bondDir);
        attachedObject.transform.localScale = new Vector3(.015f, bondLength / 4f, .015f);



        /*Vector3 crossVec = Vector3.right;
        float multipleBondOffset = 0f;
        float offseter = 0;
        switch (bondInfo[i].z)
        {
            case 0:
                Debug.Log("ERROR?");
                break;
            case 1:
                Debug.Log("single bond");
                offseter = 1;
                break;
            case 2:
                Debug.Log("double bond");
                multipleBondOffset = 0.02f;
                crossVec = Vector3.Cross(FindAnotherBondPairVectorWithSameAtom(bondPairs, i), bondDir);
                offseter = -1;
                break;
            case 3:
                offseter = -2;
                multipleBondOffset = 0.025f;
                crossVec = Vector3.up;
                Debug.Log("triple bond");
                break;
            default:
                Debug.Log("UNFORSEEN CASE! CHECK!");
                break;
        }

        Vector3 bondOffsetDir = Vector3.Cross(crossVec, bondDir);*/
        //future code reference for double bonds
        /*for (int j = 0; j < bondInfo[i].z; j++)
        {
            for (int k = 0; k < 2; k++)
            {
                GameObject bond = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                bond.transform.parent = eltern.transform;
                bond.name = bondPairs[i, k].element + " bond";
                float perJModifier = multipleBondOffset * (offseter + (float)(j * 2)) / offseter; //maybe works?
                bond.transform.localPosition = bondCenterPoints[k] + bondOffsetDir * perJModifier;
                bond.transform.rotation = LookDirection(bondDir);
                bond.GetComponent<Renderer>().material.color = aD.atomColour[bondPairs[i, k].element];
                bond.transform.localScale = new Vector3(.015f, bondLength / 2.5f, .015f);
            }
        }*/

    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) {
        return rotation * (point - pivot) + pivot;
    }

    public Quaternion LookDirection(Vector3 lookDir)
    {
        return Quaternion.LookRotation(lookDir) * Quaternion.FromToRotation(Vector3.forward, Vector3.up);
    }

    public Vector3 InBetweenFramePosition(float percentToNextFrame, int startFrame, int endFrame)
    {
        return Positions[startFrame] * (1f - percentToNextFrame) + Positions[endFrame] * percentToNextFrame;
    }

    public Vector3 GetPositionFromRotationAroundPointAroundAxisBetweenFrame(float percentToNextFrame, Vector3 startPos, Vector3 rotationPoint,Vector3 rotationAxis, float totalPossibleRotationAmount)
    {
        return rotationPoint + Quaternion.AngleAxis(totalPossibleRotationAmount * percentToNextFrame, rotationAxis) * (startPos - rotationPoint);
    }

    public Vector3 CombineVectorsBetweenFramesWithMiddleDominanceOfIdealPosition(float percentToNextFrame, Vector3 idealPosition, Vector3 setPosition, float powerFactor=1f) //higher power factor causes it to strongly favour the ideal position
    {
        float closenessToStartOrFinish = Mathf.Pow((Mathf.Abs(percentToNextFrame - 0.5f)*2f),powerFactor);
        //Debug.Log(closenessToStartOrFinish);
        return ((idealPosition * (1f - closenessToStartOrFinish)) + (setPosition * closenessToStartOrFinish));
    }

    public Vector3 CombineVectorsBetweenFramesSmooth(float percentToNextFrame, Vector3 startPosition, Vector3 endPosition)
    {
        return startPosition * (1f - percentToNextFrame) + endPosition * percentToNextFrame;
    }

    public float GetAngleBetweenTwoVectorsWhenRotatedAroundAxis(Vector3 firstVec, Vector3 secondVec, Vector3 axis)
    {
        Vector3 a = firstVec.normalized;
        Vector3 b = secondVec.normalized;
        Vector3 u = axis.normalized;
        Vector3 e = (a - (Vector3.Dot(a, u) * u)).normalized;
        Vector3 f = (Vector3.Cross(u, e));
        float ang = Mathf.Rad2Deg*Mathf.Atan2(Vector3.Dot(b, f), Vector3.Dot(b, e));
        return ang;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public class RotationObj{
        public bool isNull;
        public Vector3 rotationPoint;
        public Vector3 rotationAxis;
        public float rotationAmount;
        public RotationObj()
        {
            isNull = true;
        }

        public RotationObj(Vector3 rPC, Vector3 rACV3, float rACF)
        {
            isNull = false;
            rotationPoint = rPC;
            rotationAxis = rACV3;
            rotationAmount = rACF;
        }

        public (Vector3, Vector3, float) GetValues()
        {
            return (rotationPoint, rotationAxis, rotationAmount);
        }
    }
}
