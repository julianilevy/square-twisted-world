using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public Player player;
    public PlayerStats playerStats;
    public List<AudioClip> walkFootsteps = new List<AudioClip>();
    public List<AudioClip> runFootsteps = new List<AudioClip>();
    public List<AudioClip> walkWithKeyFootsteps = new List<AudioClip>();
    public List<AudioClip> runWithKeyFootsteps = new List<AudioClip>();

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Step()
    {
        List<AudioClip> currentFootstepsList = null;

        if (player.running)
        {
            if (playerStats.isCarryingObject)
                currentFootstepsList = runWithKeyFootsteps;
            else
                currentFootstepsList = runFootsteps;
        }
        else
        {
            if (playerStats.isCarryingObject)
                currentFootstepsList = walkWithKeyFootsteps;
            else
                currentFootstepsList = walkFootsteps;
        }

        if (currentFootstepsList != null)
        {
            var randomStep = currentFootstepsList[UnityEngine.Random.Range(0, currentFootstepsList.Count)];
            _audioSource.PlayOneShot(randomStep);
        }
    }
}