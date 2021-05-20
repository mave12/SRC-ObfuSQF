// Decompiled with JetBrains decompiler
// Type: Maverick_ObfuSQF_Windows_Interface.Config.IniFile
// Assembly: ObfuSQF, Version=2.9.3.1, Culture=neutral, PublicKeyToken=null
// MVID: 24511B23-02FB-4CC6-8157-3EC744E67057
// Assembly location: C:\Users\matze\Desktop\A3Packer-master\ObfuSQF.exe

using System.Runtime.InteropServices;
using System.Text;

namespace Maverick_ObfuSQF_Windows_Interface.Config
{
  public class IniFile
  {
    public string path;

    [DllImport("kernel32")]
    private static extern long WritePrivateProfileString(
      string section,
      string key,
      string val,
      string filePath);

    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(
      string section,
      string key,
      string def,
      StringBuilder retVal,
      int size,
      string filePath);

    public IniFile(string INIPath) => this.path = INIPath;

    public void IniWriteValue(string Section, string Key, string Value) => IniFile.WritePrivateProfileString(Section, Key, Value, this.path);

    public string IniReadValue(string Section, string Key)
    {
      StringBuilder retVal = new StringBuilder((int) byte.MaxValue);
      IniFile.GetPrivateProfileString(Section, Key, "", retVal, (int) byte.MaxValue, this.path);
      return retVal.ToString();
    }
  }
}
