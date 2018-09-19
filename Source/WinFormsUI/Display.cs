/******************************************************************************
  Copyright 2009-2014 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

using GMap.NET.WindowsForms;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.Commands;
using Dataweb.NShape.Controllers;

namespace Dataweb.NShape.WinFormsUI
{

	/// <summary>
	/// A component used for displaying diagrams.
	/// </summary>
	[Designer(typeof(DisplayDesigner))]
	public partial class Display : GMapControl/*UserControl*/, IDiagramPresenter, IDisplayService
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Display()
		{
			this.SetStyle(
				// enable double buffered painting, transparent background and resize redrawing
				ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.UserPaint
				| ControlStyles.SupportsTransparentBackColor
				| ControlStyles.ResizeRedraw
				// enable focusable container control
				| ControlStyles.ContainerControl | ControlStyles.Selectable
				, true);
			this.UpdateStyles();
			this.TabStop = true;
			infoGraphics = Graphics.FromHwnd(this.Handle);

			// Initialize components
			InitializeComponent();
			AllowDrop = true;
			AutoScroll = false;

			// Set event handlers
#if DEBUG_DIAGNOSTICS
			scrollBarV.ValueChanged += ScrollBar_ValueChanged;
			scrollBarH.ValueChanged += ScrollBar_ValueChanged;
#endif
			scrollBarH.Scroll += ScrollBar_Scroll;
			scrollBarV.Scroll += ScrollBar_Scroll;
			scrollBarH.KeyDown += scrollBar_KeyDown;
			scrollBarV.KeyDown += scrollBar_KeyDown;
			scrollBarH.KeyPress += scrollBar_KeyPress;
			scrollBarV.KeyPress += scrollBar_KeyPress;
			scrollBarH.KeyUp += scrollBar_KeyUp;
			scrollBarV.KeyUp += scrollBar_KeyUp;
			scrollBarH.MouseEnter += ScrollBars_MouseEnter;
			scrollBarV.MouseEnter += ScrollBars_MouseEnter;
			scrollBarH.VisibleChanged += ScrollBar_VisibleChanged;
			scrollBarV.VisibleChanged += ScrollBar_VisibleChanged;
			scrollBarH.GotFocus += scrollBar_GotFocus;
			scrollBarV.GotFocus += scrollBar_GotFocus;
			HScrollBarVisible = VScrollBarVisible = false;
			hScrollBarPanel.BackColor = BackColor;

			// Calculate grip shapes
			CalcControlPointShape(resizePointPath, resizePointShape, handleRadius);
			CalcControlPointShape(rotatePointPath, ControlPointShape.RotateArrow, handleRadius);
			CalcControlPointShape(connectionPointPath, connectionPointShape, handleRadius);
			//
			previewTextFormatter.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces;
			previewTextFormatter.Trimming = StringTrimming.EllipsisCharacter;

			//
			gridSpace = MmToPixels(5);
			gridSize.Width = gridSpace;
			gridSize.Height = gridSpace;
			invalidateDelta = handleRadius;

			// used for fixed refresh rate rendering
			autoScrollTimer.Enabled = false;
			autoScrollTimer.Interval = 10;
			autoScrollTimer.Tick += autoScrollTimer_Tick;

			ShowScrollBars = false;

			#region Map
			// ��������� ��� ���������� GMap
			base.Bearing = 0;

			// CanDragMap - ���� �������� ���������� � True, ������������ ����� ������������� ����� � ������� ������ ������ ����
			base.CanDragMap = true;

			// ���������, ��� �������������� ����� �������������� � �������������� ����� �������� ����. �� ��������� - ������.
			base.DragButton = MouseButtons.Middle;

			base.GrayScaleMode = true;

			// MarkersEnabled - ���� �������� ���������� � True, ����� �������, �������� ������� ����� ��������. ���� ���, ��� �� ��������
			base.MarkersEnabled = true;

			// ��������� �������� ������������� �����������
			base.MaxZoom = 18;

			// ��������� �������� ������������ �����������
			base.MinZoom = 2;

			// ������������� ����� �����������/�������� ������ ����
			base.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;

			// ������������ �� ����������� ������
			base.NegativeMode = false;

			// ��������� ��������
			base.PolygonsEnabled = true;

			//��������� ��������
			base.RoutesEnabled = true;

			// �������� ������� ����� ����� � �����������
			base.ShowTileGridLines = false;

		//?	// ��������� ��� ��� ���� �������� ���������� ������������ � ����� ����������� ��� �������� ����������(������� �����), � �� ������� ���������� ��������������� �������
		//?	base.Dock = DockStyle.Fill;

		//	GMap.NET.MapProviders.GMapProvider.Language = GMap.NET.LanguageType.Russian;
		//	GMap.NET.MapProviders.GMapProvider.Language = GMap.NET.LanguageType.English;

			// ��������� ��� ����� ������������ ����� Google.
			GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;

			// ���� �� ����������� �������� ����� ������ ������, ��������� ���� ������� ������.
			GMap.NET.MapProviders.GMapProvider.WebProxy = System.Net.WebRequest.GetSystemWebProxy();
			GMap.NET.MapProviders.GMapProvider.WebProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

			// ��������� ����������
			//base.Position = new GMap.NET.PointLatLng(54.707042, 20.507221);
			base.Position = new GMap.NET.PointLatLng(54.6496, 21.2746);	// 54.7066, 20.5105	54.6496, 21.2746
			this.lastPosition = base.Position;
			base.Zoom = 10;	// 12	9
			this.lastZoom = base.Zoom;
			ZoomLevel = Display.DefaultValueZoomLevel;

			//base.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
			base.MapProvider = GMap.NET.MapProviders.EmptyProvider.Instance;
			//IsGridVisible = false;
			base.OnPositionChanged += Display_OnPositionChanged;
			base.OnMapZoomChanged += Display_OnMapZoomChanged;
			this.ZoomChanged += Display_ZoomChanged;

//?			GMapOverlay polyOverlay = new GMapOverlay("polygons"); //�������
//?
//?			List<GMap.NET.PointLatLng> points = new List<GMap.NET.PointLatLng>(); //���� � �������
//?			points.Add(new GMap.NET.PointLatLng(54.71729, 20.53862));
//?			points.Add(new GMap.NET.PointLatLng(54.71646, 20.54012));
//?			points.Add(new GMap.NET.PointLatLng(54.71643, 20.54209));
//?			points.Add(new GMap.NET.PointLatLng(54.71558, 20.54456));
//?			points.Add(new GMap.NET.PointLatLng(54.71519, 20.5457));
//?			points.Add(new GMap.NET.PointLatLng(54.71524, 20.54732));
//?			points.Add(new GMap.NET.PointLatLng(54.71659, 20.54926));
//?			points.Add(new GMap.NET.PointLatLng(54.71668, 20.55177));
//?			points.Add(new GMap.NET.PointLatLng(54.7168, 20.55566));
//?			points.Add(new GMap.NET.PointLatLng(54.71664, 20.55632));
//?
//?		//	GMapPolygon polygon = new GMapPolygon(points, "mypolygon"); //������� �������
//?		//	polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
//?		//	polygon.Stroke = new Pen(Color.Red, 1);
//?		//	polyOverlay.Polygons.Add(polygon);
//?
//?			SolidBrush solidBrush = new SolidBrush(Color.FromArgb(80, Color.Red));
//?			GMapRoute route = new GMapRoute(points, "MyRoute");
//?			route.Stroke = new Pen(solidBrush, 5);
//?			polyOverlay.Routes.Add(route);
//?
//?			base.Overlays.Add(polyOverlay);
			#endregion
		}

		/// <summary>
		/// Finalizer
		/// </summary>
		~Display()
		{
			this.ZoomChanged -= Display_ZoomChanged;
			base.OnMapZoomChanged -= Display_OnMapZoomChanged;
			base.OnPositionChanged -= Display_OnPositionChanged;
			Dispose();
		}


		private void Display_OnPositionChanged(GMap.NET.PointLatLng point)
		{
			GMap.NET.GPoint _lastPosition = base.FromLatLngToLocal(this.lastPosition)
				, _currPosition = base.FromLatLngToLocal(base.Position)
				;
			long dx = _lastPosition.X - _currPosition.X, dy = _lastPosition.Y - _currPosition.Y;
			if(Diagram != null && Diagram.Shapes != null && (dx != 0 || dy != 0))
			{	((IDiagramPresenter)this).SuspendUpdate();
				foreach(Shape shape in Diagram.Shapes)
					if(!shape.IsToolbarHidden)
						shape.MoveBy((int)dx, (int)dy);
				((IDiagramPresenter)this).ResumeUpdate();
			}
			this.lastPosition = base.Position;
		}

		private void Display_OnMapZoomChanged()
		{
			if(this.lastZoom == base.Zoom)
				return;
			if(Diagram != null && Diagram.Shapes != null)
			{	Dictionary<ControlPointId, GMap.NET.PointLatLng> points;
				((IDiagramPresenter)this).SuspendUpdate();
				this.isZooming = true;
				try
				{	foreach(Shape shape in Diagram.Shapes)
						if(!shape.IsToolbarHidden
						&& shapesPoints.TryGetValue(shape, out points))
							foreach(KeyValuePair<ControlPointId, GMap.NET.PointLatLng> point in points)
							{	GMap.NET.GPoint pt = base.FromLatLngToLocal(point.Value);
								if(point.Key == ControlPointId.Reference)
									shape.MoveTo((int)pt.X, (int)pt.Y);
								else
									shape.MoveControlPointTo(point.Key, (int)pt.X, (int)pt.Y, ResizeModifiers.None);
							}
				}
				finally
				{
					this.isZooming = false;
					((IDiagramPresenter)this).ResumeUpdate();
				}
			}
			this.lastPosition = base.Position;
			this.lastZoom = base.Zoom;
		}

		private void Display_ZoomChanged(object sender, EventArgs e)
		{
			UpdateDiagramSize();
		}

		private void UpdateDiagramSize()
		{
			if(Diagram != null)
			{
				Rectangle bounds = Rectangle.Empty;
				ControlToDiagram(DrawBounds, out bounds);
				Diagram.Size = new Size(bounds.Width, bounds.Height);
			}
		}

		private void ChangeShapePoints(Shape[] shapes)
		{	foreach(Shape shape in shapes)
				ChangeShapePoints(shape);
		}

		private void ChangeShapePoints(IReadOnlyCollection<Shape> shapes)
		{	foreach(Shape shape in shapes)
				ChangeShapePoints(shape);
		}

		private void ChangeShapePoints(Shape shape)
		{	if(this.isZooming)
				return;

			Dictionary<ControlPointId, GMap.NET.PointLatLng> points;
			if(!shapesPoints.TryGetValue(shape, out points))
				shapesPoints.Add(shape, points = new Dictionary<ControlPointId, GMap.NET.PointLatLng>());

			points[ControlPointId.Reference] = base.FromLocalToLatLng(shape.X, shape.Y);
			foreach(ControlPointId controlPointId in new List<ControlPointId>(shape.GetControlPointIds(ControlPointCapabilities.Resize)))
			{	Point point = shape.GetControlPointPosition(controlPointId);
				points[controlPointId] = base.FromLocalToLatLng(point.X, point.Y);
			}
		}

		private void ClearShapePoints(IReadOnlyCollection<Shape> shapes)
		{	Dictionary<ControlPointId, GMap.NET.PointLatLng> points;
			foreach(Shape shape in shapes)
				if(shapesPoints.TryGetValue(shape, out points))
				{	points.Clear();
					shapesPoints.Remove(shape);
				}
		}

		#region IDisplayService Members (explicit implementation)

		/// <override></override>
		void IDisplayService.Invalidate(int x, int y, int width, int height)
		{
			DoInvalidateDiagram(x, y, width, height);
		}


		/// <override></override>
		void IDisplayService.Invalidate(Rectangle rectangle)
		{
			DoInvalidateDiagram(rectangle);
		}


		/// <override></override>
		void IDisplayService.NotifyBoundsChanged()
		{
			// Nothing to do. Will be removed soon...
		}


		/// <override></override>
		Graphics IDisplayService.InfoGraphics
		{
			get { return infoGraphics; }
		}


		/// <override></override>
		IFillStyle IDisplayService.HintBackgroundStyle
		{
			get
			{
				if(hintBackgroundStyle == null)
				{
					hintBackgroundStyle = new FillStyle("Hint Background Style");
					hintBackgroundStyle.BaseColorStyle = new ColorStyle("Hint Background Color", SelectionInteriorColor);
					hintBackgroundStyle.AdditionalColorStyle = hintBackgroundStyle.BaseColorStyle;
					hintBackgroundStyle.FillMode = FillMode.Solid;
				}
				return hintBackgroundStyle;
			}
		}


		/// <override></override>
		ILineStyle IDisplayService.HintForegroundStyle
		{
			get
			{
				if(hintForegroundStyle == null)
				{
					hintForegroundStyle = new LineStyle("Hint Foreground Line Style");
					hintForegroundStyle.ColorStyle = new ColorStyle("Hint Foreground Color", ToolPreviewColor);
					hintForegroundStyle.DashCap = DashCap.Round;
					hintForegroundStyle.LineJoin = LineJoin.Round;
					hintForegroundStyle.LineWidth = 1;
				}
				return hintForegroundStyle;
			}
		}

		/// <summary>
		/// �����-��������
		/// </summary>
		GMap.NET.MapProviders.GMapProvider IDisplayService.MapProvider
		{
			get { return base.MapProvider; }
			set
			{
				if(base.MapProvider == value)
					return;
				base.MapProvider = value;
				MapProviderChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>���������� ����������� �������</summary>
		int IDisplayService.StaticPropsCount
		{
			get { return this.staticPropsCount; }
			set
			{
				if(this.staticPropsCount == value)
					return;
				this.staticPropsCount = value;
				StaticPropsCountChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>���������� ������������ �������</summary>
		int IDisplayService.DynamicPropsCount
		{
			get { return this.dynamicPropsCount; }
			set
			{
				if(this.dynamicPropsCount == value)
					return;
				this.dynamicPropsCount = value;
				DynamicPropsCountChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		#endregion


		#region IDiagramPresenter Members (explicit implementation)

		/// <override></override>
		[Browsable(false)]
		IDisplayService IDiagramPresenter.DisplayService
		{
			get { return this; }
		}


		///// <override></override>
		//void IDiagramPresenter.NotifyShapesInserted(IEnumerable<Shape> shapes) {
		//   OnShapesInserted(new DiagramPresenterShapesEventArgs(shapes));
		//}


		///// <override></override>
		//void IDiagramPresenter.NotifyShapesRemoved(IEnumerable<Shape> shapes) {
		//   OnShapesRemoved(new DiagramPresenterShapesEventArgs(shapes));
		//}


		/// <override></override>
		int IDiagramPresenter.ZoomedGripSize
		{
			get { return Math.Max(1, (int)Math.Round(handleRadius / zoomfactor)); }
		}


		/// <override></override>
		void IDiagramPresenter.InvalidateDiagram(int x, int y, int width, int height)
		{
			DoInvalidateDiagram(x, y, width, height);
		}


		/// <override></override>
		void IDiagramPresenter.InvalidateDiagram(Rectangle rect)
		{
			DoInvalidateDiagram(rect);
		}


		/// <override></override>
		void IDiagramPresenter.InvalidateGrips(Shape shape, ControlPointCapabilities controlPointCapability)
		{
			DoInvalidateGrips(shape, controlPointCapability);
		}


		/// <override></override>
		void IDiagramPresenter.InvalidateGrips(IEnumerable<Shape> shapes, ControlPointCapabilities controlPointCapability)
		{
			if(shapes == null) throw new ArgumentNullException("shapes");
			Point p = Point.Empty;
			int transformedHandleRadius;
			ControlToDiagram(handleRadius, out transformedHandleRadius);
			++transformedHandleRadius;
			Rectangle r = Rectangle.Empty;
			foreach(Shape shape in shapes)
				r = Geometry.UniteRectangles(shape.GetBoundingRectangle(false), r);
			r.Inflate(transformedHandleRadius, transformedHandleRadius);
			DoInvalidateDiagram(r);
		}


		/// <override></override>
		void IDiagramPresenter.InvalidateSnapIndicators(Shape shape)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			int transformedPointRadius, transformedGridSize;
			ControlToDiagram(GripSize, out transformedPointRadius);
			transformedGridSize = (int)Math.Round(GridSize * (ZoomLevel / 100f));

			Rectangle bounds = shape.GetBoundingRectangle(false);
			Point p = Point.Empty;
			foreach(ControlPointId id in shape.GetControlPointIds(ControlPointCapabilities.All))
			{
				p = shape.GetControlPointPosition(id);
				bounds = Geometry.UniteRectangles(p.X, p.Y, p.X, p.Y, bounds);
			}
			bounds.Inflate(transformedPointRadius, transformedPointRadius);
			DoInvalidateDiagram(bounds);
		}


		/// <override></override>
		void IDiagramPresenter.SuspendUpdate()
		{
			DoSuspendUpdate();
		}


		/// <override></override>
		void IDiagramPresenter.ResumeUpdate()
		{
			DoResumeUpdate();
		}


		/// <override></override>
		void IDiagramPresenter.ResetTransformation()
		{
			DoResetTransformation();
		}


		/// <override></override>
		void IDiagramPresenter.RestoreTransformation()
		{
			DoRestoreTransformation();
		}


		/// <override></override>
		void IDiagramPresenter.Update()
		{
			//Console.WriteLine("[{0}]\t Update called", DateTime.Now.ToString("HH:mm:ss.ffff"));
			Update();
		}


		/// <override></override>
		void IDiagramPresenter.DrawShape(Shape shape)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			if(currentGraphics == null) throw new InvalidOperationException(ErrMessageOnlyAllowedWhilePaining);
			if(!graphicsIsTransformed) throw new InvalidOperationException(ErrMessageRestoreTransformationHasNotBeenCalled);
			shape.Draw(currentGraphics);
		}


		/// <override></override>
		void IDiagramPresenter.DrawShapes(IEnumerable<Shape> shapes)
		{
			if(shapes == null) throw new ArgumentNullException("shapes");
			if(currentGraphics == null) throw new InvalidOperationException(ErrMessageOnlyAllowedWhilePaining);
			if(!graphicsIsTransformed) throw new InvalidOperationException(ErrMessageRestoreTransformationHasNotBeenCalled);
			foreach(Shape shape in shapes)
				shape.Draw(currentGraphics);
		}


		/// <override></override>
		void IDiagramPresenter.DrawResizeGrip(IndicatorDrawMode drawMode, Shape shape, ControlPointId pointId)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			Point p = shape.GetControlPointPosition(pointId);
			DrawResizeGripCore(p.X, p.Y, drawMode);
		}


		/// <override></override>
		void IDiagramPresenter.DrawRotateGrip(IndicatorDrawMode drawMode, Shape shape, ControlPointId pointId)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			Point p = shape.GetControlPointPosition(pointId);
			DrawRotateGripCore(p.X, p.Y, drawMode);
		}


		/// <override></override>
		void IDiagramPresenter.DrawConnectionPoint(IndicatorDrawMode drawMode, Shape shape, ControlPointId pointId)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			Debug.Assert(shape.HasControlPointCapability(pointId, ControlPointCapabilities.Connect | ControlPointCapabilities.Glue));
			// Get control point popsition
			Point p = shape.GetControlPointPosition(pointId);
			// Get connection of control point
			ShapeConnectionInfo connection = ShapeConnectionInfo.Empty;
			if(shape.HasControlPointCapability(pointId, ControlPointCapabilities.Glue))
				connection = shape.GetConnectionInfo(pointId, null);
			// Draw point
			DrawConnectionPointCore(p.X, p.Y, drawMode,
				shape.HasControlPointCapability(pointId, ControlPointCapabilities.Resize | ControlPointCapabilities.Movable),
				connection);
		}


		/// <override></override>
		void IDiagramPresenter.DrawCaptionBounds(IndicatorDrawMode drawMode, ICaptionedShape shape, int captionIndex)
		{
			DoDrawCaptionBounds(drawMode, shape, captionIndex);
		}


		/// <override></override>
		void IDiagramPresenter.DrawShapeOutline(IndicatorDrawMode drawMode, Shape shape)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			if(currentGraphics == null) throw new InvalidOperationException(ErrMessageOnlyAllowedWhilePaining);
			if(!graphicsIsTransformed) throw new NShapeException(ErrMessageRestoreTransformationHasNotBeenCalled);
			DoDrawShapeOutline(drawMode, shape);
		}


		/// <override></override>
		void IDiagramPresenter.DrawSnapIndicators(Shape shape)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			if(currentGraphics == null) throw new InvalidOperationException(ErrMessageOnlyAllowedWhilePaining);
			int left = int.MaxValue;
			int top = int.MaxValue;
			int right = int.MinValue;
			int bottom = int.MinValue;
			int snapIndicatorRadius = handleRadius;

			bool graphicsWasTransformed = graphicsIsTransformed;
			if(graphicsIsTransformed) DoResetTransformation();
			try
			{
				Rectangle shapeBounds = shape.GetBoundingRectangle(true);
				int zoomedGridSize;
				ControlToDiagram(GridSize, out zoomedGridSize);

				bool drawLeft = (shapeBounds.Left % GridSize == 0);
				bool drawTop = (shapeBounds.Top % GridSize == 0);
				bool drawRight = (shapeBounds.Right % GridSize == 0);
				bool drawBottom = (shapeBounds.Bottom % GridSize == 0);

				// transform shape bounds to control coordinates
				DiagramToControl(shapeBounds, out shapeBounds);

				// draw outlines
				if(drawLeft) currentGraphics.DrawLine(outerSnapPen, shapeBounds.Left, shapeBounds.Top - 1, shapeBounds.Left, shapeBounds.Bottom + 1);
				if(drawRight) currentGraphics.DrawLine(outerSnapPen, shapeBounds.Right, shapeBounds.Top - 1, shapeBounds.Right, shapeBounds.Bottom + 1);
				if(drawTop) currentGraphics.DrawLine(outerSnapPen, shapeBounds.Left - 1, shapeBounds.Top, shapeBounds.Right + 1, shapeBounds.Top);
				if(drawBottom) currentGraphics.DrawLine(outerSnapPen, shapeBounds.Left - 1, shapeBounds.Bottom, shapeBounds.Right + 1, shapeBounds.Bottom);
				// fill interior
				if(drawLeft) currentGraphics.DrawLine(innerSnapPen, shapeBounds.Left, shapeBounds.Top, shapeBounds.Left, shapeBounds.Bottom);
				if(drawRight) currentGraphics.DrawLine(innerSnapPen, shapeBounds.Right, shapeBounds.Top, shapeBounds.Right, shapeBounds.Bottom);
				if(drawTop) currentGraphics.DrawLine(innerSnapPen, shapeBounds.Left, shapeBounds.Top, shapeBounds.Right, shapeBounds.Top);
				if(drawBottom) currentGraphics.DrawLine(innerSnapPen, shapeBounds.Left, shapeBounds.Bottom, shapeBounds.Right, shapeBounds.Bottom);

				foreach(ControlPointId id in shape.GetControlPointIds(ControlPointCapabilities.All))
				{
					Point p = Point.Empty;
					p = shape.GetControlPointPosition(id);

					// check if the point is on a gridline
					bool hGridLineContainsPoint = p.X % (GridSize * zoomfactor) == 0;
					bool vGridLineContainsPoint = p.Y % (GridSize * zoomfactor) == 0;
					// collect coordinates for bounding box
					if(p.X < left) left = p.X;
					if(p.X > right) right = p.X;
					if(p.Y < top) top = p.Y;
					if(p.Y > bottom) bottom = p.Y;

					if(hGridLineContainsPoint || vGridLineContainsPoint)
					{
						DiagramToControl(p, out p);
						currentGraphics.FillEllipse(HandleInteriorBrush, p.X - snapIndicatorRadius, p.Y - snapIndicatorRadius, snapIndicatorRadius * 2, snapIndicatorRadius * 2);
						currentGraphics.DrawEllipse(innerSnapPen, p.X - snapIndicatorRadius, p.Y - snapIndicatorRadius, snapIndicatorRadius * 2, snapIndicatorRadius * 2);
					}
				}
			}
			finally
			{
				if(graphicsWasTransformed) DoRestoreTransformation();
			}
		}


		/// <override></override>
		void IDiagramPresenter.DrawSelectionFrame(Rectangle frameRect)
		{
			if(currentGraphics == null) throw new InvalidOperationException(ErrMessageOnlyAllowedWhilePaining);
			if(graphicsIsTransformed) throw new NShapeException(ErrMessageResetTransformHasNotBeenCalled);
			DiagramToControl(frameRect, out rectBuffer);
			if(HighQualityRendering)
			{
				currentGraphics.FillRectangle(ToolPreviewBackBrush, rectBuffer);
				currentGraphics.DrawRectangle(ToolPreviewPen, rectBuffer);
			}
			else
			{
				ControlPaint.DrawLockedFrame(currentGraphics, rectBuffer, false);
				//ControlPaint.DrawFocusRectangle(graphics, rectBuffer, Color.White, Color.Black);
			}
		}


		/// <override></override>
		void IDiagramPresenter.DrawAnglePreview(Point center, Point mousePos, int cursorId, int startAngle, int sweepAngle)
		{
			if(currentGraphics == null) throw new InvalidOperationException(ErrMessageOnlyAllowedWhilePaining);
			if(graphicsIsTransformed) throw new NShapeException(ErrMessageResetTransformHasNotBeenCalled);
			// Get cursor size
			Size cursorSize = registeredCursors[cursorId].Size;
			// transform diagram coordinates to control coordinates
			int radius = (int)Math.Round(Geometry.DistancePointPoint(center, mousePos));
			DiagramToControl(radius, out radius);
			DiagramToControl(center, out center);
			DiagramToControl(mousePos, out mousePos);
			// Check if the cursor has the minimum distance from the rotation point
			if(radius > minRotateDistance)
			{
				// Calculate angle and angle info text
				float startAngleDeg = Geometry.TenthsOfDegreeToDegrees(startAngle);
				float sweepAngleDeg = Geometry.TenthsOfDegreeToDegrees(sweepAngle <= 1800 ? sweepAngle : (sweepAngle - 3600));

				string anglePrefix;
				if(sweepAngleDeg == 0) anglePrefix = string.Empty;
				else if(sweepAngleDeg < 0) anglePrefix = "-";
				else anglePrefix = "+";
				string angleInfoText = null;
				if(SelectedShapes.Count == 1 && SelectedShapes.TopMost is IPlanarShape)
				{
					float shapeAngleDeg = Geometry.TenthsOfDegreeToDegrees(((IPlanarShape)SelectedShapes.TopMost).Angle);
					angleInfoText = string.Format("{0}� ({1}� {2} {3}�)", (360 + shapeAngleDeg + sweepAngleDeg) % 360, shapeAngleDeg, anglePrefix, Math.Abs(sweepAngleDeg));
				}
				else angleInfoText = string.Format("{0}{1}�", anglePrefix, Math.Abs(sweepAngleDeg));

				// Calculate size of the text's layout rectangle
				Rectangle layoutRect = Rectangle.Empty;
				layoutRect.Size = TextMeasurer.MeasureText(currentGraphics, angleInfoText, Font, Size.Empty, previewTextFormatter);
				layoutRect.Width = Math.Min((int)Math.Round(radius * 1.5), layoutRect.Width);
				// Calculate the circumcircle of the LayoutRectangle and the distance between mouse and rotation center...
				float circumCircleRadius = Geometry.DistancePointPoint(-cursorSize.Width / 2f, -cursorSize.Height / 2f, layoutRect.Width, layoutRect.Height) / 2f;
				float mouseDistance = Math.Max(Geometry.DistancePointPoint(center, mousePos), 0.0001f);
				float interpolationFactor = circumCircleRadius / mouseDistance;
				// ... then transform the layoutRectangle towards the mouse cursor
				PointF textCenter = Geometry.VectorLinearInterpolation((PointF)mousePos, (PointF)center, interpolationFactor);
				layoutRect.X = (int)Math.Round(textCenter.X - (layoutRect.Width / 2f));
				layoutRect.Y = (int)Math.Round(textCenter.Y - (layoutRect.Height / 2f));

				// Draw angle pie
				int pieSize = radius + radius;
				if(HighQualityRendering)
				{
					currentGraphics.DrawEllipse(ToolPreviewPen, center.X - radius, center.Y - radius, pieSize, pieSize);
					currentGraphics.FillPie(ToolPreviewBackBrush, center.X - radius, center.Y - radius, pieSize, pieSize, startAngleDeg, sweepAngleDeg);
					currentGraphics.DrawPie(ToolPreviewPen, center.X - radius, center.Y - radius, pieSize, pieSize, startAngleDeg, sweepAngleDeg);
				}
				else
				{
					currentGraphics.DrawPie(Pens.Black, center.X - radius, center.Y - radius, pieSize, pieSize, startAngleDeg, sweepAngleDeg);
					currentGraphics.DrawPie(Pens.Black, center.X - radius, center.Y - radius, pieSize, pieSize, startAngleDeg, sweepAngleDeg);
				}
				currentGraphics.DrawString(angleInfoText, Font, Brushes.Black, layoutRect, previewTextFormatter);
			}
			else
			{
				// If cursor is nearer to the rotation point that the required distance,
				// draw rotation instuction preview
				if(HighQualityRendering)
				{
					float diameter = minRotateDistance + minRotateDistance;
					// draw angle preview circle
					currentGraphics.DrawEllipse(ToolPreviewPen, center.X - minRotateDistance, center.Y - minRotateDistance, diameter, diameter);
					currentGraphics.FillPie(ToolPreviewBackBrush, center.X - minRotateDistance, center.Y - minRotateDistance, diameter, diameter, 0, 45f);
					currentGraphics.DrawPie(ToolPreviewPen, center.X - minRotateDistance, center.Y - minRotateDistance, diameter, diameter, 0, 45f);

					// Draw rotation direction arrow line
					int bowInsetX, bowInsetY;
					bowInsetX = bowInsetY = minRotateDistance / 4;
					currentGraphics.DrawArc(ToolPreviewPen, center.X - minRotateDistance + bowInsetX, center.Y - minRotateDistance + bowInsetY, diameter - (2 * bowInsetX), diameter - (2 * bowInsetY), 0, 22.5f);
					// Calculate Arrow Tip
					int arrowTipX = 0; int arrowTipY = 0;
					arrowTipX = center.X + minRotateDistance - bowInsetX;
					arrowTipY = center.Y;
					Geometry.RotatePoint(center.X, center.Y, 45f, ref arrowTipX, ref arrowTipY);
					arrowShape[0].X = arrowTipX;
					arrowShape[0].Y = arrowTipY;
					//
					arrowTipX = center.X + minRotateDistance - bowInsetX - GripSize - GripSize;
					arrowTipY = center.Y;
					Geometry.RotatePoint(center.X, center.Y, 22.5f, ref arrowTipX, ref arrowTipY);
					arrowShape[1].X = arrowTipX;
					arrowShape[1].Y = arrowTipY;
					//
					arrowTipX = center.X + minRotateDistance - bowInsetX + GripSize + GripSize;
					arrowTipY = center.Y;
					Geometry.RotatePoint(center.X, center.Y, 22.5f, ref arrowTipX, ref arrowTipY);
					arrowShape[2].X = arrowTipX;
					arrowShape[2].Y = arrowTipY;
					// Draw arrow tip
					currentGraphics.FillPolygon(ToolPreviewBackBrush, arrowShape);
					currentGraphics.DrawPolygon(ToolPreviewPen, arrowShape);
				}
				else currentGraphics.DrawPie(Pens.Black, center.X - minRotateDistance, center.Y - minRotateDistance, 2 * minRotateDistance, 2 * minRotateDistance, 0, 45f);
			}
		}


		/// <override></override>
		void IDiagramPresenter.DrawLine(Point a, Point b)
		{
			if(currentGraphics == null) throw new InvalidOperationException(ErrMessageOnlyAllowedWhilePaining);
			if(graphicsIsTransformed) throw new NShapeException(ErrMessageResetTransformHasNotBeenCalled);
			DiagramToControl(a, out a);
			DiagramToControl(b, out b);
			currentGraphics.DrawLine(outerSnapPen, a, b);
			currentGraphics.DrawLine(innerSnapPen, a, b);
		}


		/// <override></override>
		void IDiagramPresenter.OpenCaptionEditor(ICaptionedShape shape, int x, int y)
		{
			DoOpenCaptionEditor(shape, x, y);
		}


		/// <override></override>
		void IDiagramPresenter.OpenCaptionEditor(ICaptionedShape shape, int labelIndex)
		{
			DoOpenCaptionEditor(shape, labelIndex, string.Empty);
		}


		/// <override></override>
		void IDiagramPresenter.OpenCaptionEditor(ICaptionedShape shape, int labelIndex, string newText)
		{
			DoOpenCaptionEditor(shape, labelIndex, newText);
		}


		/// <override></override>
		void IDiagramPresenter.CloseCaptionEditor(bool applyChanges)
		{
			DoCloseCaptionEditor(applyChanges);
		}


		/// <override></override>
		void IDiagramPresenter.SetCursor(int cursorId)
		{
			// If cursor was not loaded yet, load it now
			if(!registeredCursors.ContainsKey(cursorId))
				LoadRegisteredCursor(cursorId);
			if(registeredCursors[cursorId] != Cursor)
				Cursor = registeredCursors[cursorId] ?? Cursors.Default;
		}

		#endregion


		#region [Public] IDiagramPresenter Events

		/// <override></override>
		public event EventHandler ShapesSelected;

		/// <override></override>
		public event EventHandler<DiagramPresenterShapeClickEventArgs> ShapeClick;

		/// <override></override>
		public event EventHandler<DiagramPresenterShapeClickEventArgs> ShapeDoubleClick;

		/// <override></override>
		public event EventHandler<DiagramPresenterShapesEventArgs> ShapesInserted;

		/// <override></override>
		public event EventHandler<DiagramPresenterShapesEventArgs> ShapesRemoved;

		/// <override></override>
		public event EventHandler<DiagramPresenterShapeEventArgs> ShapeMoved;

		/// <override></override>
		public event EventHandler<DiagramPresenterShapeEventArgs> ShapeResized;

		/// <override></override>
		public event EventHandler<DiagramPresenterShapeEventArgs> ShapeRotated;

		/// <override></override>
		public event EventHandler<LayersEventArgs> LayerVisibilityChanged;

		/// <override></override>
		public event EventHandler<LayersEventArgs> ActiveLayersChanged;

		/// <override></override>
		public event EventHandler ZoomChanged;

		/// <override></override>
		public event EventHandler DiagramChanging;

		/// <override></override>
		public event EventHandler DiagramChanged;

		/// <override></override>
		public event EventHandler<UserMessageEventArgs> UserMessage;

		/// <summary></summary>
		public event EventHandler MapProviderChanged;

		/// <summary></summary>
		public event EventHandler StaticPropsCountChanged;

		/// <summary></summary>
		public event EventHandler DynamicPropsCountChanged;
		#endregion


		#region [Public] IDiagramPresenter Properties

		/// <override></override>
		[CategoryNShape]
		public DiagramSetController DiagramSetController
		{
			get { return diagramSetController; }
			set
			{
				if(diagramSetController != null)
				{
					UnregisterDiagramSetControllerEvents();
					privateTool = diagramSetController.ActiveTool;
				}
				diagramSetController = value;
				if(diagramSetController != null)
				{
					RegisterDiagramSetControllerEvents();
					if(privateTool != null)
						diagramSetController.ActiveTool = privateTool;
				}
			}
		}


		/// <override></override>
		[ReadOnly(true)]
		[Browsable(false)]
		[CategoryNShape]
		public Diagram Diagram
		{
			get { return diagramController == null ? null : diagramController.Diagram; }
			set
			{
				if(diagramSetController == null) throw new ArgumentNullException("DiagramSetController");
				OnDiagramChanging(EventArgs.Empty);
				if(Diagram != null)
					DiagramController = null;   // Close diagram and unregister events
				if(value != null)
					DiagramController = diagramSetController.OpenDiagram(value);    // Register events
				OnDiagramChanged(EventArgs.Empty);
			}
		}


		/// <summary>
		/// Provides access to a <see cref="T:Dataweb.NShape.Project" />.
		/// </summary>
		[Browsable(false)]
		public Project Project
		{
			get { return (diagramSetController == null) ? null : diagramSetController.Project; }
		}


		/// <override></override>
		[Browsable(false)]
		public IShapeCollection SelectedShapes
		{
			get { return selectedShapes; }
		}


		/// <override></override>
		[Browsable(false)]
		public LayerIds ActiveLayers
		{
			get { return activeLayers; }
		}


		/// <override></override>
		[Browsable(false)]
		public LayerIds HiddenLayers
		{
			get { return hiddenLayers; }
		}

		#endregion


		#region [Public] Properties

		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[CategoryNShape()]
		[Browsable(true)]
		public new string ProductVersion
		{
			get { return base.ProductVersion; }
		}


		/// <summary>
		/// The PropertyController for editing shapes, model objects and diagrams.
		/// </summary>
		[CategoryNShape()]
		public PropertyController PropertyController
		{
			get { return propertyController; }
			set { propertyController = value; }
		}


		/// <summary>
		/// The currently active tool.
		/// </summary>
		[ReadOnly(true)]
		[Browsable(false)]
		[CategoryNShape()]
		public Tool ActiveTool
		{
			get { return (diagramSetController == null) ? privateTool : diagramSetController.ActiveTool; }
			set
			{
				if(diagramSetController != null) diagramSetController.ActiveTool = value;
				else privateTool = value;
				if(ActiveTool.MinimumDragDistance != SystemInformation.DoubleClickSize)
					ActiveTool.MinimumDragDistance = SystemInformation.DoubleClickSize;
				if(ActiveTool.DoubleClickTime != SystemInformation.DoubleClickTime)
					ActiveTool.DoubleClickTime = SystemInformation.DoubleClickTime;
			}
		}


		/// <summary>
		/// The currently active tool.
		/// </summary>
		[Obsolete("Use ActiveTool instead")]
		[ReadOnly(true)]
		[Browsable(false)]
		public Tool CurrentTool
		{
			get { return ActiveTool; }
			set { ActiveTool = value; }
		}


		/// <summary>
		/// Bounds of the display's drawing area (client area minus scroll bars) in control coordinates
		/// </summary>
		[Browsable(false)]
		public Rectangle DrawBounds
		{
			get
			{
				if(!Geometry.IsValid(drawBounds))
				{
					drawBounds.X = drawBounds.Y = 0;
					if(VScrollBarVisible) drawBounds.Width = Width - scrollBarV.Width;
					else drawBounds.Width = Width;
					if(HScrollBarVisible) drawBounds.Height = Height - scrollBarH.Height;
					else drawBounds.Height = Height;
				}
				return drawBounds;
			}
		}


		/// <override></override>
		public override ContextMenuStrip ContextMenuStrip
		{
			get
			{
				ContextMenuStrip result = base.ContextMenuStrip;
				if(result == null && ShowDefaultContextMenu)
					result = displayContextMenuStrip;
				return result;
			}
			set
			{
				base.ContextMenuStrip = value;
			}
		}

		#endregion


		#region [Public] Properties: Behavior

		/// <summary>
		/// Specifies if MenuItemDefs that are not granted should appear as MenuItems in the dynamic context menu.
		/// </summary>
		[CategoryBehavior()]
		[DefaultValue(Display.DefaultValueHideMenuItemsIfNotGranted)]
		public bool HideDeniedMenuItems
		{
			get { return hideMenuItemsIfNotGranted; }
			set { hideMenuItemsIfNotGranted = value; }
		}


		/// <summary>
		/// Enables or disables direct zooming with mouse wheel.
		/// True means zooming when rotating the mouse wheel,
		/// False means zooming when pressing Ctrl and rotating the mouse wheel.
		/// </summary>
		[CategoryBehavior()]
		[DefaultValue(Display.DefaultValueZoomWithMouseWheel)]
		public bool ZoomWithMouseWheel
		{
			get { return zoomWithMouseWheel; }
			set { zoomWithMouseWheel = value; }
		}


		/// <summary>
		/// Shows or hides scroll bars.
		/// </summary>
		[CategoryBehavior()]
		[DefaultValue(Display.DefaultValueShowScrollBars)]
		public bool ShowScrollBars
		{
			get { return showScrollBars; }
			set { showScrollBars = value; }
		}


		/// <summary>
		/// Enables or disables snapping of shapes and control points to grid lines.
		/// </summary>
		[CategoryBehavior()]
		[DefaultValue(Display.DefaultValueSnapToGrid)]
		public bool SnapToGrid
		{
			get { return ((Control.ModifierKeys & (Keys.Control | Keys.ControlKey)) == 0) ? snapToGrid : !snapToGrid; }
			set { snapToGrid = value; }
		}


		/// <summary>
		/// Specifies the distance for snapping shapes and control points to grid lines.
		/// </summary>
		[CategoryBehavior()]
		[DefaultValue(Display.DefaultValueSnapDistance)]
		public int SnapDistance
		{
			get { return snapDistance; }
			set { snapDistance = value; }
		}


		/// <summary>
		/// If true, the standard context menu is created from MenuItemDefs. 
		/// If false, a user defined context menu is shown without creating additional menu items.
		/// </summary>
		[CategoryBehavior()]
		[DefaultValue(Display.DefaultValueShowDefaultContextMenu)]
		public bool ShowDefaultContextMenu
		{
			get { return showDefaultContextMenu; }
			set { showDefaultContextMenu = value; }
		}


		/// <summary>
		/// Specifies the minimum distance of the mouse cursor from the shape's rotate point while rotating.
		/// </summary>
		[CategoryBehavior()]
		[DefaultValue(Display.DefaultValueMinRotateDistance)]
		public int MinRotateRange
		{
			get { return minRotateDistance; }
			set { minRotateDistance = value; }
		}

		#endregion


		#region [Public] Properties: Appearance

		/// <summary>
		/// Zoom in percentage.
		/// </summary>
		[CategoryAppearance()]
		[DefaultValue(Display.DefaultValueZoomLevel)]
		public int ZoomLevel
		{
			get { return zoomLevel; }
			set
			{
				if(value < 0) throw new NShapeException("Unsupported value: Value has to be greater than 0.");
				if(zoomLevel != value)
				{
					zoomLevel = value;
					zoomfactor = Math.Max(value / 100f, 0.001f);

					UnselectShapesOfInvisibleLayers();
					DoCloseCaptionEditor(true);

					ResetBounds();
					UpdateScrollBars();
					Invalidate();

					OnZoomChanged(EventArgs.Empty);
				}
			}
		}


		/// <summary>
		/// Specifies the distance between the grid lines.
		/// </summary>
		[CategoryAppearance()]
		[DefaultValue(Display.DefaultValueGridSize)]
		public int GridSize
		{
			get { return this.gridSpace; }
			set
			{
				if(value <= 0)
					throw new Exception("Value has to be > 0.");
				gridSpace = value;
				gridSize.Width = gridSpace;
				gridSize.Height = gridSpace;
			}
		}


		/// <summary>
		/// The radius of a control point grip from the center to the outer handle bound.
		/// </summary>
		[CategoryAppearance()]
		[DefaultValue(Display.DefaultValueGripSize)]
		public int GripSize
		{
			get { return handleRadius; }
			set
			{
				if(value <= 0) throw new ArgumentOutOfRangeException("value");
				else
				{
					handleRadius = value;
					invalidateDelta = handleRadius;

					CalcControlPointShape(rotatePointPath, ControlPointShape.RotateArrow, handleRadius);
					CalcControlPointShape(resizePointPath, resizePointShape, handleRadius);
					CalcControlPointShape(connectionPointPath, connectionPointShape, handleRadius);
					Invalidate();
				}
			}
		}


		/// <summary>Will soon be removed. Use IsGridVisible instead.</summary>
		[Obsolete("Use IsGridVisible instead")]
		[Browsable(false)]
		[ReadOnly(true)]
		public bool ShowGrid
		{
			get { return IsGridVisible; }
			set { IsGridVisible = value; }
		}


		/// <summary>
		/// Specifies whether grid lines should be visible.
		/// </summary>
		[CategoryAppearance()]
		[DefaultValue(Display.DefaultValueShowGrid)]
		public bool IsGridVisible
		{
			get { return gridVisible; }
			set
			{
				gridVisible = value;
				Invalidate(this.drawBounds);
			}
		}


		/// <summary>Specifies whether the diagram sheet (including its shadow) is visible or not.</summary>
		[CategoryAppearance()]
		[DefaultValue(true)]
		public bool IsSheetVisible
		{
			get { return drawDiagramSheet; }
			set
			{
				drawDiagramSheet = value;
				Invalidate();
			}
		}


