using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public enum GameState {
    MAIN_MENU,
    GAMEPLAY,
    END_MENU
}
public class GameManager : StateManager<GameState>
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Instance.pushState(GameState.MAIN_MENU);
    }
}
