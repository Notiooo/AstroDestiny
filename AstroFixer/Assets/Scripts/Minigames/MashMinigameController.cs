using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MashMinigameController : MonoBehaviour
{
    [SerializeField] GameplayState thisState;
    [SerializeField] GameplayState nextState;
    [SerializeField] GameObject wrench;

    int increment = 1;
    int goal = 40;
    int value = 0;
    bool done = false;

    float animStart = 0;
    float animEnd = 45;
    float animAngle = 0;
    bool animationDone = false;
    Quaternion originalRotation;

    void Awake() {
        GameplayManager.Instance.onStateChange += OnGameManagerStateChanged;
        this.gameObject.SetActive(false);
    }

    void OnDestroy() {
        GameplayManager.Instance.onStateChange -= OnGameManagerStateChanged;
    }

    private void OnGameManagerStateChanged(GameplayState topState) {
        if(topState == thisState) {
            Debug.Log(thisState + " detected.");
            this.gameObject.SetActive(true);
            value = 0;
            done = false;
            animAngle = 0;
            animationDone = false;
            originalRotation = wrench.transform.rotation;
            InputAccess.playerInput.SwitchCurrentActionMap("Minigame");
        }
    }

    private void MinigameDone()
    {
        wrench.transform.rotation = originalRotation;
        this.gameObject.SetActive(false);
        GameplayManager.Instance.popState();
    }

    public void MashWrench()
    {
        value += increment;
        wrench.transform.Rotate(0, 0, Random.Range(-5, 5));
        if(value > goal)
        {
            done = true;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void AnimateWrech() {
        wrench.transform.Rotate(0, 0, Mathf.LerpAngle(animStart, animEnd, animAngle));
        animAngle += 0.005f;
        if(animAngle >= 0.2) {
            animationDone = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(done) {
            AnimateWrech();
        }
        if(animationDone) {
            MinigameDone();
        }
    }
}
