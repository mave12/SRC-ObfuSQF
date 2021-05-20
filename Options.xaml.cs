// Decompiled with JetBrains decompiler
// Type: Maverick_ObfuSQF_Windows_Interface.Options
// Assembly: ObfuSQF, Version=2.9.3.1, Culture=neutral, PublicKeyToken=null
// MVID: 24511B23-02FB-4CC6-8157-3EC744E67057
// Assembly location: C:\Users\matze\Desktop\A3Packer-master\ObfuSQF.exe

using Maverick_ObfuSQF_Windows_Interface.Config;
using Maverick_ObfuSQF_Windows_Interface.OptionsModels;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Markup;

namespace Maverick_ObfuSQF_Windows_Interface
{
  public partial class Options : Window, IComponentConnector
  {
    public const string OPTIONS_ONLYINPBO = "Only within current PBO";
    public const string OPTIONS_ACROSSPBO = "Used across PBOs";
    internal System.Windows.Controls.CheckBox cbKeyLocker;
    internal Hyperlink hlKeyLockerHelp;
    internal System.Windows.Controls.CheckBox cbImageObfuscation;
    internal Hyperlink hlImageObfuscationHelp;
    internal System.Windows.Controls.TextBox tbScriptsPath;
    internal System.Windows.Controls.Button btnSelectScriptsPath;
    internal System.Windows.Controls.GroupBox gbScriptsOnly;
    internal System.Windows.Controls.CheckBox cbPrivateObfuscation;
    internal Hyperlink hlPrivateObfuscationHelp;
    internal System.Windows.Controls.TextBox tbOutputPath;
    internal System.Windows.Controls.Button btnSelectOutputPath;
    internal Hyperlink hlChannel;
    internal System.Windows.Controls.TextBox tbChannel;
    internal System.Windows.Controls.Button btnExitNoSave;
    internal System.Windows.Controls.Button btnExitSave;
    private bool _contentLoaded;

    public ObservableCollection<VariableModel> AllVariables { get; set; } = new ObservableCollection<VariableModel>();

