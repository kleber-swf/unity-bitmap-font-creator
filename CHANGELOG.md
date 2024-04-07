# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

-  Font size configuration

### Changed

-  Auto line spacing in place of guessing

### Fixed

-  Texture preview background for small textures
-  Increased precision on character size generation

---

## [1.3.0]

### Added

-  Texture preview window
-  Line spacing property
-  Guess line spacing button
-  Support to ascent and descent font properties
-  Ascent and descent properties inside texture preview
-  Custom character padding
-  Support to TextMesh

### Changed

-  Menu access moved to Window / Text / Bitmap Font Creator
-  Monospaced now centralizes the glyph
-  Material is now generated with Unlit/Transparent shader

### Fixed

-  Alignment madness
-  Settings were not being saved
-  Properly getting bearing values for characters
-  Save profile window position

---

## [1.2.1]

### Fixed

-  Build was failing because the asmdef wasn't configured to be only available in the Editor

---

## [1.2.0]

### Added

-  Guess rows and cols based on the texture
-  Showing error for Custom Character Properties when the given character is empty
-  Monospaced font to characters (under testing)
-  Character count

### Changed

-  Ignoring \n characters to make it easier to add them to the characters field

---

## [1.1.1]

### Fixed

-  Resources folder creation at startup

---

## [1.1.0]

### Added

-  All error checks for font creation
-  Warning when the material or font settings already exists
-  Local options popup
-  Warn before replacing current settings
-  Warn on replace profile preference
-  Rollback button
-  Setting readonly editor - to avoid changing settings directly in the file and creating inconsistencies

### Changed

-  Loading selected profile on open

### Removed

-  Unecessary example folder

### Fixed

-  Prefereces creation
-  Specific data for examples
-  Canceling settings override doesn't swap profile anymore
-  When changing to no profile, no setting changes, thus no dialog should be displayed
-  Error when checking settings for the first time

---

## [1.0.0]

Initial version

[1.3.0]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.3.0
[1.2.1]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.2.1
[1.2.0]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.2.0
[1.1.1]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.1.1
[1.1.0]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.1.0
[1.0.0]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.0.0
