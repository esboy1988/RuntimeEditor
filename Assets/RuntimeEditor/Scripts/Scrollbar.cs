using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.RuntimeEditor
{
    public class Scrollbar : MonoBehaviour
    {
        UI.Scrollbar scrollbar;
        public float buttonStrength = 1.0f;

        // Use this for initialization
        void Start()
        {
            scrollbar = GetComponent<UI.Scrollbar>();
        }

        public void MoveUp()
        {
            scrollbar.value = scrollbar.value + 0.1f * buttonStrength;
        }

        public void MoveDown()
        {
            scrollbar.value = scrollbar.value - 0.1f * buttonStrength;
        }
    }
}
