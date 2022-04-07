using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollisionWithGhost : MonoBehaviour
{
    public AudioSource audio;
    public AudioClip eatGhost;
    public AudioClip playerDie;
    // Start is called before the first frame update
    public EatPill eatPill;

    private void Start()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ghost")
        {
            if (eatPill.IsScattering())
            {
                audio.PlayOneShot(eatGhost, .6f);
            }
            else
            {
                audio.PlayOneShot(playerDie, .6f);
                //end game and return to menu
                StartCoroutine(GotoMenuScene());
            }
        }
    }
    IEnumerator GotoMenuScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
    }
}