#if DEBUG_UI

		/// <summary>Will soon be removed. Use IsDebugInfoInvalidateVisible instead.</summary>
		[Obsolete("Use IsDebugInfoInvalidateVisible instead.")]
		[Browsable(false)]
		[ReadOnly(true)]
		public bool ShowInvalidatedAreas
		{
			get { return IsDebugInfoInvalidateVisible; }
			set { IsDebugInfoInvalidateVisible = value; }
		}


		/// <summary>
		/// Specifies whether grid lines should be visible.
		/// </summary>
		[CategoryAppearance()]
		[Browsable(false)]
		[ReadOnly(true)]
		public bool IsDebugInfoInvalidateVisible
		{
			get { return isInvalidateAreasVisible; }
			set
			{
				isInvalidateAreasVisible = value;
				if(isInvalidateAreasVisible)
				{
					if(clipRectBrush1 == null) clipRectBrush1 = new SolidBrush(invalidatedAreaColor1);
					if(clipRectBrush2 == null) clipRectBrush2 = new SolidBrush(invalidatedAreaColor2);
					if(invalidatedAreaPen1 == null) invalidatedAreaPen1 = new Pen(invalidatedAreaColor1);
					if(invalidatedAreaPen2 == null) invalidatedAreaPen2 = new Pen(invalidatedAreaColor2);
					invalidatedAreas = new List<Rectangle>();
				}
				else
				{
					clipRectBrush = null;
					invalidatedAreaPen = null;
					GdiHelpers.DisposeObject(ref clipRectBrush1);
					GdiHelpers.DisposeObject(ref clipRectBrush2);
					GdiHelpers.DisposeObject(ref invalidatedAreaPen1);
					GdiHelpers.DisposeObject(ref invalidatedAreaPen2);
					if(invalidatedAreas != null)
					{
						invalidatedAreas.Clear();
						invalidatedAreas = null;
					}
				}
				Invalidate();
			}
		}


		/// <summary>Will soon be removed. Use IsDebugInfoCellOccupationVisible instead.</summary>
		[Obsolete("Use IsDebugInfoCellOccupationVisible instead.")]
		[Browsable(false)]
		[ReadOnly(true)]
		public bool ShowCellOccupation
		{
			get { return IsDebugInfoCellOccupationVisible; }
			set { IsDebugInfoCellOccupationVisible = value; }
		}


		/// <summary>
		/// Specifies whether grid lines should be visible.
		/// </summary>
		[CategoryAppearance()]
		[Browsable(false)]
		[ReadOnly(true)]
		public bool IsDebugInfoCellOccupationVisible
		{
			get { return isCellOccupationVisible; }
			set
			{
				isCellOccupationVisible = value;
				Invalidate();
			}
		}

