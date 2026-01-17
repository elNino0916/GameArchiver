[![.NET](https://github.com/elNino0916/GameArchiver/actions/workflows/dotnet.yml/badge.svg)](https://github.com/elNino0916/GameArchiver/actions/workflows/dotnet.yml)

<img width="560" height="90" alt="image" src="https://github.com/user-attachments/assets/f330d87a-5580-43b0-bb58-ec323a2841d6" />

**GameArchiver** is a **manifest-driven Windows archiving and installer framework** for preserving **legally owned, DRM-free PC games**.

It focuses on long-term preservation, integrity verification, and clean, reproducible installs — without relying on launchers, background services, or online dependencies.

GameArchiver verifies archive integrity, supports offline installers, and minimizes system and registry impact to keep installations deterministic and future-proof.

---

> [!CAUTION]
> ⚠️ **Early Development Warning**  
> GameArchiver is in **early development** and not yet production-ready.  
>  
> Expect bugs, missing features, breaking changes, and incomplete documentation.

> [!IMPORTANT]
> ⚖️ **Legal Use & Anti-Piracy Notice**  
> GameArchiver is intended **only** for archiving, verifying, and reinstalling **legally owned, DRM-free games**.  
>  
> This project does **not** bypass DRM, remove copy protection, or enable piracy in any form.  
> You are solely responsible for ensuring that any game files used with GameArchiver were obtained legally and comply with applicable licenses and laws.

---

## Features

- Manifest-driven game preservation (`*.ga.json`)
- SHA-256 archive integrity verification
- Supports:
  - Pre-installed game archives
  - GOG-style offline installers
- Optional offline preservation of manuals and Terms of Service
- Minimal system impact  
  - No background services  
  - No registry pollution
- Deterministic, reproducible installations
- Fully offline-capable
- No DRM removal or circumvention

---

## Non-Goals

GameArchiver explicitly does **not** aim to:

- Circumvent DRM or licensing systems
- Patch, crack, or modify executables
- Download games or content
- Replace official launchers or storefronts
- Manage game libraries, accounts, or updates

If a game is **not DRM-free**, it is **out of scope**.

---

## Getting Started

To preserve your **legally owned, DRM-free games**, you must create a **manifest file** describing the game and its installer or archive.

A complete example manifest is provided here:  
👉 https://github.com/elNino0916/GameArchiver/blob/master/GameArchiver/examples/example.ga.json

This example documents all supported fields and available options.

---

## Required File Layout

```
[BASE DIRECTORY]
│
├─ GameArchiver files
├─ YourGame.7z
├─ YourGame.ga.json
├─ YourGame.7z.sha256
├─ Manual.pdf              (optional)
├─ TermsOfService.txt      (optional)
└─ requirements/           (optional)
   └─ Additional installers or dependencies
```

---

## File Details

### YourGame.7z
Contains either:
- A pre-installed game archive, or
- A DRM-free offline installer

### YourGame.7z.sha256
Used to verify archive integrity before installation.

Generate the hash with:
```
Get-FileHash ".\YourGame.7z" -Algorithm SHA256
```

### Manual & Terms of Service
Optional but recommended for long-term preservation and offline use.  
Files are copied into the installed game directory.

### requirements/
Optional directory for redistributables or additional installers  
(e.g. VC++ runtimes). Files are copied but not executed automatically.

---

## Typical Workflow

1. Archive your legally owned DRM-free game
2. Generate a SHA-256 hash
3. Create a `.ga.json` manifest
4. Place all files into the base directory
5. Run GameArchiver
6. Verify → Extract → Install → Done

---

## Why a Manifest?

The manifest exists to:

- Separate **game data** from **installation logic**
- Enable long-term archival without platform lock-in
- Ensure installs remain reproducible years later
- Avoid hard-coded, game-specific installers

Everything GameArchiver does is driven by **data**, not assumptions.

---

## License

GameArchiver is released under the [MIT License](LICENSE).
