public class WheelcartModel
{

    private float speed;
    private int maxHealth;
    private int currentHealth;
    
    public WheelcartModel()
    {
        speed = 0.5f;
        maxHealth = 100;
        currentHealth = 100;
    }

    public float getSpeed()
    {
        return speed;
    }

    public int getMaxHealth()
    {
        return maxHealth;
    }

    public int getCurrentHealth()
    {
        return currentHealth;
    }

    public void takeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOverScreen();
        }
    }

}
