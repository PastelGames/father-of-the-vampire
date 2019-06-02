using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RotaryHeart.Lib.SerializableDictionary;

[System.Serializable] public class OptionStateDictionary : SerializableDictionaryBase<string, State> { }

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject {

    [TextArea(14, 10)] [SerializeField] private string initialStateText;
    [SerializeField] private int suspicionValue;
    [SerializeField] bool bloodGatheringState;
    [SerializeField] int bloodValue;
    [SerializeField] string bloodGatheringButtonMessage;
    [SerializeField] OptionStateDictionary nextStates;
    [SerializeField] Sprite scenarioImage;
    [SerializeField] string questionText;
    [SerializeField] Character character;

    public string GetInitialStateText() {
        return initialStateText;
    }

    public int GetBloodValue() {
        return bloodValue;
    }

    public int GetSuspicionValue() {
        return suspicionValue;
    }

    public bool GetBloodGatheringState() {
        return bloodGatheringState;
    }

    public string GetBloodGatheringButtonMessage() {
        return bloodGatheringButtonMessage;
    }

    public OptionStateDictionary GetNextStates() {
        return nextStates;
    }

    public Sprite GetScenarioImage() {
        return scenarioImage;
    }

    public Character GetCharacter()
    {
        return character;
    }

    public string GetQuestionText()
    {
        return questionText;
    }

}
