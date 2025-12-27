![image](https://github.com/halysondev/PckKeyChanger/assets/82459544/bbda891f-19df-4669-b21a-e04947b4a2cf)

## 📋 Repository Overview

**PckKeyChanger** is a C# console application designed to change encryption keys in PCK (Perfect World Archive) files. These are archive files used by Perfect World game engine (Angelica Engine).

### Key Information: 
- **Language**: C# 
- **Created**: June 26, 2024
- **Last Updated**: October 13, 2025
- **Stars**: 3 | **Forks**: 6
- **Purpose**: Simple software for changing encryption keys in . pck archive files

---

## 🏗️ Architecture & Structure

### Main Components: 

#### 1. **Program.cs** (Entry Point)
- Interactive console application
- Prompts user for: 
  - Old encryption keys (KEY_1 and KEY_2) - defaults to Perfect World keys
  - Folder path containing .pck files
  - New encryption keys
  - Compression level (1-9)
- Processes multiple .pck files in parallel using `Parallel.ForEach`
- Real-time progress tracking with console cursor positioning
- Default keys:  `-1466731422` and `-240896429`

#### 2. **Core/ArchiveEngine/** (Archive Management)

**ArchiveManager.cs** - Main archive handler:
- Supports two PCK formats:  **V2** and **V3**
- Key methods:
  - `ReadFileTable()` - Reads file entries from archive
  - `ChangeKeys()` - Changes encryption keys (creates temporary `.defrag` file)
  - `SaveFileTableChangedKeys()` - Saves file table with new keys
  - `GetFile()` - Extracts individual files
  - `Defrag()` - Optimizes archive structure

**ArchiveStream.cs** - Low-level file I/O:
- Handles multi-part archives (. pck, .pkx, .pkx1)
- Maximum sizes: 
  - PCK: 2,147,483,392 bytes (~2GB)
  - PKX: 4,294,966,784 bytes (~4GB)
- Buffer size: 16MB for performance
- Supports reading/writing across multiple files seamlessly

**ArchiveKey.cs** - Encryption key container:
```csharp
- KEY_1, KEY_2: Main encryption keys
- ASIG_1, ASIG_2: Archive signatures
- FSIG_1, FSIG_2: File signatures
```

**ArchiveEntryV2.cs / ArchiveEntryV3.cs** - File entry structures:
- Path (260 bytes, GBK encoding)
- Offset (file location in archive)
- Size (uncompressed size)
- CSize (compressed size)

#### 3. **Core/** (Utilities)

**Zlib.cs** - Compression handler:
- Uses zlib library for compression/decompression
- Adaptive compression (only compresses if result is smaller)

**Events.cs** - Event delegates:
- Progress tracking
- Data loading notifications

#### 4. **Interfaces/**

**IArchiveEntry.cs** - Entry abstraction
**IArchiveManager.cs** - Manager contract

#### 5. **Helper Classes**

**StaticConverter.cs** - String/encoding utilities: 
- GBK encoding conversion (Chinese character set)
- String manipulation helpers

**Utils.cs** - General utilities:
- Random string generation
- File validation

---

## 🔑 How Key Changing Works

1. **Detect Archive Version**:  Reads version number from last 4 bytes
2. **Read File Table**: 
   - Decrypts file table using old keys
   - XOR operations with KEY_1 and KEY_2
3. **Extract All Files**: 
   - Decompresses each file
   - Stores in memory
4. **Recompress with New Settings**:
   - Uses specified compression level
   - Writes to temporary `.defrag` file
5. **Save with New Keys**:
   - Encrypts file table with new keys
   - XOR operations with new KEY_1 and KEY_2
6. **Replace Original**: Deletes old file, renames temp file

---

## 🔐 Perfect World Archive Format

### V2 Structure: 
```
[Header:  FSIG_1 + Size + FSIG_2]
[Compressed Files Data]
[File Table (encrypted with KEY_1/KEY_2)]
[Footer: ASIG_1 + Version + FileTableOffset (XOR KEY_1) + Signature + ASIG_2 + FileCount]
```

### V3 Structure:
Similar to V2 but uses 64-bit offsets instead of 32-bit

---

## 💡 Key Features

✅ **Parallel Processing** - Handles multiple PCK files simultaneously  
✅ **Multi-part Archives** - Supports .pck + .pkx + .pkx1 files  
✅ **Progress Tracking** - Real-time console updates for each file  
✅ **Compression Control** - User-defined compression levels (1-9)  
✅ **Default Keys** - Pre-configured Perfect World encryption keys  
✅ **Error Handling** - Try-catch blocks with detailed error messages  
✅ **Memory Efficient** - 16MB buffered streams  

---

## 🎯 Use Cases

- **Game Modding**: Re-encrypt game archives with custom keys
- **Security**:  Change default Perfect World encryption
- **Archive Management**: Recompress with different compression levels
- **Data Protection**: Prevent unauthorized access to game assets

---

## 📦 Dependencies

- **System.IO. Compression** - For compression operations
- **zlib library** (via packages.config) - Core compression engine
- **. NET Framework** - Standard C# libraries

---

This is a specialized tool for Perfect World game engine archive manipulation, focusing on encryption key modification while preserving archive integrity and file compression. 
