INSERT INTO product_categories (name, slug, description)
VALUES
(N'Phế liệu sắt', 'phe-lieu-sat',
 N'Danh mục phế liệu sắt bao gồm sắt vụn, sắt công trình, sắt công nghiệp. Chúng tôi thu mua phế liệu sắt với giá cao, cập nhật theo thị trường mỗi ngày.'),

(N'Phế liệu đồng', 'phe-lieu-dong',
 N'Phế liệu đồng gồm đồng đỏ, đồng vàng, dây điện, cáp điện. Giá thu mua đồng luôn ở mức cao do nhu cầu tái chế lớn.'),

(N'Phế liệu nhôm', 'phe-lieu-nhom',
 N'Nhôm phế liệu như nhôm định hình, nhôm vụn, nhôm công nghiệp được thu mua với giá tốt, hỗ trợ tận nơi.'),

(N'Phế liệu inox', 'phe-lieu-inox',
 N'Thu mua inox các loại 304, 201, 430 với giá cao. Inox là loại phế liệu có giá trị tái chế tốt.'),

(N'Phế liệu giấy', 'phe-lieu-giay',
 N'Thu mua giấy carton, giấy vụn, giấy báo số lượng lớn. Giá thu mua ổn định, hỗ trợ vận chuyển.'),

(N'Phế liệu nhựa', 'phe-lieu-nhua',
 N'Nhựa phế liệu như nhựa PET, nhựa HDPE, nhựa công nghiệp được thu mua với giá cạnh tranh.'),

(N'Máy móc cũ', 'may-moc-cu',
 N'Thu mua máy móc cũ, thiết bị công nghiệp, dây chuyền sản xuất đã qua sử dụng với giá cao.'),

(N'Phế liệu công trình', 'phe-lieu-cong-trinh',
 N'Thu mua phế liệu từ công trình xây dựng, tháo dỡ nhà xưởng, hỗ trợ thu gom nhanh chóng.'),

(N'Phế liệu điện tử', 'phe-lieu-dien-tu',
 N'Thu mua linh kiện điện tử, bo mạch, thiết bị điện tử hỏng với giá tốt, xử lý an toàn môi trường.'),

(N'Thu mua tổng hợp', 'thu-mua-tong-hop',
 N'Dịch vụ thu mua tất cả các loại phế liệu với giá cao, hỗ trợ tận nơi, thanh toán nhanh chóng.');

 INSERT INTO products
(name, slug, short_description, description, category_id,
 price_value, unit, price_label, primary_image,
 status, is_featured)
VALUES

(N'Thu mua đồng đỏ','thu-mua-dong-do',
N'Thu mua đồng đỏ phế liệu giá cao, cân chuẩn, thanh toán nhanh.',
N'Dịch vụ thu mua đồng đỏ phế liệu tận nơi dành cho hộ gia đình, xưởng cơ khí và nhà máy. Chúng tôi nhận thu mua đồng đỏ số lượng nhỏ lẻ đến số lượng lớn với giá cạnh tranh theo thị trường. Hỗ trợ khảo sát tận nơi, bốc xếp nhanh, thanh toán tiền mặt hoặc chuyển khoản trong ngày.',
2,185000,N'kg',N'Giá từ 185.000đ/kg',
'~/assets/images/products/dongdo1.jpg',
'active',1),

(N'Thu mua đồng vàng','thu-mua-dong-vang',
N'Nhận thu mua đồng vàng phế liệu tận nơi, giá tốt mỗi ngày.',
N'Chuyên thu mua đồng vàng phế liệu từ thiết bị cũ, phụ kiện cơ khí, vật tư điện nước và hàng thanh lý. Giá thu mua được cập nhật theo chất lượng hàng thực tế, có hỗ trợ vận chuyển tận nơi và báo giá nhanh qua điện thoại hoặc Zalo.',
2,128000,N'kg',N'Giá từ 128.000đ/kg',
'~/assets/images/products/dongvang1.jpg',
'active',1),

