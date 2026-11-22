
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class CharacterSelectionManager : MonoBehaviour
{
    public Image characterFoto;
    public TMP_Text characterNombre;
    public Sprite[] characterSprites;
    public string[] characterNames;
    public GameObject characterSelectPanel;
    public GameObject stageSelectPanel;
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public AudioMixer audioMixer;


    public void ConfirmSelection()
    {
        characterSelectPanel.SetActive(false);
        stageSelectPanel.SetActive(true);
    }
    public void BackToMainMenu()
    {
        characterSelectPanel.SetActive(false);
        stageSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void SelectCharacter1()
    {
        SelectCharacter(0);
    }

    public void SelectCharacter2()
    {
        SelectCharacter(1);
    }

    public void SelectCharacter3()
    {
        SelectCharacter(2);
    }

    public void SelectCharacter4()
    {
        SelectCharacter(3);
    }

    void SelectCharacter(int index)
    {
        characterFoto.sprite = characterSprites[index];
        characterNombre.text = characterNames[index];
    }
    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
    }
    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume",
            volume > 0.0001f ? Mathf.Log10(volume) * 20 : -80f);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume",
            volume > 0.0001f ? Mathf.Log10(volume) * 20 : -80f);
    }
}
