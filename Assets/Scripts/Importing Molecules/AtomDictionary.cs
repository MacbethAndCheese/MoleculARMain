using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomDictionary : MonoBehaviour
{
    public float nanoMeterBondWidth = 0.025f;


    //color values from here https://pymolwiki.org/index.php/Color_Values, with personal modifications to make things visible 
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
        { "Cl", new Color32(31, 240, 31, 255) },
        { "Br", new Color32(166,41,41,255) },
        { "Xe", new Color32(66,158,176,255) },
        { "Si", new Color32(240, 200, 160, 255)},
          };

    //VDW in nano metre 
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
        {"Si", 0.219f },
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
