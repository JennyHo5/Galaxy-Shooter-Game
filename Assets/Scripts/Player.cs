using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleshotPrefab;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    private bool _isTripleshotActive = false;
    private bool _isSpeedupActive = false;
    private bool _isShielderActive = false;

    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _rightEngine, _leftEngine;

    [SerializeField]
    private int _score;

    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>(); //find the object, get the component
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null) 
        {
            Debug.LogError("The spawn manager is null");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI manager is null");
        }

        if (_audioSource == null) 
        {
            Debug.LogError("Audio source on the player is null");
        } else
        {
            _audioSource.clip= _laserSoundClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        //if space key && after cool down
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }


    void CalculateMovement()
    {
        float horizonalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizonalInput, verticalInput, 0);


        transform.Translate(direction * _speed * Time.deltaTime);


        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x >= 11)
        {
            transform.position = new Vector3(-11, transform.position.y, 0);
        }
        else if (transform.position.x <= -11)
        {
            transform.position = new Vector3(11, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
            //cool down system
            _canFire = Time.time + _fireRate;

        //if tripleshotActive is true
        //  fire three lasers (triple shot prefab)
        //else fire one laser
        if (_isTripleshotActive == true)
        {
            Instantiate(_tripleshotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }
        _audioSource.Play();
    }

    public void Damage()
    {
        if (_isShielderActive== true)
        {
            _isShielderActive= false;
            _shieldVisualizer.SetActive(false);
            return;
        }
        else
        {
            _lives -= 1;
            //if lives == 2, enable right engine
            if (_lives == 2)
            {
                _rightEngine.SetActive(true);
                _uiManager.UpdateLives(_lives);
            }
            //else if live == 1, enable left engine
            else if (_lives == 1)
            {
                _leftEngine.SetActive(true);
                _uiManager.UpdateLives(_lives);
            }
            else
            {
                //communicate with SpawnManager, stop spawning
                _spawnManager.OnPlayerDeath();
                _uiManager.UpdateLives(_lives);
                Destroy(this.gameObject);
                _uiManager.GameOverSequence();
            }
        }
    }

    public void TripleShotActive()
    {
        _isTripleshotActive = true;
        //start the power down coroution for tripleshot
        StartCoroutine(TripleshotPowerDownRoutine());
    }

    IEnumerator TripleshotPowerDownRoutine()
    {
        //wait 5 second
        //set the tripleshot to false
        while (_isTripleshotActive == true)
        {
            yield return new WaitForSeconds(5.0f);
            _isTripleshotActive = false;
         
        }
    }

    public void SpeedupActive()
    {
        _isSpeedupActive = true;
        _speed = 10.0f;
        StartCoroutine(SpeedupPowerDownRoutine());
        
    }

    IEnumerator SpeedupPowerDownRoutine()
    {
        while (_isSpeedupActive == true)
        {
            yield return new WaitForSeconds(5.0f);
            _isSpeedupActive = false;
            _speed = 5.0f;
        }
    }

    public void ShielderActive()
    {
        _isShielderActive = true;
        _shieldVisualizer.SetActive(true);
        StartCoroutine(ShieldPowerDownRoutine());
    }

    IEnumerator ShieldPowerDownRoutine()
    {
        while (_isShielderActive == true)
        {
            yield return new WaitForSeconds(5.0f);
            _isShielderActive = false;
            _shieldVisualizer.SetActive(false);
        }
    }

    //method to add 10 to the score
    //communicate with the UI to update the score
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
