// Decompiled with JetBrains decompiler
// Type: Maverick_ObfuSQF_Windows_Interface.QueuedUIFile
// Assembly: ObfuSQF, Version=2.9.3.1, Culture=neutral, PublicKeyToken=null
// MVID: 24511B23-02FB-4CC6-8157-3EC744E67057
// Assembly location: C:\Users\matze\Desktop\A3Packer-master\ObfuSQF.exe

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maverick_ObfuSQF_Windows_Interface
{
  public class QueuedUIFile
  {
    public const string PBOTYPE_MOD = "Mod";
    public const string PBOTYPE_MISSIONFILE = "Missionfile";

    public string FileName { get; set; } = "";

    [JsonIgnore]
    public string Status { get; set; } = "Queued";

    [JsonIgnore]
    public double ProgressValue { get; set; } = 0.0;

    [JsonIgnore]
    public bool IsObfuscating { get; set; } = false;

    [JsonIgnore]
    public List<string> PBOTypes { get; set; } = new List<string>()
    {
      "Mod",
      "Missionfile"
    };

    public string PBOTypeSelected { get; set; } = "Missionfile";

    public bool IsSelected { get; set; } = true;
  }
}
