using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private AudioClip _explosionSoundClip;
    [SerializeField]
    private AudioSource _audioSource;
    void Start()
    {
        _audioSource= GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("The audio source of explosion is null.");
        }
        else
        {
            _audioSource.clip = _explosionSoundClip;
        }
        _audioSource.Play();
        Destroy(gameObject, 3.0f);
    }
}
