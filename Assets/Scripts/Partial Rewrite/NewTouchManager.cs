using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// <para> Deals with Touches and Canvas modification</para>
/// </summary>
public class NewTouchManager : MonoBehaviour
{//does both touch and canvas stuff

    /// TO ADD, ALLOW THE PHONE ROTATION TO MODIFY HOW THINGS ARE WORKING


    
    public Image touchImage;

    public Image settingsImage;

    public Image scanQRCode;

    public Sprite ballAndStickMode;

    public Sprite spaceFillingMode;

    public Sprite showLonePairs;

    public Sprite hideLonePairs;

   


    public MoleculeManager m_MoleculeManager;

  /*  private List<VisualisedTouch> aliveVisualisedTouches = new List<VisualisedTouch>();  //debugging only
    private List<VisualisedTouch> dyingVisualiedTouches = new List<VisualisedTouch>();*/

    public Canvas mainCanvas;



    //public Text toolText;
    //public Text settingsText;
    //public Text rSText;
    //public Text sLPText;

    /// <summary>
    /// <para> Rendering Style Button</para>
    /// </summary>
    public Button rSButton;
    /// <summary>
    /// <para> Show Lone Pairs Button</para>
    /// </summary>
    public Button sLPButton;
    
   // public Button deselectSettingsButton;
    public Button settingsButton;

    public GameObject settingsCanvas;


    bool settingsOpen = false;
    bool lonePairsShown = false;
    enum RenderingStyle { Ball_and_Stick = 0, Space_Filling = 1 };
    RenderingStyle activeStyle = RenderingStyle.Ball_and_Stick;
    enum LonePairsShown { Show_Lone_Pairs = 0, Hide_Lone_Pairs = 1 };
    LonePairsShown lonePairRendering = LonePairsShown.Show_Lone_Pairs;
   // RenderingStyle activeStyle = RenderingStyle.Ball_and_Stick;
    //private Text lockText;

    // private Vector4[] touchUIPositions;

    // private Vector2 screenInfo;

