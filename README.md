Another C# implementation of [Cap'n Proto][].

Status
------

Currently work in progress. Still hacking on the code, but it appears the
schema compiler is working (for the most part) at least. There are still some
known bugs.

This implementation is based around the ability to perform pointer arithmetic
and reinterpret casts on "managed pointers" (`ref` keyword), allowing one
internal mechanism that can work with managed and unmanaged memory. In thoery,
this means generated schema code's accessors consist of direct calls that can
potentially be inlined by the JITter up to the point of raw memory access.

Usage
-----

Works in frameworks: .NET Standard 1.1

Acquisition: (for now) build from source.

(TODO: basic overview / getting started)

Building
--------

(TODO: write detailed instructions. So far been built with VS2017 Community)

Features (that should be mostly working)
----------------------------------------

- Schema generation
- Reading unpacked messages and their contents
  - Supports `byte[]` and `SafeBuffer`
- (to be documented)

TODO
----

### Bugs / unimplemented cases

- Missing edge cases
  - Pointer field defaults
- Codegen improvements
  - annotations to control codegen (namespace, etc.)
  - missing metadata
  - auto-guess namespace by looking for project/solution files, using directory hierarchy?
- Equals, GetHashCode boilerplate
- APIs to support easier writing
  - Implement more standard interfaces?
  - Automatic discriminant setting/validation?
- Runtime and compiler version safety checks

### Work in progress

- dynamic API
- Packing/unpacking

### Missing features

- Tests / documentation
  - Check for integer overflows. I'm pretty sure there's a lot of them when
    dealing with >2GB (possibly smaller) messages, may potentially compromise
    memory safety / process stability

### Nice to haves

- VS integration
  - Wait for better language server protocol support?
- Portability improvements (data alignment, big endian support)
- Project name
- More optimizations: object pooling, inlining improvements?
- Performance analysis
- Re-incorporate `Span<T>`-based APIs
- Use new System.IO.Pipelines abstractions
  - Good as a more optimized stream abstraction, but maybe not suitable for shared memory RPC?
- Support for incremental reads?

[Cap'n Proto]: https://capnproto.org