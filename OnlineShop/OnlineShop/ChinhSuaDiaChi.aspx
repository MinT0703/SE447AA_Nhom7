<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.master" AutoEventWireup="true" CodeBehind="ChinhSuaDiaChi.aspx.cs" Inherits="OnlineShop.ChinhSuaDiaChi" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Sửa Địa Chỉ - Radian Shop</title>
    <meta name="description" content="Chỉnh sửa địa chỉ giao hàng và thanh toán của bạn." />
    <style>
        /* Styles dành riêng cho trang Sửa Địa Chỉ */
        .edit-address-container {
            background-color: rgba(255, 255, 255, 0.95);
            border-radius: var(--radius-md);
            padding: var(--space-lg);
            margin-bottom: var(--space-lg);
            box-shadow: var(--shadow-sm);
            max-width: 800px;
            margin: 0 auto var(--space-lg);
        }

        .edit-address-header {
            display: flex;
            align-items: center;
            margin-bottom: var(--space-lg);
            padding-bottom: var(--space-md);
            border-bottom: 2px solid var(--primary-light);
        }

        .back-button {
            background: none;
            border: none;
            color: var(--primary);
            font-size: 1.5rem;
            margin-right: var(--space-md);
            cursor: pointer;
            padding: var(--space-xs);
            border-radius: var(--radius-sm);
            transition: all var(--transition-normal);
        }

            .back-button:hover {
                background-color: var(--bg-light);
                color: var(--primary-dark);
            }

        .edit-address-title {
            font-size: var(--font-size-xl);
            color: var(--primary-dark);
            margin: 0;
        }

        .edit-address-subtitle {
            color: var(--text-medium);
            font-size: var(--font-size-md);
            margin-top: var(--space-xs);
        }

        .form-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: var(--space-md);
            margin-bottom: var(--space-md);
        }

            .form-grid.single-column {
                grid-template-columns: 1fr;
            }

        .form-group {
            margin-bottom: var(--space-md);
        }

            .form-group label {
                display: block;
                margin-bottom: var(--space-xs);
                font-weight: 600;
                color: var(--text-dark);
            }

                .form-group label.required::after {
                    content: " *";
                    color: #f44336;
                }

        .form-control {
            width: 100%;
            padding: var(--space-sm);
            border: 1px solid var(--border-light);
            border-radius: var(--radius-sm);
            font-size: var(--font-size-md);
            transition: all var(--transition-fast);
            box-sizing: border-box;
        }

            .form-control:focus {
                border-color: var(--primary);
                box-shadow: 0 0 0 2px rgba(0, 119, 182, 0.2);
                outline: none;
            }

            .form-control.error {
                border-color: #f44336;
                box-shadow: 0 0 0 2px rgba(244, 67, 54, 0.2);
            }

        .error-message {
            color: #f44336;
            font-size: var(--font-size-sm);
            margin-top: var(--space-xs);
            display: none;
        }

        .default-address-option {
            display: flex;
            align-items: center;
            margin-bottom: var(--space-lg);
            padding: var(--space-md);
            background-color: var(--bg-light);
            border-radius: var(--radius-md);
            border: 1px solid var(--border-light);
        }

            .default-address-option input[type="checkbox"] {
                margin-right: var(--space-sm);
                transform: scale(1.2);
            }

            .default-address-option label {
                margin: 0;
                font-weight: 600;
                color: var(--text-dark);
                cursor: pointer;
            }

        .default-address-note {
            font-size: var(--font-size-sm);
            color: var(--text-medium);
            margin-top: var(--space-xs);
        }

        .form-actions {
            display: flex;
            gap: var(--space-md);
            justify-content: flex-end;
            margin-top: var(--space-xl);
            padding-top: var(--space-lg);
            border-top: 1px solid var(--border-light);
        }

        .btn-cancel {
            background-color: white;
            border: 2px solid var(--border-light);
            color: var(--text-medium);
            padding: var(--space-sm) var(--space-lg);
            border-radius: var(--radius-md);
            font-weight: 600;
            cursor: pointer;
            transition: all var(--transition-normal);
            text-decoration: none;
            display: inline-block;
            text-align: center;
        }

            .btn-cancel:hover {
                border-color: var(--primary);
                color: var(--primary);
                text-decoration: none;
            }

        .btn-save {
            background: linear-gradient(to right, var(--primary), var(--primary-dark));
            color: white;
            border: none;
            padding: var(--space-sm) var(--space-xl);
            border-radius: var(--radius-md);
            font-weight: 600;
            cursor: pointer;
            transition: all var(--transition-normal);
            display: inline-block;
            text-align: center;
        }

            .btn-save:hover {
                background: linear-gradient(to right, var(--primary-light), var(--primary));
                transform: translateY(-2px);
                box-shadow: var(--shadow-md);
            }

        .btn-delete {
            background-color: white;
            border: 2px solid #f44336;
            color: #f44336;
            padding: var(--space-sm) var(--space-lg);
            border-radius: var(--radius-md);
            font-weight: 600;
            cursor: pointer;
            transition: all var(--transition-normal);
            display: inline-block;
            text-align: center;
        }

            .btn-delete:hover {
                background-color: #f44336;
                color: white;
            }

        .address-preview {
            background-color: var(--bg-light);
            border-radius: var(--radius-md);
            padding: var(--space-md);
            margin-bottom: var(--space-lg);
            border: 1px solid var(--border-light);
        }

            .address-preview h4 {
                margin: 0 0 var(--space-sm);
                color: var(--primary-dark);
                font-size: var(--font-size-md);
            }

        .address-preview-content {
            color: var(--text-medium);
            font-size: var(--font-size-sm);
            line-height: 1.6;
        }

        /* Responsive design */
        @media (max-width: 768px) {
            .edit-address-container {
                margin: 0 var(--space-sm) var(--space-lg);
                padding: var(--space-md);
            }

            .form-grid {
                grid-template-columns: 1fr;
            }

            .form-actions {
                flex-direction: column;
            }

            .btn-cancel,
            .btn-save,
            .btn-delete {
                width: 100%;
                margin-bottom: var(--space-sm);
            }

            .edit-address-header {
                flex-direction: column;
                align-items: flex-start;
                text-align: left;
            }

            .back-button {
                margin-bottom: var(--space-sm);
                margin-right: 0;
            }
        }

        .loading {
            opacity: 0.6;
            pointer-events: none;
        }

        .spinner {
            display: inline-block;
            width: 20px;
            height: 20px;
            border: 3px solid rgba(255,255,255,.3);
            border-radius: 50%;
            border-top-color: #fff;
            animation: spin 1s ease-in-out infinite;
            margin-right: var(--space-xs);
        }

        @keyframes spin {
            to {
                transform: rotate(360deg);
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="NewsContent" runat="server">
    <marquee behavior="scroll" direction="left">
        Cập nhật địa chỉ giao hàng để nhận được sản phẩm nhanh chóng và chính xác nhất!
    </marquee>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="SidebarContent" runat="server">
    <h2 class="sidebar-section-title">TÀI KHOẢN</h2>
    <a href="TaiKhoan.aspx?tab=info" class="menu-item">Thông Tin Tài Khoản</a>
    <a href="TaiKhoan.aspx?tab=orders" class="menu-item">Đơn Hàng Của Tôi</a>
    <a href="TaiKhoan.aspx?tab=addresses" class="menu-item active">Sổ Địa Chỉ</a>
    <a href="TaiKhoan.aspx?tab=security" class="menu-item">Bảo Mật</a>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <nav aria-label="breadcrumb">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="Default.aspx">Trang Chủ</a></li>
            <li class="breadcrumb-item"><a href="TaiKhoan.aspx">Tài Khoản</a></li>
            <li class="breadcrumb-item active" aria-current="page">Sửa Địa Chỉ</li>
        </ol>
    </nav>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="edit-address-container">
        <div class="edit-address-header">
            <button type="button" class="back-button" onclick="history.back();">
                <i class="fas fa-arrow-left"></i>
            </button>
            <div>
                <h1 class="edit-address-title">
                    <asp:Literal ID="PageTitle" runat="server" Text="Sửa Địa Chỉ"></asp:Literal>
                </h1>
                <p class="edit-address-subtitle">Cập nhật thông tin địa chỉ giao hàng của bạn</p>
            </div>
        </div>

        <!-- Preview địa chỉ hiện tại -->
        <div class="address-preview" id="addressPreview" style="display: none;">
            <h4>Xem trước địa chỉ:</h4>
            <div class="address-preview-content" id="previewContent">
                <!-- Nội dung preview sẽ được cập nhật bằng JavaScript -->
            </div>
        </div>
        <div class="form-grid">
            <div class="form-group">
                <label for="txtFullName" class="required">Họ và tên</label>
                <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Nhập họ và tên" MaxLength="100"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFullName" runat="server" ControlToValidate="txtFullName"
                    ErrorMessage="Vui lòng nhập họ và tên" CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <label for="txtPhone" class="required">Số điện thoại</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="Nhập số điện thoại" MaxLength="15"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="txtPhone"
                    ErrorMessage="Vui lòng nhập số điện thoại" CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revPhone" runat="server" ControlToValidate="txtPhone"
                    ErrorMessage="Số điện thoại không hợp lệ" ValidationExpression="^[0-9]{10,11}$" CssClass="error-message" Display="Dynamic"></asp:RegularExpressionValidator>
            </div>
        </div>

        <div class="form-group">
            <label for="txtStreetAddress" class="required">Địa chỉ cụ thể</label>
            <asp:TextBox ID="txtStreetAddress" runat="server" CssClass="form-control" placeholder="Số nhà, tên đường" MaxLength="200"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvStreetAddress" runat="server" ControlToValidate="txtStreetAddress"
                ErrorMessage="Vui lòng nhập địa chỉ cụ thể" CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>

        <div class="form-grid">
            <div class="form-group">
                <label for="ddlProvince" class="required">Tỉnh/Thành phố</label>
                <asp:DropDownList ID="ddlProvince" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlProvince_SelectedIndexChanged">
                    <asp:ListItem Text="Chọn Tỉnh/Thành phố" Value=""></asp:ListItem>
                    <asp:ListItem Text="TP. Hồ Chí Minh" Value="79"></asp:ListItem>
                    <asp:ListItem Text="Hà Nội" Value="01"></asp:ListItem>
                    <asp:ListItem Text="Đà Nẵng" Value="48"></asp:ListItem>
                    <asp:ListItem Text="Cần Thơ" Value="92"></asp:ListItem>
                    <asp:ListItem Text="Hải Phòng" Value="31"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvProvince" runat="server" ControlToValidate="ddlProvince"
                    ErrorMessage="Vui lòng chọn Tỉnh/Thành phố" CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <label for="ddlDistrict" class="required">Quận/Huyện</label>
                <asp:DropDownList ID="ddlDistrict" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDistrict_SelectedIndexChanged">
                    <asp:ListItem Text="Chọn Quận/Huyện" Value=""></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvDistrict" runat="server" ControlToValidate="ddlDistrict"
                    ErrorMessage="Vui lòng chọn Quận/Huyện" CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
        </div>

        <div class="form-group">
            <label for="ddlWard" class="required">Phường/Xã</label>
            <asp:DropDownList ID="ddlWard" runat="server" CssClass="form-control">
                <asp:ListItem Text="Chọn Phường/Xã" Value=""></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvWard" runat="server" ControlToValidate="ddlWard"
                ErrorMessage="Vui lòng chọn Phường/Xã" CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>

        <div class="default-address-option">
            <asp:CheckBox ID="chkDefaultAddress" runat="server" />
            <label for="<%= chkDefaultAddress.ClientID %>">Đặt làm địa chỉ mặc định</label>
            <div class="default-address-note">
                Địa chỉ mặc định sẽ được sử dụng cho các đơn hàng tiếp theo
            </div>
        </div>

        <div class="form-actions">
            <asp:Button ID="btnDelete" runat="server" Text="Xóa Địa Chỉ" CssClass="btn-delete"
                OnClick="btnDelete_Click" OnClientClick="return confirm('Bạn có chắc chắn muốn xóa địa chỉ này?');"
                Visible="false" />
            <a href="TaiKhoan.aspx?tab=addresses" class="btn-cancel">Hủy</a>
            <asp:Button ID="btnSave" runat="server" Text="Lưu Địa Chỉ" CssClass="btn-save" OnClick="btnSave_Click" />
        </div>
    </div>

    <script type="text/javascript">
        // Preview địa chỉ khi người dùng nhập
        function updateAddressPreview() {
            var fullName = document.getElementById('<%= txtFullName.ClientID %>').value;
            var phone = document.getElementById('<%= txtPhone.ClientID %>').value;
            var streetAddress = document.getElementById('<%= txtStreetAddress.ClientID %>').value;
            var ward = document.getElementById('<%= ddlWard.ClientID %>').options[document.getElementById('<%= ddlWard.ClientID %>').selectedIndex].text;
            var district = document.getElementById('<%= ddlDistrict.ClientID %>').options[document.getElementById('<%= ddlDistrict.ClientID %>').selectedIndex].text;
            var province = document.getElementById('<%= ddlProvince.ClientID %>').options[document.getElementById('<%= ddlProvince.ClientID %>').selectedIndex].text;

            if (fullName && streetAddress && ward !== 'Chọn Phường/Xã' && district !== 'Chọn Quận/Huyện' && province !== 'Chọn Tỉnh/Thành phố') {
                var previewContent = fullName + '<br>' + streetAddress + '<br>' + ward + ', ' + district + '<br>' + province;
                if (phone) {
                    previewContent += '<br>Điện thoại: ' + phone;
                }

                document.getElementById('previewContent').innerHTML = previewContent;
                document.getElementById('addressPreview').style.display = 'block';
            } else {
                document.getElementById('addressPreview').style.display = 'none';
            }
        }

        // Đăng ký sự kiện
        document.addEventListener('DOMContentLoaded', function () {
            var inputs = [
                '<%= txtFullName.ClientID %>',
                '<%= txtPhone.ClientID %>',
                '<%= txtStreetAddress.ClientID %>',
                '<%= ddlWard.ClientID %>',
                '<%= ddlDistrict.ClientID %>',
                '<%= ddlProvince.ClientID %>'
            ];

            inputs.forEach(function (inputId) {
                var element = document.getElementById(inputId);
                if (element) {
                    element.addEventListener('input', updateAddressPreview);
                    element.addEventListener('change', updateAddressPreview);
                }
            });

            // Hiển thị preview ban đầu nếu có dữ liệu
            updateAddressPreview();
        });

        // Xử lý loading state cho button save
        function showLoading(button) {
            button.innerHTML = '<span class="spinner"></span>Đang lưu...';
            button.disabled = true;
            button.classList.add('loading');
        }

        // Validate form trước khi submit
        function validateForm() {
            var isValid = true;
            var errors = [];

            // Kiểm tra các trường bắt buộc
            var requiredFields = [
                { id: '<%= txtFullName.ClientID %>', name: 'Họ và tên' },
                { id: '<%= txtPhone.ClientID %>', name: 'Số điện thoại' },
                { id: '<%= txtStreetAddress.ClientID %>', name: 'Địa chỉ cụ thể' },
                { id: '<%= ddlProvince.ClientID %>', name: 'Tỉnh/Thành phố' },
                { id: '<%= ddlDistrict.ClientID %>', name: 'Quận/Huyện' },
                { id: '<%= ddlWard.ClientID %>', name: 'Phường/Xã' }
            ];

            requiredFields.forEach(function (field) {
                var element = document.getElementById(field.id);
                if (!element.value || element.value.trim() === '' ||
                    (element.tagName === 'SELECT' && element.selectedIndex === 0)) {
                    element.classList.add('error');
                    errors.push(field.name + ' không được để trống');
                    isValid = false;
                } else {
                    element.classList.remove('error');
                }
            });

            // Kiểm tra định dạng số điện thoại
            var phonePattern = /^[0-9]{10,11}$/;
            var phoneElement = document.getElementById('<%= txtPhone.ClientID %>');
            if (phoneElement.value && !phonePattern.test(phoneElement.value)) {
                phoneElement.classList.add('error');
                errors.push('Số điện thoại không hợp lệ');
                isValid = false;
            }

            if (!isValid) {
                alert('Vui lòng kiểm tra lại thông tin:\n' + errors.join('\n'));
            }

            return isValid;
        }
    </script>
</asp:Content>
