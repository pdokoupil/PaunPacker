using PaunPacker.Core.Types;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace PaunPacker.GUI.Behaviors
{
    //Based on: https://www.technical-recipes.com/2017/how-to-get-mouse-cursor-position-in-wpf-mvvm-applications/
    /// <summary>
    /// Implementation of a <see cref="Behavior{T}" /> for <see cref="UserControl"/>
    /// Used to handle MouseDown event on <see cref="UserControl"/> without adding code to the code-behind
    /// </summary>
    /// <remarks>Used by the TextureAtlasView to handle clicks on the individual rectangles within the texture atlas</remarks>
    public class MouseClickBehavior : Behavior<UserControl>
    {
        /// <summary>
        /// Define the dependency property
        /// </summary>
        /// <remarks>Default value to -1, -1 to make sure that upper left rect is not selected</remarks>
        public static readonly DependencyProperty MouseClickXProperty = DependencyProperty.Register("MouseClickPosition", typeof(PPPoint), typeof(MouseClickBehavior), new PropertyMetadata(new PPPoint(-1, -1)));
        
        /// <summary>
        /// The position of mouse click accessible from View's XAML
        /// </summary>
        public PPPoint MouseClickPosition
        {
            get => (PPPoint)GetValue(MouseClickXProperty);
            set => SetValue(MouseClickXProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.MouseDown += AssociatedObject_MouseDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseDown -= AssociatedObject_MouseDown;
        }

        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(AssociatedObject);
            MouseClickPosition = new PPPoint((int)position.X, (int)position.Y);
        }
    }
}
