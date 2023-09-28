using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    private Player _player;
    private Animator _anim;
    [SerializeField]
    private AudioClip _enemyDestroyedSoundClip;
    [SerializeField]
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        if (_player == null)
        {
            Debug.LogError("The player is null");
        }
        _anim = this.GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("The anim is null");
        }
        if (_audioSource== null)
        {
            Debug.LogError("The audio source of enemy is null");
        } else
        {
            _audioSource.clip= _enemyDestroyedSoundClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //move down at 4m/s
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        //if bottom of screen, respawn from top with a new random x pos
        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7.0f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if other is player, damage the player, destroy us
        if (other.tag == "Player" && AnimatorIsPlaying() == false)
        {
            //damage player
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(this.gameObject, 2.8f); 
        }

        //if other is laser, destroy laser, destroy us
        if (other.tag == "Laser" && AnimatorIsPlaying() == false)
        {
            Destroy(other.gameObject);
            
            if (_player != null)
            {
                _player.AddScore(10);
            }
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
    }

    public bool AnimatorIsPlaying()
    {
        return _anim.GetCurrentAnimatorStateInfo(0).length >
               _anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
