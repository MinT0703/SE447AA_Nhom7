using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace OnlineShop
{
    public partial class TaiKhoan : System.Web.UI.Page
    {
        // Connection string từ web.config
        string connect = @"Data Source=localhost;Initial Catalog=SE397BJ;Integrated Security=True;Encrypt=False";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Debug session
                System.Diagnostics.Debug.WriteLine("=== DEBUG SESSION ===");
                System.Diagnostics.Debug.WriteLine("Session UserID: " + Session["UserID"]);
                System.Diagnostics.Debug.WriteLine("Session IsLoggedIn: " + Session["IsLoggedIn"]);

                // Kiểm tra tất cả session keys
                foreach (string key in Session.Keys)
                {
                    System.Diagnostics.Debug.WriteLine($"Session[{key}]: {Session[key]}");
                }

                if (Session["UserID"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("UserID is null, redirecting to login");
                    Response.Redirect("Login.aspx");
                    return;
                }

                bool isLoggedIn = false;
                if (Session["IsLoggedIn"] != null)
                {
                    bool.TryParse(Session["IsLoggedIn"].ToString(), out isLoggedIn);
                }

                if (!isLoggedIn)
                {
                    System.Diagnostics.Debug.WriteLine("IsLoggedIn is false, redirecting to login");
                    Response.Redirect("Login.aspx");
                    return;
                }

                if (!IsPostBack)
                {
                    // Debug controls trước khi load
                    System.Diagnostics.Debug.WriteLine("=== DEBUG CONTROLS ===");
                    System.Diagnostics.Debug.WriteLine("txtUserName: " + (txtUserName != null ? "EXISTS" : "NULL"));
                    System.Diagnostics.Debug.WriteLine("txtFullName: " + (txtFullName != null ? "EXISTS" : "NULL"));
                    System.Diagnostics.Debug.WriteLine("txtEmail: " + (txtEmail != null ? "EXISTS" : "NULL"));
                    System.Diagnostics.Debug.WriteLine("txtPhone: " + (txtPhone != null ? "EXISTS" : "NULL"));
                    System.Diagnostics.Debug.WriteLine("txtBirthdate: " + (txtBirthdate != null ? "EXISTS" : "NULL"));
                    System.Diagnostics.Debug.WriteLine("ddlGender: " + (ddlGender != null ? "EXISTS" : "NULL"));
                    System.Diagnostics.Debug.WriteLine("UserAvatar: " + (UserAvatar != null ? "EXISTS" : "NULL"));

                    LoadUserInfo();

                    string tab = Request.QueryString["tab"];
                    if (!string.IsNullOrEmpty(tab))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ActivateTab",
                            "activateTab('" + tab + "');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in Page_Load: " + ex.ToString());
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('Lỗi tải trang: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        private void LoadUserInfo()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== LOADING USER INFO ===");

                if (Session["UserID"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("Session UserID is null in LoadUserInfo");
                    return;
                }

                int userId;
                if (!int.TryParse(Session["UserID"].ToString(), out userId))
                {
                    System.Diagnostics.Debug.WriteLine("Cannot parse UserID: " + Session["UserID"].ToString());
                    return;
                }

                System.Diagnostics.Debug.WriteLine("UserID parsed: " + userId);

                using (SqlConnection conn = new SqlConnection(connect))
                {
                    System.Diagnostics.Debug.WriteLine("Connection string: " + connect);

                    string query = @"SELECT id_khachhang, tendangnhap, hoten, email, dienthoai, 
                           ngaysinh, gioitinh, diachi, avatar 
                           FROM KhachHang 
                           WHERE id_khachhang = @UserId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    System.Diagnostics.Debug.WriteLine("Executing query: " + query);
                    System.Diagnostics.Debug.WriteLine("With UserID parameter: " + userId);

                    conn.Open();
                    System.Diagnostics.Debug.WriteLine("Database connected successfully");

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine("=== USER DATA FOUND ===");

                        // Debug tất cả dữ liệu từ database
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string fieldName = reader.GetName(i);
                            object fieldValue = reader[i];
                            System.Diagnostics.Debug.WriteLine($"{fieldName}: {(fieldValue == DBNull.Value ? "NULL" : fieldValue.ToString())}");
                        }

                        // Xử lý tên hiển thị
                        string displayName = "";
                        if (reader["hoten"] != DBNull.Value && !string.IsNullOrEmpty(reader["hoten"].ToString()))
                        {
                            displayName = reader["hoten"].ToString();
                        }
                        else if (reader["tendangnhap"] != DBNull.Value && !string.IsNullOrEmpty(reader["tendangnhap"].ToString()))
                        {
                            displayName = reader["tendangnhap"].ToString();
                        }
                        else
                        {
                            displayName = "Khách hàng";
                        }

                        System.Diagnostics.Debug.WriteLine("Display name determined: " + displayName);

                        // Kiểm tra và gán giá trị cho từng control
                        if (txtUserName != null)
                        {
                            txtUserName.Text = displayName;
                            System.Diagnostics.Debug.WriteLine("txtUserName set successfully to: " + displayName);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: txtUserName control is null!");
                        }

                        if (txtFullName != null)
                        {
                            string fullName = reader["hoten"] != DBNull.Value ? reader["hoten"].ToString() : "";
                            txtFullName.Text = fullName;
                            System.Diagnostics.Debug.WriteLine("txtFullName set to: " + fullName);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: txtFullName control is null!");
                        }

                        if (txtEmail != null)
                        {
                            string email = reader["email"] != DBNull.Value ? reader["email"].ToString() : "";
                            txtEmail.Text = email;
                            System.Diagnostics.Debug.WriteLine("txtEmail set to: " + email);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: txtEmail control is null!");
                        }

                        if (txtPhone != null)
                        {
                            string phone = reader["dienthoai"] != DBNull.Value ? reader["dienthoai"].ToString() : "";
                            txtPhone.Text = phone;
                            System.Diagnostics.Debug.WriteLine("txtPhone set to: " + phone);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: txtPhone control is null!");
                        }

                        // Xử lý ngày sinh
                        if (txtBirthdate != null && reader["ngaysinh"] != DBNull.Value)
                        {
                            DateTime birthDate = Convert.ToDateTime(reader["ngaysinh"]);
                            txtBirthdate.Text = birthDate.ToString("yyyy-MM-dd");
                            System.Diagnostics.Debug.WriteLine("txtBirthdate set to: " + txtBirthdate.Text);
                        }
                        else if (txtBirthdate == null)
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: txtBirthdate control is null!");
                        }

                        // Xử lý giới tính
                        if (ddlGender != null && reader["gioitinh"] != DBNull.Value)
                        {
                            string gender = reader["gioitinh"].ToString().ToLower().Trim();
                            System.Diagnostics.Debug.WriteLine("Gender from DB: " + gender);

                            switch (gender)
                            {
                                case "nam":
                                case "male":
                                    ddlGender.SelectedValue = "male";
                                    break;
                                case "nữ":
                                case "nu":
                                case "female":
                                    ddlGender.SelectedValue = "female";
                                    break;
                                default:
                                    ddlGender.SelectedValue = "other";
                                    break;
                            }
                            System.Diagnostics.Debug.WriteLine("ddlGender set to: " + ddlGender.SelectedValue);
                        }
                        else if (ddlGender == null)
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: ddlGender control is null!");
                        }

                        // Tải ảnh đại diện
                        if (UserAvatar != null)
                        {
                            if (reader["avatar"] != DBNull.Value && !string.IsNullOrEmpty(reader["avatar"].ToString()))
                            {
                                UserAvatar.ImageUrl = "~/Img/avatars/" + reader["avatar"].ToString();
                            }
                            else
                            {
                                UserAvatar.ImageUrl = "~/Img/user-placeholder.jpg";
                            }
                            System.Diagnostics.Debug.WriteLine("UserAvatar set to: " + UserAvatar.ImageUrl);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: UserAvatar control is null!");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: No user data found for UserID: " + userId);

                        // Kiểm tra xem có data nào trong bảng không
                        reader.Close();
                        SqlCommand countCmd = new SqlCommand("SELECT COUNT(*) FROM KhachHang", conn);
                        int totalUsers = (int)countCmd.ExecuteScalar();
                        System.Diagnostics.Debug.WriteLine("Total users in KhachHang table: " + totalUsers);

                        // Kiểm tra UserID có tồn tại không
                        SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM KhachHang WHERE id_khachhang = @UserId", conn);
                        checkCmd.Parameters.AddWithValue("@UserId", userId);
                        int userExists = (int)checkCmd.ExecuteScalar();
                        System.Diagnostics.Debug.WriteLine("User exists with ID " + userId + ": " + (userExists > 0));

                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Không tìm thấy thông tin người dùng!');", true);
                        Response.Redirect("Login.aspx");
                    }
                    reader.Close();
                }

                LoadOrderInfo(userId);
                LoadAddressInfo(userId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR in LoadUserInfo: " + ex.ToString());
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);

                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('Lỗi khi tải thông tin: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        private void LoadOrderInfo(int userId)
        {
            try
            {
                // Kiểm tra xem bảng DonHang có tồn tại không
                using (SqlConnection conn = new SqlConnection(connect))
                {
                    // Kiểm tra bảng tồn tại
                    string checkTableQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DonHang'";
                    SqlCommand checkCmd = new SqlCommand(checkTableQuery, conn);
                    conn.Open();
                    int tableExists = (int)checkCmd.ExecuteScalar();

                    if (tableExists > 0)
                    {
                        string query = @"SELECT TOP 10 * FROM DonHang 
                                       WHERE id_khachhang = @UserId 
                                       ORDER BY ngaytao DESC";

                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        // Thực hiện query và bind vào repeater/gridview nếu có
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        System.Diagnostics.Debug.WriteLine("Orders loaded: " + dt.Rows.Count);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("DonHang table does not exist");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in LoadOrderInfo: " + ex.ToString());
            }
        }

        private void LoadAddressInfo(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connect))
                {
                    string query = "SELECT diachi FROM KhachHang WHERE id_khachhang = @UserId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        string address = result.ToString();
                        System.Diagnostics.Debug.WriteLine("Address loaded: " + address);

                        // Hiển thị địa chỉ nếu có control tương ứng
                        // txtAddress.Text = address; // uncomment nếu có control này
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in LoadAddressInfo: " + ex.ToString());
            }
        }

        protected void SaveInfo_Click(object sender, EventArgs e)
        {
            try
            {
                // Sử dụng UserID thay vì id_khachhang
                int userId = Convert.ToInt32(Session["UserID"]);

                using (SqlConnection conn = new SqlConnection(connect))
                {
                    // Cập nhật query để bao gồm tất cả các trường cần thiết
                    string query = @"UPDATE KhachHang SET 
                                   hoten = @FullName, 
                                   email = @Email,
                                   dienthoai = @Phone,
                                   ngaysinh = @BirthDate,
                                   gioitinh = @Gender
                                   WHERE id_khachhang = @UserId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    // Xử lý ngày sinh
                    if (!string.IsNullOrEmpty(txtBirthdate.Text))
                    {
                        cmd.Parameters.AddWithValue("@BirthDate", DateTime.Parse(txtBirthdate.Text));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@BirthDate", DBNull.Value);
                    }

                    // Xử lý giới tính
                    string genderText = "";
                    switch (ddlGender.SelectedValue)
                    {
                        case "male":
                            genderText = "Nam";
                            break;
                        case "female":
                            genderText = "Nữ";
                            break;
                        default:
                            genderText = "Khác";
                            break;
                    }
                    cmd.Parameters.AddWithValue("@Gender", genderText);

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        // Cập nhật lại tên hiển thị và session
                        txtUserName.Text = txtFullName.Text.Trim();
                        Session["FullName"] = txtFullName.Text.Trim();
                        Session["Email"] = txtEmail.Text.Trim();
                        Session["Phone"] = txtPhone.Text.Trim();

                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Cập nhật thông tin thành công!');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Không thể cập nhật thông tin!');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('Lỗi khi cập nhật: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        // Các phương thức khác giữ nguyên
        protected void BtnInfo_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ActivateTab",
                "activateTab('info');", true);
        }

        protected void BtnOrders_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ActivateTab",
                "activateTab('orders');", true);
        }

        protected void BtnAddresses_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ActivateTab",
                "activateTab('addresses');", true);
        }

        protected void BtnSecurity_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ActivateTab",
                "activateTab('security');", true);
        }

        private bool ColumnExists(string tableName, string columnName)
        {
            using (SqlConnection conn = new SqlConnection(connect))
            {
                string query = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName AND COLUMN_NAME = @ColumnName";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TableName", tableName);
                cmd.Parameters.AddWithValue("@ColumnName", columnName);

                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        protected void txtFullName_TextChanged(object sender, EventArgs e) { }
        protected void txtEmail_TextChanged(object sender, EventArgs e) { }
        protected void txtPhone_TextChanged(object sender, EventArgs e) { }
        protected void txtBirthdate_TextChanged(object sender, EventArgs e) { }
        protected void ddlGender_SelectedIndexChanged(object sender, EventArgs e) { }

        protected void EditAddress_Click(object sender, EventArgs e)
        {
            string addressId = ((Button)sender).CommandArgument;
            Response.Redirect("ChinhSuaDiaChi.aspx?id=" + addressId);
        }

        protected void RemoveAddress_Click(object sender, EventArgs e)
        {
            string addressId = ((Button)sender).CommandArgument;
            Response.Redirect("TaiKhoan.aspx?tab=addresses");
        }

        protected void ViewOrderDetails_Click(object sender, EventArgs e)
        {
            string orderId = ((Button)sender).CommandArgument;
            Response.Redirect("ChiTietDonHang.aspx?id=" + orderId);
        }

        protected void ChangePassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("DoiMatKhau.aspx");
        }

        protected void Setup2FA_Click(object sender, EventArgs e)
        {
            Response.Redirect("XacThucHaiLop.aspx");
        }

        protected void ViewLoginHistory_Click(object sender, EventArgs e)
        {
            Response.Redirect("LichSuDangNhap.aspx");
        }

        protected void SetupSecurityAlerts_Click(object sender, EventArgs e)
        {
            Response.Redirect("CaiDatThongBao.aspx");
        }
    }
}