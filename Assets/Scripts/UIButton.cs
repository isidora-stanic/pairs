using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{

    [SerializeField] private GameObject targetObject;
    [SerializeField] private string targetMessage;

    [SerializeField] private Color highlightColor = Color.red;
    // [SerializeField] private Vector3 hoverSize = new Vector3(1f, 1f, 0.5f);
    // [SerializeField] private Vector3 noHoverSize = new Vector3(3f, 3f, 0.5f);

    private SpriteRenderer _sprite;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void OnMouseOver()
    {
        if (_sprite != null)
        {
            _sprite.color = highlightColor;
        }
    }

    private void OnMouseExit()
    {
        if (_sprite != null)
        {
            _sprite.color = Color.white;
        }
    }

    private void OnMouseDown()
    {
        // transform.localScale = hoverSize;
    }

    private void OnMouseUp()
    {
        // transform.localScale = noHoverSize;

        if (targetObject != null)
            targetObject.SendMessage(targetMessage);
    }
}
