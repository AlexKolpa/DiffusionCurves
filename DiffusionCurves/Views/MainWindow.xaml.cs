using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.IO.Compression;
using System.Collections;
using DiffusionCurves.Diffusion;
using OpenTK;
using DiffusionCurves.Events;
using DiffusionCurves.Import;
using DiffusionCurves.Model;
using DiffusionCurves.Views;
using System.Diagnostics;
using DiffusionCurves.Interpolation;
using Emgu.CV;
using Emgu.CV.VideoSurveillance;
using Emgu.CV.Structure;
using Parago.Windows;
using DiffusionCurves.OpenGL;
using DiffusionCurves.Storage;
using DiffusionCurves.Export;

namespace DiffusionCurves
{
    /// <summary>
    /// MainWindow is the central class of this program, it:
    /// - Builds and updates the GUI.
    /// - Is the only class that can interface with the projectstate model class.
    /// - Calls dedicated classes for functionality.
    /// - Uses events to get information of dedicated classes.
    /// This class is the logic behind MainWindow.xaml. 
    /// This program uses a compromised MVC and MVVM approach.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private fields of the MainWindow class

        //Declare project state object
        private ProjectState projectstate;
        private EditorState editorState;
        private FramesContainer framesContainer;
        private RenderState renderState;

        //Declare dynamic GUI elements:
        private TimeLinePanel timeLinePanel;
        private GLControlHost glHost;
        private IDiffusionRenderer renderer;

        //Declare controls
        private EditorStateControl editorControl;
        private InterpolationControl interpolationControl;
        private DiffusionExport exporter;

        private bool interpolateMessageShown = false;

        #endregion

        /// <summary>
        /// Initializes a mainwindow GUI with logic.
        /// </summary>
        public MainWindow()
        {
            //Projectstate
            this.projectstate = new ProjectState();
            this.projectstate.PropertyChanged += projectstate_PropertyChanged;            

            //Framescontainer
            this.framesContainer = new FramesContainer();
            this.framesContainer.PropertyChanged += framesContainer_PropertyChanged;
            this.timeLinePanel = new SliderPanel(framesContainer);

            InitPathCreation();

            InitializeComponent();

            InitOpenGL();
            CreateExporter();
          
            Interpolator interpolator = new PyramidOpticalFlowInterpolator(renderState);
            interpolationControl = new InterpolationControl(interpolator, framesContainer);
            interpolationControl.NextFrameContainsCurves += interpolationControl_NextFrameContainsCurves;
            
            InactivateUIElementsOnEmptyProject();
        }
        
        #region Event Handlers for Menu Items

        /// <summary>
        /// Event for adding new frame.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewFrame_Click(object sender, RoutedEventArgs e)
        {
            DiffusionCurves.Model.Frame newframe = new DiffusionCurves.Model.Frame(null);
            this.framesContainer.AddFrame(newframe);
        }

        /// <summary>
        /// Event for deleting active frame.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteFrame_Click(object sender, RoutedEventArgs e)
        {
            if (this.framesContainer.Count <= 1)
            {
                Prompt("One frame left", "There must always be atleast one frame.");
                return;
            }

            string messageBoxText = "Do you want to remove this frame?";
            string caption = "Remove Frame";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            // Process message box results 
            switch (MessageBox.Show(messageBoxText, caption, button, icon))
            {
                case MessageBoxResult.Yes:
                    if (framesContainer.Count > 1)
                        this.framesContainer.RemoveCurrentFrame();
                    break;
                case MessageBoxResult.No:
                    return;
            }
        }

        /// <summary>
        /// Handles the click on NewProject menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            //Create NewProject object to handle new project creation
            DiffusionCurves.NewProject newProjectDialog = new DiffusionCurves.NewProject();
            
            //Inject event
            newProjectDialog.ProjectCreated += 
                new NewProject.NewProjectEventHandler(NewProject_CreateProject);

            if(this.projectstate.Saved == false)
                Close_Click(null, null);

            //Set dialog to the center of MainWindow
            newProjectDialog.Owner = this;

