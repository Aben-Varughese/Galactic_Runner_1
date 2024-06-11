using SplashKitSDK;
namespace Galactic_Runner_1
{
	public static class EnemyFactory
	{
        private static Random _random = new Random();
        public static Enemy CreateEnemy(int level, int windowWidth)
        {
            int spawnY = 50; // Top margin
            int spawnX = SplashKit.Rnd(windowWidth - 100);
            int speed = 2 + level; // Increase speed with level

            // Define enemy types
            List<string> enemyTypes = new List<string> { "enemy.png" };

            // Add more enemy types as levels increase
            if (level > 1)
            {
                enemyTypes.Add("enemy_2.png");
            }
            if (level > 3)
            {
                enemyTypes.Add("enemy_3.png");
            }

            // Randomly select an enemy type from the available types
            string enemyImage = enemyTypes[_random.Next(enemyTypes.Count)];

            return new Enemy(spawnX, spawnY, 50, 50, speed, enemyImage);
        }
    }
}