    private Touch touch;
    private float[] timeSinceTap = new float[2];

    
    // Start is called before the first frame update
    void Awake()
    {
        settingsCanvas.SetActive(settingsOpen);
      //  deselectSettingsButton.gameObject.SetActive(settingsOpen);
        settingsButton.gameObject.SetActive(!settingsOpen);

       // //load Helvetica
       // Font helvetica;

       // helvetica = Resources.Load<Font>("Fonts/Helvetica");

       // // Create Canvas GameObject.
       // GameObject canvasMainGO = new GameObject();
       // canvasMainGO.AddComponent<Canvas>();
       // canvasMainGO.AddComponent<CanvasScaler>();
       // canvasMainGO.AddComponent<GraphicRaycaster>();
       // canvasMainGO.name = "Main Canvas";

       // GameObject canvasSettingGO = new GameObject();
       // canvasSettingGO.AddComponent<Canvas>();
       // canvasSettingGO.AddComponent<CanvasScaler>();
       // canvasSettingGO.AddComponent<GraphicRaycaster>();
       // canvasSettingGO.name = "Setting Canvas";



       // // Set canvas to object
       // mainCanvas = canvasMainGO.GetComponent<Canvas>();
       // mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        

       // settingCanvas = canvasSettingGO.GetComponent<Canvas>();
       // settingCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

       // //Set up the setting canvas
       // settingCanvas.transform.SetParent(canvasMainGO.transform);
       // RectTransform sCRT = settingCanvas.GetComponent<RectTransform>();
       // sCRT.pivot = new Vector2(1, 1);
       // sCRT.localScale = new Vector2((1f/3f), 0.5f);
       // sCRT.anchorMax = new Vector2(1, 1);
       // sCRT.anchorMin = new Vector2(1, 1);
       // sCRT.anchoredPosition = new Vector2(0, 0);




       // // Create the Text GameObjects.
       // GamrectTransformOTool = new GameObject();
       // textGOTool.transform.parent = canvasMainGO.transform;
       // textGOTool.AddComponent<Text>();
       // textGOTool.name = "Tool Text";

       // Gamtempect textGOLock = new GameObject();
       // textGOLock.transform.parent = canvasSettingGO.transform;
       // textGOLock.AddComponent<Text>();
       // textGOLock.name = "Lock Setting Text";




       // // Set Text component properties.
       // toolText = textGOTool.GetComponent<Text>();
       // toolText.font = helvetica;
       // toolText.text = "TEST";
       // toolText.fontSize = 48;
       // toolText.alignment = TextAnchor.MiddleCenter;

       // lockText = textGOLock.GetComponent<Text>();
       // lockText.font = helvetica;
       // lockText.text = "Lock";
       // lockText.fontSize = 16;
       // lockText.alignment = TextAnchor.MiddleCenter;


       // // Provide Text position and size using RectTransrectTransform RectTransform rectTransform;
       // RectTransform rectTransform;
       // rectTransform = toolText.GetComponent<RectTransform>();
       // rectTransform.localPosition = new Vector3(0, 0, 0);
       // rectTransform.sizeDelta = new Vector2(600, 200);

       // RectTransform temp;
       // temp = lockText.GetComponent<RectTransform>();
       // temp.localPosition = new Vector2(-sCRT.sizeDelta.x / 2, -sCRT.sizeDelta.y / 2);
       // temp.sizeDelta = new Vector2(100, 30);

       // GameObject settingsToggleGO = new GameObject();
       // settingsToggleGO.transform.parent = canvasMainGO.transform;
       // settingsToggleGO.AddComponent<Image>();
       // settingsToggleGO.name = "Settings Toggle";

       // settingsToggle = settingsToggleGO.GetComponent<Image>();
       // settingsToggle = Instantiate(settingsImage, Vector3.zero, Quaternion.identity);
       // RectTransform sTRC = settingsToggle.GetComponent<RectTransform>();
       // sTRC.localPosition = new Vector2(touchUIPositions[0].x, touchUIPositions[0].z);
       // //sTRC.localPosition = new Vector2((touchUIPositions[0].x - touchUIPositions[0].z) / 2f, (touchUIPositions[0].y - touchUIPositions[0].w) / 2f);
       // //settingsToggle.transform.localPosition = new Vector2((touchUIPositions[0].x - touchUIPositions[0].z) / 2f, (touchUIPositions[0].y - touchUIPositions[0].w) / 2f);
       //// Debug.Log("Where but go" + (touchUIPositions[0].x - touchUIPositions[0].z) / 2f + " -- " + (touchUIPositions[0].y - touchUIPositions[0].w) / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
        timeSinceTap[0] += Time.deltaTime; //1 finger tap
        timeSinceTap[1] += Time.deltaTime; //2 finger tap

        //rSButton.GetComponentInChildren<Text>().text = WriteOppositeToRendering(activeStyle);// change to an image setting
        //sLPButton.GetComponentInChildren<Text>().text = WriteOppositeToLonepairsOn(lonePairRendering);
        rSButton.GetComponent<Image>().sprite = getRenderingTextureReturnOpposite(activeStyle);
        sLPButton.GetComponent<Image>().sprite = getLonePairOnOffReturnOpposite(lonePairRendering);

       // VisualizeTheTouch();
        SendTouchesToMoleculeManager();

        scanQRCode.gameObject.SetActive(!m_MoleculeManager.AreVisualsActive());
        
        //ToolTextUpdate();
    }

    /// <summary>
    /// <para> Returns the button sprite opposite to the active rendering style, so that button shows the option of what can be changed to </para>
    /// </summary>
    private Sprite getRenderingTextureReturnOpposite(RenderingStyle rS) {
        switch (rS)
        {
            case RenderingStyle.Ball_and_Stick:
                return spaceFillingMode; 
            case RenderingStyle.Space_Filling:
                return ballAndStickMode;
            default:
                return null;
                
        }
    }
    /// <summary>
    /// <para> Returns the button sprite opposite to the whether loine pairs are shown or not, so that button shows the option of what can be changed to </para>
    /// </summary>
    private Sprite getLonePairOnOffReturnOpposite(LonePairsShown lPS) {
        switch (lPS)
        {
            case LonePairsShown.Hide_Lone_Pairs:
                return showLonePairs;
            case LonePairsShown.Show_Lone_Pairs:
                return hideLonePairs;
            default:
                return null;
        }
    }

