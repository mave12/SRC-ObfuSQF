// Decompiled with JetBrains decompiler
// Type: Maverick_ObfuSQF_Windows_Interface.Login
// Assembly: ObfuSQF, Version=2.9.3.1, Culture=neutral, PublicKeyToken=null
// MVID: 24511B23-02FB-4CC6-8157-3EC744E67057
// Assembly location: C:\Users\matze\Desktop\A3Packer-master\ObfuSQF.exe

using Maverick_ObfuSQF_Windows_Interface.Config;
using Maverick_ObfuSQF_Windows_Interface.Obfuscation;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Maverick_ObfuSQF_Windows_Interface
{
  public partial class Login : Window, IComponentConnector
  {
    internal TextBox tbToken;
    internal Button btnLogin;
    private bool _contentLoaded;

    public Login()
    {
      this.InitializeComponent();
      IniFile iniFile = new IniFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "config.ini"));
      try
      {
        if (string.IsNullOrEmpty(iniFile.IniReadValue("Auth", "Token")))
          return;
        this.tbToken.Text = iniFile.IniReadValue("Auth", "Token");
      }
      catch
      {
      }
    }

    private void btnLogin_Click(object sender, RoutedEventArgs e)
    {
      if (this.tbToken.Text == "")
      {
        int num1 = (int) MessageBox.Show("Please enter your account token", "Token required", MessageBoxButton.OK, MessageBoxImage.Hand);
      }
      else
      {
        this.btnLogin.IsEnabled = false;
        if (!Website.IsTokenAuthorized(this.tbToken.Text))
        {
          int num2 = (int) MessageBox.Show("The entered token is not valid." + Environment.NewLine + "This could have the following reasons:" + Environment.NewLine + Environment.NewLine + "- The associated account has been locked" + Environment.NewLine + "- The token does not exist" + Environment.NewLine + "- There is no active license" + Environment.NewLine, "Invalid token", MessageBoxButton.OK, MessageBoxImage.Hand);
        }
        else
        {
          new IniFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "config.ini")).IniWriteValue("Auth", "Token", this.tbToken.Text);
          new MainWindow().Show();
          this.Close();
        }
        this.btnLogin.IsEnabled = true;
      }
    }

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/ObfuSQF;component/login.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.tbToken = (TextBox) target;
          break;
        case 2:
          this.btnLogin = (Button) target;
          this.btnLogin.Click += new RoutedEventHandler(this.btnLogin_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
