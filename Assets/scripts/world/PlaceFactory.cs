
using System;
using System.Collections.Generic;

using ABMU;

using UnityEngine;

public class PlaceFactory
{
    public List<Place> CreatePlaces(
        GameObject placeObject, Bounds bounds)
    {
        List<Place> places = new List<Place>();

        foreach (PlaceType type in Enum.GetValues(typeof(PlaceType)))
        {
            int number = type.GetCapacity();

            for (int i = 0; i < number; i++)
            {
                Vector3 pos = Utilities.RandomPointInBounds(bounds);
                GameObject placeGameObject = MonoBehaviour.Instantiate(placeObject);
                placeGameObject.transform.position = pos + Vector3.up * 0.5f;
                placeGameObject.name = type.ToString() + " " + i;
                placeGameObject.tag = type.ToString();

                Place place = placeGameObject.GetComponent<Place>();
                place.type = type;
                places.Add(place);
            }
        }

        return places;
    }
}
