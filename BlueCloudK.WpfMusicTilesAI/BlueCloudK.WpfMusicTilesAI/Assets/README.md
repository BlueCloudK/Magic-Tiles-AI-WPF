# Assets Folder

This folder contains image assets for the Magic Tiles AI application.

## Folder Structure

- **Images/** - General images (backgrounds, album art, etc.)
- **Icons/** - Small icons and symbols
- **Logos/** - Application logos and branding

## Adding Images

1. Add your image files (.png, .jpg, .svg) to the appropriate folder
2. In Visual Studio, set the **Build Action** to **Resource** or **Content**
3. If using **Resource**, images are embedded in the DLL
4. If using **Content**, set **Copy to Output Directory** to **Copy if newer**

## Using Images in XAML

### For Resource (embedded in DLL):
```xml
<Image Source="/Assets/Logos/app-logo.png" />
```

### For Content files:
```xml
<Image Source="Assets/Logos/app-logo.png" />
```

### Example - Album Art Placeholder:
```xml
<Image Source="/Assets/Images/album-placeholder.png"
       Width="50" Height="50" />
```

## Recommended Images to Add

- **app-logo.png** (in Logos/) - Main application logo (recommended: 256x256px)
- **app-icon.ico** (in Icons/) - Application icon for taskbar/window
- **album-placeholder.png** (in Images/) - Default album art (recommended: 300x300px)
- **background.png** (in Images/) - Custom background image (optional)

## Image Guidelines

- Use PNG format for transparency
- Keep file sizes reasonable (< 1MB per image)
- Use consistent naming conventions (lowercase, hyphen-separated)
- Provide high-DPI versions when possible (@2x, @3x)
