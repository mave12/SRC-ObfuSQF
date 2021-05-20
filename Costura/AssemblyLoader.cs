// Decompiled with JetBrains decompiler
// Type: Costura.AssemblyLoader
// Assembly: ObfuSQF, Version=2.9.3.1, Culture=neutral, PublicKeyToken=null
// MVID: 24511B23-02FB-4CC6-8157-3EC744E67057
// Assembly location: C:\Users\matze\Desktop\A3Packer-master\ObfuSQF.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Costura
{
  [CompilerGenerated]
  internal static class AssemblyLoader
  {
    private static readonly object nullCacheLock = new object();
    private static readonly Dictionary<string, bool> nullCache = new Dictionary<string, bool>();
    private static readonly Dictionary<string, string> assemblyNames = new Dictionary<string, string>();
    private static readonly Dictionary<string, string> symbolNames = new Dictionary<string, string>();
    private static int isAttached = 0;

    private static string CultureToString(CultureInfo culture) => culture == null ? "" : culture.Name;

    private static Assembly ReadExistingAssembly(AssemblyName name)
    {
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        AssemblyName name1 = assembly.GetName();
        // ISSUE: reference to a compiler-generated method
        // ISSUE: reference to a compiler-generated method
        if (string.Equals(name1.Name, name.Name, StringComparison.InvariantCultureIgnoreCase) && string.Equals(AssemblyLoader.CultureToString(name1.CultureInfo), AssemblyLoader.CultureToString(name.CultureInfo), StringComparison.InvariantCultureIgnoreCase))
          return assembly;
      }
      return (Assembly) null;
    }

    private static void CopyTo(Stream source, Stream destination)
    {
      byte[] buffer = new byte[81920];
      int count;
      while ((count = source.Read(buffer, 0, buffer.Length)) != 0)
        destination.Write(buffer, 0, count);
    }

    private static Stream LoadStream(string fullname)
    {
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      if (!fullname.EndsWith(".compressed"))
        return executingAssembly.GetManifestResourceStream(fullname);
      using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(fullname))
      {
        using (DeflateStream deflateStream = new DeflateStream(manifestResourceStream, CompressionMode.Decompress))
        {
          MemoryStream memoryStream = new MemoryStream();
          // ISSUE: reference to a compiler-generated method
          AssemblyLoader.CopyTo((Stream) deflateStream, (Stream) memoryStream);
          memoryStream.Position = 0L;
          return (Stream) memoryStream;
        }
      }
    }

    private static Stream LoadStream(Dictionary<string, string> resourceNames, string name)
    {
      string fullname;
      // ISSUE: reference to a compiler-generated method
      return resourceNames.TryGetValue(name, out fullname) ? AssemblyLoader.LoadStream(fullname) : (Stream) null;
    }

    private static byte[] ReadStream(Stream stream)
    {
      byte[] buffer = new byte[stream.Length];
      stream.Read(buffer, 0, buffer.Length);
      return buffer;
    }

    private static Assembly ReadFromEmbeddedResources(
      Dictionary<string, string> assemblyNames,
      Dictionary<string, string> symbolNames,
      AssemblyName requestedAssemblyName)
    {
      string name = requestedAssemblyName.Name.ToLowerInvariant();
      if (requestedAssemblyName.CultureInfo != null && !string.IsNullOrEmpty(requestedAssemblyName.CultureInfo.Name))
        name = string.Format("{0}.{1}", (object) requestedAssemblyName.CultureInfo.Name, (object) name);
      byte[] rawAssembly;
      // ISSUE: reference to a compiler-generated method
      using (Stream stream = AssemblyLoader.LoadStream(assemblyNames, name))
      {
        if (stream == null)
          return (Assembly) null;
        // ISSUE: reference to a compiler-generated method
        rawAssembly = AssemblyLoader.ReadStream(stream);
      }
      // ISSUE: reference to a compiler-generated method
      using (Stream stream = AssemblyLoader.LoadStream(symbolNames, name))
      {
        if (stream != null)
        {
          // ISSUE: reference to a compiler-generated method
          byte[] rawSymbolStore = AssemblyLoader.ReadStream(stream);
          return Assembly.Load(rawAssembly, rawSymbolStore);
        }
      }
      return Assembly.Load(rawAssembly);
    }

    public static Assembly ResolveAssembly(object sender, ResolveEventArgs e)
    {
      // ISSUE: reference to a compiler-generated field
      lock (AssemblyLoader.nullCacheLock)
      {
        // ISSUE: reference to a compiler-generated field
        if (AssemblyLoader.nullCache.ContainsKey(e.Name))
          return (Assembly) null;
      }
      AssemblyName assemblyName = new AssemblyName(e.Name);
      // ISSUE: reference to a compiler-generated method
      Assembly assembly1 = AssemblyLoader.ReadExistingAssembly(assemblyName);
      if ((object) assembly1 != null)
        return assembly1;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated method
      Assembly assembly2 = AssemblyLoader.ReadFromEmbeddedResources(AssemblyLoader.assemblyNames, AssemblyLoader.symbolNames, assemblyName);
      if ((object) assembly2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        lock (AssemblyLoader.nullCacheLock)
        {
          // ISSUE: reference to a compiler-generated field
          AssemblyLoader.nullCache[e.Name] = true;
        }
        if (assemblyName.Flags == AssemblyNameFlags.Retargetable)
          assembly2 = Assembly.Load(assemblyName);
      }
      return assembly2;
    }

    static AssemblyLoader() => AssemblyLoader.assemblyNames.Add("newtonsoft.json", "costura.newtonsoft.json.dll.compressed");

    public static void Attach()
    {
      // ISSUE: reference to a compiler-generated field
      if (Interlocked.Exchange(ref AssemblyLoader.isAttached, 1) == 1)
        return;
      AppDomain.CurrentDomain.AssemblyResolve += (ResolveEventHandler) ((sender, e) =>
      {
        // ISSUE: reference to a compiler-generated field
        lock (AssemblyLoader.nullCacheLock)
        {
          // ISSUE: reference to a compiler-generated field
          if (AssemblyLoader.nullCache.ContainsKey(e.Name))
            return (Assembly) null;
        }
        AssemblyName assemblyName = new AssemblyName(e.Name);
        // ISSUE: reference to a compiler-generated method
        Assembly assembly1 = AssemblyLoader.ReadExistingAssembly(assemblyName);
        if ((object) assembly1 != null)
          return assembly1;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated method
        Assembly assembly2 = AssemblyLoader.ReadFromEmbeddedResources(AssemblyLoader.assemblyNames, AssemblyLoader.symbolNames, assemblyName);
        if ((object) assembly2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          lock (AssemblyLoader.nullCacheLock)
          {
            // ISSUE: reference to a compiler-generated field
            AssemblyLoader.nullCache[e.Name] = true;
          }
          if (assemblyName.Flags == AssemblyNameFlags.Retargetable)
            assembly2 = Assembly.Load(assemblyName);
        }
        return assembly2;
      });
    }
  }
}
