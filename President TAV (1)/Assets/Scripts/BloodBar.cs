using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodBar : MonoBehaviour {

    [SerializeField] Image[] bloodImages;
    [SerializeField] Sprite emptyBloodSprite;
    [SerializeField] Sprite fullBloodSprite;
    SpriteRenderer dropRenderer;
   
	// Use this for initialization
	void Start () {
        dropRenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < bloodImages.Length; i++)
        {
            if (i < CoreGame.currentBloodValue)
            {
                bloodImages[i].color = Color.red;
            } else
            {
                bloodImages[i].sprite = emptyBloodSprite;
            }
        }
	}

}
