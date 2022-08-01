using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public List<AudioClip> themes;

    [HideInInspector]
    public static MusicManager instance;

    private AudioSource _audioSource;
    private List<int> _playedThemes;

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
            instance = this;
        _audioSource = GetComponent<AudioSource>();
        _playedThemes = new List<int>();
    }

    void Start()
    {
        StartCoroutine(PlayThemes());
    }

    IEnumerator PlayThemes()
    {
        var randomTheme = GetRandomTheme();
        _playedThemes.Add(randomTheme);
        if (_playedThemes.Count == themes.Count)
            _playedThemes.Clear();

        _audioSource.clip = themes[randomTheme];
        _audioSource.Play();

        yield return new WaitForSeconds(_audioSource.clip.length);
        StartCoroutine(PlayThemes());
    }

    int GetRandomTheme()
    {
        var randomTheme = Random.Range(0, themes.Count);

        if (!_playedThemes.Contains(randomTheme))
            return randomTheme;
        else
        {
            randomTheme = GetRandomTheme();
            return randomTheme;
        }
    }
}