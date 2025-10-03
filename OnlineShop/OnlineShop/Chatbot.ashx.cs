using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

// ... phần using vẫn giữ nguyên

namespace OnlineShop
{
    public class Chatbot : IHttpHandler, IRequiresSessionState
    {
        private const string Ollama_Endpoint = "http://127.0.0.1:11434";
        private const string Ollama_Model = "llama3:8b";
        private static readonly JavaScriptSerializer Json = new JavaScriptSerializer();

        // ====== FAQ: định nghĩa nhanh các câu hỏi thường gặp ======
        // Mỗi FAQ gồm: mảng từ khóa (không dấu càng tốt) và câu trả lời gọn.
        // Lưu ý: đã có normalize, nên bạn có thể viết có dấu/không dấu đều được.
        private static readonly List<(string[] keys, string answer)> FAQs = new List<(string[] keys, string answer)>
        {
            (new[] { "doi tra", "bao nhieu ngay" }, "Bạn được đổi trả trong 30 ngày nếu sản phẩm còn nguyên vẹn, đầy đủ phụ kiện & hóa đơn."),
            (new[] { "bao hanh", "may", "thang" },   "Tất cả sản phẩm được bảo hành chính hãng 12 tháng tại TTBH uỷ quyền."),
            (new[] { "freeship", "giao hang" },      "Đơn từ 500.000đ freeship nội thành. Toàn quốc 2-3 ngày làm việc."),
            (new[] { "thanh toan", "phuong thuc" },  "Hỗ trợ COD, chuyển khoản, ví điện tử và trả góp qua thẻ tín dụng."),
            (new[] { "ho tro", "lien he" },          "Bạn cần hỗ trợ? Gọi 0974 981 811 (8:00–21:00) hoặc chat ngay tại đây.")
        };