    /// <summary>
    /// <para> Changes whether the settings panel is active depending on the passed bool value </para>
    /// </summary>
    public void ChangeSettingsActive(bool changeTo )
    {
        Debug.Log("PRESSED");
        settingsOpen = changeTo;
        settingsCanvas.SetActive(settingsOpen);
        settingsButton.gameObject.SetActive(!settingsOpen);
     //   deselectSettingsButton.gameObject.SetActive(settingsOpen);
    }

    /// <summary>
    /// <para> Flips the rendering style to the opposite version </para>
    /// MAY BE CHNAGED TO BE LESS OF A TOGGLE IN FUTURE
    /// </summary>
    public void ChangeRenderingStyle()
    {
        if (activeStyle == RenderingStyle.Ball_and_Stick)
        {
            activeStyle = RenderingStyle.Space_Filling;
        }
        else
        {
            activeStyle = RenderingStyle.Ball_and_Stick;
        }
        int modifiedRenderAndLonePairValue = (int)activeStyle * 2 + (int)lonePairRendering;
        m_MoleculeManager.UpdateRendering(modifiedRenderAndLonePairValue);
    }

    /// <summary>
    /// <para> Toggles lone pairs being visible </para>
    /// </summary>
    public void ToggleLonePairs()
    {
        if(lonePairRendering == LonePairsShown.Show_Lone_Pairs)
        {
            lonePairRendering = LonePairsShown.Hide_Lone_Pairs;
        }
        else
        {
            lonePairRendering = LonePairsShown.Show_Lone_Pairs;
        }
        int modifiedRenderAndLonePairValue = (int)activeStyle * 2 + (int)lonePairRendering;
        m_MoleculeManager.UpdateRendering(modifiedRenderAndLonePairValue);
    }

    /// <summary>
    /// <para> Returns to the home screen </para>
    /// </summary>
    public void GoToHomeScreen()// would ideally go to home screen and load it as instructions open
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// <para> Sends the Touch information to the Molecule Master. 
    /// This includes 1-3 finger touches, and the necessary information for the molecule master to determine what to do with them
    /// </para>
    /// </summary>
    private void SendTouchesToMoleculeManager()
    {
        if (Input.touchCount > 0) //if there is 1 or more fingers touching
        {
            if (Input.touchCount >= 3)
                m_MoleculeManager.AnimationScrubbingOccuring = true;
            else
                m_MoleculeManager.AnimationScrubbingOccuring = false;
            touch = Input.GetTouch(0);//get the 1st of finger touches,
                                      //this means that the position for multiple finger touches (mainly 3 touches)
                                      //depends on this first finger, not the others
            switch (touch.phase)//switches across if the touch is new or old or ended
            {
                case TouchPhase.Began: //if touch just started
                    switch (Input.touchCount)//switch across number of fingers doing the touch
                    {

                        case 1: //1 finger
                            if (timeSinceTap[0] > 0.1f && timeSinceTap[0] < 0.3f) //this detects a double tap,
                                                                                  //but avoids accidental triggering on unstable input
                            {
                                Debug.Log("reset position"); 
                                m_MoleculeManager.ResetMoleculePositions(); //if one finger double tap,
                                                                            //reset positions of molecules back to center
                            }
                            timeSinceTap[0] = 0; //if not a double tap, reset this counter the first finger
                            break;

                        case 2:
                            if (timeSinceTap[1] > 0.1f && timeSinceTap[1] < 0.3f)
                            {
                                Debug.Log("locked/unlocked");
                                m_MoleculeManager.SwitchLocking(); //if two finger double tap,
                                                                   //toggle the locking of the molecules (dissallows movement)
                            }
                            timeSinceTap[1] = 0; //if not a double tap, reset this counter for the second finger
                            break;

                        default:
                            break;
                    }
                    m_MoleculeManager.UpdateTool(0);
                    break;

                case TouchPhase.Moved: //if the finger is moving
                    
                    if (Input.touchCount != 2)//in any siutation except two fingers touching
                    {
                        if (Input.touchCount == 1) //for one finger touch
                        {
                            m_MoleculeManager.ChangeTransformsOfMolecules(touch.position, touch.deltaPosition, Input.touchCount);
                            //pass just the position of first finger, change in position of first finger and the number of fingers
                        }
                        else
                        {
                            m_MoleculeManager.ChangeTransformsOfMolecules(touch.position, touch.deltaPosition, Input.touchCount);
                            m_MoleculeManager.ControlAnimations(touch.position,touch.deltaPosition);
                        }
                    }
                    else {//if its a two finger touch
                        m_MoleculeManager.ChangeTransformsOfMoleculesTwoTouch(touch.position, Input.GetTouch(1).position);
                        //pass the first and second finger positions
                    }
                    m_MoleculeManager.UpdateTool(Input.touchCount); //regardless of number of touches, change active tool to the one
                                                                    //which corresponds to the number of fingers used,
                                                                    //eg. 1 is rotate, 2 is scale, 3 is move
                    break;

                case TouchPhase.Stationary:// if no movement occuring
                    m_MoleculeManager.UpdateTool(Input.touchCount); //update to correct tool, see above comment for same function
                    break;

                case TouchPhase.Ended:
                    break;
            }

            
        }
        else
        { // no fingers doing touching
            m_MoleculeManager.UpdateTool(0); //update tool as a no touch tool
        }
        if (Input.touchCount != 2)//if there are any number of touches except 2
        {
            m_MoleculeManager.ResetTwoTouchPos(); // reset thee two finger touch positions saved in molecule manager
        }
    }

