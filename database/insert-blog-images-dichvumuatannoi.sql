USE WebPheLieu;
GO

SET NOCOUNT ON;
GO

/*
    Script insert 3 ảnh cho toàn bộ 8 nhóm bài dịch vụ vào bảng blog_images.
    Bảng dùng theo cấu trúc user cung cấp:
    - blog_images.id
    - blog_images.blog_id
    - blog_images.image_url
    - blog_images.caption
    - blog_images.order_index

    Logic:
    - Join blog_posts -> blog_post_categories -> blog_categories
    - Chỉ lấy bài thuộc nhóm dịch vụ
    - Match theo title để gán đúng bộ 3 ảnh cho từng bài
    - Insert nếu ảnh chưa tồn tại
*/

;WITH ServicePostSource AS
(
    SELECT DISTINCT
        bp.id,
        bp.title,
        bc.id AS category_id,
        bc.name AS category_name,
        bc.slug AS category_slug,
        ImageGroup = CASE
            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%tận nơi%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%tan noi%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%ắc quy%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%ac quy%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%pin ups%'
            THEN 'dichvumuatannoi'

            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%số lượng lớn%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%so luong lon%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%theo lô%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%theo lo%'
            THEN 'dichvusoluonglon'

            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%kho phế liệu%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%kho phe lieu%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%kho xưởng%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%kho xuong%'
            THEN 'dichvukhoxuong'

            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%nhà dân%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%nha dan%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%gia đình%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%gia dinh%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%bồn inox%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%bon inox%'
            THEN 'dichvunhadan'

            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%nhà xưởng%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%nha xuong%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%xác nhà xưởng%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%xac nha xuong%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%trọn gói%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%tron goi%'
            THEN 'dichvunhaxuong'

            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%công trình%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%cong trinh%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%tháo dỡ%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%thao do%'
            THEN 'dichvucongtrinh'

            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%doanh nghiệp%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%doanh nghiep%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%hồ sơ giấy%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%ho so giay%'
            THEN 'dichvucongty'

            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%giá cao%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%gia cao%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%không bị ép giá%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%khong bi ep gia%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%checklist%'
            THEN 'dichvugiacao'

            ELSE NULL
        END
    FROM blog_posts AS bp
    INNER JOIN blog_post_categories AS bpc
        ON bpc.post_id = bp.id
    INNER JOIN blog_categories AS bc
        ON bc.id = bpc.category_id
    WHERE bp.status = 'published'
      AND (
            bc.id = 1
            OR bc.name COLLATE Latin1_General_CI_AI LIKE N'%dịch vụ%'
            OR bc.name COLLATE Latin1_General_CI_AI LIKE N'%dich vu%'
            OR bc.slug COLLATE Latin1_General_CI_AI LIKE '%dich-vu%'
            OR bc.slug COLLATE Latin1_General_CI_AI LIKE '%service%'
          )
), TargetPosts AS
(
    SELECT *
    FROM ServicePostSource
    WHERE ImageGroup IS NOT NULL
), ImagesToInsert AS
(
    SELECT
        tp.id AS blog_id,
        CONCAT('/assets/images/blogs/service/', tp.ImageGroup, CAST(v.seq AS varchar(10)), '.jpg') AS image_url,
        CONCAT(N'Ảnh bài dịch vụ ', tp.title, N' - ', CAST(v.seq AS nvarchar(10))) AS caption,
        v.seq AS order_index
    FROM TargetPosts AS tp
    CROSS APPLY
    (
        VALUES (1), (2), (3)
    ) AS v(seq)
)
INSERT INTO blog_images (blog_id, image_url, caption, order_index)
SELECT
    iti.blog_id,
    iti.image_url,
    iti.caption,
    iti.order_index
FROM ImagesToInsert AS iti
WHERE NOT EXISTS
(
    SELECT 1
    FROM blog_images AS bi
    WHERE bi.blog_id = iti.blog_id
      AND bi.image_url = iti.image_url
);
GO

SELECT
    bp.id,
    bp.title,
    bi.image_url,
    bi.caption,
    bi.order_index
FROM blog_images AS bi
INNER JOIN blog_posts AS bp
    ON bp.id = bi.blog_id
WHERE bi.image_url LIKE '/assets/images/blogs/service/%'
ORDER BY bp.id, bi.order_index;
GO
