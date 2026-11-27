
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageSelectionManager : MonoBehaviour
{
    public Image stagePreview;
    public TMP_Text stageName;

    public Sprite[] stageSprites;
    public string[] stageNames;

    public GameObject stageSelectPanel;
    public GameObject characterSelectPanel;

    public void SelectStage1()
    {
        SelectStage(0);
    }

    public void SelectStage2()
    {
        SelectStage(1);
    }

    public void SelectStage3()
    {
        SelectStage(2);
    }

   

    void SelectStage(int index)
    {
        stageName.text = stageNames[index];
        stageName.enabled = true;

        if (stageSprites[index] != null)
        {
            stagePreview.sprite = stageSprites[index];
            stagePreview.enabled = true;
        }
    }

    public void BackToCharacterSelect()
    {
        stageSelectPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
    }
    public void ConfirmStage()
    {
        SceneManager.LoadScene("SampleScene");
    }
    void Start()
    {
        stagePreview.enabled = false;
        stageName.enabled = false;
    }
}
