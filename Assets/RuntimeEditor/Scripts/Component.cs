using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.RuntimeEditor
{
    public class Component : MonoBehaviour
    {
        //Manual
        public ComponentGraphics componentGraphicsDefault;
        public ComponentGraphics componentGraphicsSelected;
        public ComponentReferences componentReferences;

        //Automatic
        public List<System.Reflection.PropertyInfo> propertyList = new List<System.Reflection.PropertyInfo>();
        public List<System.Reflection.FieldInfo> fieldList = new List<System.Reflection.FieldInfo>();
        
        Inspector inspector;
        [HideInInspector] public UnityEngine.Component component;
        VariableGenerator variableGenerator;
        bool isSetup = false;
        bool isExpanded = true;
        bool canDisable = false;
        float contentHeight;

        public void SetupComponent(Inspector createdBy, UnityEngine.Component componentData)
        {
            inspector = createdBy;
            component = componentData;
            variableGenerator = GetComponent<VariableGenerator>();
            variableGenerator.component = componentData;
            System.Type myType = component.GetType();
            System.Reflection.PropertyInfo[] properties = myType.GetProperties();
            System.Reflection.FieldInfo[] fields = myType.GetFields();
            //DEBUG
            //string[] resultArray = System.Array.ConvertAll(properties, x => x.ToString());
            //Debug.LogWarning("Found component of type " + myType + " with " + properties.Length + " fields: " + string.Join(", ", resultArray));
            //DEBUG
            foreach (System.Reflection.PropertyInfo property in properties) //Find properties
            {
                if (property != null && property.Name == "enabled") //Convet enable variable to component property
                {
                    componentReferences.enabledToggle.isOn = (bool)property.GetValue(component, new object[0]);
                    canDisable = true;
                }
                else if (property.CanWrite) //Public variable
                    propertyList.Add(property);
            }
            foreach (System.Reflection.FieldInfo field in fields) //Find fields
            {
                if (field != null && field.IsPublic) //Public variable
                    fieldList.Add(field);
            }
            object output = propertyList[0];
            variableGenerator.SetCounts(propertyList.Count, fieldList.Count);
            variableGenerator.RequestVariableGeneration(componentReferences.contentRect, output, 0, true, 0);
            if (!canDisable)
                componentReferences.enabledToggle.gameObject.SetActive(false);
        }

        public void CompleteComponentSetup(float finalHeight)
        {
            RectTransform thisRect = GetComponent<RectTransform>();
            thisRect.sizeDelta = new Vector2(thisRect.sizeDelta.x, componentReferences.contentRect.sizeDelta.y + componentReferences.titleBarRect.sizeDelta.y);
            string cName = component.GetType().ToString();
            string nSpace = component.GetType().Namespace;
            if (nSpace != null)
                cName = cName.Replace(nSpace, "").Replace(".", "");
            componentReferences.componentLabel.text = Extensions.FormatStringConvention(cName);
            inspector.CompleteComponentSetup(GetComponent<RectTransform>().sizeDelta.y);
            contentHeight = finalHeight;
            isSetup = true;
        }

        public void NextVariable(int varType, int varIndex, bool isEditable, float indent)
        {
            if(varType == 0)
            {
                object obj = propertyList[varIndex];
                variableGenerator.RequestVariableGeneration(componentReferences.contentRect, obj, 0, isEditable, indent);
            }
            else if(varType == 1)
            {
                object obj = fieldList[varIndex];
                variableGenerator.RequestVariableGeneration(componentReferences.contentRect, obj, 1, isEditable, indent);
            }
        }

        public void ToggleEnabled()
        {
            if (isSetup)
            {
                System.Type myType = component.GetType();
                System.Reflection.PropertyInfo[] properties = myType.GetProperties();
                foreach (System.Reflection.PropertyInfo property in properties)
                {
                    if (property.Name == "enabled")
                    {
                        bool previousState = (bool)property.GetValue(component, new object[0]);
                        property.SetValue(component, !previousState, new object[0]);
                    }
                }
            }
        }

        public void OnSelect(int fieldIndex)
        {
            if (fieldIndex == 0) //Enabled toggle
                componentReferences.enabledToggle.image.sprite = componentGraphicsSelected.toggle;
        }

        public void OnDeselect(int fieldIndex)
        {
            if (fieldIndex == 0) //Enabled toggle
                componentReferences.enabledToggle.image.sprite = componentGraphicsDefault.toggle;
        }

        public void ExpandComponent()
        {
            isExpanded = !isExpanded;
            float previousHeight = GetComponent<RectTransform>().sizeDelta.y;
            if (isExpanded)
            {
                GetComponent<RectTransform>().sizeDelta = new Vector2(componentReferences.contentRect.sizeDelta.x, contentHeight + componentReferences.titleBarRect.sizeDelta.y);
                componentReferences.contentRect.sizeDelta = new Vector2(componentReferences.contentRect.sizeDelta.x, contentHeight);
                componentReferences.expandIcon.sprite = componentGraphicsSelected.expand;
                foreach (Variable variable in variableGenerator.variableGeneratorReferences.variables)
                {
                    if (variable != null)
                        variable.gameObject.SetActive(true);
                }
            }
            else
            {
                GetComponent<RectTransform>().sizeDelta = new Vector2(componentReferences.contentRect.sizeDelta.x, componentReferences.titleBarRect.sizeDelta.y+2);
                componentReferences.contentRect.sizeDelta = new Vector2(componentReferences.contentRect.sizeDelta.x, 2);
                componentReferences.expandIcon.sprite = componentGraphicsDefault.expand;
                foreach (Variable variable in variableGenerator.variableGeneratorReferences.variables)
                {
                    if (variable != null)
                        variable.gameObject.SetActive(false);
                }
            }
            float newHeight = GetComponent<RectTransform>().sizeDelta.y;
            inspector.SetComponentPositions(this, previousHeight, newHeight);
        }
    }    
}