        // ====== Chuẩn hoá: bỏ dấu, hạ chữ, loại ký tự thừa ======
        private static string Normalize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = s.Trim().ToLowerInvariant();
            s = RemoveDiacritics(s);
            // thay nhiều khoảng trắng về 1
            s = System.Text.RegularExpressions.Regex.Replace(s, @"\s+", " ");
            return s;
        }
        private static string RemoveDiacritics(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var normalizedString = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        // ====== Trích keyword cơ bản từ câu người dùng ======
        // Mục tiêu: lấy ra cụm dễ so khớp sản phẩm (iphone 12, macbook air, samsung a55, asus vivobook,…)
        private static string ExtractKeywords(string input)
        {
            var t = Normalize(input);
            // bỏ các hư từ hay gặp
            var stop = new HashSet<string> { "toi", "minh", "mua", "can", "muon", "co", "khong", "la", "cai", "cua", "o", "tai", "cho", "hang", "san pham" };
            var parts = t.Split(' ');
            var kept = new List<string>();
            foreach (var w in parts)
            {
                if (string.IsNullOrWhiteSpace(w)) continue;
                if (stop.Contains(w)) continue;
                kept.Add(w);
            }
            // ghép lại để đưa vào LIKE
            return string.Join(" ", kept).Trim();
        }

        // ====== So khớp FAQ: tất cả keyword trong mảng phải xuất hiện trong câu hỏi ======
        private static bool TryAnswerFAQ(string userMessage, out string answer)
        {
            var text = Normalize(userMessage);
            foreach (var faq in FAQs)
            {
                bool allHit = true;
                foreach (var k in faq.keys)
                {
                    var nk = Normalize(k);
                    if (!text.Contains(nk)) { allHit = false; break; }
                }
                if (allHit)
                {
                    answer = faq.answer;
                    return true;
                }
            }
            answer = null;
            return false;
        }

        public void ProcessRequest(HttpContext ctx)
        {
            ctx.Response.ContentType = "application/json; charset=utf-8";
            try
            {
                string body;
                using (var r = new StreamReader(ctx.Request.InputStream)) body = r.ReadToEnd();
                if (string.IsNullOrWhiteSpace(body)) { JsonError(ctx, 400, "Empty body"); return; }

                var input = Json.DeserializeObject(body) as Dictionary<string, object>;
                var action = input != null && input.ContainsKey("action") ? (input["action"] ?? "").ToString() : "";
                var userMessage = input != null && input.ContainsKey("message") ? (input["message"] ?? "").ToString() : "";

                // (A) API trả FAQ cho UI (tuỳ chọn)
                if (action == "faqs")
                {
                    var faqs = new List<object>();
                    foreach (var f in FAQs)
                        faqs.Add(new { keywords = f.keys, answer = f.answer });
                    ctx.Response.Write(Json.Serialize(new { ok = true, faqs }));
                    ctx.ApplicationInstance.CompleteRequest();
                    return;
                }

                // (B) API chào (tuỳ chọn) – nếu bạn muốn lấy tên user từ session
                if (action == "greet")
                {
                    var name = (ctx.Session["UserName"] ?? "bạn").ToString();
                    var greet = $"Xin chào {name}! Mình là Radian Bot. Mình có thể giúp gì cho bạn?";
                    ctx.Response.Write(Json.Serialize(new { ok = true, reply = greet }));
                    ctx.ApplicationInstance.CompleteRequest();
                    return;
                }

                // (C) Chat thông thường
                if (string.IsNullOrWhiteSpace(userMessage))
                {
                    JsonError(ctx, 400, "Missing 'message'"); return;
                }

                long sessionId = GetOrCreateChatSession(ctx);
                InsertChatMessage(sessionId, "user", userMessage);

                // 1) Thử trả lời FAQ trước (không gọi LLM nếu khớp)
                if (TryAnswerFAQ(userMessage, out var faqAnswer))
                {
                    InsertChatMessage(sessionId, "assistant", faqAnswer);
                    UpdateSessionTouch(sessionId);
                    ctx.Response.Write(Json.Serialize(new { ok = true, reply = faqAnswer, sessionId }));
                    ctx.ApplicationInstance.CompleteRequest();
                    return;
                }

                // 2) Không khớp FAQ => tìm SP theo từ khóa đã chuẩn hoá
                var keyword = ExtractKeywords(userMessage);
                var products = FindProducts(keyword, 8);
                var catalog = BuildCatalogContext(products, max: 8, baseUrl: "Details.aspx?id=");

                var systemPrompt =
                    "Bạn là trợ lý Radian Shop. Chỉ dùng dữ liệu trong CATALOG bên dưới. Khi nói chuyện với khách thay CATALOG bằng kho" +
                    "Nếu khách yêu cầu hỗ trợ, hỏi khách cần hỗ trợ gì và đưa ra số điện thoại chăm sóc khách hàng (0974981811)."+
                    "Nếu không có sản phẩm phù hợp, đề nghị khách viết chính xác tên sản phẩm; nếu vẫn không có, xin lỗi và đề xuất kết nối nhân viên CSKH; Khong được phép tự tạo ra sản phẩm hay đưa ra sản phẩm không có trong Kho hàng " +
                    "Trả lời ngắn gọn, chỉ được trả lời bằng tiếng Việt, không bằng Tiếng Anh (English), tối đa 50 từ. Ưu tiên sản phẩm còn hàng.\n" +
                    catalog;

                var payload = new
                {
                    model = Ollama_Model,
                    messages = new object[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user",   content = userMessage }
                    },
                    stream = false
                };
                var reqJson = Json.Serialize(payload);

                string replyText;
                using (var http = new HttpClient() { Timeout = TimeSpan.FromSeconds(60) })
                {
                    http.BaseAddress = new Uri(Ollama_Endpoint);
                    var res = http.PostAsync("/api/chat", new StringContent(reqJson, Encoding.UTF8, "application/json")).Result;
                    var resText = res.Content.ReadAsStringAsync().Result;

                    if (!res.IsSuccessStatusCode)
                    {
                        JsonError(ctx, 502, "Upstream (Ollama) error", resText); return;
                    }

                    dynamic obj = Json.DeserializeObject(resText);
                    replyText = obj != null && obj.ContainsKey("message") && obj["message"] != null
                              ? (obj["message"] as Dictionary<string, object>)["content"].ToString()
                              : "(không có nội dung)";
                }

                InsertChatMessage(sessionId, "assistant", replyText);
                UpdateSessionTouch(sessionId);

                ctx.Response.Write(Json.Serialize(new { ok = true, reply = replyText, sessionId }));
                ctx.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (Exception ex)
            {
                JsonError(ctx, 500, "Handler exception", ex.Message); return;
            }
        }

        private string ConnStr =>
            ConfigurationManager.ConnectionStrings["OnlineShopDB"].ConnectionString;

        // ====== Tìm sản phẩm (giữ nguyên cấu trúc của bạn, chỉ thay đầu vào = keyword đã chuẩn hoá) ======
        private sealed class ProductRow
        {
            public int id_hang { get; set; }
            public int id_loai { get; set; }
            public string tenhang { get; set; }
            public string donvitinh { get; set; }
            public decimal dongia { get; set; }
            public int soluongton { get; set; }
            public string mota { get; set; }
            public string tinhtrang { get; set; }
            public string hinhanh { get; set; }
            public string tenloai { get; set; }
        }

        private List<ProductRow> FindProducts(string query, int top)
        {
            var list = new List<ProductRow>();
            string sql = @"
SELECT TOP (@top)
    m.id_hang, m.id_loai, m.tenhang, m.donvitinh, m.dongia, m.soluongton, m.mota, m.tinhtrang, m.hinhanh,
    l.tenloai
FROM MatHang m
JOIN LoaiHang l ON l.id_loai = m.id_loai
WHERE
    (@q = N'' 
     OR m.tenhang LIKE N'%' + @q + N'%'
     OR m.mota    LIKE N'%' + @q + N'%'
     OR l.tenloai LIKE N'%' + @q + N'%')
ORDER BY 
    CASE WHEN m.soluongton > 0 THEN 0 ELSE 1 END,
    m.id_hang DESC;";

            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@top", top);
                cmd.Parameters.Add("@q", SqlDbType.NVarChar, 200).Value = (object)(query ?? "");
                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new ProductRow
                        {
                            id_hang = rd.GetInt32(0),
                            id_loai = rd.GetInt32(1),
                            tenhang = rd.GetString(2),
                            donvitinh = rd.IsDBNull(3) ? "" : rd.GetString(3),
                            dongia = rd.GetDecimal(4),
                            soluongton = rd.IsDBNull(5) ? 0 : rd.GetInt32(5),
                            mota = rd.IsDBNull(6) ? "" : rd.GetString(6),
                            tinhtrang = rd.IsDBNull(7) ? "" : rd.GetString(7),
                            hinhanh = rd.IsDBNull(8) ? "" : rd.GetString(8),
                            tenloai = rd.GetString(9)
                        });
                    }
                }
            }
            return list;
        }

        private static string ToVnd(decimal money)
        {
            return string.Format(new CultureInfo("vi-VN"), "{0:N0} ₫", money);
        }

        private static string BuildCatalogContext(List<ProductRow> items, int max = 8, string baseUrl = "Details.aspx?id=")
        {
            var sb = new StringBuilder();
            sb.AppendLine("CATALOG_START");
            int i = 0;
            foreach (var p in items)
            {
                if (i++ >= max) break;
                string link = $"{baseUrl}{p.id_hang}";
                sb.AppendLine($"- id:{p.id_hang} | {p.tenhang} | Giá:{ToVnd(p.dongia)} | Tồn:{p.soluongton} | Loại:{p.tenloai}");
                sb.AppendLine($"  Link:{link}");
            }
            sb.AppendLine("CATALOG_END");
            return sb.ToString();
        }

        private long GetOrCreateChatSession(HttpContext ctx)
        {
            var aspId = ctx.Session.SessionID;

            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM ChatSession WHERE AspNetSessionId=@sid AND Status='active')
