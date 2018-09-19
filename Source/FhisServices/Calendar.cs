using System;
using System.Drawing;
using System.Globalization;

using Dataweb.NShape.Advanced;

namespace Dataweb.NShape.FhisServices
{
	public class Calendar : BoxBase
	{
		private const int WeekDaysHGap = 1;
		private const int WeekDaysVGap = 2;

		static internal ShapeType BoxType;

		private IStyleSet styleSet;
		private BoxBase[] weekDays = new BoxBase[7];
		private DateTime date = DateTime.Today;

		public DateTime Date { get => date; set => SetDate(value); }

		internal static Shape CreateInstance(ShapeType shapeType, Template template)
		{
			return new Calendar(shapeType, template);
		}

		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new Calendar(Type, this.Template);
			result.CopyFrom(this);
			return result;
		}

		protected internal Calendar(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
			Init();
		}


		protected internal Calendar(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
			Init();
		}

		protected override void InitializeToDefault(IStyleSet styleSet)
		{
			this.styleSet = styleSet;
			base.InitializeToDefault(styleSet);
			LineStyle = styleSet.LineStyles.None;
			FillStyle = styleSet.FillStyles.Transparent;

			weekDays[5].LineStyle = styleSet.LineStyles.Red;	// Saturday, Суббота
			weekDays[6].LineStyle = styleSet.LineStyles.Red;	// Sunday, Воскресенье
		}

		/// <override></override>
		protected override bool CalculatePath()
		{
			if (!base.CalculatePath())
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

		private void Init()
		{
			Width = 10;
			Height = 10;
			for (int i = 0, n = weekDays.Length; i < n; ++i)
			{
				weekDays[i] = (BoxBase)BoxType.CreateInstance();
				weekDays[i].IsToolbarHidden = true;
				this.Children.Add(weekDays[i]);
			}
			PlaceWeekDays();
		}

		private void PlaceWeekDays()
		{
			int x = X - Width / 2 + WeekDaysHGap
				, dx = Width / 7
				, y = Y - Height / 2 + WeekDaysVGap
				, dy = Height - WeekDaysVGap - WeekDaysVGap
				;
			//	1	2	3
			//	4		5
			//	6	7	8
			for (int i = 0, n = weekDays.Length; i < n; x += dx, ++i)
			{
				weekDays[i].MoveTo((x + x + dx) / 2, Y);
				weekDays[i].MoveControlPointTo(1, x + WeekDaysHGap * 2, y, ResizeModifiers.None);
				weekDays[i].MoveControlPointTo(3, x + dx - WeekDaysHGap, y, ResizeModifiers.None);
				weekDays[i].MoveControlPointTo(6, x + WeekDaysHGap * 2, y + dy, ResizeModifiers.None);
			}
		}

		public void SetDate(DateTime date)
		{
			this.date = date;
			for (int i = 0, n = weekDays.Length; i < n; ++i)
				weekDays[i].FillStyle = styleSet.FillStyles.White;
			switch (date.DayOfWeek)
			{
				case DayOfWeek.Monday: weekDays[0].FillStyle = styleSet.FillStyles.Black; break;
				case DayOfWeek.Tuesday: weekDays[1].FillStyle = styleSet.FillStyles.Black; break;
				case DayOfWeek.Wednesday: weekDays[2].FillStyle = styleSet.FillStyles.Black; break;
				case DayOfWeek.Thursday: weekDays[3].FillStyle = styleSet.FillStyles.Black; break;
				case DayOfWeek.Friday: weekDays[4].FillStyle = styleSet.FillStyles.Black; break;
				case DayOfWeek.Saturday: weekDays[5].FillStyle = styleSet.FillStyles.Red; break;
				case DayOfWeek.Sunday: weekDays[6].FillStyle = styleSet.FillStyles.Red; break;
			}
			ToolTipText = CultureInfo.CurrentCulture.Name == "ru-RU" ? date.ToString("dddd") + ", " + date.ToLongDateString() : date.ToLongDateString();
		}

		public void DrawFace(int x, int y)
		{
			X = x;
			Y = y;
			PlaceWeekDays();
		}

		public void DrawFace(int x, int y, int width, int height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
			PlaceWeekDays();
		}
	}
}
