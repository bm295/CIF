# Thực hành supervised learning trên CIF

## Bài toán và nhãn

Đơn vị dữ liệu là một lần giới thiệu cặp gói + tiện ích. Nhãn `acceptedWithin7Days`
bằng 1 khi khách hàng chấp nhận trong 7 ngày, bằng 0 khi hết 7 ngày mà không chấp nhận.
Các lượt chưa đủ thời gian quan sát phải bị loại khỏi dữ liệu huấn luyện.
Repo chưa có lịch sử tương tác thực: CSV và mô hình được cung cấp hoàn toàn giả lập.
Không dùng kết quả này làm bằng chứng hiệu quả kinh doanh.

Đặc trưng hiện tại: ngân sách, giá gói, giá tiện ích, tổng giá/ngân sách.
Chưa đưa tương tác quá khứ vào mô hình vì chưa có nguồn sự kiện trong CIF.
Khi có dữ liệu thật, chỉ tổng hợp tương tác trước `offeredAt`, không dùng dữ liệu
sau đề xuất hay trường tiết lộ nhãn. Không dùng tên, email, số giấy tờ hoặc customerId làm đầu vào.

## Chạy thí nghiệm

Từ thư mục gốc repo (Python 3.12):

```bash
python -m venv .venv
. .venv/bin/activate
pip install -r experiments/offer_acceptance/requirements.txt
python -m unittest discover -s experiments/offer_acceptance -p 'test_*.py'
python experiments/offer_acceptance/train.py
```

Seed 42 tạo 4.000 lượt theo ngày, mỗi lượt thuộc một khách giả lập riêng biệt.
Chia theo thời gian ở mốc 70% và 85%; bỏ 7 ngày cuối train và validation để nhãn
đã hoàn tất trước giai đoạn kế tiếp. Số mẫu thực dùng: 2.793 / 593 / 600.
Dữ liệu thực có nhiều lượt trên cùng khách cần chọn cách đánh giá rõ ràng:
khách mới thì tách nhóm khách; khách quay lại thì giữ thứ tự thời gian và chỉ dùng lịch sử sẵn có.

StandardScaler chỉ fit trên train trong pipeline. So sánh logistic regression
với C = 0,01 / 0,1 / 1 / 10 và cây quyết định với độ sâu 1 / 2 / 3 / 5 / 8 / 12 / không giới hạn.
Chọn theo log loss validation; test chỉ được chấm sau khi chốt ứng viên.
Ngưỡng precision/recall cố định 0,5; PR-AUC trong báo cáo là average precision.
Không tối ưu lại theo test. Khi so sánh thêm giả thuyết dựa trên test, cần một tập test mới.

Kết quả nằm trong [báo cáo](../../experiments/offer_acceptance/results/REPORT.md),
`metrics.json`, `synthetic_offers.csv` và `overfitting.png` cùng thư mục.
Cây sâu học thuộc train nhưng log loss validation tăng: giảm độ sâu hoặc tăng regularization
(C nhỏ hơn với logistic) rồi chọn bằng validation. Kết quả giả lập không chứng minh mô hình
sẽ hoạt động tốt với khách thật; cần đánh giá trên lịch sử thật và theo dõi drift trước triển khai.

## Tích hợp API và kiến trúc

`Application` khai báo `IOfferAcceptanceScorer`. `Infrastructure` đọc mô hình JSON
(logistic hoặc cây) xuất từ Python; không thêm phụ thuộc ML vào Domain/Application.
API đăng ký adapter dạng singleton và sao chép artifact khi build/publish.
Mô hình đi kèm chỉ dành cho học tập nên chế độ ML phải được yêu cầu tường minh:

```json
{
  "budget": 250000,
  "basePlans": [
    {"id": "basic", "name": "Basic", "price": 100000},
    {"id": "premium", "name": "Premium", "price": 200000}
  ],
  "addOns": [{"id": "support", "name": "Support", "price": 50000}],
  "useAcceptanceModel": true
}
```

POST `/api/v1/offers/recommendation`. Các cặp vượt ngân sách bị loại trước khi chấm điểm.
ML chọn xác suất cao nhất, hòa điểm thì chọn tổng giá cao hơn, sau đó thứ tự ID hiện có.
Phản hồi thêm `acceptanceProbability`; chế độ mặc định giữ cách chọn tổng giá cao nhất
và trả xác suất null. Ngân sách 0 với gói miễn phí dùng tỷ lệ 0 để tránh chia cho 0.
Giá âm trả HTTP 400. Không có cặp phù hợp trả 404.

Mô hình này dự đoán xác suất chấp nhận, không phải tác động nhân quả hay giá trị khách hàng.
Khi có lịch sử thật, dữ liệu đề xuất được hiển thị có thể thiên lệch; cần thiết kế thu thập
và đánh giá phù hợp trước khi thay đổi chính sách phục vụ.

## Kiểm chứng

```bash
dotnet test CIF.sln
```

CI chạy test .NET, kiểm tra split và xác suất xuất từ sklearn, huấn luyện lại và
kiểm tra artifact/metrics tái lập (sai số tương đối 1e-10, tuyệt đối 1e-12 để chấp nhận sai số làm tròn giữa các nền tảng). Nhãn và dữ liệu giả lập không được ghi vào hồ sơ CIF.
