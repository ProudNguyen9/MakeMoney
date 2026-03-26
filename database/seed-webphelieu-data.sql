USE WebPheLieu;
GO

SET NOCOUNT ON;
GO

-- Xóa dữ liệu cũ để seed lại an toàn
DELETE FROM blog_post_products;
DELETE FROM blog_post_categories;
DELETE FROM price_history;
DELETE FROM product_images;
DELETE FROM blog_posts;
DELETE FROM blog_categories;
DELETE FROM products;
DELETE FROM product_categories;
DELETE FROM admins;
GO

DBCC CHECKIDENT ('price_history', RESEED, 0);
DBCC CHECKIDENT ('product_images', RESEED, 0);
DBCC CHECKIDENT ('blog_posts', RESEED, 0);
DBCC CHECKIDENT ('blog_categories', RESEED, 0);
DBCC CHECKIDENT ('products', RESEED, 0);
DBCC CHECKIDENT ('product_categories', RESEED, 0);
DBCC CHECKIDENT ('admins', RESEED, 0);
GO

DECLARE @Now DATETIME = GETDATE();

INSERT INTO admins (username, password_hash, full_name, role, status, created_at, updated_at)
VALUES
('thanhtrung', '123456', N'Thành Trung', 'super_admin', 'active', @Now, @Now);

INSERT INTO blog_categories (name, slug, description, created_at, updated_at)
VALUES
(N'Giá thu mua', 'gia-thu-mua', N'Cập nhật giá phế liệu mới nhất theo ngày, theo tuần và theo khu vực.', @Now, @Now),
(N'Kinh nghiệm bán phế liệu', 'kinh-nghiem-ban-phe-lieu', N'Hướng dẫn phân loại, lưu kho và bán phế liệu được giá cao.', @Now, @Now),
(N'Tin thị trường', 'tin-thi-truong', N'Tổng hợp biến động thị trường sắt thép, đồng, nhôm, inox, nhựa và giấy.', @Now, @Now),
(N'Tư vấn doanh nghiệp', 'tu-van-doanh-nghiep', N'Giải pháp thu gom phế liệu cho nhà xưởng, công trình và doanh nghiệp.', @Now, @Now),
(N'Mẹo phân loại', 'meo-phan-loai', N'Các mẹo nhận biết và phân loại phế liệu nhanh, chính xác, dễ bán.', @Now, @Now);

INSERT INTO product_categories (name, slug, description, created_at, updated_at)
VALUES
(N'Kim loại màu', 'kim-loai-mau', N'Nhóm kim loại có giá trị cao như đồng, nhôm, inox, chì, kẽm, thiếc.', @Now, @Now),
(N'Sắt thép', 'sat-thep', N'Nhóm sắt thép phế liệu từ công trình, nhà xưởng, máy móc, dân dụng.', @Now, @Now),
(N'Nhựa phế liệu', 'nhua-phe-lieu', N'Các loại nhựa công nghiệp và dân dụng có thể tái chế.', @Now, @Now),
(N'Giấy phế liệu', 'giay-phe-lieu', N'Giấy carton, giấy báo, giấy văn phòng, giấy hỗn hợp thu mua số lượng lớn.', @Now, @Now),
(N'Thiết bị - máy móc', 'thiet-bi-may-moc', N'Máy móc cũ, xác nhà xưởng, dây chuyền sản xuất thanh lý.', @Now, @Now),
(N'Vải - linh kiện khác', 'vai-linh-kien-khac', N'Các nhóm vải vụn, bo mạch, cáp điện, linh kiện điện tử và vật tư khác.', @Now, @Now);

