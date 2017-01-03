using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.RuntimeEditor
{
    public class ResourcesEntry : MonoBehaviour
    {
        public ResourcesReferences resourcesReferences;
    }

    [System.Serializable]
    public class ResourcesReferences
    {
        public RectTransform contentRect;
        public Text label;
        public Button button;
    }
}
