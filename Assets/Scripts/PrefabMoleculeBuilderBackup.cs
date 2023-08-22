//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PrefabMoleculeBuilder : MonoBehaviour
//{
//    public bool ballAndStick = true;
//    float scaleFactor;
//    float bondScaleFactor = 0.015f;
//    public string molName;

//    public MoleculeMaster mM; //reference to the molecularmaaster
//    // Start is called before the first frame update
//    void Start()
//    {
//        if (ballAndStick)
//        {
//            scaleFactor=0.5f;//make code for swtiching
//            //2021- the update is going to be the hard part about this
//            //mcindoe said 0.3f for ball and stick and 1.0f for the space filling, i think it should be more like 0.5f and 1.25f
//        }
//        GameObject molMasObject = GameObject.Find("MoleculeMasterManager");
//        mM = molMasObject.GetComponent<MoleculeMaster>();

//        if (mM != null)
//        {
//            string[] atomNames = getPrimaryAndSecondaryAtomNames(molName);
//            buildMol(getAtoms(atomNames));
//        }
    
//    }

//    // Update is called once per frame
//    void Update()
//    {
//    }

//    public void buildMol(AtomStorage[] atomStore) { //all of these cases could be confined to a seperate function?
//        //*****thinking might be worthwhile to first generate the atoms and then generate the orbitals
//        AtomStorage coreAtom = buildCoreAtom(atomStore[0]);
//        AtomStorage[] secondaryAtoms;
//        Vector3[] atomVecs;
//        float[] bondLengths;
//        Vector3 firstAtomVec;
//        Vector3 secondAtomVec;
//        Vector3 thirdAtomVec;
//        float tempBondLength; //temp bond length is the active assigning value for bond length
//        switch (mM.molShape[molName])//switches based on the inputed shape which is assigned to the name
//        {// to do, orbital clouds, and send a few options to scott
            

//            case "Linear2":
//                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 2,coreAtom);
//                //generates the number of secondary atom objects specified
//                //Relies on all the secondary atoms to be the same type

//                atomVecs = generateLinearVecs(molName); //could pass an angle cuz only one angle needed, but to standarize passing a name

//                bondLengths = mM.bondLengthsNano[molName]; //get bond lengths from dictionary (maybe this is unneeded step, yes but helps with understanding of code)

//                transformSecondaryAtomsAndBonds(secondaryAtoms, atomVecs, bondLengths);
//                // transformSecondaryAtoms(generateSecondaryAtoms(atomStore[1], 2, coreAtom), generateLinearVecs(molName), mM.bondLengthsNano[molName]);
//                //could do it this way to save lines, but its clearer whats happening if i dont do it like this


//             //   tempBondLength = mM.bondLengthsNano[molName][0]; //get the first bond length of the molecule
//             //   firstAtomVec = Vector3.right;//produce vec for atom pos
//             ////   secondaryAtoms[0].attachedAtom.transform.localPosition += firstAtomVec*tempBondLength; //place atom based on this

//             //   secondaryAtoms[0].placeRotateAndScaleBond(firstAtomVec, tempBondLength); //place the bond based on these


//             //   tempBondLength= mM.bondLengthsNano[molName][1]; //get the second bond length of the molecule
//             //   secondAtomVec = Quaternion.AngleAxis(mM.bondAngDeg[molName][0], Vector3.forward) * Vector3.right;
//             //  // secondaryAtoms[1].attachedAtom.transform.localPosition += secondAtomVec * tempBondLength;

//             //   secondaryAtoms[1].placeRotateAndScaleBond(secondAtomVec, tempBondLength);


//                Debug.Log("Linear2");
//                break;

//            case "TrigonalPlanar3":
//                secondaryAtoms = generateSecondaryAtoms(atomStore[1], 3,coreAtom);

//                tempBondLength = mM.bondLengthsNano[molName][0];
//                firstAtomVec = Vector3.right;
//                secondaryAtoms[0].attachedAtom.transform.localPosition += firstAtomVec * tempBondLength;

//                secondaryAtoms[0].placeRotateAndScaleBond(firstAtomVec, tempBondLength);


//                tempBondLength = mM.bondLengthsNano[molName][1];
//                secondAtomVec = Quaternion.AngleAxis(mM.bondAngDeg[molName][0], Vector3.up) * Vector3.right;
//                secondaryAtoms[1].attachedAtom.transform.localPosition += secondAtomVec * tempBondLength;

//                secondaryAtoms[1].placeRotateAndScaleBond(secondAtomVec, tempBondLength);


//                tempBondLength = mM.bondLengthsNano[molName][2];
//                thirdAtomVec = Quaternion.AngleAxis(-mM.bondAngDeg[molName][1], Vector3.up) * Vector3.right;
//                secondaryAtoms[2].attachedAtom.transform.localPosition += thirdAtomVec * tempBondLength;

//                secondaryAtoms[2].placeRotateAndScaleBond(thirdAtomVec, tempBondLength);


//                Debug.Log("TrigonalPlanar3");
//                break;

//            case "Tetrahedral4":

//                Debug.Log("Tetrahedral4");
//                break;

//            default:
//                Debug.Log("Not a valid shape or name is not valid Name is > " + molName + " returned shape is > " + mM.molShape[molName]);
//                break;
//        }
//    }

    
//    public AtomStorage buildCoreAtom(AtomStorage coreAtom)
//    {
//        GameObject core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//        core.transform.parent = this.transform;
//        core.GetComponent<Renderer>().material.color = coreAtom.color;
//        core.transform.position = Vector3.zero;
//        core.transform.localScale = Vector3.one * coreAtom.size*scaleFactor;
//        coreAtom.attachedAtom = core;
//        return coreAtom;
//    }

