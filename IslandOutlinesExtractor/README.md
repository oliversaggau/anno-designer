# Island Outlines Extractor Documentation

> [!IMPORTANT]  
> This documentation is **work in progress** and may be incomplete or subject to change.

## Overview

The **Island Outlines Extractor** is a tool that extracts island boundary information from Anno 1800 and Anno 117 map files (`.a7m`) and generates outline files (`.ad`) for use in AnnoDesigner. These outlines help players visualize buildable areas, coastlines and harbor areas when planning buildings.

**Key Files:**
- Input: `.a7m` map archive files (RDA format)
- Internal data: `gamedata.data` (FileDB document inside `.a7m`)
- Output: `.ad` layout files packaged in `outlines.zip`

## Table of Contents

1. [File Format Overview](#file-format-overview)
2. [Data Sources](#data-sources)
3. [Grid Types](#grid-types)
4. [Area ID System](#area-id-system)
5. [Sparse Block Encoding](#sparse-block-encoding)
6. [Bit Array Grids](#bit-array-grids)
7. [Color Coding](#color-coding)
8. [Output Format](#output-format)

## File Format Overview

Anno 1800 and Anno 117 map files have the extension `.a7m` and use the [RDA file format](https://github.com/lysanntranvouez/RDAExplorer/wiki/RDA-File-Format) containing a `gamedata.data` file inside the archive. The `gamedata.data` file is a [FileDB](https://github.com/anno-mods/FileDBReader/wiki) document which has been reverse-engineered by **Atria1234** and the Anno modding community.

**Key References:**
- FileDB format documentation: [FileDBReader Wiki](https://github.com/anno-mods/FileDBReader/wiki)
- Original `.a7m` research: [Atria1234/anno-scripts](https://github.com/Atria1234/anno-scripts)

## Data Sources

The tool reads several key sections from the `gamedata.data` [FileDB](https://github.com/anno-mods/FileDBReader/wiki) document:

### GameSessionManager Hierarchy

```
GameSessionManager
├── AreaIDs (sparse grid)
├── IrrigationManager
│   └── m_StaticTileGrid (sparse grid)
└── WorldManager
    ├── Water (bit array)
    └── RiverGrid (bit array)
```

### Data Structure

| Section | Type | Purpose |
|---------|------|---------|
| `AreaIDs` | Sparse Grid (UInt16) | Defines buildable and non-buildable areas |
| `m_StaticTileGrid` | Sparse Grid (byte) | Irrigation coverage areas |
| `Water` | Bit Array | Water tiles (ocean/lakes) |
| `RiverGrid` | Bit Array | River tiles |

**Grid Resolution**: All grids have the same width/height

## Grid Types

Anno 117 uses different grid storage formats optimized for different data types:

### 1. Sparse Grid (Block-Based)

Used for data with large uniform regions (e.g., AreaIDs, Irrigation).

**Structure:**

```xml
<AreaIDs>
  <SparseEnabled>01</SparseEnabled>
  <x>00020000</x>  <!-- Width: 512 (0x200) in little-endian -->
  <y>00020000</y>  <!-- Height: 512 (0x200) in little-endian -->
  <block>
    <mode>01</mode>  <!-- Mode 1: Initialize block dimensions -->
    <x>1000</x>      <!-- Block width: 16 tiles -->
    <y>1000</y>      <!-- Block height: 16 tiles -->
    <default>0000</default>  <!-- Default value for this block type -->
  </block>
  <block>
    <mode>02</mode>  <!-- Mode 2: Fill block with default value -->
    <x>3000</x>      <!-- Block X position: 48 -->
    <y>5000</y>      <!-- Block Y position: 80 -->
    <default>2001</default>  <!-- Fill value: 0x2001 (buildable) -->
  </block>
  <block>
    <!-- Mode missing: Explicit values for this block -->
    <x>4000</x>
    <y>6000</y>
    <values>00002001200120010000...</values>  <!-- 256 (16×16) UInt16 values -->
  </block>
</AreaIDs>
```

**Block Modes:**

| Mode | Purpose | Attributes |
|------|---------|------------|
| `1` | **Initialize** | `x`, `y` = block dimensions (applies to subsequent blocks) |
| `0` | **End** | Ends the current block context |
| `2` | **Fill Default** | `x`, `y` = position, `default` = fill value |
| `null` | **Explicit Values** | `x`, `y` = position, `values` = array of values |

### 2. Bit Array Grid

Used for boolean data (water, rivers)

**Structure:**

```xml
<Water>
  <x>00020000</x>  <!-- Width: 512 -->
  <y>00020000</y>  <!-- Height: 512 -->
  <bits>000...111</bits>  <!-- 262,144 bits (512×512) as byte array -->
</Water>
```

**Encoding**: Each bit represents one tile. `1` = true, `0` = false.

## Area ID System

The `AreaIDs` grid defines what type of area each tile belongs to:

### Area ID Values

| Value (Hex) | Value (Dec) | Meaning |
|-------------|-------------|---------|
| `0x0000` | 0 | Empty |
| `0x0001` | 1 | Non-buildable area |
| `0x2001` | 8193 | Buildable area |
| ? | ? | Other values? **Work in progress** |

## Sparse Block Encoding

The sparse block system efficiently stores large grids with repeating values.

### Parsing Algorithm

```csharp
private Grid2D<T> ParseBlocks<T>(Tag grid)
{
    int width = grid.Attribute("x").ToNumber<int>();
    int height = grid.Attribute("y").ToNumber<int>();
    Grid2D<T> result = new Grid2D<T>(width, height);
    
    UInt16? blockWidth = null;
    UInt16? blockHeight = null;
    
    foreach (Tag block in grid.Tags("block"))
    {
        byte? mode = block.Attribute("mode")?.ToNumber<byte>();
        
        if (mode == 1) // Initialize block dimensions
        {
            blockWidth = block.Attribute("x").ToNumber<UInt16>();
            blockHeight = block.Attribute("y").ToNumber<UInt16>();
        }
        else if (mode == 2) // Fill with default value
        {
            T value = block.Attribute("default").ToNumber<T>();
            Grid2D<T> section = new Grid2D<T>(blockWidth.Value, blockHeight.Value);
            section.Fill(value);
            
            UInt16 destX = block.Attribute("x").ToNumber<UInt16>();
            UInt16 destY = block.Attribute("y").ToNumber<UInt16>();
            result.Copy(section, destX, destY);
        }
        else if (mode == null) // Explicit values
        {
            T[] values = block.Attribute("values").ToNumbers<T>().ToArray();
            Grid2D<T> section = new Grid2D<T>(values, blockWidth.Value, blockHeight.Value);
            
            UInt16 destX = block.Attribute("x").ToNumber<UInt16>();
            UInt16 destY = block.Attribute("y").ToNumber<UInt16>();
            result.Copy(section, destX, destY);
        }
    }
    
    return result;
}
```

## Bit Array Grids

Water and river data use bit arrays for maximum space efficiency.

### Bit Array Format

```xml
<Water>
  <x>00020000</x>  <!-- 512 tiles wide -->
  <y>00020000</y>  <!-- 512 tiles tall -->
  <bits>FF00FF00...</bits>  <!-- 262,144 bits = 32,768 bytes -->
</Water>
```

**Memory**: 512×512 bits = 262,144 bits = 32 KB

### Parsing Algorithm

```csharp
Grid2D<bool> ParseBits(Tag grid, bool invert = false)
{
    int width = grid.Attribute("x").ToNumber<int>();
    int height = grid.Attribute("y").ToNumber<int>();
    bool[] values = new bool[width * height];
    
    BitArray bits = new BitArray(grid.Attribute("bits").Content);
    if (invert) bits = bits.Not();  // Invert e.g. for Water grid
    bits.CopyTo(values, 0);
    
    return new Grid2D<bool>(values, width, height);
}
```

**Inversion**: The `Water` grid is inverted because:
- Game stores: `1` = water (non-buildable), `0` = land + water in harbor area
- Extractor needs: `1` = land + water in harbor area, `0` = water (non-buildable)
- Solution: Invert all bits after parsing

## Color Coding

Each outline type is rendered in a distinct color for visual identification:

### Color Scheme

| Outline Type | Color | Purpose |
|--------------|-------|---------|
| **Island Boundary** | Black | Outer edge of buildable land |
| **Coastline** | Blue | Tiles where harbor buildings can be placed |
| **Harbor Zone** | Silver | Outer edge of buildable harbor area |
| **Irrigation Zone** | Olive | Edge of marsh land |

## Output Format

The extractor generates `.ad` (AnnoDesigner) layout files containing outline data.

### File Structure

```
outlines.zip
├── celtic_island_large_01.ad
├── celtic_island_medium_02.ad
├── roman_island_extralarge_01.ad
└── ...
```

## Notes

This project uses:
- [RDAExplorer](https://github.com/jakobharder/RDAExplorer) (custom .NET 8 build without external `zlib.dll`)
- [FileDBReader](https://github.com/anno-mods/FileDBReader)
