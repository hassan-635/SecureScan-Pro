<div align="center">

# 🛡️ SecureScan Pro
### File Integrity Checker — v2.0

**A production-ready, console-based File Integrity Checker built in C# (.NET 8)**  
*Demonstrating the full depth of Object-Oriented Programming through real-world security tooling*

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=csharp)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge&logo=docker)
![License](https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge)
![Build](https://img.shields.io/badge/Build-Passing-brightgreen?style=for-the-badge)

</div>

---

## 📖 Table of Contents

- [Overview](#-overview)
- [Key Features](#-key-features)
- [Project Architecture](#-project-architecture)
- [Feature Deep-Dive](#-feature-deep-dive)
  - [1. Baseline Snapshot](#1-baseline-snapshot)
  - [2. Quick Integrity Check](#2-quick-integrity-check)
  - [3. Deep Scan](#3-deep-scan)
  - [4. Scan History](#4-scan-history)
  - [5. Alert System](#5-alert-system)
  - [6. Report Export](#6-report-export)
  - [7. Settings & Configuration](#7-settings--configuration)
- [OOP Concepts Demonstrated](#-oop-concepts-demonstrated)
- [Class Hierarchy](#-class-hierarchy)
- [Getting Started](#-getting-started)
- [Docker Support](#-docker-support)
- [Git Commit History](#-git-commit-history)

---

## 🌟 Overview

**SecureScan Pro** is a fully-featured, interactive console application that monitors the integrity of files in any directory on your system. It works by taking a **cryptographic baseline snapshot** of your chosen folder and then comparing future scans against it — detecting any files that have been **modified**, **deleted**, or **newly added**.

Think of it as a lightweight tripwire system: once you establish a baseline, every subsequent scan will immediately flag any tampering or unexpected changes.

> **Why does this matter?**  
> File integrity checking is a core technique used in cybersecurity to detect malware infections, unauthorized file modifications, and accidental data corruption. Tools like AIDE, Tripwire, and Windows File Integrity Monitoring are all built on this concept.

---

## ✨ Key Features

| Feature | Description |
|---|---|
| 🔐 **Multi-Algorithm Hashing** | Compute file hashes using MD5, SHA-256, or SHA-512 |
| 📸 **Baseline Snapshots** | Save a cryptographic fingerprint of any directory |
| ⚡ **Quick Integrity Check** | Fast comparison against the saved baseline |
| 🔬 **Deep Scan** | Advanced scan with file permissions and metadata analysis |
| 📊 **Report Export** | Export reports as TXT, JSON, or CSV files |
| 🚨 **Real-Time Alerts** | Color-coded alerts for modified, deleted, and new files |
| 📜 **Scan History** | Keeps a rolling log of the last 5 scan reports |
| ⚙️ **Persistent Configuration** | Settings saved to `config.json` between sessions |
| 📝 **Thread-Safe Logging** | Singleton `AppLogger` writing to `app.log` |
| 🐳 **Docker Support** | Runs in any Docker container with a single command |

---

## 🏗️ Project Architecture

The project follows **SOLID principles** with a clean, layered folder structure:

```
FileIntegrityChecker/
│
├── 📁 Abstractions/          # Interfaces & Abstract Base Class
│   ├── IScannable.cs         # Contract: every scanner must implement Scan()
│   ├── IConfigurable.cs      # Contract: LoadConfig / SaveConfig
│   ├── IHashable.cs          # Contract: ComputeHash overloads
│   ├── IStorable.cs          # Contract: Save / Load to disk
│   ├── IReportable.cs        # Contract: GenerateReport / ExportReport
│   ├── IAlertable.cs         # Contract: RaiseAlert
│   └── ScannerBase.cs        # Abstract base — shared scanner infrastructure
│
├── 📁 Enums/                 # All application enumerations
│   ├── ScanType.cs           # Baseline | Quick | Deep
│   ├── AlertLevel.cs         # Info | Warning | Critical | Fatal
│   ├── FileStatus.cs         # Intact | Modified | Deleted | New | AccessDenied
│   ├── HashAlgorithmType.cs  # MD5 | SHA256 | SHA512
│   └── ReportFormat.cs       # TXT | JSON | CSV
│
├── 📁 Structs/               # Lightweight value types
│   ├── FileMetadata.cs       # File size, created/modified timestamps, permissions
│   └── HashResult.cs         # Hash value + algorithm + computed-at timestamp
│
├── 📁 Exceptions/            # Custom exception hierarchy
│   ├── FileIntegrityException.cs      # Base exception for this application
│   ├── SnapshotNotFoundException.cs   # Thrown when baseline file is missing
│   ├── HashMismatchException.cs       # Thrown on hash comparison failure
│   └── InvalidDirectoryException.cs   # Thrown for non-existent directories
│
├── 📁 Models/                # Core domain classes
│   ├── FileRecord.cs         # Single file entry with hash + status + metadata
│   ├── ScanReport.cs         # Full scan result (collection of FileRecords)
│   ├── AlertEntry.cs         # Single alert log entry (C# 10 record type)
│   └── ScanSummary.cs        # Computed summary stats (C# 10 record type)
│
├── 📁 Generics/              # Generic data structures
│   └── Repository<T>.cs      # Type-safe in-memory store for any class
│
├── 📁 Scanners/              # Scanner hierarchy (3-level inheritance)
│   ├── FileScanner.cs        # Level 2: Standard recursive file scanner
│   ├── DeepFileScanner.cs    # Level 3: File scanner + permissions & metadata
│   └── NetworkScanner.cs     # Level 2: Network path scanner (sibling branch)
│
├── 📁 Services/              # Core application services
│   ├── IntegrityMonitor.cs   # Orchestrates baseline saving, comparison, alerts
│   └── ReportGenerator.cs    # Builds and exports TXT/JSON/CSV reports
│
├── 📁 Storage/               # Persistence layer
│   └── AppConfig.cs          # Reads/writes config.json (implements IConfigurable + IStorable)
│
├── 📁 Utilities/             # Static helpers and extensions
│   ├── AppConstants.cs       # All magic strings/numbers in one place
│   ├── AppLogger.cs          # Thread-safe Singleton file logger
│   ├── ConsoleHelper.cs      # Static class for colored console output + progress bar
│   ├── HashGenerator.cs      # Multi-algorithm hash engine with retry logic
│   ├── SealedHashAlgorithm.cs# Sealed wrapper around .NET System.Security.Cryptography
│   ├── ScanValidator.cs      # Static validation helpers
│   ├── StringExtensions.cs   # Extension method: Truncate()
│   └── FileRecordExtensions.cs # Extension methods: HasIssue(), ToSummaryLine(), GetStatusColor()
│
├── 📁 UI/
│   └── ConsoleUI.cs          # Full ASCII interactive menu — wires everything together
│
└── Program.cs                # Entry point — manual DI, global error handler
```

---

## 🔍 Feature Deep-Dive

### 1. Baseline Snapshot

**Menu Option: `1. Take Baseline Snapshot`**

This is the foundation of the entire tool. When you take a baseline, the application:

1. **Recursively scans** every file in your chosen directory (including all subdirectories).
2. **Computes a cryptographic hash** for each file using your configured algorithm (MD5, SHA-256, or SHA-512).
3. **Records file metadata** — size, creation time, last modified time.
4. **Saves the snapshot** to `baseline_snapshot.json` on disk.

After a baseline exists, every future Quick Check or Deep Scan is compared against it — any deviation is immediately flagged.

```
Enter directory path: C:\MyImportantFiles
✅ Baseline saved — 127 files recorded.
```

---

### 2. Quick Integrity Check

**Menu Option: `2. Quick Integrity Check`**

Performs a fast hash-based comparison against the saved baseline. For each file:

- If the **hash matches** → `Intact` ✅
- If the **hash is different** → `Modified` ⚠️ — fires a `Critical` alert
- If the **file is gone** from disk → `Deleted` 🔴 — fires a `Fatal` alert  
- If a **new file exists** that wasn't in the baseline → `New` 🔵 — fires a `Warning` alert

Results are printed in real time with a color-coded progress bar and a final summary table.

```
  Total    : 127
  Modified : 2       ← shown in Yellow
  Deleted  : 1       ← shown in Red
  New      : 3       ← shown in Cyan
```

---

### 3. Deep Scan

**Menu Option: `3. Deep Scan (Permissions + Metadata)`**

Powered by the `DeepFileScanner` class, this goes beyond hashing. On top of everything a Quick Check does, it additionally captures:

| Extra Info Captured | Example Value |
|---|---|
| **File Attributes** | `ReadOnly`, `Hidden`, `System`, `Archive` |
| **Exact byte size** | `204,800 bytes` |
| **UTC creation time** | `2025-01-15 09:31:02` |
| **UTC last modified** | `2026-04-18 14:03:11` |

The Deep Scan uses **SHA-512 by default** (stronger algorithm) and is ideal for auditing system directories or sensitive folders where you need a full forensic picture.

---

### 4. Scan History

**Menu Option: `4. View Scan History`**

The `IntegrityMonitor` maintains a rolling in-memory history of the **last 5 scan reports** using the generic `Repository<ScanReport>`. Each history entry shows:

- Scan type (Baseline / Quick / Deep)
- Scanned directory path (truncated to 40 characters)
- Total file count
- Number of issues (modified + deleted)

```
  [Quick]  C:\MyImportantFiles              | Files: 127 | Issues: 3
  [Deep]   C:\Windows\System32              | Files: 4821 | Issues: 0
```

---

### 5. Alert System

**Menu Option: `5. Manage Alerts`**

Every integrity violation fires a **real-time alert** through a custom delegate/event system. The `IntegrityMonitor` exposes:

```csharp
public delegate void AlertHandler(string message, AlertLevel level);
public event AlertHandler? OnAlert;
```

Alerts are color-coded by severity in the console **and** persisted in an in-memory alert log viewable at any time:

| Alert Level | Color | Trigger |
|---|---|---|
| `Info` | Cyan | General informational events |
| `Warning` | Yellow | New (unexpected) file detected |
| `Critical` | Red | File hash mismatch (modified) |
| `Fatal` | Magenta | File deleted from disk |

---

### 6. Report Export

**Menu Option: `6. Export Report (TXT / JSON / CSV)`**

After any scan, you can export the full report in three formats. Reports are saved to the `Reports/` folder with a timestamped filename.

**TXT** — human-readable summary:
```
=== SCAN REPORT === 2026-04-18 19:15:30
Directory : C:\MyImportantFiles
Scan Type : Quick
Total Files: 127  Modified: 2  Deleted: 1  New: 3
------------------------------------------------------------
[Modified] C:\MyImportantFiles\config.xml | Hash: 3f4a8b1c...
```

**JSON** — structured, machine-readable (perfect for integration with other tools):
```json
{
  "ReportId": "a3f7...",
  "ScanType": "Quick",
  "TotalFiles": 127,
  "ModifiedCount": 2,
  "Files": [...]
}
```

**CSV** — import directly into Excel or any data analysis tool:
```
FilePath,HashValue,Status,Algorithm,ScannedAt
"C:\MyImportantFiles\config.xml","3f4a8b1c...","Modified","SHA256","2026-04-18T..."
```

---

### 7. Settings & Configuration

**Menu Option: `7. Configure Settings`**

All settings are persisted to `config.json` automatically. You can change:

| Setting | Options | Default |
|---|---|---|
| **Default Directory** | Any valid path | `My Documents` |
| **Hash Algorithm** | `MD5` / `SHA256` / `SHA512` | `SHA256` |
| **Alerts Enabled** | Toggle on/off | `true` |

The `AppConfig` class implements both `IConfigurable` and `IStorable`, cleanly separating the concern of what to store from how to store it.

---

## 🎓 OOP Concepts Demonstrated

This project was deliberately designed to showcase **every major OOP concept** in C#:

| Concept | Where It's Used |
|---|---|
| **Encapsulation** | Private fields with validated properties in `FileRecord`, `AppConfig`, `ScannerBase` |
| **Inheritance (3 levels)** | `ScannerBase` → `FileScanner` → `DeepFileScanner` |
| **Abstraction** | `ScannerBase` abstract class with `abstract Scan()` method |
| **Polymorphism (Runtime)** | `DeepFileScanner.Scan()` overrides `FileScanner.Scan()` which overrides `ScannerBase.Scan()` |
| **Polymorphism (Compile-time)** | `HashGenerator.ComputeHash()` has 3 overloads; `ReportGenerator.GenerateReport()` has 3 overloads |
| **Interfaces** | 6 interfaces: `IScannable`, `IConfigurable`, `IHashable`, `IStorable`, `IReportable`, `IAlertable` |
| **Multiple Interface Implementation** | `AppConfig` implements both `IConfigurable` and `IStorable` |
| **Delegates & Events** | `ScanProgressHandler` delegate, `OnProgress` event in `FileScanner`; `AlertHandler` delegate, `OnAlert` event in `IntegrityMonitor` |
| **Generics** | `Repository<T>` generic class; `PrintSummary<T>()` generic method |
| **Static Classes** | `AppConstants`, `ConsoleHelper`, `ScanValidator`, `StringExtensions` |
| **Static Constructors** | `HashGenerator` (initializes algorithm map), `Repository<T>` (logs type initialization) |
| **Sealed Class** | `AppLogger` (Singleton), `SealedHashAlgorithm` (prevents unsafe algorithm subclassing) |
| **Singleton Pattern** | `AppLogger.Instance` — one global logger, thread-safe via `Lazy<T>` |
| **Operator Overloading** | `FileRecord` overloads `==` and `!=` based on path + hash |
| **Constructor Chaining** | `FileRecord` default → parameterized → copy constructors chain via `: this()` |
| **Extension Methods** | `StringExtensions.Truncate()`, `FileRecordExtensions.HasIssue()`, `.ToSummaryLine()`, `.GetStatusColor()` |
| **C# 10 Record Types** | `AlertEntry` and `ScanSummary` — immutable, value-equality, computed properties |
| **Custom Exceptions** | `FileIntegrityException` → `SnapshotNotFoundException`, `HashMismatchException`, `InvalidDirectoryException` |
| **LINQ** | Used throughout `IntegrityMonitor`, `ConsoleUI`, `Repository<T>` for filtering and projection |
| **Pattern Matching** | `switch` expressions in `ConsoleUI`, `ConsoleHelper.PrintAlertColored()` |
| **Dependency Injection (Manual)** | `Program.cs` manually composes the object graph and injects dependencies into `ConsoleUI` |
| **Destructor / Finalizer** | `ScannerBase` and `FileScanner` define `~ClassName()` finalizers |
| **Structs** | `FileMetadata` and `HashResult` are value types (stack-allocated, immutable) |

---

## 🌳 Class Hierarchy

```
Object
└── ScannerBase (abstract, implements IScannable + IAlertable)
    ├── FileScanner (concrete – standard recursive scanner)
    │   └── DeepFileScanner (concrete – adds permissions + metadata)
    └── NetworkScanner (concrete – UNC/network path scanner)

FileIntegrityException (base custom exception)
├── SnapshotNotFoundException
├── HashMismatchException
└── InvalidDirectoryException

AppConfig (implements IConfigurable + IStorable)
IntegrityMonitor (implements IAlertable)
ReportGenerator (implements IReportable)
HashGenerator (implements IHashable)
AppLogger (Sealed Singleton)
SealedHashAlgorithm (Sealed)
Repository<T> (Generic)
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows / Linux / macOS
- Git

### Clone & Run

```bash
# Clone the repository
git clone https://github.com/your-username/SecureScan-Pro.git
cd "SecureScan Pro"

# Build the project
dotnet build FileIntegrityChecker/FileIntegrityChecker.csproj

# Run the application
dotnet run --project FileIntegrityChecker/FileIntegrityChecker.csproj
```

### Interactive Menu

```text
  +==============================================================+
  |                                                              |
  |       S E C U R E S C A N   P R O   .   v 2 . 0            |
  |               File Integrity Checker                         |
  |                                                              |
  +==============================================================+
  |  Algo: SHA256            Last Scan: --                       |
  |  Dir : C:\Example        None                                |
  +--------------------------------------------------------------+
  |  [1]  Take Baseline Snapshot                                 |
  |  [2]  Quick Integrity Check                                  |
  |  [3]  Deep Scan  (Permissions + Metadata)                    |
  |  [4]  View Scan History                                      |
  |  [5]  Manage Alerts                                          |
  |  [6]  View Saved Reports                                     |
  |  [7]  Export Report  (TXT / JSON / CSV)                      |
  |  [8]  Configure Settings                                     |
  |  [9]  Exit                                                   |
  +==============================================================+
```

### Typical Workflow

```
1  →  Enter a directory path  →  Baseline saved
2  →  Same directory          →  See what changed
6  →  Choose JSON             →  Report saved to Reports/
```

---

## 🐳 Docker Support

The application ships with a `Dockerfile` for containerized deployments.

```bash
# Build the Docker image
docker build -t securescan-pro .

# Run interactively (mount a local folder to scan)
docker run -it --rm -v "C:\MyFiles:/data" securescan-pro
```

The `.dockerignore` file ensures build artifacts (`bin/`, `obj/`) are excluded from the image.

---

## 📋 Configuration File

`config.json` is created automatically on first run (or first settings save):

```json
{
  "DefaultDirectory": "C:\\Users\\YourName\\Documents",
  "DefaultAlgorithm": "SHA256",
  "EnableAlerts": true,
  "ReportsFolder": "Reports"
}
```

---

## 📁 Generated Files

| File | Description |
|---|---|
| `baseline_snapshot.json` | The saved baseline — regenerated on every "Take Baseline" |
| `config.json` | Your persisted settings |
| `app.log` | Thread-safe log written by `AppLogger` (located in the `bin/` output folder) |
| `Reports/report_YYYYMMDD_HHmmss.txt` | Exported TXT report |
| `Reports/report_YYYYMMDD_HHmmss.json` | Exported JSON report |
| `Reports/report_YYYYMMDD_HHmmss.csv` | Exported CSV report |

---

## 👨‍💻 Author

Built as a comprehensive OOP showcase project in **C# 12 / .NET 8**.

> *Every class, method, and design decision in this project was made deliberately to demonstrate a specific OOP principle — making it an ideal reference for learning advanced C# architecture.*

---

<div align="center">

**🛡️ SecureScan Pro** — *Stay Secure. Detect Changes. Trust Nothing.*

</div>
