using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CoreGame : MonoBehaviour
{
    public enum SuspicionLevel { LESS_SUSPICIOUS, MORE_SUSPICIOUS, MOST_SUSPICIOUS }

    [SerializeField] Text textComponent;
    [SerializeField] Text userFeedbackText;
    [SerializeField] State startingState;
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] Transform contentPanel;
    [SerializeField] Transform userFeedbackPanel;
    State currentState;
    public static int currentBloodValue;
    public int currentSuspicionValue;
    [SerializeField] Text currentBloodValueTextComponent;
    [SerializeField] Text currentSuspicionValueTextComponent;
    [SerializeField] const int MAX_SUSPICION_VALUE = 10;
    [SerializeField] const int MIN_SUSPICION_VALUE = 0;
    [SerializeField] public const int MAX_BLOOD_VALUE = 5;
    private bool suspicionMaxReached = false;
    HashSet<State> visitedStates = new HashSet<State>();
    GameObject bloodGatheringButton;
    [SerializeField] float timeToDisplayFeedback = 3;
    Character currentCharacter;
    [SerializeField] Image currentCharacterImage;
    [SerializeField] Image backgroundImage;
    public static int suspicionLevelInteger;
    public SuspicionLevel suspicionLevel;
    [SerializeField] Text questionText;
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] AudioClip mainSong;
    [SerializeField] AudioClip suspiciousSong;
    [SerializeField] AudioClip gameOverSong;
    [SerializeField] AudioClip winSong;
    [SerializeField] AudioSource BGM;
    [SerializeField] Button endGameButton;

    public void DestroyButtons()
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        currentState = startingState;
        currentCharacter = currentState.GetCharacter();
        ResetPanel();
        SetBGM(mainSong);
        endGameButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPanel()
    {

        DestroyButtons();
        StopAllCoroutines();
        StartCoroutine(TypeWriteText(currentState.GetInitialStateText(), textComponent, .005f));
        StartCoroutine(TypeWriteText(currentState.GetQuestionText(), questionText, .05f));
        AddButtons();
        foreach (Transform child in userFeedbackPanel)
        {
            Destroy(child.gameObject);
        }
        if (!HasBeenVisited(currentState))
        {
            UpdateCurrentBloodValue(currentState.GetBloodValue());
            UpdateCurrentSuspicionValue(currentState.GetSuspicionValue());
        }
        currentCharacter = currentState.GetCharacter();
        currentCharacterImage.sprite = currentCharacter.GetCharacterImage(suspicionLevel);
        backgroundImage.sprite = currentState.GetScenarioImage();
        if (currentState.GetNextStates().Count == 0)
        {
            StartCoroutine(Win());
        }
        
    }

    //a method that adds all of the necessary buttons to the current state
    public void AddButtons()
    {
        GameObject newButton;
        OptionStateDictionary nextStates = currentState.GetNextStates();
        //for every state, create a button with the corresponding option text
        int buttonNumber = 1;
        foreach (KeyValuePair<string, State> pair in nextStates)
        {
            newButton = Instantiate(buttonPrefab, contentPanel.transform);
            MoveButton(newButton, buttonNumber);
            newButton.GetComponentInChildren<Text>().text = pair.Key;
            State stateToLoad = pair.Value;
            newButton.GetComponent<Button>().onClick.AddListener(() => { GoToNextState(stateToLoad); });
            buttonNumber++;
        }
        //if you can gather blood then make a bloodgathering button
        if (currentState.GetBloodGatheringState() && !BloodGatheredFromState(currentState)) {
           newButton  = Instantiate(buttonPrefab, contentPanel.transform);
            MoveButton(newButton, buttonNumber);
            newButton.GetComponentInChildren<Text>().text = currentState.GetBloodGatheringButtonMessage();
            newButton.GetComponent<Button>().onClick.AddListener(() => { OnBloodGathering(); });
            bloodGatheringButton = newButton;
        }
        
    }

    private static void MoveButton(GameObject newButton, int buttonNumber)
    {
        switch (buttonNumber)
        {
            case 2:
                newButton.transform.localPosition = new Vector3(newButton.transform.localPosition.x * -1, newButton.transform.localPosition.y);
                break;
            case 3:
                newButton.transform.localPosition = new Vector3(newButton.transform.localPosition.x, newButton.transform.localPosition.y * -1);
                break;
            case 4:
                newButton.transform.localPosition = new Vector3(newButton.transform.localPosition.x * -1, newButton.transform.localPosition.y * -1);
                break;
        }
    }

    public void GoToNextState(State nextState)
    {
        MarkAsVisited(currentState);
        currentState = nextState;
        ResetPanel();
        
    }

    //this method increases the current blood value based on the current states blood value
    public void UpdateCurrentBloodValue(int someBloodValue)
    {
        currentBloodValue += someBloodValue;
        currentBloodValueTextComponent.text = "Blood Value: " + currentBloodValue;
        if (someBloodValue > 0 && currentBloodValue < MAX_BLOOD_VALUE)
        {
            GiveUserFeedback("blood acquired.", Color.white);
        } else if (currentBloodValue < 0)
        {
            currentBloodValue = 0;
        } else if (currentBloodValue > MAX_BLOOD_VALUE)
        {
            currentBloodValue = MAX_BLOOD_VALUE;
        }
    }

    //This method updates the current suspicion value
    public void UpdateCurrentSuspicionValue(int someSuspicionValue)
    {

        if (someSuspicionValue < 0) {
            GiveUserFeedback("Your suspicion has decreased.", Color.white);
        } else if (someSuspicionValue > 0) {
            GiveUserFeedback("Your suspicion has increased.", Color.red);
        }

        //minimum threshold for suspicion value
        if (currentSuspicionValue + someSuspicionValue < MIN_SUSPICION_VALUE)
        {
            currentSuspicionValue = MIN_SUSPICION_VALUE;
            //if the suspicion value becomes greater than 10 then make it 10 at max
        }
        else if (currentSuspicionValue + someSuspicionValue >= MAX_SUSPICION_VALUE)
        {
            Debug.Log(currentSuspicionValue + someSuspicionValue);
            currentSuspicionValue = MAX_SUSPICION_VALUE;
            suspicionMaxReached = true;
            Debug.Log(suspicionMaxReached);
            if (suspicionMaxReached)
            {
                StopAllCoroutines();
                StartCoroutine(Lose());
                DestroyButtons();
            }
        }
        else
        {
            currentSuspicionValue += someSuspicionValue;
        }
        currentSuspicionValueTextComponent.text = "Suspicion Value: " + currentSuspicionValue;

        float currentSuspicionRatio = (float)currentSuspicionValue / (float)MAX_SUSPICION_VALUE;

        if (currentSuspicionRatio < 0.3f)
        {
            suspicionLevel = SuspicionLevel.LESS_SUSPICIOUS;
        }
        else if (currentSuspicionRatio >= 0.3f && currentSuspicionRatio < 0.8f)
        {
            suspicionLevel = SuspicionLevel.MORE_SUSPICIOUS;
        } else if (currentSuspicionRatio >= 0.8f)
        {
            suspicionLevel = SuspicionLevel.MOST_SUSPICIOUS;
        }

        if (currentSuspicionRatio < 0.2f)
        {
            suspicionLevelInteger = 1;
        }
        else if (currentSuspicionRatio >= 0.2f && currentSuspicionRatio < 0.4f)
        {
            suspicionLevelInteger = 2;
        }
        else if (currentSuspicionRatio >= 0.4f && currentSuspicionRatio < 0.6f)
        {
            suspicionLevelInteger = 3;
        }
        else if (currentSuspicionRatio >= 0.6f && currentSuspicionRatio < 0.8f)
        {
            suspicionLevelInteger = 4;
        }
        else if (currentSuspicionRatio >= 0.8f)
        {
            suspicionLevelInteger = 5;
        }

        if (currentSuspicionRatio >= 0.6f)
        {
            SetBGM(suspiciousSong);
        } else
        {
            SetBGM(mainSong);
        }

    }

    //returns whether or not this state has been visited previously
    public bool HasBeenVisited(State someState)
    {
        return visitedStates.Contains(someState);
    }

    //marks the argument state as visited
    public void MarkAsVisited(State someState)
    {
        visitedStates.Add(someState);
    }

    public void OnBloodGathering() {
        UpdateCurrentBloodValue(1);
        Destroy(bloodGatheringButton);
        MarkAsBloodGathered(currentState);
    }

    HashSet<State> bloodGatheredStates = new HashSet<State>();
    //returns a boolean that lets you know whether the blood has been gathered
    //from this state or not.
    public bool BloodGatheredFromState(State someState) {
        return bloodGatheredStates.Contains(someState);
    }

    //marks the current state as blood gathered
    public void MarkAsBloodGathered(State someState) {
        bloodGatheredStates.Add(someState);
    }

    public void GiveUserFeedback(string userFeedback, Color feedbackColor) {
        Text newUserFeedbackText = 
            Instantiate<Text>(userFeedbackText, 
                              userFeedbackPanel.transform);
        newUserFeedbackText.text = userFeedback;
        newUserFeedbackText.color = feedbackColor;
        newUserFeedbackText.fontSize = 15;
        newUserFeedbackText.alignment = TextAnchor.MiddleCenter;
        Destroy(newUserFeedbackText.gameObject, timeToDisplayFeedback);
    }
    
    IEnumerator TypeWriteText(string textToTypeWrite, Text elementToEdit, float delay)
    {
        elementToEdit.text = "";
        foreach(char letter in textToTypeWrite.ToCharArray())
        {
            elementToEdit.text += letter;
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator TypeWriteText(string textToTypeWrite, Text elementToEdit, float delay, Color textColor)
    {
        elementToEdit.text = "";
        elementToEdit.color = textColor;
        foreach (char letter in textToTypeWrite.ToCharArray())
        {
            elementToEdit.text += letter;
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator Win()
    {
        SetBGM(winSong);
        Text winText = Instantiate<Text>(userFeedbackText, FindObjectOfType<Canvas>().transform);
        winText.transform.localPosition = new Vector3(0, 0, 0);
        winText.fontSize = 30;
        winText.alignment = TextAnchor.MiddleCenter;
        string winPrompt = "God Bless America!\nYou're a winner!";
        StartCoroutine(TypeWriteText(winPrompt, winText, 0.2f));
        yield return new WaitForSeconds((0.2f * winPrompt.Length) + 5);
        Debug.Log("Text sussessfully displayed");
        string winStats = "You collected " + currentBloodValue + " / " + MAX_BLOOD_VALUE +  " blood.";
        if (currentBloodValue == MAX_BLOOD_VALUE) {
            winStats += "Vampirically Impressive!"; 
        }
        StartCoroutine(TypeWriteText(winStats, winText, .1f, Color.red));
        yield return new WaitForSeconds(.1f * winStats.Length + 5);
        sceneLoader.LoadNextScene();
        
    }

    public void SetBGM(AudioClip music)
    {
        if (music != BGM.clip)
        {
            BGM.Stop();
            BGM.clip = music;
            BGM.Play();
            Debug.Log(BGM.isPlaying);
        }
    }

    public void toggleMusic ()
    {
        BGM.mute = !BGM.mute;
    }

    IEnumerator Lose()
    {
        string loseSentence = "This man is far from holy! EXECUTE HIM!";
        StartCoroutine(TypeWriteText(loseSentence, textComponent, 0.1f));
        yield return new WaitForSeconds(0.1f * loseSentence.Length + 4);
        sceneLoader.LoadNextScene();
    }

}
