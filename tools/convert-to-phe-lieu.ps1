$ErrorActionPreference = 'Stop'

$projectRoot = Split-Path -Parent $PSScriptRoot

function Write-Utf8File {
  param(
    [string]$Path,
    [string]$Content
  )

  $fullPath = Join-Path $projectRoot $Path
  $directory = Split-Path -Parent $fullPath
  if ($directory -and -not (Test-Path $directory)) {
    New-Item -ItemType Directory -Path $directory -Force | Out-Null
  }

  Set-Content -Path $fullPath -Value $Content -Encoding UTF8
}

function Get-IconSvg {
  param(
    [string]$Type,
    [string]$Primary,
    [string]$Secondary
  )

  switch ($Type) {
    'truck' {
      return @"
<g transform="translate(980 265)">
  <rect x="0" y="150" width="250" height="140" rx="20" fill="$Primary" opacity="0.95"/>
  <rect x="250" y="190" width="150" height="100" rx="18" fill="$Secondary" opacity="0.95"/>
  <rect x="275" y="210" width="55" height="40" rx="8" fill="#ffffff" opacity="0.85"/>
  <rect x="50" y="110" width="120" height="60" rx="16" fill="#ffffff" opacity="0.18"/>
  <rect x="30" y="120" width="180" height="20" rx="10" fill="#ffffff" opacity="0.35"/>
  <circle cx="80" cy="315" r="34" fill="#0f172a"/>
  <circle cx="80" cy="315" r="16" fill="#d1fae5"/>
  <circle cx="300" cy="315" r="34" fill="#0f172a"/>
  <circle cx="300" cy="315" r="16" fill="#d1fae5"/>
  <path d="M30 135 L30 90 L215 90 L215 135" fill="none" stroke="#d1fae5" stroke-width="18" stroke-linecap="round" stroke-linejoin="round" opacity="0.9"/>
</g>
"@
    }
    'magnet' {
      return @"
<g transform="translate(1020 240)">
  <path d="M70 60 C70 10 120 0 165 0 C215 0 260 22 260 80 L260 230 C260 290 228 350 165 350 C102 350 70 290 70 230 Z" fill="$Primary" opacity="0.96"/>
  <path d="M130 80 L130 230 C130 255 144 290 165 290 C186 290 200 255 200 230 L200 80" fill="#0f172a" opacity="0.85"/>
  <rect x="62" y="210" width="78" height="82" rx="18" fill="#f8fafc"/>
  <rect x="190" y="210" width="78" height="82" rx="18" fill="$Secondary"/>
  <rect x="90" y="40" width="150" height="36" rx="18" fill="#d1fae5" opacity="0.28"/>
  <circle cx="340" cy="100" r="26" fill="$Secondary" opacity="0.9"/>
  <circle cx="375" cy="162" r="16" fill="#d1fae5" opacity="0.85"/>
  <circle cx="15" cy="132" r="18" fill="#d1fae5" opacity="0.75"/>
</g>
"@
    }
    'paper' {
      return @"
<g transform="translate(1000 250)">
  <rect x="40" y="180" width="260" height="150" rx="20" fill="$Primary" opacity="0.92"/>
  <rect x="80" y="120" width="260" height="150" rx="20" fill="$Secondary" opacity="0.92"/>
  <rect x="120" y="60" width="260" height="150" rx="20" fill="#d1fae5" opacity="0.95"/>
  <line x1="80" y1="195" x2="300" y2="195" stroke="#f8fafc" stroke-width="16" stroke-linecap="round" opacity="0.4"/>
  <line x1="120" y1="135" x2="340" y2="135" stroke="#0f172a" stroke-width="16" stroke-linecap="round" opacity="0.25"/>
  <line x1="160" y1="75" x2="380" y2="75" stroke="#0f172a" stroke-width="16" stroke-linecap="round" opacity="0.18"/>
  <rect x="10" y="360" width="400" height="24" rx="12" fill="#f8fafc" opacity="0.25"/>
</g>
"@
    }
    'plastic' {
      return @"
<g transform="translate(1010 235)">
  <path d="M90 70 L150 70 L165 115 L165 280 C165 312 139 340 107 340 C75 340 50 314 50 280 L50 115 Z" fill="$Primary" opacity="0.94"/>
  <rect x="74" y="32" width="92" height="42" rx="14" fill="#d1fae5" opacity="0.92"/>
  <rect x="70" y="150" width="76" height="18" rx="9" fill="#f8fafc" opacity="0.35"/>
  <path d="M250 115 L310 115 L330 160 L330 285 C330 317 304 345 272 345 C240 345 214 319 214 285 L214 160 Z" fill="$Secondary" opacity="0.94"/>
  <rect x="238" y="80" width="92" height="38" rx="14" fill="#f8fafc" opacity="0.85"/>
  <rect x="232" y="192" width="80" height="16" rx="8" fill="#0f172a" opacity="0.18"/>
  <circle cx="400" cy="140" r="42" fill="#d1fae5" opacity="0.3"/>
  <circle cx="440" cy="240" r="22" fill="#f8fafc" opacity="0.28"/>
</g>
"@
    }
    'electronic' {
      return @"
<g transform="translate(970 245)">
  <rect x="30" y="40" width="360" height="220" rx="22" fill="$Primary" opacity="0.96"/>
  <rect x="58" y="68" width="304" height="164" rx="14" fill="#0f172a" opacity="0.9"/>
  <path d="M210 260 L240 260 L270 315 L180 315 Z" fill="#d1fae5" opacity="0.8"/>
  <rect x="150" y="315" width="150" height="26" rx="13" fill="$Secondary" opacity="0.92"/>
  <rect x="430" y="90" width="120" height="220" rx="20" fill="$Secondary" opacity="0.96"/>
  <rect x="455" y="128" width="70" height="10" rx="5" fill="#ffffff" opacity="0.45"/>
  <circle cx="490" cy="250" r="24" fill="#0f172a" opacity="0.2"/>
  <path d="M105 150 H315 M160 110 L220 170 L255 135" stroke="#34d399" stroke-width="18" stroke-linecap="round" stroke-linejoin="round" opacity="0.88"/>
</g>
"@
    }
    'scale' {
      return @"
<g transform="translate(1000 230)">
  <rect x="165" y="45" width="70" height="250" rx="20" fill="$Primary" opacity="0.96"/>
  <rect x="70" y="285" width="260" height="34" rx="17" fill="$Secondary" opacity="0.96"/>
  <rect x="50" y="85" width="300" height="18" rx="9" fill="#d1fae5" opacity="0.78"/>
  <line x1="120" y1="104" x2="90" y2="190" stroke="#f8fafc" stroke-width="8"/>
  <line x1="280" y1="104" x2="310" y2="190" stroke="#f8fafc" stroke-width="8"/>
  <path d="M45 190 H135 L112 245 H68 Z" fill="$Primary" opacity="0.92"/>
  <path d="M265 190 H355 L332 245 H288 Z" fill="$Primary" opacity="0.92"/>
  <circle cx="430" cy="145" r="45" fill="#d1fae5" opacity="0.3"/>
  <circle cx="460" cy="220" r="20" fill="#f8fafc" opacity="0.24"/>
</g>
"@
    }
    'factory' {
      return @"
<g transform="translate(980 250)">
  <rect x="40" y="180" width="420" height="170" rx="24" fill="$Primary" opacity="0.95"/>
  <rect x="100" y="85" width="70" height="110" rx="16" fill="$Secondary" opacity="0.95"/>
  <rect x="200" y="120" width="70" height="75" rx="16" fill="$Secondary" opacity="0.95"/>
  <rect x="300" y="55" width="70" height="140" rx="16" fill="$Secondary" opacity="0.95"/>
  <rect x="395" y="105" width="45" height="90" rx="14" fill="#d1fae5" opacity="0.9"/>
  <path d="M65 225 L140 185 L140 225 L220 175 L220 225 L315 165 L315 225 L420 145 L420 350 L65 350 Z" fill="#0f172a" opacity="0.22"/>
  <rect x="105" y="240" width="42" height="42" rx="10" fill="#d1fae5" opacity="0.6"/>
  <rect x="175" y="240" width="42" height="42" rx="10" fill="#d1fae5" opacity="0.6"/>
  <rect x="245" y="240" width="42" height="42" rx="10" fill="#d1fae5" opacity="0.6"/>
</g>
"@
    }
    'recycle' {
      return @"
<g transform="translate(1010 240)">
  <path d="M175 25 L260 165 L200 165 L255 255 L145 255 L95 165 L38 165 Z" fill="$Primary" opacity="0.95"/>
  <path d="M365 135 L310 230 L278 175 L180 175 L235 80 L262 33 Z" fill="$Secondary" opacity="0.95"/>
  <path d="M180 285 L300 285 L270 340 L322 430 L210 430 L156 430 L124 373 Z" fill="#d1fae5" opacity="0.92"/>
  <circle cx="230" cy="220" r="38" fill="#ffffff" opacity="0.16"/>
</g>
"@
    }
    'contract' {
      return @"
<g transform="translate(1000 235)">
  <rect x="70" y="40" width="280" height="360" rx="24" fill="$Primary" opacity="0.96"/>
  <rect x="112" y="85" width="190" height="20" rx="10" fill="#d1fae5" opacity="0.4"/>
  <rect x="112" y="132" width="160" height="20" rx="10" fill="#d1fae5" opacity="0.4"/>
  <rect x="112" y="179" width="200" height="20" rx="10" fill="#d1fae5" opacity="0.4"/>
  <path d="M120 292 L170 342 L280 220" fill="none" stroke="$Secondary" stroke-width="26" stroke-linecap="round" stroke-linejoin="round"/>
  <circle cx="435" cy="320" r="72" fill="$Secondary" opacity="0.96"/>
  <path d="M400 320 L425 345 L472 290" fill="none" stroke="#0f172a" stroke-width="16" stroke-linecap="round" stroke-linejoin="round"/>
</g>
"@
    }
    'payment' {
      return @"
<g transform="translate(995 260)">
  <rect x="35" y="120" width="360" height="210" rx="26" fill="$Primary" opacity="0.95"/>
  <rect x="70" y="155" width="290" height="140" rx="20" fill="#d1fae5" opacity="0.18"/>
  <circle cx="215" cy="225" r="48" fill="$Secondary" opacity="0.95"/>
  <text x="215" y="238" text-anchor="middle" fill="#0f172a" font-size="46" font-weight="800" font-family="'Segoe UI', Arial, sans-serif">₫</text>
  <rect x="430" y="160" width="110" height="155" rx="22" fill="$Secondary" opacity="0.95"/>
  <circle cx="485" cy="215" r="28" fill="#0f172a" opacity="0.16"/>
  <rect x="455" y="255" width="60" height="16" rx="8" fill="#0f172a" opacity="0.18"/>
</g>
"@
    }
    'phone' {
      return @"
<g transform="translate(1015 245)">
  <rect x="100" y="30" width="200" height="360" rx="34" fill="$Primary" opacity="0.96"/>
  <rect x="125" y="70" width="150" height="255" rx="20" fill="#0f172a" opacity="0.85"/>
  <circle cx="200" cy="355" r="18" fill="#d1fae5" opacity="0.85"/>
  <path d="M410 135 C410 83 368 40 315 40 H300" fill="none" stroke="$Secondary" stroke-width="20" stroke-linecap="round"/>
  <path d="M450 185 C450 110 392 52 317 52" fill="none" stroke="#d1fae5" stroke-width="14" stroke-linecap="round" opacity="0.78"/>
  <circle cx="215" cy="160" r="34" fill="$Secondary" opacity="0.95"/>
  <path d="M195 160 L210 175 L236 144" fill="none" stroke="#0f172a" stroke-width="12" stroke-linecap="round" stroke-linejoin="round"/>
</g>
"@
    }
    default {
      return @"
<g transform="translate(1040 245)">
  <circle cx="180" cy="180" r="150" fill="$Primary" opacity="0.96"/>
  <circle cx="270" cy="255" r="110" fill="$Secondary" opacity="0.92"/>
  <circle cx="125" cy="125" r="40" fill="#d1fae5" opacity="0.85"/>
</g>
"@
    }
  }
}

function New-SceneSvg {
  param(
    [string]$Path,
    [string]$Title,
    [string]$Subtitle,
    [string]$Primary,
    [string]$Secondary,
    [string]$Type
  )

  $icon = Get-IconSvg -Type $Type -Primary $Primary -Secondary $Secondary

  $svg = @"
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1600 900" role="img" aria-labelledby="title desc">
  <title>$Title</title>
  <desc>$Subtitle</desc>
  <defs>
    <linearGradient id="bg" x1="0" y1="0" x2="1" y2="1">
      <stop offset="0%" stop-color="$Primary" />
      <stop offset="100%" stop-color="$Secondary" />
    </linearGradient>
    <linearGradient id="glass" x1="0" y1="0" x2="1" y2="1">
      <stop offset="0%" stop-color="#ffffff" stop-opacity="0.28" />
      <stop offset="100%" stop-color="#ffffff" stop-opacity="0.06" />
    </linearGradient>
  </defs>
  <rect width="1600" height="900" fill="#081018" />
  <circle cx="1350" cy="160" r="210" fill="$Secondary" opacity="0.15" />
  <circle cx="1260" cy="640" r="250" fill="$Primary" opacity="0.14" />
  <rect x="55" y="55" width="1490" height="790" rx="42" fill="url(#bg)" opacity="0.18" />
  <rect x="90" y="95" width="1420" height="710" rx="36" fill="url(#glass)" stroke="#ffffff" stroke-opacity="0.08" />
  <rect x="110" y="120" width="860" height="660" rx="34" fill="#0a1520" opacity="0.82" />
  <text x="150" y="190" fill="#d1fae5" font-size="24" font-weight="700" letter-spacing="4" font-family="'Segoe UI', Arial, sans-serif">PHE LIEU THANH PHAT</text>
  <text x="150" y="310" fill="#f8fafc" font-size="72" font-weight="800" font-family="'Segoe UI', Arial, sans-serif">$Title</text>
  <text x="150" y="390" fill="#d7e3f0" font-size="34" font-weight="500" font-family="'Segoe UI', Arial, sans-serif">$Subtitle</text>
  <rect x="150" y="450" width="320" height="14" rx="7" fill="$Secondary" opacity="0.8" />
  <rect x="150" y="500" width="560" height="18" rx="9" fill="#d1fae5" opacity="0.16" />
  <rect x="150" y="545" width="500" height="18" rx="9" fill="#d1fae5" opacity="0.16" />
  <rect x="150" y="590" width="420" height="18" rx="9" fill="#d1fae5" opacity="0.16" />
  <rect x="150" y="660" width="260" height="72" rx="36" fill="$Primary" opacity="0.95" />
  <text x="280" y="705" text-anchor="middle" fill="#f8fafc" font-size="28" font-weight="700" font-family="'Segoe UI', Arial, sans-serif">THU MUA TAN NOI</text>
  $icon
</svg>
"@

  Write-Utf8File -Path $Path -Content $svg
}

