using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.RuntimeEditor
{
    public class HierarchyEntry : MonoBehaviour
    {
        public HierarchyEntryReferences hierarchyReferences; //Data structure for hierarchy entry references
        public HierarchyEntry parent; //Reference to this entries parent
        public List<HierarchyEntry> children = new List<HierarchyEntry>(); //List of references to this entries children
        public bool isCollapsed = false; //Is this entry currently collapsed
        public bool isHidden = false; //Is this entry currently visible/enabled
    }
}
