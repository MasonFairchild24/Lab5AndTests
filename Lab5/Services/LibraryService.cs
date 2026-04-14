using Lab5.Components.Pages;
using Lab5.Models;

namespace Lab5.Services
{
    public interface ILibraryService
    {
        List<Book> GetBooks();
        void AddBook(Book book);
        void EditBook(Book book);
        void DeleteBook(int id);

        List<User> GetUsers();
        void AddUser(User user);
        void EditUser(User user);
        void DeleteUser(int id);

        Dictionary<int, List<Book>> GetBorrowedBooks();
        void BorrowBook(int userId, int bookId);
        void ReturnBook(int userId, int bookIndex);
    }

    public class LibraryService : ILibraryService
    {
        private List<Book> _books = new();
        private List<User> _users = new();
        private Dictionary<int, List<Book>> _borrowedBooks = new();

        private readonly string _booksPath;
        private readonly string _usersPath;

        public LibraryService(IWebHostEnvironment env)
        {
            _booksPath = Path.Combine(env.ContentRootPath, "Data", "Books.csv");
            _usersPath = Path.Combine(env.ContentRootPath, "Data", "Users.csv");
            ReadBooks();
            ReadUsers();
        }

        private void ReadBooks()
        {
            if (!File.Exists(_booksPath)) return;
            foreach (var line in File.ReadLines(_booksPath))
            {
                var fields = line.Split(',');
                if (fields.Length >= 4 && int.TryParse(fields[0].Trim(), out int id))
                {
                    _books.Add(new Book
                    {
                        Id = id,
                        Title = fields[1].Trim(),
                        Author = fields[2].Trim(),
                        ISBN = fields[3].Trim()
                    });
                }
            }
        }

        private void ReadUsers()
        {
            if (!File.Exists(_usersPath)) return;
            foreach (var line in File.ReadLines(_usersPath))
            {
                var fields = line.Split(',');
                if (fields.Length >= 3 && int.TryParse(fields[0].Trim(), out int id))
                {
                    _users.Add(new User
                    {
                        Id = id,
                        Name = fields[1].Trim(),
                        Email = fields[2].Trim()
                    });
                }
            }
        }

        private void SaveBooks()
        {
            var lines = _books.Select(b => $"{b.Id},{b.Title},{b.Author},{b.ISBN}");
            File.WriteAllLines(_booksPath, lines);
        }

        private void SaveUsers()
        {
            var lines = _users.Select(u => $"{u.Id},{u.Name},{u.Email}");
            File.WriteAllLines(_usersPath, lines);
        }

        public List<Book> GetBooks() => _books;

        public void AddBook(Book book)
        {
            book.Id = _books.Any() ? _books.Max(b => b.Id) + 1 : 1;
            _books.Add(book);
            SaveBooks();
        }

        public void EditBook(Book book)
        {
            var existing = _books.FirstOrDefault(b => b.Id == book.Id);
            if (existing != null)
            {
                existing.Title = book.Title;
                existing.Author = book.Author;
                existing.ISBN = book.ISBN;
                SaveBooks();
            }
        }

        public void DeleteBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                _books.Remove(book);
                SaveBooks();
            }
        }

        public List<User> GetUsers() => _users;

        public void AddUser(User user)
        {
            user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
            _users.Add(user);
            SaveUsers();
        }

        public void EditUser(User user)
        {
            var existing = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existing != null)
            {
                existing.Name = user.Name;
                existing.Email = user.Email;
                SaveUsers();
            }
        }

        public void DeleteUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _users.Remove(user);
                SaveUsers();
            }
        }

        public Dictionary<int, List<Book>> GetBorrowedBooks() => _borrowedBooks;

        public void BorrowBook(int userId, int bookId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            var book = _books.FirstOrDefault(b => b.Id == bookId);
            if (user == null || book == null) return;

            if (!_borrowedBooks.ContainsKey(userId))
                _borrowedBooks[userId] = new List<Book>();

            _borrowedBooks[userId].Add(book);
            _books.Remove(book);
            SaveBooks();
        }

        public void ReturnBook(int userId, int bookIndex)
        {
            if (!_borrowedBooks.ContainsKey(userId)) return;

            var borrowed = _borrowedBooks[userId];
            if (bookIndex < 0 || bookIndex >= borrowed.Count) return;

            var book = borrowed[bookIndex];
            borrowed.RemoveAt(bookIndex);
            _books.Add(book);
            SaveBooks();

            if (borrowed.Count == 0)
                _borrowedBooks.Remove(userId);
        }



        public LibraryService(string booksPath, string usersPath)
        {
            _booksPath = booksPath;
            _usersPath = usersPath;
        } //This is just for testing purposes
    }
}