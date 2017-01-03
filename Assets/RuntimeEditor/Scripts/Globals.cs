using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.RuntimeEditor
{
    public class Globals : MonoBehaviour
    {
        public string[] variableTypeIgnoreList;

        public static void WriteToConsole(int messageIndex)
        {
            switch (messageIndex)
            {
                case 0:
                    Debug.LogWarning("Not enough labels or values passed for variable type. Aborting.");
                    break;
                case 1:
                    Debug.LogWarning("Invalid value passed for boolean variable type. Must be 0 or 1. Aborting.");
                    break;
            }
        }

        public bool CheckTypeIgnoreList(string input)
        {
            for (int i = 0; i < variableTypeIgnoreList.Length; i++)
            {
                if (input == variableTypeIgnoreList[i])
                    return true;
            }
            return false;
        }
    }

    [System.Serializable]
    public static class Extensions
    {
        public static string TruncateString(string input, int length)
        {
            if (input.Length <= length)
                return input;
            return input.Substring(0, length);
        }

        public static string FormatStringConvention(string input)
        {
            char[] charArray = input.ToCharArray();
            if (char.IsLower(charArray[0]))
                charArray[0] = char.ToUpper(charArray[0]);
            string inputCap = new string(charArray);
            System.Text.StringBuilder newText = new System.Text.StringBuilder(inputCap.Length * 2);
            newText.Append(inputCap[0]);
            for (int i = 1; i < inputCap.Length; i++)
            {
                if (i < inputCap.Length - 1 && i != 0)
                {
                    if (char.IsDigit(inputCap[i]) && inputCap[i - 1] != ' ')
                    {
                        newText.Append(' ');
                    }
                    else if (char.IsUpper(inputCap[i]) && inputCap[i - 1] != ' ')
                    {
                        if (!char.IsUpper(inputCap[i + 1]) || !char.IsUpper(inputCap[i - 1]))
                            newText.Append(' ');
                    }
                }
                newText.Append(inputCap[i]);
            }
            return newText.ToString();

            /*char[] charArray = input.ToCharArray();
            if (char.IsLower(charArray[0]))
                charArray[0] = char.ToUpper(charArray[0]);
            string inputCap = new string(charArray);
            System.Text.StringBuilder newText = new System.Text.StringBuilder(inputCap.Length * 2);
            newText.Append(inputCap[0]);
            for (int i = 1; i < inputCap.Length; i++)
            {
                if (char.IsUpper(inputCap[i]) && inputCap[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(inputCap[i]);
            }
            return newText.ToString();*/
        }

        public static bool IsCustomValueType(this System.Type type)
        {
            return type.IsValueType && !type.IsPrimitive && type.Namespace != null && !type.Namespace.StartsWith("System.") && !type.IsEnum;
        }
    }

    [System.Serializable]
    public class WindowReferences
    {
        public string name;
        public GameObject content;
        public GameObject tab;
        public Image activeTab;
    }

    [System.Serializable]
    public class InspectorReferences
    {
        public List<RuntimeEditor.Component> components = new List<RuntimeEditor.Component>();
        public RectTransform scrollRect;
        public RectTransform contentWindow;
        public RectTransform footer;
    }

    [System.Serializable]
    public class ComponentReferences
    {
        public Text componentLabel;
        public Toggle enabledToggle;
        public Image expandIcon;
        public Image assetIcon;
        public RectTransform titleBarRect;
        public RectTransform contentRect;
        public List<Variable> variables = new List<Variable>(); //REMOVE
    }

    [System.Serializable]
    public class ComponentGraphics
    {
        public Sprite expand;
        public Sprite toggle;
    }

    [System.Serializable]
    public enum VariableGeneratorSourceType
    {
        Component,
        Array
    }

    [System.Serializable]
    public class VariableGeneratorReferences
    {
       public List<Variable> variables = new List<Variable>();
    }

    [System.Serializable]
    public enum VariableType
    {
        InputField,
        Slider,
        RangeSlider,
        Vector2,
        Vector2Tall,
        Vector3,
        Vector4,
        Boolean,
        Color3,
        Color4,
        Enum,
        Asset
    }

    [System.Serializable]
    public class VariableReferences
    {
        public Text variableName;
        public InputField inputField;
        public Slider slider;
        public InputField sliderInput;
        public Slider rangeSliderMin;
        public Slider rangeSliderMax;
        public Image range;
        public InputField vector2X;
        public Text vector2XText;
        public InputField vector2Y;
        public Text vector2YText;
        public InputField vector2TallX;
        public Text vector2TallXText;
        public InputField vector2TallY;
        public Text vector2TallYText;
        public InputField vector3X;
        public Text vector3XText;
        public InputField vector3Y;
        public Text vector3YText;
        public InputField vector3Z;
        public Text vector3ZText;
        public InputField vector4X;
        public Text vector4XText;
        public InputField vector4Y;
        public Text vector4YText;
        public InputField vector4Z;
        public Text vector4ZText;
        public InputField vector4W;
        public Text vector4WText;
        public Toggle boolean;
        public Image color3Overlay;
        public Image color3Value;
        public Image color3Eyedropper;
        public Image color4Overlay;
        public Image color4Value;
        public Slider color4Alpha;
        public Image color4Eyedropper;
        public Dropdown enumDropdown;
        public Image assetField;
        public Text assetLabel;
        public Image assetIcon;
        public RawImage assetImage;
    }

    [System.Serializable]
    public class VariableGraphics
    {
        public Color textColor;
        public Sprite inputField;
        public Sprite sliderHandle;
        public Sprite boolean;
        public Sprite colorOverlay;
        public Sprite eyedropper;
        public Sprite dropdown;
    }

    [System.Serializable]
    public class ArrayReferences
    {
        public Text arrayName;
        public Image expandIcon;
        public RectTransform arrayContent;
        public RectTransform sizeVariable;
        public InputField sizeInputField;
    }

    [System.Serializable]
    public class ArrayGraphics
    {
        public Sprite expand;
        public Sprite inputField;
    }

    [System.Serializable]
    public enum AssetType
    {
        None,
        Texture,
        Mesh,
        Transform,
        Material,
        PhysicMaterial,
        GameObject
    }

    [System.Serializable]
    public class AssetGraphics
    {
        public Sprite mesh;
        public Sprite transform;
    }
}
