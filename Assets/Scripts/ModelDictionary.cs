using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelDictionary : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> allModelObjects;

    [SerializeField]
    public List<AnimationController> animationControllers;


    // public List<Visual> allModelGroups;

    public Dictionary<string, List<GameObject>> ModelsByVisualName
        = new Dictionary<string, List<GameObject>>(){};

    public Dictionary<string, List<AnimationController>> AnimationsByAnimName
        = new Dictionary<string, List<AnimationController>>() { };

    //public Dictionary<string, bool> HasLonePairs = new Dictionary<string, bool>() { };
    void Awake()
    {
        foreach(GameObject gO in allModelObjects)
        {
            List<GameObject> groupedGameObjects = new List<GameObject>();
            string nameC = gO.name;
            string modelGroupingName = nameC.Substring(0, nameC.IndexOf("_"));
            // example name would be Planar3_BallAndStick-additionalData.dae
            //so modelGroupingName = Planar3 and modelType = BallAndStick

            foreach (GameObject gOCheck in allModelObjects) {
                string nameCheck = gOCheck.name;
                string modelGroupingNameCheck = nameCheck.Substring(0, nameCheck.IndexOf("_"));
                if(modelGroupingNameCheck==modelGroupingName)//not avoiding adding itself, as im relying on a self add to add the gO to the list
                {
                    groupedGameObjects.Add(gOCheck);
                }

            }

            if (ModelsByVisualName.Count == 0)//if the dictionary is currently empty
            {
                ModelsByVisualName.Add(modelGroupingName, groupedGameObjects);
            }
            else
            {
                if (!ModelsByVisualName.ContainsKey(modelGroupingName))
                {
                    ModelsByVisualName.Add(modelGroupingName, groupedGameObjects);
                }
            }
        }

        foreach (AnimationController aC in animationControllers)
        {
            List<AnimationController> groupedAnimControllers = new List<AnimationController>();
            string nameC = aC.name;
            string animGroupingName = nameC.Substring(0, nameC.IndexOf("_"));
            // example name would be Planar3_BallAndStick-additionalData.dae
            //so modelGroupingName = Planar3 and modelType = BallAndStick

            foreach (AnimationController aCCheck in animationControllers)
            {
                string nameCheck = aCCheck.name;
                string animGroupingNameCheck = nameCheck.Substring(0, nameCheck.IndexOf("_"));
                if (animGroupingNameCheck == animGroupingName)//not avoiding adding itself, as im relying on a self add to add the gO to the list
                {
                    groupedAnimControllers.Add(aCCheck);
                }

            }

            if (AnimationsByAnimName.Count == 0)//if the dictionary is currently empty
            {
                AnimationsByAnimName.Add(animGroupingName, groupedAnimControllers);
            }
            else
            {
                if (!AnimationsByAnimName.ContainsKey(animGroupingName))
                {
                    AnimationsByAnimName.Add(animGroupingName, groupedAnimControllers);
                }
            }
        }

        // CheckDictionaryAssignmentIsCorrect();
        CheckDictionaryAssignmentIsCorrect();
    }

    void ReassignNamesInGroupsBasedOnType() { ///DO NOT RUN THIS CODE IT WILL OVERWRITE THE PREFABS AND CAUSE SERIOUS ISSUES
        //this can be fixed by making base prefabs static and then making them unstatic 
        foreach (KeyValuePair<string, List<GameObject>> kvp in ModelsByVisualName)
        {
            foreach (GameObject gO in kvp.Value)
            {
                string nameC = gO.name;
                string modelType = nameC.Substring(nameC.IndexOf("_") + 1, nameC.IndexOf("-") - nameC.IndexOf("_") - 1);
                gO.name = modelType;
            }
        }
    }

    void CheckDictionaryAssignmentIsCorrect()
    {
        foreach(KeyValuePair<string,List<GameObject>> kvp in ModelsByVisualName)
        {
            //Debug.Log( "OVERALL GROUP " + kvp.Key);
            foreach(GameObject gO in kvp.Value)
            {

             //   Debug.Log( kvp.Key +" " +gO.name);

            }
        }

        foreach (KeyValuePair<string, List<AnimationController>> kvp in AnimationsByAnimName)
        {
            //Debug.Log( "OVERALL GROUP " + kvp.Key);
            foreach (AnimationController aC in kvp.Value)
            {

                //Debug.Log( kvp.Key +" " +aC.name);

            }
        }
    }
}
