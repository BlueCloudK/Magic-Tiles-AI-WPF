# Audio Analysis with Python

Thư mục này chứa Python script để phân tích file audio và tự động tạo beat map.

## Yêu cầu

- Python 3.8 hoặc mới hơn
- pip (Python package manager)

## Cài đặt

### 1. Cài Python

**Windows:**
1. Tải Python từ: https://www.python.org/downloads/
2. Chạy installer, **QUAN TRỌNG**: Tick vào "Add Python to PATH"
3. Verify: Mở Command Prompt, gõ `python --version`

### 2. Cài thư viện cần thiết

Mở Command Prompt hoặc Terminal, chạy:

```bash
pip install librosa numpy
```

Nếu gặp lỗi, thử:
```bash
python -m pip install librosa numpy
```

## Kiểm tra

Test script hoạt động:

```bash
python analyze_audio.py <đường_dẫn_file_nhạc.mp3> output.json
```

Ví dụ:
```bash
python analyze_audio.py "C:\Music\song.mp3" beatmap.json
```

## Cách hoạt động

1. **App C# gọi Python script** khi bạn play một bài hát lần đầu
2. **Python phân tích audio** với librosa:
   - Detect beats (nhịp đập)
   - Detect onsets (note bắt đầu)
   - Tính tempo (BPM)
   - Estimate difficulty
3. **Tạo beat map JSON** với notes phân bố qua 4 lanes
4. **App C# đọc JSON** và cache vào disk

## Không muốn cài Python?

Nếu không muốn cài Python, app vẫn hoạt động với **AI Generate mode**:
- Vẫn có thể dùng tab "AI Generate" để tạo beat map từ description
- Hoặc import nhạc local và dùng AI generate (không phân tích audio thực)

## Troubleshooting

### Lỗi "Python not found"
- Cài lại Python, nhớ tick "Add to PATH"
- Khởi động lại Windows sau khi cài
- Thử gõ `py` thay vì `python`

### Lỗi "No module named 'librosa'"
```bash
pip install librosa
```

### Lỗi "Microsoft Visual C++ is required"
- Tải và cài: https://aka.ms/vs/17/release/vc_redist.x64.exe

### Script chạy chậm lần đầu
- Lần đầu librosa sẽ download models, sẽ nhanh hơn lần sau

## Chi tiết kỹ thuật

Script sử dụng:
- `librosa.beat.beat_track()` - Detect beats
- `librosa.onset.onset_detect()` - Detect note onsets
- `librosa.get_duration()` - Duration
- `librosa.feature.spectral_centroid()` - Difficulty estimation

Beat map format: Tương thích với `BeatMap` model của Magic Tiles AI.
