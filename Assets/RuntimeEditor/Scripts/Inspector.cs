using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.RuntimeEditor
{
    public class Inspector : MonoBehaviour
    {
        //Manual
        public GameObject componentPrefab;
        public InspectorReferences inspectorReferences;
        public GameObject selectedObject;
        
        //Automatic
        [HideInInspector] public List<UnityEngine.Component> behavioursFound = new List<UnityEngine.Component>();
        public List<RuntimeEditor.Component> generatedComponents = new List<RuntimeEditor.Component>();
        float contentHeight;
        int currentComponent = 0;

        //DEBUG
        bool clockRunning = false;
        float clock;
        float startTime;
        float endTime;

        void Start()
        {
            //GenerateInspector();
        }

        void Update()
        {
            if(Input.GetKeyUp(KeyCode.Return))
            {
                RefreshInspector();
            }
            if(clockRunning)
            {
                clock += Time.deltaTime;
            }
        }

        public void ChangeInspectorContext(GameObject input)
        {
            selectedObject = input;
            RefreshInspector();
        }

        void RefreshInspector()
        {
            behavioursFound.Clear();
            for (int i = 0; i < generatedComponents.Count; i++)
                Destroy(generatedComponents[i].gameObject);
            generatedComponents.Clear();
            contentHeight = 0;
            currentComponent = 0;
            UnityEngine.Component[] components = selectedObject.GetComponents<UnityEngine.Component>();
            foreach (UnityEngine.Component b in components)
                behavioursFound.Add(b);
            if(behavioursFound.Count > 0)
                GenerateComponent(behavioursFound[0]);
        }

        void GenerateComponent(UnityEngine.Component component)
        {
            GameObject go = Instantiate(componentPrefab as GameObject, inspectorReferences.contentWindow.transform, false);
            string cName = component.GetType().ToString();
            string nSpace = component.GetType().Namespace;
            if (nSpace != null)
                cName = cName.Replace(nSpace, "").Replace(".", "");
            go.name = cName;
            RectTransform cRect = go.GetComponent<RectTransform>();
            cRect.anchoredPosition = new Vector2(0, -contentHeight);
            RuntimeEditor.Component newComponent = go.GetComponent<RuntimeEditor.Component>();
            generatedComponents.Add(newComponent);
            newComponent.SetupComponent(this, component);
            inspectorReferences.components.Add(newComponent);
            contentHeight += cRect.sizeDelta.y;
        }

        public void CompleteComponentSetup(float previousComponentHeight)
        {
            currentComponent++;
            contentHeight += previousComponentHeight;
            if (currentComponent <= behavioursFound.Count - 1)
            {
                GenerateComponent(behavioursFound[currentComponent]);
            }
            else
                Invoke("CompleteInspectorSetup", 0.05f);
        }

        public void CompleteInspectorSetup()
        {
            inspectorReferences.components.Reverse();
            contentHeight = contentHeight / 2;
            inspectorReferences.footer.anchoredPosition = new Vector2(inspectorReferences.footer.anchoredPosition.x, -contentHeight);
            contentHeight += inspectorReferences.footer.sizeDelta.y;
            inspectorReferences.contentWindow.sizeDelta = new Vector2(inspectorReferences.contentWindow.sizeDelta.x, contentHeight);
        }

        public void SetComponentPositions(RuntimeEditor.Component source, float previousHeight, float newHeight)
        {
            int adjustedComponent = 0;
            for (int i = 0; i < inspectorReferences.components.Count; i++)
            {
                if(inspectorReferences.components[i] == source)
                    adjustedComponent = i;
            }
            for (int i = 0; i < inspectorReferences.components.Count; i++)
            {
                if (i > adjustedComponent)
                {
                    RectTransform rect = inspectorReferences.components[i].GetComponent<RectTransform>();
                    Vector2 newPos = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + (previousHeight - newHeight));
                    rect.anchoredPosition = newPos;
                }
            }
            Vector2 newFooterPos = new Vector2(inspectorReferences.footer.anchoredPosition.x, inspectorReferences.footer.anchoredPosition.y + (previousHeight - newHeight));
            inspectorReferences.footer.anchoredPosition = newFooterPos;
            contentHeight -= (previousHeight - newHeight);
            inspectorReferences.contentWindow.sizeDelta = new Vector2(inspectorReferences.contentWindow.sizeDelta.x, contentHeight);
        }
    }
}