            newProjectDialog.ShowDialog();
        }

        /// <summary>
        /// Event that gets called to update GUI and projectstate on NewProject.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewProject_CreateProject(object sender, StateEventArgs e)
        {
            Close_Click(null, null);

            this.framesContainer.Clear();

            this.SequencePanel.Child = timeLinePanel;

            //Update projectstate
            this.projectstate.Active = e.ACTIVE;
            this.projectstate.Saved = e.SAVED;
            this.projectstate.FileName = e.FILENAME;
            this.projectstate.Destination = e.DESTINATION;
            this.projectstate.Width = e.WIDTH;
            this.projectstate.Height = e.HEIGHT;            

            this.projectstate.EditorState = ProjectState.ProjectEditorState.Create;
            
            //Set Viewport
            this.GLControl.Children.Add(glHost);
            System.Drawing.Size frameSize = new System.Drawing.Size(projectstate.Width, projectstate.Height);
            glHost.SetProjectFrameSize(frameSize);
            exporter.SetFrameSize(frameSize);

            framesContainer.AddFrame(new Model.Frame(null));
            this.projectstate.ActiveLayerIndex = 0;

            EnableMenuItems();

            EnableButtons();

            EnableLayers();
        }

        /// <summary>
        /// The handler when the user wants to interpolate manually.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InterpolateManual_Click(object sender, RoutedEventArgs e)
        {

            interpolateMessageShown = false;

            if (!projectstate.Active)
            {                
                return;
            }

            if (framesContainer.Count < 2)
            {
                MessageBox.Show("There should be at least 2 frames in the project to interpolate with!");                
                return;
            }

            if (framesContainer.CursorIndex == framesContainer.Count - 1)
            {
                MessageBox.Show("The last frame can't be interpolated!");                
                return;
            }

            interpolationControl.InterpolateManual(framesContainer.CursorIndex, double.MaxValue);
            framesContainer.CursorIndex++;
            this.projectstate.Saved = false;
        }

        /// <summary>
        /// The handler for when the user wants to interpolate automatically. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InterpolateAutomatic_Click(object sender, RoutedEventArgs e)
        {
            AutomaticInterpolation autoInterpolate = new AutomaticInterpolation();
            autoInterpolate.StartInterpolation += autoInterpolate_StartInterpolation;
            autoInterpolate.ShowDialog();
        }
        
        /// <summary>
        /// The eventhandler that gets injected in a call in InterpolateAutomatic_Click .
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void autoInterpolate_StartInterpolation(object sender, AutoInterpolationEventArgs e)
        {
            int interpolatedFrames = 0;
            double result = 0;

            interpolateMessageShown = false;

            ProgressDialogResult progressResult = ProgressDialog.Execute(this, "Interpolating", (bw, we) => {                                             
                while (framesContainer.CursorIndex + interpolatedFrames < framesContainer.Count - 1 && result < e.ErrorValue)
                {
                    result = interpolationControl.InterpolateManual(framesContainer.CursorIndex + interpolatedFrames, e.ErrorValue);

                    if (result < e.ErrorValue)
                    {
                        interpolatedFrames++;
                        ProgressDialog.ReportWithCancellationCheck(bw, we, interpolatedFrames + " frames interpolated.");
                    }                    
                }
                ProgressDialog.CheckForPendingCancellation(bw, we);

            },ProgressDialogSettings.WithSubLabelAndCancel);

            framesContainer.CursorIndex += interpolatedFrames;
            String response = "Interpolated a total of " + interpolatedFrames + " frames";

            if (progressResult.Cancelled)
                response += " before the process was cancelled.";
            else if (result >= e.ErrorValue)
                response += " before the pixel error became too high.";
            else
                response += ".";
            MessageBox.Show(response);
            this.projectstate.Saved = false;
        }

        /// <summary>
        /// Handles the click on Close menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (this.projectstate.Active == false)
                return;

            if (this.projectstate.Saved == true)
            {
                CloseProject();
            }
            else
            {
                DiffusionCurves.Views.CloseDialog closeDialog = new DiffusionCurves.Views.CloseDialog();

                //Inject event
                closeDialog.ProjectClosed +=
                    new CloseDialog.CloseProjectEventHandler(CloseProject_Close);

                closeDialog.Owner = this;

                closeDialog.ShowDialog();
            }
        }

        /// <summary>
        /// Eventhandler for close project that gets injected to handle the users input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseProject_Close(object sender, StateEventArgs e)
        {
            if (e.Wanto == StateEventArgs.WantTo.Save)
                SaveProject.Save(this.projectstate, this.framesContainer);
            else if (e.Wanto == StateEventArgs.WantTo.Dispose)
                return;

            CloseProject();
        }

        /// <summary>
        /// Handles the click on ImportImages menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportImages_Click(object sender, RoutedEventArgs e)
        {
            SetFileBrowser();

            EnableImagesOnImport();
        }

        /// <summary>
        /// Handles the click on SaveProjectItem.
        /// Handles the click on Save menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveProjectItem_Click(object sender, RoutedEventArgs e)
        {
            //saved
            if (this.projectstate.Saved == true)
            {
                Prompt("Already saved", "The project has already been saved, nothing has changed since.");
                return;
            }
            //Empty project
            else if (this.projectstate.Destination.Equals(string.Empty))
            {
                //Create NewProject object to handle new project creation
                DiffusionCurves.Views.SaveProjectGUI saveprojectDialog = new DiffusionCurves.Views.SaveProjectGUI(this.projectstate, this.framesContainer);

                //Inject event
                saveprojectDialog.ProjectSaved +=
                    new SaveProjectGUI.SaveProjectEventHandler(SaveProject_ProjectSaved);

                //Set dialog to the center of MainWindow
                saveprojectDialog.Owner = this;

                saveprojectDialog.ShowDialog();
            }
            //not saved
            else
            {
                try
                {
                    this.projectstate.Saved = true;
                    DiffusionCurves.Storage.SaveProject.Save(this.projectstate, this.framesContainer);
                }
                catch (Exception ex)
                {
                    Prompt("Application error", ex.Message);
                }
            }
        }

        /// <summary>
        /// Event that gets called to update GUI and projectstate on SaveProject.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveProject_ProjectSaved(object sender, StateEventArgs e)
        {
            this.projectstate.FileName = e.FILENAME;
            this.projectstate.Destination = e.DESTINATION;

            Storage.SaveProject.Save(this.projectstate, this.framesContainer);

            this.projectstate.Saved = true;
        }

        /// <summary>
        /// The eventhandler that gets injected in open for handling user input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            //Inject event
            Storage.LoadProject.LoadSaveEvent +=
                new Storage.LoadProject.OpenProjectEventHandler(OpenProject_Load);

            if (this.projectstate.Saved == false)
                Close_Click(null, null);

            Storage.LoadProject.Load();
        }

        /// <summary>
        /// Event that gets called to update GUI and projectstate on NewProject.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenProject_Load(object sender, StateEventArgs e)
        {
            Close_Click(null, null);

            this.projectstate.Active = true;
            this.projectstate.Saved = true;

            //Update projectstate
            framesContainer.Copy(e.Framescontainer);
            projectstate.Copy(e.Projectstate);

            //Set Viewport
            this.SequencePanel.Child = timeLinePanel;
            this.GLControl.Children.Add(glHost);
            glHost.SetProjectFrameSize(new System.Drawing.Size(projectstate.Width, projectstate.Height));
            
            EnableMenuItems();

            EnableButtons();

            EnableLayers();

            EnableImagesOnImport();
        }

        /// <summary>
        /// Show a dialog with information about the software.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void About_Click(object sender, RoutedEventArgs e)
        {
            new DiffusionCurves.Views.About().ShowDialog();
        }

        /// <summary>
        /// Show Opendialog to select where images in 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
            saveDialog.Filter = DiffusionExport.GetImageTypeFilter();
            saveDialog.ValidateNames = true;
            saveDialog.DefaultExt = "jpg";
            saveDialog.FilterIndex = 2;

            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                exporter.Export(saveDialog.FileName, saveDialog.FilterIndex - 1);
            }
        }

        /// <summary>
        /// Handles the click on Quit menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            if (this.projectstate.Saved == false && this.projectstate.Active == true)
            {
                //Ask to save
                ExitDialog exit = new ExitDialog();

                //Inject event
                exit.ProjectQuitted +=
                    new ExitDialog.QuitProjectEventHandler(QuitProject_Quit);

                //Set dialog to the center of MainWindow
                exit.Owner = this;

                exit.ShowDialog();
            }
            else
            {
                //Exit
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Handler for Quit Menu Item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitProject_Quit(object sender, StateEventArgs e)
        {
            if (e.Wanto == StateEventArgs.WantTo.Save)
                SaveProject.Save(this.projectstate, this.framesContainer);
            else if (e.Wanto == StateEventArgs.WantTo.Dispose)
                return;

            Environment.Exit(0);
        }

        /// <summary>
        /// Handles the click on Prev menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.framesContainer.CursorIndex--;
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Handles the click on Next menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.framesContainer.CursorIndex++;
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Event handler for when a new point has been created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void glHost_PointClicked(object sender, PointClickedEventArgs e)
        {
            if (projectstate.EditorState == ProjectState.ProjectEditorState.Color)
            {
                Vector4 vectorColor = Vector4.One;
                if (e.ButtonsPressed == System.Windows.Forms.MouseButtons.Left)
                    vectorColor = e.ClickedPoint.LeftColor;
                else if (e.ButtonsPressed == System.Windows.Forms.MouseButtons.Right)
                    vectorColor = e.ClickedPoint.RightColor;
                else
                    return;

                Microsoft.Samples.CustomControls.ColorPickerDialog cPicker
                        = new Microsoft.Samples.CustomControls.ColorPickerDialog();

                Color color = new Color();
                color.R = (byte)(vectorColor.X * 255);
                color.G = (byte)(vectorColor.Y * 255);
                color.B = (byte)(vectorColor.Z * 255);
                color.A = (byte)(vectorColor.W * 255);

                cPicker.Owner = this;
                cPicker.StartingColor = color;

                bool? dialogResult = cPicker.ShowDialog();
                if (dialogResult != null && (bool)dialogResult == true)
                {
                    Vector4 selectedVectorColor = new Vector4();
                    Color selectedColor = cPicker.SelectedColor;
                    selectedVectorColor.X = selectedColor.R / 255f;
                    selectedVectorColor.Y = selectedColor.G / 255f;
                    selectedVectorColor.Z = selectedColor.B / 255f;
                    selectedVectorColor.W = selectedColor.A / 255f;

                    if (e.ButtonsPressed == System.Windows.Forms.MouseButtons.Left)
                        e.ClickedPoint.LeftColor = selectedVectorColor;
                    else if (e.ButtonsPressed == System.Windows.Forms.MouseButtons.Right)
                        e.ClickedPoint.RightColor = selectedVectorColor;

                    this.projectstate.Saved = false;
                }

                editorControl.SetIdle();
            }
        }

        private void DrawCurves_Click(object sender, RoutedEventArgs e)
        {
            if (renderer != null && CheckCurves.IsChecked.HasValue)
            {                
                if (!CheckCurves.IsChecked.Value)
                    renderer.DisplayDiffusion = true;
                else
                    renderer.DisplayDiffusion = false;

                CheckCurves.IsChecked = !CheckCurves.IsChecked.Value;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Capture manipulation on glhost to set to unsaved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GLControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.projectstate.Saved = false;
        }

        #endregion

        #region Toolbutton Listeners

        /// <summary>
        /// Handles the click on Create tool button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            this.projectstate.EditorState = ProjectState.ProjectEditorState.Create;
        }

        /// <summary>
        /// Handles the click on Color tool button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Color_Click(object sender, RoutedEventArgs e)
        {
            this.projectstate.EditorState = ProjectState.ProjectEditorState.Color;
        }

        /// <summary>
        /// Handles an action on the timeline slider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiffusionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (renderer != null)
            {
                renderer.Iterations = (int)e.NewValue;
                this.IterationsLabel.Content = (int)e.NewValue;
            }
        }

        #endregion

        #region shortkey toggles

        /// <summary>
        /// Handles the checkbox for curves viewing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayDiffusion_Toggled(object sender, ExecutedRoutedEventArgs e)
        {
            if (CheckCurves.IsChecked.Value)
                CheckCurves_Unchecked(null, null);
            else
                CheckCurves_Checked(null, null);                
        }

        private void SmoothCurves_Toggled(object sender, ExecutedRoutedEventArgs e)
        {
            if (SmoothCurves.IsChecked)
                SmoothCurves_Unchecked(null, null);
            else
                SmoothCurves_Checked(null, null);                
        }

        private void SmoothCurves_Checked(object sender, RoutedEventArgs e)
        {
            if (editorState != null)
                editorState.AllowSmoothUpdate = true;

            SmoothCurves.IsChecked = true;
        }

        private void SmoothCurves_Unchecked(object sender, RoutedEventArgs e)
        {
            if (editorState != null)
                editorState.AllowSmoothUpdate = false;

            SmoothCurves.IsChecked = false;
        }

        private void CheckCurves_Checked(object sender, RoutedEventArgs e)
        {
            if (renderer != null)
                renderer.DisplayDiffusion = true;

            CheckCurves.IsChecked = true;
        }

        private void CheckCurves_Unchecked(object sender, RoutedEventArgs e)
        {
            if (renderer != null)
                renderer.DisplayDiffusion = false;

            CheckCurves.IsChecked = false;
        }

        #endregion

        #region Layers Listeners

        private void One_Click(object sender, RoutedEventArgs e)
        {
            this.projectstate.ActiveLayerIndex = 0;
        }

        private void Two_Click(object sender, RoutedEventArgs e)
        {
            this.projectstate.ActiveLayerIndex = 1;
        }

        private void Three_Click(object sender, RoutedEventArgs e)
        {
            this.projectstate.ActiveLayerIndex = 2;
        }

        #endregion

        #region Auxillary mehods

        /// <summary>
        /// Method to Prompt user with information.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public void Prompt(string title, string message)
        {
            System.Windows.MessageBox.Show(message,
                title, System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }

        /// <summary>
        /// Initializes the path creation
        /// </summary>
        private void InitPathCreation()
        {
            renderState = new RenderState();
            editorState = new EditorState();
            editorState.PropertyChanged += editorState_PropertyChanged;
            editorControl = new EditorStateControl(editorState, projectstate, renderState);
        }

        /// <summary>
        /// Initializes the GL host.
        /// </summary>
        private void InitOpenGL()
        {            
            RenderStateControl renderControl = new RenderStateControl(renderState);            

            DiffusionBuffers buffers = new DiffusionBuffers();
            DiffusionPathControl bufferControl = new DiffusionPathControl(buffers);

            //shaders
            DefaultShader pointShader = new DefaultShader("shaders/basic.vert", "shaders/basic.frag");
            TexturedShader imageShader = new TexturedShader("shaders/tex.vert", "shaders/tex.frag");
            TexturedShader diffuseShader = new TexturedShader("shaders/tex.vert", "shaders/diffuse.frag");
            NormalShader lineShader = new NormalShader("shaders/normal.vert", "shaders/normal.geom", "shaders/normal.frag");
            NormalShader endPointShader = new NormalShader("shaders/normal.vert", "shaders/endpoints.geom", "shaders/normal.frag");
            
            renderer = new DiffusionRenderer(renderState, 
                editorState, 
                buffers,                  
                pointShader, 
                imageShader,
                diffuseShader,
                lineShader, 
                endPointShader);            
            
            //Init GL            
            GLControl control = new GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 8, 4)); 
            this.glHost = new GLControlHost(control, 
                editorState,                  
                editorControl, 
                renderer, 
                renderControl, 
                bufferControl);

            this.glHost.Child = control;
            this.glHost.PointClicked += glHost_PointClicked;
        }

        void CreateExporter()
        {
            //Construct export objects
            RenderState exportRenderState = new RenderState();
            RenderStateControl exportRenderControl = new RenderStateControl(exportRenderState);
            DiffusionBuffers exportBuffers = new DiffusionBuffers();
            DiffusionPathControl exportPathControl = new DiffusionPathControl(exportBuffers);

            exporter = new DiffusionExport(renderer, exportRenderState, exportRenderControl, exportPathControl, exportBuffers, framesContainer);
        }

        void editorState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.projectstate.Saved = false;
        }

        void interpolationControl_NextFrameContainsCurves(object sender, InterpolationArgs e)
        {
            if (!interpolateMessageShown)
            {
                MessageBoxResult result = MessageBox.Show("The next image already contains drawn paths. Do you want to continue?", "Warning", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    interpolateMessageShown = true;
                    e.Handled = true;                    
                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Inactivates UI items on empty project.
        /// </summary>
        private void InactivateUIElementsOnEmptyProject()
        {
            //Menuitem
            this.Interpolate.IsEnabled = false;
            this.SaveProjectItem.IsEnabled = false;
            this.ExportItem.IsEnabled = false;
            this.ImportImagesItem.IsEnabled = false;
            this.CloseItem.IsEnabled = false;
            this.FrameItem.IsEnabled = false;
            this.ViewItem.IsEnabled = false;

            //Toolbar
            this.Buttons.IsEnabled = false;
            this.projectstate.EditorState = ProjectState.ProjectEditorState.Neutral;
            this.ViewPort.BorderThickness = new Thickness(3);
            this.ViewPort.BorderBrush = (Brush)new System.Windows.Media.BrushConverter()
                .ConvertFromString("#FF494949");

            //Layers
            this.LabelPanel.IsEnabled = false;

            //Next Prev
            this.Prev.IsEnabled = false;
            this.Next.IsEnabled = false;
            this.DiffusionSlider.IsEnabled = false;
            this.CheckCurves.IsEnabled = false;
            this.CheckCurvesLabel.Foreground = Brushes.Gray;
            this.IterationsLabel.Foreground = Brushes.Gray;
            //Timeline
            this.SequencePanel.Child = null;
        }

        /// <summary>
        /// Enable menu items on new project.
        /// </summary>
        private void EnableMenuItems()
        {
            this.Interpolate.IsEnabled = true;
            this.SaveProjectItem.IsEnabled = true;
            this.ExportItem.IsEnabled = true;
            this.ImportImagesItem.IsEnabled = true;
            this.CloseItem.IsEnabled = true;
            this.FrameItem.IsEnabled = true;
            this.ViewItem.IsEnabled = true;
        }

        /// <summary>
        /// Enable buttons on new project.
        /// </summary>
        private void EnableButtons()
        {
            this.Buttons.IsEnabled = true;
            this.NextPrevPanel.IsEnabled = true;
            this.DiffusionSlider.IsEnabled = true;
            this.CheckCurves.IsEnabled = true;
            this.CheckCurvesLabel.Foreground = Brushes.White;
            this.IterationsLabel.Foreground = Brushes.White;
        }

        /// <summary>
        /// Enable labels on new project.
        /// </summary>
        private void EnableLayers()
        {
            this.LabelPanel.IsEnabled = true;
        }

        /// <summary>
        /// Enable Images on import of images.
        /// </summary>
        private void EnableImagesOnImport()
        {
            if (framesContainer.Count > 0)
            {
                this.Next.IsEnabled = true;
                framesContainer.CursorIndex = framesContainer.CursorIndex;
                this.projectstate.Saved = false;
            }
        }

        /// <summary>
        /// Do the necessary steps to close a project but not the application.
        /// </summary>
        private void CloseProject()
        {
            InactivateUIElementsOnEmptyProject();

            this.GLControl.Children.Remove(glHost);

            //Update projectstate
            this.projectstate.Active = false;
            this.projectstate.Saved = false;
            this.projectstate.FileName = "<empty>";
            this.projectstate.Destination = string.Empty;
            this.projectstate.Width = 0;
            this.projectstate.Height = 0;

            this.framesContainer.Clear();
        }

        /// <summary>
        /// Set the filebrowser with options on image import.
        /// </summary>
        private void SetFileBrowser()
        {
            //Filebrowser
            Microsoft.Win32.OpenFileDialog openImagesDialog = new Microsoft.Win32.OpenFileDialog();
            
            // Set options for filebrowser
            openImagesDialog.Filter = "Image Files|*.jpg;*.jpeg";
            openImagesDialog.InitialDirectory = "";
            openImagesDialog.FilterIndex = 1;
            openImagesDialog.Title = "Select image(s) to import";
            openImagesDialog.Multiselect = true;

            bool result = (bool)openImagesDialog.ShowDialog();
            if(result == true)
            {
                if (framesContainer.Count == 1 && 
                framesContainer.FramesList[0].Curves.TotalPointCount == 0 && 
                framesContainer.FramesList[0].ImageUrl == null)
                framesContainer.Clear();
            }

            framesContainer.AddRange(DiffusionCurves.Import.ImportImages.Import(result,
                openImagesDialog.FileNames, this.framesContainer.Count));
        }

        #endregion

        #region Updates UI on changes in the model
        
        /// <summary>
        /// Event injection for projecstate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectstate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            updateUI(e.PropertyName);
        }
        
        void framesContainer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CursorIndex":
                    CursorIndexChanged();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// This function gets called when projectstate needs a GUI update.
        /// </summary>
        /// <param name="caller"></param>
        public void updateUI(string caller)
        {
            switch (caller)
            {
                case "Saved":
                    ProjectSaved();                    
                    return;
                case "EditorState":
                    EditorStateChanged();
                    return;
                case "ActiveLayerIndex":
                    Layerindex();
                    return;
            }
        }

        /// <summary>
        /// Gets called by projecstate on UI change: saved.
        /// </summary>
        private void ProjectSaved()
        {
            if (this.projectstate.Active == false)
                this.Title = "<empty>";
            else if (projectstate.Saved == false)
            {
                if (!this.Title[0].Equals("*"))
                    this.Title = "*" + this.projectstate.FileName;
                else
                    this.Title = this.projectstate.FileName;
            }
            else
                this.Title = this.projectstate.FileName;
        }

        /// <summary>
        /// Gets called by projecstate on UI change: cursorindex.
        /// </summary>
        private void CursorIndexChanged()
        {
            if (framesContainer.Count == 0)
                return;

            //Set frame in container
            DiffusionCurves.Model.Frame frame = framesContainer.FramesList[this.framesContainer.CursorIndex];
            System.Drawing.Bitmap bm = null;
            
            if (frame.ImageUrl != null)
                bm = new System.Drawing.Bitmap(frame.ImageUrl);
            
            glHost.SetNewFrame(bm, frame.Curves);
            
            if(bm != null)
                bm.Dispose();

            //Next prev inactive
            if (this.framesContainer.Count == 0)
            {
                this.Prev.IsEnabled = false;
                this.Next.IsEnabled = false;
            }
            //Prev
            if (this.framesContainer.CursorIndex <= 0)
                this.Prev.IsEnabled = false;
            else
                this.Prev.IsEnabled = true;
            //Next
            if (this.framesContainer.CursorIndex == framesContainer.Count - 1)
                this.Next.IsEnabled = false;
            else
                this.Next.IsEnabled = true;
        }

        /// <summary>
        /// Gets called by projecstate on UI change: edit.
        /// </summary>
        private void EditorStateChanged()
        {
            if (this.projectstate.Active == false)
                return;

            // Create a diagonal linear gradient with four stops.   
            LinearGradientBrush myLinearGradientBrush =
                new LinearGradientBrush();
            myLinearGradientBrush.StartPoint = new Point(0.5, 0);
            myLinearGradientBrush.EndPoint = new Point(0.5, 1);
            
            this.Color.Background = Brushes.Transparent;
            this.Create.Background = Brushes.Transparent;

            String maincolor = null, bottomcolor = null;

            switch(this.projectstate.EditorState) 
            {
                case ProjectState.ProjectEditorState.Neutral:
                    this.Create.Background = myLinearGradientBrush;
                    this.Color.Background = myLinearGradientBrush;
                    maincolor = "#00FFFFFF";
                    bottomcolor = "#00FFFFFF";
                    break;
                case ProjectState.ProjectEditorState.Create:
                    this.Create.Background = myLinearGradientBrush;
                    maincolor = "#1078B5";
                    bottomcolor = "#2980b9";
                    break;
                case ProjectState.ProjectEditorState.Color:
                    this.Color.Background = myLinearGradientBrush;
                    maincolor = "#e74c3c";
                    bottomcolor = "#E53424";
                    break;
            }

            //Set background
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop((Color)ColorConverter.ConvertFromString(maincolor), 0));
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop((Color)ColorConverter.ConvertFromString(bottomcolor), 1));

            //Set border viewport to color
            this.ViewPort.BorderThickness = new Thickness(3);
            this.ViewPort.BorderBrush = (Brush)new System.Windows.Media.BrushConverter()
                .ConvertFromString(bottomcolor);
        }

        /// <summary>
        /// Gets called when a new layer is selected.
        /// </summary>
        private void Layerindex()
        {
            if (this.projectstate.Active == false)
                return;

            if (framesContainer.Count > 0)
            {
                framesContainer.FramesList[framesContainer.CursorIndex].Curves.ActivePathsLayer =
                    this.projectstate.ActiveLayerIndex;

                glHost.RequestPaint();
            }

            string background = "#9b59b6";

            this.One.Background = Brushes.Transparent;
            this.Two.Background = Brushes.Transparent;
            this.Three.Background = Brushes.Transparent;

            switch (this.projectstate.ActiveLayerIndex)
            {
                case 0:
                    this.One.Background = (Brush)new System.Windows.Media.BrushConverter()
                        .ConvertFromString(background);
                    break;
                case 1:
                    this.Two.Background = (Brush)new System.Windows.Media.BrushConverter()
                        .ConvertFromString(background);
                    break;
                case 2:
                    this.Three.Background = (Brush)new System.Windows.Media.BrushConverter()
                        .ConvertFromString(background);
                    break;
                default:
                    break;
            }
        }

        #endregion        
    }
}
