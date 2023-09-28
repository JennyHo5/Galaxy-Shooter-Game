using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField]
    private int powerupID; // 0 = tripleshot, 1 = speed, 2 = shield
    
    [SerializeField]
    private AudioClip _clip;



    void Update()
    {
        //move down at a speed of 3 (can adjust in the inspector)
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        //when we leave the screen, destroy us
        if (transform.position.y < -5.0f)
        {
            Destroy(this.gameObject);
        }
    }

    //onTriggerCollision
    //Only be collectable by the player (use tags)
    //on collected, destroy
    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Player")
        {
            //communicate player script
            Player player = other.transform.GetComponent<Player>();
            AudioSource.PlayClipAtPoint(_clip, transform.position);
            if (player != null)
            {
                switch(powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedupActive();
                        break;
                    case 2:
                        player.ShielderActive();
                        break;
                    default:
                        Debug.Log("Default value");
                        break;
                }
            }
            Destroy(this.gameObject);
        }
    }
}
