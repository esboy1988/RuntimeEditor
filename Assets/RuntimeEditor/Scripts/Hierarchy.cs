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
                        entry.hierarchyReferences.expand.enabled = false; //Set expanded image to false
                    }
                    entry.hierarchyReferences.button.onClick.RemoveAllListeners(); //Reset button listener
                    entry.hierarchyReferences.button.onClick.AddListener(delegate { SelectHierarchyEntry(go, entry.hierarchyReferences.button.image); }); //Set button listener
                    currentSpawnPosition -= entryHeight; //Calulcate position for next entry spawn
                    CrawlObject(indentLevel, go); //Crawl this entries children
                }
            }
            CompleteSceneHierarchy(); //Finished with this scenes Hierarchy
        }

        //Crawl an objects children
        void CrawlObject(float indentLevel, GameObject inputGameObject)
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
                entry.hierarchyReferences.label.text = spawn.name; //Set label text
                entry.hierarchyReferences.contentRect.anchoredPosition = new Vector2(indentLevel * indentSize, 0); //Apply indent
                if (child.transform.childCount == 0) //If object has no children
                {
                    //Set expand properties here
                    entry.hierarchyReferences.expand.enabled = false; //Set expanded image to false
                }
                entry.hierarchyReferences.button.onClick.RemoveAllListeners(); //Reset button listener
                entry.hierarchyReferences.button.onClick.AddListener(delegate { this.SelectHierarchyEntry(child, entry.hierarchyReferences.button.image); }); //Set button listener
                currentSpawnPosition -= entryHeight; //Calulcate position for next entry spawn
                CrawlObject(indentLevel, child); //Crawl this entries children
            }
        }

        //Completed crawling scenes hierarchy
        void CompleteSceneHierarchy()
        {
            currentSceneIndex++; //Increment scene index
            GetNextScene(); //Work on next scene
        }

        //Selects GameObject for this entry (called by Listener on Button)
        public void SelectHierarchyEntry(GameObject input, Image entryImage)
        {
            int hierarchyChildCount = contentRect.transform.childCount; //Get count of all hierarchy items
            for (int i = 0; i < hierarchyChildCount; i++) //Iterate hierarchy items
            {
                HierarchyEntry entry = contentRect.GetChild(i).GetComponent<HierarchyEntry>(); //Get reference to HierarchyEntry.cs
                entry.hierarchyReferences.button.image.color = hierarchyGraphicsDefault.entryColor; //Reset button color to default
            }
            entryImage.color = hierarchyGraphicsSelected.entryColor; //Set this buttons color to selected
            if (inspector != null) //If reference to Inspector exists
            {
                Debug.Log("Set Inspector to GameObject: " + input.ToString());
                inspector.ChangeInspectorContext(input); //Change context on Inspector.cs to match this selected gameobject
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
