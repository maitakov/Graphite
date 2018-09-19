using System;
using System.Collections.Generic;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.WinFormsUI;

namespace Dataweb.NShape.Designer.Shapes
{	public class StatePropertyCollection
	{	static internal ShapeType CircleType;
		static internal ShapeType PropType;

		internal CircleBase circleOuter = null;
		internal Dictionary<int, TriangleBase> props = new Dictionary<int, TriangleBase>();
		internal CircleBase circleIntegralProps = null;

		public Dictionary<int, TriangleBase> Props { get{ return props; } }
		public int Count { get{ return props.Count; } }

		public StatePropertyCollection(Project project, ShapeBase parent, int diameterOuter, int diameterInner)
		{	PreInit(project, diameterOuter);
			parent.Children.Add(circleOuter);
			PostInit(project, diameterInner);
		}

		public StatePropertyCollection(Project project, Display display, int diameterOuter, int diameterInner)
		{	PreInit(project, diameterOuter);
			display.InsertShape(circleOuter);
			PostInit(project, diameterInner);
		}

		private void PreInit(Project project, int diameterOuter)
		{	circleOuter = (CircleBase)CircleType.CreateInstance();
			circleOuter.IsToolbarHidden = true;
			circleOuter.FillStyle = project.Design.FillStyles.White;
			circleOuter.Diameter = diameterOuter;
		}

		private void PostInit(Project project, int diameterInner)
		{	circleIntegralProps = (CircleBase)CircleType.CreateInstance();
			circleIntegralProps.IsToolbarHidden = true;
			circleIntegralProps.LineStyle = project.Design.LineStyles.None;
			circleIntegralProps.FillStyle = project.Design.FillStyles.Transparent;
			circleIntegralProps.Diameter = diameterInner;
			circleOuter.Children.Add(circleIntegralProps);
		}

		public TriangleBase AddProp(int id, ILineStyle lineStyle)
		{	TriangleBase prop = (TriangleBase)PropType.CreateInstance();
			prop.IsToolbarHidden = true;
			if(lineStyle != null)
				prop.LineStyle = lineStyle;
			props.Add(id, prop);
			circleOuter.Children.Add(prop);
			ChangePropCount();
			return prop;
		}

		public void RemoveProp(int id)
		{	TriangleBase prop = props[id];
			circleOuter.Children.Remove(prop);
			props.Remove(id);
			ChangePropCount();
		}

		private void ChangePropCount()
		{	TriangleBase triangle;
			double a, da, da2;
			int rO, rI, x2, y2, cx = circleOuter.X, cy = circleOuter.Y;

			a = 0d;
			da2 = Math.PI/Count;
			da = 2d*da2;
			rO = circleOuter.Diameter/2;
			rI = circleIntegralProps.Diameter/2;
			x2 = (int)Math.Round(cx - rI*Math.Sin(da2));
			y2 = (int)Math.Round(cy - rI*Math.Cos(da2));
			//		1
			//		5
			//	2	3	4
			for(int i = 0, n = Count; i < n; a += da, ++i)
			{	triangle = Props[i];

				triangle.MoveControlPointTo(1, (int)Math.Round(cx + rO*Math.Sin(a)), (int)Math.Round(cy - rO*Math.Cos(a)), ResizeModifiers.None);
				triangle.MoveControlPointTo(2, x2, y2, ResizeModifiers.None);
				triangle.MoveControlPointTo(3, x2 = (int)Math.Round(cx + rI*Math.Sin(a+da2)), y2 = (int)Math.Round(cy - rI*Math.Cos(a+da2)), ResizeModifiers.None);
			}
		}

		public void ColorsGroup()
		{
			Dictionary<IFillStyle, int> fillStyles = new Dictionary<IFillStyle, int>();
			Shape[] shapes = new Shape[circleOuter.Children.Count];
			circleOuter.Children.CopyTo(shapes, 0);
			for(int i = 0, n = shapes.Length, m = n - 1; i < m; ++i)
			{	TriangleBase shape = shapes[i] as TriangleBase;
				if(shape == null)
					continue;
				if(!fillStyles.ContainsKey(shape.FillStyle))
					fillStyles.Add(shape.FillStyle, 0);
				++fillStyles[shape.FillStyle];
			}
		//	foreach(KeyValuePair<IFillStyle, int> fillStyle in fillStyles)
		//	{
		//	}

			for(int i = 0, n = shapes.Length, m = n - 1; i < m; ++i)
			{	Shape shape1 = shapes[i];
				for(int j = i + 1; j < n; ++j)
				{
				}
			}
		}

		public void ColorsGroup(LineStyleCollection lineStyles, ILineStyle lineStyle, IFillStyle[] fillStyles)
		{	Dictionary<IFillStyle, int> fillStylesCount = new Dictionary<IFillStyle, int>();
			for(int i = 0, n = fillStyles.Length; i < n; ++i)
				fillStylesCount.Add(fillStyles[i], 0);

			Shape[] shapes = new Shape[circleOuter.Children.Count];
			circleOuter.Children.CopyTo(shapes, 0);
			for(int i = 0, n = shapes.Length; i < n; ++i)
			{	TriangleBase shape = shapes[i] as TriangleBase;
				if(shape != null)
					++fillStylesCount[shape.FillStyle];
			}

			int idx = 0;
			foreach(KeyValuePair<IFillStyle, int> fillStyleCount in fillStylesCount)
			{
				int cnt = fillStyleCount.Value;
				while(cnt > 0)
				{
					string colorName;
					TriangleBase shape = shapes[idx++] as TriangleBase;
					if(shape != null)
					{	--cnt;
						shape.FillStyle = fillStyleCount.Key;
						if(shape.LineStyle != lineStyles.Normal)
							shape.LineStyle = lineStyles.Contains(colorName = shape.FillStyle.Name) ? lineStyles[colorName] : lineStyles.None;
					}
				}
			}
		}
	}
}
