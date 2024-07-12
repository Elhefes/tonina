using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public Weapon weaponOnHand;
    public CreatureMovement creatureMovement;
    private Coroutine attackCoroutine;
    public bool onCooldown;
    public bool shouldAttack;

    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        if (creatureMovement.target)
        {
            creatureMovement.agent.destination = creatureMovement.target.position;

            var directionToTarget = creatureMovement.target.position - transform.position;
            directionToTarget.y = 0;
            var angle = Vector3.Angle(transform.forward, directionToTarget);
            Debug.DrawLine(transform.position, transform.position + directionToTarget * 114f, Color.red);
            shouldAttack = weaponOnHand.ShouldAttack(Vector3.Distance(transform.position, creatureMovement.target.position), angle);

            if (shouldAttack)
            {
                if (!onCooldown) attackCoroutine = StartCoroutine(Attack());
            }
            else
            {
                LookAt(creatureMovement.target);
            }
        }
    }

    public void LookAt(Transform objToLookAt)
    {
        if (Vector3.Distance(transform.position, objToLookAt.position) <= creatureMovement.agent.stoppingDistance + 1f)
        {
            // Calculate the direction to the destination
            Vector3 direction = (creatureMovement.agent.destination - transform.position).normalized;

            // Ignore the y-axis to prevent tilting
            direction.y = 0f;

            // Rotate towards the destination
            if (direction != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * creatureMovement.agent.angularSpeed * 0.25f);
            }
        }
    }

    protected virtual IEnumerator Attack()
    {
        onCooldown = true;
        while (creatureMovement.target && shouldAttack)
        {
            weaponOnHand.Attack(creatureMovement.animator);
            yield return new WaitForSeconds(weaponOnHand.attackCooldown);
        }
        onCooldown = false;
    }
}
