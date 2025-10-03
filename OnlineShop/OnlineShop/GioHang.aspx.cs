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
    public partial class GioHang : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Kiểm tra xem có productId từ query string không
                string productIdParam = Request.QueryString["productId"];
                if (!string.IsNullOrEmpty(productIdParam))
                {
                    int productId;
                    if (int.TryParse(productIdParam, out productId))
                    {
                        // Thêm sản phẩm vào giỏ hàng
                        AddToCart(productId);

                        // Xóa productId từ URL để tránh thêm lại khi refresh trang
                        Response.Redirect("GioHang.aspx");
                        return;
                    }
                }

                // Tải giỏ hàng
                LoadCartItems();
            }
        }

        private void AddToCart(int productId)
        {
            // Lấy thông tin sản phẩm từ database
            string connect = @"Data Source=localhost;Initial Catalog=SE397BJ;Integrated Security=True;Encrypt=False";
            using (SqlConnection conn = new SqlConnection(connect))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT id_hang AS ProductId, tenhang AS ProductName, 
                           dongia AS Price, hinhanh AS ImageUrl 
                           FROM MatHang WHERE id_hang = @ProductId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Lấy thông tin sản phẩm
                        int pId = reader.GetInt32(reader.GetOrdinal("ProductId"));
                        string pName = reader.GetString(reader.GetOrdinal("ProductName"));
                        decimal pPrice = reader.GetDecimal(reader.GetOrdinal("Price"));
                        string pImage = reader.GetString(reader.GetOrdinal("ImageUrl"));

                        // Lấy giỏ hàng từ session
                        List<CartItem> cartItems;
                        if (Session["Cart"] == null)
                        {
                            cartItems = new List<CartItem>();
                            Session["Cart"] = cartItems;
                        }
                        else
                        {
                            cartItems = (List<CartItem>)Session["Cart"];
                        }

                        // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
                        CartItem existingItem = cartItems.FirstOrDefault(c => c.ProductId == pId);
                        if (existingItem != null)
                        {
                            // Nếu sản phẩm đã có trong giỏ hàng, tăng số lượng lên 1
                            existingItem.Quantity += 1;
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
                                Quantity = 1,
                                Total = pPrice,
                                ImageUrl = pImage
                            };
                            cartItems.Add(newItem);
                        }

                        // Lưu giỏ hàng vào session
                        Session["Cart"] = cartItems;
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    // Sử dụng ScriptManager để hiển thị lỗi
                    string message = "Lỗi khi thêm sản phẩm vào giỏ hàng: " + ex.Message;
                    ScriptManager.RegisterStartupScript(this, GetType(), "ErrorMessage",
                        $"alert('{message.Replace("'", "\\'")}');", true);

                    System.Diagnostics.Debug.WriteLine(message);
                }
            }
        }

        private void LoadCartItems()
        {
            // Get cart from session
            List<CartItem> cartItems = GetCartItems();

            if (cartItems.Count == 0)
            {
                // Show empty cart message
                pnlEmptyCart.Visible = true;
                pnlCart.Visible = false;

                // Reset cart totals when empty
                ResetCartTotals();
            }
            else
            {
                // Show cart with items
                pnlEmptyCart.Visible = false;
                pnlCart.Visible = true;

                // Bind cart items to repeater
                rptCart.DataSource = cartItems;
                rptCart.DataBind();

                // Update cart total
                UpdateCartTotal();
            }
        }

        private List<CartItem> GetCartItems()
        {
            List<CartItem> cartItems;

            if (Session["Cart"] == null)
            {
                cartItems = new List<CartItem>();
                Session["Cart"] = cartItems;
            }
            else
            {
                cartItems = (List<CartItem>)Session["Cart"];
            }

            return cartItems;
        }

        protected void btnContinueShopping_Click(object sender, EventArgs e)
        {
            // Redirect to product list page
            Response.Redirect("SanPham.aspx");
        }

        protected void btnClearCart_Click(object sender, EventArgs e)
        {
            // Clear cart and voucher
            Session["Cart"] = null;
            Session["AppliedVoucherCode"] = null;
            Session["AppliedVoucherValue"] = null;
            Session["AppliedVoucherType"] = null;

            // Reload cart items
            LoadCartItems();
        }

        protected void btnUpdateCart_Click(object sender, EventArgs e)
        {
            // Update quantities
            List<CartItem> cartItems = GetCartItems();

            foreach (RepeaterItem item in rptCart.Items)
            {
                Label lblProductId = (Label)item.FindControl("lblProductId");
                TextBox txtQuantity = (TextBox)item.FindControl("txtQuantity");

                int productId = Convert.ToInt32(lblProductId.Text);
                int quantity = Convert.ToInt32(txtQuantity.Text);

                CartItem cartItem = cartItems.FirstOrDefault(c => c.ProductId == productId);
                if (cartItem != null && quantity > 0)
                {
                    cartItem.Quantity = quantity;
                    cartItem.Total = cartItem.Price * quantity;
                }
            }

            // Save updated cart
            Session["Cart"] = cartItems;

            // Reload cart items
            LoadCartItems();
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                // Redirect to login page with return URL
                Response.Redirect("DangNhap.aspx?returnUrl=ThanhToan.aspx");
            }
            else
            {
                // Proceed to checkout
                Response.Redirect("ThanhToan.aspx");
            }
        }

        protected void rptCart_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                // Get product ID
                int productId = Convert.ToInt32(e.CommandArgument);

                // Remove item from cart
                List<CartItem> cartItems = GetCartItems();
                CartItem itemToRemove = cartItems.FirstOrDefault(c => c.ProductId == productId);

                if (itemToRemove != null)
                {
                    cartItems.Remove(itemToRemove);
                }

                // Save updated cart
                Session["Cart"] = cartItems;

                // Reload cart items
                LoadCartItems();
            }
        }

        // Xử lý áp dụng voucher từ textbox
        protected void btnApplyVoucherCode_Click(object sender, EventArgs e)
        {
            string voucherCode = txtVoucherCode.Text.Trim().ToUpper();

            if (string.IsNullOrEmpty(voucherCode))
            {
                ShowMessage("Vui lòng nhập mã voucher!", "warning");
                return;
            }

            // Validate voucher code
            var voucherInfo = ValidateVoucherCode(voucherCode);

            if (voucherInfo != null)
            {
                // Kiểm tra điều kiện đơn hàng tối thiểu
                List<CartItem> cartItems = GetCartItems();
                decimal subtotal = cartItems.Sum(c => c.Total);

                if (subtotal >= voucherInfo.MinOrderValue)
                {
                    // Store voucher information in session
                    Session["AppliedVoucherCode"] = voucherCode;
                    Session["AppliedVoucherValue"] = voucherInfo.Value;
                    Session["AppliedVoucherType"] = voucherInfo.Type;
                    Session["AppliedVoucherMaxDiscount"] = voucherInfo.MaxDiscount;
                    Session["AppliedVoucherMinOrder"] = voucherInfo.MinOrderValue;

                    // Clear textbox
                    txtVoucherCode.Text = "";

                    // Update cart total
                    UpdateCartTotal();

                    ShowMessage("Áp dụng voucher thành công!", "success");
                }
                else
                {
                    ShowMessage($"Đơn hàng tối thiểu {voucherInfo.MinOrderValue:N0}đ để sử dụng voucher này!", "warning");
                }
            }
            else
            {
                ShowMessage("Mã voucher không hợp lệ hoặc đã hết hạn!", "error");
            }
        }

        // Method to validate voucher code
        private VoucherInfo ValidateVoucherCode(string voucherCode)
        {
            // Dictionary of valid voucher codes with their information
            var validVouchers = new Dictionary<string, VoucherInfo>
            {
                {"TECH500K", new VoucherInfo { Type = "fixed", Value = 500000, MinOrderValue = 5000000, MaxDiscount = 500000 }},
                {"PCTHANKYOU15", new VoucherInfo { Type = "percentage", Value = 15, MinOrderValue = 2000000, MaxDiscount = 300000 }},
                {"WELCOME200", new VoucherInfo { Type = "fixed", Value = 200000, MinOrderValue = 1000000, MaxDiscount = 200000 }},
                {"FREESHIP25", new VoucherInfo { Type = "shipping", Value = 30000, MinOrderValue = 0, MaxDiscount = 30000 }},
                {"LAPTOP20", new VoucherInfo { Type = "percentage", Value = 20, MinOrderValue = 0, MaxDiscount = 2000000 }},
                {"PHONE100K", new VoucherInfo { Type = "fixed", Value = 100000, MinOrderValue = 5000000, MaxDiscount = 100000 }},
                {"ACCESSORY150", new VoucherInfo { Type = "fixed", Value = 150000, MinOrderValue = 500000, MaxDiscount = 150000 }},
                {"MONITOR10", new VoucherInfo { Type = "percentage", Value = 10, MinOrderValue = 0, MaxDiscount = 500000 }},
                {"VIP1000K", new VoucherInfo { Type = "fixed", Value = 1000000, MinOrderValue = 10000000, MaxDiscount = 1000000 }},
                {"APP50K", new VoucherInfo { Type = "fixed", Value = 50000, MinOrderValue = 0, MaxDiscount = 50000 }}
            };

            return validVouchers.ContainsKey(voucherCode.ToUpper()) ? validVouchers[voucherCode.ToUpper()] : null;
        }

        private void UpdateCartTotal()
        {
            // Lấy danh sách các mặt hàng trong giỏ hàng
            List<CartItem> cartItems = GetCartItems();

            // Tính tổng tiền của tất cả mặt hàng (mỗi mặt hàng = đơn giá * số lượng)
            decimal subtotal = cartItems.Sum(c => c.Total);

            // Lấy thông tin voucher áp dụng
            decimal discountAmount = 0;
            decimal shippingFee = 30000;

            if (Session["AppliedVoucherCode"] != null)
            {
                string voucherType = Session["AppliedVoucherType"]?.ToString() ?? "";
                decimal voucherValue = Convert.ToDecimal(Session["AppliedVoucherValue"] ?? 0);
                decimal maxDiscount = Convert.ToDecimal(Session["AppliedVoucherMaxDiscount"] ?? 0);
                decimal minOrderValue = Convert.ToDecimal(Session["AppliedVoucherMinOrder"] ?? 0);

                // Kiểm tra điều kiện đơn hàng tối thiểu
                if (subtotal >= minOrderValue)
                {
                    switch (voucherType.ToLower())
                    {
                        case "fixed":
                            discountAmount = voucherValue;
                            break;
                        case "percentage":
                            discountAmount = Math.Min((subtotal * voucherValue / 100), maxDiscount);
                            break;
                        case "shipping":
                            shippingFee = Math.Max(0, shippingFee - voucherValue);
                            discountAmount = Math.Min(voucherValue, 30000); // Hiển thị discount cho shipping
                            break;
                    }
                }
            }

            // Tính tổng cộng = subtotal - discountAmount + shippingFee
            decimal finalTotal = Math.Max(0, subtotal - discountAmount + shippingFee);

            // Cập nhật các Label controls trong sidebar
            lblSubtotal.Text = string.Format("{0:N0}đ", subtotal);
            lblDiscount.Text = string.Format("{0:N0}đ", discountAmount);
            lblShipping.Text = string.Format("{0:N0}đ", shippingFee);
            lblFinalTotal.Text = string.Format("{0:N0}đ", finalTotal);

            // Cập nhật lblTotal trong main content
            lblTotal.Text = string.Format("{0:N0}đ", finalTotal);
        }

        private void ResetCartTotals()
        {
            // Reset all labels
            lblSubtotal.Text = "0đ";
            lblDiscount.Text = "0đ";
            lblShipping.Text = "30,000đ";
            lblFinalTotal.Text = "30,000đ";
            lblTotal.Text = "0đ";
        }

        // Helper method to show messages
        private void ShowMessage(string message, string type)
        {
            string alertClass = "";
            switch (type)
            {
                case "success":
                    alertClass = "alert-success";
                    break;
                case "warning":
                    alertClass = "alert-warning";
                    break;
                case "error":
                    alertClass = "alert-danger";
                    break;
                default:
                    alertClass = "alert-info";
                    break;
            }

            string script = $@"
                var alertDiv = document.createElement('div');
                alertDiv.className = 'alert {alertClass} alert-dismissible fade show';
                alertDiv.innerHTML = '{message.Replace("'", "\\'")} <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert""></button>';
                
                var container = document.querySelector('.container');
                if (container) {{
                    container.insertBefore(alertDiv, container.firstChild);
                    
                    // Auto hide after 5 seconds
                    setTimeout(function() {{
                        if (alertDiv.parentNode) {{
                            alertDiv.parentNode.removeChild(alertDiv);
                        }}
                    }}, 5000);
                }}
            ";

            ClientScript.RegisterStartupScript(this.GetType(), "ShowMessage", script, true);
        }

        // Method to apply voucher - call this from JavaScript (for voucher cards)
        [System.Web.Services.WebMethod]
        public static string ApplyVoucherFromCard(string voucherCode)
        {
            try
            {
                // Validate voucher code and get voucher details
                var voucherInfo = ValidateVoucherCodeStatic(voucherCode);

                if (voucherInfo != null)
                {
                    // Get cart items from session
                    var cartItems = GetCartItemsStatic();
                    decimal subtotal = cartItems.Sum(c => c.Total);

                    if (subtotal >= voucherInfo.MinOrderValue)
                    {
                        // Store voucher information in session
                        HttpContext.Current.Session["AppliedVoucherCode"] = voucherCode;
                        HttpContext.Current.Session["AppliedVoucherValue"] = voucherInfo.Value;
                        HttpContext.Current.Session["AppliedVoucherType"] = voucherInfo.Type;
                        HttpContext.Current.Session["AppliedVoucherMaxDiscount"] = voucherInfo.MaxDiscount;
                        HttpContext.Current.Session["AppliedVoucherMinOrder"] = voucherInfo.MinOrderValue;

                        return "success|Áp dụng voucher thành công!";
                    }
                    else
                    {
                        return $"error|Đơn hàng tối thiểu {voucherInfo.MinOrderValue:N0}đ để sử dụng voucher này!";
                    }
                }

                return "error|Mã voucher không hợp lệ hoặc đã hết hạn!";
            }
            catch (Exception ex)
            {
                return "error|Có lỗi xảy ra khi áp dụng voucher!";
            }
        }

        // Static methods for WebMethod calls
        private static VoucherInfo ValidateVoucherCodeStatic(string voucherCode)
        {
            var validVouchers = new Dictionary<string, VoucherInfo>
            {
                {"TECH500K", new VoucherInfo { Type = "fixed", Value = 500000, MinOrderValue = 5000000, MaxDiscount = 500000 }},
                {"PCTHANKYOU15", new VoucherInfo { Type = "percentage", Value = 15, MinOrderValue = 2000000, MaxDiscount = 300000 }},
                {"WELCOME200", new VoucherInfo { Type = "fixed", Value = 200000, MinOrderValue = 1000000, MaxDiscount = 200000 }},
                {"FREESHIP25", new VoucherInfo { Type = "shipping", Value = 30000, MinOrderValue = 0, MaxDiscount = 30000 }},
                {"LAPTOP20", new VoucherInfo { Type = "percentage", Value = 20, MinOrderValue = 0, MaxDiscount = 2000000 }},
                {"PHONE100K", new VoucherInfo { Type = "fixed", Value = 100000, MinOrderValue = 5000000, MaxDiscount = 100000 }},
                {"ACCESSORY150", new VoucherInfo { Type = "fixed", Value = 150000, MinOrderValue = 500000, MaxDiscount = 150000 }},
                {"MONITOR10", new VoucherInfo { Type = "percentage", Value = 10, MinOrderValue = 0, MaxDiscount = 500000 }},
                {"VIP1000K", new VoucherInfo { Type = "fixed", Value = 1000000, MinOrderValue = 10000000, MaxDiscount = 1000000 }},
                {"APP50K", new VoucherInfo { Type = "fixed", Value = 50000, MinOrderValue = 0, MaxDiscount = 50000 }}
            };

            return validVouchers.ContainsKey(voucherCode.ToUpper()) ? validVouchers[voucherCode.ToUpper()] : null;
        }

        private static List<CartItem> GetCartItemsStatic()
        {
            List<CartItem> cartItems;

            if (HttpContext.Current.Session["Cart"] == null)
            {
                cartItems = new List<CartItem>();
                HttpContext.Current.Session["Cart"] = cartItems;
            }
            else
            {
                cartItems = (List<CartItem>)HttpContext.Current.Session["Cart"];
            }

            return cartItems;
        }

        // Method to remove applied voucher
        [System.Web.Services.WebMethod]
        public static string RemoveVoucher()
        {
            try
            {
                // Clear voucher information from session
                HttpContext.Current.Session["AppliedVoucherCode"] = null;
                HttpContext.Current.Session["AppliedVoucherValue"] = null;
                HttpContext.Current.Session["AppliedVoucherType"] = null;
                HttpContext.Current.Session["AppliedVoucherMaxDiscount"] = null;
                HttpContext.Current.Session["AppliedVoucherMinOrder"] = null;

                return "success|Đã hủy voucher thành công!";
            }
            catch
            {
                return "error|Có lỗi xảy ra khi hủy voucher!";
            }
        }
    }

    // CartItem class to store cart item information
    [Serializable]
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public string ImageUrl { get; set; }
        public List<Gift> Gifts { get; set; }

        public CartItem()
        {
            Gifts = new List<Gift>();
        }
    }

    // Gift class to store gift information
    [Serializable]
    public class Gift
    {
        public int GiftId { get; set; }
        public string GiftName { get; set; }
    }

    // VoucherInfo class to store voucher details
    [Serializable]
    public class VoucherInfo
    {
        public string Type { get; set; } // "fixed", "percentage", "shipping"
        public decimal Value { get; set; }
        public decimal MinOrderValue { get; set; }
        public decimal MaxDiscount { get; set; }
    }
}