using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class QRTracker : MonoBehaviour
{
    private MoleculeManager m_MoleculeManager;

    private ARTrackedImageManager m_TrackedImageManager;
    // Start is called before the first frame update
    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
        m_MoleculeManager = GetComponent<MoleculeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
            foreach (ARTrackedImage trackedImage in eventArgs.added)
            {
           // Debug.Log("tracked image" + trackedImage);
           // Debug.Log("event args" +eventArgs.added.Count);
           // if (m_MoleculeManager != null) { Debug.Log("not null"); } else { Debug.Log("null"); }
                m_MoleculeManager.AddMolecule(trackedImage);
            }

            foreach (ARTrackedImage trackedImage in eventArgs.updated)
            {
                m_MoleculeManager.UpdateMolecule(trackedImage);
            }

            foreach (ARTrackedImage trackedImage in eventArgs.removed)
            {
             Debug.Log("Seeing for event removed");
                m_MoleculeManager.SetRemovals(trackedImage);
            }
        if (eventArgs.removed.Count > 0)
        {
            m_MoleculeManager.RemoveVisualsInToRemoveBuffer();
        }
        
    }
}