//    public AtomStorage[] generateSecondaryAtoms (AtomStorage template, int numberToGenerate, AtomStorage coreAtom)
//    {
//        AtomStorage[] generatedAtoms = new AtomStorage[numberToGenerate];
//        for(int i=0; i<numberToGenerate; i++) {
//            AtomStorage newAtomStorage = new AtomStorage(mM, template.name);

//            GameObject sec = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//            sec.transform.parent = this.transform;
//            sec.GetComponent<Renderer>().material.color = newAtomStorage.color;
//            sec.transform.position = Vector3.zero;
//            sec.transform.localScale = Vector3.one * newAtomStorage.size*scaleFactor;
//            newAtomStorage.attachedAtom = sec;

//            GameObject bondCore = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
//            bondCore.transform.parent = this.transform;
//            bondCore.GetComponent<Renderer>().material.color = coreAtom.color;
//            bondCore.transform.position = Vector3.zero;
//            bondCore.transform.localScale = Vector3.one * bondScaleFactor;
//            newAtomStorage.attachedBondCore = bondCore;

//            GameObject bondSec = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
//            bondSec.transform.parent = this.transform;
//            bondSec.GetComponent<Renderer>().material.color = newAtomStorage.color;
//            bondSec.transform.position = Vector3.zero;
//            bondSec.transform.localScale = Vector3.one * bondScaleFactor;
//            newAtomStorage.attachedBondSec = bondSec;

//            generatedAtoms[i] = newAtomStorage;
//        }
//        return generatedAtoms;

//    }

//    public string[] getPrimaryAndSecondaryAtomNames(string mol)
//    {
//        string primary="";
//        string secondary = "";
//        bool primaryFilled = false;
//        foreach(char c in mol)
//        {
//            if (!primaryFilled)
//            {
//                if ((primary.Length == 0) || (primary.Length == 1 && char.IsLower(c)))
//                {
//                    if (char.IsLetter(c))
//                    {
//                        primary += c;
//                    }
//                    if (char.IsLower(c))
//                    {
//                        primaryFilled = true;
//                    }
//                }
//                else
//                {
//                    primaryFilled = true;
//                    if (char.IsLetter(c))
//                    {
//                        secondary += c;
//                    }
//                }
//            }
//            else
//            {
//                if (char.IsLetter(c))
//                {
//                    secondary += c;
//                }
//                if (char.IsLower(c) || char.IsNumber(c))
//                {
//                    break;
//                }
//            }
//        }
//        if(secondary.Equals(""))
//        {
//            secondary = primary;
//        }
//        Debug.Log("P> " + primary + " S> " + secondary);
//        return new string[] { primary, secondary };

//    }

//    public AtomStorage[] getAtoms(string[] names) {

//        return new AtomStorage[]
//        {
//             new AtomStorage(mM,names[0]),
//             new AtomStorage(mM,names[1])
//        };
//    }


//    public void transformSecondaryAtomsAndBonds(AtomStorage[] atoms, Vector3[] vecs, float[] bondLengths)
//    {
//        if (atoms.Length == vecs.Length && vecs.Length == bondLengths.Length)
//        {
//            for (int i = 0; i < atoms.Length; i++) {
//                atoms[i].attachedAtom.transform.localPosition += vecs[i] * bondLengths[i];
//                atoms[i].placeRotateAndScaleBond(vecs[i], bondLengths[i]);
//            }
//        }
//        else {
//            Debug.Log("GIVEN ARRAYS FOR SECONDARY ATOM TRANSFORMATIONS HAVE DIFFERENT LENGTHS");
//        }
//    }
//    public Vector3[] generateLinearVecs(string molName)
//    {
//        Vector3[] returnVecs = new Vector3[2];
//        returnVecs[0] = Vector3.right;
//        returnVecs[1] = Quaternion.AngleAxis(mM.bondAngDeg[molName][0], Vector3.forward) * returnVecs[0];
//        return returnVecs;
//    }

   

//    public class AtomStorage
//    {
//        float bondWidth = 0.015f;
//        public string name;
//        public Color32 color;
//        public float size;
//        public GameObject attachedAtom;
//        public GameObject attachedBondCore;
//        public GameObject attachedBondSec;
//        public AtomStorage(MoleculeMaster molMaster,string nameGiven)
//        {
//            color = molMaster.atomColour[nameGiven];
//            size = molMaster.atomVDW[nameGiven];
//            name = nameGiven;
//        }

//        public void placeRotateAndScaleBond(Vector3 dirVec, float fullBondLength)
//        {
//            attachedBondCore.transform.localPosition += dirVec * fullBondLength * (1f / 4f); /// should we center it not on the center of the bond, but on the center between the two atom edges?????
//            attachedBondCore.transform.localScale = scaleBond(fullBondLength);
//            attachedBondCore.transform.localRotation = lookBond(dirVec);

//            attachedBondSec.transform.localPosition += dirVec * fullBondLength * (3f / 4f);
//            attachedBondSec.transform.localScale = scaleBond(fullBondLength);
//            attachedBondSec.transform.localRotation = lookBond(dirVec);
//        }

//        public Quaternion lookBond(Vector3 lookDir)
//        {
//            return Quaternion.LookRotation(lookDir) * Quaternion.FromToRotation(Vector3.forward, Vector3.up);
//        }

//        public Vector3 scaleBond(float scaler)
//        {
//            return new Vector3(bondWidth, scaler * (1f / 4f), bondWidth);
//        }
//    }

    
//}
