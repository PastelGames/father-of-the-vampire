using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuspicionBar : MonoBehaviour {

    [SerializeField] Image[] barSegments;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < barSegments.Length; i++)
        {
            if (i < CoreGame.suspicionLevelInteger)
            {
                barSegments[i].enabled = true;
            }
            else
            {
                barSegments[i].enabled = false;
            }
        }
    }
}
