namespace ShopQaMVC.Helpers
{
    // Helper class to convert relative image URLs to absolute URLs
    public static class UrlHelperExtensions
    {
        public static string ToAbsoluteImageUrl(this HttpRequest request, string relativePath)
        {
            return $"{request.Scheme}://{request.Host}/{relativePath.TrimStart('/')}";
        }
    }
}
