using ABMU.Core;

using System;
using System.Collections.Generic;
using System.Linq;

public class FriendshipFactory : RelationshipFactory
{
    private AbstractController controller;
    private WorldParameters world;
    public FriendshipFactory(AbstractController _controller)
    {
        controller = _controller;
        world = WorldParameters.GetInstance();
    }
    public void createNetwork(Citizen citizen)
    {
        setSocialNetworks(citizen);
    }

    private void setSocialNetworks(Citizen citizen)
    {
        List<Citizen> allCandidates = controller.agents.Cast<Citizen>().ToList();
        allCandidates = allCandidates.Except(citizen.GetFriends()).ToList();
        allCandidates.Remove(citizen);

        List<Citizen> inAgeCandidates =
            allCandidates.Where(c => Math.Abs(c.Age - citizen.Age) < 5).ToList();


        while (citizen.Friendships.Count < world.numFriends &&
            inAgeCandidates.Count > 0)
        {
            Citizen candidate =
                inAgeCandidates[UnityEngine.Random.Range(0, inAgeCandidates.Count)];
            inAgeCandidates.Remove(candidate);
            allCandidates.Remove(candidate);

            if (candidate.Friendships.Count < world.numFriends)
            {
                citizen.AddFriendship(candidate);
                candidate.AddFriendship(citizen);
            }
        }

        if (UnityEngine.Random.value < world.randomFriendProb)
        {
            Citizen candidate =
                allCandidates[UnityEngine.Random.Range(0, allCandidates.Count)];

            citizen.AddFriendship(candidate);
            candidate.AddFriendship(citizen);
        }

    }
}