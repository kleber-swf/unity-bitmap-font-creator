# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

-  Preview panel zoom with UI widget
-  Preview panel zoom with ctrl + mouse wheel
-  Preview panel pan with mouse middle button
-  Feedback when a font is created successfully

### Changed

-  Preview panel now closes itself when the game starts avoiding some unnecessary console errors
-  Using proper cursors for pan and zoom

### Fixed

-  Character count icon

---

## [1.5.1]

### Fixed

-  It wasn't possible to create custom character properties in the first execution

---

## [1.5.0]

### Added

-  Default button

### Changed

-  Monospaced fonts now respects custom character properties
-  Keeping changes at runtime

---

## [1.4.0]

### Added

-  Font size configuration
-  Support to _space_ character based on smallest character width
-  Support to case insensitive fonts

### Changed

-  Auto line spacing in place of guessing
-  UI layout changes
-  Documentation updated

### Fixed

-  Texture preview background for small textures
-  Increased precision on character size generation
-  Saving a profile with the name of another was not selecting it

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

[1.5.1]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.5.1
[1.5.0]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.5.0
[1.4.0]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.4.0
[1.3.0]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.3.0
[1.2.1]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.2.1
[1.2.0]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.2.0
[1.1.1]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.1.1
[1.1.0]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.1.0
[1.0.0]: https://github.com/kleber-swf/unity-bitmap-font-creator/releases/tag/1.0.0
