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
//#define VIEWER

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using System.Net;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

using GMap.NET.WindowsForms;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.Commands;
using Dataweb.NShape.Controllers;
using Dataweb.NShape.WinFormsUI;

namespace Dataweb.NShape.Designer
{
	public partial class DiagramDesignerMainForm : Form
	{	private const string currWeatherRequestUri = "http://api.openweathermap.org/data/2.5/weather?lat={2}&lon={3}&mode=xml&units=metric&lang={1}&appid={0}";
		private const string foreWeatherRequestUri = "http://api.openweathermap.org/data/2.5/forecast?lat={2}&lon={3}&mode=xml&units=metric&lang={1}&appid={0}";
		private const string weatherImageUri = "http://openweathermap.org/img/w/{0}.png";

		private const string ElectricPowerLinesOverlayId = "ElectricPowerLines";
		private const string EnergyPowerFlowOverlayId = "EnergyPowerFlow";

		private const string RepairNeedColor = "Red";
		private const string RepairProcColor = "Green";

		private string[][] fillColorNames = new string[][]
		{
			new string[]{ "Green"/*"White"*/, "Yellow", "Orange", "Red" }									// Цвета МЧС
		,	new string[]{ "LightGreen", "Green"/*"White"*/, "LightYellow", "Yellow", "LightRed", "Red" }	// Прочие цвета
		};
		private string[][] fillColorNames2 = new string[][]
		{
			new string[]{ "Green", "Yellow", "Orange", "Red" }												// Цвета МЧС
		,	new string[]{ "LightGreen", "Green", "LightYellow", "Yellow", "LightRed", "Red" }				// Прочие цвета
		};

		private class ReplaceInfo
		{
			internal string FromBegin;
			internal string FromEnd;
			internal string ToBegin;
			internal string ToEnd;

			internal ReplaceInfo(string from, string to) : this(from, null, to, null)
			{
			}

			internal ReplaceInfo(string fromBegin, string fromEnd, string toBegin, string toEnd)
			{
				FromBegin = fromBegin;
				FromEnd = fromEnd;
				ToBegin = toBegin;
				ToEnd = toEnd;
			}
		}
		private class MenuItemInfo
		{
			internal string Title;
			internal List<ReplaceInfo> Descriptions;
		}

		private static Thread mainThread;
		private static string cultureName = ConfigurationManager.AppSettings["cultureName"];
		private static bool weatherLog = bool.Parse(ConfigurationManager.AppSettings["weatherLog"]);
		private static string weatherRequestAppid = ConfigurationManager.AppSettings["weatherRequestAppid"];
		private static int weatherRequestTimeout = int.Parse(ConfigurationManager.AppSettings["weatherRequestTimeout"]);
		private static TimeSpan weatherForecastTimeSpan = TimeSpan.Parse(ConfigurationManager.AppSettings["weatherForecastTimeSpan"]);
		private static bool useWindvaneColors = bool.Parse(ConfigurationManager.AppSettings["useWindvaneColors"]);
		private static bool emergencyColors = bool.Parse(ConfigurationManager.AppSettings["emergencyColors"]);
		private static bool statePosTop = bool.Parse(ConfigurationManager.AppSettings["statePosTop"]);
		private static bool staticPropertyBorders = bool.Parse(ConfigurationManager.AppSettings["staticPropertyBorders"]);
		private static bool dynamicPropertyBorders = bool.Parse(ConfigurationManager.AppSettings["dynamicPropertyBorders"]);
		private static bool showElectricPowerLines = bool.Parse(ConfigurationManager.AppSettings["showElectricPowerLines"]);
		private static string electricPowerLinesFile = ConfigurationManager.AppSettings["electricPowerLinesFile"];
		private static ColorsGrouping colorsGrouping = (ColorsGrouping)Enum.Parse(typeof(ColorsGrouping), ConfigurationManager.AppSettings["colorsGrouping"]);
		private static int energyFlowInterval = int.Parse(ConfigurationManager.AppSettings["energyFlowInterval"]);
		private static string exePath;
		private static Dictionary<string, MenuItemInfo> menuTitles = new Dictionary<string, MenuItemInfo>();

		static DiagramDesignerMainForm()
		{
			mainThread = Thread.CurrentThread;
			ChangeLanguage();
			exePath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

			//	menuTitles.Add("Select...", "Выбрать...");
			//	menuTitles.Add("Select all", "Выбрать всё");
			//	menuTitles.Add("Select all shapes based on a template", "Выбрать все объекты на базе шаблона");
			//	menuTitles.Add("Select all shapes of a type", "Выбрать все объекты типа");
			//	menuTitles.Add("Unselect all", "Снять выделение");

			menuTitles.Add("Select...", null);
			menuTitles.Add("Select all", null);
			menuTitles.Add("Select all shapes based on a template", null);
			menuTitles.Add("Select all shapes of a type", null);
			menuTitles.Add("Unselect all", null);

			menuTitles.Add("Shape Info", new MenuItemInfo()
			{
				Title = Properties.Strings.MenuShapeInfoTitle //"Информация об объекте"
				, Descriptions = new List<ReplaceInfo>(new ReplaceInfo[]{ new ReplaceInfo("No shapes selected.", Properties.Strings.MenuDescription0 /*"Не выбрано ни одного объекта"*/)
				, new ReplaceInfo("More than one shape selected.", Properties.Strings.MenuDescription0 /*"Выбрано более одного объекта"*/)
				, new ReplaceInfo("Show information", string.Empty) })
			});

			menuTitles.Add("Bring to Front", new MenuItemInfo()
			{
				Title = Properties.Strings.MenuBringToFrontTitle //"На передний план"
				, Descriptions = new List<ReplaceInfo>(new ReplaceInfo[] { new ReplaceInfo("No shapes selected.", Properties.Strings.MenuDescription0 /*"Не выбрано ни одного объекта"*/)
				, new ReplaceInfo("Bring", string.Empty) })
			});
			menuTitles.Add("Send to Back", new MenuItemInfo()
			{
				Title = Properties.Strings.MenuSendToBackTitle //"На задний план"
				, Descriptions = new List<ReplaceInfo>(new ReplaceInfo[] { new ReplaceInfo("No shapes selected.", Properties.Strings.MenuDescription0 /*"Не выбрано ни одного объекта"*/)
				, new ReplaceInfo("Send", string.Empty) })
			});

			menuTitles.Add("Add shapes to active layers", null);
			menuTitles.Add("Assign shapes to active layers", null);
			menuTitles.Add("Remove shapes from all layers", null);

			menuTitles.Add("Group Shapes", new MenuItemInfo()
			{
				Title = Properties.Strings.MenuGroupShapesTitle //"Установить ролевое визуальное отношение"
				, Descriptions = new List<ReplaceInfo>(new ReplaceInfo[] { new ReplaceInfo("Not enough shapes selected.", Properties.Strings.MenuGroupShapesDescription0 /*"Не выбрано достаточного количества объектов"*/)
					, new ReplaceInfo("Group ", " shapes."
						, Properties.Strings.MenuGroupShapesDescription1/*"Установить ролевое визуальное отношение между"*/ + " "
						, " " + Properties.Strings.MenuGroupShapesDescription2/*"объектами"*/) })
			});
			menuTitles.Add("Ungroup Shapes", new MenuItemInfo()
			{
				Title = Properties.Strings.MenuUngroupShapesTitle //"Разорвать ролевое визуальное отношение"
				, Descriptions = new List<ReplaceInfo>(new ReplaceInfo[] { new ReplaceInfo("No group selected.", Properties.Strings.MenuUngroupShapesDescription0 /*"Не выбрано ни одного ролевого визуального отношения"*/)
					, new ReplaceInfo("Ungroup ", " shapes."
						, Properties.Strings.MenuUngroupShapesDescription1/*"Разорвать ролевое визуальное отношение между"*/ + " "
						, " " + Properties.Strings.MenuUngroupShapesDescription2/*"объектами"*/) })
			});

			menuTitles.Add("Aggregate Shapes", null);
			menuTitles.Add("Disaggregate Shapes", null);

			menuTitles.Add("Cut", new MenuItemInfo()
			{
				Title = Properties.Strings.MenuCutTitle //"Вырезать"
				, Descriptions = new List<ReplaceInfo>(new ReplaceInfo[] { new ReplaceInfo("No shapes selected.", Properties.Strings.MenuDescription0 /*"Не выбрано ни одного объекта"*/)
					, new ReplaceInfo("Cut", string.Empty) })
			});
			menuTitles.Add("Copy as Image", new MenuItemInfo()
			{
				Title = Properties.Strings.MenuCopyAsImageTitle //"Скопировать как рисунок"
				, Descriptions = new List<ReplaceInfo>(new ReplaceInfo[] { new ReplaceInfo("Copy", string.Empty) })
			});
			menuTitles.Add("Copy", new MenuItemInfo()
			{
				Title = Properties.Strings.MenuCopyTitle //"Скопировать"
				, Descriptions = new List<ReplaceInfo>(new ReplaceInfo[] { new ReplaceInfo("No shapes selected.", Properties.Strings.MenuDescription0 /*"Не выбрано ни одного объекта"*/)
					, new ReplaceInfo("Copy", string.Empty) })
			});
			menuTitles.Add("Paste", new MenuItemInfo()
			{
				Title = Properties.Strings.MenuPasteTitle //"Вставить"
				, Descriptions = new List<ReplaceInfo>(new ReplaceInfo[] { new ReplaceInfo("No shapes cut/copied yet.", Properties.Strings.MenuPasteDescription /*"Ни одного объекта не скопировано/не вырезано"*/) })
			});
			menuTitles.Add("Delete", new MenuItemInfo()
			{
				Title = Properties.Strings.MenuDeleteTitle //"Удалить"
				, Descriptions = new List<ReplaceInfo>(new ReplaceInfo[] { new ReplaceInfo("No shapes selected.", Properties.Strings.MenuDescription0 /*"Не выбрано ни одного объекта"*/)
					, new ReplaceInfo("Delete", string.Empty) })
			});

			menuTitles.Add("Properties", null);
			//, new MenuItemInfo() { Title = "Свойства" });

			menuTitles.Add("Undo", new MenuItemInfo()
			{
				Title = Properties.Strings.MenuUndoTitle //"Отменить"
				, Descriptions = new List<ReplaceInfo>(new ReplaceInfo[] { new ReplaceInfo("No undo commands left.", Properties.Strings.MenuUndoDescription /*"Не осталось ни одной команды для отмены"*/)
					, new ReplaceInfo(string.Empty, string.Empty) })
			});
			menuTitles.Add("Redo", new MenuItemInfo()
			{
				Title = Properties.Strings.MenuRedoTitle //"Повторить"
				, Descriptions = new List<ReplaceInfo>(new ReplaceInfo[] { new ReplaceInfo("No redo commands left.", Properties.Strings.MenuRedoDescription /*"Не осталось ни одной команды для повтора"*/)
					, new ReplaceInfo(string.Empty, string.Empty) })
			});
		}

		static private void ChangeLanguage()
		{
			CultureInfo cultureInfo = new CultureInfo(cultureName);
			Thread.CurrentThread.CurrentCulture = cultureInfo;
			Thread.CurrentThread.CurrentUICulture = cultureInfo;

			GMap.NET.LanguageType languageType;
			if(!Enum.TryParse<GMap.NET.LanguageType>(cultureInfo.EnglishName.Substring(0, cultureInfo.EnglishName.IndexOf(' ')), true, out languageType))
				languageType = GMap.NET.LanguageType.Russian;
			GMap.NET.MapProviders.GMapProvider.Language = languageType;
		}

		[DllImport("shell32")]
		public extern static int SHGetSpecialFolderPath(IntPtr hwndOwner, StringBuilder lpszPath, int nFolder, int fCreate);

		private bool selectionChanging = false;
		private DateTime currTime = DateTime.Now;
		private DateTime foreTime = DateTime.Now;
		private DateTime weatherCurrTime = DateTime.MinValue;
		private DateTime weatherForeTime = DateTime.MinValue;

		public DiagramDesignerMainForm()
		{
			InitializeComponent();

			this.toolTipTimer.Interval = this.toolTip.AutomaticDelay;
			this.hierarchyTreeView.CanExpandGetter = delegate (object x) { return ((Shape)x).Children.Count > 0; };
			this.hierarchyTreeView.ChildrenGetter = delegate (object x) { return ((Shape)x).Children; };
			this.hierarchyTreeView.SmallImageList = this.toolSetListViewPresenter.ListView.SmallImageList;
			this.hierarchyTreeView.LargeImageList = this.toolSetListViewPresenter.ListView.LargeImageList;
			this.olvColumnType.AspectGetter = delegate (object x) { return x is ShapeGroup ? "Ролевое визуальное отношение" : ((Shape)x).Type.Name; };
			this.olvColumnType.ImageGetter = delegate (object x)
			{
			//	if(x is ShapeGroup)
			//		return -1;
				//ShapeType shapeType = ((Shape)x).Type;
				Shape shape = (Shape)x;
				if(shape.Template != null)
				{	Tool tool = this.toolSetController.FindTool(shape.Template);
					if(tool != null)
						return this.hierarchyTreeView.SmallImageList.Images.IndexOfKey(tool.Name);
				}
				return -1;
			};
			this.propertyWindowTabControl.Controls.Remove(this.layersTab);
			project.LibraryLoaded += Project_LibraryLoaded;

			Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
			runtimeModeComboBox.SelectedIndex = 0;
			Visible = false;

			// Set texts for status bar tool tips
			SetToolTipTexts();

			historyTrackBar.Visible = false;
#if VIEWER
			toolStripContainer.BottomToolStripPanel.Controls.Remove(statusStrip);
			toolStripContainer.TopToolStripPanel.Controls.Remove(displayToolStrip);
			toolStripContainer.TopToolStripPanel.Controls.Remove(settingsToolStrip);
			toolStripContainer.TopToolStripPanel.Controls.Remove(editToolStrip);
#endif
#if !DEBUG_UI
			historyTrackBar.Visible = false;
			toolStripContainer.TopToolStripPanel.Controls.Remove(debugToolStrip);
#endif
			// Find out the latest repository version
			maxRepositoryVersion = 0;
			try
			{
				using(Project p = new Project())
				{
					p.Name = "Test";
					p.Create();
					maxRepositoryVersion = p.Repository.Version;
					p.Close();
				}
			}
			catch(Exception) { }
		}

		private void Project_LibraryLoaded(object sender, LibraryLoadedEventArgs e)
		{
			CreateVehicle();
		}

		#region [Public] Properties

		public bool ShowGrid
		{
			get { return showGrid; }
			set
			{
				showGrid = value;
				if(CurrentDisplay != null) CurrentDisplay.IsGridVisible = value;
			}
		}


		public bool SnapToGrid
		{
			get { return snapToGrid; }
			set
			{
				snapToGrid = value;
				if(CurrentDisplay != null) CurrentDisplay.SnapToGrid = value;
			}
		}


		public Color GridColor
		{
			get { return gridColor; }
			set
			{
				gridColor = value;
				if(CurrentDisplay != null) CurrentDisplay.GridColor = value;
			}
		}


		public bool ShowDefaultContextMenu
		{
			get { return showDefaultContextMenu; }
			set
			{
				showDefaultContextMenu = value;
				toolSetListViewPresenter.ShowDefaultContextMenu =
				modelTreePresenter.ShowDefaultContextMenu =
				layerEditorListView.ShowDefaultContextMenu = value;
				if(CurrentDisplay != null) CurrentDisplay.ShowDefaultContextMenu = value;
			}
		}


		public bool HideDeniedMenuItems
		{
			get { return hideDeniedMenuItems; }
			set
			{
				hideDeniedMenuItems = value;
				toolSetListViewPresenter.HideDeniedMenuItems =
				modelTreePresenter.HideDeniedMenuItems =
				layerEditorListView.HideDeniedMenuItems = value;
				if(CurrentDisplay != null) CurrentDisplay.HideDeniedMenuItems = value;
			}
		}


		public int GridSize
		{
			get { return gridSize; }
			set
			{
				gridSize = value;
				if(CurrentDisplay != null) CurrentDisplay.GridSize = value;
			}
		}


		public int SnapDistance
		{
			get { return snapDistance; }
			set
			{
				snapDistance = value;
				if(CurrentDisplay != null) CurrentDisplay.SnapDistance = value;
			}
		}


		public ControlPointShape ResizePointShape
		{
			get { return resizePointShape; }
			set
			{
				resizePointShape = value;
				if(CurrentDisplay != null) CurrentDisplay.ResizeGripShape = value;
			}
		}


		public ControlPointShape ConnectionPointShape
		{
			get { return connectionPointShape; }
			set
			{
				connectionPointShape = value;
				if(CurrentDisplay != null) CurrentDisplay.ConnectionPointShape = value;
			}
		}


		public int ControlPointSize
		{
			get { return controlPointSize; }
			set
			{
				controlPointSize = value;
				if(CurrentDisplay != null) CurrentDisplay.GripSize = value;
			}
		}


		public int Zoom
		{
			get { return zoom; }
			set
			{
				zoom = value;
				if(CurrentDisplay != null && CurrentDisplay.ZoomLevel != zoom)
					CurrentDisplay.ZoomLevel = zoom;
				UpdateZoomControl();
			}
		}


		public bool HighQuality
		{
			get { return highQuality; }
			set
			{
				highQuality = value;
				if(CurrentDisplay != null)
				{
					CurrentDisplay.HighQualityBackground = value;
					CurrentDisplay.HighQualityRendering = value;
					CurrentDisplay.Refresh();
				}
			}
		}


		public Display CurrentDisplay
		{
			get { return currentDisplay; }
			set
			{
				if(currentDisplay != null)
				{
					UnregisterDisplayEvents(currentDisplay);
					((IDiagramPresenter)currentDisplay).CloseCaptionEditor(true);
				}
				currentDisplay = value;
				if(currentDisplay != null)
				{
					RegisterDisplayEvents(currentDisplay);

					// Grip settings
					currentDisplay.ConnectionPointShape = ConnectionPointShape;
					currentDisplay.ResizeGripShape = ResizePointShape;
					currentDisplay.GripSize = ControlPointSize;
					// Grid settings
					currentDisplay.GridColor = GridColor;
					currentDisplay.GridSize = GridSize;
					currentDisplay.IsGridVisible = ShowGrid;
					// Security Settings
					//currentDisplay.ShowDefaultContextMenu = ShowDefaultContextMenu;
					currentDisplay.HideDeniedMenuItems = HideDeniedMenuItems;
					currentDisplay.ShowDefaultContextMenu = false;
					if(ShowDefaultContextMenu)
						currentDisplay.ContextMenuStrip = diagramContextMenuStrip;

					// Rendering settings
					currentDisplay.HighQualityRendering = HighQuality;
					currentDisplay.IsGridVisible = ShowGrid;
					currentDisplay.HighQualityBackground = HighQuality;
#if DEBUG_UI
					currentDisplay.IsDebugInfoCellOccupationVisible = ShowCellOccupation;
					currentDisplay.IsDebugInfoInvalidateVisible = ShowInvalidatedAreas;
#endif
					// Apply display's zoom to the UI
					Zoom = currentDisplay.ZoomLevel;

					if(currentDisplay.Diagram != null)
					{
						currentDisplay.ActiveTool = toolSetController.SelectedTool;
						if(currentDisplay.SelectedShapes.Count > 0)
							propertyController.SetObjects(0, currentDisplay.SelectedShapes);
						else propertyController.SetObject(0, currentDisplay.Diagram);
					}
					layerPresenter.DiagramPresenter = CurrentDisplay;

					if(layoutControlForm != null)
					{
						if(currentDisplay.Diagram != null)
							layoutControlForm.Diagram = currentDisplay.Diagram;
						else layoutControlForm.Close();
					}

					display_ShapesSelected(currentDisplay, EventArgs.Empty);
				}
			}
		}


#if DEBUG_UI

		public bool ShowCellOccupation
		{
			get { return showCellOccupation; }
			set
			{
				showCellOccupation = value;
				if(CurrentDisplay != null) CurrentDisplay.IsDebugInfoCellOccupationVisible = value;
			}
		}


		public bool ShowInvalidatedAreas
		{
			get { return showInvalidatedAreas; }
			set
			{
				showInvalidatedAreas = value;
				if(CurrentDisplay != null) CurrentDisplay.IsDebugInfoInvalidateVisible = value;
			}
		}

#endif

		#endregion


