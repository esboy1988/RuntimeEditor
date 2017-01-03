using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.RuntimeEditor
{
    public class HierarchyEntry : MonoBehaviour
    {
        public HierarchyReferences hierarchyReferences;
    }

    [System.Serializable]
    public class HierarchyReferences
    {
        public RectTransform contentRect;
        public Text label;
        public Button button;
        public Image expand;
    }
}
