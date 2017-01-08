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
        public List<HierarchyEntry> sceneEntries = new List<HierarchyEntry>(); //List of entries for scenes
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
                sceneEntries.Add(entry); //Add entry to scene entries list
                entries.Add(entry); //Add to entry list to track
                entry.hierarchyReferences.label.text = spawn.name; //Set label text
                //entry.hierarchyReferences.contentRect.anchoredPosition = new Vector2(indentLevel * indentSize, 0); //Apply indent

                GameObject[] objectsFound = scenes[currentSceneIndex].GetRootGameObjects(); //Get all root objects of scene
                if (objectsFound.Length == 0) //If object has no children
                {
                    entry.isCollapsed = false; //No children means cant be collapsed
                    entry.hierarchyReferences.expand.image.enabled = false; //Set expanded button to false
                }
                //entry.hierarchyReferences.button.onClick.RemoveAllListeners(); //Reset button listener
                //entry.hierarchyReferences.button.onClick.AddListener(delegate { SelectHierarchyEntry(go, entry); }); //Set button listener
                entry.hierarchyReferences.expand.onClick.RemoveAllListeners(); //Reset expand button listener
                entry.hierarchyReferences.expand.onClick.AddListener(delegate { this.ExpandCollapseHierarchy(entry); }); //Set expand button listener

                currentSpawnPosition -= entryHeight; //Calulcate position for next entry spawn
                CrawlSceneHierarchy(indentLevel, entry); //Crawl scene hierarchy
            }
            else //Finished crawling scenes
            {
                contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, GetVisibleItemCount() * entryHeight); //Set contentRect size to fit
                Debug.Log("Completed Hierarchy Generation");
            }
        }

        //Crawl root objects of scene
        void CrawlSceneHierarchy(float indentLevel, HierarchyEntry scene)
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
                    spawn.transform.SetParent(scene.transform); //Parent
                    HierarchyEntry entry = spawn.GetComponent<HierarchyEntry>(); //Get reference to HierarchyEntry.cs
                    entry.isCollapsed = true; //Mark as collapsed
                    entry.hierarchyReferences.expand.image.sprite = hierarchyGraphicsDefault.expandIcon; //Set expanded icon
                    entry.parent = scene; //Set parent
                    entries.Add(entry); //Add to entry list to track
                    scene.children.Add(entry); //Add to parents child list
                    entry.hierarchyReferences.label.text = spawn.name; //Set label text
                    entry.hierarchyReferences.contentRect.anchoredPosition = new Vector2(indentLevel * indentSize, 0); //Apply indent
                    if (go.transform.childCount == 0) //If object has no children
                    {
                        entry.isCollapsed = false; //No children means cant be collapsed
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
            for (int i = 0; i < childCount; i++) //Iterate children
            {
                childrenFound[i] = inputGameObject.transform.GetChild(i).gameObject; //Get references to children
            }
            foreach (GameObject child in childrenFound) //Iterate array of children
            {
                GameObject spawn = Instantiate(hierarchyEntryPrefab, parent.transform, false) as GameObject; //Spawn a hierarchy entry
                //spawn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, currentSpawnPosition); //Set entries position
                spawn.name = child.name; //Set object name
                HierarchyEntry entry = spawn.GetComponent<HierarchyEntry>(); //Get reference to HierarchyEntry.cs
                spawn.SetActive(false); //Disable 
                entry.isCollapsed = true; //Mark as collapsed
                entry.hierarchyReferences.expand.image.sprite = hierarchyGraphicsDefault.expandIcon; //Set expanded icon
                entry.isHidden = true; //Mark as hidden
                entry.parent = parent; //Set parent
                entries.Add(entry); //Add to entry list to track
                parent.children.Add(entry); //Add to parents child list
                entry.hierarchyReferences.label.text = spawn.name; //Set label text
                entry.hierarchyReferences.contentRect.anchoredPosition = new Vector2(indentLevel * indentSize, 0); //Apply indent
                if (child.transform.childCount == 0) //If object has no children
                {
                    entry.isCollapsed = false; //No children means cant be collapsed
                    entry.hierarchyReferences.expand.image.enabled = false; //Set expanded button to false
                }
                entry.hierarchyReferences.button.onClick.RemoveAllListeners(); //Reset object button listener
                entry.hierarchyReferences.button.onClick.AddListener(delegate { this.SelectHierarchyEntry(child, entry); }); //Set object button listener
                entry.hierarchyReferences.expand.onClick.RemoveAllListeners(); //Reset expand button listener
                entry.hierarchyReferences.expand.onClick.AddListener(delegate { this.ExpandCollapseHierarchy(entry); }); //Set expand button listener
                //currentSpawnPosition -= entryHeight; //Calulcate position for next entry spawn
                CrawlObject(indentLevel, child, entry); //Crawl this entries children
            }
        }

        //Completed crawling scenes hierarchy
        void CompleteSceneHierarchy()
        {
            currentSceneIndex++; //Increment scene index
            GetNextScene(); //Work on next scene
        }

        //Calculate height of all entries for adjusting content rect
        public int GetVisibleItemCount()
        {
            int count = 0; //Integer for tracking items found
            for(int i = 0; i < sceneEntries.Count; i++)
            {
                count++; //Add one to the count for the scene itself
                if(!sceneEntries[i].isCollapsed) //If the scene isnt collapsed
                    count += RecursiveDown(sceneEntries[i], HierarchySearchType.Search); //Recursive search down the scenes hierarchy
            }
            return count; //return
        }

        //Expands or collapses hierarchy from entry
        public void ExpandCollapseHierarchy(HierarchyEntry inputEntry)
        {
            inputEntry.isCollapsed = !inputEntry.isCollapsed; //Flip collapsed state
            int itemCount; //Integer for counting expanded/collapsed items
            switch (inputEntry.isCollapsed)
            {
                case true:
                    inputEntry.hierarchyReferences.expand.image.sprite = hierarchyGraphicsDefault.expandIcon; //Set expanded icon
                    itemCount = RecursiveDown(inputEntry, HierarchySearchType.Collapse); //Search down children recursively to find what to enable/disable and get nudge count
                    RecursiveUp(inputEntry, itemCount, HierarchySearchType.Collapse); //Search up hierarchy for which items come after this that need to be nudged
                    break;
                case false:
                    inputEntry.hierarchyReferences.expand.image.sprite = hierarchyGraphicsSelected.expandIcon; //Set expanded icon
                    itemCount = RecursiveDown(inputEntry, HierarchySearchType.Expand); //Search down children recursively to find what to enable/disable and get nudge count
                    RecursiveUp(inputEntry, itemCount, HierarchySearchType.Expand); //Search up hierarchy for which items come after this that need to be nudged
                    break;
            }
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, GetVisibleItemCount() * entryHeight); //Set contentRect size to fit
        }

        //Called when collapsing, go down hierarchy from selected to collapse items and calculate hierarchy nudge count
        public int RecursiveDown(HierarchyEntry inputEntry, HierarchySearchType searchType)
        {
            int count = 0; //Integer for tracking children found
            for (int i = 0; i < inputEntry.children.Count; i++) //Iterate children
            {
                switch(searchType)
                {
                    case HierarchySearchType.Collapse:
                        inputEntry.children[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //Set position
                        inputEntry.children[i].isHidden = true; //Set hidden to check against
                        inputEntry.children[i].gameObject.SetActive(false); //Set game object disabled
                        break;
                    case HierarchySearchType.Expand:
                        inputEntry.children[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -((1 + count) * entryHeight)); //Set position
                        inputEntry.children[i].isHidden = false; //Set hidden to check against
                        inputEntry.children[i].gameObject.SetActive(true); //Set game object disabled
                        break;
                }
                count++; //Add to collapsed item count;
                if (!inputEntry.children[i].isCollapsed) //If not collapsed keep going down hierarchy
                    count += RecursiveDown(inputEntry.children[i], searchType); //Expand down hierarchy
            }
            return count; //Return
        }

        //Called when collapsing, go up hierarchy from selected to find objects that need to be nudged
        public void RecursiveUp(HierarchyEntry inputEntry, int nudgeCount, HierarchySearchType searchType)
        {
            if (inputEntry.parent) //Check not top of hierarchy
            {
                bool isSelected = false; //Flipped when expanded item is found
                for (int i = 0; i < inputEntry.parent.children.Count; i++) //Iterate siblings
                {
                    if (inputEntry.parent.children[i] == inputEntry) //If currently expanded item
                    {
                        isSelected = true; //Found expanded item
                        continue;
                    }
                    else if (isSelected) //Following items
                    {
                        RectTransform rect = inputEntry.parent.children[i].GetComponent<RectTransform>(); //Get reference to rect transfrom
                        Vector2 oldPos = rect.anchoredPosition; //get current position
                        switch (searchType)
                        {
                            case HierarchySearchType.Collapse:
                                rect.anchoredPosition = new Vector2(0, oldPos.y + nudgeCount * entryHeight); //set nudged position
                                break;
                            case HierarchySearchType.Expand:
                                rect.anchoredPosition = new Vector2(0, oldPos.y - nudgeCount * entryHeight); //set nudged position
                                break;
                        }
                    }
                }
                RecursiveUp(inputEntry.parent, nudgeCount, searchType); //Continue up hierarchy
            }
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
}
