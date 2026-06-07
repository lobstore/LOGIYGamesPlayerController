using UnityEngine;

namespace LOGIYGames
{
    public class WallClimbRotaion : IRotationStrategy
    {
        SensorsModule Sensors;

        public WallClimbRotaion(SensorsModule sensors)
        {
            Sensors = sensors;
        }

        public Quaternion GetRotation()
        {
            return Quaternion.LookRotation(-Sensors.LegsFrontHit.normal);
        }
    }
}

