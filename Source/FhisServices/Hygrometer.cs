using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Dataweb.NShape.Advanced;

namespace Dataweb.NShape.FhisServices
{
	public class Hygrometer : BoxBase
	{
		private const int minWidth = 24;

		internal static Shape CreateInstance(ShapeType shapeType, Template template)
		{
			return new Hygrometer(shapeType, template);
		}

		private IStyleSet styleSet = null;
		private float humidity = 50f;

		public float Humidity
		{	get => this.humidity;
			set{ this.humidity = value; Refresh(); }
		}

		private void Refresh()
		{
			ToolTipText = humidity.ToString() + "%";
			Invalidate();
		}

		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new Hygrometer(Type, this.Template);
			result.CopyFrom(this);
			return result;
		}


		protected internal Hygrometer(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}


		protected internal Hygrometer(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
		}


		protected override void InitializeToDefault(IStyleSet styleSet)
		{
			this.styleSet = styleSet;
			base.InitializeToDefault(styleSet);
			FillStyle = styleSet.FillStyles.Transparent;
			Height = 100;
			Width = 60;

			Refresh();
		}

		/// <override></override>
		protected override bool CalculatePath()
		{	if(!base.CalculatePath()
			|| Width == 0
			|| Height == 0)
				return false;

			Path.Reset();
			int arcRadius = GetArcRadius()
				, left = (int)Math.Round(-Width / 2f)
				, right = left + Width
				, top = (int)Math.Round(-Height / 2f)
				, bottom = top + Height
				, center = (int)Math.Round((left + right)/2f)
				;
			double a = Math.Acos((double)arcRadius/(bottom - arcRadius - top))
				, b = Math.PI/2d - a
				, dif = arcRadius*(1d - Math.Cos(b))
				;
			int left1 = (int)(left + dif)
				, right1 = (int)(right - dif)
				, bottom1 = (int)Math.Round(bottom - arcRadius*(1d + Math.Sin(b)))
				;

			Path.StartFigure();

			Path.AddLine(center, top, left1, bottom1);
			Path.AddArc(left, bottom - 2*arcRadius, Width, 2*arcRadius, (float)((Math.PI + b)*180d/Math.PI), -(float)((Math.PI + 2d*b)*180d/Math.PI));
			Path.AddLine(right1, bottom1, center, top);

			Path.CloseFigure();

			return true;
		}

		public void DrawFace(int x, int y, int height)
		{	X = x;
			Y = y;
			Height = height;
			Width = (int)Math.Max(Math.Round(3f * height / 5f), minWidth);
			Refresh();
		//?	Humidity = Humidity;
		}

		public override void Draw(Graphics graphics)
		{
			GraphicsPath Path = new GraphicsPath();

			int arcRadius = GetArcRadius()
				, left = X + (int)Math.Round(-Width / 2f)
				, right = left + Width
				, top = Y + (int)Math.Round(-Height / 2f)
				, bottom = top + Height
				, center = (int)Math.Round((left + right)/2f)
				;
			double a = Math.Acos((double)arcRadius/(bottom - arcRadius - top))
				, b = Math.PI/2d - a
				, h = (double)humidity*((double)bottom - (double)top)/100d
				, dif = Width/2d - ((double)bottom - (double)top - h)*Math.Tan(b)
				;
			int bottom1 = (int)Math.Round(bottom - arcRadius*(1d + Math.Sin(b)))
				, bottomH = (int)Math.Round(bottom - h)
				;

			Path.StartFigure();

			if(bottomH < bottom1)
				Path.AddLine((int)Math.Round(right - dif), bottomH, (int)Math.Round(left + dif), bottomH);
			else
			{
				b = Math.Asin((h - arcRadius) / arcRadius);
				dif = Math.Round(arcRadius*Math.Cos(b));
				Path.AddLine(center + (int)dif, bottomH, center - (int)dif, bottomH);
			}
			Path.AddArc(left, bottom - 2*arcRadius, Width, 2*arcRadius, (float)((Math.PI + b)*180d/Math.PI), -(float)((Math.PI + 2d*b)*180d/Math.PI));

			Path.CloseFigure();

			if(styleSet != null)
			{	Brush brush = ToolCache.GetBrush(styleSet.FillStyles["LightBlue"]);
				graphics.FillPath(brush, Path);
			}
			base.Draw(graphics);
		}

		private int GetArcRadius()
		{
			return (int)Math.Round(Math.Min(Height / 2f, Width / 2f));
		}
	}
}