#endif


		/// <summary>
		/// Specifies whether high quality rendering settings should be allpied.
		/// </summary>
		[CategoryAppearance()]
		[DefaultValue(Display.DefaultValueHighQualityRendering)]
		public bool HighQualityRendering
		{
			get { return highQualityRendering; }
			set
			{
				highQualityRendering = value;
				currentRenderingQuality = highQualityRendering ? renderingQualityHigh : renderingQualityLow;
				DisposeObject(ref controlBrush);
				if(infoGraphics != null) GdiHelpers.ApplyGraphicsSettings(infoGraphics, currentRenderingQuality);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies whether the control's background should bew rendered in high quality.
		/// </summary>
		[CategoryAppearance()]
		[DefaultValue(Display.DefaultValueHighQualityBackground)]
		public bool HighQualityBackground
		{
			get { return highQualityBackground; }
			set
			{
				highQualityBackground = value;
				if(Diagram != null)
					Diagram.HighQualityRendering = value;
				DisposeObject(ref controlBrush);
				if(infoGraphics != null) GdiHelpers.ApplyGraphicsSettings(infoGraphics, currentRenderingQuality);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the shape of grips used for resizing shapes.
		/// </summary>
		[CategoryAppearance()]
		[DefaultValue(Display.DefaultValueResizePointShape)]
		public ControlPointShape ResizeGripShape
		{
			get { return resizePointShape; }
			set
			{
				resizePointShape = value;
				CalcControlPointShape(resizePointPath, resizePointShape, handleRadius);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the shape of connection points provided by a shape.
		/// </summary>
		[CategoryAppearance()]
		[DefaultValue(Display.DefaultValueConnectionPointShape)]
		public ControlPointShape ConnectionPointShape
		{
			get { return connectionPointShape; }
			set
			{
				connectionPointShape = value;
				CalcControlPointShape(connectionPointPath, connectionPointShape, handleRadius);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the angle of the background color gradient.
		/// </summary>
		[CategoryAppearance()]
		[DefaultValue(Display.DefaultValueBackgroundGradientAngle)]
		public int BackgroundGradientAngle
		{
			get { return controlBrushGradientAngle; }
			set
			{
				int x = Math.Abs(value) / 360;
				controlBrushGradientAngle = (((x * 360) + value) % 360);
				controlBrushGradientSin = Math.Sin(Geometry.DegreesToRadians(controlBrushGradientAngle));
				controlBrushGradientCos = Math.Cos(Geometry.DegreesToRadians(controlBrushGradientAngle));
				DisposeObject(ref controlBrush);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the control's background color. This defines also the base color of the color gradient.
		/// </summary>
		[CategoryAppearance()]
		//[DefaultValue(Display.DefaultValueBackColor)]
		public override Color BackColor
		{
			get { return base.BackColor; }
			set
			{
				base.BackColor = value;
				hScrollBarPanel.BackColor = value;
				DisposeObject(ref controlBrush);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the gradient color of the background color gradient.
		/// </summary>
		[CategoryAppearance()]
		//[DefaultValue(Display.DefaultValueBackColorGradient)]
		public Color BackColorGradient
		{
			get { return gradientBackColor; }
			set
			{
				gradientBackColor = value;
				DisposeObject(ref controlBrush);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the transparency of the grid lines.
		/// </summary>
		[CategoryAppearance()]
		[DefaultValue(Display.DefaultValueGridAlpha)]
		public byte GridAlpha
		{
			get { return gridAlpha; }
			set
			{
				gridAlpha = value;
				DisposeObject(ref gridPen);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the transparenc of control point grips.
		/// </summary>
		[CategoryAppearance()]
		[DefaultValue(Display.DefaultValueSelectionAlpha)]
		public byte ControlPointAlpha
		{
			get { return selectionAlpha; }
			set
			{
				selectionAlpha = value;

				DisposeObject(ref handleInteriorBrush);
				DisposeObject(ref outlineNormalPen);
				DisposeObject(ref outlineHilightPen);
				DisposeObject(ref outlineInactivePen);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the color of the grid lines.
		/// </summary>
		[CategoryAppearance()]
		public Color GridColor
		{
			get { return gridColor; }
			set
			{
				gridColor = value;
				DisposeObject(ref gridPen);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the inner color of the selection indicator.
		/// </summary>
		[CategoryAppearance()]
		public Color SelectionInteriorColor
		{
			get { return selectionInteriorColor; }
			set
			{
				if(hintBackgroundStyle != null)
				{
					ToolCache.NotifyStyleChanged(hintBackgroundStyle);
					hintBackgroundStyle = null;
				}
				selectionInteriorColor = value;

				DisposeObject(ref outlineInteriorPen);
				DisposeObject(ref handleInteriorBrush);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the color of the selection indicator.
		/// </summary>
		[CategoryAppearance()]
		public Color SelectionNormalColor
		{
			get { return selectionNormalColor; }
			set
			{
				selectionNormalColor = value;
				DisposeObject(ref outlineNormalPen);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the color of a highlighted selection.
		/// </summary>
		[CategoryAppearance()]
		public Color SelectionHilightColor
		{
			get { return selectionHilightColor; }
			set
			{
				selectionHilightColor = value;
				DisposeObject(ref outlineNormalPen);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the color of an inactive/deactivated selection.
		/// </summary>
		[CategoryAppearance()]
		public Color SelectionInactiveColor
		{
			get { return selectionInactiveColor; }
			set
			{
				selectionInactiveColor = value;
				DisposeObject(ref outlineNormalPen);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the color of tool preview hints.
		/// </summary>
		[CategoryAppearance()]
		public Color ToolPreviewColor
		{
			get { return toolPreviewColor; }
			set
			{
				if(hintForegroundStyle != null)
				{
					ToolCache.NotifyStyleChanged(hintForegroundStyle);
					hintForegroundStyle = null;
				}
				toolPreviewColor = value;
				DisposeObject(ref toolPreviewPen);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the color the background color of tool preview hints.
		/// </summary>
		[CategoryAppearance()]
		public Color ToolPreviewBackColor
		{
			get { return toolPreviewBackColor; }
			set
			{
				toolPreviewBackColor = value;
				DisposeObject(ref toolPreviewBackBrush);
				Invalidate();
			}
		}


		/// <summary>
		/// Specifies the rendering quality in high quality mode
		/// </summary>
		[CategoryAppearance()]
		[DefaultValue(Display.DefaultValueRenderingQualityHigh)]
		public RenderingQuality RenderingQualityHighQuality
		{
			get { return renderingQualityHigh; }
			set
			{
				renderingQualityHigh = value;
				if(highQualityRendering)
				{
					currentRenderingQuality = renderingQualityHigh;
					if(infoGraphics != null) GdiHelpers.ApplyGraphicsSettings(infoGraphics, currentRenderingQuality);
				}
			}
		}


		/// <summary>
		/// Specifies the rendering quality in low quality mode
		/// </summary>
		[CategoryAppearance()]
		[DefaultValue(Display.DefaultValueRenderingQualityLow)]
		public RenderingQuality RenderingQualityLowQuality
		{
			get { return renderingQualityLow; }
			set
			{
				renderingQualityLow = value;
				if(!highQualityRendering)
				{
					currentRenderingQuality = renderingQualityLow;
					if(infoGraphics != null) GdiHelpers.ApplyGraphicsSettings(infoGraphics, currentRenderingQuality);
				}
			}
		}

		#endregion


		#region [Public] Methods: Coordinate transformation

		/// <summary>
		/// Transformes diagram coordinates to control coordinates
		/// </summary>
		public void DiagramToControl(int dX, int dY, out int cX, out int cY)
		{
			cX = diagramPosX + (int)Math.Round((dX - scrollPosX) * zoomfactor);
			cY = diagramPosY + (int)Math.Round((dY - scrollPosY) * zoomfactor);
		}


		/// <summary>
		/// Transformes diagram coordinates to control coordinates
		/// </summary>
		public void DiagramToControl(Point dPt, out Point cPt)
		{
			cPt = Point.Empty;
			cPt.Offset(
				diagramPosX + (int)Math.Round((dPt.X - scrollPosX) * zoomfactor),
				diagramPosY + (int)Math.Round((dPt.Y - scrollPosY) * zoomfactor)
				);
		}


		/// <summary>
		/// Transformes diagram coordinates to control coordinates
		/// </summary>
		public void DiagramToControl(Rectangle dRect, out Rectangle cRect)
		{
			cRect = Rectangle.Empty;
			cRect.Offset(
				diagramPosX + (int)Math.Round((dRect.X - scrollPosX) * zoomfactor),
				diagramPosY + (int)Math.Round((dRect.Y - scrollPosY) * zoomfactor)
				);
			cRect.Width = (int)Math.Round(dRect.Width * zoomfactor);
			cRect.Height = (int)Math.Round(dRect.Height * zoomfactor);
		}


		/// <summary>
		/// Transformes diagram coordinates to control coordinates
		/// </summary>
		public void DiagramToControl(int dDistance, out int cDistance)
		{
			cDistance = (int)Math.Round(dDistance * zoomfactor);
		}


		/// <summary>
		/// Transformes diagram coordinates to control coordinates
		/// </summary>
		public void DiagramToControl(Size dSize, out Size cSize)
		{
			cSize = Size.Empty;
			cSize.Width = (int)Math.Round(dSize.Width * zoomfactor);
			cSize.Height = (int)Math.Round(dSize.Height * zoomfactor);
		}


		/// <summary>
		/// Transformes control coordinates to diagram coordinates
		/// </summary>
		public void ControlToDiagram(int cX, int cY, out int dX, out int dY)
		{
			dX = (int)Math.Round((cX - diagramPosX) / zoomfactor) + scrollPosX;
			dY = (int)Math.Round((cY - diagramPosY) / zoomfactor) + scrollPosY;
		}


		/// <summary>
		/// Transformes control coordinates to diagram coordinates
		/// </summary>
		public void ControlToDiagram(Point cPt, out Point dPt)
		{
			dPt = Point.Empty;
			dPt.X = (int)Math.Round((cPt.X - diagramPosX) / zoomfactor) + scrollPosX;
			dPt.Y = (int)Math.Round((cPt.Y - diagramPosY) / zoomfactor) + scrollPosY;
		}


		/// <summary>
		/// Transformes control coordinates to diagram coordinates
		/// </summary>
		public void ControlToDiagram(Rectangle cRect, out Rectangle dRect)
		{
			dRect = Rectangle.Empty;
			dRect.X = (int)Math.Round((cRect.X - diagramPosX) / zoomfactor) + scrollPosX;
			dRect.Y = (int)Math.Round((cRect.Y - diagramPosY) / zoomfactor) + scrollPosY;
			dRect.Width = (int)Math.Round((cRect.Width / zoomfactor));
			dRect.Height = (int)Math.Round((cRect.Height / zoomfactor));
		}


		/// <summary>
		/// Transformes control coordinates to diagram coordinates
		/// </summary>
		public void ControlToDiagram(Size cSize, out Size dSize)
		{
			dSize = Size.Empty;
			dSize.Width = (int)Math.Round((cSize.Width / zoomfactor));
			dSize.Height = (int)Math.Round((cSize.Height / zoomfactor));
		}


		/// <summary>
		/// Transformes control coordinates to diagram coordinates
		/// </summary>
		public void ControlToDiagram(int cDistance, out int dDistance)
		{
			dDistance = (int)Math.Round((cDistance / zoomfactor));
		}


		/// <summary>
		/// Transformes screen coordinates to diagram coordinates
		/// </summary>
		public void ScreenToDiagram(Point sPt, out Point dPt)
		{
			ControlToDiagram(PointToClient(sPt), out dPt);
		}


		/// <summary>
		/// Transformes screen coordinates to diagram coordinates
		/// </summary>
		public void ScreenToDiagram(Rectangle sRect, out Rectangle dRect)
		{
			ControlToDiagram(RectangleToClient(sRect), out dRect);
		}

		#endregion


		#region [Public] Methods: (Un)Selecting shapes

		/// <summary>
		/// Clears the current selection.
		/// </summary>
		public void UnselectAll()
		{
			if(selectedShapes.Count > 0)
			{
				ClearSelection();
				PerformSelectionNotifications();
			}
		}


		/// <summary>
		/// Removes the given Shape from the current selection.
		/// </summary>
		public void UnselectShape(Shape shape)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			if(selectedShapes.Count > 0)
			{
				DoUnselectShape(shape);
				PerformSelectionNotifications();
			}
		}


		/// <summary>
		/// Removes the given Shape from the current selection.
		/// </summary>
		private void UnselectShapes(IEnumerable<Shape> shapes)
		{
			if(shapes == null) throw new ArgumentNullException("shapes");
			if(selectedShapes.Count > 0)
			{
				DoSuspendUpdate();
				foreach(Shape s in shapes)
				{
					if(selectedShapes.Contains(s))
						DoUnselectShape(s);
				}
				DoResumeUpdate();
				PerformSelectionNotifications();
			}
		}


		/// <summary>
		/// Selects the given shape. Current selection will be cleared.
		/// </summary>
		public void SelectShape(Shape shape)
		{
			SelectShape(shape, false);
		}


		/// <summary>
		/// Selects the given shape.
		/// </summary>
		/// <param name="shape">Shape to be selected.</param>
		/// <param name="addToSelection">If true, the given shape will be added to the current selection, otherwise the current selection will be cleared before selecting this shape.</param>
		public void SelectShape(Shape shape, bool addToSelection)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			DoSelectShape(shape, addToSelection);
			PerformSelectionNotifications();
		}


		/// <summary>
		/// Selects the given shape.
		/// </summary>
		/// <param name="shapes">Shape to be selected.</param>
		/// <param name="addToSelection">If true, the given shape will be added to the current selection, otherwise the current selection will be cleared before selecting this shape.</param>
		public void SelectShapes(IEnumerable<Shape> shapes, bool addToSelection)
		{
			if(shapes == null) throw new ArgumentNullException("shapes");
			if(!addToSelection)
				ClearSelection();
			if(SelectedShapes.ContainsAll(shapes))
				SelectedShapes.RemoveRange(shapes);
			else
			{
				foreach(Shape shape in shapes)
				{
					Debug.Assert(Diagram.Shapes.Contains(shape) || shape.Parent != null);
					DoSelectShape(shape, true);
				}
			}
			PerformSelectionNotifications();
		}


		/// <summary>
		/// Selects all shapes within the given area.
		/// </summary>
		/// <param name="area">All shapes in the given rectangle will be selected.</param>
		/// <param name="addToSelection">If true, the given shape will be added to the current selection, otherwise the current selection will be cleared before selecting this shape.</param>
		public void SelectShapes(Rectangle area, bool addToSelection)
		{
			if(Diagram != null)
			{
				// ensure rectangle width and height are positive
				if(area.Size.Width < 0)
				{
					area.Width = Math.Abs(area.Width);
					area.X = area.X - area.Width;
				}
				if(area.Size.Height < 0)
				{
					area.Height = Math.Abs(area.Height);
					area.Y = area.Y - area.Height;
				}
				SelectShapes(Diagram.Shapes.FindShapes(area.X, area.Y, area.Width, area.Height, true), addToSelection);
			}
		}


		/// <summary>
		/// Selects all shapes of the given shape type.
		/// </summary>
		public void SelectShapes(ShapeType shapeType, bool addToSelection)
		{
			if(shapeType == null) throw new ArgumentNullException("shapeType");
			if(Diagram != null)
			{
				// Find all shapes of the same ShapeType
				shapeBuffer.Clear();
				foreach(Shape shape in Diagram.Shapes)
				{
					if(shape.Type == shapeType)
						shapeBuffer.Add(shape);
				}
				SelectShapes(shapeBuffer, addToSelection);
			}
		}


		/// <summary>
		/// Selects all shapes based on the given template.
		/// </summary>
		public void SelectShapes(Template template, bool addToSelection)
		{
			if(template == null) throw new ArgumentNullException("template");
			if(Diagram != null)
			{
				// Find all shapes of the same ShapeType
				shapeBuffer.Clear();
				foreach(Shape shape in Diagram.Shapes)
				{
					if(shape.Template == template)
						shapeBuffer.Add(shape);
				}
				SelectShapes(shapeBuffer, addToSelection);
			}
		}


		/// <summary>
		/// Selects all shapes of the diagram.
		/// </summary>
		public void SelectAll()
		{
			DoSuspendUpdate();
			shapesConnectedToSelectedShapes.Clear();
			selectedShapes.Clear();

			selectedShapes.AddRange(Diagram.Shapes);
			foreach(Shape shape in selectedShapes.BottomUp)
				DoInvalidateShape(shape);
			DoResumeUpdate();
			PerformSelectionNotifications();
		}

		#endregion


		#region [Public] Methods

		/// <summary>
		/// Fetches the indicated diagram from the repository and displays it.
		/// </summary>
		//[Obsolete("Use OpenDiagram method instead.")]
		public bool LoadDiagram(string diagramName)
		{
			return OpenDiagram(diagramName);
		}


		/// <summary>
		/// Fetches the indicated diagram from the repository and displays it.
		/// </summary>
		public bool OpenDiagram(string diagramName)
		{
			if(diagramName == null) throw new ArgumentNullException("diagramName");
			if(DiagramSetController == null) throw new NShapeException("Property DiagramSetController is not set!");
			bool result = false;
			// Clear current selectedShapes and models
			if(Project.Repository == null)
				throw new NShapeException("Repository is not set to an instance of IRepository.");
			if(!Project.Repository.IsOpen)
				throw new NShapeException("Repository is not open.");

			Diagram d = Project.Repository.GetDiagram(diagramName);
			if(d != null)
			{
				// Use property setter because it updates the shape's display service 
				// and loads all diagram shapes from repository
				Diagram = d;
				result = true;
			}

			UpdateScrollBars();
			Refresh();
			return result;
		}


		/// <summary>
		/// Creates the indicated diagram, inserts it into the repository and displays it.
		/// </summary>
		public bool CreateDiagram(string diagramName)
		{
			if(diagramName == null) throw new ArgumentNullException("diagramName");
			if(DiagramSetController == null) throw new NShapeException("Property DiagramSetController is not set!");
			bool result = false;
			// Clear current selectedShapes and models
			if(Project.Repository == null)
				throw new NShapeException("Repository is not set to an instance of IRepository.");
			if(!Project.Repository.IsOpen)
				throw new NShapeException("Repository is not open.");

			// Clear current content of the display
			Clear();

			Diagram d = new Diagram(diagramName);
			Project.Repository.Insert(d);
			Diagram = d;
			result = true;

			UpdateScrollBars();
			Refresh();
			return result;
		}


		/// <summary>
		/// Clears diagram and all buffers.
		/// </summary>
		public void Clear()
		{
			//ClearSelection();
			if(Diagram != null) Diagram = null;
			selectedShapes.Clear();
			shapeBuffer.Clear();
			editBuffer.Clear();
			Invalidate();
		}


		/// <summary>
		/// Returns a collection of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" /> for constructing context menus etc.
		/// </summary>
		public IEnumerable<MenuItemDef> GetMenuItemDefs()
		{
			if(Diagram != null)
			{
				Point mousePos = Point.Empty;
				ScreenToDiagram(Control.MousePosition, out mousePos);
				Shape shapeUnderCursor = Diagram.Shapes.FindShape(mousePos.X, mousePos.Y, ControlPointCapabilities.None, 0, null);
				bool modelObjectsAssigned = ModelObjectsAssigned(selectedShapes);

				#region Context menu structure of the display specific context menu
				// Select...
				// Bring to front
				// Send to bottom
				// --------------------------
				// Add Shapes to active Layers
				// Assign Shapes to active Layers
				// Remove Shapes from all Layers
				// --------------------------
				// Group shapes
				// Ungroup shapes
				// Aggregate composite shape
				// Split composite shape
				// --------------------------
				// Cut
				// Copy
				// Paste
				// Delete
				// --------------------------
				// Diagram Properties
				// --------------------------
				// Undo
				// Redo
				//
				#endregion

				// Create a action group
				yield return new GroupMenuItemDef(Properties.Resources.CaptionTxt_GroupSelect, null,
					Properties.Resources.TooltipTxt_GroupSelect, true,
					new MenuItemDef[] {
						CreateSelectAllMenuItemDef(),
						CreateSelectByTemplateMenuItemDef(shapeUnderCursor),
						CreateSelectByTypeMenuItemDef(shapeUnderCursor),
						CreateUnselectAllMenuItemDef()
					}, -1);
				yield return new SeparatorMenuItemDef();
#if DEBUG_UI
				yield return CreateShowShapeInfoMenuItemDef(Diagram, selectedShapes);
				yield return new SeparatorMenuItemDef();
#endif
				yield return CreateBringToFrontMenuItemDef(Diagram, selectedShapes);
				yield return CreateSendToBackMenuItemDef(Diagram, selectedShapes);
				yield return new SeparatorMenuItemDef();
				yield return CreateAddShapesToLayersMenuItemDef(Diagram, selectedShapes, activeLayers, false);
				yield return CreateAddShapesToLayersMenuItemDef(Diagram, selectedShapes, activeLayers, true);
				yield return CreateRemoveShapesFromLayersMenuItemDef(Diagram, selectedShapes);
				yield return new SeparatorMenuItemDef();
				yield return CreateGroupShapesMenuItemDef(Diagram, selectedShapes, activeLayers);
				yield return CreateUngroupMenuItemDef(Diagram, selectedShapes);
				yield return CreateAggregateMenuItemDef(Diagram, selectedShapes, activeLayers, mousePos);
				yield return CreateUnaggregateMenuItemDef(Diagram, selectedShapes);
				yield return new SeparatorMenuItemDef();
				yield return CreateCutMenuItemDef(Diagram, selectedShapes, modelObjectsAssigned, mousePos);
				yield return CreateCopyImageMenuItemDef(Diagram, selectedShapes);
				yield return CreateCopyMenuItemDef(Diagram, selectedShapes, modelObjectsAssigned, mousePos);
				yield return CreatePasteMenuItemDef(Diagram, selectedShapes, activeLayers, mousePos);
				yield return CreateDeleteMenuItemDef(Diagram, selectedShapes, modelObjectsAssigned);
				if(propertyController != null)
				{
					yield return new SeparatorMenuItemDef();
					yield return CreatePropertiesMenuItemDef(Diagram, selectedShapes, mousePos);
				}
				yield return new SeparatorMenuItemDef();
				yield return CreateUndoMenuItemDef();
				yield return CreateRedoMenuItemDef();
			}
		}


		/// <summary>
		/// Inserts the given shape in the displayed diagram.
		/// </summary>
		public void InsertShape(Shape shape)
		{
			InsertShapes(SingleInstanceEnumerator<Shape>.Create(shape));
		}


		/// <summary>
		/// Inserts the given shapes in the displayed diagram.
		/// </summary>
		public void InsertShapes(IEnumerable<Shape> shapes)
		{
			if(Diagram != null)
			{
				DiagramSetController.InsertShapes(Diagram, shapes, ActiveLayers, true);
				OnShapesInserted(GetShapesEventArgs(shapes));
				SelectShapes(shapes, false);
			}
		}


		/// <summary>
		/// Deletes the selected shapes.
		/// </summary>
		public void DeleteShapes()
		{
			DeleteShapes(SelectedShapes, true);
		}


		/// <summary>
		/// Deletes the selected shapes.
		/// </summary>
		public void DeleteShapes(bool withModelObjects)
		{
			DeleteShapes(SelectedShapes, withModelObjects);
		}


		/// <summary>
		/// Deletes the given shape in the displayed diagram.
		/// </summary>
		public void DeleteShape(Shape shape)
		{
			DeleteShapes(SingleInstanceEnumerator<Shape>.Create(shape), true);
		}


		/// <summary>
		/// Deletes the given shape in the displayed diagram.
		/// </summary>
		public void DeleteShape(Shape shape, bool withModelObjects)
		{
			DeleteShapes(SingleInstanceEnumerator<Shape>.Create(shape), withModelObjects);
		}


		/// <summary>
		/// Deletes the given shapes in the displayed diagram.
		/// </summary>
		public void DeleteShapes(IEnumerable<Shape> shapes)
		{
			DeleteShapes(shapes, true);
		}


		/// <summary>
		/// Deletes the given shapes in the displayed diagram.
		/// </summary>
		public void DeleteShapes(IEnumerable<Shape> shapes, bool withModelObejcts)
		{
			if(Diagram != null)
			{
				DiagramSetController.DeleteShapes(Diagram, shapes, withModelObejcts);
				OnShapesRemoved(GetShapesEventArgs(shapes));
				UnselectShapes(shapes);
			}
		}


		/// <summary>
		/// Cuts the selected shapes.
		/// </summary>
		public void Cut()
		{
			Cut(true, PointToClient(Control.MousePosition));
		}


		/// <summary>
		/// Cut the selected shapes with or without model objects.
		/// </summary>
		public void Cut(bool withModelObjects)
		{
			if(Diagram != null && selectedShapes.Count > 0)
				Cut(withModelObjects, Geometry.InvalidPoint);
		}


		/// <summary>
		/// Cut the selected shapes with or without model objects.
		/// </summary>
		/// <param name="withModelObjects">Specifies whether the shape should be copied with its model objects (if assigned).</param>
		/// <param name="mousePosDiagram">Current Mouse position in diagram coordinates.</param>
		public void Cut(bool withModelObjects, Point mousePosDiagram)
		{
			if(Diagram != null && selectedShapes.Count > 0)
				PerformCut(Diagram, selectedShapes, withModelObjects, mousePosDiagram);
		}


		/// <summary>
		/// Copies the selected shapes.
		/// </summary>
		public void Copy()
		{
			Copy(true, PointToClient(Control.MousePosition));
		}


		/// <summary>
		/// Copy the selected shapes with or without model objects.
		/// </summary>
		public void Copy(bool withModelObjects)
		{
			Copy(withModelObjects, Geometry.InvalidPoint);
		}


		/// <summary>
		/// Copy the selected shapes with or without model objects.
		/// </summary>
		/// <param name="withModelObjects">Specifies whether the shape should be copied with its model objects (if assigned).</param>
		/// <param name="mousePosDiagram">Current Mouse position in diagram coordinates.</param>
		public void Copy(bool withModelObjects, Point mousePosDiagram)
		{
			if(Diagram != null && SelectedShapes.Count > 0)
				PerformCopy(Diagram, SelectedShapes, withModelObjects, mousePosDiagram);
		}


		/// <summary>
		/// Pastes the previously copied or cut shapes into the displayed diagram.
		/// </summary>
		public void Paste()
		{
			Paste(PointToClient(Control.MousePosition));
		}


		/// <summary>
		/// Paste the copied or cut shapes.
		/// </summary>
		/// <param name="mousePosDiagram">Current mouse position in diagram coordinates.</param>
		public void Paste(Point mousePosDiagram)
		{
			if(Diagram != null && diagramSetController.CanPaste(Diagram))
				PerformPaste(Diagram, activeLayers, mousePosDiagram);
		}


		/// <summary>
		/// Paste the copied or cut shapes.
		/// </summary>
		/// <param name="offsetX">Horizontal offset in diagram coordinates relative to the original shape's X coordinate.</param>
		/// <param name="offsetY">Vertical offset in diagram coordinates relative to the original shape's Y coordinate.</param>
		public void Paste(int offsetX, int offsetY)
		{
			if(Diagram != null && diagramSetController.CanPaste(Diagram))
				PerformPaste(Diagram, activeLayers, offsetX, offsetY);
		}


		/// <summary>
		/// Delete the selected shapes with or without model objects.
		/// </summary>
		public void Delete(bool withModelObjects)
		{
			if(Diagram != null && SelectedShapes.Count > 0)
			{
				PerformDelete(Diagram, selectedShapes, withModelObjects);
			}
		}


		/// <summary>
		/// Ensures that the given coordinates are inside the displayed area by scrolling the display to the given position.
		/// </summary>
		public void EnsureVisible(int x, int y)
		{
			EnsureVisible(x, y, 0);
		}


		/// <summary>
		/// Ensures that the given coordinates are inside the displayed area by scrolling the display to the given position.
		/// </summary>
		public void EnsureVisible(int x, int y, int margin)
		{
			int ctrlX, ctrlY, ctrlMargin;
			DiagramToControl(x, y, out ctrlX, out ctrlY);
			DiagramToControl(margin, out ctrlMargin);
			if(DrawBoundsContainsPoint(ctrlX, ctrlY, ctrlMargin))
			{
				Rectangle viewArea = Rectangle.Empty;
				ControlToDiagram(DrawBounds, out viewArea);
				viewArea.Inflate(-margin, -margin);

				// scroll horizontally
				int deltaX = 0, deltaY = 0;
				if(HScrollBarVisible)
				{
					if(x < viewArea.Left)
						// Scroll left
						deltaX = x - viewArea.Left;
					else if(viewArea.Right < x)
						// Scroll right
						deltaX = x - viewArea.Right;
				}
				if(VScrollBarVisible)
				{
					if(y < viewArea.Top)
						// Scroll left
						deltaY = y - viewArea.Top;
					else if(viewArea.Bottom < y)
						// Scroll right
						deltaY = y - viewArea.Bottom;
				}
				ScrollTo(scrollPosX + deltaX, scrollPosY + deltaY);
			}
		}


		/// <summary>
		/// Ensures that the given shape is inside the displayed area.
		/// </summary>
		/// <param name="shape">The shape that should be completely visible.</param>
		public void EnsureVisible(Shape shape)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			EnsureVisible(shape.GetBoundingRectangle(false));
		}


		/// <summary>
		/// Ensures that the given shape is inside the displayed area.
		/// </summary>
		/// <param name="shape">The shape that should be completely visible.</param>
		/// <param name="margin">Specifies the margin (in diagram coordinates) around the area that should be visible.</param>
		public void EnsureVisible(Shape shape, int margin)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			EnsureVisible(shape.GetBoundingRectangle(false), margin);
		}


		/// <summary>
		/// Ensures that the given area is inside the displayed area.
		/// </summary>
		/// <param name="viewAreaDiagram">The area (in diagram coordinates) that should be visible.</param>
		public void EnsureVisible(Rectangle viewAreaDiagram)
		{
			EnsureVisible(viewAreaDiagram, 0);
		}


		/// <summary>
		/// Ensures that the given area is inside the displayed area.
		/// </summary>
		/// <param name="viewAreaDiagram">The area (in diagram coordinates) that should be visible.</param>
		/// <param name="margin">Specifies the margin (in diagram coordinates) around the area that should be visible.</param>
		public void EnsureVisible(Rectangle viewAreaDiagram, int margin)
		{
			// Check if the diagram has to be zoomed
			Rectangle areaBoundsDiag = Rectangle.Empty;
			ControlToDiagram(DrawBounds, out areaBoundsDiag);
			areaBoundsDiag.Inflate(-margin, -margin);
			// Check if Zoom has to be adjusted
			if(areaBoundsDiag.Width < viewAreaDiagram.Width || areaBoundsDiag.Height < viewAreaDiagram.Height)
			{
				float scale = Geometry.CalcScaleFactor(viewAreaDiagram.Width, viewAreaDiagram.Height, areaBoundsDiag.Width, areaBoundsDiag.Height);
				ZoomLevel = Math.Max(1, (int)Math.Floor(ZoomLevel * scale));
			}

			// Recalculate viewArea in case a new zoom level was applied
			ControlToDiagram(DrawBounds, out areaBoundsDiag);
			areaBoundsDiag.Inflate(-margin, -margin);
			if(viewAreaDiagram.Left < areaBoundsDiag.Left
				|| viewAreaDiagram.Top < areaBoundsDiag.Top
				|| viewAreaDiagram.Right > areaBoundsDiag.Right
				|| viewAreaDiagram.Bottom > areaBoundsDiag.Bottom)
			{

				// scroll horizontally
				int deltaX = 0, deltaY = 0;
				if(HScrollBarVisible)
				{
					if(viewAreaDiagram.Left < areaBoundsDiag.Left)
						// Scroll left
						deltaX = viewAreaDiagram.Left - areaBoundsDiag.Left;
					else if(areaBoundsDiag.Right < viewAreaDiagram.Right)
						// Scroll right
						deltaX = viewAreaDiagram.Right - areaBoundsDiag.Right;
				}
				if(VScrollBarVisible)
				{
					if(viewAreaDiagram.Top < areaBoundsDiag.Top)
						// Scroll up
						deltaY = viewAreaDiagram.Top - areaBoundsDiag.Top;
					else if(areaBoundsDiag.Bottom < viewAreaDiagram.Bottom)
						// Scroll down
						deltaY = viewAreaDiagram.Bottom - areaBoundsDiag.Bottom;
				}
				ScrollTo(scrollPosX + deltaX, scrollPosY + deltaY);
			}
		}


		/// <summary>
		/// Shows or hides the given layers.
		/// </summary>
		public void SetLayerVisibility(LayerIds layerIds, bool visible)
		{
			// Hide or show layers
			if(visible) hiddenLayers ^= (hiddenLayers & layerIds);
			else hiddenLayers |= layerIds;

			UnselectShapesOfInvisibleLayers();
			// Update presenter
			Invalidate();
			// Perform notification only if a diagram is displayed.
			if(Diagram != null)
				OnLayerVisibilityChanged(LayerHelper.GetLayersEventArgs(LayerHelper.GetLayers(layerIds, Diagram)));
		}


		/// <summary>
		/// Sets the given layers as active layers.
		/// </summary>
		public void SetLayerActive(LayerIds layerIds, bool active)
		{
			// Activate or deactivate layers
			if(active) activeLayers |= layerIds;
			else activeLayers ^= (activeLayers & layerIds);
			// Update presenter
			Invalidate();
			// Perform notification only if a diagram is displayed.
			if(Diagram != null)
				OnActiveLayersChanged(LayerHelper.GetLayersEventArgs(LayerHelper.GetLayers(layerIds, Diagram)));
		}


		/// <summary>
		/// Tests whether any of the given layers is visible.
		/// </summary>
		public bool IsLayerVisible(LayerIds layerId)
		{
			if(hiddenLayers == LayerIds.None || layerId == LayerIds.None) return true;
			else return !((hiddenLayers & layerId) == layerId);
		}


		/// <summary>
		/// Tests whether all of the given layers are active.
		/// </summary>
		public bool IsLayerActive(LayerIds layerId)
		{
			return (activeLayers & layerId) == layerId;
		}


		/// <summary>
		/// Copies the selected shapes (or the whole diagram if no shapes are selected) as image of the specified format to the clipboard.
		/// </summary>
		/// <param name="fileFormat">Format of the image that will be copied to the clipboard.</param>
		/// <param name="clearClipboard">Specifies if the clipboard should be emptied before copying the image.</param>
		/// <remarks>When not clearing the clipboard before adding the an image, you can add a bitmap- and a vector images to the clipboard.</remarks>
		public void CopyImageToClipboard(ImageFileFormat fileFormat, bool clearClipboard)
		{
			if(Diagram != null)
			{
				IEnumerable<Shape> shapes = (SelectedShapes.Count > 0) ? SelectedShapes : null;
				int margin = (SelectedShapes.Count > 0) ? 10 : 0;
				switch(fileFormat)
				{
					case ImageFileFormat.Bmp:
					case ImageFileFormat.Gif:
					case ImageFileFormat.Jpeg:
					case ImageFileFormat.Png:
					case ImageFileFormat.Tiff:
						if(!clearClipboard) System.Windows.Forms.Clipboard.Clear();
						// Copy diagram/selected shapes to clipboard as PNG bitmap image file
						Bitmap bmpImage = (Bitmap)Diagram.CreateImage(fileFormat, shapes, margin, false, System.Drawing.Color.Empty);
						Clipboard.SetData(DataFormats.Bitmap, bmpImage);
						Debug.Assert(Clipboard.ContainsData(DataFormats.Bitmap));
						break;
					case ImageFileFormat.Emf:
					case ImageFileFormat.EmfPlus:
						// Copy diagram/selected shapes to clipboard as EMF vector graphic file
						Metafile emfImage = (Metafile)Diagram.CreateImage(ImageFileFormat.EmfPlus, shapes, margin, false, System.Drawing.Color.Empty);
						EmfHelper.PutEnhMetafileOnClipboard(Handle, emfImage, clearClipboard);
						break;
					case ImageFileFormat.Svg:
						throw new NotImplementedException();
					default:
						throw new NShapeUnsupportedValueException(fileFormat);
				}
			}
		}

		#endregion


		/// <summary>
		/// This DiagramPresenter's controller.
		/// </summary>
		[Browsable(false)]
		protected DiagramController DiagramController
		{
			get { return diagramController; }
			set
			{
				Debug.Assert(diagramSetController != null);
				if(diagramController != null)
				{
					UnregisterDiagramControllerEvents();
					if(diagramController.Diagram != null)
					{
						diagramSetController.CloseDiagram(diagramController.Diagram);
						if(diagramController.Diagram != null)
						{
							UnregisterDiagramEvents();
							diagramController.Diagram = null;
						}
						diagramController = null;
						Clear();
					}
				}
				diagramController = value;
				if(diagramController != null)
				{
					RegisterDiagramControllerEvents();
					if(diagramController.Diagram != null)
					{
						RegisterDiagramEvents();
						DisplayDiagram();
					}
				}
			}
		}


		#region [Protected] Methods: On[Event] event processing

		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void OnShapesSelected(EventArgs eventArgs)
		{
			if(ShapesSelected != null && !SelectingChangedShapes)
				ShapesSelected(this, eventArgs);
		}

		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void OnShapeClick(DiagramPresenterShapeClickEventArgs eventArgs)
		{
			if(ShapeClick != null) ShapeClick(this, eventArgs);
		}

		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void OnShapeDoubleClick(DiagramPresenterShapeClickEventArgs eventArgs)
		{
			if(ShapeDoubleClick != null) ShapeDoubleClick(this, eventArgs);
		}

		/// <ToBeCompleted></ToBeCompleted>
		protected internal virtual void OnShapesInserted(DiagramPresenterShapesEventArgs eventArgs)
		{
			ChangeShapePoints(eventArgs.Shapes);
			if(ShapesInserted != null) ShapesInserted(this, eventArgs);
		}

		/// <ToBeCompleted></ToBeCompleted>
		protected internal virtual void OnShapesRemoved(DiagramPresenterShapesEventArgs eventArgs)
		{
			if(ShapesRemoved != null) ShapesRemoved(this, eventArgs);

			ClearShapePoints(eventArgs.Shapes);
		}

		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void OnLayerVisibilityChanged(LayersEventArgs eventArgs)
		{
			if(LayerVisibilityChanged != null) LayerVisibilityChanged(this, eventArgs);
		}

		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void OnActiveLayersChanged(LayersEventArgs eventArgs)
		{
			if(ActiveLayersChanged != null) ActiveLayersChanged(this, eventArgs);
		}

		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void OnZoomChanged(EventArgs eventArgs)
		{
			if(ZoomChanged != null) ZoomChanged(this, eventArgs);
		}

		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void OnDiagramChanging(EventArgs eventArgs)
		{
			if(DiagramChanging != null) DiagramChanging(this, eventArgs);
			ResetBounds();
			hiddenLayers = LayerIds.None;
			activeLayers = LayerIds.None;
		}

		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void OnDiagramChanged(EventArgs eventArgs)
		{
			UpdateScrollBars();
			if(DiagramChanged != null) DiagramChanged(this, eventArgs);
		}

		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void OnUserMessage(UserMessageEventArgs eventArgs)
		{
			if(UserMessage == null)
			{
				string msgFormatStr = "{0}{1}{1}In order to show this message in the correct context, handle the {2}.UserMessage event.";
				throw new WarningException(string.Format(msgFormatStr, eventArgs.MessageText, Environment.NewLine, GetType().FullName));
			}
			else
				UserMessage(this, eventArgs);
		}

		#endregion


		#region [Protected] Methods: On[Event] event processing overrides

		/// <override></override>
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
		}

		/// <override></override>
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
		}

		/// <override></override>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			// ToDo: Redirect MouseWheel movement to the current tool?
			//if (CurrentTool != null)
			//   CurrentTool.ProcessMouseEvent(this, WinFormHelpers.GetMouseEventArgs(MouseEventType.MouseWheel, e));

			if(Diagram != null)
			{
				if(zoomWithMouseWheel || (Control.ModifierKeys & Keys.Control) != 0)
				{
					// Store position of the mouse cursor (in diagram coordinates)
					Point currMousePos = Point.Empty;
					ControlToDiagram(e.Location, out currMousePos);

					if(e.Delta != 0)
					{
						// Calculate current zoom stepping according to the current zoom factor.
						// This kind of accelleration feels for the user as a more 'constant' zoom behavior
						int currZoomStep;
						if(ZoomLevel < 30) currZoomStep = 1;
						else if(ZoomLevel < 60) currZoomStep = 2;
						else if(zoomLevel < 100) currZoomStep = 5;
						else if(zoomLevel < 300) currZoomStep = 10;
						else currZoomStep = 50;
						// Calculate zoom direction
						currZoomStep *= (int)(e.Delta / Math.Abs(e.Delta));
						// Set zoom level (and ensure the value is within a reasonable range)
						ZoomLevel = Math.Min(Math.Max(0, (ZoomLevel + currZoomStep) - (ZoomLevel % currZoomStep)), 4000);
					}

					// Restore mouse cursors's position by scrolling
					Point newMousePos = Point.Empty;
					ControlToDiagram(e.Location, out newMousePos);
					ScrollBy(currMousePos.X - newMousePos.X, currMousePos.Y - newMousePos.Y);
				}
				else
				{
					int value = -(e.Delta / 6);
					if((Control.ModifierKeys & Keys.Shift) != 0)
						ScrollBy(value, 0);
					else ScrollBy(0, value);
				}
			}
		}

		/// <override></override>
		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			ProcessClickEvent(e, false);
		}

		/// <override></override>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if(base.MapProvider == GMap.NET.MapProviders.EmptyProvider.Instance && e.Button == base.DragButton)
				mouseEventWasHandled = true;
			else
				base.OnMouseDown(e);
			mouseEventWasHandled = false;
			if(inplaceTextbox != null) DoCloseCaptionEditor(true);
			if(ScrollBarContainsPoint(e.Location))
				mouseEventWasHandled = true;
			else
			{
				if(ActiveTool != null)
				{
					try
					{
						if(ActiveTool.ProcessMouseEvent(this, WinFormHelpers.GetMouseEventArgs(this, MouseEventType.MouseDown, e, DrawBounds)))
							mouseEventWasHandled = true;
					}
					catch(Exception exc)
					{
						ActiveTool.Cancel();
						Debug.Print(GetFailMessage(exc));
						throw;
					}
				}
			}
		}

		/// <override></override>
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			ProcessClickEvent(e, true);
		}

		/// <override></override>
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			if(ActiveTool != null)
			{
				try
				{
					ActiveTool.EnterDisplay(this);
				}
				catch(Exception exc)
				{
					ActiveTool.Cancel();
					Debug.Print(GetFailMessage(exc));
					throw;
				}
			}
		}

		/// <override></override>
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if(ActiveTool != null)
			{
				try
				{
					ActiveTool.LeaveDisplay(this);
				}
				catch(Exception exc)
				{
					ActiveTool.Cancel();
					Debug.Print(GetFailMessage(exc));
					throw;
				}
			}
		}

		/// <override></override>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			//Console.WriteLine("[{0}]\t OnMouseMove (Entering)", DateTime.Now.ToString("HH:mm:ss.ffff"));
			base.OnMouseMove(e);
			if(universalScrollEnabled)
				PerformUniversalScroll(e.Location);
			else
			{
				if(ActiveTool != null && !ScrollBarContainsPoint(e.Location))
				{
					try
					{
						//Console.WriteLine("[{0}]\t Tool.ProcessMouseEvent calling", DateTime.Now.ToString("HH:mm:ss.ffff"));
						if(ActiveTool.ProcessMouseEvent(this, WinFormHelpers.GetMouseEventArgs(this, MouseEventType.MouseMove, e, DrawBounds)))
							mouseEventWasHandled = true;
						//Console.WriteLine("[{0}]\t Tool.ProcessMouseEvent finished", DateTime.Now.ToString("HH:mm:ss.ffff"));

						if(ActiveTool.WantsAutoScroll && DrawBoundsContainsPoint(e.X, e.Y, autoScrollMargin))
						{
							int x, y;
							ControlToDiagram(e.X, e.Y, out x, out y);
							int margin;
							ControlToDiagram(autoScrollMargin, out margin);
							EnsureVisible(x, y, margin);
							if(!autoScrollTimer.Enabled) autoScrollTimer.Enabled = true;
						}
						else if(autoScrollTimer.Enabled)
							autoScrollTimer.Enabled = false;
					}
					catch(Exception exc)
					{
						ActiveTool.Cancel();
						Debug.Print(GetFailMessage(exc));
						throw;
					}
				}
			}
			ToolTipMouseMove(e);
			lastMousePos = e.Location;
			//Console.WriteLine("[{0}]\t OnMouseMove (Leaving)", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}

		/// <override></override>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if(!this.Focused)
				this.Focus();
			if(ActiveTool != null && !ScrollBarContainsPoint(e.Location))
			{
				try
				{
					if(ActiveTool.ProcessMouseEvent(this, WinFormHelpers.GetMouseEventArgs(this, MouseEventType.MouseUp, e, DrawBounds)))
						mouseEventWasHandled = true;
				}
				catch(Exception exc)
				{
					ActiveTool.Cancel();
					Debug.Print(GetFailMessage(exc));
					throw;
				}
			}

			// If the mouse event was not handled otherwise, handle it here:
			if(!mouseEventWasHandled)
			{
				// Use switch statement here because we do not want to handle mouse button combinations
				switch(e.Button)
				{

					// Display diagram properties
					case MouseButtons.Left:
						ShowDiagramProperties(e.Location);
						break;

				//?	// Start universal scroll
				//?	case MouseButtons.Middle:
				//?		if(universalScrollEnabled) EndUniversalScroll();
				//?		else StartUniversalScroll(e.Location);
				//?		break;

					// Show context menu
					case MouseButtons.Right:
						if(ActiveTool != null)
						{
							if(Diagram != null)
							{
								Point mousePos = Point.Empty;
								ControlToDiagram(e.Location, out mousePos);
								// if there is no selected shape under the cursor
								if(SelectedShapes.FindShape(mousePos.X, mousePos.Y, ControlPointCapabilities.None, 0, null) == null)
								{
									// Check if there is a non-selected shape under the cursor 
									// and select it in this case
									Shape shape = Diagram.Shapes.FindShape(mousePos.X, mousePos.Y, ControlPointCapabilities.None, 0, null);
									if(shape != null) SelectShape(shape);
									else UnselectAll();
								}
							}
							// Display context menu
							if(ContextMenuStrip != null)
							{
								if(ContextMenuStrip.Visible) ContextMenuStrip.Close();
								ContextMenuStrip.Show(PointToScreen(e.Location));
							}
						}
						break;
				}
			}
			mouseEventWasHandled = false;
		}

		/// <override></override>
		protected override void OnMouseCaptureChanged(EventArgs e)
		{
			base.OnMouseCaptureChanged(e);
		}

		/// <override></override>
		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if(ActiveTool != null)
			{
				try
				{
					ActiveTool.ProcessKeyEvent(this, WinFormHelpers.GetKeyEventArgs(e));
				}
				catch(Exception exc)
				{
					Debug.Print(GetFailMessage(exc));
					ActiveTool.Cancel();
					throw;
				}
			}
		}

		/// <override></override>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if(!e.Handled && ActiveTool != null)
			{
				try
				{
					e.Handled = ActiveTool.ProcessKeyEvent(this, WinFormHelpers.GetKeyEventArgs(KeyEventType.KeyDown, e));
				}
				catch(Exception exc)
				{
					Debug.Print(exc.Message);
					ActiveTool.Cancel();
					throw;
				}

				if(!e.Handled)
				{
					try
					{
						switch(e.KeyCode)
						{
							case Keys.F2:
								if(Diagram != null
									&& SelectedShapes.Count == 1
									&& Project.SecurityManager.IsGranted(Permission.Data, SelectedShapes.TopMost)
									&& SelectedShapes.TopMost is ICaptionedShape)
								{
									ICaptionedShape captionedShape = (ICaptionedShape)SelectedShapes.TopMost;
									if(captionedShape.CaptionCount > 0)
									{
										DoOpenCaptionEditor(captionedShape, 0, string.Empty);
										e.Handled = true;
									}
								}
								break;

							case Keys.Left:
							case Keys.Right:
							case Keys.Up:
							case Keys.Down:
								if(SelectedShapes.Count == 0)
								{
									int newHValue = scrollBarH.Value;
									if(HScrollBarVisible)
									{
										int deltaH = (int)Math.Round(Math.Max((int)(e.Shift ? scrollBarH.LargeChange : scrollBarH.SmallChange) / 2f, 1));
										if(e.KeyCode == Keys.Left)
											newHValue -= deltaH;
										else if(e.KeyCode == Keys.Right)
											newHValue += deltaH;
									}
									int newVValue = scrollBarV.Value;
									if(VScrollBarVisible)
									{
										int deltaV = (int)Math.Round(Math.Max((int)(e.Shift ? scrollBarV.LargeChange : scrollBarV.SmallChange) / 2f, 1));
										if(e.KeyCode == Keys.Up)
											newVValue -= deltaV;
										else if(e.KeyCode == Keys.Down)
											newVValue += deltaV;
									}
									ScrollTo(newHValue, newVValue);
									e.Handled = true;
								}
								else
								{
									int deltaX = 0, deltaY = 0;
									switch(e.KeyCode)
									{
										case Keys.Left:
										case Keys.Right:
											deltaX = e.Shift ? GridSize : 1;
											if(e.KeyCode == Keys.Left)
												deltaX = -deltaX;
											break;
										case Keys.Up:
										case Keys.Down:
											deltaY = e.Shift ? GridSize : 1;
											if(e.KeyCode == Keys.Up)
												deltaY = -deltaY;
											break;
									}
									if(DiagramSetController.CanMoveShapes(Diagram, SelectedShapes))
									{
										DiagramSetController.MoveShapes(Diagram, SelectedShapes, deltaX, deltaY);
										e.Handled = true;
									}
								}
								break;

							// Delete / Cut
							case Keys.Delete:
								if(Diagram != null && inplaceTextbox == null && SelectedShapes.Count > 0)
								{
									if(e.Modifiers == Keys.Shift)
									{
										// Cut
										if(DiagramController.Owner.CanCut(Diagram, SelectedShapes))
										{
											if(ActiveTool != null && ActiveTool.IsToolActionPending)
												ActiveTool.Cancel();

											PerformCut(Diagram, SelectedShapes, true, Geometry.InvalidPoint);
											e.Handled = true;
										}
									}
									else
									{
										// Delete
										if(DiagramController.Owner.CanDeleteShapes(Diagram, SelectedShapes))
										{
											if(ActiveTool != null && ActiveTool.IsToolActionPending)
												ActiveTool.Cancel();
											PerformDelete(Diagram, SelectedShapes, true);
											// Route event to the current tool in order to refresh the tool - otherwise it will 
											// not notice that the deleted shape was deleted...
											if(ActiveTool != null)
												ActiveTool.ProcessKeyEvent(this, WinFormHelpers.GetKeyEventArgs(KeyEventType.KeyDown, e));
											e.Handled = true;
										}
									}
								}
								break;

							// Select All
							case Keys.A:
								if((e.Modifiers & Keys.Control) == Keys.Control)
								{
									if(Diagram != null && SelectedShapes.Count != Diagram.Shapes.Count)
									{
										SelectAll();
										e.Handled = true;
									}
								}
								break;

							// Copy / Paste
							case Keys.Insert:
								if(e.Modifiers == Keys.Control)
								{
									// Copy
									if(Diagram != null
										&& SelectedShapes.Count > 0
										&& DiagramController.Owner.CanCopy(SelectedShapes))
									{
										PerformCopy(Diagram, SelectedShapes, true, Geometry.InvalidPoint);
										e.Handled = true;
									}
								}
								else if(e.Modifiers == Keys.Shift)
								{
									if(Diagram != null
										&& ActiveTool != null
										&& DiagramController.Owner.CanPaste(Diagram))
									{
										PerformPaste(Diagram, ActiveLayers, Geometry.InvalidPoint);
										e.Handled = true;
									}
								}
								break;

							// Copy
							case Keys.C:
								if((e.Modifiers & Keys.Control) == Keys.Control)
								{
									if(Diagram != null
									&& SelectedShapes.Count > 0
									&& DiagramController.Owner.CanCopy(SelectedShapes))
									{
										PerformCopy(Diagram, SelectedShapes, true, Geometry.InvalidPoint);
										e.Handled = true;
									}
								}
								break;

							// Paste
							case Keys.V:
								if((e.Modifiers & Keys.Control) == Keys.Control)
								{
									if(Diagram != null
										&& ActiveTool != null
										&& DiagramController.Owner.CanPaste(Diagram))
									{
										PerformPaste(Diagram, ActiveLayers, Geometry.InvalidPoint);
										e.Handled = true;
									}
								}
								break;

							// Cut
							case Keys.X:
								if((e.Modifiers & Keys.Control) == Keys.Control
									&& Diagram != null
									&& ActiveTool != null
									&& SelectedShapes.Count > 0
									&& DiagramController.Owner.CanCut(Diagram, SelectedShapes))
								{
									PerformCut(Diagram, SelectedShapes, true, Geometry.InvalidPoint);
									e.Handled = true;
								}
								break;

							// Undo/Redo
							case Keys.Z:
								if((e.Modifiers & Keys.Control) != 0
									&& Diagram != null
									&& ActiveTool != null)
								{
									if((e.Modifiers & Keys.Shift) != 0)
									{
										if(DiagramController.Owner.Project.History.RedoCommandCount > 0)
										{
											PerformRedo();
											e.Handled = true;
										}
									}
									else
									{
										if(DiagramController.Owner.Project.History.UndoCommandCount > 0)
										{
											PerformUndo();
											e.Handled = true;
										}
									}
								}
								break;

							case Keys.ShiftKey:
							case Keys.ControlKey:
							case Keys.LControlKey:
							case Keys.RControlKey:
								// Nothing to do here
								break;

							default:
								// Do nothing
								break;
						}
					}
					catch(Exception exc)
					{
						OnUserMessage(new UserMessageEventArgs(exc.Message));
					}
				}
			}
		}

		/// <override></override>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if(!e.Handled && ActiveTool != null)
			{
				try
				{
					e.Handled = ActiveTool.ProcessKeyEvent(this, WinFormHelpers.GetKeyEventArgs(KeyEventType.KeyUp, e));
					if(!e.Handled)
					{
						switch(e.KeyCode)
						{
							case Keys.ShiftKey:
							case Keys.ControlKey:
							case Keys.LControlKey:
							case Keys.RControlKey:
								// Nothing to do here
								break;

							default:
								// Do nothing
								break;
						}
					}
				}
				catch(Exception exc)
				{
					Debug.Print(exc.Message);
					ActiveTool.Cancel();
				}
			}
		}

		/// <override></override>
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			if(!e.Handled && ActiveTool != null)
			{
				KeyEventArgsDg eventArgs = WinFormHelpers.GetKeyEventArgs(e);
				try
				{
					e.Handled = ActiveTool.ProcessKeyEvent(this, eventArgs);
				}
				catch(Exception exc)
				{
					Debug.Print(exc.Message);
					ActiveTool.Cancel();
				}

				// Show caption editor
				if(!e.Handled
					&& selectedShapes.Count == 1
					&& selectedShapes.TopMost is ICaptionedShape
					&& !char.IsControl(eventArgs.KeyChar)
					&& Project.SecurityManager.IsGranted(Permission.Present, SelectedShapes.TopMost))
				{
					string pressedKey = eventArgs.KeyChar.ToString();
					ICaptionedShape labeledShape = (ICaptionedShape)selectedShapes.TopMost;
					if(labeledShape.CaptionCount > 0)
						DoOpenCaptionEditor(labeledShape, 0, pressedKey);
				}
			}
		}

		/// <override></override>
		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			base.OnDragEnter(drgevent);
		}

		/// <override></override>
		protected override void OnDragOver(DragEventArgs drgevent)
		{
			if(drgevent.Data.GetDataPresent(typeof(ModelObjectDragInfo)) && Diagram != null)
			{
				Point mousePosCtrl = PointToClient(MousePosition);
				Point mousePosDiagram;
				ControlToDiagram(mousePosCtrl, out mousePosDiagram);

				Shape shape = Diagram.Shapes.FindShape(mousePosDiagram.X, mousePosDiagram.Y, ControlPointCapabilities.None, 0, null);
				if(shape != null && shape.ModelObject == null)
					drgevent.Effect = DragDropEffects.Move;
				else drgevent.Effect = DragDropEffects.None;
			}
			else base.OnDragOver(drgevent);
		}

		/// <override></override>
		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			if(drgevent.Data.GetDataPresent(typeof(ModelObjectDragInfo)) && Diagram != null)
			{
				Point mousePosDiagram;
				ControlToDiagram(PointToClient(MousePosition), out mousePosDiagram);

				Shape shape = Diagram.Shapes.FindShape(mousePosDiagram.X, mousePosDiagram.Y, ControlPointCapabilities.None, 0, null);
				if(shape != null && shape.ModelObject == null)
				{
					ModelObjectDragInfo dragInfo = (ModelObjectDragInfo)drgevent.Data.GetData(typeof(ModelObjectDragInfo));
					ICommand cmd = new AssignModelObjectCommand(shape, dragInfo.ModelObject);
					Project.ExecuteCommand(cmd);
				}
			}
			else base.OnDragDrop(drgevent);
		}

		/// <override></override>
		protected override void OnDragLeave(EventArgs e)
		{
			base.OnDragLeave(e);
		}

		/// <override></override>
		protected override void OnContextMenuStripChanged(EventArgs e)
		{
			if(ContextMenuStrip != null && ContextMenuStrip != displayContextMenuStrip)
			{
				if(displayContextMenuStrip != null)
				{
					displayContextMenuStrip.Opening -= displayContextMenuStrip_Opening;
					displayContextMenuStrip.Closed -= displayContextMenuStrip_Closed;
				}
				displayContextMenuStrip = ContextMenuStrip;
				if(displayContextMenuStrip != null)
				{
					displayContextMenuStrip.Opening += displayContextMenuStrip_Opening;
					displayContextMenuStrip.Closed += displayContextMenuStrip_Closed;
				}
			}
			base.OnContextMenuStripChanged(e);
		}

		/// <override></override>
		protected override void OnScroll(ScrollEventArgs se)
		{
			// Do not call base.OnScroll() here!
			// Base implementation will be called in SetScrollPos()
			//
			bool isHScroll = (se.ScrollOrientation == ScrollOrientation.HorizontalScroll);
			if((isHScroll && !scrollBarHVisible) || (!isHScroll && !scrollBarVVisible))
				return;

			// Handle scroll messages sent directly to the control, e.g. by touch pads of notebooks.
			switch(se.Type)
			{
				case ScrollEventType.First:
					ScrollTo(se.ScrollOrientation, isHScroll ? scrollBarH.Minimum : scrollBarV.Minimum);
					break;
				case ScrollEventType.Last:
					ScrollTo(se.ScrollOrientation, isHScroll ? scrollBarH.Maximum : scrollBarV.Maximum);
					break;
				case ScrollEventType.LargeDecrement:
				case ScrollEventType.LargeIncrement:
					// Scroll by 1/4 of LargeChange because the LargeChange (== Display width/height) value 
					// would be too big for smooth scrolling
					int largeValue = (isHScroll ? scrollBarH.LargeChange : scrollBarV.LargeChange) / 4;
					ScrollBy(se.ScrollOrientation, (se.Type == ScrollEventType.SmallIncrement) ? largeValue : -largeValue);
					break;
				case ScrollEventType.SmallDecrement:
				case ScrollEventType.SmallIncrement:
					int smallValue = isHScroll ? scrollBarH.SmallChange : scrollBarV.SmallChange;
					ScrollBy(se.ScrollOrientation, (se.Type == ScrollEventType.SmallIncrement) ? smallValue : -smallValue);
					break;

				case ScrollEventType.EndScroll:
				case ScrollEventType.ThumbPosition:
				case ScrollEventType.ThumbTrack:
					// Nothing to do here because we cannot determine the correct scroll position anyway
					break;
				default:
					Debug.Fail(string.Format("Unhandled {0}.Type value: {1}", typeof(ScrollEventArgs).Name, se.Type));
					break;
			}
			// Avoid scrolling by the inherited AutoScroll mechanism
			AutoScrollPosition = Point.Empty;
		}

		/// <override></override>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
		//!	if(Diagram is Diagram)
		//!		Diagram.Size = Size;
			ResetBounds();
			UpdateScrollBars();
			Invalidate();
		}

		/// <override></override>
		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
		}

		/// <override></override>
		protected override void OnInvalidated(InvalidateEventArgs e)
		{
			base.OnInvalidated(e);
		}

		/// <override></override>
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			if(BackgroundImage != null) base.OnPaintBackground(e);
			else
			{
				try
				{
					CurrentGraphics = e.Graphics;
					CurrentGraphics.Clear(Color.Transparent);
					if(Parent != null && BackColor.A < 255 || BackColorGradient.A < 255)
					{
						// If the display has a transparent background, force the display's parent to redraw
						// by calling InvokePaintBackground and InvokePaint for the parent control.
						Rectangle rect = Rectangle.Empty;
						rect.Offset(Left, Top);
						rect.Width = Width;
						rect.Height = Height;
						e.Graphics.TranslateTransform(-rect.X, -rect.Y);
						try
						{
							using(PaintEventArgs pea = new PaintEventArgs(e.Graphics, rect))
							{
								pea.Graphics.SetClip(rect);
								InvokePaintBackground(Parent, pea);
								InvokePaint(Parent, pea);
							}
						}
						finally
						{
							e.Graphics.TranslateTransform(rect.X, rect.Y);
						}
					}
					DrawControl(e.ClipRectangle);
				}
				finally
				{
					CurrentGraphics = null;
				}
			}
		}

		/// <override></override>
		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
#if DEBUG_UI
				//Console.WriteLine("[{0}]\t OnPaint called", DateTime.Now.ToString("HH:mm:ss.ffff"));
				stopWatch.Reset();
				stopWatch.Start();
#endif
				CurrentGraphics = e.Graphics;

				// =====   DRAW DIAGRAM   =====
				if(Diagram != null) DrawDiagram(e);

				// =====   DRAW UNIVERSAL SCROLL INDICATOR   =====
				if(universalScrollEnabled)
					universalScrollCursor.Draw(currentGraphics, universalScrollFixPointBounds);

				// =====   DRAW DEBUG INFO   =====
#if DEBUG_UI
				stopWatch.Stop();
				string debugInfoTxt = string.Format("{1} ms{0}", Environment.NewLine, stopWatch.ElapsedMilliseconds);
				currentGraphics.DrawString(debugInfoTxt, Font, Brushes.Red, Point.Empty);

				//float drawAreaCenterX = (DrawBounds.X + DrawBounds.Width) / 2f;
				//float drawAreaCenterY = (DrawBounds.Y + DrawBounds.Height) / 2f;
				//float zoomedDiagramWidth = (DiagramBounds.Width * zoomfactor) + (2 * scrollAreaMargin);
				//float zoomedDiagramHeight = (DiagramBounds.Height * zoomfactor) + (2 * scrollAreaMargin);

				//Rectangle r;
				//r = DiagramBounds;
				//DiagramToControl(r, out r);
				//currentGraphics.DrawRectangle(Pens.Red, r.X, r.Y, r.Width, r.Height);
				//r = ScrollAreaBounds;
				//DiagramToControl(r, out r);
				//currentGraphics.DrawRectangle(Pens.Red, r.X, r.Y, r.Width, r.Height);
#endif
			}
			finally
			{
				CurrentGraphics = null;
			}
		}

		/// <override></override>
		protected override void NotifyInvalidate(Rectangle invalidatedArea)
		{
			base.NotifyInvalidate(invalidatedArea);
		}

		#endregion


		#region [Protected] Methods

		/// <summary>
		/// Draws a resize grip at the given position.
		/// </summary>
		protected virtual void DrawResizeGripCore(int x, int y, IndicatorDrawMode drawMode)
		{
			if(currentGraphics == null) throw new InvalidOperationException(ErrMessageOnlyAllowedWhilePaining);
			if(graphicsIsTransformed) throw new NShapeException(ErrMessageResetTransformHasNotBeenCalled);
			if(HighQualityRendering)
			{
				Pen handlePen = null;
				Brush handleBrush = null;
				switch(drawMode)
				{
					case IndicatorDrawMode.Normal:
						handlePen = HandleNormalPen;
						handleBrush = HandleInteriorBrush;
						break;
					case IndicatorDrawMode.Deactivated:
						handlePen = HandleInactivePen;
						handleBrush = Brushes.Transparent;
						break;
					case IndicatorDrawMode.Highlighted:
						handlePen = HandleHilightPen;
						handleBrush = HandleInteriorBrush;
						break;
					default: throw new NShapeUnsupportedValueException(typeof(IndicatorDrawMode), drawMode);
				}
				DoDrawControlPointPath(resizePointPath, x, y, handlePen, handleBrush);
			}
			else
			{
				DiagramToControl(x, y, out x, out y);
				rectBuffer.X = x - handleRadius;
				rectBuffer.Y = y - handleRadius;
				rectBuffer.Width = rectBuffer.Height = handleRadius + handleRadius;
				ControlPaint.DrawContainerGrabHandle(currentGraphics, rectBuffer);
			}
		}


		/// <summary>
		/// Draws a resize grip at the given position.
		/// </summary>
		protected virtual void DrawRotateGripCore(int x, int y, IndicatorDrawMode drawMode)
		{
			if(currentGraphics == null) throw new InvalidOperationException(ErrMessageOnlyAllowedWhilePaining);
			if(graphicsIsTransformed) throw new NShapeException(ErrMessageResetTransformHasNotBeenCalled);
			if(HighQualityRendering)
			{
				Pen handlePen = null;
				Brush handleBrush = null;
				switch(drawMode)
				{
					case IndicatorDrawMode.Normal:
						handlePen = HandleNormalPen;
						handleBrush = HandleInteriorBrush;
						break;
					case IndicatorDrawMode.Deactivated:
						handlePen = HandleInactivePen;
						handleBrush = Brushes.Transparent;
						break;
					case IndicatorDrawMode.Highlighted:
						handlePen = HandleHilightPen;
						handleBrush = HandleInteriorBrush;
						break;
					default: throw new NShapeUnsupportedValueException(typeof(IndicatorDrawMode), drawMode);
				}
				DoDrawControlPointPath(rotatePointPath, x, y, handlePen, handleBrush);
			}
			else
			{
				DiagramToControl(x, y, out x, out y);
				rectBuffer.X = x - handleRadius;
				rectBuffer.Y = y - handleRadius;
				rectBuffer.Width =
				rectBuffer.Height = handleRadius + handleRadius;
				ControlPaint.DrawGrabHandle(currentGraphics, rectBuffer, false, (drawMode == IndicatorDrawMode.Deactivated));
			}
		}


		/// <summary>
		/// Draws a connection point at the given position. 
		/// If the connection point is also a resize grip, the resize grip will be drawn, too.
		/// If the connection point is a is point-to-shape connected glue point, the outline of the connected shape will be highlighted.
		/// </summary>
		protected virtual void DrawConnectionPointCore(int x, int y, IndicatorDrawMode drawMode, bool isResizeGrip)
		{
			DrawConnectionPointCore(x, y, drawMode, isResizeGrip, ShapeConnectionInfo.Empty);
		}


		/// <summary>
		/// Draws a connection point at the given position. 
		/// If the connection point is also a resize grip, the resize grip will be drawn, too.
		/// If the connection point is a is point-to-shape connected glue point, the outline of the connected shape will be highlighted.
		/// </summary>
		protected virtual void DrawConnectionPointCore(int x, int y, IndicatorDrawMode drawMode, bool isResizeGrip, ShapeConnectionInfo connection)
		{
			if(currentGraphics == null) throw new InvalidOperationException(ErrMessageOnlyAllowedWhilePaining);
			if(graphicsIsTransformed) throw new NShapeException(ErrMessageResetTransformHasNotBeenCalled);
			if(HighQualityRendering)
			{
				int hdlRad;
				Pen handlePen = null;
				Brush handleBrush = null;
				switch(drawMode)
				{
					case IndicatorDrawMode.Normal:
						handlePen = HandleInactivePen;
						handleBrush = HandleInteriorBrush;
						hdlRad = handleRadius;
						// If the control point is a glue point, highlight the connected connection points
						if(!connection.IsEmpty)
						{
							if(connection.OtherPointId == ControlPointId.Reference)
							{
								// If the glue point is attached to a shape instead of a connection point, highlight the connected shape's outline.
								DoRestoreTransformation();
								DoDrawShapeOutline(IndicatorDrawMode.Highlighted, connection.OtherShape);
								DoResetTransformation();
							}
							handlePen = HandleHilightPen;
						}
						break;
					case IndicatorDrawMode.Deactivated:
						handlePen = HandleInactivePen;
						handleBrush = Brushes.Transparent;
						hdlRad = handleRadius;
						break;
					case IndicatorDrawMode.Highlighted:
						handlePen = HandleHilightPen;
						handleBrush = HandleInteriorBrush;
						hdlRad = handleRadius + 1;
						break;
					default: throw new NShapeUnsupportedValueException(typeof(IndicatorDrawMode), drawMode);
				}
				if(isResizeGrip)
				{
					// Determine the grip that has to be drawn first
					if(resizePointShape > connectionPointShape)
					{
						DrawResizeGripCore(x, y, IndicatorDrawMode.Normal);
						DoDrawControlPointPath(connectionPointPath, x, y, handlePen, handleBrush);
					}
					else
					{
						DoDrawControlPointPath(connectionPointPath, x, y, handlePen, handleBrush);
						DrawResizeGripCore(x, y, IndicatorDrawMode.Normal);
					}

				}
				else DoDrawControlPointPath(connectionPointPath, x, y, handlePen, handleBrush);
			}
			else
			{
				DiagramToControl(x, y, out x, out y);
				rectBuffer.X = x - handleRadius;
				rectBuffer.Y = y - handleRadius;
				rectBuffer.Width =
				rectBuffer.Height = handleRadius + handleRadius;
				ControlPaint.DrawGrabHandle(currentGraphics, rectBuffer, true, true);
			}
		}

		#endregion


		#region [Protected] Method overrides

		/// <override></override>
		protected override bool IsInputKey(Keys keyData)
		{
			// Get raw key data (pressed key without modifier keys)
			Keys rawKeyData = keyData & ~Keys.Modifiers;
			// Define cursor keys input keys for the display control. 
			// This is necessary because the display's scrollbars will receive the focus when 
			// pressing the arrow keys (e.g. for moving shapes with keyboard) which
			// will break scrolling and zooming behavior as the scrollbar handle arrow keys and mouse
			// wheel themselfs (for scrolling) and they will not loose the focus until another control 
			// (that is not part of the display control) was focused.
			switch(rawKeyData)
			{
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
					return true;
				// Handling the modifier keys as InputKeys is not necessary but useful for debugging...
				//case Keys.ShiftKey:
				//case Keys.LShiftKey:
				//case Keys.RShiftKey:
				//case Keys.ControlKey:
				//case Keys.LControlKey:
				//case Keys.RControlKey:
				//case Keys.Alt:
				//    return true;
				default:
					return base.IsInputKey(keyData);
			}
		}

		#endregion


		private bool MultiSelect
		{
			get
			{
				if((Control.ModifierKeys & Keys.Shift) == Keys.Shift) return true;
				else if((Control.ModifierKeys & Keys.ShiftKey) == Keys.ShiftKey) return true;
				else if((Control.ModifierKeys & Keys.Control) == Keys.Control) return true;
				else if((Control.ModifierKeys & Keys.ControlKey) == Keys.ControlKey) return true;
				else return false;
			}
		}


		#region [Private] Properties: Pens, Brushes, Bounds, etc

		private Graphics CurrentGraphics
		{
			get { return currentGraphics; }
			set
			{
				if(currentGraphics != null)
				{
					if(value != null)
						throw new InvalidOperationException("Current graphics is not null. " +
							"If this happens in the WinForms Designer, make sure your application's target framework is not a 'Client Profile' framework version.");
					graphicsIsTransformed = false;
				}
				currentGraphics = value;
				if(currentGraphics != null)
				{
					GdiHelpers.ApplyGraphicsSettings(currentGraphics, currentRenderingQuality);
					graphicsIsTransformed = false;
				}
			}
		}


		/// <summary>
		/// The bounds of the diagram 'sheet'.
		/// </summary>
		private Rectangle DiagramBounds
		{
			get
			{
				if(Diagram == null) return Rectangle.Empty;
				else
				{
					Rectangle r = Rectangle.Empty;
					r.Location = Diagram.Location;
					r.Size = Diagram.Size;
					return r;
				}
			}
		}


		/// <summary>
		/// The bounds of the scrollable area: The bounds of the diagram including all its shapes (also shapes beside the diagram 'sheet') and the scroll margin.
		/// </summary>
		private Rectangle ScrollAreaBounds
		{
			get
			{
				if(Diagram == null) return Rectangle.Empty;
				if(!Geometry.IsValid(scrollAreaBounds))
				{
					Rectangle shapeBounds = Diagram.Shapes.GetBoundingRectangle(false);
					scrollAreaBounds = Geometry.UniteRectangles(0, 0, Diagram.Width, Diagram.Height, Geometry.IsValid(shapeBounds) ? shapeBounds : Rectangle.Empty);
					scrollAreaBounds.Inflate(scrollAreaMargin, scrollAreaMargin);
				}
				return scrollAreaBounds;
			}
		}


		private bool HScrollBarVisible
		{
			get { return scrollBarHVisible; }
			set
			{
				scrollBarHVisible = value;
				scrollBarH.Visible = scrollBarHVisible;
				hScrollBarPanel.Visible = scrollBarHVisible;
				if(!scrollBarHVisible) SetScrollPosX(0);
			}
		}


		private bool VScrollBarVisible
		{
			get { return scrollBarVVisible; }
			set
			{
				scrollBarVVisible = value;
				scrollBarV.Visible = scrollBarVVisible;
				if(!scrollBarVVisible) SetScrollPosY(0);
			}
		}


		private Pen GridPen
		{
			get
			{
				if(gridPen == null)
					//CreatePen(gridColor, gridAlpha, 1, new float[2] { 2, 2 }, false, ref gridPen);
					CreatePen(gridColor, gridAlpha, 1, null, false, ref gridPen);
				return gridPen;
			}
		}


		private Pen OutlineInteriorPen
		{
			get
			{
				if(outlineInteriorPen == null) CreatePen(selectionInteriorColor, ref outlineInteriorPen);
				return outlineInteriorPen;
			}
		}


		private Pen OutlineNormalPen
		{
			get
			{
				if(outlineNormalPen == null) CreatePen(selectionNormalColor, selectionAlpha, handleRadius, ref outlineNormalPen);
				return outlineNormalPen;
			}
		}


		private Pen OutlineHilightPen
		{
			get
			{
				if(outlineHilightPen == null) CreatePen(selectionHilightColor, selectionAlpha, handleRadius, ref outlineHilightPen);
				return outlineHilightPen;
			}
		}


		private Pen OutlineInactivePen
		{
			get
			{
				if(outlineInactivePen == null) CreatePen(selectionInactiveColor, selectionAlpha, handleRadius, ref outlineInactivePen);
				return outlineInactivePen;
			}
		}


		private Pen HandleNormalPen
		{
			get
			{
				if(handleNormalPen == null) CreatePen(selectionNormalColor, ref handleNormalPen);
				return handleNormalPen;
			}
		}


		private Pen HandleHilightPen
		{
			get
			{
				if(handleHilightPen == null) CreatePen(selectionHilightColor, ref handleHilightPen);
				return handleHilightPen;
			}
		}


		private Pen HandleInactivePen
		{
			get
			{
				if(handleInactivePen == null) CreatePen(selectionInactiveColor, ref handleInactivePen);
				return handleInactivePen;
			}
		}


		private Brush HandleInteriorBrush
		{
			get
			{
				if(handleInteriorBrush == null) CreateBrush(selectionInteriorColor, selectionAlpha, ref handleInteriorBrush);
				return handleInteriorBrush;
			}
		}


		private Brush InplaceTextboxBackBrush
		{
			get
			{
				if(inplaceTextboxBackBrush == null)
					CreateBrush(SelectionInteriorColor, inlaceTextBoxBackAlpha, ref inplaceTextboxBackBrush);
				return inplaceTextboxBackBrush;
			}
		}


		private Pen ToolPreviewPen
		{
			get
			{
				if(toolPreviewPen == null) CreatePen(toolPreviewColor, toolPreviewColorAlpha, ref toolPreviewPen);
				return toolPreviewPen;
			}
		}


		private Brush ToolPreviewBackBrush
		{
			get
			{
				if(toolPreviewBackBrush == null) CreateBrush(toolPreviewBackColor, toolPreviewBackColorAlpha, ref toolPreviewBackBrush);
				return toolPreviewBackBrush;
			}
		}

		#endregion


		#region [Private] Methods: Invalidating

		/// <override></override>
		private void DoSuspendUpdate()
		{
			if(suspendUpdateCounter == 0)
				Debug.Assert(invalidatedAreaBuffer == Rectangle.Empty);
			++suspendUpdateCounter;
		}


		/// <override></override>
		private void DoResumeUpdate()
		{
			if(suspendUpdateCounter <= 0) throw new InvalidOperationException("Missing subsequent call of method SuspendUpdate.");
			--suspendUpdateCounter;
			if(suspendUpdateCounter == 0)
			{
				if(boundsChanged)
				{
					ResetBounds();
					UpdateScrollBars();
					Invalidate();
				}
				else
				{
					if(!invalidatedAreaBuffer.IsEmpty && Geometry.IsValid(invalidatedAreaBuffer))
						base.Invalidate(invalidatedAreaBuffer, true);
				}
				invalidatedAreaBuffer = Rectangle.Empty;
			}
		}


		/// <summary>
		/// Invalidates the shape (or its parent(s)) along with all ControlPoints
		/// </summary>
		/// <param name="shape"></param>
		private void DoInvalidateShape(Shape shape)
		{
			DoSuspendUpdate();
			if(shape.Parent != null)
				// parents invalidate their children themselves
				DoInvalidateShape(shape.Parent);
			else
			{
				shape.Invalidate();
				foreach(ControlPointId gluePtId in shape.GetControlPointIds(ControlPointCapabilities.Glue))
				{
					if(shape.IsConnected(gluePtId, null) == ControlPointId.Reference)
						DoInvalidateShape(shape.GetConnectionInfo(gluePtId, null).OtherShape);
				}
				DoInvalidateGrips(shape, ControlPointCapabilities.All);
			}
			DoResumeUpdate();
		}


		private void DoInvalidateGrips(Shape shape, ControlPointCapabilities controlPointCapability)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			Point p = Point.Empty;
			int transformedHandleRadius;
			ControlToDiagram(handleRadius, out transformedHandleRadius);
			++transformedHandleRadius;
			Rectangle r = Rectangle.Empty;
			foreach(ControlPointId id in shape.GetControlPointIds(controlPointCapability))
			{
				p = shape.GetControlPointPosition(id);
				if(r.IsEmpty)
				{
					r.X = p.X;
					r.Y = p.Y;
					r.Width = r.Height = 1;
				}
				else r = Geometry.UniteRectangles(p.X, p.Y, p.X, p.Y, r);
			}
			r.Inflate(transformedHandleRadius, transformedHandleRadius);
			DoInvalidateDiagram(r);

			// This consumes twice the time of the solution above:
			//int transformedHandleSize = transformedHandleRadius+transformedHandleRadius;
			//foreach (int pointId in shape.GetControlPointIds(controlPointCapabilities)) {
			//   p = shape.GetControlPointPosition(pointId);
			//   Invalidate(p.X - transformedHandleRadius, p.Y - transformedHandleRadius, transformedHandleSize, transformedHandleSize);
			//}
		}


		private void DoInvalidateDiagram(int x, int y, int width, int height)
		{
			rectBuffer.X = x;
			rectBuffer.Y = y;
			rectBuffer.Width = width;
			rectBuffer.Height = height;
			DoInvalidateDiagram(rectBuffer);
		}


		private void DoInvalidateDiagram(Rectangle rect)
		{
			if(!Geometry.IsValid(rect)) throw new ArgumentException("rect");
			DiagramToControl(rect, out rectBuffer);
			rectBuffer.Inflate(invalidateDelta + 1, invalidateDelta + 1);

#if DEBUG_UI
			if(isInvalidateAreasVisible)
			{
				if(invalidatedAreas == null) invalidatedAreas = new List<Rectangle>();
				invalidatedAreas.Add(rect);
			}
#endif

			// traditional rendering
			if(suspendUpdateCounter > 0) invalidatedAreaBuffer = Geometry.UniteRectangles(invalidatedAreaBuffer, rectBuffer);
			else base.Invalidate(rectBuffer);
		}

		#endregion


		#region [Private] Methods: Calculating and applying transformation

		private void CalcDiagramPosition()
		{
			float zoomedDiagramWidth = (ScrollAreaBounds.Width - (2 * scrollAreaMargin)) * zoomfactor;
			float zoomedDiagramHeight = (ScrollAreaBounds.Height - (2 * scrollAreaMargin)) * zoomfactor;
			float zoomedDiagramOffsetX = ((ScrollAreaBounds.X + scrollAreaMargin) * zoomfactor);
			float zoomedDiagramOffsetY = ((ScrollAreaBounds.Y + scrollAreaMargin) * zoomfactor);
			float drawAreaCenterX = (DrawBounds.X + DrawBounds.Width) / 2f;
			float drawAreaCenterY = (DrawBounds.Y + DrawBounds.Height) / 2f;
			diagramPosX = (int)Math.Round(-zoomedDiagramOffsetX + (drawAreaCenterX - (zoomedDiagramWidth / 2)));
			diagramPosY = (int)Math.Round(-zoomedDiagramOffsetY + (drawAreaCenterY - (zoomedDiagramHeight / 2)));
		}


		private void DoRestoreTransformation()
		{
			if(currentGraphics == null) throw new InvalidOperationException(ErrMessageOnlyAllowedWhilePaining);
			if(graphicsIsTransformed) throw new InvalidOperationException("Graphics context is already transformed.");
			// transform graphics object
			currentGraphics.ScaleTransform(zoomfactor, zoomfactor, MatrixOrder.Prepend);
			currentGraphics.TranslateTransform(diagramPosX, diagramPosY, MatrixOrder.Append);
			currentGraphics.TranslateTransform(-scrollPosX * zoomfactor, -scrollPosY * zoomfactor, MatrixOrder.Append);
			graphicsIsTransformed = true;
		}


		private void DoResetTransformation()
		{
			if(currentGraphics == null) throw new InvalidOperationException(ErrMessageOnlyAllowedWhilePaining);
			if(!graphicsIsTransformed) throw new InvalidOperationException("Graphics context is not transformed.");
			Debug.Assert(graphicsIsTransformed);
			currentGraphics.ResetTransform();
			graphicsIsTransformed = false;
		}

		#endregion


		#region [Private] Methods: Drawing

		/// <summary>
		/// Draws the bounds of all captions of the shape
		/// </summary>
		private void DoDrawCaptionBounds(IndicatorDrawMode drawMode, ICaptionedShape shape)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			if(graphicsIsTransformed) throw new NShapeException(ErrMessageResetTransformHasNotBeenCalled);
			if(Project.SecurityManager.IsGranted(Permission.Data, (Shape)shape))
			{
				for(int i = shape.CaptionCount - 1; i >= 0; --i)
					DoDrawCaptionBounds(drawMode, shape, i);
			}
		}


		private void DoDrawCaptionBounds(IndicatorDrawMode drawMode, ICaptionedShape shape, int captionIndex)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			if(currentGraphics == null) throw new InvalidOperationException(ErrMessageOnlyAllowedWhilePaining);
			if(graphicsIsTransformed) throw new NShapeException(ErrMessageResetTransformHasNotBeenCalled);
			// Skip shapes that are not visible
			if(!IsLayerVisible(((Shape)shape).Layers))
				return;
			if(inplaceTextbox == null || inplaceShape != shape || inplaceCaptionIndex != captionIndex)
			{
				// Draw caption rectangle (placeholder bounds are calculated for empty captions
				shape.GetCaptionTextBounds(captionIndex, out pointBuffer[0], out pointBuffer[1], out pointBuffer[2], out pointBuffer[3]);
				DiagramToControl(pointBuffer[0], out pointBuffer[0]);
				DiagramToControl(pointBuffer[1], out pointBuffer[1]);
				DiagramToControl(pointBuffer[2], out pointBuffer[2]);
				DiagramToControl(pointBuffer[3], out pointBuffer[3]);
				Pen pen = null;
				switch(drawMode)
				{
					case IndicatorDrawMode.Deactivated:
						pen = HandleInactivePen;
						break;
					case IndicatorDrawMode.Normal:
						pen = HandleNormalPen;
						break;
					case IndicatorDrawMode.Highlighted:
						pen = HandleHilightPen;
						break;
					default: throw new NShapeUnsupportedValueException(drawMode);
				}
				currentGraphics.DrawPolygon(pen, pointBuffer);
			}
		}


		private void DoDrawControlPoints(Shape shape)
		{
			DoDrawControlPoints(shape, ControlPointCapabilities.Resize | ControlPointCapabilities.Connect | ControlPointCapabilities.Glue);
		}


		private void DoDrawControlPoints(Shape shape, ControlPointCapabilities capabilities)
		{
			DoDrawControlPoints(shape, capabilities, IndicatorDrawMode.Normal);
		}


		private void DoDrawControlPoints(Shape shape, ControlPointCapabilities capabilities, IndicatorDrawMode drawMode)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			if(graphicsIsTransformed) throw new NShapeException(ErrMessageResetTransformHasNotBeenCalled);
			Point p = Point.Empty;
			if(!IsLayerVisible(((Shape)shape).Layers))
				return;
			// First, draw Resize- and ConnectionPoints
			foreach(ControlPointId id in shape.GetControlPointIds(capabilities))
			{
				if(id == ControlPointId.Reference) continue;

				// Get point position and transform the coordinates
				p = shape.GetControlPointPosition(id);
				bool isResizeGrip = shape.HasControlPointCapability(id, ControlPointCapabilities.Resize);
				if(shape.HasControlPointCapability(id, ControlPointCapabilities.Glue))
				{
					ShapeConnectionInfo connectionInfo = shape.GetConnectionInfo(id, null);
					DrawConnectionPointCore(p.X, p.Y, drawMode, isResizeGrip, connectionInfo);
				}
				else if(shape.HasControlPointCapability(id, ControlPointCapabilities.Connect))
				{
					if(shape.HasControlPointCapability(id, ControlPointCapabilities.Reference)) continue;
					DrawConnectionPointCore(p.X, p.Y, drawMode, isResizeGrip);
				}
				else if(isResizeGrip)
					DrawResizeGripCore(p.X, p.Y, drawMode);
			}
			// Draw the roation point on top of all other points
			if((capabilities & ControlPointCapabilities.Rotate) != 0)
			{
				foreach(ControlPointId id in shape.GetControlPointIds(ControlPointCapabilities.Rotate))
				{
					p = shape.GetControlPointPosition(id);
					DrawRotateGripCore(p.X, p.Y, drawMode);
				}
			}
		}


		private void DoDrawShapeOutline(IndicatorDrawMode drawMode, Shape shape)
		{
			Debug.Assert(shape != null);
			Debug.Assert(currentGraphics != null);
			if(!IsLayerVisible(((Shape)shape).Layers))
				return;

			Pen backgroundPen = null;
			Pen foregroundPen = null;
			if(shape.Parent != null) DoDrawParentOutline(shape.Parent);
			switch(drawMode)
			{
				case IndicatorDrawMode.Deactivated:
					backgroundPen = OutlineInactivePen;
					foregroundPen = OutlineInteriorPen;
					break;
				case IndicatorDrawMode.Normal:
					backgroundPen = OutlineNormalPen;
					foregroundPen = OutlineInteriorPen;
					break;
				case IndicatorDrawMode.Highlighted:
					backgroundPen = OutlineHilightPen;
					foregroundPen = OutlineInteriorPen;
					break;
				default: throw new NShapeUnsupportedValueException(typeof(IndicatorDrawMode), drawMode);
			}
			// scale lineWidth 
			backgroundPen.Width = GripSize / zoomfactor;
			foregroundPen.Width = 1 / zoomfactor;

			shape.DrawOutline(currentGraphics, backgroundPen);
			shape.DrawOutline(currentGraphics, foregroundPen);
		}


		private void DoDrawParentOutline(Shape parentShape)
		{
			Debug.Assert(currentGraphics != null);
			if(!IsLayerVisible(((Shape)parentShape).Layers))
				return;
			if(parentShape.Parent != null) DoDrawParentOutline(parentShape.Parent);
			parentShape.DrawOutline(currentGraphics, OutlineInactivePen);
		}


		/// <summary>
		/// Translates and draws the given ControlPoint path at the given position (diagram coordinates).
		/// </summary>
		private void DoDrawControlPointPath(GraphicsPath path, int x, int y, Pen pen, Brush brush)
		{
			if(currentGraphics == null) throw new InvalidOperationException(ErrMessageOnlyAllowedWhilePaining);
			// transform the given 
			DiagramToControl(x, y, out x, out y);

			// transform ControlPoint Shape
			pointMatrix.Reset();
			pointMatrix.Translate(x, y);
			path.Transform(pointMatrix);
			// draw ConnectionPoint shape
			currentGraphics.FillPath(brush, path);
			currentGraphics.DrawPath(pen, path);
			// undo ControlPoint transformation
			pointMatrix.Reset();
			pointMatrix.Translate(-x, -y);
			path.Transform(pointMatrix);
		}


		private void UpdateScrollBars()
		{
			try
			{
				//SuspendLayout();
				if(showScrollBars && Diagram != null)
				{
					// Update diagram offset and draw area
					Rectangle drawBoundsDgrmCoords;
					CalcDiagramPosition();
					ControlToDiagram(DrawBounds, out drawBoundsDgrmCoords);

					// Show/hide vertical scroll bar
					if(ScrollAreaBounds.Height < drawBoundsDgrmCoords.Height)
					{
						if(VScrollBarVisible) VScrollBarVisible = false;
					}
					else if(!VScrollBarVisible && showScrollBars) VScrollBarVisible = true;
					// Show/hide horizontal scroll bar
					if(ScrollAreaBounds.Width < drawBoundsDgrmCoords.Width)
					{
						if(HScrollBarVisible)
						{
							HScrollBarVisible = false;
							//Debug.Assert(hScrollBarPanel.Visible == false && scrollBarH.Visible == false);
						}
					}
					else if(!HScrollBarVisible && showScrollBars)
					{
						HScrollBarVisible = true;
						//Debug.Assert(hScrollBarPanel.Visible == true && scrollBarH.Visible == true);
					}
					// Set scrollbar's width/height
					scrollBarV.Height = DrawBounds.Height;
					scrollBarH.Width = DrawBounds.Width;

					// Update diagram offset and draw area
					CalcDiagramPosition();
					ControlToDiagram(DrawBounds, out drawBoundsDgrmCoords);
					if(HScrollBarVisible || VScrollBarVisible)
					{
						int zoomedDiagramPosX = (int)Math.Round(diagramPosX / zoomfactor);
						int zoomedDiagramPosY = (int)Math.Round(diagramPosY / zoomfactor);
						int smallChange = 1;    // Math.Max(1, GridSize / 2);

						// Set up vertical scrollbar
						if(VScrollBarVisible)
						{
							scrollBarV.SmallChange = smallChange;
							scrollBarV.LargeChange = Math.Max(1, drawBoundsDgrmCoords.Height);
							scrollBarV.Minimum = ScrollAreaBounds.Y + zoomedDiagramPosY;
							scrollBarV.Maximum = ScrollAreaBounds.Height + scrollBarV.Minimum;
						}

						// Set horizontal scrollBar position, size  and limits
						if(HScrollBarVisible)
						{
							scrollBarH.SmallChange = smallChange;
							scrollBarH.LargeChange = Math.Max(1, drawBoundsDgrmCoords.Width);
							scrollBarH.Minimum = ScrollAreaBounds.X + zoomedDiagramPosX;
							scrollBarH.Maximum = ScrollAreaBounds.Width + scrollBarH.Minimum;
						}

						// Maintain scroll position when zooming out
						ScrollTo(scrollBarH.Value, scrollBarV.Value);
					}
				}
				else
				{
					if(VScrollBarVisible) VScrollBarVisible = false;
					if(HScrollBarVisible) HScrollBarVisible = false;
				}
				boundsChanged = false;
			}
			finally
			{
				//ResumeLayout(); 
			}
		}


		private void UpdateControlBrush()
		{
			if(controlBrush == null)
			{
				if(highQualityBackground)
				{
					rectBuffer.X = 0;
					rectBuffer.Y = 0;
					rectBuffer.Width = 1000;
					rectBuffer.Height = 1000;
					if(gradientBackColor == Color.Empty)
					{
						// create a gradient brush based on the given BackColor (1/3 lighter in the upperLeft, 1/3 darker in the lowerRight)
						int lR = BackColor.R + ((BackColor.R / 3)); if(lR > 255) lR = 255;
						int lG = BackColor.G + ((BackColor.G / 3)); if(lG > 255) lG = 255;
						int lB = BackColor.B + ((BackColor.B / 3)); if(lB > 255) lB = 255;
						int dR = BackColor.R - ((BackColor.R / 3)); if(lR < 0) lR = 0;
						int dG = BackColor.G - ((BackColor.G / 3)); if(lG < 0) lG = 0;
						int dB = BackColor.B - ((BackColor.B / 3)); if(lB < 0) lB = 0;
						controlBrush = new LinearGradientBrush(rectBuffer, Color.FromArgb(lR, lG, lB), Color.FromArgb(dR, dG, dB), controlBrushGradientAngle);
					}
					else controlBrush = new LinearGradientBrush(rectBuffer, BackColorGradient, BackColor, controlBrushGradientAngle);
				}
				else controlBrush = new SolidBrush(BackColor);
				controlBrushSize = Size.Empty;
			}

			// apply transformation
			if(controlBrush is LinearGradientBrush && this.Size != controlBrushSize)
			{
				double rectWidth = Math.Abs((1000 * controlBrushGradientCos) - (1000 * controlBrushGradientSin));       // (width * cos) - (Height * sin)
				double rectHeight = Math.Abs((1000 * controlBrushGradientSin) + (1000 * controlBrushGradientCos));      // (width * sin) + (height * cos)
				double gradLen = (rectWidth + rectHeight) / 2;
				float scaleX = (float)(Width / gradLen);
				float scaleY = (float)(Height / gradLen);

				((LinearGradientBrush)controlBrush).ResetTransform();
				((LinearGradientBrush)controlBrush).ScaleTransform(scaleX, scaleY);
				((LinearGradientBrush)controlBrush).RotateTransform(controlBrushGradientAngle);
				controlBrushSize = this.Size;
			}
		}


		/// <summary>
		/// Draw the display control including the diagram's shadow (if a diagram is displayed)
		/// </summary>
		private void DrawControl(Rectangle clipRectangle)
		{
			UpdateControlBrush();
			clipRectangle.Inflate(2, 2);
			if(Diagram == null || !drawDiagramSheet)
			{
				// No diagram is shown... just fill the control with its background color/gradient
				//graphics.FillRectangle(controlBrush, clipRectangle.X, clipRectangle.Y, clipRectangle.Width, clipRectangle.Height);
				currentGraphics.FillRectangle(controlBrush, ClientRectangle);
			}
			else
			{
				Rectangle ctrlDiagramBounds = Rectangle.Empty;
				DiagramToControl(DiagramBounds, out ctrlDiagramBounds);

				// =====   DRAW CONTROL BACKGROUND   =====
				if(Diagram != null && !Diagram.BackgroundColor.IsEmpty && Diagram.BackgroundColor.A == 255 && Diagram.BackgroundGradientColor.A == 255)
				{
					// Above the diagram
					if(clipRectangle.Top < ctrlDiagramBounds.Top)
						currentGraphics.FillRectangle(controlBrush,
							clipRectangle.Left, clipRectangle.Top,
							clipRectangle.Width, ctrlDiagramBounds.Top - clipRectangle.Top);
					// Left of the diagram
					if(clipRectangle.Left < ctrlDiagramBounds.Left)
						currentGraphics.FillRectangle(controlBrush,
							clipRectangle.Left, Math.Max(clipRectangle.Top, ctrlDiagramBounds.Top - 1),
							ctrlDiagramBounds.Left - clipRectangle.Left, Math.Min(clipRectangle.Height, ctrlDiagramBounds.Height + 2));
					// Right of the diagram
					if(clipRectangle.Right > ctrlDiagramBounds.Right)
						currentGraphics.FillRectangle(controlBrush,
							ctrlDiagramBounds.Right, Math.Max(clipRectangle.Top, ctrlDiagramBounds.Top - 1),
							clipRectangle.Right - ctrlDiagramBounds.Right, Math.Min(clipRectangle.Height, ctrlDiagramBounds.Height + 2));
					// Below the diagram
					if(clipRectangle.Bottom > ctrlDiagramBounds.Bottom)
						currentGraphics.FillRectangle(controlBrush,
							clipRectangle.Left, ctrlDiagramBounds.Bottom,
							clipRectangle.Width, clipRectangle.Bottom - ctrlDiagramBounds.Bottom);
				}
				else currentGraphics.FillRectangle(controlBrush, clipRectangle);

				// =====   DRAW DIAGRAM SHADOW   =====
				if(clipRectangle.Right >= ctrlDiagramBounds.Right || clipRectangle.Bottom >= ctrlDiagramBounds.Bottom)
				{
					if(Diagram.BackgroundColor.A == 0 && Diagram.BackgroundGradientColor.A == 0 && NamedImage.IsNullOrEmpty(Diagram.BackgroundImage))
					{
						// Transparent diagrams have no shadow... so there's nothing to do.
					}
					else
					{
						// Change alpha value of brush according to the diagram's alpha:
						// This solves the problem that transparent 
						if(diagramShadowBrush is SolidBrush)
						{
							// Shadow alpha is half of the average value of the background color's alpha:
							int alpha = Math.Max(1, (Diagram.BackgroundColor.A + Diagram.BackgroundGradientColor.A) / 4);
							if(alpha != ((SolidBrush)diagramShadowBrush).Color.A)
							{
								Color shadowColor = Color.FromArgb(alpha, ((SolidBrush)diagramShadowBrush).Color);
								DisposeObject(ref diagramShadowBrush);
								diagramShadowBrush = new SolidBrush(shadowColor);
							}
						}
						// Now draw the shadow
						int zoomedShadowSize = (int)Math.Round(shadowSize * zoomfactor);
						// This feature is currently deactivated because it can be confusing for the
						// user when the shadow shines through the diagram
						//if (Diagram.BackgroundColor.A < 255 || Diagram.BackgroundGradientColor.A < 255)
						//    currentGraphics.FillRectangle(diagramShadowBrush, diagramBounds.X + zoomedShadowSize, diagramBounds.Y + zoomedShadowSize, diagramBounds.Width, diagramBounds.Height);
						//else 
						{
							currentGraphics.FillRectangle(diagramShadowBrush,
								ctrlDiagramBounds.Right, ctrlDiagramBounds.Y + zoomedShadowSize,
								zoomedShadowSize, ctrlDiagramBounds.Height);
							currentGraphics.FillRectangle(diagramShadowBrush,
								ctrlDiagramBounds.X + zoomedShadowSize, ctrlDiagramBounds.Bottom,
								ctrlDiagramBounds.Width - zoomedShadowSize, zoomedShadowSize);
						}
					}
				}
			}
		}


		/// <summary>
		/// Draw the diagram, selection indicators and other stuff
		/// </summary>
		private void DrawDiagram(PaintEventArgs e)
		{
			Rectangle clipRectangle = e.ClipRectangle;
			Color backgroundColor = Color.FromKnownColor(KnownColor.Window);

			Debug.Assert(Diagram != null);

			// Clipping transformation
			// transform clipping area from control to Diagram coordinates
			clipRectangle.Inflate(invalidateDelta + 1, invalidateDelta + 1);
			ControlToDiagram(clipRectangle, out clipRectBuffer);

			Rectangle diagramBounds = Rectangle.Empty;
			DiagramToControl(DiagramBounds, out diagramBounds);

			if(drawDiagramSheet)
			{
				// Draw Background
				DoRestoreTransformation();
				if(base.MapProvider == GMap.NET.MapProviders.EmptyProvider.Instance)
				{
					e.Graphics.Clear(backgroundColor);
					Diagram.DrawBackground(currentGraphics, clipRectBuffer);
				}
				else
				{
				//	Rectangle[] rectangles = new Rectangle[]
				//	{	new Rectangle(clipRectangle.X, clipRectangle.Y, clipRectangle.Width, diagramBounds.Y - clipRectangle.Y + 1)		// Top
				//	,	new Rectangle(clipRectangle.X, diagramBounds.Y + diagramBounds.Height, clipRectangle.Width, clipRectangle.Height - (diagramBounds.Y + diagramBounds.Height) + 1)	// Bottom
				//	,	new Rectangle(clipRectangle.X, clipRectangle.Y, diagramBounds.X - clipRectangle.X + 1, clipRectangle.Height)	// Left
				//	,	new Rectangle(diagramBounds.X + diagramBounds.Width, clipRectangle.Y, clipRectangle.Width - (diagramBounds.X + diagramBounds.Width) + 1, clipRectangle.Height)		// Right
				//	};
					base.OnPaint(e);
				//	using(SolidBrush solidBrush = new SolidBrush(backColor))
				//		e.Graphics.FillRectangles(solidBrush, rectangles);
				}
				DoResetTransformation();
				// Draw grid
				if(gridVisible)
					DrawGrid(ref clipRectangle, ref diagramBounds);
				// Draw diagram border
				currentGraphics.DrawRectangle(Pens.Black, diagramBounds);
			}

			// Draw shapes and their outlines (selection indicator)
			DoRestoreTransformation();
			// Draw Shapes
			LayerIds visibleLayers = GetVisibleLayers();
			Diagram.DrawShapes(currentGraphics, visibleLayers, diagramBounds);//clipRectBuffer);
			// Draw selection indicator(s)
			foreach(Shape shape in selectedShapes.BottomUp)
			{
				if(shape.DisplayService != this)
				{
					Debug.Fail("Invalid display service");
					continue;
				}
				if(shape.Layers != LayerIds.None && (shape.Layers & visibleLayers) == 0) continue;
				Rectangle shapeBounds = shape.GetBoundingRectangle(false);
				if(Geometry.RectangleIntersectsWithRectangle(shapeBounds, clipRectBuffer))
				{
					// ToDo:  (not sure if we want these features)
					// * Should DrawShapeOutline(...) draw LinearShapes with LineCaps? (It doesn't at the moment)
					// * If the selected shape implements ILinearShape, try to get its LineCaps
					// * Find a way how to obtain the CustomLineCaps from the ToolCache without knowing if a line has CapStyles properties...

					// Draw Shape's Outline
					DoDrawShapeOutline(IndicatorDrawMode.Normal, shape);
				} //else Debug.Print("{0} does not intersect with clipping rectangle {1}", shape.Type.Name, clipRectBuffer);
			}

			// Determine the type of handles to draw
			ControlPointCapabilities capabilities = ControlPointCapabilities.None;
			if(selectedShapes.Count == 1) capabilities = ControlPointCapabilities.All; //ControlPointCapabilities.Rotate | ControlPointCapabilities.Glue | ControlPointCapabilities.Resize | ControlPointCapabilities.Movable | ControlPointCapabilities.Connect;
			else if(selectedShapes.Count > 1)
			{
				// Determine the common control points
				capabilities = ControlPointCapabilities.None;
				if(selectedShapes.TopMost is IPlanarShape)
				{
					capabilities = ControlPointCapabilities.Rotate | ControlPointCapabilities.Resize;
					// If all selected shapes are of the same type, highlight all handles.
					// Otherwise only the rotate handle
					ShapeType shapeType = selectedShapes.TopMost.Type;
					foreach(Shape s in selectedShapes)
					{
						// Clear all capabilities and break is a linear shape was found
						if(s is ILinearShape)
						{
							capabilities = ControlPointCapabilities.None;
							break;
						}
						// Remove the resize capability if shape type does not match
						if(s.Type != shapeType && (capabilities & ControlPointCapabilities.Resize) != 0)
							capabilities ^= ControlPointCapabilities.Resize;
					}
				}
			}

			// Now draw Handles, caption bounds, etc
			if(selectedShapes.Count > 0)
			{
				DoResetTransformation();
				foreach(Shape shape in SelectedShapes.BottomUp)
				{
					if(shape.DisplayService != this)
					{
						Debug.Fail("Invalid display service!");
						continue;
					}
					if(shape.Layers != LayerIds.None && (shape.Layers & visibleLayers) == 0) continue;
					Rectangle shapeBounds = shape.GetBoundingRectangle(false);
					if(Geometry.RectangleIntersectsWithRectangle(shapeBounds, clipRectBuffer))
					{
						// Draw ControlPoints
						DoDrawControlPoints(shape, capabilities);
						// Draw CaptionTextBounds / InPlaceTextBox
						if(inplaceTextbox != null)
						{
							currentGraphics.FillRectangle(InplaceTextboxBackBrush, inplaceTextbox.Bounds);
							currentGraphics.DrawRectangle(HandleInactivePen, inplaceTextbox.Bounds);
						}
						if(shape is ICaptionedShape)
							DoDrawCaptionBounds(IndicatorDrawMode.Deactivated, (ICaptionedShape)shape);
					} //else Debug.Print("{0} does not intersect with clipping rectangle {1}", shape.Type.Name, clipRectBuffer);

				}
				// If the selected shape is a point-to-shape connected shape, highlight the partner shape's outline
				foreach(KeyValuePair<Shape, ShapeConnectionInfo> connectedShapePair in shapesConnectedToSelectedShapes)
				{
					Rectangle r = connectedShapePair.Key.GetBoundingRectangle(false);
					if(Geometry.RectangleIntersectsWithRectangle(r, clipRectBuffer))
						((IDiagramPresenter)this).DrawConnectionPoint(IndicatorDrawMode.Normal, connectedShapePair.Value.OtherShape, connectedShapePair.Value.OtherPointId);
				}
				DoRestoreTransformation();
			}

			if(base.MapProvider != GMap.NET.MapProviders.EmptyProvider.Instance)
			{
				Rectangle[] rectangles = new Rectangle[]
				{	new Rectangle(clipRectangle.X, clipRectangle.Y, clipRectangle.Width, diagramBounds.Y - clipRectangle.Y)		// Top
				,	new Rectangle(clipRectangle.X, diagramBounds.Y + diagramBounds.Height, clipRectangle.Width, clipRectangle.Height - (diagramBounds.Y + diagramBounds.Height))	// Bottom
				,	new Rectangle(clipRectangle.X, clipRectangle.Y, diagramBounds.X - clipRectangle.X, clipRectangle.Height)	// Left
				,	new Rectangle(diagramBounds.X + diagramBounds.Width, clipRectangle.Y, clipRectangle.Width - (diagramBounds.X + diagramBounds.Width), clipRectangle.Height)		// Right
				};
				using(SolidBrush solidBrush = new SolidBrush(backgroundColor))
					e.Graphics.FillRectangles(solidBrush, rectangles);
			}
			foreach(Shape shape in Diagram.Shapes)
				if(shape.IsToolbarHidden)
					shape.Draw(e.Graphics);

			// Draw tool preview
			if(ActiveTool != null)
			{
				try
				{
					ActiveTool.Draw(this);
				}
				catch(Exception exc)
				{
					Debug.Print(exc.Message);
					ActiveTool.Cancel();
				}
			}

			// Draw debug infos
#if DEBUG_UI
			#region Fill occupied cells and draw cell borders

			if(isCellOccupationVisible)
			{
				Rectangle bounds = Geometry.UniteRectangles(0, 0, diagramController.Diagram.Width, diagramController.Diagram.Height,
					(diagramController.Diagram.Shapes.Count > 0) ? diagramController.Diagram.Shapes.GetBoundingRectangle(false) : Rectangle.Empty);

				int startX = ((bounds.X >= 0) ? bounds.X : (bounds.X / Diagram.CellSize) - 1) * Diagram.CellSize;
				int startY = ((bounds.Y >= 0) ? bounds.Y : (bounds.Y / Diagram.CellSize) - 1) * Diagram.CellSize;
				int endX = (bounds.Width + startX) - (bounds.Width % Diagram.CellSize) + ((bounds.X >= 0) ? Diagram.CellSize : 2 * Diagram.CellSize);
				int endY = (bounds.Height + startY) - (bounds.Height % Diagram.CellSize) + ((bounds.Y >= 0) ? Diagram.CellSize : 2 * Diagram.CellSize);

				((ShapeCollection)diagramController.Diagram.Shapes).DrawOccupiedCells(currentGraphics, startX, startY, endX, endY);
				// Draw cell borders
				for(int iX = startX; iX <= endX; iX += Diagram.CellSize)
					currentGraphics.DrawLine(Pens.Green, iX, startY, iX, endY);
				for(int iY = startY; iY <= endY; iY += Diagram.CellSize)
					currentGraphics.DrawLine(Pens.Green, startX, iY, endX, iY);
			}

			#endregion

			#region Visualize invalidated Rectangles
			if(isInvalidateAreasVisible)
			{
				clipRectBrush = (clipRectBrush == clipRectBrush1) ? clipRectBrush2 : clipRectBrush1;
				invalidatedAreaPen = (invalidatedAreaPen == invalidatedAreaPen1) ? invalidatedAreaPen2 : invalidatedAreaPen1;

				if(clipRectBuffer.Width < Bounds.Width && clipRectBuffer.Height < Bounds.Height)
				{
					Debug.Print("ClipRectangle = {0}, ClipRectBuffer = {1}", clipRectangle, clipRectBuffer);
					currentGraphics.FillRectangle(clipRectBrush, clipRectBuffer);
				}

				if(invalidatedAreas != null)
				{
					foreach(Rectangle area in invalidatedAreas)
						currentGraphics.DrawRectangle(invalidatedAreaPen, area);
					invalidatedAreas.Clear();
				}
			}
			#endregion

			#region Visualize AutoScroll Bounds
			//Rectangle autoScrollArea = DrawBounds;
			//autoScrollArea.Inflate(-autoScrollMargin, -autoScrollMargin);
			//ControlToDiagram(autoScrollArea, out autoScrollArea);
			//currentGraphics.DrawRectangle(Pens.Blue, autoScrollArea);
			//currentGraphics.DrawString(string.Format("AutoScroll Area: {0}", autoScrollArea), Font, Brushes.Blue, autoScrollArea.Location);

			//Point p = PointToClient(Control.MousePosition);
			//if (AutoScrollAreaContainsPoint(p.X, p.Y)) {
			//   ControlToDiagram(p, out p);
			//   GdiHelpers.DrawPoint(currentGraphics, Pens.Red, p.X, p.Y, 3);
			//}
			#endregion

			// Count invalidation
			//++paintCounter;
#endif
			DoResetTransformation();
		}


		private void DrawGrid(ref Rectangle clipRectangle, ref Rectangle diagramBounds)
		{
			// We have to use floats for calculation or otherwise the rounding error sums up and 
			// causes the grid to wander around when zooming...
			float zoomedGridSpace = gridSpace * zoomfactor;

			// Reset smoothing mode to "None" for drawing the grid lines - there's no need for smoothing 
			// horizontal / vertical 1-pixel lines anyway...
			// This improves performance dramatically: ~3 ms vs ~9 ms for drawing an empty diagram sheet.
			SmoothingMode origSmoothingMode = currentGraphics.SmoothingMode;
			currentGraphics.SmoothingMode = SmoothingMode.None;

			// Adjust grid spacing when zooming
			float magnificationFactor = ((10 * gridSpace) / (int)Math.Ceiling(zoomedGridSpace * 10));
			if(magnificationFactor >= 2)
			{
				zoomedGridSpace = (gridSpace * magnificationFactor) * zoomfactor;
			}
			else if(zoomedGridSpace > gridSpace * 2)
				zoomedGridSpace = (gridSpace / 2f) * zoomfactor;

			if(zoomedGridSpace > 1)
			{
				// Calculate grid start/end positions for the given diagram rectangle
				float startX = diagramBounds.Left;//! - (DiagramBounds.X * zoomfactor);
				float startY = diagramBounds.Top;//! - (DiagramBounds.Y * zoomfactor);
				float endX = diagramBounds.Right;
				float endY = diagramBounds.Bottom;

				// Line grid
				// (Use integer based drawing functions because they are faster)
				// Draw vertical lines
				int top = Math.Max(clipRectangle.Top, diagramBounds.Top);//! - (int)Math.Round(DiagramBounds.Y * zoomfactor));
				int bottom = Math.Min(clipRectangle.Bottom, diagramBounds.Bottom);
				int clipLeft = clipRectangle.Left; int clipRight = clipRectangle.Right;
				for(float i = startX; i <= endX; i += zoomedGridSpace)
				{
					if(i < clipLeft || i > clipRight) continue;
					int p = (int)Math.Round(i);
					currentGraphics.DrawLine(GridPen, p, top, p, bottom);
				}
				// Draw horizontal lines
				int left = Math.Max(clipRectangle.Left, diagramBounds.Left);//! - (int)Math.Round(DiagramBounds.X * zoomfactor));
				int right = Math.Min(clipRectangle.Right, diagramBounds.Right);
				int clipTop = clipRectangle.Top; int clipBottom = clipRectangle.Bottom;
				for(float i = startY; i <= endY; i += zoomedGridSpace)
				{
					if(i < clipTop || i > clipBottom) continue;
					int p = (int)Math.Round(i);
					currentGraphics.DrawLine(GridPen, left, p, right, p);
				}

				// Cross grid (very slow!)
				// ToDo: Use a pen with a dash pattern
				//for (int x = startX; x <= endX; x += zoomedGridSpace) {
				//   for (int y = startY; y <= endY; mouseY += zoomedGridSpace) {
				//      graphics.DrawLine(gridPen, x - 1, y, x + 1, y);
				//      graphics.DrawLine(gridPen, x, y - 1, x, y + 1);
				//   }
				//}


				// Point grid
				//Rectangle r = Rectangle.Empty;
				//r.X = startX;
				//r.Y = startY;
				//r.Width = endX;
				//r.Height = endY;
				//Size s = Size.Empty;
				//s.Width = zoomedGridSpace;
				//s.Height = zoomedGridSpace;
				//ControlPaint.DrawGrid(graphics, r, s, Color.Transparent);
			}

			// Restore smoothing mode
			currentGraphics.SmoothingMode = origSmoothingMode;
		}

		#endregion


		#region [Private] Methods: Creating drawing resources

		private void CalcControlPointShape(GraphicsPath path, ControlPointShape pointShape, int halfSize)
		{
			path.Reset();
			switch(pointShape)
			{
				case ControlPointShape.Circle:
					path.StartFigure();
					path.AddEllipse(-halfSize, -halfSize, halfSize + halfSize, halfSize + halfSize);
					path.CloseFigure();
					path.FillMode = System.Drawing.Drawing2D.FillMode.Alternate;
					break;
				case ControlPointShape.Diamond:
					path.StartFigure();
					path.AddLine(0, -halfSize, halfSize, 0);
					path.AddLine(halfSize, 0, 0, halfSize);
					path.AddLine(0, halfSize, -halfSize, 0);
					path.AddLine(-halfSize, 0, 0, -halfSize);
					path.CloseFigure();
					path.FillMode = System.Drawing.Drawing2D.FillMode.Alternate;
					break;
				case ControlPointShape.Hexagon:
					float sixthSize = (halfSize + halfSize) / 6f;
					path.StartFigure();
					path.AddLine(-sixthSize, -halfSize, sixthSize, -halfSize);
					path.AddLine(sixthSize, -halfSize, halfSize, 0);
					path.AddLine(halfSize, 0, sixthSize, halfSize);
					path.AddLine(sixthSize, halfSize, -sixthSize, halfSize);
					path.AddLine(-sixthSize, halfSize, -halfSize, 0);
					path.AddLine(-halfSize, 0, -sixthSize, -halfSize);
					path.CloseFigure();
					path.FillMode = System.Drawing.Drawing2D.FillMode.Alternate;
					break;
				case ControlPointShape.RotateArrow:
					PointF p = Geometry.IntersectCircleWithLine(0f, 0f, halfSize, 0, 0, -halfSize, -halfSize, true);
					Debug.Assert(Geometry.IsValid(p));
					float quaterSize = halfSize / 2f;
					rectBuffer.X = rectBuffer.Y = -halfSize;
					rectBuffer.Width = rectBuffer.Height = halfSize + halfSize;

					path.StartFigure();
					// arrow line
					path.AddArc(rectBuffer, -90, 315);
					path.AddLine(p.X, p.Y, -halfSize, -halfSize);
					path.AddLine(-halfSize, -halfSize, 0, -halfSize);
					path.CloseFigure();

					// closed arrow tip
					//path.StartFigure();
					//path.AddLine(0, -halfSize, 0, 0);
					//path.AddLine(0, 0, p.Value.X + 1, p.Value.Y + 1);
					//path.AddLine(p.Value.X + 1, p.Value.Y + 1, 0, 0);
					//path.AddLine(0, 0, 0, -halfSize);
					//path.CloseFigure();

					// open arrow tip
					path.StartFigure();
					path.AddLine(0, -halfSize, 0, 0);
					path.AddLine(0, 0, -quaterSize, -quaterSize);
					path.AddLine(-quaterSize, -quaterSize, 0, 0);
					path.AddLine(0, 0, 0, -halfSize);
					path.CloseFigure();

					path.CloseAllFigures();
					path.FillMode = System.Drawing.Drawing2D.FillMode.Winding;
					break;
				case ControlPointShape.Square:
					rectBuffer.X = rectBuffer.Y = -halfSize;
					rectBuffer.Width = rectBuffer.Height = halfSize + halfSize;
					path.StartFigure();
					path.AddRectangle(rectBuffer);
					path.CloseFigure();
					path.FillMode = System.Drawing.Drawing2D.FillMode.Alternate;
					break;
				default: throw new NShapeUnsupportedValueException(typeof(ControlPointShape), pointShape);
			}
		}


		private void CreatePen(Color color, ref Pen pen)
		{
			CreatePen(color, 255, 1, null, true, ref pen);
		}


		private void CreatePen(Color color, byte alpha, ref Pen pen)
		{
			CreatePen(color, alpha, 1, null, true, ref pen);
		}


		private void CreatePen(Color color, float lineWidth, ref Pen pen)
		{
			CreatePen(color, 255, lineWidth, null, true, ref pen);
		}


		private void CreatePen(Color color, byte alpha, float lineWidth, ref Pen pen)
		{
			CreatePen(color, 255, lineWidth, null, true, ref pen);
		}


		private void CreatePen(Color color, byte alpha, float lineWidth, float[] dashPattern, bool highQuality, ref Pen pen)
		{
			DisposeObject(ref pen);
			pen = new Pen(alpha != 255 ? Color.FromArgb(alpha, color) : color, lineWidth);
			if(dashPattern != null)
			{
				pen.DashPattern = dashPattern;
				pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
			}
			else
				pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
			pen.LineJoin = highQuality ? LineJoin.Round : LineJoin.Miter;
			pen.StartCap = highQuality ? LineCap.Round : LineCap.Flat;
			pen.EndCap = highQuality ? LineCap.Round : LineCap.Flat;
		}


		private void CreateBrush(Color color, ref Brush brush)
		{
			CreateBrush(color, 255, ref brush);
		}


		private void CreateBrush(Color color, byte alpha, ref Brush brush)
		{
			DisposeObject(ref brush);
			brush = new SolidBrush(Color.FromArgb(alpha, color));
		}


		private void CreateBrush(Color gradientStartColor, Color gradientEndColor, Rectangle brushBounds, float gradientAngle, ref Brush brush)
		{
			DisposeObject(ref brush);
			brush = new LinearGradientBrush(brushBounds, gradientStartColor, gradientEndColor, gradientAngle, true);
		}

		#endregion


		#region [Private] Methods: Caption editor implementation

		private void DoOpenCaptionEditor(ICaptionedShape shape, int x, int y)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			inplaceShape = shape;
			inplaceCaptionIndex = shape.FindCaptionFromPoint(x, y);
			if(inplaceCaptionIndex >= 0)
				DoOpenCaptionEditor(shape, inplaceCaptionIndex, string.Empty);
		}


		private void DoOpenCaptionEditor(ICaptionedShape captionedShape, int captionIndex, string newText)
		{
			if(captionedShape == null) throw new ArgumentNullException("captionedShape");
			if(captionIndex < 0) throw new ArgumentOutOfRangeException("labelIndex");
			inplaceShape = captionedShape;
			inplaceCaptionIndex = captionIndex;

			// Store (and hide) caption's current text
			string currentText = captionedShape.GetCaptionText(inplaceCaptionIndex);
			captionedShape.HideCaptionText(inplaceCaptionIndex);

			// Create and show inplace text editor
			inplaceTextbox = new InPlaceTextBox(this, inplaceShape, inplaceCaptionIndex, currentText, newText);
			inplaceTextbox.KeyDown += inPlaceTextBox_KeyDown;
			inplaceTextbox.Leave += inPlaceTextBox_Leave;
			inplaceTextbox.ShortcutsEnabled = true;

			// Show caption editor
			this.Controls.Add(inplaceTextbox);
			inplaceTextbox.Focus();
			inplaceTextbox.Invalidate();
		}


		/// <summary>
		/// Closes the caption editor.
		/// </summary>
		private void DoCloseCaptionEditor(bool saveChanges)
		{
			if(inplaceTextbox != null)
			{
				// Hide text editor
				inplaceTextbox.Hide();

				// End editing
				if(saveChanges)
				{
					if(inplaceTextbox.Text != inplaceTextbox.OriginalText)
					{
						ICommand cmd = new SetCaptionTextCommand(inplaceShape, inplaceCaptionIndex, inplaceTextbox.OriginalText, inplaceTextbox.Text);
						Project.ExecuteCommand(cmd);
					}
					else
						inplaceShape.SetCaptionText(inplaceCaptionIndex, inplaceTextbox.Text);
				}
				else
					inplaceShape.SetCaptionText(inplaceCaptionIndex, inplaceTextbox.OriginalText);
				// Show hidden caption text
				inplaceShape.ShowCaptionText(inplaceCaptionIndex);

				// Clean up
				inplaceTextbox.KeyDown -= inPlaceTextBox_KeyDown;
				inplaceTextbox.Leave -= inPlaceTextBox_Leave;
				DisposeObject(ref inplaceTextbox);
				inplaceShape = null;
				inplaceCaptionIndex = -1;
			}
		}

		#endregion


		#region [Private] Methods: Shape selection implementation

		/// <summary>
		/// Invalidate all selected Shapes and their ControlPoints before clearing the selection
		/// </summary>
		private void ClearSelection()
		{
			DoSuspendUpdate();
			foreach(Shape shape in selectedShapes)
				DoInvalidateShape(shape);
			selectedShapes.Clear();

			foreach(KeyValuePair<Shape, ShapeConnectionInfo> pair in shapesConnectedToSelectedShapes)
			{
				pair.Key.Invalidate();
				DoInvalidateGrips(pair.Key, ControlPointCapabilities.Connect);
			}
			shapesConnectedToSelectedShapes.Clear();
			DoResumeUpdate();
		}


		private void UnselectShapesOfInvisibleLayers()
		{
			DoSuspendUpdate();
			bool shapesRemoved = false;
			foreach(Shape selectedShape in selectedShapes.TopDown)
			{
				if(selectedShape.Layers != LayerIds.None && !IsLayerVisible(selectedShape.Layers))
				{
					DoUnselectShape(selectedShape);
					shapesRemoved = true;
				}
			}
			DoResumeUpdate();
			if(shapesRemoved) PerformSelectionNotifications();
		}


		/// <summary>
		/// Removes the shape from the list of selected shapes and invalidates the shape and its ControlPoints
		/// </summary>
		/// <param name="shape"></param>
		private void DoUnselectShape(Shape shape)
		{
			if(shape.Parent != null)
			{
				foreach(Shape s in shape.Parent.Children)
					RemoveShapeFromSelection(s);
			}
			else RemoveShapeFromSelection(shape);
		}


		private void RemoveShapeFromSelection(Shape shape)
		{
			DoSuspendUpdate();
			selectedShapes.Remove(shape);
			foreach(ShapeConnectionInfo ci in shape.GetConnectionInfos(ControlPointId.Any, null))
				shapesConnectedToSelectedShapes.Remove(ci.OtherShape);
			DoInvalidateShape(shape);
			DoResumeUpdate();
		}


		/// <summary>
		/// Checks if the shape has a parent and handles ShapeAggregation selection in case.
		/// </summary>
		/// <param name="shape">The shape that has to be selected</param>
		/// <param name="addToSelection">Specifies whether the given shape is added to the current selection or the currenty selected shapes will be unseleted.</param>
		private void DoSelectShape(Shape shape, bool addToSelection)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			Debug.Assert(Diagram != null && (Diagram.Shapes.Contains(shape) || shape.Parent != null));
			if(shape.Layers != LayerIds.None && !IsLayerVisible(shape.Layers))
				return;
			// Check if the selected shape is a child shape. 
			// Sub-selection of a CompositeShapes' children is not allowed
			if(shape.Parent != null)
			{
				if(!(shape.Parent is IShapeGroup))
					DoSelectShape(shape.Parent, addToSelection);
				else
				{
					if(!addToSelection
						&& selectedShapes.Count == 1
						&& (selectedShapes.Contains(shape.Parent) || selectedShapes.TopMost.Parent == shape.Parent))
					{
						ClearSelection();
						AddShapeToSelection(shape);
					}
					else
					{
						if(!addToSelection)
							ClearSelection();
						if(!selectedShapes.Contains(shape.Parent))
							AddShapeToSelection(shape.Parent);
					}
				}
			}
			else
			{
				// standard selection
				if(!addToSelection)
					ClearSelection();
				AddShapeToSelection(shape);
			}
		}


		private void SelectShapeAggregation(ShapeAggregation aggregation, bool addToSelection)
		{
			shapeBuffer.Clear();
			shapeBuffer.AddRange(aggregation);
			bool allSelected = true;
			int cnt = shapeBuffer.Count;
			foreach(Shape s in shapeBuffer)
			{
				if(!selectedShapes.Contains(s))
				{
					allSelected = false;
					break;
				}
			}
			DoSuspendUpdate();
			// if all Shapes of the aggregation are selected, select the shape itself
			if(allSelected)
			{
				// If the shape should be added to the selection, remove the aggregation's shapes
				// from the selection and add the selected shape
				if(addToSelection)
				{
					foreach(Shape s in aggregation)
						RemoveShapeFromSelection((Shape)s);
				}
				else
					ClearSelection();
				AddShapesToSelection(aggregation);
			}
			else
			{
				if(!addToSelection)
					ClearSelection();
				AddShapesToSelection(aggregation);
			}
			DoResumeUpdate();
		}


		/// <summary>
		/// Adds the shapes to the list of selected shapes and invalidates the shapes and their controlPoints
		/// </summary>
		private void AddShapesToSelection(IEnumerable<Shape> shapes)
		{
			foreach(Shape shape in shapes)
				AddShapeToSelection(shape);
		}


		/// <summary>
		/// Adds the shapes to the list of selected shapes and invalidates the shape and its controlPoints
		/// </summary>
		private void AddShapeToSelection(Shape shape)
		{
			// When selecting with frame, it can easily happen that shapes are inside 
			// the selection frame that are already selected... so skip them here
			if(!selectedShapes.Contains(shape))
			{
				Debug.Assert(Diagram != null && (Diagram.Shapes.Contains(shape) || shape.Parent != null));
				selectedShapes.Add(shape);
				foreach(ShapeConnectionInfo ci in shape.GetConnectionInfos(ControlPointId.Any, null))
				{
					//if (selectedShapes.Contains(ci.OtherShape)) continue;
					if(ci.OtherPointId != ControlPointId.Reference) continue;
					if(shapesConnectedToSelectedShapes.ContainsKey(ci.OtherShape))
						continue;
					shapesConnectedToSelectedShapes.Add(ci.OtherShape, ShapeConnectionInfo.Create(ci.OtherPointId, shape, ci.OwnPointId));
				}
				DoInvalidateShape(shape);
			}
		}


		/// <summary>
		/// Enables/disables all ownerDisplay context menu items that are not suitable depending on the selected shapes. 
		/// Raises the ShapesSelected event.
		/// </summary>
		private void PerformSelectionNotifications()
		{
			if(propertyController != null)
			{
				if(selectedShapes.Count > 0)
				{
					propertyController.SetObjects(1, GetModelObjects(selectedShapes));
					propertyController.SetObjects(0, selectedShapes);
				}
				else
				{
					// Usage of lastMousePos is ok here because it will only be updated in OnMouseMove
					// while selection always is a result of OnMouseDown or OnMouseUp.
					ShowDiagramProperties(lastMousePos);
				}
			}
			OnShapesSelected(EventArgs.Empty);
		}


		private IEnumerable<IModelObject> GetModelObjects(IEnumerable<Shape> shapes)
		{
			foreach(Shape shape in shapes)
			{
				if(shape.ModelObject == null) continue;
				else yield return shape.ModelObject;
			}
		}

		#endregion


		#region [Private] Methods: Converting between Pixels and mm

		private SizeF PixelsToMm(Size size)
		{
			return PixelsToMm(size.Width, size.Height);
		}


		private SizeF PixelsToMm(float width, float height)
		{
			SizeF result = SizeF.Empty;
			result.Width = width / (float)(infoGraphics.DpiX * mmToInchFactor);
			result.Height = height / (float)(infoGraphics.DpiY * mmToInchFactor);
			return result;
		}


		private float PixelsToMm(int length)
		{
			int dpi = (int)Math.Round((infoGraphics.DpiX + infoGraphics.DpiY) / 2);
			return length / (float)(infoGraphics.DpiX * mmToInchFactor);
		}


		private Size MmToPixels(SizeF size)
		{
			return MmToPixels(size.Width, size.Height);
		}


		private Size MmToPixels(float width, float height)
		{
			Size result = Size.Empty;
			result.Width = (int)Math.Round((infoGraphics.DpiX * mmToInchFactor) * width);
			result.Height = (int)Math.Round((infoGraphics.DpiY * mmToInchFactor) * height);
			return result;
		}


		private int MmToPixels(float length)
		{
			int dpi = (int)Math.Round((infoGraphics.DpiX + infoGraphics.DpiY) / 2);
			return (int)Math.Round((infoGraphics.DpiX * mmToInchFactor) * length);
		}

		#endregion


		#region [Private] Methods: Scrolling implementation

		private bool ScrollBarContainsPoint(Point p)
		{
			return ScrollBarContainsPoint(p.X, p.Y);
		}


		private bool ScrollBarContainsPoint(int x, int y)
		{
			if(HScrollBarVisible && Geometry.RectangleContainsPoint(hScrollBarPanel.Bounds, x, y))
				return true;
			if(VScrollBarVisible && Geometry.RectangleContainsPoint(scrollBarV.Bounds, x, y))
				return true;
			return false;
		}


		private void ResetBounds()
		{
			drawBounds =
			scrollAreaBounds = Geometry.InvalidRectangle;
		}


		private void ScrollBy(int deltaX, int deltaY)
		{
			if(deltaX != 0 || deltaY != 0) ScrollTo(scrollPosX + deltaX, scrollPosY + deltaY);
		}


		private void ScrollBy(ScrollOrientation orientation, int delta)
		{
			if(orientation == ScrollOrientation.HorizontalScroll)
				ScrollTo(scrollPosX + delta, scrollPosY);
			else ScrollTo(scrollPosX, scrollPosY + delta);
		}


		private void ScrollTo(ScrollOrientation orientation, int value)
		{
			if(orientation == ScrollOrientation.HorizontalScroll)
				ScrollTo(value, scrollPosY);
			else ScrollTo(scrollPosX, value);
		}


		private void ScrollTo(int x, int y)
		{
			if(HScrollBarVisible)
			{
				if(x < scrollBarH.Minimum)
					x = scrollBarH.Minimum;
				else if(x > scrollBarH.Maximum - scrollBarH.LargeChange)
					x = scrollBarH.Maximum - scrollBarH.LargeChange;
			}
			else x = 0;

			if(VScrollBarVisible)
			{
				if(y < scrollBarV.Minimum)
					y = scrollBarV.Minimum;
				else if(y > scrollBarV.Maximum - scrollBarV.LargeChange)
					y = scrollBarV.Maximum - scrollBarV.LargeChange;
			}
			else y = 0;

			if(x != scrollPosX || y != scrollPosY)
			{
				// ToDo: Scroll InPlaceTextBox with along the rest of the display content
				DoCloseCaptionEditor(true);

				SetScrollPosX(x);
				SetScrollPosY(y);

				Invalidate();
			}
		}


		private void SetScrollPosX(int newValue)
		{
			if(HScrollBarVisible)
			{
				if(newValue < scrollBarH.Minimum) newValue = scrollBarH.Minimum;
				else if(newValue > scrollBarH.Maximum) newValue = scrollBarH.Maximum;
			}
			else
			{
				if(scrollBarH.Minimum != 0) scrollBarH.Minimum = 0;
				if(scrollBarH.Maximum != ScrollAreaBounds.Width) scrollBarH.Maximum = ScrollAreaBounds.Width;
			}
			ScrollEventArgs e = new ScrollEventArgs(ScrollEventType.ThumbPosition, scrollBarH.Value, newValue, ScrollOrientation.HorizontalScroll);

			scrollBarH.Value = newValue;
			scrollPosX = newValue;

			base.OnScroll(e);
		}


		private void SetScrollPosY(int newValue)
		{
			if(VScrollBarVisible)
			{
				if(newValue < scrollBarV.Minimum) newValue = scrollBarV.Minimum;
				else if(newValue > scrollBarV.Maximum) newValue = scrollBarV.Maximum;
			}
			else
			{
				if(scrollBarV.Minimum != 0) scrollBarV.Minimum = 0;
				if(scrollBarV.Maximum != ScrollAreaBounds.Height) scrollBarV.Maximum = ScrollAreaBounds.Height;
			}
			ScrollEventArgs e = new ScrollEventArgs(ScrollEventType.ThumbPosition, scrollBarV.Value, newValue, ScrollOrientation.VerticalScroll);

			scrollBarV.Value = newValue;
			scrollPosY = newValue;

			base.OnScroll(e);
		}

		#endregion


		#region [Private] Methods: UniversalScroll implementation

		/// <summary>
		/// Returns truw if the given point (control coordinates) is inside the auto scroll area
		/// </summary>
		private bool DrawBoundsContainsPoint(int x, int y, int margin)
		{
			return (x <= DrawBounds.Left + margin
				|| y <= DrawBounds.Top + margin
				|| x > DrawBounds.Width - margin
				|| y > DrawBounds.Height - margin);
		}


		private void StartUniversalScroll(Point startPos)
		{
			universalScrollEnabled = true;
			universalScrollStartPos = startPos;
			universalScrollFixPointBounds = Rectangle.Empty;
			universalScrollFixPointBounds.Size = universalScrollCursor.Size;
			universalScrollFixPointBounds.Offset(
			universalScrollStartPos.X - (universalScrollFixPointBounds.Width / 2),
			universalScrollStartPos.Y - (universalScrollFixPointBounds.Height / 2));
			Invalidate(universalScrollFixPointBounds);
		}


		private void PerformUniversalScroll(Point currentPos)
		{
			if(universalScrollEnabled)
			{
				Cursor = GetUniversalScrollCursor(currentPos);
				if(!Geometry.RectangleContainsPoint(universalScrollFixPointBounds, currentPos))
				{
					Invalidate(universalScrollFixPointBounds);
					int slowDownFactor = 8; //(int)Math.Round((100f / autoScrollTimer.Interval) * 2);
					int minimumX = universalScrollCursor.Size.Width / 2;
					int minimumY = universalScrollCursor.Size.Height / 2;
					// Calculate distance to move 
					int deltaX = (currentPos.X - universalScrollStartPos.X);
					deltaX = (Math.Abs(deltaX) < minimumX) ? 0 : (int)Math.Round(deltaX / (zoomfactor * slowDownFactor));
					int deltaY = (currentPos.Y - universalScrollStartPos.Y);
					deltaY = (Math.Abs(deltaY) < minimumY) ? 0 : (int)Math.Round(deltaY / (zoomfactor * slowDownFactor));
					// Perform scrolling
					ScrollBy(deltaX, deltaY);
					if(!autoScrollTimer.Enabled) autoScrollTimer.Enabled = true;
				}
				else autoScrollTimer.Enabled = false;
			}
		}


		private Cursor GetUniversalScrollCursor(Point currentPos)
		{
			Cursor result;
			if(Geometry.RectangleContainsPoint(universalScrollFixPointBounds, currentPos))
				result = Cursors.NoMove2D;
			else
			{
				float angle = (360 + Geometry.RadiansToDegrees(Geometry.Angle(universalScrollStartPos, currentPos))) % 360;
				if((angle > 337.5f && angle <= 360) || (angle >= 0 && angle <= 22.5f))
					result = Cursors.PanEast;
				else if(angle > 22.5f && angle <= 67.5f)
					result = Cursors.PanSE;
				else if(angle > 67.5f && angle <= 112.5f)
					result = Cursors.PanSouth;
				else if(angle > 112.5f && angle <= 157.5f)
					result = Cursors.PanSW;
				else if(angle > 157.5f && angle <= 202.5f)
					result = Cursors.PanWest;
				else if(angle > 202.5f && angle <= 247.5f)
					result = Cursors.PanNW;
				else if(angle > 247.5f && angle <= 292.5f)
					result = Cursors.PanNorth;
				else if(angle > 292.5f && angle <= 337.5f)
					result = Cursors.PanNE;
				else result = Cursors.NoMove2D;
			}
			return result;
		}


		private void EndUniversalScroll()
		{
			Invalidate(universalScrollFixPointBounds);
			autoScrollTimer.Enabled = false;
			universalScrollEnabled = false;
			universalScrollStartPos = Geometry.InvalidPoint;
			universalScrollFixPointBounds = Geometry.InvalidRectangle;
			Cursor = Cursors.Default;
		}

		#endregion


		#region [Private] Methods

		private void DisplayDiagram()
		{
			if(Diagram != null)
			{
				Diagram.DisplayService = this;
				if(Diagram is Diagram)
				{
					Diagram.HighQualityRendering = HighQualityRendering;
					Diagram.Size = Size;
				}
			}
			UpdateScrollBars();
			Invalidate();
		}


		private void ShowDiagramProperties(Point mousePosition)
		{
			if(propertyController != null)
			{
				Point mousePos = Point.Empty;
				ControlToDiagram(mousePosition, out mousePos);
				object objectToSelect = null;
				if(Geometry.RectangleContainsPoint(DiagramBounds, mousePos))
					objectToSelect = Diagram;
				propertyController.SetObject(1, null);
				propertyController.SetObject(0, objectToSelect);
			}
		}


		private bool SelectingChangedShapes
		{
			get { return collectingChangesCounter > 0; }
		}


		/// <summary>
		/// Starts collecting shapes that were changed by executing a DiagramController action.
		/// These shapes will be selected after the action was executed.
		/// </summary>
		private void StartSelectingChangedShapes()
		{
			Debug.Assert(collectingChangesCounter >= 0);
			if(collectingChangesCounter == 0)
			{
				SuspendLayout();
				UnselectAll();
			}
			++collectingChangesCounter;
		}


		/// <summary>
		/// Ends collecting shapes that were changed by executing a DiagramController action.
		/// These shapes will be selected after the action was executed.
		/// </summary>
		private void EndSelectingChangedShapes()
		{
			Debug.Assert(collectingChangesCounter > 0);
			--collectingChangesCounter;
			if(collectingChangesCounter == 0)
			{
				ResumeLayout();
				PerformSelectionNotifications();
			}
		}


		private void ProcessClickEvent(MouseEventArgs eventArgs, bool isDoubleClickEvent)
		{
			// Check if a shape has been clicked
			if(!ScrollBarContainsPoint(eventArgs.Location) && Diagram != null)
			{
				int mouseX, mouseY;
				ControlToDiagram(eventArgs.X, eventArgs.Y, out mouseX, out mouseY);
				// If a selected shape was clicked, the event will be raised for the selected shape (even if it is behind other shapes)...
				Shape clickedShape = null;
				foreach(Shape s in selectedShapes.FindShapes(mouseX, mouseY, ControlPointCapabilities.None, 0))
				{
					clickedShape = s;
					break;
				}
				// ... otherwise, the event will be raised for the topmost shape containing the clicked coordinates
				if(clickedShape == null)
				{
					foreach(Shape s in Diagram.Shapes.FindShapes(mouseX, mouseY, ControlPointCapabilities.None, 0))
					{
						clickedShape = s;
						break;
					}
				}
				// Raise event (if a clicked shape was found)
				if(clickedShape != null)
				{
					if(isDoubleClickEvent)
						OnShapeDoubleClick(new DiagramPresenterShapeClickEventArgs(clickedShape, WinFormHelpers.GetMouseEventArgs(this, MouseEventType.MouseUp, eventArgs)));
					else OnShapeClick(new DiagramPresenterShapeClickEventArgs(clickedShape, WinFormHelpers.GetMouseEventArgs(this, MouseEventType.MouseUp, eventArgs)));
				}
			}
		}


		private void DisposeObject<T>(ref T disposableObject) where T : IDisposable
		{
			if(disposableObject != null) disposableObject.Dispose();
			disposableObject = default(T);
		}


		private Cursor GetDefaultCursor()
		{
			if(universalScrollEnabled) return Cursors.NoMove2D;
			else return Cursors.Default;
		}


		private Cursor LoadCursorFromResource(byte[] resource, int cursorId)
		{
			Cursor result = null;
			if(resource != null)
			{
				MemoryStream stream = new MemoryStream(resource, 0, resource.Length, false);
				try
				{
					result = new Cursor(stream);
					result.Tag = cursorId;
				}
				finally
				{
					stream.Close();
					stream.Dispose();
				}
			}
			return result;
		}


		private void LoadRegisteredCursor(int cursorId)
		{
			Debug.Assert(!registeredCursors.ContainsKey(cursorId));
			Cursor cursor = LoadCursorFromResource(CursorProvider.GetResource(cursorId), cursorId);
			registeredCursors.Add(cursorId, cursor ?? Cursors.Default);
		}


		private LayerIds GetVisibleLayers()
		{
			// Init result with Layers.All in order to ensure that all shapes are visible when there are no layers...
			LayerIds result = LayerIds.All;
			if(diagramController.Diagram != null && diagramController.Diagram.Layers.Count > 0)
			{
				result = LayerIds.None;
				foreach(Layer layer in diagramController.Diagram.Layers)
				{
					if((HiddenLayers & layer.Id) == 0 && layer.LowerZoomThreshold <= zoomLevel && layer.UpperZoomThreshold >= zoomLevel)
						result |= layer.Id;
				}
			}
			return result;
		}


		/// <summary>
		/// Replaces all the shapes with clones and clears their DisplayServices
		/// </summary>
		private void CreateNewClones(ShapeCollection shapes, bool withModelObjects)
		{
			CreateNewClones(shapes, withModelObjects, 0, 0);
		}


		/// <summary>
		/// Replaces all the shapes with clones and clears their DisplayServices
		/// </summary>
		private void CreateNewClones(ShapeCollection shapes, bool withModelObjects, int offsetX, int offsetY)
		{
			// clone from last to first shape in order to maintain the ZOrder
			foreach(Shape shape in shapes.BottomUp)
			{
				Shape clone = shape.Clone();
				if(withModelObjects) clone.ModelObject = shape.ModelObject.Clone();
				clone.MoveControlPointBy(ControlPointId.Reference, offsetX, offsetY, ResizeModifiers.None);
				shapes.Replace(shape, clone);
			}
		}


		private string GetFailMessage(Exception exc)
		{
			if(exc != null)
				return string.Format("{0}{1}Stack Trace:{1}{2}", exc.Message, Environment.NewLine, exc.StackTrace);
			else return string.Empty;
		}

		#endregion


		#region [Private] Methods: Creating MenuItemDefs

		private string GetShapeDisplayName(Shape shape)
		{
			if(shape == null) throw new ArgumentNullException("shape");
			return (shape.Template != null) ? string.Format("'{0}'", shape.Template.Title) : shape.Type.Name;
		}


		private MenuItemDef CreateSelectAllMenuItemDef()
		{
			bool isFeasible = selectedShapes.Count != Diagram.Shapes.Count;
			string description = isFeasible ? Properties.Resources.TooltipTxt_SelectAllShapesOfTheDiagram
				: Properties.Resources.TooltipTxt_AllShapesOfTheDiagramAreSelected;

			return new DelegateMenuItemDef(Properties.Resources.CaptionTxt_SelectAll, null, description, isFeasible, Permission.None,
					(action, project) => SelectAll());
		}


		private MenuItemDef CreateUnselectAllMenuItemDef()
		{
			bool isFeasible = selectedShapes.Count > 0;
			string description = isFeasible ? Properties.Resources.TooltipTxt_UnselectAllSelectedShapes : Properties.Resources.TooltipTxt_NoShapesSelected;

			return new DelegateMenuItemDef(Properties.Resources.CaptionTxt_UnselectAll, null, description, isFeasible, Permission.None,
				(action, project) => UnselectAll());
		}


		private MenuItemDef CreateSelectByTypeMenuItemDef(Shape shapeUnderCursor)
		{
			bool isFeasible = shapeUnderCursor != null;
			string description;
			if(isFeasible)
				description = string.Format(Properties.Resources.TooltipFmt_SelectAllShapesOfType0InTheDiagram, shapeUnderCursor.Type.Name);
			else description = Properties.Resources.TooltipTxt_NoShapeUnderTheMouseCursor;

			return new DelegateMenuItemDef(
				(shapeUnderCursor == null) ? Properties.Resources.CaptionTxt_SelectAllShapesOfAType
				: string.Format(Properties.Resources.CaptionFmt_SelectAllShapesOfType0, shapeUnderCursor.Type.Name),
				null, description, isFeasible, Permission.None,
				(action, project) => SelectShapes(shapeUnderCursor.Type, MultiSelect));
		}


		private MenuItemDef CreateSelectByTemplateMenuItemDef(Shape shapeUnderCursor)
		{
			bool isFeasible;
			string description;
			string title;
			if(shapeUnderCursor == null)
			{
				isFeasible = false;
				title = Properties.Resources.CaptionTxt_SelectAllShapesBasedOnATemplate;
				description = Properties.Resources.TooltipTxt_NoShapeUnderTheMouseCursor;
			}
			else if(shapeUnderCursor.Template == null)
			{
				isFeasible = false;
				title = Properties.Resources.CaptionTxt_SelectAllShapesBasedOnATemplate;
				description = Properties.Resources.TooltipTxt_TheShapeUnderTheCursorIsNotBasedOnAnyTemplate;
			}
			else
			{
				isFeasible = true;
				string templateTitle = string.IsNullOrEmpty(shapeUnderCursor.Template.Title) ?
					shapeUnderCursor.Template.Name : shapeUnderCursor.Template.Title;
				title = string.Format(Properties.Resources.CaptionFmt_SelectAllShapesBasedOnTemplate0, templateTitle);
				description = string.Format(Properties.Resources.TooltipFmt_SelectAllShapesOfTheDiagramBasedOnTemplate0, templateTitle);
			}

			return new DelegateMenuItemDef(title, null, description, isFeasible, Permission.None,
				(action, project) => SelectShapes(shapeUnderCursor.Template, MultiSelect));
		}


		private MenuItemDef CreateShowShapeInfoMenuItemDef(Diagram diagram, IShapeCollection shapes)
		{
			bool isFeasible = shapes.Count == 1;
			string description;
			if(isFeasible)
				description = string.Format(Properties.Resources.TooltipFmt_ShowInformationFor01, shapes.TopMost.Type.Name, shapes.TopMost.Template);
			else description = shapes.Count > 0 ? Properties.Resources.TooltipFmt_MoreThan1ShapeSelected : Properties.Resources.TooltipTxt_NoShapesSelected;

			return new DelegateMenuItemDef(Properties.Resources.CaptionTxt_ShapeInfo, Properties.Resources.Information, description,
				isFeasible, Permission.None, shapes, (a, p) =>
				{
					using(ShapeInfoDialog dlg = new ShapeInfoDialog(Project, shapes.TopMost))
						dlg.ShowDialog();
				}
			);
		}


		private MenuItemDef CreateBringToFrontMenuItemDef(Diagram diagram, IShapeCollection shapes)
		{
			bool isFeasible = diagramSetController.CanLiftShapes(diagram, shapes);
			string description;
			if(isFeasible)
			{
				if(shapes.Count == 1)
					description = string.Format(Properties.Resources.TooltipFmt_Bring0ToFront, GetShapeDisplayName(shapes.TopMost));
				else description = string.Format(Properties.Resources.TooltipFmt_Bring0ShapesToFront, shapes.Count);
			}
			else description = Properties.Resources.TooltipTxt_NoShapesSelected;

			return new DelegateMenuItemDef(Properties.Resources.CaptionTxt_BringToFront, Properties.Resources.ToForeground, description,
				isFeasible, Permission.Layout, shapes, (a, p) => diagramSetController.LiftShapes(diagram, shapes, ZOrderDestination.ToTop));
		}


		private MenuItemDef CreateSendToBackMenuItemDef(Diagram diagram, IShapeCollection shapes)
		{
			bool isFeasible = diagramSetController.CanLiftShapes(diagram, shapes);
			string description;
			if(isFeasible)
			{
				if(shapes.Count == 1)
					description = string.Format(Properties.Resources.TooltipFmt_Send0ToBackground, GetShapeDisplayName(shapes.TopMost));
				else description = string.Format(Properties.Resources.TooltipFmt_Send0ShapesToBackground, shapes.Count);
			}
			else description = Properties.Resources.TooltipTxt_NoShapesSelected;

			return new DelegateMenuItemDef(Properties.Resources.CaptionTxt_SendToBack, Properties.Resources.ToBackground,
				description, isFeasible, Permission.Layout, shapes,
				(a, p) => diagramSetController.LiftShapes(diagram, shapes, ZOrderDestination.ToBottom));
		}


		private MenuItemDef CreateAddShapesToLayersMenuItemDef(Diagram diagram, IShapeCollection shapes, LayerIds layers, bool replaceLayers)
		{
			bool isFeasible = shapes.Count > 0 && layers != LayerIds.None;
			string description;
			if(isFeasible)
			{
				// Build layer description text
				int layerCnt = 0;
				string layerNames = string.Empty;
				foreach(Layer layer in Diagram.Layers.GetLayers(layers))
				{
					++layerCnt;
					if(layerCnt <= 3) layerNames += string.Format("{0}{1}{2}{1}", (layerCnt > 1) ? ", " : "", "'", layer.Name);
				}
				if(layerCnt > 3)
					layerNames = string.Format(Properties.Resources.TooltipFmt_NumberOfLayers, layerCnt);
				else if(layerCnt > 1)
					layerNames = string.Format(Properties.Resources.TooltipFmt_LayersByName, layerNames);
				else
					layerNames = string.Format(Properties.Resources.TooltipFmt_LayerByName, layerNames);

				// Build menu item description text
				if(shapes.Count == 1)
				{
					if(replaceLayers)
						description = string.Format(Properties.Resources.TooltipFmt_AssignShape0ToLayer1, GetShapeDisplayName(shapes.TopMost), layerNames);
					else description = string.Format(Properties.Resources.TooltipFmt_AddShape0ToLayer1, GetShapeDisplayName(shapes.TopMost), layerNames);
				}
				else
				{
					if(replaceLayers)
						description = string.Format(Properties.Resources.TooltipFmt_Assign0ShapesToLayer1, shapes.Count, layerNames);
					else description = string.Format(Properties.Resources.TooltipFmt_Add0ShapesToLayer1, shapes.Count, layerNames);
				}
			}
			else
			{
				if(layers == LayerIds.None) description = Properties.Resources.TooltipTxt_NoLayersActive;
				else description = Properties.Resources.TooltipTxt_NoShapesSelected;
			}
			string label = replaceLayers ? Properties.Resources.CaptionTxt_AssignShapesToActiveLayers : Properties.Resources.CaptionTxt_AddShapesToActiveLayers;
			Bitmap image = replaceLayers ? Properties.Resources.AssignToLayer : Properties.Resources.AddToLayer;
			DelegateMenuItemDef.ActionExecuteDelegate execDelegate;
			if(replaceLayers)
				execDelegate = (a, p) => diagramSetController.AssignShapesToLayers(diagram, shapes, layers);
			else execDelegate = (a, p) => diagramSetController.AddShapesToLayers(diagram, shapes, layers);

			return new DelegateMenuItemDef(label, image, description, isFeasible, Permission.Layout, shapes, execDelegate);
		}


		private MenuItemDef CreateRemoveShapesFromLayersMenuItemDef(Diagram diagram, IShapeCollection shapes)
		{
			bool isFeasible = false;
			foreach(Shape s in shapes)
			{
				if(s.Layers != LayerIds.None)
				{
					isFeasible = true;
					break;
				}
			}
			string description;
			if(isFeasible)
			{
				if(shapes.Count == 1)
					description = string.Format(Properties.Resources.TooltipFmt_RemoveShape0FromAllLayers, GetShapeDisplayName(shapes.TopMost));
				else description = string.Format(Properties.Resources.TooltipFmt_Remove0ShapesFromAllLayers, shapes.Count);
			}
			else
			{
				if(shapes.Count <= 0) description = Properties.Resources.TooltipTxt_NoShapesSelected;
				else description = Properties.Resources.TooltipFmt_ShapesNotAssignedToLayers;
			}

			return new DelegateMenuItemDef(Properties.Resources.CaptionTxt_RemoveShapesFromAllLayers, Properties.Resources.RemoveFromAllLayers, description,
				isFeasible, Permission.Layout, shapes, (a, p) => diagramSetController.RemoveShapesFromLayers(diagram, shapes));
		}


		private MenuItemDef CreateGroupShapesMenuItemDef(Diagram diagram, IShapeCollection shapes, LayerIds activeLayers)
		{
			bool isFeasible = diagramSetController.CanGroupShapes(shapes);
			string description = isFeasible ? string.Format(Properties.Resources.TooltipFmt_Group0Shapes, shapes.Count) : Properties.Resources.TooltipTxt_NotEnoughShapesSelected;

			return new DelegateMenuItemDef(Properties.Resources.CaptionTxt_GroupShapes, Properties.Resources.GroupBtn,
				description, isFeasible, Permission.Insert, shapes,
				(a, p) => PerformGroupShapes(diagram, shapes, activeLayers));
		}


		private MenuItemDef CreateUngroupMenuItemDef(Diagram diagram, IShapeCollection shapes)
		{
			bool isFeasible = diagramSetController.CanUngroupShape(diagram, shapes);
			string description;
			if(isFeasible)
				description = string.Format(Properties.Resources.TooltipFmt_Ungroup0Shapes, shapes.TopMost.Children.Count);
			else
			{
				if(shapes.TopMost is IShapeGroup && shapes.TopMost.Parent is IShapeGroup)
					description = Properties.Resources.TooltipTxt_TheSelectedGroupIsMemberOfAnotherGroup;
				else description = Properties.Resources.TooltipTxt_NoGroupSelected;
			}

			return new DelegateMenuItemDef(Properties.Resources.CaptionTxt_UngroupShapes, Properties.Resources.UngroupBtn,
				description, isFeasible, Permission.Insert, shapes,
				(a, p) => PerformUngroupShapes(diagram, shapes.TopMost));
		}


		private MenuItemDef CreateAggregateMenuItemDef(Diagram diagram, IShapeCollection shapes, LayerIds activeLayers, Point position)
		{
			string reason;
			bool isFeasible = diagramSetController.CanAggregateShapes(diagram, shapes, out reason);
			string description = isFeasible ? string.Format(Properties.Resources.TooltipFmt_Aggregate0ShapesAsCompositeShape, shapes.Count) : reason;
			// Get host for aggregated child shapes
			Shape compositeShape = shapes.Bottom;
			if(compositeShape is ILinearShape)
			{
				isFeasible = false;
				description = Properties.Resources.TooltipTxt_LinearShapesMayNotBeTheBaseForACompositeShape;
			}
			return new DelegateMenuItemDef(Properties.Resources.CaptionTxt_AggregateShapes, Properties.Resources.AggregateShapeBtn,
				description, isFeasible, Permission.Delete, shapes,
				(a, p) => PerformAggregateCompositeShape(diagram, compositeShape, shapes, activeLayers));
		}


		private MenuItemDef CreateUnaggregateMenuItemDef(Diagram diagram, IShapeCollection shapes)
		{
			string reason = null;
			bool isFeasible = diagramSetController.CanSplitShapeAggregation(diagram, shapes, out reason);
			string description = isFeasible ? Properties.Resources.TooltipTxt_DisaggregateTheSelectedShapeAggregation : reason;
			if(!isFeasible)
			{
				if(shapes.Count <= 0)
					description = Properties.Resources.TooltipTxt_NoShapesSelected;
				else if(shapes.Count == 1)
					description = Properties.Resources.TooltipTxt_SelectedShapeIsNotACompositeShape;
				else description = Properties.Resources.TooltipTxt_TooManyShapesSelected;
			}
			return new DelegateMenuItemDef(Properties.Resources.CaptionTxt_DisaggregateShapes, Properties.Resources.SplitShapeAggregationBtn,
				description, isFeasible, Permission.Insert, shapes,
				(a, p) => PerformSplitCompositeShape(diagram, shapes.Bottom));
		}


		private MenuItemDef CreateCutMenuItemDef(Diagram diagram, IShapeCollection shapes, bool modelObjectsAssigned, Point position)
		{
			string title = Properties.Resources.CaptionTxt_Cut;
			Bitmap icon = Properties.Resources.CutBtn; ;
			Permission permission = Permission.Delete; ;
			string description;
			bool isFeasible = false;
			if(!IsActionGranted(permission, shapes))
				description = string.Format(Properties.Resources.TooltipFmt_Permission0IsNotGranted, permission);
			else if(!diagramSetController.CanCut(diagram, shapes))
				description = Properties.Resources.TooltipTxt_NoShapesSelected;
			else
			{
				isFeasible = true;
				description = (shapes.Count == 1) ? string.Format(Properties.Resources.TooltipFmt_CutShape0, GetShapeDisplayName(shapes.TopMost))
					: string.Format(Properties.Resources.TooltipFmt_Cut0Shapes, shapes.Count);
			}

			if(!modelObjectsAssigned)
				return new DelegateMenuItemDef(title, icon, description, isFeasible, permission, shapes,
					(a, p) => PerformCut(diagram, shapes, false, position));
			else
				return new GroupMenuItemDef(title, icon, description, isFeasible,
					new MenuItemDef[] {
						// Cut shapes only
						new DelegateMenuItemDef(title, icon, description, isFeasible, permission, shapes,
							(a, p) => PerformCut(diagram, shapes, false, position)),
						// Cut shapes with models
						new DelegateMenuItemDef(string.Format("{0} {1}", title, Properties.Resources.TooltipTxt_WithModelsPostfix), icon,
							string.Format("{0} {1}", description, Properties.Resources.TooltipTxt_WithModelsPostfix),
							isFeasible, permission, shapes, (a, p) => PerformCut(diagram, shapes, true, position))
					}, 1);
		}


		private MenuItemDef CreateCopyImageMenuItemDef(Diagram diagram, IShapeCollection selectedShapes)
		{
			string title = Properties.Resources.CaptionTxt_CopyAsImage;
			Bitmap icon = Properties.Resources.CopyAsImage;
			Permission permission = Permission.None;
			bool isFeasible = true;
			string description = (selectedShapes.Count > 0) ? Properties.Resources.TooltipTxt_CopySelectionAsPNGAndEMFImageToClipboard
				: Properties.Resources.TooltipTxt_CopyDiagramAsPNGAndEMFImageToClipboard;

			return new DelegateMenuItemDef(title, icon, description, isFeasible, permission,
				(a, p) =>
				{
					IEnumerable<Shape> shapes = (selectedShapes.Count > 0) ? selectedShapes : null;
					int margin = (selectedShapes.Count > 0) ? 10 : 0;
					// Clear clipboard
					Clipboard.Clear();
					// Copy diagram/selected shapes to clipboard as PNG bitmap image file
					Bitmap pngImage = (Bitmap)diagram.CreateImage(ImageFileFormat.Png, shapes, margin, (shapes == null), System.Drawing.Color.Empty);
					Clipboard.SetData(DataFormats.Bitmap, pngImage);
					// Copy diagram/selected shapes to clipboard as EMF vector graphic file
					Metafile emfImage = (Metafile)diagram.CreateImage(ImageFileFormat.EmfPlus, shapes, margin, (shapes == null), System.Drawing.Color.Empty);
					EmfHelper.PutEnhMetafileOnClipboard(Handle, emfImage, false);
				}
			);
		}


		private MenuItemDef CreateCopyMenuItemDef(Diagram diagram, IShapeCollection shapes, bool modelObjectsAssigned, Point position)
		{
			string title = Properties.Resources.CaptionTxt_Copy;
			Bitmap icon = Properties.Resources.CopyBtn;
			Permission permission = Permission.None;
			bool isFeasible = diagramSetController.CanCopy(shapes);
			string description = null;
			if(isFeasible)
			{
				description = (shapes.Count == 1) ? string.Format(Properties.Resources.TooltipFmt_CopyShape0, GetShapeDisplayName(shapes.TopMost))
					: string.Format(Properties.Resources.TooltipFmt_Copy0Shapes, shapes.Count);
			}
			else description = Properties.Resources.TooltipTxt_NoShapesSelected;

			if(!modelObjectsAssigned)
				return new DelegateMenuItemDef(title, icon, description, isFeasible, permission, shapes,
					(a, p) => PerformCopy(diagram, shapes, false, position));
			else
				return new GroupMenuItemDef(title, icon, description, isFeasible,
					new MenuItemDef[] {
						// Cut shapes only
						new DelegateMenuItemDef(title, icon, description, isFeasible, permission, shapes,
						(a, p) => PerformCopy(diagram, shapes, false, position)),
						// Cut shapes with models
						new DelegateMenuItemDef(string.Format("{0} {1}", title, Properties.Resources.TooltipTxt_WithModelsPostfix), icon,
							string.Format("{0} {1}", description, Properties.Resources.TooltipTxt_WithModelsPostfix),
							isFeasible, permission, shapes, (a, p) => PerformCopy(diagram, shapes, true, position))
					}, 1);
		}


		private MenuItemDef CreatePasteMenuItemDef(Diagram diagram, IShapeCollection shapes, LayerIds activeLayers, Point position)
		{
			//string description;
			//bool isFeasible = diagramSetController.CanPaste(diagram, out description);
			//if (isFeasible)
			//    description = string.Format("Paste {0} shape{1}", shapes.Count, shapes.Count > 1 ? "s" : "");

			bool isFeasible = diagramSetController.CanPaste(diagram);
			string description = null;
			if(isFeasible)
				description = (shapes.Count == 1) ? string.Format(Properties.Resources.TooltipFmt_PasteShape0, GetShapeDisplayName(shapes.TopMost))
					: string.Format(Properties.Resources.TooltipFmt_Paste0Shapes, shapes.Count);
			else description = Properties.Resources.TooltipTxt_NoShapesCutCopiedYet;

			return new DelegateMenuItemDef(Properties.Resources.CaptionTxt_Paste, Properties.Resources.PasteBtn, description,
				isFeasible, Permission.Insert, diagram.SecurityDomainName, (a, p) => Paste(position));
		}


		private MenuItemDef CreateDeleteMenuItemDef(Diagram diagram, IShapeCollection shapes, bool modelObjectsAssigned)
		{
			string title = Properties.Resources.CaptionTxt_Delete;
			Bitmap icon = Properties.Resources.DeleteBtn;
			Permission permission = Permission.Delete;
			string description;
			bool isFeasible = false;
			if(!IsActionGranted(permission, shapes))
				description = string.Format(Properties.Resources.TooltipFmt_Permission0IsNotGranted, permission);
			else if(!diagramSetController.CanDeleteShapes(diagram, shapes))
				description = Properties.Resources.TooltipTxt_NoShapesSelected;
			else
			{
				isFeasible = true;
				description = (shapes.Count == 1) ? string.Format(Properties.Resources.TooltipFmt_DeleteShape0, GetShapeDisplayName(shapes.TopMost))
					: string.Format(Properties.Resources.TooltipFmt_Delete0Shapes, shapes.Count);
			}

			if(!modelObjectsAssigned)
				return new DelegateMenuItemDef(title, icon, description, isFeasible, permission, shapes,
					(a, p) => PerformDelete(diagram, shapes, false));
			else
			{
				bool otherShapesAssignedToModels = OtherShapesAssignedToModels(shapes);
				string deleteWithModelsDesc;
				if(otherShapesAssignedToModels)
					deleteWithModelsDesc = Properties.Resources.TooltipTxt_ThereAreShapesAssignedToTheModelObjects;
				else deleteWithModelsDesc = string.Format("{0} {1}", description, Properties.Resources.TooltipTxt_WithModelsPostfix);

				return new GroupMenuItemDef(title, icon, description, isFeasible,
					new MenuItemDef[] {
						// Cut shapes only
						new DelegateMenuItemDef(title, icon, description, isFeasible, permission, shapes,
							(a, p) => PerformDelete(diagram, shapes, false)),
						new DelegateMenuItemDef(string.Format("{0} {1}", title, Properties.Resources.TooltipTxt_WithModelsPostfix), icon,
							deleteWithModelsDesc, !otherShapesAssignedToModels, permission, shapes, (a, p) => PerformDelete(diagram, shapes, true))
					}, 1);
			}
		}


		private MenuItemDef CreatePropertiesMenuItemDef(Diagram diagram, IShapeCollection shapes, Point position)
		{
			bool isFeasible = (Diagram != null && PropertyController != null);
			string description;
			object obj = null;
			if(!isFeasible) description = Properties.Resources.TooltipTxt_PropertiesAreNotAvailable;
			else
			{
				Shape s = shapes.FindShape(position.X, position.Y, ControlPointCapabilities.None, 0, null);
				if(s != null)
				{
					description = (shapes.Count == 1) ? string.Format(Properties.Resources.TooltipFmt_ShowPropertiesOfShape0, GetShapeDisplayName(s))
						: description = string.Format(Properties.Resources.TooltipFmt_ShowPropertiesOf0Shapes, shapes.Count);
					obj = s;
				}
				else
				{
					description = string.Format(Properties.Resources.TooltipFmt_ShowPropertiesOfDiagram0, Diagram.Title);
					obj = diagram;
				}
			}

			return new DelegateMenuItemDef(Properties.Resources.CaptionTxt_Properties, Properties.Resources.DiagramPropertiesBtn3,
				description, isFeasible, Permission.Data, shapes,
				(a, p) => PropertyController.SetObjects(0, shapes));
		}


		private MenuItemDef CreateUndoMenuItemDef()
		{
			bool isFeasible = Project.History.UndoCommandCount > 0;
			string description = isFeasible ? Project.History.GetUndoCommandDescription() : Properties.Resources.TooltipTxt_NoUndoCommandsLeft;

			return new DelegateMenuItemDef(Properties.Resources.CaptionTxt_Undo, Properties.Resources.UndoBtn, description, isFeasible,
				Permission.None, (a, p) => PerformUndo());
		}


		private MenuItemDef CreateRedoMenuItemDef()
		{
			bool isFeasible = Project.History.RedoCommandCount > 0;
			string description = isFeasible ? Project.History.GetRedoCommandDescription() : Properties.Resources.TooltipTxt_NoRedoCommandsLeft;

			return new DelegateMenuItemDef(Properties.Resources.CaptionTxt_Redo, Properties.Resources.RedoBtn, description, isFeasible,
				Permission.None, (a, p) => PerformRedo());
		}


		private bool IsActionGranted(Permission permission, IEnumerable<Shape> shapes)
		{
			if(shapes == null) throw new ArgumentNullException("shapes");
			return Project.SecurityManager.IsGranted(permission, shapes);
		}

		#endregion


		#region [Private] Methods: MenuItemDef implementations

		private void PerformGroupShapes(Diagram diagram, IShapeCollection shapes, LayerIds activeLayers)
		{
			Debug.Assert(shapes.Count > 0);

			// Buffer the currently selected shapes because the collection will be emptied by calling StartSelectingChangedShapes
			shapeBuffer.Clear();
			shapeBuffer.AddRange(shapes);

			diagramSetController.GroupShapes(diagram, shapeBuffer, activeLayers);

			OnShapesRemoved(GetShapesEventArgs(shapes));
			OnShapesInserted(GetShapesEventArgs(shapeBuffer[0].Parent));
			SelectShape(shapeBuffer[0].Parent, false);
		}


		private void PerformUngroupShapes(Diagram diagram, Shape shape)
		{
			Debug.Assert(shape.Children.Count > 0);
			shapeBuffer.Clear();
			shapeBuffer.AddRange(shape.Children);

			diagramSetController.UngroupShapes(diagram, shape);

			OnShapesRemoved(GetShapesEventArgs(shape));
			OnShapesInserted(GetShapesEventArgs(shapeBuffer));
			SelectShapes(shapeBuffer, false);
		}


		private void PerformAggregateCompositeShape(Diagram diagram, Shape compositeShape, IShapeCollection shapes, LayerIds activeLayers)
		{
			// Buffer the currently selected shapes because the collection will be emptied by calling StartSelectingChangedShapes
			shapeBuffer.Clear();
			shapeBuffer.AddRange(shapes);

			diagramSetController.AggregateCompositeShape(diagram, compositeShape, shapeBuffer, activeLayers);

			OnShapesRemoved(GetShapesEventArgs(compositeShape.Children));
			SelectShape(compositeShape, false);
		}


		private void PerformSplitCompositeShape(Diagram diagram, Shape shape)
		{
			shapeBuffer.Clear();
			shapeBuffer.AddRange(shape.Children);

			// Set DiagramPresenter in "Listen for repository changes" mode
			diagramSetController.SplitCompositeShape(diagram, shape);

			OnShapesInserted(GetShapesEventArgs(shapeBuffer));
			SelectShape(shape);
			SelectShapes(shapeBuffer, true);
		}


		private void PerformCut(Diagram diagram, IEnumerable<Shape> shapes, bool withModelObjects, Point position)
		{
			DiagramPresenterShapesEventArgs e = GetShapesEventArgs(shapes);
			UnselectShapes(shapes);

			if(Geometry.IsValid(position))
				diagramSetController.Cut(diagram, e.Shapes, withModelObjects, position);
			else diagramSetController.Cut(diagram, e.Shapes, withModelObjects);

			OnShapesRemoved(e);
		}


		private void PerformCopy(Diagram diagram, IEnumerable<Shape> shapes, bool withModelObjects, Point position)
		{
			if(Geometry.IsValid(position))
				diagramSetController.Copy(diagram, shapes, withModelObjects, position);
			else diagramSetController.Copy(diagram, shapes, withModelObjects);
		}


		private void PerformPaste(Diagram diagram, LayerIds layerIds, Point position)
		{
			try
			{
				StartSelectingChangedShapes();
				if(!Geometry.IsValid(position))
					diagramSetController.Paste(diagram, layerIds, GridSize, GridSize);
				else diagramSetController.Paste(diagram, layerIds, position);
			}
			finally
			{
				OnShapesInserted(GetShapesEventArgs(selectedShapes));
				EndSelectingChangedShapes();
			}
		}


		private void PerformPaste(Diagram diagram, LayerIds layerIds, int offsetX, int offsetY)
		{
			try
			{
				StartSelectingChangedShapes();
				if(!Geometry.IsValid(offsetX, offsetY))
					diagramSetController.Paste(diagram, layerIds, GridSize, GridSize);
				else diagramSetController.Paste(diagram, layerIds, offsetX, offsetY);
			}
			finally
			{
				OnShapesInserted(GetShapesEventArgs(selectedShapes));
				EndSelectingChangedShapes();
			}
		}


		private void PerformDelete(Diagram diagram, IEnumerable<Shape> shapes, bool withModelObjects)
		{
			DiagramPresenterShapesEventArgs e = GetShapesEventArgs(shapes);

			UnselectShapes(e.Shapes);
			diagramSetController.DeleteShapes(diagram, e.Shapes, withModelObjects);

			OnShapesRemoved(e);
		}


		private void PerformUndo()
		{
			// Set DiagramPresenter in "Listen for repository changes" mode
			try
			{
				StartSelectingChangedShapes();
				Project.History.Undo();
			}
			finally { EndSelectingChangedShapes(); }
		}


		private void PerformRedo()
		{
			// Set DiagramPresenter in "Listen for repository changes" mode
			try
			{
				StartSelectingChangedShapes();
				Project.History.Redo();
			}
			finally { EndSelectingChangedShapes(); }
		}


		private bool ModelObjectsAssigned(IEnumerable<Shape> shapes)
		{
			foreach(Shape s in shapes)
				if(s.ModelObject != null) return true;
			return false;
		}


		private bool OtherShapesAssignedToModels(IEnumerable<Shape> shapes)
		{
			foreach(Shape shape in shapes)
			{
				if(shape.ModelObject != null)
					if(OtherShapeAssignedToModel(shape.ModelObject, shape))
						return true;
			}
			return false;
		}


		private bool OtherShapeAssignedToModel(IModelObject modelObject, Shape shape)
		{
#if DEBUG_DIAGNOSTICS
			Debug.Assert(modelObject.ShapeCount > 0);
			if (modelObject.ShapeCount == 1) {
				foreach (Shape assignedShape in modelObject.Shapes) 
					Debug.Assert(assignedShape == shape);
			}
#endif
			return modelObject.ShapeCount > 1;
			//foreach (Shape assignedShape in modelObject.Shapes) {
			//   if (assignedShape != shape) return true;
			//   foreach (IModelObject childModelObject in Project.Repository.GetModelObjects(modelObject))
			//      if (OtherShapeAssignedToModel(childModelObject, shape)) return true;
			//}
			//return false;
		}

		#endregion


		#region [Private] Methods: (Un)Registering events

		private void RegisterProjectEvents()
		{
			if(!projectIsRegistered)
			{
				Debug.Assert(Project != null);
				Project.Opened += Project_ProjectOpen;
				Project.Closing += Project_ProjectClosing;
				Project.Closed += Project_ProjectClosed;
				projectIsRegistered = true;
				if(Project.IsOpen) RegisterRepositoryEvents();
			}
		}


		private void UnregisterProjectEvents()
		{
			if(projectIsRegistered)
			{
				Debug.Assert(Project != null);
				Project.Opened -= Project_ProjectOpen;
				Project.Closing -= Project_ProjectClosing;
				Project.Closed -= Project_ProjectClosed;
				projectIsRegistered = false;
				if(Project.Repository != null) UnregisterRepositoryEvents();
			}
		}


		private void RegisterRepositoryEvents()
		{
			if(!repositoryIsRegistered)
			{
				Debug.Assert(Project.Repository != null);
				Project.Repository.TemplateShapeReplaced += Repository_TemplateShapeReplaced;
				Project.Repository.DiagramUpdated += Repository_DiagramUpdated;
				Project.Repository.ShapesInserted += Repository_ShapesInserted;
				Project.Repository.ShapesUpdated += Repository_ShapesUpdated;
				Project.Repository.ShapesDeleted += Repository_ShapesDeleted;
				Project.Repository.ConnectionInserted += Repository_ShapeConnectionInsertedOrDeleted;
				Project.Repository.ConnectionDeleted += Repository_ShapeConnectionInsertedOrDeleted;
				// handle this when creating connected lines
				repositoryIsRegistered = true;
			}
		}


		private void UnregisterRepositoryEvents()
		{
			if(repositoryIsRegistered)
			{
				Debug.Assert(Project.Repository != null);
				Project.Repository.TemplateShapeReplaced -= Repository_TemplateShapeReplaced;
				Project.Repository.DiagramUpdated -= Repository_DiagramUpdated;
				Project.Repository.ShapesInserted -= Repository_ShapesInserted;
				Project.Repository.ShapesUpdated -= Repository_ShapesUpdated;
				Project.Repository.ShapesDeleted -= Repository_ShapesDeleted;
				Project.Repository.ConnectionInserted -= Repository_ShapeConnectionInsertedOrDeleted;
				Project.Repository.ConnectionDeleted -= Repository_ShapeConnectionInsertedOrDeleted;
				repositoryIsRegistered = false;
			}
		}


		private void RegisterDiagramEvents()
		{
			Diagram.Resized += diagram_Resized;
			Diagram.ShapeMoved += diagram_ShapeMoved;
			Diagram.ShapeResized += diagram_ShapeResized;
			Diagram.ShapeRotated += diagram_ShapeRotated;
		}


		private void UnregisterDiagramEvents()
		{
			Diagram.ShapeMoved -= diagram_ShapeMoved;
			Diagram.ShapeResized -= diagram_ShapeResized;
			Diagram.ShapeRotated -= diagram_ShapeRotated;
			Diagram.Resized -= diagram_Resized;
		}


		private void RegisterDiagramControllerEvents()
		{
			diagramController.DiagramChanged += Controller_DiagramChanged;
			diagramController.DiagramChanging += Controller_DiagramChanging;
		}


		private void UnregisterDiagramControllerEvents()
		{
			diagramController.DiagramChanged -= Controller_DiagramChanged;
			diagramController.DiagramChanging -= Controller_DiagramChanging;
		}


		private void RegisterDiagramSetControllerEvents()
		{
			diagramSetController.ProjectChanging += diagramSetController_ProjectChanging;
			diagramSetController.ProjectChanged += diagramSetController_ProjectChanged;
			diagramSetController.SelectModelObjectsRequested += diagramSetController_SelectModelObjectRequested;
			if(diagramSetController.Project != null) RegisterProjectEvents();
		}


		private void UnregisterDiagramSetControllerEvents()
		{
			if(diagramSetController.Project != null) UnregisterProjectEvents();
			diagramSetController.SelectModelObjectsRequested -= diagramSetController_SelectModelObjectRequested;
			diagramSetController.ProjectChanging -= diagramSetController_ProjectChanging;
			diagramSetController.ProjectChanged -= diagramSetController_ProjectChanged;
		}

		#endregion


		#region [Private] Methods: EventHandler implementations

		private DiagramPresenterShapesEventArgs GetShapesEventArgs(Shape shape)
		{
			shapesEventArgs.SetShapes(shape);
			return shapesEventArgs;
		}


		private DiagramPresenterShapesEventArgs GetShapesEventArgs(IEnumerable<Shape> shapes)
		{
			shapesEventArgs.SetShapes(shapes);
			return shapesEventArgs;
		}


		private DiagramPresenterShapeEventArgs GetShapeEventArgs(Shape shape)
		{
			shapeEventArgs.SetShape(shape);
			return shapeEventArgs;
		}


		private void diagramSetController_SelectModelObjectRequested(object sender, ModelObjectsEventArgs e)
		{
			if(Diagram != null)
			{
				UnselectAll();
				foreach(IModelObject modelObject in e.ModelObjects)
				{
					if(modelObject.ShapeCount == 0) continue;
					foreach(Shape s in modelObject.Shapes)
						if(Diagram.Shapes.Contains(s)) SelectShape(s, true);
				}
				int margin;
				ControlToDiagram(autoScrollMargin, out margin);
				if(selectedShapes.Count == 1)
					EnsureVisible(selectedShapes.TopMost, margin);
				else
					EnsureVisible(selectedShapes.GetBoundingRectangle(false), margin);
			}
		}


		private void diagramSetController_ProjectChanged(object sender, EventArgs e)
		{
			if(diagramSetController != null && diagramSetController.Project != null)
				RegisterProjectEvents();
		}


		private void diagramSetController_ProjectChanging(object sender, EventArgs e)
		{
			if(diagramSetController != null && diagramSetController.Project != null)
				UnregisterProjectEvents();
		}


		private void diagram_Resized(object sender, EventArgs e)
		{
			if(suspendUpdateCounter > 0)
				boundsChanged = true;
			else
			{
				// Store current display position
				int origPosX, origPosY;
				ControlToDiagram(DrawBounds.X, DrawBounds.Y, out origPosX, out origPosY);

				ResetBounds();
				UpdateScrollBars();

				int newPosX, newPosY;
				ControlToDiagram(DrawBounds.X, DrawBounds.Y, out newPosX, out newPosY);

				// Scroll to original position
				ScrollBy(-(newPosX - origPosX), -(newPosY - origPosY));
				Invalidate();
			}
		}


		private void diagram_ShapeMoved(object sender, ShapeEventArgs e)
		{
			ChangeShapePoints(e.Shape);
			if(ShapeMoved != null) ShapeMoved(this, GetShapeEventArgs(e.Shape));
		}


		private void diagram_ShapeResized(object sender, ShapeEventArgs e)
		{
			ChangeShapePoints(e.Shape);
			if(ShapeResized != null) ShapeResized(this, GetShapeEventArgs(e.Shape));
		}


		private void diagram_ShapeRotated(object sender, ShapeEventArgs e)
		{
			if(ShapeRotated != null) ShapeRotated(this, GetShapeEventArgs(e.Shape));
		}


		private void inPlaceTextBox_Leave(object sender, EventArgs e)
		{
			DoCloseCaptionEditor(true);
		}


		private void inPlaceTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Modifiers == Keys.None && e.KeyCode == Keys.Escape)
			{
				DoCloseCaptionEditor(false);
				e.Handled = true;
			}
			//else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Enter) {
			else if(e.KeyCode == Keys.F2)
			{
				DoCloseCaptionEditor(true);
				e.Handled = true;
			}
		}


		private void Project_ProjectClosing(object sender, EventArgs e)
		{
			Clear();
		}


		private void Project_ProjectClosed(object sender, EventArgs e)
		{
			UnregisterRepositoryEvents();
		}


		private void Project_ProjectOpen(object sender, EventArgs e)
		{
			RegisterRepositoryEvents();
		}


		private void Repository_DiagramUpdated(object sender, RepositoryDiagramEventArgs e)
		{
			if(Diagram != null && e.Diagram == Diagram)
			{
				UpdateScrollBars();
				Invalidate();
			}
		}


		private void Repository_ShapesUpdated(object sender, RepositoryShapesEventArgs e)
		{
			if(Diagram != null && SelectingChangedShapes)
			{
				DoSuspendUpdate();
				foreach(Shape s in e.Shapes)
				{
					if(s.Diagram == Diagram)
						SelectShape(s, true);
				}
				DoResumeUpdate();
			}
		}


		private void Repository_ShapesInserted(object sender, RepositoryShapesEventArgs e)
		{
			if(Diagram != null && SelectingChangedShapes)
			{
				DoSuspendUpdate();
				foreach(Shape s in e.Shapes)
				{
					if(s.Diagram == Diagram)
						SelectShape(s, true);
				}
				DoResumeUpdate();
			}
		}


		private void Repository_ShapesDeleted(object sender, RepositoryShapesEventArgs e)
		{
			UnselectShapes(e.Shapes);
		}


		private void Repository_ShapeConnectionInsertedOrDeleted(object sender, RepositoryShapeConnectionEventArgs e)
		{
			if(Diagram != null && Diagram.Shapes.Contains(e.ConnectorShape) && Diagram.Shapes.Contains(e.TargetShape))
			{
				if(selectedShapes.Contains(e.ConnectorShape))
				{
					e.ConnectorShape.Invalidate();
					e.TargetShape.Invalidate();
				}
			}
		}


		private void Repository_TemplateShapeReplaced(object sender, RepositoryTemplateShapeReplacedEventArgs e)
		{
			SuspendLayout();
			foreach(Shape selectedShape in selectedShapes)
			{
				if(selectedShape.Template == e.Template)
				{
					Rectangle bounds = selectedShape.GetBoundingRectangle(true);
					foreach(Shape s in Diagram.Shapes.FindShapes(bounds.X, bounds.Y, bounds.Width, bounds.Height, false))
					{
						if(s == selectedShape)
							selectedShapes.Remove(selectedShape);
						else if(s.Template == e.Template)
							selectedShapes.Replace(selectedShape, s);
					}
				}
			}
			ResumeLayout();
		}


		private void Repository_SavedChanges(object sender, EventArgs e)
		{
			// Nothing to do here
		}


		private void scrollBar_GotFocus(object sender, EventArgs e)
		{
			// If the scrollBar gets focussed, Focus the display itself.
			// This is necessary because in WinForms, the CanFocus method can 
			// neither be set nor overwritten.
			//this.Select();
		}


		private void scrollBar_KeyUp(object sender, KeyEventArgs e)
		{
			OnKeyUp(e);
		}


		private void scrollBar_KeyPress(object sender, KeyPressEventArgs e)
		{
			OnKeyPress(e);
		}


		private void scrollBar_KeyDown(object sender, KeyEventArgs e)
		{
			OnKeyDown(e);
		}


		private void ScrollBars_MouseEnter(object sender, EventArgs e)
		{
			if(Cursor != Cursors.Default) Cursor = Cursors.Default;
		}


		private void ScrollBar_ValueChanged(object sender, EventArgs e)
		{
			ScrollBar scrollBar = sender as ScrollBar;
			if(scrollBar != null)
			{
				if(!scrollBar.Visible && scrollBar.Value != 0)
				{
					//Debug.Fail("Invisible scrollbar was scrolled!");
					Debug.Print("Scrollbar {0}'s  value changed to {1}", scrollBar.Name, scrollBar.Value);
				}
			}
		}


		private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			//OnScroll(e);
			switch(e.ScrollOrientation)
			{
				case ScrollOrientation.HorizontalScroll:
					ScrollTo(e.NewValue, scrollPosY);
					break;
				case ScrollOrientation.VerticalScroll:
					ScrollTo(scrollPosX, e.NewValue);
					break;
				default:
					Debug.Fail("Unexpected ScrollOrientation value!");
					break;
			}
		}


		private void ScrollBar_VisibleChanged(object sender, EventArgs e)
		{
			ScrollBar scrollBar = sender as ScrollBar;
			if(scrollBar != null)
				Invalidate(RectangleToClient(scrollBar.RectangleToScreen(scrollBar.Bounds)));
			ResetBounds();
			//CalcDiagramPosition();
			//// ToDo: Set ScrollBar's Min and Max properties
		}


		private void autoScrollTimer_Tick(object sender, EventArgs e)
		{
			Point p = PointToClient(Control.MousePosition);
			OnMouseMove(new MouseEventArgs(Control.MouseButtons, 0, p.X, p.Y, 0));
		}


		private void ToolTip_Popup(object sender, PopupEventArgs e)
		{
			Point p = PointToClient(Control.MousePosition);
			toolTip.ToolTipTitle = string.Empty;
			if(Diagram != null)
			{
				Shape shape = Diagram.Shapes.FindShape(p.X, p.Y, ControlPointCapabilities.None, handleRadius, null);
				if(shape != null)
				{
					if(shape.ModelObject != null)
						toolTip.ToolTipTitle = shape.ModelObject.Name + " (" + shape.ModelObject.GetType() + ")";
				}
			}
		}


		private void Controller_DiagramChanging(object sender, EventArgs e)
		{
			OnDiagramChanging(e);
		}


		private void Controller_DiagramChanged(object sender, EventArgs e)
		{
			OnDiagramChanged(e);
		}


		private void displayContextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			if(showDefaultContextMenu && Project != null)
			{
				// Remove DummyItem
				if(ContextMenuStrip.Items.Contains(dummyItem))
					ContextMenuStrip.Items.Remove(dummyItem);
				// Collect all actions provided by the current tool
				if(ActiveTool != null)
					WinFormHelpers.BuildContextMenu(ContextMenuStrip, ActiveTool.GetMenuItemDefs(this), Project, hideMenuItemsIfNotGranted);
				// Collect all actions provided by the display itself
				WinFormHelpers.BuildContextMenu(ContextMenuStrip, GetMenuItemDefs(), Project, hideMenuItemsIfNotGranted);

				if(ContextMenuStrip.Items.Count == 0) e.Cancel = true;
			}
		}


		private void displayContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
		{
			if(showDefaultContextMenu)
			{
				WinFormHelpers.CleanUpContextMenu(ContextMenuStrip);
				// Add dummy item because context menus without items will not show
				if(ContextMenuStrip.Items.Count == 0)
					ContextMenuStrip.Items.Add(dummyItem);
			}
		}

		#endregion


		#region [Private] Types

		private enum EditAction { None, Copy, Cut, CopyWithModels, CutWithModels }


		private class EditBuffer
		{
			public EditBuffer()
			{
				action = EditAction.None;
				initialMousePos = null;
				pasteCount = 0;
				shapes = new ShapeCollection(5);
			}
			public void Clear()
			{
				initialMousePos = null;
				action = EditAction.None;
				pasteCount = 0;
				shapes.Clear();
			}
			public Point? initialMousePos;
			public EditAction action;
			public int pasteCount;
			public ShapeCollection shapes;
		}


		private class DisplayInvalidatedEventArgs : InvalidateEventArgs
		{

			public DisplayInvalidatedEventArgs()
				: base(Rectangle.Empty)
			{
			}


			public DisplayInvalidatedEventArgs(Rectangle invalidatedRect)
				: base(Rectangle.Empty)
			{
				r = invalidatedRect;
			}


			public new Rectangle InvalidRect
			{
				get { return r; }
				internal set { r = value; }
			}


			private Rectangle r = Rectangle.Empty;
		}


		private class DisplayShapesEventArgs : DiagramPresenterShapesEventArgs
		{

			public DisplayShapesEventArgs(Shape shape)
				: base(shape)
			{
			}


			public DisplayShapesEventArgs(IEnumerable<Shape> shapes)
				: base(shapes)
			{
			}


			internal DisplayShapesEventArgs()
				: base()
			{
			}


			internal ReadOnlyList<Shape> ShapesList
			{
				get { return (ReadOnlyList<Shape>)Shapes; }
			}


			internal void SetShapes(IEnumerable<Shape> shapes)
			{
				ShapesList.Clear();
				ShapesList.AddRange(shapes);
			}


			internal void SetShapes(Shape shape)
			{
				ShapesList.Clear();
				ShapesList.Add(shape);
			}

		}


		private class DisplayShapeEventArgs : DiagramPresenterShapeEventArgs
		{

			public DisplayShapeEventArgs(Shape shape)
				: base(shape)
			{
			}


			internal DisplayShapeEventArgs()
				: base()
			{
			}


			internal void SetShape(Shape shape)
			{
				Shape = shape;
			}

		}

		#endregion


		#region Property Default Values

		// Behavior
		private const bool DefaultValueZoomWithMouseWheel = false;
		private const bool DefaultValueShowScrollBars = true;
		private const bool DefaultValueHideMenuItemsIfNotGranted = false;
		private const bool DefaultValueShowDefaultContextMenu = true;
		private const bool DefaultValueSnapToGrid = true;
		private const int DefaultValueSnapDistance = 5;
		private const int DefaultValueMinRotateDistance = 30;

		// Appearance
		private const int DefaultValueGripSize = 3;
		private const int DefaultValueGridSize = 20;
		private const bool DefaultValueShowGrid = true;
		private const int DefaultValueZoomLevel = 100;
		private const bool DefaultValueHighQualityBackground = true;
		private const bool DefaultValueHighQualityRendering = true;
		private const ControlPointShape DefaultValueResizePointShape = ControlPointShape.Square;
		private const ControlPointShape DefaultValueConnectionPointShape = ControlPointShape.Circle;

		private const int DefaultValueBackgroundGradientAngle = 45;
		private const byte DefaultValueGridAlpha = 255;
		private const byte DefaultValueSelectionAlpha = 255;
		private const byte defaultPreviewColorAlpha = 96;
		private const byte defaultPreviewBackColorAlpha = 64;

		private const RenderingQuality defaultRenderingQuality = RenderingQuality.HighQuality;
		private const RenderingQuality DefaultValueRenderingQualityHigh = RenderingQuality.HighQuality;
		private const RenderingQuality DefaultValueRenderingQualityLow = RenderingQuality.DefaultQuality;

		private static readonly Color DefaultValueBackColor = Color.FromKnownColor(KnownColor.Control);
		private static readonly Color DefaultValueBackColorGradient = Color.FromKnownColor(KnownColor.Control);
		private static readonly Color DefaultValueGridColor = Color.Gainsboro;
		private static readonly Color DefaultColorSelectionNormal = Color.DarkGreen;
		private static readonly Color DefaultColorSelectionHilight = Color.Firebrick;
		private static readonly Color DefaultColorSelectionInactive = Color.Gray;
		private static readonly Color DefaultColorSelectionInterior = Color.WhiteSmoke;
		private static readonly Color DefaultColorToolPreview = Color.FromArgb(defaultPreviewColorAlpha, Color.SteelBlue);
		private static readonly Color defaultToolPreviewBackColor = Color.FromArgb(defaultPreviewBackColorAlpha, Color.LightSlateGray);

		#endregion


		#region Constants

		private const int shadowSize = 20;
		private const int scrollAreaMargin = 40;    // The distance between the diagram 'sheet' and the end of the scrollable area
		private const int autoScrollMargin = 40;
		private const byte inlaceTextBoxBackAlpha = 128;

		private const double mmToInchFactor = 0.039370078740157477;

		// String constants for exception messages
		private const string ErrMessageOnlyAllowedWhilePaining = "Calling this method is only allowed while painting.";
		private const string ErrMessageResetTransformHasNotBeenCalled = "ResetTransformation has to be called before calling this method.";
		private const string ErrMessageRestoreTransformationHasNotBeenCalled = "RestoreTransformation has to be called before calling this method.";

		#endregion

		#region Fields

		private static Dictionary<int, Cursor> registeredCursors = new Dictionary<int, Cursor>(10);
		private bool projectIsRegistered = false;
		private bool repositoryIsRegistered = false;

		// Contains all active layers (new shapes are assigned to these layers)
		private LayerIds activeLayers = LayerIds.None;
		// Contains all layer hidden by the user
		private LayerIds hiddenLayers = LayerIds.None;

		// Fields for mouse related display behavior (click handling, scroll and zoom)
		private int minRotateDistance = DefaultValueMinRotateDistance;
		private bool mouseEventWasHandled = false;
		private bool zoomWithMouseWheel = false;
		private ScrollEventArgs scrollEventArgsH = new ScrollEventArgs(ScrollEventType.ThumbTrack, 0);
		private ScrollEventArgs scrollEventArgsV = new ScrollEventArgs(ScrollEventType.ThumbTrack, 0);
		private Point universalScrollStartPos = Geometry.InvalidPoint;
		private Rectangle universalScrollFixPointBounds = Geometry.InvalidRectangle;
		private Cursor universalScrollCursor = Cursors.NoMove2D;
		private bool universalScrollEnabled = false;
		private Timer autoScrollTimer = new Timer();

		private bool showScrollBars = DefaultValueShowScrollBars;
		private bool hideMenuItemsIfNotGranted = DefaultValueHideMenuItemsIfNotGranted;
		private int handleRadius = DefaultValueGripSize;
		private bool snapToGrid = DefaultValueSnapToGrid;
		private int snapDistance = DefaultValueSnapDistance;
		private int gridSpace = DefaultValueGridSize;
		private bool gridVisible = DefaultValueShowGrid;
		private Size gridSize = Size.Empty;
		private bool scrollBarHVisible = true;
		private bool scrollBarVVisible = true;
