// Type: Launcher.Settings
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6AA362FF-B506-4D02-B128-5EAE460AD817
// Assembly location: C:\Program Files (x86)\Ubisoft\Rabbids Go Home - DVD\Launcher.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

#nullable disable
namespace Launcher
{
  public class Settings : Form
  {
    private IContainer components;
    private Button cancelBtn; // Button for cancel action
    private Button okBtn; // Button for ok action
    private ComboBox langCombo; // ComboBox to select language
    private ComboBox rezCombo; // ComboBox to select screen resolution
    private CheckBox vSyncCheck; // Checkbox to enable/disable VSync
    private CheckBox windowedCheck; // Checkbox to enable/disable windowed mode
    private PictureBox rabbidPicture; // PictureBox to display image
    private Button joyCalibrate; // Button to calibrate joystick??????

    public Settings()
    {
      this.InitializeComponent();
      this.SuspendLayout();

      // Set the embedded image of the rabbid character
      this.rabbidPicture.Image = this.GetEmbededRabbid();

      // Populate language combo box with enum values from Configuration.Lang
      this.langCombo.Items.Clear();
      foreach (object name in Enum.GetNames(typeof (Configuration.Lang)))
        this.langCombo.Items.Add(name);

      // Populate resolution combo box with supported display modes
      this.rezCombo.Items.Clear();
      for (int index = 0; index < DisplayInformation.DesiredModes.Length; ++index)
      {
        string desiredMode = DisplayInformation.DesiredModes[index];
        if (Configuration.Global.DisplayInfo.IsModeSupported(desiredMode))
          this.rezCombo.Items.Add((object) desiredMode);
      }

      // Hide the joystick calibration?? when set to true it shows the button but crashes when opened
      this.joyCalibrate.Visible = false;
      this.ResumeLayout();
    }

    protected override void OnShown(EventArgs e)
    {
      // Disable the main form when settings form is shown
      if (this.Owner is MainForm owner)
        owner.Enabled = false;

      // Set initial values for controls from global settings
      this.langCombo.SelectedIndex = (int) Configuration.Global.Settings.Language;
      this.rezCombo.SelectedItem = (object) Configuration.Global.Settings.VideoMode;
      this.vSyncCheck.Checked = Configuration.Global.Settings.VSync;
      this.windowedCheck.Checked = Configuration.Global.Settings.Windowed;

      base.OnShown(e);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      // Enable the main form and reset settings form reference when settings form is closing
      if (this.Owner is MainForm owner)
      {
        owner.Enabled = true;
        owner.SettingsForm = (Form) null;
        owner.Activate();
      }
      base.OnClosing(e);
    }

    private void okBtn_Click(object sender, EventArgs e)
    {
      // Save settings when OK is clicked
      Configuration.Global.Settings.VideoMode = (string) this.rezCombo.SelectedItem;
      Configuration.Global.Settings.VSync = this.vSyncCheck.Checked;
      Configuration.Global.Settings.Windowed = this.windowedCheck.Checked;

      // If the language has changed, update and reset the app culture
      if (Configuration.Global.Settings.Language != (Configuration.Lang) this.langCombo.SelectedIndex)
      {
        Configuration.Global.Settings.Language = (Configuration.Lang) this.langCombo.SelectedIndex;
        LocalizationData.ResetAppCulture = true;

        // Close the main form to apply language changes
        if (this.Owner is MainForm owner)
          owner.Close();
      }
      this.Close(); // Close the settings form
    }

    private void cancelBtn_Click(object sender, EventArgs e) => this.Close(); // Close the settings form when cancel is clicked

    private Image GetEmbededRabbid()
    {
      // Get embedded image of a rabbid from the resources
      return Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Launcher.Images.rabbid.png"));
    }

    private void joyCalibrate_Click(object sender, EventArgs e) => new JoystickCalibration().Show(); // This part is unused for joystick calibration

