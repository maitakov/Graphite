using System;

using Dataweb.NShape.Advanced;

namespace Dataweb.NShape.FhisServices
{
	public class Thermometer : BoxBase
	{
		private const int minWidth = 24;

		static internal ShapeType PolylineType = null;
		static internal ShapeType CircleType = null;
		static internal ShapeType BoxType = null;

		internal static Shape CreateInstance(ShapeType shapeType, Template template)
		{
			return new Thermometer(shapeType, template);
		}

		private const int innerGap = 3;

		private IStyleSet styleSet = null;
		private CircleBase tempCircle = null;
		private BoxBase tempBox = null;
		//private PolylineBase tempMark = null;
		private PolylineBase[] tempNegMarks = null;
		private PolylineBase[] tempPosMarks = null;
		private float tempMin = -50f;
		private float tempMax = 50f;
		private float temp = 20f;

		public float TemperatureMinimum
		{
			get => this.tempMin;
			set { this.tempMin = value; Refresh(); }
		}

		public float TemperatureMaximum
		{
			get => this.tempMax;
			set { this.tempMax = value; Refresh(); }
		}

		public float Temperature
		{
			get => this.temp;
			set { this.temp = value; Refresh(); }
		}

		private void Refresh()
		{
			int arcRadius = GetArcRadius()
				, left = X + (int)Math.Round(-Width / 2f)
				, top = Y + (int)Math.Round(-Height / 2f)
				, right = left + Width
				, bottom = top + Height
				, left1 = left + (int)Math.Round(Width / 3f)
				, right1 = right - (int)Math.Round(Width / 3f)
				, top1 = top + arcRadius
				, bottom1 = bottom - (int)Math.Round(arcRadius * (3 + Math.Sin(60 * Math.PI / 180)))
				, w1 = (int)Math.Round(Width / 6f)
				//, w2 = (int)Math.Round(Width / 16f)
				, tZero = bottom1 + (int)Math.Round(tempMin * ((float)bottom1 - (float)top1) / (tempMax - tempMin))
				, t = bottom1 - (int)Math.Round((temp - tempMin) * ((float)bottom1 - (float)top1) / (tempMax - tempMin))
				, y
				, n
				;

			n = -(int)Math.Round(tempMin) / 10 + 1;
			if (tempMin > 0 || tempNegMarks != null && tempNegMarks.Length != n)
			{
				if (tempNegMarks != null)
					for (int i = tempNegMarks.Length - 1; i >= 0; --i)
					{
						Children.Remove(tempNegMarks[i]);
						tempNegMarks[i] = null;
					}
				tempNegMarks = null;
			}
			if (n > 0)
			{
				if ((tempNegMarks == null || tempNegMarks.Length != n))
				{
					tempNegMarks = new PolylineBase[n];
					for (int i = 0; i < n; ++i)
					{
						tempNegMarks[i] = (PolylineBase)PolylineType.CreateInstance();
						tempNegMarks[i].IsToolbarHidden = true;
						Children.Add(tempNegMarks[i]);
					}
				}
				for (int i = 0, tempNeg = 0; i < n; tempNeg -= 10, ++i)
				{
					y = bottom1 - (int)Math.Round(((float)tempNeg - (float)tempMin) * ((float)bottom1 - (float)top1) / ((float)tempMax - (float)tempMin));
					tempNegMarks[i].MoveControlPointTo(ControlPointId.FirstVertex, left, y, ResizeModifiers.None);
					tempNegMarks[i].MoveControlPointTo(ControlPointId.LastVertex, i == 0 ? left1 : left + w1, y, ResizeModifiers.None);
				}
			}

			n = (int)Math.Round(tempMax) / 10 + 1;
			if (tempMax < 0 || tempPosMarks != null && tempPosMarks.Length != n)
			{
				if (tempPosMarks != null)
					for (int i = tempPosMarks.Length - 1; i >= 0; --i)
					{
						Children.Remove(tempPosMarks[i]);
						tempPosMarks[i] = null;
					}
				tempPosMarks = null;
			}
			if (n > 0)
			{
				if ((tempPosMarks == null || tempPosMarks.Length != n))
				{
					tempPosMarks = new PolylineBase[n];
					for (int i = 0; i < n; ++i)
					{
						tempPosMarks[i] = (PolylineBase)PolylineType.CreateInstance();
						tempPosMarks[i].IsToolbarHidden = true;
						Children.Add(tempPosMarks[i]);
					}
				}
				for (int i = 0, tempPos = 0; i < n; tempPos += 10, ++i)
				{
					y = bottom1 - (int)Math.Round(((float)tempPos - (float)tempMin) * ((float)bottom1 - (float)top1) / ((float)tempMax - (float)tempMin));
					tempPosMarks[i].MoveControlPointTo(ControlPointId.FirstVertex, right, y, ResizeModifiers.None);
					tempPosMarks[i].MoveControlPointTo(ControlPointId.LastVertex, i == 0 ? right1 : right - w1, y, ResizeModifiers.None);
				}
			}

			//tempMark.MoveControlPointTo(ControlPointId.FirstVertex, left1, t, ResizeModifiers.None);
			//tempMark.MoveControlPointTo(ControlPointId.LastVertex, right1, t, ResizeModifiers.None);

			tempBox.MoveControlPointTo(1, left1 + innerGap, t, ResizeModifiers.None);
			tempBox.MoveControlPointTo(3, right1 - innerGap, t, ResizeModifiers.None);
			tempBox.MoveControlPointTo(6, left1 + innerGap, bottom - 2 * arcRadius, ResizeModifiers.None);

			tempCircle.X = X;
			tempCircle.Y = bottom - 2 * arcRadius;
			tempCircle.Diameter = 4 * arcRadius - 2 * innerGap;

			if (temp > 0)
			{
				tempBox.FillStyle = styleSet.FillStyles.Red;
				tempCircle.FillStyle = styleSet.FillStyles.Red;
			}
			else
			{
				tempBox.FillStyle = styleSet.FillStyles.Blue;
				tempCircle.FillStyle = styleSet.FillStyles.Blue;
			}
			ToolTipText = Temperature.ToString() + "\x00B0C";
		}

		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new Thermometer(Type, this.Template);
			result.CopyFrom(this);
			return result;
		}


