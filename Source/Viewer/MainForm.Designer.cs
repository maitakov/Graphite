namespace Dataweb.NShape.Viewer
{

	partial class DiagramDesignerMainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiagramDesignerMainForm));
			Dataweb.NShape.RoleBasedSecurityManager roleBasedSecurityManager1 = new Dataweb.NShape.RoleBasedSecurityManager();
			this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.statusLabelMessage = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelShapes = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusLabelShapeCount = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelResources = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusLabelResourceCount = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelProperties = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusLabelPropertyCount = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelActions = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusLabelActionCount = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelRelations = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusLabelRelationCount = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelSelection = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusLabelMousePosition = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusLabelSelectionSize = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelDisplayArea = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusLabelTopLeft = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusLabelBottomRight = new System.Windows.Forms.ToolStripStatusLabel();
			this.displayTabControl = new System.Windows.Forms.TabControl();
			this.historyTrackBar = new System.Windows.Forms.TrackBar();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.toolboxPropsPanel = new System.Windows.Forms.Panel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.toolboxListView = new System.Windows.Forms.ListView();
			this.toolboxContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.viewToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.smallIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.largeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.propertyWindowTabControl = new System.Windows.Forms.TabControl();
			this.hierarchyTab = new System.Windows.Forms.TabPage();
			this.hierarchyTreeView = new BrightIdeasSoftware.TreeListView();
			this.olvColumnType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.propertyWindowShapeTab = new System.Windows.Forms.TabPage();
			this.primaryPropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.propertyWindowModelTab = new System.Windows.Forms.TabPage();
			this.secondaryPropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.layersTab = new System.Windows.Forms.TabPage();
			this.layerEditorListView = new Dataweb.NShape.WinFormsUI.LayerListView();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.modelTreeView = new System.Windows.Forms.TreeView();
			this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newProjectInXMLStoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newProjectInSqlStoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openXMLRepositoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openSQLServerRepositoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recentProjectsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
			this.upgradeVersionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.useEmbeddedImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripSeparator();
			this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
			this.ManageShapeAndModelLibrariesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripSeparator();
			this.exportRepositoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importRepositoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportDiagramAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportDialogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.emfExportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.wmfExportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pngExportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.jpgExportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bmpExportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.quitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertDiagramMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteDiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showDiagramSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
			this.cutShapeOnlyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutShapeAndModelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyAsImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyShapeOnlyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyShapeAndModelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteShapeOnlyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteShapeAndModelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllOfTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllOfTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unselectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.toForegroundMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toBackgroundMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
			this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.redoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showGridMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showDisplaySettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.editDesignsAndStylesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewShowLayoutControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.highQualityRenderingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.resetToolbarsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.configSettingsRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
			this.adoNetDatabaseGeneratorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.testDataGeneratorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.nShapeEventMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripSeparator();
			this.cLIPSTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
			this.viewHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStrip = new System.Windows.Forms.ToolStrip();
			this.newProjectToolStripButton = new System.Windows.Forms.ToolStripDropDownButton();
			this.xmlRepositoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sQLRepositoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openProjectToolStripButton = new System.Windows.Forms.ToolStripDropDownButton();
			this.openXMLRepositoryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.openSQLRepositoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveProjectToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.cutShapeButton = new System.Windows.Forms.ToolStripButton();
			this.copyShapeButton = new System.Windows.Forms.ToolStripButton();
			this.pasteButton = new System.Windows.Forms.ToolStripButton();
			this.deleteShapeButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.undoToolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
			this.redoToolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
			this.settingsToolStrip = new System.Windows.Forms.ToolStrip();
			this.prevDiagramButton = new System.Windows.Forms.ToolStripButton();
			this.nextDiagramButton = new System.Windows.Forms.ToolStripButton();
			this.zoomToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
			this.displaySettingsToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.refreshToolbarButton = new System.Windows.Forms.ToolStripButton();
			this.showGridToolbarButton = new System.Windows.Forms.ToolStripButton();
			this.displayToolStrip = new System.Windows.Forms.ToolStrip();
			this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
			this.designEditorToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.runtimeModeComboBox = new System.Windows.Forms.ToolStripComboBox();
			this.debugToolStrip = new System.Windows.Forms.ToolStrip();
			this.debugDrawOccupationToolbarButton = new System.Windows.Forms.ToolStripButton();
			this.debugDrawInvalidatedAreaToolbarButton = new System.Windows.Forms.ToolStripButton();
			this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
			this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
			this.defaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
			this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
			this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.diagramSetController = new Dataweb.NShape.Controllers.DiagramSetController();
			this.project = new Dataweb.NShape.Project(this.components);
			this.cachedRepository = new Dataweb.NShape.Advanced.CachedRepository();
			this.layerController = new Dataweb.NShape.Controllers.LayerController();
			this.toolSetController = new Dataweb.NShape.Controllers.ToolSetController();
			this.modelTreeController = new Dataweb.NShape.Controllers.ModelController();
			this.propertyController = new Dataweb.NShape.Controllers.PropertyController();
			this.modelTreePresenter = new Dataweb.NShape.WinFormsUI.ModelTreeViewPresenter();
			this.propertyPresenter = new Dataweb.NShape.WinFormsUI.PropertyPresenter();
			this.toolSetListViewPresenter = new Dataweb.NShape.WinFormsUI.ToolSetListViewPresenter(this.components);
			this.layerPresenter = new Dataweb.NShape.Controllers.LayerPresenter();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.diagramContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.timerClock = new System.Windows.Forms.Timer(this.components);
			this.toolTipTimer = new System.Windows.Forms.Timer(this.components);
			this.timerEnergyFlow = new System.Windows.Forms.Timer(this.components);
			this.toolStripContainer.BottomToolStripPanel.SuspendLayout();
			this.toolStripContainer.ContentPanel.SuspendLayout();
			this.toolStripContainer.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer.SuspendLayout();
			this.statusStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.historyTrackBar)).BeginInit();
			this.toolboxPropsPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.toolboxContextMenuStrip.SuspendLayout();
			this.propertyWindowTabControl.SuspendLayout();
			this.hierarchyTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.hierarchyTreeView)).BeginInit();
			this.propertyWindowShapeTab.SuspendLayout();
			this.propertyWindowModelTab.SuspendLayout();
			this.layersTab.SuspendLayout();
			this.mainMenuStrip.SuspendLayout();
			this.editToolStrip.SuspendLayout();
			this.settingsToolStrip.SuspendLayout();
			this.displayToolStrip.SuspendLayout();
			this.debugToolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripContainer
			// 
			resources.ApplyResources(this.toolStripContainer, "toolStripContainer");
			// 
			// toolStripContainer.BottomToolStripPanel
			// 
			resources.ApplyResources(this.toolStripContainer.BottomToolStripPanel, "toolStripContainer.BottomToolStripPanel");
			this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.statusStrip);
			this.toolTip.SetToolTip(this.toolStripContainer.BottomToolStripPanel, resources.GetString("toolStripContainer.BottomToolStripPanel.ToolTip"));
			// 
			// toolStripContainer.ContentPanel
			// 
			resources.ApplyResources(this.toolStripContainer.ContentPanel, "toolStripContainer.ContentPanel");
			this.toolStripContainer.ContentPanel.Controls.Add(this.displayTabControl);
			this.toolStripContainer.ContentPanel.Controls.Add(this.historyTrackBar);
			this.toolStripContainer.ContentPanel.Controls.Add(this.splitter2);
			this.toolStripContainer.ContentPanel.Controls.Add(this.toolboxPropsPanel);
			this.toolStripContainer.ContentPanel.Controls.Add(this.splitter1);
			this.toolStripContainer.ContentPanel.Controls.Add(this.modelTreeView);
			this.toolTip.SetToolTip(this.toolStripContainer.ContentPanel, resources.GetString("toolStripContainer.ContentPanel.ToolTip"));
			// 
			// toolStripContainer.LeftToolStripPanel
			// 
			resources.ApplyResources(this.toolStripContainer.LeftToolStripPanel, "toolStripContainer.LeftToolStripPanel");
			this.toolTip.SetToolTip(this.toolStripContainer.LeftToolStripPanel, resources.GetString("toolStripContainer.LeftToolStripPanel.ToolTip"));
			this.toolStripContainer.Name = "toolStripContainer";
			// 
			// toolStripContainer.RightToolStripPanel
			// 
			resources.ApplyResources(this.toolStripContainer.RightToolStripPanel, "toolStripContainer.RightToolStripPanel");
			this.toolTip.SetToolTip(this.toolStripContainer.RightToolStripPanel, resources.GetString("toolStripContainer.RightToolStripPanel.ToolTip"));
			this.toolTip.SetToolTip(this.toolStripContainer, resources.GetString("toolStripContainer.ToolTip"));
			// 
			// toolStripContainer.TopToolStripPanel
			// 
			resources.ApplyResources(this.toolStripContainer.TopToolStripPanel, "toolStripContainer.TopToolStripPanel");
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.mainMenuStrip);
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.editToolStrip);
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.settingsToolStrip);
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.displayToolStrip);
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.debugToolStrip);
			this.toolTip.SetToolTip(this.toolStripContainer.TopToolStripPanel, resources.GetString("toolStripContainer.TopToolStripPanel.ToolTip"));
			// 
			// statusStrip
			// 
			resources.ApplyResources(this.statusStrip, "statusStrip");
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabelMessage,
            this.toolStripStatusLabelShapes,
            this.statusLabelShapeCount,
            this.toolStripStatusLabelResources,
            this.statusLabelResourceCount,
            this.toolStripStatusLabelProperties,
            this.statusLabelPropertyCount,
            this.toolStripStatusLabelActions,
            this.statusLabelActionCount,
            this.toolStripStatusLabelRelations,
            this.statusLabelRelationCount,
            this.toolStripStatusLabelSelection,
            this.statusLabelMousePosition,
            this.statusLabelSelectionSize,
            this.toolStripStatusLabelDisplayArea,
            this.statusLabelTopLeft,
            this.statusLabelBottomRight});
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
			this.toolTip.SetToolTip(this.statusStrip, resources.GetString("statusStrip.ToolTip"));
			// 
			// statusLabelMessage
			// 
			resources.ApplyResources(this.statusLabelMessage, "statusLabelMessage");
			this.statusLabelMessage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.statusLabelMessage.Name = "statusLabelMessage";
			this.statusLabelMessage.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.statusLabelMessage.Spring = true;
			// 
			// toolStripStatusLabelShapes
			// 
			resources.ApplyResources(this.toolStripStatusLabelShapes, "toolStripStatusLabelShapes");
			this.toolStripStatusLabelShapes.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toolStripStatusLabelShapes.Name = "toolStripStatusLabelShapes";
			this.toolStripStatusLabelShapes.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
			// 
			// statusLabelShapeCount
			// 
			resources.ApplyResources(this.statusLabelShapeCount, "statusLabelShapeCount");
			this.statusLabelShapeCount.Name = "statusLabelShapeCount";
			this.statusLabelShapeCount.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
			// 
			// toolStripStatusLabelResources
			// 
			resources.ApplyResources(this.toolStripStatusLabelResources, "toolStripStatusLabelResources");
			this.toolStripStatusLabelResources.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toolStripStatusLabelResources.Name = "toolStripStatusLabelResources";
			this.toolStripStatusLabelResources.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
			// 
			// statusLabelResourceCount
			// 
			resources.ApplyResources(this.statusLabelResourceCount, "statusLabelResourceCount");
			this.statusLabelResourceCount.Name = "statusLabelResourceCount";
			// 
			// toolStripStatusLabelProperties
			// 
			resources.ApplyResources(this.toolStripStatusLabelProperties, "toolStripStatusLabelProperties");
			this.toolStripStatusLabelProperties.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toolStripStatusLabelProperties.Name = "toolStripStatusLabelProperties";
			this.toolStripStatusLabelProperties.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
			// 
			// statusLabelPropertyCount
			// 
			resources.ApplyResources(this.statusLabelPropertyCount, "statusLabelPropertyCount");
			this.statusLabelPropertyCount.Name = "statusLabelPropertyCount";
			// 
			// toolStripStatusLabelActions
			// 
			resources.ApplyResources(this.toolStripStatusLabelActions, "toolStripStatusLabelActions");
			this.toolStripStatusLabelActions.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toolStripStatusLabelActions.Name = "toolStripStatusLabelActions";
			this.toolStripStatusLabelActions.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
			// 
			// statusLabelActionCount
			// 
			resources.ApplyResources(this.statusLabelActionCount, "statusLabelActionCount");
			this.statusLabelActionCount.Name = "statusLabelActionCount";
			// 
			// toolStripStatusLabelRelations
			// 
			resources.ApplyResources(this.toolStripStatusLabelRelations, "toolStripStatusLabelRelations");
			this.toolStripStatusLabelRelations.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toolStripStatusLabelRelations.Name = "toolStripStatusLabelRelations";
			this.toolStripStatusLabelRelations.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
			// 
			// statusLabelRelationCount
			// 
			resources.ApplyResources(this.statusLabelRelationCount, "statusLabelRelationCount");
			this.statusLabelRelationCount.Name = "statusLabelRelationCount";
			// 
			// toolStripStatusLabelSelection
			// 
			resources.ApplyResources(this.toolStripStatusLabelSelection, "toolStripStatusLabelSelection");
			this.toolStripStatusLabelSelection.AutoToolTip = true;
			this.toolStripStatusLabelSelection.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toolStripStatusLabelSelection.Name = "toolStripStatusLabelSelection";
			// 
			// statusLabelMousePosition
			// 
			resources.ApplyResources(this.statusLabelMousePosition, "statusLabelMousePosition");
			this.statusLabelMousePosition.AutoToolTip = true;
			this.statusLabelMousePosition.Name = "statusLabelMousePosition";
			this.statusLabelMousePosition.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
			// 
			// statusLabelSelectionSize
			// 
			resources.ApplyResources(this.statusLabelSelectionSize, "statusLabelSelectionSize");
			this.statusLabelSelectionSize.AutoToolTip = true;
			this.statusLabelSelectionSize.Name = "statusLabelSelectionSize";
			this.statusLabelSelectionSize.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
			// 
			// toolStripStatusLabelDisplayArea
			// 
			resources.ApplyResources(this.toolStripStatusLabelDisplayArea, "toolStripStatusLabelDisplayArea");
			this.toolStripStatusLabelDisplayArea.AutoToolTip = true;
			this.toolStripStatusLabelDisplayArea.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toolStripStatusLabelDisplayArea.Name = "toolStripStatusLabelDisplayArea";
			this.toolStripStatusLabelDisplayArea.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
			// 
			// statusLabelTopLeft
			// 
			resources.ApplyResources(this.statusLabelTopLeft, "statusLabelTopLeft");
			this.statusLabelTopLeft.AutoToolTip = true;
			this.statusLabelTopLeft.Name = "statusLabelTopLeft";
			this.statusLabelTopLeft.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
			// 
			// statusLabelBottomRight
			// 
			resources.ApplyResources(this.statusLabelBottomRight, "statusLabelBottomRight");
			this.statusLabelBottomRight.AutoToolTip = true;
			this.statusLabelBottomRight.Name = "statusLabelBottomRight";
			this.statusLabelBottomRight.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
			// 
			// displayTabControl
			// 
			resources.ApplyResources(this.displayTabControl, "displayTabControl");
			this.displayTabControl.Name = "displayTabControl";
			this.displayTabControl.SelectedIndex = 0;
			this.toolTip.SetToolTip(this.displayTabControl, resources.GetString("displayTabControl.ToolTip"));
			this.displayTabControl.SelectedIndexChanged += new System.EventHandler(this.displaysTabControl_SelectedIndexChanged);
			// 
			// historyTrackBar
			// 
			resources.ApplyResources(this.historyTrackBar, "historyTrackBar");
			this.historyTrackBar.LargeChange = 1;
			this.historyTrackBar.Name = "historyTrackBar";
			this.toolTip.SetToolTip(this.historyTrackBar, resources.GetString("historyTrackBar.ToolTip"));
			this.historyTrackBar.ValueChanged += new System.EventHandler(this.historyTrackBar_ValueChanged);
			// 
			// splitter2
			// 
			resources.ApplyResources(this.splitter2, "splitter2");
			this.splitter2.Name = "splitter2";
			this.splitter2.TabStop = false;
			this.toolTip.SetToolTip(this.splitter2, resources.GetString("splitter2.ToolTip"));
			// 
			// toolboxPropsPanel
			// 
			resources.ApplyResources(this.toolboxPropsPanel, "toolboxPropsPanel");
			this.toolboxPropsPanel.Controls.Add(this.splitContainer1);
			this.toolboxPropsPanel.Name = "toolboxPropsPanel";
			this.toolTip.SetToolTip(this.toolboxPropsPanel, resources.GetString("toolboxPropsPanel.ToolTip"));
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
			this.splitContainer1.Panel1.Controls.Add(this.toolboxListView);
			this.toolTip.SetToolTip(this.splitContainer1.Panel1, resources.GetString("splitContainer1.Panel1.ToolTip"));
			// 
			// splitContainer1.Panel2
			// 
			resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
			this.splitContainer1.Panel2.Controls.Add(this.propertyWindowTabControl);
			this.toolTip.SetToolTip(this.splitContainer1.Panel2, resources.GetString("splitContainer1.Panel2.ToolTip"));
			this.toolTip.SetToolTip(this.splitContainer1, resources.GetString("splitContainer1.ToolTip"));
			// 
			// toolboxListView
			// 
			resources.ApplyResources(this.toolboxListView, "toolboxListView");
			this.toolboxListView.ContextMenuStrip = this.toolboxContextMenuStrip;
			this.toolboxListView.FullRowSelect = true;
			this.toolboxListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.toolboxListView.HideSelection = false;
			this.toolboxListView.MultiSelect = false;
			this.toolboxListView.Name = "toolboxListView";
			this.toolboxListView.ShowItemToolTips = true;
			this.toolTip.SetToolTip(this.toolboxListView, resources.GetString("toolboxListView.ToolTip"));
			this.toolboxListView.UseCompatibleStateImageBehavior = false;
			this.toolboxListView.View = System.Windows.Forms.View.Details;
			// 
			// toolboxContextMenuStrip
			// 
			resources.ApplyResources(this.toolboxContextMenuStrip, "toolboxContextMenuStrip");
			this.toolboxContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem1});
			this.toolboxContextMenuStrip.Name = "toolboxContextMenuStrip";
			this.toolTip.SetToolTip(this.toolboxContextMenuStrip, resources.GetString("toolboxContextMenuStrip.ToolTip"));
			// 
			// viewToolStripMenuItem1
			// 
			resources.ApplyResources(this.viewToolStripMenuItem1, "viewToolStripMenuItem1");
			this.viewToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detailsToolStripMenuItem,
            this.tilesToolStripMenuItem,
            this.smallIconsToolStripMenuItem,
            this.largeIconsToolStripMenuItem});
			this.viewToolStripMenuItem1.Name = "viewToolStripMenuItem1";
			// 
			// detailsToolStripMenuItem
			// 
			resources.ApplyResources(this.detailsToolStripMenuItem, "detailsToolStripMenuItem");
			this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
			this.detailsToolStripMenuItem.Click += new System.EventHandler(this.detailsToolStripMenuItem_Click);
			// 
			// tilesToolStripMenuItem
			// 
			resources.ApplyResources(this.tilesToolStripMenuItem, "tilesToolStripMenuItem");
			this.tilesToolStripMenuItem.Name = "tilesToolStripMenuItem";
			this.tilesToolStripMenuItem.Click += new System.EventHandler(this.tilesToolStripMenuItem_Click);
			// 
			// smallIconsToolStripMenuItem
			// 
			resources.ApplyResources(this.smallIconsToolStripMenuItem, "smallIconsToolStripMenuItem");
			this.smallIconsToolStripMenuItem.Name = "smallIconsToolStripMenuItem";
			this.smallIconsToolStripMenuItem.Click += new System.EventHandler(this.smallIconsToolStripMenuItem_Click);
			// 
			// largeIconsToolStripMenuItem
			// 
			resources.ApplyResources(this.largeIconsToolStripMenuItem, "largeIconsToolStripMenuItem");
			this.largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
			this.largeIconsToolStripMenuItem.Click += new System.EventHandler(this.largeIconsToolStripMenuItem_Click);
			// 
			// propertyWindowTabControl
			// 
			resources.ApplyResources(this.propertyWindowTabControl, "propertyWindowTabControl");
			this.propertyWindowTabControl.Controls.Add(this.hierarchyTab);
			this.propertyWindowTabControl.Controls.Add(this.propertyWindowShapeTab);
			this.propertyWindowTabControl.Controls.Add(this.propertyWindowModelTab);
			this.propertyWindowTabControl.Controls.Add(this.layersTab);
			this.propertyWindowTabControl.Name = "propertyWindowTabControl";
			this.propertyWindowTabControl.SelectedIndex = 0;
			this.toolTip.SetToolTip(this.propertyWindowTabControl, resources.GetString("propertyWindowTabControl.ToolTip"));
			// 
			// hierarchyTab
			// 
			resources.ApplyResources(this.hierarchyTab, "hierarchyTab");
			this.hierarchyTab.Controls.Add(this.hierarchyTreeView);
			this.hierarchyTab.Name = "hierarchyTab";
			this.toolTip.SetToolTip(this.hierarchyTab, resources.GetString("hierarchyTab.ToolTip"));
			this.hierarchyTab.UseVisualStyleBackColor = true;
			// 
			// hierarchyTreeView
			// 
			resources.ApplyResources(this.hierarchyTreeView, "hierarchyTreeView");
			this.hierarchyTreeView.AllColumns.Add(this.olvColumnType);
			this.hierarchyTreeView.CellEditUseWholeCell = false;
			this.hierarchyTreeView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnType});
			this.hierarchyTreeView.Cursor = System.Windows.Forms.Cursors.Default;
			this.hierarchyTreeView.FullRowSelect = true;
			this.hierarchyTreeView.HideSelection = false;
			this.hierarchyTreeView.Name = "hierarchyTreeView";
			this.hierarchyTreeView.OverlayText.Text = resources.GetString("resource.Text");
			this.hierarchyTreeView.ShowGroups = false;
			this.toolTip.SetToolTip(this.hierarchyTreeView, resources.GetString("hierarchyTreeView.ToolTip"));
			this.hierarchyTreeView.UseCompatibleStateImageBehavior = false;
			this.hierarchyTreeView.View = System.Windows.Forms.View.Details;
			this.hierarchyTreeView.VirtualMode = true;
			this.hierarchyTreeView.SelectionChanged += new System.EventHandler(this.hierarchyTreeView_SelectionChanged);
			// 
			// olvColumnType
			// 
			this.olvColumnType.AspectName = "Type";
			this.olvColumnType.FillsFreeSpace = true;
			resources.ApplyResources(this.olvColumnType, "olvColumnType");
			this.olvColumnType.IsEditable = false;
			this.olvColumnType.IsTileViewColumn = true;
			this.olvColumnType.UseInitialLetterForGroup = true;
			// 
			// propertyWindowShapeTab
			// 
			resources.ApplyResources(this.propertyWindowShapeTab, "propertyWindowShapeTab");
			this.propertyWindowShapeTab.Controls.Add(this.primaryPropertyGrid);
			this.propertyWindowShapeTab.Name = "propertyWindowShapeTab";
			this.toolTip.SetToolTip(this.propertyWindowShapeTab, resources.GetString("propertyWindowShapeTab.ToolTip"));
			this.propertyWindowShapeTab.UseVisualStyleBackColor = true;
			// 
			// primaryPropertyGrid
			// 
			resources.ApplyResources(this.primaryPropertyGrid, "primaryPropertyGrid");
			this.primaryPropertyGrid.Name = "primaryPropertyGrid";
			this.toolTip.SetToolTip(this.primaryPropertyGrid, resources.GetString("primaryPropertyGrid.ToolTip"));
			// 
			// propertyWindowModelTab
			// 
			resources.ApplyResources(this.propertyWindowModelTab, "propertyWindowModelTab");
			this.propertyWindowModelTab.Controls.Add(this.secondaryPropertyGrid);
			this.propertyWindowModelTab.Name = "propertyWindowModelTab";
			this.toolTip.SetToolTip(this.propertyWindowModelTab, resources.GetString("propertyWindowModelTab.ToolTip"));
			this.propertyWindowModelTab.UseVisualStyleBackColor = true;
			// 
			// secondaryPropertyGrid
			// 
			resources.ApplyResources(this.secondaryPropertyGrid, "secondaryPropertyGrid");
			this.secondaryPropertyGrid.Name = "secondaryPropertyGrid";
			this.toolTip.SetToolTip(this.secondaryPropertyGrid, resources.GetString("secondaryPropertyGrid.ToolTip"));
			// 
			// layersTab
			// 
			resources.ApplyResources(this.layersTab, "layersTab");
			this.layersTab.Controls.Add(this.layerEditorListView);
			this.layersTab.Name = "layersTab";
			this.toolTip.SetToolTip(this.layersTab, resources.GetString("layersTab.ToolTip"));
			this.layersTab.UseVisualStyleBackColor = true;
			// 
			// layerEditorListView
			// 
			resources.ApplyResources(this.layerEditorListView, "layerEditorListView");
			this.layerEditorListView.AllowColumnReorder = true;
			this.layerEditorListView.FullRowSelect = true;
			this.layerEditorListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.layerEditorListView.HideDeniedMenuItems = false;
			this.layerEditorListView.HideSelection = false;
			this.layerEditorListView.LabelEdit = true;
			this.layerEditorListView.Name = "layerEditorListView";
			this.layerEditorListView.OwnerDraw = true;
			this.layerEditorListView.ShowDefaultContextMenu = true;
			this.layerEditorListView.ShowGroups = false;
			this.toolTip.SetToolTip(this.layerEditorListView, resources.GetString("layerEditorListView.ToolTip"));
			this.layerEditorListView.UseCompatibleStateImageBehavior = false;
			this.layerEditorListView.View = System.Windows.Forms.View.Details;
			// 
			// splitter1
			// 
			resources.ApplyResources(this.splitter1, "splitter1");
			this.splitter1.Name = "splitter1";
			this.splitter1.TabStop = false;
			this.toolTip.SetToolTip(this.splitter1, resources.GetString("splitter1.ToolTip"));
			// 
			// modelTreeView
			// 
			resources.ApplyResources(this.modelTreeView, "modelTreeView");
			this.modelTreeView.FullRowSelect = true;
			this.modelTreeView.Name = "modelTreeView";
			this.toolTip.SetToolTip(this.modelTreeView, resources.GetString("modelTreeView.ToolTip"));
			// 
			// mainMenuStrip
			// 
			resources.ApplyResources(this.mainMenuStrip, "mainMenuStrip");
			this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.toolStripMenuItem9});
			this.mainMenuStrip.Name = "mainMenuStrip";
			this.toolTip.SetToolTip(this.mainMenuStrip, resources.GetString("mainMenuStrip.ToolTip"));
			// 
			// fileToolStripMenuItem
			// 
			resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.openProjectMenuItem,
            this.recentProjectsMenuItem,
            this.toolStripMenuItem7,
            this.upgradeVersionMenuItem,
            this.useEmbeddedImagesToolStripMenuItem,
            this.toolStripMenuItem14,
            this.saveMenuItem,
            this.saveAsMenuItem,
            this.closeProjectToolStripMenuItem,
            this.toolStripMenuItem5,
            this.ManageShapeAndModelLibrariesMenuItem,
            this.toolStripMenuItem13,
            this.exportRepositoryMenuItem,
            this.importRepositoryMenuItem,
            this.exportDiagramAsMenuItem,
            this.toolStripMenuItem1,
            this.quitMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			// 
			// newProjectToolStripMenuItem
			// 
			resources.ApplyResources(this.newProjectToolStripMenuItem, "newProjectToolStripMenuItem");
			this.newProjectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectInXMLStoreToolStripMenuItem,
            this.newProjectInSqlStoreToolStripMenuItem});
			this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
			// 
			// newProjectInXMLStoreToolStripMenuItem
			// 
			resources.ApplyResources(this.newProjectInXMLStoreToolStripMenuItem, "newProjectInXMLStoreToolStripMenuItem");
			this.newProjectInXMLStoreToolStripMenuItem.Name = "newProjectInXMLStoreToolStripMenuItem";
			this.newProjectInXMLStoreToolStripMenuItem.Click += new System.EventHandler(this.newXMLRepositoryToolStripMenuItem_Click);
			// 
			// newProjectInSqlStoreToolStripMenuItem
			// 
			resources.ApplyResources(this.newProjectInSqlStoreToolStripMenuItem, "newProjectInSqlStoreToolStripMenuItem");
			this.newProjectInSqlStoreToolStripMenuItem.Name = "newProjectInSqlStoreToolStripMenuItem";
			this.newProjectInSqlStoreToolStripMenuItem.Click += new System.EventHandler(this.newSQLServerRepositoryToolStripMenuItem_Click);
			// 
			// openProjectMenuItem
			// 
			resources.ApplyResources(this.openProjectMenuItem, "openProjectMenuItem");
			this.openProjectMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openXMLRepositoryToolStripMenuItem,
            this.openSQLServerRepositoryToolStripMenuItem});
			this.openProjectMenuItem.Name = "openProjectMenuItem";
			// 
			// openXMLRepositoryToolStripMenuItem
			// 
			resources.ApplyResources(this.openXMLRepositoryToolStripMenuItem, "openXMLRepositoryToolStripMenuItem");
			this.openXMLRepositoryToolStripMenuItem.Name = "openXMLRepositoryToolStripMenuItem";
			this.openXMLRepositoryToolStripMenuItem.Click += new System.EventHandler(this.openXMLRepositoryToolStripMenuItem_Click);
			// 
			// openSQLServerRepositoryToolStripMenuItem
			// 
			resources.ApplyResources(this.openSQLServerRepositoryToolStripMenuItem, "openSQLServerRepositoryToolStripMenuItem");
			this.openSQLServerRepositoryToolStripMenuItem.Name = "openSQLServerRepositoryToolStripMenuItem";
			this.openSQLServerRepositoryToolStripMenuItem.Click += new System.EventHandler(this.openSQLServerRepositoryToolStripMenuItem_Click);
			// 
			// recentProjectsMenuItem
			// 
			resources.ApplyResources(this.recentProjectsMenuItem, "recentProjectsMenuItem");
			this.recentProjectsMenuItem.Name = "recentProjectsMenuItem";
			// 
			// toolStripMenuItem7
			// 
			resources.ApplyResources(this.toolStripMenuItem7, "toolStripMenuItem7");
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			// 
			// upgradeVersionMenuItem
			// 
			resources.ApplyResources(this.upgradeVersionMenuItem, "upgradeVersionMenuItem");
			this.upgradeVersionMenuItem.Image = global::Dataweb.NShape.Viewer.Properties.Resources.UpgradeFileVersion;
			this.upgradeVersionMenuItem.Name = "upgradeVersionMenuItem";
			this.upgradeVersionMenuItem.Click += new System.EventHandler(this.upgradeVersionMenuItem_Click);
			// 
			// useEmbeddedImagesToolStripMenuItem
			// 
			resources.ApplyResources(this.useEmbeddedImagesToolStripMenuItem, "useEmbeddedImagesToolStripMenuItem");
			this.useEmbeddedImagesToolStripMenuItem.Checked = true;
			this.useEmbeddedImagesToolStripMenuItem.CheckOnClick = true;
			this.useEmbeddedImagesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.useEmbeddedImagesToolStripMenuItem.Name = "useEmbeddedImagesToolStripMenuItem";
			this.useEmbeddedImagesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.useEmbeddedImagesToolStripMenuItem_CheckedChanged);
			// 
			// toolStripMenuItem14
			// 
			resources.ApplyResources(this.toolStripMenuItem14, "toolStripMenuItem14");
			this.toolStripMenuItem14.Name = "toolStripMenuItem14";
			// 
			// saveMenuItem
			// 
			resources.ApplyResources(this.saveMenuItem, "saveMenuItem");
			this.saveMenuItem.Name = "saveMenuItem";
			this.saveMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// saveAsMenuItem
			// 
			resources.ApplyResources(this.saveAsMenuItem, "saveAsMenuItem");
			this.saveAsMenuItem.Image = global::Dataweb.NShape.Viewer.Properties.Resources.SaveAsBtn;
			this.saveAsMenuItem.Name = "saveAsMenuItem";
			this.saveAsMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// closeProjectToolStripMenuItem
			// 
			resources.ApplyResources(this.closeProjectToolStripMenuItem, "closeProjectToolStripMenuItem");
			this.closeProjectToolStripMenuItem.Image = global::Dataweb.NShape.Viewer.Properties.Resources.CloseBtn;
			this.closeProjectToolStripMenuItem.Name = "closeProjectToolStripMenuItem";
			this.closeProjectToolStripMenuItem.Click += new System.EventHandler(this.closeProjectToolStripMenuItem_Click);
			// 
			// toolStripMenuItem5
			// 
			resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			// 
			// ManageShapeAndModelLibrariesMenuItem
			// 
			resources.ApplyResources(this.ManageShapeAndModelLibrariesMenuItem, "ManageShapeAndModelLibrariesMenuItem");
			this.ManageShapeAndModelLibrariesMenuItem.Name = "ManageShapeAndModelLibrariesMenuItem";
			this.ManageShapeAndModelLibrariesMenuItem.Click += new System.EventHandler(this.ManageShapeAndModelLibrariesMenuItem_Click);
			// 
			// toolStripMenuItem13
			// 
			resources.ApplyResources(this.toolStripMenuItem13, "toolStripMenuItem13");
			this.toolStripMenuItem13.Name = "toolStripMenuItem13";
			// 
			// exportRepositoryMenuItem
			// 
			resources.ApplyResources(this.exportRepositoryMenuItem, "exportRepositoryMenuItem");
			this.exportRepositoryMenuItem.Name = "exportRepositoryMenuItem";
			this.exportRepositoryMenuItem.Click += new System.EventHandler(this.exportRepositoryMenuItem_Click);
			// 
			// importRepositoryMenuItem
			// 
			resources.ApplyResources(this.importRepositoryMenuItem, "importRepositoryMenuItem");
			this.importRepositoryMenuItem.Name = "importRepositoryMenuItem";
			this.importRepositoryMenuItem.Click += new System.EventHandler(this.importRepositoryMenuItem_Click);
			// 
			// exportDiagramAsMenuItem
			// 
			resources.ApplyResources(this.exportDiagramAsMenuItem, "exportDiagramAsMenuItem");
			this.exportDiagramAsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportDialogToolStripMenuItem,
            this.emfExportMenuItem,
            this.wmfExportMenuItem,
            this.pngExportMenuItem,
            this.jpgExportMenuItem,
            this.bmpExportMenuItem});
			this.exportDiagramAsMenuItem.Name = "exportDiagramAsMenuItem";
			// 
			// exportDialogToolStripMenuItem
			// 
			resources.ApplyResources(this.exportDialogToolStripMenuItem, "exportDialogToolStripMenuItem");
			this.exportDialogToolStripMenuItem.Name = "exportDialogToolStripMenuItem";
			this.exportDialogToolStripMenuItem.Click += new System.EventHandler(this.exportDiagramAsMenuItem_Click);
			// 
			// emfExportMenuItem
			// 
			resources.ApplyResources(this.emfExportMenuItem, "emfExportMenuItem");
			this.emfExportMenuItem.Name = "emfExportMenuItem";
			this.emfExportMenuItem.Click += new System.EventHandler(this.emfPlusFileToolStripMenuItem_Click);
			// 
			// wmfExportMenuItem
			// 
			resources.ApplyResources(this.wmfExportMenuItem, "wmfExportMenuItem");
			this.wmfExportMenuItem.Name = "wmfExportMenuItem";
			this.wmfExportMenuItem.Click += new System.EventHandler(this.emfOnlyFileToolStripMenuItem_Click);
			// 
			// pngExportMenuItem
			// 
			resources.ApplyResources(this.pngExportMenuItem, "pngExportMenuItem");
			this.pngExportMenuItem.Name = "pngExportMenuItem";
			this.pngExportMenuItem.Click += new System.EventHandler(this.pngFileToolStripMenuItem_Click);
			// 
			// jpgExportMenuItem
			// 
			resources.ApplyResources(this.jpgExportMenuItem, "jpgExportMenuItem");
			this.jpgExportMenuItem.Name = "jpgExportMenuItem";
			this.jpgExportMenuItem.Click += new System.EventHandler(this.jpgFileToolStripMenuItem_Click);
			// 
			// bmpExportMenuItem
			// 
			resources.ApplyResources(this.bmpExportMenuItem, "bmpExportMenuItem");
			this.bmpExportMenuItem.Name = "bmpExportMenuItem";
			this.bmpExportMenuItem.Click += new System.EventHandler(this.bmpFileToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			// 
			// quitMenuItem
			// 
			resources.ApplyResources(this.quitMenuItem, "quitMenuItem");
			this.quitMenuItem.Name = "quitMenuItem";
			this.quitMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertDiagramMenuItem,
            this.deleteDiagramToolStripMenuItem,
            this.showDiagramSettingsToolStripMenuItem,
            this.toolStripMenuItem8,
            this.cutShapeOnlyMenuItem,
            this.cutShapeAndModelMenuItem,
            this.copyAsImageToolStripMenuItem,
            this.copyShapeOnlyMenuItem,
            this.copyShapeAndModelMenuItem,
            this.pasteMenuItem,
            this.deleteShapeOnlyMenuItem,
            this.deleteShapeAndModelMenuItem,
            this.toolStripMenuItem10,
            this.selectAllToolStripMenuItem,
            this.selectAllOfTypeToolStripMenuItem,
            this.selectAllOfTemplateToolStripMenuItem,
            this.unselectAllToolStripMenuItem,
            this.toolStripMenuItem2,
            this.toForegroundMenuItem,
            this.toBackgroundMenuItem,
            this.toolStripMenuItem6,
            this.undoMenuItem,
            this.redoMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			// 
			// insertDiagramMenuItem
			// 
			resources.ApplyResources(this.insertDiagramMenuItem, "insertDiagramMenuItem");
			this.insertDiagramMenuItem.Name = "insertDiagramMenuItem";
			this.insertDiagramMenuItem.Click += new System.EventHandler(this.newDiagramToolStripMenuItem_Click);
			// 
			// deleteDiagramToolStripMenuItem
			// 
			resources.ApplyResources(this.deleteDiagramToolStripMenuItem, "deleteDiagramToolStripMenuItem");
			this.deleteDiagramToolStripMenuItem.Name = "deleteDiagramToolStripMenuItem";
			this.deleteDiagramToolStripMenuItem.Click += new System.EventHandler(this.deleteDiagramToolStripMenuItem_Click);
			// 
			// showDiagramSettingsToolStripMenuItem
			// 
			resources.ApplyResources(this.showDiagramSettingsToolStripMenuItem, "showDiagramSettingsToolStripMenuItem");
			this.showDiagramSettingsToolStripMenuItem.Name = "showDiagramSettingsToolStripMenuItem";
			this.showDiagramSettingsToolStripMenuItem.Click += new System.EventHandler(this.showDiagramSettingsToolStripMenuItem_Click);
			// 
			// toolStripMenuItem8
			// 
			resources.ApplyResources(this.toolStripMenuItem8, "toolStripMenuItem8");
			this.toolStripMenuItem8.Name = "toolStripMenuItem8";
			// 
			// cutShapeOnlyMenuItem
			// 
			resources.ApplyResources(this.cutShapeOnlyMenuItem, "cutShapeOnlyMenuItem");
			this.cutShapeOnlyMenuItem.Name = "cutShapeOnlyMenuItem";
			this.cutShapeOnlyMenuItem.Click += new System.EventHandler(this.cutShapeOnlyItem_Click);
			// 
			// cutShapeAndModelMenuItem
			// 
			resources.ApplyResources(this.cutShapeAndModelMenuItem, "cutShapeAndModelMenuItem");
			this.cutShapeAndModelMenuItem.Name = "cutShapeAndModelMenuItem";
			this.cutShapeAndModelMenuItem.Click += new System.EventHandler(this.cutShapeAndModelItem_Click);
			// 
			// copyAsImageToolStripMenuItem
			// 
			resources.ApplyResources(this.copyAsImageToolStripMenuItem, "copyAsImageToolStripMenuItem");
			this.copyAsImageToolStripMenuItem.Image = global::Dataweb.NShape.Viewer.Properties.Resources.CopyAsImage;
			this.copyAsImageToolStripMenuItem.Name = "copyAsImageToolStripMenuItem";
			this.copyAsImageToolStripMenuItem.Click += new System.EventHandler(this.copyAsImageToolStripMenuItem_Click);
			// 
			// copyShapeOnlyMenuItem
			// 
			resources.ApplyResources(this.copyShapeOnlyMenuItem, "copyShapeOnlyMenuItem");
			this.copyShapeOnlyMenuItem.Name = "copyShapeOnlyMenuItem";
			this.copyShapeOnlyMenuItem.Click += new System.EventHandler(this.copyShapeOnlyItem_Click);
			// 
			// copyShapeAndModelMenuItem
			// 
			resources.ApplyResources(this.copyShapeAndModelMenuItem, "copyShapeAndModelMenuItem");
			this.copyShapeAndModelMenuItem.Name = "copyShapeAndModelMenuItem";
			this.copyShapeAndModelMenuItem.Click += new System.EventHandler(this.copyShapeAndModelItem_Click);
			// 
			// pasteMenuItem
			// 
			resources.ApplyResources(this.pasteMenuItem, "pasteMenuItem");
			this.pasteMenuItem.Name = "pasteMenuItem";
			this.pasteMenuItem.Click += new System.EventHandler(this.pasteMenuItem_Click);
			// 
			// deleteShapeOnlyMenuItem
			// 
			resources.ApplyResources(this.deleteShapeOnlyMenuItem, "deleteShapeOnlyMenuItem");
			this.deleteShapeOnlyMenuItem.Name = "deleteShapeOnlyMenuItem";
			this.deleteShapeOnlyMenuItem.Click += new System.EventHandler(this.deleteShapeOnlyItem_Click);
			// 
			// deleteShapeAndModelMenuItem
			// 
			resources.ApplyResources(this.deleteShapeAndModelMenuItem, "deleteShapeAndModelMenuItem");
			this.deleteShapeAndModelMenuItem.Name = "deleteShapeAndModelMenuItem";
			this.deleteShapeAndModelMenuItem.Click += new System.EventHandler(this.deleteShapeAndModelItem_Click);
			// 
			// toolStripMenuItem10
			// 
			resources.ApplyResources(this.toolStripMenuItem10, "toolStripMenuItem10");
			this.toolStripMenuItem10.Name = "toolStripMenuItem10";
			// 
			// selectAllToolStripMenuItem
			// 
			resources.ApplyResources(this.selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
			// 
			// selectAllOfTypeToolStripMenuItem
			// 
			resources.ApplyResources(this.selectAllOfTypeToolStripMenuItem, "selectAllOfTypeToolStripMenuItem");
			this.selectAllOfTypeToolStripMenuItem.Name = "selectAllOfTypeToolStripMenuItem";
			this.selectAllOfTypeToolStripMenuItem.Click += new System.EventHandler(this.selectAllShapesOfTheSameTypeToolStripMenuItem_Click);
			// 
			// selectAllOfTemplateToolStripMenuItem
			// 
			resources.ApplyResources(this.selectAllOfTemplateToolStripMenuItem, "selectAllOfTemplateToolStripMenuItem");
			this.selectAllOfTemplateToolStripMenuItem.Name = "selectAllOfTemplateToolStripMenuItem";
			this.selectAllOfTemplateToolStripMenuItem.Click += new System.EventHandler(this.selectAllShapesOfTheSameTemplateToolStripMenuItem_Click);
			// 
			// unselectAllToolStripMenuItem
			// 
			resources.ApplyResources(this.unselectAllToolStripMenuItem, "unselectAllToolStripMenuItem");
			this.unselectAllToolStripMenuItem.Name = "unselectAllToolStripMenuItem";
			this.unselectAllToolStripMenuItem.Click += new System.EventHandler(this.unselectAllToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			// 
			// toForegroundMenuItem
			// 
			resources.ApplyResources(this.toForegroundMenuItem, "toForegroundMenuItem");
			this.toForegroundMenuItem.Name = "toForegroundMenuItem";
			this.toForegroundMenuItem.Click += new System.EventHandler(this.toForegroundMenuItem_Click);
			// 
			// toBackgroundMenuItem
			// 
			resources.ApplyResources(this.toBackgroundMenuItem, "toBackgroundMenuItem");
			this.toBackgroundMenuItem.Name = "toBackgroundMenuItem";
			this.toBackgroundMenuItem.Click += new System.EventHandler(this.toBackgroundMenuItem_Click);
			// 
			// toolStripMenuItem6
			// 
			resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			// 
			// undoMenuItem
			// 
			resources.ApplyResources(this.undoMenuItem, "undoMenuItem");
			this.undoMenuItem.Name = "undoMenuItem";
			this.undoMenuItem.Click += new System.EventHandler(this.undoButton_Click);
			// 
			// redoMenuItem
			// 
			resources.ApplyResources(this.redoMenuItem, "redoMenuItem");
			this.redoMenuItem.Name = "redoMenuItem";
			this.redoMenuItem.Click += new System.EventHandler(this.redoButton_Click);
			// 
			// viewToolStripMenuItem
			// 
			resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showGridMenuItem,
            this.refreshToolStripMenuItem,
            this.showDisplaySettingsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.editDesignsAndStylesToolStripMenuItem,
            this.viewShowLayoutControlToolStripMenuItem,
            this.toolStripMenuItem4,
            this.highQualityRenderingMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			// 
			// showGridMenuItem
			// 
			resources.ApplyResources(this.showGridMenuItem, "showGridMenuItem");
			this.showGridMenuItem.Checked = true;
			this.showGridMenuItem.CheckOnClick = true;
			this.showGridMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.showGridMenuItem.Name = "showGridMenuItem";
			this.showGridMenuItem.Click += new System.EventHandler(this.showGridToolStripMenuItem_Click);
			// 
			// refreshToolStripMenuItem
			// 
			resources.ApplyResources(this.refreshToolStripMenuItem, "refreshToolStripMenuItem");
			this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshButton_Click);
			// 
			// showDisplaySettingsToolStripMenuItem
			// 
			resources.ApplyResources(this.showDisplaySettingsToolStripMenuItem, "showDisplaySettingsToolStripMenuItem");
			this.showDisplaySettingsToolStripMenuItem.Name = "showDisplaySettingsToolStripMenuItem";
			this.showDisplaySettingsToolStripMenuItem.Click += new System.EventHandler(this.showDisplaySettingsItem_Click);
			// 
			// toolStripMenuItem3
			// 
			resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			// 
			// editDesignsAndStylesToolStripMenuItem
			// 
			resources.ApplyResources(this.editDesignsAndStylesToolStripMenuItem, "editDesignsAndStylesToolStripMenuItem");
			this.editDesignsAndStylesToolStripMenuItem.Name = "editDesignsAndStylesToolStripMenuItem";
			this.editDesignsAndStylesToolStripMenuItem.Click += new System.EventHandler(this.editDesignsAndStylesToolStripMenuItem_Click);
			// 
			// viewShowLayoutControlToolStripMenuItem
			// 
			resources.ApplyResources(this.viewShowLayoutControlToolStripMenuItem, "viewShowLayoutControlToolStripMenuItem");
			this.viewShowLayoutControlToolStripMenuItem.Name = "viewShowLayoutControlToolStripMenuItem";
			this.viewShowLayoutControlToolStripMenuItem.Click += new System.EventHandler(this.viewShowLayoutControlToolStripMenuItem_Click);
			// 
			// toolStripMenuItem4
			// 
			resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			// 
			// highQualityRenderingMenuItem
			// 
			resources.ApplyResources(this.highQualityRenderingMenuItem, "highQualityRenderingMenuItem");
			this.highQualityRenderingMenuItem.Checked = true;
			this.highQualityRenderingMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.highQualityRenderingMenuItem.Name = "highQualityRenderingMenuItem";
			this.highQualityRenderingMenuItem.Click += new System.EventHandler(this.highQualityToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolbarsToolStripMenuItem,
            this.configSettingsRefreshToolStripMenuItem,
            this.toolStripMenuItem11,
            this.adoNetDatabaseGeneratorToolStripMenuItem,
            this.testDataGeneratorToolStripMenuItem,
            this.toolStripSeparator3,
            this.nShapeEventMonitorToolStripMenuItem,
            this.toolStripMenuItem12,
            this.cLIPSTestToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			// 
			// resetToolbarsToolStripMenuItem
			// 
			resources.ApplyResources(this.resetToolbarsToolStripMenuItem, "resetToolbarsToolStripMenuItem");
			this.resetToolbarsToolStripMenuItem.Name = "resetToolbarsToolStripMenuItem";
			this.resetToolbarsToolStripMenuItem.Click += new System.EventHandler(this.resetToolbarsToolStripMenuItem_Click);
			// 
			// configSettingsRefreshToolStripMenuItem
			// 
			resources.ApplyResources(this.configSettingsRefreshToolStripMenuItem, "configSettingsRefreshToolStripMenuItem");
			this.configSettingsRefreshToolStripMenuItem.Name = "configSettingsRefreshToolStripMenuItem";
			this.configSettingsRefreshToolStripMenuItem.Click += new System.EventHandler(this.configSettingsRefreshToolStripMenuItem_Click);
			// 
			// toolStripMenuItem11
			// 
			resources.ApplyResources(this.toolStripMenuItem11, "toolStripMenuItem11");
			this.toolStripMenuItem11.Name = "toolStripMenuItem11";
			// 
			// adoNetDatabaseGeneratorToolStripMenuItem
			// 
			resources.ApplyResources(this.adoNetDatabaseGeneratorToolStripMenuItem, "adoNetDatabaseGeneratorToolStripMenuItem");
			this.adoNetDatabaseGeneratorToolStripMenuItem.Name = "adoNetDatabaseGeneratorToolStripMenuItem";
			this.adoNetDatabaseGeneratorToolStripMenuItem.Click += new System.EventHandler(this.adoNetDatabaseGeneratorToolStripMenuItem_Click);
			// 
			// testDataGeneratorToolStripMenuItem
			// 
			resources.ApplyResources(this.testDataGeneratorToolStripMenuItem, "testDataGeneratorToolStripMenuItem");
			this.testDataGeneratorToolStripMenuItem.Name = "testDataGeneratorToolStripMenuItem";
			this.testDataGeneratorToolStripMenuItem.Click += new System.EventHandler(this.testDataGeneratorToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			// 
			// nShapeEventMonitorToolStripMenuItem
			// 
			resources.ApplyResources(this.nShapeEventMonitorToolStripMenuItem, "nShapeEventMonitorToolStripMenuItem");
			this.nShapeEventMonitorToolStripMenuItem.CheckOnClick = true;
			this.nShapeEventMonitorToolStripMenuItem.Name = "nShapeEventMonitorToolStripMenuItem";
			this.nShapeEventMonitorToolStripMenuItem.Click += new System.EventHandler(this.nShapeEventMonitorToolStripMenuItem_Click);
			// 
			// toolStripMenuItem12
			// 
			resources.ApplyResources(this.toolStripMenuItem12, "toolStripMenuItem12");
			this.toolStripMenuItem12.Name = "toolStripMenuItem12";
			// 
			// cLIPSTestToolStripMenuItem
			// 
			resources.ApplyResources(this.cLIPSTestToolStripMenuItem, "cLIPSTestToolStripMenuItem");
			this.cLIPSTestToolStripMenuItem.Name = "cLIPSTestToolStripMenuItem";
			this.cLIPSTestToolStripMenuItem.Click += new System.EventHandler(this.cLIPSTestToolStripMenuItem_Click);
			// 
			// toolStripMenuItem9
			// 
			resources.ApplyResources(this.toolStripMenuItem9, "toolStripMenuItem9");
			this.toolStripMenuItem9.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewHelpToolStripMenuItem,
            this.aboutToolStripMenuItem});
			this.toolStripMenuItem9.Name = "toolStripMenuItem9";
			// 
			// viewHelpToolStripMenuItem
			// 
			resources.ApplyResources(this.viewHelpToolStripMenuItem, "viewHelpToolStripMenuItem");
			this.viewHelpToolStripMenuItem.Name = "viewHelpToolStripMenuItem";
			this.viewHelpToolStripMenuItem.Click += new System.EventHandler(this.viewHelpToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem
			// 
			resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// editToolStrip
			// 
			resources.ApplyResources(this.editToolStrip, "editToolStrip");
			this.editToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripButton,
            this.openProjectToolStripButton,
            this.saveProjectToolStripButton,
            this.toolStripSeparator1,
            this.cutShapeButton,
            this.copyShapeButton,
            this.pasteButton,
            this.deleteShapeButton,
            this.toolStripSeparator2,
            this.undoToolStripSplitButton,
            this.redoToolStripSplitButton});
			this.editToolStrip.Name = "editToolStrip";
			this.toolTip.SetToolTip(this.editToolStrip, resources.GetString("editToolStrip.ToolTip"));
			// 
			// newProjectToolStripButton
			// 
			resources.ApplyResources(this.newProjectToolStripButton, "newProjectToolStripButton");
			this.newProjectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.newProjectToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xmlRepositoryToolStripMenuItem,
            this.sQLRepositoryToolStripMenuItem});
			this.newProjectToolStripButton.Name = "newProjectToolStripButton";
			// 
			// xmlRepositoryToolStripMenuItem
			// 
			resources.ApplyResources(this.xmlRepositoryToolStripMenuItem, "xmlRepositoryToolStripMenuItem");
			this.xmlRepositoryToolStripMenuItem.Name = "xmlRepositoryToolStripMenuItem";
			this.xmlRepositoryToolStripMenuItem.Click += new System.EventHandler(this.newXMLRepositoryToolStripMenuItem_Click);
			// 
			// sQLRepositoryToolStripMenuItem
			// 
			resources.ApplyResources(this.sQLRepositoryToolStripMenuItem, "sQLRepositoryToolStripMenuItem");
			this.sQLRepositoryToolStripMenuItem.Name = "sQLRepositoryToolStripMenuItem";
			this.sQLRepositoryToolStripMenuItem.Click += new System.EventHandler(this.newSQLServerRepositoryToolStripMenuItem_Click);
			// 
			// openProjectToolStripButton
			// 
			resources.ApplyResources(this.openProjectToolStripButton, "openProjectToolStripButton");
			this.openProjectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.openProjectToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openXMLRepositoryToolStripMenuItem1,
            this.openSQLRepositoryToolStripMenuItem});
			this.openProjectToolStripButton.Name = "openProjectToolStripButton";
			// 
			// openXMLRepositoryToolStripMenuItem1
			// 
			resources.ApplyResources(this.openXMLRepositoryToolStripMenuItem1, "openXMLRepositoryToolStripMenuItem1");
			this.openXMLRepositoryToolStripMenuItem1.Name = "openXMLRepositoryToolStripMenuItem1";
			this.openXMLRepositoryToolStripMenuItem1.Click += new System.EventHandler(this.openXMLRepositoryToolStripMenuItem_Click);
			// 
			// openSQLRepositoryToolStripMenuItem
			// 
			resources.ApplyResources(this.openSQLRepositoryToolStripMenuItem, "openSQLRepositoryToolStripMenuItem");
			this.openSQLRepositoryToolStripMenuItem.Name = "openSQLRepositoryToolStripMenuItem";
			this.openSQLRepositoryToolStripMenuItem.Click += new System.EventHandler(this.openSQLServerRepositoryToolStripMenuItem_Click);
			// 
			// saveProjectToolStripButton
			// 
			resources.ApplyResources(this.saveProjectToolStripButton, "saveProjectToolStripButton");
			this.saveProjectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.saveProjectToolStripButton.Name = "saveProjectToolStripButton";
			this.saveProjectToolStripButton.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			// 
			// cutShapeButton
			// 
			resources.ApplyResources(this.cutShapeButton, "cutShapeButton");
			this.cutShapeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.cutShapeButton.Name = "cutShapeButton";
			this.cutShapeButton.Click += new System.EventHandler(this.cutShapeOnlyItem_Click);
			// 
			// copyShapeButton
			// 
			resources.ApplyResources(this.copyShapeButton, "copyShapeButton");
			this.copyShapeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.copyShapeButton.Name = "copyShapeButton";
			this.copyShapeButton.Click += new System.EventHandler(this.copyShapeOnlyItem_Click);
			// 
			// pasteButton
			// 
			resources.ApplyResources(this.pasteButton, "pasteButton");
			this.pasteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.pasteButton.Name = "pasteButton";
			this.pasteButton.Click += new System.EventHandler(this.pasteMenuItem_Click);
			// 
			// deleteShapeButton
			// 
			resources.ApplyResources(this.deleteShapeButton, "deleteShapeButton");
			this.deleteShapeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.deleteShapeButton.Name = "deleteShapeButton";
			this.deleteShapeButton.Click += new System.EventHandler(this.deleteShapeOnlyItem_Click);
			// 
			// toolStripSeparator2
			// 
			resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			// 
			// undoToolStripSplitButton
			// 
			resources.ApplyResources(this.undoToolStripSplitButton, "undoToolStripSplitButton");
			this.undoToolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.undoToolStripSplitButton.Name = "undoToolStripSplitButton";
			this.undoToolStripSplitButton.ButtonClick += new System.EventHandler(this.undoButton_Click);
			this.undoToolStripSplitButton.DropDownOpening += new System.EventHandler(this.undoToolStripSplitButton_DropDownOpening);
			// 
			// redoToolStripSplitButton
			// 
			resources.ApplyResources(this.redoToolStripSplitButton, "redoToolStripSplitButton");
			this.redoToolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.redoToolStripSplitButton.Name = "redoToolStripSplitButton";
			this.redoToolStripSplitButton.ButtonClick += new System.EventHandler(this.redoButton_Click);
			this.redoToolStripSplitButton.DropDownOpening += new System.EventHandler(this.redoToolStripSplitButton_DropDownOpening);
			// 
			// settingsToolStrip
			// 
			resources.ApplyResources(this.settingsToolStrip, "settingsToolStrip");
			this.settingsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prevDiagramButton,
            this.nextDiagramButton,
            this.zoomToolStripComboBox,
            this.displaySettingsToolStripButton,
            this.refreshToolbarButton,
            this.showGridToolbarButton});
			this.settingsToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.settingsToolStrip.Name = "settingsToolStrip";
			this.toolTip.SetToolTip(this.settingsToolStrip, resources.GetString("settingsToolStrip.ToolTip"));
			// 
			// prevDiagramButton
			// 
			resources.ApplyResources(this.prevDiagramButton, "prevDiagramButton");
			this.prevDiagramButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.prevDiagramButton.Name = "prevDiagramButton";
			this.prevDiagramButton.Click += new System.EventHandler(this.backButton_Click);
			// 
			// nextDiagramButton
			// 
			resources.ApplyResources(this.nextDiagramButton, "nextDiagramButton");
			this.nextDiagramButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.nextDiagramButton.Name = "nextDiagramButton";
			this.nextDiagramButton.Click += new System.EventHandler(this.forwardButton_Click);
			// 
			// zoomToolStripComboBox
			// 
			resources.ApplyResources(this.zoomToolStripComboBox, "zoomToolStripComboBox");
			this.zoomToolStripComboBox.DropDownWidth = 66;
			this.zoomToolStripComboBox.Items.AddRange(new object[] {
            resources.GetString("zoomToolStripComboBox.Items"),
            resources.GetString("zoomToolStripComboBox.Items1"),
            resources.GetString("zoomToolStripComboBox.Items2"),
            resources.GetString("zoomToolStripComboBox.Items3"),
            resources.GetString("zoomToolStripComboBox.Items4"),
            resources.GetString("zoomToolStripComboBox.Items5"),
            resources.GetString("zoomToolStripComboBox.Items6"),
            resources.GetString("zoomToolStripComboBox.Items7"),
            resources.GetString("zoomToolStripComboBox.Items8"),
            resources.GetString("zoomToolStripComboBox.Items9"),
            resources.GetString("zoomToolStripComboBox.Items10"),
            resources.GetString("zoomToolStripComboBox.Items11"),
            resources.GetString("zoomToolStripComboBox.Items12"),
            resources.GetString("zoomToolStripComboBox.Items13"),
            resources.GetString("zoomToolStripComboBox.Items14"),
            resources.GetString("zoomToolStripComboBox.Items15"),
            resources.GetString("zoomToolStripComboBox.Items16"),
            resources.GetString("zoomToolStripComboBox.Items17"),
            resources.GetString("zoomToolStripComboBox.Items18")});
			this.zoomToolStripComboBox.Name = "zoomToolStripComboBox";
			this.zoomToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.zoomToolStripComboBox_SelectedIndexChanged);
			this.zoomToolStripComboBox.TextChanged += new System.EventHandler(this.toolStripComboBox1_TextChanged);
			// 
			// displaySettingsToolStripButton
			// 
			resources.ApplyResources(this.displaySettingsToolStripButton, "displaySettingsToolStripButton");
			this.displaySettingsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.displaySettingsToolStripButton.Name = "displaySettingsToolStripButton";
			this.displaySettingsToolStripButton.Click += new System.EventHandler(this.showDisplaySettingsItem_Click);
			// 
			// refreshToolbarButton
			// 
			resources.ApplyResources(this.refreshToolbarButton, "refreshToolbarButton");
			this.refreshToolbarButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.refreshToolbarButton.Name = "refreshToolbarButton";
			this.refreshToolbarButton.Click += new System.EventHandler(this.refreshButton_Click);
			// 
			// showGridToolbarButton
			// 
			resources.ApplyResources(this.showGridToolbarButton, "showGridToolbarButton");
			this.showGridToolbarButton.Checked = true;
			this.showGridToolbarButton.CheckOnClick = true;
			this.showGridToolbarButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this.showGridToolbarButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.showGridToolbarButton.Name = "showGridToolbarButton";
			this.showGridToolbarButton.Click += new System.EventHandler(this.showGridToolStripMenuItem_Click);
			// 
			// displayToolStrip
			// 
			resources.ApplyResources(this.displayToolStrip, "displayToolStrip");
			this.displayToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.designEditorToolStripButton,
            this.runtimeModeComboBox});
			this.displayToolStrip.Name = "displayToolStrip";
			this.toolTip.SetToolTip(this.displayToolStrip, resources.GetString("displayToolStrip.ToolTip"));
			// 
			// toolStripButton2
			// 
			resources.ApplyResources(this.toolStripButton2, "toolStripButton2");
			this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton2.Name = "toolStripButton2";
			this.toolStripButton2.Click += new System.EventHandler(this.showDiagramSettingsToolStripMenuItem_Click);
			// 
			// designEditorToolStripButton
			// 
			resources.ApplyResources(this.designEditorToolStripButton, "designEditorToolStripButton");
			this.designEditorToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.designEditorToolStripButton.Name = "designEditorToolStripButton";
			this.designEditorToolStripButton.Click += new System.EventHandler(this.editDesignsAndStylesToolStripMenuItem_Click);
			// 
			// runtimeModeComboBox
			// 
			resources.ApplyResources(this.runtimeModeComboBox, "runtimeModeComboBox");
			this.runtimeModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.runtimeModeComboBox.DropDownWidth = 100;
			this.runtimeModeComboBox.Items.AddRange(new object[] {
            resources.GetString("runtimeModeComboBox.Items"),
            resources.GetString("runtimeModeComboBox.Items1"),
            resources.GetString("runtimeModeComboBox.Items2"),
            resources.GetString("runtimeModeComboBox.Items3"),
            resources.GetString("runtimeModeComboBox.Items4")});
			this.runtimeModeComboBox.Name = "runtimeModeComboBox";
			this.runtimeModeComboBox.SelectedIndexChanged += new System.EventHandler(this.runtimeModeButton_SelectedIndexChanged);
			// 
			// debugToolStrip
			// 
			resources.ApplyResources(this.debugToolStrip, "debugToolStrip");
			this.debugToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugDrawOccupationToolbarButton,
            this.debugDrawInvalidatedAreaToolbarButton});
			this.debugToolStrip.Name = "debugToolStrip";
			this.toolTip.SetToolTip(this.debugToolStrip, resources.GetString("debugToolStrip.ToolTip"));
			// 
			// debugDrawOccupationToolbarButton
			// 
			resources.ApplyResources(this.debugDrawOccupationToolbarButton, "debugDrawOccupationToolbarButton");
			this.debugDrawOccupationToolbarButton.CheckOnClick = true;
			this.debugDrawOccupationToolbarButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.debugDrawOccupationToolbarButton.Name = "debugDrawOccupationToolbarButton";
			this.debugDrawOccupationToolbarButton.Click += new System.EventHandler(this.debugDrawOccupationToolbarButton_Click);
			// 
			// debugDrawInvalidatedAreaToolbarButton
			// 
			resources.ApplyResources(this.debugDrawInvalidatedAreaToolbarButton, "debugDrawInvalidatedAreaToolbarButton");
			this.debugDrawInvalidatedAreaToolbarButton.CheckOnClick = true;
			this.debugDrawInvalidatedAreaToolbarButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.debugDrawInvalidatedAreaToolbarButton.Name = "debugDrawInvalidatedAreaToolbarButton";
			this.debugDrawInvalidatedAreaToolbarButton.Click += new System.EventHandler(this.debugDrawInvalidatedAreaToolbarButton_Click);
			// 
			// BottomToolStripPanel
			// 
			resources.ApplyResources(this.BottomToolStripPanel, "BottomToolStripPanel");
			this.BottomToolStripPanel.Name = "BottomToolStripPanel";
			this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.toolTip.SetToolTip(this.BottomToolStripPanel, resources.GetString("BottomToolStripPanel.ToolTip"));
			// 
			// TopToolStripPanel
			// 
			resources.ApplyResources(this.TopToolStripPanel, "TopToolStripPanel");
			this.TopToolStripPanel.Name = "TopToolStripPanel";
			this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.toolTip.SetToolTip(this.TopToolStripPanel, resources.GetString("TopToolStripPanel.ToolTip"));
			// 
			// defaultToolStripMenuItem
			// 
			resources.ApplyResources(this.defaultToolStripMenuItem, "defaultToolStripMenuItem");
			this.defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
			// 
			// RightToolStripPanel
			// 
			resources.ApplyResources(this.RightToolStripPanel, "RightToolStripPanel");
			this.RightToolStripPanel.Name = "RightToolStripPanel";
			this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.toolTip.SetToolTip(this.RightToolStripPanel, resources.GetString("RightToolStripPanel.ToolTip"));
			// 
			// LeftToolStripPanel
			// 
			resources.ApplyResources(this.LeftToolStripPanel, "LeftToolStripPanel");
			this.LeftToolStripPanel.Name = "LeftToolStripPanel";
			this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.toolTip.SetToolTip(this.LeftToolStripPanel, resources.GetString("LeftToolStripPanel.ToolTip"));
			// 
			// ContentPanel
			// 
			resources.ApplyResources(this.ContentPanel, "ContentPanel");
			this.toolTip.SetToolTip(this.ContentPanel, resources.GetString("ContentPanel.ToolTip"));
			// 
			// openFileDialog
			// 
			resources.ApplyResources(this.openFileDialog, "openFileDialog");
			// 
			// saveFileDialog
			// 
			resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
			// 
			// diagramSetController
			// 
			this.diagramSetController.ActiveTool = null;
			this.diagramSetController.Project = this.project;
			// 
			// project
			// 
			this.project.Description = null;
			this.project.LibrarySearchPaths = ((System.Collections.Generic.IList<string>)(resources.GetObject("project.LibrarySearchPaths")));
			this.project.Name = "";
			this.project.Repository = this.cachedRepository;
			roleBasedSecurityManager1.CurrentRole = Dataweb.NShape.StandardRole.Administrator;
			roleBasedSecurityManager1.CurrentRoleName = "Administrator";
			this.project.SecurityManager = roleBasedSecurityManager1;
			this.project.Opened += new System.EventHandler(this.project_Opened);
			this.project.Closed += new System.EventHandler(this.project_Closed);
			this.project.LibraryLoaded += new System.EventHandler<Dataweb.NShape.LibraryLoadedEventArgs>(this.project_LibraryLoaded);
			// 
			// cachedRepository
			// 
			this.cachedRepository.ProjectName = "";
			this.cachedRepository.Store = null;
			this.cachedRepository.Version = 0;
			// 
			// layerController
			// 
			this.layerController.DiagramSetController = this.diagramSetController;
			// 
			// toolSetController
			// 
			this.toolSetController.DiagramSetController = this.diagramSetController;
			this.toolSetController.DesignEditorSelected += new System.EventHandler(this.toolBoxAdapter_ShowDesignEditor);
			this.toolSetController.LibraryManagerSelected += new System.EventHandler(this.toolBoxAdapter_ShowLibraryManagerDialog);
			this.toolSetController.TemplateEditorSelected += new System.EventHandler<Dataweb.NShape.Controllers.TemplateEditorEventArgs>(this.toolBoxAdapter_ShowTemplateEditorDialog);
			// 
			// modelTreeController
			// 
			this.modelTreeController.DiagramSetController = this.diagramSetController;
			// 
			// propertyController
			// 
			this.propertyController.Project = this.project;
			this.propertyController.PropertyDisplayMode = Dataweb.NShape.Controllers.NonEditableDisplayMode.ReadOnly;
			// 
			// modelTreePresenter
			// 
			this.modelTreePresenter.HideDeniedMenuItems = false;
			this.modelTreePresenter.ModelTreeController = this.modelTreeController;
			this.modelTreePresenter.PropertyController = this.propertyController;
			this.modelTreePresenter.ShowDefaultContextMenu = true;
			this.modelTreePresenter.TreeView = this.modelTreeView;
			// 
			// propertyPresenter
			// 
			this.propertyPresenter.PrimaryPropertyGrid = this.primaryPropertyGrid;
			this.propertyPresenter.PropertyController = this.propertyController;
			this.propertyPresenter.SecondaryPropertyGrid = this.secondaryPropertyGrid;
			// 
			// toolSetListViewPresenter
			// 
			this.toolSetListViewPresenter.HideDeniedMenuItems = false;
			this.toolSetListViewPresenter.ListView = this.toolboxListView;
			this.toolSetListViewPresenter.ShowDefaultContextMenu = true;
			this.toolSetListViewPresenter.ToolSetController = this.toolSetController;
			// 
			// layerPresenter
			// 
			this.layerPresenter.DiagramPresenter = null;
			this.layerPresenter.HideDeniedMenuItems = false;
			this.layerPresenter.LayerController = this.layerController;
			this.layerPresenter.LayerView = this.layerEditorListView;
			// 
			// diagramContextMenuStrip
			// 
			resources.ApplyResources(this.diagramContextMenuStrip, "diagramContextMenuStrip");
			this.diagramContextMenuStrip.Name = "myContextMenuStrip";
			this.toolTip.SetToolTip(this.diagramContextMenuStrip, resources.GetString("diagramContextMenuStrip.ToolTip"));
			this.diagramContextMenuStrip.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.display_ContextMenuStrip_Closed);
			this.diagramContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.display_ContextMenuStrip_Opening);
			// 
			// timerClock
			// 
			this.timerClock.Tick += new System.EventHandler(this.timerClock_Tick);
			// 
			// toolTipTimer
			// 
			this.toolTipTimer.Tick += new System.EventHandler(this.toolTipTimer_Tick);
			// 
			// timerEnergyFlow
			// 
			this.timerEnergyFlow.Tick += new System.EventHandler(this.timerEnergyFlow_Tick);
			// 
			// DiagramDesignerMainForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.toolStripContainer);
			this.DoubleBuffered = true;
			this.MainMenuStrip = this.mainMenuStrip;
			this.Name = "DiagramDesignerMainForm";
			this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DiagramDesignerMainForm_FormClosing);
			this.Load += new System.EventHandler(this.DiagramDesignerMainForm_Load);
			this.Shown += new System.EventHandler(this.DiagramDesignerMainForm_Shown);
			this.toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
			this.toolStripContainer.BottomToolStripPanel.PerformLayout();
			this.toolStripContainer.ContentPanel.ResumeLayout(false);
			this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer.TopToolStripPanel.PerformLayout();
			this.toolStripContainer.ResumeLayout(false);
			this.toolStripContainer.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.historyTrackBar)).EndInit();
			this.toolboxPropsPanel.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.toolboxContextMenuStrip.ResumeLayout(false);
			this.propertyWindowTabControl.ResumeLayout(false);
			this.hierarchyTab.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.hierarchyTreeView)).EndInit();
			this.propertyWindowShapeTab.ResumeLayout(false);
			this.propertyWindowModelTab.ResumeLayout(false);
			this.layersTab.ResumeLayout(false);
			this.mainMenuStrip.ResumeLayout(false);
			this.mainMenuStrip.PerformLayout();
			this.editToolStrip.ResumeLayout(false);
			this.editToolStrip.PerformLayout();
			this.settingsToolStrip.ResumeLayout(false);
			this.settingsToolStrip.PerformLayout();
			this.displayToolStrip.ResumeLayout(false);
			this.displayToolStrip.PerformLayout();
			this.debugToolStrip.ResumeLayout(false);
			this.debugToolStrip.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel statusLabelMessage;
		private System.Windows.Forms.MenuStrip mainMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem quitMenuItem;
		private System.Windows.Forms.TreeView modelTreeView;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel toolboxPropsPanel;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.ToolStripContainer toolStripContainer;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem undoMenuItem;
		private System.Windows.Forms.ToolStripMenuItem redoMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem showGridMenuItem;
		private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
		private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
		private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
		private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
		private System.Windows.Forms.ToolStripContentPanel ContentPanel;
		private System.Windows.Forms.ToolStrip editToolStrip;
		private System.Windows.Forms.ToolStripButton pasteButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripSplitButton undoToolStripSplitButton;
		private System.Windows.Forms.ToolStripSplitButton redoToolStripSplitButton;
		private System.Windows.Forms.TrackBar historyTrackBar;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem defaultToolStripMenuItem;
		private Dataweb.NShape.WinFormsUI.ModelTreeViewPresenter modelTreePresenter;
		private System.Windows.Forms.TabControl propertyWindowTabControl;
		private System.Windows.Forms.TabPage propertyWindowShapeTab;
		private System.Windows.Forms.PropertyGrid primaryPropertyGrid;
		private System.Windows.Forms.TabPage propertyWindowModelTab;
		private System.Windows.Forms.PropertyGrid secondaryPropertyGrid;
		private Dataweb.NShape.WinFormsUI.PropertyPresenter propertyPresenter;
		private Dataweb.NShape.Controllers.ToolSetController toolSetController;
		private System.Windows.Forms.ListView toolboxListView;
		private System.Windows.Forms.ToolStrip displayToolStrip;
		private System.Windows.Forms.ToolStripMenuItem openProjectMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private Dataweb.NShape.Project project;
		private System.Windows.Forms.ToolStrip settingsToolStrip;
		private System.Windows.Forms.ToolStripComboBox zoomToolStripComboBox;
		private System.Windows.Forms.TabControl displayTabControl;
		private System.Windows.Forms.ToolStripMenuItem highQualityRenderingMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem exportRepositoryMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importRepositoryMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportDiagramAsMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pngExportMenuItem;
		private System.Windows.Forms.ToolStripMenuItem emfExportMenuItem;
		private System.Windows.Forms.ToolStripMenuItem jpgExportMenuItem;
		private System.Windows.Forms.ToolStripMenuItem bmpExportMenuItem;
		private System.Windows.Forms.ToolStripMenuItem wmfExportMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ManageShapeAndModelLibrariesMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem13;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
		private System.Windows.Forms.ToolStripMenuItem toForegroundMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toBackgroundMenuItem;
		private System.Windows.Forms.ToolStripComboBox runtimeModeComboBox;
