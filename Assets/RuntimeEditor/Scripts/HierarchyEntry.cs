using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.RuntimeEditor
{
    public class HierarchyEntry : MonoBehaviour
    {
        public HierarchyEntryReferences hierarchyReferences;
        public List<HierarchyEntry> children = new List<HierarchyEntry>();
        public bool isCollapsed = false;
        public bool isHidden = false;
    }
}
