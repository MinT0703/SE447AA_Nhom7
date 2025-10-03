<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GioHang.aspx.cs" Inherits="OnlineShop.GioHang" MasterPageFile="~/Site1.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Giỏ Hàng - Radian Shop</title>
    <style>
        /* Voucher Styling */
        .voucher-container {
            margin-bottom: var(--space-md);
        }

        .voucher-item {
            position: relative;
            background: linear-gradient(to right, var(--primary-light), var(--secondary-light));
            border-radius: var(--radius-md);
            padding: var(--space-md);
            margin-bottom: var(--space-sm);
            display: flex;
            flex-direction: column;
            overflow: hidden;
            box-shadow: var(--shadow-sm);
            border-left: 4px solid var(--primary);
            transition: all var(--transition-normal);
            cursor: pointer;
        }

            .voucher-item:hover {
                transform: translateY(-2px);
                box-shadow: var(--shadow-md);
            }

            .voucher-item::before {
                content: '';
                position: absolute;
                right: -10px;
                top: 50%;
                transform: translateY(-50%);
                width: 20px;
                height: 20px;
                background-color: white;
                border-radius: 50%;
                box-shadow: inset 0 0 5px rgba(0,0,0,0.1);
            }

            .voucher-item::after {
                content: '';
                position: absolute;
                left: -10px;
                top: 50%;
                transform: translateY(-50%);
                width: 20px;
                height: 20px;
                background-color: white;
                border-radius: 50%;
                box-shadow: inset 0 0 5px rgba(0,0,0,0.1);
            }

        .voucher-value {
            font-size: var(--font-size-lg);
            font-weight: 700;
            color: var(--primary-dark);
            margin-bottom: var(--space-xs);
        }

        .voucher-code {
            background-color: rgba(255, 255, 255, 0.7);
            border: 1px dashed var(--primary);
            border-radius: var(--radius-sm);
            padding: var(--space-xs) var(--space-sm);
            font-family: monospace;
            font-weight: 600;
            font-size: var(--font-size-sm);
            letter-spacing: 1px;
            color: var(--primary-dark);
            margin: var(--space-xs) 0;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

            .voucher-code span {
                flex: 1;
                white-space: nowrap;
                overflow: hidden;
                text-overflow: ellipsis;
            }

        .copy-btn {
            background-color: var(--primary);
            color: white;
            border: none;
            border-radius: var(--radius-sm);
            padding: 2px 6px;
            font-size: var(--font-size-xs);
            cursor: pointer;
            transition: all var(--transition-fast);
            margin-left: var(--space-xs);
        }

            .copy-btn:hover {
                background-color: var(--primary-dark);
            }

        .voucher-condition {
            font-size: var(--font-size-xs);
            color: var(--text-dark);
            margin-top: var(--space-xs);
        }

        .voucher-expiry {
            font-size: var(--font-size-xs);
            color: #ff6b6b;
            margin-top: var(--space-xs);
            font-weight: 600;
        }

        .voucher-premium {
            border-left: 4px solid #FFD700;
            background: linear-gradient(to right, #FFEAC1, #FFF8E7);
        }

            .voucher-premium .voucher-value {
                color: #B7791F;
            }

        .voucher-new {
            position: absolute;
            top: 10px;
            right: 10px;
            background-color: #ff6b6b;
            color: white;
            font-size: var(--font-size-xs);
            padding: 2px 8px;
            border-radius: var(--radius-sm);
            transform: rotate(15deg);
            font-weight: 600;
        }

        .apply-voucher-btn {
            background: linear-gradient(to right, var(--primary), var(--primary-dark));
            color: white;
            border: none;
            border-radius: var(--radius-md);
            padding: var(--space-sm) var(--space-md);
            font-weight: 600;
            cursor: pointer;
            margin-top: var(--space-sm);
            width: 100%;
            transition: all var(--transition-normal);
        }

            .apply-voucher-btn:hover {
                background: linear-gradient(to right, var(--primary-dark), var(--primary));
                transform: translateY(-2px);
                box-shadow: var(--shadow-md);
            }

        .voucher-search {
            position: relative;
            margin-bottom: var(--space-md);
        }

            .voucher-search input {
                width: 100%;
                padding: var(--space-sm) var(--space-lg) var(--space-sm) var(--space-sm);
                border: 1px solid var(--border-light);
                border-radius: var(--radius-md);
                font-size: var(--font-size-sm);
            }

            .voucher-search button {
                position: absolute;
                right: 0;
                top: 0;
                height: 100%;
                padding: 0 var(--space-sm);
                background: var(--primary);
                color: white;
                border: none;
                border-top-right-radius: var(--radius-md);
                border-bottom-right-radius: var(--radius-md);
            }

        /* Cart specific styles */
        .cart-summary, .order-summary {
            background-color: rgba(255, 255, 255, 0.9);
            border-radius: var(--radius-md);
            padding: var(--space-md);
            margin-top: var(--space-lg);
            box-shadow: var(--shadow-sm);
        }

        .cart-summary-title {
            font-size: var(--font-size-md);
            font-weight: 700;
            margin-bottom: var(--space-md);
            color: var(--primary-dark);
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .cart-summary-item, .summary-row {
            display: flex;
            justify-content: space-between;
            padding: var(--space-xs) 0;
            border-bottom: 1px dashed var(--border-light);
        }

        .cart-summary-total, .total-row {
            display: flex;
            justify-content: space-between;
            padding: var(--space-md) 0;
            font-weight: 700;
            font-size: var(--font-size-lg);
            color: var(--primary);
            border-top: 2px solid var(--primary);
            margin-top: var(--space-sm);
        }

        .discount {
            color: #4CAF50;
            font-weight: 600;
        }

        /* Cart table styling */
        .cart-table {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: var(--space-lg);
        }

            .cart-table th,
            .cart-table td {
                padding: var(--space-sm);
                text-align: left;
                border-bottom: 1px solid var(--border-light);
            }

            .cart-table th {
                background-color: var(--bg-light);
                font-weight: 600;
                color: var(--text-dark);
            }

            .cart-table tfoot td {
                font-weight: 600;
                border-top: 2px solid var(--primary);
            }

        .cart-img {
            max-width: 120px; /* chiều rộng tối đa */
            max-height: 120px; /* chiều cao tối đa */
            object-fit: contain; /* giữ tỷ lệ, không méo ảnh */
            display: block;
            margin: auto; /* căn giữa */
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="SidebarContent" runat="server">
    <h3 class="sidebar-section-title">Mã Giảm Giá</h3>

    <div class="voucher-search">
        <asp:TextBox ID="txtVoucherCode" runat="server" placeholder="Nhập mã voucher..." CssClass="form-control"></asp:TextBox>
        <asp:Button ID="btnApplyVoucherCode" runat="server" Text="Áp dụng" OnClick="btnApplyVoucherCode_Click" />
    </div>

    <div class="voucher-container">
        <div class="voucher-item voucher-premium">
            <div class="voucher-new">HOT</div>
            <div class="voucher-value">Giảm 500,000đ</div>
            <div class="voucher-code">
                <span>TECH500K</span>
                <button class="copy-btn" onclick="copyVoucherCode(this)">Sao chép</button>
            </div>
            <div class="voucher-condition">Áp dụng cho đơn hàng từ 5,000,000đ</div>
            <div class="voucher-expiry">Hết hạn: 30/10/2025</div>
        </div>

        <div class="voucher-item">
            <div class="voucher-value">Giảm 15%</div>
            <div class="voucher-code">
                <span>PCTHANKYOU15</span>
                <button class="copy-btn" onclick="copyVoucherCode(this)">Sao chép</button>
            </div>
            <div class="voucher-condition">Tối đa 300,000đ cho đơn từ 2,000,000đ</div>
            <div class="voucher-expiry">Hết hạn: 25/10/2025</div>
        </div>

        <div class="voucher-item">
            <div class="voucher-new">MỚI</div>
            <div class="voucher-value">Giảm 200,000đ</div>
            <div class="voucher-code">
                <span>WELCOME200</span>
                <button class="copy-btn" onclick="copyVoucherCode(this)">Sao chép</button>
            </div>
            <div class="voucher-condition">Cho khách hàng mới, đơn từ 1,000,000đ</div>
            <div class="voucher-expiry">Hết hạn: 31/10/2025</div>
        </div>

        <div class="voucher-item">
            <div class="voucher-value">Miễn phí vận chuyển</div>
            <div class="voucher-code">
                <span>FREESHIP25</span>
                <button class="copy-btn" onclick="copyVoucherCode(this)">Sao chép</button>
            </div>
            <div class="voucher-condition">Áp dụng cho mọi đơn hàng</div>
            <div class="voucher-expiry">Hết hạn: 28/10/2025</div>
        </div>

        <div class="voucher-item voucher-premium">
            <div class="voucher-value">Giảm 20%</div>
            <div class="voucher-code">
                <span>LAPTOP20</span>
                <button class="copy-btn" onclick="copyVoucherCode(this)">Sao chép</button>
            </div>
            <div class="voucher-condition">Dành cho sản phẩm laptop, tối đa 2,000,000đ</div>
            <div class="voucher-expiry">Hết hạn: 15/10/2025</div>
        </div>

        <div class="voucher-item">
            <div class="voucher-value">Giảm 100,000đ</div>
            <div class="voucher-code">
                <span>PHONE100K</span>
                <button class="copy-btn" onclick="copyVoucherCode(this)">Sao chép</button>
            </div>
            <div class="voucher-condition">Cho điện thoại, đơn từ 5,000,000đ</div>
            <div class="voucher-expiry">Hết hạn: 25/10/2025</div>
        </div>

        <div class="voucher-item">
            <div class="voucher-value">Giảm 150,000đ</div>
            <div class="voucher-code">
                <span>ACCESSORY150</span>
                <button class="copy-btn" onclick="copyVoucherCode(this)">Sao chép</button>
            </div>
            <div class="voucher-condition">Cho phụ kiện, đơn từ 500,000đ</div>
            <div class="voucher-expiry">Hết hạn: 10/11/2025</div>
        </div>

        <div class="voucher-item">
            <div class="voucher-value">Giảm 10%</div>
            <div class="voucher-code">
                <span>MONITOR10</span>
                <button class="copy-btn" onclick="copyVoucherCode(this)">Sao chép</button>
            </div>
            <div class="voucher-condition">Dành cho màn hình, tối đa 500,000đ</div>
            <div class="voucher-expiry">Hết hạn: 20/11/2025</div>
        </div>

        <div class="voucher-item voucher-premium">
            <div class="voucher-value">Giảm 1,000,000đ</div>
            <div class="voucher-code">
                <span>VIP1000K</span>
                <button class="copy-btn" onclick="copyVoucherCode(this)">Sao chép</button>
            </div>
            <div class="voucher-condition">Dành cho khách hàng VIP, đơn từ 10,000,000đ</div>
            <div class="voucher-expiry">Hết hạn: 30/11/2025</div>
        </div>

        <div class="voucher-item">
            <div class="voucher-value">Giảm 50,000đ</div>
            <div class="voucher-code">
                <span>APP50K</span>
                <button class="copy-btn" onclick="copyVoucherCode(this)">Sao chép</button>
            </div>
            <div class="voucher-condition">Đặt hàng qua ứng dụng</div>
            <div class="voucher-expiry">Hết hạn: 15/11/2025</div>
        </div>
    </div>

    <h3 class="sidebar-section-title">Tóm Tắt Đơn Hàng</h3>
    <div class="order-summary">
        <div class="summary-row">
            <span>Tạm tính:</span>
            <span>
                <asp:Label ID="lblSubtotal" runat="server">0đ</asp:Label></span>
        </div>
        <div class="summary-row">
            <span>Giảm giá:</span>
            <span class="discount">-<asp:Label ID="lblDiscount" runat="server">0đ</asp:Label></span>
        </div>
        <div class="summary-row">
            <span>Phí vận chuyển:</span>
            <span>
                <asp:Label ID="lblShipping" runat="server">50,000đ</asp:Label></span>
        </div>
        <div class="summary-row total-row">
            <span><strong>Tổng cộng:</strong></span>
            <span><strong>
                <asp:Label ID="lblFinalTotal" runat="server">0đ</asp:Label></strong></span>
        </div>
    </div>

    <asp:Button ID="btnCheckoutSidebar" runat="server" Text="Tiến hành thanh toán" CssClass="apply-voucher-btn" OnClick="btnCheckout_Click" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4 mb-4">Giỏ hàng của bạn</h2>

        <asp:Panel ID="pnlEmptyCart" runat="server" Visible="false">
            <div class="alert alert-info">
                <h4>Giỏ hàng trống</h4>
                <p>Bạn chưa có sản phẩm nào trong giỏ hàng.</p>
                <asp:Button ID="btnShopNow" runat="server" Text="Mua sắm ngay" CssClass="btn btn-primary" OnClick="btnContinueShopping_Click" />
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlCart" runat="server">
            <div class="table-responsive">
                <table class="table table-bordered cart-table">
                    <thead>
                        <tr>
                            <th>Sản phẩm</th>
                            <th>Đơn giá</th>
                            <th>Số lượng</th>
                            <th>Thành tiền</th>
                            <th>Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptCart" runat="server" OnItemCommand="rptCart_ItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblProductId" runat="server" Text='<%# Eval("ProductId") %>' Visible="false"></asp:Label>
                                        <div class="d-flex align-items-center">
                                            <asp:Image ID="imgCartItem" runat="server" ImageUrl='<%# Eval("ImageUrl") %>' AlternateText="Ảnh sản phẩm" CssClass="cart-img" />
                                            <div class="ms-3">
                                                <h5><%# Eval("ProductName") %></h5>
                                                <div class="gift-items">
                                                    <asp:Repeater ID="rptGifts" runat="server" DataSource='<%# Eval("Gifts") %>'>
                                                        <ItemTemplate>
                                                            <div class="gift-item">
                                                                <span class="badge bg-success">Quà tặng</span>
                                                                <span><%# Eval("GiftName") %></span>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                            </div>
                                        </div>
                                    </td>
                                    <td><%# string.Format("{0:N0} VNĐ", Eval("Price")) %></td>
                                    <td>
                                        <div class="quantity-control d-flex align-items-center">
                                            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="decreaseQuantity(this)">-</button>
                                            <asp:TextBox ID="txtQuantity" runat="server" Text='<%# Eval("Quantity") %>' CssClass="form-control form-control-sm text-center mx-2" Width="60px"></asp:TextBox>
                                            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="increaseQuantity(this)">+</button>
                                        </div>
                                    </td>
                                    <td><%# string.Format("{0:N0} VNĐ", Eval("Total")) %></td>
                                    <td>
                                        <asp:LinkButton ID="btnRemove" runat="server" CssClass="btn btn-sm btn-danger" CommandName="Remove" CommandArgument='<%# Eval("ProductId") %>'>
                                            <i class="fa fa-trash"></i> Xóa
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan="3" class="text-end">Tổng cộng:</td>
                            <td colspan="2">
                                <strong>
                                    <asp:Label ID="lblTotal" runat="server"></asp:Label></strong>
                            </td>
                        </tr>
                    </tfoot>
                </table>
            </div>

            <div class="d-flex justify-content-between mt-4">
                <div>
                    <asp:Button ID="btnContinueShopping" runat="server" Text="Tiếp tục mua sắm" CssClass="btn btn-outline-primary" OnClick="btnContinueShopping_Click" />
                    <asp:Button ID="btnClearCart" runat="server" Text="Xóa giỏ hàng" CssClass="btn btn-outline-danger ms-2" OnClick="btnClearCart_Click" OnClientClick="return confirm('Bạn có chắc muốn xóa tất cả sản phẩm trong giỏ hàng?');" />
                </div>
                <div>
                    <asp:Button ID="btnUpdateCart" runat="server" Text="Cập nhật giỏ hàng" CssClass="btn btn-outline-secondary me-2" OnClick="btnUpdateCart_Click" />
                    <asp:Button ID="btnCheckout" runat="server" Text="Thanh toán" CssClass="btn btn-primary" OnClick="btnCheckout_Click" />
                </div>
            </div>
        </asp:Panel>
    </div>
    <script type="text/javascript">
        function increaseQuantity(button) {
            var quantityTextBox = button.previousElementSibling;
            var currentValue = parseInt(quantityTextBox.value);
            if (!isNaN(currentValue)) {
                quantityTextBox.value = currentValue + 1;
            }
        }

        function decreaseQuantity(button) {
            var quantityTextBox = button.nextElementSibling;
            var currentValue = parseInt(quantityTextBox.value);
            if (!isNaN(currentValue) && currentValue > 1) {
                quantityTextBox.value = currentValue - 1;
            }
        }

        function copyVoucherCode(button) {
            // Tìm mã voucher trong phần tử cha
            var codeElement = button.previousElementSibling;
            var code = codeElement.textContent;

            // Tạo input tạm thời để copy
            var tempInput = document.createElement("input");
            tempInput.value = code;
            document.body.appendChild(tempInput);
            tempInput.select();
            document.execCommand("copy");
            document.body.removeChild(tempInput);

            // Thay đổi nút sao chép để phản hồi người dùng
            var originalText = button.textContent;
            button.textContent = "Đã sao chép!";
            button.style.backgroundColor = "#4CAF50";

            // Khôi phục lại sau 2 giây
            setTimeout(function () {
                button.textContent = originalText;
                button.style.backgroundColor = "";
            }, 2000);
        }

        // Function to apply voucher from voucher card
        function applyVoucherFromCard(voucherCode) {
            // Show loading state
            showLoadingMessage("Đang áp dụng voucher...");

            // Call WebMethod to apply voucher
            PageMethods.ApplyVoucherFromCard(voucherCode,
                function (result) {
                    hideLoadingMessage();

                    var parts = result.split('|');
                    var status = parts[0];
                    var message = parts[1];

                    if (status === 'success') {
                        showMessage(message, 'success');
                        // Reload page to update totals
                        setTimeout(function () {
                            window.location.reload();
                        }, 1500);
                    } else {
                        showMessage(message, 'error');
                    }
                },
                function (error) {
                    hideLoadingMessage();
                    showMessage("Có lỗi xảy ra khi áp dụng voucher!", 'error');
                }
            );
        }

        // Function to remove applied voucher
        function removeAppliedVoucher() {
            if (confirm('Bạn có chắc muốn hủy voucher đã áp dụng?')) {
                showLoadingMessage("Đang hủy voucher...");

                PageMethods.RemoveVoucher(
                    function (result) {
                        hideLoadingMessage();

                        var parts = result.split('|');
                        var status = parts[0];
                        var message = parts[1];

                        if (status === 'success') {
                            showMessage(message, 'success');
                            // Reload page to update totals
                            setTimeout(function () {
                                window.location.reload();
                            }, 1500);
                        } else {
                            showMessage(message, 'error');
                        }
                    },
                    function (error) {
                        hideLoadingMessage();
                        showMessage("Có lỗi xảy ra khi hủy voucher!", 'error');
                    }
                );
            }
        }

        // Helper functions for UI feedback
        function showMessage(message, type) {
            var alertClass = "";
            switch (type) {
                case 'success':
                    alertClass = "alert-success";
                    break;
                case 'warning':
                    alertClass = "alert-warning";
                    break;
                case 'error':
                    alertClass = "alert-danger";
                    break;
                default:
                    alertClass = "alert-info";
                    break;
            }

            var alertDiv = document.createElement('div');
            alertDiv.className = 'alert ' + alertClass + ' alert-dismissible fade show';
            alertDiv.innerHTML = message + ' <button type="button" class="btn-close" data-bs-dismiss="alert"></button>';

            var container = document.querySelector('.container');
            if (container) {
                container.insertBefore(alertDiv, container.firstChild);

                // Auto hide after 5 seconds
                setTimeout(function () {
                    if (alertDiv.parentNode) {
                        alertDiv.parentNode.removeChild(alertDiv);
                    }
                }, 5000);
            }
        }

        function showLoadingMessage(message) {
            var loadingDiv = document.createElement('div');
            loadingDiv.id = 'loading-message';
            loadingDiv.className = 'alert alert-info';
            loadingDiv.innerHTML = '<i class="fa fa-spinner fa-spin"></i> ' + message;

            var container = document.querySelector('.container');
            if (container) {
                container.insertBefore(loadingDiv, container.firstChild);
            }
        }

        function hideLoadingMessage() {
            var loadingDiv = document.getElementById('loading-message');
            if (loadingDiv && loadingDiv.parentNode) {
                loadingDiv.parentNode.removeChild(loadingDiv);
            }
        }

        // Add click event to voucher cards
        document.addEventListener('DOMContentLoaded', function () {
            // Add click event to apply voucher buttons on voucher cards
            var voucherCards = document.querySelectorAll('.voucher-item');
            voucherCards.forEach(function (card) {
                card.addEventListener('click', function (e) {
                    // Don't trigger if copy button was clicked
                    if (e.target.classList.contains('copy-btn')) {
                        return;
                    }

                    var codeElement = card.querySelector('.voucher-code span');
                    if (codeElement) {
                        var voucherCode = codeElement.textContent.trim();
                        applyVoucherFromCard(voucherCode);
                    }
                });
            });

            // Add visual feedback for hoverable voucher cards
            voucherCards.forEach(function (card) {
                var originalTitle = card.title || '';
                card.title = originalTitle + (originalTitle ? ' - ' : '') + 'Click để áp dụng voucher';
            });
        });
    </script>
</asp:Content>
