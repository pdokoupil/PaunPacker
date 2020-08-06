using PaunPacker.GUI.Views;
using System;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace PaunPacker.GUI.Behaviors
{
    /// <summary>
    /// Implementation of a <see cref="Behavior{T}" /> for <see cref="MainWindow"/>
    /// Used to handle MouseWheel event on <see cref="MainWindow"/> without adding code to the code-behind
    /// </summary>
    /// <remarks>Used for scrolling/zooming the texture atlas</remarks>
    class MouseWheelBehavior : Behavior<MainWindow>
    {
        protected override void OnAttached()
        {
            //AssociatedObject.textureAtlas.MouseWheel += textureAtlas_MouseWheel;
            //textureAtlas element does not have to be instantiated .. at the moment (caused NullExc..), so wait for whole view to get initialized
            AssociatedObject.Initialized += AssociatedObject_Initialized;
        }

        private void AssociatedObject_Initialized(object sender, EventArgs e)
        {
            AssociatedObject.textureAtlas.MouseWheel += TextureAtlas_MouseWheel;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.textureAtlas.MouseWheel -= TextureAtlas_MouseWheel;
        }

        private void TextureAtlas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var mousePos = e.MouseDevice.GetPosition(AssociatedObject.textureAtlas);

            double delta = e.Delta > 0 ? 0.2 : -0.2;

            if (AssociatedObject.scaleTransform.ScaleX + delta >= minScaleX && AssociatedObject.scaleTransform.ScaleX + delta <= maxScaleX)
            {
                AssociatedObject.scaleTransform.ScaleX += delta;
                AssociatedObject.scrollViewer.ScrollToHorizontalOffset(AssociatedObject.scrollViewer.HorizontalOffset + (mousePos.X) * delta);
            }

            if (AssociatedObject.scaleTransform.ScaleY + delta >= minScaleY && AssociatedObject.scaleTransform.ScaleY + delta <= maxScaleY)
            {
                AssociatedObject.scaleTransform.ScaleY += delta;
                AssociatedObject.scrollViewer.ScrollToVerticalOffset(AssociatedObject.scrollViewer.VerticalOffset + (mousePos.Y) * delta);
            }

            e.Handled = true;
        }

        private const double maxScaleX = 4.0;
        private const double maxScaleY = 4.0;
        private const double minScaleX = 0.2;
        private const double minScaleY = 0.2;
    }
}