#if TdbRepository
		private Dataweb.NShape.TurboDBRepository turboDBRepository;
#endif

		private System.Windows.Forms.ToolStripMenuItem viewShowLayoutControlToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
		private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertDiagramMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteDiagramToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
		private System.Windows.Forms.ToolStripMenuItem editDesignsAndStylesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showDisplaySettingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutShapeOnlyMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutShapeAndModelMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyShapeOnlyMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyShapeAndModelMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteShapeOnlyMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteShapeAndModelMenuItem;
		private System.Windows.Forms.ToolStripButton cutShapeButton;
		private System.Windows.Forms.ToolStripButton copyShapeButton;
		private System.Windows.Forms.ToolStripButton deleteShapeButton;
		private Dataweb.NShape.WinFormsUI.ToolSetListViewPresenter toolSetListViewPresenter;
		private System.Windows.Forms.ToolStripMenuItem newProjectInXMLStoreToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newProjectInSqlStoreToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openXMLRepositoryToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openSQLServerRepositoryToolStripMenuItem;
		private System.Windows.Forms.TabPage layersTab;
		private Dataweb.NShape.WinFormsUI.LayerListView layerEditorListView;
		private Dataweb.NShape.Controllers.LayerController layerController;
		private System.Windows.Forms.TabPage hierarchyTab;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem adoNetDatabaseGeneratorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem closeProjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem recentProjectsMenuItem;
		private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showDiagramSettingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton refreshToolbarButton;
		private System.Windows.Forms.ToolStripButton showGridToolbarButton;
		private Dataweb.NShape.Controllers.ModelController modelTreeController;
		private Dataweb.NShape.Controllers.DiagramSetController diagramSetController;
		private Dataweb.NShape.Advanced.CachedRepository cachedRepository;
		private Dataweb.NShape.Controllers.LayerPresenter layerPresenter;
		private System.Windows.Forms.ToolStripMenuItem exportDialogToolStripMenuItem;
		private Dataweb.NShape.Controllers.PropertyController propertyController;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStrip debugToolStrip;
		private System.Windows.Forms.ToolStripButton debugDrawOccupationToolbarButton;
		private System.Windows.Forms.ToolStripButton prevDiagramButton;
		private System.Windows.Forms.ToolStripButton nextDiagramButton;
		private System.Windows.Forms.ToolStripButton saveProjectToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		private System.Windows.Forms.ToolStripDropDownButton newProjectToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem xmlRepositoryToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sQLRepositoryToolStripMenuItem;
		private System.Windows.Forms.ToolStripDropDownButton openProjectToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem openXMLRepositoryToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem openSQLRepositoryToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton displaySettingsToolStripButton;
		private System.Windows.Forms.ToolStripButton designEditorToolStripButton;
		private System.Windows.Forms.ToolStripButton debugDrawInvalidatedAreaToolbarButton;
		private System.Windows.Forms.ToolStripMenuItem nShapeEventMonitorToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelShapes;
		private System.Windows.Forms.ToolStripStatusLabel statusLabelShapeCount;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelResources;
		private System.Windows.Forms.ToolStripStatusLabel statusLabelResourceCount;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDisplayArea;
		private System.Windows.Forms.ToolStripStatusLabel statusLabelTopLeft;
		private System.Windows.Forms.ToolStripStatusLabel statusLabelBottomRight;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem10;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllOfTypeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllOfTemplateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem unselectAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyAsImageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem testDataGeneratorToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem11;
		private System.Windows.Forms.ContextMenuStrip toolboxContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem largeIconsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem smallIconsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem resetToolbarsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem12;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.ToolStripMenuItem upgradeVersionMenuItem;
		private System.Windows.Forms.ToolStripMenuItem useEmbeddedImagesToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem14;
		private System.Windows.Forms.ToolStripMenuItem viewHelpToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip diagramContextMenuStrip;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSelection;
		private System.Windows.Forms.ToolStripStatusLabel statusLabelMousePosition;
		private System.Windows.Forms.ToolStripStatusLabel statusLabelSelectionSize;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelProperties;
		private System.Windows.Forms.ToolStripStatusLabel statusLabelPropertyCount;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelActions;
		private System.Windows.Forms.ToolStripStatusLabel statusLabelActionCount;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelRelations;
		private System.Windows.Forms.ToolStripStatusLabel statusLabelRelationCount;
		private BrightIdeasSoftware.TreeListView hierarchyTreeView;
		private BrightIdeasSoftware.OLVColumn olvColumnType;
		private System.Windows.Forms.Timer timerClock;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem cLIPSTestToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem configSettingsRefreshToolStripMenuItem;
		private System.Windows.Forms.Timer toolTipTimer;
		private System.Windows.Forms.Timer timerEnergyFlow;
	}
}

