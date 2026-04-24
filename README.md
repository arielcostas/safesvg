# SafeSVG

A free, libre and open-source tool to sanitise SVG content in .NET applications.

[![Build status](https://img.shields.io/github/actions/workflow/status/arielcostas/safesvg/unit-tests-push.yml?branch=main&style=for-the-badge)](https://github.com/arielcostas/safesvg/actions/workflows/unit-tests-push.yml)
[![Nuget version](https://img.shields.io/nuget/v/Costasdev.SafeSvg?style=for-the-badge)](https://www.nuget.org/packages/Costasdev.Safesvg)
[![Licence](https://img.shields.io/github/license/arielcostas/SafeSvg?style=for-the-badge)](https://github.com/arielcostas/safesvg/blob/main/LICENCE)

## Features

- Removes potentially dangerous elements and attributes from SVG files (like `<script>` and `onload`).
- Adds (by default, if missing) the SVG `xmlns` attribute to the root element.
- Removes (by default) comments from the SVG content.
- Optionally (opt-in) preserves styles
- Supports .NET 8 and 10.

## Usage

1. Install the NuGet package:

   ```bash
   dotnet add package Costasdev.SafeSvg
   ```

2. Use the `SafeSvg` class to sanitise SVG content:

   ```csharp
    using Costasdev.SafeSVG;
   
    var svgContent = "<svg><script>alert('XSS');</script></svg>";
    var safeSvg = SvgSanitiser.Sanitise(svgContent);
    Console.WriteLine(safeSvg); // <svg xmlns="http://www.w3.org/2000/svg"></svg>
    ```

3. You can also provide custom options:

   ```csharp
   using Costasdev.SafeSVG;
   
   var svgContent = "<svg><script>alert('XSS');</script></svg>";
   var options = new SvgSanitiserOptions
   {
       AddNamespace = false, // Adds the default SVG namespace (http://www.w3.org/2000/svg) if missing.
       IndentOutput = true, // Indents the output SVG using XmlTextWriter's Formatting.Indented
       RemoveComments = true // Removes comments from the SVG content.
   };
   var safeSvg = SvgSanitiser.Sanitise(svgContent, options);
   Console.WriteLine(safeSvg); // <svg xmlns="http://www.w3.org/2000/svg"><!-- Comment --></svg>
   ```

## Licence

This project is licensed under the BSD 3-Clause License. See the [LICENCE](LICENCE) file for details.
