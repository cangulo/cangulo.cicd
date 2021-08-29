namespace cangulo.changelog.markdown
{
    public static class MarkdownBuilder
    {
        public static string Comment(string content)
            => $"<!-- {content} -->";

        public static string Title(string title, TitleLevel titleLevel)
            => $"{new string('#', (int)titleLevel)} {title}";

        public static string ListItem(string item, ListItemLevel listItemLevel)
            => $"{new string('\t', (int)listItemLevel)}* {item}";
    }
}