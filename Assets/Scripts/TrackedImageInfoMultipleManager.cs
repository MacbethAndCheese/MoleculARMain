using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class TrackedImageInfoMultipleManager : MonoBehaviour
{
    //[SerializeField]
    //private GameObject welcomePanel;

    //[SerializeField]
    //private Button dismissButton;

    //[SerializeField]
    //private Text imageTrackedText;

    [SerializeField]
    private GameObject[] arObjectsToPlace;

    [SerializeField]
    public Vector3 scaleFactor = new Vector3(0.03f, 0.03f, 0.03f);

    public Vector3 attachedPosition = Vector3.up * 1000f;
    public float scaleFloat = 4.907f;
    public bool modelLocked = false;
    public float scaleFactorMod = 0.1f; //this is the amount that the scale will be multiplied by to bring it down to the camera scale
    //^^ was originally 0.001f for the prefabs, but when using a generator, needed to be changed
    private ARTrackedImageManager m_TrackedImageManager;

    private Dictionary<string, GameObject> arObjects = new Dictionary<string, GameObject>();


    void Awake()
    {
        //dismissButton.onClick.AddListener(Dismiss);
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();

        foreach (GameObject arObject in arObjectsToPlace) {
            GameObject newARObject = Instantiate(arObject, Vector3.zero, Quaternion.identity);
            newARObject.name = arObject.name;
            newARObject.transform.localScale = Vector3.one*scaleFactorMod*Mathf.Pow(2f,scaleFloat);
            newARObject.transform.position = Vector3.up * 1000f;
            arObjects.Add(arObject.name, newARObject);
            
        }
    }
    private void Update()
    {
      //x  Debug.Log(scaleFloat + " << this is the scale float");
    }
    private void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    //private void Dismiss() => welcomePanel.SetActive(false);

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        if (!modelLocked)
        {
            foreach (ARTrackedImage trackedImage in eventArgs.added)
            {
                UpdateARImage(trackedImage);
            }

            foreach (ARTrackedImage trackedImage in eventArgs.updated)
            {
                UpdateARImage(trackedImage);
            }

            foreach (ARTrackedImage trackedImage in eventArgs.removed)
            {
                arObjects[trackedImage.name].SetActive(false);
            }
        }
    }

    private void UpdateARImage(ARTrackedImage trackedImage) {
      //  imageTrackedText.text = trackedImage.referenceImage.name;

        AssignGameObject(trackedImage.referenceImage.name, trackedImage.transform.position);
        Debug.Log($"trackedImage.referenceImage.name: {trackedImage.referenceImage.name}");
    }

    void AssignGameObject(string name, Vector3 newPosition)
    {
        if(arObjectsToPlace != null)
        {
            arObjects[name].SetActive(true);
            attachedPosition = newPosition + (Vector3.up * 0.1f);
            arObjects[name].transform.position = attachedPosition;
            arObjects[name].transform.localScale = Vector3.one * scaleFactorMod * Mathf.Pow(2f, scaleFloat); 
            foreach (GameObject go in arObjects.Values) {
                if (go.name != name)
                {
                    go.SetActive(false);
                }
            }
        }
    }
}
