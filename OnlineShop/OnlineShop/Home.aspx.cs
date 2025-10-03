using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace OnlineShop
{
    public partial class Home : System.Web.UI.Page
    {
        string connect = @"Data Source=localhost;Initial Catalog=SE397BJ;Integrated Security=True;Encrypt=False";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRandomProducts();
            }
        }

        private void LoadRandomProducts()
        {
            using (SqlConnection conn = new SqlConnection(connect))
            {
                try
                {
                    conn.Open();
                    // Thay đổi từ Products sang MatHang và đảm bảo tên cột tương thích
                    string query = "SELECT TOP 21 id_hang AS ProductId, tenhang AS Name, mota AS Description, dongia AS Price, hinhanh AS ImageUrl FROM MatHang ORDER BY NEWID()";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        rptRandomProducts.DataSource = dt;
                        rptRandomProducts.DataBind();
                        // Thêm log để debug
                        Response.Write($"<script>console.log('Số sản phẩm: {dt.Rows.Count}');</script>");
                    }
                    else
                    {
                        // Hiển thị thông báo không có sản phẩm
                        Response.Write("<script>console.log('Không tìm thấy sản phẩm nào.');</script>");
                        Response.Write("<div class='alert alert-warning'>Không tìm thấy sản phẩm nào.</div>");
                    }
                }
                catch (Exception ex)
                {
                    // Hiển thị thông báo lỗi và ghi log
                    Response.Write("<script>console.error('Lỗi khi tải sản phẩm: " + ex.Message + "');</script>");
                    Response.Write("<div class='alert alert-danger'>Lỗi khi tải sản phẩm. Vui lòng thử lại sau.</div>");
                }
            }
        }

        // Event handlers for the sidebar buttons - redirects handled in master page
        protected void btnAll_Click(object sender, EventArgs e)
        {
            Response.Redirect("SanPham.aspx");
        }

        protected void btnSmartphone_Click(object sender, EventArgs e)
        {
            Response.Redirect("SanPham.aspx?category=smartphone");
        }

        protected void btnLaptop_Click(object sender, EventArgs e)
        {
            Response.Redirect("SanPham.aspx?category=laptop");
        }

        protected void btnAccessories_Click(object sender, EventArgs e)
        {
            Response.Redirect("SanPham.aspx?category=accessories");
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int productId = Convert.ToInt32(btn.CommandArgument);

            // Thêm sản phẩm vào giỏ hàng với số lượng là 1
            AddProductToCart(productId, 1);

            // Thông báo cho người dùng
            ScriptManager.RegisterStartupScript(this, GetType(), "AlertMessage", "alert('Đã thêm sản phẩm vào giỏ hàng!');", true);
        }

        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int productId = Convert.ToInt32(btn.CommandArgument);

            // Thêm sản phẩm vào giỏ hàng với số lượng là 1
            AddProductToCart(productId, 1);

            // Chuyển hướng đến trang giỏ hàng
            Response.Redirect("GioHang.aspx");
        }

        private void AddProductToCart(int productId, int quantity)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connect))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT id_hang AS ProductId, tenhang AS ProductName, dongia AS Price, hinhanh AS ImageUrl FROM MatHang WHERE id_hang = @ProductId", conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductId", productId);
                        conn.Open();

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            // Lấy thông tin sản phẩm
                            int pId = reader.GetInt32(reader.GetOrdinal("ProductId"));
                            string pName = reader.GetString(reader.GetOrdinal("ProductName"));
                            decimal pPrice = reader.GetDecimal(reader.GetOrdinal("Price"));
                            string pImage = reader["ImageUrl"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("ImageUrl")) : "~/Images/no-image.jpg";

                            // Lấy giỏ hàng từ session
                            List<CartItem> cartItems;
                            if (Session["Cart"] == null)
                            {
                                cartItems = new List<CartItem>();
                            }
                            else
                            {
                                cartItems = (List<CartItem>)Session["Cart"];
                            }

                            // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
                            CartItem existingItem = cartItems.FirstOrDefault(c => c.ProductId == pId);
                            if (existingItem != null)
                            {
                                // Nếu sản phẩm đã có trong giỏ hàng, tăng số lượng
                                existingItem.Quantity += quantity;
                                existingItem.Total = existingItem.Price * existingItem.Quantity;
                            }
                            else
                            {
                                // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới
                                CartItem newItem = new CartItem
                                {
                                    ProductId = pId,
                                    ProductName = pName,
                                    Price = pPrice,
                                    Quantity = quantity,
                                    Total = pPrice * quantity,
                                    ImageUrl = pImage
                                };
                                cartItems.Add(newItem);
                            }

                            // Lưu giỏ hàng vào session
                            Session["Cart"] = cartItems;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi
                string errorMessage = "Lỗi khi thêm sản phẩm vào giỏ hàng: " + ex.Message;
                ScriptManager.RegisterStartupScript(this, GetType(), "ErrorMessage", $"alert('{errorMessage.Replace("'", "\\'")}');", true);
                System.Diagnostics.Debug.WriteLine(errorMessage);
            }
        }

        protected void rptRandomProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AddToCart")
            {
                int productId = Convert.ToInt32(e.CommandArgument);
                AddProductToCart(productId, 1);
                ScriptManager.RegisterStartupScript(this, GetType(), "AlertMessage", "alert('Đã thêm sản phẩm vào giỏ hàng!');", true);
            }
            else if (e.CommandName == "BuyNow")
            {
                int productId = Convert.ToInt32(e.CommandArgument);
                AddProductToCart(productId, 1);
                Response.Redirect("GioHang.aspx");
            }
            else if (e.CommandName == "ViewDetails")
            {
                int productId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect("ChiTietSanPham.aspx?id=" + productId);
            }
        }
    }
}