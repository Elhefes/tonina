using System.Collections;
using UnityEngine;

public class Villager : CreatureMovement
{
    private Vector3 startingPosition;
    private Coroutine freeMovementCoroutine;
    public float freeSpaceX;
    public float freeSpaceZ;
    public float minWaitTime;
    public float maxWaitTime;

    void Awake()
    {
        startingPosition = transform.position;
        freeMovementCoroutine = StartCoroutine(MoveRandomly());
    }

    private IEnumerator MoveRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            agent.destination = new Vector3(startingPosition.x + Random.Range(-freeSpaceX, freeSpaceX), startingPosition.y, startingPosition.z + Random.Range(-freeSpaceZ, freeSpaceZ));
        }
    }
}