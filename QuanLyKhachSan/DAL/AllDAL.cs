// =============================================
// DAL - Data Access Layer (tất cả các DAL)
// QuanLyKhachSan.DAL
// =============================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyKhachSan.DTO;

namespace QuanLyKhachSan.DAL
{
    // =============================================
    // ACCOUNT DAL
    // =============================================
    public class AccountDAL
    {
        public AccountDTO Login(string username, string password)
        {
            string sql = @"SELECT Id, UserName, DisplayName, PassWord, Role 
                           FROM Account 
                           WHERE UserName = @user AND PassWord = @pass";
            var dt = DBHelper.ExecuteQuery(sql,
                new SqlParameter("@user", username),
                new SqlParameter("@pass", password));

            if (dt.Rows.Count == 0) return null;
            return MapAccount(dt.Rows[0]);
        }

        public List<AccountDTO> GetAll()
        {
            var list = new List<AccountDTO>();
            var dt = DBHelper.ExecuteQuery("SELECT * FROM Account ORDER BY Id");
            foreach (DataRow row in dt.Rows)
                list.Add(MapAccount(row));
            return list;
        }

        public bool Insert(AccountDTO acc)
        {
            string sql = @"INSERT INTO Account (UserName, DisplayName, PassWord, Role)
                           VALUES (@user, @display, @pass, @role)";
            return DBHelper.ExecuteNonQuery(sql,
                new SqlParameter("@user", acc.UserName),
                new SqlParameter("@display", acc.DisplayName),
                new SqlParameter("@pass", acc.PassWord),
                new SqlParameter("@role", acc.Role)) > 0;
        }

        public bool Update(AccountDTO acc)
        {
            string sql = @"UPDATE Account SET DisplayName=@display, PassWord=@pass, Role=@role
                           WHERE Id=@id";
            return DBHelper.ExecuteNonQuery(sql,
                new SqlParameter("@display", acc.DisplayName),
                new SqlParameter("@pass", acc.PassWord),
                new SqlParameter("@role", acc.Role),
                new SqlParameter("@id", acc.Id)) > 0;
        }

        public bool Delete(int id)
        {
            return DBHelper.ExecuteNonQuery("DELETE FROM Account WHERE Id=@id",
                new SqlParameter("@id", id)) > 0;
        }

        public bool IsUserNameExists(string username, int excludeId = 0)
        {
            var dt = DBHelper.ExecuteQuery(
                "SELECT Id FROM Account WHERE UserName=@user AND Id<>@id",
                new SqlParameter("@user", username),
                new SqlParameter("@id", excludeId));
            return dt.Rows.Count > 0;
        }

        private AccountDTO MapAccount(DataRow row) => new AccountDTO
        {
            Id          = Convert.ToInt32(row["Id"]),
            UserName    = row["UserName"].ToString(),
            DisplayName = row["DisplayName"].ToString(),
            PassWord    = row["PassWord"].ToString(),
            Role        = row["Role"].ToString()
        };
    }

    // =============================================
    // ROOM CATEGORY DAL
    // =============================================
    public class RoomCategoryDAL
    {
        public List<RoomCategoryDTO> GetAll()
        {
            var list = new List<RoomCategoryDTO>();
            var dt = DBHelper.ExecuteQuery("SELECT * FROM RoomCategory ORDER BY Id");
            foreach (DataRow row in dt.Rows)
                list.Add(MapCategory(row));
            return list;
        }

        public bool Insert(RoomCategoryDTO cat)
        {
            return DBHelper.ExecuteNonQuery(
                "INSERT INTO RoomCategory (Name, Price) VALUES (@name, @price)",
                new SqlParameter("@name", cat.Name),
                new SqlParameter("@price", cat.Price)) > 0;
        }

        public bool Update(RoomCategoryDTO cat)
        {
            return DBHelper.ExecuteNonQuery(
                "UPDATE RoomCategory SET Name=@name, Price=@price WHERE Id=@id",
                new SqlParameter("@name", cat.Name),
                new SqlParameter("@price", cat.Price),
                new SqlParameter("@id", cat.Id)) > 0;
        }

        public bool Delete(int id)
        {
            return DBHelper.ExecuteNonQuery("DELETE FROM RoomCategory WHERE Id=@id",
                new SqlParameter("@id", id)) > 0;
        }

