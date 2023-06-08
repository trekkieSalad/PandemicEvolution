using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour
{
    public List<Citizen> users = new List<Citizen>();
    public PlaceType type;

    public void RegisterCitizen(Citizen citizen)
    {
        users.Add(citizen);
    }

    public void UnregisterCitizen(Citizen citizen)
    {
        users.Remove(citizen);
    }

    public void EmptyNode()
    {
        users.Clear();
    }
}
