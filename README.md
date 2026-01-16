<img width="560" height="90" alt="image" src="https://github.com/user-attachments/assets/f330d87a-5580-43b0-bb58-ec323a2841d6" />

GameArchiver is a manifest-driven Windows archiving and installer framework designed to preserve DRM-free PC games. It verifies archive integrity, supports offline installers, and provides clean, reproducible installs while keeping system and registry impact minimal.

---
>[!CAUTION]
> ⚠️ **Early Development Warning**  
> GameArchiver is still in **early development** and not yet production-ready.  
>  
> Expect bugs, missing features, and frequent changes.

> [!IMPORTANT]
> ⚠️ **Legal Use & Anti-Piracy Notice**  
> GameArchiver is intended **only** for archiving, verifying, and reinstalling **legally owned, DRM-free games**.  
>  
> This tool does **not** bypass DRM, remove copy protection, or enable piracy.  
> You are responsible for ensuring that any game files you use with GameArchiver were obtained legally and in compliance with the game’s license and applicable laws.

---
## Features

- Manifest-driven game preservation (`*.ga.json`)
- SHA-256 integrity verification before install
- Supports:
  - Pre-installed game archives
  - GOG-style offline installers
- Optional offline manuals & ToS preservation
- Minimal system impact (no services, no registry spam)
- Deterministic, reproducible installs
- No DRM removal or circumvention
  
## Non-Goals

GameArchiver intentionally does **not** aim to:

- Circumvent DRM or licensing systems
- Patch, crack, or modify executables
- Download games or content
- Replace official launchers or stores
- Manage game libraries or accounts

If a game is not DRM-free, it is **out of scope**.

---

## Instructions

To preserve your own **legally owned, DRM-free games**, you need to create a **manifest file** describing the game and its installer or archive.

Use the provided example manifest as a reference:  
👉 https://github.com/elNino0916/GameArchiver/blob/master/GameArchiver/examples/TheWitcher3WildHunt.ga.json

This example documents all available options and supported fields.

---

### Required File Layout

After creating your manifest, prepare the following directory structure:

```
[BASE DIRECTORY]
│
├─ GameArchiver files
├─ YourGame.7z
├─ YourGame.ga.json
├─ YourGame.7z.sha256
├─ Manual.pdf              (optional – copied to the installed game)
├─ TermsOfService.txt      (optional – used when no internet connection is available)
└─ requirements/           (optional)
   └─ Additional installers or dependencies (copied to the installed game)
```

---

### Notes

- **YourGame.7z**  
  Contains the archived game files or offline installer.

- **YourGame.7z.sha256**  
  Used to verify archive integrity before installation. Get the hash with the following command:
  
  ``` Get-FileHash ".\YourGame.7z" -Algorithm SHA256 ```

- **Manual & Terms of Service**  
  Optional but recommended for long-term preservation and offline use.

- **requirements/**  
  Place redistributables or additional setup files here (e.g. VC++ runtimes).  
  These files are copied to the installation directory for convenience.
---
## Typical Workflow

1. Archive your legally owned DRM-free game
2. Generate a SHA-256 hash
3. Create a `.ga.json` manifest
4. Place files into the base directory
5. Run GameArchiver
6. Verify → Extract → Install → Done
   
---

## Why a Manifest?

The manifest exists to:

- Separate **game data** from **installation logic**
- Enable long-term archival without platform lock-in
- Make installs reproducible years later
- Avoid hard-coded, game-specific installers

Everything GameArchiver does is driven by data, not assumptions.

---

## License

GameArchiver is released under the [MIT License](LICENSE).