        private RoomCategoryDTO MapCategory(DataRow row) => new RoomCategoryDTO
        {
            Id    = Convert.ToInt32(row["Id"]),
            Name  = row["Name"].ToString(),
            Price = Convert.ToDecimal(row["Price"])
        };
    }

    // =============================================
    // ROOM DAL
    // =============================================
    public class RoomDAL
    {
        public List<RoomDTO> GetAll()
        {
            var list = new List<RoomDTO>();
            string sql = @"SELECT r.Id, r.RoomNumber, r.Floor, r.IdCategory, r.Status,
                                  rc.Name AS CategoryName, rc.Price AS CategoryPrice
                           FROM Room r
                           INNER JOIN RoomCategory rc ON r.IdCategory = rc.Id
                           ORDER BY r.Floor, r.RoomNumber";
            var dt = DBHelper.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRoom(row));
            return list;
        }

        public List<int> GetDistinctFloors()
        {
            var floors = new List<int>();
            var dt = DBHelper.ExecuteQuery("SELECT DISTINCT Floor FROM Room ORDER BY Floor");
            foreach (DataRow row in dt.Rows)
                floors.Add(Convert.ToInt32(row["Floor"]));
            return floors;
        }

        public bool Insert(RoomDTO room)
        {
            return DBHelper.ExecuteNonQuery(
                "INSERT INTO Room (RoomNumber, Floor, IdCategory, Status) VALUES (@num, @floor, @cat, N'Trống')",
                new SqlParameter("@num", room.RoomNumber),
                new SqlParameter("@floor", room.Floor),
                new SqlParameter("@cat", room.IdCategory)) > 0;
        }

        public bool Update(RoomDTO room)
        {
            return DBHelper.ExecuteNonQuery(
                "UPDATE Room SET RoomNumber=@num, Floor=@floor, IdCategory=@cat WHERE Id=@id",
                new SqlParameter("@num", room.RoomNumber),
                new SqlParameter("@floor", room.Floor),
                new SqlParameter("@cat", room.IdCategory),
                new SqlParameter("@id", room.Id)) > 0;
        }

        public bool UpdateStatus(int roomId, string status)
        {
            return DBHelper.ExecuteNonQuery(
                "UPDATE Room SET Status=@status WHERE Id=@id",
                new SqlParameter("@status", status),
                new SqlParameter("@id", roomId)) > 0;
        }

        public bool Delete(int id)
        {
            return DBHelper.ExecuteNonQuery("DELETE FROM Room WHERE Id=@id",
                new SqlParameter("@id", id)) > 0;
        }

        public bool IsRoomNumberExists(string roomNumber, int excludeId = 0)
        {
            var dt = DBHelper.ExecuteQuery(
                "SELECT Id FROM Room WHERE RoomNumber=@num AND Id<>@id",
                new SqlParameter("@num", roomNumber),
                new SqlParameter("@id", excludeId));
            return dt.Rows.Count > 0;
        }

