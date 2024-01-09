using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameplayState {
    MAIN,
    MINIGAME
}
public class GameplayManager : StateManager<GameplayState>
{
    [SerializeField] GameState thisState;
    [SerializeField] GameState nextState;

    public static GameplayManager Instance { get; private set; }

    void Awake() {
        if(Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        GameManager.Instance.onStateChange += OnGameManagerStateChanged;
        this.gameObject.SetActive(false);
    }

    public void EndGame(string endScreenMessage)
    {
        GameOverScript.Instance.SetGameOverText(endScreenMessage);
        GameManager.Instance.pushState(nextState);
    }

    void OnDestroy() {
        GameManager.Instance.onStateChange -= OnGameManagerStateChanged;
    }

    private void OnGameManagerStateChanged(GameState topState) {
        if(topState == thisState) {
            Debug.Log(thisState + " detected.");
            this.gameObject.SetActive(true);
            Instance.pushState(GameplayState.MAIN);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
