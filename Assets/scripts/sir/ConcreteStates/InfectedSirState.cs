using UnityEngine;

public class InfectedSirState : SirState
{
    public InfectedSirState(Citizen citizen) : base(citizen)
    {
        Utils.ChangeColor(_citizen, SirStateColor.Red);
        Type = StateType.Infected;
    }

    protected override void CalculateNextState()
    {
        double probability = Random.value;

        if (probability < _worldParameters.pId)
        {
            _nextState = StateType.Dead;
            SetTimeToStateUpdate(_worldParameters.infectiousDaysToDead);
        }
        else if (probability < _worldParameters.pIh)
        {
            _nextState = StateType.Hospitalized;
            SetTimeToStateUpdate(_worldParameters.infectiousDaysToHospitalized);
        }
        else
        {
            _nextState = StateType.Recovered;
            SetTimeToStateUpdate(_worldParameters.infectiousDaysToRecovered);
        }
    }

    protected override void ChangeState()
    {
        if (_nextState.Equals(StateType.Dead))
            _citizen.actualState = new DeadSirState(_citizen);
        else if (_nextState.Equals(StateType.Hospitalized))
            _citizen.actualState = new HospitalizedSirState(_citizen);
        else
            _citizen.actualState = new RecoveredSirState(_citizen);
    }
}