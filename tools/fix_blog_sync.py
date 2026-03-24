# -*- coding: utf-8 -*-
from pathlib import Path
import re

ROOT = Path('.')
blog_path = ROOT / 'blog.html'
main_css_path = ROOT / 'assets/css/main.css'

blog = blog_path.read_text(encoding='utf-8')
main_css = main_css_path.read_text(encoding='utf-8')

style_match = re.search(r'\n\s*<style>\s*(.*?)\s*</style>', blog, flags=re.S)
legacy_css = style_match.group(1).strip() if style_match else None
if style_match:
    blog = re.sub(r'\n\s*<style>\s*.*?\s*</style>', '', blog, count=1, flags=re.S)

old_font = '<link href="https://fonts.googleapis.com/css?family=Open+Sans:300,300i,400,400i,600,600i,700,700i|Roboto:300,400,500,700&display=swap" rel="stylesheet">'
new_font = '''<link
    href="https://fonts.googleapis.com/css2?family=Open+Sans:ital,wght@0,300;0,400;0,500;0,600;0,700;1,300;1,400;1,600;1,700&family=Roboto:ital,wght@0,300;0,400;0,500;0,700;1,300;1,400;1,500;1,700&family=Work+Sans:ital,wght@0,300;0,400;0,500;0,600;0,700;1,300;1,400;1,500;1,600;1,700&display=swap"
    rel="stylesheet">'''
blog = blog.replace(old_font, new_font)

if 'class="blog-page"' not in blog:
    blog = re.sub(r'<body\s*>', '<body class="blog-page">', blog, count=1)

header_block = '''  <header id="header" class="header">
    <div class="container-fluid home-wide-shell">
      <div class="header-top-content">
        <div class="header-top-left">
          <div class="header-top-item">
            <i class="bi bi-telephone-fill"></i>
            <a href="tel:0909123456">Hotline: 0909 123 456</a>
          </div>
          <div class="header-top-item">
            <i class="bi bi-envelope-fill"></i>
            <a href="mailto:info@phelieuhoangdoanh.com">info@phelieuhoangdoanh.com</a>
          </div>
          <div class="header-top-item">
            <i class="bi bi-pin-map-fill"></i>
            <span>Phục vụ TP.HCM, Bình Dương, Đồng Nai</span>
          </div>
          <div class="header-top-item">
            <i class="bi bi-flower1"></i>
            <span class="tag-location">Giới thiệu càng nhiều – hoa hồng càng cao 🔥</span>
          </div>
        </div>
        <div class="header-social">
          <a href="#" title="Facebook" target="_blank"><i class="bi bi-facebook"></i></a>
          <a href="#" title="Zalo" target="_blank"><i class="bi bi-chat-dots"></i></a>
          <a href="#" title="YouTube" target="_blank"><i class="bi bi-youtube"></i></a>
          <a href="#" title="Instagram" target="_blank"><i class="bi bi-instagram"></i></a>
        </div>
      </div>

      <div class="header-main-bar">
        <a href="index.html" class="logo d-flex align-items-center" aria-label="Trang chủ">
          <img src="assets/img/logo2.png" alt="Logo Phế Liệu Hoàng Doanh">
        </a>

        <div class="header-main-actions">
          <i class="mobile-nav-toggle mobile-nav-show bi bi-list"></i>
          <i class="mobile-nav-toggle mobile-nav-hide d-none bi bi-x"></i>
          <nav id="navbar" class="navbar">
            <ul>
              <li><a href="index.html">Trang chủ</a></li>
              <li><a href="about.html">Giới thiệu</a></li>
              <li class="dropdown">
                <a href="services.html">Dịch vụ <i class="bi bi-chevron-down dropdown-indicator"></i></a>
                <ul>
                  <li><a href="services.html">Khảo sát tận nơi</a></li>
                  <li><a href="services.html">Thu mua số lượng lớn</a></li>
                  <li><a href="services.html">Bốc xếp & vận chuyển</a></li>
                  <li><a href="services.html">Dọn kho thanh lý</a></li>
                  <li><a href="services.html">Thu gom định kỳ</a></li>
                </ul>
              </li>
              <li><a href="projects.html">Mặt hàng</a></li>
              <li><a href="blog.html" class="active">Tin tức</a></li>
              <li><a href="contact.html">Liên hệ</a></li>
            </ul>
          </nav>
          <a href="#newsletter-signup" class="header-cta d-none d-lg-inline-flex">
            <i class="bi bi-lightning-charge-fill"></i>
            Báo giá ngay
          </a>
        </div>
      </div>
    </div>
  </header>'''

