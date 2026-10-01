---
name: pre-commit-review
description: Review thay đổi chưa commit trước khi commit. Chỉ chạy khi người dùng gõ /pre-commit-review.
disable-model-invocation: true
---

## File đang thay đổi

!`git status --short`

## Chỉ dẫn

### Bước 1: Liệt kê file
Liệt kê từng file ở trên, mỗi file kèm nhãn: modified, added, deleted hoặc untracked.
Với file untracked, chỉ nêu tên và nói rõ bạn chưa đọc nội dung, không đoán nó là gì.

### Bước 2: Chạy test
Chạy `dotnet test`. Báo lại đúng kết quả thật: số test passed, failed, skipped.
Nếu có test fail, nêu tên test fail và thông báo lỗi, rồi dừng lại, không đi tiếp.
Không được nói "test pass" nếu bạn chưa thấy output của lần chạy này.

### Bước 3: Quét secret
Chạy `git diff HEAD` và chỉ xét các dòng được thêm vào (bắt đầu bằng `+`).
Tìm các dấu hiệu: password, secret, apikey, api_key, token, connectionstring,
"Bearer ", "BEGIN PRIVATE KEY", chuỗi dài giống khóa.
- Nếu thấy: nêu tên file và SỐ DÒNG TRONG FILE MỚI (lấy từ phần @@ của hunk),
  KHÔNG in lại giá trị nghi là secret.
- Nếu không thấy: nói "không thấy dấu hiệu theo các từ khóa trên" và nêu rõ
  rằng đây chỉ là quét từ khóa, không đảm bảo không có secret.
- File untracked không được quét vì chưa đọc nội dung; hãy nói rõ điều này.
- Nếu thấy dấu hiệu secret: dừng lại, không đi tiếp sang Bước 4.

### Bước 4: Trình kế hoạch commit
Chạy `git status -uall -- .claude` để biết chính xác từng file bên trong `.claude/`.
Rồi trình kế hoạch gồm:
- Danh sách file ĐỀ XUẤT đưa vào commit (kèm lý do ngắn).
- Danh sách file đề xuất KHÔNG đưa vào (ví dụ thư mục chưa đọc, hoặc file cá nhân
  như settings.local.json), kèm lý do.
- Một thông điệp commit đề xuất, theo dạng `type: mô tả ngắn`.
Kết thúc bằng câu hỏi: "OK để commit theo kế hoạch này không?" rồi DỪNG và chờ.

### Quy tắc chung
Không chạy lệnh commit hay push. Chỉ trình kế hoạch và chờ người dùng xác nhận.