		protected internal Thermometer(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}


		protected internal Thermometer(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
		}

		public void DrawFace(int x, int y, int height)
		{
			X = x;
			Y = y;

			Height = height;
			Width = (int)Math.Max(Math.Round(height / 4f), minWidth);
		}

		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet)
		{
			this.styleSet = styleSet;
			base.InitializeToDefault(styleSet);
			FillStyle = styleSet.FillStyles.Transparent;
			Height = 120;
			Width = 30;

			tempCircle = (CircleBase)CircleType.CreateInstance();
			tempCircle.IsToolbarHidden = true;
			tempCircle.LineStyle = styleSet.LineStyles.None;
			Children.Add(tempCircle);

			tempBox = (BoxBase)BoxType.CreateInstance();
			tempBox.IsToolbarHidden = true;
			tempBox.LineStyle = styleSet.LineStyles.None;
			Children.Add(tempBox);

			//tempMark = (Polyline)polylineType.CreateInstance();
			//tempMark.IsToolbarHidden = true;
			//Children.Add(tempMark);
		}

		/// <override></override>
		protected override bool CalculatePath()
		{
			if (!base.CalculatePath())
				return false;

			if(Width == 0
			|| Height == 0)
				return false;

			Path.Reset();
			int arcRadius = GetArcRadius()
				, left = (int)Math.Round(-Width / 2f)
				, left1 = left + (int)Math.Round(Width / 3f)
				, right = left + Width
				, right1 = right - (int)Math.Round(Width / 3f)
				, top = (int)Math.Round(-Height / 2f)
				, top1 = top + arcRadius
				, bottom = top + Height
				, bottom1 = bottom - (int)Math.Round(arcRadius * (3 + Math.Sin(60 * Math.PI / 180)))
				//, w1 = (int)Math.Round(Width / 8f)
				//, w2 = (int)Math.Round(Width / 16f)
				//, t = bottom1 - (int)Math.Round(((float)temp - (float)tempMin)*((float)bottom1 - (float)top1)/((float)tempMax - (float)tempMin))
				;

			Path.StartFigure();

			Path.AddArc(left1, top, right1 - left1, 2 * arcRadius, 0, -180);
			Path.AddLine(left1, top1, left1, bottom1);
			Path.AddArc(left, bottom - 4 * arcRadius, Width, 4 * arcRadius, 240, -300);
			Path.AddLine(right1, bottom1, right1, top1);

			Path.CloseFigure();

			return true;
		}

		private int GetArcRadius()
		{
			return (int)Math.Round(Math.Min(Height / 4f, Width / 4f));
		}
	}
}