hero_block = '''  <section class="hero scrap-hero blog-hero d-flex align-items-center">
    <div class="container-fluid home-wide-shell position-relative">
      <div class="row align-items-center gy-5">
        <div class="col-lg-6 hero-content" data-aos="fade-right" data-aos-delay="100">
          <span class="hero-badge"><i class="bi bi-newspaper"></i> Tin tức thị trường phế liệu</span>
          <h1 class="hero-title"><span class="hero-title-line">Cập nhật giá cả</span><span>kinh nghiệm bán phế liệu</span><span class="hero-title-line">và mẹo thanh lý hiệu quả</span><span class="hero-title-tail">dành cho cá nhân & doanh nghiệp</span></h1>
          <p class="blog-hero-lead">Tổng hợp bài viết thực tế về phân loại hàng, xu hướng giá phế liệu, mẹo chụp ảnh báo giá và kinh nghiệm dọn kho xưởng để bạn bán hàng nhanh, đúng giá và dễ làm việc hơn.</p>
          <div class="hero-actions">
            <a href="#blog-posts" class="hero-btn-primary"><i class="bi bi-journal-text"></i> Xem bài viết mới</a>
            <a href="#newsletter-signup" class="hero-btn-secondary"><i class="bi bi-bell-fill"></i> Nhận cập nhật</a>
          </div>
          <div class="blog-hero-meta">
            <div class="blog-hero-stat">
              <strong>06+</strong>
              <span>chủ đề nổi bật</span>
            </div>
            <div class="blog-hero-stat">
              <strong>Hàng tuần</strong>
              <span>cập nhật xu hướng giá</span>
            </div>
            <div class="blog-hero-stat">
              <strong>Thực chiến</strong>
              <span>mẹo dành cho người bán thật</span>
            </div>
          </div>
        </div>

        <div class="col-lg-6" data-aos="fade-left" data-aos-delay="180">
          <div class="blog-hero-panel">
            <div class="blog-hero-panel__card">
              <span class="blog-hero-panel__eyebrow"><i class="bi bi-lightning-charge-fill"></i> Gợi ý đọc nhanh</span>
              <h3>Ưu tiên các bài viết giúp bạn chốt giá nhanh và tránh bị ép giá.</h3>
              <div class="blog-hero-panel__list">
                <div class="blog-hero-panel__item">
                  <i class="bi bi-check2-circle"></i>
                  <span>Phân loại đúng nhóm kim loại trước khi liên hệ báo giá.</span>
                </div>
                <div class="blog-hero-panel__item">
                  <i class="bi bi-check2-circle"></i>
                  <span>Chụp ảnh lô hàng rõ hiện trạng để rút ngắn thời gian khảo sát.</span>
                </div>
                <div class="blog-hero-panel__item">
                  <i class="bi bi-check2-circle"></i>
                  <span>Chuẩn bị vị trí, số lượng và lối xe để việc thu gom thuận lợi hơn.</span>
                </div>
              </div>
            </div>

            <div class="blog-hero-panel__floating">
              <span><i class="bi bi-graph-up-arrow"></i> Theo dõi giá, mẹo bán hàng và cách dọn kho hiệu quả</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>'''

