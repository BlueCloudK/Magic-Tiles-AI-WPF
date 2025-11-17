# Cách Thêm Logo và Hình Ảnh

## Bước 1: Chuẩn bị hình ảnh

Chuẩn bị các file hình ảnh:
- **Logo chính**: `app-logo.png` (khuyến nghị: 256x256px, định dạng PNG với nền trong suốt)
- **Album placeholder**: `album-placeholder.png` (khuyến nghị: 300x300px)
- **Icon**: `app-icon.ico` (nhiều kích thước: 16x16, 32x32, 48x48, 256x256)

## Bước 2: Copy files vào thư mục Assets

Đặt các file vào đúng thư mục:
```
Assets/
├── Logos/
│   └── app-logo.png          <-- Logo chính của app
├── Images/
│   └── album-placeholder.png <-- Hình mặc định cho album
└── Icons/
    └── app-icon.ico          <-- Icon cho window
```

## Bước 3: Cấu hình Build Action trong Visual Studio

1. Mở Visual Studio
2. Trong Solution Explorer, tìm file ảnh vừa thêm
3. Click phải vào file → Properties
4. Đặt **Build Action** = **Resource** (để embed vào DLL)
   - Hoặc **Content** + **Copy to Output Directory** = **Copy if newer** (để copy riêng)

## Bước 4: Sử dụng trong XAML

### Thay logo trên StartView:

Mở file `Views/StartView.xaml`, tìm TextBlock "Magic Tiles AI" và thay bằng:

```xml
<!-- Thay thế TextBlock title bằng Image + Text -->
<StackPanel Orientation="Horizontal" HorizontalAlignment="Center" Margin="0,0,0,30">
    <Image Source="/Assets/Logos/app-logo.png"
           Width="64" Height="64"
           Margin="0,0,15,0"
           VerticalAlignment="Center"/>
    <TextBlock Text="Magic Tiles AI"
               FontSize="48"
               FontWeight="Bold"
               Foreground="White"
               VerticalAlignment="Center">
        <TextBlock.Effect>
            <DropShadowEffect Color="#e94560" BlurRadius="20" ShadowDepth="0"/>
        </TextBlock.Effect>
    </TextBlock>
</StackPanel>
```

### Thay album art placeholder trong LibraryView:

Mở file `Views/LibraryView.xaml`, tìm Border với TextBlock "♪" và thay bằng:

```xml
<!-- Thay thế music note bằng hình ảnh -->
<Border Grid.Column="0"
        Width="50"
        Height="50"
        CornerRadius="4"
        VerticalAlignment="Center">
    <Image Source="/Assets/Images/album-placeholder.png"
           Stretch="UniformToFill"/>
</Border>
```

### Thêm logo vào Window Icon:

Mở file `MainWindow.xaml`, thêm thuộc tính Icon:

```xml
<Window ...
        Icon="/Assets/Icons/app-icon.ico"
        ...>
```

## Bước 5: Test

1. Build project (Ctrl + Shift + B)
2. Chạy app (F5)
3. Kiểm tra logo hiển thị đúng

## Tips

- Sử dụng PNG cho logo (hỗ trợ trong suốt)
- Sử dụng ICO cho window icon (multi-size support)
- Giữ kích thước file < 500KB
- Có thể dùng online tools như [favicon.io](https://favicon.io) để tạo .ico từ .png

## Ví dụ XAML đầy đủ

Xem file `Assets/ExampleUsage.xaml` để có ví dụ đầy đủ về cách sử dụng assets.
