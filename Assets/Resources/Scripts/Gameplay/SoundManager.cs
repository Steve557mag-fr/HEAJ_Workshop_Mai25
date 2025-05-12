using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource sourceFX, sourceAmbience, sourceDialog;


    public void PlayFX(AudioClip clip)
    {
        sourceFX.PlayOneShot(clip);
    }
    public void PlayDialog(AudioClip clip)
    {
        sourceAmbience.PlayOneShot(clip);
    }
    public void SetAmbience(AudioClip clip = null)
    {
        sourceAmbience.clip = clip;
        sourceAmbience.Pla
    }




    public void LoadParameters()
    {
        mixer.SetFloat("FXVolume", DataSystem.instance.GetData<float>("FXVolume"));
        mixer.SetFloat("DialogVolume", DataSystem.instance.GetData<float>("DialogVolume"));
        mixer.SetFloat("AmbienceVolume", DataSystem.instance.GetData<float>("AmbienceVolume"));
        mixer.SetFloat("MasterVolume", DataSystem.instance.GetData<float>("MasterVolume"));

    }
    public void SaveParameters()
    {
        mixer.GetFloat("FXVolume", out float volFX);
        mixer.GetFloat("DialogVolume", out float volDialog);
        mixer.GetFloat("AmbienceVolume", out float ambienceVolume);
        mixer.GetFloat("MasterVolume", out float masterVolume);

        DataSystem.instance.SetData("FXVolume", volFX);
        DataSystem.instance.SetData("DialogVolume", volDialog);
        DataSystem.instance.SetData("AmbienceVolume", ambienceVolume);
        DataSystem.instance.SetData("MasterVolume", masterVolume);

    }

}
