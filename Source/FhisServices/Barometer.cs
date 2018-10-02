using System;

using Dataweb.NShape.Advanced;

namespace Dataweb.NShape.FhisServices
{
	public class Barometer : CircleBase
	{
		private const int CenterDiameter = 6;
		private const int SunDiameter = 10;

		private const float MmHgToHPa = 1.33322f;

		static internal ShapeType CircleType;
		static internal ShapeType PolylineType;

		internal static Shape CreateInstance(ShapeType shapeType, Template template)
		{
			return new Barometer(shapeType, template);
		}

		private CircleBase circleCenter = null;

		private CircleBase circleSun1 = null;
		private PolylineBase[] sunLights1 = new PolylineBase[4];

		private CircleBase circleSun2 = null;
		private PolylineBase[] sunLights2 = new PolylineBase[4];

		private PolylineBase pressureArrow = null;

		private PolylineBase minMark = null;
		private PolylineBase maxMark = null;

		private float pressureMin = 720f;   // mmHg
		private float pressureMax = 795f;   // mmHg
		private float pressure = 760f;      // mmHg
											// hPa - гекто Паскаль
											// mmHg - миллиметр ртутного столба
											// 1 Па = 0,0075 мм рт. ст. Или 1 мм рт. ст. = 133,3 Па
											// Отмечены колебания атмосферного давления на уровне моря в пределах 641 — 816 мм рт. ст.

		public new int Diameter
		{
			get => base.Diameter;
			set
			{
				base.Diameter = value;
				if (circleCenter != null)
					circleCenter.Diameter = CenterDiameter;
				if (circleSun1 != null)
					circleSun1.Diameter = SunDiameter;
				if (circleSun2 != null)
					circleSun2.Diameter = SunDiameter;
				if (pressureArrow != null)
					PressureMmHg = pressure;
			}
		}

		public float PressureMmHg { get => pressure; set => SetPressure(value); }
		public float PressureHPa { get => (float)Math.Round(pressure * MmHgToHPa); set => SetPressure((float)Math.Round(value / MmHgToHPa)); }

		public float PressureMinMmHg { get => pressureMin; set => pressureMin = value; }
		public float PressureMinHPa { get => (float)Math.Round(pressureMin * MmHgToHPa); set => pressureMin = (float)Math.Round(value / MmHgToHPa); }

		public float PressureMaxMmHg { get => pressureMax; set => pressureMax = value; }
		public float PressureMaxHPa { get => (float)Math.Round(pressureMax * MmHgToHPa); set => pressureMax = (float)Math.Round(value / MmHgToHPa); }

		public IFillStyle CenterFillStyle { get => circleCenter.FillStyle; set => circleCenter.FillStyle = value; }
		public ILineStyle PressureArrowLineStyle { get => pressureArrow.LineStyle; set => pressureArrow.LineStyle = value; }

		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new Barometer(Type, this.Template);
			result.CopyFrom(this);
			return result;
		}


		protected internal Barometer(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
			Init();
		}


		protected internal Barometer(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
			Init();
		}

		protected override void InitializeToDefault(IStyleSet styleSet)
		{
			base.InitializeToDefault(styleSet);

			CenterFillStyle = styleSet.FillStyles["Gray"];
			PressureArrowLineStyle = styleSet.LineStyles.Thick;
			FillStyle = styleSet.FillStyles.Transparent;
			minMark.LineStyle = styleSet.LineStyles.Thick;
			maxMark.LineStyle = styleSet.LineStyles.Thick;
			circleCenter.Diameter = CenterDiameter;
			InitializeSunToDefault(styleSet, circleSun1);
			InitializeSunToDefault(styleSet, circleSun2);
		}

		private void InitializeSunToDefault(IStyleSet styleSet, CircleBase circleSun)
		{
			circleSun.Diameter = SunDiameter;
			circleSun.FillStyle = styleSet.FillStyles.White;
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

			circleSun1 = Init(sunLights1);
			circleSun2 = Init(sunLights2);

			minMark = (PolylineBase)PolylineType.CreateInstance();
			minMark.IsToolbarHidden = true;
			this.Children.Add(minMark);

			maxMark = (PolylineBase)PolylineType.CreateInstance();
			maxMark.IsToolbarHidden = true;
			this.Children.Add(maxMark);

			pressureArrow = (PolylineBase)PolylineType.CreateInstance();
			pressureArrow.IsToolbarHidden = true;
			this.Children.Add(pressureArrow);

			circleCenter = (CircleBase)CircleType.CreateInstance();
			circleCenter.IsToolbarHidden = true;
			circleCenter.Diameter = CenterDiameter;
			this.Children.Add(circleCenter);
		}

