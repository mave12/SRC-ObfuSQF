// Decompiled with JetBrains decompiler
// Type: Maverick_ObfuSQF_Windows_Interface.OptionsModels.VariableModel
// Assembly: ObfuSQF, Version=2.9.3.1, Culture=neutral, PublicKeyToken=null
// MVID: 24511B23-02FB-4CC6-8157-3EC744E67057
// Assembly location: C:\Users\matze\Desktop\A3Packer-master\ObfuSQF.exe

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maverick_ObfuSQF_Windows_Interface.OptionsModels
{
  public class VariableModel
  {
    public string VariableName { get; set; } = "My Variable";

    [JsonIgnore]
    public List<string> PresenceOptions { get; set; } = new List<string>()
    {
      "Only within current PBO",
      "Used across PBOs"
    };

    public string PresenceItemSelected { get; set; } = "Only within current PBO";
  }
}
