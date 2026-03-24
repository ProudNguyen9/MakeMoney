$pages = @(
  @{ File = 'UpConstruction-1.0.0/index.html'; PageCss = 'index.css' },
  @{ File = 'UpConstruction-1.0.0/blog.html'; PageCss = 'blog.css' },
  @{ File = 'UpConstruction-1.0.0/contact.html'; PageCss = 'contact.css' },
  @{ File = 'UpConstruction-1.0.0/detail.html'; PageCss = 'detail.css' }
)

$fontBlock = @"
  <link rel="preconnect" href="https://fonts.googleapis.com">
  <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
  <link
    href="https://fonts.googleapis.com/css2?family=Open+Sans:ital,wght@0,300;0,400;0,500;0,600;0,700;1,300;1,400;1,600;1,700&family=Roboto:ital,wght@0,300;0,400;0,500;0,700;1,300;1,400;1,500;1,700&family=Work+Sans:ital,wght@0,300;0,400;0,500;0,600;0,700;1,300;1,400;1,500;1,600;1,700&display=swap"
    rel="stylesheet">
"@

foreach ($page in $pages) {
  $content = Get-Content -Raw -Path $page.File

  $content = [regex]::Replace(
    $content,
    '(?s)<link rel="preconnect" href="https://fonts.googleapis.com">\s*<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>\s*<link.*?rel="stylesheet">',
    [System.Text.RegularExpressions.MatchEvaluator]{
      param($match)
      return $fontBlock.TrimEnd()
    },
    1
  )

  $cssBlock = @"
  <link href="assets/css/main.css" rel="stylesheet">
  <link href="assets/css/site.css" rel="stylesheet">
  <link href="assets/css/$($page.PageCss)" rel="stylesheet">
"@

  $content = [regex]::Replace(
    $content,
    '(?s)<link href="assets/css/main\.css" rel="stylesheet">\s*<style>.*?</style>',
    [System.Text.RegularExpressions.MatchEvaluator]{
      param($match)
      return $cssBlock.TrimEnd()
    },
    1
  )

  Set-Content -Path $page.File -Value $content -Encoding UTF8
  Write-Output "UPDATED $($page.File)"
}