#if DEBUG_UI
		private bool isInvalidateAreasVisible = false;
		private bool isCellOccupationVisible = false;
#endif
		private bool showDefaultContextMenu = DefaultValueShowDefaultContextMenu;
		private ControlPointShape resizePointShape = DefaultValueResizePointShape;
		private ControlPointShape connectionPointShape = DefaultValueConnectionPointShape;

		// Graphics and Graphics Settings
		private Graphics infoGraphics;
		private Matrix pointMatrix = new Matrix();
		private Matrix matrix = new Matrix();
		private Graphics currentGraphics;
		private Rectangle invalidationBuffer = Rectangle.Empty;
		private bool graphicsIsTransformed = false;
		private int suspendUpdateCounter = 0;
		private int collectingChangesCounter = 0;
		private Rectangle invalidatedAreaBuffer = Rectangle.Empty;
		private DisplayInvalidatedEventArgs invalidatedEventArgs = new DisplayInvalidatedEventArgs();
		private bool boundsChanged = false;
		private FillStyle hintBackgroundStyle = null;
		private LineStyle hintForegroundStyle = null;

		private bool highQualityBackground = DefaultValueHighQualityBackground;
		private bool highQualityRendering = DefaultValueHighQualityRendering;
		private RenderingQuality renderingQualityHigh = DefaultValueRenderingQualityHigh;
		private RenderingQuality renderingQualityLow = DefaultValueRenderingQualityLow;
		private RenderingQuality currentRenderingQuality = defaultRenderingQuality;

		// Colors
		private byte gridAlpha = DefaultValueGridAlpha;
		private byte toolPreviewColorAlpha = defaultPreviewColorAlpha;
		private byte toolPreviewBackColorAlpha = defaultPreviewBackColorAlpha;
		private byte selectionAlpha = DefaultValueSelectionAlpha;
		private Color backColor = DefaultValueBackColor;
		private Color gradientBackColor = DefaultValueBackColorGradient;
		private Color gridColor = DefaultValueGridColor;
		private Color selectionNormalColor = DefaultColorSelectionNormal;
		private Color selectionHilightColor = DefaultColorSelectionHilight;
		private Color selectionInactiveColor = DefaultColorSelectionInactive;
		private Color selectionInteriorColor = DefaultColorSelectionInterior;
		private Color toolPreviewColor = DefaultColorToolPreview;
		private Color toolPreviewBackColor = defaultToolPreviewBackColor;

		// Pens
		private Pen gridPen;                        // pen for drawing the grid
		private Pen outlineInteriorPen;             // pen for the interior of thick outlines
		private Pen outlineNormalPen;               // pen for drawing thick shape outlines (normal)
		private Pen outlineHilightPen;              // pen for drawing thick shape outlines (highlighted)
		private Pen outlineInactivePen;             // pen for drawing thick shape outlines (inactive)
		private Pen handleNormalPen;                // pen for drawing shape handles (normal)
		private Pen handleHilightPen;               // pen for drawing connection point indicators
		private Pen handleInactivePen;              // pen for drawing inactive handles
		private Pen toolPreviewPen;                 // Pen for drawing tool preview infos (rotation preview, selection frame, etc)
		private Pen outerSnapPen = new Pen(Color.FromArgb(196, Color.WhiteSmoke), 2);
		private Pen innerSnapPen = new Pen(Color.FromArgb(196, Color.SteelBlue), 1);

		// Brushes
		private Brush controlBrush = null;              // brush for painting the ownerDisplay control's background
		private Brush handleInteriorBrush = null;       // brush for filling shape handles
		private Brush toolPreviewBackBrush = null;  // brush for filling tool preview info (rotation preview, selection frame, etc)
		private Brush inplaceTextboxBackBrush = null; // Brush for filling the background of the inplaceTextBox.
		private Brush diagramShadowBrush = new SolidBrush(Color.FromArgb(128, Color.Gray)); // Brush for a shadow underneath the diagram

		// Other drawing stuff
		private int controlBrushGradientAngle = DefaultValueBackgroundGradientAngle;
		private StringFormat previewTextFormatter = new StringFormat();
		private double controlBrushGradientSin = Math.Sin(Geometry.DegreesToRadians(45f));
		private double controlBrushGradientCos = Math.Cos(Geometry.DegreesToRadians(45f));
		private Size controlBrushSize = Size.Empty;
		private GraphicsPath rotatePointPath = new GraphicsPath();
		private GraphicsPath connectionPointPath = new GraphicsPath();
		private GraphicsPath resizePointPath = new GraphicsPath();
		private Point[] arrowShape = new Point[3];

		private bool drawDiagramSheet = true;   // Specifies whether to draw the diagram sheet
		private int diagramPosX;        // Position of the left side of the Diagram on the control
		private int diagramPosY;        // Position of the upper side of the Diagram on the control
		private int zoomLevel = DefaultValueZoomLevel;  // Zoom level in percentage
		private float zoomfactor = 1f;// zoomFactor for transforming Diagram coordinates to control coordinates (range: >0 to x, 100% == 1)
		private int scrollPosX = 0; // horizontal position of the scrolled Diagram (== horizontal scrollbar value)
		private int scrollPosY = 0; // vertical position of the scrolled Diagram (== vertical scrollbar value)
		private int invalidateDelta;    // handle radius or selection outline lineWidth (amount of pixels the invalidated area has to be increased)

		// Components
		private PropertyController propertyController;
		private DiagramSetController diagramSetController;
		private DiagramController diagramController;
		private Tool privateTool = null;

		// -- In-Place Editing --
		// text box currently used for in-place text editing
		private InPlaceTextBox inplaceTextbox;
		// shape currently edited
		private ICaptionedShape inplaceShape;
		// index of caption within shape
		private int inplaceCaptionIndex;

		// Lists and Collections
		private ShapeCollection selectedShapes = new ShapeCollection();
		private Dictionary<Shape, ShapeConnectionInfo> shapesConnectedToSelectedShapes = new Dictionary<Shape, ShapeConnectionInfo>();
		private EditBuffer editBuffer = new EditBuffer();   // Buffer for Copy/Cut/Paste-Actions
		private Rectangle copyCutBounds = Rectangle.Empty;
		private Point copyCutMousePos = Point.Empty;
		private List<Shape> shapeBuffer = new List<Shape>();
		private List<IModelObject> modelBuffer = new List<IModelObject>();

		// Buffers
		private Rectangle rectBuffer;                   // buffer for rectangles
		private Point[] pointBuffer = new Point[4];     // point array buffer
		private Rectangle clipRectBuffer;               // buffer for clipRectangle transformation
		private Rectangle drawBounds;                   // drawing area of the display (ClientRectangle - scrollbars)
		private Rectangle scrollAreaBounds;             // Scrollable area (Diagram sheet incl. off-sheet shapes and margin)
														//private GraphicsPath selectionPath = new GraphicsPath();	// Path used for highlighting all selected selectedShapes

		// Temporary Buffer for last Mouse position (for MouseCursor sensitive context menu actions, e.g. Paste)
		private Point lastMousePos;

		DisplayShapesEventArgs shapesEventArgs = new DisplayShapesEventArgs();
		DisplayShapeEventArgs shapeEventArgs = new DisplayShapeEventArgs();

