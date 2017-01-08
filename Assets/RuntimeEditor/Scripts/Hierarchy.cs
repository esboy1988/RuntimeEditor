using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UnityEngine.RuntimeEditor
{
    public class Hierarchy : MonoBehaviour
    {
        //Manual
        public GameObject hierarchyScenePrefab; //Used to spawn scene entries
        public GameObject hierarchyEntryPrefab; //Used to spawn object entries
        public RectTransform contentRect; //Used to spawn entires in
        public HierarchyGraphics hierarchyGraphicsDefault; //Unselected graphics data structure
        public HierarchyGraphics hierarchyGraphicsSelected; //Selected graphics data structure
        public float indentSize = 14f; //Size for each level of indent in pixels
        
        //Automatic
        public List<Scene> scenes = new List<Scene>();
        public List<HierarchyEntry> entries = new List<HierarchyEntry>(); //List all of generated entries
        Inspector inspector; //Reference to Inspector.cs
        int sceneCount; //Count of scenes
        int currentSceneIndex; //Used to iterate scenes
        float currentSpawnPosition; //Used to calculate position to position each entry
        float entryHeight = 18f; //Height of each entry

        //Run when component is enabled
        void Start()
        {
            inspector = GameObject.Find("Inspector").GetComponent<Inspector>(); //Get Inspector (dirty)
            RefreshHierarchy(); //Get Hierarchy data
        }

        //Run every frame
        public void RefreshHierarchy()
        {
            for (int i = 0; i < entries.Count; i++) //Iterate entry list
                Destroy(entries[i].gameObject); //Destroy entry item
            entries.Clear(); //Clear entry list
            scenes.Clear(); //Clear scene list
            currentSceneIndex = 0; //Reset scene index
            currentSpawnPosition = 0; //Reset spawn position
            sceneCount = SceneManager.sceneCount; //Get the scene count to iterate
            GetNextScene(); //Start work on next scene
        }

        //Get data for each scene in SceManager
        void GetNextScene()
        {
            if (currentSceneIndex < sceneCount) //Iterate scenes
            {
                scenes.Add(SceneManager.GetSceneAt(currentSceneIndex)); //Add scene to scenes list
                GameObject spawn = Instantiate(hierarchyScenePrefab, contentRect.transform, false) as GameObject; //Spawn a scene entry prefab
                spawn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, currentSpawnPosition); //Set entries position
                spawn.name = SceneManager.GetSceneAt(currentSceneIndex).name; //Set object name
                float indentLevel = 0; //Set indent level
                HierarchyEntry entry = spawn.GetComponent<HierarchyEntry>(); //Get reference to HierarchyEntry.cs
                entries.Add(entry); //Add to entry list to track
                entry.hierarchyReferences.label.text = spawn.name; //Set label text
                entry.hierarchyReferences.contentRect.anchoredPosition = new Vector2(indentLevel * indentSize, 0); //Apply indent
                currentSpawnPosition -= entryHeight; //Calulcate position for next entry spawn
                CrawlSceneHierarchy(indentLevel); //Crawl scene hierarchy
            }
            else //Finished crawling scenes
                Debug.Log("Completed Hierarchy Generation");
        }
        
        //Crawl root objects of scene
        void CrawlSceneHierarchy(float indentLevel)
        {
            indentLevel++; //Add indent level
            GameObject[] objectsFound = scenes[currentSceneIndex].GetRootGameObjects(); //Get all root objects of scene
            foreach (GameObject go in objectsFound) //Iterate root objects
            {
                if (go.name != "RuntimeUICanvas") //Ignore Runtime Editor objects
                {
                    GameObject spawn = Instantiate(hierarchyEntryPrefab, contentRect.transform, false) as GameObject; //Spawn a hierarchy entry
                    spawn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, currentSpawnPosition); //Set entries position
                    spawn.name = go.name; //Set object name
                    HierarchyEntry entry = spawn.GetComponent<HierarchyEntry>(); //Get reference to HierarchyEntry.cs
                    entries.Add(entry); //Add to entry list to track
                    entry.hierarchyReferences.label.text = spawn.name; //Set label text
                    entry.hierarchyReferences.contentRect.anchoredPosition = new Vector2(indentLevel * indentSize, 0); //Apply indent
                    if (go.transform.childCount == 0) //If object has no children
                    {
                        //Set expand properties here
                        //entry.isCollapsed = true; //Set as collapsed
                        entry.hierarchyReferences.expand.image.enabled = false; //Set expanded button to false
                    }
                    entry.hierarchyReferences.button.onClick.RemoveAllListeners(); //Reset button listener
                    entry.hierarchyReferences.button.onClick.AddListener(delegate { SelectHierarchyEntry(go, entry); }); //Set button listener
                    entry.hierarchyReferences.expand.onClick.RemoveAllListeners(); //Reset expand button listener
                    entry.hierarchyReferences.expand.onClick.AddListener(delegate { this.ExpandCollapseHierarchy(entry); }); //Set expand button listener
                    currentSpawnPosition -= entryHeight; //Calulcate position for next entry spawn
                    CrawlObject(indentLevel, go, entry); //Crawl this entries children
                }
            }
            CompleteSceneHierarchy(); //Finished with this scenes Hierarchy
        }

        //Crawl an objects children
        void CrawlObject(float indentLevel, GameObject inputGameObject, HierarchyEntry parent)
        {
            indentLevel++; //Add indent level
            int childCount = inputGameObject.transform.childCount; //Get child count
            GameObject[] childrenFound = new GameObject[childCount]; //Create an array for children
            for(int i = 0; i < childCount; i++) //Iterate children
            {
                childrenFound[i] = inputGameObject.transform.GetChild(i).gameObject; //Get references to children
            }
            foreach(GameObject child in childrenFound) //Iterate array of children
            {
                GameObject spawn = Instantiate(hierarchyEntryPrefab, contentRect.transform, false) as GameObject; //Spawn a hierarchy entry
                spawn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, currentSpawnPosition); //Set entries position
                spawn.name = child.name; //Set object name
                HierarchyEntry entry = spawn.GetComponent<HierarchyEntry>(); //Get reference to HierarchyEntry.cs
                entries.Add(entry); //Add to entry list to track
                parent.children.Add(entry); //Add to parents child list
                entry.hierarchyReferences.label.text = spawn.name; //Set label text
                entry.hierarchyReferences.contentRect.anchoredPosition = new Vector2(indentLevel * indentSize, 0); //Apply indent
                if (child.transform.childCount == 0) //If object has no children
                {
                    //entry.isCollapsed = true; //Set as collapsed
                    entry.hierarchyReferences.expand.image.enabled = false; //Set expanded button to false
                }
                entry.hierarchyReferences.button.onClick.RemoveAllListeners(); //Reset object button listener
                entry.hierarchyReferences.button.onClick.AddListener(delegate { this.SelectHierarchyEntry(child, entry); }); //Set object button listener
                entry.hierarchyReferences.expand.onClick.RemoveAllListeners(); //Reset expand button listener
                entry.hierarchyReferences.expand.onClick.AddListener(delegate { this.ExpandCollapseHierarchy(entry); }); //Set expand button listener
                currentSpawnPosition -= entryHeight; //Calulcate position for next entry spawn
                CrawlObject(indentLevel, child, entry); //Crawl this entries children
            }
        }

        //Completed crawling scenes hierarchy
        void CompleteSceneHierarchy()
        {
            currentSceneIndex++; //Increment scene index
            GetNextScene(); //Work on next scene
        }

        //Expands or collapses hierarchy from entry
        public void ExpandCollapseHierarchy(HierarchyEntry inputEntry)
        {
            inputEntry.isCollapsed = !inputEntry.isCollapsed; //Flip collapsed state
            int index = 0; //Integer for tracking collapsed item
            for(int i = 0; i < entries.Count; i++) //Iterate entries to find collapsed
            {
                if (entries[i] == inputEntry) //If matches
                {
                    index = i+1; //Set to integer of first to collapse
                    break; //Stop iterating
                }
            }
            if (inputEntry.isCollapsed) //If collapsing
            {
                int count = 0; //Integer for tracking how many items to collapse
                for (int i = 0; i < inputEntry.children.Count; i++) //Iterate collapsed items recursive children
                {
                    count += CollapsingRecursiveChildCount(inputEntry.children[i]); //Get child count
                }
                int range = index + count; //Range to iterate needs to be altered when already hidden items are found
                for (int i = index; i < range; i++) //Iterate entry list
                {
                    if (!entries[i].isHidden) //If not already hidden by its recursive parent
                    {
                        entries[i].isHidden = true; //Set hidden to check against
                        entries[i].gameObject.SetActive(false); //Set game object disabled
                    }
                    else //Otherwise
                        range++; //Dont count it
                }
                for (int i = index + count; i < entries.Count; i++) //Iterate all items after collapsing area
                {
                    RectTransform rect = entries[i].GetComponent<RectTransform>(); //Get entries rect
                    rect.anchoredPosition = new Vector2(0, rect.anchoredPosition.y + entryHeight * count); //Move up by amount of items collapsed
                }
            }
            else if (!inputEntry.isCollapsed) //If expanding
            {
                int count = 0; //Integer for tracking how many items to collapse
                for (int i = 0; i < inputEntry.children.Count; i++) //Iterate collapsed items recursive children
                {
                    count += ExpandingRecursiveChildCount(inputEntry.children[i]); //Get child count
                }
                int range = index + count; //Range to iterate needs to be altered when already hidden items are found
                for (int i = index; i < range; i++) //Iterate entry list
                {
                    if (entries[i].isHidden) //If not already hidden by its recursive parent
                    {
                        entries[i].isHidden = false; //Set hidden to check against
                        entries[i].gameObject.SetActive(true); //Set game object disabled
                    }
                    else //Otherwise
                        range++; //Dont count it
                }
                for (int i = index + count; i < entries.Count; i++) //Iterate all items after collapsing area
                {
                    RectTransform rect = entries[i].GetComponent<RectTransform>(); //Get entries rect
                    rect.anchoredPosition = new Vector2(0, rect.anchoredPosition.y - entryHeight * count); //Move up by amount of items collapsed
                }
            }
        }

        //Used to recursive search of child count for collapsing
        public int CollapsingRecursiveChildCount(HierarchyEntry inputEntry)
        {
            int count = 0; //Integer for tracking children found
            if (!inputEntry.isHidden) //If not collapsed
            {
                count = 1; //Set to one before searching children
                for (int i = 0; i < inputEntry.children.Count; i++) //Iterate children
                {
                    int c = CollapsingRecursiveChildCount(inputEntry.children[i]);
                    count += c; //Search for its children
                }
            }
            return count; //Return
        }

        //Used to recursive search of child count for expanding
        public int ExpandingRecursiveChildCount(HierarchyEntry inputEntry)
        {
            int count = 1; //Integer for tracking children found
            if (!inputEntry.isCollapsed) //If not collapsed
            {
                for (int i = 0; i < inputEntry.children.Count; i++) //Iterate children
                {
                    int c = ExpandingRecursiveChildCount(inputEntry.children[i]);
                    count += c; //Search for its children
                }
            }
            return count; //Return
        }

        //Selects GameObject for this entry (called by Listener on Button)
        public void SelectHierarchyEntry(GameObject inputObject, HierarchyEntry inputEntry)
        {
            for (int i = 0; i < entries.Count; i++) //Iterate hierarchy items
            {
                entries[i].hierarchyReferences.button.image.color = hierarchyGraphicsDefault.entryColor; //Reset button color to default
            }
            inputEntry.hierarchyReferences.button.image.color = hierarchyGraphicsSelected.entryColor; //Set this buttons color to selected
            if (inspector != null) //If reference to Inspector exists
            {
                Debug.Log("Set Inspector to GameObject: " + inputObject.ToString());
                inspector.ChangeInspectorContext(inputObject); //Change context on Inspector.cs to match this selected gameobject
            }
        }
    }

    /// <summary>
    /// DATA STRUCTURES
    /// </summary>

    [System.Serializable]
    public class HierarchyGraphics
    {
        public Sprite expandIcon;
        public Color entryColor;
        public Color textColor;
    }
}