		private CircleBase Init(PolylineBase[] sunLights)
		{
			for (int i = 0, n = sunLights.Length; i < n; ++i)
			{
				sunLights[i] = (PolylineBase)PolylineType.CreateInstance();
				sunLights[i].IsToolbarHidden = true;
				this.Children.Add(sunLights[i]);
			}
			CircleBase circleSun = (CircleBase)CircleType.CreateInstance();
			circleSun.IsToolbarHidden = true;
			circleSun.Diameter = SunDiameter;
			this.Children.Add(circleSun);
			return circleSun;
		}

		private void SetPressure(float pressure)
		{
			this.pressure = pressure;
			int centerX = X, centerY = Y;
			double a = 3d * Math.PI * (pressure - pressureMin) / (pressureMax - pressureMin) / 2d - 3d * Math.PI / 4d
				, len = 25d * Diameter / 64d
				;
			pressureArrow.MoveControlPointTo(ControlPointId.FirstVertex, centerX, centerY, ResizeModifiers.None);
			pressureArrow.MoveControlPointTo(ControlPointId.LastVertex, (int)Math.Round(centerX + len * Math.Sin(a)), (int)Math.Round(centerY - len * Math.Cos(a)), ResizeModifiers.None);

			ToolTipText = PressureMmHg.ToString() + " " + Properties.Strings.PressureMmHg + "/" + PressureHPa.ToString() + " " + Properties.Strings.PressureHPa;
		}

		public void DrawFace(int x, int y)
		{
			X = x;
			Y = y;

			circleCenter.X = X;
			circleCenter.Y = Y;
			//-----------------------
			int pos1 = Diameter / 2 - Diameter / 16
				, pos2 = Diameter / 2 - Diameter / 64
				;
			double d = Math.Sqrt(2d) / 2d;

			minMark.MoveControlPointTo(ControlPointId.FirstVertex, X - (int)Math.Round(d * pos2), Y + (int)Math.Round(d * pos2), ResizeModifiers.None);
			minMark.MoveControlPointTo(ControlPointId.LastVertex, X - (int)Math.Round(d * pos1), Y + (int)Math.Round(d * pos1), ResizeModifiers.None);

			maxMark.MoveControlPointTo(ControlPointId.FirstVertex, X + (int)Math.Round(d * pos2), Y + (int)Math.Round(d * pos2), ResizeModifiers.None);
			maxMark.MoveControlPointTo(ControlPointId.LastVertex, X + (int)Math.Round(d * pos1), Y + (int)Math.Round(d * pos1), ResizeModifiers.None);
			//-----------------------
			DrawSun(X + Diameter / 4, Y, circleSun1, sunLights1);
			DrawSun(X, Y - Diameter / 4, circleSun2, sunLights2);
		}

		private void DrawSun(int x, int y, CircleBase circleSun, PolylineBase[] sunLights)
		{
			circleSun.X = x;
			circleSun.Y = y;

			sunLights[0].MoveControlPointTo(ControlPointId.FirstVertex, circleSun.X - circleSun.Diameter, circleSun.Y, ResizeModifiers.None);
			sunLights[0].MoveControlPointTo(ControlPointId.LastVertex, circleSun.X + circleSun.Diameter, circleSun.Y, ResizeModifiers.None);

			sunLights[1].MoveControlPointTo(ControlPointId.FirstVertex, circleSun.X, circleSun.Y - circleSun.Diameter, ResizeModifiers.None);
			sunLights[1].MoveControlPointTo(ControlPointId.LastVertex, circleSun.X, circleSun.Y + circleSun.Diameter, ResizeModifiers.None);

			int pos = (int)Math.Round(circleSun.Diameter * Math.Sqrt(2d) / 2d);
			sunLights[2].MoveControlPointTo(ControlPointId.FirstVertex, circleSun.X - pos, circleSun.Y - pos, ResizeModifiers.None);
			sunLights[2].MoveControlPointTo(ControlPointId.LastVertex, circleSun.X + pos, circleSun.Y + pos, ResizeModifiers.None);

			sunLights[3].MoveControlPointTo(ControlPointId.FirstVertex, circleSun.X + pos, circleSun.Y - pos, ResizeModifiers.None);
			sunLights[3].MoveControlPointTo(ControlPointId.LastVertex, circleSun.X - pos, circleSun.Y + pos, ResizeModifiers.None);
		}
	}
}
