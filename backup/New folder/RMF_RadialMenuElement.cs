using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

[AddComponentMenu("Radial Menu Framework/RMF Element")]
public class RMF_RadialMenuElement : MonoBehaviour {

    [HideInInspector]
    public RectTransform rt;
    [HideInInspector]
    public RMF_RadialMenu parentRM;

	//public GameObject RadialMenu;
    [Tooltip("Each radial element needs a button. This is generally a child one level below this primary radial element game object.")]
    public Button button;
	[HideInInspector]
    [Tooltip("This is the text label that will appear in the center of the radial menu when this option is moused over. Best to keep it short.")]
    public string label;
	public string Screen_name;
	[HideInInspector]
	public string Product_name;
	[HideInInspector]
	public Sprite sprite;
    [HideInInspector]
    public float angleMin, angleMax;

    [HideInInspector]
    public float angleOffset;

    [HideInInspector]
    public bool active = false;

    [HideInInspector]
    public int assignedIndex = 0;
    // Use this for initialization

	//public Button m_YourFirstButton;
    private CanvasGroup cg;

    void Awake() {

        rt = gameObject.GetComponent<RectTransform>();

        if (gameObject.GetComponent<CanvasGroup>() == null)
            cg = gameObject.AddComponent<CanvasGroup>();
        else
            cg = gameObject.GetComponent<CanvasGroup>();


        if (rt == null)
            Debug.LogError("Radial Menu: Rect Transform for radial element " + gameObject.name + " could not be found. Please ensure this is an object parented to a canvas.");

        if (button == null)
            Debug.LogError("Radial Menu: No button attached to " + gameObject.name + "!");

    }

    void Start () {

		Button btn1 = button.GetComponent<Button>();
		//btn1.onClick.AddListener(TaskOnClick);
		//btn1.onClick.AddListener(delegate {TaskWithParameters("Hello"); });
		btn1.onClick.AddListener(delegate {SetProduct(Product_name); });
		btn1.onClick.AddListener(delegate {SetSeries(label); });
		btn1.onClick.AddListener(delegate {SetSprite(sprite); });
		btn1.onClick.AddListener(switchscreen);

		rt.rotation = Quaternion.Euler(0, 0, -angleOffset); //Apply rotation determined by the parent radial menu.

        //If we're using lazy selection, we don't want our normal mouse-over effects interfering, so we turn raycasts off.
        if (parentRM.useLazySelection)
            cg.blocksRaycasts = false;
        else {

            //Otherwise, we have to do some magic with events to get the label stuff working on mouse-over.

            EventTrigger t;

            if (button.GetComponent<EventTrigger>() == null) {
                t = button.gameObject.AddComponent<EventTrigger>();
                t.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
            } else
                t = button.GetComponent<EventTrigger>();


            EventTrigger.Entry enter = new EventTrigger.Entry();
            enter.eventID = EventTriggerType.PointerEnter;
            enter.callback.AddListener((eventData) => { setParentMenuLable(label); });


            EventTrigger.Entry exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((eventData) => { setParentMenuLable(""); });

            t.triggers.Add(enter);
            t.triggers.Add(exit);



        }

    }


	public void SetProduct(string Product_name){
		Debug.Log (Product_name);
		ApplicationModel.curr_product = Product_name;
	}


	public void SetSeries(string Series_name){
		//Debug.Log ("SetSeries");
		ApplicationModel.curr_series = Series_name;
	}

	public void SetSprite(Sprite Sprite_name){
		//Debug.Log ("SetSprite");
		ApplicationModel.curr_sprite = Sprite_name;
	}

	public void switchscreen(){
		SceneManager.LoadScene (Screen_name);
		//SceneManager.LoadSceneMode(Screen_name);
	}
	/*
	void TaskWithParameters(string message)
	{
		//Output this to console when the Button is clicked
		Debug.Log(message);
	}

	void TaskOnClick()
	{
		//Output this to console when the Button is clicked
		Debug.Log("You have clicked the button!");
	}*/

    //Used by the parent radial menu to set up all the approprate angles. Affects master Z rotation and the active angles for lazy selection.
    public void setAllAngles(float offset, float baseOffset) {

        angleOffset = offset;
        angleMin = offset - (baseOffset / 2f);
        angleMax = offset + (baseOffset / 2f);

    }

    //Highlights this button. Unity's default button wasn't really meant to be controlled through code so event handlers are necessary here.
    //I would highly recommend not messing with this stuff unless you know what you're doing, if one event handler is wrong then the whole thing can break.
    public void highlightThisElement(PointerEventData p) {

        ExecuteEvents.Execute(button.gameObject, p, ExecuteEvents.selectHandler);
        active = true;
        setParentMenuLable(label);

    }

    //Sets the label of the parent menu. Is set to public so you can call this elsewhere if you need to show a special label for something.
    public void setParentMenuLable(string l) {

        if (parentRM.textLabel != null)
            parentRM.textLabel.text = l;


    }


    //Unhighlights the button, and if lazy selection is off, will reset the menu's label.
    public void unHighlightThisElement(PointerEventData p) {

        ExecuteEvents.Execute(button.gameObject, p, ExecuteEvents.deselectHandler);
        active = false;

        if (!parentRM.useLazySelection)
            setParentMenuLable(" ");


    }

    //Just a quick little test you can run to ensure things are working properly.
    public void clickMeTest() {

        Debug.Log(assignedIndex);


    }




}
