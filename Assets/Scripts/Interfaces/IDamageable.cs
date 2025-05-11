
namespace Assets.Scripts.Interfaces
{
    public interface IDamageable
    {
        public void TakeDamage(float damageAmout, string hittedById);
        public string GetTag();
    }
}
