using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PermanentUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cherryCountText;                   // Text with the current cherry count.
    [SerializeField] private TextMeshProUGUI gemCountText;                      // Text with the current gem count.

    private int cherryCount = 0;
    private int gemCount = 0;

    public static PermanentUI shared;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Singleton pattern
        if (!shared) shared = this;
        else Destroy(gameObject);
    }

    public void IncreaseGemCount(int numGems)
    {
        gemCount += numGems;
        gemCountText.text = gemCount.ToString();
    }

    public void IncreaseCherryCount(int numCherries)
    {
        cherryCount += numCherries;
        cherryCountText.text = cherryCount.ToString();
    }

}
