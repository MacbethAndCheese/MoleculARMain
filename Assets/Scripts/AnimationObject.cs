using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationObject
{
    public string objName;
    public GameObject attachedObject; //holds the main attached object can be a atom or bond
    public Vector3[] Positions; //holds position path
    public Quaternion[] Rotations; //holds rotation path
    //public int TargetPositionFrame;
    //public int CurrentPositionFrame;
    public float[] Presences; //how much the object needs to fade or not, or controls the fade of the attached bond
    public bool IsMasterObj; //is this an object that has its position completely determined by the animation curve (true) or is determined by nearby atoms (false)
    public bool IsSubObj; //is this an object 
    public GameObject[] ConnectedMasterObjects = new GameObject[2]; //this used for like the elements of a bond to know its connected bonds
    public List<GameObject> NextDoorObjects = new List<GameObject>(); //this is used to contain information about like nearby atoms for setting 
    public float[] DistToMainNeighbourAtFrame;

    private MoleculeMaster _mM = GameObject.FindObjectOfType<MoleculeMaster>();
    private float _VDWScaler = .5f;

    public AnimationObject(GameObject atC, string nameC, GameObject[] connectedC)
    {
        attachedObject = atC;
        objName = nameC;
        ConnectedMasterObjects = connectedC;
        IsSubObj = true;
        IsMasterObj = false;
      //  _mM = GameObject.FindObjectOfType<MoleculeMaster>();
    }
    public AnimationObject(GameObject atC, string nameC, bool isMasterC)
    {
        attachedObject = atC;
        objName = nameC;
        IsMasterObj = isMasterC;
        IsSubObj = false;
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

    public void SetStartOfFrameNextDoorDist()
    {
        for(int i=0; i<DistToMainNeighbourAtFrame.Length; i++)
        {
            DistToMainNeighbourAtFrame[i] = (attachedObject.transform.position - NextDoorObjects[0].transform.position).magnitude;
        }
    }

    public void SetToFrame(int frame)
    {
        attachedObject.transform.position = Positions[frame];
        attachedObject.transform.rotation = Rotations[frame];
        // do something with Presences[frame];
    }

    public void AnimateBetweenFrames(float percentToNextFrame, int startFrame, int endFrame)
    {
        percentToNextFrame /= 100f;
        if (IsMasterObj)
        {
            attachedObject.transform.position = Positions[startFrame] * (1f - percentToNextFrame) + Positions[endFrame] * percentToNextFrame;
        }
        else
        {
            Vector3 alongPath = Positions[startFrame] * (1f - percentToNextFrame) + Positions[endFrame] * percentToNextFrame;
            Vector3 toMainNeighbourFromPath =   alongPath - NextDoorObjects[0].transform.position;
            float toMainNeighbourFromPathDist = toMainNeighbourFromPath.magnitude;
            float diffBetweenIdealAndCurrentDist =  DistToMainNeighbourAtFrame[startFrame]- toMainNeighbourFromPathDist;
            attachedObject.transform.position = alongPath+toMainNeighbourFromPath.normalized * diffBetweenIdealAndCurrentDist;
        }

    }

    public void AnimateBetweenFramesBonds()
    {
        Vector3 bondDir = (ConnectedMasterObjects[0].transform.position - ConnectedMasterObjects[1].transform.position).normalized;
        Vector3[] atomEdges = new Vector3[]
        {
            ConnectedMasterObjects[0].transform.position + -bondDir * _mM.atomVDW[ConnectedMasterObjects[0].name.TrimEnd(" 1234567890_#".ToCharArray())] * _VDWScaler/2f,
            ConnectedMasterObjects[1].transform.position + bondDir * _mM.atomVDW[ConnectedMasterObjects[1].name.TrimEnd(" 1234567890_#".ToCharArray())] * _VDWScaler/2f
        };

        Vector3 centerPoint = (atomEdges[0] + atomEdges[1]) / 2f;
        float bondLength = Vector3.Distance(atomEdges[0], atomEdges[1]);
        /*Vector3[] bondCenterPoints = new Vector3[]
        {
                centerPoint + bondDir * bondLength/2.5f,
                centerPoint - bondDir * bondLength/2.5f
        };*/
        attachedObject.transform.position = centerPoint + bondDir * bondLength / 2.5f;
        attachedObject.transform.rotation = LookDirection(bondDir);
        attachedObject.transform.localScale = new Vector3(.015f, bondLength / 2.5f, .015f);



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


    public Quaternion LookDirection(Vector3 lookDir)
    {
        return Quaternion.LookRotation(lookDir) * Quaternion.FromToRotation(Vector3.forward, Vector3.up);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
