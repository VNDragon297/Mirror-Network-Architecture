using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoardUI : MonoBehaviour
{
    public List<PlayerScoreUI> scoreList;

    private void Awake()
    {
        DisableUI();
    }

    private void OnEnable()
    {
        UpdateUIDetails();
    }

    private void UpdateUIDetails()
    {
        foreach(var scoreCard in scoreList)
        {

        }
    }

    public void DisableUI() => this.enabled = false;
}
