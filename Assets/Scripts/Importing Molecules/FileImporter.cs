using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class FileImporter : MonoBehaviour
{
    [SerializeField]
    float VDWScaler = 0.5f;

    const string _materialFolder = "Assets/Resources/Materials"; //where to find materials 
    const string _prefabFolder = "Assets/Generated/Prefabs";
    Dictionary<string, Material> _materialCache = new Dictionary<string, Material>();

    const string _nameOfFolder = "Tests/";
    static readonly string[] _fileNames = new string[]{
        "HandoverTest.txt",
        "asda.txt"
        //"SN2Halfway.txt",
//"SN2OH-.txt",
//"SN2Reacted.txt",
//"SN2Unreacted.txt"
    //    "Newman1.txt",
    //"Newman2.txt",
    //"Newman3.txt",
        /*"Tut4.txt",
        "Tut2.txt",
        "Tut3_1.txt",
        "Tut3_2.txt"*/
       /* "B16N_CI_NSI_R0.txt",
"B1M_C_SI_R180x.txt",
"B2M_C_SI_R180y.txt",
"B3M_C_SI_R120z.txt",
"B4M_C_NSI_R180x.txt",
"B5M_C_NSI_R180y.txt",
"B6M_C_NSI_R90z.txt",
"B7M_CI_NSI_R180x.txt",
"B7N_CI_NSI_R180x.txt",
"B8M_CI_NSI_R120z.txt",
"B8N_CI_NSI_R120z.txt",
"B9M_AC_SI_R180x.txt",
"B10M_AC_SI_R180x.txt",
"B11M_AC_SI_R180y.txt",
"B12M_AC_SI_R180y.txt",
"B13M_AC_SI_R90z.txt",
"B14M_AC_SI_R60z.txt",
"B15M_CI_NSI_R180y.txt",
"B15N_CI_NSI_R180y.txt",
"B16M_CI_NSI_R0.txt",
"A16M_CI_NSI_R0_Fixed.txt",
"A16N_CI_NSI_R0_Fixed.txt"*/
        /*"A1M_C_SI_R180x.txt",
        "A2M_C_SI_R180y.txt",
        "A3M_C_SI_R120z.txt",
        "A4M_C_NSI_R180x.txt",
"A4N_C_NSI_R180x.txt",
"A5M_C_NSI_R180y.txt",
"A5N_C_NSI_R180y.txt",
"A6M_C_NSI_R180z.txt",
"A6N_C_NSI_R180z.txt",
"A7M_CI_NSI_R180x.txt",
"A7N_CI_NSI_R180x.txt",
"A8M_CI_NSI_R120z.txt",
"A8N_CI_NSI_R120z.txt",
"A9M_AC_SI_R180x.txt",
"A10M_AC_SI_R180x.txt",
"A11M_AC_SI_R180y.txt",
"A12M_AC_SI_R180y.txt",
"A13M_AC_SI_R180z.txt",
"A14M_AC_SI_R180z.txt",
"A15M_CI_NSI_R180x.txt",
"A15N_CI_NSI_R180x.txt",
"A16M_CI_NSI_R0.txt",
"A16N_CI_NSI_R0.txt"*/
       //"Testing Alkyne.txt",
        //"1NicPg29LP.txt",
        //"2TriptPg38.txt",
        //"3NorborPg38.txt",
        //"4TEOBzPg38.txt",
        /*"5EtStgPg48.txt",
        "6EthylenePg00AddPi.txt",
        "7AcetylenePg00AddPi.txt",
        "8DoxoPg75.txt",
        "9LevPg154.txt",
        "10DexPg154.txt",
        "11IPrpnlPg156.txt",
        "12SBtnlPg157.txt",
        "13RBtnlPg157.txt",
        "14MDiBrButPg00.txt",
        "15EtEclPg165.txt",
        "16CHexChairPg172.txt",
        "17CHexBoatPg172.txt",
        "18MCHexAxPg175.txt",
        "19MCHexEqPg175.txt",
        "20BPButEclPg226.txt",
        "21BPButE2Pg226.txt",
        "22BrMCHexEqPg227.txt",
        "23BrMCHexAxPg227.txt",
        "asda.txt"*/
        };
    //int fileIndex = 24;
    bool generateLonePairs = false;

    string fileName;

    [SerializeField]
    AtomDictionary aD;

    public List<Vector3> bondPairsAndOrder = new List<Vector3>();
    public List<AtomStorage> atoms = new List<AtomStorage>();


    [ContextMenu("Generate Molecules")]
    void GenerateMolecules()
    {
#if UNITY_EDITOR
    EnsureFolder(_prefabFolder);
#endif
        _materialCache.Clear();
        Vector3 drawPoint = Vector3.zero;
        for (int readInd = 0; readInd < _fileNames.Length; readInd++)
        {
            fileName = _fileNames[readInd];
            string nameNoFileType = fileName.Split("."[0])[0];
            GameObject eltern = new GameObject(nameNoFileType);
            ///this.transform.gameObject.name = nameNoFileType;
            eltern.transform.parent = this.transform;
            eltern.transform.position = drawPoint;
            drawPoint += Vector3.right;
            var sr = new StreamReader(Application.dataPath + "/Text Files/" + _nameOfFolder + fileName);
            var fileContents = sr.ReadToEnd();
            sr.Close();

            var lines = fileContents.Split("\n"[0]);
            foreach (var line in lines)
            {
                // print(line);
            }

            int numberOfAtoms = int.Parse(lines[3].Substring(0, 3));
            int numberOfBonds = int.Parse(lines[3].Substring(3, 3));
            Debug.Log("number of atoms: " + numberOfAtoms);
            Debug.Log("number of bonds: " + numberOfBonds);
            List<string> atomLines = new List<string>();
            List<string> bondLines = new List<string>();
            for (int i = 4; i < numberOfAtoms + 4; i++)
            {
                atomLines.Add(lines[i]);
                print(lines[i]);
            }
            for (int i = 4 + numberOfAtoms; i < numberOfBonds + 4 + numberOfAtoms; i++)
            {
                bondLines.Add(lines[i]);
                print(lines[i]);
            }
            Debug.Log(atomLines.Count);
            Debug.Log(bondLines.Count);

            var atomPositionsAndType = ReturnPositionsAndType(atomLines);
            bondPairsAndOrder = ReturnBondInformation(bondLines);

            atoms = GenerateAtoms(atomPositionsAndType, eltern);
            var pairedBonds = ConnectBonds(bondPairsAndOrder, atoms, eltern);
            if (generateLonePairs)
            {
                GenerateLonePairs(pairedBonds, atoms, eltern);
            }

#if UNITY_EDITOR
        PrefabUtility.SaveAsPrefabAsset(eltern, $"{_prefabFolder}/{nameNoFileType}.prefab");
        DestroyImmediate(eltern);
#endif
        }
#if UNITY_EDITOR
    AssetDatabase.SaveAssets();
#endif
    }

    void EnsureFolder(string path)
    {
        Directory.CreateDirectory(path);   // relative paths resolve to the project root
#if UNITY_EDITOR
    AssetDatabase.Refresh();
#endif
    }

    Material GetElementMaterial(string element)
    {
        if (_materialCache.TryGetValue(element, out var mat)) return mat;
#if UNITY_EDITOR
    // Search inside _materialFolder (set it to "Assets" to search the whole project)
    string[] guids = AssetDatabase.FindAssets($"{element} t:Material", new[] { _materialFolder });
    var matches = new List<string>();
    foreach (string guid in guids)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (Path.GetFileNameWithoutExtension(path) == element)   // exact name match only
            matches.Add(path);
    }

    if (matches.Count > 1)
        Debug.LogWarning($"Found {matches.Count} materials named '{element}', using {matches[0]}:\n" + string.Join("\n", matches));
    if (matches.Count > 0)
        mat = AssetDatabase.LoadAssetAtPath<Material>(matches[0]);
#endif
        if (mat == null)
            Debug.LogError($"No material named '{element}' found in {_materialFolder}");

        _materialCache[element] = mat;
        return mat;
    }

    List<Vector4> ReturnPositionsAndType(List<string> atomLines)
    {
        List<Vector4> toReturn = new List<Vector4>();

        foreach(string line in atomLines)
        {
            Vector4 newVec = Vector4.zero;
            newVec.x = float.Parse(line.Substring(0, 10))/10f;
            newVec.y = float.Parse(line.Substring(10, 10))/10f; //divide by 10 allows for matching of scale expected by end app
            newVec.z = float.Parse(line.Substring(20, 10))/10f; 
            string type = line.Substring(31, 2);
            if (char.IsWhiteSpace(type[1])){
                type = type[0].ToString();
            }
            switch (type)
            {

                case "H":
                    newVec.w = 1;
                    break;
                case "C":
                    newVec.w = 6;
                    break;
                case "N":
                    newVec.w = 7;
                    break;
                case "O":
                    newVec.w = 8;
                    break;
                case "Si":
                    newVec.w = 14;
                    break;
                case "S":
                    newVec.w = 16;
                    break;
                case "Cl":
                    newVec.w = 17;
                    break;
                case "Br":
                    newVec.w = 35;
                    break;
                default:
                    Debug.Log(type);
                    Debug.LogError("UNRECOGNIZED ATOM TYPE in " + fileName + "of the element > " + type);
                    break;
            }
            Debug.Log(newVec);
            toReturn.Add(newVec);
        }
        return toReturn;
    }
    List<Vector3> ReturnBondInformation(List<string> bondLines)
    {
        List<Vector3> toReturn = new List<Vector3>();
        foreach(string line in bondLines)
        {
            Vector3 newVec = Vector3.zero;
            newVec.x = int.Parse(line.Substring(0, 3));
            newVec.y = int.Parse(line.Substring(3, 3));
            newVec.z = int.Parse(line.Substring(6, 3));
            Debug.Log(newVec);
            toReturn.Add(newVec);
        }
        return toReturn;
    }

    List<AtomStorage> GenerateAtoms(List<Vector4> positionsAndTypes, GameObject eltern)
    {
        List<AtomStorage> atoms = new List<AtomStorage>();
        foreach (Vector4 pAT in positionsAndTypes) {
            
            string element ="H";
            Vector3 scale;
            switch (pAT.w)
            {
                case 1:
                    element = "H";
                    break;
                case 6:
                    element = "C";
                    break;
                case 7:
                    element = "N";
                    break;
                case 8:
                    element = "O";
                    break;
                case 14:
                    element = "Si";
                    break;
                case 16:
                    element = "S";
                    break;
                case 17:
                    element = "Cl";
                    break;
                case 35:
                    element = "Br";
                    break;
                default:
                    break;
            }
            GameObject atom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            atom.transform.parent = eltern.transform;
            atom.name = element;
            var rend = atom.GetComponent<Renderer>();
            atom.GetComponent<Renderer>().sharedMaterial = GetElementMaterial(element);
            atom.transform.localPosition = new Vector3(pAT.x,pAT.y,pAT.z);
            float scaleF = aD.atomVDW[element]*VDWScaler; 
            atom.transform.localScale = new Vector3(scaleF, scaleF, scaleF);
            atoms.Add(new AtomStorage(atom.transform.localPosition, element,atom));

        }
        return atoms;
    }

    AtomStorage[,] ConnectBonds(List<Vector3> bondInfo, List<AtomStorage> atoms,GameObject eltern)
    {
        AtomStorage[,] bondPairs = new AtomStorage[bondInfo.Count,2];
        for (int i = 0; i < bondPairs.Length / bondPairs.Rank; i++)
        {
            bondPairs[i, 0] = atoms[(int)bondInfo[i].x - 1];
            bondPairs[i, 1] = atoms[(int)bondInfo[i].y - 1];
        }
        for (int i = 0; i < bondPairs.Length / bondPairs.Rank; i++)
        {

            Vector3 bondDir = (bondPairs[i, 0].position - bondPairs[i, 1].position).normalized;
            Vector3[] atomEdges = new Vector3[]
            {
                bondPairs[i, 0].position + -bondDir * aD.atomVDW[bondPairs[i, 0].element] * VDWScaler/2f,
                bondPairs[i, 1].position+ bondDir * aD.atomVDW[bondPairs[i, 1].element] * VDWScaler/2f
            };
            
            Vector3 centerPoint = (atomEdges[0]+atomEdges[1])/ 2f;
            float bondLength = Vector3.Distance(atomEdges[0], atomEdges[1]);
            Vector3[] bondCenterPoints = new Vector3[]
            {
                centerPoint + bondDir * bondLength/2.5f,
                centerPoint - bondDir * bondLength/2.5f
            };
            

            Vector3 crossVec = Vector3.right;
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
                    crossVec = Vector3.Cross(FindAnotherBondPairVectorWithSameAtom(bondPairs, i),bondDir);
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

            Vector3 bondOffsetDir = Vector3.Cross(crossVec, bondDir);

            for(int j=0; j<bondInfo[i].z; j++)
            {
                for(int k=0; k<2; k++) {
                    GameObject bond = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    bond.transform.parent = eltern.transform;
                    bond.name = bondPairs[i,k].element+" bond";
                    float perJModifier = multipleBondOffset * (offseter + (float)(j * 2))/offseter; //maybe works?
                    bond.transform.localPosition = bondCenterPoints[k] + bondOffsetDir * perJModifier;
                    bond.transform.rotation = LookDirection(bondDir);
                    var bondRend = bond.GetComponent<Renderer>();
                    bond.GetComponent<Renderer>().sharedMaterial = GetElementMaterial(bondPairs[i, k].element);
                    bond.transform.localScale = new Vector3(.015f, bondLength / 2.5f, .015f);
                }
            }
            
        }
        return bondPairs;
    }

    void GenerateLonePairs(AtomStorage[,] bondPairs, List<AtomStorage> atoms, GameObject eltern)
    {
        for(int i=0; i< atoms.Count; i++)
        {
            if (atoms[i].element.Equals("N"))
            {
                Vector3 lonePairDirection = FindLonePairDirectionForAtomIndex(bondPairs, atoms[i]);
                GameObject lonePair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                lonePair.transform.parent = eltern.transform;
                lonePair.name = "lonePair";
                lonePair.transform.localPosition = atoms[i].position + lonePairDirection * (.075f+ aD.atomVDW[atoms[i].element] * VDWScaler / 2f);
                lonePair.transform.rotation = LookDirection(lonePairDirection);
                lonePair.transform.localScale = new Vector3(0.18f, 0.13f, 0.18f);

            }
        }
        //lone pair dist .15, xy size 0.25, z size .18
    }

    public Quaternion LookDirection(Vector3 lookDir)
    {
        return Quaternion.LookRotation(lookDir) * Quaternion.FromToRotation(Vector3.forward, Vector3.up);
    }

    public class AtomStorage
    {
        public Vector3 position;
        public string element;
        public GameObject atom;

        public AtomStorage(Vector3 positionC, string elementC, GameObject atomC)
        {
            position = positionC;
            element = elementC;
            atom = atomC;
        }
    }
    Vector3 FindAnotherBondPairVectorWithSameAtom(AtomStorage[,] bondPairs, int index)
    {
        Vector3 returnVector = Vector3.zero;
        AtomStorage a1 = bondPairs[index, 0];
        AtomStorage a2 = bondPairs[index, 1];
        for (int i=0; i<bondPairs.Length/bondPairs.Rank; i++)
        {
            if((bondPairs[i,0].position==a1.position|| bondPairs[i, 1].position == a1.position
                || bondPairs[i, 0].position == a2.position || bondPairs[i, 1].position == a2.position)
                && i != index)
            {
                returnVector = (bondPairs[i, 0].position - bondPairs[i, 1].position).normalized;
                break;
            }
        }
        return returnVector;
    }

    Vector3 FindLonePairDirectionForAtomIndex(AtomStorage[,] bondPairs, AtomStorage atom)
    {
        Vector3 sumVector = Vector3.zero;
        for (int i = 0; i < bondPairs.Length / bondPairs.Rank; i++)
        {
            if (bondPairs[i, 0].position == atom.position)
            {
               // Debug.Log(bondPairs[i, 0].element + " " + bondPairs[i, 1].position);
                sumVector += (bondPairs[i, 1].position - bondPairs[i, 0].position).normalized;
            }
            if (bondPairs[i, 1].position == atom.position)
            {
                //Debug.Log(bondPairs[i, 1].element + " " + bondPairs[i, 0].position);
                sumVector += (bondPairs[i, 0].position - bondPairs[i, 1].position).normalized;
            }
            
        }
        return sumVector.normalized * -1;
    }
    // Update is called once per frame
    void Update()
    {
        //ConnectBonds(bondPairsAndOrder, atoms);
    }
}