        private RoomDTO MapRoom(DataRow row) => new RoomDTO
        {
            Id            = Convert.ToInt32(row["Id"]),
            RoomNumber    = row["RoomNumber"].ToString(),
            Floor         = Convert.ToInt32(row["Floor"]),
            IdCategory    = Convert.ToInt32(row["IdCategory"]),
            CategoryName  = row["CategoryName"].ToString(),
            CategoryPrice = Convert.ToDecimal(row["CategoryPrice"]),
            Status        = row["Status"].ToString()
        };
    }

    // =============================================
    // CUSTOMER DAL
    // =============================================
    public class CustomerDAL
    {
        public List<CustomerDTO> GetAll()
        {
            var list = new List<CustomerDTO>();
            var dt = DBHelper.ExecuteQuery("SELECT * FROM Customer ORDER BY Id");
            foreach (DataRow row in dt.Rows)
                list.Add(MapCustomer(row));
            return list;
        }

        public CustomerDTO GetById(int id)
        {
            var dt = DBHelper.ExecuteQuery("SELECT * FROM Customer WHERE Id=@id",
                new SqlParameter("@id", id));
            return dt.Rows.Count > 0 ? MapCustomer(dt.Rows[0]) : null;
        }

        public int Insert(CustomerDTO cust)
        {
            string sql = @"INSERT INTO Customer (FullName, IdCard, Phone, Address, Email)
                           VALUES (@name, @idcard, @phone, @addr, @email);
                           SELECT SCOPE_IDENTITY();";
            var result = DBHelper.ExecuteScalar(sql,
                new SqlParameter("@name",   cust.FullName),
                new SqlParameter("@idcard", cust.IdCard),
                new SqlParameter("@phone",  cust.Phone),
                new SqlParameter("@addr",   (object)cust.Address  ?? DBNull.Value),
                new SqlParameter("@email",  (object)cust.Email    ?? DBNull.Value));
            return Convert.ToInt32(result);
        }

        public bool Update(CustomerDTO cust)
        {
            return DBHelper.ExecuteNonQuery(
                "UPDATE Customer SET FullName=@name, IdCard=@idcard, Phone=@phone, Address=@addr, Email=@email WHERE Id=@id",
                new SqlParameter("@name",   cust.FullName),
                new SqlParameter("@idcard", cust.IdCard),
                new SqlParameter("@phone",  cust.Phone),
                new SqlParameter("@addr",   (object)cust.Address ?? DBNull.Value),
                new SqlParameter("@email",  (object)cust.Email   ?? DBNull.Value),
                new SqlParameter("@id",     cust.Id)) > 0;
        }

        public bool Delete(int id)
        {
            // Backup thông tin khách vào Booking trước khi xóa
            DBHelper.ExecuteNonQuery(@"
                UPDATE b SET 
                    b.CustomerNameBackup   = c.FullName,
                    b.CustomerPhoneBackup  = c.Phone,
                    b.CustomerIdCardBackup = c.IdCard
                FROM Booking b
                INNER JOIN Customer c ON b.IdCustomer = c.Id
                WHERE b.IdCustomer = @id",
                new SqlParameter("@id", id));

            // Null hóa IdCustomer
            DBHelper.ExecuteNonQuery(
                "UPDATE Booking SET IdCustomer = NULL WHERE IdCustomer = @id",
                new SqlParameter("@id", id));

            // Xóa khách hàng
            return DBHelper.ExecuteNonQuery(
                "DELETE FROM Customer WHERE Id = @id",
                new SqlParameter("@id", id)) > 0;
        }

        public List<CustomerDTO> Search(string keyword)
        {
            var list = new List<CustomerDTO>();
            string sql = @"SELECT * FROM Customer 
                           WHERE FullName LIKE @kw OR IdCard LIKE @kw OR Phone LIKE @kw
                           ORDER BY Id";
            var dt = DBHelper.ExecuteQuery(sql,
                new SqlParameter("@kw", $"%{keyword}%"));
            foreach (DataRow row in dt.Rows)
                list.Add(MapCustomer(row));
            return list;
        }

        private CustomerDTO MapCustomer(DataRow row) => new CustomerDTO
        {
            Id       = Convert.ToInt32(row["Id"]),
            FullName = row["FullName"].ToString(),
            IdCard   = row["IdCard"].ToString(),
            Phone    = row["Phone"].ToString(),
            Address  = row["Address"] == DBNull.Value ? "" : row["Address"].ToString(),
            Email    = row["Email"]   == DBNull.Value ? "" : row["Email"].ToString()
        };
    }

    // =============================================
    // BOOKING DAL
    // =============================================
    public class BookingDAL
    {
        public int Insert(BookingDTO booking)
        {
            string sql = @"INSERT INTO Booking (CheckIn, CheckOut, IdRoom, IdCustomer, Status, RoomAmount, ServiceAmount, TotalPrice, Note)
                           VALUES (@ci, @co, @room, @cust, N'Đang ở', @roomAmt, 0, @roomAmt, @note);
                           SELECT SCOPE_IDENTITY();";
            var result = DBHelper.ExecuteScalar(sql,
                new SqlParameter("@ci",      booking.CheckIn),
                new SqlParameter("@co",      booking.CheckOut),
                new SqlParameter("@room",    booking.IdRoom),
                new SqlParameter("@cust",    booking.IdCustomer),
                new SqlParameter("@roomAmt", booking.RoomAmount),
                new SqlParameter("@note",    (object)booking.Note ?? DBNull.Value));
            return Convert.ToInt32(result);
        }

        public BookingDTO GetActiveByRoomId(int roomId)
        {
            string sql = @"
                SELECT b.*,
                COALESCE(c.FullName, b.CustomerNameBackup,  N'(Đã xóa)') AS CustomerName,
                COALESCE(c.Phone,    b.CustomerPhoneBackup,  '')           AS CustomerPhone,
                COALESCE(c.IdCard,   b.CustomerIdCardBackup, '')           AS CustomerIdCard,
                r.RoomNumber
                FROM Booking b
                LEFT JOIN Customer c ON b.IdCustomer = c.Id
                INNER JOIN Room r    ON b.IdRoom     = r.Id
                WHERE b.IdRoom = @roomId AND b.Status = N'Đang ở'";
            var dt = DBHelper.ExecuteQuery(sql, new SqlParameter("@roomId", roomId));
            return dt.Rows.Count > 0 ? MapBooking(dt.Rows[0]) : null;
        }

        public List<BookingDTO> GetAll()
        {
            string sql = @"
                SELECT b.*,
                COALESCE(c.FullName, b.CustomerNameBackup,  N'(Đã xóa)') AS CustomerName,
                COALESCE(c.Phone,    b.CustomerPhoneBackup,  '')           AS CustomerPhone,
                COALESCE(c.IdCard,   b.CustomerIdCardBackup, '')           AS CustomerIdCard,
                r.RoomNumber
                FROM Booking b
                LEFT JOIN Customer c ON b.IdCustomer = c.Id
                INNER JOIN Room r    ON b.IdRoom     = r.Id
                ORDER BY b.Id DESC";
            var list = new List<BookingDTO>();
            var dt = DBHelper.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
                list.Add(MapBooking(row));
            return list;
        }

        public bool UpdateAmounts(int bookingId, decimal roomAmount, decimal serviceAmount)
        {
            decimal total = roomAmount + serviceAmount;
            return DBHelper.ExecuteNonQuery(
                "UPDATE Booking SET RoomAmount=@r, ServiceAmount=@s, TotalPrice=@t WHERE Id=@id",
                new SqlParameter("@r",  roomAmount),
                new SqlParameter("@s",  serviceAmount),
                new SqlParameter("@t",  total),
                new SqlParameter("@id", bookingId)) > 0;
        }

        public bool Checkout(int bookingId)
        {
            return DBHelper.ExecuteNonQuery(
                "UPDATE Booking SET Status=N'Đã trả', CheckOut=GETDATE() WHERE Id=@id",
                new SqlParameter("@id", bookingId)) > 0;
        }

        // Chuyển phòng: cập nhật IdRoom trong booking đang ở
        public bool TransferRoom(int bookingId, int newRoomId)
        {
            return DBHelper.ExecuteNonQuery(
                "UPDATE Booking SET IdRoom=@newRoom WHERE Id=@id",
                new SqlParameter("@newRoom", newRoomId),
                new SqlParameter("@id", bookingId)) > 0;
        }

        private BookingDTO MapBooking(DataRow row) => new BookingDTO
        {
            Id = Convert.ToInt32(row["Id"]),
            CheckIn = Convert.ToDateTime(row["CheckIn"]),
            CheckOut = Convert.ToDateTime(row["CheckOut"]),
            IdRoom = Convert.ToInt32(row["IdRoom"]),
            RoomNumber = row["RoomNumber"].ToString(),
            IdCustomer = row["IdCustomer"] == DBNull.Value
                     ? 0 : Convert.ToInt32(row["IdCustomer"]),
            CustomerName = row["CustomerName"].ToString(),
            CustomerPhone = row["CustomerPhone"].ToString(),
            CustomerIdCard = row["CustomerIdCard"].ToString(),
            Status = row["Status"].ToString(),
            TotalPrice = Convert.ToDecimal(row["TotalPrice"]),
            RoomAmount = Convert.ToDecimal(row["RoomAmount"]),
            ServiceAmount = Convert.ToDecimal(row["ServiceAmount"]),
            Note = row["Note"] == DBNull.Value ? "" : row["Note"].ToString()
        };
    }

    // =============================================
    // SERVICE DAL
    // =============================================
    public class ServiceDAL
    {
        public List<ServiceDTO> GetAll()
        {
            var list = new List<ServiceDTO>();
            var dt = DBHelper.ExecuteQuery("SELECT * FROM Service ORDER BY ServiceName");
            foreach (DataRow row in dt.Rows)
                list.Add(MapService(row));
            return list;
        }

        public List<ServiceDTO> GetActive()
        {
            var list = new List<ServiceDTO>();
            var dt = DBHelper.ExecuteQuery("SELECT * FROM Service WHERE IsActive=1 ORDER BY ServiceName");
            foreach (DataRow row in dt.Rows)
                list.Add(MapService(row));
            return list;
        }

        public bool Insert(ServiceDTO svc)
        {
            return DBHelper.ExecuteNonQuery(
                "INSERT INTO Service (ServiceName, Price, IsActive) VALUES (@name, @price, 1)",
                new SqlParameter("@name",  svc.ServiceName),
                new SqlParameter("@price", svc.Price)) > 0;
        }

        public bool Update(ServiceDTO svc)
        {
            return DBHelper.ExecuteNonQuery(
                "UPDATE Service SET ServiceName=@name, Price=@price, IsActive=@active WHERE Id=@id",
                new SqlParameter("@name",   svc.ServiceName),
                new SqlParameter("@price",  svc.Price),
                new SqlParameter("@active", svc.IsActive),
                new SqlParameter("@id",     svc.Id)) > 0;
        }

        public bool Delete(int id)
        {
            return DBHelper.ExecuteNonQuery("DELETE FROM Service WHERE Id=@id",
                new SqlParameter("@id", id)) > 0;
        }

        private ServiceDTO MapService(DataRow row) => new ServiceDTO
        {
            Id          = Convert.ToInt32(row["Id"]),
            ServiceName = row["ServiceName"].ToString(),
            Price       = Convert.ToDecimal(row["Price"]),
            IsActive    = Convert.ToBoolean(row["IsActive"])
        };
    }

    // =============================================
    // BOOKING SERVICE DAL
    // =============================================
    public class BookingServiceDAL
    {
        public List<BookingServiceDTO> GetByBookingId(int bookingId)
        {
            var list = new List<BookingServiceDTO>();
            string sql = @"SELECT bs.*, s.ServiceName
                           FROM BookingService bs
                           INNER JOIN Service s ON bs.IdService = s.Id
                           WHERE bs.IdBooking = @bid
                           ORDER BY bs.AddedAt";
            var dt = DBHelper.ExecuteQuery(sql, new SqlParameter("@bid", bookingId));
            foreach (DataRow row in dt.Rows)
                list.Add(MapBS(row));
            return list;
        }

        public bool Insert(BookingServiceDTO bs)
        {
            return DBHelper.ExecuteNonQuery(
                "INSERT INTO BookingService (IdBooking, IdService, Count, UnitPrice, AddedAt) VALUES (@bid, @sid, @cnt, @price, GETDATE())",
                new SqlParameter("@bid",   bs.IdBooking),
                new SqlParameter("@sid",   bs.IdService),
                new SqlParameter("@cnt",   bs.Count),
                new SqlParameter("@price", bs.UnitPrice)) > 0;
        }

        public bool Delete(int id)
        {
            return DBHelper.ExecuteNonQuery("DELETE FROM BookingService WHERE Id=@id",
                new SqlParameter("@id", id)) > 0;
        }

        public decimal GetTotalServiceAmount(int bookingId)
        {
            var result = DBHelper.ExecuteScalar(
                "SELECT ISNULL(SUM(Count * UnitPrice), 0) FROM BookingService WHERE IdBooking=@bid",
                new SqlParameter("@bid", bookingId));
            return Convert.ToDecimal(result);
        }

        private BookingServiceDTO MapBS(DataRow row) => new BookingServiceDTO
        {
            Id          = Convert.ToInt32(row["Id"]),
            IdBooking   = Convert.ToInt32(row["IdBooking"]),
            IdService   = Convert.ToInt32(row["IdService"]),
            ServiceName = row["ServiceName"].ToString(),
            Count       = Convert.ToInt32(row["Count"]),
            UnitPrice   = Convert.ToDecimal(row["UnitPrice"]),
            AddedAt     = Convert.ToDateTime(row["AddedAt"])
        };
    }
}