function New-AvatarSvg {
  param(
    [string]$Path,
    [string]$Name,
    [string]$Role,
    [string]$Primary,
    [string]$Secondary
  )

  $svg = @"
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 600 600" role="img" aria-labelledby="title desc">
  <title>$Name</title>
  <desc>$Role</desc>
  <defs>
    <linearGradient id="avatarBg" x1="0" y1="0" x2="1" y2="1">
      <stop offset="0%" stop-color="$Primary" />
      <stop offset="100%" stop-color="$Secondary" />
    </linearGradient>
  </defs>
  <rect width="600" height="600" rx="46" fill="#0b1320" />
  <circle cx="300" cy="300" r="240" fill="url(#avatarBg)" opacity="0.28" />
  <circle cx="300" cy="210" r="86" fill="#f5d7bb" />
  <path d="M180 360 C205 300 260 270 300 270 C340 270 395 300 420 360 L430 390 C438 415 418 438 392 438 H208 C182 438 162 415 170 390 Z" fill="$Primary" />
  <rect x="212" y="118" width="176" height="38" rx="18" fill="#ffd166" />
  <rect x="236" y="92" width="128" height="42" rx="20" fill="#ffb703" />
  <rect x="240" y="444" width="120" height="14" rx="7" fill="#d1fae5" opacity="0.4" />
  <text x="300" y="506" text-anchor="middle" fill="#f8fafc" font-size="30" font-weight="800" font-family="'Segoe UI', Arial, sans-serif">$Name</text>
  <text x="300" y="546" text-anchor="middle" fill="#d1fae5" font-size="20" font-weight="600" font-family="'Segoe UI', Arial, sans-serif">$Role</text>
</svg>
"@

  Write-Utf8File -Path $Path -Content $svg
}

function Get-NavItem {
  param(
    [string]$Href,
    [string]$Label,
    [string]$Key,
    [string]$Active
  )

  $class = if ($Key -eq $Active) { ' class="active"' } else { '' }
  return ('          <li><a href="{0}"{1}>{2}</a></li>' -f $Href, $class, $Label)
}

function Get-Head {
  param(
    [string]$Title,
    [string]$Description,
    [string]$Keywords
  )

  return @"
<!DOCTYPE html>
<html lang="vi">

<head>
  <meta charset="utf-8">
  <meta content="width=device-width, initial-scale=1.0" name="viewport">

  <title>$Title</title>
  <meta content="$Description" name="description">
  <meta content="$Keywords" name="keywords">

  <link href="assets/img/favicon.png" rel="icon">
  <link href="assets/img/apple-touch-icon.png" rel="apple-touch-icon">

  <link rel="preconnect" href="https://fonts.googleapis.com">
  <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
  <link href="https://fonts.googleapis.com/css2?family=Open+Sans:ital,wght@0,300;0,400;0,500;0,600;0,700;1,300;1,400;1,600;1,700&family=Roboto:ital,wght@0,300;0,400;0,500;0,600;0,700;1,300;1,400;1,500;1,600;1,700&family=Work+Sans:ital,wght@0,300;0,400;0,500;0,600;0,700;1,300;1,400;1,500;1,600;1,700&display=swap" rel="stylesheet">

  <link href="assets/vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet">
  <link href="assets/vendor/bootstrap-icons/bootstrap-icons.css" rel="stylesheet">
  <link href="assets/vendor/fontawesome-free/css/all.min.css" rel="stylesheet">
  <link href="assets/vendor/aos/aos.css" rel="stylesheet">
  <link href="assets/vendor/glightbox/css/glightbox.min.css" rel="stylesheet">
  <link href="assets/vendor/swiper/swiper-bundle.min.css" rel="stylesheet">

  <link href="assets/css/main.css" rel="stylesheet">
  <link href="assets/css/scrap-theme.css" rel="stylesheet">
</head>

<body>
"@
}

function Get-Header {
  param([string]$Active)

  $nav = @(
    (Get-NavItem -Href 'index.html' -Label 'Trang chủ' -Key 'home' -Active $Active),
    (Get-NavItem -Href 'about.html' -Label 'Giới thiệu' -Key 'about' -Active $Active),
    (Get-NavItem -Href 'services.html' -Label 'Dịch vụ' -Key 'services' -Active $Active),
    (Get-NavItem -Href 'projects.html' -Label 'Mặt hàng' -Key 'projects' -Active $Active),
    (Get-NavItem -Href 'blog.html' -Label 'Tin tức' -Key 'blog' -Active $Active),
    (Get-NavItem -Href 'contact.html' -Label 'Liên hệ' -Key 'contact' -Active $Active)
  ) -join "`n"

  return @"
  <header id="header" class="header d-flex align-items-center">
    <div class="container-fluid container-xl d-flex align-items-center justify-content-between">
      <a href="index.html" class="logo d-flex align-items-center">
        <h1>Phế Liệu Thành Phát<span>.</span></h1>
      </a>

      <i class="mobile-nav-toggle mobile-nav-show bi bi-list"></i>
      <i class="mobile-nav-toggle mobile-nav-hide d-none bi bi-x"></i>
      <nav id="navbar" class="navbar">
        <ul>
$nav
        </ul>
      </nav>
    </div>
  </header>
"@
}

function Get-Breadcrumb {
  param([string]$Title)

  return @"
    <div class="breadcrumbs d-flex align-items-center" style="background-image: url('assets/img/scrap/breadcrumbs.svg');">
      <div class="container position-relative d-flex flex-column align-items-center" data-aos="fade">
        <h2>$Title</h2>
        <ol>
          <li><a href="index.html">Trang chủ</a></li>
          <li>$Title</li>
        </ol>
      </div>
    </div>
"@
}

function Get-TestimonialsSection {
  return @"
    <section id="testimonials" class="testimonials section-bg">
      <div class="container" data-aos="fade-up">
        <div class="section-header">
          <h2>Khách hàng nói gì</h2>
          <p>Doanh nghiệp, nhà xưởng và hộ kinh doanh đánh giá cao quy trình thu mua minh bạch, cân đúng và thanh toán nhanh của chúng tôi.</p>
        </div>

        <div class="slides-2 swiper">
          <div class="swiper-wrapper">
            <div class="swiper-slide">
              <div class="testimonial-wrap">
                <div class="testimonial-item">
                  <img src="assets/img/scrap/team-1.svg" class="testimonial-img" alt="Khách hàng thu mua phế liệu">
                  <h3>Công ty Cơ khí An Phú</h3>
                  <h4>Thu mua thép và máy móc thanh lý</h4>
                  <div class="stars"><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i></div>
                  <p><i class="bi bi-quote quote-icon-left"></i>Đội khảo sát đến đúng giờ, báo giá rõ ràng theo từng hạng mục sắt thép và thanh toán ngay sau khi cân xong.<i class="bi bi-quote quote-icon-right"></i></p>
                </div>
              </div>
            </div>

            <div class="swiper-slide">
              <div class="testimonial-wrap">
                <div class="testimonial-item">
                  <img src="assets/img/scrap/team-2.svg" class="testimonial-img" alt="Khách hàng thu mua phế liệu">
                  <h3>Xưởng Bao bì Minh Khang</h3>
                  <h4>Thu mua giấy carton và nhựa tồn kho</h4>
                  <div class="stars"><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i></div>
                  <p><i class="bi bi-quote quote-icon-left"></i>Giá thu mua cạnh tranh, có xe tải và nhân công bốc xếp nên chúng tôi không phải mất thêm chi phí phát sinh.<i class="bi bi-quote quote-icon-right"></i></p>
                </div>
              </div>
            </div>

            <div class="swiper-slide">
              <div class="testimonial-wrap">
                <div class="testimonial-item">
                  <img src="assets/img/scrap/team-3.svg" class="testimonial-img" alt="Khách hàng thu mua phế liệu">
                  <h3>Nhà máy Nhôm Việt Thành</h3>
                  <h4>Thu mua nhôm, inox, đồng</h4>
                  <div class="stars"><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i></div>
                  <p><i class="bi bi-quote quote-icon-left"></i>Chúng tôi cần đối tác lâu dài và Thành Phát đáp ứng tốt từ hồ sơ, hợp đồng cho đến lịch thu gom định kỳ mỗi tuần.<i class="bi bi-quote quote-icon-right"></i></p>
                </div>
              </div>
            </div>

            <div class="swiper-slide">
              <div class="testimonial-wrap">
                <div class="testimonial-item">
                  <img src="assets/img/scrap/team-4.svg" class="testimonial-img" alt="Khách hàng thu mua phế liệu">
                  <h3>Cửa hàng điện lạnh Hải Nam</h3>
                  <h4>Thu mua thiết bị điện tử cũ</h4>
                  <div class="stars"><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i></div>
                  <p><i class="bi bi-quote quote-icon-left"></i>Máy lạnh hỏng, block cũ, dây đồng và bo mạch đều được phân loại kỹ nên giá cao hơn các đơn vị thu gom nhỏ lẻ.<i class="bi bi-quote quote-icon-right"></i></p>
                </div>
              </div>
            </div>

            <div class="swiper-slide">
              <div class="testimonial-wrap">
                <div class="testimonial-item">
                  <img src="assets/img/scrap/team-5.svg" class="testimonial-img" alt="Khách hàng thu mua phế liệu">
                  <h3>Kho vận Tân Lộc</h3>
                  <h4>Dọn kho và thanh lý hàng tồn</h4>
                  <div class="stars"><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i></div>
                  <p><i class="bi bi-quote quote-icon-left"></i>Khâu làm việc rất chuyên nghiệp, có biên bản cân, hình ảnh hiện trạng và bàn giao mặt bằng gọn gàng sau khi thu dọn.<i class="bi bi-quote quote-icon-right"></i></p>
                </div>
              </div>
            </div>
          </div>
          <div class="swiper-pagination"></div>
        </div>
      </div>
    </section>
"@
}

function Get-RecentPostsSection {
  return @"
    <section id="recent-blog-posts" class="recent-blog-posts">
      <div class="container" data-aos="fade-up">
        <div class="section-header">
          <h2>Tin thị trường phế liệu</h2>
          <p>Cập nhật giá, kinh nghiệm phân loại và các lưu ý khi thanh lý phế liệu số lượng lớn.</p>
        </div>

        <div class="row gy-5">
          <div class="col-xl-4 col-md-6" data-aos="fade-up" data-aos-delay="100">
            <div class="post-item position-relative h-100">
              <div class="post-img position-relative overflow-hidden">
                <img src="assets/img/scrap/blog-price.svg" class="img-fluid" alt="Biến động giá phế liệu">
                <span class="post-date">Tháng 03</span>
              </div>
              <div class="post-content d-flex flex-column">
                <h3 class="post-title">Bảng giá thu mua sắt, đồng, nhôm, inox cập nhật theo thị trường</h3>
                <div class="meta d-flex align-items-center">
                  <div class="d-flex align-items-center"><i class="bi bi-person"></i><span class="ps-2">Ban biên tập</span></div>
                  <span class="px-3 text-black-50">/</span>
                  <div class="d-flex align-items-center"><i class="bi bi-folder2"></i><span class="ps-2">Bảng giá</span></div>
                </div>
                <hr>
                <a href="blog-details.html" class="readmore stretched-link"><span>Xem chi tiết</span><i class="bi bi-arrow-right"></i></a>
              </div>
            </div>
          </div>

          <div class="col-xl-4 col-md-6" data-aos="fade-up" data-aos-delay="200">
            <div class="post-item position-relative h-100">
              <div class="post-img position-relative overflow-hidden">
                <img src="assets/img/scrap/blog-sorting.svg" class="img-fluid" alt="Phân loại phế liệu">
                <span class="post-date">Tháng 03</span>
              </div>
              <div class="post-content d-flex flex-column">
                <h3 class="post-title">5 cách phân loại phế liệu tại kho để bán được giá cao hơn</h3>
                <div class="meta d-flex align-items-center">
                  <div class="d-flex align-items-center"><i class="bi bi-person"></i><span class="ps-2">Phòng thu mua</span></div>
                  <span class="px-3 text-black-50">/</span>
                  <div class="d-flex align-items-center"><i class="bi bi-folder2"></i><span class="ps-2">Kinh nghiệm</span></div>
                </div>
                <hr>
                <a href="blog-details.html" class="readmore stretched-link"><span>Xem chi tiết</span><i class="bi bi-arrow-right"></i></a>
              </div>
            </div>
          </div>

          <div class="col-xl-4 col-md-6" data-aos="fade-up" data-aos-delay="300">
            <div class="post-item position-relative h-100">
              <div class="post-img position-relative overflow-hidden">
                <img src="assets/img/scrap/blog-storage.svg" class="img-fluid" alt="Lưu kho phế liệu">
                <span class="post-date">Tháng 03</span>
              </div>
              <div class="post-content d-flex flex-column">
                <h3 class="post-title">Lưu ý an toàn khi lưu trữ phế liệu giấy, nhựa và thiết bị điện tử</h3>
                <div class="meta d-flex align-items-center">
                  <div class="d-flex align-items-center"><i class="bi bi-person"></i><span class="ps-2">Bộ phận vận hành</span></div>
                  <span class="px-3 text-black-50">/</span>
                  <div class="d-flex align-items-center"><i class="bi bi-folder2"></i><span class="ps-2">Vận hành kho</span></div>
                </div>
                <hr>
                <a href="blog-details.html" class="readmore stretched-link"><span>Xem chi tiết</span><i class="bi bi-arrow-right"></i></a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
"@
}

