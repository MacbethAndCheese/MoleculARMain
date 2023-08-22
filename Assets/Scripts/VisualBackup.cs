using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class VisualBackup : MonoBehaviour
{//really need to rename this class and maybe also its internal structures because they are mad unclear.
    [SerializeField]
    private List<GameObject> myModels;


    [SerializeField]
    private GameObject activeObject;



    public Material lonePairMaterial;

    public bool lonePairsOn = true;
    public int renderingMethod; //0 - ball and stick, 1 - space filling, 2 - stick only
    float scaleFactor; //set by renderingMethod
    float bondScaleFactor; //set by renderingMethod

    public float lonePairDist; //default 0.15f
    public float lonePairSizeXY; //default 0.25f
    public float lonePairSizeZ; //default 0.18f
    public Color lonePairColor; //(some transparancy wanted here)

    [SerializeField]
    public string modelName;

    public MoleculeMaster mM; //reference to the molecularmaaster

    public int timeActiveWithoutUpdate = 0;

    public int ughhh = 0;
    public Vector3 attachedPosition = Vector3.zero;


    public VisualBackup(string nameC)
    {
        modelName = nameC;
    }

    void Start()
    {
        buildSystem();
    }

    // Update is called once per frame
    void Update()
    {
        //ughhh++;
        //if (ughhh == 300)
        //{
        //    changeRenderingMethod(1);
        //}

        //if (ughhh == 600)
        //{
        //    changeTheMolecule("PF5");
        //}

        //if (ughhh == 900)
        //{
        //    changeRenderingMethod(2);
        //}
    }

    public void ShowMe() {
        activeObject = Instantiate(myModels[0], Vector3.zero, Quaternion.identity);
    }

    public void changeRenderingMethod(int newRenderingMethod)
    {
        renderingMethod = newRenderingMethod;
        buildSystem();
        Debug.Log("CHANGED THE RENDERING METHOD to: "+newRenderingMethod);

    }

    public void changeTheMolecule(string name)
    {
        modelName = name;
        buildSystem();
        Debug.Log("CHANGED THE MOLECULE");
    }

    public void setLonePairs(bool toggleLonePairs)
    {
        lonePairsOn = toggleLonePairs;
        buildSystem();
        Debug.Log("toggled 'are lone pairs on' to: " + lonePairsOn);
    }

    public void addModel(GameObject modelToAdd)
    {
        myModels.Add(modelToAdd);
    }

    public List<string> getContainedModelNamesByType()
    {
        List<string> toReturn = new List<string>();
        foreach(GameObject gO in myModels)
        {
            toReturn.Add(gO.name.Substring(gO.name.IndexOf("_") + 1, gO.name.IndexOf("-")));
            Debug.Log("name returning "+ gO.name.Substring(gO.name.IndexOf("_") + 1, gO.name.IndexOf("-")- gO.name.IndexOf("_")));
        }
        return toReturn;
    }


    void buildSystem()
    {
        MeshRenderer[] toDestroy = GetComponentsInChildren<MeshRenderer>();
        Debug.Log("# of things to destroy "+toDestroy.Length);
        foreach(MeshRenderer tD in toDestroy)
        {
            Debug.Log("destroying something");
            Destroy(tD.gameObject);
        }
        switch (renderingMethod)
        {            //mcindoe said 0.3f for ball and stick and 1.0f for the space filling, i think it should be more like 0.5f and 1.25f

            case 0: // ball and stick
                scaleFactor = 0.5f;
                bondScaleFactor = 0.015f;
                break;

            case 1: // space filling
                scaleFactor = 1.45f;
                bondScaleFactor = 0.0001f;
                break;

            case 2: // stick only
                scaleFactor = 0.033f;
                bondScaleFactor = 0.03f;
                break;

            default:
                break;
        }
        
        GameObject molMasObject = GameObject.Find("MoleculeMasterManager");
        mM = molMasObject.GetComponent<MoleculeMaster>();
        if (mM != null)
        {
            string[] atomNames = getPrimaryAndSecondaryAtomNames(modelName);
            buildMol(getAtoms(atomNames));
        }
    }

    public void buildMol(AtomStorage[] atomStore) { //all of these cases could be confined to a seperate function?
        //*****thinking might be worthwhile to first generate the atoms and then generate the orbitals
        AtomStorage coreAtom = buildCoreAtom(atomStore[0]);
        AtomStorage[] secondaryAtoms;
        GameObject[] lonePairs;
        Vector3[] atomVecs;
        Vector3[] allVecs;
        float[] bondLengths;
        switch (mM.molShape[modelName])//switches based on the inputed shape which is assigned to the name
        {// to do, orbital clouds, and send a few options to scott
            

            case "Linear2":
                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 2,coreAtom);
                //generates the number of secondary atom objects specified
                //Relies on all the secondary atoms to be the same type

                atomVecs = generateLinearVecs(modelName); //could pass an angle cuz only one angle needed, but to standarize passing a name

                bondLengths = mM.bondLengthsNano[modelName]; //get bond lengths from dictionary (maybe this is unneeded step, yes but helps with understanding of code)

                transformSecondaryAtoms(secondaryAtoms, atomVecs, bondLengths);
                transformSecondaryAtomBonds(secondaryAtoms, atomVecs, bondLengths);
                // transformSecondaryAtoms(generateSecondaryAtoms(atomStore[1], 2, coreAtom), generateLinearVecs(molName), mM.bondLengthsNano[molName]);
                //could do it this way to save lines, but its clearer whats happening if i dont do it like this

                Debug.Log("Linear2");
                break;
           

            case "TrigonalPlanar3":
                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 3,coreAtom);

                atomVecs = generateTrigonalPlanarVecs(modelName); //in this case, mole name is good not just angle (even tho angles are all the same)

                bondLengths = mM.bondLengthsNano[modelName];

                transformSecondaryAtoms(secondaryAtoms, atomVecs, bondLengths);
                transformSecondaryAtomBonds(secondaryAtoms, atomVecs, bondLengths);

                Debug.Log("TrigonalPlanar3");
                break;

            case "Bent3":
                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 2, coreAtom);
                lonePairs = generateLonePairs(1, coreAtom);

                allVecs = generateTrigonalPlanarVecs(modelName);
                atomVecs = new Vector3[] { allVecs[0], allVecs[1] };
                allVecs = new Vector3[] { allVecs[2] };


                bondLengths = mM.bondLengthsNano[modelName];
                transformSecondaryAtoms(secondaryAtoms, atomVecs, bondLengths);
                transformSecondaryAtomBonds(secondaryAtoms, atomVecs, bondLengths);
                transformLonePairs(lonePairs, allVecs, coreAtom);
                //this.transform.localRotation = lookDirection(Vector3.right);
                Debug.Log("Bent3");
                break;

            case "Tetrahedral4":
                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 4, coreAtom);

                atomVecs = generateTetrahedralVecs(modelName);
                bondLengths = mM.bondLengthsNano[modelName];

                transformSecondaryAtoms(secondaryAtoms, atomVecs, bondLengths);
                transformSecondaryAtomBonds(secondaryAtoms, atomVecs, bondLengths);

                Debug.Log("Tetrahedral4");
                break;

            case "TrigonalPyramidal4":
                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 3, coreAtom);
                lonePairs = generateLonePairs(1, coreAtom);

                allVecs = generateTrigonalPyramidalVecs(modelName);
                atomVecs = new Vector3[] { allVecs[0], allVecs[1],allVecs[2] };
                allVecs = new Vector3[] { allVecs[3] };

                bondLengths = mM.bondLengthsNano[modelName];
                transformSecondaryAtoms(secondaryAtoms, atomVecs, bondLengths);
                transformSecondaryAtomBonds(secondaryAtoms, atomVecs, bondLengths);
                transformLonePairs(lonePairs, allVecs, coreAtom);
                //this.transform.localRotation = lookDirection(Vector3.right);
                Debug.Log("TrigonalPyramidal4");
                break;

            case "Bent4":
                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 2, coreAtom);
                lonePairs = generateLonePairs(2,coreAtom);

                allVecs = generateBentVecs(modelName);
                atomVecs = new Vector3[] { allVecs[0], allVecs[1] };
                allVecs = new Vector3[] { allVecs[2], allVecs[3] };

                bondLengths = mM.bondLengthsNano[modelName];
                transformSecondaryAtoms(secondaryAtoms, atomVecs, bondLengths);
                transformSecondaryAtomBonds(secondaryAtoms, atomVecs, bondLengths);
                transformLonePairs(lonePairs, allVecs, coreAtom);
                //this.transform.localRotation = lookDirection(Vector3.right);
                Debug.Log("Bent4");
                break;


            case "TrigonalBiPyramidal5":
                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 5, coreAtom);

                atomVecs = generateTrigonalBiPyramidalVecs(modelName,Vector3.up);

                bondLengths = mM.bondLengthsNano[modelName];

                transformSecondaryAtoms(secondaryAtoms, atomVecs, bondLengths);
                transformSecondaryAtomBonds(secondaryAtoms, atomVecs, bondLengths);
                Debug.Log("TrigonalBiPyramidal5");
                break;

            case "SeeSaw5":
                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 4, coreAtom);
                lonePairs = generateLonePairs(1, coreAtom);

                allVecs = generateTrigonalBiPyramidalVecs(modelName, Vector3.right);
                atomVecs = new Vector3[] { allVecs[0], allVecs[1],allVecs[2], allVecs[3] };
                allVecs = new Vector3[] { allVecs[4] };


                bondLengths = mM.bondLengthsNano[modelName];
                transformSecondaryAtoms(secondaryAtoms, atomVecs, bondLengths);
                transformSecondaryAtomBonds(secondaryAtoms, atomVecs, bondLengths);
                transformLonePairs(lonePairs, allVecs, coreAtom);
                //this.transform.localRotation = lookDirection(Vector3.right);
                Debug.Log("SeeSaw5");
                break;

            case "TShaped5":
                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 3, coreAtom);
                lonePairs = generateLonePairs(2, coreAtom);

                allVecs = generateTrigonalBiPyramidalVecs(modelName, Vector3.right);
                atomVecs = new Vector3[] { allVecs[0], allVecs[1], allVecs[2]  };
                allVecs = new Vector3[] { allVecs[3], allVecs[4] };


                bondLengths = mM.bondLengthsNano[modelName];
                transformSecondaryAtoms(secondaryAtoms, atomVecs, bondLengths);
                transformSecondaryAtomBonds(secondaryAtoms, atomVecs, bondLengths);
                transformLonePairs(lonePairs, allVecs, coreAtom);
                //this.transform.localRotation = lookDirection(Vector3.right);
                Debug.Log("TShaped5");
                break;

            case "Linear5":
                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 2, coreAtom);
                lonePairs = generateLonePairs(3, coreAtom);

                allVecs = generateTrigonalBiPyramidalVecs(modelName,Vector3.right);
                atomVecs = new Vector3[] { allVecs[0], allVecs[1] };
                allVecs = new Vector3[] { allVecs[2], allVecs[3], allVecs[4] };


                bondLengths = mM.bondLengthsNano[modelName];
                transformSecondaryAtoms(secondaryAtoms, atomVecs, bondLengths);
                transformSecondaryAtomBonds(secondaryAtoms, atomVecs, bondLengths);
                transformLonePairs(lonePairs, allVecs, coreAtom);
                //this.transform.localRotation = lookDirection(Vector3.right);
                Debug.Log("Linear5");
                break;

            case "Octahedral6":
                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 6, coreAtom);

                atomVecs = generateOctahedralVecs(modelName, Vector3.up);

                bondLengths = mM.bondLengthsNano[modelName];

                transformSecondaryAtoms(secondaryAtoms, atomVecs, bondLengths);
                transformSecondaryAtomBonds(secondaryAtoms, atomVecs, bondLengths);

                Debug.Log("Octahedral6");
                break;

            case "SquarePyramidal6":
                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 5, coreAtom);
                lonePairs = generateLonePairs(1,coreAtom);

                allVecs = generateOctahedralVecs(modelName, Vector3.up);
                atomVecs = new Vector3[] { allVecs[0], allVecs[2], allVecs[3], allVecs[4], allVecs[5] };
                allVecs = new Vector3[] { allVecs[1] };


                bondLengths = mM.bondLengthsNano[modelName];
                transformSecondaryAtoms(secondaryAtoms, atomVecs, bondLengths);
                transformSecondaryAtomBonds(secondaryAtoms, atomVecs, bondLengths);
                transformLonePairs(lonePairs, allVecs, coreAtom);
                //this.transform.localRotation = lookDirection(Vector3.right);
                Debug.Log("SquarePyramidal6");
                break;

            case "SquarePlanar6":
                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 4, coreAtom);
                lonePairs = generateLonePairs(2, coreAtom);

                allVecs = generateOctahedralVecs(modelName, Vector3.up);
                atomVecs = new Vector3[] {allVecs[2], allVecs[3], allVecs[4], allVecs[5]};//if we are doing manual assingment,
                                                                                                        ///i dont think we need to add a starting vector value
                allVecs = new Vector3[] { allVecs[0], allVecs[1] };


                bondLengths = mM.bondLengthsNano[modelName];
                transformSecondaryAtoms(secondaryAtoms, atomVecs, bondLengths);
                transformSecondaryAtomBonds(secondaryAtoms, atomVecs, bondLengths);
                transformLonePairs(lonePairs, allVecs,coreAtom);
                //this.transform.localRotation = lookDirection(Vector3.right);
                Debug.Log("SquarePlanar6");
                break;
            default:
                Debug.Log("Not a valid shape or name is not valid Name is > " + modelName + " returned shape is > " + mM.molShape[modelName]);
                break;
        }
    }

    
    public AtomStorage buildCoreAtom(AtomStorage coreAtom)
    {
        GameObject core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        core.transform.parent = this.transform;
        core.GetComponent<Renderer>().material.color = coreAtom.color;
        core.transform.localPosition = Vector3.zero;
        if (renderingMethod != 2)
        {
            core.transform.localScale = Vector3.one * coreAtom.size * scaleFactor;
        }
        else {
            core.transform.localScale = Vector3.one * scaleFactor;
        }
        coreAtom.attachedAtom = core;
        return coreAtom;
    }

    public GameObject[] generateLonePairs (int numberToGenerate, AtomStorage coreAtom)
    {
        if (lonePairsOn)
        {
            GameObject[] generatedLonePairs = new GameObject[numberToGenerate];
            for (int i = 0; i < numberToGenerate; i++)
            {
                GameObject lonePair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                lonePair.transform.parent = this.transform;
                lonePair.GetComponent<Renderer>().material = lonePairMaterial;
                lonePair.GetComponent<Renderer>().material.color = lonePairColor;
                lonePair.transform.localPosition = coreAtom.attachedAtom.transform.localPosition;
                generatedLonePairs[i] = lonePair;
            }
            return generatedLonePairs;
        }
        else
        {
            return null;
        }

    }

    public AtomStorage[] generateSecondaryAtoms (AtomStorage template, int numberToGenerate, AtomStorage coreAtom)
    {
        AtomStorage[] generatedAtoms = new AtomStorage[numberToGenerate];
        for(int i=0; i<numberToGenerate; i++) {
            AtomStorage newAtomStorage = new AtomStorage(mM, template.name);

            GameObject sec = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sec.transform.parent = this.transform;
            sec.GetComponent<Renderer>().material.color = newAtomStorage.color;
            sec.transform.localPosition = Vector3.zero;
            if (renderingMethod != 2)
            {
                sec.transform.localScale = Vector3.one * newAtomStorage.size * scaleFactor;
            }
            else
            {
                sec.transform.localScale = Vector3.one * 1f * scaleFactor;

            }
            newAtomStorage.attachedAtom = sec;

            GameObject bondCore = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bondCore.transform.parent = this.transform;
            bondCore.GetComponent<Renderer>().material.color = coreAtom.color;
            bondCore.transform.localPosition = Vector3.zero;
            bondCore.transform.localScale = Vector3.one * bondScaleFactor;
            newAtomStorage.attachedBondCore = bondCore;

            GameObject bondSec = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bondSec.transform.parent = this.transform;
            bondSec.GetComponent<Renderer>().material.color = newAtomStorage.color;
            bondSec.transform.localPosition = Vector3.zero;
            bondSec.transform.localScale = Vector3.one * bondScaleFactor;
            newAtomStorage.attachedBondSec = bondSec;
            newAtomStorage.nameCore = coreAtom.name;
            generatedAtoms[i] = newAtomStorage;
        }
        return generatedAtoms;

    }

    public string[] getPrimaryAndSecondaryAtomNames(string mol)
    {
        string primary="";
        string secondary = "";
        bool primaryFilled = false;
        foreach(char c in mol)
        {
            if (!primaryFilled)
            {
                if ((primary.Length == 0) || (primary.Length == 1 && char.IsLower(c)))
                {
                    if (char.IsLetter(c))
                    {
                        primary += c;
                    }
                    if (char.IsLower(c))
                    {
                        primaryFilled = true;
                    }
                }
                else
                {
                    primaryFilled = true;
                    if (char.IsLetter(c))
                    {
                        secondary += c;
                    }
                }
            }
            else
            {
                if (char.IsLetter(c))
                {
                    secondary += c;
                }
                if (char.IsLower(c) || char.IsNumber(c))
                {
                    break;
                }
            }
        }
        if(secondary.Equals(""))
        {
            secondary = primary;
        }
      //  Debug.Log("P> " + primary + " S> " + secondary);
        return new string[] { primary, secondary };

    }

    public AtomStorage[] getAtoms(string[] names) { //only works if all secondary atoms are the same type 

        return new AtomStorage[]
        {
             new AtomStorage(mM,names[0]),
             new AtomStorage(mM,names[1])
        };
    }
    
    public void transformSecondaryAtoms(AtomStorage[] atoms, Vector3[] vecs, float[] bondLengths)
    {
        if (atoms.Length == vecs.Length && vecs.Length == bondLengths.Length)
        {
            for (int i = 0; i < atoms.Length; i++) {
                atoms[i].attachedAtom.transform.localPosition += vecs[i] * bondLengths[i];
                //atoms[i].placeRotateAndScaleBond(vecs[i], bondLengths[i]);
            }
        }
        else {
            Debug.Log("GIVEN ARRAYS FOR SECONDARY ATOM TRANSFORMATIONS HAVE DIFFERENT LENGTHS");
        }
    }

    public void transformSecondaryAtomBonds(AtomStorage[] atoms, Vector3[] vecs, float[] bondLengths) {
        for (int i = 0; i < atoms.Length; i++)
        {
            float edgeToEdgeLength = bondLengths[i] - ((mM.atomVDW[atoms[i].nameCore] + mM.atomVDW[atoms[i].name] - 0.08f) * (scaleFactor/2f));//this is referencing scale factor, but its difficutl to assing(??what this word - jae later???), maybe remove this as a function declared by the atom 
            //the 0.08f is to place it slightly inside the atom rather than on its surface (i.e. pretend the VDW is smaller than it is)
            atoms[i].attachedBondCore.transform.localPosition += vecs[i] * (edgeToEdgeLength * (1f / 4f) + (mM.atomVDW[atoms[i].nameCore]-0.04f)*(scaleFactor/2f));
            //the 0.04f is then needed to correct!
            atoms[i].attachedBondCore.transform.localScale = scaleBond(edgeToEdgeLength);
            atoms[i].attachedBondCore.transform.localRotation = lookDirection(vecs[i]);

            atoms[i].attachedBondSec.transform.localPosition += vecs[i] * (edgeToEdgeLength * (3f / 4f) + (mM.atomVDW[atoms[i].nameCore] - 0.04f) * (scaleFactor / 2f));
            atoms[i].attachedBondSec.transform.localScale = scaleBond(edgeToEdgeLength);
            atoms[i].attachedBondSec.transform.localRotation = lookDirection(vecs[i]);
            //Debug.Log(edgeToEdgeLength);
            //Debug.Log(bondLengths[i]);
        }
    }

    public void transformLonePairs(GameObject[] lonePairs, Vector3[] vecs, AtomStorage coreAtom)
    {
        if (lonePairs!=null)
        {
            for (int i = 0; i < lonePairs.Length; i++)
            {
                lonePairs[i].transform.localPosition += (lonePairDist+ mM.atomVDW[coreAtom.name]*scaleFactor/2f) * vecs[i];
                lonePairs[i].transform.localScale = new Vector3(lonePairSizeXY, lonePairSizeZ, lonePairSizeXY);
                lonePairs[i].transform.localRotation = lookDirection(vecs[i]);
            }
        }
    }

    public Quaternion lookDirection(Vector3 lookDir)
    {
        return Quaternion.LookRotation(lookDir) * Quaternion.FromToRotation(Vector3.forward, Vector3.up);
    }

    public Vector3 scaleBond(float scaler)
    {
        return new Vector3(bondScaleFactor, scaler * (1f / 4f), bondScaleFactor);
    }

    public Vector3[] generateLinearVecs(string molName)
    {
        Vector3[] returnVecs = new Vector3[2];
        returnVecs[0] = Vector3.right;
        returnVecs[1] = Quaternion.AngleAxis(mM.bondAngDeg[molName][0], Vector3.up) * returnVecs[0];
        //swtiched from forward to up for the quaternion if geometry is weird this might be cause
        return returnVecs;
    }

    public Vector3[] generateTrigonalPlanarVecs(string molName)
    {
        Vector3[] returnVecs = new Vector3[3];
        returnVecs[0] = Vector3.right;
        returnVecs[1] = Quaternion.AngleAxis(mM.bondAngDeg[molName][0], Vector3.up) * returnVecs[0];
        returnVecs[2] = Quaternion.AngleAxis(-mM.bondAngDeg[molName][1], Vector3.up) * returnVecs[0];
        return returnVecs;
    }

    public Vector3[] generateTetrahedralVecs(string molName) ///CHANGE THIS CHANGE THIS, so water and NH3 can work for this too
    {/// doing this the dumb easy way(just rotate each of the 2nd substituant around the first bonds axis 120 degrees to form the rest of them),
        //which means it will break down if all atoms are not identical, but thats a later problem
        Vector3[] returnVecs = new Vector3[4];
        returnVecs[0] = Vector3.right;
        returnVecs[1] = Quaternion.AngleAxis(mM.bondAngDeg[molName][0], Vector3.forward) * returnVecs[0];
        returnVecs[2] = Quaternion.AngleAxis(120f, returnVecs[0]) * returnVecs[1];
        returnVecs[3] = Quaternion.AngleAxis(-120f, returnVecs[0]) * returnVecs[1];
        return returnVecs;
    }

    public Vector3[] generateTrigonalPyramidalVecs(string molName)
        //this is a stupid solution but i was tired of trying to figure it out, so we doing it via guess and check...
        //having written it now i realise its actually not that stupid, it finds the right answer in <50 rounds w/ high accuracy, and seems pretty durable...
    {
        Vector3[] returnVecs = new Vector3[4];
        returnVecs[0] = Vector3.right;
        returnVecs[1] = Quaternion.AngleAxis(-mM.bondAngDeg[molName][0], Vector3.forward) * returnVecs[0];
        Vector3 testVec = (returnVecs[0] + returnVecs[1]).normalized;
        Vector3 rotateAxis = Vector3.Cross(Vector3.Cross(testVec, returnVecs[0]), testVec);
        int i = 0;
        while((Mathf.Abs(Vector3.Angle(testVec,returnVecs[0])- mM.bondAngDeg[molName][0])>0.002f)&&(Mathf.Abs(Vector3.Angle(testVec, returnVecs[1]) - mM.bondAngDeg[molName][0]) > 0.002f) && i < 2500) {
            float angleChangeAmount = (Vector3.Angle(testVec, returnVecs[0]) - mM.bondAngDeg[molName][0])*0.5f;
            //Debug.Log(angleChangeAmount);
            testVec = Quaternion.AngleAxis(angleChangeAmount, rotateAxis) * testVec;
            i++;
            if (i == 2499) {
                Debug.Log("while loop for generating trigonal pyramidal has failed and automatically ended");
            }
        }
        Debug.Log("while loop done, angle 1 -> " + Vector3.Angle(testVec, returnVecs[0]) + "  angle 2 -> " + Vector3.Angle(testVec, returnVecs[1]) + "  target angle > " + mM.bondAngDeg[molName][0]+"  loops perfromed -> "+i);

        returnVecs[2] = testVec;
        returnVecs[3] = -(testVec + returnVecs[0] + returnVecs[1]).normalized;

        return returnVecs;
    }

    public Vector3[] generateBentVecs(string molName)
    {
        Vector3[] returnVecs = new Vector3[4];

        returnVecs[0]= Quaternion.AngleAxis(-mM.bondAngDeg[molName][0]/2f, Vector3.forward) * Vector3.down;
        returnVecs[1] = Quaternion.AngleAxis(mM.bondAngDeg[molName][0]/2f, Vector3.forward) * Vector3.down;
        returnVecs[2] = Quaternion.AngleAxis(-mM.bondAngDeg[molName][1]/2f, Vector3.right) * Vector3.up;
        returnVecs[3] = Quaternion.AngleAxis(mM.bondAngDeg[molName][1]/2f, Vector3.right) * Vector3.up;
        return returnVecs;

    }

    public Vector3[] generateTrigonalBiPyramidalVecs(string molName, Vector3 startingVec)//starting vec must not be forward/backward
    {
        Vector3[] returnVecs = new Vector3[5];
        returnVecs[0] = startingVec; 
        returnVecs[1] = Quaternion.AngleAxis(mM.bondAngDeg[molName][0]*2, Vector3.forward) * returnVecs[0];
        returnVecs[2] = Quaternion.AngleAxis(mM.bondAngDeg[molName][0], Vector3.forward) * returnVecs[0];
        returnVecs[3] = Quaternion.AngleAxis(mM.bondAngDeg[molName][1], returnVecs[0]) * returnVecs[2];
        returnVecs[4] = Quaternion.AngleAxis(-mM.bondAngDeg[molName][2], returnVecs[0]) * returnVecs[2];
        return returnVecs;
    }

    public Vector3[] generateOctahedralVecs(string molName, Vector3 startingVec)
    {
        Vector3[] returnVecs = new Vector3[6];
        returnVecs[0] = startingVec;
        returnVecs[1] = Quaternion.AngleAxis(mM.bondAngDeg[molName][2], Vector3.forward) * returnVecs[0];
        returnVecs[2] = Quaternion.AngleAxis(mM.bondAngDeg[molName][0] , Vector3.forward) * returnVecs[0];
        returnVecs[3] = Quaternion.AngleAxis(mM.bondAngDeg[molName][1], returnVecs[0]) * returnVecs[2];
        returnVecs[4] = Quaternion.AngleAxis(-mM.bondAngDeg[molName][1], returnVecs[0]) * returnVecs[2];
        returnVecs[5] = Quaternion.AngleAxis(mM.bondAngDeg[molName][1]*2, returnVecs[0]) * returnVecs[2];
        return returnVecs;
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
