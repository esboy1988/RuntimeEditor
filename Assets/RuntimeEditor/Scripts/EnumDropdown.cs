using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnumDropdown : MonoBehaviour {

    RectTransform rect;
    Dropdown dropdown;
    
	void Start ()
    {
        rect = GetComponent<RectTransform>();
        dropdown = transform.parent.GetComponent<Dropdown>();
	}
	
	void Update ()
    {
        if(rect && dropdown)
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 12 + (dropdown.options.Count * 22));
	}
}
