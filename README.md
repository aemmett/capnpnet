Currently work in progress. Another C# implementation of [Cap'n Proto][].

This implementation is based around the ability to perform pointer arithmatic
and reinterpret casts on "managed pointers" (`ref` keyword), allowing one
internal mechanism that can work with managed and unmanaged memory.

Usage
-----

Works in frameworks: (TODO: investigate; currently targeting 4.5.2 but may be
able to target earlier?)

Acquisition: (for now) build from source.

(TODO: basic overview / getting started)

Features
--------

- Supports `byte[]` segments or `SafeBuffer`s

Building
--------

(TODO: write detailed instructions. So far been built with VS2017 Community)

Features (that should be mostly working)
----------------------------------------

- Schema generation
- Reading unpacked messages and their contents

TODO
----

(in no particular order)

- Tests / documentation
  - Check for integer overflows. I'm pretty sure there's a lot of them when
    dealing with >2GB (possibly smaller) messages, may potentially compromise
    memory safety / process stability
- Missing edge cases (upgrade list structs, etc.)
- Equals, GetHashCode boilerplate
- Codegen improvements
  - annotations to control codegen
  - metadata attributes
- dynamic API
- VS integration 
- .Net Core compatibility
- Project name
- More optimizations: object pooling, inlining improvements?
- Performance analysis
- RPC layer
- Generalize segment memory (or wait until )
- Re-incorporate `Span<T>`-based APIs
- support for incremental reads?

[Cap'n Proto] : https://capnproto.org