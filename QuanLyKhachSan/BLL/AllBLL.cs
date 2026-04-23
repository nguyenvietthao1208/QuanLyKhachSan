// =============================================
// BLL - Business Logic Layer
// QuanLyKhachSan.BLL
// =============================================

using QuanLyKhachSan.DAL;
using QuanLyKhachSan.DTO;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyKhachSan.BLL
{
    // =============================================
    // ACCOUNT BLL
    // =============================================
    public class AccountBLL
    {
        private readonly AccountDAL _dal = new AccountDAL();

        public AccountDTO Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new Exception("Vui lòng nhập tên đăng nhập và mật khẩu!");
            return _dal.Login(username, password);
        }

        public List<AccountDTO> GetAll() => _dal.GetAll();

        public (bool success, string message) Insert(AccountDTO acc)
        {
            if (string.IsNullOrWhiteSpace(acc.UserName))   return (false, "Tên đăng nhập không được để trống!");
            if (string.IsNullOrWhiteSpace(acc.DisplayName)) return (false, "Họ tên không được để trống!");
            if (string.IsNullOrWhiteSpace(acc.PassWord))   return (false, "Mật khẩu không được để trống!");
            if (acc.PassWord.Length < 6)                    return (false, "Mật khẩu phải từ 6 ký tự trở lên!");
            if (_dal.IsUserNameExists(acc.UserName))        return (false, "Tên đăng nhập đã tồn tại!");

            return _dal.Insert(acc) ? (true, "Thêm tài khoản thành công!") : (false, "Thêm thất bại!");
        }

        public (bool success, string message) Update(AccountDTO acc)
        {
            if (string.IsNullOrWhiteSpace(acc.DisplayName)) return (false, "Họ tên không được để trống!");
            if (string.IsNullOrWhiteSpace(acc.PassWord))   return (false, "Mật khẩu không được để trống!");
            if (_dal.IsUserNameExists(acc.UserName, acc.Id)) return (false, "Tên đăng nhập đã tồn tại!");

            return _dal.Update(acc) ? (true, "Cập nhật thành công!") : (false, "Cập nhật thất bại!");
        }

        public (bool success, string message) Delete(int id, int currentUserId)
        {
            if (id == currentUserId) return (false, "Không thể xóa tài khoản đang đăng nhập!");
            return _dal.Delete(id) ? (true, "Xóa thành công!") : (false, "Xóa thất bại!");
        }
    }

    // =============================================
    // ROOM CATEGORY BLL
    // =============================================
    public class RoomCategoryBLL
    {
        private readonly RoomCategoryDAL _dal = new RoomCategoryDAL();

        public List<RoomCategoryDTO> GetAll() => _dal.GetAll();

        public (bool success, string message) Insert(RoomCategoryDTO cat)
        {
            if (string.IsNullOrWhiteSpace(cat.Name)) return (false, "Tên loại phòng không được để trống!");
            if (cat.Price < 0) return (false, "Giá không hợp lệ!");
            return _dal.Insert(cat) ? (true, "Thêm thành công!") : (false, "Thêm thất bại!");
        }

        public (bool success, string message) Update(RoomCategoryDTO cat)
        {
            if (string.IsNullOrWhiteSpace(cat.Name)) return (false, "Tên loại phòng không được để trống!");
            if (cat.Price < 0) return (false, "Giá không hợp lệ!");
            return _dal.Update(cat) ? (true, "Cập nhật thành công!") : (false, "Cập nhật thất bại!");
        }

        public (bool success, string message) Delete(int id)
        {
            return _dal.Delete(id) ? (true, "Xóa thành công!") : (false, "Xóa thất bại! Có thể có phòng đang sử dụng loại này.");
        }
    }

    // =============================================
    // ROOM BLL
    // =============================================
    public class RoomBLL
    {
        private readonly RoomDAL _dal = new RoomDAL();

        public List<RoomDTO> GetAll() => _dal.GetAll();
        public List<RoomDTO> GetByFloor(int floor) => _dal.GetAll().Where(r => r.Floor == floor).ToList();
        public List<int> GetDistinctFloors() => _dal.GetDistinctFloors();

        public (bool success, string message) Insert(RoomDTO room)
        {
            if (string.IsNullOrWhiteSpace(room.RoomNumber)) return (false, "Số phòng không được để trống!");
            if (room.Floor < 1) return (false, "Số tầng không hợp lệ!");
            if (_dal.IsRoomNumberExists(room.RoomNumber)) return (false, "Số phòng đã tồn tại!");
            return _dal.Insert(room) ? (true, "Thêm phòng thành công!") : (false, "Thêm thất bại!");
        }

        public (bool success, string message) Update(RoomDTO room)
        {
            if (string.IsNullOrWhiteSpace(room.RoomNumber)) return (false, "Số phòng không được để trống!");
            if (_dal.IsRoomNumberExists(room.RoomNumber, room.Id)) return (false, "Số phòng đã tồn tại!");
            return _dal.Update(room) ? (true, "Cập nhật thành công!") : (false, "Cập nhật thất bại!");
        }

        public (bool success, string message) Delete(int id)
        {
            return _dal.Delete(id) ? (true, "Xóa thành công!") : (false, "Xóa thất bại! Phòng có thể đang được sử dụng.");
        }

        public bool UpdateStatus(int roomId, string status) => _dal.UpdateStatus(roomId, status);
    }

    // =============================================
    // CUSTOMER BLL
    // =============================================
    public class CustomerBLL
    {
        private readonly CustomerDAL _dal = new CustomerDAL();

        public List<CustomerDTO> GetAll() => _dal.GetAll();
        public CustomerDTO GetById(int id) => _dal.GetById(id);
        public List<CustomerDTO> Search(string kw) => _dal.Search(kw);

        public (bool success, string message, int newId) Insert(CustomerDTO cust)
        {
            if (string.IsNullOrWhiteSpace(cust.FullName)) return (false, "Họ tên không được để trống!", 0);
            if (string.IsNullOrWhiteSpace(cust.IdCard))   return (false, "CCCD không được để trống!", 0);
            if (string.IsNullOrWhiteSpace(cust.Phone))    return (false, "Số điện thoại không được để trống!", 0);

            int id = _dal.Insert(cust);
            return id > 0 ? (true, "Thêm thành công!", id) : (false, "Thêm thất bại!", 0);
        }

        public (bool success, string message) Update(CustomerDTO cust)
        {
            if (string.IsNullOrWhiteSpace(cust.FullName)) return (false, "Họ tên không được để trống!");
            if (string.IsNullOrWhiteSpace(cust.IdCard))   return (false, "CCCD không được để trống!");
            if (string.IsNullOrWhiteSpace(cust.Phone))    return (false, "Số điện thoại không được để trống!");
            return _dal.Update(cust) ? (true, "Cập nhật thành công!") : (false, "Cập nhật thất bại!");
        }

        public (bool success, string message) Delete(int id)
        {
            return _dal.Delete(id) ? (true, "Xóa thành công!") : (false, "Xóa thất bại! Khách hàng có thể đang có đặt phòng.");
        }
    }

    // =============================================
    // BOOKING BLL
    // =============================================
    public class BookingBLL
    {
        private readonly BookingDAL     _bookingDal = new BookingDAL();
        private readonly CustomerDAL    _custDal    = new CustomerDAL();
        private readonly RoomDAL        _roomDal    = new RoomDAL();
        private readonly BookingServiceDAL _bsDal   = new BookingServiceDAL();

        public List<BookingDTO> GetAll() => _bookingDal.GetAll();
        public BookingDTO GetActiveByRoomId(int roomId) => _bookingDal.GetActiveByRoomId(roomId);

        /// <summary>Đặt phòng: tạo khách hàng nếu mới, tạo booking, đổi trạng thái phòng</summary>
        public (bool success, string message) BookRoom(CustomerDTO customer, BookingDTO booking, bool isNewCustomer)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(customer.FullName)) return (false, "Họ tên khách không được để trống!");
            if (string.IsNullOrWhiteSpace(customer.IdCard)) return (false, "CCCD không được để trống!");
            if (string.IsNullOrWhiteSpace(customer.Phone)) return (false, "Số điện thoại không được để trống!");
            if (booking.CheckOut <= booking.CheckIn) return (false, "Ngày trả phòng phải sau ngày nhận phòng!");

            int customerId;

            if (isNewCustomer)
            {
                // Tìm khách hàng có trùng CCCD, SĐT hoặc Email
                var allCustomers = _custDal.GetAll();
                var matchIdCard = allCustomers.Find(c => c.IdCard == customer.IdCard);
                var matchPhone = allCustomers.Find(c => c.Phone == customer.Phone);
                var matchEmail = !string.IsNullOrWhiteSpace(customer.Email)
                                   ? allCustomers.Find(c => !string.IsNullOrWhiteSpace(c.Email)
                                                          && c.Email == customer.Email)
                                   : null;

                // Kiểm tra trùng cả 3 (cùng 1 người)
                bool trungCa3 = matchIdCard != null
                             && matchPhone != null
                             && matchEmail != null
                             && matchIdCard.Id == matchPhone.Id
                             && matchPhone.Id == matchEmail.Id;

                if (trungCa3)
                {
                    return (false, $"Khách hàng đã tồn tại trong hệ thống!\n" +
                                   $"Tên: {matchIdCard.FullName}\n" +
                                   $"CCCD: {matchIdCard.IdCard}\n" +
                                   $"SĐT: {matchIdCard.Phone}");
                }

                // Kiểm tra trùng 1 hoặc 2 thông tin
                var trungList = new System.Text.StringBuilder();
                CustomerDTO existingCustomer = null;

                if (matchIdCard != null)
                {
                    existingCustomer = matchIdCard;
                    trungList.AppendLine($"• CCCD {customer.IdCard} trùng với khách: {matchIdCard.FullName}");
                }
                if (matchPhone != null && (existingCustomer == null || matchPhone.Id == existingCustomer.Id))
                {
                    existingCustomer = matchPhone;
                    trungList.AppendLine($"• SĐT {customer.Phone} trùng với khách: {matchPhone.FullName}");
                }
                if (matchEmail != null && (existingCustomer == null || matchEmail.Id == existingCustomer.Id))
                {
                    existingCustomer = matchEmail;
                    trungList.AppendLine($"• Email {customer.Email} trùng với khách: {matchEmail.FullName}");
                }

                if (trungList.Length > 0 && existingCustomer != null)
                {
                    // Hiển thị thông báo hỏi có muốn cập nhật không
                    string msg = $"Phát hiện thông tin trùng:\n{trungList}\n" +
                                 $"Thông tin cũ:\n" +
                                 $"  Tên: {existingCustomer.FullName}\n" +
                                 $"  CCCD: {existingCustomer.IdCard}\n" +
                                 $"  SĐT: {existingCustomer.Phone}\n" +
                                 $"  Email: {existingCustomer.Email}\n\n" +
                                 $"Thông tin mới:\n" +
                                 $"  Tên: {customer.FullName}\n" +
                                 $"  CCCD: {customer.IdCard}\n" +
                                 $"  SĐT: {customer.Phone}\n" +
                                 $"  Email: {customer.Email}\n\n" +
                                 $"Bạn có muốn CẬP NHẬT thông tin mới thay thế thông tin cũ không?";

                    var answer = MessageBox.Show(msg, "Phát hiện thông tin trùng",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (answer == DialogResult.Yes)
                    {
                        // Cập nhật thông tin mới vào khách hàng cũ
                        customer.Id = existingCustomer.Id;
                        _custDal.Update(customer);
                        customerId = existingCustomer.Id;
                    }
                    else
                    {
                        // Hỏi thêm: có muốn tiếp tục đặt phòng không
                        var continueBooking = MessageBox.Show(
                            "Bạn có muốn tiếp tục đặt phòng với thông tin khách hàng hiện tại không?",
                            "Xác nhận đặt phòng",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (continueBooking == DialogResult.Yes)
                        {
                            // Dùng khách hàng cũ, không cập nhật
                            customerId = existingCustomer.Id;
                        }
                        else
                        {
                            // Hủy đặt phòng
                            return (false, "Đã hủy đặt phòng!");
                        }
                    }
                }
                else
                {
                    // Không trùng gì → thêm mới hoàn toàn
                    customerId = _custDal.Insert(customer);
                    if (customerId <= 0) return (false, "Không thể tạo thông tin khách hàng!");
                }
            }
            else
            {
                customerId = customer.Id;
            }

            booking.IdCustomer = customerId;

            // Tính tiền phòng theo số đêm
            int nights = Math.Max(1, (booking.CheckOut - booking.CheckIn).Days);
            var rooms = _roomDal.GetAll();
            var room = rooms.Find(r => r.Id == booking.IdRoom);
            booking.RoomAmount = nights * (room?.CategoryPrice ?? 0);

            int bookingId = _bookingDal.Insert(booking);
            if (bookingId <= 0) return (false, "Không thể tạo booking!");

            _roomDal.UpdateStatus(booking.IdRoom, "Có người");
            return (true, "Đặt phòng thành công!");
        }

        /// <summary>Thêm dịch vụ vào phòng đang ở</summary>
        public (bool success, string message) AddService(int bookingId, int serviceId, string serviceName, decimal price, int count)
        {
            if (count < 1) return (false, "Số lượng phải ít nhất là 1!");

            var bs = new BookingServiceDTO
            {
                IdBooking = bookingId,
                IdService = serviceId,
                ServiceName = serviceName,
                Count = count,
                UnitPrice = price
            };

            bool ok = _bsDal.Insert(bs);
            if (!ok) return (false, "Thêm dịch vụ thất bại!");

            // Cập nhật tổng tiền dịch vụ trong booking
            decimal svcTotal = _bsDal.GetTotalServiceAmount(bookingId);
            var booking = _bookingDal.GetActiveByRoomId(0); // sẽ dùng bookingId
            // Cập nhật qua bookingId trực tiếp
            UpdateBookingAmountByBookingId(bookingId, svcTotal);

            return (true, "Thêm dịch vụ thành công!");
        }

        private void UpdateBookingAmountByBookingId(int bookingId, decimal serviceAmount)
        {
            // Lấy roomAmount từ booking hiện tại
            var allBookings = _bookingDal.GetAll();
            var bk = allBookings.Find(b => b.Id == bookingId);
            if (bk != null)
                _bookingDal.UpdateAmounts(bookingId, bk.RoomAmount, serviceAmount);
        }

        public List<BookingServiceDTO> GetServices(int bookingId) => _bsDal.GetByBookingId(bookingId);

        public bool RemoveService(int bookingServiceId, int bookingId)
        {
            bool ok = _bsDal.Delete(bookingServiceId);
            if (ok)
            {
                decimal svcTotal = _bsDal.GetTotalServiceAmount(bookingId);
                UpdateBookingAmountByBookingId(bookingId, svcTotal);
            }
            return ok;
        }

        /// <summary>Thanh toán và trả phòng</summary>
        public (bool success, string message) Checkout(int bookingId, int roomId)
        {
            bool ok = _bookingDal.Checkout(bookingId);
            if (ok) _roomDal.UpdateStatus(roomId, "Trống");
            return ok ? (true, "Thanh toán thành công! Phòng đã được trả.") : (false, "Thanh toán thất bại!");
        }

        /// <summary>Chuyển phòng: swap trạng thái 2 phòng, cập nhật booking</summary>
        public (bool success, string message) TransferRoom(int bookingId, int currentRoomId, int targetRoomId, string targetStatus)
        {
            if (targetStatus == "Có người") return (false, "Phòng đích đã có khách!");
            if (currentRoomId == targetRoomId) return (false, "Vui lòng chọn phòng khác!");

            bool ok = _bookingDal.TransferRoom(bookingId, targetRoomId);
            if (ok)
            {
                _roomDal.UpdateStatus(currentRoomId, "Trống");
                _roomDal.UpdateStatus(targetRoomId, "Có người");

                // ── THÊM: Tính lại tiền phòng theo loại phòng mới ──
                var allBookings = _bookingDal.GetAll();
                var booking = allBookings.Find(b => b.Id == bookingId);
                if (booking != null)
                {
                    var rooms = _roomDal.GetAll();
                    var newRoom = rooms.Find(r => r.Id == targetRoomId);
                    if (newRoom != null)
                    {
                        int nights = Math.Max(1, (booking.CheckOut - booking.CheckIn).Days);
                        decimal newRoomAmount = nights * newRoom.CategoryPrice;
                        decimal svcAmount = _bsDal.GetTotalServiceAmount(bookingId);
                        _bookingDal.UpdateAmounts(bookingId, newRoomAmount, svcAmount);
                    }
                }
                // ────────────────────────────────────────────────────
            }
            return ok ? (true, "Chuyển phòng thành công!") : (false, "Chuyển phòng thất bại!");
        }
        public List<BookingServiceDTO> GetBookingServices(int bookingId)
    => _bsDal.GetByBookingId(bookingId);
    }

    // =============================================
    // SERVICE BLL
    // =============================================
    public class ServiceBLL
    {
        private readonly ServiceDAL _dal = new ServiceDAL();

        public List<ServiceDTO> GetAll() => _dal.GetAll();
        public List<ServiceDTO> GetActive() => _dal.GetActive();

        public (bool success, string message) Insert(ServiceDTO svc)
        {
            if (string.IsNullOrWhiteSpace(svc.ServiceName)) return (false, "Tên dịch vụ không được để trống!");
            if (svc.Price < 0) return (false, "Giá không hợp lệ!");
            return _dal.Insert(svc) ? (true, "Thêm thành công!") : (false, "Thêm thất bại!");
        }

        public (bool success, string message) Update(ServiceDTO svc)
        {
            if (string.IsNullOrWhiteSpace(svc.ServiceName)) return (false, "Tên dịch vụ không được để trống!");
            if (svc.Price < 0) return (false, "Giá không hợp lệ!");
            return _dal.Update(svc) ? (true, "Cập nhật thành công!") : (false, "Cập nhật thất bại!");
        }

        public (bool success, string message) Delete(int id)
        {
            return _dal.Delete(id) ? (true, "Xóa thành công!") : (false, "Xóa thất bại!");
        }
    }
}
