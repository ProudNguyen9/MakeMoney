using System.ComponentModel.DataAnnotations;

namespace WebThuMuaPheLieu.Models;

public class AdminPostsPageViewModel
{
    public string HeaderTitle { get; set; } = string.Empty;

    public string HeaderDescription { get; set; } = string.Empty;

    public List<string> HeaderChips { get; set; } = [];

    public List<AdminPostMetricCardViewModel> MetricCards { get; set; } = [];

    public List<AdminPostWorkflowItemViewModel> WorkflowItems { get; set; } = [];

    public List<AdminPostQuickActionViewModel> QuickActions { get; set; } = [];

    public List<string> EditorialChecklist { get; set; } = [];

    public string ImplementationNote { get; set; } = string.Empty;

    public List<AdminPostCategoryCardViewModel> Categories { get; set; } = [];

    public List<AdminPostFeatureItemViewModel> FeatureItems { get; set; } = [];

    public List<string> FilterTabs { get; set; } = [];

    public List<AdminFilterOptionViewModel> StatusFilters { get; set; } = [];

    public List<AdminFilterOptionViewModel> CategoryFilters { get; set; } = [];

    public string SearchTerm { get; set; } = string.Empty;

    public string SelectedStatus { get; set; } = string.Empty;

    public int? SelectedCategoryId { get; set; }

    public List<AdminPostRowViewModel> Posts { get; set; } = [];

    public List<AdminPostResourceCardViewModel> ResourceCards { get; set; } = [];

    public string ConflictNote { get; set; } = string.Empty;

    public string SuccessMessage { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public string ActiveSection { get; set; } = string.Empty;

    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; } = 1;

    public int PageSize { get; set; } = 8;

    public int TotalFilteredPosts { get; set; }

    public int StartItemIndex { get; set; }

    public int EndItemIndex { get; set; }

    public AdminPostEditorViewModel Editor { get; set; } = new();

    public AdminCategoryEditorViewModel CategoryEditor { get; set; } = new();
}

public class AdminFilterOptionViewModel
{
    public string Value { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public bool Selected { get; set; }
}

public class AdminPostMetricCardViewModel
{
    public string Label { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public string Meta { get; set; } = string.Empty;

    public string Tone { get; set; } = string.Empty;
}

public class AdminPostWorkflowItemViewModel
{
    public string Label { get; set; } = string.Empty;

    public int Count { get; set; }

    public string Hint { get; set; } = string.Empty;

    public string Tone { get; set; } = string.Empty;
}

public class AdminPostQuickActionViewModel
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Cta { get; set; } = string.Empty;

    public string ActionUrl { get; set; } = string.Empty;
}

public class AdminPostCategoryCardViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public int TotalPosts { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Tone { get; set; } = string.Empty;

    public string FilterUrl { get; set; } = string.Empty;
}

public class AdminPostFeatureItemViewModel
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

public class AdminPostRowViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public int? PrimaryCategoryId { get; set; }

    public List<int> CategoryIds { get; set; } = [];

    public string Category { get; set; } = string.Empty;

    public string CategorySlug { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string StatusLabel { get; set; } = string.Empty;

    public string StatusClass { get; set; } = string.Empty;

    public DateTime? PublishedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string PublishText { get; set; } = string.Empty;

    public string PublishHint { get; set; } = string.Empty;

    public int RelatedProducts { get; set; }

    public string RelatedProductSummary { get; set; } = string.Empty;

    public int GalleryImages { get; set; }

    public string GallerySummary { get; set; } = string.Empty;

    public int Likes { get; set; }

    public bool HasCover { get; set; }

    public string CoverText { get; set; } = string.Empty;

    public string CoverClass { get; set; } = string.Empty;

    public int SeoScore { get; set; }

    public string SeoClass { get; set; } = string.Empty;

    public string Excerpt { get; set; } = string.Empty;

    public string PrimaryAction { get; set; } = string.Empty;

    public string PrimaryActionValue { get; set; } = string.Empty;

    public string SecondaryAction { get; set; } = string.Empty;

    public string SecondaryActionValue { get; set; } = string.Empty;

    public string CreatedText { get; set; } = string.Empty;

    public string UpdatedText { get; set; } = string.Empty;

    public string PublicUrl { get; set; } = string.Empty;
}

public class AdminPostResourceCardViewModel
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;
}

public class AdminPostEditorViewModel
{
    public bool IsEditing { get; set; }

    public string FormTitle { get; set; } = string.Empty;

    public string SubmitLabel { get; set; } = string.Empty;

    public string CancelUrl { get; set; } = string.Empty;

    public AdminPostEditorInputModel Input { get; set; } = new();

    public List<AdminSelectableItemViewModel> AuthorOptions { get; set; } = [];

    public List<AdminSelectableItemViewModel> CategoryOptions { get; set; } = [];

    public List<AdminSelectableItemViewModel> ProductOptions { get; set; } = [];

    public List<AdminPostGalleryItemViewModel> GalleryItems { get; set; } = [];
}

public class AdminCategoryEditorViewModel
{
    public string SubmitLabel { get; set; } = string.Empty;

    public AdminCategoryEditorInputModel Input { get; set; } = new();
}

public class AdminSelectableItemViewModel
{
    public int Id { get; set; }

    public string Label { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string MetaValue { get; set; } = string.Empty;

    public bool Selected { get; set; }
}

public class AdminPostGalleryItemViewModel
{
    public string ImageUrl { get; set; } = string.Empty;

    public string Caption { get; set; } = string.Empty;

    public int OrderIndex { get; set; }
}

public class AdminPostEditorInputModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Tiêu đề bài viết là bắt buộc.")]
    [StringLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Slug không được vượt quá 255 ký tự.")]
    public string? Slug { get; set; }

    public string? Excerpt { get; set; }

    [Required(ErrorMessage = "Nội dung bài viết là bắt buộc.")]
    public string Content { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Đường dẫn ảnh cover không được vượt quá 500 ký tự.")]
    public string? CoverImage { get; set; }

    public int? AuthorId { get; set; }

    [Required(ErrorMessage = "Trạng thái bài viết là bắt buộc.")]
    public string Status { get; set; } = "draft";

    public DateTime? PublishedAt { get; set; }

    public int? PrimaryCategoryId { get; set; }

    public List<int> SelectedCategoryIds { get; set; } = [];

    public List<int> SelectedProductIds { get; set; } = [];

    public string? GalleryInput { get; set; }

    public string? SearchTerm { get; set; }

    public string? ReturnStatus { get; set; }

    public int? ReturnCategoryId { get; set; }

    public int? ReturnPage { get; set; }
}

public class AdminCategoryEditorInputModel
{
    [Required(ErrorMessage = "Tên chuyên mục là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Tên chuyên mục không được vượt quá 100 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Slug chuyên mục không được vượt quá 255 ký tự.")]
    public string? Slug { get; set; }

    public string? Description { get; set; }

    public string? SearchTerm { get; set; }

    public string? ReturnStatus { get; set; }

    public int? ReturnCategoryId { get; set; }

    public int? ReturnPage { get; set; }
}
