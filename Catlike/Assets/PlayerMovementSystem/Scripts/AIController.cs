using System;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 3f;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position + Vector3.forward * 5f, speed * Time.deltaTime);
    }
}
