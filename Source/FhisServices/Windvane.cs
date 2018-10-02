using System;
using System.Reflection;

using Dataweb.NShape.Advanced;

namespace Dataweb.NShape.FhisServices
{
	public class Windvane : CircleBase
	{
		static internal ShapeType PolylineType;

		static private PropertyInfo polyline_StartCapStyle = null;
		static private PropertyInfo polyline_EndCapStyle = null;

		internal static Shape CreateInstance(ShapeType shapeType, Template template)
		{
			return new Windvane(shapeType, template);
		}

		private int lineOutline = 4;
		private float windMaxSpeed = 24.4f;
		private bool useColors = true;
		private IStyleSet styleSet;
		private PolylineBase lineWind = null;
		private PolylineBase lineNS = null;
		private PolylineBase lineWE = null;
		private PolylineBase lineNESW = null;
		private PolylineBase lineNWSE = null;

		/*
		 0	 0,0- 0,2	Штиль
		 1	 0,3- 1,5	Тихий
		 2	 1,6- 3,3	Лёгкий
		 3	 3,4- 5,4	Слабый
		 4	 5,5- 7,9	Умеренный
		 5	 8,0-10,7	Свежий
		 6	10,8-13,8	Сильный
		 7	13,9-17,1	Крепкий
		 8	17,2-20,7	Очень крепкий
		 9	20,8-24,4	Шторм
		10	24,5-28,4	Сильный шторм
		11	28,5-32,6	Жестокий шторм
		12	33,0		Ураган
		 */

		public float WindMaxSpeed { get => windMaxSpeed; set => windMaxSpeed = value; }
		public bool UseColors { get => useColors; set => useColors = value; }

		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new Windvane(Type, this.Template);
			result.CopyFrom(this);
			return result;
		}


		protected internal Windvane(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
			Init();
		}


		protected internal Windvane(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
			Init();
		}


		protected override void InitializeToDefault(IStyleSet styleSet)
		{
			this.styleSet = styleSet;
			base.InitializeToDefault(styleSet);
			FillStyle = styleSet.FillStyles.Transparent;
			lineWind.LineStyle = styleSet.LineStyles.Thick;
		//?	polyline_StartCapStyle.SetValue(lineWind, styleSet.CapStyles["WindStart"], null);
			polyline_EndCapStyle.SetValue(lineWind, styleSet.CapStyles["WindEnd"], null);
		}

		/// <override></override>
		protected override bool CalculatePath()
		{
			if (!base.CalculatePath()
			|| Diameter == 0)
				return false;

			int left = (int)Math.Round(-Diameter / 2f);
			int top = (int)Math.Round(-Diameter / 2f);
			Path.Reset();
			Path.StartFigure();
			Path.AddEllipse(left, top, Diameter, Diameter);
			Path.CloseFigure();
			return true;
		}

		private void Init()
		{
			Diameter = 10;

			lineWind = (PolylineBase)PolylineType.CreateInstance();
			lineWind.IsToolbarHidden = true;

			if (polyline_StartCapStyle == null)
				polyline_StartCapStyle = lineWind.GetType().GetProperty("StartCapStyle");
			if (polyline_EndCapStyle == null)
				polyline_EndCapStyle = lineWind.GetType().GetProperty("EndCapStyle");

			int x = X, y = Y;
			float direction = 90, speed = windMaxSpeed;
			double a = Math.PI * direction / 180d
				, len2 = speed * Diameter / windMaxSpeed / 2d
				;
			lineWind.MoveControlPointTo(ControlPointId.FirstVertex, (int)Math.Round(x + len2 * Math.Sin(a)), (int)Math.Round(y - len2 * Math.Cos(a)), ResizeModifiers.None);
			lineWind.MoveControlPointTo(ControlPointId.LastVertex, (int)Math.Round(x - len2 * Math.Sin(a)), (int)Math.Round(y + len2 * Math.Cos(a)), ResizeModifiers.None);
			this.Children.Add(lineWind);

			lineNS = (PolylineBase)PolylineType.CreateInstance();
			lineNS.IsToolbarHidden = true;
			this.Children.Add(lineNS);

			lineWE = (PolylineBase)PolylineType.CreateInstance();
			lineWE.IsToolbarHidden = true;
			this.Children.Add(lineWE);

			lineNESW = (PolylineBase)PolylineType.CreateInstance();
			lineNESW.IsToolbarHidden = true;
			this.Children.Add(lineNESW);

			lineNWSE = (PolylineBase)PolylineType.CreateInstance();
			lineNWSE.IsToolbarHidden = true;
			this.Children.Add(lineNWSE);
		}

		public void SetWind(float direction, float speed, bool southLat = false)
		{
			int x = X, y = Y;
			direction %= 360;

			double a = Math.PI * direction / 180d
				, len2 = speed * Diameter / windMaxSpeed / 2d   // делим на 200 - половина диаметра (радиуса) на 100
				;
			lineWind.MoveControlPointTo(ControlPointId.FirstVertex, (int)Math.Round(x + len2 * Math.Sin(a)), (int)Math.Round(y - len2 * Math.Cos(a)), ResizeModifiers.None);
			lineWind.MoveControlPointTo(ControlPointId.LastVertex, (int)Math.Round(x - len2 * Math.Sin(a)), (int)Math.Round(y + len2 * Math.Cos(a)), ResizeModifiers.None);
			if(!useColors)
				lineWind.LineStyle = styleSet.LineStyles.Thick;
			else if (direction < 45 || 315 < direction)
				lineWind.LineStyle = styleSet.LineStyles[southLat ? "WindSouth" : "WindNord"];
			else if (135 < direction && direction < 225)
				lineWind.LineStyle = styleSet.LineStyles[southLat ? "WindNord" : "WindSouth"];
			else
				lineWind.LineStyle = styleSet.LineStyles.Thick;

			ToolTipText = speed.ToString() + " " + Properties.Strings.WindSpeed + "; " + direction.ToString() + "\x00B0";
		}

		public void DrawFace(int x, int y)
		{
			X = x;
			Y = y;

			int len = Diameter / 2 + lineOutline, len2 = (int)Math.Round(Math.Sin(Math.PI / 4d) * len);

			// Север -- Юг
			lineNS.MoveControlPointTo(ControlPointId.FirstVertex, x, y - len, ResizeModifiers.None);
			lineNS.MoveControlPointTo(ControlPointId.LastVertex, x, y + len, ResizeModifiers.None);

			// Запад -- Восток
			lineWE.MoveControlPointTo(ControlPointId.FirstVertex, x - len, y, ResizeModifiers.None);
			lineWE.MoveControlPointTo(ControlPointId.LastVertex, x + len, y, ResizeModifiers.None);

			// Северо-восток -- Юго-запад
			lineNESW.MoveControlPointTo(ControlPointId.FirstVertex, x + len2, y - len2, ResizeModifiers.None);
			lineNESW.MoveControlPointTo(ControlPointId.LastVertex, x - len2, y + len2, ResizeModifiers.None);

			// Северо-запад -- Юго-восток
			lineNWSE.MoveControlPointTo(ControlPointId.FirstVertex, x - len2, y - len2, ResizeModifiers.None);
			lineNWSE.MoveControlPointTo(ControlPointId.LastVertex, x + len2, y + len2, ResizeModifiers.None);
		}
	}
}
