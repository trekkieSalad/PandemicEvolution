using UnityEngine;
using ABMU.Core;

using System.Collections.Generic;

public class Scientist : AbstractController 
{
    [Header("Simulation")]
    public Bounds Bounds;

    [Header("Agents")]
    public bool CreateSimulatedCitizens;
    public GameObject Cube;
    public int NumberOfCitizens;

    void Start(){
        for (int i = 0; i < NumberOfCitizens; i++)
        {
            GameObject citizen = Instantiate(Cube, new Vector3(Random.Range(Bounds.min.x, Bounds.max.x), Random.Range(Bounds.min.y, Bounds.max.y), Random.Range(Bounds.min.z, Bounds.max.z)), Quaternion.identity);
        }
    }

    
}
