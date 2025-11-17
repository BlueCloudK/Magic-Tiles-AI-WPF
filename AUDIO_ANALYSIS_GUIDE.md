# Hướng Dẫn Sử Dụng Audio Analysis

App **Magic Tiles AI** giờ đây có thể **phân tích file nhạc thật** để tạo beat map thay vì chỉ dùng AI đoán!

## 🎵 Tính năng mới

### Trước đây:
- Import file .mp3 → AI generate beat map từ title/artist (không nghe nhạc)
- Beat map không khớp với nhạc thực tế

### Bây giờ:
- Import file .mp3/.wav → **Phân tích audio** → Tạo beat map từ rhythm thật
- Detect beats, tempo, và notes tự động
- Beat map khớp chính xác với nhạc!

## 📦 Cài đặt

### Bước 1: Cài Python

1. Tải Python 3.8+ từ: https://www.python.org/downloads/
2. **QUAN TRỌNG**: Khi cài, tick vào ✅ **"Add Python to PATH"**
3. Cài xong, mở Command Prompt, gõ: `python --version` để kiểm tra

### Bước 2: Cài thư viện

**Cách dễ nhất** - Chạy file tự động:
```
Double-click vào: BeatAnalysis/install.bat
```

**Hoặc cài thủ công**:
```bash
cd BeatAnalysis
pip install -r requirements.txt
```

Nếu lỗi, thử:
```bash
python -m pip install librosa numpy
```

## 🎮 Cách sử dụng

1. **Mở app Magic Tiles AI**
2. **Vào tab "📚 Your Library"**
3. **Click "Add Music"** → Chọn file .mp3 hoặc .wav
4. **Click "▶ Play"**

### Lần đầu play:
- App tự động phân tích audio (mất khoảng 10-30 giây)
- Tạo beat map và lưu vào cache
- Bắt đầu game với beat map mới tạo

### Lần sau play:
- Load beat map từ cache (tức thì)
- Không cần phân tích lại

### Regenerate:
- Click nút **"⟳"** để phân tích lại và tạo beat map mới

## ⚙️ Cách hoạt động

```
[File .mp3] → [Python Script] → [librosa phân tích] → [Beat Map JSON] → [App đọc & cache]
     ↓              ↓                   ↓                      ↓               ↓
 Local file    analyze_audio.py    Detect beats        Format chuẩn      Game play
```

## 🔄 Fallback Mode

Nếu **không cài Python**, app vẫn hoạt động:
- Tự động dùng **AI Generate mode** (Gemini API)
- Tạo beat map từ song title/artist
- Ít chính xác hơn nhưng không cần Python

## 🐛 Troubleshooting

### "Python not found"
→ Cài lại Python, nhớ tick "Add to PATH", restart Windows

### "No module named 'librosa'"
→ Chạy: `pip install librosa`

### "Microsoft Visual C++ is required"
→ Tải cài: https://aka.ms/vs/17/release/vc_redist.x64.exe

### Script chạy chậm lần đầu
→ Bình thường, librosa download models lần đầu. Lần sau nhanh hơn.

### Beat map không khớp với nhạc
→ Click nút "⟳" để regenerate, hoặc điều chỉnh code trong `analyze_audio.py`

## 📝 Chi tiết kỹ thuật

### Python Script: `BeatAnalysis/analyze_audio.py`

Phân tích audio với:
- `librosa.beat.beat_track()` - Phát hiện beats
- `librosa.onset.onset_detect()` - Phát hiện onset (note bắt đầu)
- `librosa.get_duration()` - Duration
- `librosa.feature.spectral_centroid()` - Ước lượng độ khó

### C# Service: `AudioAnalysisService.cs`

- Gọi Python script từ C#
- Parse JSON output thành `BeatMap` object
- Tự động fallback về AI nếu Python fail

### Priority:
1. **Audio Analysis** (nếu Python available)
2. **AI Generate** (nếu có Gemini API key)
3. **Error** (nếu cả 2 không có)

## 🎯 Tips

- File .wav cho kết quả tốt hơn .mp3 (không nén)
- Nếu beat map không khớp, thử adjust parameters trong Python script
- Có thể customize lane distribution trong `create_beat_map()` function
- Beat density affects difficulty: >2.5 = Hard, >1.5 = Medium, ≤1.5 = Easy

## 📚 Đọc thêm

- Python script source: `BeatAnalysis/analyze_audio.py`
- Setup guide: `BeatAnalysis/README.md`
- Service implementation: `Services/AudioAnalysisService.cs`
- librosa docs: https://librosa.org/doc/latest/index.html