function Get-Footer {
  return @"
  <footer id="footer" class="footer">
    <div class="footer-content position-relative">
      <div class="container">
        <div class="row">
          <div class="col-lg-4 col-md-6">
            <div class="footer-info">
              <h3>Phế Liệu Thành Phát</h3>
              <p>
                88 Quốc lộ 1A, P. An Phú Đông <br>
                Quận 12, TP. Hồ Chí Minh<br><br>
                <strong>Hotline:</strong> 0909 686 889<br>
                <strong>Email:</strong> info@phelieuthanhphat.vn<br>
              </p>
              <div class="social-links d-flex mt-3">
                <a href="#" class="d-flex align-items-center justify-content-center"><i class="bi bi-facebook"></i></a>
                <a href="#" class="d-flex align-items-center justify-content-center"><i class="bi bi-instagram"></i></a>
                <a href="#" class="d-flex align-items-center justify-content-center"><i class="bi bi-youtube"></i></a>
                <a href="#" class="d-flex align-items-center justify-content-center"><i class="bi bi-telephone"></i></a>
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
            <h4>Dịch vụ</h4>
            <ul>
              <li><a href="service-details.html">Thu mua kim loại</a></li>
              <li><a href="service-details.html">Thu mua giấy carton</a></li>
              <li><a href="service-details.html">Thu mua nhựa công nghiệp</a></li>
              <li><a href="service-details.html">Thanh lý nhà xưởng</a></li>
              <li><a href="service-details.html">Dọn kho tận nơi</a></li>
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

          <div class="col-lg-2 col-md-3 footer-links">
            <h4>Cam kết</h4>
            <ul>
              <li><a href="sample-inner-page.html">Báo giá minh bạch</a></li>
              <li><a href="sample-inner-page.html">Cân đo chuẩn xác</a></li>
              <li><a href="sample-inner-page.html">Thanh toán nhanh</a></li>
              <li><a href="sample-inner-page.html">Thu gom an toàn</a></li>
              <li><a href="sample-inner-page.html">Hỗ trợ 24/7</a></li>
            </ul>
          </div>
        </div>
      </div>
    </div>

    <div class="footer-legal text-center position-relative">
      <div class="container">
        <div class="copyright">
          &copy; Copyright <strong><span>Phế Liệu Thành Phát</span></strong>. All Rights Reserved
        </div>
        <div class="credits">
          Giao diện được tùy biến lại theo mô hình website thu mua phế liệu.
        </div>
      </div>
    </div>
  </footer>

  <a href="#" class="scroll-top d-flex align-items-center justify-content-center"><i class="bi bi-arrow-up-short"></i></a>
  <div id="preloader"></div>

  <script src="assets/vendor/bootstrap/js/bootstrap.bundle.min.js"></script>
  <script src="assets/vendor/aos/aos.js"></script>
  <script src="assets/vendor/glightbox/js/glightbox.min.js"></script>
  <script src="assets/vendor/isotope-layout/isotope.pkgd.min.js"></script>
  <script src="assets/vendor/swiper/swiper-bundle.min.js"></script>
  <script src="assets/vendor/purecounter/purecounter_vanilla.js"></script>
  <script src="assets/vendor/php-email-form/validate.js"></script>
  <script src="assets/js/main.js"></script>
</body>

</html>
"@
}

$scrapThemeCss = @"
:root {
  --scrap-green: #2d6a4f;
  --scrap-green-dark: #1b4332;
  --scrap-gold: #f4a261;
  --scrap-light: #d1fae5;
}

.header,
.footer:before {
  background: rgba(17, 44, 34, 0.96) !important;
}

.header .logo h1 span,
.footer .copyright span,
.section-header h2:after,
.portfolio .portfolio-flters li.filter-active,
.portfolio .portfolio-flters li:hover,
.scroll-top,
.php-email-form button,
.hero .info .btn-get-started {
  color: #fff;
  background: var(--scrap-green) !important;
  border-color: var(--scrap-green) !important;
}

.section-header h2:after {
  content: "";
  display: block;
  width: 70px;
  height: 4px;
  margin: 14px auto 0;
  background: var(--scrap-green) !important;
}

.header .logo h1 span,
.info-item i,
.icon-box i,
.stats-item i,
.service-item .icon i,
.post-content .meta i,
.readmore,
.portfolio-info h4,
.about .our-story h4,
.breadcrumbs h2,
.footer .social-links a:hover {
  color: var(--scrap-green) !important;
}

.hero .info h2 span,
.post-date,
.testimonials .testimonial-item .stars i,
.service-details .services-list a.active,
.portfolio .portfolio-flters li.filter-active {
  color: var(--scrap-gold) !important;
}

.php-email-form button:hover,
.hero .info .btn-get-started:hover,
.scroll-top:hover {
  background: var(--scrap-green-dark) !important;
  border-color: var(--scrap-green-dark) !important;
}

.img-bg,
.card-bg,
.about-img,
.breadcrumbs {
  background-size: cover !important;
  background-position: center !important;
}

.breadcrumbs:before,
.hero:before {
  background: rgba(6, 15, 11, 0.55) !important;
}

.member .member-img,
.testimonial-item,
.portfolio-content,
.post-item,
.info-item,
.service-item,
.card-item {
  border-radius: 16px;
  overflow: hidden;
}

.footer .social-links a {
  border-color: rgba(255,255,255,.24);
}

.footer .social-links a:hover {
  border-color: var(--scrap-green);
}

@media (max-width: 768px) {
  .hero .info h2 {
    font-size: 34px;
  }
}
"@

Write-Utf8File -Path 'assets/css/scrap-theme.css' -Content $scrapThemeCss

New-SceneSvg -Path 'assets/img/scrap/breadcrumbs.svg' -Title 'Thu mua phế liệu tận nơi' -Subtitle 'Khảo sát nhanh - cân chuẩn - thanh toán ngay' -Primary '#1b4332' -Secondary '#f4a261' -Type 'truck'
New-SceneSvg -Path 'assets/img/scrap/hero-1.svg' -Title 'Thu mua phế liệu giá cao' -Subtitle 'Phục vụ hộ gia đình, kho xưởng và công trình' -Primary '#2d6a4f' -Secondary '#ffb703' -Type 'truck'
New-SceneSvg -Path 'assets/img/scrap/hero-2.svg' -Title 'Thu mua sắt thép công trình' -Subtitle 'Nhận hàng số lượng lớn, có xe cẩu và nhân công' -Primary '#264653' -Secondary '#f4a261' -Type 'factory'
New-SceneSvg -Path 'assets/img/scrap/hero-3.svg' -Title 'Giá cập nhật theo thị trường' -Subtitle 'Báo giá minh bạch từng loại phế liệu' -Primary '#1d3557' -Secondary '#e9c46a' -Type 'scale'
New-SceneSvg -Path 'assets/img/scrap/hero-4.svg' -Title 'Thu mua đồng, nhôm, inox' -Subtitle 'Phân loại đúng chuẩn để tối ưu giá trị' -Primary '#6a040f' -Secondary '#f4a261' -Type 'magnet'
New-SceneSvg -Path 'assets/img/scrap/hero-5.svg' -Title 'Thu mua giấy, nhựa, điện tử' -Subtitle 'Dọn kho nhanh, bàn giao gọn sạch' -Primary '#0f766e' -Secondary '#84cc16' -Type 'recycle'
New-SceneSvg -Path 'assets/img/scrap/yard.svg' -Title 'Kho bãi và đội xe chuyên dụng' -Subtitle 'Đáp ứng thu gom tận nơi toàn khu vực phía Nam' -Primary '#1b4332' -Secondary '#ffb703' -Type 'factory'
New-SceneSvg -Path 'assets/img/scrap/material-ferrous.svg' -Title 'Sắt thép công trình' -Subtitle 'Dầm, tôn, xà gồ, khung sắt, thép phế' -Primary '#3a506b' -Secondary '#f4a261' -Type 'magnet'
New-SceneSvg -Path 'assets/img/scrap/material-nonferrous.svg' -Title 'Đồng, nhôm, inox' -Subtitle 'Thu mua tận nơi, phân loại chuẩn' -Primary '#7f1d1d' -Secondary '#fbbf24' -Type 'magnet'
New-SceneSvg -Path 'assets/img/scrap/material-paper.svg' -Title 'Giấy carton - nhựa' -Subtitle 'Bao bì, hồ sơ thanh lý, nhựa công nghiệp' -Primary '#22577a' -Secondary '#80ed99' -Type 'paper'
New-SceneSvg -Path 'assets/img/scrap/material-electronic.svg' -Title 'Thiết bị điện tử cũ' -Subtitle 'Bo mạch, dây cáp, máy móc thanh lý' -Primary '#1d3557' -Secondary '#2a9d8f' -Type 'electronic'
New-SceneSvg -Path 'assets/img/scrap/service-processing.svg' -Title 'Khảo sát và báo giá tận nơi' -Subtitle 'Đội ngũ đến tận kho, xưởng, công trình' -Primary '#2d6a4f' -Secondary '#e9c46a' -Type 'phone'
New-SceneSvg -Path 'assets/img/scrap/service-logistics.svg' -Title 'Xe tải - bốc xếp - cẩu kéo' -Subtitle 'Thu gom nhanh cho mọi quy mô hàng hóa' -Primary '#264653' -Secondary '#f4a261' -Type 'truck'
New-SceneSvg -Path 'assets/img/scrap/service-scale.svg' -Title 'Cân đo điện tử chuẩn xác' -Subtitle 'Niêm yết rõ khối lượng và đơn giá' -Primary '#1d3557' -Secondary '#ffb703' -Type 'scale'
New-SceneSvg -Path 'assets/img/scrap/service-cleanup.svg' -Title 'Dọn kho và thanh lý nhà xưởng' -Subtitle 'Tháo dỡ an toàn, bàn giao sạch sẽ' -Primary '#6a040f' -Secondary '#f4a261' -Type 'factory'
New-SceneSvg -Path 'assets/img/scrap/service-payment.svg' -Title 'Thanh toán ngay trong ngày' -Subtitle 'Tiền mặt hoặc chuyển khoản đầy đủ chứng từ' -Primary '#0f766e' -Secondary '#84cc16' -Type 'payment'
New-SceneSvg -Path 'assets/img/scrap/feature-pricing.svg' -Title 'Bảng giá rõ ràng' -Subtitle 'Cập nhật theo biến động thị trường phế liệu' -Primary '#2d6a4f' -Secondary '#f4a261' -Type 'scale'
New-SceneSvg -Path 'assets/img/scrap/feature-materials.svg' -Title 'Đa dạng chủng loại' -Subtitle 'Kim loại, giấy, nhựa, máy móc, điện tử' -Primary '#22577a' -Secondary '#80ed99' -Type 'recycle'
New-SceneSvg -Path 'assets/img/scrap/feature-support.svg' -Title 'Hỗ trợ doanh nghiệp' -Subtitle 'Thu mua định kỳ, hợp đồng, hồ sơ đầy đủ' -Primary '#1d3557' -Secondary '#e9c46a' -Type 'phone'
New-SceneSvg -Path 'assets/img/scrap/feature-contract.svg' -Title 'Hợp đồng và chứng từ' -Subtitle 'Phù hợp nhà máy, xưởng sản xuất, kho vận' -Primary '#6a040f' -Secondary '#f4a261' -Type 'contract'
New-SceneSvg -Path 'assets/img/scrap/blog-market.svg' -Title 'Tin tức thị trường phế liệu' -Subtitle 'Nhận định xu hướng giá mua bán và nhu cầu thu gom' -Primary '#1b4332' -Secondary '#f4a261' -Type 'factory'
New-SceneSvg -Path 'assets/img/scrap/blog-price.svg' -Title 'Bảng giá thu mua mới nhất' -Subtitle 'Sắt, đồng, nhôm, inox, giấy và nhựa' -Primary '#2d6a4f' -Secondary '#ffb703' -Type 'scale'
New-SceneSvg -Path 'assets/img/scrap/blog-storage.svg' -Title 'Lưu kho phế liệu an toàn' -Subtitle 'Giảm cháy nổ và thất thoát trong quá trình lưu trữ' -Primary '#22577a' -Secondary '#80ed99' -Type 'warehouse'
New-SceneSvg -Path 'assets/img/scrap/blog-sorting.svg' -Title 'Phân loại để bán giá cao' -Subtitle 'Tách riêng kim loại màu, kim loại đen, nhựa, giấy' -Primary '#6a040f' -Secondary '#f4a261' -Type 'recycle'
New-SceneSvg -Path 'assets/img/scrap/blog-policy.svg' -Title 'Chính sách thu mua minh bạch' -Subtitle 'Cân chuẩn, thanh toán nhanh, hợp đồng rõ ràng' -Primary '#1d3557' -Secondary '#e9c46a' -Type 'contract'
New-SceneSvg -Path 'assets/img/scrap/blog-machinery.svg' -Title 'Thanh lý máy móc cũ' -Subtitle 'Thu mua thiết bị công nghiệp, điện tử, dây chuyền' -Primary '#264653' -Secondary '#f4a261' -Type 'electronic'
New-SceneSvg -Path 'assets/img/scrap/contact.svg' -Title 'Phục vụ TP.HCM và lân cận' -Subtitle 'Khảo sát nhanh trong ngày tại kho xưởng và công trình' -Primary '#1b4332' -Secondary '#ffb703' -Type 'phone'

New-AvatarSvg -Path 'assets/img/scrap/team-1.svg' -Name 'Nguyễn Thanh Phát' -Role 'Điều phối thu mua' -Primary '#2d6a4f' -Secondary '#ffb703'
New-AvatarSvg -Path 'assets/img/scrap/team-2.svg' -Name 'Lê Minh Quân' -Role 'Khảo sát hiện trường' -Primary '#22577a' -Secondary '#80ed99'
New-AvatarSvg -Path 'assets/img/scrap/team-3.svg' -Name 'Trần Quốc Huy' -Role 'Giám định vật tư' -Primary '#1d3557' -Secondary '#e9c46a'
New-AvatarSvg -Path 'assets/img/scrap/team-4.svg' -Name 'Phạm Hoàng Nam' -Role 'Quản lý đội xe' -Primary '#6a040f' -Secondary '#f4a261'
New-AvatarSvg -Path 'assets/img/scrap/team-5.svg' -Name 'Đặng Gia Bảo' -Role 'Kế toán thanh toán' -Primary '#0f766e' -Secondary '#84cc16'
New-AvatarSvg -Path 'assets/img/scrap/team-6.svg' -Name 'Võ Khánh Linh' -Role 'Chăm sóc khách hàng' -Primary '#7f1d1d' -Secondary '#fbbf24'

