using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class peopleController : MonoBehaviour
{
    public NavMeshAgent agent;

    public GameObject PATH;
    private Transform[] PathPoints;

    public float minDistance;

    public int index = 0;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        PathPoints = new Transform[PATH.transform.childCount];
        for (int i=0; i<PathPoints.Length; i++)
        {
            PathPoints[i] = PATH.transform.GetChild(i);
        }
    }

    void Update()
    {
        roam();
    }

    void roam()
    {
        if (Vector3.Distance(transform.position, PathPoints[index].position) < minDistance)
        {
            if (index >= 0 && index < PathPoints.Length)
            {
                index += 1;
                if (index == PathPoints.Length)
                {
                    //Debug.Log("Hiiii2222222");
                    index = 0;
                }
            }
            else if(index == PathPoints.Length)
            {
                //Debug.Log("Hiiii2222222");
                index = 0;
            }
            else
            {
                //Debug.Log("Hiiii");
                index = 0;
            }

            //Debug.Log(index);
        }
        agent.SetDestination(PathPoints[index].position);

    }

}
