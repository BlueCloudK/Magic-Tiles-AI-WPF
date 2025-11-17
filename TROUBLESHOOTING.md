# Hướng Dẫn Khắc Phục Lỗi

## 🚨 Lỗi phổ biến và cách fix

### 1. "Gemini API rate limit exceeded" (Lỗi 429)

**Triệu chứng:**
- Khi click "Generate & Play" app nhảy màn hình rồi về
- Hiển thị lỗi: "Gemini API rate limit exceeded"

**Nguyên nhân:**
- Bạn đã gọi Gemini API quá nhiều lần trong thời gian ngắn
- Free tier có giới hạn: 15 requests/phút, 1500 requests/ngày

**Giải pháp:**

✅ **Cách 1: Đợi vài phút**
```
Đợi 1-2 phút rồi thử lại
```

✅ **Cách 2: Sử dụng Audio Analysis thay vì AI Generate**
```
1. Cài Python + librosa (xem AUDIO_ANALYSIS_GUIDE.md)
2. Vào tab "📚 Your Library"
3. Add file nhạc .mp3/.wav
4. Click Play → Tự động phân tích audio, không cần gọi API!
```

✅ **Cách 3: Upgrade API Quota**
```
1. Vào https://aistudio.google.com/app/apikey
2. Request higher quota (nếu cần)
```

**Tips để tránh rate limit:**
- Không spam nút "Generate & Play"
- Đợi ít nhất 5 giây giữa mỗi lần generate
- Dùng Audio Analysis cho file local thay vì AI Generate

---

### 2. Màn hình nhảy rồi về lại StartView

**Triệu chứng:**
- Click "Generate & Play"
- Loading screen hiện ra
- Rồi tự động quay về màn hình Start
- Không có thông báo lỗi (hoặc không thấy)

**Nguyên nhân:**
- Beat map generation fail nhưng error không hiển thị
- Có thể do:
  - Rate limit (429)
  - Network error
  - Invalid API key
  - API response format thay đổi

**Giải pháp:**

✅ **Check error message:**
Sau khi fix, error message sẽ hiện màu đỏ bên dưới nút "Generate & Play". Đọc để biết lỗi gì.

✅ **Kiểm tra API Key:**
1. Mở `App.config`
2. Kiểm tra `GEMINI_API_KEY` có đúng không
3. Không để placeholder như "YOUR_API_KEY_HERE"
4. Copy lại API key từ https://aistudio.google.com/app/apikey

✅ **Check network:**
```
ping generativelanguage.googleapis.com
```

✅ **Xem debug log:**
- Chạy app từ Visual Studio
- Xem Output window (Debug)
- Tìm lỗi chi tiết

---

### 3. "Python not found" hoặc Audio Analysis không hoạt động

**Triệu chứng:**
- Import nhạc vào Library → Click Play
- Lỗi: "Python not found" hoặc "librosa not found"
- App fallback về AI Generate (hoặc lỗi nếu không có API key)

**Giải pháp:**

✅ **Cài Python:**
```
1. Tải: https://www.python.org/downloads/
2. QUAN TRỌNG: Tick "Add Python to PATH"
3. Restart Windows
4. Verify: Mở CMD, gõ: python --version
```

✅ **Cài librosa:**
```bash
cd BeatAnalysis
install.bat
```

Hoặc thủ công:
```bash
pip install librosa numpy
```

✅ **Nếu vẫn lỗi:**
```bash
# Thử với python3
python3 -m pip install librosa numpy

# Hoặc py launcher
py -m pip install librosa numpy
```

---

### 4. "Failed to generate beat map: Invalid response format"

**Nguyên nhân:**
- Gemini API response format thay đổi
- Hoặc API trả về lỗi thay vì JSON

**Giải pháp:**

✅ **Thử lại:**
- Đợi vài giây rồi generate lại
- AI có thể đôi khi trả về format khác

✅ **Simplify prompt:**
- Thay vì: "A complex symphonic orchestral piece with dramatic crescendos"
- Thử: "pop music" hoặc "dance music"

✅ **Dùng Audio Analysis:**
- Vào Library tab
- Import file nhạc
- Không cần AI generate!

---

### 5. Beat map không khớp với nhạc

**Triệu chứng:**
- Beat map được tạo nhưng không match với rhythm nhạc
- Notes xuất hiện lung tung

**Nguyên nhân:**
- AI Generate chỉ đoán based on description, không nghe nhạc thật

**Giải pháp:**

✅ **Dùng Audio Analysis:** (KHUYẾN NGHỊ)
```
1. Cài Python + librosa
2. Vào Library tab
3. Import file .mp3/.wav
4. Click Play → Tự động phân tích audio và detect beats thật!
```

✅ **Regenerate:**
- Click nút "⟳" để tạo lại
- Thử mô tả chi tiết hơn (nếu dùng AI)

---

### 6. App không mở được

**Kiểm tra:**

✅ **.NET 9.0 Runtime:**
```
Tải: https://dotnet.microsoft.com/download/dotnet/9.0
Cài .NET 9.0 Runtime (Desktop)
```

✅ **Visual C++ Redistributable:**
```
Tải: https://aka.ms/vs/17/release/vc_redist.x64.exe
Cài đặt
```

✅ **Windows 10/11:**
App yêu cầu Windows 10 hoặc mới hơn

---

### 7. Audio không play

**Nguyên nhân:**
- Game hiện tại chỉ có beat map, không play audio thật
- Beat map là sequence of notes để bạn chơi
- Nhạc thật sẽ được thêm trong update sau

**Workaround:**
- Play nhạc từ Spotify/YouTube song song
- Hoặc đợi update có audio playback

---

## 🔍 Debug Checklist

Nếu vẫn gặp lỗi, check theo thứ tự:

1. ✅ API Key configured đúng trong `App.config`
2. ✅ Internet connection hoạt động
3. ✅ Không bị rate limit (đợi 1-2 phút)
4. ✅ .NET 9.0 Runtime đã cài
5. ✅ Python + librosa installed (nếu dùng Audio Analysis)
6. ✅ Xem Output window trong Visual Studio để debug

## 📞 Báo lỗi

Nếu vẫn không fix được:

1. Chụp screenshot lỗi
2. Copy error message
3. Mở issue tại: https://github.com/BlueCloudK/Magic-Tiles-AI-WPF/issues
4. Ghi rõ:
   - Lỗi gì
   - Bước nào gặp lỗi
   - Screenshot/error text
   - Windows version
   - Đã cài Python chưa

## 💡 Tips

- **Dùng Audio Analysis cho file local** → Chính xác hơn AI Generate
- **Đợi ít nhất 5 giây** giữa mỗi lần generate để tránh rate limit
- **Cache beat maps** → Lần 2 play sẽ nhanh hơn
- **Regenerate nếu không hài lòng** → Click nút "⟳"
