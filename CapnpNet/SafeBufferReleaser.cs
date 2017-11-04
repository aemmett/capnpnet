using System;
using System.Runtime.InteropServices;

namespace CapnpNet
{
  public sealed class SafeBufferReleaser : IDisposable
  {
    private SafeBuffer _safeBuffer;

    public SafeBufferReleaser(SafeBuffer safeBuffer) => this.Init(safeBuffer);

    public bool IsDisposed { get; private set; } = true;
    
    public void Init(SafeBuffer safeBuffer)
    {
      if (this.IsDisposed == false) throw new InvalidOperationException("Previous pointer not disposed");

      _safeBuffer = safeBuffer;
      this.IsDisposed = false;
    }

    #region IDisposable Support
    private void Dispose(bool disposing)
    {
      if (this.IsDisposed == false)
      {
        if (disposing)
        {
          // no managed resources to dispose
        }

        _safeBuffer.ReleasePointer();
        _safeBuffer = null;

        this.IsDisposed = true;
      }
    }
    
    ~SafeBufferReleaser()
    {
      Dispose(false);
    }
    
    public void Dispose()
    {
      Dispose(true);

      // TODO: don't suppress finalize if returned to a pool?
      GC.SuppressFinalize(this);
    }
    #endregion
  }
}