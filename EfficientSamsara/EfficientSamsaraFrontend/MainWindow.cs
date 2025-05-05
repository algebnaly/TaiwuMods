using Config;
using FrameWork;
using GameData.Common;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityGameUI;
using Navigation = UnityGameUI.Navigation;
using Object = UnityEngine.Object;

namespace EfficientSamsaraFrontend
{
    public class MainWindow : MonoBehaviour
    {
        // Trainer Base
        public GameObject obj = null;
        public MainWindow instance;
        public bool initialized = false;
        public bool _optionToggle = false;
        public static KeyCode Hot_Key = KeyCode.Insert;

        // UI
        public AssetBundle testAssetBundle = null;
        public GameObject canvas = null;

        // private bool isVisible = false;
        private GameObject uiPanel = null;
        private static readonly int width = Mathf.Min(Screen.width, 740);
        private static readonly int height = (Screen.height < 400) ? Screen.height : (450);
        private bool cursorWasVisible;
        // 界面开关
        public bool optionToggle
        {
            get { return _optionToggle; }
            set
            {
                // 设置鼠标显示
                if (value)
                {
                    //Cursor.lockState = CursorLockMode.None;
                    // This is cached for situations where cursor was visible before opening mod menu.
                    cursorWasVisible = Cursor.visible;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.visible = cursorWasVisible;
                }
                _optionToggle = value;
            }
        }

        // 按钮位置
        private static int initialX
        {
            get
            {
                return -width / 2 + 120;
            }
        }
        private static int initialY
        {
            get
            {
                return height / 2 - 60;
            }
        }
        private int elementX = initialX;
        private int elementY = initialY;

        internal GameObject Create(string name)
        {
            obj = new GameObject(name);
            DontDestroyOnLoad(obj);
            obj.AddComponent<MainWindow>();
            return obj;
        }

        public MainWindow()
        {
            instance = this;
        }

        public void Start()
        {
            Initialize();
        }

        public void Update()
        {
            if (!initialized)
            {
                Initialize();
            }

            if (Input.GetKeyDown(Hot_Key))
            {
                optionToggle = !optionToggle;
                Debug.Log("窗口开关状态：" + optionToggle.ToString());
                canvas.SetActive(optionToggle);
                Event.current.Use();
            }
        }
    
        public void OnDestroy()
        {
            if (canvas != null)
            {
                Object.Destroy(canvas);
                initialized = false;
            }
            //Debug.Log("MainWindow 销毁完毕");
        }

        public void Initialize()
        {
            initialized = true;
            instance.CreateUI();
        }

        private void CreateUI()
        {
            try
            {
                if (canvas == null)
                {
                    Debug.Log("创建 UI 元素");

                    canvas = UIControls.createUICanvas();
                    Object.DontDestroyOnLoad(canvas);

                    // 设置背景
                    GameObject background = UIControls.createUIPanel(canvas, (height + 40).ToString(), (width + 40).ToString(), null);
                    background.GetComponent<Image>().color = UIControls.HTMLString2Color("#2D2D30FF");


                    uiPanel = UIControls.createUIPanel(background, height.ToString(), width.ToString(), null);
                    uiPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");

                    background.AddComponent<WindowDragHandler>();

                    AddTitle("【太吾绘卷】高效轮回 By:algebnaly 1.0.1");

                    GameObject closeButton = UIControls.createUIButton(uiPanel, "#B71C1CFF", "X", () =>
                    {
                        optionToggle = false;
                        canvas.SetActive(optionToggle);
                    }, new Vector3(width / 2 + 10, height / 2 + 10, 0));
                    
                    GameObject addButton = UIControls.createUIButton(uiPanel, "#4CAF50FF", "添加前世", () =>
                    {
                        CallBackend.Call();
                        // 添加按钮逻辑
                    }, new Vector3(0, 0, 0));
                    
                    closeButton.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
                    // 字体颜色为白色
                    closeButton.GetComponentInChildren<Text>().color = UIControls.HTMLString2Color("#FFFFFFFF");
                    
                    
                    addButton.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
                    // 字体颜色为白色
                    addButton.GetComponentInChildren<Text>().color = UIControls.HTMLString2Color("#FFFFFFFF");
                    
                    Debug.Log("创建 创建标题 完成");
                    canvas.SetActive(optionToggle);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                initialized = false;
            }
        }



        // 添加标题
        public GameObject AddTitle(string Title)
        {
            GameObject TitleBackground = UIControls.createUIPanel(uiPanel, "30", (width - 20).ToString(), null);
            TitleBackground.GetComponent<Image>().color = UIControls.HTMLString2Color("#2D2D30FF");
            TitleBackground.GetComponent<RectTransform>().localPosition = new Vector3(0, height / 2 - 30, 0);

            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(TitleBackground, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 10, 30);
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Text text = uiText.GetComponent<Text>();
            text.text = Title;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 16;

            return uiText;
        }

        public GameObject AddButton(string Text, GameObject panel, UnityAction action)
        {
            string backgroundColor = "#8C9EFFFF";
            Vector3 localPosition = new Vector3(elementX, elementY, 0);
            elementX += 90;

            GameObject button = UIControls.createUIButton(panel, backgroundColor, Text, action, localPosition);

            // 按钮样式
            button.AddComponent<Shadow>().effectColor = UIControls.HTMLString2Color("#000000FF");   // 添加阴影
            button.GetComponent<Shadow>().effectDistance = new Vector2(2, -2);              // 设置阴影偏移
            button.GetComponentInChildren<Text>().fontSize = 14;     // 设置字体大小
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 30);    // 设置按钮大小


            return button;
        }

