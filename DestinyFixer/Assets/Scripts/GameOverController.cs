using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [SerializeField] GameState thisState;
    [SerializeField] GameState nextState;

    void Awake() {
        GameManager.Instance.onStateChange += OnGameManagerStateChanged;
        this.gameObject.SetActive(false);
    }

    void OnDestroy() {
        GameManager.Instance.onStateChange -= OnGameManagerStateChanged;
    }

    private void OnGameManagerStateChanged(GameState topState) {
        if(topState == thisState) {
            Debug.Log(thisState + " detected.");
            this.gameObject.SetActive(true);
            InputAccess.playerInput.SwitchCurrentActionMap("EndMenu");
            GameOverScript.Instance.TriggerGameOver();
        }
    }

    public void OnRestartGameClicked() {
        this.gameObject.SetActive(false);
        GameManager.Instance.popState();
        GameOverScript.Instance.RestartGame();
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