$indexContent = (Get-Head -Title 'Phế Liệu Thành Phát | Thu mua phế liệu giá cao tận nơi' -Description 'Website thu mua phế liệu giá cao tận nơi tại TP.HCM, Bình Dương, Đồng Nai. Nhận thu mua sắt, đồng, nhôm, inox, giấy carton, nhựa, điện tử, máy móc cũ.' -Keywords 'thu mua phế liệu, thu mua sắt vụn, thu mua đồng, thu mua nhôm, phế liệu TP.HCM') +
(Get-Header -Active 'home') + @"

  <section id="hero" class="hero">
    <div class="info d-flex align-items-center">
      <div class="container">
        <div class="row justify-content-center">
          <div class="col-lg-7 text-center">
            <h2 data-aos="fade-down">Thu mua <span>phế liệu giá cao</span> tận nơi</h2>
            <p data-aos="fade-up">Chuyên thu mua sắt thép, đồng, nhôm, inox, giấy carton, nhựa công nghiệp, bo mạch, máy móc cũ và hàng thanh lý cho hộ gia đình, kho xưởng, nhà máy và công trình.</p>
            <a data-aos="fade-up" data-aos-delay="200" href="#get-started" class="btn-get-started">Nhận báo giá nhanh</a>
          </div>
        </div>
      </div>
    </div>

    <div id="hero-carousel" class="carousel slide" data-bs-ride="carousel" data-bs-interval="5000">
      <div class="carousel-item active" style="background-image: url(assets/img/scrap/hero-1.svg)"></div>
      <div class="carousel-item" style="background-image: url(assets/img/scrap/hero-2.svg)"></div>
      <div class="carousel-item" style="background-image: url(assets/img/scrap/hero-3.svg)"></div>
      <div class="carousel-item" style="background-image: url(assets/img/scrap/hero-4.svg)"></div>
      <div class="carousel-item" style="background-image: url(assets/img/scrap/hero-5.svg)"></div>
      <a class="carousel-control-prev" href="#hero-carousel" role="button" data-bs-slide="prev"><span class="carousel-control-prev-icon bi bi-chevron-left" aria-hidden="true"></span></a>
      <a class="carousel-control-next" href="#hero-carousel" role="button" data-bs-slide="next"><span class="carousel-control-next-icon bi bi-chevron-right" aria-hidden="true"></span></a>
    </div>
  </section>

  <main id="main">
    <section id="get-started" class="get-started section-bg">
      <div class="container">
        <div class="row justify-content-between gy-4">
          <div class="col-lg-6 d-flex align-items-center" data-aos="fade-up">
            <div class="content">
              <h3>Khảo sát nhanh, báo giá rõ, thu mua tận nơi trong ngày</h3>
              <p>Phế Liệu Thành Phát nhận thu mua số lượng nhỏ lẻ đến số lượng lớn cho kho xưởng, nhà máy, công trình tháo dỡ và cửa hàng kinh doanh. Chúng tôi có đội khảo sát, xe tải, nhân công bốc xếp và cân đo điện tử.</p>
              <p>Làm việc minh bạch theo 4 bước: tiếp nhận thông tin, khảo sát và báo giá, cân đo tại chỗ, thanh toán ngay bằng tiền mặt hoặc chuyển khoản.</p>
            </div>
          </div>

          <div class="col-lg-5" data-aos="fade">
            <form action="forms/quote.php" method="post" class="php-email-form">
              <h3>Yêu cầu thu mua</h3>
              <p>Gửi nhanh loại phế liệu, khối lượng ước tính và địa chỉ. Chúng tôi sẽ liên hệ tư vấn và báo giá sớm nhất.</p>
              <div class="row gy-3">
                <div class="col-md-12"><input type="text" name="name" class="form-control" placeholder="Họ và tên" required></div>
                <div class="col-md-12"><input type="email" class="form-control" name="email" placeholder="Email" required></div>
                <div class="col-md-12"><input type="text" class="form-control" name="phone" placeholder="Số điện thoại" required></div>
                <div class="col-md-12"><textarea class="form-control" name="message" rows="6" placeholder="Loại phế liệu, khối lượng, địa chỉ thu mua" required></textarea></div>
                <div class="col-md-12 text-center">
                  <div class="loading">Đang gửi</div>
                  <div class="error-message"></div>
                  <div class="sent-message">Yêu cầu của bạn đã được gửi thành công. Chúng tôi sẽ liên hệ sớm.</div>
                  <button type="submit">Nhận báo giá</button>
                </div>
              </div>
            </form>
          </div>
        </div>
      </div>
    </section>

    <section id="constructions" class="constructions">
      <div class="container" data-aos="fade-up">
        <div class="section-header">
          <h2>Mặt hàng thu mua</h2>
          <p>Thu mua đa dạng chủng loại phế liệu dân dụng và công nghiệp, hỗ trợ phân loại và định giá đúng theo từng vật tư.</p>
        </div>
        <div class="row gy-4">
          <div class="col-lg-6" data-aos="fade-up" data-aos-delay="100">
            <div class="card-item">
              <div class="row">
                <div class="col-xl-5"><div class="card-bg" style="background-image: url(assets/img/scrap/material-ferrous.svg);"></div></div>
                <div class="col-xl-7 d-flex align-items-center">
                  <div class="card-body">
                    <h4 class="card-title">Sắt thép, tôn, xà gồ, khung nhà xưởng</h4>
                    <p>Nhận thu mua sắt thép công trình, phế liệu cơ khí, dầm kèo, máy móc khung sắt và các loại vật tư tháo dỡ.</p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="col-lg-6" data-aos="fade-up" data-aos-delay="200">
            <div class="card-item">
              <div class="row">
                <div class="col-xl-5"><div class="card-bg" style="background-image: url(assets/img/scrap/material-nonferrous.svg);"></div></div>
                <div class="col-xl-7 d-flex align-items-center">
                  <div class="card-body">
                    <h4 class="card-title">Đồng, nhôm, inox, chì, hợp kim</h4>
                    <p>Phân loại theo độ tinh khiết để tối ưu giá trị, đặc biệt với đồng cáp, nhôm thanh định hình, inox 304 và kim loại màu.</p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="col-lg-6" data-aos="fade-up" data-aos-delay="300">
            <div class="card-item">
              <div class="row">
                <div class="col-xl-5"><div class="card-bg" style="background-image: url(assets/img/scrap/material-paper.svg);"></div></div>
                <div class="col-xl-7 d-flex align-items-center">
                  <div class="card-body">
                    <h4 class="card-title">Giấy carton, nhựa công nghiệp, bao bì</h4>
                    <p>Thu mua giấy, hồ sơ thanh lý, pallet giấy, màng co, nhựa PP, PE, PET và phế liệu đóng gói từ kho xưởng.</p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="col-lg-6" data-aos="fade-up" data-aos-delay="400">
            <div class="card-item">
              <div class="row">
                <div class="col-xl-5"><div class="card-bg" style="background-image: url(assets/img/scrap/material-electronic.svg);"></div></div>
                <div class="col-xl-7 d-flex align-items-center">
                  <div class="card-body">
                    <h4 class="card-title">Thiết bị điện tử, máy móc, hàng thanh lý</h4>
                    <p>Nhận máy lạnh cũ, mô tơ, bo mạch, dây cáp, tủ điện, máy sản xuất cũ và dây chuyền không còn sử dụng.</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <section id="services" class="services section-bg">
      <div class="container" data-aos="fade-up">
        <div class="section-header">
          <h2>Dịch vụ thu mua</h2>
          <p>Giải pháp thu gom phế liệu trọn gói từ khảo sát, bốc xếp, vận chuyển đến thanh toán và hoàn thiện chứng từ.</p>
        </div>
        <div class="row gy-4">
          <div class="col-lg-4 col-md-6" data-aos="fade-up" data-aos-delay="100">
            <div class="service-item position-relative"><div class="icon"><i class="fa-solid fa-industry"></i></div><h3>Thu mua kim loại phế liệu</h3><p>Thu mua sắt, đồng, nhôm, inox, chì và hợp kim cho công trình, nhà xưởng, xí nghiệp.</p><a href="service-details.html" class="readmore stretched-link">Xem chi tiết <i class="bi bi-arrow-right"></i></a></div>
          </div>
          <div class="col-lg-4 col-md-6" data-aos="fade-up" data-aos-delay="200">
            <div class="service-item position-relative"><div class="icon"><i class="fa-solid fa-boxes-stacked"></i></div><h3>Thu mua giấy & carton</h3><p>Nhận giấy vụn, hồ sơ thanh lý, bao bì carton, ống giấy và hàng tồn kho số lượng lớn.</p><a href="service-details.html" class="readmore stretched-link">Xem chi tiết <i class="bi bi-arrow-right"></i></a></div>
          </div>
          <div class="col-lg-4 col-md-6" data-aos="fade-up" data-aos-delay="300">
            <div class="service-item position-relative"><div class="icon"><i class="fa-solid fa-recycle"></i></div><h3>Thu mua nhựa công nghiệp</h3><p>Thu gom nhựa PP, PE, PET, ABS, pallet nhựa, phế phẩm sản xuất và nhựa tái chế.</p><a href="service-details.html" class="readmore stretched-link">Xem chi tiết <i class="bi bi-arrow-right"></i></a></div>
          </div>
          <div class="col-lg-4 col-md-6" data-aos="fade-up" data-aos-delay="400">
            <div class="service-item position-relative"><div class="icon"><i class="fa-solid fa-truck-ramp-box"></i></div><h3>Dọn kho và bốc xếp tận nơi</h3><p>Bố trí xe tải, nhân công, xe nâng để xử lý nhanh kho hàng tồn, vật tư cũ và phế liệu lẫn tạp.</p><a href="service-details.html" class="readmore stretched-link">Xem chi tiết <i class="bi bi-arrow-right"></i></a></div>
          </div>
          <div class="col-lg-4 col-md-6" data-aos="fade-up" data-aos-delay="500">
            <div class="service-item position-relative"><div class="icon"><i class="fa-solid fa-screwdriver-wrench"></i></div><h3>Thanh lý máy móc nhà xưởng</h3><p>Thu mua máy sản xuất cũ, dây chuyền dừng hoạt động, khung nhà xưởng và thiết bị công nghiệp.</p><a href="service-details.html" class="readmore stretched-link">Xem chi tiết <i class="bi bi-arrow-right"></i></a></div>
          </div>
          <div class="col-lg-4 col-md-6" data-aos="fade-up" data-aos-delay="600">
            <div class="service-item position-relative"><div class="icon"><i class="fa-solid fa-money-bill-transfer"></i></div><h3>Thanh toán nhanh trong ngày</h3><p>Hỗ trợ chuyển khoản, tiền mặt, xuất xác nhận khối lượng và chứng từ thu mua theo nhu cầu.</p><a href="service-details.html" class="readmore stretched-link">Xem chi tiết <i class="bi bi-arrow-right"></i></a></div>
          </div>
        </div>
      </div>
    </section>

    <section id="alt-services" class="alt-services">
      <div class="container" data-aos="fade-up">
        <div class="row justify-content-around gy-4">
          <div class="col-lg-6 img-bg" style="background-image: url(assets/img/scrap/service-processing.svg);" data-aos="zoom-in" data-aos-delay="100"></div>
          <div class="col-lg-5 d-flex flex-column justify-content-center">
            <h3>Quy trình thu mua phế liệu chuyên nghiệp và minh bạch</h3>
            <p>Chúng tôi triển khai nhanh từ khâu tiếp nhận thông tin đến khi hoàn tất cân đo và thanh toán, giúp khách hàng tiết kiệm thời gian xử lý phế liệu.</p>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="100"><i class="bi bi-telephone-inbound flex-shrink-0"></i><div><h4><a href="contact.html" class="stretched-link">Tiếp nhận yêu cầu</a></h4><p>Nhận thông tin qua điện thoại, Zalo hoặc biểu mẫu website 24/7.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="200"><i class="bi bi-search flex-shrink-0"></i><div><h4><a href="contact.html" class="stretched-link">Khảo sát thực tế</a></h4><p>Đến tận nơi xem hàng, phân loại vật tư và báo giá rõ ràng theo từng loại phế liệu.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="300"><i class="bi bi-truck flex-shrink-0"></i><div><h4><a href="services.html" class="stretched-link">Bốc xếp và vận chuyển</a></h4><p>Chuẩn bị xe tải, xe nâng, nhân công, thiết bị cắt dỡ phù hợp với mặt bằng hiện trường.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="400"><i class="bi bi-cash-coin flex-shrink-0"></i><div><h4><a href="contact.html" class="stretched-link">Cân đo và thanh toán</a></h4><p>Cân điện tử, chốt khối lượng công khai và thanh toán ngay khi hoàn tất.</p></div></div>
          </div>
        </div>
      </div>
    </section>

    <section id="features" class="features section-bg">
      <div class="container" data-aos="fade-up">
        <ul class="nav nav-tabs row g-2 d-flex">
          <li class="nav-item col-3"><a class="nav-link active show" data-bs-toggle="tab" data-bs-target="#tab-1"><h4>Giá minh bạch</h4></a></li>
          <li class="nav-item col-3"><a class="nav-link" data-bs-toggle="tab" data-bs-target="#tab-2"><h4>Đa dạng hàng</h4></a></li>
          <li class="nav-item col-3"><a class="nav-link" data-bs-toggle="tab" data-bs-target="#tab-3"><h4>Hỗ trợ doanh nghiệp</h4></a></li>
          <li class="nav-item col-3"><a class="nav-link" data-bs-toggle="tab" data-bs-target="#tab-4"><h4>Chứng từ đầy đủ</h4></a></li>
        </ul>
        <div class="tab-content">
          <div class="tab-pane active show" id="tab-1"><div class="row"><div class="col-lg-6 order-2 order-lg-1 mt-3 mt-lg-0 d-flex flex-column justify-content-center" data-aos="fade-up" data-aos-delay="100"><h3>Báo giá theo thị trường và chủng loại thực tế</h3><p class="fst-italic">Đơn giá được cập nhật liên tục theo giá phế liệu đầu ngày, loại hàng, độ sạch và khối lượng.</p><ul><li><i class="bi bi-check2-all"></i> Tư vấn sơ bộ qua ảnh hoặc video trước khi khảo sát.</li><li><i class="bi bi-check2-all"></i> Báo giá tách riêng cho từng nhóm vật tư.</li><li><i class="bi bi-check2-all"></i> Không phát sinh phí vận chuyển khi đã chốt đơn thu mua.</li></ul></div><div class="col-lg-6 order-1 order-lg-2 text-center" data-aos="fade-up" data-aos-delay="200"><img src="assets/img/scrap/feature-pricing.svg" alt="Bảng giá phế liệu" class="img-fluid"></div></div></div>
          <div class="tab-pane" id="tab-2"><div class="row"><div class="col-lg-6 order-2 order-lg-1 mt-3 mt-lg-0 d-flex flex-column justify-content-center"><h3>Nhận nhiều loại phế liệu tại cùng một điểm thu gom</h3><p class="fst-italic">Phù hợp cho kho xưởng và công trình có nhiều loại vật tư thanh lý trong cùng một đợt.</p><ul><li><i class="bi bi-check2-all"></i> Kim loại đen, kim loại màu, điện tử, giấy và nhựa.</li><li><i class="bi bi-check2-all"></i> Có hỗ trợ phân loại ngay tại kho.</li><li><i class="bi bi-check2-all"></i> Tối ưu giá thu mua nhờ xử lý đúng chủng loại.</li></ul></div><div class="col-lg-6 order-1 order-lg-2 text-center"><img src="assets/img/scrap/feature-materials.svg" alt="Chủng loại phế liệu" class="img-fluid"></div></div></div>
          <div class="tab-pane" id="tab-3"><div class="row"><div class="col-lg-6 order-2 order-lg-1 mt-3 mt-lg-0 d-flex flex-column justify-content-center"><h3>Phục vụ định kỳ cho nhà máy, xưởng sản xuất và chuỗi kho vận</h3><ul><li><i class="bi bi-check2-all"></i> Thiết lập lịch thu gom cố định theo tuần hoặc tháng.</li><li><i class="bi bi-check2-all"></i> Tổ chức nhân sự, xe tải và quy trình vào hàng phù hợp.</li><li><i class="bi bi-check2-all"></i> Có biên bản, hình ảnh và đầu mối CSKH theo từng khách hàng.</li></ul><p class="fst-italic">Đây là giải pháp phù hợp khi doanh nghiệp cần đối tác thu mua phế liệu lâu dài, ổn định.</p></div><div class="col-lg-6 order-1 order-lg-2 text-center"><img src="assets/img/scrap/feature-support.svg" alt="Hỗ trợ doanh nghiệp" class="img-fluid"></div></div></div>
          <div class="tab-pane" id="tab-4"><div class="row"><div class="col-lg-6 order-2 order-lg-1 mt-3 mt-lg-0 d-flex flex-column justify-content-center"><h3>Hợp đồng rõ ràng, chứng từ nhanh gọn</h3><p class="fst-italic">Hỗ trợ các nhu cầu xác nhận khối lượng, biên bản bàn giao, hợp đồng nguyên tắc hoặc thu mua từng đợt.</p><ul><li><i class="bi bi-check2-all"></i> Phối hợp với kế toán và kho vận của doanh nghiệp.</li><li><i class="bi bi-check2-all"></i> Xác nhận số lượng và giá thu mua sau mỗi đợt.</li><li><i class="bi bi-check2-all"></i> Dễ dàng đối soát cho khách hàng tổ chức.</li></ul></div><div class="col-lg-6 order-1 order-lg-2 text-center"><img src="assets/img/scrap/feature-contract.svg" alt="Chứng từ thu mua" class="img-fluid"></div></div></div>
        </div>
      </div>
    </section>

    <section id="projects" class="projects">
      <div class="container" data-aos="fade-up">
        <div class="section-header">
          <h2>Danh mục thu mua thực tế</h2>
          <p>Một số nhóm hàng được thu mua thường xuyên tại kho xưởng, nhà máy, công trình tháo dỡ và cửa hàng thanh lý.</p>
        </div>
        <div class="portfolio-isotope" data-portfolio-filter="*" data-portfolio-layout="masonry" data-portfolio-sort="original-order">
          <ul class="portfolio-flters" data-aos="fade-up" data-aos-delay="100">
            <li data-filter="*" class="filter-active">Tất cả</li>
            <li data-filter=".filter-metal">Kim loại</li>
            <li data-filter=".filter-paper">Giấy & nhựa</li>
            <li data-filter=".filter-machine">Máy móc</li>
            <li data-filter=".filter-electronic">Điện tử</li>
          </ul>
          <div class="row gy-4 portfolio-container" data-aos="fade-up" data-aos-delay="200">
            <div class="col-lg-4 col-md-6 portfolio-item filter-metal"><div class="portfolio-content h-100"><img src="assets/img/scrap/material-ferrous.svg" class="img-fluid" alt="Sắt thép"><div class="portfolio-info"><h4>Sắt thép công trình</h4><p>Xà gồ, tôn, dầm, ống sắt, khung thép</p><a href="assets/img/scrap/material-ferrous.svg" title="Sắt thép công trình" data-gallery="portfolio-gallery-metal" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-metal"><div class="portfolio-content h-100"><img src="assets/img/scrap/material-nonferrous.svg" class="img-fluid" alt="Đồng nhôm inox"><div class="portfolio-info"><h4>Đồng, nhôm, inox</h4><p>Kim loại màu giá trị cao, phân loại riêng</p><a href="assets/img/scrap/material-nonferrous.svg" title="Đồng nhôm inox" data-gallery="portfolio-gallery-metal" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-paper"><div class="portfolio-content h-100"><img src="assets/img/scrap/material-paper.svg" class="img-fluid" alt="Giấy carton"><div class="portfolio-info"><h4>Giấy carton</h4><p>Giấy vụn, hồ sơ, bao bì và pallet giấy</p><a href="assets/img/scrap/material-paper.svg" title="Giấy carton" data-gallery="portfolio-gallery-paper" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-paper"><div class="portfolio-content h-100"><img src="assets/img/scrap/feature-materials.svg" class="img-fluid" alt="Nhựa công nghiệp"><div class="portfolio-info"><h4>Nhựa công nghiệp</h4><p>PP, PE, PET, ABS và phế phẩm sản xuất</p><a href="assets/img/scrap/feature-materials.svg" title="Nhựa công nghiệp" data-gallery="portfolio-gallery-paper" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-machine"><div class="portfolio-content h-100"><img src="assets/img/scrap/service-cleanup.svg" class="img-fluid" alt="Nhà xưởng thanh lý"><div class="portfolio-info"><h4>Thanh lý nhà xưởng</h4><p>Khung nhà xưởng, kệ, thiết bị cũ</p><a href="assets/img/scrap/service-cleanup.svg" title="Thanh lý nhà xưởng" data-gallery="portfolio-gallery-machine" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-machine"><div class="portfolio-content h-100"><img src="assets/img/scrap/hero-2.svg" class="img-fluid" alt="Máy móc"><div class="portfolio-info"><h4>Máy móc thanh lý</h4><p>Máy sản xuất cũ, mô tơ, băng chuyền</p><a href="assets/img/scrap/hero-2.svg" title="Máy móc thanh lý" data-gallery="portfolio-gallery-machine" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-electronic"><div class="portfolio-content h-100"><img src="assets/img/scrap/material-electronic.svg" class="img-fluid" alt="Điện tử"><div class="portfolio-info"><h4>Bo mạch điện tử</h4><p>Bo mạch, dây cáp, linh kiện điện tử cũ</p><a href="assets/img/scrap/material-electronic.svg" title="Bo mạch điện tử" data-gallery="portfolio-gallery-electronic" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-electronic"><div class="portfolio-content h-100"><img src="assets/img/scrap/blog-machinery.svg" class="img-fluid" alt="Điện lạnh"><div class="portfolio-info"><h4>Thiết bị điện lạnh cũ</h4><p>Máy lạnh, block, dây đồng, tủ điện</p><a href="assets/img/scrap/blog-machinery.svg" title="Thiết bị điện lạnh cũ" data-gallery="portfolio-gallery-electronic" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-metal"><div class="portfolio-content h-100"><img src="assets/img/scrap/service-scale.svg" class="img-fluid" alt="Cân đo chuẩn"><div class="portfolio-info"><h4>Cân đo tại chỗ</h4><p>Kiểm soát khối lượng công khai, minh bạch</p><a href="assets/img/scrap/service-scale.svg" title="Cân đo tại chỗ" data-gallery="portfolio-gallery-metal" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-paper"><div class="portfolio-content h-100"><img src="assets/img/scrap/blog-storage.svg" class="img-fluid" alt="Kho giấy nhựa"><div class="portfolio-info"><h4>Dọn kho giấy nhựa</h4><p>Thu gom hàng tồn và bàn giao kho sạch sẽ</p><a href="assets/img/scrap/blog-storage.svg" title="Dọn kho giấy nhựa" data-gallery="portfolio-gallery-paper" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-machine"><div class="portfolio-content h-100"><img src="assets/img/scrap/service-logistics.svg" class="img-fluid" alt="Xe tải thu gom"><div class="portfolio-info"><h4>Xe tải thu gom tận nơi</h4><p>Linh hoạt cho đơn hàng nhỏ lẻ đến số lượng lớn</p><a href="assets/img/scrap/service-logistics.svg" title="Xe tải thu gom tận nơi" data-gallery="portfolio-gallery-machine" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-electronic"><div class="portfolio-content h-100"><img src="assets/img/scrap/service-payment.svg" class="img-fluid" alt="Thanh toán nhanh"><div class="portfolio-info"><h4>Thanh toán nhanh</h4><p>Hoàn tất giao dịch ngay sau khi thu gom</p><a href="assets/img/scrap/service-payment.svg" title="Thanh toán nhanh" data-gallery="portfolio-gallery-electronic" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
          </div>
        </div>
      </div>
    </section>

