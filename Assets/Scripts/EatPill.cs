using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EatPill : MonoBehaviour
{
    public AudioClip eatPill;
    public AudioClip scatter;
    public AudioSource audio;

    public GameManager gameManager;

    float timer;
    float timerDelay = 2f;
    bool isScattering = false;

    int score;
    public TMP_Text scoreText;

    private void Start()
    {
        score = 0;
    }
    private void Update()
    {
        CheckIfAnyPillsLeft();
        // handle scatter
        if (isScattering)
        {
            if ( timer <= 0)
            {
                audio.loop = false;
                gameManager.SetScatterMode();
                isScattering = false;
            }
            timer -= Time.deltaTime;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pill")
        {
            score += 5;
            SetScoreText();
            audio.PlayOneShot(eatPill, 0.6f);
            Destroy(other.gameObject);
        }
        if (other.tag == "PowerPill")
        {
            score += 50;
            SetScoreText();
            audio.clip = scatter;
            audio.loop = true;
            audio.Play();
            gameManager.SetScatterMode();
            timer = Time.time + timerDelay;
            isScattering = true;
            Destroy(other.gameObject);
        }
    }
    void SetScoreText()
    {
        scoreText.text = score.ToString();
    }
    void CheckIfAnyPillsLeft()
    {
        GameObject[] pills = GameObject.FindGameObjectsWithTag("Pill");
        if (pills.Length == 0)
        {
            SceneManager.LoadScene(0);
        }
    }
    public bool IsScattering()
    {
        return isScattering;
    }

}
