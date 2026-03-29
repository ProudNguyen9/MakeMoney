USE WebPheLieu;
GO

SET NOCOUNT ON;
GO

/*
    Script cập nhật cover_image cho đúng 8 bài viết dịch vụ.
    Logic:
    - Join blog_posts -> blog_post_categories -> blog_categories
    - Chỉ lấy các bài thuộc nhóm dịch vụ
    - Tự map ảnh theo từ khóa trong title
    - Chỉ update khi tìm được ảnh phù hợp
*/

;WITH ServicePostSource AS
(
    SELECT DISTINCT
        bp.id,
        bp.title,
        bp.cover_image,
        bc.id AS category_id,
        bc.name AS category_name,
        bc.slug AS category_slug,
        MatchedCoverImage = CASE
            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%tận nơi%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%tan noi%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%ắc quy%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%ac quy%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%pin ups%'
            THEN '/assets/images/blogs/service/dichvumuatannoi1.jpg'

            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%số lượng lớn%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%so luong lon%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%theo lô%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%theo lo%'
            THEN '/assets/images/blogs/service/dichvusoluonglon1.jpg'

            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%kho phế liệu%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%kho phe lieu%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%kho trong nhà xưởng%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%kho trong nha xuong%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%kho xưởng%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%kho xuong%'
            THEN '/assets/images/blogs/service/dichvukhoxuong1.jpg'

            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%nhà dân%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%nha dan%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%gia đình%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%gia dinh%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%bồn inox%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%bon inox%'
            THEN '/assets/images/blogs/service/dichvunhadan1.jpg'

            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%nhà xưởng%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%nha xuong%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%xác nhà xưởng%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%xac nha xuong%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%trọn gói%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%tron goi%'
            THEN '/assets/images/blogs/service/dichvunhaxuong1.jpg'

            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%công trình%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%cong trinh%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%tháo dỡ%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%thao do%'
            THEN '/assets/images/blogs/service/dichvucongtrinh1.jpg'

            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%doanh nghiệp%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%doanh nghiep%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%hồ sơ giấy%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%ho so giay%'
            THEN '/assets/images/blogs/service/dịchvucongty1.jpg'

            WHEN bp.title COLLATE Latin1_General_CI_AI LIKE N'%giá cao%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%gia cao%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%không bị ép giá%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%khong bi ep gia%'
              OR bp.title COLLATE Latin1_General_CI_AI LIKE N'%checklist%'
            THEN '/assets/images/blogs/service/dichvugiacao1.jpg'

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
), ServicePostsToUpdate AS
(
    SELECT *
    FROM ServicePostSource
    WHERE MatchedCoverImage IS NOT NULL
)
UPDATE bp
SET
    bp.cover_image = spu.MatchedCoverImage,
    bp.updated_at = GETDATE()
FROM blog_posts AS bp
INNER JOIN ServicePostsToUpdate AS spu
    ON spu.id = bp.id;
GO

SELECT
    bp.id,
    bp.title,
    bp.cover_image,
    bc.name AS category_name,
    bc.slug AS category_slug
FROM blog_posts AS bp
INNER JOIN blog_post_categories AS bpc
    ON bpc.post_id = bp.id
INNER JOIN blog_categories AS bc
    ON bc.id = bpc.category_id
WHERE bp.status = 'published'
  AND bp.cover_image IN (
        '/assets/images/blogs/service/dichvumuatannoi1.jpg',
        '/assets/images/blogs/service/dichvusoluonglon1.jpg',
        '/assets/images/blogs/service/dichvukhoxuong1.jpg',
        '/assets/images/blogs/service/dichvunhadan1.jpg',
        '/assets/images/blogs/service/dichvunhaxuong1.jpg',
        '/assets/images/blogs/service/dichvucongtrinh1.jpg',
        '/assets/images/blogs/service/dịchvucongty1.jpg',
        '/assets/images/blogs/service/dichvugiacao1.jpg'
      )
ORDER BY bp.id;
GO
