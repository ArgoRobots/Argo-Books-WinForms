namespace Argo_Books.ReportGenerator
{
    /// <summary>
    /// Helper class for dynamically positioning elements in report templates.
    /// </summary>
    public static class TemplateLayoutHelper
    {
        public class LayoutContext
        {
            public Size PageSize { get; set; }
            public int Margin { get; set; } = 40;
            public int HeaderHeight { get; set; } = 80;
            public int FooterHeight { get; set; } = 50;
            public int ElementSpacing { get; set; } = 10;
            public int DateRangeHeight { get; set; } = 30;
            public int ContentTop => HeaderHeight + Margin;
            public int ContentWidth => PageSize.Width - (Margin * 2);
            public int ContentHeight => PageSize.Height - HeaderHeight - FooterHeight - (Margin * 2);
        }

        /// <summary>
        /// Creates a grid layout with specified rows and columns.
        /// </summary>
        public static Rectangle[,] CreateGrid(LayoutContext context, int rows, int columns, bool includeDateRange = true)
        {
            int startY = context.ContentTop;
            int availableHeight = context.ContentHeight;

            if (includeDateRange)
            {
                startY += context.DateRangeHeight + context.ElementSpacing;
                availableHeight -= (context.DateRangeHeight + context.ElementSpacing);
            }

            int cellWidth = (context.ContentWidth - (context.ElementSpacing * (columns - 1))) / columns;
            int cellHeight = (availableHeight - (context.ElementSpacing * (rows - 1))) / rows;

            Rectangle[,] grid = new Rectangle[rows, columns];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    int x = context.Margin + (col * (cellWidth + context.ElementSpacing));
                    int y = startY + (row * (cellHeight + context.ElementSpacing));
                    grid[row, col] = new Rectangle(x, y, cellWidth, cellHeight);
                }
            }

            return grid;
        }

        /// <summary>
        /// Creates a vertical stack layout with proportional heights.
        /// </summary>
        public static Rectangle[] CreateVerticalStack(LayoutContext context, params float[] heightRatios)
        {
            int startY = context.ContentTop + context.DateRangeHeight + context.ElementSpacing;
            int availableHeight = context.ContentHeight - context.DateRangeHeight - context.ElementSpacing;

            // Remove spacing from available height
            availableHeight -= context.ElementSpacing * (heightRatios.Length - 1);

            Rectangle[] stack = new Rectangle[heightRatios.Length];
            int currentY = startY;

            for (int i = 0; i < heightRatios.Length; i++)
            {
                int height = (int)(availableHeight * heightRatios[i]);
                stack[i] = new Rectangle(context.Margin, currentY, context.ContentWidth, height);
                currentY += height + context.ElementSpacing;
            }

            return stack;
        }

        /// <summary>
        /// Gets the date range element bounds.
        /// </summary>
        public static Rectangle GetDateRangeBounds(LayoutContext context)
        {
            return new Rectangle(context.Margin, context.ContentTop, context.ContentWidth, context.DateRangeHeight);
        }
    }
}