		private void CheckFrameworkVersion()
		{
			Assembly exeAssembly = this.GetType().Assembly;
			Assembly coreAssembly = typeof(Project).Assembly;
			Assembly uiAssembly = typeof(Display).Assembly;

			if(exeAssembly == null || coreAssembly == null || uiAssembly == null)
				throw new Exception("Failed to retrive component's assemblies.");

			// Check installed .NET framework version
			Version coreAssemblyVersion = new Version(coreAssembly.ImageRuntimeVersion.Replace("v", ""));
			Version uiAssemblyVersion = new Version(uiAssembly.ImageRuntimeVersion.Replace("v", ""));
			Version exeAssemblyVersion = new Version(exeAssembly.ImageRuntimeVersion.Replace("v", ""));
			if(Environment.Version < coreAssemblyVersion
			|| Environment.Version < uiAssemblyVersion
			|| Environment.Version < exeAssemblyVersion)
			{
				string msg = string.Empty;
				msg += string.Format("The installed .NET framework version does not meet the requirements:{0}", Environment.NewLine);
				msg += string.Format(".NET Framework {0} is installed, version {1} is required.", Environment.Version, coreAssembly.ImageRuntimeVersion);
				throw new NShapeException(msg);
			}

			System.Reflection.AssemblyName designerAssemblyName = this.GetType().Assembly.GetName();
			System.Reflection.AssemblyName coreAssemblyName = typeof(Project).Assembly.GetName();
			System.Reflection.AssemblyName uiAssemblyName = typeof(Display).Assembly.GetName();
			// Check nShape framework library versions
			if(coreAssemblyName.Version != uiAssemblyName.Version)
			{
				string msg = string.Empty;
				msg += "The versions of the loaded nShape framework libraries do not match:" + Environment.NewLine;
				msg += string.Format("{0}: Version {1}{2}", coreAssemblyName.Name, coreAssemblyName.Version, Environment.NewLine);
				msg += string.Format("{0}: Version {1}{2}", uiAssemblyName.Name, uiAssemblyName.Version, Environment.NewLine);
				throw new NShapeException(msg);
			}
			// Check program against used nShape framework library versions
			if(coreAssemblyName.Version != designerAssemblyName.Version
				|| uiAssemblyName.Version != designerAssemblyName.Version)
			{
				string msg = string.Empty;
				msg += "The version of this program does not match the versions of the loaded nShape framework libraries:" + Environment.NewLine;
				msg += string.Format("{0}: Version {1}{2}", designerAssemblyName.Name, designerAssemblyName.Version, Environment.NewLine);
				msg += string.Format("{0}: Version {1}{2}", coreAssemblyName.Name, coreAssemblyName.Version, Environment.NewLine);
				msg += string.Format("{0}: Version {1}{2}", uiAssemblyName.Name, uiAssemblyName.Version, Environment.NewLine);
				MessageBox.Show(this, msg, "Assembly Version Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}


		private void SetToolTipTexts()
		{
			statusStrip.ShowItemToolTips = true;

			statusLabelMessage.ToolTipText = "";

			toolStripStatusLabelShapes.ToolTipText =
			statusLabelShapeCount.ToolTipText = "Количество объектов на текущей диаграмме";

			toolStripStatusLabelSelection.ToolTipText = "Position and size of the current selection";
			statusLabelMousePosition.ToolTipText = "Mouse position in diagram coordinates";
			statusLabelSelectionSize.ToolTipText = "The size of the current selection in diagram coordinates";

			toolStripStatusLabelDisplayArea.ToolTipText = "The currently displayed area in diagram coordinates";
			statusLabelTopLeft.ToolTipText = "The top left corner of currently displayed area in diagram coordinates";
			statusLabelBottomRight.ToolTipText = "The bottom right corner of currently displayed area in diagram coordinates";
		}


		#region [Private] Methods: ConfigFile, Project and Store

		private XmlReader OpenCfgReader(string filePath)
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.CloseInput = true;
			return XmlReader.Create(filePath, settings);
		}


		private XmlWriter OpenCfgWriter(string filePath)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.CloseOutput = true;
			settings.Indent = true;
			return XmlWriter.Create(filePath, settings);
		}


		private void CreateConfigFile(string filePath)
		{
			// Create config file and basic nodes
			XmlWriter cfgWriter = OpenCfgWriter(filePath);
			cfgWriter.WriteStartDocument();
			cfgWriter.WriteStartElement(NodeNameSettings);
			// Setting "ProjectDirectory"
			cfgWriter.WriteStartElement(NodeNameProjectDirectory);
			cfgWriter.WriteEndElement();
			// Setting "LoadToolbarSettings"
			cfgWriter.WriteStartElement(NodeNameLoadToolbarSettings);
			cfgWriter.WriteEndElement();
			// Setting "Recent Projects"
			cfgWriter.WriteStartElement(NodeNameProjects);
			cfgWriter.WriteEndElement();
			// Setting "Window Settings"
			cfgWriter.WriteStartElement(NodeNameWindowSettings);
			cfgWriter.WriteEndElement();
			cfgWriter.Close();
		}


		private XmlNode CreateConfigNode(XmlDocument xmlDoc, XmlNode parent, string nodeName)
		{
			XmlNode result = xmlDoc.CreateNode(XmlNodeType.Element, nodeName, xmlDoc.NamespaceURI);
			if(parent != null)
				parent.AppendChild(result);
			else xmlDoc.AppendChild(result);
			return result;
		}


		private XmlDocument ReadConfigFile()
		{
			XmlDocument result = new XmlDocument();
			string filePath = Path.Combine(configFolder, ConfigFileName);
			if(File.Exists(filePath))
			{
				XmlReader cfgReader = OpenCfgReader(filePath);

				result.Load(cfgReader);
				cfgReader.Close();
			}
			return result;
		}


		private void ReadConfig(XmlDocument xmlDoc)
		{
			// Read recently used directory for XML repositories
			XmlNode projectDirNode = xmlDoc.SelectSingleNode(string.Format(QueryNodeAttr, NodeNameProjectDirectory, AttrNamePath));
			if(projectDirNode != null) xmlStoreDirectory = projectDirNode.Attributes[AttrNamePath].Value;

			// Read recently used directory for XML repositories
			XmlNode loadToolbarsNode = xmlDoc.SelectSingleNode(string.Format(QueryNodeAttr, NodeNameLoadToolbarSettings, AttrNameValue));
			if(loadToolbarsNode != null) loadToolStripLayoutOnStartup = bool.Parse(loadToolbarsNode.Attributes[AttrNameValue].Value);

			// Read recently opened projects
			foreach(XmlNode xmlNode in xmlDoc.SelectNodes(string.Format(QueryNode, NodeNameProject)))
			{
				try
				{
					RepositoryInfo repositoryInfo = RepositoryInfo.Empty;
					repositoryInfo.projectName = xmlNode.Attributes[AttrNameName].Value;
					repositoryInfo.typeName = xmlNode.Attributes[AttrNameRepositoryType].Value;
					repositoryInfo.computerName = xmlNode.Attributes[AttrNameServerName].Value;
					repositoryInfo.location = xmlNode.Attributes[AttrNameDataSource].Value;
					if(repositoryInfo != RepositoryInfo.Empty && !recentProjects.Contains(repositoryInfo))
					{
						if(recentProjects.Count == recentProjectsItemCount)
							recentProjects.RemoveAt(0);
						recentProjects.Add(repositoryInfo);
					}
				}
				catch(Exception exc)
				{
					Debug.Fail(exc.Message);
				}
			}
		}


		private void ReadWindowConfig(XmlDocument xmlDoc)
		{
			// Load ToolStrip Positions only if a config was saved before
			if(loadToolStripLayoutOnStartup) ToolStripManager.LoadSettings(this, GetToolStripSettingsName());
			else loadToolStripLayoutOnStartup = true;   // Load next time

			// Read window settings
			XmlNode wndSettingsNode = xmlDoc.SelectSingleNode(string.Format(QueryNode, NodeNameWindowSettings));
			if(wndSettingsNode != null && wndSettingsNode.Attributes.Count > 0)
			{
				try
				{
					int val;
					if(int.TryParse(wndSettingsNode.Attributes[AttrNamePositionX].Value, out val)) Left = val;
					if(int.TryParse(wndSettingsNode.Attributes[AttrNamePositionY].Value, out val)) Top = val;
					if(int.TryParse(wndSettingsNode.Attributes[AttrNameWidth].Value, out val)) Width = val;
					if(int.TryParse(wndSettingsNode.Attributes[AttrNameHeight].Value, out val)) Height = val;
					WindowState = (FormWindowState)int.Parse(wndSettingsNode.Attributes[AttrNameState].Value);
				}
				catch(Exception exc)
				{
					Debug.Fail(exc.Message);
				}
			}
		}


		private void SaveConfigFile()
		{
			// Save ToolStrip Positions
			try
			{
				ToolStripManager.SaveSettings(this, GetToolStripSettingsName());

				// Create a new config file if it does not exist
				string filePath = Path.Combine(configFolder, ConfigFileName);
				if(!File.Exists(filePath)) CreateConfigFile(filePath);

				XmlDocument xmlDoc = new XmlDocument();
				using(XmlReader cfgReader = OpenCfgReader(filePath))
				{
					xmlDoc.Load(cfgReader);
					cfgReader.Close();
				}

				XmlNode settingsNode = xmlDoc.SelectSingleNode(string.Format(QueryNode, NodeNameSettings));
				Debug.Assert(settingsNode != null);

				// Find the "Project Directory" node
				XmlNode projectDirectoryNode = xmlDoc.SelectSingleNode(string.Format(QueryNode, NodeNameProjectDirectory));
				if(projectDirectoryNode == null)
					projectDirectoryNode = CreateConfigNode(xmlDoc, settingsNode, NodeNameProjectDirectory);
				// Save last project directory
				projectDirectoryNode.RemoveAll();
				projectDirectoryNode.Attributes.Append(xmlDoc.CreateAttribute(AttrNamePath)).Value = xmlStoreDirectory;

				// Find the "Load Toolbar Settings" node
				XmlNode loadToolbarSettingsNode = xmlDoc.SelectSingleNode(string.Format(QueryNode, NodeNameLoadToolbarSettings));
				if(loadToolbarSettingsNode == null)
					loadToolbarSettingsNode = CreateConfigNode(xmlDoc, settingsNode, NodeNameLoadToolbarSettings);
				// Save last project directory
				loadToolbarSettingsNode.RemoveAll();
				loadToolbarSettingsNode.Attributes.Append(xmlDoc.CreateAttribute(AttrNameValue)).Value = loadToolStripLayoutOnStartup.ToString();

				// Find the "Projects" node
				XmlNode repositoriesNode = xmlDoc.SelectSingleNode(string.Format(QueryNode, NodeNameProjects));
				Debug.Assert(repositoriesNode != null);
				// Save all recent projects
				repositoriesNode.RemoveAll();
				foreach(RepositoryInfo projectInfo in recentProjects)
				{
					XmlNode newNode = xmlDoc.CreateNode(XmlNodeType.Element, NodeNameProject, xmlDoc.NamespaceURI);
					newNode.Attributes.Append(xmlDoc.CreateAttribute(AttrNameName)).Value = projectInfo.projectName;
					newNode.Attributes.Append(xmlDoc.CreateAttribute(AttrNameRepositoryType)).Value = projectInfo.typeName;
					newNode.Attributes.Append(xmlDoc.CreateAttribute(AttrNameServerName)).Value = projectInfo.computerName;
					newNode.Attributes.Append(xmlDoc.CreateAttribute(AttrNameDataSource)).Value = projectInfo.location;
					repositoriesNode.AppendChild(newNode);
				}

				// Find "WindowSettings" node
				XmlNode wndSettingsNode = xmlDoc.SelectSingleNode(string.Format(QueryNode, NodeNameWindowSettings));
				Debug.Assert(wndSettingsNode != null);
				if(wndSettingsNode.Attributes.Count == 0)
				{
					wndSettingsNode.Attributes.Append(xmlDoc.CreateAttribute(AttrNamePositionX));
					wndSettingsNode.Attributes.Append(xmlDoc.CreateAttribute(AttrNamePositionY));
					wndSettingsNode.Attributes.Append(xmlDoc.CreateAttribute(AttrNameWidth));
					wndSettingsNode.Attributes.Append(xmlDoc.CreateAttribute(AttrNameHeight));
					wndSettingsNode.Attributes.Append(xmlDoc.CreateAttribute(AttrNameState));
				}
				switch(WindowState)
				{
					case FormWindowState.Maximized:
					case FormWindowState.Minimized:
						wndSettingsNode.Attributes[AttrNameState].Value = ((int)FormWindowState.Maximized).ToString();
						break;
					case FormWindowState.Normal:
						wndSettingsNode.Attributes[AttrNamePositionX].Value = Left.ToString();
						wndSettingsNode.Attributes[AttrNamePositionY].Value = Top.ToString();
						wndSettingsNode.Attributes[AttrNameWidth].Value = Width.ToString();
						wndSettingsNode.Attributes[AttrNameHeight].Value = Height.ToString();
						wndSettingsNode.Attributes[AttrNameState].Value = ((int)FormWindowState.Normal).ToString();
						break;
				}

				// Save to file
				using(XmlWriter cfgWriter = OpenCfgWriter(filePath))
				{
					xmlDoc.Save(cfgWriter);
					cfgWriter.Close();
				}
			}
			catch(System.Configuration.ConfigurationException exc)
			{
				// This kind of exception is thrown when multiple instances of the NShape designer are 
				// closed with the "Close Group" command and all instances try to save their config. 
				// As Merging the changes of each instance is too much overhead here, the first instance 
				// that opened the config files may write its changes, all other changes are discarded.
				Debug.Print(exc.Message);
			}
			catch(IOException exc)
			{
				// See comment above (this exception deals with the XML config file)
				Debug.Print(exc.Message);
			}
		}


		private string GetToolStripSettingsName()
		{
			return string.Format("{0} {1}", this.Name, ProductVersion);
		}


		private void MaintainRecentProjects()
		{
			// Check existence of all recently opened (XML) projects 
			List<int> missingProjects = null;
			for(int i = recentProjects.Count - 1; i >= 0; --i)
			{
				if(recentProjects[i].typeName == RepositoryInfo.SqlServerStoreTypeName)
					continue;
				else if(recentProjects[i].typeName == RepositoryInfo.XmlStoreTypeName)
				{
					if(!File.Exists(recentProjects[i].location))
					{
						// Add all indexes of missing projects, decide later what to to
						if(missingProjects == null) missingProjects = new List<int>();
						missingProjects.Insert(0, i);
					}
				}
			}
			// If projects are missing, let the user decide what to do
			if(missingProjects != null)
			{
				Debug.Assert(missingProjects.Count > 0);
				// We use the DialogResult as decision for the 'remove all?' question:
				// Yes = Ask for each missing project
				// No = Remove all
				// Cancel = Remove none
				DialogResult res = DialogResult.Cancel;
				if(missingProjects.Count > 1)
				{
					const string msgText = "{0} recently opened project files were not found:{1}{2}{1}Do you want to decide for each project whether to remove it from the 'Recently opened projects' list?";
					string projectList = string.Empty;
					foreach(int i in missingProjects)
						projectList += recentProjects[i].location + Environment.NewLine;
					res = MessageBox.Show(this, string.Format(msgText, missingProjects.Count, Environment.NewLine, projectList), "File not found", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				}
				else
				{
					// Preselect "Ask for each missin project" if only one project is missing
					res = DialogResult.Yes;
				}

				if(res != DialogResult.Cancel)
				{
					if(res == DialogResult.Yes)
					{
						// Ask for each missing projects if it should be removed
						string msgFormat = "A recently opened project file was not found:{0}{1}{0}{0}Do you want to remove it from the 'Recently opened projects' list?";
						for(int i = missingProjects.Count - 1; i >= 0; --i)
						{
							int prjIdx = missingProjects[i];
							res = MessageBox.Show(this, string.Format(msgFormat, Environment.NewLine, recentProjects[prjIdx].location), "File not found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
							if(res == DialogResult.No)
								missingProjects.RemoveAt(i);
						}
					}
					// Remove all (remaining) missing projects
					if(missingProjects.Count > 0)
					{
						for(int i = missingProjects.Count - 1; i >= 0; --i)
							recentProjects.RemoveAt(missingProjects[i]);
						SaveConfigFile();
					}
				}
			}
		}


		// Old version:
		//private void MaintainRecentProjects() {
		//    bool modified = false, remove = false;
		//    for (int i = recentProjects.Count - 1; i >= 0; --i) {
		//        remove = false;
		//        if (recentProjects[i].typeName == RepositoryInfo.SqlServerStoreTypeName)
		//            continue;
		//        else if (recentProjects[i].typeName == RepositoryInfo.XmlStoreTypeName) {
		//            if (!File.Exists(recentProjects[i].location)) {
		//                string msgFormat = "The file or folder '{0}' cannot be opened. Do you want to remove it from the 'Recently opened projects' list?";
		//                remove = (MessageBox.Show(this, string.Format(msgFormat, recentProjects[i].location), "File not found", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
		//            }
		//        }
		//        if (remove) {
		//            recentProjects.RemoveAt(i);
		//            modified = true;
		//        }
		//    }
		//    if (modified) SaveConfigFile();
		//}


		private void ReplaceStore(string projectName, Store store)
		{
			UnregisterRepositoryEvents();
			project.Name = projectName;
			((CachedRepository)project.Repository).Store = store;
			if(store is XmlStore)
			{
				XmlStore xmlStore = (XmlStore)store;
				xmlStore.BackupFileExtension = XmlStore.DefaultBackupFileExtension;
				xmlStore.BackupGenerationMode = XmlStore.BackupFileGenerationMode.BakFile;
			}
			RegisterRepositoryEvents();
		}


		private void PrependRecentProjectsMenuItem(RepositoryInfo projectInfo)
		{
			ToolStripItem item = new ToolStripMenuItem(projectInfo.projectName);
			item.Tag = projectInfo;
			item.ToolTipText = string.Format("Project: {0}{3}Location: {1}{3}Repository Type: {2}", projectInfo.projectName, projectInfo.location, projectInfo.typeName, Environment.NewLine);
			item.Click += openRecentProjectMenuItem_Click;
			recentProjectsMenuItem.DropDownItems.Insert(0, item);
			if(!recentProjectsMenuItem.Visible) recentProjectsMenuItem.Visible = true;
		}


		private void CreateRecentProjectsMenuItems()
		{
			ClearRecentProjectsMenu();
			recentProjectsMenuItem.Visible = (recentProjects.Count > 0);
			foreach(RepositoryInfo recentProject in recentProjects)
				PrependRecentProjectsMenuItem(recentProject);
		}


		private RepositoryInfo GetReposistoryInfo(Project project)
		{
			RepositoryInfo projectInfo = RepositoryInfo.Empty;
			projectInfo.projectName = project.Name;
			Store store = ((CachedRepository)project.Repository).Store;
			if(store is XmlStore)
			{
				projectInfo.typeName = RepositoryInfo.XmlStoreTypeName;
				string filePath = ((XmlStore)store).ProjectFilePath;
				projectInfo.location = filePath;
				projectInfo.computerName = Environment.MachineName;
			}
			else if(store is SqlStore)
			{
				projectInfo.typeName = RepositoryInfo.SqlServerStoreTypeName;
				projectInfo.location = ((SqlStore)store).DatabaseName;
				projectInfo.computerName = ((SqlStore)store).ServerName;
			}
			else Debug.Fail("Unexpected repository type");
			return projectInfo;
		}


		private bool AddToRecentProjects(Project project)
		{
			return AddToRecentProjects(GetReposistoryInfo(project));
		}


		private bool AddToRecentProjects(RepositoryInfo projectInfo)
		{
			// Check if the project already exists in the recent projects list
			foreach(RepositoryInfo recentProject in recentProjects)
				if(recentProject == projectInfo) return false;
			// If it does not, add it and create a new menu item
			if(recentProjects.Count == recentProjectsItemCount)
				recentProjects.RemoveAt(0);
			recentProjects.Add(projectInfo);
			PrependRecentProjectsMenuItem(projectInfo);
			SaveConfigFile();
			return true;
		}


		private bool RemoveFromRecentProjects(Project project)
		{
			return RemoveFromRecentProjects(GetReposistoryInfo(project));
		}


		private bool RemoveFromRecentProjects(RepositoryInfo projectInfo)
		{
			return recentProjects.Remove(projectInfo);
		}


		private void UpdateRecentProjectsMenu()
		{
			ClearRecentProjectsMenu();
			foreach(RepositoryInfo pi in recentProjects)
				PrependRecentProjectsMenuItem(pi);
		}


		private void ClearRecentProjectsMenu()
		{
			for(int i = recentProjectsMenuItem.DropDownItems.Count - 1; i >= 0; --i)
			{
				recentProjectsMenuItem.DropDownItems[i].Click -= openRecentProjectMenuItem_Click;
				recentProjectsMenuItem.DropDownItems[i].Dispose();
			}
			recentProjectsMenuItem.DropDownItems.Clear();
		}


		private void CreateProject(string projectName, Store store, bool askUserLoadLibraries)
		{
			ReplaceStore(projectName, store);
			project.Create();
			//---------------------------------------------------------------------------
			// Стили
			ColorStyleCollection colorStyles = project.Design.ColorStyles;
			ColorStyle colorStyle;
			colorStyle = new ColorStyle("Orange", Color.Orange);	colorStyles.Add(colorStyle, colorStyle);
			colorStyle = new ColorStyle("Gold", Color.Gold);		colorStyles.Add(colorStyle, colorStyle);

			FillStyleCollection fillStyles = project.Design.FillStyles;
			((FillStyle)fillStyles.Transparent).FillMode = FillMode.Solid;
			((FillStyle)fillStyles.White      ).FillMode = FillMode.Solid;
			((FillStyle)fillStyles.Black      ).FillMode = FillMode.Solid;
			((FillStyle)fillStyles.Red        ).FillMode = FillMode.Solid;
			((FillStyle)fillStyles.Blue       ).FillMode = FillMode.Solid;
			((FillStyle)fillStyles.Green      ).FillMode = FillMode.Solid;
			((FillStyle)fillStyles.Yellow     ).FillMode = FillMode.Solid;

			IColorStyle iColorStyle, additionalColorStyle = colorStyles.White;
			FillStyle fillStyle;
			iColorStyle = colorStyles.LightYellow;	fillStyle = new FillStyle(iColorStyle.Name, iColorStyle, additionalColorStyle);	fillStyle.FillMode = FillMode.Solid;	fillStyles.Add(fillStyle, fillStyle);
			iColorStyle = colorStyles.Gray;			fillStyle = new FillStyle(iColorStyle.Name, iColorStyle, additionalColorStyle);	fillStyle.FillMode = FillMode.Solid;	fillStyles.Add(fillStyle, fillStyle);
			iColorStyle = colorStyles.LightRed;		fillStyle = new FillStyle(iColorStyle.Name, iColorStyle, additionalColorStyle);	fillStyle.FillMode = FillMode.Solid;	fillStyles.Add(fillStyle, fillStyle);
			iColorStyle = colorStyles.LightBlue;	fillStyle = new FillStyle(iColorStyle.Name, iColorStyle, additionalColorStyle);	fillStyle.FillMode = FillMode.Solid;	fillStyles.Add(fillStyle, fillStyle);
			iColorStyle = colorStyles.LightGreen;	fillStyle = new FillStyle(iColorStyle.Name, iColorStyle, additionalColorStyle);	fillStyle.FillMode = FillMode.Solid;	fillStyles.Add(fillStyle, fillStyle);
			iColorStyle = colorStyles["Orange"];	fillStyle = new FillStyle(iColorStyle.Name, iColorStyle, additionalColorStyle);	fillStyle.FillMode = FillMode.Solid;	fillStyles.Add(fillStyle, fillStyle);
			iColorStyle = colorStyles["Gold"];		fillStyle = new FillStyle(iColorStyle.Name, iColorStyle, additionalColorStyle);	fillStyle.FillMode = FillMode.Solid;	fillStyles.Add(fillStyle, fillStyle);

			CapStyleCollection capStyles = project.Design.CapStyles;
			CapStyle capStyle;
			capStyle = new CapStyle("WindStart", CapShape.CenteredHalfCircle, colorStyles.Black);	capStyle.CapSize = 6;	capStyles.Add(capStyle, capStyle);
			capStyle = new CapStyle("WindEnd",   CapShape.OpenArrow,          colorStyles.Black);	capStyle.CapSize = 6;	capStyles.Add(capStyle, capStyle);

			LineStyleCollection lineStyles = project.Design.LineStyles;
			LineStyle lineStyle;
			lineStyle = new LineStyle("LightYellow",  lineStyles.Normal.LineWidth, colorStyles["LightYellow"]);	lineStyles.Add(lineStyle, lineStyle);
			lineStyle = new LineStyle("Gray",         lineStyles.Normal.LineWidth, colorStyles["Gray"       ]);	lineStyles.Add(lineStyle, lineStyle);
			lineStyle = new LineStyle("LightRed",     lineStyles.Normal.LineWidth, colorStyles["LightRed"   ]);	lineStyles.Add(lineStyle, lineStyle);
			lineStyle = new LineStyle("LightBlue",    lineStyles.Normal.LineWidth, colorStyles["LightBlue"  ]);	lineStyles.Add(lineStyle, lineStyle);
			lineStyle = new LineStyle("LightGreen",   lineStyles.Normal.LineWidth, colorStyles["LightGreen" ]);	lineStyles.Add(lineStyle, lineStyle);
			lineStyle = new LineStyle("Orange",       lineStyles.Normal.LineWidth, colorStyles["Orange"     ]);	lineStyles.Add(lineStyle, lineStyle);
			lineStyle = new LineStyle("Gold",         lineStyles.Normal.LineWidth, colorStyles["Gold"       ]);	lineStyles.Add(lineStyle, lineStyle);
			lineStyle = new LineStyle("WindNord",     lineStyles.Thick .LineWidth, colorStyles.Blue          );	lineStyles.Add(lineStyle, lineStyle);
			lineStyle = new LineStyle("WindSouth",    lineStyles.Thick .LineWidth, colorStyles.Red           );	lineStyles.Add(lineStyle, lineStyle);
			//---------------------------------------------------------------------------
			projectSaved = false;
			DisplayDiagrams();
			if(askUserLoadLibraries)
				CheckLibrariesLoaded();
			// Adjust menu items
			saveMenuItem.Enabled = true;
			saveAsMenuItem.Enabled = true;
			upgradeVersionMenuItem.Enabled = false;
			if(store is XmlStore || store == null)
			{
				useEmbeddedImagesToolStripMenuItem.Enabled = true;
				if(store != null)
					useEmbeddedImagesToolStripMenuItem.Checked = (((XmlStore)store).ImageLocation == XmlStore.ImageFileLocation.Embedded);
			}
			else useEmbeddedImagesToolStripMenuItem.Enabled = false;
		}


		private void CheckLibrariesLoaded()
		{
			bool librariesLoaded = false;
			foreach(Assembly a in project.Libraries)
			{
				librariesLoaded = true;
				break;
			}
			if(!librariesLoaded)
			{
				if(MessageBox.Show(this, "Do you want to load shape libraries now?", "Load shape libraries",
					MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					using(LibraryManagementDialog dlg = new LibraryManagementDialog(project))
						dlg.ShowDialog(this);
				}
			}
		}


		private void OpenProject(string projectName, Store repository)
		{
			Cursor = Cursors.WaitCursor;
			Application.DoEvents();
			string errorMessage = null;
			try
			{
				ReplaceStore(projectName, repository);
				project.Open();
				DisplayDiagrams();

				projectSaved = true;
				// Move project on top of the recent projects list 
				RepositoryInfo repositoryInfo = GetReposistoryInfo(project);
				RemoveFromRecentProjects(repositoryInfo);
				AddToRecentProjects(repositoryInfo);
				UpdateRecentProjectsMenu();
			}
			catch(LoadLibraryException exc)
			{
				errorMessage = string.Format("Loading library '{0}' failed", exc.AssemblyName);
				errorMessage += (exc.InnerException != null) ? string.Format(": {0}{0}{1}", Environment.NewLine, exc.InnerException.Message) : ".";
			}
			catch(Exception exc)
			{
				errorMessage = exc.Message;
			}
			finally
			{
				if(errorMessage != null)
				{
					MessageBox.Show(this, errorMessage, "Error while opening Repository.", MessageBoxButtons.OK, MessageBoxIcon.Error);
					project.Close();
				}
				Cursor = Cursors.Default;
			}
		}


		private bool SaveProject()
		{
			bool result = false;
			if(!projectSaved || !project.Repository.Exists())
				result = SaveProjectAs();
			else result = DoSaveProject();
			return result;
		}


		private bool SaveProjectAs()
		{
			bool result = false;
			CachedRepository cachedRepository = (CachedRepository)project.Repository;
			bool isNewstore = false;

			// If there is no store, create an XmlStore
			if(cachedRepository.Store == null)
			{
				// Get default storage location
				if(!Directory.Exists(xmlStoreDirectory))
				{
					StringBuilder path = new StringBuilder(512);
					const int COMMON_DOCUMENTS = 0x002e;
					if(SHGetSpecialFolderPath(IntPtr.Zero, path, COMMON_DOCUMENTS, 0) != 0)
						xmlStoreDirectory = Path.Combine(Path.Combine(path.ToString(), "NShape"), "Demo Projects");
					else xmlStoreDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				}

				XmlStore xmlStore = new XmlStore()
				{
					DirectoryName = xmlStoreDirectory,
					FileExtension = ProjectFileExtension,
					LazyLoading = true
				};
				ReplaceStore(cachedRepository.ProjectName, xmlStore);
				isNewstore = true;
			}

			// Save store as...
			if(cachedRepository.Store is XmlStore)
			{
				XmlStore xmlStore = (XmlStore)cachedRepository.Store;

				// Select a file name
				saveFileDialog.CreatePrompt = false;        // Do not ask whether to create the file
				saveFileDialog.CheckFileExists = false;     // Do not check whether the file does NOT exist
				saveFileDialog.CheckPathExists = true;      // Ask whether to overwrite existing file
				saveFileDialog.Filter = FileFilterXmlRepository;
				if(Directory.Exists(xmlStore.DirectoryName))
					saveFileDialog.InitialDirectory = xmlStore.DirectoryName;
				else
					saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				saveFileDialog.FileName = Path.GetFileName(xmlStore.ProjectFilePath);

				// Try to save repository to file...
				if(saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					if(xmlStore.ProjectFilePath != saveFileDialog.FileName)
					{
						project.Name = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
						xmlStore.DirectoryName = Path.GetDirectoryName(saveFileDialog.FileName);
						Text = string.Format("{0} - {1}", Properties.Strings.AppTitle, project.Name);
					}
					// Delete file if it exists, because the user was prompted whether to overwrite it before (SaveFileDialog.CheckPathExists).
					if(project.Repository.Exists())
						project.Repository.Erase();
					// If the XmlStore was freshly created, call create in order to create the store media and (internally) open the store
					if(isNewstore)
						xmlStore.Create(cachedRepository);
					xmlStore.ImageLocation = XmlImageFileLocation;

					saveMenuItem.Enabled = true;
					result = DoSaveProject();
				}
			}
			else if(cachedRepository.Store is AdoNetStore)
			{
				// Save repository to database because the database and the project name are 
				// selected before creating the project when using AdoNet stores
				saveMenuItem.Enabled = true;
				result = DoSaveProject();
			}
			else
			{
				if(cachedRepository.Store == null)
					MessageBox.Show(this, "There is no store component attached to the repository.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
				{
					string msg = string.Format("Unsupported store type: '{0}'.", cachedRepository.Store.GetType().Name);
					MessageBox.Show(this, msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			return result;
		}


		private bool DoSaveProject()
		{
			bool result = false;
			this.Cursor = Cursors.WaitCursor;
			Application.DoEvents();
			try
			{
				if(cachedRepository.Store is XmlStore)
				{
					// Ensure that all diagrams are loaded completely for XmlStores with LazyLoading enabled
					foreach(Diagram d in project.Repository.GetDiagrams())
						project.Repository.GetDiagramShapes(d);
				}

				// Close open caption editor before saving
				if(CurrentDisplay != null)
					((IDiagramPresenter)CurrentDisplay).CloseCaptionEditor(true);

				// Save changes to file / database
				project.Repository.SaveChanges();
				projectSaved = true;

				// Add project to "Recent Projects" list
				RepositoryInfo projectInfo = GetReposistoryInfo(project);
				RemoveFromRecentProjects(projectInfo);
				AddToRecentProjects(projectInfo);
				UpdateRecentProjectsMenu();
				result = true;
			}
			catch(IOException exc)
			{
				MessageBox.Show(this, exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch(Exception exc)
			{
				MessageBox.Show(this, exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
			return result;
		}


		private bool CloseProject()
		{
			bool result = true;
			// We check not only the Repository's IsModified property but also the projectIsModified field
			// in order to suppress the "Save changes?" question when closing a new project with loaded 
			// libraries (which modify the repository)
			if(projectIsModified && project.Repository.IsModified)
			{
				string msg = string.Format("Do you want to save the current project '{0}' before closing it?", project.Name);
				DialogResult dlgResult = MessageBox.Show(this, msg, "Save changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch(dlgResult)
				{
					case DialogResult.Yes:
						if(project.Repository.Exists())
							DoSaveProject();
						else SaveProjectAs();
						break;
					case DialogResult.No:
						// do nothing
						break;
					case DialogResult.Cancel:
						result = false;
						break;
				}
			}

			if(result)
			{
				project.Close();
				projectSaved = false;
				// Clear all displays and diagramControllers
				for(int i = displayTabControl.TabPages.Count - 1; i >= 0; --i)
				{
					DisplayTabPage displayTabPage = displayTabControl.TabPages[i] as DisplayTabPage;
					displayTabControl.TabPages.RemoveAt(i);
					if(displayTabPage != null)
					{
						UnregisterDisplayEvents(displayTabPage.Display);
						displayTabPage.Dispose();
					}
				}
			}

			return result;
		}


		private XmlStore.ImageFileLocation XmlImageFileLocation
		{
			get
			{
				return useEmbeddedImagesToolStripMenuItem.Checked ? XmlStore.ImageFileLocation.Embedded : XmlStore.ImageFileLocation.Directory;
			}
		}


		private void ImportRepositoryFromClipboard()
		{
			try
			{
				const StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;
				string xmlString = Clipboard.GetText();
				// We insist on UTF-8 encoding because of the conversion into a byte array which must use a defined encoding
				if(!xmlString.StartsWith("<?xml", comparison) || xmlString.IndexOf("encoding=\"utf-8\"?>", comparison) < 0)
					MessageBox.Show(this, "Clipboard does not contain valid UTF-8 encoded XML text.", "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
				else if(xmlString.IndexOf("<dataweb_nshape version=", comparison) < 0)
					MessageBox.Show(this, "Clipboard does not contain valid XML repository data.", "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
				{
					if(project.IsOpen)
					{
						DialogResult res = MessageBox.Show("Importing XML from Clipboard requires the project to be closed. Do you want to close the current project now?",
							"Close Project?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
						if(res == DialogResult.No)
							return;
						if(!CloseProject())
							return;
					}

					// Delete current repository
					project.Repository = null;
					project.Name = Properties.Strings.NewProjectName;

					using(MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
						project.ReadXml(memStream);
					// Assign the new Repository to the main form's cachedRepository component
					cachedRepository = (CachedRepository)project.Repository;

					// Display the result
					DisplayDiagrams();
				}
			}
			catch(Exception exc)
			{
				MessageBox.Show(this, exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


		private void ExportRepositoryToClipboard()
		{
			try
			{
				if(project.Repository is CachedRepository && project.Repository.IsOpen)
				{
					string xmlString = project.GetXml();
					Clipboard.SetText(xmlString);
				}
			}
			catch(Exception exc)
			{
				MessageBox.Show(this, exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#endregion


		#region [Private] Methods: Update Controls states and visibility

		private void DisplayMouseCoordinates(int x, int y)
		{
			statusLabelMousePosition.Text = string.Format(PointFormatStr, x, y);
		}


		private void UpdateStatusInfo()
		{
			Point mousePos = Control.MousePosition;
			Size selectionSize = Size.Empty;
			Rectangle bounds = Rectangle.Empty;
			int shapeCnt = 0, relationCnt = 0, resourceCnt = 0, propertyCnt = 0, actionCnt = 0;
			if(CurrentDisplay != null && CurrentDisplay.Diagram != null)
			{
				CurrentDisplay.ControlToDiagram(CurrentDisplay.DrawBounds, out bounds);
				//diagramSize = currentDisplay.Diagram.Size;
				//shapeCnt = CurrentDisplay.Diagram.Shapes.Count;
				foreach(Shape shape in CurrentDisplay.Diagram.Shapes)
				{
					if(shape.IsToolbarHidden)
						continue;

					++shapeCnt;
					if(shape is ShapeGroup)
						++relationCnt;
					else if(shape.Type.LibraryName == "FhisShapes")
						switch(shape.Type.Name)
						{
							case "Ресурс":
								++resourceCnt;
								break;
							case "Свойство":
								++propertyCnt;
								break;
							case "Действие":
								++actionCnt;
								break;
						}
				}

				if(CurrentDisplay.SelectedShapes.Count > 0)
					selectionSize = CurrentDisplay.SelectedShapes.GetBoundingRectangle(true).Size;
			}
			statusLabelTopLeft.Text = string.Format(PointFormatStr, bounds.Left, bounds.Top);
			statusLabelBottomRight.Text = string.Format(PointFormatStr, bounds.Right, bounds.Bottom);
			statusLabelMousePosition.Text = string.Format(PointFormatStr, mousePos.X, mousePos.X);
			statusLabelSelectionSize.Text = string.Format(SizeFormatStr, selectionSize.Width, selectionSize.Height);
			statusLabelShapeCount.Text = shapeCnt.ToString();
			statusLabelResourceCount.Text = resourceCnt.ToString();
			statusLabelPropertyCount.Text = propertyCnt.ToString();
			statusLabelActionCount.Text = actionCnt.ToString();
			statusLabelRelationCount.Text = relationCnt.ToString();
		}


		private void UpdateAllMenuItems()
		{
			upgradeVersionMenuItem.Enabled = false;
			if(!project.IsOpen)
				upgradeVersionMenuItem.ToolTipText = "No project opened.";
			else if(!project.Repository.CanModifyVersion)
				upgradeVersionMenuItem.ToolTipText = "The current repository does not support upgrading storage version.";
			else if(project.Repository.Version >= maxRepositoryVersion)
				upgradeVersionMenuItem.ToolTipText = "The repository storage version is up to date.";
			else
			{
				upgradeVersionMenuItem.Enabled = true;
				upgradeVersionMenuItem.ToolTipText = string.Format("Upgrade repository storage version from {0} to {1}.",
					project.Repository.Version, maxRepositoryVersion);
			}

			if(cachedRepository.Store is XmlStore || cachedRepository.Store == null)
			{
				useEmbeddedImagesToolStripMenuItem.Enabled = true;
				if(cachedRepository.Store != null)
					useEmbeddedImagesToolStripMenuItem.Checked = (((XmlStore)cachedRepository.Store).ImageLocation == XmlStore.ImageFileLocation.Embedded);
				useEmbeddedImagesToolStripMenuItem.ToolTipText = "Embed images into the storage file instead of saving them to a seperate image directory.";
			}
			else
			{
				useEmbeddedImagesToolStripMenuItem.Enabled = false;
				useEmbeddedImagesToolStripMenuItem.Checked = false;
				useEmbeddedImagesToolStripMenuItem.ToolTipText = "The current repository does not support embedding images.";
			}

			UpdateEditMenuItems();
			UpdateUndoMenuItems();
		}


		private void UpdateUndoMenuItems()
		{
			// Undo / Redo
			undoToolStripSplitButton.Enabled =
			undoMenuItem.Enabled = diagramSetController.Project.History.UndoCommandCount > 0;
			redoToolStripSplitButton.Enabled =
			redoMenuItem.Enabled = diagramSetController.Project.History.RedoCommandCount > 0;
		}


		private void UpdateEditMenuItems()
		{
			bool shapesOnly, shapesAndModels;
			if(CurrentDisplay != null && CurrentDisplay.Diagram != null)
			{
				// Cut
				shapesOnly = shapesAndModels = CurrentDisplay.DiagramSetController.CanCut(CurrentDisplay.Diagram, CurrentDisplay.SelectedShapes);
				cutShapeButton.Enabled = shapesOnly;
				cutShapeOnlyMenuItem.Enabled = shapesOnly;
				cutShapeAndModelMenuItem.Enabled = shapesAndModels;
				// Copy
				shapesOnly =
				shapesAndModels = CurrentDisplay.DiagramSetController.CanCopy(CurrentDisplay.SelectedShapes);
				copyShapeButton.Enabled = shapesOnly;
				copyAsImageToolStripMenuItem.Enabled = true;
				copyShapeOnlyMenuItem.Enabled = shapesOnly;
				copyShapeAndModelMenuItem.Enabled = shapesAndModels;
				// Paste
				pasteButton.Enabled =
				pasteMenuItem.Enabled = CurrentDisplay.DiagramSetController.CanPaste(CurrentDisplay.Diagram);
				// Delete
				shapesOnly =
				shapesAndModels = CurrentDisplay.DiagramSetController.CanDeleteShapes(CurrentDisplay.Diagram, CurrentDisplay.SelectedShapes);
				deleteShapeButton.Enabled = shapesOnly;
				deleteShapeOnlyMenuItem.Enabled = shapesOnly;
				deleteShapeAndModelMenuItem.Enabled = shapesAndModels;
				// ToForeGround / ToBackground
				toForegroundMenuItem.Enabled =
				toBackgroundMenuItem.Enabled = CurrentDisplay.DiagramSetController.CanLiftShapes(CurrentDisplay.Diagram, CurrentDisplay.SelectedShapes);
				// Selection
				selectAllToolStripMenuItem.Enabled = (CurrentDisplay.Diagram.Shapes.Count > 0
													&& CurrentDisplay.SelectedShapes.Count < CurrentDisplay.Diagram.Shapes.Count);
				unselectAllToolStripMenuItem.Enabled = (CurrentDisplay.SelectedShapes.Count > 0);
				selectAllOfTypeToolStripMenuItem.Enabled = (CurrentDisplay.SelectedShapes.Count == 1);
				selectAllOfTemplateToolStripMenuItem.Enabled = (CurrentDisplay.SelectedShapes.Count == 1 && CurrentDisplay.SelectedShapes.TopMost.Template != null);
			}
			else
			{
				// Cut
				cutShapeButton.Enabled =
				cutShapeOnlyMenuItem.Enabled =
				cutShapeAndModelMenuItem.Enabled = false;
				// Copy
				shapesOnly =
				shapesAndModels =
				copyShapeButton.Enabled =
				copyAsImageToolStripMenuItem.Enabled =
				copyShapeOnlyMenuItem.Enabled =
				copyShapeAndModelMenuItem.Enabled = false;
				// Paste
				pasteButton.Enabled =
				pasteMenuItem.Enabled = false;
				// Delete
				shapesOnly =
				shapesAndModels =
				deleteShapeButton.Enabled =
				deleteShapeOnlyMenuItem.Enabled =
				deleteShapeAndModelMenuItem.Enabled = false;
				// ToForeGround / ToBackground
				toForegroundMenuItem.Enabled =
				toBackgroundMenuItem.Enabled = false;
				// Selection
				selectAllToolStripMenuItem.Enabled =
				unselectAllToolStripMenuItem.Enabled =
				selectAllOfTypeToolStripMenuItem.Enabled =
				selectAllOfTemplateToolStripMenuItem.Enabled = false;
			}
		}


		private void UpdateZoomControl()
		{
			int cursorPos = -1;
			if(zoomToolStripComboBox.Focused)
				cursorPos = zoomToolStripComboBox.SelectionStart;

			string txt = string.Format(PercentFormatStr, CurrentDisplay.ZoomLevel);
			if(txt != zoomToolStripComboBox.Text)
				zoomToolStripComboBox.Text = txt;

			if(zoomToolStripComboBox.Focused)
				zoomToolStripComboBox.SelectionStart = cursorPos;

			UpdateStatusInfo();
		}


		private void UpdateModelComponentsVisibility(bool visible)
		{
#if VIEWER
			visible = false;
			if(toolboxPropsPanel.Visible != visible) toolboxPropsPanel.Visible = visible;
#endif
			if(modelTreeView.Visible != visible) modelTreeView.Visible = visible;
			if(visible)
			{
				if(!propertyWindowTabControl.TabPages.Contains(propertyWindowModelTab))
					propertyWindowTabControl.TabPages.Insert(1, propertyWindowModelTab);
			}
			else
			{
				if(propertyWindowTabControl.TabPages.Contains(propertyWindowModelTab))
					propertyWindowTabControl.TabPages.Remove(propertyWindowModelTab);
			}
		}

		#endregion


		#region [Private] Methods: Manage Display Controls

		/// <summary>
		/// Creates a ownerDisplay for each diagram in the project and a default one if there isn't any.
		/// </summary>
		private void DisplayDiagrams()
		{
			// Display all diagramControllers of the project
			bool diagramAdded = false;
			foreach(Diagram diagram in project.Repository.GetDiagrams())
			{
				DisplayTabPage displayTabPage = CreateDiagramTabPage(diagram, !diagramAdded);
				displayTabControl.TabPages.Add(displayTabPage);
				if(!diagramAdded) diagramAdded = true;
			}
			// If the project has no diagram, create the a new one.
			if(!diagramAdded)
			{
				Diagram diagram = new Diagram(string.Format(Properties.Strings.DefaultDiagramNameFmt, displayTabControl.TabPages.Count + 1));
				project.Repository.InsertAll(diagram);
			}
			Debug.Assert(displayTabControl.TabCount > 0);
			displayTabControl.SelectedIndex = 0;
			// Call selectedIndexChanged event handler immideiatly because otherwise it would be called 
			// when this method is completed (but we need a CurrentDisplay now)
			displaysTabControl_SelectedIndexChanged(displayTabControl, EventArgs.Empty);
			showDiagramSettingsToolStripMenuItem_Click(this, EventArgs.Empty);

			UpdateAllMenuItems();
		}


		private Display CreateDiagramDisplay(Diagram diagram)
		{
			// Create a new ownerDisplay
			Display display = new Display();
			display.Name = string.Format("Display{0}", displayTabControl.TabCount + 1);
			display.BackColor = Color.DarkGray;
			display.HighQualityRendering = HighQuality;
			display.HighQualityBackground = HighQuality;
			display.IsGridVisible = ShowGrid;
			display.GridSize = GridSize;
			display.SnapToGrid = SnapToGrid;
			display.SnapDistance = SnapDistance;
			display.GripSize = ControlPointSize;
			display.ResizeGripShape = ResizePointShape;
			display.ConnectionPointShape = ConnectionPointShape;
			display.ZoomLevel = Zoom;
#if DEBUG_UI
			display.IsDebugInfoCellOccupationVisible = ShowCellOccupation;
			display.IsDebugInfoInvalidateVisible = ShowInvalidatedAreas;
#endif
			display.Dock = DockStyle.Fill;
			//
			// Assign DiagramSetController and diagram
			display.PropertyController = propertyController;
			display.DiagramSetController = diagramSetController;
			display.Diagram = diagram;
			display.ActiveTool = toolSetController.SelectedTool;
			display.UserMessage += display_UserMessage;
			//
			return display;
		}


		private DisplayTabPage CreateDiagramTabPage(Diagram diagram, bool createDisplay)
		{
			DisplayTabPage tabPage = new DisplayTabPage(diagram.Title);
			tabPage.Tag = diagram;
			if(createDisplay)
				tabPage.Display = CreateDiagramDisplay(diagram);
			return tabPage;
		}


		private void RemoveDisplayOfDiagram(Diagram diagram)
		{
			DisplayTabPage tabPage = FindDisplayTabPage(diagram);
			if(tabPage != null)
			{
				displayTabControl.TabPages.Remove(tabPage);
				tabPage.Dispose();
			}
			UpdateAllMenuItems();
		}


		private DisplayTabPage FindDisplayTabPage(Diagram diagram)
		{
			foreach(TabPage tabPage in displayTabControl.TabPages)
			{
				if(tabPage is DisplayTabPage && tabPage.Tag == diagram)
					return (DisplayTabPage)tabPage;
			}
			return null;
		}

		#endregion


		#region [Private] Methods: Register event handlers

		private void RegisterRepositoryEvents()
		{
			if(project.Repository != null)
			{
				// Diagram events
				project.Repository.DiagramInserted += repository_DiagramInserted;
				project.Repository.DiagramUpdated += repository_DiagramUpdated;
				project.Repository.DiagramDeleted += repository_DiagramDeleted;
				// ModelObject events
				project.Repository.ModelObjectsInserted += repository_ModelObjectsInsertedOrDeleted;
				project.Repository.ModelObjectsDeleted += repository_ModelObjectsInsertedOrDeleted;

				// Event handlers used for detecting changes
				project.Repository.ConnectionDeleted += Repository_ConnectionModified;
				project.Repository.ConnectionInserted += Repository_ConnectionModified;
				project.Repository.DesignDeleted += Repository_DesignModified;
				project.Repository.DesignInserted += Repository_DesignModified;
				project.Repository.DesignUpdated += Repository_DesignModified;
				project.Repository.ModelDeleted += Repository_ModelModified;
				project.Repository.ModelInserted += Repository_ModelModified;
				project.Repository.ModelUpdated += Repository_ModelModified;
				project.Repository.ModelMappingsDeleted += Repository_ModelMappingsModified;
				project.Repository.ModelMappingsInserted += Repository_ModelMappingsModified;
				project.Repository.ModelMappingsUpdated += Repository_ModelMappingsModified;
				project.Repository.ModelObjectsUpdated += Repository_ModelObjectsModified;
				project.Repository.ProjectUpdated += Repository_ProjectModified;
				project.Repository.ShapesDeleted += Repository_ShapesModified;
				project.Repository.ShapesInserted += Repository_ShapesModified;
				project.Repository.ShapesUpdated += Repository_ShapesModified;
				project.Repository.StyleDeleted += Repository_StyleModified;
				project.Repository.StyleInserted += Repository_StyleModified;
				project.Repository.StyleUpdated += Repository_StyleModified;
				project.Repository.TemplateDeleted += Repository_TemplateModified;
				project.Repository.TemplateInserted += Repository_TemplateModified;
				project.Repository.TemplateShapeReplaced += Repository_TemplateModified;
				project.Repository.TemplateUpdated += Repository_TemplateModified;
			}
		}


		private void UnregisterRepositoryEvents()
		{
			if(project.Repository != null)
			{
				// Diagram events
				project.Repository.DiagramInserted -= repository_DiagramInserted;
				project.Repository.DiagramUpdated -= repository_DiagramUpdated;
				project.Repository.DiagramDeleted -= repository_DiagramDeleted;

				// ModelObject events
				project.Repository.ModelObjectsInserted -= repository_ModelObjectsInsertedOrDeleted;
				project.Repository.ModelObjectsDeleted -= repository_ModelObjectsInsertedOrDeleted;

				// Event handlers used for detecting changes
				project.Repository.ConnectionDeleted -= Repository_ConnectionModified;
				project.Repository.ConnectionInserted -= Repository_ConnectionModified;
				project.Repository.DesignDeleted -= Repository_DesignModified;
				project.Repository.DesignInserted -= Repository_DesignModified;
				project.Repository.DesignUpdated -= Repository_DesignModified;
				project.Repository.ModelDeleted -= Repository_ModelModified;
				project.Repository.ModelInserted -= Repository_ModelModified;
				project.Repository.ModelUpdated -= Repository_ModelModified;
				project.Repository.ModelMappingsDeleted -= Repository_ModelMappingsModified;
				project.Repository.ModelMappingsInserted -= Repository_ModelMappingsModified;
				project.Repository.ModelMappingsUpdated -= Repository_ModelMappingsModified;
				project.Repository.ModelObjectsUpdated -= Repository_ModelObjectsModified;
				project.Repository.ProjectUpdated -= Repository_ProjectModified;
				project.Repository.ShapesDeleted -= Repository_ShapesModified;
				project.Repository.ShapesInserted -= Repository_ShapesModified;
				project.Repository.ShapesUpdated -= Repository_ShapesModified;
				project.Repository.StyleDeleted -= Repository_StyleModified;
				project.Repository.StyleInserted -= Repository_StyleModified;
				project.Repository.StyleUpdated -= Repository_StyleModified;
				project.Repository.TemplateDeleted -= Repository_TemplateModified;
				project.Repository.TemplateInserted -= Repository_TemplateModified;
				project.Repository.TemplateShapeReplaced -= Repository_TemplateModified;
				project.Repository.TemplateUpdated -= Repository_TemplateModified;
			}
		}


		private void RegisterDisplayEvents(Display display)
		{
			if(display != null)
			{
				display.Scroll += display_Scroll;
				display.Resize += display_Resize;
				display.MouseMove += display_MouseMove;
				display.ShapesSelected += display_ShapesSelected;
				display.ShapesInserted += display_ShapesInserted;
				display.ShapesRemoved += display_ShapesRemoved;
				display.ZoomChanged += display_ZoomChanged;
				display.MapProviderChanged += display_MapProviderChanged;
				display.StaticPropsCountChanged += display_PropsCountChanged;
				display.DynamicPropsCountChanged += display_PropsCountChanged;
				display.ToolTipTextChanged += display_ToolTipTextChanged;
			}
		}

		private void UnregisterDisplayEvents(Display display)
		{
			if(display != null)
			{
				display.Scroll -= display_Scroll;
				display.Resize -= display_Resize;
				display.MouseMove -= display_MouseMove;
				display.ShapesSelected -= display_ShapesSelected;
				display.ShapesInserted -= display_ShapesInserted;
				display.ShapesRemoved -= display_ShapesRemoved;
				display.ZoomChanged -= display_ZoomChanged;
				display.MapProviderChanged -= display_MapProviderChanged;
				display.StaticPropsCountChanged -= display_PropsCountChanged;
				display.DynamicPropsCountChanged -= display_PropsCountChanged;
				display.ToolTipTextChanged -= display_ToolTipTextChanged;
			}
		}

		private void RegisterPropertyControllerEvents()
		{
			propertyController.ObjectsSet += propertyController_ObjectsSet;
		}


		private void UnregisterPropertyControllerEvents()
		{
			propertyController.ObjectsSet -= propertyController_ObjectsSet;
		}

		#endregion


		#region [Private] Event Handler implementations - Project, History and Repository

		private void project_LibraryLoaded(object sender, LibraryLoadedEventArgs e)
		{
			// nothing to do here...
		}


		private void project_Opened(object sender, EventArgs e)
		{
			RegisterRepositoryEvents();
			RegisterPropertyControllerEvents();
			// Set main form title
			Text = string.Format("{0} - {1}", Properties.Strings.AppTitle, project.Name);
			// Hide/Show ModelTreeView
			UpdateModelComponentsVisibility(false);
			foreach(IModelObject m in project.Repository.GetModelObjects(null))
			{
				UpdateModelComponentsVisibility(true);
				break;
			}
		}


		private void project_Closed(object sender, EventArgs e)
		{
			if(layoutControlForm != null) layoutControlForm.Close();

			UnregisterPropertyControllerEvents();
			UnregisterRepositoryEvents();

			historyTrackBar.Maximum = project.History.UndoCommandCount + project.History.RedoCommandCount;
			Text = Properties.Strings.AppTitle;
			statusLabelMessage.Text =
			statusLabelMousePosition.Text = string.Empty;
		}


		private void history_CommandAdded(object sender, CommandEventArgs e)
		{
			UpdateUndoMenuItems();
			if(CurrentDisplay != null)
			{
				if(project.History.UndoCommandCount + project.History.RedoCommandCount != historyTrackBar.Maximum)
					historyTrackBar.Maximum = project.History.UndoCommandCount + project.History.RedoCommandCount;
				currHistoryPos = 0;
				historyTrackBar.Value = 0;
			}
		}


		private void history_CommandsExecuted(object sender, CommandsEventArgs e)
		{
			UpdateUndoMenuItems();
			try
			{
				historyTrackBar.ValueChanged -= historyTrackBar_ValueChanged;
				if(e.Reverted)
				{
					if(currHistoryPos != historyTrackBar.Value)
					{
						currHistoryPos += e.Commands.Count;
						historyTrackBar.Value += e.Commands.Count;
					}
				}
				else
				{
					if(currHistoryPos > 0) currHistoryPos -= e.Commands.Count;
					if(historyTrackBar.Value > 0) historyTrackBar.Value -= e.Commands.Count;
				}
			}
			finally
			{
				historyTrackBar.ValueChanged += historyTrackBar_ValueChanged;
			}
		}


		private void repository_DiagramDeleted(object sender, RepositoryDiagramEventArgs e)
		{
			RemoveDisplayOfDiagram(e.Diagram);
			projectIsModified = true;
		}


		private void repository_DiagramUpdated(object sender, RepositoryDiagramEventArgs e)
		{
			DisplayTabPage tabPage = FindDisplayTabPage(e.Diagram);
			if(tabPage != null) tabPage.Text = e.Diagram.Title;
			UpdateStatusInfo();
			projectIsModified = true;
		}


		private void repository_DiagramInserted(object sender, RepositoryDiagramEventArgs e)
		{
			DisplayTabPage tabPage = FindDisplayTabPage(e.Diagram);
			if(tabPage == null)
				displayTabControl.TabPages.Add(CreateDiagramTabPage(e.Diagram, false));
			UpdateAllMenuItems();
			projectIsModified = true;
		}


		private void repository_ModelObjectsInsertedOrDeleted(object sender, RepositoryModelObjectsEventArgs e)
		{
			bool modelExists = false;
			foreach(IModelObject modelObject in project.Repository.GetModelObjects(null))
			{
				modelExists = true;
				break;
			}
			UpdateModelComponentsVisibility(modelExists);
			projectIsModified = true;
		}


		private void Repository_TemplateModified(object sender, RepositoryTemplateEventArgs e)
		{
			projectIsModified = true;
		}


		private void Repository_StyleModified(object sender, RepositoryStyleEventArgs e)
		{
			projectIsModified = true;
		}


		private void Repository_ShapesModified(object sender, RepositoryShapesEventArgs e)
		{
			projectIsModified = true;
		}


		private void Repository_ProjectModified(object sender, RepositoryProjectEventArgs e)
		{
			projectIsModified = true;
		}


		private void Repository_ModelObjectsModified(object sender, RepositoryModelObjectsEventArgs e)
		{
			projectIsModified = true;
		}


		private void Repository_ModelMappingsModified(object sender, RepositoryTemplateEventArgs e)
		{
			projectIsModified = true;
		}


		private void Repository_ModelModified(object sender, RepositoryModelEventArgs e)
		{
			projectIsModified = true;
		}


		private void Repository_DesignModified(object sender, RepositoryDesignEventArgs e)
		{
			projectIsModified = true;
		}


		private void Repository_ConnectionModified(object sender, RepositoryShapeConnectionEventArgs e)
		{
			projectIsModified = true;
		}

		#endregion


		#region [Private] Event Handler implementations - Display and Diagrams


		private void diagramSetController_SelectModelObjectsRequested(object sender, ModelObjectsEventArgs e)
		{
			// ToDo: Find the first display that contains the selected model object's shapes.
			// Problem: 
			// As there is only one propertyController for all displays and the displays unselect all selected
			// shapes before selecting the shapes of the model objects, all selected objects of the property controller 
			// are unselected when processing the next display. We have to find a solution for this issue.
		}


		private void display_Resize(object sender, EventArgs e)
		{
			UpdateStatusInfo();
		}


		private void display_Scroll(object sender, ScrollEventArgs e)
		{
			UpdateStatusInfo();
		}


		private void display_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int x = e.X, y = e.Y;
			if(CurrentDisplay != null)
				CurrentDisplay.ControlToDiagram(e.X, e.Y, out x, out y);
			DisplayMouseCoordinates(x, y);
		}

		private Shape toolTipShape = null;
		private void display_ToolTipTextChanged(object sender, EventArgs e)
		{	toolTipTimer.Stop();
			Display display = (Display)sender;
			toolTipTimer.Tag = display;
		//?	string toolTipText = display.ToolTipText;
		//?	statusLabelMessage.Text = !string.IsNullOrEmpty(toolTipText) ? toolTipText : string.Empty;
			if(string.IsNullOrEmpty(display.ToolTipText))
			{	toolTip.Hide(display);
				display.Invalidate();
			}
			else if(toolTipShape == display.ToolTipShape)
				ShowToolTip(display);
			else
			{	toolTipShape = display.ToolTipShape;
				toolTipTimer.Start();
			}
		}

		private void toolTipTimer_Tick(object sender, EventArgs e)
		{	System.Windows.Forms.Timer timer = (System.Windows.Forms.Timer)sender;
			timer.Stop();
			if(timer.Tag is Display)
				ShowToolTip((Display)timer.Tag);
		}

		private void ShowToolTip(Display display)
		{	string toolTipText;
			if(!string.IsNullOrEmpty(toolTipText = display.ToolTipText))
			{	Point point = CurrentDisplay.PointToClient(MousePosition);
				Size mouseCursorSize = Cursor.Size;
				point.Offset(0, mouseCursorSize.Height/2);
				toolTip.Show(toolTipText, CurrentDisplay, point);
			}
		}

		private void display_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			p.X = e.X; p.Y = e.Y;
			if(CurrentDisplay != null)
				CurrentDisplay.ScreenToDiagram(p, out p);
			DisplayMouseCoordinates(p.X, p.Y);
		}


		private void display_ShapesSelected(object sender, EventArgs e)
		{
			if(sender.Equals(CurrentDisplay))
			{
				int cnt = CurrentDisplay.SelectedShapes.Count;
				if(cnt > 0)
				{
					statusLabelMessage.Text = string.Format("{0} объектов выбрано", cnt);
					if(propertyController.GetSelectedObjects(0) != CurrentDisplay.SelectedShapes)
						propertyController.SetObjects(0, CurrentDisplay.SelectedShapes);
				}
				else statusLabelMessage.Text = string.Empty;
				UpdateAllMenuItems();
				UpdateStatusInfo();
				UpdateHierarchyTreeView(true);
				//
				if(layoutControlForm != null)
					layoutControlForm.SelectedShapes = CurrentDisplay.SelectedShapes;
			}
		}


		private void display_ShapesRemoved(object sender, DiagramPresenterShapesEventArgs e)
		{
			UpdateStatusInfo();
			UpdateHierarchyTreeView(false);
		}


		private void display_ShapesInserted(object sender, DiagramPresenterShapesEventArgs e)
		{
			UpdateStatusInfo();
			UpdateHierarchyTreeView(false);
		}


		private void display_ZoomChanged(object sender, EventArgs e)
		{
			UpdateZoomControl();
		}

		private void display_PropsCountChanged(object sender, EventArgs e)
		{
			ChangeVehicle();
		}

		private void display_UserMessage(object sender, UserMessageEventArgs e)
		{
			MessageBoxIcon icon = MessageBoxIcon.Information;
			MessageBox.Show(this, e.MessageText, "Information", MessageBoxButtons.OK, icon);
		}

		private void display_ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;
			if(contextMenuStrip.Items.Count > 0)		// Костыль - почему-то вызывается два раза,
				ResetMenuItems(contextMenuStrip.Items);	// второй раз - правильно
			ConfigureMenuItems(contextMenuStrip.Items, CurrentDisplay.GetMenuItemDefs());
		}

		private void display_ContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
		{
			ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;
			ResetMenuItems(contextMenuStrip.Items);
		}

		private void hierarchyTreeView_SelectionChanged(object sender, EventArgs e)
		{
			if(this.hierarchyTreeView.Tag is bool
			&& (bool)this.hierarchyTreeView.Tag)	// Отложенное выделение объектов в гриде
			{	this.hierarchyTreeView.Tag = null;
				return;
			}

			if(selectionChanging)
				return;
			selectionChanging = true;
			try
			{
				List<Shape> selectedShapes = new List<Shape>(this.hierarchyTreeView.SelectedObjects.Count);
				foreach(var selectedObject in this.hierarchyTreeView.SelectedObjects)
					selectedShapes.Add((Shape)selectedObject);
				if(selectedShapes.Count == 1)
					CurrentDisplay.SelectShape(selectedShapes[0], false);
				else
					CurrentDisplay.SelectShapes(selectedShapes, false);
			}
			finally
			{
				selectionChanging = false;
			}
		}

		private void UpdateHierarchyTreeView(bool selection = false)
		{
			if(selection)
			{	if(selectionChanging)
					return;
				selectionChanging = true;
				try
				{
					List<Shape> selectedShapes = new List<Shape>(CurrentDisplay.SelectedShapes);
					foreach(Shape selectedShape in selectedShapes)
						if(!selectedShape.IsToolbarHidden)
							for(Shape parent = selectedShape.Parent; parent != null; parent = parent.Parent)
								this.hierarchyTreeView.Expand(parent);
					this.hierarchyTreeView.Tag = true;	// Отложенное выделение объектов в гриде
					this.hierarchyTreeView.SelectObjects(selectedShapes);
				}
				finally
				{
					selectionChanging = false;
				}
			}
			else
			{
				IShapeCollection shapes = CurrentDisplay.Diagram.Shapes;
				List<Shape> roots = new List<Shape>(shapes.Count);
				foreach(Shape shape in shapes)
					if(!shape.IsToolbarHidden)
						roots.Add(shape);
				this.hierarchyTreeView.Roots = roots;
				this.hierarchyTreeView.RebuildAll(true);
			}
		}

		private void ConfigureMenuItems(ToolStripItemCollection toolStripItems, IEnumerable<MenuItemDef> itemDefs)
		{
			MenuItemInfo menuItemInfo;
			string title, description;
			bool sep = true;
			foreach(MenuItemDef itemDef in itemDefs)
			{
				if(CurrentDisplay.HideDeniedMenuItems
				&& !itemDef.IsGranted(project.SecurityManager))
					continue;

				if(itemDef is SeparatorMenuItemDef)
				{
					if(sep)
						continue;
					ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
					toolStripSeparator.Tag = itemDef;
					toolStripItems.Add(toolStripSeparator);
					sep = true;
				}
				else
				{
					description = itemDef.Description;
					if(!menuTitles.TryGetValue(itemDef.Title, out menuItemInfo))
						title = itemDef.Title;
					else if(menuItemInfo == null)
						continue;
					else
					{
						title = menuItemInfo.Title;
						if(!string.IsNullOrEmpty(description) && menuItemInfo.Descriptions != null)
							foreach(ReplaceInfo replaceInfo in menuItemInfo.Descriptions)
								if(!string.IsNullOrEmpty(replaceInfo.FromBegin) && !string.IsNullOrEmpty(replaceInfo.FromEnd))
								{
									if(description.StartsWith(replaceInfo.FromBegin) && description.EndsWith(replaceInfo.FromEnd))
									{
										description = (!string.IsNullOrEmpty(replaceInfo.ToBegin) ? replaceInfo.ToBegin : string.Empty)
													+ description.Substring(replaceInfo.FromBegin.Length, description.Length - replaceInfo.FromBegin.Length - replaceInfo.FromEnd.Length)
													+ (!string.IsNullOrEmpty(replaceInfo.ToEnd) ? replaceInfo.ToEnd : string.Empty);
										break;
									}
								}
								else //if(!string.IsNullOrEmpty(replaceInfo.FromBegin))
								{
									if(description.StartsWith(replaceInfo.FromBegin))
									{
										description = replaceInfo.ToBegin;
										break;
									}
								}
						//	else if(!string.IsNullOrEmpty(replaceInfo.FromBegin))
						//	{
						//		if(description.StartsWith(replaceInfo.FromBegin))
						//		{
						//			description = (!string.IsNullOrEmpty(replaceInfo.ToBegin) ? replaceInfo.ToBegin : string.Empty)
						//						+ description.Substring(replaceInfo.FromBegin.Length, description.Length - replaceInfo.FromBegin.Length)
						//						+ (!string.IsNullOrEmpty(replaceInfo.ToEnd) ? replaceInfo.ToEnd : string.Empty);
						//			break;
						//		}
						//	}
						//	else if(!string.IsNullOrEmpty(replaceInfo.FromEnd))
						//	{
						//		if(description.StartsWith(replaceInfo.FromEnd))
						//		{
						//			description = (!string.IsNullOrEmpty(replaceInfo.ToBegin) ? replaceInfo.ToBegin : string.Empty)
						//						+ description.Substring(0, description.Length - replaceInfo.FromEnd.Length)
						//						+ (!string.IsNullOrEmpty(replaceInfo.ToEnd) ? replaceInfo.ToEnd : string.Empty);
						//			break;
						//		}
						//	}
					}

					ToolStripMenuItem menuItem = new ToolStripMenuItem(title);
					// Add event handler (implemented as lambda expression in this example)
					menuItem.Click += new EventHandler((_sender, _eventArgs) => { MenuItemDef mnuItemDef = (MenuItemDef)((ToolStripMenuItem)_sender).Tag; mnuItemDef.Execute(mnuItemDef, project); });
					// Set menu item properties
					menuItem.Tag = itemDef;
					menuItem.Name = itemDef.Name;
					menuItem.Enabled = (itemDef.IsFeasible && itemDef.IsGranted(project.SecurityManager));
					menuItem.ToolTipText = description;
					// Set icon and the background color rendered as transparent
					menuItem.Image = itemDef.Image;
					menuItem.ImageTransparentColor = itemDef.ImageTransparentColor;

					// Insert the created menu item into your ContextMenuStrip
					//myContextMenuStrip.Items.Insert(insertPos, menuItem);
					toolStripItems.Add(menuItem);

					if(itemDef.SubItems != null)
						ConfigureMenuItems(menuItem.DropDownItems, itemDef.SubItems);

					sep = false;
				}
			}
		}

		private void ResetMenuItems(ToolStripItemCollection toolStripItems)
		{
			for(int i = toolStripItems.Count - 1; i >= 0; --i)
			{
				// Delete and release inserted items
				if(toolStripItems[i].Tag is MenuItemDef)
				{
					ToolStripItem menuItem = toolStripItems[i];
					if(menuItem is ToolStripMenuItem)
						ResetMenuItems(((ToolStripMenuItem)menuItem).DropDownItems);
					toolStripItems.RemoveAt(i);
					menuItem.Dispose();
				}
			}
		}

		#endregion


		#region [Private] Event Handler implementations - ModelTree

		private void modelTree_ModelObjectSelected(object sender, ModelObjectSelectedEventArgs eventArgs)
		{
			this.propertyWindowTabControl.SelectedTab = this.propertyWindowModelTab;
		}

		#endregion


		#region [Private] Event Handler implementations - ToolBox

		private void toolBoxAdapter_ShowTemplateEditorDialog(object sender, TemplateEditorEventArgs e)
		{
			templateEditorDialog = new TemplateEditorDialog(e.Project, e.Template);
			templateEditorDialog.Show(this);
		}


		private void toolBoxAdapter_ShowLibraryManagerDialog(object sender, EventArgs e)
		{
			LibraryManagementDialog dlg = new LibraryManagementDialog(project);
			dlg.Show(this);
		}


		private void toolBoxAdapter_ToolSelected(object sender, EventArgs e)
		{
			// ToDo:
			// If the ownerDisplay and the ToolBox are not connected directly, one can handle this event and 
			// assign the SelectedTool as the DIsplay's CurrentTool
		}


		private void toolBoxAdapter_ShowDesignEditor(object sender, System.EventArgs e)
		{
			DesignEditorDialog dlg = new DesignEditorDialog(project);
			dlg.Show(this);
		}

		#endregion


		#region [Private] Event Handler implementations - Misc

		private void DiagramDesignerMainForm_Load(object sender, EventArgs e)
		{
			try
			{
				// Read config file
				configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nShapeConfigDirectory);
				if(!Directory.Exists(configFolder)) Directory.CreateDirectory(configFolder);
				// Init history controls
				historyTrackBar.Minimum = 0;
				historyTrackBar.Maximum = 0;
				historyTrackBar.Value = 0;
				historyTrackBar.TickStyle = System.Windows.Forms.TickStyle.BottomRight;
				historyTrackBar.TickFrequency = 1;
				currHistoryPos = historyTrackBar.Value;
				project.History.CommandAdded += history_CommandAdded;
				project.History.CommandsExecuted += history_CommandsExecuted;
				diagramSetController.SelectModelObjectsRequested += diagramSetController_SelectModelObjectsRequested;

				// Deactivate "View Help" menu item if help file cannot be found
				viewHelpToolStripMenuItem.Enabled = false;
				DirectoryInfo helpDir = GetHelpDir();
				if(!helpDir.Exists)
					viewHelpToolStripMenuItem.ToolTipText = String.Format("Help file directory '{0}' does not exist (or is not accessible).", helpDir.FullName);
				else
				{
					FileInfo helpFile = GetHelpFile();
					if(helpFile != null && helpFile.Exists)
						viewHelpToolStripMenuItem.Enabled = true;
					else
						viewHelpToolStripMenuItem.ToolTipText = "Help file does not exist (or is not accessible).";
				}

				// Add library load support
				project.AutoLoadLibraries = true;
				project.LibrarySearchPaths.Add(Application.StartupPath);

				XmlDocument config = ReadConfigFile();
				ReadConfig(config);

				MaintainRecentProjects();
				CreateRecentProjectsMenuItems();

				// Get command line parameters and check if a repository should be loaded on startup
				RepositoryInfo repositoryInfo = RepositoryInfo.Empty;
				string[] commandLineArgs = Environment.GetCommandLineArgs();
				if(commandLineArgs != null)
				{
					int cnt = commandLineArgs.Length;
					foreach(string arg in commandLineArgs)
					{
						Debug.Print(arg);
						string path = Path.GetFullPath(arg);
						if(string.IsNullOrEmpty(path)) continue;
						else
						{
							if(path == Path.GetFullPath(Application.ExecutablePath))
								continue;
							// Check if the file is an xml document
							if(File.Exists(path))
							{
								TextReader reader = null;
								try
								{
									reader = File.OpenText(path);
									if(reader.ReadLine().Contains("xml"))
									{
										repositoryInfo.computerName = Environment.MachineName;
										repositoryInfo.location = path;
										repositoryInfo.projectName = Path.GetFileNameWithoutExtension(path);
										repositoryInfo.typeName = RepositoryInfo.XmlStoreTypeName;
									}
								}
								finally
								{
									if(reader != null)
									{
										reader.Close();
										reader.Dispose();
										reader = null;
									}
								}
							}
						}
					}
				}

				XmlStore store;
				if(repositoryInfo != RepositoryInfo.Empty)
				{
					store = new XmlStore()
					{
						DirectoryName = Path.GetDirectoryName(repositoryInfo.location),
						FileExtension = Path.GetExtension(repositoryInfo.location),
						LazyLoading = true
					};
					OpenProject(repositoryInfo.projectName, store);
				}
				else
				{
					NewXmlRepositoryProject(false);
					project.AddLibraryByName("Dataweb.NShape.FhisShapes", true);
					project.AddLibraryByName("Dataweb.NShape.FhisPrimitives", true);
					project.AddLibraryByName("Dataweb.NShape.FhisServices", true);
					//	project.AddLibraryByName("Dataweb.NShape.GeneralShapes", true);
#if DEBUG
					// Shape libraries
					//project.AddLibraryByName("Dataweb.NShape.SoftwareArchitectureShapes");
					//project.AddLibraryByName("Dataweb.NShape.FlowChartShapes");
					//project.AddLibraryByName("Dataweb.NShape.ElectricalShapes");
					// ModelObjectTypes libraries
					//project.AddLibraryByFilePath("Dataweb.NShape.GeneralModelObjects.dll");
#endif
					// Mark project as "Not modified" in order to suppress the "Do you want to save changes" question
					projectIsModified = false;
				}

				UpdateAllMenuItems();
				UpdateStatusInfo();

				// Setting the form's WindowState will show the form immediately, so we have to perform this step
				// after all initialization is done.
				ReadWindowConfig(config);

				ShowElectricPowerLines();
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


		private void DiagramDesignerMainForm_Shown(object sender, System.EventArgs e)
		{
			CheckFrameworkVersion();
			CheckLibrariesLoaded();
		}


		private void DiagramDesignerMainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(!CloseProject()) e.Cancel = true;
			else SaveConfigFile();
		}


		private void displaysTabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				DisplayTabPage tab = displayTabControl.SelectedTab as DisplayTabPage;
				if(tab != null)
				{
					// Create a display and load diagram if not done yet
					if(tab.Display == null)
					{
						Diagram diagram = (Diagram)tab.Tag;
						tab.Display = CreateDiagramDisplay(diagram);
					}
					CurrentDisplay = tab.Display;
					UpdateAllMenuItems();
				}
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}


		private void LayoutControlForm_LayoutChanged(object sender, EventArgs e)
		{
			// Why? Makes preview switch between original and new State.
			// currentDisplay.SaveChanges();
		}


		private void LayoutControlForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			layoutControlForm = null;
		}


		private void propertyController_ObjectsSet(object sender, PropertyControllerEventArgs e)
		{
			if(e.PageIndex == 0)
			{
				string typeName = GetCommonTypeName(e.Objects);
				propertyWindowShapeTab.Text = string.Format(string.IsNullOrEmpty(typeName) ? Properties.Strings.ObjectProperties : typeName);
				//?propertyWindowTabControl.TabPages[e.PageIndex].Text = string.Format(string.IsNullOrEmpty(typeName) ? Properties.Strings.ObjectProperties : typeName);
			}
		}


		private string GetCommonTypeName(IReadOnlyCollection<object> objects)
		{
			if(objects.Count == 0)
				return string.Empty;
			if(objects.Count == 1)
				foreach(object o in objects)
					return o is Diagram ? Properties.Strings.ObjectDiagram
						: o is ShapeGroup ? Properties.Strings.ObjectRVR
						: o is Shape ? ((Shape)o).Type.Name
						: o.GetType().Name
						;
			return Properties.Strings.Objects;

			//	Type type = null;
			//	foreach(object o in objects)
			//	{
			//		Type objType = null;
			//		if(o is ShapeGroup)
			//			objType = typeof(ShapeGroup);
			//		else if(o is Shape)
			//			objType = typeof(Shape);
			//		else if(o is IModelObject)
			//			objType = typeof(IModelObject);
			//		else
			//			objType = o != null ? o.GetType() : null;
			//	
			//		if(type == null)
			//			type = objType;
			//		else if(type != objType)
			//			type = typeof(object);
			//	}
			//	if(type == typeof(IModelObject))
			//		return "Model";
			//	//return type != null ? type.Name : string.Empty;
			//	return type == typeof(Diagram) ? Properties.Strings.ObjectDiagram
			//		: type == typeof(ShapeGroup) ? Properties.Strings.ObjectRVR
			//		: type == typeof(Shape) ? Properties.Strings.Object
			//		: type != null ? type.Name
			//		: string.Empty
			//		;
		}


		private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolboxListView.View = View.Details;
		}


		private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolboxListView.View = View.LargeIcon;
		}


		private void listToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolboxListView.View = View.List;
		}


		private void tilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolboxListView.View = View.Tile;
		}


		private void smallIconsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolboxListView.View = View.SmallIcon;
		}

		#endregion


		#region [Private] Event Handler implementations - Toolbar

		private void refreshButton_Click(object sender, EventArgs e)
		{
			if(CurrentDisplay != null)
				CurrentDisplay.Refresh();
		}


		private void forwardButton_Click(object sender, EventArgs e)
		{
			if(displayTabControl.SelectedIndex < displayTabControl.TabPages.Count - 1)
				++displayTabControl.SelectedIndex;
		}


		private void backButton_Click(object sender, EventArgs e)
		{
			if(displayTabControl.SelectedIndex > 0)
				displayTabControl.SelectedIndex--;
		}


		private void undoToolStripSplitButton_DropDownOpening(object sender, EventArgs e)
		{
			undoToolStripSplitButton.DropDownItems.Clear();
			if(CurrentDisplay != null)
			{
				int nr = 0;
				foreach(string cmdDesc in project.History.GetUndoCommandDescriptions(historyDropDownItemCount))
				{
					System.Windows.Forms.ToolStripItem item = new System.Windows.Forms.ToolStripMenuItem();
					item.Text = string.Format("{0}: {1}", ++nr, cmdDesc);
					item.Click += undoItem_Click;
					undoToolStripSplitButton.DropDownItems.Add(item);
				}
			}
		}


		private void redoToolStripSplitButton_DropDownOpening(object sender, EventArgs e)
		{
			redoToolStripSplitButton.DropDownItems.Clear();
			if(CurrentDisplay != null)
			{
				int nr = 0;
				foreach(string cmdDesc in project.History.GetRedoCommandDescriptions(historyDropDownItemCount))
				{
					System.Windows.Forms.ToolStripItem item = new System.Windows.Forms.ToolStripMenuItem();
					item.Text = string.Format("{0}: {1}", ++nr, cmdDesc);
					item.Click += redoItem_Click;
					redoToolStripSplitButton.DropDownItems.Add(item);
				}
			}
		}


		private void zoomToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			int zoom;
			if(int.TryParse(zoomToolStripComboBox.Text.Replace('%', ' ').Trim(), out zoom))
			{
				if(Zoom != zoom) Zoom = zoom;
			}
		}


		private void toolStripComboBox1_TextChanged(object sender, EventArgs e)
		{
			int cursorPos = -1;
			if(zoomToolStripComboBox.Focused)
				cursorPos = zoomToolStripComboBox.SelectionStart;

			string txt = null;
			if(zoomToolStripComboBox.Text.Contains("%"))
				txt = zoomToolStripComboBox.Text.Replace("%", "");
			else txt = zoomToolStripComboBox.Text;
			// Parse text and set zoom level
			int zoom;
			if(int.TryParse(txt.Trim(), out zoom))
				if(zoom > 0 && Zoom != zoom) Zoom = zoom;

			if(zoomToolStripComboBox.Focused)
				zoomToolStripComboBox.SelectionStart = cursorPos;
		}


		private void runtimeModeButton_SelectedIndexChanged(object sender, EventArgs e)
		{
			((RoleBasedSecurityManager)project.SecurityManager).CurrentRoleName = runtimeModeComboBox.Text;
		}

		#endregion


		#region [Private] Event Handler implementations - Menu item "File"

		private void newXMLRepositoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewXmlRepositoryProject(true);
		}


		private void NewXmlRepositoryProject(bool askUserLoadLibraries)
		{
			if(CloseProject())
			{
				//if (!Directory.Exists(xmlStoreDirectory)) {
				//    StringBuilder path = new StringBuilder(512);
				//    const int COMMON_DOCUMENTS = 0x002e;
				//    if (SHGetSpecialFolderPath(IntPtr.Zero, path, COMMON_DOCUMENTS, 0) != 0)
				//        xmlStoreDirectory = Path.Combine(Path.Combine(path.ToString(), "NShape"), "Demo Projects");
				//    else xmlStoreDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				//}
				//XmlStore store = new XmlStore(xmlStoreDirectory, ProjectFileExtension);

				// Do not create a new XmlStore yet. This will be done when saving for the first time.
				CreateProject(Properties.Strings.NewProjectName, null, askUserLoadLibraries);
			}
		}


		private void newSQLServerRepositoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string projectName;
			AdoNetStore store = GetAdoNetStore(out projectName, OpenAdoNetRepositoryDialog.Mode.CreateProject);
			if(store != null) CreateProject(projectName, store, true);
		}


		private AdoNetStore GetAdoNetStore()
		{
			string projectName;
			return GetAdoNetStore(out projectName, OpenAdoNetRepositoryDialog.Mode.CreateSchema);
		}


		private AdoNetStore GetAdoNetStore(out string projectName, OpenAdoNetRepositoryDialog.Mode mode)
		{
			projectName = string.Empty;
			AdoNetStore result = null;
			try
			{
				Cursor = Cursors.WaitCursor;
				Application.DoEvents();
				using(OpenAdoNetRepositoryDialog dlg = new OpenAdoNetRepositoryDialog(this, DefaultServerName, DefaultDatabaseName, mode))
				{
					if(dlg.ShowDialog() == DialogResult.OK && CloseProject())
					{
						if(dlg.ProviderName == SqlServerProviderName)
						{
							result = new SqlStore(dlg.ServerName, dlg.DatabaseName);
							projectName = dlg.ProjectName;
						}
						else MessageBox.Show(this, "Unsupported database repository.", "Unsupported repository", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			finally { Cursor = Cursors.Default; }
			return result;
		}


		private void openXMLRepositoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			openFileDialog.Filter = FileFilterXmlRepository;
			if(Directory.Exists(xmlStoreDirectory))
				openFileDialog.InitialDirectory = xmlStoreDirectory;
			if(openFileDialog.ShowDialog() == DialogResult.OK && CloseProject())
			{
				xmlStoreDirectory = Path.GetDirectoryName(openFileDialog.FileName);
				XmlStore store = new XmlStore()
				{
					DirectoryName = xmlStoreDirectory,
					FileExtension = Path.GetExtension(openFileDialog.FileName),
					LazyLoading = true
				};
				OpenProject(Path.GetFileNameWithoutExtension(openFileDialog.FileName), store);
			}
		}


		private void openSQLServerRepositoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string projectName;
			AdoNetStore store = GetAdoNetStore(out projectName, OpenAdoNetRepositoryDialog.Mode.OpenProject);
			if(store != null) OpenProject(projectName, store);
		}


		private void openRecentProjectMenuItem_Click(object sender, EventArgs e)
		{
			Debug.Assert(sender is ToolStripItem && ((ToolStripItem)sender).Tag is RepositoryInfo);
			RepositoryInfo repositoryInfo = (RepositoryInfo)((ToolStripItem)sender).Tag;
			if(CloseProject())
			{
				Store store = RepositoryInfo.CreateStore(repositoryInfo);
				if(store != null) OpenProject(repositoryInfo.projectName, store);
				else MessageBox.Show(this, string.Format("{0} repositories are not supported by this version.", repositoryInfo.typeName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveProjectAs();
		}


		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveProject();
		}


		private void upgradeVersionMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				string msg = string.Format("Project '{0}' will be upgraded from file version {1} to {2}. "
					+ "{3}After upgrading, the project will be saved automatically.{3}Do you want to continue upgrading and saving the project?",
					project.Name, project.Repository.Version, maxRepositoryVersion, Environment.NewLine);
				DialogResult res = MessageBox.Show(msg, "Upgrade and Save Project?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if(res == DialogResult.Yes)
				{
					// Upgrade repository version
					project.UpgradeVersion();
				}
			}
			catch(Exception exc)
			{
				MessageBox.Show(this, exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


		private void useEmbeddedImagesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			XmlStore xmlStore = cachedRepository.Store as XmlStore;
			if(xmlStore != null)
			{
				if(xmlStore.ImageLocation != XmlImageFileLocation)
					xmlStore.ImageLocation = XmlImageFileLocation;
			}
		}


		private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CloseProject();
		}


		private void ManageShapeAndModelLibrariesMenuItem_Click(object sender, EventArgs e)
		{
			using(LibraryManagementDialog dlg = new LibraryManagementDialog(project))
			{
				dlg.StartPosition = FormStartPosition.CenterParent;
				dlg.ShowDialog(this);
			}
		}


		private void importRepositoryMenuItem_Click(object sender, System.EventArgs e)
		{
			ImportRepositoryFromClipboard();
		}


		private void exportRepositoryMenuItem_Click(object sender, System.EventArgs e)
		{
			ExportRepositoryToClipboard();
		}


		private void exportDiagramAsMenuItem_Click(object sender, EventArgs e)
		{
			using(ExportDiagramDialog dlg = new ExportDiagramDialog(CurrentDisplay))
				dlg.ShowDialog(this);
		}


		private void emfPlusFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportMetaFile(ImageFileFormat.EmfPlus);
		}


		private void emfOnlyFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportMetaFile(ImageFileFormat.Emf);
		}


		private void pngFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportBitmapFile(ImageFileFormat.Png);
		}


		private void jpgFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportBitmapFile(ImageFileFormat.Jpeg);
		}


		private void bmpFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportBitmapFile(ImageFileFormat.Bmp);
		}


		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}


		private Image GetImageFromDiagram(ImageFileFormat imageFormat)
		{
			Image result = null;
			Color backColor = Color.Transparent;
			if(CurrentDisplay.SelectedShapes.Count > 0)
				result = CurrentDisplay.Diagram.CreateImage(imageFormat, CurrentDisplay.SelectedShapes.BottomUp, CurrentDisplay.GridSize, false, backColor);
			else
				result = CurrentDisplay.Diagram.CreateImage(imageFormat, null, 0, true, backColor);
			return result;
		}


		[DllImport("gdi32")]
		static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, string fileName);

		[DllImport("gdi32")]
		static extern bool DeleteEnhMetaFile(IntPtr hemf);

		private void ExportMetaFile(ImageFileFormat imageFormat)
		{
			string fileFilter = null;
			switch(imageFormat)
			{
				case ImageFileFormat.Emf:
				case ImageFileFormat.EmfPlus:
					fileFilter = "Enhanced Meta Files|*.emf|All Files|*.*"; break;
				default: throw new NShapeUnsupportedValueException(imageFormat);
			}
			saveFileDialog.Filter = fileFilter;
			if(saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = saveFileDialog.FileName;
				if(File.Exists(fileName)) File.Delete(fileName);

				using(Metafile image = GetImageFromDiagram(imageFormat) as Metafile)
				{
					if(image != null)
					{
						IntPtr hEmf = image.GetHenhmetafile();
						if(hEmf != IntPtr.Zero)
						{
							IntPtr resHEnh = CopyEnhMetaFile(hEmf, fileName);
							if(resHEnh != IntPtr.Zero)
								DeleteEnhMetaFile(resHEnh);
							DeleteEnhMetaFile(hEmf);
						}
					}
					else Debug.Fail("GetImageFromDiagram did not return a metafile image!");
				}
			}
		}


		private void ExportBitmapFile(ImageFileFormat imageFileFormat)
		{
			string fileFilter = null;
			ImageFormat imageFormat = null;
			switch(imageFileFormat)
			{
				case ImageFileFormat.Bmp:
					fileFilter = "Bitmap Picture Files|*.bmp|All Files|*.*";
					imageFormat = ImageFormat.Bmp;
					break;
				case ImageFileFormat.Gif:
					fileFilter = "Graphics Interchange Format Files|*.gif|All Files|*.*";
					imageFormat = ImageFormat.Gif;
					break;
				case ImageFileFormat.Jpeg:
					fileFilter = "Joint Photographic Experts Group (JPEG) Files|*.jpeg;*.jpg|All Files|*.*";
					imageFormat = ImageFormat.Jpeg;
					break;
				case ImageFileFormat.Png:
					fileFilter = "Portable Network Graphics Files|*.png|All Files|*.*";
					imageFormat = ImageFormat.Png;
					break;
				case ImageFileFormat.Tiff:
					fileFilter = "Tagged Image File Format Files|*.tiff;*.tif|All Files|*.*";
					imageFormat = ImageFormat.Tiff;
					break;
				default: throw new NShapeUnsupportedValueException(imageFileFormat);
			}
			saveFileDialog.Filter = fileFilter;
			if(saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = saveFileDialog.FileName;
				if(File.Exists(fileName)) File.Delete(fileName);

				using(Image image = GetImageFromDiagram(imageFileFormat))
				{
					if(image != null)
					{
						ImageCodecInfo codecInfo = null;
						ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
						foreach(ImageCodecInfo ici in encoders)
						{
							if(ici.FormatID.Equals(imageFormat.Guid))
							{
								codecInfo = ici;
								break;
							}
						}
						EncoderParameters encoderParams = new EncoderParameters(3);
						encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.RenderMethod, (long)EncoderValue.RenderProgressive);
						// JPG specific encoder parameter
						encoderParams.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)95);
						// TIFF specific encoder parameter
						encoderParams.Param[2] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)EncoderValue.CompressionLZW);

						image.Save(fileName, codecInfo, encoderParams);
					}
				}
			}
		}

		#endregion


		#region [Private] Event Handler implementations - Menu item "Edit"

		private void newDiagramToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Diagram diagram = new Diagram(string.Format("Диаграмма {0}", displayTabControl.TabPages.Count + 1));
			diagram.Title = diagram.Name;
			ICommand cmd = new CreateDiagramCommand(diagram);
			project.ExecuteCommand(cmd);
			displayTabControl.SelectedTab = FindDisplayTabPage(diagram);
			showDiagramSettingsToolStripMenuItem_Click(this, EventArgs.Empty);
		}


		private void deleteDiagramToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Diagram diagram = CurrentDisplay.Diagram;
			ICommand cmd = new DeleteDiagramCommand(diagram);
			project.ExecuteCommand(cmd);

			// Try to remove Display (in case the Cache-Event was not handled)
			RemoveDisplayOfDiagram(diagram);
		}


