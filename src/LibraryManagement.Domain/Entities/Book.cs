using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Entities
{
    public class Book : BaseEntity
    {
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Isbn { get; private set; }
        public int PublicationYear { get; private set; }
        public int AvailableCopies { get; private set; }
        public int TotalCopies { get; private set; }

        // Construtor privado para EF
        private Book() { }

        public Book(string title, string author, string isbn, int publicationYear, int totalCopies)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Author cannot be empty", nameof(author));
            
            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentException("ISBN cannot be empty", nameof(isbn));
            
            if (publicationYear < 1000 || publicationYear > DateTime.Now.Year)
                throw new ArgumentException("Invalid publication year", nameof(publicationYear));
            
            if (totalCopies <= 0)
                throw new ArgumentException("Total copies must be greater than 0", nameof(totalCopies));

            Title = title;
            Author = author;
            Isbn = isbn;
            PublicationYear = publicationYear;
            TotalCopies = totalCopies;
            AvailableCopies = totalCopies;
        }

        public void UpdateDetails(string title, string author, int publicationYear)
        {
            if (!string.IsNullOrWhiteSpace(title))
                Title = title;
            
            if (!string.IsNullOrWhiteSpace(author))
                Author = author;
            
            if (publicationYear >= 1000 && publicationYear <= DateTime.Now.Year)
                PublicationYear = publicationYear;
        }

        public void BorrowCopy()
        {
            if (AvailableCopies <= 0)
                throw new InvalidOperationException("No available copies to borrow");
            
            AvailableCopies--;
        }

        public void ReturnCopy()
        {
            if (AvailableCopies >= TotalCopies)
                throw new InvalidOperationException("Cannot return more copies than total");
            
            AvailableCopies++;
        }

        public void UpdateStock(int totalCopies)
        {
            if (totalCopies < 0)
                throw new ArgumentException("Total copies cannot be negative");
            
            var difference = totalCopies - TotalCopies;
            TotalCopies = totalCopies;
            AvailableCopies += difference;
            
            if (AvailableCopies < 0)
                AvailableCopies = 0;
        }
    }
}