    protected override void Dispose(bool disposing)
    {
      // Dispose of components if disposing is true
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      // Initialize components and layout for the settings form
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Settings));
      this.windowedCheck = new CheckBox();
      this.vSyncCheck = new CheckBox();
      this.rezCombo = new ComboBox();
      this.langCombo = new ComboBox();
      this.cancelBtn = new Button();
      this.okBtn = new Button();
      this.rabbidPicture = new PictureBox();
      this.joyCalibrate = new Button();
      GroupBox groupBox1 = new GroupBox();
      Label label = new Label();
      GroupBox groupBox2 = new GroupBox();

      groupBox1.SuspendLayout();
      groupBox2.SuspendLayout();
      ((ISupportInitialize) this.rabbidPicture).BeginInit();
      this.SuspendLayout();

      // Configuration of the display group containing resolution, VSync, and windowed mode controls
      componentResourceManager.ApplyResources((object) groupBox1, "displayGroup");
      groupBox1.Controls.Add((Control) this.windowedCheck);
      groupBox1.Controls.Add((Control) this.vSyncCheck);
      groupBox1.Controls.Add((Control) label);
      groupBox1.Controls.Add((Control) this.rezCombo);
      groupBox1.Name = "displayGroup";
      groupBox1.TabStop = false;

      // Configuration of windowed mode checkbox
      componentResourceManager.ApplyResources((object) this.windowedCheck, "windowedCheck");
      this.windowedCheck.Name = "windowedCheck";
      this.windowedCheck.UseVisualStyleBackColor = true;

      // Configuration of VSync checkbox
      componentResourceManager.ApplyResources((object) this.vSyncCheck, "vSyncCheck");
      this.vSyncCheck.Name = "vSyncCheck";
      this.vSyncCheck.UseVisualStyleBackColor = true;

      // Configuration of resolution combo box and label
      componentResourceManager.ApplyResources((object) label, "rezLabel");
      componentResourceManager.ApplyResources((object) this.rezCombo, "rezCombo");
      this.rezCombo.DropDownStyle = ComboBoxStyle.DropDownList;

      // Configuration of the language group containing the language combo box
      componentResourceManager.ApplyResources((object) groupBox2, "langGroup");
      groupBox2.Controls.Add((Control) this.langCombo);
      groupBox2.Name = "langGroup";
      groupBox2.TabStop = false;

      // Configuration of language combo box
      componentResourceManager.ApplyResources((object) this.langCombo, "langCombo");
      this.langCombo.DropDownStyle = ComboBoxStyle.DropDownList;

      // Configuration of the cancel button
      componentResourceManager.ApplyResources((object) this.cancelBtn, "cancelBtn");
      this.cancelBtn.DialogResult = DialogResult.Cancel;
      this.cancelBtn.Name = "cancelBtn";
      this.cancelBtn.Click += new EventHandler(this.cancelBtn_Click);

      // Configuration of the OK button
      componentResourceManager.ApplyResources((object) this.okBtn, "okBtn");
      this.okBtn.Name = "okBtn";
      this.okBtn.Click += new EventHandler(this.okBtn_Click);

      // Configuration of the rabbid picture box
      componentResourceManager.ApplyResources((object) this.rabbidPicture, "rabbidPicture");
      this.rabbidPicture.Name = "rabbidPicture";

      // Configuration of the joystick calibration button?
      componentResourceManager.ApplyResources((object) this.joyCalibrate, "joyCalibrate");
      this.joyCalibrate.Name = "joyCalibrate";
      this.joyCalibrate.Click += new EventHandler(this.joyCalibrate_Click);

      // Final form setup
      this.AcceptButton = (IButtonControl) this.okBtn;
      componentResourceManager.ApplyResources((object) this, "$this");
      this.CancelButton = (IButtonControl) this.cancelBtn;
      this.Controls.Add((Control) this.joyCalibrate);
      this.Controls.Add((Control) this.rabbidPicture);
      this.Controls.Add((Control) this.okBtn);
      this.Controls.Add((Control) this.cancelBtn);
      this.Controls.Add((Control) groupBox2);
      this.Controls.Add((Control) groupBox1);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.ShowInTaskbar = false;

      groupBox1.ResumeLayout(false);
      groupBox1.PerformLayout();
      groupBox2.ResumeLayout(false);
      ((ISupportInitialize) this.rabbidPicture).EndInit();
      this.ResumeLayout(false);
    }
  }
}
