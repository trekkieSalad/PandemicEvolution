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
        allCandidates = allCandidates.Except(citizen.getFriends()).ToList();
        allCandidates.Remove(citizen);

        List<Citizen> inAgeCandidates =
            allCandidates.Where(c => Math.Abs(c.age - citizen.age) < 5).ToList();


        while (citizen.friendships.Count < world.numFriends &&
            inAgeCandidates.Count > 0)
        {
            Citizen candidate =
                inAgeCandidates[UnityEngine.Random.Range(0, inAgeCandidates.Count)];
            inAgeCandidates.Remove(candidate);
            allCandidates.Remove(candidate);

            if (candidate.friendships.Count < world.numFriends)
            {
                citizen.addFriendship(candidate);
                candidate.addFriendship(citizen);
            }
        }

        if (UnityEngine.Random.value < world.randomFriendProb)
        {
            Citizen candidate =
                allCandidates[UnityEngine.Random.Range(0, allCandidates.Count)];

            citizen.addFriendship(candidate);
            candidate.addFriendship(citizen);
        }

    }
}