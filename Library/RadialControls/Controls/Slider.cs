﻿/**
 *  RadialControls - A circular controls library for Windows 8 Apps
 *  Copyright (C) Ben Thorner 2015
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
 **/

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Thorner.RadialControls.Utilities.Extensions;
using System;

namespace Thorner.RadialControls.Controls
{
    public class Slider : Control
    {
        #region Events

        public event Action<object, SliderEventArgs> SlideStart, SlideStop;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
            "Offset", typeof(double), typeof(Slider), new PropertyMetadata(0.0));

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle", typeof(double), typeof(Slider), new PropertyMetadata(0.0));

        public static readonly DependencyProperty ThumbProperty = DependencyProperty.Register(
            "Thumb", typeof(ControlTemplate), typeof(Slider), new PropertyMetadata(null));

        #endregion

        public Slider()
        {
            DefaultStyleKey = typeof(Slider);
        }

        #region Properties

        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public ControlTemplate Thumb
        {
            get { return (ControlTemplate)GetValue(ThumbProperty); }
            set { SetValue(ThumbProperty, value); }
        }

        #endregion

        #region UIElement Overrides

        protected override void OnApplyTemplate()
        {
            AddHandler(
                PointerPressedEvent, new PointerEventHandler(StealPointer), true
            );

            AddHandler(
                PointerReleasedEvent, new PointerEventHandler(ReleasePointer), true
            );

            AddHandler(
                PointerCanceledEvent, new PointerEventHandler(ReleasePointer), true
            );

            AddHandler(
                PointerCaptureLostEvent, new PointerEventHandler(ReleasePointer), true
            );

            AddHandler(
                PointerMovedEvent, new PointerEventHandler(UpdateValue), true
            );
        }

        #endregion

        #region Event Handlers

        private void StealPointer(object sender, PointerRoutedEventArgs e)
        {
            if (SlideStart != null)
            {
                SlideStart(this, new SliderEventArgs(Angle));
            }

            CapturePointer(e.Pointer);
        }

        private void UpdateValue(object sender, PointerRoutedEventArgs e)
        {
            if (PointerCaptures == null || PointerCaptures.Count != 1)
            {
                return;
            }

            SetValue(AngleProperty,
                SliderAngle(e) - (double)GetValue(OffsetProperty)
            );
        }

        private void ReleasePointer(object sender, PointerRoutedEventArgs e)
        {
            ReleasePointerCapture(e.Pointer);

            if (SlideStop != null)
            {
                SlideStop(this, new SliderEventArgs(Angle));
            }
        }

        #endregion

        #region Private Members

        private double SliderAngle(PointerRoutedEventArgs e)
        {
            var centre = new Point(
                ActualWidth / 2, ActualHeight / 2
            );

            var thumb = e.GetCurrentPoint(this)
                .Position.RelativeTo(centre);

            var vertical = new Vector(0, -1);
            return thumb.AngleTo(vertical);
        }

        #endregion
    }

    public sealed class SliderEventArgs
    {
        public SliderEventArgs(double angle)
        {
            Angle = angle;
        }

        public double Angle { get; private set; }
    }
}
