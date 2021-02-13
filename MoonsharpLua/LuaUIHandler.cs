using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LuaUIHandler : MonoBehaviour {

    public List<GameObject> AllUIComponents;

    [SerializeField] GameObject WindowPrefab;
    [SerializeField] GameObject PanelPrefab;
    [SerializeField] GameObject TextPrefab;
    [SerializeField] GameObject ButtonPrefab;

    // Use this for initialization
    void Start()
    {
        Task.luaUIHandler = this;
        //var myWindow = CreateUIWindow(new Vector2(50, 50), new Vector2(400, 400), "I made a window in code!");// Color.grey, Color.white, Color.black);
        //CreateUIText(Vector2.zero, new Vector2(250,40), "This is added text!", myWindow, 11, Color.white);
        //CreateUIButton(Vector2.one*40, new Vector2(200, 100), "Start", myWindow);
        //CreateUIPanel(Vector2.zero, new Vector2(200, 100), null, myWindow);

    }
	
	void Update()
    {
		
	}

    public GameObject CreateUIWindow(Vector2 _position, Vector2 _size, string _title=" ", Color? _colorN = null, Color? _titleBarColorN = null, Color? _titleColorN = null)
    {
        
        _position.y = -_position.y;
        // note: still need to put scrollbars on windows in unity ui
        GameObject windowPiece = Instantiate(WindowPrefab, GetComponent<RectTransform>());

        Color _color = windowPiece.GetComponent<Image>().color;
        if (_colorN != null) {
            _color = (Color)_colorN;
        }

        Color _titleBarColor = windowPiece.transform.Find("TitleBar").GetComponent<Image>().color;
        if (_titleBarColorN != null)
        {
            _titleBarColor = (Color)_titleBarColorN;
        }

        Color _titleColor = windowPiece.transform.Find("TitleBar").Find("Title").GetComponent<Text>().color;
        if (_titleColorN != null)
        {
            _titleColor = (Color)_titleColorN;
        }
        
        RectTransform windowRect = windowPiece.GetComponent<RectTransform>();
        windowRect.localPosition = _position;
        windowRect.sizeDelta = _size;
        windowPiece.transform.Find("TitleBar").Find("Title").GetComponent<Text>().text = _title;
        windowPiece.transform.Find("TitleBar").Find("Title").GetComponent<Text>().color = _titleColor;
        windowPiece.transform.Find("TitleBar").GetComponent<Image>().color = _titleBarColor;
        windowPiece.GetComponent<Image>().color = _color;

        AllUIComponents.Add(windowPiece);
        return windowPiece;
    }

    // Creates a panel, a square, for parenting other things to
    public GameObject CreateUIPanel(Vector2 _position, Vector2 _size, Color? _colorN = null, GameObject _parent = null)
    {
        _position.y = -_position.y;
        // note: still need to put scrollbars on windows in unity ui
        GameObject panelPiece = Instantiate(PanelPrefab, GetComponent<RectTransform>());

        Color color = panelPiece.GetComponent<Image>().color;
        if (_colorN != null)
        {
            color = (Color)_colorN;
        }

        RectTransform panelRect = panelPiece.GetComponent<RectTransform>();
        if (_parent != null)
        {
            panelRect.SetParent(_parent.transform);
            panelRect.localPosition = _position;
        }
        else
        {
            panelRect.position = _position + new Vector2(0,Screen.height);
        }
        panelRect.sizeDelta = _size;
        panelPiece.GetComponent<Image>().color = color;
        return panelPiece;
        
    }

    public GameObject CreateUIText(Vector2 _position, Vector2 _size, string _text = " ", GameObject _parent = null, int _fontSize = 11, Color? _textColorN = null)
    {
        _position.y = -_position.y;
        // note: still need to put scrollbars on windows in unity ui
        GameObject textPiece = Instantiate(TextPrefab, GetComponent<RectTransform>());

        Text textComp = textPiece.GetComponent<Text>();
        Image imageComp = textPiece.GetComponent<Image>();
        RectTransform textRect = textPiece.GetComponent<RectTransform>();


        Color textColor = textComp.color;
        if (_textColorN != null)
        {
            textColor = (Color)_textColorN;
        }
        
        if (_parent != null)
        {
            textRect.SetParent(_parent.transform);
            textRect.localPosition = _position;
        }
        else
        {
            textRect.position = _position + new Vector2(0, Screen.height);
        }
        textRect.sizeDelta = _size;

        textComp.text = _text;
        textComp.color = textColor;
        textComp.fontSize = _fontSize;

        AllUIComponents.Add(textPiece);
        return textPiece;

    }

    public GameObject CreateUIButton(Vector2 _position, Vector2 _size, string _text = " ", GameObject _parent = null, int _fontSize = 11, Color? _buttonColorN = null, Color? _textColorN = null)
    {
        _position.y = -_position.y;
        // note: still need to put scrollbars on windows in unity ui
        GameObject buttonPiece = Instantiate(ButtonPrefab, GetComponent<RectTransform>());

        Text textComp = buttonPiece.transform.Find("Text").GetComponent<Text>();
        Image imageComp = buttonPiece.GetComponent<Image>();
        Button buttonComp = buttonPiece.GetComponent<Button>();
        RectTransform buttonRect = buttonPiece.GetComponent<RectTransform>();

        Color buttonColor = imageComp.color;
        if (_buttonColorN != null)
        {
            buttonColor = (Color)_buttonColorN;
        }


        Color textColor = textComp.color;
        if (_textColorN != null)
        {
            textColor = (Color)_textColorN;
        }


        if (_parent != null)
        {
            buttonRect.SetParent(_parent.transform);
            buttonRect.localPosition = _position;
        }
        else
        {
            buttonRect.position = _position + new Vector2(0, Screen.height);
        }

        buttonRect.sizeDelta = _size;

        textComp.text = _text;
        textComp.color = textColor;
        textComp.fontSize = _fontSize;
        imageComp.color = buttonColor;

        buttonComp.onClick.AddListener(
            delegate {
                OnLuaUIButtonClick(buttonPiece);
            } 
        );

        AllUIComponents.Add(buttonPiece);
        return buttonPiece;
    }

    /// <summary>
    /// hook called from LuaUIButtons
    /// </summary>
    void OnLuaUIButtonClick(GameObject button)
    {
        foreach (LuaHandler luaHandler in FindObjectsOfType<LuaHandler>())
        {
            MoonSharp.Interpreter.Script luaScript = luaHandler.luaScript;

            if (luaScript != null && luaScript.Globals["OnUIButtonClick"] != null)
            {
                luaScript.Call(luaScript.Globals["OnUIButtonClick"], Task.GetOrMakeLuaUIObject(button));
            }
        }
    }

    /*
        public void MoveInHierarchy(int delta) {
            int index = transform.GetSiblingIndex();
            transform.SetSiblingIndex (index + delta);
        }
     * */

    /*
     * 
     * // PLAYERS NEED SCREEN WIDTH AND HEIGHT
     * 
        all extend UIComponent - .pos(hooks to recttransform) .size(*) .parent .onclick .onmousedown .onmouseup .color
        notice anything can have .onclick
        .pos is always relative to .parent if there is one

        unity-done - class GuiWindow(pos, size, title="", color=white); - creates a window with title bar, x button, and scrollbars, that you can parent items to. has .titlebarcolor, .titlecolor , maybe .draggable/.fixed

        class GuiSquare(pos, size, color=red, UIComponent parent=null); - creates a square rectangle that you can parent items to.

        class GuiText(pos, size, parent=null, textcolor=black, textsize=7, font=asap); - has .value that get/sets the text, .textcolor, .textsize, .font

        class GuiTextbox(pos, size, parent=null, function_onsubmit=null, color=white, textcolor=black, textsize=7, font=asap); - has .onsubmit, .textsize, .textcolor, .font, .backgroundcolor

        class GuiToggle(pos, size, parent=null, function_ontoggle=null, color=white, backgroundcolor=white); has .value with bool

        class GuiSlider(pos, sizing_information_idk_how_sliders_work_exactly, min, max , parent=null, function_onchange=null); has .value with number
    */
}
