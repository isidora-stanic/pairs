using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public const int gridRows = 4;
    public const int gridCols = 5;
    public const float offsetX = 3f;
    public const float offsetY = 3f;

    [SerializeField] private MainCard originalCard;
    [SerializeField] private Sprite[] images;
    [SerializeField] private TextMesh scoreLabel;
    [SerializeField] private TextMesh movesLabel;
    [SerializeField] private int[] cardNumbers;

    private int _score = 0;
    private int _moves = 0;
    private MainCard _firstRevealedCard;
    private MainCard _secondRevealedCard;

    public bool canReveal
    {
        get
        {
            return _secondRevealedCard == null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCardLayout();
    }

    private void SetCardLayout()
    {
        Vector3 startPosition = originalCard.transform.position;

        // int[] cardNumbers = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4,  };
        cardNumbers = ShuffleCards(cardNumbers);

        MainCard card;

        for (int i = 0; i < gridCols; i++)
        {
            for (int j = 0; j < gridRows; j++)
            {
                if (i == 0 && j == 0)
                {
                    card = originalCard;
                }
                else
                {
                    card = Instantiate(originalCard);
                }

                int index = j * gridCols + i;
                int id = cardNumbers[index];
                card.ChangeSprite(id, images[id]);

                float xPos = startPosition.x + (i * offsetX);
                float yPos = startPosition.y + (j * offsetY);

                card.transform.position = new Vector3(xPos, yPos, startPosition.z);
            }
        }
    }

    private int[] ShuffleCards(int[] cardNumbers)
    {
        int[] shCardNumbers = cardNumbers.Clone() as int[];
        
        for (int i = 0; i < shCardNumbers.Length; i++)
        {
            // get index to swap current card with
            int r = Random.Range(0, shCardNumbers.Length);
            // swap
            int temp = shCardNumbers[i];
            shCardNumbers[i] = shCardNumbers[r];
            shCardNumbers[r] = temp;
        }

        return shCardNumbers;
    }

    public void CardRevealed(MainCard card)
    {
        if (_firstRevealedCard == null)
        {
            _firstRevealedCard = card;
        } 
        else if (_secondRevealedCard == null)
        {
            _secondRevealedCard = card;
            StartCoroutine(CheckCardMatchCoroutine());
        }
    }

    private IEnumerator CheckCardMatchCoroutine()
    {
        if (_firstRevealedCard.Id == _secondRevealedCard.Id)
        {
            _score++;
            scoreLabel.text = "Score: " + _score;
            // _firstRevealedCard.SetActive(false); // za ovo ti treba referenca i za sliku karte ne samo za poledjinu
            // _secondRevealedCard.SetActive(false);
            _firstRevealedCard = null;
            _secondRevealedCard = null;
        }
        else 
        {
            yield return new WaitForSeconds(0.5f); // moze da bude posebna promenjiva, pa ne mora da se pravi svaki put novi obj
            _firstRevealedCard.Unreveal();
            _secondRevealedCard.Unreveal();
            _firstRevealedCard = null;
            _secondRevealedCard = null;
        }
        _moves++;
        movesLabel.text = "Moves: " + _moves;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