		private void copyShapeOnlyItem_Click(object sender, EventArgs e)
		{
			CurrentDisplay.Copy(false);
			UpdateAllMenuItems();
		}


		private void copyShapeAndModelItem_Click(object sender, EventArgs e)
		{
			CurrentDisplay.Copy(true);
			UpdateAllMenuItems();
		}


		private void copyAsImageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(CurrentDisplay != null && CurrentDisplay.Diagram != null)
			{
				// Copy image as PNG and EMFPlusDual formats to clipboard
				CurrentDisplay.CopyImageToClipboard(ImageFileFormat.Png, true);
				CurrentDisplay.CopyImageToClipboard(ImageFileFormat.EmfPlus, false);
			}
		}


		private void cutShapeOnlyItem_Click(object sender, EventArgs e)
		{
			CurrentDisplay.Cut(false);
			UpdateAllMenuItems();
		}


		private void cutShapeAndModelItem_Click(object sender, EventArgs e)
		{
			CurrentDisplay.Cut(true);
			UpdateAllMenuItems();
		}


		private void pasteMenuItem_Click(object sender, EventArgs e)
		{
			CurrentDisplay.Paste(CurrentDisplay.GridSize, CurrentDisplay.GridSize);
			UpdateAllMenuItems();
		}


		private void deleteShapeAndModelItem_Click(object sender, EventArgs e)
		{
			CurrentDisplay.DeleteShapes(true);
			UpdateAllMenuItems();
		}


