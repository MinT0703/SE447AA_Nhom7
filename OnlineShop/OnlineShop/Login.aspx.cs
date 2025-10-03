﻿using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Web.Security;

namespace OnlineShop
{
    public partial class Login : System.Web.UI.Page
    {
        string connect = @"Data Source=localhost;Initial Catalog=SE397BJ;Integrated Security=True;Encrypt=False";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Xử lý khi trang được tải lần đầu
            }
        }

        private bool ValidateLogin(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connect))
            {
                string query = "SELECT COUNT(*) FROM KhachHang WHERE (tendangnhap = @username OR email = @username) AND matkhau = @password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                try
                {
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Lỗi khi kiểm tra đăng nhập: " + ex.Message);
                    return false;
                }
            }
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                // Kiểm tra đầu vào
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    Response.Write("<script>alert('Vui lòng nhập tên đăng nhập và mật khẩu!');</script>");
                    return;
                }

                if (!CheckIfUserExists(username))
                {
                    // Tài khoản không tồn tại
                    Response.Write("<script>alert('Tài khoản chưa tồn tại trong hệ thống!');</script>");
                    Response.Redirect("Register.aspx");
                    return;
                }

                if (ValidateLogin(username, password))
                {
                    // Lưu thông tin người dùng vào Session
                    SaveUserInfoToSession(username);

                    // Đăng nhập thành công
                    FormsAuthentication.SetAuthCookie(username, true);
                    Response.Write("<script>alert('Đăng nhập thành công!');</script>");
                    Response.Redirect("Home.aspx");
                }
                else
                {
                    // Đăng nhập thất bại (sai mật khẩu)
                    Response.Write("<script>alert('Mật khẩu không đúng!');</script>");
                }
            }
            catch (Exception exx)
            {
                Console.Error.WriteLine("Lỗi khi đăng nhập: " + exx.Message);
                Response.Write("<script>alert('Có lỗi xảy ra khi đăng nhập!');</script>");
            }
        }
        private bool CheckIfUserExists(string username)
        {
            using (SqlConnection conn = new SqlConnection(connect))
            {
                string query = "SELECT COUNT(*) FROM KhachHang WHERE tendangnhap = @username OR email = @username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);

                try
                {
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Lỗi khi kiểm tra tài khoản: " + ex.Message);
                    return false;
                }
            }
        }

        private void SaveUserInfoToSession(string username)
        {
            using (SqlConnection conn = new SqlConnection(connect))
            {
                // Chỉ SELECT các trường có trong bảng
                string query = "SELECT id_khachhang, tendangnhap, hoten, email, dienthoai FROM KhachHang WHERE tendangnhap = @username OR email = @username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Chỉ lưu các trường có dữ liệu
                        Session["UserID"] = reader["id_khachhang"].ToString();
                        Session["Username"] = reader["tendangnhap"].ToString();
                        Session["FullName"] = reader["hoten"].ToString();
                        Session["Email"] = reader["email"].ToString();
                        Session["Phone"] = reader["dienthoai"] != DBNull.Value ? reader["dienthoai"].ToString() : "";
                        Session["IsLoggedIn"] = true;

                        // Không lưu ngaysinh và gioitinh vì không có trong query
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Lỗi khi lưu thông tin người dùng: " + ex.Message);
                }
            }
        }
    }
}