footer_block = '''  <footer id="footer" class="footer">
    <div class="footer-content position-relative">
      <div class="container">
        <div class="row">

          <div class="col-lg-4 col-md-6">
            <div class="footer-info">
              <h3>Phế Liệu Hoàng Doanh</h3>
              <p>
                88 Quốc lộ 1A, P. An Phú Đông <br>
                Quận 12, TP. Hồ Chí Minh<br><br>
                <strong>Hotline:</strong> 0909 123 456<br>
                <strong>Email:</strong> info@phelieuhoangdoanh.com<br>
              </p>
              <div class="social-links d-flex mt-3">
                <a href="#" class="d-flex align-items-center justify-content-center"><i class="bi bi-facebook"></i></a>
                <a href="#" class="d-flex align-items-center justify-content-center"><i class="bi bi-instagram"></i></a>
                <a href="#" class="d-flex align-items-center justify-content-center"><i class="bi bi-youtube"></i></a>
                <a href="tel:0909123456" class="d-flex align-items-center justify-content-center"><i class="bi bi-telephone"></i></a>
              </div>
            </div>
          </div>

          <div class="col-lg-2 col-md-3 footer-links">
            <h4>Liên kết nhanh</h4>
            <ul>
              <li><a href="index.html">Trang chủ</a></li>
              <li><a href="about.html">Giới thiệu</a></li>
              <li><a href="services.html">Dịch vụ</a></li>
              <li><a href="projects.html">Mặt hàng</a></li>
              <li><a href="contact.html">Liên hệ</a></li>
            </ul>
          </div>

          <div class="col-lg-2 col-md-3 footer-links">
            <h4>Chuyên mục</h4>
            <ul>
              <li><a href="blog.html">Kinh nghiệm phân loại</a></li>
              <li><a href="blog.html">Giá thị trường</a></li>
              <li><a href="blog.html">Thanh lý kho xưởng</a></li>
              <li><a href="blog.html">Máy móc cũ</a></li>
              <li><a href="blog.html">Giấy & nhựa</a></li>
            </ul>
          </div>

          <div class="col-lg-2 col-md-3 footer-links">
            <h4>Dịch vụ</h4>
            <ul>
              <li><a href="services.html">Khảo sát tận nơi</a></li>
              <li><a href="services.html">Thu mua số lượng lớn</a></li>
              <li><a href="services.html">Bốc xếp, vận chuyển</a></li>
              <li><a href="services.html">Dọn kho thanh lý</a></li>
              <li><a href="services.html">Thu gom định kỳ</a></li>
            </ul>
          </div>

          <div class="col-lg-2 col-md-3 footer-links">
            <h4>Khu vực phục vụ</h4>
            <ul>
              <li><a href="contact.html">TP. Hồ Chí Minh</a></li>
              <li><a href="contact.html">Bình Dương</a></li>
              <li><a href="contact.html">Đồng Nai</a></li>
              <li><a href="contact.html">Long An</a></li>
              <li><a href="contact.html">Tây Ninh</a></li>
            </ul>
          </div>

        </div>
      </div>
    </div>

    <div class="footer-legal text-center position-relative">
      <div class="container">
        <div class="copyright">
          &copy; Copyright <strong><span>Phế Liệu Hoàng Doanh</span></strong>. All Rights Reserved
        </div>
        <div class="credits">
          Giao diện trang tin tức đã được đồng bộ theo phong cách của trang chủ.
        </div>
      </div>
    </div>
  </footer>'''

