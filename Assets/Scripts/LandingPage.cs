using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LandingPage : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject instructionsPanel;
    static bool alreadyClosed=false;
    // Start is called before the first frame update
    void Awake()
    {
        if (alreadyClosed) {
            mainPanel.SetActive(false);
            instructionsPanel.SetActive(true);
        }
        else
        {
            mainPanel.SetActive(true);
            instructionsPanel.SetActive(false);
        }

    }
    

    // Update is called once per frame
    void Update()
    {
    }

    public void OpenInstructions()
    {
        mainPanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    public void CloseInstructions()
    {
        if (alreadyClosed)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            mainPanel.SetActive(true);
            instructionsPanel.SetActive(false);
        }
    }

    public void StartProgram()
    {
        alreadyClosed = true;
        SceneManager.LoadScene(1);
    }
}
