using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.RuntimeEditor
{
    public class Inspector : MonoBehaviour
    {
        //Manual
        public GameObject componentPrefab; //Used to spawn components
        public InspectorReferences inspectorReferences; //Data structure for component references
        public Object selectedObject; //Tracks selected object
        
        //Automatic
        [HideInInspector] public List<UnityEngine.Component> behavioursFound = new List<UnityEngine.Component>(); //Track behaviours found when updating inspector
        public List<string> layerList = new List<string>(); //Collapsed layer list
        public List<RuntimeEditor.Component> generatedComponents = new List<RuntimeEditor.Component>(); //Current list of component scripts
        float contentHeight; //Used to calculate position to position each component
        int currentComponent = 0; //Used to iterate components
        Hierarchy hierarchy; //Used to perform code on the Hierarchy window

        //DEBUG
        bool clockRunning = false;
        float clock;
        float startTime;
        float endTime;

        //Run when component is enabled
        void Start()
        {
            //GenerateInspector();
            hierarchy = GameObject.Find("Hierarchy").GetComponent<Hierarchy>(); //Get Hierarchy (dirty)
            GetLayerList(); //Get layer list (never updated)
        }

        //Run every frame
        void Update()
        {
            // DEBUG START
            if (clockRunning)
            {
                clock += Time.deltaTime;
            }
            // DEBUG END
        }

        //Called by Hierarchy.cs when an object is selected
        public void ChangeInspectorContext(Object input)
        {
            selectedObject = (GameObject)input; //Selected object is input
            RefreshInspector(); //Refresh the inspector
        }

        //Refresh Inspector context
        void RefreshInspector()
        {
            behavioursFound.Clear(); //Clear found behaviours list
            for (int i = 0; i < generatedComponents.Count; i++) //Iterate current generated component objects
                Destroy(generatedComponents[i].gameObject); //Destroy
            generatedComponents.Clear();  //Clear list
            contentHeight = 0; //Reset height calculations
            currentComponent = 0; //Reset component index
            contentHeight += inspectorReferences.titleBar.sizeDelta.y; //Add title bars height to calculations
            UnityEngine.Component[] components = ((GameObject)selectedObject).GetComponents<UnityEngine.Component>(); //Get array of components from selected object
            foreach (UnityEngine.Component b in components) //Iterate components
                behavioursFound.Add(b); //Add to behaviours list
            if(behavioursFound.Count > 0) //If behaviours exist
                GenerateComponent(behavioursFound[0]); //Generate a component
            UpdateTitleBar(); //Set title bar contents
        }

        //Set title bar contents
        void UpdateTitleBar()
        {
            inspectorReferences.titleBar.SetAsLastSibling(); //Set to end of hierarchy so dropdowns are visible
            if (selectedObject) //If there is a selected object
            {
                //GameObject code start (need to handle assets)
                GameObject go = selectedObject as GameObject;  //Get selected as gameobject
                inspectorReferences.selectedObjectName.text = go.name; //Set the title bar input field
                inspectorReferences.enabledToggle.isOn = go.activeSelf; //Set enabled state
                inspectorReferences.staticToggle.isOn = go.isStatic; //Set static state

                inspectorReferences.tagDropdown.ClearOptions(); //Clear tag dropdown
                List<Dropdown.OptionData> tagOptionList = new List<Dropdown.OptionData>(); //Create list of option data
                for (int i = 0; i < 1; i++) //Iterate tags
                {
                    Dropdown.OptionData option = new Dropdown.OptionData(); //Make new option data
                    option.text = go.tag; //Set text to tag
                    tagOptionList.Add(option); //Add to option list
                }
                inspectorReferences.tagDropdown.AddOptions(tagOptionList); //Set dropdown to option list
                inspectorReferences.tagDropdown.value = 0; //Set current tag value

                string layerName = LayerMask.LayerToName(go.layer); //Get objects layer name
                for(int i = 0; i < layerList.Count; i++) //Iterate collapsed layer list to find layer index
                {
                    if(layerList[i] == layerName) //If layer name is equal to this one
                    {
                        inspectorReferences.layerDropdown.value = i; //Set dropdown value
                        break; //stop iteration
                    }
                }
                //GameObject code end (need to handle assets)
            }
            else //No selected object
                inspectorReferences.selectedObjectName.text = ""; //Clear the title bar input field
        }

        //Get layer list (never updated at runtime)
        void GetLayerList()
        {
            inspectorReferences.layerDropdown.ClearOptions(); //Clear layer dropdown
            List<Dropdown.OptionData> layerOptionList = new List<Dropdown.OptionData>(); //Create list of option data
            for (int i = 0; i < 32; i++) //Iterate layer list
            {
                string layerName = LayerMask.LayerToName(i); //Get layer name
                layerList.Add(layerName); //Add to collapsed layer list
                if (LayerMask.LayerToName(i) != null && LayerMask.LayerToName(i) != "") //If layer is found
                {
                    Dropdown.OptionData option = new Dropdown.OptionData(); //Make new option data
                    option.text = layerName; //Set object text to layer name
                    layerOptionList.Add(option); //Add to option list
                }
            }
            inspectorReferences.layerDropdown.AddOptions(layerOptionList); //Set dropdown to option list
        }

        //Set layer of selected object
        public void SetLayer ()
        {
            List<Dropdown.OptionData> options = inspectorReferences.layerDropdown.options; //Get options list from layer dropdown
            string layerName = options[inspectorReferences.layerDropdown.value].text; //Get selected layer name from selected option
            for (int i = 0; i < layerList.Count; i++) //Iterate collapsed layer list to find layer name
            {
                if (layerList[i] == layerName) //If layer name is equal to this one
                {
                    ((GameObject)selectedObject).layer = i; //Set objects layer
                    break; //stop iteration
                }
            }
        }

        //Sets GameObject name
        public void SetGameObjectName()
        {
            GameObject go = selectedObject as GameObject; //Get selected object as GameObject
            go.name = inspectorReferences.selectedObjectName.text; //Set GameObject name from input field text
            hierarchy.RefreshHierarchy(); //Refresh Hierarchy to reflect change
        }

        //Enables or disables the gameobject
        public void SetEnabled()
        {
            GameObject go = selectedObject as GameObject; //Get selected object as game object
            go.SetActive(inspectorReferences.enabledToggle.isOn); //Set enabled state to match UI toggle
        }

        //Sets the objects static flag
        public void SetStatic()
        {
            GameObject go = selectedObject as GameObject; //Get selected object as game object
            go.isStatic = inspectorReferences.staticToggle.isOn; //Set static state to match UI toggle
        }

        //Generate a component
        void GenerateComponent(UnityEngine.Component component)
        {
            inspectorReferences.components.Clear(); //Clear current component references
            GameObject go = Instantiate(componentPrefab as GameObject, inspectorReferences.contentWindow.transform, false); //Instantiate component element
            string cName = component.GetType().ToString(); //Get Component name from type
            string nSpace = component.GetType().Namespace; //Get Component namespace
            if (nSpace != null)
                cName = cName.Replace(nSpace, "").Replace(".", ""); //Remove namespace from name
            go.name = cName; //Set object name
            RectTransform cRect = go.GetComponent<RectTransform>(); //Get rect transform
            cRect.anchoredPosition = new Vector2(0, -contentHeight); //Set component position to calculated position
            RuntimeEditor.Component newComponent = go.GetComponent<RuntimeEditor.Component>(); //Get Component.cs reference
            generatedComponents.Add(newComponent); //Add to list of component scripts
            newComponent.SetupComponent(this, component); //Start setup of component script
            inspectorReferences.components.Add(newComponent); //Add to component script list on Inspector.cs
            //contentHeight += cRect.sizeDelta.y; //Calculate position for next Inspector item
        }

        //Finalize previous component (called by previous component)
        public void CompleteComponentSetup(float previousComponentHeight)
        {
            currentComponent++; //Increment component index
            contentHeight += previousComponentHeight; //Add previous component height to calculation for next components position
            if (currentComponent <= behavioursFound.Count - 1) //If not finished generating components
                GenerateComponent(behavioursFound[currentComponent]); //Generate next component
            else //If finished
                Invoke("CompleteInspectorSetup", 0.05f); //Complete setup
        }

        //Finalise component setup
        public void CompleteInspectorSetup()
        {
            inspectorReferences.components.Reverse(); //Flip components list (why?)
            inspectorReferences.footer.anchoredPosition = new Vector2(inspectorReferences.footer.anchoredPosition.x, -contentHeight); //Set footer position
            contentHeight += inspectorReferences.footer.sizeDelta.y; //Add footer height to position calculations
            inspectorReferences.contentWindow.sizeDelta = new Vector2(inspectorReferences.contentWindow.sizeDelta.x, contentHeight); //Set content window to match all components
        }

        //Set new component positions when one is adjusted
        public void SetComponentPositions(RuntimeEditor.Component source, float previousHeight, float newHeight)
        {
            int adjustedComponent = 0; //Int for finding component adjusted
            for (int i = 0; i < inspectorReferences.components.Count; i++) //Iterate component list
            {
                if(inspectorReferences.components[i] == source) //If this index matches adjusted component
                    adjustedComponent = i; //Set integer
            }
            for (int i = 0; i < inspectorReferences.components.Count; i++) //Iterate component list
            {
                if (i > adjustedComponent) //If component is after the one that was adjusted
                {
                    RectTransform rect = inspectorReferences.components[i].GetComponent<RectTransform>(); //Get rect transform
                    Vector2 newPos = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + (previousHeight - newHeight)); //Set position based on calculations of resizing of adjusted component
                    rect.anchoredPosition = newPos; //Set position to match calculations
                }
            }
            Vector2 newFooterPos = new Vector2(inspectorReferences.footer.anchoredPosition.x, inspectorReferences.footer.anchoredPosition.y + (previousHeight - newHeight)); //Calculate new footer position
            inspectorReferences.footer.anchoredPosition = newFooterPos; //Set footer position
            contentHeight -= (previousHeight - newHeight); //Calculate final position
            inspectorReferences.contentWindow.sizeDelta = new Vector2(inspectorReferences.contentWindow.sizeDelta.x, contentHeight); //Set content window size
        }
    }
}