		private void deleteShapeOnlyItem_Click(object sender, EventArgs e)
		{
			CurrentDisplay.DeleteShapes(false);
			UpdateAllMenuItems();
		}


		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(CurrentDisplay != null && CurrentDisplay.Diagram != null)
				CurrentDisplay.SelectAll();
		}


		private void selectAllShapesOfTheSameTypeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(CurrentDisplay != null && CurrentDisplay.Diagram != null && CurrentDisplay.SelectedShapes.Count > 0)
			{
				Shape refShape = CurrentDisplay.SelectedShapes.TopMost;
				foreach(Shape s in CurrentDisplay.Diagram.Shapes)
				{
					if(s == refShape) continue;
					if(s.Type == refShape.Type)
						CurrentDisplay.SelectShape(s, true);
				}
			}
		}


		private void selectAllShapesOfTheSameTemplateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(CurrentDisplay != null && CurrentDisplay.Diagram != null && CurrentDisplay.SelectedShapes.Count > 0)
			{
				Shape refShape = CurrentDisplay.SelectedShapes.TopMost;
				foreach(Shape s in CurrentDisplay.Diagram.Shapes)
				{
					if(s == refShape) continue;
					if(s.Template == refShape.Template)
						CurrentDisplay.SelectShape(s, true);
				}
			}
		}


		private void unselectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(CurrentDisplay != null && CurrentDisplay.Diagram != null)
				CurrentDisplay.UnselectAll();
		}


		private void toForegroundMenuItem_Click(object sender, System.EventArgs e)
		{
			CurrentDisplay.DiagramSetController.LiftShapes(CurrentDisplay.Diagram, CurrentDisplay.SelectedShapes, ZOrderDestination.ToTop);
			UpdateAllMenuItems();
		}


		private void toBackgroundMenuItem_Click(object sender, System.EventArgs e)
		{
			CurrentDisplay.DiagramSetController.LiftShapes(CurrentDisplay.Diagram, CurrentDisplay.SelectedShapes, ZOrderDestination.ToBottom);
			UpdateAllMenuItems();
		}


		private void historyTrackBar_ValueChanged(object sender, EventArgs e)
		{
			int d = currHistoryPos - historyTrackBar.Value;
			bool commandExecuted = false;
			try
			{
				//project.History.CommandExecuted -= history_CommandExecuted;
				project.History.CommandsExecuted -= history_CommandsExecuted;

				if(d != 0)
				{
					if(d < 0) project.History.Undo(d * (-1));
					else if(d > 0) project.History.Redo(d);
					commandExecuted = true;
				}
			}
			catch(NShapeSecurityException exc)
			{
				MessageBox.Show(this, exc.Message, "Command execution failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
				commandExecuted = false;
			}
			catch(Exception exc)
			{
				Debug.Fail(exc.Message);
				throw;
			}
			finally
			{
				//project.History.CommandExecuted += history_CommandExecuted;
				project.History.CommandsExecuted += history_CommandsExecuted;
			}

			if(commandExecuted)
				currHistoryPos = historyTrackBar.Value;
			else if(historyTrackBar.Value != currHistoryPos)
				historyTrackBar.Value = currHistoryPos;
			UpdateAllMenuItems();
		}


		private void undoButton_Click(object sender, EventArgs e)
		{
			if(historyTrackBar.Value < historyTrackBar.Maximum)
				historyTrackBar.Value += 1;
			if(sender is ToolStripItem) ((ToolStripItem)sender).Invalidate();
		}


		private void redoButton_Click(object sender, EventArgs e)
		{
			if(historyTrackBar.Value > historyTrackBar.Minimum)
				historyTrackBar.Value -= 1;
			if(sender is ToolStripItem) ((ToolStripItem)sender).Invalidate();
		}


		private void undoItem_Click(object sender, EventArgs e)
		{
			System.Windows.Forms.ToolStripSplitButton button = (System.Windows.Forms.ToolStripSplitButton)((System.Windows.Forms.ToolStripItem)sender).OwnerItem;
			// Undo was executed from the main menu (DropDownList)
			if(button != null)
			{
				int idx = button.DropDownItems.IndexOf((System.Windows.Forms.ToolStripMenuItem)sender);
				historyTrackBar.Value += idx + 1;
			}
			else
				// Undo was executed from context menu
				historyTrackBar.Value += 1;
		}


		private void redoItem_Click(object sender, EventArgs e)
		{
			System.Windows.Forms.ToolStripSplitButton button = (System.Windows.Forms.ToolStripSplitButton)((System.Windows.Forms.ToolStripItem)sender).OwnerItem;
			// Redo was executed from the main menu (DropDownList)
			if(button != null)
			{
				int idx = button.DropDownItems.IndexOf((System.Windows.Forms.ToolStripMenuItem)sender);
				historyTrackBar.Value -= idx + 1;
			}
			else
				// Redo was executed from context menu
				historyTrackBar.Value -= 1;
		}

		#endregion


		#region [Private] Event Handler implementations - Menu Item "View"

		private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool isChecked = false;
			if(sender is System.Windows.Forms.ToolStripMenuItem)
				isChecked = ((System.Windows.Forms.ToolStripMenuItem)sender).Checked;
			else if(sender is System.Windows.Forms.ToolStripButton)
				isChecked = ((System.Windows.Forms.ToolStripButton)sender).Checked;
			ShowGrid = isChecked;
			showGridMenuItem.Checked = isChecked;
			showGridToolbarButton.Checked = isChecked;
		}


		private void debugDrawOccupationToolbarButton_Click(object sender, EventArgs e)
		{
#if DEBUG_UI
			ShowCellOccupation = debugDrawOccupationToolbarButton.Checked;
#endif
		}


		private void debugDrawInvalidatedAreaToolbarButton_Click(object sender, EventArgs e)
		{
#if DEBUG_UI
			ShowInvalidatedAreas = debugDrawInvalidatedAreaToolbarButton.Checked;
#endif
		}


		private void showDisplaySettingsItem_Click(object sender, EventArgs e)
		{
			using(DisplaySettingsForm dlg = new DisplaySettingsForm(this))
			{
				dlg.ShowGrid = ShowGrid;
				dlg.SnapToGrid = SnapToGrid;
				dlg.GridColor = GridColor;
				dlg.GridSize = GridSize;
				dlg.SnapDistance = SnapDistance;
				dlg.ResizePointShape = ResizePointShape;
				dlg.ConnectionPointShape = ConnectionPointShape;
				dlg.ControlPointSize = ControlPointSize;
				dlg.ShowDefaultContextMenu = ShowDefaultContextMenu;
				dlg.HideDeniedMenuItems = HideDeniedMenuItems;

				if(dlg.ShowDialog(this) == DialogResult.OK)
				{
					ShowGrid = dlg.ShowGrid;
					GridColor = dlg.GridColor;
					showGridMenuItem.Checked = ShowGrid;
					showGridToolbarButton.Checked = ShowGrid;
					SnapToGrid = dlg.SnapToGrid;
					GridSize = dlg.GridSize;
					SnapDistance = dlg.SnapDistance;
					ResizePointShape = dlg.ResizePointShape;
					ConnectionPointShape = dlg.ConnectionPointShape;
					ControlPointSize = dlg.ControlPointSize;
					ShowDefaultContextMenu = dlg.ShowDefaultContextMenu;
					HideDeniedMenuItems = dlg.HideDeniedMenuItems;
				}
			}
		}


		private void showDiagramSettingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			propertyController.SetObject(0, CurrentDisplay.Diagram);
		}


		private void editDesignsAndStylesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DesignEditorDialog dlg = new DesignEditorDialog(project);
			dlg.Show(this);
		}


		private void viewShowLayoutControlToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(layoutControlForm == null)
			{
				LayoutDialog lcf = new LayoutDialog();
				lcf.Project = CurrentDisplay.Project;
				lcf.Diagram = CurrentDisplay.Diagram;
				lcf.SelectedShapes = CurrentDisplay.SelectedShapes;
				lcf.FormClosed += LayoutControlForm_FormClosed;
				lcf.LayoutChanged += LayoutControlForm_LayoutChanged;
				lcf.Show(this);
				layoutControlForm = lcf;
			}
			else
			{
				layoutControlForm.Activate();
			}
		}


		private void highQualityToolStripMenuItem_Click(object sender, EventArgs e)
		{
			HighQuality = !HighQuality;
			highQualityRenderingMenuItem.Checked = HighQuality;
		}


		private void resetToolbarsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult res;
			string areYouSureQuestion = "Do you really want to reset the layout of all toolbars to default?";
			res = MessageBox.Show(this, areYouSureQuestion, "Reset Toolbar Layout", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
			if(res == DialogResult.Yes)
			{
				// Store setting "Do no load layout at next start"
				loadToolStripLayoutOnStartup = false;
				// Ask for restart
				string msgTxt = "Resetting the toolbar layout requires restarting the application. \nDo want to restart now?";
				MessageBox.Show(this, msgTxt, "Restart Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
				if(res == System.Windows.Forms.DialogResult.Yes)
				{
					if(CloseProject())
					{
						try
						{
							const string quotedPathFmt = "\"{0}\"";
							ProcessStartInfo startInfo = new ProcessStartInfo(string.Format(quotedPathFmt, Application.ExecutablePath));
							if(cachedRepository.Store is XmlStore) startInfo.Arguments = string.Format(quotedPathFmt, ((XmlStore)cachedRepository.Store).ProjectFilePath);

							// Save settings, start the application and end this instance
							SaveConfigFile();
							Application.Exit();

							Process.Start(startInfo);
						}
						catch(Exception exc)
						{
							MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
				else MessageBox.Show(this, "The toolbar layout will be reset when restarting the application.", "Reset Toolbar Layout", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		#endregion


		#region [Private] Event Handler implementations - Menu item "Tools"

		private void adoNetDatabaseGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(project.Repository == null)
				MessageBox.Show(this, "No repository set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else if(!(project.Repository is CachedRepository))
				MessageBox.Show(this, string.Format("Repositories of type '{0}' are not supported by the database generator.", project.Repository.GetType().Name), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else
			{
				if(project.IsOpen)
				{
					string msgStr = "You are about to create a new database schema for a NShape database repository." + Environment.NewLine + Environment.NewLine;
					msgStr += "If you proceed, the current project will be closed and you will be asked for a database server and for choosing a set of NShape libraries." + Environment.NewLine;
					msgStr += "You can not save projects in the database using other than the selected libraries." + Environment.NewLine + Environment.NewLine;
					msgStr += "Do you want to proceed?";
					DialogResult result = MessageBox.Show(this, msgStr, "Create ADO.NET database schema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if(result != DialogResult.Yes) return;
					if(!CloseProject()) return;
				}
				AdoNetStore store = GetAdoNetStore();
				if(store != null)
				{
					CachedRepository cachedReporitory = (CachedRepository)project.Repository;
					cachedReporitory.Store = store;
					project.RemoveAllLibraries();
					using(LibraryManagementDialog dlg = new LibraryManagementDialog(project))
						dlg.ShowDialog(this);
					project.RegisterEntityTypes();

					Cursor = Cursors.WaitCursor;
					Application.DoEvents();
					try
					{
						store.DropDbSchema();
						store.CreateDbCommands(cachedReporitory);
						store.CreateDbSchema(cachedReporitory);
						project.Close();
						MessageBox.Show(this, "Database schema created successfully.", "Schema created", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					catch(Exception exc)
					{
						MessageBox.Show(this, "An error occured while creating database schema:" + Environment.NewLine + exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					finally
					{
						Cursor = Cursors.Default;
					}
				}
			}
		}


		private void testDataGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				if(project.ShapeTypes["RoundedBox"] == null || project.ShapeTypes["Polyline"] == null)
					MessageBox.Show(this, "Shape library 'GeneralShapes' is required for creating test data.", "Library missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				else
				{
					using(TestDataGeneratorDialog dlg = new TestDataGeneratorDialog(project))
					{
						DialogResult res = dlg.ShowDialog(this);
						if(res == DialogResult.OK)
							displayTabControl.SelectedTab = displayTabControl.TabPages[displayTabControl.TabCount - 1];
					}
				}
			}
			catch(Exception exc)
			{
				MessageBox.Show(this, exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


		private void nShapeEventMonitorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(eventMoniorForm == null)
			{
				eventMoniorForm = new EventMonitorForm();
				eventMoniorForm.FormClosed += eventMoniorForm_FormClosed;
				try
				{
					// Display
					eventMoniorForm.AddEventSource(CurrentDisplay);
					// Project + History
					eventMoniorForm.AddEventSource(project);
					eventMoniorForm.AddEventSource(project.History);
					// Repository
					eventMoniorForm.AddEventSource(cachedRepository);
					// DiagramSetController
					eventMoniorForm.AddEventSource(diagramSetController);
					// ToolSetController + ToolSetPresenter + Tools
					eventMoniorForm.AddEventSource(toolSetController);
					eventMoniorForm.AddEventSource(toolSetListViewPresenter);
					foreach(Tool tool in toolSetController.Tools)
						eventMoniorForm.AddEventSource(tool);
					// PropertyController + PropertyPresenter
					eventMoniorForm.AddEventSource(propertyController);
					eventMoniorForm.AddEventSource(propertyPresenter);

					eventMoniorForm.AddEventSource(layerController);
					eventMoniorForm.AddEventSource(layerPresenter);

					if(modelTreeController != null)
						eventMoniorForm.AddEventSource(modelTreeController);
					if(modelTreePresenter != null)
						eventMoniorForm.AddEventSource(modelTreePresenter);

					eventMoniorForm.Show();
				}
				catch(Exception exc)
				{
					MessageBox.Show(this, exc.Message, "Error while opening EventMonitor", MessageBoxButtons.OK, MessageBoxIcon.Error);
					eventMoniorForm.Close();
				}
			}
			else eventMoniorForm.Close();
			nShapeEventMonitorToolStripMenuItem.Checked = (eventMoniorForm != null);
		}


		private void eventMoniorForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			eventMoniorForm.FormClosed -= eventMoniorForm_FormClosed;
			eventMoniorForm.Dispose();
			eventMoniorForm = null;
			nShapeEventMonitorToolStripMenuItem.Checked = (eventMoniorForm != null);
		}

		#endregion


		#region [Private] Event Handler implementations - Menu item "Help"

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using(AboutBox dlg = new AboutBox())
				dlg.ShowDialog(this);
		}


		private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FileInfo helpFile = GetHelpFile();
			if(helpFile != null)
				Process.Start(new ProcessStartInfo(helpFile.FullName));
		}


		private DirectoryInfo GetHelpDir()
		{
			DirectoryInfo programDir = new DirectoryInfo(Application.StartupPath);
			return new DirectoryInfo(Path.Combine(programDir.Parent.FullName, "Documentation"));
		}


		private FileInfo GetHelpFile()
		{
			FileInfo helpFile = null;
			DirectoryInfo helpDir = GetHelpDir();
			if(helpDir.Exists)
			{
				foreach(FileInfo fileInfo in helpDir.GetFiles("NShape*.chm"))
				{
					if(helpFile == null || fileInfo.CreationTimeUtc > helpFile.CreationTimeUtc)
						helpFile = fileInfo;
				}
			}
			return helpFile;
		}

		#endregion


		#region [Private] Types

		private struct RepositoryInfo : IEquatable<RepositoryInfo>
		{

			public static readonly RepositoryInfo Empty;

			public const string XmlStoreTypeName = "XML";

			public const string SqlServerStoreTypeName = "SQL Server";

			public static bool operator ==(RepositoryInfo a, RepositoryInfo b)
			{
				return (a.location == b.location
					&& a.projectName == b.projectName
					&& a.computerName == b.computerName
					&& a.typeName == b.typeName);
			}

			public static bool operator !=(RepositoryInfo a, RepositoryInfo b) { return !(a == b); }

			public static Store CreateStore(RepositoryInfo repositoryInfo)
			{
				Store store = null;
				if(repositoryInfo.typeName == RepositoryInfo.XmlStoreTypeName)
				{
					store = new XmlStore()
					{
						DirectoryName = Path.GetDirectoryName(repositoryInfo.location),
						FileExtension = Path.GetExtension(repositoryInfo.location),
						LazyLoading = true
					};
				}
				else if(repositoryInfo.typeName == RepositoryInfo.SqlServerStoreTypeName)
				{
					store = new SqlStore();
					((SqlStore)store).DatabaseName = repositoryInfo.location;
					((SqlStore)store).ServerName = repositoryInfo.computerName;
				}
				else
				{
					Debug.Fail(string.Format("Unsupported {0} value '{1}'", typeof(RepositoryInfo).Name, repositoryInfo));
				}
				return store;
			}

			public override bool Equals(object obj)
			{
				return (obj is RepositoryInfo && ((RepositoryInfo)obj) == this);
			}

			public bool Equals(RepositoryInfo other)
			{
				return other == this;
			}

			public override int GetHashCode()
			{
				int hashCode = base.GetHashCode();
				if(location != null) hashCode ^= location.GetHashCode();
				if(projectName != null) hashCode ^= projectName.GetHashCode();
				if(computerName != null) hashCode ^= computerName.GetHashCode();
				if(typeName != null) hashCode ^= typeName.GetHashCode();
				return hashCode;
			}

			public RepositoryInfo(string projectName, string typeName, string serverName, string dataSource)
			{
				this.projectName = projectName;
				this.typeName = typeName;
				this.computerName = serverName;
				this.location = dataSource;
			}

			public string projectName;

			public string typeName;

			/// <summary>
			/// Contains the server name (in case of an AdoRepository) or the computer name (in case of a XmlStore).
			/// </summary>
			public string computerName;

			/// <summary>
			/// Contains the database' data source (in case of an AdoRepository) or the path to an XML file (in case of a XmlStore).
			/// </summary>
			public string location;

			static RepositoryInfo()
			{
				Empty.location = string.Empty;
				Empty.projectName = string.Empty;
				Empty.computerName = string.Empty;
				Empty.typeName = string.Empty;
			}
		}

		#endregion


		#region [Private] Constants

		private const string SqlServerProviderName = "SQL Server";
		private const string DefaultDatabaseName = "NShape";
		private const string DefaultServerName = ".\\SQLEXPRESS";
		private const string ConfigFileName = "Config.cfg";
		private const string ProjectFileExtension = ".nspj";
		private const string FileFilterXmlRepository = "NShape Designer Files|*.nspj;*.xml|XML Repository Files|*.xml|All Files|*.*";

		private const string QueryNode = "//{0}";
		private const string QueryNodeAttr = "//{0}[@{1}]";

		private const string NodeNameSettings = "Settings";
		private const string NodeNameLoadToolbarSettings = "LoadToolBarSettings";
		private const string NodeNameProjectDirectory = "ProjectDirectory";
		private const string NodeNameProjects = "Projects";
		private const string NodeNameProject = "Project";
		private const string NodeNameWindowSettings = "WindowSettings";

		private const string AttrNameValue = "Value";
		private const string AttrNamePath = "Path";
		private const string AttrNameName = "Name";
		private const string AttrNameRepositoryType = "RepositoryType";
		private const string AttrNameServerName = "ServerName";
		private const string AttrNameDataSource = "DataSource";
		private const string AttrNameState = "State";
		private const string AttrNamePositionX = "PositionX";
		private const string AttrNamePositionY = "PositionY";
		private const string AttrNameWidth = "Width";
		private const string AttrNameHeight = "Height";
		private const int RecentProjectsLimit = 15;

		private const string PercentFormatStr = "{0} %";
		private const string PointFormatStr = "{0}, {1}";
		private const string SizeFormatStr = "{0} x {1}";

		#endregion


		#region [Private] Fields

		private const int historyDropDownItemCount = 20;
		private const int recentProjectsItemCount = 25;

		private EventMonitorForm eventMoniorForm;
		private LayoutDialog layoutControlForm;
		private TemplateEditorDialog templateEditorDialog;

		private string nShapeConfigDirectory = Path.Combine(Path.Combine("dataweb", "NShape"), Application.ProductName);
		private string xmlStoreDirectory;
		private bool projectSaved = false;
		private int maxRepositoryVersion;

		private Point p;
		private int currHistoryPos;
		private Display currentDisplay;

		// display config
		private bool showGrid = true;
		private bool snapToGrid = true;
		private int gridSize = 20;
		private Color gridColor = Color.Gainsboro;
		private int snapDistance = 20;  //!!!	5;
		private ControlPointShape resizePointShape = ControlPointShape.Square;
		private ControlPointShape connectionPointShape = ControlPointShape.Circle;
		private int controlPointSize = 3;
		private bool showDefaultContextMenu = true;
		private bool hideDeniedMenuItems = false;
#if DEBUG
		private bool showCellOccupation = false;
		private bool showInvalidatedAreas = false;
#endif

		private string configFolder;
		private int zoom = 100;
		private bool highQuality = true;

		private List<RepositoryInfo> recentProjects = new List<RepositoryInfo>(recentProjectsItemCount);
		private bool loadToolStripLayoutOnStartup = true;

		// Specifies whether the project is regarded as changed from the user's perspective 
		// (loading libraries in an empty repository created at startup will set the repository's IsModified state to "True")
		private bool projectIsModified = false;

#if TdbRepository
		private const string fileFilterAllRepositories = "NShape Repository Files|*.xml;*.tdbd|XML Repository Files|*.xml|TurboDB Repository Databases|*.tdbd|All Files|*.*";
		private const string fileFilterTurboDBRepository = "TurboDB Repository Databases|*.tdbd|All Files|*.*";
#endif
		#endregion

		internal const int padding = 0;//20;
		internal const int weatherHeight = 80;	// 100;
		internal const int weatherImageGap = 4;
		internal const int weatherWindvaneGap = 8;
		internal const int weatherCalendarHeight = 12;
		internal const int calendarHeight = 16;
		internal const int calendarGap = 5;
		internal const int diameter = 200;
		internal const int actionsCurrCount = 4;	//5;
		internal const int actionsForeCount = 4;
		private string[] actionsCurrToolTipText = new string[] { Properties.Strings.ActionCurrGeneration, Properties.Strings.ActionCurrTransmission, Properties.Strings.ActionCurrTransformation, Properties.Strings.ActionCurrConsumption };
		private string[] actionsForeToolTipText = new string[] { Properties.Strings.ActionForeGeneration, Properties.Strings.ActionForeTransmission, Properties.Strings.ActionForeTransformation, Properties.Strings.ActionForeConsumption };

		private BoxBase weatherCurr = null;
		private FhisServices.Calendar weatherCurrCalendar = null;
		private FhisServices.Watch weatherCurrWatch = null;
		private PictureBase weatherCurrImage = null;
		private FhisServices.Thermometer weatherCurrThermometer = null;
		private FhisServices.Windvane weatherCurrWindvane = null;
		private FhisServices.Hygrometer weatherCurrHygrometer = null;
		private FhisServices.Barometer weatherCurrBarometer = null;

		private BoxBase weatherFore = null;
		private FhisServices.Calendar weatherForeCalendar = null;
		private FhisServices.Watch weatherForeWatch = null;
		private PictureBase weatherForeImage = null;
		private FhisServices.Thermometer weatherForeThermometer = null;
		private FhisServices.Windvane weatherForeWindvane = null;
		private FhisServices.Hygrometer weatherForeHygrometer = null;
		private FhisServices.Barometer weatherForeBarometer = null;

		private BoxBase situationCurr = null;
		private Shapes.State stateCurr = null;
		private Shapes.Actions actionsCurr = null;
		private FhisServices.Calendar currCalendar = null;
		private FhisServices.Watch currWatch = null;

		private BoxBase situationFore = null;
		private Shapes.State stateFore = null;
		private Shapes.Actions actionsFore = null;
		private FhisServices.Calendar foreCalendar = null;
		private FhisServices.Watch foreWatch = null;

		private ShapeType propType = null;
		private ShapeType triangleType = null;
		private ShapeType pictureType = null;
		private bool currDynaDown = false;
		private bool currStatDown = false;
		private bool foreDynaDown = false;
		private bool foreStatDown = false;

		private Weather currWeather = null;
		private Weather foreWeather = null;

		private bool emptyMapProvider = true;
		private void display_MapProviderChanged(object sender, EventArgs e)
		{	if(CurrentDisplay.MapProvider is GMap.NET.MapProviders.EmptyProvider)
			{	if(!emptyMapProvider)
				{	emptyMapProvider = true;

					weatherCurr.Children.Remove(weatherCurrBarometer);		weatherCurrBarometer = null;
					weatherCurr.Children.Remove(weatherCurrHygrometer);		weatherCurrHygrometer = null;
					weatherCurr.Children.Remove(weatherCurrWindvane);		weatherCurrWindvane = null;
					weatherCurr.Children.Remove(weatherCurrThermometer);	weatherCurrThermometer = null;
					weatherCurr.Children.Remove(weatherCurrImage);			weatherCurrImage = null;
					weatherCurr.Children.Remove(weatherCurrWatch);			weatherCurrWatch = null;
					weatherCurr.Children.Remove(weatherCurrCalendar);		weatherCurrCalendar = null;
					CurrentDisplay.DeleteShape(weatherCurr);				weatherCurr = null;

					weatherFore.Children.Remove(weatherForeBarometer);		weatherForeBarometer = null;
					weatherFore.Children.Remove(weatherForeHygrometer);		weatherForeHygrometer = null;
					weatherFore.Children.Remove(weatherForeWindvane);		weatherForeWindvane = null;
					weatherFore.Children.Remove(weatherForeThermometer);	weatherForeThermometer = null;
					weatherFore.Children.Remove(weatherForeImage);			weatherForeImage = null;
					weatherFore.Children.Remove(weatherForeWatch);			weatherForeWatch = null;
					weatherFore.Children.Remove(weatherForeCalendar);		weatherForeCalendar = null;
					CurrentDisplay.DeleteShape(weatherFore);				weatherFore = null;

					situationCurr.Children.Remove(currWatch);				currWatch = null;
					situationCurr.Children.Remove(currCalendar);			currCalendar = null;
					situationCurr.Children.Remove(actionsCurr.Acts.circleOuter);
					situationCurr.Children.Remove(stateCurr.DynamicProperties.circleOuter);
					CurrentDisplay.DeleteShape(situationCurr);				situationCurr = null;

					situationFore.Children.Remove(foreWatch);				foreWatch = null;
					situationFore.Children.Remove(foreCalendar);			foreCalendar = null;
					situationFore.Children.Remove(actionsFore.Acts.circleOuter);
					situationFore.Children.Remove(stateFore.DynamicProperties.circleOuter);
					CurrentDisplay.DeleteShape(situationFore);				situationFore = null;

					ShowGrid = showGridMenuItem.Checked;
				}
			}
			else
			{	if(emptyMapProvider)
				{	emptyMapProvider = false;

					CreateVehicle();
				}
			}
		}

		private void CreateVehicle()
		{	ShapeType actionType = null
				, circleType = null
				, boxType = null
				, polylineType = null
				, calendarType = null
				, watchType = null
				, windvaneType = null
				, hygrometerType = null
				, thermometerType = null
				, barometerType = null
				;

			foreach(ShapeType shapeType in project.ShapeTypes)
				switch(shapeType.FullName)
				{	case "FhisShapes.Свойство":
						this.propType = shapeType;
						break;
					case "FhisShapes.Действие":
						actionType = shapeType;
						break;
					case "FhisPrimitives.Треугольник":
						this.triangleType = shapeType;
						break;
					case "FhisPrimitives.Круг":
						circleType = shapeType;
						break;
					case "FhisPrimitives.Прямоугольник":
						boxType = shapeType;
						break;
					case "FhisPrimitives.Полилиния":
						polylineType = shapeType;
						break;
					case "FhisPrimitives.Изображение":
						this.pictureType = shapeType;
						break;
					case "FhisServices.Календарь":
						calendarType = shapeType;
						break;
					case "FhisServices.Часы":
						watchType = shapeType;
						break;
					case "FhisServices.Флюгер":
						windvaneType = shapeType;
						break;
					case "FhisServices.Гигрометр":
						hygrometerType = shapeType;
						break;
					case "FhisServices.Термометр":
						thermometerType = shapeType;
						break;
					case "FhisServices.Барометр":
						barometerType = shapeType;
						break;
				}
			if(this.propType == null
			|| actionType == null
			|| this.triangleType == null
			|| circleType == null
			|| boxType == null
			|| polylineType == null
			|| this.pictureType == null
			|| calendarType == null
			|| watchType == null
			|| windvaneType == null
			|| hygrometerType == null
			|| thermometerType == null
			|| barometerType == null)
				return;
		//----------------------------------
			Shapes.State.CircleType = circleType;
			Shapes.State.PropType = propType;

			Shapes.Actions.CircleType = circleType;
			Shapes.Actions.ActionType = actionType;

		//----------------------------------
		// Погода (текущая)
			weatherCurr = (BoxBase)boxType.CreateInstance();
			weatherCurr.IsToolbarHidden = true;
			weatherCurr.FillStyle = project.Design.FillStyles.White;
			weatherCurr.Height = weatherHeight;
			weatherCurr.ToolTipText = Properties.Strings.WeatherCurr;
			CurrentDisplay.InsertShape(weatherCurr);

			weatherCurrCalendar = (FhisServices.Calendar)calendarType.CreateInstance();
			weatherCurrCalendar.IsToolbarHidden = true;
			weatherCurrCalendar.Width = weatherCurr.Height - weatherCalendarHeight - 2;
			weatherCurrCalendar.Height = weatherCalendarHeight;
			weatherCurr.Children.Add(weatherCurrCalendar);

			weatherCurrWatch = (FhisServices.Watch)watchType.CreateInstance();
			weatherCurrWatch.IsToolbarHidden = true;
			weatherCurrWatch.Diameter = weatherCurr.Height - weatherCurrCalendar.Height - 2;
			weatherCurrWatch.HasSeconds = false;
			weatherCurr.Children.Add(weatherCurrWatch);

			weatherCurrImage = (PictureBase)pictureType.CreateInstance();
			weatherCurrImage.IsToolbarHidden = true;
			weatherCurrImage.LineStyle = project.Design.LineStyles.None;
			weatherCurrImage.Height = weatherHeight - weatherImageGap*2;
			weatherCurrImage.Width = weatherCurrImage.Height;
			weatherCurr.Children.Add(weatherCurrImage);

			weatherCurrThermometer = (FhisServices.Thermometer)thermometerType.CreateInstance();
			weatherCurrThermometer.TemperatureMinimum = -30f;
			weatherCurrThermometer.TemperatureMaximum = 40f;
			weatherCurrThermometer.Temperature = 0f;
			weatherCurrThermometer.Height = weatherCurr.Height - 2;
			weatherCurr.Children.Add(weatherCurrThermometer);

			weatherCurrWindvane = (FhisServices.Windvane)windvaneType.CreateInstance();
			weatherCurrWindvane.IsToolbarHidden = true;
			weatherCurrWindvane.Diameter = weatherCurr.Height - weatherWindvaneGap*2;
			weatherCurr.Children.Add(weatherCurrWindvane);

			weatherCurrHygrometer = (FhisServices.Hygrometer)hygrometerType.CreateInstance();
			weatherCurrHygrometer.IsToolbarHidden = true;
			weatherCurrHygrometer.Humidity = 60f;
			weatherCurr.Children.Add(weatherCurrHygrometer);

			weatherCurrBarometer = (FhisServices.Barometer)barometerType.CreateInstance();
			weatherCurrBarometer.IsToolbarHidden = true;
			weatherCurrBarometer.Diameter = weatherCurrWatch.Diameter;
			weatherCurr.Children.Add(weatherCurrBarometer);

		//----------------------------------
		// Погода (прогноз)
			weatherFore = (BoxBase)boxType.CreateInstance();
			weatherFore.IsToolbarHidden = true;
			weatherFore.FillStyle = project.Design.FillStyles.White;
			weatherFore.Height = weatherHeight;
			weatherFore.ToolTipText = Properties.Strings.WeatherFore;
			CurrentDisplay.InsertShape(weatherFore);

			weatherForeCalendar = (FhisServices.Calendar)calendarType.CreateInstance();
			weatherForeCalendar.IsToolbarHidden = true;
			weatherForeCalendar.Width = weatherFore.Height - weatherCalendarHeight - 2;
			weatherForeCalendar.Height = weatherCalendarHeight;
			weatherFore.Children.Add(weatherForeCalendar);

			weatherForeWatch = (FhisServices.Watch)watchType.CreateInstance();
			weatherForeWatch.IsToolbarHidden = true;
			weatherForeWatch.Diameter = weatherFore.Height - weatherForeCalendar.Height - 2;
			weatherForeWatch.HasSeconds = false;
			weatherFore.Children.Add(weatherForeWatch);

			weatherForeImage = (PictureBase)pictureType.CreateInstance();
			weatherForeImage.IsToolbarHidden = true;
			weatherForeImage.LineStyle = project.Design.LineStyles.None;
			weatherForeImage.Height = weatherHeight - weatherImageGap*2;
			weatherForeImage.Width = weatherForeImage.Height;
			weatherFore.Children.Add(weatherForeImage);

			weatherForeThermometer = (FhisServices.Thermometer)thermometerType.CreateInstance();
			weatherForeThermometer.TemperatureMinimum = -30f;
			weatherForeThermometer.TemperatureMaximum = 40f;
			weatherForeThermometer.Temperature = 0f;
			weatherForeThermometer.Height = weatherFore.Height - 2;
			weatherFore.Children.Add(weatherForeThermometer);

			weatherForeWindvane = (FhisServices.Windvane)windvaneType.CreateInstance();
			weatherForeWindvane.IsToolbarHidden = true;
			weatherForeWindvane.Diameter = weatherFore.Height - weatherWindvaneGap*2;
			weatherFore.Children.Add(weatherForeWindvane);

			weatherForeHygrometer = (FhisServices.Hygrometer)hygrometerType.CreateInstance();
			weatherForeHygrometer.IsToolbarHidden = true;
			weatherForeHygrometer.Humidity = 70f;
			weatherFore.Children.Add(weatherForeHygrometer);

			weatherForeBarometer = (FhisServices.Barometer)barometerType.CreateInstance();
			weatherForeBarometer.IsToolbarHidden = true;
			weatherForeBarometer.Diameter = weatherForeWatch.Diameter;
			weatherFore.Children.Add(weatherForeBarometer);

		//----------------------------------
		// Ситуация (текущая)
			situationCurr = (BoxBase)boxType.CreateInstance();
			situationCurr.IsToolbarHidden = true;
			situationCurr.FillStyle = project.Design.FillStyles.White;
			situationCurr.Height = diameter;
			situationCurr.ToolTipText = Properties.Strings.SituationCurr;
			CurrentDisplay.InsertShape(situationCurr);
		//----------------------------------
		// Свойства (текущие)
			stateCurr = new Shapes.State(project, situationCurr, diameter);
			stateCurr.ToolTipText = Properties.Strings.StateCurr;
		//----------------------------------
		// Действия/рекомендации (текущие)
			actionsCurr = new Shapes.Actions(project, situationCurr, diameter);
			actionsCurr.ToolTipText = Properties.Strings.ActionsCurr;
		//----------------------------------
		// Календарь (текущий)
			currCalendar = (FhisServices.Calendar)calendarType.CreateInstance();
			currCalendar.IsToolbarHidden = true;
			currCalendar.Width = diameter/2;
			currCalendar.Height = calendarHeight;
			situationCurr.Children.Add(currCalendar);
		// Часы (текущие)
			currWatch = (FhisServices.Watch)watchType.CreateInstance();
			currWatch.IsToolbarHidden = true;
			currWatch.Diameter = diameter/2;
			situationCurr.Children.Add(currWatch);

		//----------------------------------
		// Ситуация (прогнозная)
			situationFore = (BoxBase)boxType.CreateInstance();
			situationFore.IsToolbarHidden = true;
			situationFore.FillStyle = project.Design.FillStyles.White;
			situationFore.Height = diameter;
			situationFore.ToolTipText = Properties.Strings.SituationFore;
			CurrentDisplay.InsertShape(situationFore);
		//----------------------------------
		// Свойства (прогнозные)
			stateFore = new Shapes.State(project, situationFore, diameter);
			stateFore.ToolTipText = Properties.Strings.StateFore;
		//----------------------------------
		// Действия/рекомендации (прогнозные)
			actionsFore = new Shapes.Actions(project, situationFore, diameter);
			actionsFore.ToolTipText = Properties.Strings.ActionsFore;
		//----------------------------------
		// Календарь (прогнозный)
			foreCalendar = (FhisServices.Calendar)calendarType.CreateInstance();
			foreCalendar.IsToolbarHidden = true;
			foreCalendar.Width = diameter/2;
			foreCalendar.Height = calendarHeight;
			situationFore.Children.Add(foreCalendar);
		// Часы (прогнозные)
			foreWatch = (FhisServices.Watch)watchType.CreateInstance();
			foreWatch.IsToolbarHidden = true;
			foreWatch.Diameter = diameter/2;
			foreWatch.HasSeconds = false;
			situationFore.Children.Add(foreWatch);
		//----------------------------------
			foreach(Shape selectedShape in CurrentDisplay.SelectedShapes)
				CurrentDisplay.UnselectShape(selectedShape);

			ChangeVehicle();

			timerClock.Start();

			timerEnergyFlow.Interval = energyFlowInterval;
			timerEnergyFlow.Start();

			CurrentDisplay.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
			emptyMapProvider = false;
			ShowGrid = false;
		}

		internal void ChangeVehicle()
		{	if(this.WindowState == FormWindowState.Minimized
			|| CurrentDisplay.MapProvider is GMap.NET.MapProviders.EmptyProvider)
				return;

			Diagram diagram = CurrentDisplay.Diagram;
			ILineStyle staticLineStyle = staticPropertyBorders ? project.Design.LineStyles.Normal : project.Design.LineStyles.None
				, dynamicLineStyle = dynamicPropertyBorders ? project.Design.LineStyles.Normal : project.Design.LineStyles.None
				;
			((IDiagramPresenter)CurrentDisplay).SuspendUpdate();

			// Свойства (текущие)
			//  динамические
			if(stateCurr.DynamicProperties.Count > CurrentDisplay.Diagram.DynamicPropsCount)
				for(int i = stateCurr.DynamicProperties.Count - 1, n = CurrentDisplay.Diagram.DynamicPropsCount; i >= n; --i)
					stateCurr.DynamicProperties.RemoveProp(i);
			else if(stateCurr.DynamicProperties.Count < CurrentDisplay.Diagram.DynamicPropsCount)
				for(int i = stateCurr.DynamicProperties.Count, n = CurrentDisplay.Diagram.DynamicPropsCount; i < n; ++i)
					ChangeFillStyleRandom(stateCurr.DynamicProperties.AddProp(i, dynamicLineStyle), fillColorNames);
			//  статические
			if(stateCurr.StaticProperties.Count > CurrentDisplay.Diagram.StaticPropsCount)
				for(int i = stateCurr.StaticProperties.Count - 1, n = CurrentDisplay.Diagram.StaticPropsCount; i >= n; --i)
					stateCurr.StaticProperties.RemoveProp(i);
			else if(stateCurr.StaticProperties.Count < CurrentDisplay.Diagram.StaticPropsCount)
				for(int i = stateCurr.StaticProperties.Count, n = CurrentDisplay.Diagram.StaticPropsCount; i < n; ++i)
					ChangeFillStyleRandom(stateCurr.StaticProperties.AddProp(i, staticLineStyle), fillColorNames);

			// Свойства (прогнозные)
			//  динамические
			if(stateFore.DynamicProperties.Count > CurrentDisplay.Diagram.DynamicPropsCount)
				for(int i = stateFore.DynamicProperties.Count - 1, n = CurrentDisplay.Diagram.DynamicPropsCount; i >= n; --i)
					stateFore.DynamicProperties.RemoveProp(i);
			else if(stateFore.DynamicProperties.Count < CurrentDisplay.Diagram.DynamicPropsCount)
				for(int i = stateFore.DynamicProperties.Count, n = CurrentDisplay.Diagram.DynamicPropsCount; i < n; ++i)
					ChangeFillStyleRandom(stateFore.DynamicProperties.AddProp(i, dynamicLineStyle), fillColorNames2);
			//  статические
			if(stateFore.StaticProperties.Count > CurrentDisplay.Diagram.StaticPropsCount)
				for(int i = stateFore.StaticProperties.Count - 1, n = CurrentDisplay.Diagram.StaticPropsCount; i >= n; --i)
					stateFore.StaticProperties.RemoveProp(i);
			else if(stateFore.StaticProperties.Count < CurrentDisplay.Diagram.StaticPropsCount)
				for(int i = stateFore.StaticProperties.Count, n = CurrentDisplay.Diagram.StaticPropsCount; i < n; ++i)
					ChangeFillStyleRandom(stateFore.StaticProperties.AddProp(i, staticLineStyle), fillColorNames2);

			int bodyHeight = 20, bodyLen = 20, arrowLen = 16, dx = bodyLen + arrowLen, dy = bodyHeight + 8, ax, ay, idx;

			// Действия (текущие)
//?			if(actionsCurr.Acts.Count > actionsCurrCount)
//?				for(int i = actionsCurr.Acts.Count - 1, n = actionsCurrCount; i >= n; --i)
//?					actionsCurr.Acts.RemoveAction(i);
//?			else 
			if(actionsCurr.Acts.Count < actionsCurrCount)
				for(int i = 0, n = actionsCurrCount; i < n; ++i)
					actionsCurr.Acts.AddAction(i).ToolTipText = actionsCurrToolTipText[i];
			ax = actionsCurr.X - actionsCurr.Diameter/2 + dx;
			ay = actionsCurr.Y - actionsCurr.Diameter/2 + dy;
			idx = 0;
			for(int i = 0, n = actionsCurrCount; i < n; ++i)
			{
				RectangleBase act = actionsCurr.Acts.Acts[i];
				act.MoveControlPointTo(1, ax + bodyLen + arrowLen, ay + bodyHeight/2, ResizeModifiers.None);
				act.MoveControlPointTo(2, ax + bodyLen, ay, ResizeModifiers.None);
				act.MoveControlPointTo(3, ax + bodyLen, ay + bodyHeight, ResizeModifiers.None);
				act.MoveControlPointTo(4, ax, ay + bodyHeight/2, ResizeModifiers.None);
				act.Angle = 0;
				act.ToolTipText = actionsForeToolTipText[idx++];
				ChangeFillStyleRandom(act, fillColorNames);
				ax += dx;
				ay += dy;
			}

			// Действия (прогнозные)
//?			if(actionsFore.Acts.Count > actionsForeCount)
//?				for(int i = actionsFore.Acts.Count - 1, n = actionsForeCount; i >= n; --i)
//?					actionsFore.Acts.RemoveAction(i);
//?			else
			if(actionsFore.Acts.Count < actionsForeCount)
				for(int i = 0, n = actionsForeCount; i < n; ++i)
					actionsFore.Acts.AddAction(i).ToolTipText = actionsForeToolTipText[i];
			ax = actionsFore.X - actionsFore.Diameter/2 + dx;
			ay = actionsFore.Y - actionsFore.Diameter/2 + dy;
			idx = 0;
			for(int i = 0, n = actionsForeCount; i < n; ++i)
			{
				RectangleBase act = actionsFore.Acts.Acts[i];
				act.MoveControlPointTo(1, ax + bodyLen + arrowLen, ay + bodyHeight/2, ResizeModifiers.None);
				act.MoveControlPointTo(2, ax + bodyLen, ay, ResizeModifiers.None);
				act.MoveControlPointTo(3, ax + bodyLen, ay + bodyHeight, ResizeModifiers.None);
				act.MoveControlPointTo(4, ax, ay + bodyHeight/2, ResizeModifiers.None);
				act.Angle = 0;
				act.ToolTipText = actionsForeToolTipText[idx++];
				ChangeFillStyleRandom(act, fillColorNames2);
				ax += dx;
				ay += dy;
			}


			int		boxX1
				,	boxX2
				,	boxY1
				,	boxY2
				,	boxYC

				,	weatherCurrX1 = diagram.X
				,	weatherCurrX2 = diagram.X + diagram.Width/2
				,	weatherCurrY1 = statePosTop ? diagram.Y + diagram.Height - 1 - weatherHeight : diagram.Y
				,	weatherCurrY2 = weatherCurrY1 + weatherHeight

				,	weatherForeX1 = weatherCurrX2
				,	weatherForeX2 = diagram.X + diagram.Width
				,	weatherForeY1 = weatherCurrY1
				,	weatherForeY2 = weatherCurrY2

				,	stateCurrX1 = diagram.X
				,	stateCurrX2 = diagram.X + diagram.Width/2
				,	stateCurrY1 = statePosTop ? diagram.Y : diagram.Y + diagram.Height - 1 - diameter
				,	stateCurrY2 = stateCurrY1 + diameter

				,	stateForeX1 = stateCurrX2
				,	stateForeX2 = diagram.X + diagram.Width
				,	stateForeY1 = stateCurrY1
				,	stateForeY2 = stateCurrY2
				;

			int		weatherCurrCalendarWidth = weatherCurrCalendar.Width
				,	weatherCurrCalendarHeight = weatherCurrCalendar.Height
				,	weatherCurrWatchDiameter = weatherCurrWatch.Diameter
				,	weatherCurrWindvaneDiameter = weatherCurrWindvane.Diameter
				,	weatherCurrBarometerDiameter = weatherCurrBarometer.Diameter

				,	weatherForeCalendarWidth = weatherForeCalendar.Width
				,	weatherForeCalendarHeight = weatherForeCalendar.Height
				,	weatherForeWatchDiameter = weatherForeWatch.Diameter
				,	weatherForeWindvaneDiameter = weatherForeWindvane.Diameter
				,	weatherForeBarometerDiameter = weatherForeBarometer.Diameter
				;
		//	1	2	3
		//	4		5
		//	6	7	8
			// Погодные условия (текущие)
			boxX1 = weatherCurrX1;
			boxX2 = weatherCurrX2;
			boxY1 = weatherCurrY1;
			boxY2 = weatherCurrY2;
			boxYC = (int)Math.Round((boxY1 + boxY2 + 1)/2f);
			weatherCurr.MoveTo((boxX1 + boxX2)/2, (boxY1 + boxY2)/2);
			weatherCurr.MoveControlPointTo(1, boxX1, boxY1, ResizeModifiers.None);
			weatherCurr.MoveControlPointTo(3, boxX2, boxY1, ResizeModifiers.None);
			weatherCurr.MoveControlPointTo(6, boxX1, boxY2, ResizeModifiers.None);

			weatherCurrCalendar.DrawFace(boxX1 + weatherCurrCalendarWidth/2, boxY1 + weatherCurrCalendarHeight/2, weatherCurr.Height - weatherCalendarHeight - 2, weatherCalendarHeight);
			weatherCurrWatch.Diameter = weatherCurrWatchDiameter;
			weatherCurrWatch.DrawFace(boxX1 + (int)Math.Round(weatherCurrWatchDiameter/2f) + 1, boxYC + weatherCurrCalendarHeight/2);

			weatherCurrImage.X = boxX1 + weatherCurrWatchDiameter + 60;
			weatherCurrImage.Y = boxYC;
			weatherCurrImage.Height = weatherCurr.Height - weatherImageGap*2;
			weatherCurrImage.Width = weatherCurrImage.Height;

			weatherCurrThermometer.DrawFace(boxX1 + weatherCurrWatchDiameter + weatherCurrImage.Width + 60, boxYC, weatherCurr.Height - 2);
			weatherCurrThermometer.Temperature = weatherCurrThermometer.Temperature;	// Выравниваем столбик

			weatherCurrWindvane.Diameter = weatherCurrWindvaneDiameter;
			weatherCurrWindvane.DrawFace(boxX1 + weatherCurrWatchDiameter + weatherCurrImage.Width + 60 + 70, boxYC);

			weatherCurrHygrometer.DrawFace(boxX1 + weatherCurrWatchDiameter + weatherCurrImage.Width + 60 + 70 + weatherCurr.Height, boxYC, weatherCurr.Height - 16);

			weatherCurrBarometer.Diameter = weatherCurrBarometerDiameter;
			weatherCurrBarometer.DrawFace(boxX1 + weatherCurrWatchDiameter + weatherCurrImage.Width + 60 + 70 + 80 + weatherCurr.Height, boxYC);

			// Погодные условия (прогноз)
			boxX1 = weatherForeX1;
			boxX2 = weatherForeX2;
			boxY1 = weatherForeY1;
			boxY2 = weatherForeY2;
			boxYC = (int)Math.Round((boxY1 + boxY2 + 1)/2f);
			weatherFore.MoveTo((boxX1 + boxX2)/2, (boxY1 + boxY2)/2);
			weatherFore.MoveControlPointTo(1, boxX1, boxY1, ResizeModifiers.None);
			weatherFore.MoveControlPointTo(3, boxX2, boxY1, ResizeModifiers.None);
			weatherFore.MoveControlPointTo(6, boxX1, boxY2, ResizeModifiers.None);

			weatherForeCalendar.DrawFace(boxX1 + weatherForeCalendarWidth/2, boxY1 + weatherForeCalendarHeight/2, weatherFore.Height - weatherCalendarHeight - 2, weatherCalendarHeight);
			weatherForeWatch.Diameter = weatherForeWatchDiameter;
			weatherForeWatch.DrawFace(boxX1 + (int)Math.Round(weatherForeWatchDiameter/2f) + 1, boxYC + weatherForeCalendarHeight/2);

			weatherForeImage.X = boxX1 + weatherForeWatchDiameter + 60;
			weatherForeImage.Y = boxYC;
			weatherForeImage.Height = weatherFore.Height - weatherImageGap*2;
			weatherForeImage.Width = weatherForeImage.Height;

			weatherForeThermometer.DrawFace(boxX1 + weatherForeWatchDiameter + weatherForeImage.Width + 60, boxYC, weatherFore.Height - 2);
			weatherForeThermometer.Temperature = weatherForeThermometer.Temperature;	// Выравниваем столбик

			weatherForeWindvane.Diameter = weatherForeWindvaneDiameter;
			weatherForeWindvane.DrawFace(boxX1 + weatherForeWatchDiameter + weatherForeImage.Width + 60 + 70, boxYC);

			weatherForeHygrometer.DrawFace(boxX1 + weatherForeWatchDiameter + weatherForeImage.Width + 60 + 70 + weatherFore.Height, boxYC, weatherFore.Height - 16);

			weatherForeBarometer.Diameter = weatherForeBarometerDiameter;
			weatherForeBarometer.DrawFace(boxX1 + weatherForeWatchDiameter + weatherForeImage.Width + 60 + 70 + 80 + weatherFore.Height, boxYC);

			//		1
			//		5
			//	2	3	4
			int stateCurrDiameter = stateCurr.Diameter
				, actionsCurrDiameter = actionsCurr.Diameter
				, currCalendarWidth = currCalendar.Width
				, currCalendarHeight = currCalendar.Height
				, currWatchDiameter = currWatch.Diameter
				, stateForeDiameter = stateFore.Diameter
				, actionsForeDiameter = actionsFore.Diameter
				, foreWatchDiameter = foreWatch.Diameter
				, foreCalendarWidth = foreCalendar.Width
				, foreCalendarHeight = foreCalendar.Height
				;
			// Ситуация (текущая)
			boxX1 = stateCurrX1;
			boxX2 = stateCurrX2;
			boxY1 = stateCurrY1;
			boxY2 = stateCurrY2;
			boxYC = (stateForeY1 + stateForeY2)/2;//stateCurrY2 - diameter/2;

			situationCurr.MoveTo((boxX1 + boxX2)/2, boxYC);
			situationCurr.MoveControlPointTo(1, boxX1, boxY1, ResizeModifiers.None);
			situationCurr.MoveControlPointTo(3, boxX2, boxY1, ResizeModifiers.None);
			situationCurr.MoveControlPointTo(6, boxX1, boxY2, ResizeModifiers.None);
			// Свойства (текущие)
			stateCurr.X = boxX1 + diameter;
			stateCurr.Y = boxYC;
			stateCurr.Diameter = stateCurrDiameter;
			// Действия (текущие)
			actionsCurr.X = boxX1 + diameter + stateCurr.Diameter;
			actionsCurr.Y = boxYC;
			actionsCurr.Diameter = actionsCurrDiameter;
			// Календарь (текущий)
			currCalendar.Width = currCalendarWidth;
			currCalendar.Height = currCalendarHeight;
			currCalendar.DrawFace(boxX1 + currWatchDiameter/2, boxY1 + currCalendar.Height/2 + calendarGap);
			// Часы (текущие)
			currWatch.Diameter = currWatchDiameter;
			currWatch.DrawFace(boxX1 + currWatchDiameter/2, boxY1 + currCalendar.Height + calendarGap*2 + diameter/4);

			// Ситуация (прогнозная)
			boxX1 = stateForeX1;
			boxX2 = stateForeX2;
			boxY1 = stateForeY1;
			boxY2 = stateForeY2;
			boxYC = (stateForeY1 + stateForeY2)/2;//stateForeY2 - diameter/2;

			situationFore.MoveTo((boxX1 + boxX2)/2, boxYC);
			situationFore.MoveControlPointTo(1, boxX1, boxY1, ResizeModifiers.None);
			situationFore.MoveControlPointTo(3, boxX2, boxY1, ResizeModifiers.None);
			situationFore.MoveControlPointTo(6, boxX1, boxY2, ResizeModifiers.None);
			// Свойства (прогнозные)
			stateFore.X = boxX1 + diameter;
			stateFore.Y = boxYC;
			stateFore.Diameter = stateForeDiameter;
			// Действия (прогнозные)
			actionsFore.X = boxX1 + diameter + stateFore.Diameter;
			actionsFore.Y = boxYC;
			actionsFore.Diameter = actionsForeDiameter;
			// Календарь (прогнозный)
			foreCalendar.Width = foreCalendarWidth;
			foreCalendar.Height = foreCalendarHeight;
			foreCalendar.DrawFace(boxX1 + currWatchDiameter/2, boxY1 + foreCalendar.Height/2 + calendarGap);
			// Часы (прогнозные)
			foreWatch.Diameter = foreWatchDiameter;
			foreWatch.DrawFace(boxX1 + currWatchDiameter/2, boxY1 + foreCalendar.Height + calendarGap*2 + diameter/4);

			// Обновляем время
			currTime = DateTime.Now;
			ChangeCurrTime(currTime = currTime.AddMilliseconds(currTime.Millisecond > 500 ? 1000 - currTime.Millisecond : -currTime.Millisecond));
			ChangeForeTime(foreTime = currTime + weatherForecastTimeSpan);

			if(weatherCurrTime == DateTime.MinValue)
			{	weatherCurrTime = currTime;
				weatherForeTime = currTime + weatherForecastTimeSpan;
//#if DEBUG
				GetWeather(currTime);
//#else
//				GetWeather();
//#endif
			}

			((IDiagramPresenter)CurrentDisplay).ResumeUpdate();
		}

		internal void ChangeCurrTime(DateTime time)
		{	if(CurrentDisplay.MapProvider is GMap.NET.MapProviders.EmptyProvider)
				return;

			Diagram diagram = CurrentDisplay.Diagram;
			((IDiagramPresenter)CurrentDisplay).SuspendUpdate();

			currCalendar.Date = time.Date;
			currWatch.Time = time;
			weatherCurrCalendar.Date = weatherCurrTime.Date;
			weatherCurrWatch.Time = weatherCurrTime;

			ILineStyle staticLineStyle = staticPropertyBorders ? project.Design.LineStyles.Normal : project.Design.LineStyles.None
				, dynamicLineStyle = dynamicPropertyBorders ? project.Design.LineStyles.Normal : project.Design.LineStyles.None
				;

			lock(weatherCurr)
			{	if(currWeather != null)
				{	weatherCurrImage.Image = currWeather.Image;
					weatherCurrImage.ToolTipText = currWeather.SymbolText;
					weatherCurrThermometer.Temperature = currWeather.Temperature;
					weatherCurrWindvane.UseColors = useWindvaneColors;
					weatherCurrWindvane.SetWind(currWeather.WindDirection, currWeather.WindSpeed);
					weatherCurrHygrometer.Humidity = currWeather.Humidity;
					weatherCurrBarometer.PressureHPa = currWeather.Pressure;
				}

				int bodyHeight = 20, dy = bodyHeight + 8;
				List<int> addActs = new List<int>()
					, removeActs = new List<int>()
					;
				if(time.Second % 2 == 0)
				{	if(ChangeFillStyle(stateCurr.DynamicProperties.circleIntegralProps, currDynaDown, fillColorNames))
						currDynaDown = !currDynaDown;
					foreach(KeyValuePair<int, TriangleBase> prop in stateCurr.DynamicProperties.Props)
					{	prop.Value.LineStyle = dynamicLineStyle;
						ChangeFillStyleRandom(prop.Value, fillColorNames, true);
					}
					if(ChangeFillStyle(stateFore.StaticProperties.circleIntegralProps, foreStatDown, fillColorNames2))
						foreStatDown = !foreStatDown;
					foreach(KeyValuePair<int, TriangleBase> prop in stateFore.StaticProperties.Props)
					{	prop.Value.LineStyle = staticLineStyle;
						ChangeFillStyleRandom(prop.Value, fillColorNames2, true);
					}
					switch(colorsGrouping)
					{	case ColorsGrouping.ByColors:
							stateCurr.DynamicProperties.ColorsGroup(project.Design.LineStyles, dynamicLineStyle, GetFillStyles(fillColorNames[emergencyColors ? 0 : 1]));
							stateFore.StaticProperties.ColorsGroup(project.Design.LineStyles, staticLineStyle, GetFillStyles(fillColorNames2[emergencyColors ? 0 : 1]));
							break;
						case ColorsGrouping.ByPercent:
							stateCurr.DynamicProperties.ColorsGroup();
							stateFore.StaticProperties.ColorsGroup();
							break;
					}

					//		2
					//	4	5	1
					//		3
					foreach(KeyValuePair<int, RectangleBase> act in actionsCurr.Acts.Acts)
					{
						if(act.Key > 9)
							continue;

						ChangeFillStyleRandom(act.Value, fillColorNames);
						if(act.Value.FillStyle.Name == RepairNeedColor)			// Нужен ремент
							addActs.Add(act.Key);
						else													// Ремон не нужен
							removeActs.Add(act.Key);
					}
					foreach(int index in removeActs)
						if(actionsCurr.Acts.Acts.ContainsKey(index + 10))
							actionsCurr.Acts.RemoveAction(index + 10);
					foreach(int index in addActs)
					{	if(actionsCurr.Acts.Acts.ContainsKey(index + 10))
							continue;

						RectangleBase act = actionsCurr.Acts.Acts[index]
							, repairAct = actionsCurr.Acts.AddAction(index + 10)
							;

						Point p1 = act.GetControlPointPosition(1)
							, p2 = act.GetControlPointPosition(2)
							, p3 = act.GetControlPointPosition(3)
							, p4 = act.GetControlPointPosition(4)
							, p5 = act.GetControlPointPosition(5)
							;
						repairAct.MoveTo(p5.X, p5.Y);
						repairAct.MoveControlPointTo(1, p1.X, p1.Y + dy, ResizeModifiers.None);
						repairAct.MoveControlPointTo(2, p2.X, p2.Y + dy, ResizeModifiers.None);
						repairAct.MoveControlPointTo(3, p3.X, p3.Y + dy, ResizeModifiers.None);
						repairAct.MoveControlPointTo(4, p4.X, p4.Y + dy, ResizeModifiers.None);
						repairAct.Angle = 0;
						repairAct.ToolTipText = act.ToolTipText + Properties.Strings.SuffixRepair;
						repairAct.FillStyle = project.Design.FillStyles[RepairProcColor];
					}
				}
				else
				{	if(ChangeFillStyle(stateCurr.StaticProperties.circleIntegralProps, currStatDown, fillColorNames))
						currStatDown = !currStatDown;
					foreach(KeyValuePair<int, TriangleBase> prop in stateCurr.StaticProperties.Props)
					{	prop.Value.LineStyle = staticLineStyle;
						ChangeFillStyleRandom(prop.Value, fillColorNames, true);
					}
					if(ChangeFillStyle(stateFore.DynamicProperties.circleIntegralProps, foreDynaDown, fillColorNames2))
						foreDynaDown = !foreDynaDown;
					foreach(KeyValuePair<int, TriangleBase> prop in stateFore.DynamicProperties.Props)
					{	prop.Value.LineStyle = dynamicLineStyle;
						ChangeFillStyleRandom(prop.Value, fillColorNames2, true);
					}
					switch(colorsGrouping)
					{	case ColorsGrouping.ByColors:
							stateCurr.StaticProperties.ColorsGroup(project.Design.LineStyles, staticLineStyle, GetFillStyles(fillColorNames[emergencyColors ? 0 : 1]));
							stateFore.DynamicProperties.ColorsGroup(project.Design.LineStyles, dynamicLineStyle, GetFillStyles(fillColorNames2[emergencyColors ? 0 : 1]));
							break;
						case ColorsGrouping.ByPercent:
							stateCurr.StaticProperties.ColorsGroup();
							stateFore.DynamicProperties.ColorsGroup();
							break;
					}

					//		2
					//	4	5	1
					//		3
					foreach(KeyValuePair<int, RectangleBase> act in actionsFore.Acts.Acts)
					{
						if(act.Key > 9)
							continue;

						ChangeFillStyleRandom(act.Value, fillColorNames2);
						if(act.Value.FillStyle.Name == RepairNeedColor)			// Нужен ремент
							addActs.Add(act.Key);
						else													// Ремон не нужен
							removeActs.Add(act.Key);
					}
					foreach(int index in removeActs)
						if(actionsFore.Acts.Acts.ContainsKey(index + 10))
							actionsFore.Acts.RemoveAction(index + 10);
					foreach(int index in addActs)
					{	if(actionsFore.Acts.Acts.ContainsKey(index + 10))
							continue;

						RectangleBase act = actionsFore.Acts.Acts[index]
							, repairAct = actionsFore.Acts.AddAction(index + 10)
							;
						Point p1 = act.GetControlPointPosition(1)
							, p2 = act.GetControlPointPosition(2)
							, p3 = act.GetControlPointPosition(3)
							, p4 = act.GetControlPointPosition(4)
							, p5 = act.GetControlPointPosition(5)
							;
						repairAct.MoveTo(p5.X, p5.Y);
						repairAct.MoveControlPointTo(1, p1.X, p1.Y + dy, ResizeModifiers.None);
						repairAct.MoveControlPointTo(2, p2.X, p2.Y + dy, ResizeModifiers.None);
						repairAct.MoveControlPointTo(3, p3.X, p3.Y + dy, ResizeModifiers.None);
						repairAct.MoveControlPointTo(4, p4.X, p4.Y + dy, ResizeModifiers.None);
						repairAct.Angle = 0;
						repairAct.ToolTipText = act.ToolTipText + Properties.Strings.SuffixRepair;
						repairAct.FillStyle = project.Design.FillStyles[RepairProcColor];
					}
				}
			}

			((IDiagramPresenter)CurrentDisplay).ResumeUpdate();
		}

		internal void ChangeForeTime(DateTime time)
		{	if(CurrentDisplay.MapProvider is GMap.NET.MapProviders.EmptyProvider)
				return;

			((IDiagramPresenter)CurrentDisplay).SuspendUpdate();

			foreCalendar.Date = time.Date;
			foreWatch.Time = time;
			weatherForeCalendar.Date = weatherForeTime.Date;
			weatherForeWatch.Time = weatherForeTime;

			lock(weatherFore)
			{	if(foreWeather != null)
				{	weatherForeImage.Image = foreWeather.Image;
					weatherForeImage.ToolTipText = foreWeather.SymbolText;
					weatherForeThermometer.Temperature = (float)Math.Round(foreWeather.Temperature);
					weatherForeWindvane.UseColors = useWindvaneColors;
					weatherForeWindvane.SetWind((float)Math.Round(foreWeather.WindDirection), (float)Math.Round(foreWeather.WindSpeed));
					weatherForeHygrometer.Humidity = foreWeather.Humidity;
					weatherForeBarometer.PressureHPa = currWeather.Pressure;
				}
			}

			((IDiagramPresenter)CurrentDisplay).ResumeUpdate();
		}

		private IFillStyle[] GetFillStyles(string[] colorNames)
		{	IFillStyle[] fillStyles = new IFillStyle[colorNames.Length];
			for(int i = 0, n = colorNames.Length; i < n; ++i)
				fillStyles[i] = project.Design.FillStyles[colorNames[i]];
			return fillStyles;
		}

		private Random random = new Random();
		private void ChangeFillStyleRandom(PathBasedPlanarShape shape, string[][] fillColorNames, bool border = false)
		{	string[] colorNames = fillColorNames[emergencyColors ? 0 : 1];
			string colorName;
			bool f = false;
			for(int i = 0, n = colorNames.Length; i < n; ++i)
				if(colorNames[i] == shape.FillStyle.Name)
				{	f = true;
					break;
				}
			if(!f)
				colorName = colorNames[0];
			else
			{	int index = random.Next() % colorNames.Length;
				colorName = colorNames[index];
			}
			shape.FillStyle = project.Design.FillStyles[colorName];
			if(border
			&& shape.LineStyle != project.Design.LineStyles.Normal
			&& project.Design.LineStyles.Contains(colorName))
				shape.LineStyle = project.Design.LineStyles[colorName];
		}

		private bool ChangeFillStyle(PathBasedPlanarShape shape, bool down, string[][] fillColorNames)
		{	bool changeDirection = false;
			string[] colorNames = fillColorNames[emergencyColors ? 0 : 1];
			string colorName;
			int index = -1;
			for(int i = 0, n = colorNames.Length; i < n; ++i)
				if(colorNames[i] == shape.FillStyle.Name)
				{	index = i;
					break;
				}
			if(index == -1)								// Не нашли
				colorName = colorNames[0];				// Назначаем первый цвет
			else										// Нашли
			{	if(down)								// Цвета меняются вниз
				{	if(--index < 0)						// Дошли до конца
					{	changeDirection = true;			// Меняем направление движения
						if((index += 2) >= colorNames.Length)
							index = colorNames.Length - 1;
					}
				}
				else									// Цвета меняются вверх
				{	if(++index >= colorNames.Length)	// Дошли до конца
					{	changeDirection = true;			// Меняем направление движения
						if((index -= 2) < 0)
							index = 0;
					}
				}
				colorName = colorNames[index];
			}
			shape.FillStyle = project.Design.FillStyles[colorName];
			return changeDirection;
		}

		static internal NamedImage GetWeatherImage(string imageName)
		{	NamedImage namedImage = null;
			Uri uri = new Uri(string.Format(weatherImageUri, imageName));
			WebProxy webProxy = new WebProxy("127.0.0.1", true, new string[]{uri.Host});
			HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
			webRequest.Timeout = weatherRequestTimeout;
			webRequest.Proxy = webProxy;
			HttpWebResponse webResponse = null;
			Stream stream = null;
			try
			{	webResponse = (HttpWebResponse)webRequest.GetResponse();
				stream = webResponse.GetResponseStream();
				namedImage = new NamedImage(Image.FromStream(stream), imageName);
			}
			catch { }
			finally
			{	if(stream != null)
					stream.Close();
				if(webResponse != null)
					webResponse.Close();
			}
			return namedImage;
		}

		private void timerClock_Tick(object sender, EventArgs e)
		{	DateTime now = DateTime.Now;
			TimeSpan timeSpan = now - currTime;
			if(timeSpan.TotalMilliseconds > 900)
			{	ChangeCurrTime(currTime = now.AddMilliseconds(now.Millisecond > 500 ? 1000 - now.Millisecond : -now.Millisecond));
				ChangeForeTime(foreTime = currTime + weatherForecastTimeSpan);

				if(now.Second == 0			// Каждую минуту запрашиваем информацию о погоде
				&& weatherCurrTime != now)	// Устраняем "дребезг"
				{
					weatherCurrTime = now;
//#if DEBUG
					GetWeather(weatherCurrTime);
//#else
//					GetWeather();
//#endif
				}
			}
		}

		private void timerEnergyFlow_Tick(object sender, EventArgs e)
		{	ShowEnergyFlow();
		}

		private XmlDocument GetWeatherDocument(string uriFormat)
		{	XmlDocument xmlDocument = new XmlDocument();

			double lat = CurrentDisplay.Position.Lat
				, lon = CurrentDisplay.Position.Lng
				;
			string cultureName = mainThread.CurrentUICulture.Name;
			if(cultureName.Contains("-"))
				cultureName = cultureName.Substring(0, cultureName.IndexOf('-'));
			Uri uri = new Uri(string.Format(uriFormat, weatherRequestAppid, cultureName, lat, lon));
			WebProxy webProxy = new WebProxy("127.0.0.1", true, new string[]{uri.Host});
			HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
			webRequest.Timeout = weatherRequestTimeout;
			webRequest.Proxy = webProxy;
			HttpWebResponse webResponse = null;
			Stream stream = null;
			try
			{	webResponse = (HttpWebResponse)webRequest.GetResponse();
				stream = webResponse.GetResponseStream();
				xmlDocument.Load(stream);
			}
			catch(Exception e) { string message = e.Message; }
			finally
			{	if(stream != null)
					stream.Close();
				if(webResponse != null)
					webResponse.Close();
			}

			return xmlDocument;
		}

//#if DEBUG
		private void GetWeather(DateTime dateTime)
		{//?	statusLabelMessage.Text = dateTime.ToString();
			Task.Factory.StartNew(() => { GetCurrentWeather(dateTime); GetForecastWeather(dateTime); });
		}
//#else
//		private void GetWeather() { Task.Factory.StartNew(() => { GetCurrentWeather(); GetForecastWeather(); }); }
//#endif

//#if DEBUG
		private void GetCurrentWeather(DateTime dateTime)
//#else
//		private void GetCurrentWeather()
//#endif
		{	if(CurrentDisplay.MapProvider is GMap.NET.MapProviders.EmptyProvider)
				return;

			XmlDocument xmlDocument = GetWeatherDocument(currWeatherRequestUri);
			List<Weather> weathers = ParseWeatherDocument(xmlDocument);
			if(weathers != null && weathers.Count > 0)
				lock(weatherCurr)
				{	currWeather = weathers[0];
					weatherCurrTime = currWeather.DateTime;
				}
//#if DEBUG
			SaveXmlDocument(xmlDocument, dateTime, "curr-");
//#endif
		}

//#if DEBUG
		private void GetForecastWeather(DateTime dateTime)
//#else
//		private void GetForecastWeather()
//#endif
		{	if(CurrentDisplay.MapProvider is GMap.NET.MapProviders.EmptyProvider)
				return;

			XmlDocument xmlDocument = GetWeatherDocument(foreWeatherRequestUri);
			List<Weather> weathers = ParseWeatherDocument(xmlDocument);
			if(weathers != null)
			{	Weather weather;
				DateTime time = currTime + weatherForecastTimeSpan;
				for(int i = 0, n = weathers.Count; i < n; ++i)
				{	weather = weathers[i];
					if(weather.DateTimeF <= time && time <= weather.DateTimeT)
						lock(weatherFore)
						{	foreWeather = weather;
							weatherForeTime = foreWeather.DateTime;
							break;
						}
				}
			}
//#if DEBUG
			SaveXmlDocument(xmlDocument, dateTime, "fore-");
//#endif
		}

		private List<Weather> ParseWeatherDocument(XmlDocument xmlDocument)
		{	List<Weather> weathers = new List<Weather>();

			XmlElement rootElement = xmlDocument.DocumentElement;
			if(rootElement != null)
			{	Weather weather;
				if(rootElement.Name == "current")			// Текущая погода
				{	weather = new Weather();
					for(XmlNode childNode = rootElement.FirstChild; childNode != null; childNode = childNode.NextSibling)
						switch(childNode.Name)
						{	case "city":					// Местоположение
								for(XmlNode cityNode = childNode.FirstChild; cityNode != null; cityNode = cityNode.NextSibling)
									switch(cityNode.Name)
									{	case "sun":
											weather.SunRise = DateTime.Parse(cityNode.Attributes["rise"].Value).ToLocalTime();	// Восход солнца
											weather.SunSet  = DateTime.Parse(cityNode.Attributes["set" ].Value).ToLocalTime();	// Заказ солнца
											break;
									}
								break;
							case "temperature":				// Температура
								weather.Temperature = float.Parse(childNode.Attributes["value"].Value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
								break;
							case "humidity":				// Влажность
								weather.Humidity = float.Parse(childNode.Attributes["value"].Value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
								break;
							case "pressure":				// Давление
								weather.Pressure = float.Parse(childNode.Attributes["value"].Value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
								break;
							case "wind":					// Ветер
								for(XmlNode windNode = childNode.FirstChild; windNode != null; windNode = windNode.NextSibling)
									switch(windNode.Name)
									{	case "speed":
											weather.WindSpeed = float.Parse(windNode.Attributes["value"].Value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
											break;
										case "direction":
											weather.WindDirection = float.Parse(windNode.Attributes["value"].Value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
											break;
									}
								break;
							case "clouds":					// Облачность
								break;
							case "visibility":				// Видимость
								break;
							case "precipitation":			// Осадки
								break;
							case "weather":					// Погода
								weather.SymbolName = childNode.Attributes["icon"].Value;
								weather.SymbolText = childNode.Attributes["value"].Value;
								break;
							case "lastupdate":				// Время последнего изменения
								weather.DateTime = DateTime.Parse(childNode.Attributes["value"].Value).ToLocalTime();
								break;
						}
					weathers.Add(weather);
				}
				else if(rootElement.Name == "weatherdata")	// Прогноз погоды
				{	DateTime sunRise = DateTime.MinValue, sunSet = DateTime.MaxValue;
					for(XmlNode childNode = rootElement.FirstChild; childNode != null; childNode = childNode.NextSibling)
						switch(childNode.Name)
						{	case "sun":
								sunRise = DateTime.Parse(childNode.Attributes["rise"].Value).ToLocalTime();	// Восход солнца
								sunSet  = DateTime.Parse(childNode.Attributes["set" ].Value).ToLocalTime();	// Заказ солнца
								break;
							case "forecast":
								for(XmlNode forecastNode = childNode.FirstChild; forecastNode != null; forecastNode = forecastNode.NextSibling)
								{	weather = new Weather() { SunRise = sunRise, SunSet = sunSet
										, DateTimeF = DateTime.Parse(forecastNode.Attributes["from"].Value).ToLocalTime()
										, DateTimeT = DateTime.Parse(forecastNode.Attributes["to"  ].Value).ToLocalTime() };
									weather.DateTime = weather.DateTimeF + new TimeSpan((weather.DateTimeT - weather.DateTimeF).Ticks/2);
									for(XmlNode timeNode = forecastNode.FirstChild; timeNode != null; timeNode = timeNode.NextSibling)
										switch(timeNode.Name)
										{	case "symbol":
												weather.SymbolName = timeNode.Attributes["var"].Value;
												weather.SymbolText = timeNode.Attributes["name"].Value;
												break;
											case "windDirection":
												weather.WindDirection = float.Parse(timeNode.Attributes["deg"].Value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
												break;
											case "windSpeed":
												weather.WindSpeed = float.Parse(timeNode.Attributes["mps"].Value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
												break;
											case "temperature":
												weather.Temperature = float.Parse(timeNode.Attributes["value"].Value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
												break;
											case "pressure":
												weather.Pressure = float.Parse(timeNode.Attributes["value"].Value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
												break;
											case "humidity":
												weather.Humidity = float.Parse(timeNode.Attributes["value"].Value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
												break;
										}
									weathers.Add(weather);
								}
								break;
						}
				}
			}

			return weathers;
		}

		private void SaveXmlDocument(XmlDocument xmlDocument, DateTime dateTime, string prefix)
		{	if(!weatherLog)
				return;
			string filePath = Path.Combine(exePath, "openweathermap.org")
				, fileName = prefix + dateTime.ToString("yyyy-MM-dd_HH-mm-ss") + ".xml"
				;
			if(!Directory.Exists(filePath))
				Directory.CreateDirectory(filePath);

			XmlTextWriter xmlTextWriter = null;
			try
			{	xmlTextWriter = new XmlTextWriter(File.OpenWrite(Path.Combine(filePath, fileName)), Encoding.UTF8);
				xmlTextWriter.BaseStream.SetLength(0);
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlTextWriter.Indentation = 1;
				xmlTextWriter.IndentChar = '\t';
				xmlDocument.WriteTo(xmlTextWriter);
			}
			catch { }
			finally
			{	if(xmlTextWriter != null)
					xmlTextWriter.Close();
			}
		}

		private void cLIPSTestToolStripMenuItem_Click(object sender, EventArgs e)
		{	AI.WineFormsExample wineFormsExample = new AI.WineFormsExample();
			wineFormsExample.ShowDialog(this);
		}

		private void configSettingsRefreshToolStripMenuItem_Click(object sender, EventArgs e)
		{	bool f = statePosTop;
			ConfigurationManager.RefreshSection("appSettings");

			cultureName = ConfigurationManager.AppSettings["cultureName"];
			weatherLog = bool.Parse(ConfigurationManager.AppSettings["weatherLog"]);
			weatherRequestAppid = ConfigurationManager.AppSettings["weatherRequestAppid"];
			weatherRequestTimeout = int.Parse(ConfigurationManager.AppSettings["weatherRequestTimeout"]);
			weatherForecastTimeSpan = TimeSpan.Parse(ConfigurationManager.AppSettings["weatherForecastTimeSpan"]);
			useWindvaneColors = bool.Parse(ConfigurationManager.AppSettings["useWindvaneColors"]);
			emergencyColors = bool.Parse(ConfigurationManager.AppSettings["emergencyColors"]);
			statePosTop = bool.Parse(ConfigurationManager.AppSettings["statePosTop"]);
			staticPropertyBorders = bool.Parse(ConfigurationManager.AppSettings["staticPropertyBorders"]);
			dynamicPropertyBorders = bool.Parse(ConfigurationManager.AppSettings["dynamicPropertyBorders"]);
			showElectricPowerLines = bool.Parse(ConfigurationManager.AppSettings["showElectricPowerLines"]);
			electricPowerLinesFile = ConfigurationManager.AppSettings["electricPowerLinesFile"];
			colorsGrouping = (ColorsGrouping)Enum.Parse(typeof(ColorsGrouping), ConfigurationManager.AppSettings["colorsGrouping"]);
			energyFlowInterval = int.Parse(ConfigurationManager.AppSettings["energyFlowInterval"]);

			ChangeLanguage();

			if(statePosTop != f)
				ChangeVehicle();

			timerEnergyFlow.Interval = energyFlowInterval;
			timerEnergyFlow.Start();

			weatherCurrTime = DateTime.Now;
//#if DEBUG
			GetWeather(weatherCurrTime);
//#else
//			GetWeather();
//#endif
			ShowElectricPowerLines();
		}

		private void ShowElectricPowerLines()
		{
			if(!showElectricPowerLines)
			{	GMapOverlay overlay;
				for(int i = 0, n = CurrentDisplay.Overlays.Count; i < n; ++i)
				{	overlay = CurrentDisplay.Overlays[i];
					if(overlay.Id == ElectricPowerLinesOverlayId
					|| overlay.Id == EnergyPowerFlowOverlayId)
						CurrentDisplay.Overlays.Remove(overlay);
				}
			}
			else if(File.Exists(electricPowerLinesFile))
			{	GMapOverlay electricPowerLinesOverlay = new GMapOverlay(ElectricPowerLinesOverlayId)
					, energyPowerFlowOverlay = new GMapOverlay(EnergyPowerFlowOverlayId)
					;
				GMapRoute flowRoute = null;
				Color vlColor = Color.Blue
					, otpColor = Color.Green
					//, otpColor = Color.LightBlue
					, powerFlowColor = Color.Red
					;
				SolidBrush vlBrush = new SolidBrush(Color.FromArgb(90, vlColor))
					, otpBrush = new SolidBrush(Color.FromArgb(90, otpColor))
					, powerFlowBrush = new SolidBrush(powerFlowColor)	//new SolidBrush(Color.FromArgb(90, powerFlowColor))
					;

				int p1, p2, p;
				string line, wkt, wktPart, wktLon, wktLat;
				string[] lineParts, wktParts, lines = File.ReadAllLines(electricPowerLinesFile, Encoding.GetEncoding(1251));
				for(int i = 0, n = lines.Length; i < n; ++i)
				{	line = lines[i].Trim();
					if(line.StartsWith("#"))
						continue;
					lineParts = line.Split('\t');
					if(lineParts.Length != 8)
						continue;
					//0-Class, 1-Type, 2-Id, 3-TypeCode, 4-TypeAlias, 5-Code, 6-Alias, 7-WKT
					wkt = lineParts[7];
					if(!wkt.StartsWith("LINESTRING"))
						continue;
					p1 = wkt.IndexOf('(');
					p2 = wkt.LastIndexOf(')');
					if(p1 < 0 || p2 < 0 || p1 > p2)
						continue;
					List<GMap.NET.PointLatLng> points = new List<GMap.NET.PointLatLng>();
					wkt = wkt.Substring(p1 + 1, p2 - p1 - 1);
					wktParts = wkt.Split(',');
					for(int j = 0, m = wktParts.Length; j < m; ++j)
					{	wktPart = wktParts[j].Trim();
						p = wktPart.IndexOf(' ');
						if(p < 0)
							continue;
						wktLon = wktPart.Substring(0, p ).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
						wktLat = wktPart.Substring(p + 1).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
						points.Add(new GMap.NET.PointLatLng(double.Parse(wktLat), double.Parse(wktLon)));
					}

					GMapRoute route = new GMapRoute(points, lineParts[6]);
					route.Stroke = new Pen(lineParts[3] == "ОТП" ? otpBrush : vlBrush, 5);
					electricPowerLinesOverlay.Routes.Add(route);

				//	if(flowRoute == null)
					{	List<GMap.NET.PointLatLng> energyFlowPoints = new List<GMap.NET.PointLatLng>();

						EnergyFlowMarker energyFlowMarker = new EnergyFlowMarker() { Route = route };
						flowRoute = new GMapRoute(energyFlowPoints, lineParts[6]);
						flowRoute.Stroke = new Pen(powerFlowBrush, 5);
						flowRoute.Tag = energyFlowMarker;
						energyPowerFlowOverlay.Routes.Add(flowRoute);
					}
				}

				CurrentDisplay.Overlays.Add(electricPowerLinesOverlay);
				CurrentDisplay.Overlays.Add(energyPowerFlowOverlay);
			}
		}

		private double flowStep = 0.01d;
		private double flowLen = 0.05d;
		private int roundDecimals = 2;

		private void ShowEnergyFlow()
		{
			GMapOverlay overlay = null;
			for(int i = 0, n = CurrentDisplay.Overlays.Count; i < n; ++i)
			{	overlay = CurrentDisplay.Overlays[i];
				if(overlay.Id == EnergyPowerFlowOverlayId)
					break;
			}
			if(overlay == null
			|| overlay.Routes.Count == 0)
				return;
			GMapRoute mapRouteOld, mapRouteNew;
			GMapRoute[] mapRoutes = new GMapRoute[overlay.Routes.Count];
			overlay.Routes.CopyTo(mapRoutes, 0);
			for(int i = 0, n = mapRoutes.Length; i < n; ++i)
			{	mapRouteOld = mapRoutes[i];
				EnergyFlowMarker energyFlowMarker = (EnergyFlowMarker)mapRouteOld.Tag;
				List<GMap.NET.PointLatLng> newPoints = new List<GMap.NET.PointLatLng>();
				double x, y, len, k, part1, part2, maxLen, totalLen = 0;
				GMap.NET.PointLatLng p1, p2;

				if(mapRouteOld.Name == "ЛС13010000ВЛ11001310")
				{	if(Math.Round(energyFlowMarker.StartPointLen, roundDecimals) >= 0.370)
					{
					}
				}

				if(mapRouteOld.Points.Count > 0)					// Двигаемся дальше
				{
					if(energyFlowMarker.StartPointIndex == 0		// Удлиняем
					&& Math.Round(energyFlowMarker.Len, roundDecimals) < flowLen)
					{
						int idx = 0;

						totalLen = energyFlowMarker.Len;
						maxLen = flowStep*(Math.Round(totalLen/flowStep) + 1);

						newPoints.Add(p1 = mapRouteOld.Points[0]);
						for(int j = 1, m = mapRouteOld.Points.Count - 1; j < m; p1 = p2, idx = ++j)
							newPoints.Add(p2 = mapRouteOld.Points[j]);

						for(int j = idx + 1, m = energyFlowMarker.Route.Points.Count; j < m; p1 = p2, ++j)
						{	p2 = energyFlowMarker.Route.Points[j];
							x = p2.Lng - p1.Lng;
							y = p2.Lat - p1.Lat;
							len = Math.Sqrt(x*x + y*y);
							if(totalLen + len > maxLen)
							{
								part2 = totalLen + len - maxLen;
								part1 = len - part2;
								k = part1/part2;
								newPoints.Add(p2 = new GMap.NET.PointLatLng((p1.Lat + k*p2.Lat)/(1 + k), (p1.Lng + k*p2.Lng)/(1 + k)));

								x = p2.Lng - p1.Lng;
								y = p2.Lat - p1.Lat;
								len = Math.Sqrt(x*x + y*y);
								energyFlowMarker.PointIndex = j;
								energyFlowMarker.PointLen = totalLen;
								energyFlowMarker.Len = totalLen + len;

								break;
							}
							totalLen += len;
							newPoints.Add(p2);
						}
					}
					else											// Продвигаем
					{
						bool f = true;
						totalLen = energyFlowMarker.StartPointLen;
						maxLen = flowStep*(Math.Round(totalLen/flowStep) + 1);

						p1 = energyFlowMarker.Route.Points[energyFlowMarker.StartPointIndex];
						for (int j = energyFlowMarker.StartPointIndex + 1, m = energyFlowMarker.Route.Points.Count; f = j < m; p1 = p2, ++j)
						{	p2 = energyFlowMarker.Route.Points[j];
							x = p2.Lng - p1.Lng;
							y = p2.Lat - p1.Lat;
							len = Math.Sqrt(x*x + y*y);
							if(totalLen + len > maxLen)
							{
								part2 = totalLen + len - maxLen;
								part1 = len - part2;
								k = part1/part2;
								newPoints.Add(p2 = new GMap.NET.PointLatLng((p1.Lat + k*p2.Lat)/(1 + k), (p1.Lng + k*p2.Lng)/(1 + k)));

								x = p2.Lng - p1.Lng;
								y = p2.Lat - p1.Lat;
								len = Math.Sqrt(x*x + y*y);

								energyFlowMarker.StartPointIndex = j - 1;
								energyFlowMarker.StartPointLen = totalLen + len;

								p1 = p2;

								break;
							}
							totalLen += len;
						}
						if(!f)
						{	energyFlowMarker.PointIndex = 0;
							energyFlowMarker.PointLen = 0;
							energyFlowMarker.Len = flowLen;

							energyFlowMarker.StartPointIndex = 0;
							energyFlowMarker.StartPointLen = 0;
						}
						else
						{
							totalLen = 0;
							for (int j = energyFlowMarker.StartPointIndex + 1, m = energyFlowMarker.Route.Points.Count; j < m; p1 = p2, ++j)
							{
								p2 = energyFlowMarker.Route.Points[j];
								x = p2.Lng - p1.Lng;
								y = p2.Lat - p1.Lat;
								len = Math.Sqrt(x*x + y*y);
								if(totalLen + len > flowLen)
								{
									part1 = flowLen - totalLen;
									part2 = len - part1;
									k = part1/part2;
									newPoints.Add(p2 = new GMap.NET.PointLatLng((p1.Lat + k*p2.Lat)/(1 + k), (p1.Lng + k*p2.Lng)/(1 + k)));

									x = p2.Lng - p1.Lng;
									y = p2.Lat - p1.Lat;
									len = Math.Sqrt(x*x + y*y);

									energyFlowMarker.PointIndex = j;
									energyFlowMarker.PointLen = totalLen;
									energyFlowMarker.Len = len;

									break;
								}
								totalLen += len;
								newPoints.Add(p2);
							}
						}
					}
				}
				else if(energyFlowMarker.Route.Points.Count > 0)	// Делаем первый шаг
				{
					p1 = energyFlowMarker.Route.Points[0];
					newPoints.Add(p1);
					for(int j = 1, m = energyFlowMarker.Route.Points.Count; j < m; p1 = p2, ++j)
					{	p2 = energyFlowMarker.Route.Points[j];
						x = p2.Lng - p1.Lng;
						y = p2.Lat - p1.Lat;
						len = Math.Sqrt(x*x + y*y);
						if(totalLen + len > flowStep)
						{
							part1 = flowStep - totalLen;
							part2 = len - part1;
							k = part1/part2;
							newPoints.Add(p2 = new GMap.NET.PointLatLng((p1.Lat + k*p2.Lat)/(1 + k), (p1.Lng + k*p2.Lng)/(1 + k)));

							x = p2.Lng - p1.Lng;
							y = p2.Lat - p1.Lat;
							len = Math.Sqrt(x*x + y*y);
							energyFlowMarker.PointIndex = j;
							energyFlowMarker.PointLen = totalLen;
							energyFlowMarker.Len = totalLen + len;

							break;
						}
						totalLen += len;
						newPoints.Add(p2);
					}
				}

				mapRouteNew = new GMapRoute(newPoints, mapRouteOld.Name);
				mapRouteNew.Stroke = mapRouteOld.Stroke;
				mapRouteNew.Tag = energyFlowMarker;
				overlay.Routes.Remove(mapRouteOld);
				overlay.Routes.Add(mapRouteNew);
			}
		}
	}


	public class DisplayTabPage : TabPage
	{
		private Display tabDisplay;

		public DisplayTabPage() : base()
		{
		}

		public DisplayTabPage(string text) : base(text)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
				Display = null; // Clears and disposes the display
			base.Dispose(disposing);
		}

		protected override void OnClientSizeChanged(EventArgs e)
		{
			base.OnClientSizeChanged(e);
			if(Display != null
			&& Display.Diagram != null)
			{	DiagramDesignerMainForm form = (DiagramDesignerMainForm)FindForm();
				if(form.WindowState != FormWindowState.Minimized)
				{	Size size = ClientSize;
					size.Height -= DiagramDesignerMainForm.padding*2;// + DiagramDesignerMainForm.diameter;
					size.Width -= DiagramDesignerMainForm.padding*2;
					Display.Diagram.Location = new Point(DiagramDesignerMainForm.padding, DiagramDesignerMainForm.padding);
					Display.Diagram.Size = size;

					form.ChangeVehicle();
				}
			}
		}

		public Display Display
		{
			get { return tabDisplay; }
			set
			{
				if(tabDisplay != null && tabDisplay != value)
				{
					Controls.Remove(tabDisplay);
					tabDisplay.Clear();
					tabDisplay.Dispose();
					tabDisplay = null;
				}
				tabDisplay = value;
				if(tabDisplay != null)
					Controls.Add(tabDisplay);
			}
		}
	}


	internal class EnergyFlowMarker
	{
		internal GMapRoute Route;
		internal int StartPointIndex = 0;
		internal double StartPointLen = 0d;
		internal int PointIndex = 0;
		internal double PointLen = 0d;
		internal double Len = 0d;
	}

	public class Weather
	{	private string symbolName = null;
		private NamedImage image = null;

		public DateTime DateTime;	// Время показаний
		public DateTime DateTimeF;	// Время показаний от
		public DateTime DateTimeT;	// Время показаний до
		public float Temperature;	// Температура
		public float Humidity;		// Влажность
		public float Pressure;		// Давление
		public float WindSpeed;		// Скорость ветра
		public float WindDirection;	// Направление ветра
		public DateTime SunRise;	// Восход солнца
		public DateTime SunSet;		// Заказ солнца
		public string SymbolText;	// Описание погодных условий
		public string SymbolName	// Символ погодных условий
		{	get => this.symbolName;
			set
			{	this.symbolName = value;
				if(!string.IsNullOrEmpty(value))
					this.image = DiagramDesignerMainForm.GetWeatherImage(value);
			}
		}
		public NamedImage Image { get => this.image; }
	}

	public enum ColorsGrouping
	{
		None,
		ByColors,
		ByPercent,
	}
}