INSERT INTO products (
    name, slug, short_description, description, category_id,
    price_value, unit, price_label, primary_image, status, is_featured, created_at, updated_at
)
VALUES
(N'Đồng đỏ phế liệu', 'dong-do-phe-lieu', N'Thu mua đồng đỏ sạch, đồng dây, đồng tấm, đồng thanh số lượng lớn giá cao.', N'Đồng đỏ phế liệu là nhóm kim loại màu có giá trị cao, thường phát sinh từ dây điện, thanh cái, motor, máy biến áp và xưởng cơ khí. Chúng tôi thu mua tận nơi, cân điện tử minh bạch, thanh toán nhanh trong ngày.', 1, 185000, N'kg', N'185.000 đ/kg', NULL, 'active', 1, @Now, @Now),
(N'Đồng vàng phế liệu', 'dong-vang-phe-lieu', N'Thu mua đồng vàng từ van nước, phụ kiện, tiện đồng, bột đồng.', N'Đồng vàng phế liệu thường phát sinh từ ngành nước, cơ khí chính xác và đồ gia dụng cũ. Chúng tôi nhận thu mua đồng vàng lẫn tạp và đồng vàng sạch với giá cạnh tranh.', 1, 128000, N'kg', N'128.000 đ/kg', NULL, 'active', 1, @Now, @Now),
(N'Nhôm đặc phế liệu', 'nhom-dac-phe-lieu', N'Thu mua nhôm đặc, nhôm thanh, nhôm tấm, nhôm công nghiệp giá tốt.', N'Nhôm đặc phế liệu phát sinh nhiều tại công trình, xưởng nhôm kính, cơ khí và nội thất. Hàng được phân loại kỹ giúp khách bán được giá cao hơn.', 1, 52000, N'kg', N'52.000 đ/kg', NULL, 'active', 1, @Now, @Now),
(N'Nhôm xingfa phế liệu', 'nhom-xingfa-phe-lieu', N'Thu mua nhôm xingfa cửa cũ, nhôm định hình, nhôm hệ công trình.', N'Nhôm xingfa có giá trị tái chế cao nhờ tỷ lệ nhôm tốt, ít tạp chất. Chúng tôi nhận thu mua tại nhà dân, chung cư, công trình và xưởng thi công.', 1, 58000, N'kg', N'58.000 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Inox 304 phế liệu', 'inox-304-phe-lieu', N'Thu mua inox 304 tấm, ống, bồn, bàn ghế, hàng gia dụng cũ.', N'Inox 304 phế liệu có giá cao hơn các loại inox thông thường nhờ hàm lượng niken ổn định. Chúng tôi hỗ trợ kiểm tra chủng loại và báo giá rõ ràng.', 1, 36000, N'kg', N'36.000 đ/kg', NULL, 'active', 1, @Now, @Now),
(N'Inox 201 phế liệu', 'inox-201-phe-lieu', N'Thu mua inox 201 từ bàn ghế, kệ, ống, phụ kiện inox dân dụng.', N'Inox 201 là nhóm hàng phổ biến tại nhà hàng, gia đình, xưởng sản xuất. Giá thu mua phụ thuộc độ sạch, khối lượng và vị trí bốc xếp.', 1, 22000, N'kg', N'22.000 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Chì phế liệu', 'chi-phe-lieu', N'Thu mua chì bản cực, chì tấm, chì công nghiệp và chì từ thiết bị cũ.', N'Chúng tôi thu mua chì phế liệu phát sinh từ nhà máy, cơ sở điện, viễn thông và hệ thống lưu điện. Quy trình thu gom an toàn, hỗ trợ vận chuyển.', 1, 42000, N'kg', N'42.000 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Cáp điện phế liệu', 'cap-dien-phe-lieu', N'Thu mua cáp điện công trình, dây điện lõi đồng, dây cáp cũ các loại.', N'Cáp điện phế liệu được định giá theo lõi kim loại, độ dày vỏ và tỷ lệ đồng. Chúng tôi nhận cả cáp điện công trình, cáp điện lực và dây điện dân dụng số lượng lớn.', 6, 97000, N'kg', N'97.000 đ/kg', NULL, 'active', 1, @Now, @Now),
(N'Sắt đặc phế liệu', 'sat-dac-phe-lieu', N'Thu mua sắt đặc, sắt cây, sắt bản mã, sắt công nghiệp nặng.', N'Sắt đặc có tỷ trọng cao nên giá thu mua thường tốt hơn sắt pha tạp. Chúng tôi nhận bốc xếp tại công trình, nhà xưởng và kho thanh lý.', 2, 10500, N'kg', N'10.500 đ/kg', NULL, 'active', 1, @Now, @Now),
(N'Sắt vụn công trình', 'sat-vun-cong-trinh', N'Thu mua sắt vụn từ tháo dỡ nhà xưởng, nhà tiền chế, công trình xây dựng.', N'Bao gồm sắt hộp, sắt hình, sắt giàn giáo, thép cây dư thừa và xác kết cấu tháo dỡ. Đội xe cẩu, xe tải hỗ trợ thu gom nhanh tại công trình.', 2, 9200, N'kg', N'9.200 đ/kg', NULL, 'active', 1, @Now, @Now),
(N'Thép phế liệu', 'thep-phe-lieu', N'Thu mua thép tấm, thép hình, thép chế tạo, thép công nghiệp giá cao.', N'Chúng tôi thu mua thép phế liệu từ xưởng cơ khí, nhà máy, kho vật tư và công trình. Có báo giá theo lô và hỗ trợ tháo dỡ khi cần.', 2, 9800, N'kg', N'9.800 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Máy móc cũ thanh lý', 'may-moc-cu-thanh-ly', N'Thu mua máy móc cũ, dây chuyền sản xuất, thiết bị công nghiệp ngừng hoạt động.', N'Nhận thu mua máy ép, máy tiện, máy cắt, dây chuyền đóng gói, bồn bể, khung sườn và thiết bị nhà xưởng. Khảo sát tận nơi, báo giá nhanh, thu mua gọn.', 5, 0, N'lô', N'Giá thương lượng', NULL, 'active', 1, @Now, @Now),
(N'Motor điện cũ', 'motor-dien-cu', N'Thu mua motor điện cũ, mô tơ quạt, mô tơ công nghiệp, đầu máy.', N'Motor điện cũ được định giá theo trọng lượng, lõi đồng, công suất và khả năng tái sử dụng. Phù hợp cho doanh nghiệp cần thanh lý nhanh.', 5, 0, N'cái', N'Liên hệ', NULL, 'active', 0, @Now, @Now),
(N'Bo mạch điện tử', 'bo-mach-dien-tu', N'Thu mua bo mạch máy tính, bo mạch công nghiệp, linh kiện điện tử cũ.', N'Bo mạch điện tử phế liệu có giá trị tái chế từ các kim loại quý và linh kiện còn dùng được. Chúng tôi phân loại theo nhóm bo dân dụng, bo công nghiệp và bo server.', 6, 0, N'kg', N'Giá thương lượng', NULL, 'active', 1, @Now, @Now),
(N'Nhựa ABS phế liệu', 'nhua-abs-phe-lieu', N'Thu mua nhựa ABS từ vỏ thiết bị điện tử, đồ gia dụng, linh kiện xe.', N'Nhựa ABS có độ cứng tốt, được tái chế mạnh trong ngành nhựa kỹ thuật. Chúng tôi nhận hàng sạch hoặc hàng lẫn cần phân loại.', 3, 18000, N'kg', N'18.000 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Nhựa PP phế liệu', 'nhua-pp-phe-lieu', N'Thu mua nhựa PP bao bì, sọt nhựa, két nhựa, pallet nhựa cũ.', N'Nhựa PP phế liệu phổ biến trong kho vận, sản xuất và nông nghiệp. Hàng càng sạch, khô và đồng màu thì giá thu mua càng cao.', 3, 12500, N'kg', N'12.500 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Nhựa PE phế liệu', 'nhua-pe-phe-lieu', N'Thu mua nhựa PE màng, bao nylon, bao bì công nghiệp số lượng lớn.', N'Nhựa PE màng cần được phân loại sạch, ít lẫn nước và ít đất cát để đạt giá tốt. Chúng tôi thu mua tận nơi cho kho, xưởng và đại lý thu gom.', 3, 9800, N'kg', N'9.800 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Giấy carton phế liệu', 'giay-carton-phe-lieu', N'Thu mua thùng carton cũ, giấy carton công nghiệp, giấy sóng số lượng lớn.', N'Giấy carton phế liệu phát sinh nhiều tại kho hàng, siêu thị, xưởng sản xuất và đơn vị logistics. Chúng tôi hỗ trợ cân tại kho và thu mua định kỳ.', 4, 4200, N'kg', N'4.200 đ/kg', NULL, 'active', 1, @Now, @Now),
(N'Giấy văn phòng phế liệu', 'giay-van-phong-phe-lieu', N'Thu mua giấy A4, hồ sơ lưu trữ, giấy in lỗi, giấy trắng loại 1.', N'Giấy văn phòng phế liệu phù hợp cho công ty, trường học, ngân hàng và trung tâm dữ liệu muốn thanh lý tài liệu số lượng lớn.', 4, 3600, N'kg', N'3.600 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Vải vụn công nghiệp', 'vai-vun-cong-nghiep', N'Thu mua vải vụn may mặc, vải cây lỗi, đầu tấm, vải lau máy.', N'Vải vụn phát sinh từ xưởng may, xưởng sofa, sản xuất giày dép và nội thất. Chúng tôi nhận cả vải cotton, poly, kaki và vải pha.', 6, 4500, N'kg', N'4.500 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Bình inox cũ', 'binh-inox-cu', N'Thu mua bồn inox, thùng inox, thiết bị bếp inox thanh lý.', N'Bồn inox, chậu rửa, bàn inox, thiết bị bếp công nghiệp cũ được định giá theo chủng loại inox, độ dày và khả năng tháo dỡ vận chuyển.', 5, 0, N'cái', N'Liên hệ', NULL, 'active', 0, @Now, @Now),
(N'Pin UPS và ắc quy cũ', 'pin-ups-ac-quy-cu', N'Thu mua pin UPS, ắc quy viễn thông, ắc quy xe nâng và bình điện cũ.', N'Chúng tôi thu mua bình điện, ắc quy và pin lưu điện từ doanh nghiệp, trung tâm dữ liệu, xưởng sản xuất. Có xe chuyên chở và quy trình thu gom an toàn.', 6, 26000, N'kg', N'26.000 đ/kg', NULL, 'active', 1, @Now, @Now),
(N'Kẽm phế liệu', 'kem-phe-lieu', N'Thu mua kẽm tấm, kẽm mạ, kẽm công nghiệp và phế liệu hợp kim kẽm.', N'Kẽm phế liệu thường phát sinh trong ngành cơ khí, khuôn mẫu và mạ điện. Đơn giá phụ thuộc độ sạch và tỷ lệ lẫn tạp chất.', 1, 31000, N'kg', N'31.000 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Thiếc phế liệu', 'thiec-phe-lieu', N'Thu mua thiếc hàn, thiếc công nghiệp, dross thiếc từ nhà máy điện tử.', N'Thiếc phế liệu có giá tốt nhờ nhu cầu tái chế cao trong ngành điện tử. Chúng tôi nhận thu mua từ nhà máy hàn, sản xuất mạch điện và cơ khí chính xác.', 1, 145000, N'kg', N'145.000 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Máy lạnh cũ thanh lý', 'may-lanh-cu-thanh-ly', N'Thu mua máy lạnh cũ, cục nóng, cục lạnh, hệ thống điều hòa tháo dỡ.', N'Chúng tôi nhận thu mua máy lạnh dân dụng và công nghiệp đã qua sử dụng, hàng thanh lý từ văn phòng, khách sạn và nhà xưởng.', 5, 0, N'bộ', N'Liên hệ', NULL, 'active', 0, @Now, @Now),
(N'Tủ điện công nghiệp cũ', 'tu-dien-cong-nghiep-cu', N'Thu mua tủ điện, máng cáp, thiết bị điện công nghiệp đã tháo dỡ.', N'Tủ điện công nghiệp cũ thường có giá trị từ vỏ tủ, thanh đồng, aptomat và linh kiện điện. Chúng tôi nhận khảo sát và báo giá tận nơi.', 6, 0, N'tủ', N'Giá thương lượng', NULL, 'active', 0, @Now, @Now),
(N'Pallet nhựa cũ', 'pallet-nhua-cu', N'Thu mua pallet nhựa cũ, pallet gãy, pallet kho vận số lượng lớn.', N'Pallet nhựa cũ phát sinh nhiều tại kho hàng và nhà máy. Chúng tôi nhận thu mua pallet nguyên chiếc hoặc pallet hư hỏng theo lô.', 3, 9000, N'kg', N'9.000 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Lon nhôm phế liệu', 'lon-nhom-phe-lieu', N'Thu mua lon nhôm, vỏ lon nước giải khát, nhôm mỏng tái chế.', N'Lon nhôm phế liệu cần được phân loại sạch, ép kiện hoặc đóng bao gọn để tối ưu khối lượng và đơn giá thu mua.', 1, 38000, N'kg', N'38.000 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Bồn nước inox cũ', 'bon-nuoc-inox-cu', N'Thu mua bồn nước inox gia đình, công nghiệp, bồn đứng bồn ngang.', N'Chúng tôi thu mua bồn nước inox cũ theo trọng lượng, chủng loại inox và hiện trạng tháo dỡ thực tế.', 5, 0, N'cái', N'Liên hệ', NULL, 'active', 0, @Now, @Now),
(N'Dây điện cháy', 'day-dien-chay', N'Thu mua dây điện cháy, dây điện pha tạp, dây đồng đã qua đốt.', N'Dây điện cháy vẫn có thể thu mua nhưng giá phụ thuộc nhiều vào tỷ lệ đồng thực tế và mức độ lẫn tạp chất.', 6, 65000, N'kg', N'65.000 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Xác nhà xưởng thanh lý', 'xac-nha-xuong-thanh-ly', N'Thu mua xác nhà xưởng, khung kèo, mái tôn, kết cấu thép tháo dỡ.', N'Nhận thu mua trọn gói xác nhà xưởng cũ, tháo dỡ nhanh, hỗ trợ xe cẩu xe tải và xử lý mặt bằng sau khi thu gom.', 5, 0, N'lô', N'Giá thương lượng', NULL, 'active', 1, @Now, @Now),
(N'Bình gas công nghiệp cũ', 'binh-gas-cong-nghiep-cu', N'Thu mua bình gas công nghiệp, vỏ bình sắt thép thanh lý.', N'Bình gas công nghiệp cũ được thu mua theo trọng lượng sắt thép và tình trạng an toàn của vỏ bình.', 2, 9800, N'kg', N'9.800 đ/kg', NULL, 'active', 0, @Now, @Now),
(N'Laptop hỏng phế liệu', 'laptop-hong-phe-lieu', N'Thu mua laptop hỏng, main laptop, xác máy tính điện tử.', N'Laptop hỏng và linh kiện điện tử cũ có thể được định giá theo bo mạch, màn hình, pin và khả năng tận dụng linh kiện.', 6, 0, N'cái', N'Giá thương lượng', NULL, 'active', 0, @Now, @Now),
(N'Máy phát điện cũ', 'may-phat-dien-cu', N'Thu mua máy phát điện cũ, đầu nổ, tổ máy công nghiệp thanh lý.', N'Chúng tôi nhận thu mua máy phát điện cũ từ công trình, xưởng sản xuất, bệnh viện, khách sạn và kho bãi.', 5, 0, N'bộ', N'Giá thương lượng', NULL, 'active', 0, @Now, @Now);

