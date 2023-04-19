using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCard : MonoBehaviour
{
    


    [SerializeField] private SceneController sceneController;
    [SerializeField] private GameObject cardBack;

    private int _id;

    public int Id
    {
        get { return _id; }
    }

    public void OnMouseDown() 
    {
        if (cardBack.activeSelf && sceneController.canReveal) 
        {
            cardBack.SetActive(false);
            sceneController.CardRevealed(this);
        }
    }

    public void Unreveal() 
    {
        cardBack.SetActive(true);
    }

    public bool Revealed()
    {
        return !cardBack.activeSelf;
    }

    public void ChangeSprite(int id, Sprite image)
    {
        _id = id;
        GetComponent<SpriteRenderer>().sprite = image;
    }

    void Start() {

    }

    void Update() {

    }
}
