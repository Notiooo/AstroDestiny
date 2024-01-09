using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameState thisState;
    [SerializeField] GameState nextState;
    Camera stateCamera;
    void Awake() {
        GameManager.Instance.onStateChange += OnGameManagerStateChanged;
        this.gameObject.SetActive(false);

        stateCamera = this.transform.Find("Camera").GetComponent<Camera>();
    }

    void OnDestroy() {
        GameManager.Instance.onStateChange -= OnGameManagerStateChanged;
    }

    private void OnGameManagerStateChanged(GameState topState) {
        if(topState == thisState) {
            Debug.Log(thisState + " detected.");
            this.gameObject.SetActive(true);
            stateCamera.enabled = true;
            InputAccess.playerInput.SwitchCurrentActionMap("MainMenu");
        }
    }

    public void OnStartGameClicked() {
        this.gameObject.SetActive(false);
        stateCamera.enabled = false;
        GameManager.Instance.changeTopState(nextState);
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
