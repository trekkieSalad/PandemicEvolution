using UnityEngine;

public class SocialCircleFactory : RelationshipFactory
{
    public void createNetwork(Citizen citizen)
    {
        Collider[] hitColliders =
                Physics.OverlapSphere(citizen.transform.position, 10f);
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.name != citizen.gameObject.name)
            {
                // Comprobar que no este en los amigos
                Citizen candidate = collider.gameObject.GetComponent<Citizen>();
                if ( !citizen.IsFriend(candidate))
                {
                    citizen.AddNeighbor(candidate);
                }
            }
        }
    }

}