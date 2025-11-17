# Magic Tiles AI - WPF

🎮 **AI-Powered Rhythm Game cho Windows**

Magic Tiles AI là một rhythm game (trò chơi âm nhạc theo nhịp) được xây dựng bằng WPF. Import nhạc local (.mp3/.wav) từ máy bạn, app tự động **phân tích audio** và tạo beat map chính xác theo rhythm thật!

## ✨ Features

- 🎵 **Local Music Library**: Import file nhạc .mp3/.wav từ máy tính
- 🤖 **Auto Beat Map Generation**: Phân tích audio thực tế với Python/librosa
- 💾 **Beat Map Caching**: Lần đầu phân tích, lần sau load ngay từ cache
- 🎹 **4-Lane Rhythm Game**: Gameplay giống Piano Tiles với 4 lanes
- 🎨 **Spotify-like UI**: Giao diện library đẹp mắt, hiện đại
- ⚡ **Real-time Scoring**: Hệ thống điểm và combo theo thời gian thực
- 🔄 **Regenerate Option**: Tạo lại beat map nếu không hài lòng
- 📊 **Play Statistics**: Theo dõi số lần chơi cho mỗi bài
- 🏗️ **MVVM Architecture**: Code được tổ chức tốt theo mẫu MVVM

## 🛠️ Tech Stack

### C# / .NET
- **.NET 9.0** - Latest .NET framework
- **WPF** - Windows Presentation Foundation
- **MVVM Pattern** - Clean architecture với CommunityToolkit.Mvvm
- **NAudio** - Audio playback
- **Dependency Injection** - Microsoft.Extensions.DependencyInjection
- **Newtonsoft.Json** - JSON serialization

### Python (Audio Analysis)
- **Python 3.8+** - Runtime
- **librosa** - Audio analysis và beat detection
- **numpy** - Numerical computing

### AI (Optional)
- **Google Gemini API** - Fallback beat map generation nếu không dùng audio analysis

## 📋 Prerequisites

