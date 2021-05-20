// Decompiled with JetBrains decompiler
// Type: Maverick_ObfuSQF_Windows_Interface.MainWindow
// Assembly: ObfuSQF, Version=2.9.3.1, Culture=neutral, PublicKeyToken=null
// MVID: 24511B23-02FB-4CC6-8157-3EC744E67057
// Assembly location: C:\Users\matze\Desktop\A3Packer-master\ObfuSQF.exe

using Maverick_ObfuSQF_Windows_Interface.Config;
using Maverick_ObfuSQF_Windows_Interface.Obfuscation;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;

namespace Maverick_ObfuSQF_Windows_Interface
{
  public partial class MainWindow : Window, IComponentConnector
  {
    internal System.Windows.Controls.MenuItem btnChangeToken;
    internal System.Windows.Controls.DataGrid dgFileBrowser;
    internal System.Windows.Controls.Button btnAddSourceFolder;
    internal System.Windows.Controls.Button btnRemoveSelectedFolder;
    internal System.Windows.Controls.Button btnRemoveAllFolders;
    internal System.Windows.Controls.Button btnOptions;
    internal System.Windows.Controls.Button btnObfuscate;
    private bool _contentLoaded;

    public static bool FirstStartup { get; set; } = true;

    public ObservableCollection<QueuedUIFile> UIFiles { get; set; } = new ObservableCollection<QueuedUIFile>();

    private IniFile Configuration { get; set; }

