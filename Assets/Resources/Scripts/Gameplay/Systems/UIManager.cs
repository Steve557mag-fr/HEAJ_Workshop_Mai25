using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{

    [SerializeField] Dictionary<string, GameObject> containers;
    [SerializeField] Slider sliderFX,sliderDialog,sliderAmbience;

    [SerializeField] List<GameObject> userInterfaces;

    private void Start()
    {
        
        foreach(GameObject go in userInterfaces)
        {
            containers.Add(go.name, go);
        }

    }

    internal void SetUI(string name, bool state = false)
    {
        if (containers.ContainsKey(name)) containers[name].SetActive(state);
    }

    public void ShowUI(string name)
    {
        SetUI(name, false);
    }

    public void HideUI(string name)
    {
        SetUI(name, false);
    }

    static internal UIManager Get()
    {
        return FindAnyObjectByType<UIManager>();
    }

    public void UpdateSettingsSliders()
    {
        SoundManager.Get().mixer.GetFloat("FXVolume", out float fx);
        SoundManager.Get().mixer.GetFloat("DialogVolume", out float diag);
        SoundManager.Get().mixer.GetFloat("AmibenceVolume", out float amb);

        sliderFX.value = fx;
        sliderDialog.value = diag;
        sliderAmbience.value = amb;

    }

    public void UpdateSliderVolumeFX(float v)
    {
        SoundManager.Get().mixer.SetFloat("FXVolume", v);
        SoundManager.Get().SaveParameters();
    }

    public void UpdateSliderVolumeDialog(float v)
    {
        SoundManager.Get().mixer.SetFloat("DialogVolume", v);
        SoundManager.Get().SaveParameters();
    }

    public void UpdateSliderVolumeAmibance(float v)
    {
        SoundManager.Get().mixer.SetFloat("AmbienceVolume", v);
        SoundManager.Get().SaveParameters(); 
    }

}
