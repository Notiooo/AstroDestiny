using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager<State> : MonoBehaviour
{
    public event System.Action<State> onStateChange;
    Stack<State> statesStack = new Stack<State>();

    public void pushState(State state)
    {
        statesStack.Push(state);
        onStateChange(state);
    }
    public void popState()
    {
        statesStack.Pop();
        if(statesStack.Count > 0) {
            onStateChange(statesStack.Peek());
        }
    }

    public void changeTopState(State state)
    {
        statesStack.Pop();
        statesStack.Push(state);
        onStateChange(state);
    }
}
