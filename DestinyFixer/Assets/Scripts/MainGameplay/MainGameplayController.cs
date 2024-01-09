using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameplayController : MonoBehaviour
{
    [SerializeField] GameplayState thisState;
    [SerializeField] GameplayState nextState;
    Camera stateCamera;
    void Awake() {
        GameplayManager.Instance.onStateChange += OnGameManagerStateChanged;
        this.gameObject.SetActive(false);

        stateCamera = this.transform.Find("Main Camera").GetComponent<Camera>();
    }

    void OnDestroy() {
        GameplayManager.Instance.onStateChange -= OnGameManagerStateChanged;
    }

    private void OnGameManagerStateChanged(GameplayState topState) {
        if(topState == thisState) {
            Debug.Log(thisState + " detected.");
            this.gameObject.SetActive(true);
            stateCamera.enabled = true;
            InputAccess.playerInput.SwitchCurrentActionMap("Gameplay");
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
