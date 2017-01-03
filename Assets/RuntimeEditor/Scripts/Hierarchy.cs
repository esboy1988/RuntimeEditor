using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UnityEngine.RuntimeEditor
{
    public class Hierarchy : MonoBehaviour
    {
        public List<Scene> scenes = new List<Scene>();
        public GameObject hierarchyScenePrefab;
        public GameObject hierarchyEntryPrefab;
        public RectTransform contentRect;
        public HierarchyGraphics hierarchyGraphicsDefault;
        public HierarchyGraphics hierarchyGraphicsSelected;
        public float indentSize = 14f;
        Inspector inspector;
        int sceneCount;
        int currentSceneIndex;
        float currentSpawnPosition;
        float entryHeight = 18f;
        

        // Use this for initialization
        void Start()
        {
            sceneCount = SceneManager.sceneCount;
            inspector = GameObject.Find("Inspector").GetComponent<Inspector>(); //dirty
            GetNextScene();
        }

        void GetNextScene()
        {
            if (currentSceneIndex < sceneCount)
            {
                scenes.Add(SceneManager.GetSceneAt(currentSceneIndex));
                GameObject spawn = Instantiate(hierarchyScenePrefab, contentRect.transform, false) as GameObject;
                spawn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, currentSpawnPosition);
                spawn.name = SceneManager.GetSceneAt(currentSceneIndex).name;
                float indentLevel = 0;
                HierarchyEntry entry = spawn.GetComponent<HierarchyEntry>();
                entry.hierarchyReferences.label.text = spawn.name;
                entry.hierarchyReferences.contentRect.anchoredPosition = new Vector2(indentLevel * indentSize, 0);
                currentSpawnPosition -= entryHeight;
                CrawlSceneHierarchy(indentLevel);
            }
            else
                Debug.LogError("Completed Hierarchy Generation");
        }

        void CrawlSceneHierarchy(float indentLevel)
        {
            indentLevel++;
            GameObject[] objectsFound = scenes[currentSceneIndex].GetRootGameObjects();
            foreach (GameObject go in objectsFound)
            {
                if (go.name != "RuntimeUICanvas")
                {
                    GameObject spawn = Instantiate(hierarchyEntryPrefab, contentRect.transform, false) as GameObject;
                    spawn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, currentSpawnPosition);
                    spawn.name = go.name;
                    HierarchyEntry entry = spawn.GetComponent<HierarchyEntry>();
                    entry.hierarchyReferences.label.text = spawn.name;
                    entry.hierarchyReferences.contentRect.anchoredPosition = new Vector2(indentLevel * indentSize, 0);
                    if (go.transform.childCount == 0)
                        entry.hierarchyReferences.expand.enabled = false;
                    entry.hierarchyReferences.button.onClick.RemoveAllListeners();
                    entry.hierarchyReferences.button.onClick.AddListener(delegate { SelectHierarchyEntry(go, entry.hierarchyReferences.button.image); });
                    currentSpawnPosition -= entryHeight;
                    CrawlObject(indentLevel, go);
                }
            }
            CompleteSceneHierarchy();
        }

        void CrawlObject(float indentLevel, GameObject inputGameObject)
        {
            indentLevel++;
            int childCount = inputGameObject.transform.childCount;
            GameObject[] childrenFound = new GameObject[childCount];
            for(int i = 0; i < childCount; i++)
            {
                childrenFound[i] = inputGameObject.transform.GetChild(i).gameObject;
            }
            foreach(GameObject child in childrenFound)
            {
                GameObject spawn = Instantiate(hierarchyEntryPrefab, contentRect.transform, false) as GameObject;
                spawn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, currentSpawnPosition);
                spawn.name = child.name;
                HierarchyEntry entry = spawn.GetComponent<HierarchyEntry>();
                entry.hierarchyReferences.label.text = spawn.name;
                entry.hierarchyReferences.contentRect.anchoredPosition = new Vector2(indentLevel * indentSize, 0);
                if (child.transform.childCount == 0)
                    entry.hierarchyReferences.expand.enabled = false;
                entry.hierarchyReferences.button.onClick.AddListener(delegate { this.SelectHierarchyEntry(child, entry.hierarchyReferences.button.image); });
                currentSpawnPosition -= entryHeight;
                CrawlObject(indentLevel, child);
            }
        }

        void CompleteSceneHierarchy()
        {
            currentSceneIndex++;
            GetNextScene();
        }

        public void SelectHierarchyEntry(GameObject input, Image entryImage)
        {
            int hierarchyChildCount = contentRect.transform.childCount;
            for (int i = 0; i < hierarchyChildCount; i++)
            {
                HierarchyEntry entry = contentRect.GetChild(i).GetComponent<HierarchyEntry>();
                entry.hierarchyReferences.button.image.color = hierarchyGraphicsDefault.entryColor;
            }
            entryImage.color = hierarchyGraphicsSelected.entryColor;
            if (inspector != null)
            {
                inspector.ChangeInspectorContext(input);
            }
        }
    }

    [System.Serializable]
    public class HierarchyGraphics
    {
        public Sprite expandIcon;
        public Color entryColor;
        public Color textColor;
    }
}
