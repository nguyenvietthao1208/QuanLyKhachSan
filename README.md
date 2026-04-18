# 🏨 QuanLyKhachSan - Hướng Dẫn Cài Đặt

## 📁 Cấu trúc dự án (Mô hình 3 lớp)

```
QuanLyKhachSan/
├── DTO/
│   └── DTOs.cs                    ← Tất cả các lớp DTO
├── DAL/
│   ├── DBHelper.cs                ← Helper kết nối SQL
│   └── AllDAL.cs                  ← AccountDAL, RoomDAL, BookingDAL...
├── BLL/
│   └── AllBLL.cs                  ← AccountBLL, RoomBLL, BookingBLL...
├── GUI/
│   ├── Forms/
│   │   ├── frmLogin.cs            ← Form đăng nhập
│   │   ├── frmMain.cs             ← Form chính (sidebar)
│   │   └── frmDatPhong.cs         ← Form đặt phòng
│   └── Controls/
│       ├── ucSoDo.cs              ← UC Sơ đồ phòng
│       ├── ucKhachHang_HoaDon.cs  ← UC Khách hàng + Hóa đơn
│       ├── ucThietLap.cs          ← UC Thiết lập (5 tab)
│       └── ucBaoCao.cs            ← UC Báo cáo (Excel/PDF/In)
├── SQL/
│   └── CreateDatabase.sql         ← Script tạo database
├── App.config                     ← Cấu hình kết nối SQL
└── Program.cs                     ← Entry point
```

---

## 🚀 Hướng Dẫn Cài Đặt

### BƯỚC 1: Tạo Database

1. Mở **SQL Server Management Studio (SSMS)**
2. Kết nối đến SQL Server của bạn
3. Mở file `SQL/CreateDatabase.sql`
4. Nhấn **F5** để chạy script
5. Kiểm tra database `QuanLyKhachSan` đã được tạo

---

### BƯỚC 2: Tạo Project trong Visual Studio 2022

1. Mở VS 2022 → **Create a new project**
2. Chọn **Windows Forms App (.NET Framework)**
3. Đặt tên: `QuanLyKhachSan`
4. Framework: **.NET Framework 4.8**
5. Nhấn **Create**

---

### BƯỚC 3: Thêm References NuGet

Mở **Tools → NuGet Package Manager → Package Manager Console**, chạy:

```powershell
# Bắt buộc (kết nối SQL Server)
Install-Package System.Configuration.ConfigurationManager

# Tùy chọn (xuất Excel - cài nếu cần)
Install-Package EPPlus

# Tùy chọn (xuất PDF - cài nếu cần)  
Install-Package iTextSharp
```

---

### BƯỚC 4: Thêm System.Configuration

1. **Solution Explorer** → References → **Add Reference**
2. Tìm và tích chọn: **System.Configuration**
3. Nhấn **OK**

---

### BƯỚC 5: Copy code vào project

Tạo thư mục và copy file theo cấu trúc trên:

1. Tạo folder **DTO** → thêm `DTOs.cs`
2. Tạo folder **DAL** → thêm `DBHelper.cs`, `AllDAL.cs`  
3. Tạo folder **BLL** → thêm `AllBLL.cs`
4. Tạo folder **GUI/Forms** → thêm 3 file form
5. Tạo folder **GUI/Controls** → thêm 3 file UC
6. Thay `Program.cs` bằng file đã cung cấp
7. **Thay** `App.config` bằng file đã cung cấp

> **Cách thêm file vào VS:**
> Click phải vào folder → Add → New Item → Class → đặt tên file → paste code vào

---

### BƯỚC 6: Cấu hình App.config

Mở `App.config`, sửa `Data Source` cho phù hợp:

```xml
<!-- Nếu dùng SQL Server Express -->
connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=QuanLyKhachSan;Integrated Security=True"

<!-- Nếu dùng SQL Server thông thường -->
connectionString="Data Source=localhost;Initial Catalog=QuanLyKhachSan;Integrated Security=True"

<!-- Nếu tên máy cụ thể -->
connectionString="Data Source=TENMAYTINHCUABAN\SQLEXPRESS;..."

<!-- Dùng SQL Authentication (user/pass) -->
connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=QuanLyKhachSan;User ID=sa;Password=matkhau"
```

---

### BƯỚC 7: Thiết lập namespace

Đảm bảo tất cả các file đều có namespace đúng:
- `DTOs.cs` → `namespace QuanLyKhachSan.DTO`
- `DBHelper.cs`, `AllDAL.cs` → `namespace QuanLyKhachSan.DAL`
- `AllBLL.cs` → `namespace QuanLyKhachSan.BLL`
- Tất cả GUI → `namespace QuanLyKhachSan.GUI`
- `Program.cs` → `namespace QuanLyKhachSan`

---

### BƯỚC 8: Build và chạy

1. Nhấn **Ctrl+Shift+B** để Build
2. Sửa lỗi nếu có (thường là missing `using`)
3. Nhấn **F5** để chạy

---

## 🔑 Tài khoản mẫu

| Username | Password | Quyền |
|----------|----------|-------|
| admin    | admin123 | Admin (toàn quyền) |
| staff1   | staff123 | Staff (3 chức năng) |
| staff2   | staff123 | Staff (3 chức năng) |

---

## 💡 Tính năng theo từng màn hình

### 🏠 Sơ đồ phòng
- Hiển thị phòng phân theo **tầng**
- Phòng **xanh** = Trống → Click phải → Đặt phòng
- Phòng **đỏ** = Có người → Click trái → Xem dịch vụ
- Thêm/xóa dịch vụ cho phòng đang ở
- **Chuyển phòng** (swap trạng thái 2 phòng)
- **Thanh toán** (tính tổng tiền phòng + dịch vụ)

### 👥 Khách hàng
- Xem danh sách, tìm kiếm, thêm/sửa/xóa

### 🧾 Hóa đơn
- Xem lịch sử tất cả booking
- Lọc theo trạng thái (Đang ở / Đã trả)

### ⚙️ Thiết lập (Admin only)
- **Tab Phòng**: CRUD phòng
- **Tab Loại phòng**: CRUD loại phòng
- **Tab Dịch vụ**: CRUD dịch vụ
- **Tab Tài khoản**: CRUD tài khoản
- **Tab Khách hàng**: CRUD khách hàng

### 📊 Báo cáo (Admin only)
- Lọc theo khoảng thời gian
- Xem theo: Tổng hợp / Theo phòng / Theo khách hàng
- **Xuất Excel** (CSV) / **Xuất PDF** (cần cài iTextSharp)
- **In ấn** với Print Preview

---

## ❓ Lỗi thường gặp

| Lỗi | Nguyên nhân | Cách sửa |
|-----|-------------|----------|
| Cannot connect to SQL | SQL Server chưa chạy | Start SQL Server service |
| Database not found | Chưa chạy script SQL | Chạy CreateDatabase.sql |
| Cannot find type | Missing using | Thêm `using QuanLyKhachSan.XXX;` |
| PlaceholderText not found | .NET < 4.8 | Nâng lên .NET 4.8 hoặc xóa dòng đó |

---

## 📌 Ghi chú thêm

- **PlaceholderText** chỉ có từ **.NET Framework 4.8** trở lên
- Nếu dùng framework cũ hơn, xóa dòng `PlaceholderText = "..."` và thay bằng Text
- Để xuất **PDF**, cần cài iTextSharp và bỏ comment đoạn code trong `ucBaoCao.cs`
- Để xuất **Excel chuẩn .xlsx**, cần cài EPPlus và implement hàm xuất
