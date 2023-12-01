using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicData.Binding;
using SteamgiftsClient.Services.SiteManager;
using SteamgiftsClient.ViewModels.ReordedContainer;
using System;

namespace SteamgiftsClient.Views.ReordedContainer
{
    public partial class ReorderContainerItemView : UserControl
    {
        private bool _isPressed;
        private Point _positionInBlock;
        private TranslateTransform _transform = null!;

        public ReorderContainerItemViewModel? ViewModel { private set; get; }

        public ReorderContainerItemView()
        {
            InitializeComponent();

            var moveButton = this.FindControl<Button>("MoveButton");
            moveButton.AddHandler(Button.PointerPressedEvent, ReorderContainerItemView_PointerPressed, handledEventsToo: true);
            moveButton.AddHandler(Button.PointerReleasedEvent, ReorderContainerItemView_PointerReleased, handledEventsToo: true);
            PointerMoved += ReorderContainerItemView_PointerMoved;

            DataContextChanged += ReorderContainerItemView_DataContextChanged;

            this.WhenPropertyChanged(x => x.Bounds, false)
                .Subscribe(x => ViewModel?.ParentContainer.ItemsBounds.Add(Bounds));
        }

        public Rect GetCurrentBounds()
        {
            if (_transform == null)
            {
                return Bounds;
            }
            return new Rect(Bounds.X, Bounds.Y + _transform.Y, Bounds.Width, Bounds.Height);
        }

        public void SetTransform(TranslateTransform transform)
        {
            _transform = transform;
            RenderTransform = transform;
        }

        private void ReorderContainerItemView_DataContextChanged(object? sender, System.EventArgs e)
        {
            ViewModel = DataContext as ReorderContainerItemViewModel;
        }

        private void ReorderContainerItemView_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            _isPressed = true;
            _positionInBlock = e.GetPosition(Parent);

            if (_transform != null)
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

            ViewModel?.ParentContainer.SetItemDefaultBounds(this);
        }

        private void ReorderContainerItemView_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            if (!_isPressed || Parent == null)
                return;

            var currentPosition = e.GetPosition(Parent);
            var offsetY = currentPosition.Y - _positionInBlock.Y;

            _transform = new TranslateTransform(0, offsetY);

            // Check we in parent container bounds
            if (this.Bounds.Top + offsetY > Parent.Bounds.Top && this.Bounds.Bottom + offsetY < Parent.Bounds.Bottom)
            {
                RenderTransform = _transform;
            }

            ViewModel?.ParentContainer.CheckIntersections(this);
        }
    }
}
