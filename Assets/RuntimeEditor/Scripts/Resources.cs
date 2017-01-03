using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.RuntimeEditor
{
    public class Resources : MonoBehaviour
    {

        public HierarchyGraphics resourcesGraphicsDefault;
        public HierarchyGraphics resourcesGraphicsSelected;
        public GameObject resourcesEntryPrefab;
        public RectTransform contentRect;
        Globals globals;
        Inspector inspector;
        float currentSpawnPosition;
        float entryHeight = 18f;

        // Use this for initialization
        void Start ()
        {
            globals = transform.root.GetComponent<Globals>();
            inspector = GameObject.Find("Inspector").GetComponent<Inspector>(); //dirty
            FindResources();
        }
	
	    void FindResources()
        {
            object[] resources = UnityEngine.Resources.LoadAll("");
            for (int i = 0; i < resources.Length; i++)
                NextResource(resources[i]);
            CompleteResourcesSetup();
        }

        public void NextResource(object obj)
        {
            if (!globals.CheckTypeIgnoreList(obj.GetType().ToString()))
            {
                GameObject spawn = Instantiate(resourcesEntryPrefab, contentRect.transform, false) as GameObject;
                spawn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, currentSpawnPosition);
                string title = obj.ToString();
                int l = title.IndexOf("(");
                if (l > 0)
                    title = title.Substring(0, l);
                string type = obj.GetType().ToString().Replace("UnityEngine.", "");
                string label = title + "(" + type + ")";
                spawn.name = label;
                ResourcesEntry entry = spawn.GetComponent<ResourcesEntry>();
                entry.resourcesReferences.label.text = spawn.name;
                entry.resourcesReferences.contentRect.anchoredPosition = Vector2.zero;
                entry.resourcesReferences.button.onClick.AddListener(delegate { this.SelectResourcesEntry(obj, entry.resourcesReferences.button.image); });
                currentSpawnPosition -= entryHeight;
            }
        }

        public void SelectResourcesEntry(object input, Image entryImage)
        {
            int hierarchyChildCount = contentRect.transform.childCount;
            for (int i = 0; i < hierarchyChildCount; i++)
            {
                ResourcesEntry entry = contentRect.GetChild(i).GetComponent<ResourcesEntry>();
                entry.resourcesReferences.button.image.color = resourcesGraphicsDefault.entryColor;
            }
            entryImage.color = resourcesGraphicsSelected.entryColor;
            if (inspector != null)
            {
                Debug.Log("Set Inspector to Resource: " + input.ToString());
                //inspector.ChangeInspectorContext(input);
            }
        }

        public void CompleteResourcesSetup()
        {
            Debug.LogError("Completed Resources Generation");
        }
    }
}
