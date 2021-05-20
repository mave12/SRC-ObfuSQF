// Decompiled with JetBrains decompiler
// Type: Maverick_ObfuSQF_Windows_Interface.Obfuscation.Models.UploadFileResponse
// Assembly: ObfuSQF, Version=2.9.3.1, Culture=neutral, PublicKeyToken=null
// MVID: 24511B23-02FB-4CC6-8157-3EC744E67057
// Assembly location: C:\Users\matze\Desktop\A3Packer-master\ObfuSQF.exe

namespace Maverick_ObfuSQF_Windows_Interface.Obfuscation.Models
{
  public class UploadFileResponse
  {
    public string Error { get; set; } = "";

    public UploadFileResponse.ResponseObject Response { get; set; } = (UploadFileResponse.ResponseObject) null;

    public class ResponseObject
    {
      public string Status { get; set; } = "";

      public int UserId { get; set; } = -1;

      public string JobId { get; set; } = "";

      public string FileName { get; set; }
    }
  }
}
