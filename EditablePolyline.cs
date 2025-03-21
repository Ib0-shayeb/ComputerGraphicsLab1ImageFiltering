using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Collections;
using Avalonia.Controls.Shapes;
using System.Linq;

namespace ComputerGraphicsLab1_ImageFiltering
{
    public class EditablePolyline : Canvas
    {
        public readonly Polyline _polyline;
        public readonly List<Ellipse> _handles = new List<Ellipse>();
        public int _draggingIndex = -1;
        public bool restrictHorizontalRepositioning = false;

        public EditablePolyline(Points points)//pass points
        {
            Width = 255;
            Height = 255;
            Background = Brushes.LightGray;

            // Create Polyline
            _polyline = new Polyline
            {
                Points = points,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.Transparent
            };
            var rectangle = new Rectangle{
                Height = 300,
                Width = 300,
                Fill = Brushes.Blue,
                Stroke = Brushes.Blue,
                StrokeThickness = 2
            };
            

            Children.Add(rectangle);
            Children.Add(_polyline);
            CreateHandles();

            // Mouse Events for dragging
            PointerPressed += OnPointerPressed;
            PointerMoved += OnPointerMoved;
            PointerReleased += OnPointerReleased;
            DoubleTapped += OnPointerDoubleTap;
        }

        private void CreateHandles()
        {
            foreach (var point in _polyline.Points)
            {
                var handle = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Red,
                    [Canvas.LeftProperty] = point.X - 5,
                    [Canvas.TopProperty] = point.Y - 5
                };

                _handles.Add(handle);
                Children.Add(handle);
            }
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var rightClick = e.GetCurrentPoint(this).Properties.IsRightButtonPressed;
            var pos = e.GetPosition(this);

            if (rightClick){
                for (int i = 0; i < _handles.Count; i++)
                {
                    var handle = _handles[i];
                    var handlePos = new Point((double)handle.GetValue(Canvas.LeftProperty) + 5, (double)handle.GetValue(Canvas.TopProperty) + 5);
                    
                    if (Math.Abs(pos.X - handlePos.X) < 10 && Math.Abs(pos.Y - handlePos.Y) < 10 && i > 0 && i < 255)
                    {   
                        Children.Remove(_handles[i]);
                        _handles.RemoveAt(i);
                        _polyline.Points.RemoveAt(i);
                        break;
                    }
                }
            }
            else{
                for (int i = 0; i < _handles.Count; i++)
                {
                    var handle = _handles[i];
                    var handlePos = new Point((double)handle.GetValue(Canvas.LeftProperty) + 5, (double)handle.GetValue(Canvas.TopProperty) + 5);
                    
                    if (Math.Abs(pos.X - handlePos.X) < 10 && Math.Abs(pos.Y - handlePos.Y) < 10)
                    {
                        _draggingIndex = i;
                        break;
                    }
                }
            }
        }
        private void OnPointerDoubleTap(object? sender, TappedEventArgs e)
        {
            var pos = e.GetPosition(this);

            int insertIndex = 0;
            while (insertIndex < _polyline.Points.Count && _polyline.Points[insertIndex].X < pos.X)
            {
                insertIndex++;
            }
            if(insertIndex>=255 || insertIndex < 0) return;
            _polyline.Points.Insert(insertIndex, new Point(pos.X, pos.Y));

            var handle = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Red,
                    [Canvas.LeftProperty] = pos.X - 5,
                    [Canvas.TopProperty] = pos.Y - 5
                };
            _handles.Insert(insertIndex, handle);
            Children.Add(handle);
        }
        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_draggingIndex == -1) return;

            var pos = e.GetPosition(this);

            // Update handle position
            if(_draggingIndex != 0 && _draggingIndex != _handles.Count - 1){
                _handles[_draggingIndex].SetValue(Canvas.LeftProperty, pos.X - 5);
            }
            _handles[_draggingIndex].SetValue(Canvas.TopProperty, pos.Y - 5);

            // Update polyline
            var points = new Points(_polyline.Points);
            if(_draggingIndex != 0 && _draggingIndex != _handles.Count - 1){
                points[_draggingIndex] = pos;
            }
            else {
                points[_draggingIndex] = new Point( points[_draggingIndex].X, pos.Y);
            }
            _polyline.Points = points;
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _draggingIndex = -1;
        }
    }
}