### Bắt buộc:
- Windows 10/11
- .NET 9.0 SDK
- Visual Studio 2022 (hoặc VS Code với C# extension)

### Khuyến nghị (cho Audio Analysis):
- Python 3.8 hoặc mới hơn
- pip package manager
- librosa và numpy

### Tùy chọn (nếu không dùng Audio Analysis):
- Google Gemini API key (free)

## 🚀 Getting Started

### 1. Clone Repository

```bash
git clone https://github.com/BlueCloudK/Magic-Tiles-AI-WPF.git
cd Magic-Tiles-AI-WPF
```

### 2. Setup Python (Khuyến nghị)

**Cách 1: Tự động (Windows)**
```bash
cd BeatAnalysis
install.bat
```

**Cách 2: Thủ công**
```bash
pip install librosa numpy
```

Chi tiết: Xem [AUDIO_ANALYSIS_GUIDE.md](AUDIO_ANALYSIS_GUIDE.md)

### 3. Cấu hình API Key (Tùy chọn)

Nếu không muốn cài Python, bạn có thể dùng Gemini API để generate beat map.

1. Lấy FREE API key từ: https://aistudio.google.com/app/apikey
2. Copy `App.config.example` thành `App.config`
3. Điền API key vào:

```xml
<add key="GEMINI_API_KEY" value="YOUR_API_KEY_HERE"/>
<add key="GEMINI_MODEL" value="gemini-2.0-flash-exp"/>
```

### 4. Build và Run

```bash
cd BlueCloudK.WpfMusicTilesAI
dotnet restore
dotnet build
dotnet run
```

Hoặc mở solution trong Visual Studio và nhấn F5.

## 🎮 How to Play

### Cách 1: Audio Analysis (Recommend)

1. **Add Music**: Click "Add Music" button
2. **Chọn file**: Chọn file .mp3 hoặc .wav từ máy bạn
3. **Click Play**:
   - Lần đầu: App phân tích audio → Tạo beat map (10-30s)
   - Lần sau: Load beat map từ cache (tức thì!)
4. **Chơi game**:
   - Nhấn phím **D, F, J, K** tương ứng với 4 lanes
   - Nhấn đúng thời điểm khi tile chạm vào hit zone (đường đỏ)
   - Giữ phím cho long notes

### Cách 2: AI Generate (Nếu không có Python)

1. Cần có Gemini API key trong App.config
2. App sẽ tự động fallback về AI generation
3. Beat map sẽ dựa trên title/artist (ít chính xác hơn)

### Scoring:
- Perfect hit: 150 điểm
- Good hit: 100 điểm
- Combo multiplier: Điểm nhân với (combo/10 + 1)

## 📁 Project Structure

```
Magic-Tiles-AI-WPF/
├── BeatAnalysis/                    # Python audio analysis
│   ├── analyze_audio.py            # Main analysis script
│   ├── requirements.txt            # Python dependencies
│   ├── install.bat                 # Auto installer (Windows)
│   └── README.md                   # Setup guide
│
├── BlueCloudK.WpfMusicTilesAI/
│   ├── Models/                     # Data models
│   │   ├── LocalSong.cs           # Local music file model
│   │   ├── MusicLibrary.cs        # Library container
│   │   ├── BeatMap.cs             # Beat map structure
│   │   └── ...
│   ├── ViewModels/                 # MVVM ViewModels
│   │   ├── LibraryViewModel.cs    # Music library logic
│   │   ├── MainViewModel.cs       # App state
│   │   └── GameViewModel.cs       # Game logic
│   ├── Views/                      # XAML views
│   │   ├── LibraryView.xaml       # Spotify-like library UI
│   │   ├── StartView.xaml         # Main screen
│   │   └── GameView.xaml          # Game screen
│   ├── Services/                   # Services
│   │   ├── AudioAnalysisService.cs     # Python integration
│   │   ├── BeatMapCacheService.cs      # Beat map caching
│   │   ├── MusicLibraryService.cs      # Library management
│   │   ├── GeminiService.cs            # AI fallback
│   │   └── AudioService.cs             # Audio playback
│   ├── Assets/                     # Images and logos
│   └── App.config.example          # Configuration template
│
├── AUDIO_ANALYSIS_GUIDE.md         # Audio analysis guide
├── TROUBLESHOOTING.md              # Common issues & fixes
└── README.md                       # This file
```

## 🎯 Features Roadmap

- [x] Local music library
- [x] Audio analysis với librosa
- [x] Beat map caching
- [x] Spotify-like UI
- [x] Play statistics
- [ ] Album art extraction
- [ ] Multiple difficulty modes
- [ ] Leaderboard system
- [ ] Visual effects improvements
- [ ] Sound effects khi hit notes
- [ ] Settings screen (volume, speed, etc.)

## 💾 Data Storage

Beat maps và library được lưu tại:

```
%LocalAppData%/MagicTilesAI/
├── BeatMaps/              # Cached beat maps (.json)
│   ├── {songId}.json
│   └── ...
└── library.json           # Music library
```

## 🐛 Troubleshooting

Xem [TROUBLESHOOTING.md](TROUBLESHOOTING.md) để biết cách fix các lỗi phổ biến:
- Rate limit (429)
- Python not found
- Audio analysis errors
- Beat map không khớp với nhạc

## 🤝 Contributing

Contributions are welcome! Feel free to:
- Report bugs
- Suggest new features
- Submit pull requests

## 📝 License

This project is open source và available under the [MIT License](LICENSE).

## 🙏 Credits

- **librosa** - Audio analysis và beat detection
- **Google Gemini AI** - Fallback beat map generation
- **NAudio** - Audio playback
- **CommunityToolkit.MVVM** - MVVM helpers
- **Newtonsoft.Json** - JSON serialization

## 📧 Contact

- GitHub: [@BlueCloudK](https://github.com/BlueCloudK)
- Project Link: [https://github.com/BlueCloudK/Magic-Tiles-AI-WPF](https://github.com/BlueCloudK/Magic-Tiles-AI-WPF)

---

Made with ❤️ and AI