$(Get-TestimonialsSection)
$(Get-RecentPostsSection)
  </main>
"@ +
(Get-Footer)

Write-Utf8File -Path 'index.html' -Content $indexContent

$aboutContent = (Get-Head -Title 'Giới thiệu | Phế Liệu Thành Phát' -Description 'Giới thiệu đơn vị thu mua phế liệu Thành Phát với đội ngũ khảo sát, xe vận chuyển, cân điện tử và quy trình làm việc minh bạch.' -Keywords 'giới thiệu phế liệu thành phát, công ty thu mua phế liệu') +
(Get-Header -Active 'about') + @"
  <main id="main">
$(Get-Breadcrumb 'Giới thiệu')

    <section id="about" class="about">
      <div class="container" data-aos="fade-up">
        <div class="row position-relative">
          <div class="col-lg-7 about-img" style="background-image: url(assets/img/scrap/yard.svg);"></div>
          <div class="col-lg-7">
            <h2>Đơn vị thu mua phế liệu tận nơi, làm việc minh bạch</h2>
            <div class="our-story">
              <h4>Phục vụ từ 2015</h4>
              <h3>Câu chuyện của chúng tôi</h3>
              <p>Phế Liệu Thành Phát được xây dựng từ nhu cầu rất thực tế của khách hàng: cần một đơn vị thu mua phế liệu có mặt nhanh, cân đúng, thanh toán rõ ràng và xử lý được nhiều chủng loại hàng hóa trong cùng một lần thu gom.</p>
              <ul>
                <li><i class="bi bi-check-circle"></i> <span>Khảo sát tận nơi tại TP.HCM và các tỉnh lân cận</span></li>
                <li><i class="bi bi-check-circle"></i> <span>Báo giá theo thị trường, tách riêng từng loại phế liệu</span></li>
                <li><i class="bi bi-check-circle"></i> <span>Có xe tải, nhân công, thiết bị hỗ trợ bốc xếp và tháo dỡ</span></li>
              </ul>
              <p>Chúng tôi phục vụ từ khách hàng cá nhân đến kho xưởng, doanh nghiệp sản xuất, trung tâm logistics và công trình tháo dỡ cần xử lý vật tư cũ nhanh gọn, an toàn và có hồ sơ bàn giao rõ ràng.</p>
              <div class="watch-video d-flex align-items-center position-relative">
                <i class="bi bi-arrow-right-circle"></i>
                <a href="services.html" class="stretched-link">Xem quy trình thu mua</a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <section id="stats-counter" class="stats-counter section-bg">
      <div class="container">
        <div class="row gy-4">
          <div class="col-lg-3 col-md-6"><div class="stats-item d-flex align-items-center w-100 h-100"><i class="bi bi-truck color-blue flex-shrink-0"></i><div><span data-purecounter-start="0" data-purecounter-end="2800" data-purecounter-duration="1" class="purecounter"></span><p>Chuyến thu mua</p></div></div></div>
          <div class="col-lg-3 col-md-6"><div class="stats-item d-flex align-items-center w-100 h-100"><i class="bi bi-box-seam color-orange flex-shrink-0"></i><div><span data-purecounter-start="0" data-purecounter-end="950" data-purecounter-duration="1" class="purecounter"></span><p>Tấn phế liệu mỗi tháng</p></div></div></div>
          <div class="col-lg-3 col-md-6"><div class="stats-item d-flex align-items-center w-100 h-100"><i class="bi bi-buildings color-green flex-shrink-0"></i><div><span data-purecounter-start="0" data-purecounter-end="460" data-purecounter-duration="1" class="purecounter"></span><p>Khách hàng doanh nghiệp</p></div></div></div>
          <div class="col-lg-3 col-md-6"><div class="stats-item d-flex align-items-center w-100 h-100"><i class="bi bi-geo-alt color-pink flex-shrink-0"></i><div><span data-purecounter-start="0" data-purecounter-end="12" data-purecounter-duration="1" class="purecounter"></span><p>Tỉnh thành phục vụ</p></div></div></div>
        </div>
      </div>
    </section>

    <section id="alt-services" class="alt-services">
      <div class="container" data-aos="fade-up">
        <div class="row justify-content-around gy-4">
          <div class="col-lg-6 img-bg" style="background-image: url(assets/img/scrap/service-logistics.svg);" data-aos="zoom-in" data-aos-delay="100"></div>
          <div class="col-lg-5 d-flex flex-column justify-content-center">
            <h3>Vì sao khách hàng chọn Phế Liệu Thành Phát?</h3>
            <p>Chúng tôi tập trung vào ba yếu tố cốt lõi: tốc độ phản hồi, giá thu mua hợp lý và quy trình giao dịch minh bạch.</p>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="100"><i class="bi bi-clock-history flex-shrink-0"></i><div><h4><a href="contact.html" class="stretched-link">Phản hồi nhanh</a></h4><p>Khảo sát và chốt lịch thu gom nhanh, phù hợp với lịch hoạt động của kho xưởng.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="200"><i class="bi bi-calculator flex-shrink-0"></i><div><h4><a href="projects.html" class="stretched-link">Định giá chuẩn</a></h4><p>Phân loại hàng hóa kỹ để bảo đảm mỗi loại phế liệu được áp đúng đơn giá.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="300"><i class="bi bi-truck flex-shrink-0"></i><div><h4><a href="services.html" class="stretched-link">Thiết bị đầy đủ</a></h4><p>Xe tải, xe nâng, nhân công và dụng cụ tháo dỡ cho đơn hàng từ nhỏ đến lớn.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="400"><i class="bi bi-wallet2 flex-shrink-0"></i><div><h4><a href="contact.html" class="stretched-link">Thanh toán linh hoạt</a></h4><p>Hỗ trợ tiền mặt, chuyển khoản và xác nhận giao dịch ngay sau khi cân xong.</p></div></div>
          </div>
        </div>
      </div>
    </section>

    <section id="alt-services-2" class="alt-services section-bg">
      <div class="container" data-aos="fade-up">
        <div class="row justify-content-around gy-4">
          <div class="col-lg-5 d-flex flex-column justify-content-center">
            <h3>Năng lực vận hành phù hợp cho doanh nghiệp và công trình</h3>
            <p>Không chỉ thu mua phế liệu thông thường, chúng tôi còn xử lý các đợt thanh lý hàng tồn, di dời nhà xưởng, tháo dỡ vật tư cũ và thu gom định kỳ.</p>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="100"><i class="bi bi-building flex-shrink-0"></i><div><h4><a href="services.html" class="stretched-link">Nhà máy & xưởng sản xuất</a></h4><p>Làm việc theo lịch vào hàng, quy định nội bộ và tiêu chuẩn an toàn của từng nhà máy.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="200"><i class="bi bi-boxes flex-shrink-0"></i><div><h4><a href="projects.html" class="stretched-link">Kho vận & trung tâm phân phối</a></h4><p>Dọn kho, thu gom giấy carton, nhựa đóng gói, pallet, thiết bị kho cũ.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="300"><i class="bi bi-tools flex-shrink-0"></i><div><h4><a href="project-details.html" class="stretched-link">Công trình tháo dỡ</a></h4><p>Thu mua sắt thép, tôn, cáp điện, hệ thống cơ điện và máy móc đã ngừng sử dụng.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="400"><i class="bi bi-file-text flex-shrink-0"></i><div><h4><a href="sample-inner-page.html" class="stretched-link">Biên bản & chứng từ</a></h4><p>Hỗ trợ xác nhận khối lượng, hợp đồng nguyên tắc và hồ sơ đối soát theo nhu cầu.</p></div></div>
          </div>
          <div class="col-lg-6 img-bg" style="background-image: url(assets/img/scrap/feature-support.svg);" data-aos="zoom-in" data-aos-delay="100"></div>
        </div>
      </div>
    </section>

    <section id="team" class="team">
      <div class="container" data-aos="fade-up">
        <div class="section-header">
          <h2>Đội ngũ vận hành</h2>
          <p>Nhân sự khảo sát, giám định, vận chuyển và chăm sóc khách hàng phối hợp chặt chẽ để mỗi đợt thu mua diễn ra nhanh, an toàn và minh bạch.</p>
        </div>
        <div class="row gy-5">
          <div class="col-lg-4 col-md-6 member" data-aos="fade-up" data-aos-delay="100"><div class="member-img"><img src="assets/img/scrap/team-1.svg" class="img-fluid" alt="Điều phối thu mua"></div><div class="member-info text-center"><h4>Nguyễn Thanh Phát</h4><span>Điều phối thu mua</span><p>Phụ trách tiếp nhận yêu cầu và lên lịch khảo sát nhanh tại hiện trường.</p></div></div>
          <div class="col-lg-4 col-md-6 member" data-aos="fade-up" data-aos-delay="200"><div class="member-img"><img src="assets/img/scrap/team-2.svg" class="img-fluid" alt="Khảo sát hiện trường"></div><div class="member-info text-center"><h4>Lê Minh Quân</h4><span>Khảo sát hiện trường</span><p>Đánh giá khối lượng hàng, hiện trạng bốc xếp và đưa ra phương án thu gom phù hợp.</p></div></div>
          <div class="col-lg-4 col-md-6 member" data-aos="fade-up" data-aos-delay="300"><div class="member-img"><img src="assets/img/scrap/team-3.svg" class="img-fluid" alt="Giám định vật tư"></div><div class="member-info text-center"><h4>Trần Quốc Huy</h4><span>Giám định vật tư</span><p>Phân loại kim loại màu, kim loại đen, nhựa và điện tử để chốt giá chính xác.</p></div></div>
          <div class="col-lg-4 col-md-6 member" data-aos="fade-up" data-aos-delay="400"><div class="member-img"><img src="assets/img/scrap/team-4.svg" class="img-fluid" alt="Quản lý đội xe"></div><div class="member-info text-center"><h4>Phạm Hoàng Nam</h4><span>Quản lý đội xe</span><p>Điều phối xe tải, xe cẩu, nhân công và bảo đảm tiến độ bốc xếp theo kế hoạch.</p></div></div>
          <div class="col-lg-4 col-md-6 member" data-aos="fade-up" data-aos-delay="500"><div class="member-img"><img src="assets/img/scrap/team-5.svg" class="img-fluid" alt="Kế toán thanh toán"></div><div class="member-info text-center"><h4>Đặng Gia Bảo</h4><span>Kế toán thanh toán</span><p>Xử lý chứng từ, đối soát khối lượng và thanh toán nhanh cho từng đơn hàng.</p></div></div>
          <div class="col-lg-4 col-md-6 member" data-aos="fade-up" data-aos-delay="600"><div class="member-img"><img src="assets/img/scrap/team-6.svg" class="img-fluid" alt="Chăm sóc khách hàng"></div><div class="member-info text-center"><h4>Võ Khánh Linh</h4><span>Chăm sóc khách hàng</span><p>Hỗ trợ khách hàng doanh nghiệp theo lịch thu gom định kỳ và các yêu cầu sau bán hàng.</p></div></div>
        </div>
      </div>
    </section>

