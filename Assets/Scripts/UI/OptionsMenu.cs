using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioController audioController;
    public Slider musicVolumeSlider;
    public Slider soundVolumeSlider;

    public GameObject normalMenu;
    public GameObject confirmReturnHomeMenu;

    public bool returnFromBuilder;

    public GameObject battleReturnConfirmText;
    public GameObject builderReturnConfirmText;

    private void Awake()
    {
        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.2f);
        musicVolumeSlider.onValueChanged.AddListener(delegate { audioController.ChangeMusicVolume(musicVolumeSlider); });
        soundVolumeSlider.value = PlayerPrefs.GetFloat("soundSliderValue", 0.5f);
        soundVolumeSlider.onValueChanged.AddListener(delegate { audioController.ChangeSoundVolume(soundVolumeSlider); });
    }

    public void SetBuilderMode() { returnFromBuilder = true; }
    public void SetBattleMode() { returnFromBuilder = false; }

    private void OnEnable()
    {
        if (normalMenu != null) normalMenu.SetActive(true);
        if (confirmReturnHomeMenu != null) confirmReturnHomeMenu.SetActive(false);
        if (battleReturnConfirmText != null && builderReturnConfirmText != null)
        {
            if (returnFromBuilder)
            {
                builderReturnConfirmText.SetActive(true);
                battleReturnConfirmText.SetActive(false);
            }
            else
            {
                builderReturnConfirmText.SetActive(false);
                battleReturnConfirmText.SetActive(true);
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
