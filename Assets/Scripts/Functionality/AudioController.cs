using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource bg_adudio;
    [SerializeField] private AudioSource audioPlayer_wl;
    [SerializeField] private AudioSource audioPlayer_button;
    [SerializeField] private AudioSource audioPlayer_Spin;
    [SerializeField] private AudioSource reelGlow_Sound;


    [Header("clips")]
    [SerializeField] private AudioClip SpinButtonClip;
    [SerializeField] private AudioClip SpinClip;
    [SerializeField] private AudioClip GambleSpinClip;
    [SerializeField] private AudioClip Button;
    [SerializeField] private AudioClip Win_Audio;
    [SerializeField] private AudioClip GambleWin_Audio;
    [SerializeField] private AudioClip NormalBg_Audio;
    [SerializeField] private AudioClip FreeSpinBg_Audio;
    [SerializeField] private AudioClip ReelGlow_audio;

    private bool isForceMuted = false;
    private List<AudioSource> allSources;
    private readonly Dictionary<AudioSource, bool> preFocusMuteState = new Dictionary<AudioSource, bool>();

    private void Awake()
    {
        allSources = new List<AudioSource> { bg_adudio, audioPlayer_wl, audioPlayer_button, audioPlayer_Spin, reelGlow_Sound };
        reelGlow_Sound.clip = ReelGlow_audio;
        playBgAudio();

        //if (bg_adudio) bg_adudio.Play();
        //audioPlayer_button.clip = clips[clips.Length - 1];
    }

    internal void PlayWLAudio(string type = "default")
    {
        StopWLAaudio();
        // audioPlayer_wl.loop=loop;
        if (type == "gamble")
            audioPlayer_wl.clip = GambleWin_Audio;
        else
        {
            audioPlayer_wl.clip = Win_Audio;
            audioPlayer_wl.pitch = 1.5f;

        }

        audioPlayer_wl.Play();

    }

    internal void PlaySpinAudio(string type = "default")
    {
        if (type == "gamble")
            audioPlayer_Spin.clip = GambleSpinClip;
        else
            audioPlayer_Spin.clip = SpinClip;

        audioPlayer_Spin.Play();

    }

    internal void StopSpinAudio()
    {

        if (audioPlayer_Spin) audioPlayer_Spin.Stop();

    }

    private void OnApplicationFocus(bool focus)
    {
        SetMuteAll(!focus);
    }

    internal void SetMuteAll(bool forceMute)
    {
        if (forceMute == isForceMuted) return;
        isForceMuted = forceMute;

        foreach (var source in allSources)
        {
            if (source == null) continue;
            if (forceMute)
            {
                preFocusMuteState[source] = source.mute;
                source.mute = true;
            }
            else
            {
                source.mute = preFocusMuteState.TryGetValue(source, out bool prevMuted) ? prevMuted : source.mute;
            }
        }
    }



    internal void playBgAudio(string type = "default")
    {


        //int randomIndex = UnityEngine.Random.Range(0, Bg_Audio.Length);
        StopBgAudio();
        bg_adudio.loop = true;
        if (bg_adudio)
        {
            if (type == "FP")
                bg_adudio.clip = FreeSpinBg_Audio;
            else
                bg_adudio.clip = NormalBg_Audio;


            bg_adudio.Play();
        }

    }

    internal void PlayButtonAudio(string type = "default")
    {

        if (type == "spin")
            audioPlayer_button.clip = SpinButtonClip;
        else
            audioPlayer_button.clip = Button;

        //StopButtonAudio();
        audioPlayer_button.Play();
        // Invoke("StopButtonAudio", audioPlayer_button.clip.length);

    }

    internal void StopWLAaudio()
    {
        audioPlayer_wl.Stop();
        audioPlayer_wl.loop = false;
        audioPlayer_wl.pitch = 1f;

    }

    internal void ReelGlowSound(bool play)
    {
        if (play)
            reelGlow_Sound.Play();
        else
            reelGlow_Sound.Stop();

    }


    internal void StopButtonAudio()
    {

        audioPlayer_button.Stop();

    }


    internal void StopBgAudio()
    {
        bg_adudio.Stop();

    }



    internal void ToggleMute(bool toggle, string type)
    {

        switch (type)
        {
            case "bg":
                SetSourceMute(bg_adudio, toggle);
                break;
            case "button":
                SetSourceMute(audioPlayer_button, toggle);
                SetSourceMute(audioPlayer_Spin, toggle);
                break;
            case "wl":
                SetSourceMute(audioPlayer_wl, toggle);
                break;
            case "all":
                SetSourceMute(bg_adudio, toggle);
                SetSourceMute(audioPlayer_button, toggle);
                SetSourceMute(audioPlayer_Spin, toggle);
                SetSourceMute(audioPlayer_wl, toggle);
                SetSourceMute(reelGlow_Sound, toggle);
                break;



        }
    }

    private void SetSourceMute(AudioSource source, bool mute)
    {
        if (source == null) return;
        source.mute = mute;
        if (isForceMuted) preFocusMuteState[source] = mute;
    }

}
