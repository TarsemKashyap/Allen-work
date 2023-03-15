// Decompiled with JetBrains decompiler
// Type: WhitespaceFilter
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class WhitespaceFilter : Stream
{
  private Stream _sink;
  private static Regex reg = new Regex("(?<=[^])\\t{2,}|(?<=[>])\\s{2,}(?=[<])|(?<=[>])\\s{2,11}(?=[<])|(?=[\\n])\\s{2,}");
  private long _position;

  public WhitespaceFilter(Stream sink) => this._sink = sink;

  public override bool CanRead => true;

  public override bool CanSeek => true;

  public override bool CanWrite => true;

  public override void Flush() => this._sink.Flush();

  public override long Length => 0;

  public override long Position
  {
    get => this._position;
    set => this._position = value;
  }

  public override int Read(byte[] buffer, int offset, int count) => this._sink.Read(buffer, offset, count);

  public override long Seek(long offset, SeekOrigin origin) => this._sink.Seek(offset, origin);

  public override void SetLength(long value) => this._sink.SetLength(value);

  public override void Close() => this._sink.Close();

  public override void Write(byte[] buffer, int offset, int count)
  {
    byte[] dst = new byte[count];
    Buffer.BlockCopy((Array) buffer, offset, (Array) dst, 0, count);
    string input = Encoding.Default.GetString(buffer);
    byte[] bytes = Encoding.Default.GetBytes(WhitespaceFilter.reg.Replace(input, string.Empty));
    this._sink.Write(bytes, 0, bytes.GetLength(0));
  }
}
