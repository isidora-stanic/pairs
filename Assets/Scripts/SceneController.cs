using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public const int gridRows = 4;
    public const int gridCols = 5;
    [Header("Layout")]
    public float offsetX = 3f;
    public float offsetY = 3f;

    [Header("Audio Clips")]
    public AudioClip welcomeClip;
    public AudioClip exitClip;
    public AudioClip resetClip;
    public AudioClip finishedClip;
    public AudioClip bingoClip;
    public AudioClip failClip;

    [Header("Cards")]
    [SerializeField] private MainCard originalCard;
    [SerializeField] private Sprite[] images;
    [SerializeField] private int[] cardNumbers;

    [Header("Labels")]
    [SerializeField] private TextMesh scoreLabel;
    [SerializeField] private TextMesh movesLabel;
    [SerializeField] private TextMesh timerLabel;

    private int _score = 0;
    private int _moves = 0;
    private MainCard _firstRevealedCard;
    private MainCard _secondRevealedCard;
    private MainCard[] _allCards = {null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null};

    private float _startTime;
    private float _elapsedTime;

    private bool _finished = false;

    private static bool hasPlayedWelcomeSound = false;

    public bool canReveal
    {
        get
        {
            return _secondRevealedCard == null;
        }
    }

    private void Awake()
    {
        if (!hasPlayedWelcomeSound)
            StartCoroutine(WaitAndPlayWelcomeSoundCoroutine());
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCardLayout();
        _startTime = Time.time;
        
    }

    void Update()
    {
        if (!_finished) 
        {
            _elapsedTime = Time.time - _startTime;

            int hours = Mathf.FloorToInt(_elapsedTime / 3600f);
            int minutes = Mathf.FloorToInt((_elapsedTime - hours * 3600f) / 60f);
            int seconds = Mathf.FloorToInt(_elapsedTime - hours * 3600f - minutes * 60f);
            
            // update timer display
            timerLabel.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
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

                _allCards[index] = card;
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
        if (_firstRevealedCard.Id == _secondRevealedCard.Id) // pogodjene iste karte
        {
            PlaySound(bingoClip);
            _score++;
            scoreLabel.text = "Score: " + _score;
            // _firstRevealedCard.SetActive(false); // za ovo ti treba referenca i za sliku karte ne samo za poledjinu
            // _secondRevealedCard.SetActive(false);
            _firstRevealedCard = null;
            _secondRevealedCard = null;
        }
        else 
        {   
            PlaySound(failClip, 0.1f);
            yield return new WaitForSeconds(0.5f); // moze da bude posebna promenjiva, pa ne mora da se pravi svaki put novi obj
            _firstRevealedCard.Unreveal();
            _secondRevealedCard.Unreveal();
            _firstRevealedCard = null;
            _secondRevealedCard = null;

        }
        _moves++;
        movesLabel.text = "Moves: " + _moves;

        yield return new WaitForSeconds(1f);
        _finished = CheckIfFinished();
    }

    public bool CheckIfFinished()
    {
        for (int i = 0; i < _allCards.Length; i++)
        {
            if (!_allCards[i].Revealed())
            {
                return false;
            }
        }
        PlaySound(finishedClip);
        return true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        Application.Quit();
    }

    private float PlaySound(AudioClip clip, float volume = 0.3f) 
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.PlayOneShot(clip, volume);
        return clip.length;
    }

    private IEnumerator WaitSoundAndExitCoroutine() 
    {
        float soundLength = PlaySound(exitClip);
        yield return new WaitForSeconds(soundLength);
        // do your action here
        Exit();
    }

    private IEnumerator WaitSoundAndRestartCoroutine()
    {
        float soundLength = PlaySound(resetClip);
        yield return new WaitForSeconds(soundLength);
        // do your action here
        Restart();
    }

    private IEnumerator WaitAndPlayWelcomeSoundCoroutine() 
    {
        yield return new WaitForSeconds(0.5f);
        float soundLength = PlaySound(welcomeClip);
        hasPlayedWelcomeSound = true;
    }
}