(N'Thu mua nhôm định hình','thu-mua-nhom-dinh-hinh',
N'Thu mua nhôm định hình cửa, khung nhôm, nhôm công nghiệp giá cao.',
N'Chúng tôi chuyên thu mua nhôm định hình từ công trình, nhà ở, xưởng sản xuất và cửa hàng nhôm kính. Dịch vụ thu mua nhanh gọn, có xe đến tận nơi, cân đo minh bạch và thanh toán ngay sau khi hoàn tất giao dịch.',
3,52000,N'kg',N'Giá từ 52.000đ/kg',
'~/assets/images/products/nhomdinhhinh1.jpg',
'active',1),

(N'Thu mua nhôm vụn','thu-mua-nhom-vun',
N'Nhận thu mua nhôm vụn số lượng lớn nhỏ với giá cạnh tranh.',
N'Dịch vụ thu mua nhôm vụn phù hợp cho hộ gia đình, cửa hàng sửa chữa, cơ sở sản xuất và công trình tháo dỡ. Chúng tôi hỗ trợ thu gom tận nơi, phân loại nhanh và đưa ra mức giá hợp lý theo từng loại nhôm thực tế.',
3,38000,N'kg',N'Giá từ 38.000đ/kg',
'~/assets/images/products/nhomvun1.jpg',
'active',0),

(N'Thu mua sắt đặc','thu-mua-sat-dac',
N'Thu mua sắt đặc phế liệu giá tốt, nhận tận nơi toàn khu vực.',
N'Chuyên thu mua sắt đặc từ công trình xây dựng, nhà xưởng, cơ sở cơ khí và hàng thanh lý. Giá thu mua sắt đặc được cập nhật mỗi ngày theo thị trường, có hỗ trợ xe tải và nhân công bốc xếp nếu khách hàng cần.',
1,11500,N'kg',N'Giá từ 11.500đ/kg',
'~/assets/images/products/satdac1.jpg',
'active',1),

(N'Thu mua sắt vụn công trình','thu-mua-sat-vun-cong-trinh',
N'Thu mua sắt vụn từ công trình tháo dỡ, nhà xưởng, kho bãi.',
N'Dịch vụ thu mua sắt vụn công trình dành cho các đơn vị xây dựng, tháo dỡ nhà xưởng và cải tạo mặt bằng. Chúng tôi nhận hàng số lượng lớn, báo giá nhanh, thu gom trong ngày và hỗ trợ dọn dẹp sau khi vận chuyển.',
8,9200,N'kg',N'Giá từ 9.200đ/kg',
'~/assets/images/products/satvun1.jpg',
'active',0),

(N'Thu mua inox 304','thu-mua-inox-304',
N'Nhận thu mua inox 304 phế liệu giá cao, thanh toán ngay.',
N'Inox 304 là mặt hàng có giá trị tái chế tốt và được nhiều khách hàng quan tâm. Chúng tôi chuyên thu mua inox 304 từ nhà bếp công nghiệp, xưởng gia công, nhà hàng và cơ sở sản xuất với mức giá ổn định, cân chuẩn, không ép giá.',
4,34000,N'kg',N'Giá từ 34.000đ/kg',
'~/assets/images/products/inox1.jpg',
'active',1),

(N'Thu mua giấy carton','thu-mua-giay-carton',
N'Thu mua giấy carton cũ số lượng lớn cho kho xưởng và cửa hàng.',
N'Chúng tôi nhận thu mua giấy carton cũ, giấy bao bì, giấy đóng gói từ siêu thị, kho hàng, nhà máy và cửa hàng bán lẻ. Giá thu mua ổn định, hỗ trợ xe thu gom tận nơi đối với đơn hàng số lượng lớn, thanh toán nhanh và rõ ràng.',
5,4200,N'kg',N'Giá từ 4.200đ/kg',
'~/assets/images/products/giaycarton1.jpg',
'active',0),

(N'Thu mua nhựa PET','thu-mua-nhua-pet',
N'Thu mua chai nhựa PET, nhựa tái chế với giá tốt theo số lượng.',
N'Dịch vụ thu mua nhựa PET phù hợp cho cơ sở thu gom, nhà hàng, quán nước, kho hàng và đơn vị xử lý rác thải. Chúng tôi nhận thu mua số lượng lớn nhỏ, hỗ trợ phân loại và vận chuyển nhanh, giúp khách hàng tối ưu giá trị từ nguồn nhựa phế liệu.',
6,7800,N'kg',N'Giá từ 7.800đ/kg',
'~/assets/images/products/nhua1.jpg',
'active',0),