INSERT INTO price_history (product_id, price_value, price_unit, price_type, note, effective_date, recorded_at)
VALUES
(1, 182000, N'kg', 'fixed', N'Giá đầu tháng', '2026-03-01', @Now),
(1, 185000, N'kg', 'fixed', N'Giá hiện tại', '2026-03-20', @Now),
(2, 125000, N'kg', 'fixed', N'Giá cập nhật mới', '2026-03-20', @Now),
(3, 50000, N'kg', 'fixed', N'Nhôm đặc loại 1', '2026-03-15', @Now),
(4, 58000, N'kg', 'fixed', N'Nhôm xingfa sạch', '2026-03-20', @Now),
(5, 36000, N'kg', 'fixed', N'Inox 304 loại 1', '2026-03-20', @Now),
(8, 97000, N'kg', 'fixed', N'Cáp điện lõi đồng', '2026-03-20', @Now),
(9, 10500, N'kg', 'fixed', N'Sắt đặc nặng', '2026-03-20', @Now),
(10, 9200, N'kg', 'fixed', N'Sắt vụn công trình', '2026-03-20', @Now),
(12, 0, N'lô', 'negotiable', N'Khảo sát trước khi báo giá', '2026-03-20', @Now),
(14, 0, N'kg', 'negotiable', N'Giá theo chất lượng bo mạch', '2026-03-20', @Now),
(17, 4200, N'kg', 'fixed', N'Giấy carton loại khô', '2026-03-20', @Now),
(20, 26000, N'kg', 'fixed', N'Ắc quy viễn thông', '2026-03-20', @Now),
(22, 145000, N'kg', 'fixed', N'Thiếc hàn công nghiệp', '2026-03-20', @Now),
(26, 38000, N'kg', 'fixed', N'Lon nhôm sạch ép kiện', '2026-03-20', @Now),
(28, 65000, N'kg', 'fixed', N'Dây điện cháy phân loại', '2026-03-20', @Now),
(30, 9800, N'kg', 'fixed', N'Vỏ bình sắt thép', '2026-03-20', @Now);

