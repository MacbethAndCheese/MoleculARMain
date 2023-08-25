using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    public List<AnimationObject> MainAnimationObjects = new List<AnimationObject>();
    public List<AnimationObject> SubAnimationObjects = new List<AnimationObject>();
    public AnimationFrame[] frames;
    public MoleculeMaster MM;

    private int _maxSubFrames = 1200;
    private int _currentSubFrame = 0;
    private int _animationDir = 1;


    void Awake()
    {
        Debug.Log("gothere");
        ConnectObjects();
        foreach (AnimationFrame aF in frames)
        {
            aF.SetFrameInfo();
        }
        foreach (AnimationFrame aF in frames)
        {
            Debug.Log("---");
            aF.DebugFrameInfo();
        }
        SetAnimationObjectsToFrameInfos();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Animate()
    {
        if (_currentSubFrame == _maxSubFrames - 1)
        {
            _animationDir = -1;
        }
        if (_currentSubFrame == 0)
        {
            _animationDir = 1;
        }
        _currentSubFrame += _animationDir;
        var LowerFrameBetweenAndDistance = FindWhichFramesBetweenAndHowFar(_currentSubFrame, frames.Length, _maxSubFrames);
        int lowerFrameNumber = LowerFrameBetweenAndDistance.Item1;
        float percentBetweenFrames = LowerFrameBetweenAndDistance.Item2;
        AnimateBetweenFrames(percentBetweenFrames,lowerFrameNumber,lowerFrameNumber+1);
    }

    public void Animate(float percentThroughEntireAnimation)
    {
        _currentSubFrame = Mathf.FloorToInt((float)_maxSubFrames * percentThroughEntireAnimation / 100f);

        if (_currentSubFrame == _maxSubFrames)
            _currentSubFrame--;
        
        var LowerFrameBetweenAndDistance = FindWhichFramesBetweenAndHowFar(_currentSubFrame, frames.Length, _maxSubFrames);
        int lowerFrameNumber = LowerFrameBetweenAndDistance.Item1;
        float percentBetweenFrames = LowerFrameBetweenAndDistance.Item2;
        Debug.Log(percentBetweenFrames + " < % thru frames -- entire animation % thru > " + percentThroughEntireAnimation);
        AnimateBetweenFrames(percentBetweenFrames, lowerFrameNumber, lowerFrameNumber + 1);
    }

    void ConnectObjects()
    {
        MeshRenderer[] fetch = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer f in fetch)
        {
            if (f.name.Contains("bond"))
            {
                string name = f.name;
                int endOfBondIndex = name.IndexOf("bond");
                if (endOfBondIndex != -1)
                {
                    int startOfBondedIndex = endOfBondIndex + 5;
                    string firstObject = name.Substring(startOfBondedIndex).GetUntilOrEmpty("-");
                    int secondObjectStartindex = name.IndexOf("-");
                    if (secondObjectStartindex != -1)
                    {
                        string secondObject = name.Substring(secondObjectStartindex + 1);
                        Debug.Log(name + " ---- /" + firstObject + "/ <> /" + secondObject + "/");
                        GameObject[] connected = new GameObject[2];
                        foreach (MeshRenderer toMatch in fetch)
                        {
                            if (firstObject.Equals(toMatch.name.GetUntilOrEmpty("_")))
                            {
                                connected[0] = toMatch.transform.gameObject;
                            }
                            if (secondObject.Equals(toMatch.name.GetUntilOrEmpty("_")))
                            {
                                connected[1] = toMatch.transform.gameObject;
                            }
                        }
                        if (connected[0] == null || connected[1] == null)
                        {
                            Debug.LogError("Error with matching bonds, perhaps a naming error?");
                        }
                        else
                        {
                            SubAnimationObjects.Add(new AnimationObject(f.transform.gameObject, name, connected));
                        }
                    }
                    else
                    {
                        Debug.LogError("ISSUE WITH NAMING OF THE BONDS");
                    }
                }
                else
                {
                    Debug.LogError("ISSUE WITH NAMING OF THE BONDS");
                }
            }
            else
            {
                MainAnimationObjects.Add(new AnimationObject(f.transform.gameObject, f.name.GetUntilOrEmpty("_"), f.name.Contains("#") ? true : false));
            }
        }

        foreach (AnimationObject sAO in SubAnimationObjects)
        {
            foreach (AnimationObject mAO in MainAnimationObjects)
            {
                if (sAO.ConnectedMasterObjects[0].name.GetUntilOrEmpty("_").Equals(mAO.objName))
                {
                    mAO.NextDoorObjects.Add(sAO.ConnectedMasterObjects[1]);
                }
            }
        }

        foreach (AnimationObject aO in MainAnimationObjects)
        {
            string connects = "";
            foreach (GameObject gO in aO.NextDoorObjects)
            {
                connects += " " + gO.name + ",";
            }
            Debug.Log(aO.objName + (aO.IsMasterObj ? " which is master" : " which is not a master") + " is connected to" + connects);

        }
        foreach (AnimationObject aO in SubAnimationObjects)
        {
            Debug.Log(aO.ConnectedMasterObjects[0].name + "-" + aO.ConnectedMasterObjects[1].name);
        }
    }

    void SetAnimationObjectsToFrameInfos()
    {

        foreach (AnimationObject mAO in MainAnimationObjects)
        {

            mAO.SetLengths(frames.Length);
            for (int i = 0; i < frames.Length; i++)
            {
                var values = frames[i].GetInfoByName(mAO.objName);
                mAO.SetVars(values.Item1, values.Item2, values.Item3, i);
            }
        }

        foreach (AnimationObject mAO in MainAnimationObjects)
        {
            mAO.SetStartOfFrameNextDoorDist();
        }
    }

    public void DebugSetToFrame(int frame)
    {
        Debug.Log(MainAnimationObjects.Count);

        foreach (AnimationObject mAO in MainAnimationObjects)
        {
            mAO.SetToFrame(frame);
        }
    }

    public void AnimateBetweenFrames(float percentToNextFrame, int startFrame, int endFrame)
    {
        //Debug.Log(percentToNextFrame);
        //Debug.Log(MainAnimationObjects.Count);
        foreach (AnimationObject mAO in MainAnimationObjects)
        {
            //  Debug.Log("here");
            mAO.AnimateBetweenFrames(percentToNextFrame, startFrame, endFrame);
        }
        foreach (AnimationObject sAO in SubAnimationObjects)
        {
            sAO.AnimateBetweenFramesBonds();
        }
    }

    public (int, float) FindWhichFramesBetweenAndHowFar(int currentSubFrame, int numberOfMasterFrames, int maxSubFrames) //i know all these are avaliable to the entire class, but i like the clarity of passing the values
    {
        int lowerFrameNum = 0;
        float percentBetweenFrames = 0f;

        float subFramesBetweenMasterFrames = (float)maxSubFrames / (float)(numberOfMasterFrames-1);
        lowerFrameNum = Mathf.FloorToInt((float) currentSubFrame / subFramesBetweenMasterFrames);
        
        percentBetweenFrames = ((float)currentSubFrame % subFramesBetweenMasterFrames)/subFramesBetweenMasterFrames*100f;
        Debug.Log(percentBetweenFrames+" < % between master frames, lower frame # > "+lowerFrameNum+"     current sub frame> "+currentSubFrame);

        return (lowerFrameNum, percentBetweenFrames);
    }
    //void DebugSetToFrame(int frame)
    //{
    //    foreach(AnimationObject mAO in MainAnimationObjects)
    //    {
    //        foreach()
    //        if(mAO.objName.Equals())
    //    }
    //}
}
