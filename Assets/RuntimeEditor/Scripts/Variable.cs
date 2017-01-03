using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.RuntimeEditor
{
    public class Variable : MonoBehaviour {

        //Manual
        public GameObject[] variableTypeContent;
        public VariableGraphics variableGraphicsDefault;
        public VariableGraphics variableGraphicsSelected;
        public AssetGraphics assetGraphics;
        public VariableReferences variableReferences;

        //Automatic
        VariableType variableType;
        AssetType assetType;
        bool isInteger = false;
        bool isEditable = true;
        UnityEngine.Component unityComponent;
        RuntimeEditor.VariableGenerator variableGenerator;
        RectTransform rangeRect;
        bool variableReady = false;
        int varType;
        float labelIndent;

        void Start()
        {
            rangeRect = variableReferences.range.GetComponent<RectTransform>();
        }

        void Update()
        {
            if (variableReady)
            {
                if (variableType == VariableType.RangeSlider)
                {
                    //Set range graphic size and position
                    if (variableReferences.rangeSliderMin.value > variableReferences.rangeSliderMax.value)
                        variableReferences.rangeSliderMin.value = variableReferences.rangeSliderMax.value;
                    if (variableReferences.rangeSliderMax.value < variableReferences.rangeSliderMin.value)
                        variableReferences.rangeSliderMax.value = variableReferences.rangeSliderMin.value;
                    rangeRect.transform.position = variableReferences.rangeSliderMin.handleRect.transform.position;
                    rangeRect.sizeDelta = new Vector2(Vector3.Distance(variableReferences.rangeSliderMin.handleRect.transform.position, variableReferences.rangeSliderMax.handleRect.transform.position), rangeRect.sizeDelta.y);
                }
            }
        }

        public void VariableSetup(RuntimeEditor.VariableGenerator generator, UnityEngine.Component source, int inputType, VariableType vType, AssetType aType, bool integer, bool isEditable, string[] labels, object[] values, float indent)
        {
            unityComponent = source;
            variableGenerator = generator;
            variableType = vType;
            assetType = aType;
            isInteger = integer;
            varType = inputType;
            labelIndent = indent;
            SetVariableRect();
            SetVariableReferences(labels, values);
            SetEditable(isEditable);
        }

        void SetVariableReferences(string[] labels, object[] values)
        {
            variableTypeContent[(int)variableType].SetActive(true);
            if (labels.Length >= 1)
            {
                this.name = labels[0];
                variableReferences.variableName.text = Extensions.FormatStringConvention(labels[0]);
                variableReferences.variableName.GetComponent<RectTransform>().anchoredPosition = new Vector2(106 + labelIndent, 0);
            }
            else
                Globals.WriteToConsole(0);
            switch (variableType)
            {
                case VariableType.InputField:
                    if (isInteger)
                        variableReferences.inputField.contentType = InputField.ContentType.IntegerNumber;
                    if (values.Length >= 1)
                        variableReferences.inputField.text = values[0].ToString();
                    else
                        Globals.WriteToConsole(0);
                    break;
                //EDITOR STYLES
                /*case VariableType.Slider:
                    if (isInteger)
                    {
                        componentReferences.slider.wholeNumbers = true;
                        componentReferences.sliderInput.contentType = InputField.ContentType.IntegerNumber;
                    }
                    if (values.Length >= 3)
                    {
                        componentReferences.slider.minValue = (float)values[0];
                        componentReferences.slider.maxValue = (float)values[1];
                        componentReferences.slider.value = (float)values[2];
                        componentReferences.sliderInput.text = values[2].ToString();
                    }
                    else
                        Globals.WriteToConsole(0);
                    break;*/
                //EDITOR STYLES
                /*case VariableType.RangeSlider:
                    if (isInteger)
                    {
                        componentReferences.rangeSliderMin.wholeNumbers = true;
                        componentReferences.rangeSliderMax.wholeNumbers = true;
                    }
                    if (values.Length >= 4)
                    {
                        componentReferences.rangeSliderMin.minValue = (float)values[0];
                        componentReferences.rangeSliderMax.minValue = (float)values[0];
                        componentReferences.rangeSliderMin.maxValue = (float)values[1];
                        componentReferences.rangeSliderMax.maxValue = (float)values[1];
                        componentReferences.rangeSliderMin.value = (float)values[2];
                        componentReferences.rangeSliderMax.value = (float)values[3];
                    }
                    else
                        Globals.WriteToConsole(0);
                    break;*/
                case VariableType.Vector2:
                    if (isInteger)
                    {
                        variableReferences.vector2X.contentType = InputField.ContentType.IntegerNumber;
                        variableReferences.vector2Y.contentType = InputField.ContentType.IntegerNumber;
                    }
                    if (values.Length >= 1 && labels.Length >= 3)
                    {
                        variableReferences.vector2XText.text = Extensions.TruncateString(labels[1], 1);
                        variableReferences.vector2YText.text = Extensions.TruncateString(labels[2], 1);
                        Vector2 vector = (Vector2)values[0];
                        variableReferences.vector2X.text = vector.x.ToString();
                        variableReferences.vector2Y.text = vector.y.ToString();
                    }
                    else
                        Globals.WriteToConsole(0);
                    break;
                //EDITOR STYLES
                /*case VariableType.Vector2Tall:
                    if (isInteger)
                    {
                        componentReferences.vector2TallX.contentType = InputField.ContentType.IntegerNumber;
                        componentReferences.vector2TallY.contentType = InputField.ContentType.IntegerNumber;
                    }
                    if (values.Length >= 1 && labels.Length >= 3)
                    {
                        componentReferences.vector2TallXText.text = Extensions.TruncateString(labels[1], 1);
                        componentReferences.vector2TallYText.text = Extensions.TruncateString(labels[2], 1);
                        Vector2 vector = (Vector2)values[0];
                        componentReferences.vector2TallX.text = vector.x.ToString();
                        componentReferences.vector2TallY.text = vector.y.ToString();
                    }
                    else
                        Globals.WriteToConsole(0);
                    break;*/
                case VariableType.Vector3:
                    if (isInteger)
                    {
                        variableReferences.vector3X.contentType = InputField.ContentType.IntegerNumber;
                        variableReferences.vector3Y.contentType = InputField.ContentType.IntegerNumber;
                        variableReferences.vector3Z.contentType = InputField.ContentType.IntegerNumber;
                    }
                    if (values.Length >= 1 && labels.Length >= 4)
                    {
                        variableReferences.vector3XText.text = Extensions.TruncateString(labels[1], 1);
                        variableReferences.vector3YText.text = Extensions.TruncateString(labels[2], 1);
                        variableReferences.vector3ZText.text = Extensions.TruncateString(labels[3], 1);
                        Vector3 vector = (Vector3)values[0];
                        variableReferences.vector3X.text = vector.x.ToString();
                        variableReferences.vector3Y.text = vector.y.ToString();
                        variableReferences.vector3Z.text = vector.z.ToString();
                    }
                    else
                        Globals.WriteToConsole(0);
                    break;
                case VariableType.Vector4:
                    if (isInteger)
                    {
                        variableReferences.vector4X.contentType = InputField.ContentType.IntegerNumber;
                        variableReferences.vector4Y.contentType = InputField.ContentType.IntegerNumber;
                        variableReferences.vector4Z.contentType = InputField.ContentType.IntegerNumber;
                        variableReferences.vector4W.contentType = InputField.ContentType.IntegerNumber;
                    }
                    if (values.Length >= 1 && labels.Length >= 5)
                    {
                        variableReferences.vector4XText.text = Extensions.TruncateString(labels[1], 1);
                        variableReferences.vector4YText.text = Extensions.TruncateString(labels[2], 1);
                        variableReferences.vector4ZText.text = Extensions.TruncateString(labels[3], 1);
                        variableReferences.vector4WText.text = Extensions.TruncateString(labels[4], 1);
                        Vector4 vector = (Vector4)values[0];
                        variableReferences.vector4X.text = vector.x.ToString();
                        variableReferences.vector4Y.text = vector.y.ToString();
                        variableReferences.vector4Z.text = vector.z.ToString();
                        variableReferences.vector4W.text = vector.w.ToString();
                    }
                    else
                        Globals.WriteToConsole(0);
                    break;
                case VariableType.Boolean:
                    if (values.Length >= 1)
                        variableReferences.boolean.isOn = (bool)values[0];
                    else
                        Globals.WriteToConsole(0);
                    break;
                //EDITOR STYLES
                /*case VariableType.Color3:
                    if (values.Length >= 1)
                        componentReferences.color3Value.color = (Color)values[0];
                    else
                        Globals.WriteToConsole(0);
                    break;*/
                case VariableType.Color4:
                    if (values.Length >= 1)
                    {
                        if (values[0].GetType().ToString() == "UnityEngine.Color")
                        {
                            Color col = (Color)values[0];
                            variableReferences.color4Alpha.value = col.a;
                            col.a = 1;
                            variableReferences.color4Value.color = col;
                        }
                        else if (values[0].GetType().ToString() == "UnityEngine.Color32")
                        {
                            Color32 col = (Color32)values[0];
                            variableReferences.color4Alpha.value = col.a;
                            col.a = 255;
                            variableReferences.color4Value.color = col;
                        }
                    }
                    else
                        Globals.WriteToConsole(0);
                    break;
                case VariableType.Enum:
                    if (values.Length >= 2)
                    {
                        List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
                        for(int i = 1; i < values.Length; i++)
                        {
                            Dropdown.OptionData option = new Dropdown.OptionData();
                            option.text = values[i].ToString();
                            optionList.Add(option);
                        }
                        variableReferences.enumDropdown.AddOptions(optionList);
                        variableReferences.enumDropdown.value = (int)values[0];
                    }
                    else
                        Globals.WriteToConsole(0);
                    break;
                case VariableType.Asset:
                    SetAsset(values);
                    break;
            }
            variableGenerator.CompleteVariableSetup(GetComponent<RectTransform>().sizeDelta.y);
            variableReady = true;
        }

        void SetVariableRect()
        {
            RectTransform rect = GetComponent<RectTransform>();
            switch (variableType)
            {
                case VariableType.Vector4:
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, 34);
                    break;
                case VariableType.Vector2Tall:
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, 34);
                    break;
            }
        }

        void SetEditable(bool editable)
        {
            isEditable = editable;
            CanvasGroup cGroup = GetComponent<CanvasGroup>();
            if (isEditable)
                cGroup.alpha = 1;
            else
                cGroup.alpha = 0.33f;
        }

        void SendValueToComponent(object value)
        {
            System.Type myType = unityComponent.GetType();
            if (value != null)
            {
                if (varType == 0)
                {
                    System.Reflection.PropertyInfo[] properties = myType.GetProperties();
                    foreach (System.Reflection.PropertyInfo property in properties)
                    {
                        if (property.Name == this.name)
                        {
                            property.SetValue(unityComponent, value, new object[0]);
                        }
                    }
                }
                else if (varType == 1)
                {
                    System.Reflection.FieldInfo[] fields = myType.GetFields();
                    foreach (System.Reflection.FieldInfo field in fields)
                    {
                        if (field.Name == this.name)
                        {
                            field.SetValue(unityComponent, value);
                        }
                    }
                }
            }
        }

        object GetValueFromComponent()
        {
            System.Type myType = unityComponent.GetType();
            object output = null;
            if (varType == 0)
            {
                System.Reflection.PropertyInfo[] properties = myType.GetProperties();
                foreach (System.Reflection.PropertyInfo property in properties)
                {
                    if (property.Name == this.name)
                    {
                        output = property.GetValue(unityComponent, new object[0]);
                    }
                }
            }
            else if (varType == 1)
            {
                System.Reflection.FieldInfo[] fields = myType.GetFields();
                foreach (System.Reflection.FieldInfo field in fields)
                {
                    if (field.Name == this.name)
                    {
                        output = field.GetValue(unityComponent);
                    }
                }
            }
            return output;
        }

        public void OnSet(int fieldIndex)
        {
            if (variableReady)
            {
                switch (variableType)
                {
                    case VariableType.InputField:
                        if (variableReferences.inputField.text.Length > 0)
                        {
                            if (variableReferences.inputField.contentType == InputField.ContentType.DecimalNumber)
                                SendValueToComponent(float.Parse(variableReferences.inputField.text));
                            else
                                SendValueToComponent(int.Parse(variableReferences.inputField.text));
                        }
                        break;
                    //EDITOR STYLES
                    /*case VariableType.Slider:
                        if (fieldIndex == 0) //Slider changed
                        {
                            if (!componentReferences.slider.wholeNumbers) //float
                                componentReferences.sliderInput.text = componentReferences.slider.value.ToString();
                            else //integer
                                componentReferences.sliderInput.text = componentReferences.slider.value.ToString();
                        }
                        else if (fieldIndex == 1) //Input field changed
                        {
                            if (componentReferences.sliderInput.text.Length > 0)
                                if (componentReferences.sliderInput.contentType == InputField.ContentType.DecimalNumber) //float
                                    componentReferences.slider.value = float.Parse(componentReferences.sliderInput.text);
                                else //integer
                                    componentReferences.slider.value = int.Parse(componentReferences.sliderInput.text);
                        }
                        break;*/
                    //EDITOR STYLES
                    /*case VariableType.RangeSlider:
                        break;*/
                    case VariableType.Vector2:
                        if (variableReferences.vector2X.text.Length > 0 && variableReferences.vector2Y.text.Length > 0)
                        {
                            Vector2 vector2 = new Vector2(float.Parse(variableReferences.vector2X.text), float.Parse(variableReferences.vector2Y.text));
                            SendValueToComponent(vector2);
                        }
                        break;
                    //EDITOR STYLES
                    /*case VariableType.Vector2Tall:
                        if (componentReferences.vector2TallX.text.Length > 0 && componentReferences.vector2TallY.text.Length > 0)
                        {
                            Vector2 vector2 = new Vector2(float.Parse(componentReferences.vector2TallX.text), float.Parse(componentReferences.vector2TallY.text));
                            SendValueToComponent(vector2);
                        }
                        break;*/
                    case VariableType.Vector3:
                        if (variableReferences.vector3X.text.Length > 0 && variableReferences.vector3Y.text.Length > 0 && variableReferences.vector3Z.text.Length > 0)
                        {
                            Vector3 vector3 = new Vector3(float.Parse(variableReferences.vector3X.text), float.Parse(variableReferences.vector3Y.text), float.Parse(variableReferences.vector3Z.text));
                            SendValueToComponent(vector3);
                        }
                        break;
                    case VariableType.Vector4:
                        if (variableReferences.vector4X.text.Length > 0 && variableReferences.vector4Y.text.Length > 0 && variableReferences.vector4Z.text.Length > 0 && variableReferences.vector4W.text.Length > 0)
                        {
                            Vector4 vector4 = new Vector4(float.Parse(variableReferences.vector4X.text), float.Parse(variableReferences.vector4Y.text), float.Parse(variableReferences.vector4Z.text), float.Parse(variableReferences.vector4W.text));
                            SendValueToComponent(vector4);
                        }
                        break;
                    case VariableType.Boolean:
                        SendValueToComponent(variableReferences.boolean.isOn);
                        break;
                    case VariableType.Color3:
                        //Nothing yet
                        break;
                    case VariableType.Color4:
                        //Nothing yet
                        break;
                    case VariableType.Enum:
                        SendValueToComponent(variableReferences.enumDropdown.value);
                        break;
                    case VariableType.Asset:
                        //Nothing yet
                        break;
                }
            }
        }

        public void OnEndEdit(int fieldIndex)
        {
            if (variableReady)
            {
                switch (variableType)
                {
                    case VariableType.InputField:
                        if (variableReferences.inputField.text.Length > 0)
                        {
                            if (variableReferences.inputField.contentType == InputField.ContentType.DecimalNumber)
                                SendValueToComponent(float.Parse(variableReferences.inputField.text));
                            else if (variableReferences.inputField.contentType == InputField.ContentType.IntegerNumber)
                                SendValueToComponent(int.Parse(variableReferences.inputField.text));
                        }
                        else
                            variableReferences.inputField.text = GetValueFromComponent().ToString();
                        break;
                    //EDITOR STYLES
                    /*case VariableType.Slider:
                        if (componentReferences.sliderInput.text.Length > 0)
                        {
                            if (componentReferences.sliderInput.contentType == InputField.ContentType.DecimalNumber)
                            {
                                float f = float.Parse(componentReferences.sliderInput.text);
                                if (f > componentReferences.slider.maxValue)
                                    f = componentReferences.slider.maxValue;
                                else if (f < componentReferences.slider.minValue)
                                    f = componentReferences.slider.minValue;
                                componentReferences.sliderInput.text = f.ToString();
                                SendValueToComponent(f);
                            }
                            else if (componentReferences.sliderInput.contentType == InputField.ContentType.IntegerNumber)
                            {
                                int i = int.Parse(componentReferences.sliderInput.text);
                                if (i > componentReferences.slider.maxValue)
                                    i = (int)componentReferences.slider.maxValue;
                                else if (i < componentReferences.slider.minValue)
                                    i = (int)componentReferences.slider.minValue;
                                componentReferences.sliderInput.text = i.ToString();
                                SendValueToComponent(i);
                            }
                        }
                        else
                            componentReferences.sliderInput.text = GetValueFromComponent().ToString();
                        break;*/
                    case VariableType.Vector2:
                        if (variableReferences.vector2X.text.Length > 0 && variableReferences.vector2Y.text.Length > 0)
                        {
                            Vector2 vector2 = new Vector2(float.Parse(variableReferences.vector2X.text), float.Parse(variableReferences.vector2Y.text));
                            SendValueToComponent(vector2);
                        }
                        else
                        {
                            Vector2 vector2 = (Vector2)GetValueFromComponent();
                            variableReferences.vector2X.text = vector2.x.ToString();
                            variableReferences.vector2Y.text = vector2.y.ToString();
                        }
                        break;
                    //EDITOR STYLES
                    /*case VariableType.Vector2Tall:
                        if (componentReferences.vector2TallX.text.Length > 0 && componentReferences.vector2TallY.text.Length > 0)
                        {
                            Vector2 vector2 = new Vector2(float.Parse(componentReferences.vector2TallX.text), float.Parse(componentReferences.vector2TallY.text));
                            SendValueToComponent(vector2);
                        }
                        else
                        {
                            Vector2 vector2 = (Vector2)GetValueFromComponent();
                            componentReferences.vector2TallX.text = vector2.x.ToString();
                            componentReferences.vector2TallY.text = vector2.y.ToString();
                        }
                        break;*/
                    case VariableType.Vector3:
                        if (variableReferences.vector3X.text.Length > 0 && variableReferences.vector3Y.text.Length > 0 && variableReferences.vector3Z.text.Length > 0)
                        {
                            Vector3 vector3 = new Vector3(float.Parse(variableReferences.vector3X.text), float.Parse(variableReferences.vector3Y.text), float.Parse(variableReferences.vector3Z.text));
                            SendValueToComponent(vector3);
                        }
                        else
                        {
                            Vector3 vector3 = (Vector3)GetValueFromComponent();
                            variableReferences.vector3X.text = vector3.x.ToString();
                            variableReferences.vector3Y.text = vector3.y.ToString();
                            variableReferences.vector3Z.text = vector3.z.ToString();
                        }
                        break;
                    case VariableType.Vector4:
                        if (variableReferences.vector4X.text.Length > 0 && variableReferences.vector4Y.text.Length > 0 && variableReferences.vector4Z.text.Length > 0 && variableReferences.vector4W.text.Length > 0)
                        {
                            Vector4 vector4 = new Vector4(float.Parse(variableReferences.vector4X.text), float.Parse(variableReferences.vector4Y.text), float.Parse(variableReferences.vector4Z.text), float.Parse(variableReferences.vector4W.text));
                            SendValueToComponent(vector4);
                        }
                        else
                        {
                            Vector4 vector4 = (Vector4)GetValueFromComponent();
                            variableReferences.vector4X.text = vector4.x.ToString();
                            variableReferences.vector4Y.text = vector4.y.ToString();
                            variableReferences.vector4Z.text = vector4.z.ToString();
                            variableReferences.vector4W.text = vector4.w.ToString();
                        }
                        break;
                    case VariableType.Color3:
                        //Nothing yet
                        break;
                    case VariableType.Color4:
                        //Nothing yet
                        break;
                    case VariableType.Asset:
                        //Nothing yet
                        break;
                }
            }
        }

        public void OnSelect(int fieldIndex)
        {
            variableReferences.variableName.color = variableGraphicsSelected.textColor;
            switch (variableType)
            {
                case VariableType.InputField:
                    variableReferences.inputField.image.sprite = variableGraphicsSelected.inputField;
                    break;
                case VariableType.Slider:
                    variableReferences.slider.handleRect.GetComponent<Image>().sprite = variableGraphicsSelected.sliderHandle;
                    variableReferences.sliderInput.image.sprite = variableGraphicsSelected.inputField;
                    break;
                case VariableType.Vector2:
                    if (fieldIndex == 0)
                    {
                        variableReferences.vector2X.image.sprite = variableGraphicsSelected.inputField;
                        variableReferences.vector2XText.color = variableGraphicsSelected.textColor;
                    }
                    else if (fieldIndex == 1)
                    {
                        variableReferences.vector2Y.image.sprite = variableGraphicsSelected.inputField;
                        variableReferences.vector2YText.color = variableGraphicsSelected.textColor;
                    }
                    break;
                case VariableType.Vector2Tall:
                    if (fieldIndex == 0)
                    {
                        variableReferences.vector2TallX.image.sprite = variableGraphicsSelected.inputField;
                        variableReferences.vector2TallXText.color = variableGraphicsSelected.textColor;
                    }
                    else if (fieldIndex == 1)
                    {
                        variableReferences.vector2TallY.image.sprite = variableGraphicsSelected.inputField;
                        variableReferences.vector2TallYText.color = variableGraphicsSelected.textColor;
                    }
                    break;
                case VariableType.Vector3:
                    if (fieldIndex == 0)
                    {
                        variableReferences.vector3X.image.sprite = variableGraphicsSelected.inputField;
                        variableReferences.vector3XText.color = variableGraphicsSelected.textColor;
                    }
                    else if (fieldIndex == 1)
                    {
                        variableReferences.vector3Y.image.sprite = variableGraphicsSelected.inputField;
                        variableReferences.vector3YText.color = variableGraphicsSelected.textColor;
                    }
                    else if (fieldIndex == 2)
                    {
                        variableReferences.vector3Z.image.sprite = variableGraphicsSelected.inputField;
                        variableReferences.vector3ZText.color = variableGraphicsSelected.textColor;
                    }
                    break;
                case VariableType.Vector4:
                    if (fieldIndex == 0)
                    {
                        variableReferences.vector4X.image.sprite = variableGraphicsSelected.inputField;
                        variableReferences.vector4XText.color = variableGraphicsSelected.textColor;
                    }
                    else if (fieldIndex == 1)
                    {
                        variableReferences.vector4Y.image.sprite = variableGraphicsSelected.inputField;
                        variableReferences.vector4YText.color = variableGraphicsSelected.textColor;
                    }
                    else if (fieldIndex == 2)
                    {
                        variableReferences.vector4Z.image.sprite = variableGraphicsSelected.inputField;
                        variableReferences.vector4ZText.color = variableGraphicsSelected.textColor;
                    }
                    else if (fieldIndex == 3)
                    {
                        variableReferences.vector4W.image.sprite = variableGraphicsSelected.inputField;
                        variableReferences.vector4WText.color = variableGraphicsSelected.textColor;
                    }
                    break;
                case VariableType.Boolean:
                    variableReferences.boolean.image.sprite = variableGraphicsSelected.boolean;
                    break;
                case VariableType.Color3:
                    if (fieldIndex == 0)
                        variableReferences.color3Overlay.sprite = variableGraphicsSelected.colorOverlay;
                    else if (fieldIndex == 1)
                        variableReferences.color3Eyedropper.sprite = variableGraphicsSelected.eyedropper;
                    break;
                case VariableType.Color4:
                    if (fieldIndex == 0)
                        variableReferences.color4Overlay.sprite = variableGraphicsSelected.colorOverlay;
                    else if (fieldIndex == 1)
                        variableReferences.color4Eyedropper.sprite = variableGraphicsSelected.eyedropper;
                    break;
                case VariableType.Enum:
                    variableReferences.enumDropdown.image.sprite = variableGraphicsSelected.dropdown;
                    break;
                case VariableType.Asset:
                    variableReferences.assetField.sprite = variableGraphicsSelected.inputField;
                    break;
            }
        }

        public void OnDeselect(int fieldIndex)
        {
            variableReferences.variableName.color = variableGraphicsDefault.textColor;
            switch (variableType)
            {
                case VariableType.InputField:
                    variableReferences.inputField.image.sprite = variableGraphicsDefault.inputField;
                    break;
                case VariableType.Slider:
                    variableReferences.slider.handleRect.GetComponent<Image>().sprite = variableGraphicsDefault.sliderHandle;
                    variableReferences.sliderInput.image.sprite = variableGraphicsDefault.inputField;
                    break;
                case VariableType.Vector2:
                    if (fieldIndex == 0)
                    {
                        variableReferences.vector2X.image.sprite = variableGraphicsDefault.inputField;
                        variableReferences.vector2XText.color = variableGraphicsDefault.textColor;
                    }
                    else if (fieldIndex == 1)
                    {
                        variableReferences.vector2Y.image.sprite = variableGraphicsDefault.inputField;
                        variableReferences.vector2YText.color = variableGraphicsDefault.textColor;
                    }
                    break;
                case VariableType.Vector2Tall:
                    if (fieldIndex == 0)
                    {
                        variableReferences.vector2TallX.image.sprite = variableGraphicsDefault.inputField;
                        variableReferences.vector2TallXText.color = variableGraphicsDefault.textColor;
                    }
                    else if (fieldIndex == 1)
                    {
                        variableReferences.vector2TallY.image.sprite = variableGraphicsDefault.inputField;
                        variableReferences.vector2TallYText.color = variableGraphicsDefault.textColor;
                    }
                    break;
                case VariableType.Vector3:
                    if (fieldIndex == 0)
                    {
                        variableReferences.vector3X.image.sprite = variableGraphicsDefault.inputField;
                        variableReferences.vector3XText.color = variableGraphicsDefault.textColor;
                    }
                    else if (fieldIndex == 1)
                    {
                        variableReferences.vector3Y.image.sprite = variableGraphicsDefault.inputField;
                        variableReferences.vector3YText.color = variableGraphicsDefault.textColor;
                    }
                    else if (fieldIndex == 2)
                    {
                        variableReferences.vector3Z.image.sprite = variableGraphicsDefault.inputField;
                        variableReferences.vector3ZText.color = variableGraphicsDefault.textColor;
                    }
                    break;
                case VariableType.Vector4:
                    if (fieldIndex == 0)
                    {
                        variableReferences.vector4X.image.sprite = variableGraphicsDefault.inputField;
                        variableReferences.vector4XText.color = variableGraphicsDefault.textColor;
                    }
                    else if (fieldIndex == 1)
                    {
                        variableReferences.vector4Y.image.sprite = variableGraphicsDefault.inputField;
                        variableReferences.vector4YText.color = variableGraphicsDefault.textColor;
                    }
                    else if (fieldIndex == 2)
                    {
                        variableReferences.vector4Z.image.sprite = variableGraphicsDefault.inputField;
                        variableReferences.vector4ZText.color = variableGraphicsDefault.textColor;
                    }
                    else if (fieldIndex == 3)
                    {
                        variableReferences.vector4W.image.sprite = variableGraphicsDefault.inputField;
                        variableReferences.vector4WText.color = variableGraphicsDefault.textColor;
                    }
                    break;
                case VariableType.Boolean:
                    variableReferences.boolean.image.sprite = variableGraphicsDefault.boolean;
                    break;
                case VariableType.Color3:
                    if (fieldIndex == 0)
                        variableReferences.color3Overlay.sprite = variableGraphicsDefault.colorOverlay;
                    else if (fieldIndex == 1)
                        variableReferences.color3Eyedropper.sprite = variableGraphicsDefault.eyedropper;
                    break;
                case VariableType.Color4:
                    if (fieldIndex == 0)
                        variableReferences.color4Overlay.sprite = variableGraphicsDefault.colorOverlay;
                    else if (fieldIndex == 1)
                        variableReferences.color4Eyedropper.sprite = variableGraphicsDefault.eyedropper;
                    break;
                case VariableType.Enum:
                    variableReferences.enumDropdown.image.sprite = variableGraphicsDefault.dropdown;
                    break;
                case VariableType.Asset:
                    variableReferences.assetField.sprite = variableGraphicsDefault.inputField;
                    break;
            }
        }

        void SetAsset(object[] values)
        {
            switch (assetType)
            {
                case AssetType.None:
                    SetAsset(null);
                    break;
                case AssetType.Mesh:
                    variableReferences.assetImage.enabled = false;
                    variableReferences.assetIcon.enabled = true;
                    variableReferences.assetIcon.sprite = assetGraphics.mesh;
                    break;
                case AssetType.Texture:
                    variableReferences.assetImage.enabled = true;
                    variableReferences.assetIcon.enabled = false;
                    variableReferences.assetImage.texture = values[0] as Texture2D;
                    break;
                case AssetType.Transform:
                    variableReferences.assetImage.enabled = false;
                    variableReferences.assetIcon.enabled = true;
                    variableReferences.assetIcon.sprite = assetGraphics.transform;
                    break;
                case AssetType.Material:
                    variableReferences.assetImage.enabled = false;
                    variableReferences.assetIcon.enabled = true;
                    variableReferences.assetIcon.sprite = assetGraphics.transform;
                    break;
                case AssetType.PhysicMaterial:
                    variableReferences.assetImage.enabled = false;
                    variableReferences.assetIcon.enabled = true;
                    variableReferences.assetIcon.sprite = assetGraphics.transform;
                    break;
                case AssetType.GameObject:
                    variableReferences.assetImage.enabled = false;
                    variableReferences.assetIcon.enabled = true;
                    variableReferences.assetIcon.sprite = assetGraphics.transform;
                    break;
            }
            if (values != null)
            {
                string title = values[0].ToString();
                int l = title.IndexOf("(");
                if (l > 0)
                {
                    title = title.Substring(0, l);
                }
                string aLabel = title+ "(" + assetType + ")";
                variableReferences.assetLabel.text = aLabel;
                variableReferences.assetLabel.GetComponent<RectTransform>().anchoredPosition = new Vector2(18, 0);
                variableReferences.assetIcon.enabled = true;
            }
            else
            {
                string aLabel = "None (" + assetType + ")";
                variableReferences.assetLabel.text = aLabel;
                variableReferences.assetLabel.GetComponent<RectTransform>().anchoredPosition = new Vector2(4, 0);
                variableReferences.assetIcon.enabled = false;
            }
        }

        public void OpenColorPicker()
        {
            Debug.Log("Open color picker");
        }

        public void Eyedropper()
        {
            Debug.Log("Enable eyedropper");
        }

        public void OpenAssetBrowser()
        {
            Debug.Log("Open asset browser");
        }
    }
}
