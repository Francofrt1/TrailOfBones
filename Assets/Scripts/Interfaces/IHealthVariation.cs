using System;

namespace Assets.Scripts.Interfaces
{
    public interface IHealthVariation
    {
        event Action<float, float> OnHealthVariation;
        event Action OnDie;
    }
}
