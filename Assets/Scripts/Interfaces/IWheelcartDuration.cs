
using System;

namespace Assets.Scripts.Interfaces
{
    public interface IWheelcartDuration
    {
        event Action<float> OnWheelcartDuration;
    }
}
