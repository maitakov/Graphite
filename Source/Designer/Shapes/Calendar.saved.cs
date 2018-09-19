#define BOX

using System;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.WinFormsUI;

namespace Dataweb.NShape.Designer.Shapes
{	public class Calendar
	{	private const int WeekDaysGap = 2;

		static internal ShapeType BoxType;
		static internal ShapeType PolylineType;

		private Project project = null;
		private BoxBase boxCalendar = null;
#if BOX
		private BoxBase[] currWeekDays = new BoxBase[7];
		private BoxBase[] nextWeekDays = new BoxBase[7];
#else
		private PolylineBase[] currWeekDays = new PolylineBase[7];
		private PolylineBase[] nextWeekDays = new PolylineBase[7];
#endif

		public Calendar(Project project, ShapeBase parent, int width, int height)
		{	this.project = project;

			boxCalendar = (BoxBase)BoxType.CreateInstance();
			boxCalendar.Tag = int.MinValue;
			boxCalendar.Width = width;
			boxCalendar.Height = height;
			parent.Children.Add(boxCalendar);

			Init();
		}

		public Calendar(Project project, Display display, int width, int height)
		{	this.project = project;

			boxCalendar = (BoxBase)BoxType.CreateInstance();
			boxCalendar.Tag = int.MinValue;
			boxCalendar.Width = width;
			boxCalendar.Height = height;
			display.InsertShape(boxCalendar);

			Init();
		}

		private void Init()
		{
#if BOX
			for(int i = 0, n = currWeekDays.Length; i < n; ++i)
			{	currWeekDays[i] = (BoxBase)BoxType.CreateInstance();
				currWeekDays[i].Tag = int.MinValue;
				boxCalendar.Children.Add(currWeekDays[i]);
			}
			currWeekDays[5].LineStyle = project.Design.LineStyles.Red;	// Saturday, Суббота
			currWeekDays[6].LineStyle = project.Design.LineStyles.Red;	// Sunday, Воскресенье
			PlaceWeekDays(false, currWeekDays);

			for(int i = 0, n = nextWeekDays.Length; i < n; ++i)
			{	nextWeekDays[i] = (BoxBase)BoxType.CreateInstance();
				nextWeekDays[i].Tag = int.MinValue;
				boxCalendar.Children.Add(nextWeekDays[i]);
			}
			nextWeekDays[5].LineStyle = project.Design.LineStyles.Red;	// Saturday, Суббота
			nextWeekDays[6].LineStyle = project.Design.LineStyles.Red;	// Sunday, Воскресенье
			PlaceWeekDays(true, nextWeekDays);
#else
			for(int i = 0, n = currWeekDays.Length; i < n; ++i)
			{	currWeekDays[i] = (PolylineBase)PolylineType.CreateInstance();
				currWeekDays[i].Tag = int.MinValue;
				boxCalendar.Children.Add(currWeekDays[i]);
			}
			currWeekDays[5].LineStyle = project.Design.LineStyles.Red;	// Saturday, Суббота
			currWeekDays[6].LineStyle = project.Design.LineStyles.Red;	// Sunday, Воскресенье
			PlaceWeekDays(false, currWeekDays);

			for(int i = 0, n = nextWeekDays.Length; i < n; ++i)
			{	nextWeekDays[i] = (PolylineBase)PolylineType.CreateInstance();
				nextWeekDays[i].Tag = int.MinValue;
				boxCalendar.Children.Add(nextWeekDays[i]);
			}
			nextWeekDays[5].LineStyle = project.Design.LineStyles.Red;	// Saturday, Суббота
			nextWeekDays[6].LineStyle = project.Design.LineStyles.Red;	// Sunday, Воскресенье
			PlaceWeekDays(true, nextWeekDays);
#endif
		}

		public void SetDate(DateTime date)
		{	MarkWeekDays(date.DayOfWeek, currWeekDays);
		}

		public void DrawFace(int x, int y)
		{	boxCalendar.X = x;
			boxCalendar.Y = y;
#if BOX
			PlaceWeekDays(false, currWeekDays);
			PlaceWeekDays(true, nextWeekDays);
#else
			PlaceWeekDays(false, currWeekDays);
			PlaceWeekDays(true, nextWeekDays);
#endif
		}

#if BOX
		private void PlaceWeekDays(bool next, BoxBase[] weekDays)
		{	int x = boxCalendar.X - boxCalendar.Width/2
				, dx = boxCalendar.Width/7
				, y = boxCalendar.Y - boxCalendar.Height/2
				, dy = boxCalendar.Height/5
				;
			y += dy;
			if(next)
				y += dy + dy;
		//	1	2	3
		//	4		5
		//	6	7	8
			for(int i = 0, n = weekDays.Length; i < n; x += dx, ++i)
			{	weekDays[i].MoveTo((x + x + dx)/2, y);
				weekDays[i].MoveControlPointTo(1, x + WeekDaysGap*2, y, ResizeModifiers.None);
				weekDays[i].MoveControlPointTo(3, x + dx - WeekDaysGap, y, ResizeModifiers.None);
				weekDays[i].MoveControlPointTo(6, x + WeekDaysGap*2, y + dy, ResizeModifiers.None);

			}
		}
#else
		private void PlaceWeekDays(bool next, PolylineBase[] weekDays)
		{	int x = boxCalendar.X - boxCalendar.Width/2
				, dx = boxCalendar.Width/7
				, y = boxCalendar.Y - boxCalendar.Height/2
				, dy = boxCalendar.Height/3
				;
			y += dy;
			if(next)
				y += dy;
			for(int i = 0, n = weekDays.Length; i < n; x += dx, ++i)
			{	weekDays[i].MoveControlPointTo(ControlPointId.FirstVertex, x + WeekDaysGap*2, y, ResizeModifiers.None);
				weekDays[i].MoveControlPointTo(ControlPointId.LastVertex,  x + dx - WeekDaysGap, y, ResizeModifiers.None);
			}
		}
#endif

#if BOX
		private void MarkWeekDays(DayOfWeek dayOfWeek, BoxBase[] weekDays)
		{	for(int i = 0, n = weekDays.Length; i < n; ++i)
				weekDays[i].FillStyle = project.Design.FillStyles.White;
			switch(dayOfWeek)
			{	case DayOfWeek.Monday:		weekDays[0].FillStyle = project.Design.FillStyles.Black;	break;
				case DayOfWeek.Tuesday:		weekDays[1].FillStyle = project.Design.FillStyles.Black;	break;
				case DayOfWeek.Wednesday:	weekDays[2].FillStyle = project.Design.FillStyles.Black;	break;
				case DayOfWeek.Thursday:	weekDays[3].FillStyle = project.Design.FillStyles.Black;	break;
				case DayOfWeek.Friday:		weekDays[4].FillStyle = project.Design.FillStyles.Black;	break;
				case DayOfWeek.Saturday:	weekDays[5].FillStyle = project.Design.FillStyles.Red;		break;
				case DayOfWeek.Sunday:		weekDays[6].FillStyle = project.Design.FillStyles.Red;		break;
			}
		}
#else
		private void MarkWeekDays(DayOfWeek dayOfWeek, PolylineBase[] weekDays)
		{
		}
#endif
	}
}
