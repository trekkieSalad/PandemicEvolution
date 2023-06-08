using UnityEngine;

public class ExposedSirState : SirState
{
    public ExposedSirState(Citizen citizen) : base(citizen)
    {
        Utils.ChangeColor(_citizen, SirStateColor.Yellow);
        Type = StateType.Exposed;
    }

    protected override void CalculateNextState()
    {
        _nextState = StateType.Infected;
        SetTimeToStateUpdate(Utils.logNormal());
    }

    protected override void ChangeState()
    {
        _citizen.actualState = new InfectedSirState(_citizen);
        _citizen.asintomatic = Random.value < 0.4;
    }
}