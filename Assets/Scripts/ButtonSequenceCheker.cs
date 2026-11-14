using UnityEngine;
using UnityEngine.UI;

public class ButtonSequenceChecker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Text infoText;

    private int clickedCount = 0;

    public void OnFirstButtonClicked()
    {
        ProcessClick();
    }

    public void OnSecondButtonClicked()
    {
        ProcessClick();
    }

    public void OnThirdButtonClicked()
    {
        ProcessClick();
    }

    private void ProcessClick()
    {
        if (clickedCount >= 3) return;

        clickedCount++;

        if (infoText != null)
        {
            infoText.text = $"Найдено {clickedCount} из трех";
        }

        if (clickedCount == 3)
        {
            Debug.Log("победа");
        }
    }
}