        // 添加复选框
        public GameObject AddToggle(string Text, int width, GameObject panel, UnityAction<bool> action)
        {
            // 计算x轴偏移
            elementX += width / 2 - 30;

            Sprite toggleBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#3E3E42FF"));
            Sprite toggleSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#18FFFFFF"));
            GameObject uiToggle = UIControls.createUIToggle(panel, toggleBgSprite, toggleSprite);
            uiToggle.GetComponentInChildren<Text>().color = Color.white;
            uiToggle.GetComponentInChildren<Toggle>().isOn = false;
            uiToggle.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);

            uiToggle.GetComponentInChildren<Text>().text = Text;
            uiToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener(action);


            elementX += width / 2 + 10;

            return uiToggle;
        }

        // 添加输入框
        public GameObject AddInputField(string Text, int width, string defaultText, GameObject panel, UnityAction<string> action)
        {
            // 计算x轴偏移
            elementX += width / 2 - 30;

            // label
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(panel, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = Text;
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);
            //uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 30);
            uiText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;


            // 坐标偏移
            elementX += 10;

            // 输入框
            Sprite inputFieldSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject uiInputField = UIControls.createUIInputField(panel, inputFieldSprite, "#FFFFFFFF");
            uiInputField.GetComponent<InputField>().text = defaultText;
            uiInputField.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);
            uiInputField.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 60, 30);

            // 文本框失去焦点时触发方法
            uiInputField.GetComponent<InputField>().onEndEdit.AddListener(action);

            elementX += width / 2 + 10;
            return uiInputField;
        }

        // 添加下拉框
        public GameObject AddDropdown(string Text, int width, List<string> options, GameObject panel, UnityAction<int> action)
        {
            // 计算x轴偏移
            elementX += width / 2 - 30;

            // label
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(panel, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = Text;
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);
            //uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 30);
            uiText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

            // 坐标偏移
            elementX += 60;

            // 创建下拉框
            Sprite dropdownBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));      // 背景颜色
            Sprite dropdownScrollbarSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));   // 滚动条颜色 (如果有的话
            Sprite dropdownDropDownSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));    // 框右侧小点的颜色
            Sprite dropdownCheckmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));   // 选中时的颜色
            Sprite dropdownMaskSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#E65100FF"));        // 不知道是哪的颜色
            Color LabelColor = UIControls.HTMLString2Color("#EFEBE9FF");
            GameObject uiDropDown = UIControls.createUIDropDown(panel, dropdownBgSprite, dropdownScrollbarSprite, dropdownDropDownSprite, dropdownCheckmarkSprite, dropdownMaskSprite, options, LabelColor);
            Object.DontDestroyOnLoad(uiDropDown);
            uiDropDown.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);

            // 下拉框选中时触发方法
            uiDropDown.GetComponent<Dropdown>().onValueChanged.AddListener(action);

            elementX += width / 2 + 60;
            return uiDropDown;
        }

        // 添加小标题
        public GameObject AddH3(string text, GameObject panel, Color color = default(Color))
        {
            elementX += 40;

            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(panel, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = text;
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);
            //uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 30);  // 设置宽度

            // 设置字体样式为h3小标题
            uiText.GetComponent<Text>().fontSize = 14;
            uiText.GetComponent<Text>().fontStyle = FontStyle.Bold;

            // 设置字体颜色
            if (color != default(Color)) uiText.GetComponent<Text>().color = color;

            hr();
            elementY += 20;
            elementX += 10;
            return uiText;
        }

        // 换行
        public void hr(int offsetX = 0, int offsetY = 0)
        {
            ResetCoordinates(true);
            elementX += offsetX;
            elementY -= 50 + offsetY;

        }
        // 重置坐标
        public void ResetCoordinates(bool x, bool y = false)
        {
            if (x) elementX = initialX;
            if (y) elementY = initialY;
        }
    }
}
