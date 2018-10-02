using System;

using Dataweb.NShape.Advanced;

namespace Dataweb.NShape.FhisServices
{
	public class Watch : CircleBase
	{
		private const int CenterDiameter = 6;

		static internal ShapeType CircleType;
		static internal ShapeType PolylineType;

		internal static Shape CreateInstance(ShapeType shapeType, Template template)
		{
			return new Watch(shapeType, template);
		}

		private CircleBase circleCenter = null;
		private PolylineBase[] clockMarks = new PolylineBase[12];
		private PolylineBase[] clockArrows = new PolylineBase[3];   // 0-Hour, 1-Monute, 2-Second
		private DateTime time = DateTime.Now;
		private bool hasSeconds = true;

		public new int Diameter { get => base.Diameter; set { base.Diameter = value; if (circleCenter != null) circleCenter.Diameter = CenterDiameter; } }

		public DateTime Time { get => time; set => SetTime(value); }
		public IFillStyle CenterFillStyle { get => circleCenter.FillStyle; set => circleCenter.FillStyle = value; }
		public ILineStyle HourMarkLineStyle { get => clockMarks[0].LineStyle; set { clockMarks[0].LineStyle = value; clockMarks[3].LineStyle = value; clockMarks[6].LineStyle = value; clockMarks[9].LineStyle = value; } }
		public ILineStyle HourArrowLineStyle { get => clockArrows[0].LineStyle; set => clockArrows[0].LineStyle = value; }
		public ILineStyle MinuteArrowLineStyle { get => clockArrows[1].LineStyle; set => clockArrows[1].LineStyle = value; }
		public ILineStyle SecondArrowLineStyle { get => clockArrows[2]?.LineStyle; set { if (clockArrows[2] != null) clockArrows[2].LineStyle = value; } }
		public bool HasSeconds
		{
			get => hasSeconds;
			set
			{
				hasSeconds = value;
				if (hasSeconds)
				{
					if (clockArrows[2] == null)
					{
						clockArrows[2] = (PolylineBase)PolylineType.CreateInstance();
						clockArrows[2].IsToolbarHidden = true;
						this.Children.Add(clockArrows[2]);
					}
				}
				else
				{
					if (clockArrows[2] != null)
					{
						this.Children.Remove(clockArrows[2]);
						clockArrows[2] = null;
					}
				}
			}
		}


		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new Watch(Type, this.Template);
			result.CopyFrom(this);
			return result;
		}


		protected internal Watch(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
			Init();
		}


		protected internal Watch(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
			Init();
		}

		protected override void InitializeToDefault(IStyleSet styleSet)
		{
			base.InitializeToDefault(styleSet);

			CenterFillStyle = styleSet.FillStyles["Gray"];
			HourMarkLineStyle = styleSet.LineStyles.Thick;
			HourArrowLineStyle = styleSet.LineStyles.Thick;
			MinuteArrowLineStyle = styleSet.LineStyles.Thick;
			FillStyle = styleSet.FillStyles.Transparent;
			circleCenter.Diameter = CenterDiameter;
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
			for (int i = 0, n = clockMarks.Length; i < n; ++i)
			{
				clockMarks[i] = (PolylineBase)PolylineType.CreateInstance();
				clockMarks[i].IsToolbarHidden = true;
				this.Children.Add(clockMarks[i]);
			}

			for (int i = 0, n = clockArrows.Length - 1; i < n; ++i) // Часовая и минутная стрелки - жирные
			{
				clockArrows[i] = (PolylineBase)PolylineType.CreateInstance();
				clockArrows[i].IsToolbarHidden = true;
				this.Children.Add(clockArrows[i]);
			}
			if (hasSeconds) // секундная стрелка - нежирная
			{
				clockArrows[2] = (PolylineBase)PolylineType.CreateInstance();
				clockArrows[2].IsToolbarHidden = true;
				this.Children.Add(clockArrows[2]);
			}

			circleCenter = (CircleBase)CircleType.CreateInstance();
			circleCenter.IsToolbarHidden = true;
			circleCenter.Diameter = CenterDiameter;
			this.Children.Add(circleCenter);
		}

