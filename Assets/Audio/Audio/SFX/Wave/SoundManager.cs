using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MusicSelector : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip[] musicTracks;

    [Header("UI")]
    public TMP_Dropdown musicDropdown;

    public static MusicSelector instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetupDropdown();
    }

    void SetupDropdown()
    {
        musicDropdown.ClearOptions();

        List<string> trackNames = new List<string>();

        for (int i = 0; i < musicTracks.Length; i++)
        {
            trackNames.Add(musicTracks[i].name);
        }

        musicDropdown.AddOptions(trackNames);
        musicDropdown.onValueChanged.AddListener(OnTrackSelected);
    }

    public void OnTrackSelected(int trackIndex)
    {
        if (trackIndex >= 0 && trackIndex < musicTracks.Length)
        {
            audioSource.clip = musicTracks[trackIndex];
            audioSource.Play();
        }
    }
}