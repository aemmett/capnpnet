Another C# implementation of [Cap'n Proto][].

Status
------

Currently work in progress. Still hacking on the code, but it appears the
schema compiler is working at least. There are still some known bugs.

This implementation is based around the ability to perform pointer arithmetic
and reinterpret casts on "managed pointers" (`ref` keyword), allowing one
internal mechanism that can work with managed and unmanaged memory. In thoery,
this means generated schema code's accessors consist of direct calls that can
potentially be inlined by the JITter up to the point of raw memory access.

Usage
-----

Works in frameworks: (TODO: investigate; currently targeting 4.5.2 but may be
able to target earlier?)

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
  - Upgraded list structs
  - Pointer field defaults
  - Copying messages with lists containing pointers
  - Primitive list -> struct list upgradability (merge PrimitiveList and CopositeList?)
- Codegen improvements
  - annotations to control codegen (namespace, etc.)
  - missing metadata
- Equals, GetHashCode boilerplate
- Better AnyPointer support
- APIs to support easier writing
  - struct copying, size measurements
  - Implement more standard interfaces?
  - Automatic discriminant setting/validation?
- Runtime and compiler version safety checks

### Missing features
- Tests / documentation
  - Check for integer overflows. I'm pretty sure there's a lot of them when
    dealing with >2GB (possibly smaller) messages, may potentially compromise
    memory safety / process stability
- Packing/unpacking
- dynamic API

### Nice to haves
- VS integration 
- .Net Core compatibility
  - what does this entail? Do I need to make unsafe code optional?
- Portability improvements (data alignment, big endian support)
- Project name
- More optimizations: object pooling, inlining improvements?
- Performance analysis
- Re-incorporate `Span<T>`-based APIs
- Support for incremental reads?
- Find some tool to inline/prune all the compiler dependencies?

[Cap'n Proto]: https://capnproto.org