social_button_block = '''  <div class="social-button is-open">
    <div class="social-button-content">
      <a href="tel:0909123456" class="call-icon" rel="nofollow" aria-label="Hotline 0909 123 456">
        <i class="fas fa-phone-alt" aria-hidden="true"></i>
        <div class="animated alo-circle"></div>
        <div class="animated alo-circle-fill"></div>
        <span>Hotline: 0909 123 456</span>
      </a>
      <a href="#" class="mes" aria-label="Nhắn tin Facebook">
        <i class="fab fa-facebook-messenger" aria-hidden="true"></i>
        <div class="animated alo-circle"></div>
        <div class="animated alo-circle-fill"></div>
        <span>Nhắn tin Facebook</span>
      </a>
      <a href="#" class="zalo" aria-label="Zalo 0909 123 456">
        <span class="social-button-zalo-text">Zalo</span>
        <div class="animated alo-circle"></div>
        <div class="animated alo-circle-fill"></div>
        <span>Zalo: 0909 123 456</span>
      </a>
    </div>
  </div>'''

blog = re.sub(r'(?s)<header id="header" class="header">.*?</header>', header_block, blog, count=1)
blog = re.sub(r'(?s)<footer id="footer" class="footer">.*?</footer>', footer_block, blog, count=1)
blog = re.sub(r'(?s)<div class="social-button is-open">.*?</div>\s*(<a href="#" class="scroll-top[^>]*>)', social_button_block + '\n\n  ' + r'\1', blog, count=1)

if 'class="hero scrap-hero blog-hero' not in blog:
    blog = re.sub(r'<main id="main">', hero_block + '\n\n  <main id="main" class="blog-page-main">', blog, count=1)
else:
    blog = re.sub(r'<main id="main">', '<main id="main" class="blog-page-main">', blog, count=1)

blog = re.sub(
    r'<section class="blog-layout">\s*<div class="container" data-aos="fade-up">',
    '''    <section id="blog-posts" class="blog-layout home-card-section">\n      <div class="container-fluid home-section-shell" data-aos="fade-up">\n        <div class="section-header blog-section-header">\n          <p class="materials-title">Kho bài viết mới nhất</p>\n          <p>Khám phá các bài viết về giá phế liệu, kinh nghiệm phân loại, thanh lý kho xưởng và các mẹo chuẩn bị giúp quá trình báo giá, khảo sát và thu gom diễn ra nhanh hơn.</p>\n        </div>''',
    blog,
    count=1
)

blog = re.sub(
    r'<section class="tips-section">\s*<div class="container" data-aos="fade-up">',
    '    <section class="tips-section home-card-section">\n      <div class="container-fluid home-section-shell" data-aos="fade-up">',
    blog,
    count=1
)

blog = re.sub(
    r'<section class="newsletter-section">\s*<div class="container" data-aos="fade-up">',
    '    <section id="newsletter-signup" class="newsletter-section home-card-section">\n      <div class="container-fluid home-section-shell" data-aos="fade-up">',
    blog,
    count=1
)

blog = re.sub(
    r'<div class="col-lg-4">\s*<div class="sidebar-card">',
    '          <div class="col-lg-4 blog-sidebar-wrap">\n            <div class="sidebar-card">',
    blog,
    count=1
)