    public IniFile Configuration { get; set; } = new IniFile(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "config.ini")));

    public Options()
    {
      this.InitializeComponent();
      this.Closing += new CancelEventHandler(this.Options_Closing);
      this.LoadConfiguration();
    }

    private void Options_Closing(object sender, CancelEventArgs e) => new MainWindow().Show();

    private void LoadConfiguration()
    {
      this.tbScriptsPath.Text = this.Configuration.IniReadValue("Settings", "ScriptsPath");
      this.tbOutputPath.Text = this.Configuration.IniReadValue("Settings", "OutputMasterDirectory");
      this.cbImageObfuscation.IsChecked = new bool?(this.Configuration.IniReadValue("Settings", "ImageObfuscationEnabled") == "True");
      this.cbPrivateObfuscation.IsChecked = new bool?(this.Configuration.IniReadValue("Settings", "PrivateObfuscationEnabled") == "True");
      this.tbChannel.Text = this.Configuration.IniReadValue("Settings", "Channel");
      this.cbKeyLocker.IsChecked = new bool?(this.Configuration.IniReadValue("Settings", "KeyLocker") == "True");
      string str = "";
      if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "global_variables.json")))
        str = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "global_variables.json"));
      if (string.IsNullOrEmpty(str))
        str = "[]";
      try
      {
        this.AllVariables = (ObservableCollection<VariableModel>) JsonConvert.DeserializeObject<ObservableCollection<VariableModel>>(str);
        if (this.AllVariables == null)
          this.AllVariables = new ObservableCollection<VariableModel>();
        this.LoadScriptsOnlyBox();
      }
      catch
      {
      }
    }

    private void LoadScriptsOnlyBox() => this.gbScriptsOnly.IsEnabled = true;

    private void hlImageObfuscationHelp_Click(object sender, RoutedEventArgs e) => Process.Start("https://obfusqf.com/questions");

    private void tbScriptsPath_TextChanged(object sender, TextChangedEventArgs e) => this.LoadScriptsOnlyBox();

    private void btnSelectScriptsPath_Click(object sender, RoutedEventArgs e)
    {
      using (OpenFileDialog openFileDialog = new OpenFileDialog())
      {
        if (openFileDialog.ShowDialog() != DialogResult.OK)
          return;
        this.tbScriptsPath.Text = openFileDialog.FileName;
        this.LoadScriptsOnlyBox();
      }
    }

    private void hlPrivateObfuscationHelp_Click(object sender, RoutedEventArgs e) => Process.Start("https://obfusqf.com/questions");

    private void hlGlobalObfuscationHelp_Click(object sender, RoutedEventArgs e) => Process.Start("https://obfusqf.com/questions");

    private void btnAddVariable_Click(object sender, RoutedEventArgs e) => this.AllVariables.Add(new VariableModel());

    private void btnRemoveSelectedVariable_Click(object sender, RoutedEventArgs e)
    {
    }

    private void btnExitNoSave_Click(object sender, RoutedEventArgs e) => this.Close();

    private void btnExitSave_Click(object sender, RoutedEventArgs e)
    {
      if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "global_variables.json")))
        File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "global_variables.json"));
      this.Configuration.IniWriteValue("Settings", "ScriptsPath", this.tbScriptsPath.Text);
      IniFile configuration1 = this.Configuration;
      bool? isChecked = this.cbImageObfuscation.IsChecked;
      string str1 = isChecked.ToString();
      configuration1.IniWriteValue("Settings", "ImageObfuscationEnabled", str1);
      IniFile configuration2 = this.Configuration;
      isChecked = this.cbPrivateObfuscation.IsChecked;
      string str2 = isChecked.ToString();
      configuration2.IniWriteValue("Settings", "PrivateObfuscationEnabled", str2);
      this.Configuration.IniWriteValue("Settings", "Channel", this.tbChannel.Text.ToLower().Length == 0 ? "stable" : this.tbChannel.Text.ToLower());
      File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "global_variables.json"), JsonConvert.SerializeObject((object) this.AllVariables, (Formatting) 1));
      IniFile configuration3 = this.Configuration;
      isChecked = this.cbKeyLocker.IsChecked;
      string str3 = isChecked.ToString();
      configuration3.IniWriteValue("Settings", "KeyLocker", str3);
      this.Configuration.IniWriteValue("Settings", "OutputMasterDirectory", this.tbOutputPath.Text);
      this.Close();
    }

    private void hlKeyLockerHelp_Click(object sender, RoutedEventArgs e) => Process.Start("https://obfusqf.com/questions");

    private void tbOutputPath_TextChanged(object sender, TextChangedEventArgs e) => this.Configuration.IniWriteValue("Settings", "OutputMasterDirectory", this.tbOutputPath.Text);

    private void btnSelectOutputPath_Click(object sender, RoutedEventArgs e)
    {
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
      {
        if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
          return;
        this.tbOutputPath.Text = folderBrowserDialog.SelectedPath;
      }
    }

    private void hlChannel_Click(object sender, RoutedEventArgs e) => Process.Start("https://obfusqf.com/questions");

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/ObfuSQF;component/options.xaml", UriKind.Relative));
    }

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.cbKeyLocker = (System.Windows.Controls.CheckBox) target;
          break;
        case 2:
          this.hlKeyLockerHelp = (Hyperlink) target;
          this.hlKeyLockerHelp.Click += new RoutedEventHandler(this.hlKeyLockerHelp_Click);
          break;
        case 3:
          this.cbImageObfuscation = (System.Windows.Controls.CheckBox) target;
          break;
        case 4:
          this.hlImageObfuscationHelp = (Hyperlink) target;
          this.hlImageObfuscationHelp.Click += new RoutedEventHandler(this.hlImageObfuscationHelp_Click);
          break;
        case 5:
          this.tbScriptsPath = (System.Windows.Controls.TextBox) target;
          this.tbScriptsPath.TextChanged += new TextChangedEventHandler(this.tbScriptsPath_TextChanged);
          break;
        case 6:
          this.btnSelectScriptsPath = (System.Windows.Controls.Button) target;
          this.btnSelectScriptsPath.Click += new RoutedEventHandler(this.btnSelectScriptsPath_Click);
          break;
        case 7:
          this.gbScriptsOnly = (System.Windows.Controls.GroupBox) target;
          break;
        case 8:
          this.cbPrivateObfuscation = (System.Windows.Controls.CheckBox) target;
          break;
        case 9:
          this.hlPrivateObfuscationHelp = (Hyperlink) target;
          this.hlPrivateObfuscationHelp.Click += new RoutedEventHandler(this.hlPrivateObfuscationHelp_Click);
          break;
        case 10:
          this.tbOutputPath = (System.Windows.Controls.TextBox) target;
          this.tbOutputPath.TextChanged += new TextChangedEventHandler(this.tbOutputPath_TextChanged);
          break;
        case 11:
          this.btnSelectOutputPath = (System.Windows.Controls.Button) target;
          this.btnSelectOutputPath.Click += new RoutedEventHandler(this.btnSelectOutputPath_Click);
          break;
        case 12:
          this.hlChannel = (Hyperlink) target;
          this.hlChannel.Click += new RoutedEventHandler(this.hlChannel_Click);
          break;
        case 13:
          this.tbChannel = (System.Windows.Controls.TextBox) target;
          break;
        case 14:
          this.btnExitNoSave = (System.Windows.Controls.Button) target;
          this.btnExitNoSave.Click += new RoutedEventHandler(this.btnExitNoSave_Click);
          break;
        case 15:
          this.btnExitSave = (System.Windows.Controls.Button) target;
          this.btnExitSave.Click += new RoutedEventHandler(this.btnExitSave_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
