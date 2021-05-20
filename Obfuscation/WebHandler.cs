// Decompiled with JetBrains decompiler
// Type: Maverick_ObfuSQF_Windows_Interface.Obfuscation.WebHandler
// Assembly: ObfuSQF, Version=2.9.3.1, Culture=neutral, PublicKeyToken=null
// MVID: 24511B23-02FB-4CC6-8157-3EC744E67057
// Assembly location: C:\Users\matze\Desktop\A3Packer-master\ObfuSQF.exe

using Maverick_ObfuSQF_Windows_Interface.Config;
using Maverick_ObfuSQF_Windows_Interface.Obfuscation.Models;
using Maverick_ObfuSQF_Windows_Interface.OptionsModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Windows;

namespace Maverick_ObfuSQF_Windows_Interface.Obfuscation
{
  public class WebHandler
  {
    public const string HOST_URI = "https://api.obfusqf.bytex.digital/";
    private ObservableCollection<QueuedUIFile> files;
    private MainWindow parentWindow;
    private string webToken;
    private ManualResetEvent webActionDone = new ManualResetEvent(false);
    private byte[] lastResponse;
    private QueuedUIFile currentFile;
    private bool currentFileHasError = false;
    private Exception lastException = (Exception) null;

    private IniFile Configuration { get; set; } = new IniFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "config.ini"));

    public WebHandler(
      MainWindow parentWindowInstance,
      ObservableCollection<QueuedUIFile> fileList,
      string token)
    {
      this.files = fileList;
      this.parentWindow = parentWindowInstance;
      this.webToken = token;
    }

    public void Start()
    {
      ServicePointManager.ServerCertificateValidationCallback += (RemoteCertificateValidationCallback) ((sender, cert, chain, sslPolicyErrors) => true);
      WebClient webClient = new WebClient();
      webClient.UploadProgressChanged += new UploadProgressChangedEventHandler(this.Wc_UploadProgressChanged);
      webClient.UploadFileCompleted += new UploadFileCompletedEventHandler(this.Wc_UploadFileCompleted);
      webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.Wc_DownloadProgressChanged);
      webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.Wc_DownloadFileCompleted);
      foreach (QueuedUIFile file in (Collection<QueuedUIFile>) this.files)
      {
        if (file.IsSelected)
        {
          file.ProgressValue = 0.0;
          file.IsObfuscating = false;
          file.Status = "Queued";
        }
        else
        {
          file.ProgressValue = 0.0;
          file.IsObfuscating = false;
          file.Status = "Skipped";
        }
      }
      this.UpdateUI();
      foreach (QueuedUIFile file in (Collection<QueuedUIFile>) this.files)
      {
        if (file.IsSelected)
        {
          if (!Directory.Exists(file.FileName))
          {
            file.Status = "Source not found";
            this.UpdateUI();
          }
          else
          {
            this.currentFile = file;
            string str1 = "";
            this.currentFileHasError = false;
            try
            {
              file.IsObfuscating = true;
              file.Status = "Packing";
              this.UpdateUI();
              string name = new DirectoryInfo(file.FileName).Name;
              string fullName = Directory.GetParent(file.FileName).FullName;
              str1 = Path.Combine(fullName, name) + ".zip";
              string lower1 = this.Configuration.IniReadValue("Settings", "ImageObfuscationEnabled").ToLower();
              string lower2 = this.Configuration.IniReadValue("Settings", "PrivateObfuscationEnabled").ToLower();
              string lower3 = this.Configuration.IniReadValue("Settings", "PublicObfuscationEnabled").ToLower();
              string lower4 = this.Configuration.IniReadValue("Settings", "KeyLocker").ToLower();
              string str2 = this.Configuration.IniReadValue("Settings", "ScriptsPath");
              string str3 = this.Configuration.IniReadValue("Settings", "Channel").ToLower();
              if (str3.Length == 0)
                str3 = "stable";
              if (System.IO.File.Exists(str2))
                System.IO.File.Copy(str2, Path.Combine(file.FileName, "$scripts$"), true);
              else
                System.IO.File.WriteAllText(Path.Combine(file.FileName, "$scripts$"), "");
              int num1 = -1;
              string pboTypeSelected = file.PBOTypeSelected;
              if (!(pboTypeSelected == "Missionfile"))
              {
                if (pboTypeSelected == "Mod")
                  num1 = 1;
              }
              else
                num1 = 0;
              if (lower3 == "true")
              {
                string str4 = System.IO.File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObfuSQF", "global_variables.json"));
                if (string.IsNullOrEmpty(str4))
                  str4 = "[]";
                try
                {
                  List<VariableModel> variableModelList = (List<VariableModel>) JsonConvert.DeserializeObject<List<VariableModel>>(str4) ?? new List<VariableModel>();
                  if (!System.IO.File.Exists(Path.Combine(file.FileName, "$vars1$")))
                    System.IO.File.WriteAllText(Path.Combine(file.FileName, "$vars1$"), "");
                  if (!System.IO.File.Exists(Path.Combine(file.FileName, "$vars2$")))
                    System.IO.File.WriteAllText(Path.Combine(file.FileName, "$vars2$"), "");
                  foreach (VariableModel variableModel in variableModelList)
                  {
                    if (variableModel.PresenceItemSelected == "Only within current PBO")
                      System.IO.File.AppendAllLines(Path.Combine(file.FileName, "$vars1$"), (IEnumerable<string>) new string[1]
                      {
                        variableModel.VariableName
                      });
                    else if (variableModel.PresenceItemSelected == "Used across PBOs")
                      System.IO.File.AppendAllLines(Path.Combine(file.FileName, "$vars2$"), (IEnumerable<string>) new string[1]
                      {
                        variableModel.VariableName
                      });
                  }
                }
                catch
                {
                }
              }
              if (System.IO.File.Exists(str1))
                System.IO.File.Delete(str1);
              ZipFile.CreateFromDirectory(file.FileName, str1);
              if (System.IO.File.Exists(Path.Combine(file.FileName, "$scripts$")))
                System.IO.File.Delete(Path.Combine(file.FileName, "$scripts$"));
              if (System.IO.File.Exists(Path.Combine(file.FileName, "$vars1$")))
                System.IO.File.Delete(Path.Combine(file.FileName, "$vars1$"));
              if (System.IO.File.Exists(Path.Combine(file.FileName, "$vars2$")))
                System.IO.File.Delete(Path.Combine(file.FileName, "$vars2$"));
              if (new FileInfo(str1).Length >= 1073741824L)
              {
                file.Status = "File Too Large";
                file.ProgressValue = 1.0;
                file.IsObfuscating = false;
                this.UpdateUI();
                try
                {
                  System.IO.File.Delete(str1);
                  continue;
                }
                catch
                {
                  continue;
                }
              }
              else
              {
                file.IsObfuscating = false;
                file.Status = "Uploading";
                this.UpdateUI();
                this.webActionDone.Reset();
                string uriString = string.Format("{0}api/intern/v1/iobfuscationprovider/uploadfile.ashx?token={1}&settingImageObfuscation={2}&settingPrivateVariableObfuscation={3}&settingGlobalVariableObfuscation={4}&settingKeyLocker={5}&filetype={6}&channel={7}", (object) "https://api.obfusqf.bytex.digital/", (object) this.webToken, (object) lower1, (object) lower2, (object) lower3, (object) lower4, (object) num1, (object) str3);
                webClient.UploadFileAsync(new Uri(uriString), "POST", str1);
                this.webActionDone.WaitOne();
                try
                {
                  System.IO.File.Delete(str1);
                }
                catch
                {
                }
                if (this.currentFileHasError)
                {
                  file.Status = "Error uploading";
                  file.ProgressValue = 1.0;
                  file.IsObfuscating = false;
                  this.UpdateUI();
                  int num2 = (int) MessageBox.Show("An error occured while attempting to upload the file:" + Environment.NewLine + this.lastException.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
                  continue;
                }
                this.currentFileHasError = false;
                if (this.lastResponse == null)
                {
                  file.Status = "Internal Server Error";
                  file.ProgressValue = 1.0;
                  file.IsObfuscating = false;
                  this.UpdateUI();
                  continue;
                }
                string jobId;
                try
                {
                  UploadFileResponse uploadFileResponse = (UploadFileResponse) JsonConvert.DeserializeObject<UploadFileResponse>(Encoding.ASCII.GetString(this.lastResponse));
                  this.lastResponse = (byte[]) null;
                  if (uploadFileResponse.Error != "")
                  {
                    file.Status = "Server Error: " + uploadFileResponse.Error;
                    file.IsObfuscating = false;
                    file.ProgressValue = 1.0;
                    this.UpdateUI();
                    break;
                  }
                  jobId = uploadFileResponse.Response.JobId;
                  file.ProgressValue = 1.0;
                  file.IsObfuscating = true;
                  file.Status = "Obfuscating";
                  this.UpdateUI();
                }
                catch (Exception ex)
                {
                  file.Status = "Server Error: Cannot deserialize server response to upload" + Environment.NewLine + ex.ToString();
                  file.IsObfuscating = false;
                  file.ProgressValue = 1.0;
                  this.UpdateUI();
                  break;
                }
                bool flag = false;
label_73:
                try
                {
                  Thread.Sleep(500);
                  GetStatusResponse getStatusResponse = (GetStatusResponse) JsonConvert.DeserializeObject<GetStatusResponse>(webClient.DownloadString("https://api.obfusqf.bytex.digital/api/intern/v1/iobfuscationprovider/getstatus.ashx?token=" + this.webToken + "&jobid=" + jobId));
                  if (!(getStatusResponse.Response.Status == "JOB_READY"))
                  {
                    if (getStatusResponse.Response.Status.StartsWith("JOB_ERROR_"))
                    {
                      file.Status = getStatusResponse.Response.Status;
                      file.IsObfuscating = false;
                      file.ProgressValue = 1.0;
                      this.UpdateUI();
                      flag = true;
                    }
                    else
                      goto label_73;
                  }
                }
                catch
                {
                  flag = true;
                  file.Status = "Server Error: Cannot fetch status";
                  file.IsObfuscating = false;
                  file.ProgressValue = 1.0;
                  this.UpdateUI();
                }
                if (!flag)
                {
                  file.ProgressValue = 0.0;
                  file.IsObfuscating = false;
                  file.Status = "Downloading";
                  this.UpdateUI();
                  try
                  {
                    if (System.IO.File.Exists(Path.Combine(fullName, name) + ".pbo"))
                      System.IO.File.Delete(Path.Combine(fullName, name) + ".pbo");
                  }
                  catch
                  {
                  }
                  string path1 = fullName;
                  if (Directory.Exists(this.Configuration.IniReadValue("Settings", "OutputMasterDirectory")))
                    path1 = this.Configuration.IniReadValue("Settings", "OutputMasterDirectory");
                  try
                  {
                    this.webActionDone.Reset();
                    webClient.DownloadFileAsync(new Uri("https://api.obfusqf.bytex.digital/api/intern/v1/iobfuscationprovider/getfile.ashx?token=" + this.webToken + "&jobid=" + jobId), Path.Combine(path1, name) + ".pbo");
                  }
                  catch (WebException ex)
                  {
                    file.ProgressValue = 1.0;
                    file.IsObfuscating = false;
                    file.Status = "Server Error: Cannot download final file";
                    this.UpdateUI();
                    continue;
                  }
                  catch (Exception ex)
                  {
                    file.ProgressValue = 1.0;
                    file.IsObfuscating = false;
                    file.Status = "Local Error: Error saving file to drive";
                    this.UpdateUI();
                    continue;
                  }
                  this.webActionDone.WaitOne();
                  if (this.currentFileHasError)
                  {
                    file.Status = "Error downloading";
                    file.ProgressValue = 1.0;
                    file.IsObfuscating = false;
                    this.UpdateUI();
                    continue;
                  }
                  file.ProgressValue = 1.0;
                  file.IsObfuscating = false;
                  file.Status = "Done";
                  this.UpdateUI();
                }
                else
                  continue;
              }
            }
            catch (Exception ex)
            {
              file.ProgressValue = 1.0;
              file.IsObfuscating = false;
              file.Status = "Unexpected Error: " + ex.ToString();
              this.UpdateUI();
            }
            if (System.IO.File.Exists(str1))
              System.IO.File.Delete(str1);
          }
        }
      }
      this.parentWindow.Dispatcher.Invoke((Action) (() =>
      {
        this.parentWindow.ToggleUI();
        int num = (int) MessageBox.Show("Operation successfull", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
      }));
    }

    private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        this.currentFileHasError = true;
        this.webActionDone.Set();
      }
      else
        this.webActionDone.Set();
    }

    private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
      this.currentFile.IsObfuscating = false;
      this.currentFile.ProgressValue = (double) e.BytesReceived / (double) e.TotalBytesToReceive;
      this.UpdateUI();
    }

    private void Wc_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
    {
      try
      {
        this.lastResponse = e.Result;
        this.webActionDone.Set();
      }
      catch (Exception ex)
      {
        this.lastException = ex;
        this.currentFileHasError = true;
        this.currentFile.Status = "Internal Server Error";
        this.webActionDone.Set();
      }
    }

    private void Wc_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
    {
      this.currentFile.IsObfuscating = false;
      this.currentFile.ProgressValue = (double) e.BytesSent / (double) e.TotalBytesToSend;
      if (this.currentFile.ProgressValue == 1.0)
        this.currentFile.IsObfuscating = true;
      this.UpdateUI();
    }

    public void UpdateUI() => this.parentWindow.Dispatcher.Invoke((Action) (() => this.parentWindow.dgFileBrowser.Items.Refresh()));
  }
}