INSERT INTO blog_posts (title, slug, excerpt, content, cover_image, author_id, published_at, status, created_at, updated_at)
VALUES
(N'Bảng giá thu mua đồng đỏ hôm nay tại TP.HCM', 'bang-gia-thu-mua-dong-do-hom-nay-tai-tp-hcm', N'Cập nhật giá đồng đỏ mới nhất, yếu tố ảnh hưởng đến giá và mẹo bán đồng đỏ được giá cao.', N'Đồng đỏ luôn là nhóm phế liệu có giá trị cao nhờ nhu cầu tái chế lớn từ ngành điện và cơ khí. Giá thu mua chịu ảnh hưởng bởi độ sạch, tỷ lệ tạp chất, hình thức hàng hóa và khối lượng giao dịch. Khi bán đồng đỏ, khách nên bóc tách riêng đồng sạch, đồng cháy và đồng lẫn nhựa để nhận báo giá tốt hơn. Đối với lô hàng lớn từ công trình hoặc nhà xưởng, việc cân tại chỗ và bốc xếp nhanh cũng giúp tối ưu chi phí. Chúng tôi nhận thu mua tận nơi, báo giá nhanh, thanh toán trong ngày.', NULL, 1, DATEADD(DAY, -30, @Now), 'published', @Now, @Now),
(N'Nhôm xingfa cũ có giá bao nhiêu một ký?', 'nhom-xingfa-cu-co-gia-bao-nhieu-mot-ky', N'Tìm hiểu cách định giá nhôm xingfa cũ từ cửa nhôm, vách kính và công trình tháo dỡ.', N'Nhôm xingfa cũ là mặt hàng được thu mua nhiều nhờ chất lượng nhôm đồng đều và khả năng tái chế cao. Giá nhôm xingfa phụ thuộc vào độ sạch của thanh nhôm, tỷ lệ phụ kiện đi kèm như kính, ron cao su, vít và các tạp chất khác. Nếu khách hàng chủ động tháo kính, loại bỏ sắt thép lẫn vào thì đơn giá sẽ cao hơn rõ rệt. Với công trình lớn, nên chia lô theo tầng hoặc khu vực để quá trình bốc xếp diễn ra thuận lợi và an toàn.', NULL, 1, DATEADD(DAY, -29, @Now), 'published', @Now, @Now),
(N'Kinh nghiệm phân loại inox 304 và inox 201 trước khi bán', 'kinh-nghiem-phan-loai-inox-304-va-inox-201-truoc-khi-ban', N'Phân biệt inox 304 và 201 giúp bạn bán đúng giá, tránh bị ép giá khi thanh lý.', N'Inox 304 có hàm lượng niken cao hơn inox 201 nên giá trị thu mua cũng cao hơn. Trước khi bán, khách hàng có thể phân loại sơ bộ theo nguồn gốc hàng hóa như bồn nước, bàn bếp, lan can, ống công nghiệp hoặc hàng gia dụng. Tránh để inox lẫn sắt hoặc nhôm vì sẽ làm giảm giá cả lô hàng. Với khối lượng lớn, đơn vị thu mua có thể hỗ trợ kiểm tra bằng nam châm hoặc test nhanh tại chỗ để xác định chủng loại.', NULL, 1, DATEADD(DAY, -28, @Now), 'published', @Now, @Now),
(N'Thu mua sắt công trình số lượng lớn cần lưu ý gì?', 'thu-mua-sat-cong-trinh-so-luong-lon-can-luu-y-gi', N'Các lưu ý quan trọng khi thanh lý sắt công trình, nhà thép tiền chế và kết cấu cũ.', N'Sắt công trình thường phát sinh sau tháo dỡ nhà xưởng, cải tạo kho bãi hoặc thay mới hệ kết cấu. Khi thanh lý số lượng lớn, khách hàng nên chụp ảnh hiện trạng, ước lượng khối lượng và xác định đường vào cho xe cẩu xe tải. Việc phân tách sắt nặng, sắt mỏng và sắt lẫn tạp sẽ giúp tối ưu giá bán. Ngoài ra, cần ưu tiên các đơn vị thu mua có đội bốc xếp chuyên nghiệp, cân điện tử minh bạch và thanh toán rõ ràng.', NULL, 1, DATEADD(DAY, -27, @Now), 'published', @Now, @Now),
(N'Giá cáp điện phế liệu tăng hay giảm theo thị trường đồng?', 'gia-cap-dien-phe-lieu-tang-hay-giam-theo-thi-truong-dong', N'Tìm hiểu mối liên hệ giữa giá cáp điện phế liệu và biến động giá đồng thế giới.', N'Giá cáp điện phế liệu thường biến động theo giá đồng nguyên liệu trên thị trường quốc tế, nhưng còn phụ thuộc vào tỷ lệ lõi đồng, độ dày lớp vỏ và công tác phân loại. Dây cáp càng ít tạp, ít ẩm nước và lõi đồng càng tốt thì giá thu mua càng cao. Với cáp điện công trình, khách hàng nên bóc tách theo nhóm lõi đơn, lõi nhiều sợi và cáp điện lực để dễ báo giá hơn. Đối với lô hàng lớn, mức giá có thể được điều chỉnh tốt hơn theo số lượng.', NULL, 1, DATEADD(DAY, -26, @Now), 'published', @Now, @Now),
(N'Bo mạch điện tử cũ có thể bán được không?', 'bo-mach-dien-tu-cu-co-the-ban-duoc-khong', N'Bo mạch máy tính, bo công nghiệp, linh kiện điện tử cũ vẫn có giá trị tái chế cao.', N'Bo mạch điện tử cũ vẫn có giá trị thu mua nhờ chứa linh kiện điện tử, kim loại màu và kim loại quý ở mức độ nhất định. Giá thu mua phụ thuộc vào loại bo mạch, tình trạng linh kiện và nguồn gốc hàng hóa. Bo máy tính, bo server, bo điều khiển công nghiệp thường có mức định giá khác nhau. Để được giá tốt, khách hàng nên bảo quản bo khô ráo, tránh lẫn nhựa, sắt và tạp chất trong quá trình lưu kho.', NULL, 1, DATEADD(DAY, -25, @Now), 'published', @Now, @Now),
(N'Giấy carton phế liệu bán sao cho được giá cao?', 'giay-carton-phe-lieu-ban-sao-cho-duoc-gia-cao', N'Mẹo gom giấy carton sạch, khô và đồng loại để nâng giá thu mua.', N'Giấy carton phế liệu có giá tốt khi đảm bảo khô ráo, ít lẫn nilon, băng keo và rác sinh hoạt. Đối với kho hàng và xưởng sản xuất, nên bó gọn thành kiện để tiết kiệm diện tích và dễ bốc xếp. Nếu có lượng giấy phát sinh thường xuyên, doanh nghiệp nên ký lịch thu mua định kỳ để ổn định giá và giảm tồn kho. Hàng đồng đều, sạch và số lượng lớn luôn được ưu tiên báo giá cao hơn.', NULL, 1, DATEADD(DAY, -24, @Now), 'published', @Now, @Now),
(N'Nhựa ABS là gì và vì sao được thu mua nhiều?', 'nhua-abs-la-gi-va-vi-sao-duoc-thu-mua-nhieu', N'Nhựa ABS từ vỏ thiết bị điện tử, xe máy và đồ gia dụng có khả năng tái chế tốt.', N'Nhựa ABS là dòng nhựa kỹ thuật có độ cứng, độ bền và khả năng gia công tốt nên được nhiều đơn vị tái chế quan tâm. Phế liệu ABS thường đến từ vỏ máy in, TV, máy lạnh, linh kiện xe máy và đồ gia dụng. Giá thu mua nhựa ABS phụ thuộc vào màu sắc, độ sạch, mức độ giòn gãy và tỷ lệ tạp chất. Phân loại đúng ngay từ đầu sẽ giúp nâng cao giá trị lô hàng.', NULL, 1, DATEADD(DAY, -23, @Now), 'published', @Now, @Now),
(N'Có nên bán máy móc cũ theo ký hay theo lô?', 'co-nen-ban-may-moc-cu-theo-ky-hay-theo-lo', N'Bán máy móc cũ theo lô hay theo ký sẽ phụ thuộc vào chủng loại, khả năng tái sử dụng và chi phí tháo dỡ.', N'Máy móc cũ thanh lý có thể được định giá theo hai cách: theo trọng lượng phế liệu hoặc theo giá trị sử dụng của thiết bị. Với máy móc còn khả năng vận hành, bán theo lô thường mang lại hiệu quả cao hơn. Ngược lại, máy hư hỏng nặng, thiếu linh kiện hoặc rỉ sét nhiều sẽ phù hợp báo giá theo ký hoặc theo tổng xác máy. Để tối ưu giá bán, khách hàng nên cung cấp hình ảnh, model, năm sản xuất và hiện trạng vận hành trước khi khảo sát.', NULL, 1, DATEADD(DAY, -22, @Now), 'published', @Now, @Now),
(N'Ắc quy cũ và pin UPS có thu mua tận nơi không?', 'ac-quy-cu-va-pin-ups-co-thu-mua-tan-noi-khong', N'Giải đáp dịch vụ thu mua tận nơi đối với ắc quy cũ, pin UPS, bình điện công nghiệp.', N'Ắc quy cũ và pin UPS là nhóm hàng cần thu gom đúng cách để đảm bảo an toàn trong quá trình vận chuyển. Chúng tôi nhận thu mua tận nơi đối với doanh nghiệp, tòa nhà, nhà xưởng và trung tâm dữ liệu có nhu cầu thanh lý pin lưu điện. Giá thu mua phụ thuộc vào khối lượng, chủng loại bình, mức độ nguyên vẹn và hàm lượng chì. Quy trình thu gom chuyên nghiệp giúp khách hàng tiết kiệm thời gian và bảo đảm vệ sinh môi trường.', NULL, 1, DATEADD(DAY, -21, @Now), 'published', @Now, @Now),
(N'Bảng giá sắt vụn công trình mới nhất tuần này', 'bang-gia-sat-vun-cong-trinh-moi-nhat-tuan-nay', N'Cập nhật giá sắt vụn công trình mới nhất và các yếu tố làm thay đổi đơn giá thực tế.', N'Giá sắt vụn công trình phụ thuộc vào tình trạng sắt, độ dày, khối lượng, vị trí thu mua và chi phí bốc xếp. Những lô hàng có tỷ lệ sắt nặng cao, dễ cẩu kéo và ít lẫn bê tông thường có đơn giá tốt hơn. Khi thanh lý tại công trình, khách hàng nên ưu tiên dọn lối đi, xác định vị trí tập kết và chụp hình thực tế để nhận báo giá nhanh. Đơn vị thu mua uy tín sẽ giúp quá trình tháo dỡ diễn ra gọn gàng và an toàn.', NULL, 1, DATEADD(DAY, -20, @Now), 'published', @Now, @Now),
(N'Đồng vàng khác đồng đỏ như thế nào khi bán phế liệu?', 'dong-vang-khac-dong-do-nhu-the-nao-khi-ban-phe-lieu', N'Hiểu rõ sự khác nhau giữa đồng vàng và đồng đỏ để bán đúng nhóm hàng, đúng đơn giá.', N'Đồng đỏ có hàm lượng đồng cao hơn nên giá trị thu mua thường cao hơn đồng vàng. Đồng vàng là hợp kim đồng với kẽm, thường xuất hiện ở van nước, phụ kiện ren, tay nắm và chi tiết cơ khí. Khi bán phế liệu, khách hàng nên tách riêng hai nhóm này để tránh bị tính giá trung bình thấp. Việc phân loại đúng sẽ giúp đơn vị thu mua đánh giá chính xác và báo giá minh bạch.', NULL, 1, DATEADD(DAY, -19, @Now), 'published', @Now, @Now),
(N'Những loại giấy phế liệu doanh nghiệp thường thanh lý', 'nhung-loai-giay-phe-lieu-doanh-nghiep-thuong-thanh-ly', N'Tổng hợp các loại giấy phế liệu phổ biến tại văn phòng, nhà máy, kho vận và cửa hàng.', N'Các doanh nghiệp thường phát sinh giấy văn phòng, hồ sơ lưu trữ, carton đóng gói, giấy in lỗi và giấy hỗn hợp sau quá trình hoạt động. Mỗi nhóm giấy có mức giá khác nhau tùy độ sạch và chất lượng sợi giấy tái chế. Khi thanh lý số lượng lớn, nên phân tách giấy trắng, carton và giấy màu riêng để tối ưu giá. Đồng thời, lưu trữ ở nơi khô ráo sẽ giúp hạn chế hao hụt trọng lượng do ẩm mốc.', NULL, 1, DATEADD(DAY, -18, @Now), 'published', @Now, @Now),
(N'Nhôm đặc và nhôm mỏng khác nhau ra sao khi báo giá?', 'nhom-dac-va-nhom-mong-khac-nhau-ra-sao-khi-bao-gia', N'Khác biệt về trọng lượng riêng, độ sạch và tỷ lệ hao hụt khiến nhôm đặc và nhôm mỏng có giá khác nhau.', N'Nhôm đặc thường có độ dày và tỷ trọng tốt hơn nhôm mỏng, do đó quá trình tái chế cho hiệu suất kim loại cao hơn. Những lô nhôm đặc như nhôm thanh, nhôm tấm, nhôm cơ khí thường được báo giá tốt hơn. Trong khi đó, nhôm mỏng hoặc nhôm lẫn sơn, lẫn tạp chất sẽ cần công đoạn xử lý nhiều hơn. Phân loại ngay từ đầu là cách đơn giản nhất để tối ưu giá bán.', NULL, 1, DATEADD(DAY, -17, @Now), 'published', @Now, @Now),
(N'Khi nào nên thanh lý toàn bộ kho phế liệu trong nhà xưởng?', 'khi-nao-nen-thanh-ly-toan-bo-kho-phe-lieu-trong-nha-xuong', N'Thời điểm thanh lý hợp lý giúp giải phóng mặt bằng, tăng dòng tiền và giảm chi phí lưu kho.', N'Kho phế liệu tồn đọng lâu ngày không chỉ chiếm diện tích mà còn làm tăng rủi ro cháy nổ, ẩm mốc và thất thoát vật tư. Doanh nghiệp nên thanh lý định kỳ khi khối lượng hàng đủ lớn hoặc khi chuẩn bị cải tạo mặt bằng. Việc gom hàng theo nhóm như sắt thép, nhôm, đồng, inox, giấy và nhựa sẽ giúp quá trình cân đo minh bạch và báo giá chính xác hơn. Một đơn vị thu mua chuyên nghiệp có thể hỗ trợ khảo sát, phân loại và bốc xếp trọn gói.', NULL, 1, DATEADD(DAY, -16, @Now), 'published', @Now, @Now),
(N'Vải vụn công nghiệp có thể bán cho ai?', 'vai-vun-cong-nghiep-co-the-ban-cho-ai', N'Vải vụn may mặc, vải sofa, đầu tấm lỗi vẫn có thể được thu mua nếu phân loại tốt.', N'Vải vụn công nghiệp thường phát sinh tại xưởng may mặc, sản xuất nội thất, giày dép và túi xách. Các loại vải cotton, poly, kaki, nỉ hoặc vải pha đều có nhóm khách tái sử dụng hoặc tái chế riêng. Muốn bán được giá tốt, khách hàng nên phân loại theo chất liệu, màu sắc và độ sạch của vải. Với số lượng lớn, đơn vị thu mua có thể đến tận nơi để khảo sát và chốt giá nhanh.', NULL, 1, DATEADD(DAY, -15, @Now), 'published', @Now, @Now),
(N'Thị trường thép phế liệu quý này có gì đáng chú ý?', 'thi-truong-thep-phe-lieu-quy-nay-co-gi-dang-chu-y', N'Nhận định xu hướng thép phế liệu theo nhu cầu xây dựng, sản xuất và xuất khẩu.', N'Thị trường thép phế liệu biến động theo nhu cầu xây dựng, giá quặng, chi phí vận tải và sản lượng từ các nhà máy luyện thép. Trong giai đoạn nhu cầu phục hồi, các lô thép sạch, đồng đều và khối lượng lớn thường có tính thanh khoản tốt hơn. Doanh nghiệp có hàng thanh lý nên theo dõi chu kỳ giá và chủ động gom hàng đúng thời điểm. Việc hợp tác với đơn vị thu mua lâu dài cũng giúp ổn định đầu ra và chi phí logistics.', NULL, 1, DATEADD(DAY, -14, @Now), 'published', @Now, @Now),
(N'Hướng dẫn chụp ảnh lô phế liệu để báo giá nhanh', 'huong-dan-chup-anh-lo-phe-lieu-de-bao-gia-nhanh', N'Chụp ảnh đúng góc, đúng ánh sáng giúp đơn vị thu mua đánh giá nhanh và chính xác hơn.', N'Khi cần báo giá nhanh, ảnh chụp lô phế liệu nên thể hiện rõ toàn cảnh, cận cảnh vật liệu, khối lượng ước tính và lối vào bốc xếp. Với hàng đồng, nhôm, inox hoặc cáp điện, nên chụp riêng từng nhóm để tránh nhầm lẫn. Ngoài hình ảnh, khách hàng có thể cung cấp thêm địa chỉ, số lượng tương đối và thời gian dự kiến thanh lý. Thông tin càng rõ ràng thì tốc độ chốt giá càng nhanh.', NULL, 1, DATEADD(DAY, -13, @Now), 'published', @Now, @Now),
(N'Tại sao nên chọn đơn vị thu mua có cân điện tử?', 'tai-sao-nen-chon-don-vi-thu-mua-co-can-dien-tu', N'Cân điện tử giúp minh bạch trọng lượng và hạn chế tranh chấp trong quá trình mua bán phế liệu.', N'Trọng lượng là yếu tố trực tiếp quyết định giá trị lô phế liệu, vì vậy hệ thống cân điện tử minh bạch là tiêu chí rất quan trọng. Khi giao dịch với lượng lớn sắt thép, đồng, nhôm hoặc giấy carton, sai số nhỏ cũng có thể tạo ra chênh lệch đáng kể. Cân điện tử kết hợp biên nhận rõ ràng giúp khách hàng yên tâm hơn trong toàn bộ quá trình giao dịch. Đây cũng là dấu hiệu cho thấy đơn vị thu mua làm việc chuyên nghiệp và nghiêm túc.', NULL, 1, DATEADD(DAY, -12, @Now), 'published', @Now, @Now),
(N'Bồn inox cũ từ nhà hàng có bán được giá tốt không?', 'bon-inox-cu-tu-nha-hang-co-ban-duoc-gia-tot-khong', N'Bồn inox, bàn inox, thiết bị bếp nhà hàng thanh lý vẫn có giá khá tốt nếu còn dày và sạch.', N'Thiết bị inox từ nhà hàng, bếp ăn công nghiệp và quán ăn thường có giá trị tốt nếu còn nguyên khối, ít han gỉ và ít lẫn phụ kiện khác. Bồn inox, bàn sơ chế, kệ bếp hoặc chậu rửa có thể được thu mua theo trọng lượng hoặc theo món nếu còn khả năng tái sử dụng. Việc làm sạch sơ bộ dầu mỡ và tách phụ kiện sắt thép sẽ giúp tối ưu giá. Với số lượng lớn, đơn vị thu mua có thể hỗ trợ tháo dỡ trọn gói.', NULL, 1, DATEADD(DAY, -11, @Now), 'published', @Now, @Now),
(N'Pin UPS cũ có cần lưu kho riêng trước khi bán không?', 'pin-ups-cu-co-can-luu-kho-rieng-truoc-khi-ban-khong', N'Lưu kho pin UPS đúng cách giúp bảo đảm an toàn và giữ nguyên tình trạng hàng hóa trước khi thanh lý.', N'Pin UPS cũ nên được lưu kho ở nơi khô ráo, thoáng mát, tránh nắng trực tiếp và tránh va đập mạnh. Nếu số lượng lớn, khách hàng nên xếp theo lô, theo chủng loại và hạn chế chồng quá cao để đảm bảo an toàn. Việc lưu kho đúng cách không chỉ giúp đơn vị thu mua kiểm đếm nhanh hơn mà còn giảm rủi ro rò rỉ hoặc hư hỏng thêm. Đây là yếu tố quan trọng trong nhóm hàng điện - điện tử đặc thù.', NULL, 1, DATEADD(DAY, -10, @Now), 'published', @Now, @Now),
(N'Giá chì phế liệu phụ thuộc vào yếu tố nào?', 'gia-chi-phe-lieu-phu-thuoc-vao-yeu-to-nao', N'Giá chì từ bản cực, chì tấm, chì công nghiệp thay đổi theo độ sạch và nhu cầu tái chế.', N'Chì phế liệu thường phát sinh từ ắc quy, bản cực, chì cán tấm và thiết bị công nghiệp. Đơn giá thu mua phụ thuộc vào mức độ tinh khiết, độ sạch bề mặt, tỷ lệ lẫn nhựa hoặc axit và chi phí vận chuyển an toàn. Với lô hàng số lượng lớn, nên phân loại riêng chì đặc, chì lẫn nhựa và chì từ bình điện để báo giá chính xác. Quá trình thu gom cần thực hiện cẩn thận để đảm bảo an toàn môi trường.', NULL, 1, DATEADD(DAY, -9, @Now), 'published', @Now, @Now),
(N'Nhựa PP và nhựa PE loại nào dễ bán hơn?', 'nhua-pp-va-nhua-pe-loai-nao-de-ban-hon', N'So sánh khả năng thu mua, mức giá và cách phân loại nhựa PP, PE trong thực tế.', N'Nhựa PP và nhựa PE đều là hai dòng nhựa phổ biến trong ngành tái chế, nhưng giá trị thu mua khác nhau tùy nguồn phát sinh và chất lượng hàng. Nhựa PP thường có ở két nhựa, sọt nhựa, bao bì cứng, trong khi nhựa PE phổ biến ở màng nilon và bao bì mềm. Hàng càng sạch, ít lẫn nước và cùng màu thì càng dễ bán được giá tốt. Việc ép kiện hoặc đóng bao gọn gàng cũng giúp tiết kiệm chi phí thu gom.', NULL, 1, DATEADD(DAY, -8, @Now), 'published', @Now, @Now),
(N'Cách xử lý hồ sơ giấy cũ khi thanh lý cho doanh nghiệp', 'cach-xu-ly-ho-so-giay-cu-khi-thanh-ly-cho-doanh-nghiep', N'Thanh lý hồ sơ giấy cũ cần chú ý đến bảo mật thông tin và quy trình đóng gói phù hợp.', N'Khi thanh lý hồ sơ giấy cũ, doanh nghiệp nên phân loại tài liệu còn giá trị lưu trữ và tài liệu có thể hủy hoặc bán phế liệu. Các hồ sơ nhạy cảm cần được niêm phong hoặc tiêu hủy bảo mật theo quy trình nội bộ trước khi giao cho đơn vị thu mua. Sau đó, giấy được bó gọn theo lô để thuận tiện cho khâu cân và vận chuyển. Quy trình rõ ràng sẽ giúp doanh nghiệp vừa giải phóng kho vừa đảm bảo an toàn thông tin.', NULL, 1, DATEADD(DAY, -7, @Now), 'published', @Now, @Now),
(N'Thu mua phế liệu nhà xưởng trọn gói gồm những gì?', 'thu-mua-phe-lieu-nha-xuong-tron-goi-gom-nhung-gi', N'Dịch vụ trọn gói bao gồm khảo sát, báo giá, tháo dỡ, bốc xếp, vận chuyển và vệ sinh mặt bằng.', N'Dịch vụ thu mua phế liệu nhà xưởng trọn gói thường bao gồm khảo sát hiện trạng, phân loại hàng hóa, chốt giá theo từng nhóm vật tư, tổ chức tháo dỡ và vận chuyển ra khỏi mặt bằng. Những hạng mục phổ biến gồm sắt thép, máy móc, cáp điện, inox, nhôm, bo mạch và các vật tư dư thừa khác. Với đội ngũ có kinh nghiệm, quá trình thu gom sẽ diễn ra nhanh, an toàn và hạn chế ảnh hưởng đến hoạt động sản xuất. Đây là lựa chọn phù hợp cho doanh nghiệp cần thanh lý khối lượng lớn trong thời gian ngắn.', NULL, 1, DATEADD(DAY, -6, @Now), 'published', @Now, @Now),
(N'Kẽm phế liệu có ứng dụng tái chế như thế nào?', 'kem-phe-lieu-co-ung-dung-tai-che-nhu-the-nao', N'Kẽm phế liệu được tái chế trong nhiều ngành công nghiệp như mạ, đúc và hợp kim.', N'Kẽm phế liệu được thu hồi để phục vụ nhiều ngành công nghiệp như sản xuất hợp kim, mạ kim loại và đúc chi tiết kỹ thuật. Nguồn hàng thường đến từ xưởng cơ khí, khuôn mẫu, mạ điện hoặc công trình tháo dỡ. Giá thu mua phụ thuộc vào độ sạch, tình trạng oxy hóa và mức độ lẫn tạp chất. Việc tập kết riêng kẽm với nhôm hoặc sắt sẽ giúp tránh thất thoát giá trị khi bán.', NULL, 1, DATEADD(DAY, -5, @Now), 'published', @Now, @Now),
(N'Thiếc hàn công nghiệp cũ có giá trị không?', 'thiec-han-cong-nghiep-cu-co-gia-tri-khong', N'Thiếc hàn, thiếc lỗi, bã thiếc từ nhà máy điện tử là nhóm hàng có giá trị tái chế khá cao.', N'Thiếc hàn công nghiệp cũ là nhóm hàng có giá trị vì chứa hàm lượng kim loại hữu ích cao, đặc biệt trong sản xuất điện tử và cơ khí chính xác. Giá thu mua phụ thuộc vào dạng thiếc, độ tinh khiết, mức độ oxy hóa và tỷ lệ pha trộn. Doanh nghiệp nên lưu trữ riêng thiếc lỗi, bã thiếc và thiếc hàn dây để việc đánh giá dễ dàng hơn. Khi có số lượng lớn, đơn vị thu mua có thể báo giá tốt hơn theo lô.', NULL, 1, DATEADD(DAY, -4, @Now), 'published', @Now, @Now),
(N'5 mẹo giúp bán phế liệu nhanh, gọn và không bị ép giá', '5-meo-giup-ban-phe-lieu-nhanh-gon-va-khong-bi-ep-gia', N'Những mẹo đơn giản giúp cá nhân và doanh nghiệp thanh lý phế liệu hiệu quả hơn.', N'Muốn bán phế liệu nhanh và được giá tốt, trước tiên cần phân loại hàng hóa theo từng nhóm cụ thể như đồng, nhôm, inox, sắt thép, giấy và nhựa. Tiếp theo, hãy chụp ảnh rõ ràng và chuẩn bị thông tin về địa điểm, khối lượng và tình trạng bốc xếp để nhận báo giá chính xác. Nên ưu tiên đơn vị có cân điện tử, thanh toán minh bạch và hỗ trợ thu mua tận nơi. Cuối cùng, đừng để hàng tồn quá lâu gây hao hụt, ẩm mốc hoặc lẫn tạp chất.', NULL, 1, DATEADD(DAY, -3, @Now), 'published', @Now, @Now),
(N'Checklist chuẩn bị trước khi gọi thu mua phế liệu tận nơi', 'checklist-chuan-bi-truoc-khi-goi-thu-mua-phe-lieu-tan-noi', N'Danh sách các đầu việc nên chuẩn bị để việc thu mua diễn ra nhanh, an toàn và thuận lợi.', N'Trước khi gọi đơn vị thu mua phế liệu tận nơi, khách hàng nên chuẩn bị danh sách chủng loại hàng, ước lượng số lượng, ảnh thực tế và thông tin vị trí lấy hàng. Nếu là kho hoặc nhà xưởng, cần kiểm tra lối đi cho xe tải, xe cẩu và khu vực tập kết hàng. Đồng thời nên bố trí người phụ trách để xác nhận khối lượng, đơn giá và chứng từ thanh toán khi giao dịch. Chuẩn bị kỹ càng sẽ giúp tiết kiệm rất nhiều thời gian cho cả hai bên.', NULL, 1, DATEADD(DAY, -2, @Now), 'published', @Now, @Now);

INSERT INTO blog_post_categories (post_id, category_id)
VALUES
(1, 1),(2, 1),(3, 5),(4, 4),(5, 3),(6, 2),(7, 2),(8, 5),(9, 4),(10, 4),
(11, 1),(12, 5),(13, 2),(14, 5),(15, 4),(16, 3),(17, 2),(18, 2),(19, 4),(20, 4),
(21, 1),(22, 5),(23, 2),(24, 3),(25, 2);

INSERT INTO blog_post_products (post_id, product_id)
VALUES
(1, 1),
(2, 4),
(3, 5), (3, 6),
(4, 9), (4, 10),
(5, 8),
(6, 14),
(7, 17), (7, 18),
(8, 15),
(9, 12), (9, 13),
(10, 20),
(11, 10),
(12, 2), (12, 1),
(13, 18), (13, 17),
(14, 3),
(15, 12),
(16, 19),
(17, 11),
(18, 8), (18, 1),
(19, 5), (19, 21),
(20, 20),
(21, 7),
(22, 16), (22, 15),
(23, 18),
(24, 21),
(25, 22);
GO
