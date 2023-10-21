using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TriggerNextScene : MonoBehaviour
{

    //load scenes async
    public static AsyncOperation loadingOperation;

    //time to next scene once trigger has been hit
    public float timeToNextScene = 5f;
    private bool timerOn = false;
    private float timer;

    //if checked, next scene will load based on timer, as soon as current scene loads
    public bool isIntro = false;
    
    //Loading UI Objects
    public GameObject Loading;
    public GameObject TimeObject;
    public Text TimeText;

    // Start is called before the first frame update
    void Start()
    {        
        if (TimeObject != null) {            
            timer = timeToNextScene;
            TimeObject.SetActive(false);
        }
        if (Loading != null) {
            Loading.SetActive(false);
        }
        if (isIntro) {
            StartCoroutine(myWaitCoroutine());
        };
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeText != null && timerOn) {
            timer -= Time.deltaTime;
            TimeText.text = Mathf.RoundToInt(timer).ToString();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "Projectile" || collider.gameObject.tag == "CandiceShockwaveCollider")
        {
            if (!isIntro)
            {
                StartCoroutine(myWaitCoroutine());
                timerOn = true;
                TimeObject.SetActive(true);
            };
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "CandiceShockwaveCollider") {
            if (!isIntro)
            {
                StartCoroutine(myWaitCoroutine());
                timerOn = true;
                TimeObject.SetActive(true);
            };
        }
    }

    public void LoadNextScene()
    {
        if (Loading != null) {
            Loading.SetActive(true);
        }        
        loadingOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1,  LoadSceneMode.Single);
    }

    IEnumerator myWaitCoroutine()
    {
        yield return new WaitForSeconds(timeToNextScene);
        LoadNextScene();
    }

}
