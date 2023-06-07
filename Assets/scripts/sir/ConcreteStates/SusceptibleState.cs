
using UnityEngine;

public class SusceptibleSirState : SirState
{
    public SusceptibleSirState(Citizen citizen) : base(citizen)
    {
        Utils.ChangeColor(_citizen, StateColor.Green);
        _citizen.typeOfState = TypeOfState.Susceptible;
    }

    protected override void CalculateNextState()
    {
        _nextState = TypeOfState.Exposed;
        SetTimeToStateUpdate(0);
    }

    protected override void ChangeState()
    {
        _citizen.actualState = new ExposedSirState(_citizen);
    }
}