#if DEBUG_UI
		// Debugging stuff
		private Stopwatch stopWatch = new Stopwatch();
		//private long paintCounter;
		private Color invalidatedAreaColor1 = Color.FromArgb(32, Color.Red);
		private Color invalidatedAreaColor2 = Color.FromArgb(32, Color.Green);
		private Brush clipRectBrush;
		private Brush clipRectBrush1;
		private Brush clipRectBrush2;
		private Pen invalidatedAreaPen = null;
		private Pen invalidatedAreaPen1 = null;
		private Pen invalidatedAreaPen2 = null;
		private List<Rectangle> invalidatedAreas;
#endif
		#region Map
		private GMap.NET.PointLatLng lastPosition;
		private double lastZoom;
		private bool isZooming = false;
		private Dictionary<Shape, Dictionary<ControlPointId, GMap.NET.PointLatLng>> shapesPoints = new Dictionary<Shape, Dictionary<ControlPointId, GMap.NET.PointLatLng>>();
		private int staticPropsCount = 8;
		private int dynamicPropsCount = 24;
		#endregion
		#endregion

		private void ToolTipMouseMove(MouseEventArgs e)
		{
			Shape toolTipShape = null;
			string toolTipText = null;
			Shape shapeMouseHover = null;
			foreach(Shape shape in Diagram.Shapes)
				if(shape.ContainsPoint(e.X, e.Y))
				{	shapeMouseHover = ShapeMouseHover(shape, e.X, e.Y);
					break;
				}
			if(shapeMouseHover != null)
			{	toolTipShape = shapeMouseHover;
				while(string.IsNullOrEmpty(toolTipText = toolTipShape.ToolTipText) && toolTipShape.Parent != null)
					toolTipShape = toolTipShape.Parent;
		//	System.Windows.Forms.SystemInformation.MouseHoverSize;
		//	System.Windows.Forms.SystemInformation.MouseHoverTime;
			}
			bool toolTipTextChanged = this.toolTipText != toolTipText
				, toolTipShapeChanged = this.toolTipShape != toolTipShape
				;
			if(toolTipTextChanged)
				this.toolTipText = toolTipText;
			if(toolTipShapeChanged)
			{	if(this.toolTipShape != null)
					this.toolTipShape.ToolTipTextChanged -= toolTipShape_ToolTipTextChanged;
				this.toolTipShape = toolTipShape;
				if(this.toolTipShape != null)
					this.toolTipShape.ToolTipTextChanged += toolTipShape_ToolTipTextChanged;
				if(toolTipTextChanged)
					ToolTipTextChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		private void toolTipShape_ToolTipTextChanged(object sender, EventArgs e)
		{	this.toolTipText = ((Shape)sender).ToolTipText;
			ToolTipTextChanged?.Invoke(this, EventArgs.Empty);
		}

		private Shape ShapeMouseHover(Shape shape, int x, int y)
		{	if(shape.Children != null)
				foreach(Shape child in shape.Children)
					if(child.ContainsPoint(x, y))
						return ShapeMouseHover(child, x, y);
			return shape;
		}

		private Shape toolTipShape = null;
		private string toolTipText = null;
		/// <summary></summary>
		public Shape ToolTipShape { get => this.toolTipShape; }
		/// <summary></summary>
		public string ToolTipText { get => this.toolTipText; }

		/// <summary></summary>
		public event EventHandler ToolTipTextChanged;
	}


	internal class DisplayVScrollBar : VScrollBar
	{

		public DisplayVScrollBar()
			: base()
		{
		}

		public new bool CanFocus
		{
			get { return false; }
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
		}

	}


	internal class DisplayHScrollBar : HScrollBar
	{

		public DisplayHScrollBar()
			: base()
		{
		}

		public new bool CanFocus
		{
			get { return false; }
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
		}

	}

}