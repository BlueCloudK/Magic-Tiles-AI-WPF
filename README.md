# Magic Tiles AI - WPF

🎮 **AI-Powered Rhythm Game cho Windows**

Magic Tiles AI là một rhythm game (trò chơi âm nhạc theo nhịp) được xây dựng bằng WPF, nơi **beat map được AI tự động tạo ra** dựa trên mô tả của bạn. Mỗi lần chơi sẽ là một trải nghiệm hoàn toàn mới!

## ✨ Features

- 🤖 **AI-Generated Beat Maps**: Sử dụng Google Gemini AI để tạo beat map dựa trên mô tả của bạn
- 🔐 **Google OAuth 2.0**: Đăng nhập an toàn với Google account, không cần nhập API key
- 🎵 **4-Lane Rhythm Game**: Gameplay giống Piano Tiles với 4 lanes
- 🎹 **Long Notes**: Hỗ trợ cả tap notes và long/sustain notes
- 🎨 **Modern UI**: Giao diện đẹp mắt với gradient backgrounds và animations
- ⚡ **Real-time Scoring**: Hệ thống điểm và combo theo thời gian thực
- 🏗️ **MVVM Architecture**: Code được tổ chức tốt theo mẫu MVVM
- 💾 **Token Persistence**: Lưu OAuth token, không cần đăng nhập lại mỗi lần

## 🛠️ Tech Stack

- **.NET 9.0** - Latest .NET framework
- **WPF** - Windows Presentation Foundation
- **MVVM Pattern** - Clean architecture với CommunityToolkit.Mvvm
- **Google Gemini AI** - AI để generate beat maps
- **NAudio** - Audio playback
- **Dependency Injection** - Microsoft.Extensions.DependencyInjection

## 📋 Prerequisites

- Windows 10/11
- .NET 9.0 SDK
- Visual Studio 2022 (hoặc VS Code với C# extension)
- Google Cloud Project với OAuth 2.0 credentials

## 🚀 Getting Started

### 1. Clone Repository

```bash
git clone https://github.com/BlueCloudK/Magic-Tiles-AI-WPF.git
cd Magic-Tiles-AI-WPF/BlueCloudK.WpfMusicTilesAI
```

### 2. Cấu hình Google OAuth 2.0

App sử dụng **Google OAuth 2.0** để authentication - giống như các app Google khác (Gmail, Drive, v.v.). Người dùng chỉ cần đăng nhập Google, không phải tự tạo API key.

#### **Bước 1: Tạo Google Cloud Project**

1. Truy cập [Google Cloud Console](https://console.cloud.google.com/)
2. Tạo project mới hoặc chọn project hiện có
3. Enable **Generative Language API**:
   - Vào **APIs & Services → Library**
   - Tìm "Generative Language API"
   - Click **Enable**

#### **Bước 2: Tạo OAuth 2.0 Credentials**

1. Vào **APIs & Services → Credentials**
2. Click **Create Credentials → OAuth client ID**
3. Nếu chưa configure OAuth consent screen:
   - Click **Configure Consent Screen**
   - Chọn **External** (cho testing) hoặc **Internal** (nếu có Google Workspace)
   - Điền app name (ví dụ: "Magic Tiles AI")
   - Điền user support email và developer contact
   - Click **Save and Continue**
4. Quay lại **Create OAuth client ID**:
   - Chọn **Desktop app** làm Application type
   - Đặt tên cho OAuth client (ví dụ: "Magic Tiles AI Desktop")
   - Click **Create**
5. Copy **Client ID** và **Client secret**

#### **Bước 3: Cấu hình App.config**

Mở file `BlueCloudK.WpfMusicTilesAI/App.config` và điền credentials:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <appSettings>
        <add key="GOOGLE_CLIENT_ID" value="YOUR_CLIENT_ID.apps.googleusercontent.com"/>
        <add key="GOOGLE_CLIENT_SECRET" value="YOUR_CLIENT_SECRET"/>
    </appSettings>
</configuration>
```

#### **Bước 4: Đăng nhập lần đầu**

1. Run app bằng `dotnet run` hoặc F5 trong Visual Studio
2. Bạn sẽ thấy màn hình **Sign in with Google**
3. Click vào button "Sign in with Google"
4. Browser sẽ mở ra, đăng nhập Google account của bạn
5. Authorize app để truy cập Generative Language API
6. Token sẽ được lưu tự động tại `%AppData%/MagicTilesAI/token.json`
7. Lần sau không cần đăng nhập lại!

### 3. Build và Run

```bash
cd BlueCloudK.WpfMusicTilesAI
dotnet restore
dotnet build
dotnet run
```

Hoặc mở solution trong Visual Studio và nhấn F5.

## 🎮 How to Play

1. **Nhập mô tả bài hát**: Mô tả loại nhạc bạn muốn chơi
   - Ví dụ: "fast electronic dance music"
   - Ví dụ: "calm piano ballad"
   - Ví dụ: "energetic rock song"

2. **Nhấn "Generate & Play"**: AI sẽ tạo beat map dựa trên mô tả của bạn

3. **Chơi game**:
   - Nhấn phím **D, F, J, K** tương ứng với 4 lanes
   - Nhấn đúng thời điểm khi tile chạm vào hit zone (đường đỏ)
   - Giữ phím cho long notes

4. **Scoring**:
   - Perfect hit: 150 điểm
   - Good hit: 100 điểm
   - Combo multiplier: Điểm nhân với (combo/10 + 1)

## 📁 Project Structure

```
BlueCloudK.WpfMusicTilesAI/
├── Models/              # Data models (GameState, Note, BeatMap, Song, AuthenticationState)
├── ViewModels/          # MVVM ViewModels (Main, Start, Game, Login)
├── Views/               # XAML views (LoginView, StartView, GameView)
├── Services/            # Services (GoogleAuthService, GeminiService, AudioService)
├── Helpers/             # Utility classes (Converters)
├── App.xaml             # Application resources
├── MainWindow.xaml      # Main window với state management
└── App.config           # Configuration file
```

## 🎯 Features Roadmap

- [ ] Upload file nhạc local thay vì chỉ mô tả
- [ ] Lưu và replay beat maps đã generate
- [ ] Leaderboard system
- [ ] Different difficulty levels
- [ ] More visual effects và animations
- [ ] Sound effects khi hit notes
- [ ] Settings screen (volume, speed, etc.)

## 🐛 Known Issues

- Audio playback chưa được implement đầy đủ (cần file audio)
- Game loop có thể lag trên máy yếu
- Một số edge cases với long notes chưa được handle

## 🤝 Contributing

Contributions are welcome! Feel free to:
- Report bugs
- Suggest new features
- Submit pull requests

## 📝 License

This project is open source và available under the [MIT License](LICENSE).

## 🙏 Credits

- **Google Gemini AI** - For AI beat map generation
- **Google OAuth 2.0** - For secure authentication
- **NAudio** - For audio playback
- **CommunityToolkit.MVVM** - For MVVM helpers
- **Google.Apis.Auth** - For OAuth implementation
- Lấy ý tưởng từ dự án **magic-tiles-ai** (React/TypeScript version)

## 📧 Contact

- GitHub: [@BlueCloudK](https://github.com/BlueCloudK)
- Project Link: [https://github.com/BlueCloudK/Magic-Tiles-AI-WPF](https://github.com/BlueCloudK/Magic-Tiles-AI-WPF)

---

Made with ❤️ and AI
