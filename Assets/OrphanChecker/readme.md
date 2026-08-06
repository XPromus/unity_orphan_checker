# Orphan Checker

Editor tool that scans your project for orphaned assets — assets that are no longer referenced by any scannable file (prefabs, scenes, materials, animators, etc.).

## Usage

Open the tool window via **Tools > Orphan Checker**.

- Click **Check** to scan the project. The window lists orphaned scripts and materials.
- Use the **Show** button next to a material to highlight it in the Project window.
- Use **Clear Selected** to reset all material toggles.

## Packaging

This folder is structured as a Unity Package Manager (UPM) package so it can be shared. It is currently developed in `Assets/` and uses asmdefs (`OrphanChecker.Runtime`, `OrphanChecker.Editor`), so no further code changes are required to package it.

To publish as a UPM package:

1. Copy the contents of this folder into `Packages/com.example.orphanchecker/` (or install it directly from a git URL pointing at the folder).
2. Keep the `package.json` — it carries the package identity and version.
3. Optionally bump the version in `package.json` and add a `CHANGELOG.md` / `LICENSE`.
