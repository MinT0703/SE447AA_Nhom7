using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace OnlineShop
{
    public partial class ChinhSuaDiaChi : System.Web.UI.Page
    {
        string connect = @"Data Source=localhost;Initial Catalog=SE397BJ;Integrated Security=True;Encrypt=False";

        private int addressId = 0;
        private int userId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đăng nhập
                if (Session["UserId"] == null)
                {
                    Response.Redirect("DangNhap.aspx", false);
                    return;
                }

                if (!int.TryParse(Session["UserId"].ToString(), out userId))
                {
                    Response.Redirect("DangNhap.aspx", false);
                    return;
                }

                // Lấy AddressId từ query string
                if (Request.QueryString["id"] != null)
                {
                    if (int.TryParse(Request.QueryString["id"], out addressId))
                    {
                        if (!IsPostBack)
                        {
                            LoadProvinces();
                            LoadAddressData();
                            btnDelete.Visible = true; // Hiện nút xóa khi edit
                            PageTitle.Text = "Sửa Địa Chỉ";
                        }
                    }
                    else
                    {
                        Response.Redirect("TaiKhoan.aspx?tab=addresses", false);
                        return;
                    }
                }
                else
                {
                    // Nếu không có id thì đây là trang thêm mới
                    if (!IsPostBack)
                    {
                        LoadProvinces();
                        PageTitle.Text = "Thêm Địa Chỉ Mới";
                        btnDelete.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi và redirect về trang chính
                System.Diagnostics.Debug.WriteLine("Error in Page_Load: " + ex.Message);
                Response.Redirect("TaiKhoan.aspx?tab=addresses&error=1", false);
            }
        }

        private void LoadProvinces()
        {
            try
            {
                // Dữ liệu tỉnh/thành phố mẫu
                ddlProvince.Items.Clear();
                ddlProvince.Items.Add(new ListItem("Chọn Tỉnh/Thành phố", ""));
                ddlProvince.Items.Add(new ListItem("TP. Hồ Chí Minh", "79"));
                ddlProvince.Items.Add(new ListItem("Hà Nội", "01"));
                ddlProvince.Items.Add(new ListItem("Đà Nẵng", "48"));
                ddlProvince.Items.Add(new ListItem("Cần Thơ", "92"));
                ddlProvince.Items.Add(new ListItem("Hải Phòng", "31"));
                ddlProvince.Items.Add(new ListItem("Bình Dương", "74"));
                ddlProvince.Items.Add(new ListItem("Đồng Nai", "75"));
                ddlProvince.Items.Add(new ListItem("Khánh Hòa", "56"));
                ddlProvince.Items.Add(new ListItem("Lâm Đồng", "68"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in LoadProvinces: " + ex.Message);
            }
        }

        private void LoadDistricts(string provinceId)
        {
            try
            {
                ddlDistrict.Items.Clear();
                ddlDistrict.Items.Add(new ListItem("Chọn Quận/Huyện", ""));

                if (string.IsNullOrEmpty(provinceId)) return;

                // Dữ liệu quận/huyện mẫu dựa theo tỉnh
                switch (provinceId)
                {
                    case "79": // TP.HCM
                        ddlDistrict.Items.Add(new ListItem("Quận 1", "7901"));
                        ddlDistrict.Items.Add(new ListItem("Quận 2", "7902"));
                        ddlDistrict.Items.Add(new ListItem("Quận 3", "7903"));
                        ddlDistrict.Items.Add(new ListItem("Quận 4", "7904"));
                        ddlDistrict.Items.Add(new ListItem("Quận 5", "7905"));
                        ddlDistrict.Items.Add(new ListItem("Quận 6", "7906"));
                        ddlDistrict.Items.Add(new ListItem("Quận 7", "7907"));
                        ddlDistrict.Items.Add(new ListItem("Quận 8", "7908"));
                        ddlDistrict.Items.Add(new ListItem("Quận 9", "7909"));
                        ddlDistrict.Items.Add(new ListItem("Quận 10", "7910"));
                        ddlDistrict.Items.Add(new ListItem("Quận 11", "7911"));
                        ddlDistrict.Items.Add(new ListItem("Quận 12", "7912"));
                        ddlDistrict.Items.Add(new ListItem("Quận Thủ Đức", "7913"));
                        ddlDistrict.Items.Add(new ListItem("Quận Gò Vấp", "7914"));
                        ddlDistrict.Items.Add(new ListItem("Quận Bình Thạnh", "7915"));
                        ddlDistrict.Items.Add(new ListItem("Quận Tân Bình", "7916"));
                        ddlDistrict.Items.Add(new ListItem("Quận Tân Phú", "7917"));
                        ddlDistrict.Items.Add(new ListItem("Quận Phú Nhuận", "7918"));
                        break;

                    case "01": // Hà Nội
                        ddlDistrict.Items.Add(new ListItem("Quận Ba Đình", "0101"));
                        ddlDistrict.Items.Add(new ListItem("Quận Hoàn Kiếm", "0102"));
                        ddlDistrict.Items.Add(new ListItem("Quận Tây Hồ", "0103"));
                        ddlDistrict.Items.Add(new ListItem("Quận Long Biên", "0104"));
                        ddlDistrict.Items.Add(new ListItem("Quận Cầu Giấy", "0105"));
                        ddlDistrict.Items.Add(new ListItem("Quận Đống Đa", "0106"));
                        ddlDistrict.Items.Add(new ListItem("Quận Hai Bà Trưng", "0107"));
                        ddlDistrict.Items.Add(new ListItem("Quận Hoàng Mai", "0108"));
                        ddlDistrict.Items.Add(new ListItem("Quận Thanh Xuân", "0109"));
                        break;

                    case "48": // Đà Nẵng
                        ddlDistrict.Items.Add(new ListItem("Quận Hải Châu", "4801"));
                        ddlDistrict.Items.Add(new ListItem("Quận Thanh Khê", "4802"));
                        ddlDistrict.Items.Add(new ListItem("Quận Sơn Trà", "4803"));
                        ddlDistrict.Items.Add(new ListItem("Quận Ngũ Hành Sơn", "4804"));
                        ddlDistrict.Items.Add(new ListItem("Quận Liên Chiểu", "4805"));
                        ddlDistrict.Items.Add(new ListItem("Quận Cẩm Lệ", "4806"));
                        break;

                    default:
                        ddlDistrict.Items.Add(new ListItem("Quận/Huyện 1", provinceId + "01"));
                        ddlDistrict.Items.Add(new ListItem("Quận/Huyện 2", provinceId + "02"));
                        ddlDistrict.Items.Add(new ListItem("Quận/Huyện 3", provinceId + "03"));
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in LoadDistricts: " + ex.Message);
            }
        }

        private void LoadWards(string districtId)
        {
            try
            {
                ddlWard.Items.Clear();
                ddlWard.Items.Add(new ListItem("Chọn Phường/Xã", ""));

                if (string.IsNullOrEmpty(districtId)) return;

                // Dữ liệu phường/xã mẫu
                for (int i = 1; i <= 10; i++)
                {
                    ddlWard.Items.Add(new ListItem($"Phường {i}", districtId + i.ToString("00")));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in LoadWards: " + ex.Message);
            }
        }

        private void LoadAddressData()
        {
            if (addressId == 0) return;

            string query = @"SELECT hoten, dienthoai, diachi_chitiet, tinh_thanh, quan_huyen, phuong_xa, macdinh 
                           FROM DiaChi WHERE id = @id AND user_id = @userId";

            try
            {
                using (SqlConnection conn = new SqlConnection(connect))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", addressId);
                        cmd.Parameters.AddWithValue("@userId", userId);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Load thông tin cơ bản
                                txtFullName.Text = reader["hoten"]?.ToString() ?? "";
                                txtPhone.Text = reader["dienthoai"]?.ToString() ?? "";
                                txtStreetAddress.Text = reader["diachi_chitiet"]?.ToString() ?? "";
                                chkDefaultAddress.Checked = reader["macdinh"] != DBNull.Value && Convert.ToBoolean(reader["macdinh"]);

                                // Load và set province
                                string provinceName = reader["tinh_thanh"]?.ToString() ?? "";
                                if (!string.IsNullOrEmpty(provinceName))
                                {
                                    ListItem provinceItem = ddlProvince.Items.FindByText(provinceName);
                                    if (provinceItem != null)
                                    {
                                        ddlProvince.SelectedValue = provinceItem.Value;
                                        LoadDistricts(provinceItem.Value);

                                        // Load và set district
                                        string districtName = reader["quan_huyen"]?.ToString() ?? "";
                                        if (!string.IsNullOrEmpty(districtName))
                                        {
                                            ListItem districtItem = ddlDistrict.Items.FindByText(districtName);
                                            if (districtItem != null)
                                            {
                                                ddlDistrict.SelectedValue = districtItem.Value;
                                                LoadWards(districtItem.Value);

                                                // Load và set ward
                                                string wardName = reader["phuong_xa"]?.ToString() ?? "";
                                                if (!string.IsNullOrEmpty(wardName))
                                                {
                                                    ListItem wardItem = ddlWard.Items.FindByText(wardName);
                                                    if (wardItem != null)
                                                    {
                                                        ddlWard.SelectedValue = wardItem.Value;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Địa chỉ không tồn tại hoặc không thuộc user này
                                Response.Redirect("TaiKhoan.aspx?tab=addresses", false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error và redirect
                System.Diagnostics.Debug.WriteLine("Error in LoadAddressData: " + ex.Message);
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    $"alert('Lỗi khi tải dữ liệu: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        protected void ddlProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDistricts(ddlProvince.SelectedValue);
                ddlWard.Items.Clear();
                ddlWard.Items.Add(new ListItem("Chọn Phường/Xã", ""));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in ddlProvince_SelectedIndexChanged: " + ex.Message);
            }
        }

        protected void ddlDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadWards(ddlDistrict.SelectedValue);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in ddlDistrict_SelectedIndexChanged: " + ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                // Validate dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                    string.IsNullOrWhiteSpace(txtPhone.Text) ||
                    string.IsNullOrWhiteSpace(txtStreetAddress.Text) ||
                    string.IsNullOrEmpty(ddlProvince.SelectedValue) ||
                    string.IsNullOrEmpty(ddlDistrict.SelectedValue) ||
                    string.IsNullOrEmpty(ddlWard.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert",
                        "alert('Vui lòng điền đầy đủ thông tin!');", true);
                    return;
                }

                // Nếu đây là địa chỉ mặc định, cập nhật các địa chỉ khác
                if (chkDefaultAddress.Checked)
                {
                    SetOtherAddressesAsNonDefault();
                }

                if (addressId > 0)
                {
                    // Cập nhật địa chỉ hiện có
                    UpdateAddress();
                }
                else
                {
                    // Thêm địa chỉ mới
                    InsertAddress();
                }

                Response.Redirect("TaiKhoan.aspx?tab=addresses&msg=success", false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in btnSave_Click: " + ex.Message);
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    $"alert('Lỗi khi lưu địa chỉ: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        private void SetOtherAddressesAsNonDefault()
        {
            string query = "UPDATE DiaChi SET macdinh = 0 WHERE user_id = @userId";
            if (addressId > 0)
            {
                query += " AND id != @addressId";
            }

            using (SqlConnection conn = new SqlConnection(connect))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    if (addressId > 0)
                    {
                        cmd.Parameters.AddWithValue("@addressId", addressId);
                    }

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateAddress()
        {
            string query = @"UPDATE DiaChi SET 
                           hoten = @hoten, 
                           dienthoai = @dienthoai, 
                           diachi_chitiet = @diachi_chitiet, 
                           tinh_thanh = @tinh_thanh, 
                           quan_huyen = @quan_huyen, 
                           phuong_xa = @phuong_xa, 
                           macdinh = @macdinh,
                           ngay_capnhat = GETDATE()
                           WHERE id = @id AND user_id = @userId";

            using (SqlConnection conn = new SqlConnection(connect))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@hoten", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@dienthoai", txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@diachi_chitiet", txtStreetAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@tinh_thanh", ddlProvince.SelectedItem.Text);
                    cmd.Parameters.AddWithValue("@quan_huyen", ddlDistrict.SelectedItem.Text);
                    cmd.Parameters.AddWithValue("@phuong_xa", ddlWard.SelectedItem.Text);
                    cmd.Parameters.AddWithValue("@macdinh", chkDefaultAddress.Checked);
                    cmd.Parameters.AddWithValue("@id", addressId);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception("Không thể cập nhật địa chỉ. Vui lòng thử lại.");
                    }
                }
            }
        }

        private void InsertAddress()
        {
            string query = @"INSERT INTO DiaChi (user_id, hoten, dienthoai, diachi_chitiet, tinh_thanh, quan_huyen, phuong_xa, macdinh, ngay_tao, ngay_capnhat) 
                           VALUES (@userId, @hoten, @dienthoai, @diachi_chitiet, @tinh_thanh, @quan_huyen, @phuong_xa, @macdinh, GETDATE(), GETDATE())";

            using (SqlConnection conn = new SqlConnection(connect))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@hoten", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@dienthoai", txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@diachi_chitiet", txtStreetAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@tinh_thanh", ddlProvince.SelectedItem.Text);
                    cmd.Parameters.AddWithValue("@quan_huyen", ddlDistrict.SelectedItem.Text);
                    cmd.Parameters.AddWithValue("@phuong_xa", ddlWard.SelectedItem.Text);
                    cmd.Parameters.AddWithValue("@macdinh", chkDefaultAddress.Checked);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (addressId == 0) return;

            try
            {
                string query = "DELETE FROM DiaChi WHERE id = @id AND user_id = @userId";

                using (SqlConnection conn = new SqlConnection(connect))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", addressId);
                        cmd.Parameters.AddWithValue("@userId", userId);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Response.Redirect("TaiKhoan.aspx?tab=addresses&msg=deleted", false);
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert",
                                "alert('Không thể xóa địa chỉ này!');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in btnDelete_Click: " + ex.Message);
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    $"alert('Lỗi khi xóa địa chỉ: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }
    }
}