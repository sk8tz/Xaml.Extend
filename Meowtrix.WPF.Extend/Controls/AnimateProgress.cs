﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Meowtrix.WPF.Extend.Controls
{
    public class AnimateProgress : ProgressBar
    {
        private FrameworkElement PART_Indicator;
        private FrameworkElement PART_Track;

        public InitAnimateFrom InitAnimateFrom
        {
            get { return (InitAnimateFrom)GetValue(InitAnimateFromProperty); }
            set { SetValue(InitAnimateFromProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InitAnimateFrom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InitAnimateFromProperty =
            DependencyProperty.Register(nameof(InitAnimateFrom), typeof(InitAnimateFrom), typeof(AnimateProgress), new PropertyMetadata(InitAnimateFrom.None));

        public double CustomAnimateFrom
        {
            get { return (double)GetValue(CustomAnimateFromProperty); }
            set { SetValue(CustomAnimateFromProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomAnimateFrom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomAnimateFromProperty =
            DependencyProperty.Register(nameof(CustomAnimateFrom), typeof(double), typeof(AnimateProgress), new PropertyMetadata(0.0));

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum) => SetIndicator(Value);

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum) => SetIndicator(Value);

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            DoAnimation(oldValue, newValue);
        }

        private double InRange(double value) => value > Maximum ? Maximum : value < Minimum ? Minimum : value;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Indicator = GetTemplateChild(nameof(PART_Indicator)) as FrameworkElement;
            PART_Track = GetTemplateChild(nameof(PART_Track)) as FrameworkElement;
            switch (InitAnimateFrom)
            {
                case InitAnimateFrom.None:
                    SetIndicator(Value);
                    break;
                case InitAnimateFrom.Minimum:
                    DoAnimation(Minimum, Value);
                    break;
                case InitAnimateFrom.Maximum:
                    DoAnimation(Maximum, Value);
                    break;
                case InitAnimateFrom.Custom:
                    DoAnimation(CustomAnimateFrom, Value);
                    break;
            }
        }

        private void DoAnimation(double fromValue, double toValue)
        {
            if (PART_Track != null && PART_Indicator != null)
            {
                double fromwidth = PART_Track.ActualWidth * (Maximum <= Minimum ? 0 : ((InRange(fromValue) - Minimum) / (Maximum - Minimum)));
                double towidth = PART_Track.ActualWidth * (Maximum <= Minimum ? 0 : ((InRange(toValue) - Minimum) / (Maximum - Minimum)));
                var animation = new DoubleAnimation
                {
                    From = fromwidth,
                    To = towidth,
                    Duration = TimeSpan.FromSeconds(1),
                    EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
                };
                PART_Indicator.BeginAnimation(WidthProperty, animation);
            }
        }

        private void SetIndicator(double value)
        {
            if (PART_Track != null && PART_Indicator != null)
            {
                double rate = Maximum <= Minimum ? 0 : ((InRange(value) - Minimum) / (Maximum - Minimum));
                PART_Indicator.Width = PART_Track.ActualWidth * rate;
            }
        }
    }
    public enum InitAnimateFrom { None, Minimum, Maximum, Custom }
}
