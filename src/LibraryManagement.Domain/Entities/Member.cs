using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Entities
{
    public class Member : BaseEntity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public DateTime RegistrationDate { get; private set; }
        public bool IsActive { get; private set; }

        // Construtor privado para EF
        private Member() { }

        public Member(string firstName, string lastName, string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty", nameof(lastName));
            
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone ?? string.Empty;
            RegistrationDate = DateTime.UtcNow;
            IsActive = true;
        }

        public void UpdateDetails(string firstName, string lastName, string email, string phone)
        {
            if (!string.IsNullOrWhiteSpace(firstName))
                FirstName = firstName;
            
            if (!string.IsNullOrWhiteSpace(lastName))
                LastName = lastName;
            
            if (!string.IsNullOrWhiteSpace(email))
                Email = email;

            Phone = phone ?? string.Empty;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;

        public string GetFullName() => $"{FirstName} {LastName}";
    }
}