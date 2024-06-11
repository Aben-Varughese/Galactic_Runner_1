using SplashKitSDK;

namespace Galactic_Runner_1
{
	public class HighScore
	{
		private string _filePath;
		private List<(string name,int score)> _highScore;
        private const int StartX = 150;
        private const int StartY = 200;
        private const int LineHeight = 30;
        private const int NameColumnWidth = 450;

        public HighScore(string filePath)
		{
			_filePath = filePath;
			_highScore = new List<(string name,int score)>();
            LoadScores();
		}

        private void LoadScores()
        {
            if (File.Exists(_filePath))
            {
                var lines = File.ReadAllLines(_filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int score))
                    {
                        _highScore.Add((parts[0], score));
                    }
                }
            }
        }

        public bool IsHighScore(int score)
        {
            return !_highScore.Any() || score > _highScore.Max(hs => hs.score);
        }

        public void AddScore(string name, int score)
        {
            _highScore.Add((name, score));
            _highScore = _highScore.OrderByDescending(s => s.score).Take(5).ToList(); // Keep top 10 scores
            SaveScores();
        }

        private void SaveScores()
        {
            var lines = _highScore.Select(s => $"{s.name},{s.score}");
            File.WriteAllLines(_filePath, lines);
        }

        public void DisplayHighScores(Window window, Font font)
        {
            int currentY = StartY;
            foreach (var score in _highScore)
            {
                string name = score.name;
                string scoreText = score.score.ToString();
                SplashKit.DrawText(name, Color.White, font, 20, StartX, currentY);
                SplashKit.DrawText(scoreText, Color.White, font, 20, StartX + NameColumnWidth, currentY);
                currentY += LineHeight;
            }
        }
    }
}