		public void SetTime(DateTime time)
		{
			this.time = time;
			int centerX = X, centerY = Y, hour = time.Hour % 12;
			double a
				, lenH = 16d * Diameter / 64d
				, lenM = 23d * Diameter / 64d
				, lenS = 25d * Diameter / 64d
				;

			a = Math.PI * ((double)hour / 6d + (double)time.Minute / 360d); // 720d = 60d*12d
			clockArrows[0].MoveControlPointTo(ControlPointId.FirstVertex, centerX, centerY, ResizeModifiers.None);
			clockArrows[0].MoveControlPointTo(ControlPointId.LastVertex, (int)Math.Round(centerX + lenH * Math.Sin(a)), (int)Math.Round(centerY - lenH * Math.Cos(a)), ResizeModifiers.None);

			a = Math.PI * (double)time.Minute / 30d;
			clockArrows[1].MoveControlPointTo(ControlPointId.FirstVertex, centerX, centerY, ResizeModifiers.None);
			clockArrows[1].MoveControlPointTo(ControlPointId.LastVertex, (int)Math.Round(centerX + lenM * Math.Sin(a)), (int)Math.Round(centerY - lenM * Math.Cos(a)), ResizeModifiers.None);

			if (clockArrows[2] == null)
				ToolTipText = time.ToShortTimeString();
			else
			{
				ToolTipText = time.ToLongTimeString();

				a = Math.PI * (double)time.Second / 30d;
				clockArrows[2].MoveControlPointTo(ControlPointId.FirstVertex, centerX, centerY, ResizeModifiers.None);
				clockArrows[2].MoveControlPointTo(ControlPointId.LastVertex, (int)Math.Round(centerX + lenS * Math.Sin(a)), (int)Math.Round(centerY - lenS * Math.Cos(a)), ResizeModifiers.None);
			}
		}

		public void DrawFace(int x, int y)
		{
			X = x;
			Y = y;

			circleCenter.X = X;
			circleCenter.Y = Y;

			int posH1 = Diameter / 2 - Diameter / 12
				, posH2 = Diameter / 2 - Diameter / 32
				;

			clockMarks[0].MoveControlPointTo(ControlPointId.FirstVertex, x, y - posH2, ResizeModifiers.None);
			clockMarks[0].MoveControlPointTo(ControlPointId.LastVertex, x, y - posH1, ResizeModifiers.None);

			clockMarks[6].MoveControlPointTo(ControlPointId.FirstVertex, x, y + posH1, ResizeModifiers.None);
			clockMarks[6].MoveControlPointTo(ControlPointId.LastVertex, x, y + posH2, ResizeModifiers.None);

			clockMarks[3].MoveControlPointTo(ControlPointId.FirstVertex, x + posH1, y, ResizeModifiers.None);
			clockMarks[3].MoveControlPointTo(ControlPointId.LastVertex, x + posH2, y, ResizeModifiers.None);

			clockMarks[9].MoveControlPointTo(ControlPointId.FirstVertex, x - posH2, y, ResizeModifiers.None);
			clockMarks[9].MoveControlPointTo(ControlPointId.LastVertex, x - posH1, y, ResizeModifiers.None);

			DrawFaceHourMark(1, clockMarks[1], posH1, posH2);
			DrawFaceHourMark(2, clockMarks[2], posH1, posH2);
			DrawFaceHourMark(4, clockMarks[4], posH1, posH2);
			DrawFaceHourMark(5, clockMarks[5], posH1, posH2);
			DrawFaceHourMark(7, clockMarks[7], posH1, posH2);
			DrawFaceHourMark(8, clockMarks[8], posH1, posH2);
			DrawFaceHourMark(10, clockMarks[10], posH1, posH2);
			DrawFaceHourMark(11, clockMarks[11], posH1, posH2);
		}

		private void DrawFaceHourMark(int mark, PolylineBase polylineBase, int pos1, int pos2)
		{
			double a = Math.PI * mark / 6d, sinA = Math.Sin(a), cosA = Math.Cos(a);
			polylineBase.MoveControlPointTo(ControlPointId.FirstVertex, X + (int)Math.Round(sinA * pos2), Y - (int)Math.Round(cosA * pos2), ResizeModifiers.None);
			polylineBase.MoveControlPointTo(ControlPointId.LastVertex, X + (int)Math.Round(sinA * pos1), Y - (int)Math.Round(cosA * pos1), ResizeModifiers.None);
		}

	}
}
