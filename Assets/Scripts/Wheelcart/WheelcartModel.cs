public class WheelcartModel
{

    public float speed { get; private set; }
    public int maxHealth { get; private set; }
    public int currentHealth { get; private set; }
    
    public WheelcartModel()
    {
        speed = 0.5f;
        maxHealth = 100;
        currentHealth = 100;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetHealth(int health)
    {
        currentHealth = health;
    }

}
