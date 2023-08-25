using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeMaster : MonoBehaviour
{//is more of a library than anything, and should be renamed


    public float nanoMeterBondWidth = 0.025f;

    public Dictionary<string, Color32> atomColour = new Dictionary<string, Color32>() {
        { "H", new Color32(255,255,255,255) },
        { "Be", new Color32(194,255,0,255) },
        { "B", new Color32(255,181,181,255) },
        { "C", new Color32(144,144,144,255) },
        { "N", new Color32(48,80,248,255) },
        { "O", new Color32(255,13,13,255) },
        { "F", new Color32(144,224,80,255) },
        { "P", new Color32(255,128,0,255) },
        { "S", new Color32(255,255,48,255) },
        { "Br", new Color32(166,41,41,255) },
        { "Xe", new Color32(66,158,176,255) },
          };

    public Dictionary<string, float> atomVDW = new Dictionary<string, float>() {
        { "H", 0.110f },
        { "Be", 0.153f },
        { "B", 0.192f },
        { "C", 0.170f },
        { "N", 0.155f },
        { "O", 0.152f },
        { "F", 0.147f },
        { "P", 0.18f },
        { "S", 0.18f },
        { "Cl", 0.175f },
        { "Br", 0.185f },
        { "Xe", 0.216f },
          };

    public Dictionary<string, float[]> bondLengthsNano = new Dictionary<string, float[]>() {
        //axial bond lengths, before equatorial
        { "BeF2", new float[]{0.154f,0.154f} },
        { "O3", new float[]{0.1293f, 0.1293f } },
        { "BF3", new float[]{0.1333f, 0.1333f, 0.1333f } },
        { "CH4", new float[]{0.1091f, 0.1091f, 0.1091f, 0.1091f } },
        { "NH3", new float[]{0.1008f, 0.1008f, 0.1008f } },
        { "OH2", new float[]{0.0958f, 0.0958f } },
        { "PF5", new float[]{0.158f, 0.158f, 0.153f, 0.153f, 0.153f } },
        { "SF4", new float[]{0.1646f, 0.1646f, 0.1545f, 0.1545f } },
        { "BrF3", new float[]{0.181f, 0.181f, 0.172f } },
        { "XeF2", new float[]{0.19773f, 0.19773f } },
        { "SF6", new float[]{0.1564f, 0.1564f, 0.1564f, 0.1564f, 0.1564f, 0.1564f } },
        { "BrF5", new float[]{0.1774f, 0.1689f, 0.1689f, 0.1689f, 0.1689f } },
        { "XeF4", new float[]{0.194f, 0.194f, 0.194f, 0.194f } },
          };



    public Dictionary<string, float[]> bondAngDeg = new Dictionary<string, float[]>() {
        // the ones w/ repeats in their values are those that have easy simulation of differences in these values
        //ie it talks about how im generating them more than it does anything else
        //for things which axial and equitorial values, the first bond angle is between ax and equitorial, and the second is between equitorial parts

        { "BeF2", new float[]{179.9f} },
        { "O3", new float[]{116.8f, 121.6f} }, //bond angle between O and O, and between the O and lone pair
        { "BF3", new float[]{120f, 120f} },
        { "CH4", new float[]{109.47f, 109.47f } }, //H-C-H, H-C-H //need to swtich how tetra is being generated
        { "NH3", new float[]{106.7f} }, //this is the bond angle between the H's
        { "OH2", new float[]{104.45f, 114.49f} },  //bond angle between H and H, and between lone pairs
        { "PF5", new float[]{90f, 120f, 120f} }, //ax-eq, eq-eq, eq-eq
        { "SF4", new float[]{90f, 101.6f, 129.2f} }, //ax-eq, eq-eq, eq-lone pair
        { "BrF3", new float[]{86.2f, 120.0f, 120.0f} }, //ax-eq, eq-lone pair, eq-lone pair
        { "XeF2", new float[]{90f, 120f, 120f} }, //F-Xe-F, lone pair positions
        { "SF6", new float[]{90.0f, 90.0f, 180f}}, // ax-eq, eq-eq, ax-ax
        { "BrF5", new float[]{ 84.8f, 90.0f, 180f } }, //ax-eq, eq-eq, ax-longpair
        { "XeF4", new float[]{ 90.0f, 90.0f, 180f } },
          };

    public Dictionary<string, string> molShape = new Dictionary<string, string> {
        { "BeF2", "Linear2" },
        { "O3", "Bent3" },
        { "BF3", "TrigonalPlanar3" },
        { "CH4", "Tetrahedral4" },
        { "NH3", "TrigonalPyramidal4" }, // just need to do these now
        { "OH2", "Bent4" },
        { "PF5", "TrigonalBiPyramidal5" },
        { "SF4", "SeeSaw5" },
        { "BrF3", "TShaped5" },
        { "XeF2", "Linear5" },
        { "SF6", "Octahedral6" },
        { "BrF5", "SquarePyramidal6" },
        { "XeF4", "SquarePlanar6" },
          };


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
