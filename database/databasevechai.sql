CREATE DATABASE WebPheLieu;
GO
USE WebPheLieu;
GO
CREATE TABLE admins (
    id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50),
    password_hash VARCHAR(255),
    full_name NVARCHAR(100),
    role VARCHAR(30), -- super_admin | editor | staff | sales
    status VARCHAR(20), -- active | inactive
    created_at DATETIME,
    updated_at DATETIME
);

CREATE TABLE blog_posts (
    id INT IDENTITY(1,1) PRIMARY KEY,
    title NVARCHAR(255),
    slug VARCHAR(255),
    excerpt NVARCHAR(MAX),
    content NVARCHAR(MAX),
    cover_image VARCHAR(500),
    author_id INT,
    published_at DATETIME,
    status VARCHAR(20), -- draft | published | hidden
    created_at DATETIME,
    updated_at DATETIME,
    CONSTRAINT FK_blog_posts_admins
        FOREIGN KEY (author_id) REFERENCES admins(id)
);

CREATE TABLE blog_categories (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100),
    slug VARCHAR(255),
    description NVARCHAR(MAX),
    created_at DATETIME,
    updated_at DATETIME
);

CREATE TABLE product_categories (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100),
    slug VARCHAR(255),
    description NVARCHAR(MAX),
    created_at DATETIME,
    updated_at DATETIME
);

CREATE TABLE products (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(255),
    slug VARCHAR(255),
    short_description NVARCHAR(MAX),
    description NVARCHAR(MAX),
    category_id INT,

    price_value DECIMAL(18,2),
    unit NVARCHAR(50),
    price_label NVARCHAR(255), -- 12.000 đ/kg | Liên hệ | Đang cập nhật | Giá thương lượng

    primary_image VARCHAR(500),
    status VARCHAR(20), -- draft | active | inactive | hidden
    is_featured BIT,
    created_at DATETIME,
    updated_at DATETIME,

    CONSTRAINT FK_products_product_categories
        FOREIGN KEY (category_id) REFERENCES product_categories(id)
);

CREATE TABLE blog_post_categories (
    post_id INT,
    category_id INT,
    CONSTRAINT FK_blog_post_categories_blog_posts
        FOREIGN KEY (post_id) REFERENCES blog_posts(id),
    CONSTRAINT FK_blog_post_categories_blog_categories
        FOREIGN KEY (category_id) REFERENCES blog_categories(id)
);

CREATE TABLE blog_post_products (
    post_id INT,
    product_id INT,
    CONSTRAINT FK_blog_post_products_blog_posts
        FOREIGN KEY (post_id) REFERENCES blog_posts(id),
    CONSTRAINT FK_blog_post_products_products
        FOREIGN KEY (product_id) REFERENCES products(id)
);

CREATE TABLE product_images (
    id INT IDENTITY(1,1) PRIMARY KEY,
    product_id INT,
    image_url VARCHAR(500),
    caption NVARCHAR(255),
    order_index INT,
    CONSTRAINT FK_product_images_products
        FOREIGN KEY (product_id) REFERENCES products(id)
);

CREATE TABLE price_history (
    id INT IDENTITY(1,1) PRIMARY KEY,
    product_id INT,
    price_value DECIMAL(18,2),
    price_unit NVARCHAR(50),
    price_type VARCHAR(20), -- fixed | negotiable
    note NVARCHAR(255),
    effective_date DATE,
    recorded_at DATETIME,
    CONSTRAINT FK_price_history_products
        FOREIGN KEY (product_id) REFERENCES products(id)
);

CREATE TABLE contact_requests (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100),
    phone VARCHAR(20),
    email VARCHAR(100),
    request_type VARCHAR(30), -- tu_van | bao_gia | thu_mua
    area NVARCHAR(100),
    message NVARCHAR(MAX),
    source_page VARCHAR(500),
    status VARCHAR(20), -- new | contacted | closed
    created_at DATETIME,
    updated_at DATETIME
);

CREATE TABLE banners (
    id INT IDENTITY(1,1) PRIMARY KEY,
    title NVARCHAR(255),
    subtitle NVARCHAR(MAX),
    sell_text NVARCHAR(255),
    button_primary_text NVARCHAR(100),
    button_primary_link VARCHAR(500),
    button_secondary_text NVARCHAR(100),
    button_secondary_link VARCHAR(500),
    order_index INT,
    status VARCHAR(20), -- active | inactive
    created_at DATETIME,
    updated_at DATETIME
);

CREATE TABLE banner_images (
    id INT IDENTITY(1,1) PRIMARY KEY,
    banner_id INT,
    image_url VARCHAR(500),
    caption NVARCHAR(255),
    order_index INT,
    CONSTRAINT FK_banner_images_banners
        FOREIGN KEY (banner_id) REFERENCES banners(id)
);

CREATE TABLE site_settings (
    id INT IDENTITY(1,1) PRIMARY KEY,
    setting_key VARCHAR(100),
    setting_value NVARCHAR(MAX),
    setting_group VARCHAR(50), -- general | seo | contact | homepage
    description NVARCHAR(255),
    updated_at DATETIME
);