$(Get-TestimonialsSection)
  </main>
"@ +
(Get-Footer)

Write-Utf8File -Path 'about.html' -Content $aboutContent

$servicesContent = (Get-Head -Title 'Dịch vụ | Phế Liệu Thành Phát' -Description 'Các dịch vụ thu mua phế liệu tận nơi, dọn kho, thanh lý nhà xưởng, thu gom định kỳ và hỗ trợ chứng từ cho doanh nghiệp.' -Keywords 'dịch vụ thu mua phế liệu, thanh lý nhà xưởng, dọn kho phế liệu') +
(Get-Header -Active 'services') + @"
  <main id="main">
$(Get-Breadcrumb 'Dịch vụ')

    <section id="services" class="services section-bg">
      <div class="container" data-aos="fade-up">
        <div class="row gy-4">
          <div class="col-lg-4 col-md-6" data-aos="fade-up" data-aos-delay="100"><div class="service-item position-relative"><div class="icon"><i class="fa-solid fa-industry"></i></div><h3>Thu mua kim loại</h3><p>Thu mua sắt, đồng, nhôm, inox, chì, hợp kim từ công trình, xưởng cơ khí và nhà máy.</p><a href="service-details.html" class="readmore stretched-link">Xem chi tiết <i class="bi bi-arrow-right"></i></a></div></div>
          <div class="col-lg-4 col-md-6" data-aos="fade-up" data-aos-delay="200"><div class="service-item position-relative"><div class="icon"><i class="fa-solid fa-box-open"></i></div><h3>Thu mua giấy, carton</h3><p>Nhận giấy vụn, hồ sơ, bao bì carton, thùng giấy cũ và hàng tồn kho cần giải phóng diện tích.</p><a href="service-details.html" class="readmore stretched-link">Xem chi tiết <i class="bi bi-arrow-right"></i></a></div></div>
          <div class="col-lg-4 col-md-6" data-aos="fade-up" data-aos-delay="300"><div class="service-item position-relative"><div class="icon"><i class="fa-solid fa-recycle"></i></div><h3>Thu mua nhựa công nghiệp</h3><p>Thu gom nhựa phế phẩm, pallet nhựa, thùng chứa cũ, nhựa PP/PE/PET/ABS.</p><a href="service-details.html" class="readmore stretched-link">Xem chi tiết <i class="bi bi-arrow-right"></i></a></div></div>
          <div class="col-lg-4 col-md-6" data-aos="fade-up" data-aos-delay="400"><div class="service-item position-relative"><div class="icon"><i class="fa-solid fa-truck"></i></div><h3>Bốc xếp và vận chuyển</h3><p>Có xe tải, nhân công, xe nâng, xe cẩu để thu gom nhanh và an toàn theo hiện trường thực tế.</p><a href="service-details.html" class="readmore stretched-link">Xem chi tiết <i class="bi bi-arrow-right"></i></a></div></div>
          <div class="col-lg-4 col-md-6" data-aos="fade-up" data-aos-delay="500"><div class="service-item position-relative"><div class="icon"><i class="fa-solid fa-warehouse"></i></div><h3>Dọn kho, thanh lý tồn kho</h3><p>Thu gom trọn gói các lô hàng không còn sử dụng, vật tư cũ, kệ hàng và bao bì tồn đọng.</p><a href="service-details.html" class="readmore stretched-link">Xem chi tiết <i class="bi bi-arrow-right"></i></a></div></div>
          <div class="col-lg-4 col-md-6" data-aos="fade-up" data-aos-delay="600"><div class="service-item position-relative"><div class="icon"><i class="fa-solid fa-file-invoice-dollar"></i></div><h3>Hỗ trợ doanh nghiệp</h3><p>Phối hợp theo hợp đồng nguyên tắc, lịch thu gom định kỳ và đối soát khối lượng minh bạch.</p><a href="service-details.html" class="readmore stretched-link">Xem chi tiết <i class="bi bi-arrow-right"></i></a></div></div>
        </div>
      </div>
    </section>

    <section id="services-cards" class="services-cards">
      <div class="container" data-aos="fade-up">
        <div class="row gy-4">
          <div class="col-lg-3 col-md-6" data-aos="zoom-in" data-aos-delay="100"><h3>Khảo sát nhanh</h3><p>Tiếp nhận thông tin và sắp lịch đến tận nơi để xem hàng, phân loại và ước lượng khối lượng.</p><ul class="list-unstyled"><li><i class="bi bi-check2"></i> <span>Phản hồi trong ngày</span></li><li><i class="bi bi-check2"></i> <span>Hỗ trợ qua điện thoại, Zalo</span></li><li><i class="bi bi-check2"></i> <span>Tư vấn trước qua hình ảnh</span></li></ul></div>
          <div class="col-lg-3 col-md-6" data-aos="zoom-in" data-aos-delay="200"><h3>Báo giá minh bạch</h3><p>Mỗi loại vật tư được tách giá rõ ràng theo thị trường, giúp khách hàng dễ dàng so sánh và đối chiếu.</p><ul class="list-unstyled"><li><i class="bi bi-check2"></i> <span>Giá sắt, đồng, nhôm, inox riêng</span></li><li><i class="bi bi-check2"></i> <span>Giá cập nhật liên tục</span></li><li><i class="bi bi-check2"></i> <span>Không mập mờ chủng loại</span></li></ul></div>
          <div class="col-lg-3 col-md-6" data-aos="zoom-in" data-aos-delay="300"><h3>Thiết bị đầy đủ</h3><p>Chúng tôi có đội xe và nhân lực phù hợp cho thu gom tại kho hẹp, nhà xưởng hoặc công trình tháo dỡ.</p><ul class="list-unstyled"><li><i class="bi bi-check2"></i> <span>Xe tải và xe nâng</span></li><li><i class="bi bi-check2"></i> <span>Nhân công bốc xếp</span></li><li><i class="bi bi-check2"></i> <span>Dụng cụ cắt dỡ cơ bản</span></li></ul></div>
          <div class="col-lg-3 col-md-6" data-aos="zoom-in" data-aos-delay="400"><h3>Thanh toán ngay</h3><p>Hoàn tất giao dịch ngay khi cân xong, hỗ trợ xác nhận khối lượng và chứng từ theo nhu cầu khách hàng.</p><ul class="list-unstyled"><li><i class="bi bi-check2"></i> <span>Tiền mặt hoặc chuyển khoản</span></li><li><i class="bi bi-check2"></i> <span>Đối soát nhanh</span></li><li><i class="bi bi-check2"></i> <span>Phục vụ khách hàng tổ chức</span></li></ul></div>
        </div>
      </div>
    </section>

    <section id="alt-services-2" class="alt-services section-bg">
      <div class="container" data-aos="fade-up">
        <div class="row justify-content-around gy-4">
          <div class="col-lg-5 d-flex flex-column justify-content-center">
            <h3>Quy trình triển khai dịch vụ cho doanh nghiệp</h3>
            <p>Với khách hàng doanh nghiệp, chúng tôi ưu tiên sự ổn định, đúng lịch và đầy đủ đầu mối phối hợp để việc thu gom không ảnh hưởng vận hành.</p>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="100"><i class="bi bi-calendar-check flex-shrink-0"></i><div><h4><a href="contact.html" class="stretched-link">Chốt lịch làm việc</a></h4><p>Thống nhất thời gian, cổng vào, quy định an toàn và phương án bốc xếp.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="200"><i class="bi bi-rulers flex-shrink-0"></i><div><h4><a href="service-details.html" class="stretched-link">Phân loại & cân đo</a></h4><p>Thực hiện theo từng nhóm hàng để đảm bảo đơn giá và khối lượng chính xác.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="300"><i class="bi bi-shield-check flex-shrink-0"></i><div><h4><a href="sample-inner-page.html" class="stretched-link">An toàn hiện trường</a></h4><p>Tuân thủ quy định làm việc, giữ vệ sinh mặt bằng và bàn giao gọn gàng.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="400"><i class="bi bi-receipt flex-shrink-0"></i><div><h4><a href="contact.html" class="stretched-link">Hoàn thiện chứng từ</a></h4><p>Đối soát nhanh, xác nhận khối lượng và thanh toán theo quy trình đã thống nhất.</p></div></div>
          </div>
          <div class="col-lg-6 img-bg" style="background-image: url(assets/img/scrap/feature-support.svg);" data-aos="zoom-in" data-aos-delay="100"></div>
        </div>
      </div>
    </section>

    <section id="alt-services" class="alt-services">
      <div class="container" data-aos="fade-up">
        <div class="row justify-content-around gy-4">
          <div class="col-lg-6 img-bg" style="background-image: url(assets/img/scrap/service-payment.svg);" data-aos="zoom-in" data-aos-delay="100"></div>
          <div class="col-lg-5 d-flex flex-column justify-content-center">
            <h3>Cam kết khi sử dụng dịch vụ thu mua của chúng tôi</h3>
            <p>Mọi giao dịch đều hướng đến lợi ích lâu dài cho khách hàng bằng cách giữ sự minh bạch và đúng hẹn trong từng khâu.</p>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="100"><i class="bi bi-check-circle flex-shrink-0"></i><div><h4><a href="sample-inner-page.html" class="stretched-link">Cân chuẩn - giá rõ</a></h4><p>Không ép giá, không gộp hàng sai chủng loại, không trừ hao thiếu minh bạch.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="200"><i class="bi bi-lightning-charge flex-shrink-0"></i><div><h4><a href="contact.html" class="stretched-link">Xử lý nhanh</a></h4><p>Ưu tiên các đơn hàng cần giải phóng kho gấp hoặc công trình cần bàn giao mặt bằng.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="300"><i class="bi bi-people flex-shrink-0"></i><div><h4><a href="about.html" class="stretched-link">Đội ngũ lành nghề</a></h4><p>Nhân công quen việc bốc xếp, tháo dỡ nhẹ và phân loại hàng tại chỗ.</p></div></div>
            <div class="icon-box d-flex position-relative" data-aos="fade-up" data-aos-delay="400"><i class="bi bi-arrow-repeat flex-shrink-0"></i><div><h4><a href="projects.html" class="stretched-link">Hợp tác lâu dài</a></h4><p>Hỗ trợ lịch thu gom định kỳ để khách hàng không bị ứ đọng phế liệu trong kho.</p></div></div>
          </div>
        </div>
      </div>
    </section>

$(Get-TestimonialsSection)
  </main>
"@ +
(Get-Footer)

Write-Utf8File -Path 'services.html' -Content $servicesContent

$projectsContent = (Get-Head -Title 'Mặt hàng thu mua | Phế Liệu Thành Phát' -Description 'Danh mục phế liệu và mặt hàng thu mua thực tế: kim loại, giấy carton, nhựa công nghiệp, điện tử, máy móc thanh lý.' -Keywords 'mặt hàng thu mua phế liệu, kim loại, nhựa, giấy carton, điện tử cũ') +
(Get-Header -Active 'projects') + @"
  <main id="main">
