# Changelog
This is the changelog for Mag Perception! This changelog only features public release versions.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html)

## [1.3.0] - 8/9/2025
This update was a joint effort between [HAHOOS](https://new.thunderstore.io/c/bonelab/p/HAHOOS/) and [adamdev](https://new.thunderstore.io/c/bonelab/p/NotEnoughPhotons).

### Added
- Fusion support (Fusion is not a dependency!)
- Added "Text Opacity" setting
- Added "Text Color" setting
- Savable settings
- A fade out animation when a magazine/gun is dropped

### Updated
- UI will appear on all guns and/or magazines held, instead of it appearing on just one of them
- Made UI display Max Ammo, Reserve & Platform even when magazine is not present in a gun
- When UI gets created, it starts at the position of the fire / insert point
- UI will show when grabbing a grip on slide
- Pressing the "Menu" button when holding a magazine will cause the info to show if hidden
- Instead of throwing an error, because of BoneLib missing, it will send an error message in console "BoneLib is required for this mod to work"
- The info will now update every frame, not when something changed to ensure that info is up-to-date
- General cleanup of the code

### Changed
- MagPerception's manager class is no longer a singleton, but a static class

### Fixed
- Fixed a problem with FadeIn not working when FadeOut is in progress

## [1.2.2] - 8/4/2024

### Updated
- Updated the mod to work with Patch 5

## [1.2.0] - 5/8/2024

### Added
- Quest 2 support

## [1.1.1] - 3/1/2024

### Added
- An "Always Show" option

### Fixed
- Fixed an issue with the wrong gun showing up on the UI

### Removed
- Debug log text

## [1.1.0] - 2/25/2024

### Added
- A scale value to change the size of the UI
- An option to show the UI on either a gun or magazine

### Fixed
- Fixed an issue with the UI not showing on shotguns

## [1.0.1] - 2/25/2024

### Fixed
- Fixed broken images in the readme on Thunderstore

## [1.0.0] - 2/25/2024

Initial release!