using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.RuntimeEditor
{
    public class Window : MonoBehaviour
    {
        public List<WindowReferences> dockedWindows = new List<WindowReferences>();
        int activeWindowIndex;
        public GameObject expandButton;
        public GameObject collapseButton;

        void Start()
        {
            GetDockedWindows();
        }

        void GetDockedWindows()
        {
            for (int i = 0; i < dockedWindows.Count; i++)
            {
                dockedWindows[i].name = dockedWindows[i].content.name;
                dockedWindows[i].content.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                if (dockedWindows[i].activeTab.enabled)
                {
                    activeWindowIndex = i;
                    dockedWindows[activeWindowIndex].content.transform.SetSiblingIndex(9999);
                    dockedWindows[activeWindowIndex].activeTab.enabled = true;
                }
                else
                {
                    dockedWindows[i].activeTab.enabled = false;
                }
            }
        }

        public void SetActiveTab(GameObject selectedTab)
        {
            for (int i = 0; i < dockedWindows.Count; i++)
            {
                if(dockedWindows[i].tab == selectedTab)
                {
                    activeWindowIndex = i;
                    dockedWindows[activeWindowIndex].content.transform.SetSiblingIndex(9999);
                    dockedWindows[activeWindowIndex].activeTab.GetComponent<Image>().enabled = true;
                }
                else
                {
                    dockedWindows[i].activeTab.enabled = false;
                }
            }
        }

        public void ExpandCollapse(bool io)
        {
            if(io)
            {
                expandButton.SetActive(true);
                collapseButton.SetActive(false);
                RectTransform rect = GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(-rect.sizeDelta.x, -18);
            }
            else
            {
                expandButton.SetActive(false);
                collapseButton.SetActive(true);
                RectTransform rect = GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0, -18);
            }
        }
    }
}
