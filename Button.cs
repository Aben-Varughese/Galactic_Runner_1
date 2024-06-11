using SplashKitSDK;

namespace Galactic_Runner
{
    public class Button
    {
        private string _text;
        private int _x, _y, _width, _height;
        private Action _onClickAction;

        public Button(string text, int x, int y, int width, int height, Action onClickAction)
        {
            _text = text;
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _onClickAction = onClickAction;
        }

        public void Draw()
        {
            Color buttonColor = Color.White;
            SplashKit.FillRectangle(buttonColor, _x, _y, _width, _height);

            int textX = _x + (_width / 2) - (SplashKit.TextWidth(_text, "Nasalization", 16) / 2);
            int textY = _y + (_height / 2) - (SplashKit.TextHeight(_text, "Nasalization", 16) / 2);

            SplashKit.DrawText(_text, Color.Black, "Nasalization", 16, textX, textY);
       
        }

        public bool Clicked()
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                if (SplashKit.MouseX() >= _x && SplashKit.MouseX() <= _x + _width &&
                    SplashKit.MouseY() >= _y && SplashKit.MouseY() <= _y + _height)
                {
                    _onClickAction(); // Execute the stored action
                    return true;
                }
            }
            return false;
        }
    }
}