using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Visual : MonoBehaviour
{//really need to rename this class and maybe also its internal structures because they are mad unclear.

    private List<GameObject> myModels;

    public string vName;

    private GameObject activeObject;

    private ModelDictionary modelDictionary;


    public int timeActiveWithoutUpdate = 0;

    public bool isMolecule;//handles if it is a molecule or not

    public bool hasLonePairs;//handles if there are lone pairs or not, doesnt work for all options

    public bool hasSpaceFillingRendering;

    public int ughhh = 0;//????ah just for testing
    public Vector3 attachedPosition = Vector3.zero;

    enum RenderingStyles {
        BallAndStickLP,
        BallAndStickNoLP,
        SpaceFillingLP,
        SpaceFillingNoLP
    }
    RenderingStyles renderingMethod;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //ughhh++;

        //if (ughhh == 300)
        //{
        //    ShowMe();
        //}

        //if (ughhh == 600)
        //{
        //}

        //if (ughhh == 900)
        //{
        //}
    }

    public void RunSetup() {
        AttachModelsToThis();
        CheckTypes();
    }
    public void AttachModelsToThis()
    {
        GameObject modelDictionaryObject = GameObject.Find("ModelDictionaryObject");//change names christ
        modelDictionary = modelDictionaryObject.GetComponent<ModelDictionary>();
        Debug.Log(vName + " -- ");
        myModels = modelDictionary.ModelsByVisualName[vName];
        //Debug.Log(myModels[0].name);
    }

    public void CheckTypes()
    {
        if (myModels.Count > 1)
        {
            isMolecule = true;

            //Debug.Log(myModels.Count);
            string secondModelName = myModels[1].name;
            string secondModelType = secondModelName.Substring(secondModelName.IndexOf("_") + 1, secondModelName.IndexOf("#") - secondModelName.IndexOf("_") - 1);
            
           // Debug.Log(secondModelType+"<><><><><><><><>");
            if (myModels.Count == 2)
            {
                if (secondModelType.Equals("BallAndStickNoLP"))
                {
                    hasSpaceFillingRendering = false;
                    hasLonePairs = true;
                }
                else
                {
                    hasSpaceFillingRendering = true;
                    hasLonePairs = false;
                }
            }
            else if(myModels.Count==4)
            {
                hasLonePairs = true;
                hasSpaceFillingRendering = true;
            }
            else
            {
                Debug.LogError("heck what is going on here???");
            }
        }
        else {
            isMolecule = false;
        }
    }

    public void ShowMe() {
        activeObject = Instantiate(myModels[0], Vector3.zero, Quaternion.identity);//instantiate it as the basic version of it, ball and stick with 
        activeObject.transform.parent = this.transform;
        activeObject.transform.localScale = Vector3.one; //setting it to be quite big rn
    }


    public void changeRenderingMethod(int newRenderingMethod)
    {
        renderingMethod = (RenderingStyles)newRenderingMethod;
        int indexOfModel = AdjustRenderingMethodIndexBasedOnTypeOfVisual(newRenderingMethod);
        Debug.Log("CHANGED THE RENDERING METHOD to: "+renderingMethod+" Actual Index: "+indexOfModel);
        Vector3 savedPos = activeObject.transform.position;
        Quaternion savedRot = activeObject.transform.rotation;//not sure if these are important because it might be controlled on the manager side
        GameObject.Destroy(activeObject);
        activeObject = Instantiate(myModels[indexOfModel], savedPos, savedRot);
        activeObject.transform.parent = this.transform;
        activeObject.transform.localScale = Vector3.one; //setting it to be quite big rn
    }

    public void changeTheMolecule(string name)
    {
        vName = name;
        Debug.Log("CHANGED THE MOLECULE");
    }
    /// <summary>
    /// Adjusts the rendering value so that it can account for the number of models that different visuals have
    ///
    /// </summary>
    /// <param name="newRenderingMethod"> value passed from molecule master for rendering changes</param>
    /// <returns> Returns index for models needed for this visual.
    /// If it is a molecule and has lone pairs, returns passed variable, if no lone pairs,
    /// returns 0 or 1 depending on ball and stick vs space filling, if not a molecule, returns a 0</returns>
    public int AdjustRenderingMethodIndexBasedOnTypeOfVisual(int newRenderingMethod)
    {
        int indexOfModel=0;
        if (isMolecule)
        {
            if (hasLonePairs && hasSpaceFillingRendering)
            {
                indexOfModel = newRenderingMethod;
            }
            else
            {
                if (hasSpaceFillingRendering)
                {
                    switch (newRenderingMethod)
                    {
                        case 0:
                            indexOfModel = 0;
                            break;
                        case 1:
                            indexOfModel = 0;
                            break;
                        case 2:
                            indexOfModel = 1;
                            break;
                        case 3:
                            indexOfModel = 1;
                            break;
                        default:
                            Debug.LogError("THERE IS A PROBLEM HERE");
                            break;
                    }
                }
                else
                {
                    switch (newRenderingMethod)
                    {
                        case 0:
                            indexOfModel = 0;
                            break;
                        case 1:
                            indexOfModel = 1;
                            break;
                        case 2:
                            indexOfModel = 0;
                            break;
                        case 3:
                            indexOfModel = 1;
                            break;
                        default:
                            Debug.LogError("THERE IS A PROBLEM HERE");
                            break;
                    }
                }
            }
        }

        return indexOfModel;
    }

   

  
   
    
    public class AtomStorage
    {
        float bondWidth = 0.015f;//get this working
        public string name;
        public string nameCore = "me";
        public Color32 color;
        public float size;
        public GameObject attachedAtom;
        public GameObject attachedBondCore;
        public GameObject attachedBondSec;
        public MoleculeMaster mMAS; //molculemasterAtomStorage
        public AtomStorage(MoleculeMaster molMaster,string nameGiven)
        {
          //  Debug.Log("name given" + nameGiven);
            color = molMaster.atomColour[nameGiven];
            size = molMaster.atomVDW[nameGiven];
            name = nameGiven;
            mMAS = molMaster;
        }

        //public void placeRotateAndScaleBond(Vector3 dirVec, float fullBondLength)
        //{
        //    float edgeToEdgeLength = fullBondLength - ((mMAS.atomVDW[nameCore] + mMAS.atomVDW[name]-0.08f)*0.5f);//this is referencing scale factor, but its difficutl to assing, maybe remove this as a function declared by the atom 

        //    Debug.Log(edgeToEdgeLength);
        //    Debug.Log(fullBondLength);
        //    attachedBondCore.transform.localPosition += dirVec * (edgeToEdgeLength * (1f / 4f)+ mMAS.atomVDW[nameCore]);
        //    attachedBondCore.transform.localScale = scaleBond(edgeToEdgeLength);
        //    attachedBondCore.transform.localRotation = lookBond(dirVec);

        //    attachedBondSec.transform.localPosition += dirVec * (edgeToEdgeLength * (3f / 4f) + mMAS.atomVDW[nameCore]);
        //    attachedBondSec.transform.localScale = scaleBond(edgeToEdgeLength);
        //    attachedBondSec.transform.localRotation = lookBond(dirVec);



        //    //attachedBondCore.transform.localPosition += dirVec * fullBondLength * (1f / 4f); /// should we center it not on the center of the bond, but on the center between the two atom edges?????
        //    ////thinking the edges, but thats a pain so lets figure out how to do that?
        //    //attachedBondCore.transform.localScale = scaleBond(fullBondLength);
        //    //attachedBondCore.transform.localRotation = lookBond(dirVec);

        //    //attachedBondSec.transform.localPosition += dirVec * fullBondLength * (3f / 4f);
        //    //attachedBondSec.transform.localScale = scaleBond(fullBondLength);
        //    //attachedBondSec.transform.localRotation = lookBond(dirVec);
        //}

        
    }

    
}
