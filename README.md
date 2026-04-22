<div align="center">

# 🛡️ SecureScan Pro
### File Integrity Checker — v2.0

**A production-ready, console-based File Integrity Checker built in C# (.NET 8)**  
*Demonstrating the full depth of Object-Oriented Programming through real-world security tooling*

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=csharp)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge&logo=docker)
![License](https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge)
![Contributions welcome](https://img.shields.io/badge/Contributions-welcome-orange.svg?style=for-the-badge)

</div>

---

## 📖 Table of Contents

- [👨‍🏫 Project Overview (For Evaluators & Teachers)](#-project-overview-for-evaluators--teachers)
- [✨ Key Features](#-key-features)
- [🎓 Object-Oriented Programming (OOP) in Action](#-object-oriented-programming-oop-in-action)
- [🏗️ Architecture & Class Hierarchy](#-architecture--class-hierarchy)
- [🚀 Getting Started (How to Run)](#-getting-started-how-to-run)
  - [Method 1: Standard .NET (Without Docker)](#method-1-standard-net-without-docker)
  - [Method 2: Using Docker (Containerized)](#method-2-using-docker-containerized)
- [💻 Interactive Workflow](#-interactive-workflow)
- [🤝 Contributing (You're Invited!)](#-contributing-youre-invited)

---

## 👨‍🏫 Project Overview (For Evaluators & Teachers)

**Welcome to SecureScan Pro!** If you are evaluating this project, here is a simple breakdown of what it does and why it was built.

**What is it?**
SecureScan Pro is a cybersecurity tool known as a **File Integrity Checker**. It acts like a security guard for your folders. You point it at a directory, and it takes a "snapshot" (a cryptographic baseline) of every file inside. Later, you can scan that folder again, and the application will instantly tell you if any files were:
- **Modified** (Tampered with by a hacker or malware)
- **Deleted** (Accidentally or maliciously removed)
- **Newly Added** (Unauthorized files dropped into the folder)

**Why was it built?**
The main goal of this project is to solve a real-world cybersecurity problem while heavily utilizing **Advanced Object-Oriented Programming (OOP)** concepts in C#. It proves that complex logic can be structured cleanly using interfaces, inheritance, polymorphism, and solid design principles.

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

---

## 🎓 Object-Oriented Programming (OOP) in Action

This project is a masterclass in OOP. Here is exactly how the 4 pillars (and more) are used to make the application scalable and robust:

### 1. Encapsulation (Data Hiding)
All classes (like `FileRecord` and `AppConfig`) hide their internal states using `private` fields and only expose safe `public` properties. This prevents external code from randomly altering sensitive data like a file's hash or the application's configuration.

### 2. Abstraction (Hiding Complexity)
The project heavily uses **Interfaces** (`IScannable`, `IHashable`, `IReportable`). For example, the core system doesn't need to know *how* a report is generated; it just calls `GenerateReport()` from the `IReportable` interface. We also use an `abstract class ScannerBase` that defines the blueprint for all scanners without revealing the complex low-level file reading mechanics.

### 3. Inheritance (Code Reusability)
We have a 3-level Scanner Hierarchy:
- `ScannerBase` (Abstract Base)
  - ↳ `FileScanner` (Standard recursive scanner)
      - ↳ `DeepFileScanner` (Inherits from FileScanner, adds metadata & permission checks)

### 4. Polymorphism (Many Forms)
- **Runtime Polymorphism:** When we call the `Scan()` method, the system dynamically decides whether to use the `FileScanner`'s basic scan or the `DeepFileScanner`'s advanced scan depending on the object instantiated.
- **Compile-Time Polymorphism:** The `HashGenerator` class has multiple `ComputeHash()` overloads (e.g., passing a string path vs. passing a file stream).

### Advanced C# Concepts Used
- **Generics:** A type-safe `Repository<T>` is used to store both Scan History and Alert Logs without duplicating code.
- **Delegates & Events:** The alert system uses `AlertHandler` delegates. When a file is modified, an event fires, and the UI responds instantly.
- **Singleton Pattern:** The `AppLogger` ensures only one thread-safe logging instance exists globally.

---

## 🏗️ Architecture & Class Hierarchy

The project is built on **SOLID principles**, keeping concerns cleanly separated into folders like `Models`, `Services`, `Scanners`, and `Storage`.

```text
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
```

---

## 🚀 Getting Started (How to Run)

We’ve made it incredibly easy to run SecureScan Pro, whether you want to run it natively using the .NET SDK or containerized using Docker.

### Method 1: Standard .NET (Without Docker)
Use this method if you have the .NET 8 SDK installed on your machine.

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/SecureScan-Pro.git
   cd "SecureScan Pro"
   ```
2. **Build the project:**
   ```bash
   dotnet build FileIntegrityChecker/FileIntegrityChecker.csproj
   ```
3. **Run the application:**
   ```bash
   dotnet run --project FileIntegrityChecker/FileIntegrityChecker.csproj
   ```

### Method 2: Using Docker (Containerized)
Use this method if you don't have .NET installed, or if you want an isolated environment. **Docker must be installed and running.**

1. **Build the Docker Image:**
   Open your terminal in the root project folder (where the `Dockerfile` is) and run:
   ```bash
   docker build -t securescan-pro .
   ```
2. **Run the Docker Container:**
   Run the interactive console application inside the container:
   ```bash
   docker run -it securescan-pro
   ```
   *Note: If you want to scan files on your host machine using Docker, you will need to mount a volume (e.g., `docker run -it -v C:\MyFolder:/scan-target securescan-pro`).*

---

## 💻 Interactive Workflow

When you run the app, you will be greeted with a stunning, interactive ASCII console interface.

1. **Take Baseline Snapshot:** Enter a directory. The app calculates the cryptographic hashes of all files and saves a baseline.
2. **Quick Integrity Check:** Scan the same directory again. The system will color-code files that are Intact (Green), Modified (Yellow/Red), Deleted, or New.
3. **Export Reports:** Found an issue? Export the results as `TXT`, `JSON`, or `CSV` to share with your security team.

---

## 🤝 Contributing (You're Invited!)

**We want YOU to contribute!**  
Whether you are a beginner looking to make your first open-source contribution, or an advanced C# developer wanting to add new security features, your help is welcome!

### How you can contribute:
- 🐛 **Find & Fix Bugs:** Notice an issue? Open an issue or submit a Pull Request.
- ✨ **Add New Features:** Want to add cloud-backup for reports? Or a new hashing algorithm? Go for it!
- 📚 **Improve Documentation:** Help us make this README even better.
- 🎨 **Enhance the UI:** Make the console ASCII art and colors even more beautiful.

### Steps to Contribute:
1. **Fork** this repository.
2. **Clone** your fork locally.
3. **Create a new branch** (`git checkout -b feature/MyAwesomeFeature`).
4. **Commit your changes** (`git commit -m "Add some AwesomeFeature"`).
5. **Push to the branch** (`git push origin feature/MyAwesomeFeature`).
6. **Open a Pull Request** and we will review it ASAP!

Let's build the best open-source console security tool together! 🚀

---

<div align="center">
**🛡️ SecureScan Pro** — *Stay Secure. Detect Changes. Trust Nothing.*
</div>
