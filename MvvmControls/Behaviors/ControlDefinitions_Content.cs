using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace RFBCodeWorks.MvvmControls.Behaviors
{
    public static partial class ControlDefinitions
    {
        /// <summary> </summary>
        public static readonly DependencyProperty FallbackContentProperty =
            DependencyProperty.RegisterAttached(
                "FallbackContent",
                typeof(string),
                typeof(ControlDefinitions),
                new PropertyMetadata(null, FallbackContentPropertyChanged));

        /// <summary> Get <see cref="FallbackContentProperty"/> </summary>
        public static string GetFallbackContent(DependencyObject obj)
        {
            return (string)obj.GetValue(FallbackContentProperty);
        }

        /// <summary> Set <see cref="FallbackContentProperty"/> </summary>
        public static void SetFallbackContent(DependencyObject obj, string value)
        {
            obj.SetValue(FallbackContentProperty, value);
        }

        private static void FallbackContentPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is null && args.OldValue != null)
            {
                //sender.SetValue(BoundContentProperty, null);
            }
            else if (sender is FrameworkElement el)
            {
                // Update the binding's fallback values
                var dp = sender.GetValue(BoundContentProperty) as DependencyProperty;
                if (dp != null)
                {
                    var binding = el.GetBindingExpression(dp);
                    if (binding != null)
                    {
                        binding.ParentBinding.FallbackValue = (string)args.NewValue;
                        binding.ParentBinding.TargetNullValue = (string)args.NewValue;
                    }
                }
            }
        }

        /// <summary> Helper to facility the FallBackContent property changing </summary>
        private static readonly DependencyProperty BoundContentProperty =
            DependencyProperty.RegisterAttached(
                "BoundContent",
                typeof(DependencyProperty),
                typeof(ControlDefinitions));

        /// <summary>
        /// Set the content of the control, as well as its binding mode, and fallback values.
        /// <br/> Fallback values will use <see cref="FallbackContentProperty"/>
        /// </summary>
        /// <param name="cntrl">the control whose property should be set</param>
        /// <param name="cntrlProperty">the dependency property to bind</param>
        /// <param name="source">the source to bind to</param>
        /// <param name="sourceProperty">the name of the source property</param>
        /// <param name="mode">the binding mode - use null for BindingMode.Default</param>
        public static void SetContent(FrameworkElement cntrl, DependencyProperty cntrlProperty, object source, string sourceProperty, BindingMode? mode = BindingMode.OneWay)
        {
            cntrl.SetValue(BoundContentProperty, cntrlProperty);
            //Set the Content if the content is null
            if (cntrl.GetBindingExpression(cntrlProperty) is null)
            {
                BindingOperations.SetBinding(cntrl, cntrlProperty, new Binding(sourceProperty)
                {
                    Source = source,
                    Mode = mode ?? BindingMode.Default,
                    FallbackValue = GetFallbackContent(cntrl),
                    TargetNullValue = GetFallbackContent(cntrl)
                });
            }
        }

        /// <summary>
        /// Set the Content binding of a button control to the DisplayText property
        /// </summary>
        /// <param name="cntrl"></param>
        /// <param name="source"></param>
        public static void SetContent(ButtonBase cntrl, object source)
        {
            SetContent(cntrl, ButtonBase.ContentProperty, source, nameof(IButtonDefinition.DisplayText), BindingMode.OneWay);
        }

        /// <summary>
        /// Set the Content binding of a TextBlock control to the DisplayText property
        /// </summary>
        /// <param name="cntrl"></param>
        /// <param name="source"></param>
        public static void SetContent(TextBlock cntrl, object source)
        {
            SetContent(cntrl, TextBlock.TextProperty, source, nameof(IDisplayTextProvider.DisplayText), BindingMode.OneWay);
        }
    }
}
