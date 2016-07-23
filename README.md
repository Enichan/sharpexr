# SharpEXR

C# implementation of OpenEXR file format, a high dynamic range imaging image file format created by Industrial Light and Magic (ILM).

Currently only reads single part files with uncompressed scanlines.

## Example Usage

```cs
var filePath = "somefile.exr";

// load the EXR file headers and parts, but not actual pixel data
var exrFile = EXRFile.FromFile(filePath);

// read pixel data for first part in file
var part = exrFile.Parts[0].Open(filePath);

// get pixel data as an array of RGBA unsigned bytes,
//   auto detecting source format,
//   converted to sRGB (screen) color space,
//   premultiplying RGB values by alpha values
byte[] bytes = part.GetBytes(ImageDestFormat.PremultipliedBGRA8, GammaEncoding.sRGB);

// get pixel data as an array of BGR 16 bit floats,
//   auto detecting source format,
//   converted to Gamma color space,
//   not including alpha channel
Half[] halfs = part.GetHalfs(ChannelConfiguration.BGR, false, GammaEncoding.Gamma, false);

// get pixel data as an array of RGBA 32 bit floats,
//   auto detecting source format,
//   not converted (linear color space),
//   including alpha but not premultiplied
float[] floats = part.GetFloats(ChannelConfiguration.RGB, true, GammaEncoding.Linear, true);

// close part
part.Close();
```

## Saving

Saving files is currently not supported.

## Single-threaded Load

The project is compiled with the PARALLEL conditional set by default. Removing this conditional compilation symbol will force the library to read image scanlines sequentially instead of in parallel. This is much slower and not recommended.