$(Get-Breadcrumb 'Mặt hàng thu mua')

    <section id="projects" class="projects">
      <div class="container" data-aos="fade-up">
        <div class="portfolio-isotope" data-portfolio-filter="*" data-portfolio-layout="masonry" data-portfolio-sort="original-order">
          <ul class="portfolio-flters" data-aos="fade-up" data-aos-delay="100">
            <li data-filter="*" class="filter-active">Tất cả</li>
            <li data-filter=".filter-metal">Kim loại</li>
            <li data-filter=".filter-paper">Giấy & nhựa</li>
            <li data-filter=".filter-machine">Máy móc</li>
            <li data-filter=".filter-electronic">Điện tử</li>
          </ul>

          <div class="row gy-4 portfolio-container" data-aos="fade-up" data-aos-delay="200">
            <div class="col-lg-4 col-md-6 portfolio-item filter-metal"><div class="portfolio-content h-100"><img src="assets/img/scrap/material-ferrous.svg" class="img-fluid" alt="Sắt thép"><div class="portfolio-info"><h4>Sắt thép công trình</h4><p>Khung thép, dầm, tôn, ống sắt</p><a href="assets/img/scrap/material-ferrous.svg" title="Sắt thép công trình" data-gallery="portfolio-gallery-metal" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-metal"><div class="portfolio-content h-100"><img src="assets/img/scrap/material-nonferrous.svg" class="img-fluid" alt="Đồng"><div class="portfolio-info"><h4>Đồng đỏ, đồng cáp</h4><p>Đồng dây, đồng cháy, đồng công nghiệp</p><a href="assets/img/scrap/material-nonferrous.svg" title="Đồng đỏ, đồng cáp" data-gallery="portfolio-gallery-metal" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-metal"><div class="portfolio-content h-100"><img src="assets/img/scrap/hero-4.svg" class="img-fluid" alt="Nhôm inox"><div class="portfolio-info"><h4>Nhôm, inox, chì</h4><p>Kim loại màu, thanh định hình, phôi</p><a href="assets/img/scrap/hero-4.svg" title="Nhôm, inox, chì" data-gallery="portfolio-gallery-metal" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-paper"><div class="portfolio-content h-100"><img src="assets/img/scrap/material-paper.svg" class="img-fluid" alt="Giấy carton"><div class="portfolio-info"><h4>Giấy carton</h4><p>Thùng carton, giấy vụn, hồ sơ</p><a href="assets/img/scrap/material-paper.svg" title="Giấy carton" data-gallery="portfolio-gallery-paper" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-paper"><div class="portfolio-content h-100"><img src="assets/img/scrap/feature-materials.svg" class="img-fluid" alt="Nhựa công nghiệp"><div class="portfolio-info"><h4>Nhựa công nghiệp</h4><p>PP, PE, PET, nhựa phế phẩm</p><a href="assets/img/scrap/feature-materials.svg" title="Nhựa công nghiệp" data-gallery="portfolio-gallery-paper" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-paper"><div class="portfolio-content h-100"><img src="assets/img/scrap/blog-storage.svg" class="img-fluid" alt="Bao bì tồn kho"><div class="portfolio-info"><h4>Bao bì & hàng tồn kho</h4><p>Nhận thu gom khi cần giải phóng mặt bằng</p><a href="assets/img/scrap/blog-storage.svg" title="Bao bì & hàng tồn kho" data-gallery="portfolio-gallery-paper" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-machine"><div class="portfolio-content h-100"><img src="assets/img/scrap/service-cleanup.svg" class="img-fluid" alt="Thanh lý nhà xưởng"><div class="portfolio-info"><h4>Thanh lý nhà xưởng</h4><p>Khung thép, kệ chứa hàng, thiết bị cũ</p><a href="assets/img/scrap/service-cleanup.svg" title="Thanh lý nhà xưởng" data-gallery="portfolio-gallery-machine" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-machine"><div class="portfolio-content h-100"><img src="assets/img/scrap/service-logistics.svg" class="img-fluid" alt="Xe tải"><div class="portfolio-info"><h4>Thu gom bằng xe tải</h4><p>Đáp ứng nhanh đơn hàng nhỏ lẻ đến lớn</p><a href="assets/img/scrap/service-logistics.svg" title="Thu gom bằng xe tải" data-gallery="portfolio-gallery-machine" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-machine"><div class="portfolio-content h-100"><img src="assets/img/scrap/hero-2.svg" class="img-fluid" alt="Máy móc"><div class="portfolio-info"><h4>Máy móc thanh lý</h4><p>Dây chuyền, mô tơ, máy sản xuất cũ</p><a href="assets/img/scrap/hero-2.svg" title="Máy móc thanh lý" data-gallery="portfolio-gallery-machine" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-electronic"><div class="portfolio-content h-100"><img src="assets/img/scrap/material-electronic.svg" class="img-fluid" alt="Bo mạch"><div class="portfolio-info"><h4>Bo mạch điện tử</h4><p>Bo công nghiệp, linh kiện, dây cáp</p><a href="assets/img/scrap/material-electronic.svg" title="Bo mạch điện tử" data-gallery="portfolio-gallery-electronic" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-electronic"><div class="portfolio-content h-100"><img src="assets/img/scrap/blog-machinery.svg" class="img-fluid" alt="Điện lạnh"><div class="portfolio-info"><h4>Thiết bị điện lạnh cũ</h4><p>Máy lạnh, block, tủ điện, dây đồng</p><a href="assets/img/scrap/blog-machinery.svg" title="Thiết bị điện lạnh cũ" data-gallery="portfolio-gallery-electronic" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
            <div class="col-lg-4 col-md-6 portfolio-item filter-electronic"><div class="portfolio-content h-100"><img src="assets/img/scrap/service-payment.svg" class="img-fluid" alt="Thanh toán nhanh"><div class="portfolio-info"><h4>Thu gom & thanh toán nhanh</h4><p>Quy trình chốt đơn gọn cho hàng điện tử tồn kho</p><a href="assets/img/scrap/service-payment.svg" title="Thu gom & thanh toán nhanh" data-gallery="portfolio-gallery-electronic" class="glightbox preview-link"><i class="bi bi-zoom-in"></i></a><a href="project-details.html" title="Chi tiết" class="details-link"><i class="bi bi-link-45deg"></i></a></div></div></div>
          </div>
        </div>
      </div>
    </section>
  </main>
"@ +
(Get-Footer)

Write-Utf8File -Path 'projects.html' -Content $projectsContent

$projectDetailsContent = (Get-Head -Title 'Chi tiết mặt hàng thu mua | Phế Liệu Thành Phát' -Description 'Thông tin chi tiết về dự án thu mua và thanh lý nhà xưởng, máy móc, kim loại, phế liệu điện tử.' -Keywords 'chi tiết mặt hàng thu mua phế liệu, thanh lý nhà xưởng') +
(Get-Header -Active 'projects') + @"
  <main id="main">
$(Get-Breadcrumb 'Chi tiết mặt hàng')

    <section id="project-details" class="project-details">
      <div class="container" data-aos="fade-up">
        <div class="portfolio-details-slider swiper">
          <div class="swiper-wrapper align-items-center">
            <div class="swiper-slide"><img src="assets/img/scrap/service-cleanup.svg" alt="Thanh lý nhà xưởng"></div>
            <div class="swiper-slide"><img src="assets/img/scrap/material-ferrous.svg" alt="Sắt thép công trình"></div>
            <div class="swiper-slide"><img src="assets/img/scrap/material-electronic.svg" alt="Thiết bị điện tử cũ"></div>
            <div class="swiper-slide"><img src="assets/img/scrap/service-logistics.svg" alt="Xe tải thu gom"></div>
          </div>
          <div class="swiper-pagination"></div>
        </div>

        <div class="row justify-content-between gy-4 mt-4">
          <div class="col-lg-8">
            <div class="portfolio-description">
              <h2>Dự án thanh lý nhà xưởng kết hợp thu mua phế liệu tổng hợp</h2>
              <p>Một doanh nghiệp cơ khí tại Bình Dương cần giải phóng nhà xưởng cũ với nhiều hạng mục gồm sắt thép, máy móc ngừng hoạt động, dây cáp điện, tủ điện, giấy hồ sơ và phế liệu nhựa.</p>
              <p>Phế Liệu Thành Phát triển khai khảo sát trước, chia mặt bằng thành từng khu vực thu gom, ước lượng khối lượng từng nhóm vật tư và bố trí xe tải phù hợp. Toàn bộ quá trình được thực hiện trong 2 ngày, có cân đo điện tử và biên bản đối soát rõ ràng.</p>
              <p>Kết quả là khách hàng giải phóng nhanh diện tích nhà xưởng, không phải thuê nhiều đầu mối khác nhau và tối ưu được giá trị thanh lý nhờ phân loại chuẩn từng loại hàng.</p>
            </div>
          </div>
          <div class="col-lg-3">
            <div class="portfolio-info">
              <h3>Thông tin dự án</h3>
              <ul>
                <li><strong>Khách hàng</strong> <span>Nhà máy cơ khí tại Bình Dương</span></li>
                <li><strong>Địa điểm</strong> <span>KCN Sóng Thần</span></li>
                <li><strong>Thời gian</strong> <span>02 ngày triển khai</span></li>
                <li><strong>Khối lượng</strong> <span>Hơn 38 tấn hàng hóa các loại</span></li>
                <li><strong>Dịch vụ</strong> <span>Khảo sát, bốc xếp, vận chuyển, thanh toán</span></li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </section>
  </main>
"@ +
(Get-Footer)

Write-Utf8File -Path 'project-details.html' -Content $projectDetailsContent

$blogContent = (Get-Head -Title 'Tin tức | Phế Liệu Thành Phát' -Description 'Tin tức thị trường phế liệu, bảng giá thu mua mới, kinh nghiệm phân loại và lưu kho phế liệu an toàn.' -Keywords 'tin tức phế liệu, bảng giá phế liệu, kinh nghiệm bán phế liệu') +
(Get-Header -Active 'blog') + @"
  <main id="main">
$(Get-Breadcrumb 'Tin tức')

    <section id="blog" class="blog">
      <div class="container" data-aos="fade-up" data-aos-delay="100">
        <div class="row gy-4 posts-list">
          <div class="col-xl-4 col-md-6"><article><div class="post-img position-relative overflow-hidden"><img src="assets/img/scrap/blog-price.svg" class="img-fluid" alt="Bảng giá"><span class="post-date">12/03</span></div><div class="post-content d-flex flex-column"><h3 class="post-title">Bảng giá thu mua sắt, đồng, nhôm, inox cập nhật mới nhất</h3><div class="meta d-flex align-items-center"><div class="d-flex align-items-center"><i class="bi bi-person"></i> <span class="ps-2">Ban biên tập</span></div><span class="px-3 text-black-50">/</span><div class="d-flex align-items-center"><i class="bi bi-folder2"></i> <span class="ps-2">Bảng giá</span></div></div><hr><a href="blog-details.html" class="readmore stretched-link"><span>Xem thêm</span><i class="bi bi-arrow-right"></i></a></div></article></div>
          <div class="col-xl-4 col-md-6"><article><div class="post-img position-relative overflow-hidden"><img src="assets/img/scrap/blog-sorting.svg" class="img-fluid" alt="Phân loại"><span class="post-date">10/03</span></div><div class="post-content d-flex flex-column"><h3 class="post-title">Cách phân loại đồng đỏ, đồng vàng, dây cáp để bán được giá cao</h3><div class="meta d-flex align-items-center"><div class="d-flex align-items-center"><i class="bi bi-person"></i> <span class="ps-2">Phòng thu mua</span></div><span class="px-3 text-black-50">/</span><div class="d-flex align-items-center"><i class="bi bi-folder2"></i> <span class="ps-2">Kinh nghiệm</span></div></div><hr><a href="blog-details.html" class="readmore stretched-link"><span>Xem thêm</span><i class="bi bi-arrow-right"></i></a></div></article></div>
          <div class="col-xl-4 col-md-6"><article><div class="post-img position-relative overflow-hidden"><img src="assets/img/scrap/blog-storage.svg" class="img-fluid" alt="Lưu kho"><span class="post-date">08/03</span></div><div class="post-content d-flex flex-column"><h3 class="post-title">Lưu ý an toàn khi lưu kho giấy, nhựa và thiết bị điện tử cũ</h3><div class="meta d-flex align-items-center"><div class="d-flex align-items-center"><i class="bi bi-person"></i> <span class="ps-2">Bộ phận vận hành</span></div><span class="px-3 text-black-50">/</span><div class="d-flex align-items-center"><i class="bi bi-folder2"></i> <span class="ps-2">Kho vận</span></div></div><hr><a href="blog-details.html" class="readmore stretched-link"><span>Xem thêm</span><i class="bi bi-arrow-right"></i></a></div></article></div>
          <div class="col-xl-4 col-md-6"><article><div class="post-img position-relative overflow-hidden"><img src="assets/img/scrap/blog-market.svg" class="img-fluid" alt="Thị trường"><span class="post-date">05/03</span></div><div class="post-content d-flex flex-column"><h3 class="post-title">Xu hướng thị trường phế liệu quý I và nhu cầu thu gom của doanh nghiệp</h3><div class="meta d-flex align-items-center"><div class="d-flex align-items-center"><i class="bi bi-person"></i> <span class="ps-2">Ban phân tích</span></div><span class="px-3 text-black-50">/</span><div class="d-flex align-items-center"><i class="bi bi-folder2"></i> <span class="ps-2">Thị trường</span></div></div><hr><a href="blog-details.html" class="readmore stretched-link"><span>Xem thêm</span><i class="bi bi-arrow-right"></i></a></div></article></div>
          <div class="col-xl-4 col-md-6"><article><div class="post-img position-relative overflow-hidden"><img src="assets/img/scrap/blog-policy.svg" class="img-fluid" alt="Chính sách"><span class="post-date">03/03</span></div><div class="post-content d-flex flex-column"><h3 class="post-title">Những điều cần hỏi trước khi chọn đơn vị thu mua phế liệu số lượng lớn</h3><div class="meta d-flex align-items-center"><div class="d-flex align-items-center"><i class="bi bi-person"></i> <span class="ps-2">CSKH</span></div><span class="px-3 text-black-50">/</span><div class="d-flex align-items-center"><i class="bi bi-folder2"></i> <span class="ps-2">Tư vấn</span></div></div><hr><a href="blog-details.html" class="readmore stretched-link"><span>Xem thêm</span><i class="bi bi-arrow-right"></i></a></div></article></div>
          <div class="col-xl-4 col-md-6"><article><div class="post-img position-relative overflow-hidden"><img src="assets/img/scrap/blog-machinery.svg" class="img-fluid" alt="Máy móc"><span class="post-date">01/03</span></div><div class="post-content d-flex flex-column"><h3 class="post-title">Khi nào nên thanh lý máy móc cũ và cách định giá hợp lý</h3><div class="meta d-flex align-items-center"><div class="d-flex align-items-center"><i class="bi bi-person"></i> <span class="ps-2">Bộ phận kỹ thuật</span></div><span class="px-3 text-black-50">/</span><div class="d-flex align-items-center"><i class="bi bi-folder2"></i> <span class="ps-2">Thanh lý</span></div></div><hr><a href="blog-details.html" class="readmore stretched-link"><span>Xem thêm</span><i class="bi bi-arrow-right"></i></a></div></article></div>
        </div>
      </div>
    </section>
  </main>
