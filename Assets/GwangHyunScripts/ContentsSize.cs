using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentsSize : MonoBehaviour
{
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    void Start()
    {
        SetContentsSize();
    }

    void Update()
    {
        
    }
    void SetContentsSize()
    {
        float height = 0;
        for(int i = 0; i < transform.childCount; i++)
        {
            height += 320;
        }

        rect.sizeDelta = new Vector2(1080, height);
    }
}
