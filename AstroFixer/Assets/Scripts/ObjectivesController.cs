using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectivesController : MonoBehaviour
{
    [SerializeField] public float minEventTimeout;
    [SerializeField] public float maxEventTimeout;
    [SerializeField] public float minEventSpawnInterval;
    [SerializeField] public float maxEventSpawnInterval;
    [SerializeField] public ObjectiveIndicator objectiveIndicator;
    [SerializeField] public GameObject player;
    public enum ObjectiveType {
        OBJECTIVE_TIGHTEN
    }
    public class ObjectiveEvent {
        public ObjectiveEvent(Objective _objective){
            objective = _objective;
        }
        public Objective objective;
        public float timeout = 20;
        public bool active = false;
        public ObjectiveType type = ObjectiveType.OBJECTIVE_TIGHTEN;
    }
    private List<ObjectiveEvent> events = new List<ObjectiveEvent>();
    private List<ObjectiveEvent> activeEvents = new List<ObjectiveEvent>();
    private List<ObjectiveEvent> dormantEvents = new List<ObjectiveEvent>();

    // Start is called before the first frame update
    void Start()
    {
        SetObjectiveMarkers();
        StartCoroutine(SpawnEvents());
    }

    void SetObjectiveMarkers()
    {
        foreach(Transform child in transform){
            dormantEvents.Add(
                new ObjectiveEvent(child.GetComponent<Objective>())
            );
        }
    }

    public IEnumerator SpawnEvents()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minEventSpawnInterval, maxEventSpawnInterval));
            if (activeEvents.Count < dormantEvents.Count)
            {
                int activateEvent = Random.Range(0, dormantEvents.Count);
                activeEvents.Add(dormantEvents[activateEvent]);
                dormantEvents.RemoveAt(activateEvent);

                int activatedEvent = activeEvents.Count - 1;
                activeEvents[activatedEvent].timeout = Random.Range(minEventTimeout, maxEventTimeout);
                activeEvents[activatedEvent].active = true;
                objectiveIndicator.AddObjective(activeEvents[activatedEvent].objective);
                StartCoroutine(RunEvent(activeEvents[activatedEvent]));
            }
        }
    }

    public IEnumerator RunEvent(ObjectiveEvent objectiveEvent)
    {
        yield return new WaitForSeconds(objectiveEvent.timeout);
        if(objectiveEvent.active) {
            GameOverScript.Instance.TriggerGameOver("Your ship has been irreparably damaged. You face a lonely death on this ship. \n\n Gotta go fast next time.");
            RemoveObjectiveEvent(objectiveEvent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckObjectivesInteraction()
    {
        if(activeEvents.Count == 0) {
            return;
        }

        List<Objective> objectivesToRemove = new List<Objective>();
        Vector3 playerPos = player.transform.position;
        for(int i = 0; i < activeEvents.Count; i++) {
            Vector3 objectivePos = activeEvents[i].objective.transform.position;

            float distance = Vector3.Distance(playerPos, objectivePos);
            if (distance < 2.0f)
            {
                //Debug.Log("System fixed! :D");
                RemoveObjectiveEvent(activeEvents[i]);
                GameplayManager.Instance.pushState(GameplayState.MINIGAME);
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        CheckObjectivesInteraction();
    }

    private void RemoveObjectiveEvent(ObjectiveEvent objectiveEvent)
    {
        objectiveEvent.active = false;
        dormantEvents.Add(objectiveEvent);
        activeEvents.Remove(objectiveEvent);
        objectiveIndicator.RemoveObjective(objectiveEvent.objective);
    }
}
