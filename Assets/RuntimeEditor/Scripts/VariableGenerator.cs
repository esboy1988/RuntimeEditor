using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.RuntimeEditor
{
    public class VariableGenerator : MonoBehaviour
    {
        //Manual
        public VariableGeneratorSourceType variableGeneratorSourceType;
        public VariableGeneratorReferences variableGeneratorReferences;
        public GameObject variablePrefab;

        //Automatic
        RectTransform contentRect;
        float spawnHeight;
        RuntimeEditor.Component rComponent;
        [HideInInspector] public UnityEngine.Component component;
        Globals globals;
        GameObject currentVariable;
        bool sourceFound = false;
        int propertyCount = 0;
        int currentProperty = 0;
        bool propertyListComplete = false;
        int fieldCount = 0;
        int currentField = -1;
        bool fieldListComplete = false;

        void Awake()
        {
            globals = transform.root.GetComponent<Globals>();
            if (variableGeneratorSourceType == VariableGeneratorSourceType.Component)
            {
                rComponent = GetComponent<RuntimeEditor.Component>();
                sourceFound = true;
            }
        }

        public void SetCounts(int properties, int fields)
        {
            if (variableGeneratorSourceType == VariableGeneratorSourceType.Component)
            {
                propertyCount = properties;
                fieldCount = fields;
            }
        }

        public void GenerateVariable(object input, int inputType, VariableType vType, AssetType aType, bool integer, bool isEditable, float indent, string[] addLabels, object[] inputObj)
        {
            GameObject go = Instantiate(variablePrefab as GameObject, contentRect.transform, false);
            currentVariable = go;
            RectTransform vRect = currentVariable.GetComponent<RectTransform>();
            vRect.anchoredPosition = new Vector2(0, -spawnHeight);
            Variable newVariable = currentVariable.GetComponent<Variable>();
            variableGeneratorReferences.variables.Add(newVariable);
            object[] objects = null;
            string[] labels = null;
            if (inputType == 0) //property
            {
                System.Reflection.PropertyInfo property = input as System.Reflection.PropertyInfo;
                labels = new string[] { property.Name };
                if (property.GetValue(component, new object[0]) != null)
                {
                    if (vType == VariableType.Enum)
                        objects = inputObj;
                    else
                        objects = new object[] { property.GetValue(component, new object[0]) };
                }
            }
            else if (inputType == 1) //field
            {
                System.Reflection.FieldInfo field = input as System.Reflection.FieldInfo;
                labels = new string[] { field.Name };
                if (field.GetValue(component) != null)
                {
                    if (vType == VariableType.Enum)
                        objects = inputObj;
                    else
                        objects = new object[] { field.GetValue(component) };
                }
            }
            if (addLabels != null)
                labels = labels.Concat(addLabels).ToArray();
            newVariable.VariableSetup(this, component, inputType, vType, aType, integer, isEditable, labels, objects, indent);
        }

        public void RequestVariableGeneration(RectTransform cRect, object input, int inputType, bool isEditable, float indent)
        {
            if (sourceFound)
            {
                contentRect = cRect;
                System.Reflection.PropertyInfo property = input as System.Reflection.PropertyInfo;
                System.Reflection.FieldInfo field = input as System.Reflection.FieldInfo;
                string typeCheck = null;
                if (inputType == 0)
                    typeCheck = property.PropertyType.ToString();
                else if (inputType == 1)
                    typeCheck = field.FieldType.ToString();
                CheckVariableType(input, inputType, typeCheck, isEditable, indent);
            }
        }

        void CheckVariableType(object input, int inputType, string typeCheck, bool isEditable, float indent)
        {
            //Check standard variable types
            if (!ProcessStandardVariableTypes(input, inputType, typeCheck, isEditable, indent))
            {
                //Check asset variable types
                if (!ProcessAssetVariableTypes(input, inputType, typeCheck, isEditable, indent))
                {
                    //Check enum variable types
                    if (!ProcessEnumVariableTypes(input, inputType, typeCheck, isEditable, indent))
                    {
                        string variableString = null;
                        if (inputType == 0)
                        {
                            System.Reflection.PropertyInfo property = input as System.Reflection.PropertyInfo;
                            variableString = property.ToString();
                        }
                        else if (inputType == 1)
                        {
                            System.Reflection.FieldInfo field = input as System.Reflection.FieldInfo;
                            variableString = field.ToString();
                        }
                        OnVariableFail(variableString, currentVariable);
                    }
                }
            }
        }
        
        bool ProcessStandardVariableTypes(object input, int inputType, string typeCheck, bool isEditable, float indent)
        {
            string[] addLabels = null;
            switch (typeCheck)
            {
                case "System.Int32":
                    GenerateVariable(input, inputType, VariableType.InputField, AssetType.None, true, isEditable, indent, addLabels, null);
                    return true;
                case "System.Single":
                    GenerateVariable(input, inputType, VariableType.InputField, AssetType.None, false, isEditable, indent, addLabels, null);
                    return true;
                case "System.Boolean":
                    GenerateVariable(input, inputType, VariableType.Boolean, AssetType.None, false, isEditable, indent, addLabels, null);
                    return true;
                case "UnityEngine.Color":
                    GenerateVariable(input, inputType, VariableType.Color4, AssetType.None, false, isEditable, indent, addLabels, null);
                    return true;
                case "UnityEngine.Color32":
                    GenerateVariable(input, inputType, VariableType.Color4, AssetType.None, false, isEditable, indent, addLabels, null);
                    return true;
                case "UnityEngine.Vector2":
                    addLabels = new string[] { "X", "Y" };
                    GenerateVariable(input, inputType, VariableType.Vector2, AssetType.None, false, isEditable, indent, addLabels, null);
                    return true;
                case "UnityEngine.Vector3":
                    addLabels = new string[] { "X", "Y", "Z" };
                    GenerateVariable(input, inputType, VariableType.Vector3, AssetType.None, false, isEditable, indent, addLabels, null);
                    return true;
                case "UnityEngine.Vector4":
                    addLabels = new string[] { "X", "Y", "Z", "W" };
                    GenerateVariable(input, inputType, VariableType.Vector4, AssetType.None, false, isEditable, indent, addLabels, null);
                    return true;
            }
            return false;
        }

        public bool ProcessAssetVariableTypes(object input, int inputType, string typeCheck, bool isEditable, float indent)
        {
            string[] addLabels = null;
            switch (typeCheck)
            {
                case "UnityEngine.Transform":
                    GenerateVariable(input, inputType, VariableType.Asset, AssetType.Transform, false, isEditable, indent, addLabels, null);
                    return true;
                case "UnityEngine.Mesh":
                    GenerateVariable(input, inputType, VariableType.Asset, AssetType.Mesh, false, isEditable, indent, addLabels, null);
                    return true;
                case "UnityEngine.Texture2D":
                    GenerateVariable(input, inputType, VariableType.Asset, AssetType.Texture, false, isEditable, indent, addLabels, null);
                    return true;
                case "UnityEngine.Material":
                    GenerateVariable(input, inputType, VariableType.Asset, AssetType.Material, false, isEditable, indent, addLabels, null);
                    return true;
                case "UnityEngine.PhysicMaterial":
                    GenerateVariable(input, inputType, VariableType.Asset, AssetType.PhysicMaterial, false, isEditable, indent, addLabels, null);
                    return true;
                case "UnityEngine.GameObject":
                    GenerateVariable(input, inputType, VariableType.Asset, AssetType.GameObject, false, isEditable, indent, addLabels, null);
                    return true;
            }
            return false;
        }

        bool ProcessEnumVariableTypes(object input, int inputType, string typeCheck, bool isEditable, float indent)
        {
            if (inputType == 0)
            {
                System.Reflection.PropertyInfo property = input as System.Reflection.PropertyInfo;
                if (property.PropertyType.IsEnum)
                {
                    System.Array enumValues = System.Enum.GetValues(property.PropertyType);
                    object[] value = new object[1] { property.GetValue(component, new object[0]) };
                    object[] options = enumValues.OfType<object>().ToArray();
                    object[] outputObj = new object[value.Length + options.Length];
                    value.CopyTo(outputObj, 0);
                    options.CopyTo(outputObj, 1);
                    GenerateVariable(input, inputType, VariableType.Enum, AssetType.None, false, isEditable, indent, null, outputObj);
                    return true;
                }
                else
                    return false;
            }
            else if (inputType == 1)
            {
                System.Reflection.FieldInfo field = input as System.Reflection.FieldInfo;
                if (field.FieldType.IsEnum)
                {
                    System.Array enumValues = System.Enum.GetValues(field.FieldType);
                    object[] value = new object[1] { field.GetValue(component) };
                    object[] options = enumValues.OfType<object>().ToArray();
                    object[] outputObj = new object[value.Length + options.Length];
                    value.CopyTo(outputObj, 0);
                    options.CopyTo(outputObj, 1);
                    GenerateVariable(input, inputType, VariableType.Enum, AssetType.None, false, isEditable, indent, null, outputObj);
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        //THIS DOESNT WORK - REMOVED FROM THE CHECK FUNCTION FOR NOW
        bool ProcessStructVariableTypes(object input, int inputType)
        {
            if (inputType == 0)
            {
                System.Reflection.PropertyInfo p = input as System.Reflection.PropertyInfo;
                Debug.Log("name: " + p.Name + " attributes: " + p.Attributes + " declaring type: " + p.DeclaringType + " accessors: " + p.GetAccessors() + " attributes: " + p.GetCustomAttributes(true) + " getmethod: " + p.GetGetMethod() + " setmethod: " + p.GetSetMethod() + " type: " + p.GetType() + " membertype: " + p.MemberType + " module: " + p.Module + " proptype: " + p.PropertyType + " reftype: " + p.ReflectedType);
                CompleteVariableSetup(16);
                return false;
            }
            return false;
        }
        
        void OnVariableFail(string inputString, GameObject varObject)
        {
            int l = inputString.IndexOf(" ");
            if (l > 0)
                inputString = inputString.Substring(0, l);
            if (!globals.CheckTypeIgnoreList(inputString))
                Debug.LogWarning("A variable " + inputString + " was removed from component " + component.GetType());
            CompleteVariableSetup(0);
        }

        public void CompleteVariableSetup(float previousVariableHeight)
        {
            spawnHeight += previousVariableHeight;
            if (variableGeneratorSourceType == VariableGeneratorSourceType.Component)
            {
                if (propertyCount > 0 && !propertyListComplete)
                {
                    if (currentProperty < propertyCount - 1)
                    {
                        currentProperty++;
                        rComponent.NextVariable(0, currentProperty, true, 0);
                    }
                    else
                    {
                        propertyListComplete = true;
                        CompleteVariableSetup(0);
                    }
                }
                else if (fieldCount > 0 && !fieldListComplete)
                {
                    if (currentField < fieldCount - 1)
                    {
                        currentField++;
                        rComponent.NextVariable(1, currentField, true, 0);
                    }
                    else
                    {
                        fieldListComplete = true;
                        CompleteVariableSetup(0);
                    }
                }
                else
                {
                    spawnHeight += 2; //add space for bottom line
                    contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, spawnHeight);
                    if (variableGeneratorSourceType == VariableGeneratorSourceType.Component)
                    {
                        rComponent.CompleteComponentSetup(spawnHeight);
                    }
                }
            }
        }
    }
}