"@ +
(Get-Footer)

Write-Utf8File -Path 'blog.html' -Content $blogContent

$blogDetailsContent = (Get-Head -Title 'Chi tiết bài viết | Phế Liệu Thành Phát' -Description 'Bài viết chi tiết về cách phân loại phế liệu, bảng giá thu mua và kinh nghiệm thanh lý hàng hóa cũ.' -Keywords 'chi tiết bài viết phế liệu, phân loại đồng, bảng giá phế liệu') +
(Get-Header -Active 'blog') + @"
  <main id="main">
$(Get-Breadcrumb 'Chi tiết bài viết')

    <section id="blog" class="blog">
      <div class="container" data-aos="fade-up" data-aos-delay="100">
        <div class="row g-5">
          <div class="col-lg-8">
            <article class="blog-details">
              <div class="post-img"><img src="assets/img/scrap/blog-sorting.svg" alt="Phân loại đồng" class="img-fluid"></div>
              <h2 class="title">Cách phân loại đồng đỏ, đồng vàng, dây cáp và đồng cháy để bán được giá tốt</h2>
              <div class="meta-top">
                <ul>
                  <li class="d-flex align-items-center"><i class="bi bi-person"></i> <a href="blog-details.html">Phòng thu mua</a></li>
                  <li class="d-flex align-items-center"><i class="bi bi-clock"></i> <a href="blog-details.html"><time datetime="2026-03-10">10/03/2026</time></a></li>
                  <li class="d-flex align-items-center"><i class="bi bi-folder2"></i> <a href="blog-details.html">Kinh nghiệm bán phế liệu</a></li>
                </ul>
              </div>
              <div class="content">
                <p>Đồng là nhóm phế liệu có giá trị cao, tuy nhiên mỗi loại đồng lại có mức giá khác nhau. Nếu khách hàng phân loại sơ bộ trước khi bán, giá thu mua thường tốt hơn đáng kể so với việc để lẫn nhiều loại trong cùng một đống hàng.</p>
                <p><strong>Đồng đỏ</strong> thường có màu đỏ ánh kim, ít tạp chất, giá cao nhất trong nhóm đồng thông dụng. Loại này thường xuất hiện ở dây điện lõi đồng, thanh đồng, ống đồng lạnh hoặc các chi tiết cơ khí.</p>
                <blockquote><p>Nguyên tắc đơn giản: càng sạch, càng tách riêng rõ chủng loại thì giá thu mua càng cao.</p></blockquote>
                <p><strong>Đồng vàng</strong> là hợp kim có màu vàng sẫm, thường gặp ở van nước, linh kiện, phụ kiện cơ khí. Giá thấp hơn đồng đỏ nhưng vẫn cao nếu hàng khô ráo, ít lẫn sắt và không dính nhiều tạp chất.</p>
                <p><strong>Dây cáp đồng</strong> nên tách riêng theo nhóm có vỏ và đã lột vỏ. Với đơn vị thu mua chuyên nghiệp, hình ảnh hoặc video trước khi khảo sát sẽ giúp báo giá sơ bộ chính xác hơn.</p>
                <img src="assets/img/scrap/blog-price.svg" class="img-fluid" alt="Bảng giá đồng phế liệu">
                <h3>3 lưu ý quan trọng khi bán đồng phế liệu</h3>
                <ul>
                  <li>Tách riêng đồng đỏ, đồng vàng, dây cáp, đồng cháy và phần lẫn sắt.</li>
                  <li>Để hàng khô ráo, hạn chế dính dầu, nước hoặc vật liệu phi kim.</li>
                  <li>Chọn đơn vị có cân điện tử và báo giá rõ theo từng nhóm hàng.</li>
                </ul>
                <p>Tại Phế Liệu Thành Phát, đội khảo sát sẽ hỗ trợ phân loại thực tế tại kho hoặc công trình, sau đó báo giá minh bạch theo từng chủng loại để khách hàng tối ưu giá trị thanh lý.</p>
              </div>
            </article>
          </div>

          <div class="col-lg-4">
            <div class="sidebar">
              <div class="sidebar-item recent-posts">
                <h3 class="sidebar-title">Bài viết gần đây</h3>
                <div class="mt-3">
                  <div class="post-item mt-3"><img src="assets/img/scrap/blog-price.svg" alt="Bảng giá"><div><h4><a href="blog-details.html">Bảng giá thu mua phế liệu mới nhất</a></h4><time datetime="2026-03-12">12/03/2026</time></div></div>
                  <div class="post-item"><img src="assets/img/scrap/blog-storage.svg" alt="Lưu kho"><div><h4><a href="blog-details.html">Lưu kho giấy và nhựa an toàn</a></h4><time datetime="2026-03-08">08/03/2026</time></div></div>
                  <div class="post-item"><img src="assets/img/scrap/blog-policy.svg" alt="Chính sách"><div><h4><a href="blog-details.html">Tiêu chí chọn đơn vị thu mua uy tín</a></h4><time datetime="2026-03-03">03/03/2026</time></div></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  </main>
"@ +
(Get-Footer)

Write-Utf8File -Path 'blog-details.html' -Content $blogDetailsContent

$contactContent = (Get-Head -Title 'Liên hệ | Phế Liệu Thành Phát' -Description 'Liên hệ đơn vị thu mua phế liệu Thành Phát để khảo sát, báo giá và thu gom tận nơi tại TP.HCM và các tỉnh lân cận.' -Keywords 'liên hệ thu mua phế liệu, hotline phế liệu thành phát') +
(Get-Header -Active 'contact') + @"
  <main id="main">
$(Get-Breadcrumb 'Liên hệ')

    <section id="contact" class="contact">
      <div class="container" data-aos="fade-up" data-aos-delay="100">
        <div class="row gy-4">
          <div class="col-lg-6"><div class="info-item d-flex flex-column justify-content-center align-items-center"><i class="bi bi-map"></i><h3>Địa chỉ làm việc</h3><p>88 Quốc lộ 1A, P. An Phú Đông, Quận 12, TP. Hồ Chí Minh</p></div></div>
          <div class="col-lg-3 col-md-6"><div class="info-item d-flex flex-column justify-content-center align-items-center"><i class="bi bi-envelope"></i><h3>Email</h3><p>info@phelieuthanhphat.vn</p></div></div>
          <div class="col-lg-3 col-md-6"><div class="info-item d-flex flex-column justify-content-center align-items-center"><i class="bi bi-telephone"></i><h3>Hotline</h3><p>0909 686 889</p></div></div>
        </div>

        <div class="row gy-4 mt-1">
          <div class="col-lg-6">
            <img src="assets/img/scrap/contact.svg" alt="Khu vực phục vụ" class="img-fluid rounded-4">
            <div class="info-item mt-4 d-flex flex-column justify-content-center align-items-start p-4">
              <h3>Khu vực phục vụ</h3>
              <p class="mb-2">Chúng tôi nhận khảo sát và thu mua tận nơi tại:</p>
              <ul class="mb-0">
                <li>TP. Hồ Chí Minh</li>
                <li>Bình Dương, Đồng Nai</li>
                <li>Long An, Tây Ninh</li>
                <li>Các tỉnh lân cận theo lịch hẹn</li>
              </ul>
            </div>
          </div>

          <div class="col-lg-6">
            <form action="forms/contact.php" method="post" role="form" class="php-email-form">
              <div class="row gy-4">
                <div class="col-lg-6 form-group"><input type="text" name="name" class="form-control" id="name" placeholder="Họ và tên" required></div>
                <div class="col-lg-6 form-group"><input type="email" class="form-control" name="email" id="email" placeholder="Email" required></div>
              </div>
              <div class="form-group"><input type="text" class="form-control" name="subject" id="subject" placeholder="Chủ đề liên hệ" required></div>
              <div class="form-group"><textarea class="form-control" name="message" rows="5" placeholder="Loại phế liệu, khối lượng, địa chỉ và thời gian cần khảo sát" required></textarea></div>
              <div class="my-3"><div class="loading">Đang gửi</div><div class="error-message"></div><div class="sent-message">Tin nhắn của bạn đã được gửi. Chúng tôi sẽ phản hồi sớm.</div></div>
              <div class="text-center"><button type="submit">Gửi liên hệ</button></div>
            </form>
          </div>
        </div>
      </div>
    </section>
  </main>
"@ +
(Get-Footer)

Write-Utf8File -Path 'contact.html' -Content $contactContent

$serviceDetailsContent = (Get-Head -Title 'Chi tiết dịch vụ | Phế Liệu Thành Phát' -Description 'Chi tiết dịch vụ thu mua phế liệu và thanh lý nhà xưởng, máy móc cũ, dọn kho, bốc xếp tận nơi.' -Keywords 'chi tiết dịch vụ thu mua phế liệu, thanh lý máy móc') +
(Get-Header -Active 'services') + @"
  <main id="main">
$(Get-Breadcrumb 'Chi tiết dịch vụ')

    <section id="service-details" class="service-details">
      <div class="container" data-aos="fade-up">
        <div class="row gy-4">
          <div class="col-lg-4">
            <div class="services-list">
              <a href="#" class="active">Thu mua kim loại phế liệu</a>
              <a href="#">Thu mua giấy carton, nhựa</a>
              <a href="#">Thanh lý máy móc, nhà xưởng</a>
              <a href="#">Dọn kho và bốc xếp tận nơi</a>
              <a href="#">Thu gom định kỳ cho doanh nghiệp</a>
            </div>
            <h4>Dịch vụ phù hợp cho nhiều nhu cầu</h4>
            <p>Khách hàng có thể bán lẻ theo từng đợt hoặc thiết lập lịch thu gom định kỳ nếu thường xuyên phát sinh phế liệu trong quá trình sản xuất.</p>
          </div>

          <div class="col-lg-8">
            <img src="assets/img/scrap/service-cleanup.svg" alt="Chi tiết dịch vụ thu mua" class="img-fluid services-img">
            <h3>Thu mua và thanh lý nhà xưởng, máy móc, vật tư cũ trọn gói</h3>
            <p>Dịch vụ này phù hợp khi khách hàng cần xử lý đồng thời nhiều nhóm hàng như khung nhà xưởng, sắt thép, máy móc ngừng hoạt động, dây cáp điện, giấy hồ sơ và hàng tồn kho.</p>
            <ul>
              <li><i class="bi bi-check-circle"></i> <span>Khảo sát tận nơi để lên phương án bốc xếp, cắt dỡ và vận chuyển phù hợp.</span></li>
              <li><i class="bi bi-check-circle"></i> <span>Phân loại từng nhóm vật tư để đưa ra đơn giá minh bạch và tối ưu nhất.</span></li>
              <li><i class="bi bi-check-circle"></i> <span>Bố trí xe tải, nhân công, xe nâng và thanh toán ngay sau khi cân xong.</span></li>
            </ul>
            <p>Đối với nhà máy và doanh nghiệp, chúng tôi có thể phối hợp theo quy trình nội bộ, yêu cầu an toàn và hồ sơ đối soát của từng đơn vị để bảo đảm hoạt động thu gom diễn ra suôn sẻ.</p>
          </div>
        </div>
      </div>
    </section>
  </main>
"@ +
(Get-Footer)

Write-Utf8File -Path 'service-details.html' -Content $serviceDetailsContent

$sampleInnerPageContent = (Get-Head -Title 'Chính sách thu mua | Phế Liệu Thành Phát' -Description 'Chính sách thu mua, quy trình làm việc và cam kết dịch vụ của Phế Liệu Thành Phát.' -Keywords 'chính sách thu mua phế liệu, cam kết dịch vụ') +
(Get-Header -Active 'services') + @"
  <main id="main">
$(Get-Breadcrumb 'Chính sách thu mua')

    <section class="sample-inner-page">
      <div class="container" data-aos="fade-up">
        <div class="row justify-content-center">
          <div class="col-lg-10">
            <div class="section-header">
              <h2>Cam kết và chính sách làm việc</h2>
              <p>Những nguyên tắc chúng tôi áp dụng để mọi giao dịch thu mua phế liệu diễn ra rõ ràng, nhanh chóng và đáng tin cậy.</p>
            </div>
            <div class="row gy-4">
              <div class="col-md-6"><h4>1. Báo giá minh bạch</h4><p>Giá được áp dụng theo từng nhóm hàng, loại vật tư và thời điểm thị trường. Khách hàng có quyền đối chiếu và xác nhận trước khi thu gom.</p></div>
              <div class="col-md-6"><h4>2. Cân đo chuẩn xác</h4><p>Khối lượng được ghi nhận công khai. Với đơn hàng lớn, chúng tôi hỗ trợ lập biên bản cân và xác nhận khối lượng sau khi hoàn tất.</p></div>
              <div class="col-md-6"><h4>3. Thanh toán nhanh</h4><p>Thanh toán ngay sau khi giao dịch xong bằng tiền mặt hoặc chuyển khoản. Doanh nghiệp có thể đối soát theo lịch và quy trình riêng.</p></div>
              <div class="col-md-6"><h4>4. Thu gom an toàn</h4><p>Đội ngũ bốc xếp và vận chuyển tuân thủ quy định an toàn, giữ vệ sinh khu vực và bàn giao mặt bằng gọn gàng.</p></div>
            </div>
            <hr class="my-5">
            <h4>Quy trình làm việc 4 bước</h4>
            <ul>
              <li>Tiếp nhận yêu cầu qua hotline hoặc form website.</li>
              <li>Khảo sát thực tế, phân loại và báo giá.</li>
              <li>Bốc xếp, cân đo, thu gom tại chỗ.</li>
              <li>Thanh toán, xác nhận khối lượng và bàn giao mặt bằng.</li>
            </ul>
          </div>
        </div>
      </div>
    </section>
  </main>
"@ +
(Get-Footer)

Write-Utf8File -Path 'sample-inner-page.html' -Content $sampleInnerPageContent

Write-Utf8File -Path 'forms/Readme.txt' -Content @"
This form backend belongs to the template.
Frontend content has been adapted for the scrap buying website theme.
"@

Write-Output 'Scrap website conversion files generated successfully.'
