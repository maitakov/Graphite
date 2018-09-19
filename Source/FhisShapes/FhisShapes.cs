using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.FhisShapes
{
	public class Resource : BoxBase
	{
		internal static Shape CreateInstance(ShapeType shapeType, Template template)
		{
			return new Resource(shapeType, template);
		}

		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new Resource(Type, this.Template);
			result.CopyFrom(this);
			return result;
		}

		protected internal Resource(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}


		protected internal Resource(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
		}

		protected override void InitializeToDefault(IStyleSet styleSet)
		{
			base.InitializeToDefault(styleSet);
			FillStyle = styleSet.FillStyles.Transparent;
		}

		/// <override></override>
		protected override bool CalculatePath()
		{
			if(!base.CalculatePath())
				return false;

			Path.Reset();

			Rectangle shapeRect = Rectangle.Empty;
			shapeRect.Offset((int)Math.Round(-Width / 2f), (int)Math.Round(-Height / 2f));
			shapeRect.Width = Width;
			shapeRect.Height = Height;

			Path.StartFigure();
			Path.AddRectangle(shapeRect);
			Path.CloseFigure();
			return true;
		}
	}


	public class Property : TriangleBase
	{

		/// <ToBeCompleted></ToBeCompleted>
		public static Shape CreateInstance(ShapeType shapeType, Template template)
		{
			return new Property(shapeType, template);
		}


		public override Shape Clone()
		{
			Shape result = new Property(Type, this.Template);
			result.CopyFrom(this);
			return result;
		}


		protected Property(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}


		protected Property(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
		}

		protected override void InitializeToDefault(IStyleSet styleSet)
		{
			base.InitializeToDefault(styleSet);
			FillStyle = styleSet.FillStyles.Transparent;
		}

	}


	public class Action : RectangleBase
	{

		/// <summary>
		/// Provides constants for the control point id's of the shape.
		/// </summary>
		new public class ControlPointIds
		{
			/// <summary>ControlPointId at the tip of the arrow head.</summary>
			public const int ArrowTipControlPoint = 1;
			/// <summary>ControlPointId at the top of the arrow head.</summary>
			public const int ArrowTopControlPoint = 2;
			/// <summary>ControlPointId at the bottom of the arrow head.</summary>
			public const int ArrowBottomControlPoint = 3;
			/// <summary>ControlPointId at the end of the arrow.</summary>
			public const int BodyEndControlPoint = 4;
			/// <summary>ControlPointId of the center control point.</summary>
			public const int MiddleCenterControlPoint = 5;
		}


		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet)
		{
			base.InitializeToDefault(styleSet);
			FillStyle = styleSet.FillStyles.Transparent;
			bodyHeightRatio = 1d / 3d;
			headWidth = (int)Math.Round(Width / 2f);
		}


		/// <override></override>
		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is Action)
			{
				this.headWidth = ((Action)source).headWidth;
				this.bodyHeightRatio = ((Action)source).bodyHeightRatio;
			}
			InvalidateDrawCache();
		}


		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new Action(Type, this.Template);
			result.CopyFrom(this);
			return result;
		}


		public int BodyWidth
		{
			get { return Width - headWidth; }
		}


		[CategoryLayout()]
		[Description("The height of the arrow's body.")]
		[PropertyMappingId(PropertyIdBodyHeight)]
		[RequiredPermission(Permission.Layout)]
		public int BodyHeight
		{
			get { return (int)Math.Round(Height * bodyHeightRatio); }
			set
			{
				Invalidate();
				if (value > Height) throw new ArgumentOutOfRangeException("BodyHeight");

				if (Height == 0) bodyHeightRatio = 0;
				else bodyHeightRatio = value / (float)Height;

				InvalidateDrawCache();
				Invalidate();
			}
		}


		[CategoryLayout()]
		[Description("The width of the arrow's tip.")]
		[PropertyMappingId(PropertyIdHeadWidth)]
		[RequiredPermission(Permission.Layout)]
		public int HeadWidth
		{
			get { return headWidth; }
			set
			{
				Invalidate();
				headWidth = value;
				InvalidateDrawCache();
				Invalidate();
			}
		}


		/// <override></override>
		protected override int ControlPointCount
		{
			get { return 5; }
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability)
		{
			switch (controlPointId)
			{
				case ControlPointIds.ArrowTipControlPoint:
				case ControlPointIds.BodyEndControlPoint:
					// ToDo: Implement GluePoint behavior for Actions
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0
						//|| (controlPointCapability & ControlPointCapabilities.Glue) != 0
						|| ((controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId)));
				case ControlPointIds.ArrowTopControlPoint:
				case ControlPointIds.ArrowBottomControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0
								|| ((controlPointCapability & ControlPointCapabilities.Connect) != 0
								&& IsConnectionPointEnabled(controlPointId)));
				case ControlPointIds.MiddleCenterControlPoint:
					return ((controlPointCapability & ControlPointCapabilities.Reference) != 0
								|| (controlPointCapability & ControlPointCapabilities.Rotate) != 0
								|| ((controlPointCapability & ControlPointCapabilities.Connect) != 0
									&& IsConnectionPointEnabled(controlPointId)));
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		/// <override></override>
		public override void Fit(int x, int y, int width, int height)
		{
			float headWidthRatio = this.HeadWidth / (float)Width;
			HeadWidth = (int)Math.Round(width * headWidthRatio);
			base.Fit(x, y, width, height);
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int startX, int startY)
		{
			CalcShapePoints();
			PointF rotationCenter = PointF.Empty;
			rotationCenter.X = X;
			rotationCenter.Y = Y;
			Matrix.Reset();
			Matrix.Translate(X, Y, MatrixOrder.Prepend);
			Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), rotationCenter, MatrixOrder.Append);
			Matrix.TransformPoints(shapePoints);
			Matrix.Reset();

			Point startPoint = Point.Empty;
			startPoint.X = startX;
			startPoint.Y = startY;
			Point result = Geometry.GetNearestPoint(startPoint, Geometry.IntersectPolygonLine(shapePoints, startX, startY, X, Y, true));
			if (!Geometry.IsValid(result)) result = Center;
			return result;
		}


		#region IEntity Members

		/// <override></override>
		protected override void LoadFieldsCore(IRepositoryReader reader, int version)
		{
			base.LoadFieldsCore(reader, version);
			HeadWidth = reader.ReadInt32();
			BodyHeight = reader.ReadInt32();
		}


		/// <override></override>
		protected override void SaveFieldsCore(IRepositoryWriter writer, int version)
		{
			base.SaveFieldsCore(writer, version);
			writer.WriteInt32(HeadWidth);
			writer.WriteInt32(BodyHeight);
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.Action" />.
		/// </summary>
		new public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			foreach (EntityPropertyDefinition pi in RectangleBase.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("HeadWidth", typeof(int));
			yield return new EntityFieldDefinition("BodyHeight", typeof(int));
		}

		#endregion


		protected internal Action(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}


		protected internal Action(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds)
		{
			if (index != 0) throw new ArgumentOutOfRangeException("index");
			captionBounds = Rectangle.Empty;
			captionBounds.Width = (int)Math.Round(Width - (HeadWidth / 3f));
			captionBounds.Height = BodyHeight;
			captionBounds.X = -(int)Math.Round((Width / 2f) - (HeadWidth / 3f));
			captionBounds.Y = -(int)Math.Round(captionBounds.Height / 2f);
			if (ParagraphStyle != null)
			{
				captionBounds.X += ParagraphStyle.Padding.Left;
				captionBounds.Y += ParagraphStyle.Padding.Top;
				captionBounds.Width -= ParagraphStyle.Padding.Horizontal;
				captionBounds.Height -= ParagraphStyle.Padding.Vertical;
			}
		}


		/// <override></override>
		protected override bool CalculatePath()
		{
			if(!base.CalculatePath())
				return false;
			Path.Reset();
			CalcShapePoints();
			Path.StartFigure();
			Path.AddPolygon(shapePoints);
			Path.CloseFigure();
			return true;
		}


		/// <override></override>
		protected override bool ContainsPointCore(int x, int y)
		{
			if (base.ContainsPointCore(x, y))
			{
				CalcShapePoints();
				// Transform points
				Matrix.Reset();
				Matrix.Translate(X, Y, MatrixOrder.Prepend);
				Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), Center, MatrixOrder.Append);
				Matrix.TransformPoints(shapePoints);

				return Geometry.PolygonContainsPoint(shapePoints, x, y);
			}
			return false;
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight)
		{
			Rectangle result = Geometry.InvalidRectangle;
			if (Width >= 0 && Height >= 0)
			{
				CalcShapePoints();
				Geometry.CalcBoundingRectangle(shapePoints, 0, 0, Geometry.TenthsOfDegreeToDegrees(Angle), out result);
				if (Geometry.IsValid(result))
				{
					result.Offset(X, Y);
					ShapeUtils.InflateBoundingRectangle(ref result, LineStyle);
				}
			}
			return result;
		}


		/// <override></override>
		protected override int GetControlPointIndex(ControlPointId id)
		{
			switch (id)
			{
				case ControlPointIds.ArrowTipControlPoint:		return 0;
				case ControlPointIds.ArrowTopControlPoint:		return 1;
				case ControlPointIds.BodyEndControlPoint:		return 2;
				case ControlPointIds.ArrowBottomControlPoint:	return 3;
				case ControlPointIds.MiddleCenterControlPoint:	return 4;
				default:
					return base.GetControlPointIndex(id);
			}
		}


		/// <override></override>
		protected override void CalcControlPoints()
		{
			int left = -(int)Math.Round(Width / 2f);
			int right = left + Width;
			int top = -(int)Math.Round(Height / 2f);
			int bottom = top + Height;

			int i = 0;
			ControlPoints[i].X = right;						// 0-ArrowTipControlPoint
			ControlPoints[i].Y = 0;
			++i;
			ControlPoints[i].X = left + BodyWidth;			// 1-ArrowTopControlPoint
			ControlPoints[i].Y = top;
			++i;
			ControlPoints[i].X = left;						// 2-BodyEndControlPoint
			ControlPoints[i].Y = 0;
			++i;
			ControlPoints[i].X = left + BodyWidth;			// 3-ArrowBottomControlPoint
			ControlPoints[i].Y = bottom;
			++i;
			ControlPoints[i].X = 0;							// 4-MiddleCenterControlPoint
			ControlPoints[i].Y = 0;
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, int deltaX, int deltaY, ResizeModifiers modifiers)
		{
			if (pointId == ControlPointIds.ArrowTipControlPoint || pointId == ControlPointIds.BodyEndControlPoint)
			{
				bool result = true;
				int dx = 0, dy = 0;
				int width = Width;
				int angle = Angle;
				Point tipPt = GetControlPointPosition(ControlPointIds.ArrowTipControlPoint);
				Point endPt = GetControlPointPosition(ControlPointIds.BodyEndControlPoint);

				if (pointId == ControlPointIds.ArrowTipControlPoint)
					result = Geometry.MoveArrowPoint(Center, tipPt, endPt, angle, headWidth, 0.5f, deltaX, deltaY, modifiers, out dx, out dy, out width, out angle);
				else
					result = Geometry.MoveArrowPoint(Center, endPt, tipPt, angle, headWidth, 0.5f, deltaX, deltaY, modifiers, out dx, out dy, out width, out angle);

				RotateCore(angle - Angle, X, Y);
				MoveByCore(dx, dy);
				Width = width;
				return result;
			}
			return base.MovePointByCore(pointId, deltaX, deltaY, modifiers);
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, float transformedDeltaX, float transformedDeltaY, float sin, float cos, ResizeModifiers modifiers)
		{
			bool result = true;
			int dx = 0, dy = 0;
			int width = Width;
			int height = Height;
			switch (pointId)
			{
				case ControlPointIds.ArrowTopControlPoint:
				case ControlPointIds.ArrowBottomControlPoint:
					if (pointId == ControlPointIds.ArrowTopControlPoint)
					{
						//result = (transformedDeltaX == 0);
						if (!Geometry.MoveRectangleTop(width, height, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out width, out height))
							result = false;
					}
					else
					{
						//result = (transformedDeltaX == 0);
						if (!Geometry.MoveRectangleBottom(width, height, transformedDeltaX, transformedDeltaY, cos, sin, modifiers, out dx, out dy, out width, out height))
							result = false;
					}
					int newHeadWidth = HeadWidth - (int)Math.Round(transformedDeltaX);
					if (newHeadWidth < 0)
					{
						newHeadWidth = 0;
						result = false;
					}
					else if (newHeadWidth > Width)
					{
						newHeadWidth = Width;
						result = false;
					}
					HeadWidth = newHeadWidth;
					break;
				default:
					return base.MovePointByCore(pointId, transformedDeltaX, transformedDeltaY, sin, cos, modifiers);
			}
			if (width < headWidth)
			{
				width = headWidth;
				result = false;
			}
			MoveByCore(dx, dy);
			Width = width;
			Height = height;
			return result;
		}


		/// <override></override>
		protected override void ProcessExecModelPropertyChange(IModelMapping propertyMapping)
		{
			switch (propertyMapping.ShapePropertyId)
			{
				case PropertyIdBodyHeight:
					BodyHeight = propertyMapping.GetInteger();
					break;
				case PropertyIdHeadWidth:
					HeadWidth = propertyMapping.GetInteger();
					break;
				default:
					base.ProcessExecModelPropertyChange(propertyMapping);
					break;
			}
		}


		private void CalcShapePoints()
		{
			int left = -(int)Math.Round(Width / 2f);
			int right = left + Width;
			int top = -(int)Math.Round(Height / 2f);
			int bottom = top + Height;

			// head tip
			shapePoints[0].X = right;
			shapePoints[0].Y = 0;

			// head side tip (top)
			shapePoints[1].X = left + BodyWidth;
			shapePoints[1].Y = top;

			// body corner (top)
			shapePoints[2].X = left;
			shapePoints[2].Y = top;

			// body corner (bottom)
			shapePoints[3].X = left;
			shapePoints[3].Y = bottom;

			// head / body connection point
			shapePoints[4].X = left + BodyWidth;
			shapePoints[4].Y = bottom;

			// head side tip (bottom)
			shapePoints[5].X = left + BodyWidth;
			shapePoints[5].Y = bottom;
		}


		#region Fields

		protected const int PropertyIdBodyHeight = 9;
		protected const int PropertyIdHeadWidth = 10;

		private Point newTipPos = Point.Empty;
		private Point oldTipPos = Point.Empty;

		private Point[] shapePoints = new Point[6];
		private int headWidth;
		private double bodyHeightRatio;
		#endregion
	}

}