blog_css = '''
/*--------------------------------------------------------------
# Blog page styles aligned with index.html
--------------------------------------------------------------*/
body.blog-page {
  background: linear-gradient(180deg, #f4fbf6 0%, #ffffff 24%, #f6fbf8 100%);
}

body.blog-page .blog-page-main {
  position: relative;
  z-index: 2;
  margin-top: -56px;
}

body.blog-page .blog-hero.scrap-hero {
  min-height: 620px;
  padding: 170px 0 118px;
}

body.blog-page .blog-hero .hero-content {
  max-width: 720px;
}

body.blog-page .blog-hero .hero-title {
  margin-bottom: 22px;
}

body.blog-page .blog-hero-lead {
  max-width: 680px;
  color: rgba(255, 255, 255, 0.92);
  font-size: 18px;
  line-height: 1.82;
  margin-bottom: 26px;
  font-weight: 500;
}

body.blog-page .blog-hero-meta {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 16px;
  margin-top: 28px;
}

body.blog-page .blog-hero-stat {
  padding: 18px 20px;
  border-radius: 24px;
  background: rgba(255, 255, 255, 0.12);
  border: 1px solid rgba(255, 255, 255, 0.12);
  box-shadow: 0 18px 40px rgba(8, 27, 19, 0.16);
}

body.blog-page .blog-hero-stat strong {
  display: block;
  color: #fff;
  font-size: 20px;
  font-weight: 800;
  margin-bottom: 6px;
}

body.blog-page .blog-hero-stat span {
  display: block;
  color: rgba(255, 255, 255, 0.78);
  font-size: 13px;
  text-transform: uppercase;
  letter-spacing: 0.04em;
}

body.blog-page .blog-hero-panel {
  position: relative;
  padding-left: 36px;
}

body.blog-page .blog-hero-panel__card {
  background: rgba(255, 255, 255, 0.96);
  border-radius: 32px;
  padding: 32px;
  box-shadow: 0 30px 70px rgba(10, 37, 26, 0.22);
  color: #1a3528;
}

body.blog-page .blog-hero-panel__eyebrow {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 10px 16px;
  border-radius: 999px;
  background: #f4fbf6;
  color: var(--scrap-green-dark);
  font-size: 13px;
  font-weight: 800;
  margin-bottom: 18px;
}

body.blog-page .blog-hero-panel__card h3 {
  color: var(--scrap-slate);
  font-size: 31px;
  line-height: 1.28;
  margin-bottom: 20px;
}

body.blog-page .blog-hero-panel__list {
  display: grid;
  gap: 14px;
}

body.blog-page .blog-hero-panel__item {
  display: flex;
  gap: 12px;
  align-items: flex-start;
  padding: 14px 16px;
  border-radius: 20px;
  background: #f6fbf8;
  border: 1px solid rgba(21, 128, 61, 0.08);
}

body.blog-page .blog-hero-panel__item i {
  color: var(--scrap-green);
  font-size: 18px;
  margin-top: 2px;
}

body.blog-page .blog-hero-panel__item span {
  color: #42574e;
  line-height: 1.7;
  font-weight: 600;
}

body.blog-page .blog-hero-panel__floating {
  position: absolute;
  left: 0;
  right: 28px;
  bottom: -24px;
  padding: 14px 18px;
  border-radius: 20px;
  background: linear-gradient(135deg, #fff59a 0%, #ffe066 42%, #ffc107 100%);
  box-shadow: 0 16px 36px rgba(255, 193, 7, 0.22);
}

body.blog-page .blog-hero-panel__floating span {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  color: #173326;
  font-size: 14px;
  font-weight: 800;
}

body.blog-page .blog-layout,
body.blog-page .tips-section,
body.blog-page .newsletter-section {
  padding: 0 0 88px;
}

body.blog-page .tips-section,
body.blog-page .newsletter-section {
  background: transparent;
}

body.blog-page .blog-section-header {
  margin-bottom: 34px;
}

body.blog-page .post-card,
body.blog-page .sidebar-card,
body.blog-page .tip-card,
body.blog-page .newsletter-box {
  background: #fff;
  border: 1px solid rgba(21, 128, 61, 0.08);
  border-radius: 30px;
  box-shadow: 0 24px 60px rgba(15, 23, 42, 0.08);
}

body.blog-page .post-card {
  overflow: hidden;
  height: 100%;
  transition: transform 0.35s ease, box-shadow 0.35s ease, border-color 0.35s ease;
}

body.blog-page .post-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 30px 70px rgba(16, 40, 29, 0.12);
  border-color: rgba(251, 191, 36, 0.28);
}

body.blog-page .post-visual {
  background: linear-gradient(180deg, #f7fcf8, #edf7f1);
  padding: 22px;
  border-bottom: 1px solid rgba(21, 128, 61, 0.08);
}

body.blog-page .post-body,
body.blog-page .sidebar-card,
body.blog-page .tip-card,
body.blog-page .newsletter-box {
  padding: 30px;
}

body.blog-page .post-body h3 {
  font-size: 24px;
  line-height: 1.38;
  margin-bottom: 14px;
  color: var(--scrap-slate);
}

body.blog-page .post-body p,
body.blog-page .sidebar-card p,
body.blog-page .tip-card p,
body.blog-page .newsletter-box p,
body.blog-page .mini-post p {
  margin-bottom: 0;
  line-height: 1.8;
}

body.blog-page .post-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 14px;
  margin-bottom: 18px;
  font-size: 14px;
  color: #6f828b;
}

body.blog-page .post-meta span {
  display: inline-flex;
  align-items: center;
  gap: 7px;
}

body.blog-page .post-link {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  color: var(--scrap-green-dark);
  font-weight: 800;
  transition: transform 0.3s ease, color 0.3s ease;
}

body.blog-page .post-link:hover {
  color: #d89d00;
  transform: translateX(4px);
}

body.blog-page .blog-sidebar-wrap {
  position: relative;
}

body.blog-page .sidebar-card {
  margin-bottom: 24px;
}

body.blog-page .sidebar-card:last-child {
  margin-bottom: 0;
}

body.blog-page .sidebar-card h4 {
  margin-bottom: 18px;
  font-size: 22px;
  color: var(--scrap-slate);
}

body.blog-page .search-box,
body.blog-page .newsletter-form {
  display: flex;
  gap: 12px;
}

body.blog-page .search-box input,
body.blog-page .newsletter-form input {
  flex: 1;
  min-width: 0;
  border: 1px solid rgba(21, 128, 61, 0.12);
  border-radius: 18px;
  padding: 14px 18px;
  outline: none;
  background: #f8fcf9;
}

body.blog-page .search-box button,
body.blog-page .newsletter-form button {
  border: none;
  border-radius: 18px;
  background: linear-gradient(135deg, #fff07a 0%, #ffd84d 42%, #f4b400 100%);
  color: #173326;
  padding: 0 20px;
  font-weight: 800;
  box-shadow: 0 12px 24px rgba(244, 180, 0, 0.22);
}

body.blog-page .category-list,
body.blog-page .service-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

body.blog-page .category-list li,
body.blog-page .service-list li {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  padding: 13px 0;
  border-bottom: 1px dashed rgba(21, 128, 61, 0.12);
}

body.blog-page .category-list li:last-child,
body.blog-page .service-list li:last-child {
  border-bottom: none;
  padding-bottom: 0;
}

body.blog-page .category-list a,
body.blog-page .service-list a {
  color: var(--scrap-slate);
  font-weight: 700;
}

body.blog-page .category-list span {
  color: var(--scrap-green);
  font-weight: 800;
}

body.blog-page .mini-post {
  display: flex;
  gap: 14px;
  padding-bottom: 16px;
  margin-bottom: 16px;
  border-bottom: 1px dashed rgba(21, 128, 61, 0.12);
}

body.blog-page .mini-post:last-child {
  padding-bottom: 0;
  margin-bottom: 0;
  border-bottom: none;
}

body.blog-page .mini-thumb {
  width: 86px;
  height: 72px;
  flex-shrink: 0;
  border-radius: 16px;
  overflow: hidden;
  background: linear-gradient(180deg, #f8fbf9, #edf7f1);
  border: 1px solid rgba(31, 95, 67, 0.08);
}

body.blog-page .mini-post h5 {
  font-size: 17px;
  line-height: 1.45;
  margin-bottom: 6px;
  color: var(--scrap-slate);
}

body.blog-page .mini-post span {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  color: #70828b;
  margin-bottom: 6px;
}

body.blog-page .tag-cloud {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

body.blog-page .sidebar-card .tag {
  display: inline-flex;
  align-items: center;
  padding: 9px 14px;
  border-radius: 999px;
  background: var(--scrap-green-soft);
  color: var(--scrap-green-dark);
  font-weight: 700;
  font-size: 13px;
  transition: 0.3s;
}

body.blog-page .sidebar-card .tag:hover {
  background: var(--scrap-orange-soft);
  color: var(--scrap-green-dark);
}

body.blog-page .tip-card {
  height: 100%;
}

body.blog-page .tip-card i {
  width: 64px;
  height: 64px;
  border-radius: 20px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 28px;
  margin-bottom: 18px;
  color: #fff;
  background: linear-gradient(135deg, var(--scrap-green), #2ea56c);
  box-shadow: 0 16px 28px rgba(21, 128, 61, 0.2);
}

body.blog-page .tip-card h4 {
  color: var(--scrap-slate);
  font-size: 22px;
  margin-bottom: 12px;
}

body.blog-page .newsletter-box {
  padding: 38px;
  background: linear-gradient(135deg, rgba(24, 112, 71, 0.98), rgba(30, 126, 86, 0.96));
  color: #fff;
  position: relative;
  overflow: hidden;
}

body.blog-page .newsletter-box:before {
  content: '';
  position: absolute;
  width: 220px;
  height: 220px;
  right: -40px;
  top: -50px;
  background: rgba(243, 156, 18, 0.14);
  border-radius: 50%;
}

body.blog-page .newsletter-box > * {
  position: relative;
  z-index: 1;
}

body.blog-page .newsletter-box h3,
body.blog-page .newsletter-box p,
body.blog-page .newsletter-box label,
body.blog-page .newsletter-box small {
  color: #fff;
}

body.blog-page .newsletter-box p {
  color: rgba(255, 255, 255, 0.86);
  margin-bottom: 20px;
}

body.blog-page .newsletter-form {
  margin-bottom: 14px;
}

body.blog-page .newsletter-form input {
  border: none;
  background: rgba(255, 255, 255, 0.96);
}

body.blog-page .newsletter-box small {
  color: rgba(255, 255, 255, 0.78);
}

@media (max-width: 1199px) {
  body.blog-page .blog-hero-meta {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  body.blog-page .blog-hero-panel {
    padding-left: 0;
  }

  body.blog-page .blog-hero-panel__floating {
    position: static;
    margin-top: 18px;
  }
}

@media (max-width: 991px) {
  body.blog-page .blog-page-main {
    margin-top: -24px;
  }

  body.blog-page .blog-hero.scrap-hero {
    min-height: auto;
    padding: 150px 0 90px;
  }

  body.blog-page .blog-layout,
  body.blog-page .tips-section,
  body.blog-page .newsletter-section {
    padding-bottom: 72px;
  }
}

@media (max-width: 767px) {
  body.blog-page .blog-hero-meta {
    grid-template-columns: 1fr;
  }

  body.blog-page .post-body,
  body.blog-page .sidebar-card,
  body.blog-page .tip-card,
  body.blog-page .newsletter-box {
    padding: 24px;
  }
}

@media (max-width: 575px) {
  body.blog-page .post-card,
  body.blog-page .sidebar-card,
  body.blog-page .tip-card,
  body.blog-page .newsletter-box,
  body.blog-page .blog-hero-panel__card {
    border-radius: 22px;
  }

  body.blog-page .search-box,
  body.blog-page .newsletter-form {
    flex-direction: column;
  }

  body.blog-page .search-box button,
  body.blog-page .newsletter-form button {
    padding: 13px 18px;
  }
}
'''.strip()

if legacy_css and 'Blog page custom styles moved from blog.html' not in main_css:
    main_css = main_css.rstrip() + '\n\n/*--------------------------------------------------------------\n# Blog page custom styles moved from blog.html\n--------------------------------------------------------------*/\n' + legacy_css + '\n'

if 'Blog page styles aligned with index.html' not in main_css:
    main_css = main_css.rstrip() + '\n\n' + blog_css + '\n'

blog_path.write_text(blog, encoding='utf-8', newline='\n')
main_css_path.write_text(main_css, encoding='utf-8', newline='\n')
print('Python fixer is ready.')