    public MainWindow()
    {
      ServicePointManager.Expect100Continue = true;
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
      this.InitializeComponent();
      if (MainWindow.FirstStartup)
      {
        MainWindow.FirstStartup = false;
        try
        {
          string str1 = new WebClient().DownloadString("https://obfusqf.bytex.digital/downloads/windows_interface_version.txt");
          string str2 = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version.ToString();
          if (str1 != str2)
          {
            int num = (int) System.Windows.MessageBox.Show("An update is available. Go to obfusqf.bytex.digital to download the new client. " + Environment.NewLine + Environment.NewLine + "Local Version: " + str2 + Environment.NewLine + "Server Version: " + str1, "Update available", MessageBoxButton.OK, MessageBoxImage.Asterisk);
          }
        }
        catch (Exception ex)
        {
          int num = (int) System.Windows.MessageBox.Show("Could not check for update: " + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF"));
        if (!System.IO.File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "config.ini")))
          System.IO.File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "config.ini"), "");
      }
      this.Configuration = new IniFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "config.ini"));
      if (this.Configuration.IniReadValue("Auth", "Token") == "")
      {
        new Login().Show();
        this.Close();
      }
      else
      {
        try
        {
          string str = "";
          if (System.IO.File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "file_list.json")))
            str = System.IO.File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "file_list.json"));
          if (str == "")
            str = "[]";
          this.UIFiles = (ObservableCollection<QueuedUIFile>) JsonConvert.DeserializeObject<ObservableCollection<QueuedUIFile>>(str);
          if (this.UIFiles == null)
            this.UIFiles = new ObservableCollection<QueuedUIFile>();
        }
        catch
        {
        }
        try
        {
          int int32_1 = Convert.ToInt32(this.Configuration.IniReadValue("Window_Main", "sizeX"));
          int int32_2 = Convert.ToInt32(this.Configuration.IniReadValue("Window_Main", "sizeY"));
          if (int32_1 > 670 && int32_2 > 420)
          {
            this.Height = (double) int32_2;
            this.Width = (double) int32_1;
          }
        }
        catch
        {
        }
        this.dgFileBrowser.ItemsSource = (IEnumerable) this.UIFiles;
        this.Closing += new CancelEventHandler(this.MainWindow_Closing);
        this.SizeChanged += new SizeChangedEventHandler(this.MainWindow_SizeChanged);
      }
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.Configuration.IniWriteValue("Window_Main", "sizeX", e.NewSize.Width.ToString());
      this.Configuration.IniWriteValue("Window_Main", "sizeY", e.NewSize.Height.ToString());
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
      if (System.IO.File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "file_list.json")))
        System.IO.File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "file_list.json"));
      System.IO.File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "file_list.json"), JsonConvert.SerializeObject((object) this.UIFiles, (Formatting) 1));
    }

    private void btnAddSourceFolder_Click(object sender, RoutedEventArgs e)
    {
      string sourceFolderPath = "";
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
      {
        if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
          return;
        if (Directory.Exists(folderBrowserDialog.SelectedPath))
        {
          sourceFolderPath = folderBrowserDialog.SelectedPath;
        }
        else
        {
          int num = (int) System.Windows.MessageBox.Show("Invalid path", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
          return;
        }
      }
      if (this.UIFiles.Where<QueuedUIFile>((Func<QueuedUIFile, bool>) (x => x.FileName == sourceFolderPath)).Count<QueuedUIFile>() > 0)
      {
        int num1 = (int) System.Windows.MessageBox.Show("Path already in list", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
      }
      else
        this.UIFiles.Add(new QueuedUIFile()
        {
          FileName = sourceFolderPath,
          IsObfuscating = false,
          ProgressValue = 0.0,
          Status = "Queued"
        });
    }

    private void btnRemoveSelectedFolder_Click(object sender, RoutedEventArgs e)
    {
      if (!(this.dgFileBrowser.SelectedItem is QueuedUIFile selectedItem))
        return;
      this.UIFiles.Remove(selectedItem);
    }

    private void btnRemoveAllFolders_Click(object sender, RoutedEventArgs e) => this.UIFiles.Clear();

    private void btnObfuscate_Click(object sender, RoutedEventArgs e)
    {
      if (this.UIFiles.Count == 0)
        return;
      this.ToggleUI(false);
      this.UIFiles.ToArray<QueuedUIFile>();
      Task.Run((Action) (() => new WebHandler(this, this.UIFiles, this.Configuration.IniReadValue("Auth", "Token")).Start()));
    }

    public void ToggleUI(bool enabled = true)
    {
      this.btnAddSourceFolder.IsEnabled = enabled;
      this.btnObfuscate.IsEnabled = enabled;
      this.btnRemoveAllFolders.IsEnabled = enabled;
      this.btnRemoveSelectedFolder.IsEnabled = enabled;
      this.btnOptions.IsEnabled = enabled;
      this.dgFileBrowser.IsEnabled = enabled;
      this.btnChangeToken.IsEnabled = enabled;
    }

    private void btnOptions_Click(object sender, RoutedEventArgs e)
    {
      new Options().Show();
      this.Close();
    }

    private void BtnChangeToken_Click(object sender, RoutedEventArgs e)
    {
      new Login().Show();
      this.Close();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/ObfuSQF;component/mainwindow.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.btnChangeToken = (System.Windows.Controls.MenuItem) target;
          this.btnChangeToken.Click += new RoutedEventHandler(this.BtnChangeToken_Click);
          break;
        case 2:
          this.dgFileBrowser = (System.Windows.Controls.DataGrid) target;
          break;
        case 3:
          this.btnAddSourceFolder = (System.Windows.Controls.Button) target;
          this.btnAddSourceFolder.Click += new RoutedEventHandler(this.btnAddSourceFolder_Click);
          break;
        case 4:
          this.btnRemoveSelectedFolder = (System.Windows.Controls.Button) target;
          this.btnRemoveSelectedFolder.Click += new RoutedEventHandler(this.btnRemoveSelectedFolder_Click);
          break;
        case 5:
          this.btnRemoveAllFolders = (System.Windows.Controls.Button) target;
          this.btnRemoveAllFolders.Click += new RoutedEventHandler(this.btnRemoveAllFolders_Click);
          break;
        case 6:
          this.btnOptions = (System.Windows.Controls.Button) target;
          this.btnOptions.Click += new RoutedEventHandler(this.btnOptions_Click);
          break;
        case 7:
          this.btnObfuscate = (System.Windows.Controls.Button) target;
          this.btnObfuscate.Click += new RoutedEventHandler(this.btnObfuscate_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
