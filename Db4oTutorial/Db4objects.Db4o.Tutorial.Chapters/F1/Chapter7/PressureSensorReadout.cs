using System;

namespace Db4objects.Db4o.Tutorial.F1.Chapter7
{    public class PressureSensorReadout : SensorReadout
    {
        private readonly double _pressure;

        public PressureSensorReadout(DateTime time, Car car, String description, double pressure)
            : base(time, car, description)
        {
            this._pressure = pressure;
        }

        public double Pressure
        {
            get
            {
                Activate();
                return _pressure;
            }
        }

        override public String ToString()
        {
            return String.Format("{0} pressure {1}", base.ToString(), _pressure);
        }
    }
}