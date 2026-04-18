// =============================================
// DTO - Data Transfer Objects
// QuanLyKhachSan.DTO
// =============================================

using System;

namespace QuanLyKhachSan.DTO
{
    // DTO Tài khoản
    public class AccountDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string PassWord { get; set; }
        public string Role { get; set; }  // "admin" hoặc "staff"

        public bool IsAdmin => Role?.ToLower() == "admin";
    }

    // DTO Loại phòng
    public class RoomCategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public override string ToString() => Name;
    }

    // DTO Phòng
    public class RoomDTO
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public int Floor { get; set; }
        public int IdCategory { get; set; }
        public string CategoryName { get; set; }
        public decimal CategoryPrice { get; set; }
        public string Status { get; set; }  // "Trống" hoặc "Có người"

        public bool IsEmpty => Status == "Trống";
        public override string ToString() => $"Phòng {RoomNumber}";
    }

    // DTO Khách hàng
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string IdCard { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        public override string ToString() => FullName;
    }

    // DTO Đặt phòng
    public class BookingDTO
    {
        public int Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int IdRoom { get; set; }
        public string RoomNumber { get; set; }
        public int IdCustomer { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerIdCard { get; set; }
        public string Status { get; set; }  // "Đang ở" / "Đã trả"
        public decimal TotalPrice { get; set; }
        public decimal RoomAmount { get; set; }
        public decimal ServiceAmount { get; set; }
        public string Note { get; set; }

        public int Nights => Math.Max(1, (CheckOut - CheckIn).Days);
    }

    // DTO Dịch vụ
    public class ServiceDTO
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

        public override string ToString() => ServiceName;
    }

    // DTO Dịch vụ trong booking
    public class BookingServiceDTO
    {
        public int Id { get; set; }
        public int IdBooking { get; set; }
        public int IdService { get; set; }
        public string ServiceName { get; set; }
        public int Count { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime AddedAt { get; set; }

        public decimal Total => Count * UnitPrice;
    }
}
