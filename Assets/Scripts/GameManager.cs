using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://gameinternals.com/understanding-pac-man-ghost-behavior#:~:text=The%20purpose%20of%20the%20game%20is%20very%20simple,four%20ghosts%20that%20pursue%20Pac-Man%20through%20the%20maze.

public class GameManager : MonoBehaviour
{

    public AudioSource audio;
    public AudioClip gameStartMusic;

    // Reference variables
    public Ghost pinky;
    public Ghost blinky;
    public Ghost inky;
    public Ghost clyde;
    // Targets
    [HideInInspector] public Vector3 pinkyTargetPos;
    [HideInInspector] public Vector3 inkyTargetPos;

    float timer;
    float timerDelay;

    List<int> timers = new List<int>();
    int timerIndex = 0;
    bool timerRunning = false;


    // Start is called before the first frame update
    void Start()
    {
        timers.Add(7);
        timers.Add(20);
        timers.Add(7);
        timers.Add(20);
        timers.Add(5);
        timers.Add(20);
        timers.Add(5);
        timers.Add(20);
        timers.Add(5);

        audio.PlayOneShot(gameStartMusic, 0.6f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            SetScatterMode();
        }
        // Timers
        if (timerIndex < timers.Count-1)
        {
            if (!timerRunning)
            {
                timerDelay = timers[timerIndex];
                timer = Time.time + timerDelay;

                timerRunning = true;
            }
            else
            {
                if (timer < Time.time)
                {
                    timerIndex++;
                    print(timerIndex);
                    SetScatterMode();
                    timerRunning = false;
                }
            }
        }
    }

    public void SetScatterMode()
    {
        if (pinky != null) pinky.ToggleGhostMode();
        if (inky != null) inky.ToggleGhostMode();
        if (blinky != null) blinky.ToggleGhostMode();
        if (clyde != null) clyde.ToggleGhostMode();
    }

    public void SetPinkyTarget(Vector3 targetPos)
    {
        pinkyTargetPos = targetPos;
        //print("pinky target = " + pinkyTarget);
    }
    public Vector3 GetPinkyTarget()
    {
        return pinkyTargetPos;
    }
    public void SetInkyTarget(Vector3 targetPos)
    {
        inkyTargetPos = targetPos;
        //print("inky target = " + inkyTarget);
    }
    public Vector3 GetInkyTarget()
    {
        return inkyTargetPos;
    }

}