BEGIN
    UPDATE ChatSession SET LastActivityAt = SYSUTCDATETIME()
    OUTPUT inserted.SessionId
    WHERE AspNetSessionId=@sid AND Status='active';
END
ELSE
BEGIN
    INSERT INTO ChatSession(AspNetSessionId) OUTPUT inserted.SessionId VALUES(@sid);
END
", con))
            {
                cmd.Parameters.Add("@sid", SqlDbType.NVarChar, 100).Value = aspId;
                con.Open();
                var id = (long)cmd.ExecuteScalar();
                ctx.Session["ChatSessionId"] = id;
                return id;
            }
        }

        private void UpdateSessionTouch(long sessionId)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "UPDATE ChatSession SET LastActivityAt = SYSUTCDATETIME() WHERE SessionId=@id", con))
            {
                cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = sessionId;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void InsertChatMessage(long sessionId, string role, string content)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "INSERT INTO ChatMessage(SessionId, Role, Content) VALUES(@sid, @role, @content)", con))
            {
                cmd.Parameters.Add("@sid", SqlDbType.BigInt).Value = sessionId;
                cmd.Parameters.Add("@role", SqlDbType.NVarChar, 20).Value = role;
                cmd.Parameters.Add("@content", SqlDbType.NVarChar).Value = content ?? "";
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void JsonError(HttpContext ctx, int status, string message, string detail = null)
        {
            ctx.Response.StatusCode = status;
            ctx.Response.Write(Json.Serialize(new { ok = false, error = message, detail }));
            ctx.ApplicationInstance.CompleteRequest();
        }

        public bool IsReusable => false;
    }
}

