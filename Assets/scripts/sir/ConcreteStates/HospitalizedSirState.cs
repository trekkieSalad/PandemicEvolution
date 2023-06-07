using UnityEngine;

public class HospitalizedSirState : SirState
{
    public HospitalizedSirState(Citizen citizen) : base(citizen)
    {
        Utils.ChangeColor(citizen, StateColor.Violet);
        _citizen.typeOfState = TypeOfState.Hospitalized;
    }

    protected override void CalculateNextState()
    {
        double probability = Random.value;

        if (probability < _worldParameters.pHd)
        {
            _nextState = TypeOfState.Dead;
            SetTimeToStateUpdate(_worldParameters.hospitalizedDaysToDead);
        }
        else if (probability < _worldParameters.pHicu)
        {
            _nextState = TypeOfState.ICU;
            SetTimeToStateUpdate(_worldParameters.hospitalizedDaysToIcu);
        }
        else
        {
            _nextState = TypeOfState.Recovered;
            SetTimeToStateUpdate(_worldParameters.hospitalizedDaysToRecovered);
        }
    }

    protected override void ChangeState()
    {
        if (_nextState.Equals(TypeOfState.Dead))
            _citizen.actualState = new DeadSirState(_citizen);
        else if (_nextState.Equals(TypeOfState.ICU))
            _citizen.actualState = new ICUSirState(_citizen);
        else
            _citizen.actualState = new RecoveredSirState(_citizen);
    }
}