using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Character")]
public class Character : ScriptableObject {

    [SerializeField] Sprite level1SuspicionPhoto;
    [SerializeField] Sprite level2SuspicionPhoto;
    [SerializeField] Sprite level3SuspicionPhoto;

    public Sprite GetCharacterImage(CoreGame.SuspicionLevel suspicion)
    {
        switch(suspicion)
        {
            case CoreGame.SuspicionLevel.LESS_SUSPICIOUS:
                return level1SuspicionPhoto;
            case CoreGame.SuspicionLevel.MORE_SUSPICIOUS:
                return level2SuspicionPhoto;
            case CoreGame.SuspicionLevel.MOST_SUSPICIOUS:
                return level3SuspicionPhoto;
        }
        return null;
    }
    
}