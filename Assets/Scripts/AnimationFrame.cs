using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFrame : MonoBehaviour
{
    public List<InfoHolder> FrameInfo;
    public GameObject ReferenceObject;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetFrameInfo()
    {
        FrameInfo = new List<InfoHolder>();
        MeshRenderer[] fetch = ReferenceObject.GetComponentsInChildren<MeshRenderer>();
        foreach(var f in fetch)
        {
            if (!f.name.Contains("bond")) {

                FrameInfo.Add(new InfoHolder(f.transform.position, f.transform.rotation, 1f, f.transform.gameObject.name));
                Debug.Log("HELLO?");
            }
        }
    }

    public void DebugFrameInfo()
    {
        Debug.Log(FrameInfo.Count);
        foreach(InfoHolder fI in FrameInfo)
        {
            Debug.Log(fI.pos);
            Debug.Log(fI.rot );
            Debug.Log(fI.presence);
            Debug.Log(fI.objName);
        }
    }

    public (Vector3, Quaternion, float) GetInfoByName(string name)
    {
        Vector3 returnVec = new Vector3(-999,0,0);
        Quaternion returnRot = Quaternion.identity;
        float returnPresence = 1;

        //Debug.Log(name);
        foreach (InfoHolder fI in FrameInfo)
        {
            if (fI.objName.Equals(name))
            {
                returnVec = fI.pos;
                returnRot = fI.rot;
                returnPresence = fI.presence;
            }
        }
        if (returnVec.x==-999)
        {
            Debug.LogError("THERE IS AN ISSUE");
        }
        return (returnVec,returnRot,returnPresence);
    }

    public class InfoHolder:MonoBehaviour {
        [SerializeField]
        public Vector3 pos;
        public Quaternion rot;
        public float presence;
        public string objName;

        public InfoHolder(Vector3 posC, Quaternion rotC, float presenceC, string nameC)
        {
            pos = posC;
            rot = rotC;
            presence = presenceC;
            objName = nameC;

        }

    }


}
