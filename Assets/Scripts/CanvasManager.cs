using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CanvasManager : MonoBehaviour
{
    List<VisualisedTouch> aliveVisualisedTouches = new List<VisualisedTouch>();
    List<VisualisedTouch> dyingVisualiedTouches = new List<VisualisedTouch>();


    public Image touchImage;

    private Canvas touchVisuals;

    public MoleculeManager m_MoleculeManager;

    private Text toolText;

    private Text lockText;


    // Create Canvas GameObject.
   

    private void Awake()
    {
        // Load the Arial font from the Unity Resources folder.
        //Font arial;

        //arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        //load Helvetica
        Font helvetica;

        helvetica = Resources.Load<Font>("Fonts/Helvetica");

        // Create Canvas GameObject.
        GameObject canvasGO = new GameObject();
        canvasGO.AddComponent<Canvas>();
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Set to touch visuals 
        touchVisuals = canvasGO.GetComponent<Canvas>();
        touchVisuals.renderMode = RenderMode.ScreenSpaceOverlay;

        // Create the Text GameObject.
        GameObject textGOTool = new GameObject();
        textGOTool.transform.parent = canvasGO.transform;
        textGOTool.AddComponent<Text>();

        GameObject textGOToggle = new GameObject();
        textGOToggle.transform.parent = canvasGO.transform;
        textGOToggle.AddComponent<Text>();

        // Set Text component properties.
        toolText = textGOTool.GetComponent<Text>();
        toolText.font = helvetica;
        toolText.text = "TEST";
        toolText.fontSize = 48;
        toolText.alignment = TextAnchor.MiddleCenter;

        lockText = textGOToggle.GetComponent<Text>();
        lockText.font = helvetica;
        lockText.text = "Lock";
        lockText.fontSize = 16;
        lockText.alignment = TextAnchor.UpperRight;


        // Provide Text position and size using RectTransform.
        RectTransform rectTransform;
        rectTransform = toolText.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, 0, 0);
        rectTransform.sizeDelta = new Vector2(600, 200);

        RectTransform temp;
        temp = lockText.GetComponent<RectTransform>();
        temp.localPosition = new Vector3(0, 0, 0);
        temp.sizeDelta = new Vector2(600, 200);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        VisualizeTheTouch(); //seems to look ok
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

            case (MoleculeManager.Tools.AnimationControl):
                //Debug.Log("AnimationControl");
                break;

            default:
                Debug.Log("ERROR tool not in known state");
                break;
            
        }
    }

    //public void SetupText()
    //{

    //}

    public void VisualizeTheTouch() {
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
                //Debug.Log(touch.phase);
                if (touch.phase == TouchPhase.Began)
                {
                    VisualisedTouch newVT = new VisualisedTouch(touch, touchImage, false);
                    //                newVT.visual.transform.parent = touchVisuals.transform;
                    newVT.visual.transform.SetParent(touchVisuals.transform);
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
                // Debug.Log("we are good");
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
    }


}