(N'Thu mua máy móc cũ thanh lý','thu-mua-may-moc-cu-thanh-ly',
N'Nhận thu mua máy móc cũ, thiết bị công nghiệp và hàng thanh lý.',
N'Chúng tôi chuyên thu mua máy móc cũ thanh lý từ nhà xưởng, xí nghiệp, cơ sở sản xuất và công trình di dời. Đội ngũ kỹ thuật hỗ trợ khảo sát, tháo dỡ, vận chuyển và báo giá theo tình trạng máy thực tế. Phù hợp cho khách hàng cần giải phóng mặt bằng nhanh và bán trọn gói thiết bị cũ.',
7,0,N'lô',N'Liên hệ báo giá',
'~/assets/images/products/maymoccuthanhly1.jpg',
'active',0);

INSERT INTO product_images (product_id, image_url, caption, order_index)
VALUES

-- Đồng đỏ
(1,'~/assets/images/products/dongdo1.jpg',N'Thu mua đồng đỏ',1),
(1,'~/assets/images/products/dongdo2.jpg',N'Thu mua đồng đỏ',2),
(1,'~/assets/images/products/dongdo3.jpg',N'Thu mua đồng đỏ',3),

-- Đồng vàng
(2,'~/assets/images/products/dongvang1.jpg',N'Thu mua đồng vàng',1),
(2,'~/assets/images/products/dongvang2.jpg',N'Thu mua đồng vàng',2),
(2,'~/assets/images/products/dongvang3.jpg',N'Thu mua đồng vàng',3),

-- Nhôm định hình
(3,'~/assets/images/products/nhomdinhhinh1.jpg',N'Thu mua nhôm định hình',1),
(3,'~/assets/images/products/nhomdinhhinh2.jpg',N'Thu mua nhôm định hình',2),
(3,'~/assets/images/products/nhomdinhhinh3.jpg',N'Thu mua nhôm định hình',3),

-- Nhôm vụn
(4,'~/assets/images/products/nhomvun1.jpg',N'Thu mua nhôm vụn',1),
(4,'~/assets/images/products/nhomvun2.jpg',N'Thu mua nhôm vụn',2),
(4,'~/assets/images/products/nhomvun3.jpg',N'Thu mua nhôm vụn',3),

-- Sắt đặc
(5,'~/assets/images/products/satdac1.jpg',N'Thu mua sắt đặc',1),
(5,'~/assets/images/products/satdac2.jpg',N'Thu mua sắt đặc',2),
(5,'~/assets/images/products/satdac3.jpg',N'Thu mua sắt đặc',3),

-- Sắt vụn
(6,'~/assets/images/products/satvun1.jpg',N'Thu mua sắt vụn',1),
(6,'~/assets/images/products/satvun2.jpg',N'Thu mua sắt vụn',2),
(6,'~/assets/images/products/satvun3.jpg',N'Thu mua sắt vụn',3),

-- Inox
(7,'~/assets/images/products/inox1.jpg',N'Thu mua inox',1),
(7,'~/assets/images/products/inox2.jpg',N'Thu mua inox',2),
(7,'~/assets/images/products/inox3.jpg',N'Thu mua inox',3),

-- Giấy carton
(8,'~/assets/images/products/giaycarton1.jpg',N'Thu mua giấy carton',1),
(8,'~/assets/images/products/giaycarton2.jpg',N'Thu mua giấy carton',2),
(8,'~/assets/images/products/giaycarton3.jpg',N'Thu mua giấy carton',3),

-- Nhựa
(9,'~/assets/images/products/nhua1.jpg',N'Thu mua nhựa',1),
(9,'~/assets/images/products/nhua2.jpg',N'Thu mua nhựa',2),
(9,'~/assets/images/products/nhua3.jpg',N'Thu mua nhựa',3),

-- Máy móc
(10,'~/assets/images/products/maymoccuthanhly1.jpg',N'Thu mua máy móc cũ',1),
(10,'~/assets/images/products/maymoccuthanhly2.jpg',N'Thu mua máy móc cũ',2),
(10,'~/assets/images/products/maymoccuthanhly3.jpg',N'Thu mua máy móc cũ',3);