   /* private void ToolTextUpdate() //only useful for debugging
    {
        toolText.text = m_MoleculeManager.toolState.ToString();
        switch (m_MoleculeManager.toolState)
        {
            case (MoleculeManager.Tools.Idle):
                //Debug.Log("Idle");
                break;

            case (MoleculeManager.Tools.Rotate):
                //Debug.Log("Rotate");
                break;

            case (MoleculeManager.Tools.Scale):
                //Debug.Log("Scale");
                break;

            case (MoleculeManager.Tools.Move):
                //Debug.Log("Move");
                break;

            default:
                Debug.Log("ERROR tool not in known state");
                break;

        }
    }*/

   /* private void VisualizeTheTouch() //only useful for debugging
    {
        List<VisualisedTouch> toRemove = new List<VisualisedTouch>();

        foreach (VisualisedTouch vT in dyingVisualiedTouches)
        {
            vT.UpdateMe();

            if (vT.dead)
            {
                vT.DestroyVisual();
                toRemove.Add(vT);
            }
        }
        foreach (VisualisedTouch vT in toRemove)
        {
            dyingVisualiedTouches.Remove(vT);
        }

        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    VisualisedTouch newVT = new VisualisedTouch(touch, touchImage, false);
                    newVT.visual.transform.SetParent(mainCanvas.transform);
                    aliveVisualisedTouches.Add(newVT);
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    dyingVisualiedTouches.Add(aliveVisualisedTouches[aliveVisualisedTouches.Count - 1]);
                    dyingVisualiedTouches[dyingVisualiedTouches.Count - 1].dying = true;
                    aliveVisualisedTouches.RemoveAt(aliveVisualisedTouches.Count - 1);
                }
            }
            if (Input.touchCount == aliveVisualisedTouches.Count)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (Input.GetTouch(i).phase != TouchPhase.Ended)
                    {
                        aliveVisualisedTouches[i].MoveMe(Input.GetTouch(i).position);
                    }
                }
            }
            else
            {
                Debug.Log("something is wrong " + Input.touchCount + " < touch count -- alive visual > " + aliveVisualisedTouches.Count);
            }
        }
    }


    public class VisualisedTouch
    {
        public bool dying = false;
        public bool dead = false;
        float countDown = 50f;

        public Image visual;

        public VisualisedTouch(Touch tT, Image toInst, bool areWeDying)

        {
            dying = areWeDying;
            visual = Instantiate(toInst, tT.position, Quaternion.identity);
        }

        public void MoveMe(Vector2 touchPos)
        {
            visual.transform.position = touchPos;
        }

        public void UpdateMe()
        {
            if (dying)
            {
                countDown -= 1;
            }

            if (countDown == 0)
            {
                dead = true;
                dying = false;
            }
            if (dying)
            {
                visual.color = new Color(1, 0, 0, countDown * 2 / 255f);
            }
        }

        public void StartDying()
        {
            countDown = 100f;
            dying = true;
        }

        public void DestroyVisual()
        {
            GameObject.Destroy(visual.gameObject);
        }
    }*/
}
