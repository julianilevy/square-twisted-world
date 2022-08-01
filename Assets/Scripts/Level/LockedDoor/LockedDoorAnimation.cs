using UnityEngine;

public class LockedDoorAnimation : MonoBehaviour
{
    public LockedDoor lockedDoor;
    public AudioClip opening1;
    public AudioClip opening2;
    public AudioClip disappearing;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Opening1()
    {
        _audioSource.PlayOneShot(opening1);
    }

    private void Opening2()
    {
        _audioSource.PlayOneShot(opening2);
    }

    private void Disappearing()
    {
        _audioSource.PlayOneShot(disappearing);
    }

    public void OnDisappearingAnimationEnded()
    {
        lockedDoor.DestroyColliders();
    }
}