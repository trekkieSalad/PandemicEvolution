using UnityEngine;

public class ICUSirState : SirState
{
    public ICUSirState(Citizen citizen) : base(citizen)
    {
        Utils.ChangeColor(citizen, StateColor.Brown);
        _citizen.typeOfState = TypeOfState.ICU;
        _citizen.transform.SetParent(GameObject.Find("ICUs").transform);
    }

    protected override void CalculateNextState()
    {
        double probability = Random.value;

        if (probability < _worldParameters.pId)
        {
            _nextState = TypeOfState.Dead;
            SetTimeToStateUpdate(_worldParameters.icuDaysToDead);
        }
        else
        {
            _nextState = TypeOfState.Recovered;
            SetTimeToStateUpdate(_worldParameters.icuDaysToRecovered);
        }
    }

    protected override void ChangeState()
    {
        if (_nextState.Equals(TypeOfState.Dead))
            _citizen.actualState = new DeadSirState(_citizen);
        else
            _citizen.actualState = new RecoveredSirState(_citizen);
    }
}