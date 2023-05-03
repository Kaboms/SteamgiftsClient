using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace SteamgiftsClient.Views.ReordedContainer
{
    public partial class ReorderContainerItemView : UserControl
    {
        private bool _isPressed;
        private Point _positionInBlock;
        private TranslateTransform _transform = null!;

        public ReorderContainerItemView()
        {
            InitializeComponent();

            var moveButton = this.FindControl<Button>("MoveButton");
            moveButton.AddHandler(Button.PointerPressedEvent, ReorderContainerItemView_PointerPressed, handledEventsToo: true);
            moveButton.AddHandler(Button.PointerReleasedEvent, ReorderContainerItemView_PointerReleased, handledEventsToo: true);
            PointerMoved += ReorderContainerItemView_PointerMoved;
        }

        private void ReorderContainerItemView_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            _isPressed = true;
            _positionInBlock = e.GetPosition(Parent);

            if (_transform != null!)
            {
                _positionInBlock = new Point
                (
                    _positionInBlock.X - _transform.X,
                    _positionInBlock.Y - _transform.Y
                );
            }
        }

        private void ReorderContainerItemView_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            _isPressed = false;
        }

        private void ReorderContainerItemView_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            if (!_isPressed || Parent == null)
                return;

            var currentPosition = e.GetPosition(Parent);
            var offsetY = currentPosition.Y - _positionInBlock.Y;

            _transform = new TranslateTransform(0, offsetY);

            if (this.Bounds.Top + offsetY > Parent.Bounds.Top && this.Bounds.Bottom + offsetY < Parent.Bounds.Bottom)
            {
                RenderTransform = _transform;
            }

        }
    }
}
